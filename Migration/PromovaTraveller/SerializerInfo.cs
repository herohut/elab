using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PromovaTraveller
{
    [Serializable]
    public class SerializerInfo
    {
        public SerializerInfo()
        {
            _counterForObject = int.MinValue;
        }

        [NonSerialized]
        internal Dictionary<object, int> Object2Id = new Dictionary<object, int>();

        [NonSerialized]
        internal Dictionary<int, object> Id2Object = new Dictionary<int, object>();

        private readonly Dictionary<Type, byte> _type2Id = new Dictionary<Type, byte>();
        internal Dictionary<byte, Type> Id2Type = new Dictionary<byte, Type>();

        private int _counterForObject;
        private byte _counterForType;
        private readonly Dictionary<Type, FieldInfo[]> _type2FieldInfo = new Dictionary<Type, FieldInfo[]>();
        private readonly Dictionary<Type, MethodInfo[]> _type2MethodInfo = new Dictionary<Type, MethodInfo[]>();
        private readonly Dictionary<Type, PropertyInfo[]> _type2PropertyInfo = new Dictionary<Type, PropertyInfo[]>();
        internal Dictionary<Type, Type[]> Dictionary2Types = new Dictionary<Type, Type[]>();
        internal Dictionary<Type, MethodInfo> DictionaryBase2AddMethod = new Dictionary<Type, MethodInfo>();
        internal Dictionary<Type, Type> List2Type = new Dictionary<Type, Type>();
        internal Dictionary<Type, Type> Array2Type = new Dictionary<Type, Type>();
        internal Dictionary<Type, MethodInfo> CollectionBase2AddMethod = new Dictionary<Type, MethodInfo>();
        private readonly Dictionary<Type, ConstructorInfo> _type2Constructor = new Dictionary<Type, ConstructorInfo>();
        internal Dictionary<Type, EnumHandler> EnumType2EnumHandler = new Dictionary<Type, EnumHandler>();

        public bool CanCreateInstanceByActivator(Type type)
        {
            if (!_type2Constructor.ContainsKey(type))
            {
                ConstructorInfo con = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(c => c.GetParameters().Length == 0);
                _type2Constructor[type] = con;
            }

            return _type2Constructor[type] != null;
        }

        public ConstructorInfo GetConstructor(Type type)
        {
            return _type2Constructor[type];
        }

        public void InitId2Object()
        {
            Id2Object = new Dictionary<int, object>();
        }
        public object GetObjectById(int id)
        {
            if (Id2Object.ContainsKey(id))
                return Id2Object[id];
            return null;
        }

        public void AddId2Object(int id, object obj)
        {
            Id2Object[id] = obj;
        }

        public byte GetTypeId(Type type)
        {
            if (!_type2Id.ContainsKey(type))
            {
                _counterForType++;
                _type2Id[type] = _counterForType;
                Id2Type[_counterForType] = type;
            }

            return _type2Id[type];
        }

        public Type GetTypeById(byte id)
        {
            if (!Id2Type.ContainsKey(id))
                throw new Exception("SerializeInfo incorrect");
            return Id2Type[id];
        }

        public int GetObjectId(object obj, out bool hasExisted)
        {
            hasExisted = true;
            if (!Object2Id.ContainsKey(obj))
            {
                _counterForObject++;
                Object2Id[obj] = _counterForObject;
                hasExisted = false;
            }
            return Object2Id[obj];
        }

        public FieldInfo[] GetSerializableFieldInfo(object obj)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();
            return GetSerializableFieldInfo(type);
        }

        public bool AdvoidDuplicateNLevel;

        public FieldInfo[] GetSerializableFieldInfo(Type type)
        {
            if (!_type2FieldInfo.ContainsKey(type))
            {
                _type2FieldInfo[type] = GetSerializedFieldInNLevel(type);
            }
            return _type2FieldInfo[type];
        }

        private readonly List<Type> _excludedTypesInNLevel = new List<Type>
                                                                 {
                                                            typeof(object),
                                                            typeof(DictionaryBase),
                                                            typeof(CollectionBase),
                                                            typeof(ArrayList)
                                                        };
        private FieldInfo[] GetSerializedFieldInNLevel(Type type)
        {
            var result = new List<FieldInfo>();
            while (!_excludedTypesInNLevel.Contains(type) && !type.IsValueType && !type.IsPrimitive)
            {
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (FieldInfo fieldInfo in fields)
                {
                    if (AdvoidDuplicateNLevel && result.Exists(f => f.Name == fieldInfo.Name))
                        continue;

                    if (result.Exists(f => (f.Name == fieldInfo.Name) && f.IsFamily && f.IsPublic))
                        continue;
                    if (!fieldInfo.IsPrivate && result.Exists(f => (f.Name == fieldInfo.Name) && f.IsAssembly))
                        continue;
                    
                    result.Add(fieldInfo);
                }

                type = type.BaseType;
            }

            return result.ToArray();
        }

        public List<Type> PrimativeValueTypes = new List<Type>
                                                    {
            typeof(bool),
            typeof(byte),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(sbyte),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(char),
            //typeof(string),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime)            
        };

        public List<Type> TreatedCollectionTypes = new List<Type>
                                                       {
                                                           typeof(IListSource),
                                                           typeof(IEnumerable)
                                                       };

        public static bool IsBasedOn(Type source, Type baseType)
        {
            do
            {
                if (source == baseType)
                    return true;
                source = source.BaseType;
            } while (source != null);

            return false;
        }

        public MethodInfo[] GetMethodInfo(object obj)
        {
            Type type = obj.GetType();
            return GetMethodInfo(type);
        }

        public MethodInfo[] GetMethodInfo(Type type)
        {
            if (!_type2MethodInfo.ContainsKey(type))
                _type2MethodInfo[type] = type.GetMethods();
            return _type2MethodInfo[type];
        }

        public PropertyInfo[] GetPropertyInfo(object obj)
        {
            Type type = obj.GetType();
            if (!_type2PropertyInfo.ContainsKey(type))
                _type2PropertyInfo[type] = type.GetProperties();
            return _type2PropertyInfo[type];
        }

        public void GetDicTypes(Type dic, out Type key, out Type value)
        {
            if (!Dictionary2Types.ContainsKey(dic))
            {
                Type[] types = dic.GetGenericArguments();
                Dictionary2Types[dic] = types;
            }

            key = Dictionary2Types[dic][0];
            value = Dictionary2Types[dic][1];
        }

        public void GetDicBaseTypes(Type dic, out Type key, out Type value)
        {
            if (!Dictionary2Types.ContainsKey(dic))
            {
                var addMethods = GetDicBaseAddMethod(dic);

                var parameters = addMethods.GetParameters();

                Dictionary2Types[dic] = new[] { parameters[0].ParameterType, parameters[1].ParameterType };
            }

            key = Dictionary2Types[dic][0];
            value = Dictionary2Types[dic][1];
        }

        public MethodInfo GetDicBaseAddMethod(Type dic)
        {
            if (!DictionaryBase2AddMethod.ContainsKey(dic))
            {
                var addMethods = dic.GetMethods().FirstOrDefault(m => m.Name == "Add" && m.GetParameters().Length == 2);
                if (addMethods == null)
                    throw new Exception("Invalid supported DictionaryBase type, must have an Add(Key,Value)");

                DictionaryBase2AddMethod[dic] = addMethods;
            }

            return DictionaryBase2AddMethod[dic];
        }

        public MethodInfo GetCollectBasAddMethod(Type collect)
        {
            if (!CollectionBase2AddMethod.ContainsKey(collect))
            {
                var addMethod =
                    collect.GetMethods().FirstOrDefault(m => m.Name == "Add" && m.GetParameters().Length == 1);
                CollectionBase2AddMethod[collect] = addMethod;

            }
            return CollectionBase2AddMethod[collect];
        }

        internal Type GetListBaseType(Type type)
        {
            if (!List2Type.ContainsKey(type))
            {
                List2Type[type] = type.GetGenericArguments()[0];
            }

            return List2Type[type];
        }

        internal Type GetArrayBaseType(Type type)
        {
            if (!Array2Type.ContainsKey(type))
            {
                Array2Type[type] = type.GetElementType();
            }
            return Array2Type[type];
        }

        internal void GetCollectBaseType(Type type, out Type valType)
        {
            if (!List2Type.ContainsKey(type))
            {
                var addMethod = GetCollectBasAddMethod(type);
                List2Type[type] = addMethod.GetParameters()[0].ParameterType;

            }

            valType = List2Type[type];
        }


        internal byte GetEnumValId(Type type, object val)
        {
            if (!EnumType2EnumHandler.ContainsKey(type))
                EnumType2EnumHandler[type] = new EnumHandler(type);
            return EnumType2EnumHandler[type].GetEnumValId(val);
        }

        internal object GetEnmValFromId(Type type, byte id)
        {
            if (!EnumType2EnumHandler.ContainsKey(type))
                throw new Exception("The type do not exist in serialize info");
            return EnumType2EnumHandler[type].GetEnumVal(id);
        }

        private Comparer _comparer;
        internal void AddComparer(object input)
        {
            if (_comparer == null)
                _comparer = input as Comparer;
        }
        public Comparer GetComparer()
        {
            return _comparer;
        }

        [Serializable]
        public class EnumHandler
        {
            private readonly Type _enumType;
            private readonly Dictionary<object, byte> _mapVal2ValId = new Dictionary<object, byte>();
            private readonly Dictionary<byte, object> _mapValId2Val = new Dictionary<byte, object>();

            public EnumHandler(Type type)
            {
                _enumType = type;
                string[] names = Enum.GetNames(_enumType);
                for (byte i = 0; i < names.Length; i++)
                {
                    object val = Enum.Parse(type, names[i]);
                    _mapValId2Val[i] = val;
                    _mapVal2ValId[val] = i;

                }
            }

            public byte GetEnumValId(object obj)
            {
                return _mapVal2ValId[obj];
            }
            public object GetEnumVal(byte id)
            {
                return _mapValId2Val[id];
            }
        }

        internal Dictionary<Type, MethodInfo> DerivedArrayList2AddMethod = new Dictionary<Type, MethodInfo>();
        internal MethodInfo GetAddMethodOfDerivedArrayList(Type type)
        {
            if (!DerivedArrayList2AddMethod.ContainsKey(type))
            {
                DerivedArrayList2AddMethod[type] = type.GetMethods().FirstOrDefault(
                    m => m.Name == "Add" && m.GetParameters()[0].ParameterType == typeof(object));

            }

            return DerivedArrayList2AddMethod[type];
        }


        public void DumpInfo()
        {
            // Hung: Must remove later
            var sWriter = new StreamWriter("dump.txt");
            sWriter.WriteLine();
            sWriter.WriteLine();
            sWriter.WriteLine("---------------------");
            sWriter.WriteLine("[Number of object in graph]");
            sWriter.WriteLine(Object2Id.Count);

            sWriter.WriteLine();
            sWriter.WriteLine();
            sWriter.WriteLine("------------------------------------------------------------------------------------");
            sWriter.WriteLine("[Object statistic]");
            sWriter.WriteLine("[Count]\t[Type(FullName)]");
            var group = Object2Id.Keys.GroupBy(o => o.GetType()).OrderBy(g => g.Count());
            foreach (var g in group)
            {
                sWriter.WriteLine("{0}\t{1}", g.Count(), g.Key.FullName);
            }

            sWriter.WriteLine();
            sWriter.WriteLine();
            sWriter.WriteLine("------------------------------------------------------------------------------------");
            sWriter.WriteLine("[Cannot Create Instance Types]");
            foreach (var b in CannotCreateInstancetypes)
            {
                sWriter.WriteLine(b.FullName);
            }
            sWriter.WriteLine();
            sWriter.WriteLine();
            sWriter.WriteLine("------------------------------------------------------------------------------------");
            sWriter.WriteLine("[Treated types using reflection]");
            foreach (var b in UsingReflectionToCreateInstanceTypes)
            {
                sWriter.WriteLine(b.FullName);
            }

            sWriter.WriteLine();
            sWriter.WriteLine();
            sWriter.WriteLine("------------------------------------------------------------------------------------");
            sWriter.WriteLine("[All types in system]");
            sWriter.WriteLine("[Index]\t[Type(FullName)]");
            foreach (var keyval in Id2Type)
            {
                sWriter.WriteLine("{0}\t{1}", keyval.Key, keyval.Value.FullName);
            }

            sWriter.Close();
        }

        internal List<Type> CannotCreateInstancetypes = new List<Type>();
        internal List<Type> UsingReflectionToCreateInstanceTypes = new List<Type>();

    }
}

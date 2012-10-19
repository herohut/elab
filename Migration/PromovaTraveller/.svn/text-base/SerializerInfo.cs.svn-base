using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PromovaTraveller
{
    [Serializable]
    public class SerializerInfo
    {
        public SerializerInfo()
        {
            counter_for_object = int.MinValue;
        }

        [NonSerialized]
        internal Dictionary<object, int> _Object2Id = new Dictionary<object, int>();

        [NonSerialized]
        internal Dictionary<int, object> _Id2Object = new Dictionary<int, object>();

        private Dictionary<Type, byte> _Type2Id = new Dictionary<Type, byte>();
        internal Dictionary<byte, Type> _Id2Type = new Dictionary<byte, Type>();

        private int counter_for_object;
        private byte counter_for_type;
        private Dictionary<Type, FieldInfo[]> _Type2FieldInfo = new Dictionary<Type, FieldInfo[]>();
        private Dictionary<Type, MethodInfo[]> _Type2MethodInfo = new Dictionary<Type, MethodInfo[]>();
        private Dictionary<Type, PropertyInfo[]> _Type2PropertyInfo = new Dictionary<Type, PropertyInfo[]>();
        internal Dictionary<Type, Type[]> _Dictionary2Types = new Dictionary<Type, Type[]>();
        internal Dictionary<Type, MethodInfo> _DictionaryBase2AddMethod = new Dictionary<Type, MethodInfo>();
        internal Dictionary<Type, Type> _List2Type = new Dictionary<Type, Type>();
        internal Dictionary<Type, Type> _Array2Type = new Dictionary<Type, Type>();
        internal Dictionary<Type, MethodInfo> _CollectionBase2AddMethod = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, ConstructorInfo> _Type2Constructor = new Dictionary<Type, ConstructorInfo>();
        internal Dictionary<Type, EnumHandler> _EnumType2EnumHandler = new Dictionary<Type, EnumHandler>();

        public bool CanCreateInstanceByActivator(Type type)
        {
            if (!_Type2Constructor.ContainsKey(type))
            {
                ConstructorInfo con = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(c => c.GetParameters().Length == 0);
                _Type2Constructor[type] = con;
            }

            return _Type2Constructor[type] != null;
        }

        public ConstructorInfo GetConstructor(Type type)
        {
            return _Type2Constructor[type];
        }

        public void InitId2Object()
        {
            _Id2Object = new Dictionary<int, object>();
        }
        public object GetObjectById(int id)
        {
            if (_Id2Object.ContainsKey(id))
                return _Id2Object[id];
            return null;
        }

        public void AddId2Object(int id, object obj)
        {
            _Id2Object[id] = obj;
        }

        public byte GetTypeId(Type type)
        {
            if (!_Type2Id.ContainsKey(type))
            {
                counter_for_type++;
                _Type2Id[type] = counter_for_type;
                _Id2Type[counter_for_type] = type;
            }

            return _Type2Id[type];
        }

        public Type GetTypeById(byte id)
        {
            if (!_Id2Type.ContainsKey(id))
                throw new Exception("SerializeInfo incorrect");
            return _Id2Type[id];
        }

        public int GetObjectId(object obj, out bool hasExisted)
        {
            hasExisted = true;
            if (!_Object2Id.ContainsKey(obj))
            {
                counter_for_object++;
                _Object2Id[obj] = counter_for_object;
                hasExisted = false;
            }
            return _Object2Id[obj];
        }

        public FieldInfo[] GetSerializableFieldInfo(object obj)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();
            return GetSerializableFieldInfo(type);
        }

        public FieldInfo[] GetSerializableFieldInfo(Type type)
        {
            if (!_Type2FieldInfo.ContainsKey(type))
            {
                _Type2FieldInfo[type] = GetSerializedFieldInNLevel(type);
            }
            return _Type2FieldInfo[type];
        }

        private List<Type> _ExcludedTypesInNLevel = new List<Type>()
                                                        {
                                                            typeof(object),
                                                            typeof(DictionaryBase),
                                                            typeof(CollectionBase),
                                                            typeof(ArrayList)
                                                        };
        private FieldInfo[] GetSerializedFieldInNLevel(Type type)
        {
            List<FieldInfo> result = new List<FieldInfo>();
            while (!_ExcludedTypesInNLevel.Contains(type) && !type.IsValueType && !type.IsPrimitive)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo fieldInfo = fields[i];

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

        public List<Type> PrimativeValueTypes = new List<Type>()
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
            if (!_Type2MethodInfo.ContainsKey(type))
                _Type2MethodInfo[type] = type.GetMethods();
            return _Type2MethodInfo[type];
        }

        public PropertyInfo[] GetPropertyInfo(object obj)
        {
            Type type = obj.GetType();
            if (!_Type2PropertyInfo.ContainsKey(type))
                _Type2PropertyInfo[type] = type.GetProperties();
            return _Type2PropertyInfo[type];
        }

        public void GetDicTypes(Type dic, out Type key, out Type value)
        {
            if (!_Dictionary2Types.ContainsKey(dic))
            {
                Type[] types = dic.GetGenericArguments();
                _Dictionary2Types[dic] = types;
            }

            key = _Dictionary2Types[dic][0];
            value = _Dictionary2Types[dic][1];
        }

        public void GetDicBaseTypes(Type dic, out Type key, out Type value)
        {
            if (!_Dictionary2Types.ContainsKey(dic))
            {
                var addMethods = GetDicBaseAddMethod(dic);

                var parameters = addMethods.GetParameters();

                _Dictionary2Types[dic] = new Type[] { parameters[0].ParameterType, parameters[1].ParameterType };
            }

            key = _Dictionary2Types[dic][0];
            value = _Dictionary2Types[dic][1];
        }

        public MethodInfo GetDicBaseAddMethod(Type dic)
        {
            if (!_DictionaryBase2AddMethod.ContainsKey(dic))
            {
                var addMethods = dic.GetMethods().FirstOrDefault(m => m.Name == "Add" && m.GetParameters().Length == 2);
                if (addMethods == null)
                    throw new Exception("Invalid supported DictionaryBase type, must have an Add(Key,Value)");

                _DictionaryBase2AddMethod[dic] = addMethods;
            }

            return _DictionaryBase2AddMethod[dic];
        }

        public MethodInfo GetCollectBasAddMethod(Type collect)
        {
            if (!_CollectionBase2AddMethod.ContainsKey(collect))
            {
                var addMethod =
                    collect.GetMethods().FirstOrDefault(m => m.Name == "Add" && m.GetParameters().Length == 1);
                _CollectionBase2AddMethod[collect] = addMethod;

            }
            return _CollectionBase2AddMethod[collect];
        }

        internal Type GetListBaseType(Type type)
        {
            if (!_List2Type.ContainsKey(type))
            {
                _List2Type[type] = type.GetGenericArguments()[0];
            }

            return _List2Type[type];
        }

        internal Type GetArrayBaseType(Type type)
        {
            if (!_Array2Type.ContainsKey(type))
            {
                _Array2Type[type] = type.GetElementType();
            }
            return _Array2Type[type];
        }

        internal void GetCollectBaseType(Type type, out Type valType)
        {
            if (!_List2Type.ContainsKey(type))
            {
                var addMethod = GetCollectBasAddMethod(type);
                _List2Type[type] = addMethod.GetParameters()[0].ParameterType;

            }

            valType = _List2Type[type];
        }


        internal byte GetEnumValId(Type type, object val)
        {
            if (!_EnumType2EnumHandler.ContainsKey(type))
                _EnumType2EnumHandler[type] = new EnumHandler(type);
            return _EnumType2EnumHandler[type].GetEnumValId(val);
        }

        internal object GetEnmValFromId(Type type, byte id)
        {
            if (!_EnumType2EnumHandler.ContainsKey(type))
                throw new Exception("The type do not exist in serialize info");
            return _EnumType2EnumHandler[type].GetEnumVal(id);
        }

        private Comparer comparer = null;
        internal void AddComparer(object input)
        {
            if (comparer == null)
                comparer = input as Comparer;
        }
        public Comparer GetComparer()
        {
            return comparer;
        }

        [Serializable]
        public class EnumHandler
        {
            private Type _EnumType;
            private Dictionary<object, byte> _MapVal2ValId = new Dictionary<object, byte>();
            private Dictionary<byte, object> _MapValId2Val = new Dictionary<byte, object>();

            public EnumHandler(Type type)
            {
                _EnumType = type;
                string[] names = Enum.GetNames(_EnumType);
                for (byte i = 0; i < names.Length; i++)
                {
                    object val = Enum.Parse(type, names[i]);
                    _MapValId2Val[i] = val;
                    _MapVal2ValId[val] = i;

                }
            }

            public byte GetEnumValId(object obj)
            {
                return _MapVal2ValId[obj];
            }
            public object GetEnumVal(byte id)
            {
                return _MapValId2Val[id];
            }
        }

        internal Dictionary<Type, MethodInfo> _DerivedArrayList2AddMethod = new Dictionary<Type, MethodInfo>();
        internal MethodInfo GetAddMethodOfDerivedArrayList(Type type)
        {
            if (!_DerivedArrayList2AddMethod.ContainsKey(type))
            {
                _DerivedArrayList2AddMethod[type] = type.GetMethods().FirstOrDefault(
                    m => m.Name == "Add" && m.GetParameters()[0].ParameterType == typeof(object));

            }

            return _DerivedArrayList2AddMethod[type];
        }


        public void DumpInfo()
        {
            // Hung: Must remove later
            StreamWriter sWriter = new StreamWriter("dump.txt");
            sWriter.WriteLine();
            sWriter.WriteLine();
            sWriter.WriteLine("---------------------");
            sWriter.WriteLine("[Number of object in graph]");
            sWriter.WriteLine(this._Object2Id.Count);

            sWriter.WriteLine();
            sWriter.WriteLine();
            sWriter.WriteLine("------------------------------------------------------------------------------------");
            sWriter.WriteLine("[Object statistic]");
            sWriter.WriteLine("[Count]\t[Type(FullName)]");
            var group = this._Object2Id.Keys.GroupBy(o => o.GetType()).OrderBy(g => g.Count());
            foreach (var g in group)
            {
                sWriter.WriteLine("{0}\t{1}", g.Count(), g.Key.FullName);
            }

            sWriter.WriteLine();
            sWriter.WriteLine();
            sWriter.WriteLine("------------------------------------------------------------------------------------");
            sWriter.WriteLine("[Cannot Create Instance Types]");
            foreach (var b in cannotCreateInstancetypes)
            {
                sWriter.WriteLine(b.FullName);
            }
            sWriter.WriteLine();
            sWriter.WriteLine();
            sWriter.WriteLine("------------------------------------------------------------------------------------");
            sWriter.WriteLine("[Treated types using reflection]");
            foreach (var b in usingReflectionToCreateInstanceTypes)
            {
                sWriter.WriteLine(b.FullName);
            }

            sWriter.WriteLine();
            sWriter.WriteLine();
            sWriter.WriteLine("------------------------------------------------------------------------------------");
            sWriter.WriteLine("[All types in system]");
            sWriter.WriteLine("[Index]\t[Type(FullName)]");
            foreach (var keyval in this._Id2Type)
            {
                sWriter.WriteLine("{0}\t{1}", keyval.Key, keyval.Value.FullName);
            }

            sWriter.Close();
        }

        internal List<Type> cannotCreateInstancetypes = new List<Type>();
        internal List<Type> usingReflectionToCreateInstanceTypes = new List<Type>();

    }
}

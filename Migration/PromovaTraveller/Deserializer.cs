using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Reflection;

namespace PromovaTraveller
{
    public class Deserializer
    {
        BinaryReader _reader;
        readonly BinaryFormatter _formatter = new BinaryFormatter();
        SerializerInfo _serializeInfo;

        public object Deserialize(Stream dataStream)
        {
            _reader = new BinaryReader(dataStream);
            _reader.BaseStream.Position = _reader.BaseStream.Length - 4;
            int infoLen = _reader.ReadInt32();
            _reader.BaseStream.Position = _reader.BaseStream.Length - 4 - infoLen;
            byte[] bytes = _reader.ReadBytes(infoLen);
            var ms = new MemoryStream(bytes);
            _serializeInfo = (SerializerInfo)_formatter.Deserialize(ms);
            _serializeInfo.InitId2Object();
            _reader.BaseStream.Position = 0;

            return ReadObject();
        }

        public object Deserialize(Stream dataStream, Stream infoStream)
        {
            _reader = new BinaryReader(dataStream);
            _serializeInfo = (SerializerInfo)_formatter.Deserialize(infoStream);
            _serializeInfo.InitId2Object();
            return ReadObject();
        }

        private object ReadObject()
        {
            if (ReadNullNotNull())
                return null;
            Type type = ReadObjectType();

            if (_serializeInfo.PrimativeValueTypes.Contains(type))
            {
                return ReadPrimativeObject(type);
            }
            if (type == typeof(Comparer))
                return _serializeInfo.GetComparer();

            if (typeof(Enum).Equals(type.BaseType))
            {
                return ReadEnum(type);
            }

            if (type == typeof(TimeSpan))
            {
                return new TimeSpan(_reader.ReadInt64());
            }


            // REFERENCE TYPES
            int objectId = _reader.ReadInt32();

            object output = _serializeInfo.GetObjectById(objectId);
            if (output != null)
                return output;
            if (type == typeof(string))
            {
                output = _reader.ReadString();
            }

                // TODO: If input is Array
            else if (type.IsArray)
            {
                output = ReadArray(type);
            }

                // TODO: If input is IDictionary<>/Dictionary<>
            else if (type.Name == "IDictionary`2" || type.Name == "Dictionary`2")
            {
                output = ReadDictionary(type);
            }
                // TODO: If input is List/IList
            else if (type.Name == "List`1" || type.Name == "IList`1")
            {
                output = ReadList(type);
            }

                // TODO: SortedList
            else if (type == typeof(SortedList))
            {
                output = ReadSortedList();
            }

                // TODO: if input is System.Drawing.Point
            else if (type == typeof(Point))
            {
                output = ReadDrawingPoint();
            }
                // TODO: Hashtable
            else if (type == typeof(Hashtable))
            {
                output = ReadHashtable();
            }

                // TODO: If input is ArrayList
            else if (SerializerInfo.IsBasedOn(type, typeof(ArrayList)))
            {
                output = ReadArrayList(type);

            }
                // TODO: If input is DictionaryBase
            else if (SerializerInfo.IsBasedOn(type, typeof(DictionaryBase)))
            {
                output = ReadDicionaryBase(type);
            }

                // TODO: CollectionBase
            else if (SerializerInfo.IsBasedOn(type, typeof(CollectionBase)))
            {
                output = ReadCollectionBase(type);
            }

                // TODO: Type that cannot create Instance by Activator
            else if (!_serializeInfo.CanCreateInstanceByActivator(type))
            {
                output = GeneralObjectDeserialize();
            }
            else
            {
                output = _serializeInfo.GetConstructor(type).Invoke(null);
                _serializeInfo.AddId2Object(objectId, output);

                // Read Fields
                ReadCustomFields(output, type);
            }

            _serializeInfo.AddId2Object(objectId, output);
            return output;

        }

        private object ReadSortedList()
        {
            int count = _reader.ReadInt32();
            var output = new SortedList();
            for (int i = 0; i < count; i++)
                output.Add(ReadObject(), ReadObject());
            return output;
        }

        private object ReadEnum(Type type)
        {
            byte valId = _reader.ReadByte();
            return _serializeInfo.GetEnmValFromId(type, valId);
        }

        private object ReadArrayList(Type type)
        {
            int count = _reader.ReadInt32();

            if (type == typeof(ArrayList))
            {
                var internalOutput = new ArrayList();
                for (int i = 0; i < count; i++)
                {
                    internalOutput.Add(ReadObject());
                }

                return internalOutput;
            }

            var output = Activator.CreateInstance(type);
            var addMethod = _serializeInfo.GetAddMethodOfDerivedArrayList(type);

            for (var i = 0; i < count; i++)
            {
                addMethod.Invoke(output, new[] { ReadObject() });
                //type.InvokeMember("Add", BindingFlags.InvokeMethod, null, output, new object[] { ReadObject() });
            }

            // Custom fields
            ReadCustomFields(output, type);

            return output;
        }

        private object ReadDrawingPoint()
        {
            return new Point(_reader.ReadInt16(), _reader.ReadInt16());
        }

        private object ReadArray(Type type)
        {
            int count = _reader.ReadInt32();
            Type valType = _serializeInfo.GetArrayBaseType(type);
            var al = new ArrayList(count);

            for (int i = 0; i < count; i++)
            {
                object val = ReadObject();
                al.Insert(i, val);
            }

            object output = al.ToArray(valType);
            return output;
        }

        private object ReadDicionaryBase(Type type)
        {
            object output = Activator.CreateInstance(type);

            MethodInfo method = _serializeInfo.GetDicBaseAddMethod(type);

            int count = _reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var key = ReadObject();
                var val = ReadObject();
                method.Invoke(output, new[] { key, val });
            }

            // Custom fields
            ReadCustomFields(output, type);
            return output;
        }

        private void ReadCustomFields(object output, Type type)
        {
            FieldInfo[] fs = _serializeInfo.GetSerializableFieldInfo(type);
            foreach (var item in fs)
            {
                object fieldVal = ReadObject();
                item.SetValue(output, fieldVal);
            }
        }

        public object ReadDictionary(Type type)
        {
            var count = _reader.ReadInt32();
            Type typeKey;
            Type typeVal;
            _serializeInfo.GetDicTypes(type, out typeKey, out typeVal);

            var generic = typeof(Dictionary<,>);
            Type[] typeArgs = { typeKey, typeVal };
            var constructed = generic.MakeGenericType(typeArgs);
            object output = Activator.CreateInstance(constructed);

            for (int i = 0; i < count; i++)
            {
                var key = ReadObject();
                var val = ReadObject();

                constructed.InvokeMember("Add", BindingFlags.InvokeMethod, null, output, new[] { key, val });
            }

            return output;

        }

        private object ReadList(Type type)
        {
            var count = _reader.ReadInt32();
            var listType = _serializeInfo.GetListBaseType(type);
            var generic = typeof(List<>);
            var constructed = generic.MakeGenericType(listType);
            object output = Activator.CreateInstance(constructed);
            for (int i = 0; i < count; i++)
            {
                object val = ReadObject();
                constructed.InvokeMember("Add", BindingFlags.InvokeMethod, null, output, new[] { val });
            }

            return output;
        }

        private object ReadCollectionBase(Type type)
        {
            var output = Activator.CreateInstance(type);
            Type valType;
            _serializeInfo.GetCollectBaseType(type, out valType);
            var method = _serializeInfo.GetCollectBasAddMethod(type);
            int count = _reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var val = ReadObject();
                method.Invoke(output, new[] { val });
            }

            // Custom fields
            ReadCustomFields(output, type);

            return output;
        }

        private object ReadHashtable()
        {
            var hashTable = new Hashtable();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var key = ReadObject();
                var val = ReadObject();
                hashTable.Add(key, val);
            }
            return hashTable;
        }

        private object GeneralObjectDeserialize()
        {
            int len = _reader.ReadInt32();
            byte[] bytes = _reader.ReadBytes(len);
            var ms = new MemoryStream(bytes);
            object output = _formatter.Deserialize(ms);
            ms.Close();

            return output;
        }

        private object ReadPrimativeObject(Type type)
        {
            if (type == typeof(bool))
                return _reader.ReadBoolean();
            if (type == typeof(byte))
                return _reader.ReadByte();
            if (type == typeof(ushort))
                return _reader.ReadUInt16();
            if (type == typeof(uint))
                return _reader.ReadUInt32();
            if (type == typeof(ulong))
                return _reader.ReadUInt64();
            if (type == typeof(sbyte))
                return _reader.ReadSByte();
            if (type == typeof(short))
                return _reader.ReadInt16();
            if (type == typeof(int))
                return _reader.ReadInt32();
            if (type == typeof(long))
                return _reader.ReadInt64();
            if (type == typeof(char))
                return _reader.ReadChar();
            if (type == typeof(string))
                return _reader.ReadString();
            if (type == typeof(float))
                return _reader.ReadSingle();
            if (type == typeof(double))
                return _reader.ReadDouble();
            if (type == typeof(decimal))
                return _reader.ReadDecimal();
            if (type == typeof(DateTime))
                return new DateTime(_reader.ReadInt64());
            throw new Exception("Not primative type");
        }
        private bool ReadNullNotNull()
        {
            return _reader.ReadBoolean();
        }

        private Type ReadObjectType()
        {
            byte id = _reader.ReadByte();
            return _serializeInfo.GetTypeById(id);
        }
    }
}

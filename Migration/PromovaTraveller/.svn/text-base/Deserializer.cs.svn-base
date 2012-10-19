using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Reflection;

namespace PromovaTraveller
{
    public class Deserializer
    {
        BinaryReader reader;
        BinaryFormatter formatter = new BinaryFormatter();
        SerializerInfo serializeInfo;

        public object Deserialize(Stream dataStream)
        {
            reader = new BinaryReader(dataStream);
            reader.BaseStream.Position = reader.BaseStream.Length - 4;
            int infoLen = reader.ReadInt32();
            reader.BaseStream.Position = reader.BaseStream.Length - 4 - infoLen;
            byte[] bytes = reader.ReadBytes(infoLen);
            MemoryStream ms = new MemoryStream(bytes);
            serializeInfo = (SerializerInfo)formatter.Deserialize(ms);
            serializeInfo.InitId2Object();
            reader.BaseStream.Position = 0;

            return ReadObject();
        }

        public object Deserialize(Stream dataStream, Stream infoStream)
        {
            reader = new BinaryReader(dataStream);
            serializeInfo = (SerializerInfo)formatter.Deserialize(infoStream);
            serializeInfo.InitId2Object();
            return ReadObject();
        }

        private object ReadObject()
        {
            if (ReadNullNotNull())
                return null;
            Type type = ReadObjectType();

            if (serializeInfo.PrimativeValueTypes.Contains(type))
            {
                return ReadPrimativeObject(type);
            }
            if (type == typeof(Comparer))
                return serializeInfo.GetComparer();

            else if (typeof(Enum).Equals(type.BaseType))
            {
                return ReadEnum(type);
            }

            else if (type == typeof(TimeSpan))
            {
                return new TimeSpan(reader.ReadInt64());
            }


            // REFERENCE TYPES
            int objectId = reader.ReadInt32();

            object output = serializeInfo.GetObjectById(objectId);
            if (output != null)
                return output;
            else if (type == typeof(string))
            {
                output = reader.ReadString();
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
                output = ReadSortedList(type);
            }

                  // TODO: if input is System.Drawing.Point
            else if (type == typeof(Point))
            {
                output = ReadDrawingPoint();
            }
            // TODO: Hashtable
            else if (type == typeof(Hashtable))
            {
                output = ReadHashtable(type);
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
            else if (!serializeInfo.CanCreateInstanceByActivator(type))
            {
                output = GeneralObjectDeserialize();
            }
            else
            {
                try
                {
                    output = serializeInfo.GetConstructor(type).Invoke(null);
                    serializeInfo.AddId2Object(objectId, output);

                    // Read Fields
                    ReadCustomFields(output, type);
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }

            serializeInfo.AddId2Object(objectId, output);
            return output;

        }

        private object ReadSortedList(Type type)
        {
            int count = reader.ReadInt32();
            SortedList output = new SortedList();
            for (int i = 0; i < count; i++)
                output.Add(ReadObject(), ReadObject());
            return output;
        }

        private object ReadEnum(Type type)
        {
            byte valId = reader.ReadByte();
            return serializeInfo.GetEnmValFromId(type, valId);
        }

        private object ReadArrayList(Type type)
        {
            int count = reader.ReadInt32();

            if (type == typeof(ArrayList))
            {
                ArrayList internalOutput = new ArrayList();
                for (int i = 0; i < count; i++)
                {
                    internalOutput.Add(ReadObject());
                }

                return internalOutput;
            }

            object output = Activator.CreateInstance(type);
            MethodInfo addMethod = serializeInfo.GetAddMethodOfDerivedArrayList(type);

            for (int i = 0; i < count; i++)
            {
                addMethod.Invoke(output, new object[] { ReadObject() });
                //type.InvokeMember("Add", BindingFlags.InvokeMethod, null, output, new object[] { ReadObject() });
            }

            // Custom fields
            ReadCustomFields(output, type);

            return output;
        }

        private object ReadDrawingPoint()
        {
            return new Point(reader.ReadInt16(), reader.ReadInt16());
        }

        private object ReadArray(Type type)
        {
            int count = reader.ReadInt32();
            Type valType = serializeInfo.GetArrayBaseType(type);
            ArrayList al = new ArrayList(count);

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

            MethodInfo method = serializeInfo.GetDicBaseAddMethod(type);

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var key = ReadObject();
                var val = ReadObject();
                method.Invoke(output, new object[] { key, val });
            }

            // Custom fields
            ReadCustomFields(output, type);
            return output;
        }

        private void ReadCustomFields(object output, Type type)
        {
            FieldInfo[] fs = serializeInfo.GetSerializableFieldInfo(type);
            foreach (var item in fs)
            {
                object fieldVal = ReadObject();
                item.SetValue(output, fieldVal);
            }
        }

        public object ReadDictionary(Type type)
        {
            int count = reader.ReadInt32();
            Type typeKey = null;
            Type typeVal = null;
            serializeInfo.GetDicTypes(type, out typeKey, out typeVal);

            Type generic = typeof(Dictionary<,>);
            Type[] typeArgs = { typeKey, typeVal };
            Type constructed = generic.MakeGenericType(typeArgs);
            object output = Activator.CreateInstance(constructed);

            for (int i = 0; i < count; i++)
            {
                object key = ReadObject();
                object val = ReadObject();

                constructed.InvokeMember("Add", BindingFlags.InvokeMethod, null, output, new object[] { key, val });
            }

            return output;

        }

        private object ReadList(Type type)
        {
            int count = reader.ReadInt32();
            Type listType = serializeInfo.GetListBaseType(type);
            Type generic = typeof(List<>);
            Type constructed = generic.MakeGenericType(listType);
            object output = Activator.CreateInstance(constructed);
            for (int i = 0; i < count; i++)
            {
                object val = ReadObject();
                constructed.InvokeMember("Add", BindingFlags.InvokeMethod, null, output, new object[] { val });
            }

            return output;
        }

        private object ReadCollectionBase(Type type)
        {
            object output = Activator.CreateInstance(type);
            Type valType = null;
            serializeInfo.GetCollectBaseType(type, out valType);
            MethodInfo method = serializeInfo.GetCollectBasAddMethod(type);
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                object val = ReadObject();
                method.Invoke(output, new object[] { val });
            }

            // Custom fields
            ReadCustomFields(output, type);

            return output;
        }

        private object ReadHashtable(Type type)
        {
            Hashtable hashTable = new Hashtable();
            int count = reader.ReadInt32();
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
            int len = reader.ReadInt32();
            byte[] bytes = reader.ReadBytes(len);
            MemoryStream ms = new MemoryStream(bytes);
            object output = formatter.Deserialize(ms);
            ms.Close();

            return output;
        }

        private object ReadPrimativeObject(Type type)
        {
            if (type == typeof(bool))
                return reader.ReadBoolean();
            else if (type == typeof(byte))
                return reader.ReadByte();
            else if (type == typeof(ushort))
                return reader.ReadUInt16();
            else if (type == typeof(uint))
                return reader.ReadUInt32();
            else if (type == typeof(ulong))
                return reader.ReadUInt64();
            else if (type == typeof(sbyte))
                return reader.ReadSByte();
            else if (type == typeof(short))
                return reader.ReadInt16();
            else if (type == typeof(int))
                return reader.ReadInt32();
            else if (type == typeof(long))
                return reader.ReadInt64();
            else if (type == typeof(char))
                return reader.ReadChar();
            else if (type == typeof(string))
                return reader.ReadString();
            else if (type == typeof(float))
                return reader.ReadSingle();
            else if (type == typeof(double))
                return reader.ReadDouble();
            else if (type == typeof(decimal))
                return reader.ReadDecimal();
            else if (type == typeof(DateTime))
                return new DateTime(reader.ReadInt64());
            else
                throw new Exception("Not primative type");
        }
        private bool ReadNullNotNull()
        {
            return reader.ReadBoolean();
        }

        private Type ReadObjectType()
        {
            byte id = reader.ReadByte();
            return serializeInfo.GetTypeById(id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace PromovaTraveller
{
    public class RawDataWriter
    {
        public RawDataWriter()
        {
            this.SerializeInfo = new SerializerInfo();
        }

        StreamWriter sw;
        public void Serialize(object input, string rawFilePath)
        {
            sw = new StreamWriter(rawFilePath);
            MemoryStream ms = new MemoryStream();
            Serialize(input, ms);
            byte[] bytes = ms.ToArray();
            WritePrimativeObject(bytes, typeof(byte[]));
            WritePrimativeObject(bytes.Length, typeof(int));
            ms.Close();
            sw.Close();
        }

        public void Serialize(object input, Stream infoStream)
        {
            //writer = new BinaryWriter(dataStream);
            WriteObject(input);
            formatter.Serialize(infoStream, SerializeInfo);
        }
        public SerializerInfo SerializeInfo { get; private set; }

        private void WriteObject(object input)
        {
            #region Write Null or NotNull/ Write type code
            bool isNull = WriteNullNotNull(input);
            if (isNull)
                return;
            Type type = input.GetType();
            WriteObjectType(type);

            #endregion
            #region Write value primitive type
            if (SerializeInfo.PrimativeValueTypes.Contains(type))
            {
                WritePrimativeObject(input, type);
                return;
            }
            #endregion

            // TODO: Comparer

            if (type == typeof(Comparer))
            {
                SerializeInfo.AddComparer(input);
                return;
            }
            else if (input is Enum)
            {
                WriteEnum(input, type);
                return;
            }
            else if (type == typeof(TimeSpan))
            {
                WritePrimativeObject(((TimeSpan)input).Ticks, typeof(long));
                return;
            }

            // REFERENCE TYPES .............
            #region Write object Id
            bool hasExisted = true;
            int objId = SerializeInfo.GetObjectId(input, out hasExisted);
            WritePrimativeObject(objId, typeof(int));
            if (hasExisted)
                return;
            #endregion
            else if (type == typeof(string))
            {
                WritePrimativeObject(input, typeof(string));
            }

            // TODO: TREATED TYPES
            // TODO: If input is Array
            else if (type.IsArray)
                WriteArray(input, type);
            else if (input is ArrayList)
            {
                WriteArrayList(input, type);
            }

                 // TODO: If input is DictionaryBase
            else if (input is DictionaryBase)
                WriteDicionaryBase(input, type);

        // TODO: CollectionBase
            else if (input is CollectionBase)
            {
                WriteCollectionBase(input, type);
            }

                // TODO: Hashtable
            else if (type == typeof(Hashtable))
            {
                WriteHashtable(input, type);
            }

                 // TODO: If input is IDictionary<>/Dictionary<>
            else if (type.Name == "IDictionary`2" || type.Name == "Dictionary`2")
            {
                WriteDictionary(input, type);
            }

            // TODO: If input is List/IList
            else if (type.Name == "List`1" || type.Name == "IList`1")
            {
                WriteList(input, type);
            }

                // TODO: SortedList
            else if (type == typeof(SortedList))
            {
                WriteSortedList(input, type);
            }

            // TODO: if input is System.Drawing.Point
            else if (type == typeof(Point))
            {
                WriteDrawingPoint(input);
            }

        // TODO: Type that cannot create Instance by Activator
            else if (!SerializeInfo.CanCreateInstanceByActivator(type))
            {
                if (!this.SerializeInfo.cannotCreateInstancetypes.Contains(type))
                    this.SerializeInfo.cannotCreateInstancetypes.Add(type);
                GeneralObjectSerialize(input);
            }

        // TODO: UnTreated types -> get fields come back to WriteObject
            else
            {
                if (!this.SerializeInfo.usingReflectionToCreateInstanceTypes.Contains(type))
                    this.SerializeInfo.usingReflectionToCreateInstanceTypes.Add(type);

                // Write custom fields
                WriteCustomFields(input);
            }
        }

        private void WriteSortedList(object input, Type type)
        {
            SortedList sortedList = input as SortedList;
            WritePrimativeObject(sortedList.Count, typeof(int));
            foreach (var item in sortedList)
            {
                DictionaryEntry dicEntry = (DictionaryEntry)item;
                WriteObject(dicEntry.Key);
                WriteObject(dicEntry.Value);
            }
        }

        private void WriteEnum(object input, Type type)
        {
            byte valId = SerializeInfo.GetEnumValId(type, input);
            WritePrimativeObject(valId, typeof(byte));
        }

        private void WriteArrayList(object input, Type type)
        {
            ArrayList arrayList = input as ArrayList;
            WritePrimativeObject(arrayList.Count, typeof(int));
            foreach (object t in arrayList)
            {
                WriteObject(t);
            }
            // Custom field
            WriteCustomFields(input);
        }



        private void WriteDrawingPoint(object input)
        {
            Point p = (Point)input;
            WritePrimativeObject((short)p.X, typeof(short));
            WritePrimativeObject((short)p.Y, typeof(short));
        }

        private void WriteArray(object input, Type type)
        {
            Array arr = input as Array;
            int count = arr.GetLength(0);
            WritePrimativeObject(count, typeof(int));
            for (int i = 0; i < count; i++)
            {
                object val = arr.GetValue(i);
                WriteObject(val);
            }
        }

        private void WriteDictionary(object obj, Type dictionaryType)
        {
            var keys = dictionaryType.InvokeMember("Keys", BindingFlags.GetProperty, null, obj, null);
            var vals = dictionaryType.InvokeMember("Values", BindingFlags.GetProperty, null, obj, null);
            Type keysType = keys.GetType();
            Type valsType = vals.GetType();
            int count = (int)keysType.InvokeMember("Count", BindingFlags.GetProperty, null, keys, null);
            WritePrimativeObject(count, typeof(int));
            var keysEnum = (IEnumerator)keysType.InvokeMember("GetEnumerator", BindingFlags.InvokeMethod, null, keys, null);
            var valsEnum = (IEnumerator)valsType.InvokeMember("GetEnumerator", BindingFlags.InvokeMethod, null, vals, null);
            for (int i = 0; i < count; i++)
            {
                keysEnum.MoveNext();
                valsEnum.MoveNext();
                WriteObject(keysEnum.Current);
                WriteObject(valsEnum.Current);
            }
        }

        private void WriteCollectionBase(object input, Type type)
        {
            int count = (input as CollectionBase).Count;
            WritePrimativeObject(count, typeof(int));

            IEnumerator enumerator = (input as CollectionBase).GetEnumerator();
            while (enumerator.MoveNext())
            {
                WriteObject(enumerator.Current);
            }
            // Custom field
            WriteCustomFields(input);
        }

        private void WriteList(object input, Type type)
        {
            IEnumerator enumerator = (input as IEnumerable).GetEnumerator();
            int count = (input as ICollection).Count;
            WritePrimativeObject(count, typeof(int));
            while (enumerator.MoveNext())
            {
                WriteObject(enumerator.Current);
            }
        }

        private void WriteCustomFields(object input)
        {
            FieldInfo[] fs = SerializeInfo.GetSerializableFieldInfo(input);
            foreach (var item in fs)
                WriteObject(item.GetValue(input));
        }

        private void WriteDicionaryBase(object input, Type type)
        {
            int count = (input as DictionaryBase).Count;
            WritePrimativeObject(count, typeof(int));
            IDictionaryEnumerator dicEnum = (input as DictionaryBase).GetEnumerator();
            Dictionary<object, object> keyValList = new Dictionary<object, object>();

            while (dicEnum.MoveNext())
            {
                keyValList[dicEnum.Key] = dicEnum.Value;
            }

            var query = keyValList.OrderBy(keyVal => keyVal.Key.ToString());
            foreach (KeyValuePair<object, object> keyValuePair in query)
            {
                WriteObject(keyValuePair.Key);
                WriteObject(keyValuePair.Value);
            }

            //while (dicEnum.MoveNext())
            //{
            //    WriteObject(dicEnum.Key);
            //    WriteObject(dicEnum.Value);
            //}

            // Custom fields
            WriteCustomFields(input);
        }
        private void WriteHashtable(object input, Type type)
        {
            Hashtable hashTable = input as Hashtable;
            WritePrimativeObject(hashTable.Count, typeof(int));

            foreach (var i in hashTable)
            {
                WriteObject(((DictionaryEntry)i).Key);
                WriteObject(((DictionaryEntry)i).Value);
            }
        }

        private BinaryFormatter formatter = new BinaryFormatter();
        private void GeneralObjectSerialize(object input)
        {
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, input);
            byte[] bytes = ms.ToArray();
            WritePrimativeObject(bytes.Length, typeof(int));
            WritePrimativeObject(bytes, typeof(byte[]));
            ms.Close();
        }

        private int line = 0;
        private void WritePrimativeObject(object obj, Type type)
        {
            line++;
            sw.Write(type.FullName + ":");
            sw.WriteLine(obj);

            return;

            //if (type == typeof(bool))
            //    writer.Write((bool)obj);
            //else if (type == typeof(byte))
            //    writer.Write((byte)obj);
            //else if (type == typeof(ushort))
            //    writer.Write((ushort)obj);
            //else if (type == typeof(uint))
            //    writer.Write((uint)obj);
            //else if (type == typeof(ulong))
            //    writer.Write((ulong)obj);
            //else if (type == typeof(sbyte))
            //    writer.Write((sbyte)obj);
            //else if (type == typeof(short))
            //    writer.Write((short)obj);
            //else if (type == typeof(int))
            //    writer.Write((int)obj);
            //else if (type == typeof(long))
            //    writer.Write((long)obj);
            //else if (type == typeof(char))
            //    writer.Write((char)obj);
            //else if (type == typeof(string))
            //    writer.Write((string)obj);
            //else if (type == typeof(float))
            //    writer.Write((float)obj);
            //else if (type == typeof(double))
            //    writer.Write((double)obj);
            //else if (type == typeof(decimal))
            //    writer.Write((decimal)obj);
            //else if (type == typeof(DateTime))
            //    writer.Write(((DateTime)obj).Ticks);
            //else if (type == typeof(byte[]))
            //    writer.Write((byte[])obj);
        }

        private void WriteObjectType(Type type)
        {
            byte typeId = SerializeInfo.GetTypeId(type);
            WritePrimativeObject(typeId, typeof(byte));
        }

        private BinaryWriter writer;
        private bool WriteNullNotNull(object obj)
        {
            if (obj == null)
            {
                WritePrimativeObject(true, typeof(bool));
                return true;
            }
            else
            {
                WritePrimativeObject(false, typeof(bool));
                return false;
            }
        }

    }


}

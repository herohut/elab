using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace PromovaTraveller
{
    public class Serializer
    {
        public Serializer()
        {
            SerializeInfo = new SerializerInfo();
        }
        public void Serialize(object input, Stream dataStream)
        {
            var ms = new MemoryStream();
            Serialize(input, dataStream, ms);
            byte[] bytes = ms.ToArray();
            _writer.Write(bytes);
            _writer.Write(bytes.Length);
            ms.Close();
        }

        public void Serialize(object input, Stream dataStream, Stream infoStream)
        {
            _writer = new BinaryWriter(dataStream);
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
            if (input is Enum)
            {
                WriteEnum(input, type);
                return;
            }
            if (type == typeof(TimeSpan))
            {
                _writer.Write(((TimeSpan)input).Ticks);
                return;
            }

            // REFERENCE TYPES .............
            #region Write object Id
            bool hasExisted;
            int objId = SerializeInfo.GetObjectId(input, out hasExisted);
            _writer.Write(objId);
            if (hasExisted)
                return;
            #endregion
            if (type == typeof(string))
            {
                _writer.Write((string)input);
            }

                // TODO: TREATED TYPES
                // TODO: If input is Array
            else if (type.IsArray)
                WriteArray(input);
            else if (input is ArrayList)
            {
                WriteArrayList(input);
            }

                // TODO: If input is DictionaryBase
            else if (input is DictionaryBase)
                WriteDicionaryBase(input);

                // TODO: CollectionBase
            else if (input is CollectionBase)
            {
                WriteCollectionBase(input);
            }

                // TODO: Hashtable
            else if (type == typeof(Hashtable))
            {
                WriteHashtable(input);
            }

                // TODO: If input is IDictionary<>/Dictionary<>
            else if (type.Name == "IDictionary`2" || type.Name == "Dictionary`2")
            {
                WriteDictionary(input, type);
            }

                // TODO: If input is List/IList
            else if (type.Name == "List`1" || type.Name == "IList`1")
            {
                WriteList(input);
            }

                // TODO: SortedList
            else if (type == typeof(SortedList))
            {
                WriteSortedList(input);
            }

                // TODO: if input is System.Drawing.Point
            else if (type == typeof(Point))
            {
                WriteDrawingPoint(input);
            }

                // TODO: Type that cannot create Instance by Activator
            else if (!SerializeInfo.CanCreateInstanceByActivator(type))
            {
                if (!SerializeInfo.CannotCreateInstancetypes.Contains(type))
                    SerializeInfo.CannotCreateInstancetypes.Add(type);
                GeneralObjectSerialize(input);
            }

                // TODO: UnTreated types -> get fields come back to WriteObject
            else
            {
                if (!SerializeInfo.UsingReflectionToCreateInstanceTypes.Contains(type))
                    SerializeInfo.UsingReflectionToCreateInstanceTypes.Add(type);

                // Write custom fields
                WriteCustomFields(input);
            }
        }

        private void WriteSortedList(object input)
        {
            var sortedList = (SortedList)input;
            _writer.Write(sortedList.Count);
            foreach (var item in sortedList)
            {
                var dicEntry = (DictionaryEntry)item;
                WriteObject(dicEntry.Key);
                WriteObject(dicEntry.Value);
            }
        }

        private void WriteEnum(object input, Type type)
        {
            byte valId = SerializeInfo.GetEnumValId(type, input);
            _writer.Write(valId);
        }

        private void WriteArrayList(object input)
        {
            var arrayList = (ArrayList)input;
            _writer.Write(arrayList.Count);
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
            _writer.Write((short)p.X);
            _writer.Write((short)p.Y);
        }

        private void WriteArray(object input)
        {
            var arr = (Array)input;
            int count = arr.GetLength(0);
            _writer.Write(count);
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
            var count = (int)keysType.InvokeMember("Count", BindingFlags.GetProperty, null, keys, null);
            _writer.Write(count);
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

        private void WriteCollectionBase(object input)
        {
            int count = ((CollectionBase) input).Count;
            _writer.Write(count);

            var enumerator = (input as CollectionBase).GetEnumerator();
            while (enumerator.MoveNext())
            {
                WriteObject(enumerator.Current);
            }
            // Custom field
            WriteCustomFields(input);
        }

        private void WriteList(object input)
        {
            var enumerator = ((IEnumerable) input).GetEnumerator();
            int count = ((ICollection) input).Count;
            _writer.Write(count);
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

        private void WriteDicionaryBase(object input)
        {
            int count = ((DictionaryBase) input).Count;
            _writer.Write(count);
            var dicEnum = (input as DictionaryBase).GetEnumerator();

            while (dicEnum.MoveNext())
            {
                WriteObject(dicEnum.Key);
                WriteObject(dicEnum.Value);
            }

            // Custom fields
            WriteCustomFields(input);
        }
        private void WriteHashtable(object input)
        {
            var hashTable = (Hashtable) input;
            _writer.Write(hashTable.Count);

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
            _writer.Write(bytes.Length);
            _writer.Write(bytes);
            ms.Close();
        }

        private void WritePrimativeObject(object obj, Type type)
        {
            if (type == typeof(bool))
                _writer.Write((bool)obj);
            else if (type == typeof(byte))
                _writer.Write((byte)obj);
            else if (type == typeof(ushort))
                _writer.Write((ushort)obj);
            else if (type == typeof(uint))
                _writer.Write((uint)obj);
            else if (type == typeof(ulong))
                _writer.Write((ulong)obj);
            else if (type == typeof(sbyte))
                _writer.Write((sbyte)obj);
            else if (type == typeof(short))
                _writer.Write((short)obj);
            else if (type == typeof(int))
                _writer.Write((int)obj);
            else if (type == typeof(long))
                _writer.Write((long)obj);
            else if (type == typeof(char))
                _writer.Write((char)obj);
            else if (type == typeof(string))
                _writer.Write((string)obj);
            else if (type == typeof(float))
                _writer.Write((float)obj);
            else if (type == typeof(double))
                _writer.Write((double)obj);
            else if (type == typeof(decimal))
                _writer.Write((decimal)obj);
            else if (type == typeof(DateTime))
                _writer.Write(((DateTime)obj).Ticks);
        }

        private void WriteObjectType(Type type)
        {
            byte typeId = SerializeInfo.GetTypeId(type);
            _writer.Write(typeId);
        }

        private BinaryWriter _writer;
        private bool WriteNullNotNull(object obj)
        {
            if (obj == null)
            {
                _writer.Write(true);
                return true;
            }
            _writer.Write(false);
            return false;
        }

    }


}

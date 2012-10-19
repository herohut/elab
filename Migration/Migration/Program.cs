using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using DataDomain.DataObjects;
using ErikEJ.SqlCeScripting;
using GenericRemoteServer.Database;
using PromovaTraveller;

namespace Migration
{
    class Program
    {
        //private const string InputFile = "sample.dat";
        private const string InputFile = @"E:\PROMOVA\config\ESPAS\Data\00000000000000000081.snapshot";
        private static bool BuilDbOnly = false;

        //private const string ConStringSqlServer = @"Data Source=HERO-PC\SQLEXPRESS;Initial Catalog=Modeva;Integrated Security=True";
        //static readonly IRepository Repo = new ServerDBRepository4(ConStringSqlServer);
        //private static readonly SqlCeHelper SqlHelper = new SqlServerHelper();

        private const string ConStringSqlCe = @"Data Source=D:\HERO\elab\Migration\MigDb.sdf;Password=hero;Max Database Size=1024";

        static readonly IRepository Repo = new DB4Repository(ConStringSqlCe);
        private static readonly SqlCeHelper SqlHelper = new SqlCeHelper();

        static void Main()
        {

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            RemoteDatabase db;
            var formatter = new BinaryFormatter();
            Console.WriteLine("Start deserializing");
            using (var f = File.OpenRead(InputFile))
            {
                db = (RemoteDatabase)formatter.Deserialize(f);
            }

            Console.WriteLine("Finish deserializing");
            Console.WriteLine("Start writing");

            _startTime = DateTime.Now;
            DropAllTables();

            WriteObject(db);

            //ExcecuteCommands();

            Repo.Dispose();
            Console.WriteLine();
            Console.WriteLine("Finish. OBJ_NUM: " + Objects.Count);
            Console.ReadLine();
        }

        private static void ExcecuteCommands()
        {
            foreach (var dbCommand in DataCommandList)
            {
                dbCommand.ExecuteNonQuery();
            }
        }

        private static DateTime _startTime = DateTime.Now;
        static void Measure()
        {
            if (Objects.Count % 2000 != 0)
                return;
            Console.SetCursorPosition(0, Console.CursorTop);

            var speed = string.Format("OBJ_NUM: {1}      SPEED: {0:0.00} objs/s", Objects.Count / (DateTime.Now - _startTime).TotalSeconds, Objects.Count);
            Console.Write(speed);
        }


        static readonly SerializerInfo SerializeInfo = new SerializerInfo { AdvoidDuplicateNLevel = true };
        public class Hero
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
        // FIRST STEP: NO CONSTRAINTS
        private static void WriteObject(object input, object key = null)
        {
            // TODO: 1. Null -> escape
            // Not null ->  get type of input: inputType

            // TODO: 1.1 Comparer, Enum -> escape (CHECK LATER)


            // TODO: 2. if inputType is NOT primatives/string/collections, (mean RefType)

            // TODO: 3. get serialized fields

            // TODO: 4. if serializedType IS primatives/string -> create column for inputType-> insert data
            // Create Table (id)
            // If input.Parent is NOT null
            // create ref column ParentId -> constraint to parent

            // TODO: 5. if serializedType IS RefType -> create ref column for inputType, set serializeType.Parent = input -> goto WriteObject(serializedType)

            // TODO: 6. if serializedType IS collections
            // looping collections: collectionItem->, collectionItem.Parent = input,  goto WriteObject(collectionItem)

            // create table serializedType (id, ref table = input)
            // looping collection: collectionItem
            // collectionItem.Parent = serializedType

            // CODES
            // 1.
            if (input == null)
                return;
            var type = input.GetType();

            // 1.1 
            if (type == typeof(Comparer) || type.IsEnum)
                return;

            if (Objects.Contains(input))
                return;
            Measure();

            Objects.Add(input);

            if (input is IEnumerable)
            {
                WriteEnumerable((IEnumerable)input);
                return;
            }

            // 2.
            if (SerializeInfo.PrimativeValueTypes.Contains(type))
            {
                // CHECK LATER
                return;
            }

            // 3.
            var fields = SerializeInfo.GetSerializableFieldInfo(input).ToList();

            // 4.
            var primativeAndTextFields = fields.GetOut(f => SerializeInfo.PrimativeValueTypes.Contains(f.FieldType) || (Type)f.FieldType == typeof(string));

            //CreateTableColumn(tName, primativeAndTextFields);
            CreateAndInsertObjToDb(input, primativeAndTextFields, key);


            // 5.
            var refTypes = fields.GetOut(f =>
                                            {
                                                if (SerializeInfo.PrimativeValueTypes.Contains(f.FieldType))
                                                    return false;

                                                if (typeof(string).IsAssignableFrom(f.FieldType))
                                                    return false;

                                                if (SerializeInfo.TreatedCollectionTypes.Exists(t => t.IsAssignableFrom(f.FieldType)))
                                                    return false;
                                                return true;
                                            });

            foreach (var field in refTypes)
            {
                var val = field.GetValue(input);
                if (val == null)
                    continue;
                val.SetParent(input);
                //val.SetName(field.FieldType.Name);
                WriteObject(val);
            }

            // 6.
            var enumerables = fields.GetOut(f => typeof(IEnumerable).IsAssignableFrom(f.FieldType));
            foreach (var enumerable in enumerables)
            {
                var d = enumerable.GetValue(input) as IEnumerable;
                if (d == null)
                    continue;
                d.SetParent(input);
                WriteEnumerable(d);
            }
        }


        private static void WriteEnumerable(IEnumerable input)
        {
            var enumerator = input.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == null)
                    continue;
                dynamic current = enumerator.Current;
                Type type = enumerator.Current.GetType();
                if (type.Name == "KeyValuePair`2" || enumerator.Current is DictionaryEntry)
                {
                    // TODO: Write Value Only, CHECK LATER
                    object key = current.Key;
                    object value = current.Value;
                    if (value == null || key == null)
                        continue;

                    value.SetParent(input.GetParent());
                    key.SetParent(input.GetParent());
                    WriteObject(value, key);

                    continue;
                }

                object other = current;
                other.SetParent(input.GetParent());
                WriteObject(other);
            }
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var pos = args.Name.IndexOf(",");
            var exactName = args.Name.Substring(0, pos);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var t in assemblies.Where(t => t.GetName().Name == exactName))
            {
                return t;
            }
            throw new Exception("Cannot find Assembly" + args.Name);
        }

        private static string GetParentName(object input)
        {
            var p = input.GetParent();
            if (p == null)
                return null;
            //var n = p.GetName();
            //return n ?? p.GetType().Name;
            return p.GetType().Name;
        }

        // Database Methods
        private static void CreateTable(string name)
        {
            name = RedirectSqlTableColName(name);
            var cmd = string.Format("Create Table {0}(__Id int NOT NULL)", name);

            Repo.ExecuteNonQuery(cmd);

            cmd = string.Format("Alter Table {0} Add Constraint PK_{0} Primary Key (__Id)", name);
            Repo.ExecuteNonQuery(cmd);

            RefreshCache();
        }

        private static void CreateFkConstraint(string tName, string refTName)
        {
            var cmd = string.Format("Alter Table {0} Add Constraint Fk_{0}_{1} Foreign Key (__Id{1}) References {1}(__Id) On Update NO ACTION On Delete NO ACTION", tName, refTName);
            Repo.ExecuteNonQuery(cmd);
        }

        //private static List<object> _hashes = new List<object>();
        private static readonly HashSet<object> Objects = new HashSet<object>();

        private static void FixTextCol(string tName, string colName, string val)
        {
            var col = _columns[ColName(tName, colName)];
            if (val.Length > 4000 && col.DataType != "ntext")
            {
                Repo.ExecuteNonQuery(string.Format("Alter Table {0} Alter Column {1} ntext", tName, colName));
                RefreshCache(tableList: false);
            }
            else if (val.Length > 200 && col.CharacterMaxLength < 4000)
            {
                Repo.ExecuteNonQuery(string.Format("Alter Table {0} Alter Column {1} nvarchar(4000)", tName, colName));
                RefreshCache(tableList: false);
            }
            else if (val.Length > 100 && col.CharacterMaxLength < 400)
            {
                Repo.ExecuteNonQuery(string.Format("Alter Table {0} Alter Column {1} nvarchar(400)", tName, colName));
                RefreshCache(tableList: false);
            }
            else if (val.Length > 10 && col.CharacterMaxLength < 100)
            {
                Repo.ExecuteNonQuery(string.Format("Alter Table {0} Alter Column {1} nvarchar(100)", tName, colName));
                RefreshCache(tableList: false);
            }
        }

        private static DbCommand CreateInsertCommand(string tName, Dictionary<string, object> keyValues)
        {

            var cols = keyValues.Keys.Aggregate("", (a, b) => a + RedirectSqlTableColName(b) + ",").TrimEnd(',');
            var vals = keyValues.Keys.Aggregate("", (a, b) => a + "@" + RedirectSqlTableColName(b) + ",").TrimEnd(',');
            var cmd = SqlHelper.CreateCommand(string.Format("INSERT INTO {0}({1}) VALUES({2})", tName, cols, vals), Repo.Connection);
            foreach (var key in keyValues.Keys)
            {
                var val = keyValues[key];

                if (val != null && val is string)
                    FixTextCol(tName, key, val as string);

                var para = SqlHelper.CreateDbParameter("@" + RedirectSqlTableColName(key), val);
                cmd.Parameters.Add(para);
                if (val == null)
                    para.Value = DBNull.Value;
                if (val is DateTime && (((DateTime)val).Year <= 1753 || ((DateTime)val).Year >= 9999))
                    para.Value = DBNull.Value;
            }

            return cmd;
        }

        private static void CreateAndInsertObjToDb(object input, IEnumerable<FieldInfo> fields, object key = null)
        {
            //var tName = input.GetName() ?? input.GetType().Name;
            var tName = input.GetType().Name;
            tName = RedirectSqlTableColName(tName);
            var parentTableName = GetParentName(input);
            if (parentTableName != null)
                parentTableName = RedirectSqlTableColName(parentTableName);

            if (!HasTable(tName))
                CreateTable(tName);

            if (parentTableName != null && !_columns.ContainsKey(ColName(tName, "__Id" + parentTableName)))
            {
                CreateTableColumn(tName, "__Id" + parentTableName, typeof(int));
                CreateFkConstraint(tName, parentTableName);
            }


            var keyValues = new Dictionary<string, object>();
            var id = GenNewSysId(input.GetType());
            input.SetId(id);
            keyValues["__Id"] = id;

            if (key != null)
            {
                var keyType = key.GetType();
                // Normal types
                if (SerializeInfo.PrimativeValueTypes.Contains(keyType) || keyType == typeof(string))
                {
                    CreateTableColumn(tName, "__Key" + keyType.Name, keyType);
                    keyValues["__Key" + keyType.Name] = key;
                }
                // KEY IS RefType
                else
                {
                    CreateTableColumn(tName, "__Key", typeof(int));
                    key.SetParent(input);
                    WriteObject(key);
                    keyValues["__Key"] = key.GetId();
                }
            }


            if (parentTableName != null)
            {
                var pId = input.GetParent().GetId();
                keyValues["__Id" + parentTableName] = pId == -1 ? DBNull.Value : (object)pId;
            }
            foreach (var field in fields)
            {
                keyValues[field.Name] = field.GetValue(input);
            }

            CreateTableColumn(tName, fields);

            var command = CreateInsertCommand(tName, keyValues);

            if (BuilDbOnly)
                DataCommandList.Add(command);
            else
                command.ExecuteNonQuery();
        }

        private static readonly List<DbCommand> DataCommandList = new List<DbCommand>();

        private static void CreateTableColumn(string tName, IEnumerable<FieldInfo> fields)
        {
            foreach (var field in fields)
            {
                CreateTableColumn(tName, field);
            }
        }

        private static int GenNewSysId(Type type)
        {
            if (!SysIds.ContainsKey(type))
                SysIds[type] = 0;
            var output = SysIds[type] + 1;
            SysIds[type] = output;
            return output;
        }

        private static void CreateTableColumn(string tName, FieldInfo field)
        {
            CreateTableColumn(tName, field.Name, field.FieldType);
        }

        private static readonly Dictionary<Type, int> SysIds = new Dictionary<Type, int>();

        private static void CreateTableColumn(string tName, string colName, Type fieldType)
        {
            colName = RedirectSqlTableColName(colName);
            tName = RedirectSqlTableColName(tName);
            if (_columns.ContainsKey(ColName(tName, colName)))
                return;
            var cmd = string.Format(SqlHelper.AddColumn, tName, colName, GetMapDbType(fieldType));

            Repo.ExecuteNonQuery(cmd);
            RefreshCache(tableList: false);
        }


        private class SqlCeHelper
        {
            public virtual string AddColumn
            {
                get { return "ALTER TABLE {0} ADD COLUMN {1} {2}"; }
            }

            public virtual DbCommand CreateCommand(string cmdText, IDbConnection conn)
            {
                return new SqlCeCommand(cmdText, (SqlCeConnection)conn);
            }

            public virtual DbParameter CreateDbParameter(string name, object value)
            {
                return new SqlCeParameter(name, value);
            }
        }

        private class SqlServerHelper : SqlCeHelper
        {
            public override string AddColumn
            {
                get { return "ALTER TABLE {0} ADD {1} {2}"; }
            }

            public override DbCommand CreateCommand(string cmdText, IDbConnection conn)
            {
                return new SqlCommand(cmdText, (SqlConnection)conn);
            }

            public override DbParameter CreateDbParameter(string name, object value)
            {
                return new SqlParameter(name, value);
            }
        }

        private static string GetMapDbType(Type fieldType)
        {
            if (fieldType == typeof(string))
                return "nvarchar(10)";
            if (fieldType == typeof(int))
                return "int";
            if (fieldType == typeof(float))
                return "float";
            if (fieldType == typeof(long))
                return "bigint";
            if (fieldType == typeof(DateTime))
                return "datetime";
            if (fieldType == typeof(bool))
                return "bit";
            if (fieldType == typeof(char))
                return "nchar";
            throw new Exception("Untreated type");
        }

        private static bool HasTable(string name)
        {
            name = RedirectSqlTableColName(name);
            return _tableList.Contains(name);
        }

        private static string RedirectSqlTableColName(string name)
        {
            //return name;

            switch (name.ToUpper())
            {
                case "USER":
                    name = "_User";
                    break;
                case "END":
                    name = "_End";
                    break;
                case "GROUP":
                    name = "_Group";
                    break;
                case "OFFSET":
                    name = "_OffSet";
                    break;
                case "CHECK":
                    name = "_Check";
                    break;
            }

            return name;
        }


        private static void DropAllTables()
        {

            var keys = Repo.GetAllForeignKeys();
            foreach (var constraint in keys)
            {
                Repo.ExecuteNonQuery(string.Format("ALTER TABLE {0} DROP CONSTRAINT {1}", constraint.ConstraintTableName,
                                                   constraint.ConstraintName));
            }

            var tbList = Repo.GetAllTableNames();
            tbList.ForEach(tb => Repo.ExecuteNonQuery("DROP TABLE " + tb));
            ClearCache();
        }



        private static HashSet<string> _tableList = new HashSet<string>();
        //private static HashSet<string> _columnList = new HashSet<string>();
        private static Dictionary<string, Column> _columns = new Dictionary<string, Column>();


        private static void RefreshCache(bool colList = true, bool tableList = true)
        {
            if (tableList)
                _tableList = new HashSet<string>(Repo.GetAllTableNames());
            if (colList)
            {
                var lst = Repo.GetColumnsFromTable();

                _columns = lst.ToDictionary(c => ColName(c.TableName, c.ColumnName), c => c);
                //_columnList =
                //    new HashSet<string>(Repo.GetColumnsFromTable().Select(c => ColName(c.TableName, c.ColumnName)));
            }
        }

        private static string ColName(string tableName, string colName)
        {
            tableName = RedirectSqlTableColName(tableName);
            return tableName + "." + colName;
        }

        private static void ClearCache()
        {
            _tableList.Clear();
            //_columnList.Clear();
            _columns.Clear();
        }
    }

    internal static class Extensive
    {
        private static readonly Dictionary<object, object> ObjToParent = new Dictionary<object, object>();
        public static void SetParent(this object input, object parent)
        {
            ObjToParent[input] = parent;
        }

        public static object GetParent(this object input)
        {
            return !ObjToParent.ContainsKey(input) ? null : ObjToParent[input];
        }

        private static readonly Dictionary<object, string> ObjToName = new Dictionary<object, string>();
        public static void SetName(this object input, string name)
        {
            ObjToName[input] = name;
        }

        private static readonly Dictionary<object, int> ObjIds = new Dictionary<object, int>();

        public static void SetId(this object input, int id)
        {
            ObjIds[input] = id;
        }

        public static int GetId(this object input)
        {
            if (!ObjIds.ContainsKey(input))
                return -1;
            return ObjIds[input];
        }

        public static string GetName(this object input)
        {
            return ObjToName == null || !ObjToName.ContainsKey(input) ? null : ObjToName[input];
        }

        public static List<T> GetOut<T>(this List<T> input, Func<T, bool> func)
        {
            var filters = input.Where(func).ToList();
            input.RemoveAll(filters.Contains);
            return filters;
        }
    }
}

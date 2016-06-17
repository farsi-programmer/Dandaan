// http://offtopic.blog.ir/

using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan
{
    class SQL
    {
        // from ef
        public static string AppendStringLiteral(string literalValue)
        {
            return "N'" + literalValue.Replace("'", "''") + "'";
        }

        public static string IfNotExistsTable(string tableName)
        {
            // CREATE TABLE locks sys.tables return $@"if not exists (select * from information_schema.tables where table_name=N'{tableName}')";
            return $@"if not exists (select * from sys.tables where name=N'{tableName}')";
        }

        public static string IfNotExistsDefaultConstraint(string name)
        {
            return $@"if not exists (select * from sys.default_constraints where name=N'{name}')";
        }

        public static void CreateTable(Type t)
        {
            var ps = t.GetProperties();
            var sb = new StringBuilder();
            sb.AppendLine(IfNotExistsTable(t.Name));
            sb.AppendLine($"CREATE TABLE [dbo].[{t.Name}] (");

            foreach (var item in ps)
            {
                var desc = Reflection.GetDandaanColumnAttribute(item).Sql;
                sb.AppendLine($"[{item.Name}] {desc},");
            }

            sb.AppendLine(");");

            var sql = Transaction(sb.ToString());

            if (t.Name != nameof(Tables.Table))
            {
                // if the table has changed, there should be a migration method for it

                // first we insert, if there is nothing for this table
                if (Insert(new Tables.Table() { Name = t.Name, SQL = sql, Version = 0 },
                    nameof(Tables.Table.Name)) == 0)
                {
                    var last = Tables.Table.Select(new Tables.Table() { Name = t.Name });

                    if (sql != last.SQL)
                    {
                        t.GetMethod("MigrateTo" + (last.Version + 1)).Invoke(null, null);

                        // what if the program is closed here, can we call MigrateTo more than once?
                        // should we make these two lines atomic?

                        Insert(new Tables.Table() { Name = t.Name, SQL = sql, Version = last.Version + 1 });
                    }
                }
            }

            DB.ExecuteNonQuery(sql);
        }

        public static string IfNotExistsReferentialConstraint(string constraintName)
        {
            return $@"if not exists (select * from information_schema.referential_constraints where
constraint_name=N'{constraintName}')";
        }

        // important
        // always use multiple lines, something might have been commented in the sql parameter
        // and always use semicolons
        // important

        public static string SerializableTransaction(string sql)
        {
            return $@"set transaction isolation level serializable;
begin transaction; {sql};
commit transaction;";
        }

        public static string ReadCommittedTransaction(string sql)
        {
            return $@"set transaction isolation level read committed;
begin transaction; {sql};
commit transaction;";
        }

        public static string Transaction(string sql)
        {
            return $@"begin transaction; {sql};
commit transaction;";
        }

        public static string RepeatableReadTransaction(string sql)
        {
            return $@"set transaction isolation level repeatable read;
begin transaction; {sql};
commit transaction;";
        }

        public static string ReadUncommittedTransaction(string sql)
        {
            return $@"set transaction isolation level read uncommitted;
begin transaction; {sql};
commit transaction;";
        }

        public static string SnapshotTransaction(string sql)
        {
            return $@"set transaction isolation level snapshot;
begin transaction; {sql};
commit transaction;";
        }

        public static string TransactionalLock(string sql, string mutexName, int TimeoutInMilliSeconds)
        {
            return $@"BEGIN TRANSACTION;
DECLARE @getapplock_result int;
EXEC @getapplock_result=sp_getapplock @Resource=N'{mutexName}', @LockMode=N'Exclusive',
	@LockOwner=N'Transaction', @LockTimeout={TimeoutInMilliSeconds}, @DbPrincipal=N'dbo';
if @getapplock_result=>0
begin
{sql}
end;
COMMIT TRANSACTION;";
        }

        // Locks associated with the current transaction are released when the transaction
        // commits or rolls back.
        // Locks can be explicitly released with sp_releaseapplock.

        public static string NonWaitableTransactionalLock(string sql, string mutexName)
        {
            // an alternative to this is to set LOCK_TIMEOUT to zero, and try to lock a table row
            // (0 means to not wait at all and return a message as soon as a lock is encountered)
            // but this method seems simpler:

            return $@"BEGIN TRANSACTION;
DECLARE @getapplock_result int;
EXEC @getapplock_result=sp_getapplock @Resource=N'{mutexName}', @LockMode=N'Exclusive',
	@LockOwner=N'Transaction', @LockTimeout=0, @DbPrincipal=N'dbo';
if @getapplock_result=0
begin
{sql}
end;
COMMIT TRANSACTION;";
        }

        public static string Insert(string table, List<string> columns, params string[] IfNotExists)
        {
            var sb = new StringBuilder();

            if (IfNotExists.Length > 0)
            {
                // we get an updlock and hold it for the duration of the transaction
                sb.Append($@"if not exists (select * from [dbo].[{table}] with (updlock,holdlock) where ");

                for (int i = 0; i < IfNotExists.Length; i++)
                {
                    sb.Append($"{IfNotExists[i]}=@{IfNotExists[i]}");
                    if (i != IfNotExists.Length - 1) sb.Append(" and ");
                }

                sb.Append(") ");
            }

            sb.Append($@"
begin
--waitfor delay '0:00:15'; --use this to see if locks are working correctly
insert into [dbo].[{table}] (");

            for (int i = 0; i < columns.Count; i++)
            {
                sb.Append(columns[i]);
                if (i != columns.Count - 1) sb.Append(", ");
            }            

            sb.Append(") values (");

            for (int i = 0; i < columns.Count; i++)
            {
                sb.Append("@" + columns[i]);
                if (i != columns.Count - 1) sb.Append(", ");
            }            

            sb.Append(@");
select scope_identity();
end;");

            return IfNotExists.Length > 0 ? Transaction(sb.ToString()) : sb.ToString();
        }

        /*DB.DataContextRun((context) =>
        {
            if (log.Message.Length > 800) log.Message = log.Message.Substring(0, 800);
            context.Logs.InsertOnSubmit(log);
            context.SubmitChanges();
        });*/

        public static int Insert(object obj, params string[] IfNotExists)
        {
            var p1 = new List<string>();
            var p2 = new List<SqlParameter>();

            var t = obj.GetType();
            var ms = t.GetProperties();

            foreach (var m in ms)
            {
                var desc = Reflection.GetDandaanColumnAttribute(m).Sql;

                if (!Common.IsMatch(desc, @"[\s]+identity[\s]+"))
                {
                    //!Common.IsMatch(desc, @"[\s]+default[\(\s]+")
                    p1.Add(m.Name);
                    p2.Add(SqlParameter(m.Name, t.GetProperty(m.Name).GetValue(obj), desc));
                }
            }

            var result = DB.ExecuteScalar(Insert(t.Name, p1, IfNotExists), p2.ToArray());

            //if (result != null && result != DBNull.Value)
            if (result is decimal) return (int)(decimal)result;

            return 0;
        }

        public static void Delete<T>(T obj) where T : class
        {
            editOrDelete(obj, null);
        }

        public static void Update<T>(T obj, T _obj) where T : class
        {
            editOrDelete(obj, _obj);
        }

        private static void editOrDelete<T>(T obj, T _obj) where T : class
        {
            using (var context = DB.DataContext)
            {
                var pi = context.GetType().GetField(typeof(T).Name + "s");
                var table = ((System.Data.Linq.Table<T>)(pi.GetValue(context)));

                if (_obj == null)
                {
                    table.Attach(obj);
                    table.DeleteOnSubmit(obj);
                }
                else table.Attach(obj, _obj);

                context.SubmitChanges();
            }
        }

        public static SqlParameter SqlParameter(string name, object value, string desc/*description attribute*/)
        {
            SqlParameter p = null;

            if (desc.Contains("[nvarchar]"))
            {
                var len = Common.Match(desc, $@"\[nvarchar][\s]*\(([\d]+)\)").Groups[1].Value;
                p = new SqlParameter($"@{name}", SqlDbType.NVarChar, int.Parse(len)) { Value = value };
            }
            else
            {
                var t = typeof(SqlDbType);
                foreach (var item in t.GetEnumNames())
                    if (desc.Contains("[" + item.ToLower() + "]"))
                        p = new SqlParameter($"@{name}", Enum.Parse(t, item)) { Value = value };
            }

            return p;
        }

        public static int Count<T>(T searchObj = null) where T : class
        {
            if (searchObj != null)
            {
                var sb = new StringBuilder();
                var ps = new List<object>();
                where(searchObj, true, typeof(T).GetProperties(), ref sb, ref ps, true);

                return (int)DB.ExecuteScalar($"SELECT COUNT(*) FROM {typeof(T).Name} WHERE 1=1 " + sb, ps.Cast<SqlParameter>().ToArray()); ;
            }
            else
            {
                using (var context = DB.DataContext)
                {
                    var pi = context.GetType().GetField(typeof(T).Name + "s");

                    return ((System.Data.Linq.Table<T>)(pi.GetValue(context))).Count();
                }
            }
        }

        // i am doing this because LINQ2SQL doesn't understand reflection
        // another method is to compile the c# code on the fly

        public static IEnumerable<T> Select<T>(int page, int pageSize, T obj, bool like = false) where T : class
        {
            if (page < 1) page = 1;
            int start = ((page - 1) * pageSize) + 1, end = start + pageSize - 1;

            var pi = typeof(T).GetProperties();

            var sb = new StringBuilder($"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ");

            foreach (var item in pi) sb.Append($"[t0].{item.Name},");
            sb = sb.Remove(sb.Length - 1, 1);

            sb.Append($") AS [ROW_NUMBER],* FROM [{typeof(T).Name}] AS [t0]");

            //var ps = new List<SqlParameter>();
            var ps = new List<object>();

            if (obj != null)
            {
                sb.Append(" WHERE 1=1 ");
                where(obj, like, pi, ref sb, ref ps);
            }

            sb.Append($") AS [t1] WHERE [t1].[ROW_NUMBER] BETWEEN {start} AND {end} ORDER BY [t1].[ROW_NUMBER]");

            /*using (var con = DB.Connection)
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = sb.ToString();
                cmd.Parameters.AddRange(ps.ToArray());
                cmd.ExecuteReader();

                var sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    yield return new Tables.Patient()
                    {
                        Id = (int)sdr[nameof(Id)],
                        DateTime = (DateTime)sdr[nameof(DateTime)],
                        Message = (string)sdr[nameof(Message)]
                    };
                }
                sdr.Close();
            }*/

            using (var context = DB.DataContext)
            using (var en = context.ExecuteQuery<T>(sb.ToString(), ps.ToArray()).GetEnumerator())
                while (en.MoveNext())
                    yield return en.Current;
        }

        private static void where<T>(T obj, bool like, PropertyInfo[] pi, ref StringBuilder sb, ref List<object> ps, bool sqlParams = false) where T : class
        {
            var i = 0;

            foreach (var item in pi)
            {
                if (like)
                {
                    var value = item.GetValue(obj);

                    if (item.PropertyType == typeof(string))
                    {
                        if ((value as string).Trim() != "")
                        {
                            if (sqlParams)
                            {
                                sb.Append($@" AND {item.Name} LIKE @{item.Name} ESCAPE '\'");
                                ps.Add(new SqlParameter($"@{item.Name}", escapeForLike(value as string) + "%"));
                            }
                            else
                            {
                                sb.Append($@" AND {item.Name} LIKE {{{i}}} ESCAPE '\'");
                                ps.Add(escapeForLike(value as string) + "%");
                                i++;
                            }
                        }
                    }
                    else if (value != null)
                    {
                        if (sqlParams)
                        {
                            sb.Append($" AND {item.Name} = @{item.Name}");
                            ps.Add(new SqlParameter($"@{item.Name}", value));
                        }
                        else
                        {
                            sb.Append($" AND {item.Name} = {{{i}}}");
                            ps.Add(value);
                            i++;
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> Select<T>(int page, int pageSize) where T : class
        {
            /*using (var connection = DB.Connection)
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"select * from [Log] order by id";
                var sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    yield return new Log()
                    {
                        Id = (int)sdr[nameof(Id)],
                        DateTime = (DateTime)sdr[nameof(DateTime)],
                        Message = (string)sdr[nameof(Message)]
                    };
                }
                sdr.Close();
            }*/

            if (page < 1) page = 1;

            using (var context = DB.DataContext)
            {
                var t = (System.Data.Linq.Table<T>)context.GetType().GetField(typeof(T).Name + "s")
                    .GetValue(context);

                var q = t.Skip((page - 1) * pageSize).Take(pageSize);

                //MessageBox.Show(context.GetCommand(q).CommandText);
                //.Where((x) => System.Data.Linq.SqlClient.SqlMethods.Like("", ""))

                using (var en = q.GetEnumerator())
                    while (en.MoveNext())
                        yield return en.Current;
            }
        }

        static string escapeForLike(string str)
        {
            str = Regex.Replace(str, Regex.Escape("%"), @"\%");
            str = Regex.Replace(str, Regex.Escape("_"), @"\_");
            str = Regex.Replace(str, Regex.Escape("["), @"\[");

            return str;
        }
    }
}

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
            var ms = t.GetProperties();
            var sb = new StringBuilder();
            sb.AppendLine(IfNotExistsTable(t.Name));
            sb.AppendLine($"CREATE TABLE [dbo].[{t.Name}] (");

            foreach (var m in ms)
            {
                var desc = GetDandaanAttribute(m).Sql;
                sb.AppendLine($"[{m.Name}] {desc},");
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

        public static int Insert(object obj, params string[] IfNotExists)
        {
            var p1 = new List<string>();
            var p2 = new List<SqlParameter>();

            var t = obj.GetType();
            var ms = t.GetProperties();

            foreach (var m in ms)
            {
                var desc = GetDandaanAttribute(m).Sql;

                if(!IsMatch(desc, @"[\s]+identity[\s]+")
                    && !IsMatch(desc, @"[\s]+default[\(\s]+"))
                {
                    p1.Add(m.Name);
                    p2.Add(SqlParameter(m.Name, t.GetProperty(m.Name).GetValue(obj), desc));
                }
            }

            var result = DB.ExecuteScalar(Insert(t.Name, p1, IfNotExists), p2.ToArray());

            //if (result != null && result != DBNull.Value)
            if (result is decimal) return (int)(decimal)result;

            return 0;
        }

        public static SqlParameter SqlParameter(string name, object value, string desc/*description attribute*/)
        {
            SqlParameter p = null;

            if (desc.Contains("[nvarchar]"))
            {
                var len = Match(desc, $@"\[nvarchar][\s]*\(([\d]+)\)").Groups[1].Value;
                p = new SqlParameter($"@{name}", SqlDbType.NVarChar, int.Parse(len)) { Value = value };
            }
            else if (desc.Contains("[int]"))
            {
                p = new SqlParameter($"@{name}", SqlDbType.Int) { Value = value };
            }
            else if (desc.Contains("[tinyint]"))
            {
                p = new SqlParameter($"@{name}", SqlDbType.TinyInt) { Value = value };
            }
            //else if

            return p;
        }

        public static Match Match(string input, string pattern)
        {
            return Regex.Match(input, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsMatch(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        public static Tables.DandaanAttribute GetDandaanAttribute(MemberInfo m)
        {
            var attributes = (Tables.DandaanAttribute[])m.GetCustomAttributes<Tables.DandaanAttribute>();

            return attributes[0];
        }

        public static Tables.DandaanAttribute GetDandaanAttribute(Type t)
        {
            var attributes = (Tables.DandaanAttribute[])t.GetCustomAttributes<Tables.DandaanAttribute>();

            return attributes[0];
        }

        public static string GetDescriptionAttribute(MemberInfo m, bool shouldHave = true) 
        {
            var attributes = (DescriptionAttribute[])m.GetCustomAttributes<DescriptionAttribute>();

            return GetFirstDescription(attributes, shouldHave);
        }

        public static string GetDescriptionAttribute(Type t, bool shouldHave = true) 
        {
            var attributes = (DescriptionAttribute[])t.GetCustomAttributes<DescriptionAttribute>();

            return GetFirstDescription(attributes, shouldHave);
        }

        private static string GetFirstDescription(DescriptionAttribute[] attributes, bool shouldHave)
        {
            if (shouldHave)
                return attributes[0].Description;
            else if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;

            return "";
        }

        public static int Count<T>() where T : class
        {
            using (var context = DB.DataContext)
            {
                var pi = context.GetType().GetField(typeof(T).Name + "s");

                return ((System.Data.Linq.Table<T>)(pi.GetValue(context))).Count();
            }
        }
    }
}

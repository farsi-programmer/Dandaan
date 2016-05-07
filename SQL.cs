using System;
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
    }
}

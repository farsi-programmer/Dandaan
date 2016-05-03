using System;
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
            return $@"
if not exists (select * from information_schema.tables where table_name=N'{tableName}')
";
        }

        public static string IfNotExistsDefaultConstraint(string name)
        {
            return $@"
if not exists (select * from sys.default_constraints where name=N'{name}')
";
        }

        public static string IfNotExistsReferentialConstraint(string constraintName)
        {
            return $@"
if not exists (select * from information_schema.referential_constraints where constraint_name=N'{constraintName}')
";
        }
    }
}

using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan.Tables
{
    [Table(Name = l)]
    public class Log
    {
        [Column]//(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column]
        public string Message { get; set; }

        [Column]//(IsDbGenerated = true)]
        public DateTime DateTime { get; set; }

        const string l = nameof(Log), m = nameof(Message), i = nameof(Id), d = nameof(DateTime);

        public static void CreateAndMigrate()
        {
            var sql = SQL.IfNotExistsTable(l) + $@"
CREATE TABLE [dbo].[{l}] (
    [{i}] [int] IDENTITY NOT NULL CONSTRAINT [PK_{l}] PRIMARY KEY CLUSTERED,
    [{m}] [nvarchar](800) NOT NULL,
    [{d}] [smalldatetime] NOT NULL CONSTRAINT [DF_{l}_{d}] DEFAULT (getdate()),
);";
            DB.ExecuteNonQuery(SQL.Transaction(sql));
       }

        public static IEnumerable<Log> Select()
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

            using (var context = DB.DataContext)
            using (var en = context.Logs.GetEnumerator())
                while (en.MoveNext())
                    yield return en.Current;
        }

        public static void Insert(Log log)
        {
            DB.ExecuteNonQuery($@"insert into [{l}] ({m}) values (@{m});",
                new SqlParameter($"@{m}", SqlDbType.NVarChar, 800) { Value = log.Message });

            /*DB.DataContextRun((context) =>
            {
                if (log.Message.Length > 800) log.Message = log.Message.Substring(0, 800);
                context.Logs.InsertOnSubmit(log);
                context.SubmitChanges();
            });*/
        }
    }
}

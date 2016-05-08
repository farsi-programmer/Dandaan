using System;
using System.Reflection;
using System.ComponentModel;
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
    [Table(Name = nameof(Log))]
    public class Log
    {
        [Column]//(IsPrimaryKey = true, IsDbGenerated = true)]
        [Description("[int] IDENTITY NOT NULL CONSTRAINT [PK_" + nameof(Log) + "] PRIMARY KEY CLUSTERED")]
        public int Id { get; set; }

        [Column]
        [Description("[nvarchar](800) NOT NULL")]
        public string Message { get; set; }

        [Column]//(IsDbGenerated = true)]
        [Description("[smalldatetime] NOT NULL CONSTRAINT [DF_" + nameof(Log) + "_" + nameof(DateTime) + "] DEFAULT (getdate())")]
        public DateTime DateTime { get; set; }

        public static void CreateAndMigrate(Type t)
        {
            SQL.CreateTable(t);
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

        public static int Insert(Log log)
        {
            /*DB.DataContextRun((context) =>
            {
                if (log.Message.Length > 800) log.Message = log.Message.Substring(0, 800);
                context.Logs.InsertOnSubmit(log);
                context.SubmitChanges();
            });*/

            var id = SQL.Insert(log);

            return id;
        }        
    }
}

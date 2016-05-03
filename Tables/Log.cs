using System;
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
        [Column]//[Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column]
        public string Message { get; set; }

        [Column]//[Column(IsDbGenerated = true)]
        public DateTime DateTime { get; set; }

        public static void CreateAndMigrate()
        {
            DB.ExecuteNonQuery($@"
{SQL.IfNotExistsTable(nameof(Log))}
    CREATE TABLE [dbo].[Log](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [Message] [nvarchar](800) NOT NULL,
	    [DateTime] [smalldatetime] NOT NULL CONSTRAINT [DF_Log_DateTime]  DEFAULT (getdate()),
     CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]");
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
            DB.ExecuteNonQuery(@"insert into [Log] (Message) values (@Message)",
                new SqlParameter("@Message", SqlDbType.NVarChar, 800) { Value = log.Message });

            /*DB.LinqContextRun((context) =>
            {
                context.Logs.InsertOnSubmit(log);
                context.SubmitChanges();
            });*/
        }
    }
}

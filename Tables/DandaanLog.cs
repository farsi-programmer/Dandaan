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
    [Table(Name = "Log")]
    public class DandaanLog
    {
        [Column]//[Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column]
        public string Message { get; set; }

        [Column]//[Column(IsDbGenerated = true)]
        public DateTime DateTime { get; set; }

        public const string CreateTable = @"
IF (SELECT count(*) FROM information_schema.tables WHERE table_name=N'Log') < 1
BEGIN
    CREATE TABLE [dbo].[Log](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [Message] [nvarchar](800) NOT NULL,
	    [DateTime] [smalldatetime] NOT NULL,
     CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[Log] ADD  CONSTRAINT [DF_Log_DateTime]  DEFAULT (getdate()) FOR [DateTime]
END
";

        public static IEnumerable<DandaanLog> Select()
        {
            /*using (var connection = DB.Connection)
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM log ORDER BY id";
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

            using (var context = DB.LinqContext)
            using (var en = context.DandaanLogs.GetEnumerator())
                while (en.MoveNext())
                    yield return en.Current;
        }

        public static void Insert(DandaanLog log)
        {
            DB.ExecuteNonQuery(@"insert into Log(Message) values(@Message)",
                new SqlParameter("@Message", SqlDbType.NVarChar, 800) { Value = log.Message });

            /*DB.LinqContextRun((context) =>
            {
                context.Logs.InsertOnSubmit(log);
                context.SubmitChanges();
            });*/
        }
    }
}

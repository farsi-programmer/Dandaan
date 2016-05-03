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
    [Table(Name = nameof(DandaanUser))]
    public class DandaanUser
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string Password { get; set; }

        [Column]
        public byte Enabled { get; set; }

        public const string CreateTable = @"
IF (SELECT count(*) FROM information_schema.tables WHERE table_name=N'DandaanUser') < 1
BEGIN
    CREATE TABLE [dbo].[DandaanUser](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [Name] [nvarchar](100) NOT NULL,
        [Password] [nvarchar](100) NOT NULL,
        [Enabled] [tinyint] NOT NULL,
     CONSTRAINT [PK_DandaanUser] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END";

        public static int Count()
        {
            using (var context = DB.LinqContext)
                return context.DandaanUsers.Count();
        }

        public static void Insert(DandaanUser dandaanUser)
        {
            DB.ExecuteNonQuery(@"insert into DandaanUser(Name) values(@Name)",
                new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = dandaanUser.Name });
        }
    }
}

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
    [Table(Name = nameof(User))]
    public class User
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string Password { get; set; }

        [Column]
        public byte Enabled { get; set; }

        public static void CreateAndMigrate()
        {
            DB.ExecuteNonQuery($@"
{SQL.IfNotExistsTable(nameof(User))}
    CREATE TABLE [dbo].[User](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Password] [nvarchar](100) NOT NULL CONSTRAINT [DF_User_Password]  DEFAULT (N''),
        [Enabled] [tinyint] NOT NULL CONSTRAINT [DF_User_Enabled]  DEFAULT ((1)),
     CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
    (
        [Name] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    CONSTRAINT
    IX_User UNIQUE NONCLUSTERED 
    (
        Id
    ) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]    
    ) ON [PRIMARY]");
        }

        public static int Count()
        {
            using (var context = DB.DataContext)
                return context.Users.Count();
        }

        public static void Insert(User user)
        {
            DB.ExecuteNonQuery(@"insert into [User] (Name) values (@Name)",
                new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = user.Name });
        }
    }
}

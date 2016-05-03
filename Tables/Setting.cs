using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan.Tables
{
    [Table(Name = nameof(Setting))]
    public class Setting
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int User { get; set; }

        [Column]
        public byte FormMainWindowState { get; set; }

        public static void CreateAndMigrate()
        {
            // Setting references User
            Tables.User.CreateAndMigrate();

            DB.ExecuteNonQuery($@"
{SQL.IfNotExistsTable(nameof(Setting))}
    CREATE TABLE [dbo].[Setting](
        [Id] [int] IDENTITY(1, 1) NOT NULL,
        [User] [int] NOT NULL CONSTRAINT [FK_Setting_User] FOREIGN KEY([User]) REFERENCES [dbo].[User] ([Id]),
        [FormMainWindowState] [tinyint] NOT NULL,
     CONSTRAINT [PK_Setting] PRIMARY KEY CLUSTERED
    (
       [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]");
            }
        public static Setting Select(int user)
        {
            using (var context = DB.DataContext)
                return context.Settings.Where(s => s.User == user).FirstOrDefault();
        }

        public static void Insert(Setting setting)
        {
            DB.ExecuteNonQuery($@"insert into Setting
({nameof(User)}, {nameof(FormMainWindowState)}) values
(@{nameof(User)}, @{nameof(FormMainWindowState)})",
                new SqlParameter($"@{nameof(User)}", SqlDbType.Int)
                { Value = setting.User },
                new SqlParameter($"@{nameof(FormMainWindowState)}", SqlDbType.TinyInt)
                { Value = setting.FormMainWindowState });
        }
    }
}

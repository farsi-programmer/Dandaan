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
        [Column(IsPrimaryKey = true)]
        public int UserId { get; set; }

        [Column]
        public byte FormMainWindowState { get; set; }

        public static void CreateAndMigrate()
        {
            // Setting references User
            User.CreateAndMigrate();

            var sql = SQL.IfNotExistsTable(nameof(Setting)) + @"
CREATE TABLE [dbo].[Setting] (
    [UserId] [int] NOT NULL CONSTRAINT [PK_Setting] PRIMARY KEY CLUSTERED
        CONSTRAINT [FK_Setting_User] FOREIGN KEY REFERENCES [dbo].[User] ([Id]),
    [FormMainWindowState] [tinyint] NOT NULL,
);";
            DB.ExecuteNonQuery(SQL.Transaction(sql));
        }

        public static Setting SelectOrInsert(int userId)
        {
            Setting setting = null;

            using (var context = DB.DataContext)
                setting = context.Settings.Where(s => s.UserId == userId).FirstOrDefault();

            if (setting == null) // this can happen
                Insert(new Setting() { UserId = userId });
            else return setting;

            return SelectOrInsert(userId);
        }

        private static void Insert(Setting setting)
        {
            var sql = SQL.Transaction(
                @"if not exists (select * from [Setting] with (updlock,holdlock) where UserId=@UserId)
insert into [Setting] ([UserId], [FormMainWindowState]) values (@UserId, @FormMainWindowState);");

            DB.ExecuteNonQuery(sql,
                new SqlParameter("@UserId", SqlDbType.Int) { Value = setting.UserId },
                new SqlParameter("@FormMainWindowState", SqlDbType.TinyInt)
                { Value = setting.FormMainWindowState });
        }

        internal static void Update(Setting setting)
        {
            using (var context = DB.DataContext)
            {
                context.Settings.Attach(setting, SelectOrInsert(setting.UserId));
                context.SubmitChanges();
            }      
        }
    }
}

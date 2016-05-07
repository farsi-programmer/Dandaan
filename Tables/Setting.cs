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
    [Table(Name = s)]
    public class Setting
    {
        [Column(IsPrimaryKey = true)]
        public int UserId { get; set; }

        [Column]
        public System.Windows.Forms.FormWindowState MainFormWindowState { get; set; }

        const string s = nameof(Setting), f = nameof(MainFormWindowState), u = nameof(UserId);

        public static void CreateAndMigrate()
        {
            // Setting references User
            User.CreateAndMigrate();

            var sql = SQL.IfNotExistsTable(s) + $@"
CREATE TABLE [dbo].[{s}] (
    [{u}] [int] NOT NULL CONSTRAINT [PK_{s}] PRIMARY KEY CLUSTERED
        CONSTRAINT [FK_{s}_{nameof(User)}] FOREIGN KEY REFERENCES [dbo].[{nameof(User)}] ([{nameof(User.Id)}]),
    [{f}] [int] NOT NULL,
);";
            DB.ExecuteNonQuery(SQL.Transaction(sql));
        }

        public static Setting SelectOrInsertDefault(int userId)
        {
            Setting setting = null;

            using (var context = DB.DataContext)
                setting = context.Settings.Where(s => s.UserId == userId).FirstOrDefault();

            if (setting == null) // this can happen
                Insert(new Setting() { UserId = userId });
            else return setting;

            return SelectOrInsertDefault(userId);
        }

        private static void Insert(Setting setting)
        {
            var sql = SQL.Transaction(
                $@"if not exists (select * from [{s}] with (updlock,holdlock) where {u}=@{u})
insert into [{s}] ([{u}], [{f}]) values (@{u}, @{f});");

            DB.ExecuteNonQuery(sql,
                new SqlParameter($"@{u}", SqlDbType.Int) { Value = setting.UserId },
                new SqlParameter($"@{f}", SqlDbType.TinyInt)
                { Value = setting.MainFormWindowState });
        }

        internal static void Update(Setting setting)
        {
            using (var context = DB.DataContext)
            {
                context.Settings.Attach(setting, SelectOrInsertDefault(setting.UserId));
                context.SubmitChanges();
            }      
        }
    }
}

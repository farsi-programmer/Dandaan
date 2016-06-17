using System;
using System.ComponentModel;
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
        [DandaanColumn(Sql = "[int] NOT NULL CONSTRAINT [PK_" + nameof(Setting) + @"] PRIMARY KEY CLUSTERED
CONSTRAINT [FK_" + nameof(Setting) + "_" + nameof(User) + @"] FOREIGN KEY REFERENCES [dbo].[" + nameof(User) + "] ([" + nameof(User.Id) + "])")]
        public int UserId { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public System.Windows.Forms.FormWindowState MainFormWindowState { get; set; }

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

        /*public static void MigrateTo1()
        {
            ;
        }*/

        private static void Insert(Setting setting)
        {
            SQL.Insert(setting, nameof(UserId));
        }

        public static void Update(Setting setting)
        {
            using (var context = DB.DataContext)
            {
                context.Settings.Attach(setting, SelectOrInsertDefault(setting.UserId));
                context.SubmitChanges();
            }
        }
    }
}

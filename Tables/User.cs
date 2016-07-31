using System;
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
    [Table(Name = nameof(User))]
    public class User
    {
        [Column]//(IsDbGenerated = true)]
        [DandaanColumn(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [IX_" + nameof(User) + @"]
UNIQUE NONCLUSTERED")]
        public int? Id { get; set; }

        [Column]//(IsPrimaryKey = true)]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL CONSTRAINT [PK_" + nameof(User) + @"]
PRIMARY KEY CLUSTERED")]
        public string Name { get; set; }

        [Column]//(IsDbGenerated = true)]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL")]// we don't use defaults, it makes things complicated CONSTRAINT [DF_" + nameof(User) + "_" + nameof(Password) + "] DEFAULT (N'')")]
        public string Password { get; set; }

        [Column]//(IsDbGenerated = true)]
        [DandaanColumn(Sql = "[int] NOT NULL")]// CONSTRAINT [DF_" + nameof(User) + "_" + nameof(Enabled) + "] DEFAULT ((1))")]
        public int? State { get; set; }

        [Obsolete("For Linq2Sql.", true)]
        public User() { }

        public User(bool withDefaultValuesForInsert)
        {
            if (withDefaultValuesForInsert)
            {
                State = 0;
            }
        }

        public static int Login(string name, string password)
        {
            var user = SQL.SelectFirstWithWhere(new User(false) { Name = name, Password = password }, false);
            return user != null ? user.Id.Value : 0;
        }

        public static bool IsEnabled(int id)
        {
            var user = SQL.SelectFirstWithWhere(new User(false) { Id = id }, false);
            return user != null ? (user.State == (int)UserState.Enabled) : false;
        }

        public static int Add(User user)
        {
            var id = SQL.Insert(user, nameof(Name));

            if (id > 0) SQL.SelectOrInsert(new Setting(false) { UserId = id }, new Setting(true) { UserId = id });

            return id;
        }
    }

    public enum UserState { Enabled, Disabled, }
}

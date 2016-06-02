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
        [Dandaan(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [IX_" + nameof(User) + "] UNIQUE NONCLUSTERED")]
        public int Id { get; set; }

        [Column]//(IsPrimaryKey = true)]
        [Dandaan(Sql = "[nvarchar](100) NOT NULL CONSTRAINT [PK_" + nameof(User) + "] PRIMARY KEY CLUSTERED")]
        public string Name { get; set; }

        [Column]//(IsDbGenerated = true)]
        [Dandaan(Sql = "[nvarchar](100) NOT NULL")]// we don't use defaults, it makes things complicated CONSTRAINT [DF_" + nameof(User) + "_" + nameof(Password) + "] DEFAULT (N'')")]
        public string Password { get; set; } = "";

        [Column]//(IsDbGenerated = true)]
        [Dandaan(Sql = "[tinyint] NOT NULL")]// CONSTRAINT [DF_" + nameof(User) + "_" + nameof(Enabled) + "] DEFAULT ((1))")]
        public byte Enabled { get; set; } = 1;

        public static int Count()
        {
            using (var context = DB.DataContext) return context.Users.Count();
        }

        public static int Login(string name, string password)
        {
            using (var context = DB.DataContext)
            {
                var user = context.Users.Where(u => u.Name == name && u.Password == password)         
                    .FirstOrDefault();

                return user != null ? user.Id : 0;
            }
        }

        public static bool IsEnabled(int id)
        {
            using (var context = DB.DataContext)
            {
                var user = context.Users.Where(u => u.Id == id).FirstOrDefault();

                return user != null ? (user.Enabled == 1) : false;
            }
        }

        public static int Insert(User user)
        {
            var id = SQL.Insert(user, nameof(Name));

            /*DB.DataContextRun((context) =>
            {
                if (user.Name.Length > 100) user.Name = user.Name.Substring(0, 100);
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            });*/

            if (id > 0) Setting.SelectOrInsertDefault(id);            

            return id;
        }
    }
}

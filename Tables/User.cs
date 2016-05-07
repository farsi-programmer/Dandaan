using System;
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
    [Table(Name = u)]
    public class User
    {
        [Column]//(IsDbGenerated = true)]
        public int Id { get; set; }

        [Column]//(IsPrimaryKey = true)]
        public string Name { get; set; }

        [Column]//(IsDbGenerated = true)]
        public string Password { get; set; }

        [Column]//(IsDbGenerated = true)]
        public byte Enabled { get; set; }

        const string u = nameof(User), n = nameof(Name), i = nameof(Id), p = nameof(Password),
            e = nameof(Enabled);

        public static void CreateAndMigrate()
        {
            var sql = SQL.IfNotExistsTable(u) + $@"
CREATE TABLE [dbo].[{u}] (
    [{i}] [int] IDENTITY NOT NULL CONSTRAINT [IX_{u}] UNIQUE NONCLUSTERED,
    [{n}] [nvarchar](100) NOT NULL CONSTRAINT [PK_{u}] PRIMARY KEY CLUSTERED,
    [{p}] [nvarchar](100) NOT NULL CONSTRAINT [DF_{u}_{p}] DEFAULT (N''),
    [{e}] [tinyint] NOT NULL CONSTRAINT [DF_{u}_{e}] DEFAULT ((1)), 
);";
            DB.ExecuteNonQuery(SQL.Transaction(sql));
        }

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
            var sql = SQL.Transaction(// we get an updlock and hold it for the duration of the transaction
                $@"if not exists (select * from [{u}] with (updlock,holdlock) where {n}=@{n})
begin
--waitfor delay '0:00:15'; -- use this to see if locks are working correctly
insert into [{u}] ({n}) values (@{n});
select scope_identity();
end;");

            var id = DB.ExecuteScalar(sql, new SqlParameter($"@{n}", SqlDbType.NVarChar, 100)
            { Value = user.Name });

            /*DB.DataContextRun((context) =>
            {
                if (user.Name.Length > 100) user.Name = user.Name.Substring(0, 100);
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            });*/

            //if (id != null && id != DBNull.Value)
            if(id is decimal)
            {
                Setting.SelectOrInsertDefault((int)(decimal)id);
                return (int)(decimal)id;
            }

            return 0;
        }
    }
}

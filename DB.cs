//#define using_sqlite
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity.Core.Common;
using System.Reflection;
#if using_sqlite
using System.Data.SQLite.EF6;
using System.Data.SQLite;
#endif

namespace Dandaan
{
#if using_sqlite
    class DB
    {
        public static MyDbContext<Log> Log = GetMyDbContext<Log>();

        public static MyDbContext<Patient> Patient = GetMyDbContext<Patient>();

        static MyDbContext<T> GetMyDbContext<T>() where T : class
        {
            using (var connection = new SQLiteConnection(ConnectionString<T>()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;

                    //MessageBox.Show(DB.GenerateSql<Log>());

                    command.CommandText = typeof(T) == typeof(Log) ?
                        @"CREATE TABLE IF NOT EXISTS """+ typeof(T).Name + @"s"" 
([Id] INTEGER, [Message] nvarchar, [DateTime] datetime NOT NULL, PRIMARY KEY(Id));"
                    : "";

                    command.ExecuteNonQuery();
                }
            }

            return new MyDbContext<T>(ConnectionString<T>());
        }

        public static string ConnectionString<T>()
        {
            return @"Data Source=" + typeof(T) + @".sqlite;Compress=False;";
        }

        public static string GenerateSql<T>() where T : class
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<T>();

            using (var connection = new SQLiteConnection(ConnectionString<T>()))
            {
                var model = modelBuilder.Build(connection);

                var sqliteSqlGenerator = new SQLite.CodeFirst.SqliteSqlGenerator();

                return sqliteSqlGenerator.Generate(model.StoreModel);
            }
        }

        public static void Init()
        {
            ;
        }

        public static void Close()
        {
            Log.Dispose();
            Patient.Dispose();
        }
    }

    public class SQLiteConnectionFactory : IDbConnectionFactory
    {
        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            return new SQLiteConnection(nameOrConnectionString);
        }
    }

    public class MyDbContext<T> : DbContext where T : class
    {
        public MyDbContext(string cs) : base(cs) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //var sqliteConnectionInitializer = new SQLite.CodeFirst.SqliteCreateDatabaseIfNotExists<MyDbContext<T>>(modelBuilder);
            //Database.SetInitializer(sqliteConnectionInitializer);

            //var model = modelBuilder.Build(Database.Connection);

            //SQLite.CodeFirst.IDatabaseCreator sqliteDatabaseCreator = new SQLite.CodeFirst.SqliteDatabaseCreator();
            //sqliteDatabaseCreator.Create(Database, model);
        }

        public DbSet<T> DbSet { get; set; }
    }
#else
    class DB
    {
        public const string ConnectionString = @"Data Source=(LocalDB)\mssqllocaldb;AttachDbFilename=|DataDirectory|\Dandaan.mdf;"
            //+ @"Initial Catalog=Dandaan;"
            //+ @"APP=Dandaan";
            + @"Integrated Security=True;MultipleActiveResultSets=True;";

        static MyDbContext firstContext = new MyDbContext(ConnectionString);

        public static void Init()
        {
            //Thread.Sleep(5000);
            Run((c) => c.Logs.Count());
            firstContext.Database.Connection.Open();
        }

        public static void Close()
        {
            try
            {
                firstContext.Database.Connection.Close();
                firstContext.Dispose();
            }
            catch { }
        }

        public static void Run(Action<MyDbContext> act, bool saveChanges = false)
        {
            if (Monitor.TryEnter(firstContext))
                try
                {
                    act(firstContext);
                    if (saveChanges) firstContext.SaveChanges();
                }
                finally
                {
                    Monitor.Exit(firstContext);
                }
            else
                using (var context = new MyDbContext(ConnectionString))
                {
                    try
                    {
                        context.Database.Connection.Open();
                        act(context);
                        if (saveChanges) context.SaveChanges();
                    }
                    finally
                    {
                        context.Database.Connection.Close();
                    }
                }
        }

        public static object ExecuteScalar(string sql)
        {
            object result = null;

            Run((context) =>
            {
                using (var cmd = context.Database.Connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    result = cmd.ExecuteScalar();
                }
            });

            return result;
        }

        public static int ExecuteNonQuery(string sql)
        {
            int result = 0;

            Run((context) =>
            {
                using (var cmd = context.Database.Connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    result = cmd.ExecuteNonQuery();
                }
            });

            return result;
        }

        public static DbDataReader ExecuteReader(string sql)
        {
            DbDataReader result = null;

            Run((context) =>
            {
                using (var cmd = context.Database.Connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    result = cmd.ExecuteReader();
                }
            });

            return result;
        }
    }

    public class MyDbContext : DbContext
    {
        public MyDbContext(string cs) : base(cs)
        {
            //Database.Log = s => System.Windows.Forms.MessageBox.Show(s);
        }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Patient> Patients { get; set; }
    }
#endif

    public class MyConfiguration : DbConfiguration
    {
        public MyConfiguration()
        {
#if using_sqlite
            SetDefaultConnectionFactory(new SQLiteConnectionFactory());
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
#else
            SetDefaultConnectionFactory(new LocalDbConnectionFactory("mssqllocaldb"));
#endif
        }
    }

    public class Log
    {
        public Log()
        {
            DateTime = DateTime.Now;
        }

        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }
    }

    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

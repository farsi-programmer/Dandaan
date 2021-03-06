﻿// http://offtopic.blog.ir/

//#define using_sqlite
//#define using_ef

namespace Dandaan
{
    using System;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
#if using_ef || using_sqlite
    using System.Data.Common;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Core.Common;
#endif
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;
    using System.Text;
    using System.Threading;
    using System.Reflection;
#if using_sqlite
    using System.Data.SQLite.EF6;
    using System.Data.SQLite;
#endif

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
        // TODO: we should use a text file (LocalSettings) for storing the connection string,
        // we might want to use sql server (express)

        private static bool attachDB = false;

        public static string ConnectionString =>
            @"Data Source=(LocalDB)\mssqllocaldb;"
              + (attachDB ? $@"AttachDbFilename=|DataDirectory|\{nameof(Dandaan)}.mdf;" : "")
            //+ @"Initial Catalog=Dandaan;"
            //+ $@"Initial Catalog=[{Program.DataDirectory}\{nameof(Dandaan)}.mdf];"
            //+ @"APP=Dandaan;"
            + @"Integrated Security=True;"
// i don't think i need this + @"MultipleActiveResultSets=True;"
;

#if using_ef
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

                //context.Database.ExecuteSqlCommand(sql
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

    internal sealed class Configuration : DbMigrationsConfiguration<MyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MyDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }  

    public class MyDbContext : DbContext
    {
        public MyDbContext(string cs) : base(cs)
        {
            // Or use the Profiler
            //Database.Log = s => System.Windows.Forms.MessageBox.Show(s);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<MyDbContext>(new MigrateDatabaseToLatestVersion<MyDbContext, Configuration>(true));
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Setting> Settings { get; set; }
#else

        /*public const string DB_DIRECTORY = "Data";

        public static SqlConnection GetLocalDB(string dbName, bool deleteIfExists = false)
        {
            try
            {
                string outputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DB_DIRECTORY);
                string mdfFilename = dbName + ".mdf";
                string dbFileName = Path.Combine(outputFolder, mdfFilename);
                string logFileName = Path.Combine(outputFolder, String.Format("{0}_log.ldf", dbName));
                // Create Data Directory If It Doesn't Already Exist.
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                // If the file exists, and we want to delete old data, remove it here and create a new database.
                if (File.Exists(dbFileName) && deleteIfExists)
                {
                    if (File.Exists(logFileName)) File.Delete(logFileName);
                    File.Delete(dbFileName);
                    CreateDatabase(dbName, dbFileName);
                }
                // If the database does not already exist, create it.
                else if (!File.Exists(dbFileName))
                {
                    CreateDatabase(dbName, dbFileName);
                }

                // Open newly created, or old database.
                string connectionString = String.Format(@"Data Source=(LocalDB)\v11.0;AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", dbName, dbFileName);
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch
            {
                throw;
            }
        }

        public static bool CreateDatabase(string dbName, string dbFileName)
        {
            try
            {
                string connectionString = String.Format(@"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();


                    DetachDatabase(dbName);

                    cmd.CommandText = String.Format("CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", dbName, dbFileName);
                    cmd.ExecuteNonQuery();
                }

                if (File.Exists(dbFileName)) return true;
                else return false;
            }
            catch
            {
                throw;
            }
        }

        public static bool DetachDatabase(string dbName)
        {
            try
            {
                string connectionString = String.Format(@"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = String.Format("exec sp_detach_db '{0}'", dbName);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }*/

        public static SqlConnection Connection
        {
            get
            {
                var connection = new SqlConnection(ConnectionString);
                connection.Open();
                return connection;
            }
        }

        public static Type DataContextType = typeof(DataContext);

        public static DataContext DataContext =>
            //new DataContext(ConnectionString);
            (DataContext)Activator.CreateInstance(DataContextType, new object[] { ConnectionString });

        public static void ContextRun(Action<DataContext> act)
        {
            using (var context = DataContext) act(context);
        }

        public static void ConnectionRun(Action<SqlConnection> act)
        {
            using (var connection = Connection) act(connection);
        }

        public static void ExecuteReader(string sql, Action<SqlDataReader> act,
            CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            ConnectionRun((connection) =>
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader(commandBehavior);

                while (reader.Read()) act(reader);

                reader.Close();
            });
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] sps)
        {
            using (var connection = Connection)
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(sps);
                return cmd.ExecuteScalar();
            }
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] sps)
        {
            using (var connection = Connection)
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                foreach (var p in sps) cmd.Parameters.Add(p);
                return cmd.ExecuteNonQuery();
            }
        }

        public static void Init()
        {
            attachOrCreateDatabase();

            createTablesAndMigrateData();

            loadAssemblies();
        }

        static void loadAssemblies()
        { 
            var name = $"{nameof(Dandaan)}.{nameof(Tables)}.{nameof(Tables.UserTable)}";
            var path = "";

            foreach (var item in DataContext.UserTableAssemblys)
            {
                path = $"{Program.DataDirectory}\\{name}{item.UserTableId}.dll";

                if (!File.Exists(path) || new FileInfo(path).Length == 0)
                    File.WriteAllBytes(path, item.Assembly);

                var assembly = Assemblies.Load(path);
                var type = assembly.GetType($"{nameof(Dandaan)}.{nameof(DataContext)}{item.UserTableId}");

                if (type.IsSubclassOf(DataContextType)) DataContextType = type;
            }
        }

        private static void createTablesAndMigrateData()
        {
            //ExecuteNonQuery(File.ReadAllText(Dir + "\\" + nameof(Dandaan) + ".sql"));

            var tables = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.Namespace == nameof(Dandaan) + "." + nameof(Tables)
                && t.IsPublic/*this is necessary because of ienumerable*/);
            
            var done = new List<Type>();

            // this is the first table
            createTable(typeof(Tables.Table), tables, done);

            foreach (var item in tables) createTable(item, tables, done);

            /*foreach (var t in tables)
            {
                //var f = t.GetField(nameof(Tables.DandaanLog.CreateTable));
                //sb.AppendLine((string)f.GetRawConstantValue());

                //var m = t.GetMethod(nameof(Tables.Log.CreateAndMigrate));
                //m.Invoke(null, new object[] { t });

                SQL.CreateTable(t);
            }*/
        }

        private static void createTable(Type t, IEnumerable<Type> tables, List<Type> done)
        {
            if (done.Contains(t)) return;

            var ps = t.GetProperties();

            foreach (var A in ps)
            {
                var sql = Reflection.GetDandaanColumnAttribute(A).Sql;

                if (Common.IsMatch(sql, "FOREIGN KEY REFERENCES"))
                {
                    var name = Common.Match(sql, @"FOREIGN KEY REFERENCES[\s]*\[dbo].\[([^]]+)]").Groups[1].Value;

                    if (t.Name != name) // self referencing table
                        createTable(tables.Where(_t => _t.Name == name).First(), tables, done);
                }
            }

            SQL.CreateTable(t);

            done.Add(t);
        }

        private static void attachOrCreateDatabase()
        {
            var name = nameof(Dandaan);
            var mdf = ".mdf";
            var dir = Program.DataDirectory;
            var path = dir + "\\" + name + mdf;

            if (!File.Exists(path))
            {
                // detach_db if the file is deleted

                /*if ((int)ExecuteScalar($@"if db_id(N'{path}') is not null select 1
else select count(*) from sys.databases where [name]=N'{path}'") > 0)
                    ExecuteScalar($@"USE [master];
-- Drop Connections
--ALTER DATABASE [{path}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
EXEC master.dbo.sp_detach_db @dbname = N'{path}';");*/

                //

                //try
                {
                    string DBName = "DBName", Location = "Location";
                    ExecuteReader($@"USE [master];
SELECT
db.name AS {DBName},
--type_desc AS FileType,
Physical_Name AS {Location}
FROM
sys.master_files mf
INNER JOIN
sys.databases db
ON db.database_id=mf.database_id
--WHERE db.name LIKE N'Dandaan_%'", (reader) =>
                           {
                               if (reader[Location] as string == path)
                                   ExecuteScalar($@"USE [master];
EXEC master.dbo.sp_detach_db @dbname = N'{reader[DBName]}';");
                           });
                }
                //catch { }
                
                //

                ExecuteScalar($@"create database
{name}_{Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)/*this is how ef does it*/}
on (name={name}, filename='{path}')
log on (name={name}_log, filename='{dir}\{name}_log.ldf');");
            }

            attachDB = true;
        }

        /*public static void CreateTable(Type tableType)
        {
            // if a table doesn't exist, we create it, and
            // if it does exist, we add the missing columns,
            // migrate data, remove columns, ...

            if (!TableExists(tableType.Name))
                ;//ExecuteNonQuery($@"{tableType.Name}");
            else
            {
                ;
            }
        }

        public static bool TableExists(string tableName)
        {
            return (int)ExecuteScalar($@"select count(*) from information_schema.tables
where table_name=N'{tableName}'") > 0;
        }*/

        public static void Log(string message)
        {
            try
            {
                SQL.Insert(new Tables.Log(true) { Message = message });
            }
            catch
            {
                Mutex mutex = null;

                try
                {
                    var name = nameof(Dandaan);

                    mutex = new Mutex(false, $"Global\\{name}Log");
                    mutex.WaitOne();

                    File.AppendAllText($"{Program.DataDirectory}\\{name}Log.txt",
                        $"{name}\t\t{DateTime.Now.ToString(CultureInfo.InvariantCulture)}\r\n{message}\r\n\r\n");
                }
                catch { }
                //catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                finally
                {
                    mutex?.ReleaseMutex();
                }
            }
        }
#endif
    }
#endif

#if using_ef || using_sqlite
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
#endif
}
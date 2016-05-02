﻿// http://offtopic.blog.ir/

//#define using_sqlite
//#define using_ef

namespace Dandaan
{
    using System;
    using System.Data;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Globalization;
    using System.IO;
    using System.Collections.Generic;
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
        // TODO: we should use a text file for storing the connection string,
        // we might want to use sql server (express)

        private static bool attachDb = false;

        public static string ConnectionString =>
            @"Data Source=(LocalDB)\mssqllocaldb;"
              + (attachDb ? $@"AttachDbFilename=|DataDirectory|\{nameof(Dandaan)}.mdf;" : "")
            //+ @"Initial Catalog=Dandaan;"
            //+ @"APP=Dandaan;"
            + @"Integrated Security=True;MultipleActiveResultSets=True;";

        public static readonly string Dir = Application.StartupPath;

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

        public static MyLinqContext LinqContext => new MyLinqContext(ConnectionString);

        public static void LinqContextRun(Action<MyLinqContext> act)
        {
            using (var context = DB.LinqContext) act(context);
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] sps)
        {
            using (var connection = Connection)
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                foreach (var p in sps) cmd.Parameters.Add(p);
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
            //Thread.Sleep(5000);            

            AttachOrCreateDatabase();

            //ExecuteNonQuery(File.ReadAllText(Dir + "\\" + nameof(Dandaan) + ".sql"));

            var sb = new StringBuilder();
            sb.AppendLine(@"BEGIN TRANSACTION");

            var tables = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.Namespace == nameof(Dandaan) + "." + nameof(Tables)
                && t.IsPublic/*this is necessary because of ienumerable*/);

            foreach (var t in tables)
            {
                //CreateTable(t);

                var f = t.GetField(nameof(Tables.Log.CreateTable));

                //if (f != null)
                sb.AppendLine((string)f.GetRawConstantValue());
            }

            sb.AppendLine(@"COMMIT");

            //MessageBox.Show(sb.ToString());
            ExecuteNonQuery(sb.ToString());
        }

        private static void AttachOrCreateDatabase()
        {
            var name = nameof(Dandaan);
            var mdf = ".mdf";

            /*if ((int)DB.ExecuteScalar($@"if db_id(N'{name}') is not null select 1
else select count(*) from sys.databases where [name]=N'{name}'") < 1)*/

            if (!File.Exists(Dir + "\\" + name + mdf))
            {          
                ExecuteScalar($@"create database
{name}_{Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)/*this is how ef does it*/}
on (name={name}, filename='{Dir}\{name}{mdf}')
log on (name={name}_log, filename='{Dir}\{name}_log.ldf')");
            }

            attachDb = true;
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
        }*/

        public static bool TableExists(string tableName)
        {
            return (int)ExecuteScalar($@"select count(*) from information_schema.tables
where table_name=N'{tableName}'") > 0;
        }

        // from ef
        public static string AppendStringLiteral(string literalValue)
        {
            return "N'" + literalValue.Replace("'", "''") + "'";
        }

        public static void Log(string message)
        {
            try
            {
                Tables.Log.Insert(new Tables.Log() { Message = message });
            }
            catch
            {
                Mutex mutex = null;

                try
                {
                    var name = nameof(Dandaan);

                    mutex = new Mutex(false, "Global\\" + name + "Log");
                    mutex.WaitOne();

                    File.AppendAllText(Dir + "\\" + name + ".txt",
                        name + "\t\t" + DateTime.Now.ToString(CultureInfo.InvariantCulture)
                        + "\r\n" + message + "\r\n\r\n");
                }
                catch { }
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

    public class MyLinqContext : DataContext
    {
        public Table<Tables.Log> Logs;

        public Table<Tables.DandaanSetting> DandaanSettings;

        public Table<Tables.DandaanUser> DandaanUsers;

        public MyLinqContext(string connection) : base(connection) { }

        public MyLinqContext(SqlConnection connection) : base(connection) { }
    }

    namespace Tables
    {
        /*public class DatabaseInfo
        {
            public int Version { get; set; }
        }*/

        [Table(Name = "Log")]
        public class Log
        {
            [Column]//[Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id { get; set; }

            [Column]
            public string Message { get; set; }

            [Column]//[Column(IsDbGenerated = true)]
            public DateTime DateTime { get; set; }
           
            public const string CreateTable = @"
IF (SELECT count(*) FROM information_schema.tables WHERE table_name=N'Log') < 1
BEGIN
    CREATE TABLE [dbo].[Log](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [Message] [nvarchar](800) NOT NULL,
	    [DateTime] [smalldatetime] NOT NULL,
     CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[Log] ADD  CONSTRAINT [DF_Log_DateTime]  DEFAULT (getdate()) FOR [DateTime]
END
";

            public static IEnumerable<Log> Select()
            {
                /*using (var connection = DB.Connection)
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"SELECT * FROM log ORDER BY id";
                    var sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        yield return new Log()
                        {
                            Id = (int)sdr[nameof(Id)],
                            DateTime = (DateTime)sdr[nameof(DateTime)],
                            Message = (string)sdr[nameof(Message)]
                        };
                    }
                    sdr.Close();
                }*/               

                using (var context = DB.LinqContext)
                using (var en = context.Logs.GetEnumerator())
                    while (en.MoveNext())
                        yield return en.Current;
            }

            public static void Insert(Log log)
            {
                DB.ExecuteNonQuery(@"insert into Log(Message) values(@Message)",
                    new SqlParameter("@Message", SqlDbType.NVarChar, 800) { Value = log.Message });

                /*DB.LinqContextRun((context) =>
                {
                    context.Logs.InsertOnSubmit(log);
                    context.SubmitChanges();
                });*/
            }
        }

        /*public class Patient
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }*/

        [Table(Name = nameof(DandaanSetting))]
        public class DandaanSetting
        {
            [Column]
            public int Id { get; set; }

            [Column]
            public int DandaanUser { get; set; }

            [Column]
            public int FormMainWindowState { get; set; }

            public const string CreateTable = @"
IF (SELECT count(*) FROM information_schema.tables WHERE table_name=N'DandaanSetting') < 1
BEGIN
    CREATE TABLE [dbo].[DandaanSetting](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [DandaanUser] [int] NOT NULL,
	    [FormMainWindowState] [int] NOT NULL,
     CONSTRAINT [PK_DandaanSetting] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[DandaanSetting]  WITH CHECK ADD  CONSTRAINT [FK_DandaanSetting_DandaanUser] FOREIGN KEY([DandaanUser])
    REFERENCES [dbo].[DandaanUser] ([Id])

    ALTER TABLE [dbo].[DandaanSetting] CHECK CONSTRAINT [FK_DandaanSetting_DandaanUser] 
END";
            public static DandaanSetting Select(int dandaanUser)
            {
                using (var context = DB.LinqContext)
                    return context.DandaanSettings.Where(s => s.DandaanUser == dandaanUser).FirstOrDefault();
            }

            public static void Insert(DandaanSetting dandaanSetting)
            {
                DB.ExecuteNonQuery($@"insert into 
DandaanSetting({nameof(DandaanUser)}, {nameof(FormMainWindowState)})
values(@{nameof(DandaanUser)}, @{nameof(FormMainWindowState)})",
                    new SqlParameter($"@{nameof(DandaanUser)}", SqlDbType.Int)
                    { Value = dandaanSetting.DandaanUser },
                    new SqlParameter($"@{nameof(FormMainWindowState)}", SqlDbType.Int)
                    { Value = dandaanSetting.FormMainWindowState });
            }
        }

        [Table(Name = nameof(DandaanUser))]
        public class DandaanUser
        {
            [Column]
            public int Id { get; set; }

            [Column]
            public string Name { get; set; }

            //public string Password { get; set; }

            public const string CreateTable = @"
IF (SELECT count(*) FROM information_schema.tables WHERE table_name=N'DandaanUser') < 1
BEGIN
    CREATE TABLE [dbo].[DandaanUser](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [Name] [nvarchar](100) NOT NULL,
     CONSTRAINT [PK_DandaanUser] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END";

            public static void Insert(DandaanUser dandaanUser)
            {
                DB.ExecuteNonQuery(@"insert into DandaanUser(Name) values(@Name)",
                    new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = dandaanUser.Name });
            }
        }
    }
}
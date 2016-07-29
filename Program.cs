using System;
using System.IO;
using System.Windows.Forms;

namespace Dandaan
{
    static class Program
    {
        public const string Title = "مدیریت دندانپزشکی";

        public static readonly string DataDirectory = Application.StartupPath;

        public static int UserId { get; set; }

        public static LocalSettings LocalSettings = new LocalSettings();

        public static void ReadLocalSettings()
        {
            var name = nameof(Dandaan);
            var filePath = DataDirectory + "\\" + name + ".txt";
            if (File.Exists(filePath)) LocalSettings = Serializer.Deserialize<LocalSettings>(filePath, name);
        }

        public static void WriteLocalSettings()
        {
            var name = nameof(Dandaan);
            var filePath = DataDirectory + "\\" + name + ".txt";
            Serializer.Serialize(LocalSettings, filePath, name);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", DataDirectory);
            Application.ThreadExit += Application_ThreadExit;
            Application.ApplicationExit += Application_ApplicationExit;

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.ConnectDB());
        }

        private static void Application_ThreadExit(object sender, EventArgs e)
        {
            ;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // we never want to get here, it crashes the program

            var str = e.ExceptionObject.ToString();

            if (e.ExceptionObject is Exception)
            {
                var ex = (Exception)e.ExceptionObject;

                while (ex.InnerException != null) ex = ex.InnerException;

                str = ex.ToString();
            }

            DB.Log(str);

            MessageBox.Show("برنامه با مشکل مواجه شده است:‏\r\n" + str, Title);
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;

            while (ex.InnerException != null) ex = ex.InnerException;

            DB.Log(ex.ToString());

            new Forms.Message(Title, "برنامه با مشکل مواجه شده است:‏\r\n" + ex).Show();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            ;
        }
    }
}

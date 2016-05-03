using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", DB.Dir);
            Application.ApplicationExit += Application_ApplicationExit;

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.FormDB());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var str = e.ExceptionObject.ToString();

            if (e.ExceptionObject is Exception)
            {
                var ex = (Exception)e.ExceptionObject;

                while (ex.InnerException != null) ex = ex.InnerException;

                str = ex.ToString();
            }

            DB.Log(str);

            MessageBox.Show("برنامه با مشکل مواجه شده است\r\n" + str, "دندانپزشکی");
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;

            while (ex.InnerException != null) ex = ex.InnerException;

            DB.Log(ex.ToString());

            MessageBox.Show("برنامه با مشکل مواجه شده است\r\n" + ex.ToString(), "دندانپزشکی");
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            ;
        }
    }
}

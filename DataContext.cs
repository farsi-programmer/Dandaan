using System;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan
{
    public class DataContext : System.Data.Linq.DataContext
    {
        public Table<Tables.Log> Logs;

        public Table<Tables.User> Users;

        public Table<Tables.Setting> Settings;

        public Table<Tables.Patient> Patients;

        public DataContext(string connection) : base(connection) { }

        public DataContext(SqlConnection connection) : base(connection) { }
    }
}

using System;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan
{
    public class DandaanDataContext : DataContext
    {
        public Table<Tables.Log> Logs;

        public Table<Tables.Setting> Settings;

        public Table<Tables.User> Users;

        public DandaanDataContext(string connection) : base(connection) { }

        public DandaanDataContext(SqlConnection connection) : base(connection) { }
    }
}

using System;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan
{
    public class DandaanLinqContext : DataContext
    {
        public Table<Tables.DandaanLog> DandaanLogs;

        public Table<Tables.DandaanSetting> DandaanSettings;

        public Table<Tables.DandaanUser> DandaanUsers;

        public DandaanLinqContext(string connection) : base(connection) { }

        public DandaanLinqContext(SqlConnection connection) : base(connection) { }
    }
}

using System;
using System.Reflection;
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
    [Table(Name = nameof(Table))]
    public class Table
    {
        [Column]
        [DandaanColumn(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [IX_" + nameof(Table) + @"]
UNIQUE NONCLUSTERED")]
        public int? Id { get; set; }

        [Column]
        [DandaanColumn(Sql = "[nvarchar](100) NOT NULL CONSTRAINT [PK_" + nameof(Table) + @"]
PRIMARY KEY CLUSTERED ([Name] ASC, [Version] DESC)")]
        public string Name { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public int? Version { get; set; }

        [Column]
        [DandaanColumn(Sql = "[nvarchar](1000) NOT NULL")]
        public string SQL { get; set; }

        [Obsolete("For Linq2Sql.", true)]
        public Table() { }

        public Table(bool withDefaultValuesForInsert)
        {
            if (withDefaultValuesForInsert)
            {
                ;
            }
        }
    }
}

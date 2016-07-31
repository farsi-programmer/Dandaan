using System;
using System.ComponentModel;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan.Tables
{
    [Table(Name = nameof(Setting))]
    public class Setting
    {
        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = "[int] NOT NULL CONSTRAINT [PK_" + nameof(Setting) + @"] PRIMARY KEY CLUSTERED
CONSTRAINT [FK_" + nameof(Setting) + "_" + nameof(User) + @"]
FOREIGN KEY REFERENCES [dbo].[" + nameof(User) + "] ([" + nameof(User.Id) + "])")]
        public int UserId { get; set; }

        [Column]
        [DandaanColumn(Sql = "[int] NOT NULL")]
        public int? MainFormWindowState { get; set; }

        [Obsolete("For Linq2Sql.", true)]
        public Setting() { }

        public Setting(bool withDefaultValuesForInsert)
        {
            if (withDefaultValuesForInsert)
            {
                MainFormWindowState = 0;
            }
        }

        /*public static void MigrateTo1()
        {
            ;
        }*/
    }
}

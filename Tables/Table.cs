﻿using System;
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
        [Dandaan(Sql = "[int] IDENTITY NOT NULL CONSTRAINT [IX_" + nameof(Table) + "] UNIQUE NONCLUSTERED")]
        public int? Id { get; set; }

        [Column]
        [Dandaan(Sql = "[nvarchar](100) NOT NULL CONSTRAINT [PK_" + nameof(Table) + @"] PRIMARY KEY CLUSTERED 
([Name] ASC, [Version] DESC)")]
        public string Name { get; set; }

        [Column]
        [Dandaan(Sql = "[int] NOT NULL")]
        public int? Version { get; set; }

        [Column]
        [Dandaan(Sql = "[nvarchar](1000) NOT NULL")]
        public string SQL { get; set; }

        public static Table Select(Table table)
        {
            using (var context = DB.DataContext)
                return context.Tables
                    .Where(t => t.Name == table.Name)
                    .OrderByDescending(t => t.Version)
                    .FirstOrDefault();
        }
    }
}

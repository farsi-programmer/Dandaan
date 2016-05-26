using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan.Tables
{
    class DandaanAttribute : Attribute
    {
        public string Sql { get; set; }

        public string Label { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan
{
    public class DandaanAttribute : Attribute
    {
        public string Sql { get; set; }

        public string Label { get; set; }

        public bool EnableAdd { get; set; }

        public bool EnableDelete { get; set; }

        public bool EnableEdit { get; set; }

        public bool EnableSearch { get; set; }
    }
}

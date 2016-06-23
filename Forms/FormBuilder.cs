using System;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dandaan.Forms
{
    public partial class FormBuilder : Form
    {
        public FormBuilder()
        {
            InitializeComponent();
        }

        bool working = true;

        public FormBuilder(string path, string name) : this()
        {
            textBox1.AppendText("در حال ساختن فرم، لطفا اندکی صبر کنید...\r\n");

            Common.Thread(() =>
            {
                try
                {
                    var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

                    var tree = CSharpSyntaxTree.ParseText($@"
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Linq;
using System.Data.SqlClient;

namespace Dandaan.Tables
{{
    [Table(Name = nameof(UserTable32))]
    [Dandaan(Label = ""فیلدها"", EnableAdd = true, EnableDelete = true, EnableSearch = true, EnableEdit = true)]
    public class UserTable32
    {{
        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = ""[int] NOT NULL CONSTRAINT[PK_"" + nameof(UserTable32) + @""] PRIMARY KEY CLUSTERED
CONSTRAINT[FK_"" + nameof(UserTable32) + ""_"" + nameof(User) + @""]
FOREIGN KEY REFERENCES[dbo].["" + nameof(User) + ""](["" + nameof(User.Id) + ""])"")]
        public int UserId {{ get; set; }}
    }}
}}

namespace Dandaan
{{
    public class DataContext32 : {DB.DataContext.GetType().Name}
    {{
        public Table<Tables.UserTable32> UserTable32s;

        public DataContext32(string connection) : base(connection) {{ }}

        public DataContext32(SqlConnection connection) : base(connection) {{ }}
    }}
}}");
                    var references = new List<MetadataReference>()
                {
                    //MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    //MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(GetType().Assembly.Location),
                    MetadataReference.CreateFromFile(DB.DataContextType.Assembly.Location),
                };

                    references.AddRange(GetType().Assembly.GetReferencedAssemblies().Select((a) =>
                    MetadataReference.CreateFromFile(System.Reflection.Assembly.Load(a).Location)).ToArray());

                    var compiler = CSharpCompilation.Create(name, new[] { tree }, references, options);

                    using (var file = File.OpenWrite(path))
                    {
                        var result = compiler.Emit(file);

                        if (!result.Success)
                        {
                            file.Close();
                            File.Delete(path);

                            var failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);

                            foreach (Diagnostic diagnostic in failures)
                                throw new Exception(diagnostic.ToString());
                        }
                    }
                }
                finally { Invoke(() => working = false); }
            }).Start();
        }

        private void FormBuilder_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = working;
        }
    }
}

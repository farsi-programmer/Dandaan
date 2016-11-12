using System;
using System.Reflection;
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

        public FormBuilder(string path, string name, Tables.UserTable table) : this()
        {
            textBox1.AppendText("در حال ساختن فرم، لطفا اندکی صبر کنید...\r\n");

            Common.Thread(() =>
            {
                try
                {
                    var tableName = $"{nameof(Tables.UserTable)}{table.Id}";

                    var sb = new StringBuilder($@"
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
    [Table(Name = nameof({tableName}))]
    [Dandaan(Label = ""{table.Label}""");

                    {
                        Func<Tables.YesOrNo?, bool> f = _ => _ == Tables.YesOrNo.بله;

                        if (f(table.EnableSearch)) sb.Append(@", EnableSearch = true");

                        if (f(table.EnableEdit)) sb.Append(@", EnableEdit = true");

                        if (f(table.EnableAdd)) sb.Append(@", EnableAdd = true");

                        if (f(table.EnableDelete)) sb.Append(@", EnableDelete = true");
                    }

                    sb.Append($@")]
    public class {tableName}
    {{
        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = ""[int] IDENTITY NOT NULL CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED"",
            Label = ""شماره"")]
        public int? Id {{ get; set; }}
");

                    {
                        var columnType = "";
                        var sqlColumnType = "";
                        var foreignTableId = 0;

                        foreach (var c in DB.DataContext.Columns.Where(_ => _.UserTableId == table.Id))
                        {
                            sb.Append($@"
        [Column]
        [DandaanColumn(Sql = @""");

                            columnType = c.Type.ToString();

                            if (columnType.Contains("تاریخ_شمسی_با_دقت_دقیقه") || columnType.Contains("تاریخ_میلادی_با_دقت_دقیقه"))
                                sqlColumnType = "[" + nameof(SqlDbType.SmallDateTime).ToLower() + "]";
                            else if (columnType.Contains("تاریخ_شمسی_با_دقت_ثانیه") || columnType.Contains("تاریخ_میلادی_با_دقت_ثانیه"))
                                sqlColumnType = "[" + nameof(SqlDbType.DateTime).ToLower() + "]";
                            else if (columnType.Contains("عدد_اعشاری"))
                                sqlColumnType = "[" + nameof(SqlDbType.Real).ToLower() + "]";
                            else if (columnType.Contains("متن"))
                                sqlColumnType = "[" + nameof(SqlDbType.NVarChar).ToLower() + "]" + "(4000)";
                            else if (columnType.Contains("فایل"))
                                sqlColumnType = "[" + nameof(SqlDbType.VarBinary).ToLower() + "]" + "(max)";
                            else
                                sqlColumnType = "[" + nameof(SqlDbType.Int).ToLower() + "]";

                            if (c.ColumnId != null) sb.Append("[int]");
                            else sb.Append(sqlColumnType);

                            if (c.NotNull == Tables.YesOrNo.بله || columnType.Contains("بدون_تکرار"))
                                sb.Append(" NOT NULL");

                            if (columnType.Contains("بدون_تکرار"))
                                sb.Append($@" CONSTRAINT [IX_{tableName}_{c.Id}] UNIQUE NONCLUSTERED");

                            if (columnType.StartsWith("تاریخ")) columnType = "DateTime";
                            else if (columnType.StartsWith("متن")) columnType = "string";
                            else if (columnType.StartsWith("فایل")) columnType = "byte[]";
                            else if (columnType.StartsWith("عدد_اعشاری")) columnType = "float";
                            else columnType = "int";

                            if (c.ColumnId != null)
                            {
                                foreignTableId = DB.DataContext.Columns
                                .Where(_ => _.Id == c.ColumnId.Value).First().UserTableId.Value;

                                sb.Append($@" CONSTRAINT [FK_{tableName}_UserTable{foreignTableId}_{c.Id}]
FOREIGN KEY REFERENCES [dbo].[UserTable{foreignTableId}] ([Id])");

                                columnType = "int";
                            }

                            sb.Append($@""",
                Label = ""{c.Label}"",
                ColumnType = {nameof(Tables)}.{nameof(Tables.ColumnType)}.{c.Type})]
                    public {columnType}");

                            if (columnType != "string" && columnType != "byte[]") sb.Append("?");

                            sb.Append($@" Column{c.Id} {{ get; set; }}{(c.Type.ToString().Contains("مقدار_پیش_فرض_اکنون") ? " = DateTime.Now;" : "")}");

                            if (c.Type.ToString().Contains("فایل"))
                                sb.Append($@"
        [Column]
        [DandaanColumn(Sql = @""[nvarchar](1000) NOT NULL"",
            Label = ""نام فایل"")]
        public string Column{c.Id}FileName {{ get; set; }}");
                        }
                    }

                    {
                        var contextName = $"DataContext{table.Id}";

                        sb.Append($@"
    }}
}}

namespace Dandaan
{{
    public class {contextName} : {DB.DataContextType.Name}
    {{
        public Table<Tables.{tableName}> {tableName}s;

        public {contextName}(string connection) : base(connection) {{ }}

        public {contextName}(SqlConnection connection) : base(connection) {{ }}
    }}
}}");
                    }

                    {
                        var tree = CSharpSyntaxTree.ParseText(sb.ToString());

                        var references = new List<MetadataReference>()
                        {
                            //MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                            MetadataReference.CreateFromFile(GetType().Assembly.Location),
                        };

                        if (DB.DataContextType != typeof(DataContext))
                        {
                            references.Add(MetadataReference.CreateFromFile(DB.DataContextType.Assembly.Location));

                            var baseType = DB.DataContextType.BaseType;

                            while (baseType != typeof(DataContext))
                            {
                                references.Add(MetadataReference.CreateFromFile(baseType.Assembly.Location));
                                baseType = baseType.BaseType;
                            }
                        }

                        references.AddRange(GetType().Assembly.GetReferencedAssemblies().Select(_ =>
                        MetadataReference.CreateFromFile(Assembly.Load(_).Location)).ToArray());

                        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

                        var compiler = CSharpCompilation.Create(name, new[] { tree }, references, options);

                        using (var file = File.OpenWrite(path))
                        {
                            var result = compiler.Emit(file);
                            file.Close();

                            if (!result.Success)
                            {
                                File.Delete(path);

                                result.Diagnostics.Where(_ => _.IsWarningAsError || _.Severity == DiagnosticSeverity.Error)
                                .Iter(_ => { throw new Exception(_ + "" + sb); });
                            }
                            else
                            {
                                SQL.Insert(new Tables.UserTableAssembly(true)
                                { UserTableId = table.Id, Assembly = File.ReadAllBytes(path) });

                                DB.DataContextType = Assemblies.Load(path).GetType($"{typeof(DataContext).FullName}{table.Id}");
                            }
                        }
                    }
                }
                finally { Invoke(() => { working = false; Close(); }); }
            }).Start();

            ShowDialog();
        }

        private void FormBuilder_FormClosing(object sender, FormClosingEventArgs e) => e.Cancel = working;
    }
}

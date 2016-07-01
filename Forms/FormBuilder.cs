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

        public FormBuilder(string path, string name, Tables.UserTable table) : this()
        {
            textBox1.AppendText("در حال ساختن فرم، لطفا اندکی صبر کنید...\r\n");

            Common.Thread(() =>
            {
                try
                {
                    var columns = DB.DataContext.Columns.Where(_ => _.UserTableId == table.Id);

                    var tableName = "UserTable" + table.Id;

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

                    if (table.EnableSearch == Tables.YesOrNo.بله) sb.Append(@", EnableSearch = true");
                    if (table.EnableEdit == Tables.YesOrNo.بله) sb.Append(@", EnableEdit = true");
                    if (table.EnableAdd == Tables.YesOrNo.بله) sb.Append(@", EnableAdd = true");
                    if (table.EnableDelete == Tables.YesOrNo.بله) sb.Append(@", EnableDelete = true");

                    sb.Append($@")]
    public class {tableName}
    {{
        [Column(IsPrimaryKey = true)]
        [DandaanColumn(Sql = ""[int] IDENTITY NOT NULL CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED"",
            Label = ""شماره"")]
        public int? Id {{ get; set; }}
");

                    var columnType = "";
                    var sqlColumnType = "";
                    var foreignTableId = 0;

                    foreach (var column in columns)
                    {
                        sb.Append($@"
        [Column]
        [DandaanColumn(Sql = @""");

                        columnType = column.Type.ToString();

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

                        if (column.ReferenceColumnId != null) sb.Append("[int]");
                        else sb.Append(sqlColumnType);

                        if (column.NotNull == Tables.YesOrNo.بله || columnType.Contains("بدون_تکرار"))
                            sb.Append(" NOT NULL");

                        if (columnType.Contains("بدون_تکرار"))
                            sb.Append($@" CONSTRAINT [IX_{tableName}_{column.Id}] UNIQUE NONCLUSTERED");

                        if (columnType.StartsWith("تاریخ")) columnType = "DateTime";
                        else if (columnType.StartsWith("متن")) columnType = "string";
                        else if (columnType.StartsWith("فایل")) columnType = "byte[]";
                        else if (columnType.StartsWith("عدد_اعشاری")) columnType = "float";
                        else columnType = "int";

                        if (column.ReferenceColumnId != null)
                        {
                            foreignTableId = DB.DataContext.Columns
                            .Where(_ => _.Id == column.ReferenceColumnId.Value).First().UserTableId.Value;

                            sb.Append($@" CONSTRAINT [FK_{tableName}_UserTable{foreignTableId}_{column.Id}]
FOREIGN KEY REFERENCES [dbo].[UserTable{foreignTableId}] ([Id])");

                            columnType = "int";
                        }

                        sb.Append($@""",
                Label = ""{column.Label}""{((column.Type.ToString().Contains("متن_فارسی_چند_خطی")
                || column.Type.ToString().Contains("متن_انگلیسی_چند_خطی")) ? @",
                " + nameof(DandaanColumnAttribute.Multiline) + " = true" : "")})]
                    public {columnType}");

                        if (columnType != "string" && columnType != "byte[]") sb.Append("?");

                        sb.Append($@" Column{column.Id} {{ get; set; }}{(column.Type.ToString().Contains("مقدار_پیش_فرض_اکنون") ? " = DateTime.Now;" : "")}");

                        if (column.Type.ToString().Contains("فایل"))
                            sb.Append($@"
        [Column]
        [DandaanColumn(Sql = @""[nvarchar](1000) NOT NULL"",
            Label = ""نام فایل"")]
        public string Column{column.Id}FileName {{ get; set; }}");
                    }

                    sb.Append($@"
    }}
}}

namespace Dandaan
{{
    public class DataContext{table.Id} : {DB.DataContextType.Name}
    {{
        public Table<Tables.UserTable{table.Id}> UserTable{table.Id}s;

        public DataContext{table.Id}(string connection) : base(connection) {{ }}

        public DataContext{table.Id}(SqlConnection connection) : base(connection) {{ }}
    }}
}}");

                    var tree = CSharpSyntaxTree.ParseText(sb.ToString());

                    var references = new List<MetadataReference>()
                    {
                        //MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                        //MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
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

                    references.AddRange(GetType().Assembly.GetReferencedAssemblies().Select((a) =>
                    MetadataReference.CreateFromFile(System.Reflection.Assembly.Load(a).Location)).ToArray());

                    var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

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
                                throw new Exception(diagnostic + "" + sb);
                        }
                        else
                        {
                            file.Close();
                            SQL.Insert(new Tables.UserTableAssembly()
                            { UserTableId = table.Id, Assembly = File.ReadAllBytes(path) });

                            var assembly = Reflection.LoadAssembly(path);
                            DB.DataContextType = assembly.GetType($"{nameof(Dandaan)}.{nameof(DataContext)}{table.Id}");
                        }
                    }
                }
                finally { Invoke(() => { working = false; Close(); }); }
            }).Start();

            ShowDialog();
        }

        private void FormBuilder_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = working;
        }
    }
}

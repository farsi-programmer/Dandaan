using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan
{
    class Reflection
    {
        public static ColumnAttribute GetColumnAttribute(MemberInfo m) => 
            m.GetCustomAttributes<ColumnAttribute>().First();        

        public static DandaanAttribute GetDandaanAttribute(MemberInfo m) =>        
            m.GetCustomAttributes<DandaanAttribute>().First();

        public static DandaanColumnAttribute GetDandaanColumnAttribute(MemberInfo m) =>
            (DandaanColumnAttribute)GetDandaanAttribute(m);

        public static DandaanAttribute GetDandaanAttribute(Type t) =>
            t.GetCustomAttributes<DandaanAttribute>().First();

        public static DandaanColumnAttribute GetDandaanColumnAttribute(Type t) =>
            (DandaanColumnAttribute)GetDandaanAttribute(t);

        public static string GetDescriptionAttribute(MemberInfo m, bool shouldHave = true)
        {
            var attributes = (DescriptionAttribute[])m.GetCustomAttributes<DescriptionAttribute>();

            return GetFirstDescription(attributes, shouldHave);
        }

        public static string GetDescriptionAttribute(Type t, bool shouldHave = true)
        {
            var attributes = (DescriptionAttribute[])t.GetCustomAttributes<DescriptionAttribute>();

            return GetFirstDescription(attributes, shouldHave);
        }

        private static string GetFirstDescription(DescriptionAttribute[] attributes, bool shouldHave)
        {
            if (shouldHave) return attributes[0].Description;
            else if (attributes != null && attributes.Length > 0) return attributes[0].Description;

            return "";
        }

        internal static Type GetType(string name) => typeof(Common).Assembly.GetType(name);
    }

    class Assemblies
    {
        static Dictionary<string, Assembly> assemblies;

        public static Assembly Load(string path)
        {
            if (assemblies == null) assemblies = new Dictionary<string, Assembly>();

            if (assemblies.ContainsKey(path)) return assemblies[path];
            else
            {
                var assembly = Assembly.LoadFile(path);
                assemblies.Add(path, assembly);

                var dir = Path.GetDirectoryName(assembly.Location);
                var u = typeof(Tables.UserTable).FullName;

                foreach (var reference in assembly.GetReferencedAssemblies())
                    if (reference.Name.StartsWith(u))
                    {
                        var p = $"{dir}\\{reference.Name}.dll";

                        FromDbToFile(int.Parse(reference.Name.Substring(u.Length)), p);

                        Load(p);
                    }

                return assembly;
            }
        }

        public static bool FromDbToFile(int id, string path)
        {
            if (!File.Exists(path) || new FileInfo(path).Length == 0)
            {
                var result = DB.DataContext.UserTableAssemblys.Where(_ => _.UserTableId == id).FirstOrDefault();

                if (result == null) return false;
                else File.WriteAllBytes(path, result.Assembly);
            }

            return true;
        }
    }
}

using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan
{
    class Reflection
    {
        public static ColumnAttribute GetColumnAttribute(MemberInfo m)
        {
            var attributes = (ColumnAttribute[])m.GetCustomAttributes<ColumnAttribute>();

            return attributes[0];
        }

        public static DandaanAttribute GetDandaanAttribute(MemberInfo m)
        {
            var attributes = (DandaanAttribute[])m.GetCustomAttributes<DandaanAttribute>();

            return attributes[0];
        }

        public static DandaanColumnAttribute GetDandaanColumnAttribute(MemberInfo m)
        {
            return (DandaanColumnAttribute)GetDandaanAttribute(m);
        }

        public static DandaanAttribute GetDandaanAttribute(Type t)
        {
            var attributes = (DandaanAttribute[])t.GetCustomAttributes<DandaanAttribute>();

            if (attributes[0] is DandaanColumnAttribute)
                return (DandaanColumnAttribute)attributes[0];
            else
                return attributes[0];
        }

        public static DandaanColumnAttribute GetDandaanColumnAttribute(Type t)
        {
            return (DandaanColumnAttribute)GetDandaanAttribute(t);
        }

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
            if (shouldHave)
                return attributes[0].Description;
            else if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;

            return "";
        }

        internal static Type GetType(string name)
        {
            Assembly assembly = typeof(Common).Assembly;

            return assembly.GetType(name);
        }

        static Dictionary<string, Assembly> assemblies;

        public static Assembly LoadAssembly(string path)
        {
            if (assemblies == null) assemblies = new Dictionary<string, Assembly>();

            Assembly assembly;

            if (assemblies.ContainsKey(path)) assembly = assemblies[path];
            else
            {
                assembly = Assembly.LoadFile(path);
                assemblies.Add(path, assembly);
            }

            return assembly;
        }
    }
}

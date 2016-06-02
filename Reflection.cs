using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan
{
    class Reflection
    {
        public static DandaanAttribute GetDandaanAttribute(MemberInfo m)
        {
            var attributes = (DandaanAttribute[])m.GetCustomAttributes<DandaanAttribute>();

            return attributes[0];
        }

        public static DandaanAttribute GetDandaanAttribute(Type t)
        {
            var attributes = (DandaanAttribute[])t.GetCustomAttributes<DandaanAttribute>();

            return attributes[0];
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
    }
}

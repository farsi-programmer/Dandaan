using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandaan
{
    class Serializer
    {
        public static T Deserialize<T>(string filePath, string mutexName)
        {
            Mutex mutex = null;

            try
            {
                mutex = new Mutex(false, "Global\\" + mutexName);
                mutex.WaitOne();

                return Deserialize<T>(File.ReadAllText(filePath));
            }
            finally
            {
                mutex?.ReleaseMutex();
            }
        }

        public static void Serialize(object obj, string filePath, string mutexName)
        {
            Mutex mutex = null;

            try
            {
                mutex = new Mutex(false, "Global\\" + mutexName);
                mutex.WaitOne();

                File.WriteAllText(filePath, Serialize(obj));
            }
            finally
            {
                mutex?.ReleaseMutex();
            }
        }

        public static T Deserialize<T>(string rawXml)
        {
            using (var reader = XmlReader.Create(new StringReader(rawXml)))
            {                
                DataContractSerializer formatter0 = new DataContractSerializer(typeof(T));
                return (T)formatter0.ReadObject(reader);
            }
        }

        public static string Serialize(object obj)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(writer, obj);
            }
            return sb.ToString();
        }
    }
}

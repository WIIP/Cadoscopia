using System.IO;
using System.Xml.Serialization;

namespace Cadoscopia.IO
{
    public class GenericXmlSerializer<T>
    {
        #region Méthodes

        public T Read(string fileName)
        {
            var serializer = new XmlSerializer(typeof (T));
            T obj;
            using (var sr = new StreamReader(fileName))
            {
                obj = (T) serializer.Deserialize(sr);
            }
            return obj;
        }

        public void Write(T obj, string fileName)
        {
            var serializer = new XmlSerializer(typeof (T));
            using (var sw = new StreamWriter(fileName))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(sw, obj, ns);
            }
        }

        #endregion
    }
}
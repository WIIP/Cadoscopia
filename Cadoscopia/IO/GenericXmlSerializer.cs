// MIT License

// Copyright(c) 2016 Cadoscopia http://cadoscopia.com

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Cadoscopia.IO
{
    public class GenericXmlSerializer<T>
    {
        #region Methods

        public T ReadFromString([NotNull] string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            using (var sr = new StringReader(s))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T) serializer.Deserialize(sr);
            }
        }

        public T Read(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(fileName));

            var serializer = new XmlSerializer(typeof(T));
            T obj;
            using (var sr = new StreamReader(fileName))
            {
                obj = (T)serializer.Deserialize(sr);
            }
            return obj;
        }

        public void Write([NotNull] T obj, [NotNull] string fileName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(fileName));

            var serializer = new XmlSerializer(typeof(T));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (var sw = new StreamWriter(fileName))
            {
                serializer.Serialize(sw, obj, ns);
            }
        }

        public string WriteToString([NotNull] T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, obj, ns);
            }
            return sb.ToString();
        }

        #endregion
    }
}
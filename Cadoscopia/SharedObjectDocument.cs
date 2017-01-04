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

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Cadoscopia.Parametric.SketchServices;

namespace Cadoscopia
{
    public class SharedObjectDocument : Document
    {
        #region Constants

        public const string EXTENSION = ".cso";

        #endregion

        #region Properties

        public List<Feature> Features { get; } = new List<Feature>();

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement(nameof(SharedObjectDocument));
            if (!isEmptyElement)
            {
                reader.ReadStartElement(nameof(Features));
                while (reader.IsStartElement())
                {
                    if (reader.Name == nameof(Sketch))
                    {
                        var xs = new XmlSerializer(typeof(Sketch));
                        Features.Add((Sketch) xs.Deserialize(reader));
                    }
                }
                reader.ReadEndElement();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Features));
            int id = 0;
            foreach (Feature feature in Features)
            {
                writer.WriteStartElement(feature.GetType().Name);
                writer.WriteAttributeString("Id", id.ToString());
                id++;
                feature.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
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
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Cadoscopia.Parametric.SketchServices.Entities
{
    public class Point : Entity, IXmlSerializable, IObjectWithReferences
    {
        #region Properties
        
        public override Geometry.Entity Geometry => new Geometry.Point(X.Value, Y.Value);

        public Parameter X => parameters[0];

        public Parameter Y => parameters[1];

        #endregion

        #region Constructors

        internal Point()
        {
            parameters.Add(new Parameter());
            parameters.Add(new Parameter());
        }

        internal Point(double x, double y)
        {
            parameters.Add(new Parameter(x));
            parameters.Add(new Parameter(y));
        }

        #endregion

        #region Methods

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();
            X.ReadXml(reader);
            Y.ReadXml(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(X));
            X.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(Y));
            Y.WriteXml(writer);
            writer.WriteEndElement();
        }

        public void WriteXmlWithReferences(XmlWriter writer)
        {
            if (Sketch == null)
                throw new InvalidOperationException("This method cannot be called if the point is not associated to a sketch.");

            writer.WriteStartElement(nameof(X));
            writer.WriteAttributeString("Ref", Sketch.Parameters.ToList().IndexOf(X).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(Y));
            writer.WriteAttributeString("Ref", Sketch.Parameters.ToList().IndexOf(Y).ToString());
            writer.WriteEndElement();
        }

        public void ReadXmlWithReferences(XmlReader reader)
        {
            if (Sketch == null)
                throw new InvalidOperationException("This method cannot be called if the point is not associated to a sketch.");

            reader.MoveToContent();
            reader.ReadStartElement(nameof(Point));
            ReadParameterReference(reader, nameof(X), 0);
            ReadParameterReference(reader, nameof(Y), 1);
            reader.ReadEndElement();
        }

        void ReadParameterReference(XmlReader reader, string parameterName, int parameterIndex)
        {
            string refAsString = reader.GetAttribute("Ref");
            reader.ReadStartElement(parameterName);
            if (refAsString == null) return;
            int reference = int.Parse(refAsString);
            Debug.Assert(Sketch != null, "Sketch != null");
            parameters[parameterIndex] = Sketch.ParameterReferences.ElementAt(reference);
        }

        #endregion
    }
}
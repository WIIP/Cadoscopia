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
using JetBrains.Annotations;

namespace Cadoscopia.Parametric.SketchServices.Entities
{
    public class Line : Entity, IObjectWithReferences
    {
        #region Properties

        public Point End { get; set; }

        public override Geometry.Entity Geometry => new Geometry.Line(new Geometry.Point(Start.X.Value, Start.Y.Value),
            new Geometry.Point(End.X.Value, End.Y.Value));

        public Point Start { get; set; }

        #endregion

        #region Constructors

        public Line([NotNull] Point start, [NotNull] Point end)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));
            if (start.Sketch != end.Sketch)
                throw new ArgumentException("The two points must belong to the same sketch.");

            Start = start;
            End = end;

            start.Sketch?.Entities.Add(this);

            Sketch = start.Sketch;
        }

        /// <summary>
        /// For XML deserialization.
        /// </summary>
        internal Line()
        {
        }

        public void WriteXmlWithReferences(XmlWriter writer)
        {
            if (Sketch == null)
                throw new InvalidOperationException("This method cannot be called if the line is not associated to a sketch.");

            writer.WriteStartElement(nameof(Start));
            writer.WriteAttributeString("Ref", Sketch.Entities.IndexOf(Start).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(End));
            writer.WriteAttributeString("Ref", Sketch.Entities.IndexOf(End).ToString());
            writer.WriteEndElement();
        }

        public void ReadXmlWithReferences(XmlReader reader)
        {
            if (Sketch == null)
                throw new InvalidOperationException(
                    "This method cannot be called if the line is not associated to a sketch.");

            reader.MoveToContent();
            reader.ReadStartElement(nameof(Line));
            Start = ReadPoint(reader, nameof(Start));
            End = ReadPoint(reader, nameof(End));
            reader.ReadEndElement();
        }

        Point ReadPoint(XmlReader reader, string pointName)
        {
            string refAsString = reader.GetAttribute("Ref");
            reader.ReadStartElement(pointName);
            if (refAsString == null) return null;
            int reference = int.Parse(refAsString);
            Debug.Assert(Sketch != null, "Sketch != null");
            return (Point) Sketch.Entities.ElementAt(reference);
        }

        #endregion
    }
}
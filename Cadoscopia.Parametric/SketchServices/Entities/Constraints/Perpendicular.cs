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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;

namespace Cadoscopia.Parametric.SketchServices.Entities.Constraints
{
    public class Perpendicular : Constraint, IObjectWithReferences
    {
        #region Fields

        [NotNull]
        public Line Line1 { get; private set; }

        [NotNull]
        public Line Line2 { get; private set; }

        #endregion

        #region Properties

        public override double Error
        {
            get
            {
                double dotProduct =
                    ((Geometry.Line) Line1.Geometry).Direction.DotProduct(((Geometry.Line) Line2.Geometry).Direction);
                return dotProduct * dotProduct;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// For XML serialization.
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once NotNullMemberIsNotInitialized
        internal Perpendicular()
        {
        }

        public Perpendicular([NotNull] Line line1, [NotNull] Line line2)
        {
            if (line1 == null) throw new ArgumentNullException(nameof(line1));
            if (line2 == null) throw new ArgumentNullException(nameof(line2));
            if (line1.Sketch != line2.Sketch)
                throw new ArgumentException("The two lines must belong to the same sketch.");

            Line1 = line1;
            Line2 = line2;

            line1.Sketch?.Entities.Add(this);
            Sketch = line1.Sketch;

            parameters.Add(line1.Start.X);
            parameters.Add(line1.Start.Y);
            parameters.Add(line1.End.X);
            parameters.Add(line1.End.Y);

            parameters.Add(line2.Start.X);
            parameters.Add(line2.Start.Y);
            parameters.Add(line2.End.X);
            parameters.Add(line2.End.Y);
        }

        #endregion

        #region Methods

        public static bool IsApplicable(IEnumerable<Entity> entities)
        {
            return entities.OfType<Line>().Count() == 2;
        }

        public override Geometry.Entity Geometry { get; }

        public void WriteXmlWithReferences(XmlWriter writer)
        {
            if (Sketch == null)
                throw new InvalidOperationException("This method cannot be called if the perpendicular constraint is not associated to a sketch.");

            writer.WriteStartElement(nameof(Line1));
            writer.WriteAttributeString("Ref", Sketch.Entities.IndexOf(Line1).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(Line2));
            writer.WriteAttributeString("Ref", Sketch.Entities.IndexOf(Line2).ToString());
            writer.WriteEndElement();
        }

        public void ReadXmlWithReferences(XmlReader reader)
        {
            if (Sketch == null)
                throw new InvalidOperationException(
                    "This method cannot be called if the perpendicular constraint  is not associated to a sketch.");

            reader.MoveToContent();
            reader.ReadStartElement(nameof(Perpendicular));
            Line1 = ReadLine(reader, nameof(Line1));
            Line2 = ReadLine(reader, nameof(Line2));
            reader.ReadEndElement();
        }

        Line ReadLine(XmlReader reader, string lineName)
        {
            string refAsString = reader.GetAttribute("Ref");
            reader.ReadStartElement(lineName);
            if (refAsString == null) return null;
            int reference = int.Parse(refAsString);
            Debug.Assert(Sketch != null, "Sketch != null");
            return (Line)Sketch.Entities.ElementAt(reference);
        }

        #endregion
    }
}
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using Cadoscopia.Parametric.SketchServices.Entities;
using Cadoscopia.Parametric.SketchServices.Entities.Constraints;
using JetBrains.Annotations;

namespace Cadoscopia.Parametric.SketchServices
{
    public class Sketch : Feature
    {
        #region Properties

        public ObservableCollection<Entity> Entities { get; } = new ObservableCollection<Entity>();

        /// <summary>
        /// Used internally for XML deserialization of points.
        /// </summary>
        internal List<Parameter> ParameterReferences { get; private set; }

        public IEnumerable<Parameter> Parameters
        {
            get { return Entities.SelectMany(e => e.Parameters).Distinct(); }
        }

        #endregion

        #region Methods

        public Point AddPoint()
        {
            var point = new Point {Sketch = this};
            Entities.Add(point);
            return point;
        }

        public Point AddPoint(double x, double y)
        {
            var point = new Point(x, y) {Sketch = this};
            Entities.Add(point);
            return point;
        }

        /// <summary>
        /// Returns the entity which is near the passed point or null if there are no element.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        [CanBeNull]
        T EntityAtPoint<T>(Point point, double radius) where T : Entity
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entity entity = Entities[i];
                if (!(entity is T)) continue;

                double d = entity.Geometry.GetDistanceTo((Geometry.Point) point.Geometry);
                if (d > 10) continue;
                return (T) entity;
            }
            return null;
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Name = reader.GetAttribute(nameof(Name))?.Trim();
            bool sketchElementIsEmpty = reader.IsEmptyElement;
            reader.ReadStartElement(nameof(Sketch));
            if (sketchElementIsEmpty) return;
            
            ParameterReferences = new List<Parameter>();
            bool parametersElementIsEmpty = reader.IsEmptyElement;
            reader.ReadStartElement(nameof(Parameters));
            if (!parametersElementIsEmpty)
            {
                while (reader.Name == "Parameter")
                {
                    var parameter = new Parameter();
                    parameter.ReadXml(reader);
                    ParameterReferences.Add(parameter);
                }
                reader.ReadEndElement();
            }

            bool entitiesElementIsEmpty = reader.IsEmptyElement;
            reader.ReadStartElement(nameof(Entities));
            if (entitiesElementIsEmpty) return;
            while (reader.Name != nameof(Entities))
            {
                switch (reader.Name)
                {
                    case nameof(Point):
                        var point = new Point {Sketch = this};
                        Entities.Add(point);
                        point.ReadXmlWithReferences(reader);
                        break;

                    case nameof(Line):
                        var line = new Line {Sketch = this};
                        Entities.Add(line);
                        line.ReadXmlWithReferences(reader);
                        break;

                    case nameof(Perpendicular):
                        var perpendicular = new Perpendicular {Sketch = this};
                        Entities.Add(perpendicular);
                        perpendicular.ReadXmlWithReferences(reader);
                        break;
                }
            }
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(Name))
                writer.WriteAttributeString(nameof(Name), Name);

            if (Parameters.Any())
            {
                writer.WriteStartElement("Parameters");
                IEnumerable<Parameter> parameters = Parameters;
                foreach (Parameter parameter in parameters)
                {
                    writer.WriteStartElement(nameof(Parameter));
                    parameter.WriteXml(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            // ReSharper disable once InvertIf
            if (Entities.Count > 0)
            {
                writer.WriteStartElement(nameof(Entities));
                foreach (Entity entity in Entities)
                {
                    writer.WriteStartElement(entity.GetType().Name);
                    var owr = entity as IObjectWithReferences;
                    owr?.WriteXmlWithReferences(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
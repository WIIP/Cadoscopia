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
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using Cadoscopia.Parametric.SketchServices.Entities;
using Cadoscopia.Parametric.SketchServices.Entities.Constraints;
using JetBrains.Annotations;

namespace Cadoscopia.Parametric.SketchServices
{
    [Serializable]
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
            var point = new Point {Parent = this};
            Entities.Add(point);
            return point;
        }

        public Point AddPoint(double x, double y)
        {
            var point = new Point(x, y) {Parent = this};
            Entities.Add(point);
            return point;
        }

        public int CountReferences([NotNull] Point point)
        {
            if (point == null) throw new ArgumentNullException(nameof(point));

            int count = 1;
            foreach (Entity entity in Entities)
            {
                var line = entity as Line;
                if (line == null) continue;
                if (line.Start == point) count++;
                if (line.End == point) count++;
            }
            return count;
        }

        /// <summary>
        /// Returns the entities which is near the passed point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public IEnumerable<T> EntitiesAtPoint<T>(Geometry.Point point, double radius) where T : Entity
        {
            foreach (T entity in Entities.OfType<T>())
            {
                double d = entity.Geometry.GetDistanceTo(point);
                if (d > radius) continue;
                yield return (T) entity;
            }
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
                        var point = new Point {Parent = this};
                        Entities.Add(point);
                        point.ReadXmlWithReferences(reader);
                        break;

                    case nameof(Line):
                        var line = new Line {Parent = this};
                        Entities.Add(line);
                        line.ReadXmlWithReferences(reader);
                        break;

                    case nameof(Perpendicular):
                        var perpendicular = new Perpendicular {Parent = this};
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
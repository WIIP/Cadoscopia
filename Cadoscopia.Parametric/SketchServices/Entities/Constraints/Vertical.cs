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
using System.Linq;
using Cadoscopia.Core;
using Cadoscopia.Geometry;
using Cadoscopia.Parametric.Properties;
using JetBrains.Annotations;

namespace Cadoscopia.Parametric.SketchServices.Entities.Constraints
{
    public class Vertical : Constraint
    {
        public override Geometry.Entity Geometry
        {
            get
            {
                var line = geometricEntities[0] as Line;
                if (line != null) return GetSymbolPosition(line);

                throw new NotImplementedException();

                //var point1 = (Geometry.Point) geometricEntities[0].Geometry;
                //var point2 = (Geometry.Point) geometricEntities[1].Geometry;
                //Geometry.Point midPoint = (point1 + point2.AsVector()) / 2.0;
                //Vector perp = point1.GetVector(point2).GetPerpendicular().Normalize();
                //Geometry.Point basePoint = midPoint + perp * 10;
            }
        }

        public override void Move(Vector vector)
        {
        }

        /// <summary>
        /// Create a vertical constraint.
        /// </summary>
        /// <param name="line"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UserException"></exception>
        public Vertical([NotNull] Line line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

            if (line.Constraints.OfType<Vertical>().Any())
                throw new UserException(Resources.ThereIsAlreadyAVerticalConstraintOnThisLine);

            if (line.Constraints.OfType<Horizontal>().Any())
                throw new UserException(Resources.ThereIsAlreadyAnHorizontalConstraintOnThisLine);

            geometricEntities.Add(line);

            line.End.X.Value = (line.Start.X.Value + line.End.X.Value) / 2.0;
            line.Start.X = line.End.X;
        }

        public Vertical(Point point1, Point point2)
        {
        }
    }
}
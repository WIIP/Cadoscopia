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
using System.Linq;
using Cadoscopia.Geometry;
using JetBrains.Annotations;

namespace Cadoscopia.Parametric.SketchServices.Entities.Constraints
{
    public class Parallel : Constraint
    {
        #region Properties

        public override double Error
        {
            get
            {
                Vector dir1 = ((Geometry.Line) Line1.Geometry).Direction;
                Vector dir2 = ((Geometry.Line) Line2.Geometry).Direction;
                double crossProduct = dir1.X * dir2.Y - dir2.X * dir1.Y;
                return crossProduct * crossProduct;
            }
        }

        // TODO Should have an offset and should be a point cloud.
        public override Geometry.Entity Geometry => ((Geometry.Line) Line1.Geometry).MidPoint;

        [NotNull]
        public Line Line1 => (Line)GeometricEntities.ElementAt(0);

        [NotNull]
        public Line Line2 => (Line)GeometricEntities.ElementAt(1);

        #endregion

        #region Constructors

        /// <summary>
        /// For XML serialization.
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once NotNullMemberIsNotInitialized
        Parallel()
        {
        }

        public Parallel([NotNull] Line line1, [NotNull] Line line2)
        {
            if (line1 == null) throw new ArgumentNullException(nameof(line1));
            if (line2 == null) throw new ArgumentNullException(nameof(line2));
            if (line1.Parent != line2.Parent)
                throw new ArgumentException("The two lines must belong to the same sketch.");

            geometricEntities.Add(line1);
            geometricEntities.Add(line2);

            parameters.Add(line1.Start.X);
            parameters.Add(line1.Start.Y);
            parameters.Add(line1.End.X);
            parameters.Add(line1.End.Y);

            parameters.Add(line2.Start.X);
            parameters.Add(line2.Start.Y);
            parameters.Add(line2.End.X);
            parameters.Add(line2.End.Y);

            line1.Parent?.Entities.Add(this);
            Parent = line1.Parent;
        }

        #endregion

        #region Methods

        public static bool IsApplicable(IEnumerable<Entity> entities)
        {
            return entities.OfType<Line>().Count() == 2;
        }

        public override void Move(Vector vector)
        {
        }

        #endregion
    }
}
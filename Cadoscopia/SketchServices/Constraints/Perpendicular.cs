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
using JetBrains.Annotations;

namespace Cadoscopia.SketchServices.Constraints
{
    public class Perpendicular : Constraint
    {
        #region Fields

        [NotNull] readonly Line line1;

        [NotNull] readonly Line line2;

        #endregion

        #region Properties

        public override double Error
        {
            get
            {
                double dotProduct =
                    ((Geometry.Line) line1.Geometry).Direction.DotProduct(((Geometry.Line) line2.Geometry).Direction);
                return dotProduct * dotProduct;
            }
        }

        #endregion

        #region Constructors

        public Perpendicular([NotNull] Line line1, [NotNull] Line line2)
        {
            if (line1 == null) throw new ArgumentNullException(nameof(line1));
            if (line2 == null) throw new ArgumentNullException(nameof(line2));

            this.line1 = line1;
            this.line2 = line2;

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

        #endregion
    }
}
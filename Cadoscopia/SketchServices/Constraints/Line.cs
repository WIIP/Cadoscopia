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
using JetBrains.Annotations;

namespace Cadoscopia.SketchServices.Constraints
{
    public class Line : Entity
    {
        #region Properties

        public Point End { get; set; }

        public override Geometry.Entity Geometry => new Geometry.Line(new Geometry.Point(Start.X.Value, Start.Y.Value),
            new Geometry.Point(End.X.Value, End.Y.Value));

        public Point Start { get; set; }

        #endregion

        #region Constructors

        public Line()
        {
            Start = new Point();
            End = new Point();
        }

        public Line([NotNull] Parameter startX, [NotNull] Parameter startY)
        {
            if (startX == null) throw new ArgumentNullException(nameof(startX));
            if (startY == null) throw new ArgumentNullException(nameof(startY));

            Start = new Point(startX, startY);
            End = new Point();
            End.X.Value = startX.Value;
            End.Y.Value = startY.Value;
        }

        #endregion
    }
}
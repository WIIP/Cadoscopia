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

namespace Cadoscopia.Geometry
{
    sealed class Line : Entity
    {
        #region Properties

        public Vector Direction => Start.GetVector(End).Normalize();

        public Point End { get; }

        public double Length => Start.GetDistanceTo(End);

        public Point Start { get; }

        #endregion

        #region Constructors

        public Line(Point start, Point end)
        {
            End = end;
            Start = start;
        }

        #endregion

        #region Methods

        public Point GetClosestPointTo(Point point)
        {
            double dotProduct = Direction.DotProduct(point - Start);
            if (dotProduct < 0) return Start;
            if (dotProduct > Length) return End;
            return Start + Direction * dotProduct;
        }

        public override double GetDistanceTo(Point point)
        {
            return GetClosestPointTo(point).GetDistanceTo(point);
        }

        #endregion
    }
}
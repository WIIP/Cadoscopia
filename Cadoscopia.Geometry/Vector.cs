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

using static System.Math;

namespace Cadoscopia.Geometry
{
    public struct Vector
    {
        public double X { get; }

        public double Y { get; }

        public double Length => Sqrt(DotProduct(this));

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector Normalize()
        {
            return this / Length;
        }

        /// <summary>
        /// Returns the dot product of this vector and the other vector.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DotProduct(Vector other)
        {
            return X * other.X + Y * other.Y;
        }

        public static Vector operator /(Vector v, double n)
        {
            return new Vector(v.X / n, v.Y / n);
        }

        public static Vector operator *(Vector v, double n)
        {
            return new Vector(v.X * n, v.Y * n);
        }

        public Vector GetPerpendicular()
        {
            return new Vector(-Y, X);
        }
    }
}
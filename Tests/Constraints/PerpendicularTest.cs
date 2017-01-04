﻿// MIT License

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

using Cadoscopia.Parametric.SketchServices;
using Cadoscopia.Parametric.SketchServices.Entities;
using Cadoscopia.Parametric.SketchServices.Entities.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Constraints
{
    [TestClass]
    public class PerpendicularTest
    {
        [TestMethod]
        public void ErrorTest()
        {
            var sketch = new Sketch();

            var p0 = sketch.AddPoint();
            var p1 = sketch.AddPoint(1, 0);
            var p2 = sketch.AddPoint(0, 1);
            var p3 = sketch.AddPoint(1, 1);

            var horizontalLine = new Line(p0, p1);

            var verticalLine = new Line(p0, p2);

            var diagonalLine = new Line(p0, p3);

            {
                var sut = new Perpendicular(horizontalLine, verticalLine);
                Assert.AreEqual(sut.Error, 0, "The error should be zero because the 2 lines are perpendicular.");
            }

            {
                var sut = new Perpendicular(horizontalLine, diagonalLine);
                Assert.AreNotEqual(sut.Error, 0,
                    "The error should not be zero because the 2 lines are not perpendicular.");
            }
        }
    }
}

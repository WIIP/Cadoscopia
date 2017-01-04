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

using Cadoscopia.IO;
using Cadoscopia.Parametric.SketchServices;
using Cadoscopia.Parametric.SketchServices.Entities;
using Cadoscopia.Parametric.SketchServices.Entities.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Parametric.SketchServices
{
    [TestClass]
    public class SketchTest
    {
        #region Methods

        [TestMethod]
        public void TestXmlSerialization()
        {
            var before = new Sketch();

            Point p0 = before.AddPoint();
            Point p1 = before.AddPoint(1, 0);
            Point p2 = before.AddPoint(0, 1);

            var line1 = new Line(p0, p1);
            var line2 = new Line(p0, p2);

            new Perpendicular(line1, line2);
            var gxs = new GenericXmlSerializer<Sketch>();
            string xml = gxs.WriteToString(before);
            Sketch after = gxs.ReadFromString(xml);
            Assert.AreEqual(before.Entities.Count, after.Entities.Count);
        }

        #endregion
    }
}
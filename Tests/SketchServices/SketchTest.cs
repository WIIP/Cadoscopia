using Cadoscopia.SketchServices;
using Cadoscopia.SketchServices.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.SketchServices
{
    [TestClass]
    public class SketchTest
    {
        #region Methods

        [TestMethod]
        public void AddEntityTest()
        {
            var sut = new Sketch();

            var line = new Line();
            Assert.AreEqual(line.Id, Sketch.INVALID_ID);

            sut.Entities.Add(line);
            Assert.AreNotEqual(line.Id, Sketch.INVALID_ID);

            var point = new Point();
            sut.Entities.Add(point);
            Assert.AreNotEqual(point.Id, Sketch.INVALID_ID);
            Assert.AreNotEqual(point.Id, line.Id);
        }

        #endregion
    }
}
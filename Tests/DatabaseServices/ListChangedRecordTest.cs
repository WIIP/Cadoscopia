using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Cadoscopia.DatabaseServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.DatabaseServices
{
    [TestClass]
    public class ListChangedRecordTest
    {
        #region Methods

        [TestMethod]
        public void UndoTest()
        {
            var oc = new ObservableCollection<string> {"A", "B"};
            var lcr = new ListChangedRecord(oc, NotifyCollectionChangedAction.Add, new List<string> {"B"}, 1, null, -1);
            lcr.Undo();
            Assert.AreEqual(1, oc.Count);
            CollectionAssert.AreEqual(new[] {"A"}, oc);
        }

        #endregion
    }
}
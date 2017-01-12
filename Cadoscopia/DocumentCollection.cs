using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Cadoscopia.DatabaseServices;
using Cadoscopia.Parametric.SketchServices;

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

namespace Cadoscopia
{
    public class DocumentCollection : INotifyCollectionChanged, IEnumerable<Document>
    {
        #region Fields

        readonly App application;

        readonly List<Document> documents = new List<Document>();

        #endregion

        #region Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Constructors

        public DocumentCollection(App application)
        {
            this.application = application;
        }

        #endregion

        #region Methods

        public void Add()
        {
            var db = new SharedObjectDatabase(undoRecording: false);
            db.Objects.Add(new Sketch { Database = db });
            db.StartUndoRecording();
            var sharedObjectDocument = new SharedObjectDocument(db);
            documents.Add(sharedObjectDocument);
            application.ActiveDocument = sharedObjectDocument;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Document> GetEnumerator()
        {
            return documents.GetEnumerator();
        }

        #endregion
    }
}
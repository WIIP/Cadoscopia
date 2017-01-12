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

namespace Cadoscopia.DatabaseServices
{
    public class Transaction : IDisposable
    {
        #region Fields

        bool committed;

        readonly TransactionManager manager;

        #endregion

        #region Properties

        public string Name { get; }

        public List<UndoRedoRecord> Records { get; } = new List<UndoRedoRecord>();

        #endregion

        #region Constructors

        internal Transaction([NotNull] TransactionManager manager, string name)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            this.manager = manager;

            Name = name;
        }

        #endregion

        #region Methods

        public void Abort()
        {
            manager.TopTransaction = null;
        }

        public void Commit()
        {
            committed = true;
            manager.CommittedTransactions.Push(this);
            manager.TopTransaction = null;
        }

        public void Dispose()
        {
            if (committed) return;
            Abort();
        }

        public void Redo()
        {
            manager.TransactionsThatCanBeRedone.Remove(this);

            manager.IsRedoing = true;
            try
            {
                foreach (UndoRedoRecord record in Records)
                    record.Redo();
            }
            finally
            {
                manager.IsRedoing = false;
            }

            manager.CommittedTransactions.Push(this);
        }

        public override string ToString()
        {
            return Name;
        }

        public void Undo()
        {
            manager.IsUndoing = true;
            try
            {
                while (true)
                {
                    Transaction transaction = manager.CommittedTransactions.Pop();
                    for (int i = transaction.Records.Count - 1; i >= 0; i--)
                        Records[i].Undo();
                    manager.TransactionsThatCanBeRedone.Insert(0, transaction);
                    if (transaction == this) break;
                }
            }
            finally
            {
                manager.IsUndoing = false;
            }
        }

        #endregion
    }
}
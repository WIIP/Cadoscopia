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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Cadoscopia.Core;
using JetBrains.Annotations;

namespace Cadoscopia.DatabaseServices
{
    /// <summary>
    /// Manage undo/redo.
    /// </summary>
    public class TransactionManager
    {
        #region Properties

        public bool IsRedoing { get; internal set; }

        public bool IsUndoing { get; internal set; }

        public bool CanRedo => TransactionsThatCanBeRedone.Count > 0;

        public bool CanUndo => CommittedTransactions.Count > 0;

        public ObservableStack<Transaction> CommittedTransactions { get; } = new ObservableStack<Transaction>();

        public ObservableCollection<Transaction> TransactionsThatCanBeRedone { get; } = new ObservableCollection<Transaction>();

        public Transaction TopTransaction { get; internal set; }

        #endregion

        #region Constructors

        public TransactionManager([NotNull] Database database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            database.Objects.CollectionChanged += CollectionChanged;
            foreach (DatabaseObject dbo in database.Objects)
                MonitorObject(dbo);
        }

        #endregion

        #region Methods

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsUndoing || IsRedoing) return;

            if (TopTransaction == null)
                throw new ModificationOutsideTransactionException();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    TopTransaction.Records.Add(new ListChangedRecord((IList)sender, e.Action, e.NewItems, e.NewStartingIndex,
                        e.OldItems, e.OldStartingIndex));

                    foreach (DatabaseObject newItem in e.NewItems)
                        MonitorObject(newItem);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    TopTransaction.Records.Add(new ListChangedRecord((IList)sender, e.Action, e.NewItems, e.NewStartingIndex,
                        e.OldItems, e.OldStartingIndex));
                    break;
            }
        }

        void DatabaseObject_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (IsUndoing || IsRedoing) return;

            if (TopTransaction == null)
                throw new ModificationOutsideTransactionException();

            if (TopTransaction.Records.OfType<PropertiesChangedRecord>().All(r => r.DatabaseObject != sender))
                TopTransaction.Records.Add(new PropertiesChangedRecord((DatabaseObject)sender));
        }

        void MonitorObject([NotNull] DatabaseObject dbo)
        {
            if (dbo == null) throw new ArgumentNullException(nameof(dbo));

            dbo.PropertyChanging += DatabaseObject_PropertyChanging;

            PropertyInfo[] properties = dbo.GetType().GetProperties();

            Type dboType = typeof(DatabaseObject);
            IEnumerable<PropertyInfo> dboPropertyInfos
                = properties.Where(pi => dboType.IsAssignableFrom(pi.PropertyType));
            foreach (PropertyInfo pi in dboPropertyInfos)
            {
                if (Attribute.GetCustomAttribute(pi, typeof(IgnoreForUndoRedoMonitoringAttribute), inherit:true) != null)
                    continue;
                object propertyValue = pi.GetValue(dbo);
                MonitorObject((DatabaseObject)propertyValue);
            }

            Type iNotifyCollectionChangedType = typeof(INotifyCollectionChanged);
            IEnumerable<PropertyInfo> iNotifyCollectionChangedPropertyInfos
                = properties.Where(pi => iNotifyCollectionChangedType.IsAssignableFrom(pi.PropertyType));
            foreach (PropertyInfo pi in iNotifyCollectionChangedPropertyInfos)
            {
                object propertyValue = pi.GetValue(dbo);
                ((INotifyCollectionChanged)propertyValue).CollectionChanged += CollectionChanged;
                var collection = propertyValue as ICollection;
                if (collection == null) continue;
                foreach (object item in collection)
                    MonitorObject((DatabaseObject)item);
            }
        }

        public Transaction StartTransaction([NotNull] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            TopTransaction = new Transaction(this, name);
            return TopTransaction;
        }

        #endregion
    }
}
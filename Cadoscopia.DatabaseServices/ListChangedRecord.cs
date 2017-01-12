using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Cadoscopia.DatabaseServices
{
    public class ListChangedRecord : UndoRedoRecord
    {
        #region Fields

        readonly NotifyCollectionChangedAction action;

        readonly IList list;

        readonly IList newItems;

        readonly int newStartingIndex;

        readonly IList oldItems;

        readonly int oldStartingIndex;

        #endregion

        #region Constructors

        public ListChangedRecord(IList list, NotifyCollectionChangedAction action, IList newItems, int newStartingIndex,
            IList oldItems, int oldStartingIndex)
        {
            this.list = list;
            this.action = action;
            this.newItems = newItems;
            this.newStartingIndex = newStartingIndex;
            this.oldItems = oldItems;
            this.oldStartingIndex = oldStartingIndex;
        }

        #endregion

        #region Methods

        public override void Undo()
        {
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < newItems.Count; i++)
                        list.RemoveAt(newStartingIndex + i);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object oldItem in oldItems)
                        list.Add(oldItem);
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public override void Redo()
        {
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < newItems.Count; i++)
                        list.Insert(newStartingIndex + i, newItems[i]);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < oldItems.Count; i++)
                        list.RemoveAt(oldStartingIndex);
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        #endregion
    }
}
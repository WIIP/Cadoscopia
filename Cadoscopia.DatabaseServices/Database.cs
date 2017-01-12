using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Cadoscopia.DatabaseServices
{
    public abstract class Database
    {
        #region Properties

        public TransactionManager TransactionManager { get; private set; }

        public ObservableCollection<DatabaseObject> Objects { get; } = new ObservableCollection<DatabaseObject>();

        #endregion

        #region Constructors

        protected Database(bool undoRecording = true)
        {
            // ReSharper disable once InvertIf
            if (undoRecording)
                StartUndoRecording();
        }

        public void StartUndoRecording()
        {
            TransactionManager = new TransactionManager(this);
        }

        #endregion
    }
}
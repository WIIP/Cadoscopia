namespace Cadoscopia.DatabaseServices
{
    public class SharedObjectDatabase : Database
    {
        #region Constructors

        public SharedObjectDatabase(bool undoRecording = true) : base(undoRecording)
        {
        }

        #endregion
    }
}
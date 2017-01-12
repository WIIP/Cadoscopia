namespace Cadoscopia.DatabaseServices
{
    public abstract class UndoRedoRecord
    {
        public abstract void Undo();

        public abstract void Redo();
    }
}
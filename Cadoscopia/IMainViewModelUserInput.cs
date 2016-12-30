using JetBrains.Annotations;

namespace Cadoscopia
{
    public interface IMainViewModelUserInput
    {
        [CanBeNull]
        string GetSaveFileName();
    }
}
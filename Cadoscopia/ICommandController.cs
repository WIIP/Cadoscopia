using System.Windows;

namespace Cadoscopia
{
    public interface ICommandController
    {
        #region Methods

        void OnCanvasClick(Point position, bool shiftPressed);

        void OnCanvasMove(Point from, Point to, bool leftButtonPressed);

        void StopCommand();

        #endregion
    }
}
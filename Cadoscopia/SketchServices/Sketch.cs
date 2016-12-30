using System.Collections.ObjectModel;

namespace Cadoscopia.SketchServices
{
    public class Sketch
    {
        #region Properties

        public ObservableCollection<Entity> Entities { get; } = new ObservableCollection<Entity>();

        #endregion
    }
}
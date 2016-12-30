using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Cadoscopia.SketchServices
{
    public class Sketch
    {
        #region Fields

        int nextId;

        #endregion

        #region Properties

        public ObservableCollection<Entity> Entities { get; } = new ObservableCollection<Entity>();

        #endregion

        #region Constructors

        public Sketch()
        {
            Entities.CollectionChanged += Entities_CollectionChanged;
        }

        #endregion

        #region Methods

        void Entities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            foreach (object newItem in e.NewItems)
            {
                var entity = (Entity) newItem;
                entity.Id = nextId;
                nextId++;
            }
        }

        #endregion
    }
}
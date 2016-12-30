using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Serialization;
using Cadoscopia.SketchServices.Constraints;

namespace Cadoscopia.SketchServices
{
    public class Sketch
    {
        #region Constants

        public const int INVALID_ID = -1;

        #endregion

        #region Fields

        int nextId;

        #endregion

        #region Properties

        [XmlElement(nameof(Line), Type = typeof(Line))]
        [XmlElement(nameof(Point), Type = typeof(Point))]
        public ObservableCollection<Entity> Entities { get; } = new ObservableCollection<Entity>();

        [XmlElement(nameof(Parallel), Type = typeof(Parallel))]
        [XmlElement(nameof(Perpendicular), Type = typeof(Perpendicular))]
        public ObservableCollection<Constraint> Constraints { get; } = new ObservableCollection<Constraint>();

        #endregion

        #region Constructors

        public Sketch()
        {
            Entities.CollectionChanged += Objects_CollectionChanged;
            Constraints.CollectionChanged += Objects_CollectionChanged;
        }

        #endregion

        #region Methods

        void Objects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            foreach (object newItem in e.NewItems)
            {
                var so = (SketchObject) newItem;
                so.Id = nextId;
                nextId++;
            }
        }

        #endregion
    }
}
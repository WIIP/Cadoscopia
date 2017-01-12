using System.Linq;
using Cadoscopia.Parametric.SketchServices.Entities;
using Cadoscopia.Parametric.SketchServices.Entities.Constraints;
using JetBrains.Annotations;
using Point = Cadoscopia.Geometry.Point;

namespace Cadoscopia
{
    class VerticalViewModel : EntityViewModel
    {
        double left;

        double top;

        public VerticalViewModel([NotNull] Entity sketchEntity) : base(sketchEntity)
        {
            var vertical = (Vertical) sketchEntity;
            UpdatePosition();
            var line = vertical.GeometricEntities.First() as Line;
            if (line == null) return;

            line.Start.PropertyChanged += LineStartOrEnd_PropertyChanged;
            line.Start.X.PropertyChanged += LineStartOrEnd_PropertyChanged;
            line.Start.Y.PropertyChanged += LineStartOrEnd_PropertyChanged;

            line.End.PropertyChanged += LineStartOrEnd_PropertyChanged;
            line.End.X.PropertyChanged += LineStartOrEnd_PropertyChanged;
            line.End.Y.PropertyChanged += LineStartOrEnd_PropertyChanged;
        }

        void UpdatePosition()
        {
            var vertical = (Vertical)SketchEntity;
            var point = (Point)vertical.Geometry;
            Left = point.X - 7;
            Top = point.Y - 7;
        }

        void LineStartOrEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdatePosition();
        }

        public sealed override double Left
        {
            get { return left; }
            set
            {
                if (value.Equals(left)) return;
                left = value;
                OnPropertyChanged(nameof(Left));
            }
        }

        public sealed override double Top
        {
            get { return top; }
            set
            {
                if (value.Equals(top)) return;
                top = value;
                OnPropertyChanged(nameof(Top));
            }
        }
    }
}
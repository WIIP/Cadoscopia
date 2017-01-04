using Cadoscopia.Parametric.SketchServices.Entities;
using Cadoscopia.Parametric.SketchServices.Entities.Constraints;
using JetBrains.Annotations;
using Line = Cadoscopia.Geometry.Line;

namespace Cadoscopia
{
    public class ConstraintParallelViewModel : EntityViewModel
    {
        #region Fields

        double left;

        double top;

        #endregion

        #region Properties

        public override double Left
        {
            get { return left; }
            set
            {
                if (value.Equals(left)) return;
                left = value;
                OnPropertyChanged(nameof(Left));
            }
        }

        public override double Top
        {
            get { return top; }
            set
            {
                if (value.Equals(top)) return;
                top = value;
                OnPropertyChanged(nameof(Top));
            }
        }

        #endregion

        #region Constructors

        public ConstraintParallelViewModel([NotNull] Entity entity) : base(entity)
        {
        }

        #endregion

        #region Methods

        public override void UpdateBindings()
        {
            var parallel = (Parallel) SketchEntity;
            Left = ((Line) parallel.Line1.Geometry).MidPoint.X;
            Top = ((Line) parallel.Line1.Geometry).MidPoint.Y;
        }

        #endregion
    }
}
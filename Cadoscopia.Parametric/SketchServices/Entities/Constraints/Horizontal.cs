using Cadoscopia.Geometry;

namespace Cadoscopia.Parametric.SketchServices.Entities.Constraints
{
    public class Horizontal : Constraint
    {
        public override Geometry.Entity Geometry { get; }

        public override void Move(Vector vector)
        {
        }
    }
}
using System;
using System.Linq;
using JetBrains.Annotations;

namespace Cadoscopia.Geometry
{
    public class PointCloud : Entity
    {
        #region Fields

        [NotNull] readonly Point[] points;

        #endregion

        #region Constructors

        public PointCloud([NotNull] Point[] points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));
            if (points.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(points));

            this.points = points;
        }

        #endregion

        #region Methods

        public override double GetDistanceTo(Point point)
        {
            return points.Min(p => p.GetDistanceTo(point));
        }

        #endregion
    }
}
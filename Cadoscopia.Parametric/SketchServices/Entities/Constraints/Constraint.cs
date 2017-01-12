// MIT License

// Copyright(c) 2016 Cadoscopia http://cadoscopia.com

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cadoscopia.Geometry;
using JetBrains.Annotations;

namespace Cadoscopia.Parametric.SketchServices.Entities.Constraints
{
    public abstract class Constraint : Entity
    {
        #region Fields

        protected readonly List<GeometricEntity> geometricEntities = new List<GeometricEntity>();

        #endregion

        #region Properties

        public virtual double Error => 0;

        public IReadOnlyCollection<GeometricEntity> GeometricEntities
            => new ReadOnlyCollection<GeometricEntity>(geometricEntities);

        public virtual bool UseSharedParameters => false;

        #endregion

        #region Methods

        protected Geometry.Point GetSymbolPosition([NotNull] GeometricEntity entity)
        {
            var line = (Geometry.Line) entity.Geometry;
            Vector perp = line.Direction.GetPerpendicular().Normalize();
            Geometry.Point tPoint = line.MidPoint + perp * 10;
            return tPoint;
            List<Constraint> constraints = entity.Constraints.ToList();
            int indexOf = constraints.IndexOf(this);
            // ReSharper disable once PossibleLossOfFraction
            Geometry.Point basePoint = tPoint - line.Direction * (16 * constraints.Count / 2);
            return basePoint + line.Direction * (16 * indexOf);
        }

        #endregion
    }
}
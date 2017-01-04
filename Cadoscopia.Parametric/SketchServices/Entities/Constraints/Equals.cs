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
using System.Linq;
using JetBrains.Annotations;

namespace Cadoscopia.Parametric.SketchServices.Entities.Constraints
{
    public class Equals : Constraint
    {
        #region Properties

        public override double Error
        {
            get
            {
                double diff = Parameters[0].Value - Parameters[1].Value;
                return diff * diff;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// For XML deserialization.
        /// </summary>
        [UsedImplicitly]
        Equals()
        {
        }

        public Equals([NotNull] Parameter first, [NotNull] Parameter second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            parameters.Add(first);
            parameters.Add(second);
        }

        #endregion

        #region Methods

        public static bool IsApplicable(IEnumerable<Entity> entities)
        {
            return entities.OfType<Line>().Any();
        }

        public override Geometry.Entity Geometry { get; }

        #endregion
    }
}
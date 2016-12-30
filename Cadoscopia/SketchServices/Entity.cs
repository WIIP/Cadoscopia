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

using System.Diagnostics;

namespace Cadoscopia.SketchServices
{
    // ReSharper disable once UseNameofExpression
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class Entity
    {
        #region Constants

        const int INVALID_ID = -1;

        #endregion

        #region Fields

        #endregion

        #region Properties

        string DebuggerDisplay => $"{Geometry.GetType().Name}: {Id}";

        public abstract Geometry.Entity Geometry { get; }

        /// <summary>
        /// Id of this entity.
        /// </summary>
        /// <remarks>
        /// Each entity has a different id. Ids start at zero.
        /// </remarks>
        public int Id { get; internal set; } = INVALID_ID;

        #endregion
    }
}
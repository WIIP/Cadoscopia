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

using Cadoscopia.SketchServices;
using Cadoscopia.SketchServices.Constraints;
using JetBrains.Annotations;

namespace Cadoscopia
{
    class LineViewModel : EntityViewModel
    {
        #region Properties

        [NotNull]
        public PointViewModel End { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Only here to avoid binding error.
        /// </remarks>
        public override double Left
        {
            get { return 0; }
            set { }
        }

        public Line SketchLine => (Line) SketchEntity;

        [NotNull]
        public PointViewModel Start { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Only here to avoid binding error.
        /// </remarks>
        public override double Top
        {
            get { return 0; }
            set { }
        }

        public double X1
        {
            get { return SketchLine.Start.X.Value; }
            set
            {
                SketchLine.Start.X.Value = value;
                OnPropertyChanged(nameof(X1));
            }
        }

        public double X2
        {
            get { return SketchLine.End.X.Value; }
            set
            {
                SketchLine.End.X.Value = value;
                OnPropertyChanged(nameof(X2));
            }
        }

        public double Y1
        {
            get { return SketchLine.Start.Y.Value; }
            set
            {
                SketchLine.Start.Y.Value = value;
                OnPropertyChanged(nameof(Y1));
            }
        }

        public double Y2
        {
            get { return SketchLine.End.Y.Value; }
            set
            {
                SketchLine.End.Y.Value = value;
                OnPropertyChanged(nameof(Y2));
            }
        }

        #endregion

        #region Constructors

        public LineViewModel(Line line) : base(line)
        {
            Start = new PointViewModel(line.Start);
            End = new PointViewModel(line.End);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Forces re-evaluation of parameters.
        /// </summary>
        public override void UpdateBindings()
        {
            X1 = SketchLine.Start.X.Value;
            Y1 = SketchLine.Start.Y.Value;

            X2 = SketchLine.End.X.Value;
            Y2 = SketchLine.End.Y.Value;
        }

        #endregion
    }
}
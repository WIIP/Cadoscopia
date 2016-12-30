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
using JetBrains.Annotations;

namespace Cadoscopia
{
    class PointViewModel : EntityViewModel
    {
        #region Fields

        double left;

        double top;

        #endregion

        #region Properties

        public Point Point => (Point)SketchEntity;

        [UsedImplicitly]
        public override double Left
        {
            get { return left; }
            set
            {
                left = value;
                OnPropertyChanged(nameof(Left));
            }
        }

        [UsedImplicitly]
        public override double Top
        {
            get { return top; }
            set
            {
                top = value;
                OnPropertyChanged(nameof(Top));
            }
        }

        public double X
        {
            get { return Point.X.Value; }
            set
            {
                Point.X.Value = value;
                OnPropertyChanged(nameof(X));
                Left = X - Constants.POINT_RADIUS;
            }
        }

        public double Y
        {
            get { return Point.Y.Value; }
            set
            {
                Point.Y.Value = value;
                OnPropertyChanged(nameof(Y));
                Top = Y - Constants.POINT_RADIUS;
            }
        }

        #endregion

        #region Constructors

        public PointViewModel(Point point) : base(point)
        {
        }

        #endregion

        #region Methods

        public override void UpdateBindings()
        {
            X = Point.X.Value;
            Y = Point.Y.Value;
        }

        #endregion
    }
}
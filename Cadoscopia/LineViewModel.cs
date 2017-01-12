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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Cadoscopia.Parametric.SketchServices.Entities;
using JetBrains.Annotations;

namespace Cadoscopia
{
    class LineViewModel : EntityViewModel
    {
        #region Fields

        double x1;

        double x2;

        double y1;

        double y2;

        #endregion

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
                x1 = value;
                OnPropertyChanged(nameof(X1));
            }
        }

        public double X2
        {
            get { return SketchLine.End.X.Value; }
            set
            {
                x2 = value;
                OnPropertyChanged(nameof(X2));
            }
        }

        public double Y1
        {
            get { return SketchLine.Start.Y.Value; }
            set
            {
                y1 = value;
                OnPropertyChanged(nameof(Y1));
            }
        }

        public double Y2
        {
            get { return SketchLine.End.Y.Value; }
            set
            {
                y2 = value;
                OnPropertyChanged(nameof(Y2));
            }
        }

        #endregion

        #region Constructors

        public LineViewModel(Line line, ObservableCollection<EntityViewModel> entities) : base(line)
        {
            Start = entities.OfType<PointViewModel>().First(pvm => pvm.SketchEntity == line.Start);
            End = entities.OfType<PointViewModel>().First(pvm => pvm.SketchEntity == line.End);

            X1 = line.Start.X.Value;
            Y1 = line.Start.Y.Value;
            X2 = line.End.X.Value;
            Y2 = line.End.Y.Value;

            line.PropertyChanging += Line_PropertyChanging;
            ;
            line.PropertyChanged += Line_PropertyChanged;

            line.Start.PropertyChanging += Start_PropertyChanging;
            line.Start.PropertyChanged += Start_PropertyChanged;
            line.Start.X.PropertyChanged += Start_X_PropertyChanged;
            line.Start.Y.PropertyChanged += Start_Y_PropertyChanged;

            line.End.PropertyChanging += End_PropertyChanging;
            line.End.PropertyChanged += End_PropertyChanged;
            line.End.X.PropertyChanged += End_X_PropertyChanged;
            line.End.Y.PropertyChanged += End_Y_PropertyChanged;
        }

        #endregion

        #region Methods

        void End_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            X2 = SketchLine.End.X.Value;
            Y2 = SketchLine.End.Y.Value;
            SketchLine.End.X.PropertyChanged += End_X_PropertyChanged;
            SketchLine.End.Y.PropertyChanged += End_Y_PropertyChanged;
        }

        void End_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            SketchLine.End.X.PropertyChanged -= End_X_PropertyChanged;
            SketchLine.End.Y.PropertyChanged -= End_Y_PropertyChanged;
        }

        void End_X_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            X2 = SketchLine.End.X.Value;
        }

        void End_Y_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Y2 = SketchLine.End.Y.Value;
        }

        void Line_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            X1 = SketchLine.Start.X.Value;
            Y1 = SketchLine.Start.Y.Value;
            X2 = SketchLine.End.X.Value;
            Y2 = SketchLine.End.Y.Value;
        }

        void Line_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            SketchLine.Start.PropertyChanged -= Start_PropertyChanged;
            SketchLine.End.PropertyChanged -= End_PropertyChanged;
        }

        void Start_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            X1 = SketchLine.Start.X.Value;
            Y1 = SketchLine.Start.Y.Value;
            SketchLine.Start.X.PropertyChanged += Start_X_PropertyChanged;
            SketchLine.Start.Y.PropertyChanged += Start_Y_PropertyChanged;
        }

        void Start_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            SketchLine.Start.X.PropertyChanged -= Start_X_PropertyChanged;
            SketchLine.Start.Y.PropertyChanged -= Start_Y_PropertyChanged;
        }

        void Start_X_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            X1 = SketchLine.Start.X.Value;
        }

        void Start_Y_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Y1 = SketchLine.Start.Y.Value;
        }

        #endregion
    }
}
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Cadoscopia.Constraints;
using Cadoscopia.Properties;
using Cadoscopia.SketchServices;
using Cadoscopia.Wpf;
using JetBrains.Annotations;
using Point = Cadoscopia.SketchServices.Point;

namespace Cadoscopia
{
    public class MainViewModel : ViewModel
    {
        #region Fields

        Cursor canvasCursor;

        readonly List<Constraint> constraints = new List<Constraint>();

        EntityViewModel entityViewModel;

        readonly HashSet<EntityViewModel> selection = new HashSet<EntityViewModel>();

        string status = Resources.Ready;

        int step;

        #endregion

        #region Properties

        public Cursor CanvasCursor
        {
            get { return canvasCursor; }
            set
            {
                if (Equals(value, canvasCursor)) return;
                canvasCursor = value;
                OnPropertyChanged(nameof(CanvasCursor));
            }
        }

        public ObservableCollection<EntityViewModel> Entities { get; }

        public ICommand HorizontalCommand { get; }

        public ICommand LineCommand { get; }

        public ICommand PerpendicularCommand { get; }

        public string Status
        {
            get { return status; }
            set
            {
                if (value == status) return;
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public ICommand VerticalCommand { get; }

        public ICommand ParallelCommand { get; }

        #endregion

        #region Constructors

        public MainViewModel()
        {
            Entities = new ObservableCollection<EntityViewModel>();

            HorizontalCommand = new RelayCommand(HorizontalCommandExecute, HorizontalVerticalCommandCanExecute);
            ParallelCommand = new RelayCommand(ParallelCommandExecute, ParallelCommandCommandCanExecute);
            PerpendicularCommand = new RelayCommand(PerpendicularCommandExecute, PerpendicularCommandCanExecute);
            VerticalCommand = new RelayCommand(VerticalCommandExecute, HorizontalVerticalCommandCanExecute);
            LineCommand = new RelayCommand(LineCommandExecute);
        }

        bool ParallelCommandCommandCanExecute(object obj)
        {
            return Parallel.IsApplicable(selection.Select(e => e.SketchEntity));
        }

        void ParallelCommandExecute(object obj)
        {
            AddConstraint(new Parallel((Line)selection.ElementAt(0).SketchEntity,
                (Line)selection.ElementAt(1).SketchEntity));
        }

        #endregion

        #region Methods

        void AddConstraint(Constraint constraint)
        {
            constraints.Add(constraint);
            if (!Solver.Solve(constraints))
                MessageBox.Show(Resources.NoSolutionFound);

            UpdateBindings();
        }

        void HorizontalCommandExecute(object obj)
        {
            // TODO Must fail if the line already has a vertical constraint.
            var line = (Line) selection.First().SketchEntity;
            AddConstraint(new Equals(line.Start.Y, line.End.Y));
        }

        bool HorizontalVerticalCommandCanExecute(object obj)
        {
            return Constraints.Equals.IsApplicable(selection.Select(vm => vm.SketchEntity));
        }

        void LineCommandExecute(object obj)
        {
            entityViewModel = new LineViewModel();
            step = 0;
            Status = Resources.LineFirstPoint;
            CanvasCursor = Cursors.Cross;
        }

        /// <summary>
        /// Returns the entity which is near the passed point or null if there are no element.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        [CanBeNull]
        EntityViewModel EntityAtPoint(System.Windows.Point point)
        {
            var gePoint = new Geometry.Point(point.X, point.Y);
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                EntityViewModel evm = Entities[i];
                double d = evm.SketchEntity.Geometry.GetDistanceTo(gePoint);
                if (d > Constants.SELECTION_DISTANCE) continue;
                return evm;
            }
            return null;
        }

        public void OnCanvasClick(System.Windows.Point position, bool shiftPressed)
        {
            if (entityViewModel == null)
            {
                // We are in selection mode.
                if (!shiftPressed)
                    ClearSelection();

                EntityViewModel entityAtPoint = EntityAtPoint(position);
                if (entityAtPoint != null)
                {
                    entityAtPoint.IsSelected = !entityAtPoint.IsSelected;
                    if (entityAtPoint.IsSelected)
                        selection.Add(entityAtPoint);
                    else
                        selection.Remove(entityAtPoint);
                }
                switch (selection.Count)
                {
                    case 0:
                        Status = Resources.Ready;
                        break;

                    case 1:
                        string name = selection.First().SketchEntity.Geometry.GetType().Name.ToLower();
                        Status = string.Format(Resources.OneEntitySelected, name);
                        break;

                    default:
                        var data = selection.GroupBy(e => e.SketchEntity.Geometry.GetType()).Select(g => new {Metric = g.Key, Count = g.Count()})
                            .OrderBy(x => x.Count);
                        if (data.Count() > 3)
                            Status = string.Format(Resources.EntitiesSelected, selection.Count);
                        else
                            Status = string.Join(", ", data.Select(e => $"{e.Count} {Pluralize.Do(e.Metric.Name.ToLower(), e.Count)}"))
                                + " " + Resources.Selected;
                        break;
                }
            }
            else
            {
                var lineViewModel = entityViewModel as LineViewModel;
                // ReSharper disable once InvertIf
                if (lineViewModel != null)
                {
                    switch (step)
                    {
                        case 0:
                            lineViewModel.X1 = lineViewModel.X2 = position.X;
                            lineViewModel.Y1 = lineViewModel.Y2 = position.Y;

                            Status = Resources.LineSecondPoint;

                            step++;
                            break;

                        default:
                            lineViewModel.X2 = position.X;
                            lineViewModel.Y2 = position.Y;

                            Status = Resources.NextPoint;

                            Point sketchLineEnd = lineViewModel.SketchLine.End;
                            entityViewModel = new LineViewModel(sketchLineEnd.X, sketchLineEnd.Y);

                            step++;
                            break;
                    }

                    // Important: points are added last so that they are drawn on top and selected first.
                    Entities.Add(entityViewModel);
                    Entities.Add(lineViewModel.Start);
                    Entities.Add(lineViewModel.End);

                    UpdateBindings();
                }
            }
        }

        void ClearSelection()
        {
            foreach (EntityViewModel entity in Entities)
                entity.IsSelected = false;
            selection.Clear();
        }

        public void OnCanvasMove(System.Windows.Point position)
        {
            var lineViewModel = entityViewModel as LineViewModel;
            // ReSharper disable once InvertIf
            if (lineViewModel != null && step > 0)
            {
                lineViewModel.X2 = position.X;
                lineViewModel.Y2 = position.Y;

                UpdateBindings();
            }
        }

        public void OnKeyDown(Key key)
        {
            switch (key)
            {
                case Key.Escape:
                    if (entityViewModel != null)
                    {
                        Entities.Remove(entityViewModel);
                        var lineViewModel = entityViewModel as LineViewModel;
                        if (lineViewModel != null)
                        {
                            Entities.Remove(lineViewModel.Start);
                            Entities.Remove(lineViewModel.End);
                        }
                        entityViewModel = null;
                        CanvasCursor = Cursors.Arrow;
                        Status = Resources.Ready;
                    }
                    step = 0;
                    break;
            }
        }

        bool PerpendicularCommandCanExecute(object obj)
        {
            return Perpendicular.IsApplicable(selection.Select(vm => vm.SketchEntity));
        }

        void PerpendicularCommandExecute(object obj)
        {
            AddConstraint(new Perpendicular((Line) selection.ElementAt(0).SketchEntity,
                (Line) selection.ElementAt(1).SketchEntity));
        }

        /// <summary>
        /// Update bindings.
        /// </summary>
        void UpdateBindings()
        {
            foreach (EntityViewModel entity in Entities)
                entity.UpdateBindings();
        }

        void VerticalCommandExecute(object obj)
        {
            // TODO Must fail if the line already has a horizontal constraint.
            var line = (Line) selection.First().SketchEntity;
            AddConstraint(new Equals(line.Start.X, line.End.X));
        }

        #endregion
    }
}
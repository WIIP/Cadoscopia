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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Cadoscopia.IO;
using Cadoscopia.Parametric;
using Cadoscopia.Parametric.SketchServices;
using Cadoscopia.Parametric.SketchServices.Entities;
using Cadoscopia.Parametric.SketchServices.Entities.Constraints;
using Cadoscopia.Properties;
using Cadoscopia.Wpf;
using JetBrains.Annotations;
using Point = System.Windows.Point;

namespace Cadoscopia
{
    public class MainViewModel : ViewModel
    {
        #region Fields

        Cursor canvasCursor;

        Entity entityInProgress;

        readonly HashSet<EntityViewModel> selection = new HashSet<EntityViewModel>();

        readonly Sketch sketch = new Sketch();

        string status = Resources.Ready;

        SolidColorBrush statusBackground = new SolidColorBrush(Color.FromRgb(43, 87, 154));

        int step = -1;

        readonly IMainViewModelUserInput userInput;

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

        public ICommand ParallelCommand { get; }

        public ICommand PerpendicularCommand { get; }

        public ICommand SaveCommand { get; }

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

        public SolidColorBrush StatusBackground
        {
            get { return statusBackground; }
            set
            {
                if (Equals(value, statusBackground)) return;
                statusBackground = value;
                OnPropertyChanged(nameof(StatusBackground));
            }
        }

        public ICommand VerticalCommand { get; }

        #endregion

        #region Constructors

        public MainViewModel([NotNull] IMainViewModelUserInput userInput)
        {
            if (userInput == null) throw new ArgumentNullException(nameof(userInput));

            this.userInput = userInput;

            Entities = new ViewModelCollection<EntityViewModel, Entity>(sketch.Entities, EntityViewModelCreator);

            HorizontalCommand = new RelayCommand(HorizontalCommandExecute, HorizontalVerticalCommandCanExecute);
            ParallelCommand = new RelayCommand(ParallelCommandExecute, ParallelCommandCommandCanExecute);
            PerpendicularCommand = new RelayCommand(PerpendicularCommandExecute, PerpendicularCommandCanExecute);
            VerticalCommand = new RelayCommand(VerticalCommandExecute, HorizontalVerticalCommandCanExecute);
            LineCommand = new RelayCommand(LineCommandExecute);
            SaveCommand = new RelayCommand(SaveCommandExecute);
        }

        #endregion

        #region Methods

        void Solve()
        {
            IEnumerable<Constraint> constraints = sketch.Entities.OfType<Constraint>();
            if (!constraints.Any()) return;
            if (!Solver.Solve(constraints.ToList()))
                MessageBox.Show(Resources.NoSolutionFound);

            UpdateBindings();
        }

        void ClearSelection()
        {
            foreach (EntityViewModel entity in Entities)
                entity.IsSelected = false;
            selection.Clear();
        }

        /// <summary>
        /// Returns the entity which is near the passed point or null if there are no element.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        [CanBeNull]
        T EntityAtPoint<T>(Point point) where T : EntityViewModel
        {
            var gePoint = new Geometry.Point(point.X, point.Y);
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                EntityViewModel evm = Entities[i];
                if (!(evm is T)) continue;
                if (evm.SketchEntity == entityInProgress) continue;

                double d = evm.SketchEntity.Geometry.GetDistanceTo(gePoint);
                if (d > Constants.SELECTION_DISTANCE) continue;
                var pvm = evm as PointViewModel;
                if (pvm == null || !(entityInProgress is Line)) return (T) evm;
                var lineInProgress = (Line) entityInProgress;
                if (pvm.Point == lineInProgress.Start || pvm.Point == lineInProgress.End)
                    continue;
                return (T) evm;
            }
            return null;
        }

        /// <summary>
        /// Create an appropriate ViewModel each time an entity is added to the sketch.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        EntityViewModel EntityViewModelCreator([NotNull] Entity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var line = entity as Line;
            if (line != null) return new LineViewModel(line);

            var point = entity as Parametric.SketchServices.Entities.Point;
            if (point != null) return new PointViewModel(point);

            var parallelConstraint = entity as Parallel;
            if (parallelConstraint != null) return new ConstraintParallelViewModel(parallelConstraint);

            throw new NotSupportedException();
        }

        void HorizontalCommandExecute(object obj)
        {
            // TODO Must fail if the line already has a vertical constraint.
            var line = (Line) selection.First().SketchEntity;
            Solve();
        }

        bool HorizontalVerticalCommandCanExecute(object obj)
        {
            return Parametric.SketchServices.Entities.Constraints.Equals.IsApplicable(selection.Select(vm => vm.SketchEntity));
        }

        void LineCommandExecute(object obj)
        {
            step = 0;
            Status = Resources.LineFirstPoint;
            CanvasCursor = Cursors.Cross;
        }

        public void OnCanvasClick(Point position, bool shiftPressed)
        {
            if (step == -1)
            {
                // We are in selection mode.
                if (!shiftPressed)
                    ClearSelection();

                var entityAtPoint = EntityAtPoint<EntityViewModel>(position);
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
                        var data = selection.GroupBy(e => e.SketchEntity.Geometry.GetType())
                            .Select(g => new {Metric = g.Key, Count = g.Count()})
                            .OrderBy(x => x.Count);
                        if (data.Count() > 3)
                            Status = string.Format(Resources.EntitiesSelected, selection.Count);
                        else
                            Status = string.Join(", ",
                                         data.Select(e => $"{e.Count} {Pluralize.Do(e.Metric.Name.ToLower(), e.Count)}"))
                                     + " " + Resources.Selected;
                        break;
                }
            }
            else
            {
                Line line;
                switch (step)
                {
                    case 0:
                        entityInProgress = line = new Line(sketch.AddPoint(),
                            sketch.AddPoint());
                        line.Start.X.Value = line.End.X.Value = position.X;
                        line.Start.Y.Value = line.End.Y.Value = position.Y;
                        UpdateBinding(line);
                        UpdateBinding(line.Start);
                        UpdateBinding(line.End);
                        Status = Resources.LineSecondPoint;
                        step++;
                        break;

                    default:
                        line = (Line) entityInProgress;
                        var pointAtPosition = EntityAtPoint<PointViewModel>(position);
                        if (pointAtPosition != null)
                        {
                            sketch.Entities.Remove(line.End);
                            line.End = pointAtPosition.Point;
                            line.End.X.Value = position.X;
                            line.End.Y.Value = position.Y;
                            // Update entities which share this point
                            IEnumerable<LineViewModel> lines =
                                Entities.OfType<LineViewModel>()
                                    .Where(
                                        lvm =>
                                            lvm.SketchLine != line &&
                                            (lvm.Start.Point == pointAtPosition.Point ||
                                             lvm.End.Point == pointAtPosition.Point));
                            foreach (LineViewModel lvm in lines)
                                lvm.UpdateBindings();
                        }

                        line.End.X.Value = position.X;
                        line.End.Y.Value = position.Y;
                        UpdateBinding(line);
                        UpdateBinding(line.End);

                        if (pointAtPosition != null)
                            Reinit();
                        else
                        {
                            Status = Resources.NextPoint;
                            Parametric.SketchServices.Entities.Point sketchLineEnd = line.End;
                            entityInProgress = new Line(sketchLineEnd,
                                sketch.AddPoint());
                            // Add the line here, as we know its starting point
                            UpdateBinding((Line) entityInProgress);
                            step++;
                        }
                        break;
                }
            }
        }

        public void OnCanvasKeyDown(Key key)
        {
            switch (key)
            {
                case Key.Escape:
                    StopCommand();
                    break;
            }
        }

        public void OnCanvasMove(Point position)
        {
            var sketchLine = entityInProgress as Line;
            // ReSharper disable once InvertIf
            if (sketchLine != null && step > 0)
            {
                sketchLine.End.X.Value = position.X;
                sketchLine.End.Y.Value = position.Y;

                UpdateBinding(sketchLine);
                UpdateBinding(sketchLine.End);
            }
        }

        bool ParallelCommandCommandCanExecute(object obj)
        {
            return Parallel.IsApplicable(selection.Select(e => e.SketchEntity));
        }

        void ParallelCommandExecute(object obj)
        {
            var parallel = new Parallel((Line) selection.ElementAt(0).SketchEntity, 
                (Line) selection.ElementAt(1).SketchEntity);
            Solve();
        }

        bool PerpendicularCommandCanExecute(object obj)
        {
            return Perpendicular.IsApplicable(selection.Select(vm => vm.SketchEntity));
        }

        void PerpendicularCommandExecute(object obj)
        {
            Solve();
        }

        void Reinit()
        {
            entityInProgress = null;
            CanvasCursor = Cursors.Arrow;
            Status = Resources.Ready;
            step = -1;
        }

        void SaveCommandExecute(object obj)
        {
            string fileName = userInput.GetSaveFileName();
            if (fileName == null) return;
            var gxs = new GenericXmlSerializer<Sketch>();
            gxs.Write(sketch, fileName);
        }

        /// <summary>
        /// Stop the command in progress.
        /// </summary>
        void StopCommand()
        {
            if (entityInProgress != null)
            {
                sketch.Entities.Remove(entityInProgress);
                var lineViewModel = entityInProgress as Line;
                if (lineViewModel != null)
                {
                    if (sketch.CountReferences(lineViewModel.Start) == 1)
                        sketch.Entities.Remove(lineViewModel.Start);
                    if (sketch.CountReferences(lineViewModel.End) == 1)
                        sketch.Entities.Remove(lineViewModel.End);
                }
                Reinit();
            }
            step = -1;
        }

        void UpdateBinding([NotNull] Entity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Entities.First(evm => evm.SketchEntity == entity).UpdateBindings();
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
            Solve();
        }

        #endregion
    }
}
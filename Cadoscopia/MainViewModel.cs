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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Cadoscopia.IO;
using Cadoscopia.Properties;
using Cadoscopia.SketchServices;
using Cadoscopia.SketchServices.Constraints;
using Cadoscopia.Wpf;
using JetBrains.Annotations;
using Point = Cadoscopia.SketchServices.Constraints.Point;

namespace Cadoscopia
{
    public class MainViewModel : ViewModel
    {
        readonly IMainViewModelUserInput userInput;

        #region Fields

        Cursor canvasCursor;

        readonly List<Constraint> constraints = new List<Constraint>();

        Entity entityInProgress;

        readonly HashSet<EntityViewModel> selection = new HashSet<EntityViewModel>();

        string status = Resources.Ready;

        int step;

        readonly Sketch sketch = new Sketch();

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

        public ICommand SaveCommand { get; }

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

        void SaveCommandExecute(object obj)
        {
            string fileName = userInput.GetSaveFileName();
            if (fileName == null) return;
            var gxs = new GenericXmlSerializer<Sketch>();
            gxs.Write(sketch, fileName);
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
            var point = entity as Point;
            if (point != null) return new PointViewModel(point);
            throw new NotSupportedException();
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
            return SketchServices.Constraints.Equals.IsApplicable(selection.Select(vm => vm.SketchEntity));
        }

        void LineCommandExecute(object obj)
        {
            // The line will be added to the canvas at first click.
            entityInProgress = new Line();
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
        T EntityAtPoint<T>(System.Windows.Point point) where T: EntityViewModel
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
                var lineInProgress = (Line)entityInProgress;
                if (pvm.Point == lineInProgress.Start || pvm.Point == lineInProgress.End)
                    continue;
                return (T) evm;
            }
            return null;
        }

        public void OnCanvasClick(System.Windows.Point position, bool shiftPressed)
        {
            if (entityInProgress == null)
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
                var sketchLineInProgress = entityInProgress as Line;
                // ReSharper disable once InvertIf
                if (sketchLineInProgress != null)
                {
                    switch (step)
                    {
                        case 0:
                            sketchLineInProgress.Start.X.Value = sketchLineInProgress.End.X.Value = position.X;
                            sketchLineInProgress.Start.Y.Value = sketchLineInProgress.End.Y.Value = position.Y;
                            AddLineToSketch(sketchLineInProgress);
                            Status = Resources.LineSecondPoint;

                            step++;
                            break;

                        default:
                            var pointAtPosition = EntityAtPoint<PointViewModel>(position);
                            if (pointAtPosition != null)
                            {
                                sketch.Entities.Remove(sketchLineInProgress.End);
                                sketchLineInProgress.End = pointAtPosition.Point;
                                sketchLineInProgress.End.X.Value = position.X;
                                sketchLineInProgress.End.Y.Value = position.Y;
                                // Update entities which share this point
                                IEnumerable<LineViewModel> lines = Entities.OfType<LineViewModel>().Where(lvm => lvm.SketchLine != sketchLineInProgress && (lvm.Start.Point == pointAtPosition.Point || lvm.End.Point == pointAtPosition.Point));
                                foreach (LineViewModel lvm in lines)
                                    lvm.UpdateBindings();
                            }

                            sketchLineInProgress.End.X.Value = position.X;
                            sketchLineInProgress.End.Y.Value = position.Y;
                            UpdateBinding(sketchLineInProgress);
                            UpdateBinding(sketchLineInProgress.End);

                            if (pointAtPosition != null)
                                Reinit();
                            else
                            {
                                Status = Resources.NextPoint;
                                Point sketchLineEnd = sketchLineInProgress.End;
                                entityInProgress = new Line(sketchLineEnd.X, sketchLineEnd.Y);
                                // Add the line here, as we know its starting point
                                AddLineToSketch((Line) entityInProgress);
                                step++;
                            }
                            break;
                    }
                }
            }
        }

        void AddLineToSketch([NotNull] Line sketchLine)
        {
            if (sketchLine == null) throw new ArgumentNullException(nameof(sketchLine));

            // Important: points are added last so that they are drawn on top and selected first.
            sketch.Entities.Add(sketchLine);
            UpdateBinding(sketchLine);

            sketch.Entities.Add(sketchLine.Start);
            UpdateBinding(sketchLine.Start);

            sketch.Entities.Add(sketchLine.End);
            UpdateBinding(sketchLine.End);
        }

        void ClearSelection()
        {
            foreach (EntityViewModel entity in Entities)
                entity.IsSelected = false;
            selection.Clear();
        }

        public void OnCanvasMove(System.Windows.Point position)
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

        void UpdateBinding([NotNull] Entity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Entities.First(evm => evm.SketchEntity == entity).UpdateBindings();
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
                    sketch.Entities.Remove(lineViewModel.Start);
                    sketch.Entities.Remove(lineViewModel.End);
                }
                Reinit();
            }
            step = 0;
        }

        void Reinit()
        {
            entityInProgress = null;
            CanvasCursor = Cursors.Arrow;
            Status = Resources.Ready;
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
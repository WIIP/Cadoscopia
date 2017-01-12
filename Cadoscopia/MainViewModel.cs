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
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Cadoscopia.DatabaseServices;
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
        readonly IApp app;

        #region Fields

        [CanBeNull] ICommandController commandController;

        Cursor canvasCursor;

        public IEnumerable<EntityViewModel> Selection
        {
            get { return Entities.Where(e => e.IsSelected); }
        }

        string status = Resources.Ready;

        SolidColorBrush statusBackground = new SolidColorBrush(Color.FromRgb(43, 87, 154));

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

        public ICommand ParallelCommand { get; }

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

        public ICommand ExecuteCommandCommand { get; }

        public ObservableCollection<TransactionViewModel> UndoItems { get; }

        public bool UndoIsEnabled
        {
            get
            {
                Document activeDocument = app.ActiveDocument;
                if (activeDocument == null) return false;
                return activeDocument.Database.TransactionManager.CanUndo
                       && activeDocument.CommandInProgress == null;
            }
        }

        public bool RedoIsEnabled
        {
            get
            {
                Document activeDocument = app.ActiveDocument;
                if (activeDocument == null) return false;
                return activeDocument.Database.TransactionManager.CanRedo
                       && activeDocument.CommandInProgress == null;
            }
        }

        public ObservableCollection<TransactionViewModel> RedoItems { get; }

        #endregion

        #region Constructors

        public MainViewModel([NotNull] IApp app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            this.app = app;

            Document activeDocument = app.ActiveDocument;
            Debug.Assert(activeDocument != null, "activeDocument != null");

            var sketch = (Sketch)app.ActiveEditObject;
            Debug.Assert(sketch != null, "sketch != null");

            Entities = new ViewModelCollection<EntityViewModel, Entity>(sketch.Entities, EntityViewModelCreator);
            
            TransactionManager tm = activeDocument.Database.TransactionManager;

            RedoItems = new ViewModelCollection<TransactionViewModel, Transaction>(tm.TransactionsThatCanBeRedone, 
                TransactionViewModelCreator);
            tm.TransactionsThatCanBeRedone.CollectionChanged += TransactionsThatCanBeRedone_CollectionChanged;

            UndoItems = new ViewModelCollection<TransactionViewModel, Transaction>(tm.CommittedTransactions, 
                TransactionViewModelCreator);
            tm.CommittedTransactions.CollectionChanged += CommittedTransactions_CollectionChanged;

            ExecuteCommandCommand = new RelayCommand(ExecuteCommandCommandExecute);
            HorizontalCommand = new RelayCommand(HorizontalCommandExecute, HorizontalVerticalCommandCanExecute);
            ParallelCommand = new RelayCommand(ParallelCommandExecute, ParallelCommandCommandCanExecute);
            PerpendicularCommand = new RelayCommand(PerpendicularCommandExecute, PerpendicularCommandCanExecute);
        }

        void CommittedTransactions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(UndoIsEnabled));
        }

        void TransactionsThatCanBeRedone_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(RedoIsEnabled));
        }

        static TransactionViewModel TransactionViewModelCreator(Transaction tr)
        {
            return new TransactionViewModel(tr);
        }

        void ExecuteCommandCommandExecute(object obj)
        {
            string cmd = (string)obj;
            commandController = app.CommandManager.ExecuteCommand(cmd);
        }

        #endregion

        #region Methods

        public void Solve()
        {
            var sketch = (Sketch)app.ActiveEditObject;
            Debug.Assert(sketch != null, "sketch != null");
            IEnumerable<Constraint> constraints = sketch.Entities.OfType<Constraint>();
            if (!constraints.Any()) return;
            if (!Solver.Solve(constraints.ToList()))
                MessageBox.Show(Resources.NoSolutionFound);
        }

        public void ClearSelection()
        {
            foreach (EntityViewModel entity in Entities)
                entity.IsSelected = false;
        }

        /// <summary>
        /// Returns the entities which are near the passed point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IEnumerable<T> EntitiesAtPoint<T>(Point point) where T : EntityViewModel
        {
            var gePoint = new Geometry.Point(point.X, point.Y);
            foreach (T evm in Entities.OfType<T>())
            {
                double d = evm.SketchEntity.Geometry.GetDistanceTo(gePoint);
                if (d > Constants.SELECTION_DISTANCE) continue;
                yield return (T) evm;
            }
        }

        /// <summary>
        /// Create an appropriate ViewModel each time an entity is added to the sketch.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NotNull]
        EntityViewModel EntityViewModelCreator([NotNull] Entity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var line = entity as Line;
            if (line != null) return new LineViewModel(line, Entities);

            var point = entity as Parametric.SketchServices.Entities.Point;
            if (point != null) return new PointViewModel(point);

            var equalsConstraint = entity as Equals;
            if (equalsConstraint != null) return new EqualsViewModel(equalsConstraint);

            var parallelConstraint = entity as Parallel;
            if (parallelConstraint != null) return new ParallelViewModel(parallelConstraint);

            var verticalConstraint = entity as Vertical;
            if (verticalConstraint != null) return new VerticalViewModel(verticalConstraint);

            throw new NotImplementedException();
        }

        void HorizontalCommandExecute(object obj)
        {
            // TODO Must fail if the line already has a vertical constraint.
            var line = (Line) Selection.First().SketchEntity;
            Solve();
        }

        bool HorizontalVerticalCommandCanExecute(object obj)
        {
            return Parametric.SketchServices.Entities.Constraints.Equals.IsApplicable(Selection.Select(vm => vm.SketchEntity));
        }

        public void OnCanvasClick(Point position, bool shiftPressed)
        {
            if (commandController != null)
                commandController.OnCanvasClick(position, shiftPressed);
            else
            {
                // We are in selection mode.
                if (!shiftPressed)
                    ClearSelection();

                EntityViewModel entityAtPoint = EntitiesAtPoint<EntityViewModel>(position).FirstOrDefault();
                if (entityAtPoint != null)
                    entityAtPoint.IsSelected = !entityAtPoint.IsSelected;
                switch (Selection.Count())
                {
                    case 0:
                        Status = Resources.Ready;
                        break;

                    case 1:
                        string name = Selection.First().SketchEntity.Geometry.GetType().Name.ToLower();
                        Status = string.Format(Resources.OneEntitySelected, name);
                        break;

                    default:
                        var data = Selection.GroupBy(e => e.SketchEntity.Geometry.GetType())
                            .Select(g => new { Metric = g.Key, Count = g.Count() })
                            .OrderBy(x => x.Count);
                        if (data.Count() > 3)
                            Status = string.Format(Resources.EntitiesSelected, Selection.Count());
                        else
                            Status = string.Join(", ",
                                         data.Select(e => $"{e.Count} {Pluralize.Do(e.Metric.Name.ToLower(), e.Count)}"))
                                     + " " + Resources.Selected;
                        break;
                }
            }
        }

        public void OnCanvasKeyDown(Key key)
        {
            switch (key)
            {
                case Key.Escape:
                    commandController?.StopCommand();
                    StopCommand();
                    ClearSelection();
                    break;
            }
        }

        public void OnCanvasMove(Point from, Point to, bool leftButtonPressed)
        {
            if (commandController != null)
                commandController?.OnCanvasMove(from, to, leftButtonPressed);
            else if (leftButtonPressed)
                commandController = app.CommandManager.ExecuteCommand("Drag");
        }

        bool ParallelCommandCommandCanExecute(object obj)
        {
            return Parallel.IsApplicable(Selection.Select(e => e.SketchEntity));
        }

        void ParallelCommandExecute(object obj)
        {
            new Parallel((Line) Selection.ElementAt(0).SketchEntity, (Line) Selection.ElementAt(1).SketchEntity);
            Solve();
        }

        bool PerpendicularCommandCanExecute(object obj)
        {
            return Perpendicular.IsApplicable(Selection.Select(vm => vm.SketchEntity));
        }

        void PerpendicularCommandExecute(object obj)
        {
            Solve();
        }

        public void Reinit()
        {
            CanvasCursor = Cursors.Arrow;
            app.SetStatus();
            commandController = null;
            if (app.ActiveDocument != null) app.ActiveDocument.CommandInProgress = null;
            OnPropertyChanged(nameof(RedoIsEnabled));
            OnPropertyChanged(nameof(UndoIsEnabled));
        }

        /// <summary>
        /// Stop the command in progress.
        /// </summary>
        void StopCommand()
        {
            Reinit();
        }

        #endregion
    }
}
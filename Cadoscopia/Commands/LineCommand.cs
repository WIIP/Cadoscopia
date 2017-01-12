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
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Cadoscopia.DatabaseServices;
using Cadoscopia.Parametric.SketchServices;
using Cadoscopia.Parametric.SketchServices.Entities;
using Cadoscopia.Properties;
using JetBrains.Annotations;
using Point = System.Windows.Point;

namespace Cadoscopia.Commands
{
    // TODO Remove dependencies to MVVM pattern.

    [CommandClass("Line")]
    [UsedImplicitly]
    public class LineCommand : Command, ICommandController
    {
        #region Fields

        Line lineInProgress;

        int step;

        #endregion

        #region Constructors

        public LineCommand([NotNull] IApp app) : base(app)
        {
        }

        #endregion

        #region Methods

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            step = 0;
            app.SetStatus(Resources.LineFirstPoint, StatusType.RequestUserInput);
            app.CanvasCursor = Cursors.Cross;
        }

        public void OnCanvasClick(Point position, bool shiftPressed)
        {
            var sharedObjectDocument = (SharedObjectDocument) app.ActiveDocument;
            Debug.Assert(sharedObjectDocument != null, "sharedObjectDocument != null");

            Database db = sharedObjectDocument.Database;

            var sketch = (Sketch) app.ActiveEditObject;
            Debug.Assert(sketch != null, "sketch != null");

            switch (step)
            {
                case 0:
                    db.TransactionManager.StartTransaction(Resources.Line);
                    
                    lineInProgress = new Line(sketch.AddPoint(), sketch.AddPoint());
                    lineInProgress.Start.X.Value = lineInProgress.End.X.Value = position.X;
                    lineInProgress.Start.Y.Value = lineInProgress.End.Y.Value = position.Y;
                    app.SetStatus(Resources.LineSecondPoint, StatusType.RequestUserInput);
                    step++;
                    break;

                default:
                    var testPoint = new Geometry.Point(position.X, position.Y);
                    Parametric.SketchServices.Entities.Point pointAtPosition =
                        sketch.EntitiesAtPoint<Parametric.SketchServices.Entities.Point>(testPoint, app.RadiusForSelection)
                            .FirstOrDefault(e => e != lineInProgress.Start && e != lineInProgress.End);
                    if (pointAtPosition != null)
                    {
                        sketch.Entities.Remove(lineInProgress.End);
                        lineInProgress.End = pointAtPosition;
                        lineInProgress.End.X.Value = position.X;
                        lineInProgress.End.Y.Value = position.Y;
                    }

                    lineInProgress.End.X.Value = position.X;
                    lineInProgress.End.Y.Value = position.Y;

                    if (pointAtPosition != null)
                        app.StopCommand();
                    else
                    {
                        app.SetStatus(Resources.NextPoint, StatusType.RequestUserInput);
                        Parametric.SketchServices.Entities.Point sketchLineEnd = lineInProgress.End;
                        lineInProgress = new Line(sketchLineEnd, sketch.AddPoint());
                        // Add the line here, as we know its starting point
                        step++;
                    }
                    break;
            }
        }

        public void OnCanvasMove(Point from, Point to, bool leftButtonPressed)
        {
            // ReSharper disable once InvertIf
            if (lineInProgress != null && step > 0)
            {
                lineInProgress.End.X.Value = to.X;
                lineInProgress.End.Y.Value = to.Y;
            }
        }

        public void StopCommand()
        {
            if (lineInProgress == null) return;

            var sketch = (Sketch)app.ActiveEditObject;
            Debug.Assert(sketch != null, "sketch != null");

            sketch.Entities.Remove(lineInProgress);
            if (sketch.CountReferences(lineInProgress.Start) == 1)
                sketch.Entities.Remove(lineInProgress.Start);
            if (sketch.CountReferences(lineInProgress.End) == 1)
                sketch.Entities.Remove(lineInProgress.End);
            sketch.Database.TransactionManager.TopTransaction.Commit();
        }

        public override event EventHandler CanExecuteChanged;

        #endregion
    }
}
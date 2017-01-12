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
using System.Windows;
using System.Windows.Input;
using Cadoscopia.DatabaseServices;
using Cadoscopia.Parametric.SketchServices;
using Cadoscopia.Properties;
using JetBrains.Annotations;

namespace Cadoscopia.Commands
{
    [CommandClass("Point")]
    [UsedImplicitly]
    public class PointCommand : Command, ICommandController
    {
        #region Methods

        public PointCommand([NotNull] IApp app) : base(app)
        {
        }

        public override event EventHandler CanExecuteChanged;

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            app.SetStatus(Resources.PointPosition, StatusType.RequestUserInput);
            app.CanvasCursor = Cursors.Cross;
            Document activeDocument = app.ActiveDocument;
            Debug.Assert(activeDocument != null, "activeDocument != null");
            activeDocument.Database.TransactionManager.StartTransaction(Resources.Point);
        }

        public void OnCanvasClick(Point position, bool shiftPressed)
        {
            var sketch = (Sketch) app.ActiveEditObject;
            sketch.AddPoint(position.X, position.Y);
        }

        public void OnCanvasMove(Point from, Point to, bool leftButtonPressed)
        {
        }

        public void StopCommand()
        {
            Document activeDocument = app.ActiveDocument;
            Debug.Assert(activeDocument != null, "activeDocument != null");
            activeDocument.Database.TransactionManager.TopTransaction.Commit();
        }

        #endregion
    }
}
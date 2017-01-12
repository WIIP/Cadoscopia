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
using Cadoscopia.DatabaseServices;
using Cadoscopia.Properties;
using JetBrains.Annotations;
using Vector = Cadoscopia.Geometry.Vector;

namespace Cadoscopia.Commands
{
    [CommandClass("Drag")]
    [UsedImplicitly]
    public class DragCommand : Command, ICommandController
    {
        #region Events

        public override event EventHandler CanExecuteChanged;

        #endregion

        #region Constructors

        public DragCommand([NotNull] IApp app) : base(app)
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
        }

        public void OnCanvasClick(Point position, bool shiftPressed)
        {
        }

        public void OnCanvasMove(Point from, Point to, bool leftButtonPressed)
        {
            MainViewModel mvm = app.MainViewModel;
            if (leftButtonPressed)
            {
                Document activeDocument = app.ActiveDocument;
                Debug.Assert(activeDocument != null, "activeDocument != null");
                TransactionManager tm = activeDocument.Database.TransactionManager;
                if (tm.TopTransaction == null)
                    tm.StartTransaction(Resources.Drag);
                foreach (EntityViewModel entityViewModel in mvm.Selection)
                    entityViewModel.SketchEntity.Move(new Vector(to.X - from.X, to.Y - from.Y));
                mvm.Solve();
            }
            else
                StopCommand();
        }

        public void StopCommand()
        {
            Document activeDocument = app.ActiveDocument;
            Debug.Assert(activeDocument != null, "activeDocument != null");
            activeDocument.Database.TransactionManager.TopTransaction?.Commit();
        }

        #endregion
    }
}
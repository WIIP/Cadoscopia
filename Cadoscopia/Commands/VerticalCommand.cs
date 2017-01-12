using System;
using System.Diagnostics;
using System.Linq;
using Cadoscopia.DatabaseServices;
using Cadoscopia.Parametric.SketchServices;
using Cadoscopia.Parametric.SketchServices.Entities;
using Cadoscopia.Parametric.SketchServices.Entities.Constraints;
using Cadoscopia.Properties;
using JetBrains.Annotations;

namespace Cadoscopia.Commands
{
    [CommandClass("Vertical")]
    [UsedImplicitly]
    class VerticalCommand : Command
    {
        #region Events

        public override event EventHandler CanExecuteChanged;

        #endregion

        #region Constructors

        public VerticalCommand([NotNull] IApp app) : base(app)
        {
        }

        #endregion

        #region Methods

        public override bool CanExecute(object parameter)
        {
            return
                Parametric.SketchServices.Entities.Constraints.Equals.IsApplicable(
                    app.MainViewModel.Selection.Select(vm => vm.SketchEntity));
        }

        public override void Execute(object parameter)
        {
            Document activeDocument = app.ActiveDocument;
            Debug.Assert(activeDocument != null, "activeDocument != null");
            using (Transaction tr = activeDocument.Database.TransactionManager.StartTransaction(Resources.Vertical))
            {
                Line line = app.MainViewModel.Selection.OfType<LineViewModel>().First().SketchLine;
                var vertical = new Vertical(line);
                var sketch = (Sketch) app.ActiveEditObject;
                Debug.Assert(sketch != null, "sketch != null");
                sketch.Entities.Add(vertical);
                app.MainViewModel.Solve();
                tr.Commit();
            }
        }

        #endregion
    }
}
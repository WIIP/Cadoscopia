using System;
using JetBrains.Annotations;

namespace Cadoscopia.Commands
{
    [CommandClass("Parallel")]
    class ParallelCommand : Command
    {
        #region Events

        public override event EventHandler CanExecuteChanged;

        #endregion

        #region Constructors

        public ParallelCommand([NotNull] IApp app) : base(app)
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

        #endregion
    }
}
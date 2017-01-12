using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Cadoscopia
{
    public abstract class Command : ICommand
    {
        #region Fields

        protected readonly IApp app;

        #endregion

        #region Events

        public abstract event EventHandler CanExecuteChanged;

        #endregion

        #region Constructors

        protected Command([NotNull] IApp app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            this.app = app;
        }

        #endregion

        #region Methods

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);

        #endregion
    }
}
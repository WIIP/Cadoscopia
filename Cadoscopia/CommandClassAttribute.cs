using System;

namespace Cadoscopia
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandClassAttribute : Attribute
    {
        #region Properties

        public string CommandName { get; }

        #endregion

        #region Constructors

        public CommandClassAttribute(string commandName)
        {
            CommandName = commandName;
        }

        #endregion
    }
}
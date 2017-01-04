using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Cadoscopia
{
    public class CommandManager
    {
        #region Properties

        public Dictionary<string, Type> Commands { get; } = new Dictionary<string, Type>();

        #endregion

        #region Constructors

        public CommandManager()
        {
            LoadCommands();
        }

        #endregion

        #region Methods

        void LoadCommands()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            Type iCommandInterface = typeof (ICommand);
            foreach (Type type in types)
            {
                object[] attributes = type.GetCustomAttributes(typeof (CommandClassAttribute), false);
                if (attributes.Length == 0 || !iCommandInterface.IsAssignableFrom(type)) continue;
                string commandName = ((CommandClassAttribute) attributes[0]).CommandName;
                Commands.Add(commandName, type);
            }
        }

        public void ExecuteCommand([NotNull] string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(commandName));

            if (!Commands.ContainsKey(commandName))
                throw new ApplicationException($"Command {commandName} not found.");
            
            var command = (ICommand)Activator.CreateInstance(Commands[commandName]);
            command.Execute(null);
        }

        #endregion
    }
}
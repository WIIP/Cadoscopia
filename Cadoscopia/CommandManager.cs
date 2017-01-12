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
using System.Reflection;
using System.Windows;
using Cadoscopia.Core;
using Cadoscopia.Properties;
using JetBrains.Annotations;

namespace Cadoscopia
{
    [UsedImplicitly]
    public class CommandManager
    {
        #region Fields

        [CanBeNull] readonly IApp app;

        #endregion

        #region Properties

        public Dictionary<string, Type> Commands { get; } = new Dictionary<string, Type>();

        #endregion

        #region Constructors

        public CommandManager([CanBeNull] IApp app)
        {
            this.app = app;

            LoadCommands();
        }

        #endregion

        #region Methods

        [CanBeNull]
        public ICommandController ExecuteCommand([NotNull] string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(commandName));

            if (!Commands.ContainsKey(commandName))
                throw new ApplicationException($"Command \"{commandName}\" not found.");

            var command = (Command) Activator.CreateInstance(Commands[commandName], app);
            try
            {
                command.Execute(null);
            }
            catch (UserException ex)
            {
                if (app != null)
                    app.SetStatus(ex.Message, StatusType.Error, true);
                else
                    throw;
            }
            if (app?.ActiveDocument != null) app.ActiveDocument.CommandInProgress = command;
            return command as ICommandController;
        }

        void LoadCommands()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            Type commandInterface = typeof(Command);
            foreach (Type type in types)
            {
                object[] attributes = type.GetCustomAttributes(typeof(CommandClassAttribute), false);
                if (attributes.Length == 0 || !commandInterface.IsAssignableFrom(type)) continue;
                string commandName = ((CommandClassAttribute) attributes[0]).CommandName;
                Commands.Add(commandName, type);
            }
        }

        #endregion
    }
}
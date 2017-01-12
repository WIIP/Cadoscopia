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
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Cadoscopia.DatabaseServices;

namespace Cadoscopia
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : IApp
    {
        #region Constants

        static readonly SolidColorBrush defaultBackground = new SolidColorBrush(Color.FromRgb(43, 87, 154));

        #endregion

        #region Fields

        readonly DispatcherTimer timer;

        #endregion

        #region Properties

        public Document ActiveDocument { get; set; }

        public DatabaseObject ActiveEditObject => ActiveDocument?.Database.Objects.FirstOrDefault();

        public Cursor CanvasCursor
        {
            get { return MainViewModel.CanvasCursor; }
            set { MainViewModel.CanvasCursor = value; }
        }

        public CommandManager CommandManager => new CommandManager(this);

        public new static App Current => (App) Application.Current;

        public DocumentCollection Documents => new DocumentCollection(this);

        public MainViewModel MainViewModel => (MainViewModel) MainWindow.DataContext;

        public double RadiusForSelection => 10;

        #endregion

        #region Constructors

        public App()
        {
            Documents.Add();

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
        }

        #endregion

        #region Methods

        public void SetStatus(string status = null, StatusType type = StatusType.Information, bool transient = false)
        {
            MainViewModel.Status = status ?? Cadoscopia.Properties.Resources.Ready;

            switch (type)
            {
                case StatusType.Error:
                    MainViewModel.StatusBackground = new SolidColorBrush(Colors.Red);
                    break;

                case StatusType.Information:
                    MainViewModel.StatusBackground = defaultBackground;
                    break;

                case StatusType.RequestUserInput:
                    MainViewModel.StatusBackground = new SolidColorBrush(Color.FromRgb(194, 6, 197));
                    break;

                case StatusType.Result:
                    MainViewModel.StatusBackground = new SolidColorBrush(Colors.Green);
                    break;

                case StatusType.Warning:
                    MainViewModel.StatusBackground = new SolidColorBrush(Colors.Orange);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (!transient) return;

            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Start();
        }

        public void StopCommand()
        {
            if (ActiveDocument != null) ActiveDocument.CommandInProgress = null;
            SetStatus();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            SetStatus(Cadoscopia.Properties.Resources.Ready);
        }

        #endregion
    }
}
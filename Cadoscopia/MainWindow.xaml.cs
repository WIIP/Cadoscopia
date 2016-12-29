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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cadoscopia
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        readonly MainViewModel mainViewModel;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            mainViewModel = new MainViewModel();
            DataContext = mainViewModel;
        }

        #endregion

        #region Methods

        void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            mainViewModel.OnKeyDown(e.Key);
        }

        void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var canvas = (Canvas) sender;
            // Put the focus on the canvas in order to catch key press.
            canvas.Focus();
            Point position = e.GetPosition(canvas);
            mainViewModel.OnCanvasClick(position,
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
        }

        void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = (Canvas) sender;
            Point position = e.GetPosition(canvas);
            mainViewModel.OnCanvasMove(position);
        }

        #endregion
    }
}
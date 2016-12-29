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

using System.Windows.Media;
using Cadoscopia.SketchServices;
using Cadoscopia.Wpf;
using JetBrains.Annotations;

namespace Cadoscopia
{
    public abstract class EntityViewModel : ViewModel
    {
        #region Fields

        bool isSelected;

        #endregion

        #region Properties

        public Brush Color => new SolidColorBrush(IsSelected
            ? Constants.SELECTED_ENTITY_COLOR
            : Constants.ENTITY_COLOR);

        [UsedImplicitly]
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value == isSelected) return;
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
                OnPropertyChanged(nameof(Color));
            }
        }

        public abstract double Left { get; set; }

        public Entity SketchEntity { get; protected set; }

        public abstract double Top { get; set; }

        #endregion

        #region Methods

        public abstract void UpdateBindings();

        #endregion
    }
}
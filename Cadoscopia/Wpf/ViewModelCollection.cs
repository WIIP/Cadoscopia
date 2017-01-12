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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Cadoscopia.Core;
using Cadoscopia.DatabaseServices;
using JetBrains.Annotations;

namespace Cadoscopia.Wpf
{
    /// <summary>
    /// Allow to synchronize a ViewModel collection with a Model collection. 
    /// 
    /// Each time a Model is added to the wrapped collection, a new ViewModel is created with 
    /// the factory passed in the constructor. And if a Model is removed, the corresponding 
    /// ViewModel is also removed from this collection.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <remarks>
    /// Inspired by http://stackoverflow.com/a/2177659/200443
    /// </remarks>
    sealed class ViewModelCollection<TViewModel, TModel> : ObservableCollection<TViewModel>
    {
        #region Fields

        readonly Func<TModel, TViewModel> viewModelFactory;

        #endregion

        #region Constructors

        public ViewModelCollection([NotNull] ObservableCollection<TModel> source,
            [NotNull] Func<TModel, TViewModel> viewModelFactory)
            : base(source.Select(viewModelFactory))
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (viewModelFactory == null) throw new ArgumentNullException(nameof(viewModelFactory));

            source.CollectionChanged += OnSourceCollectionChanged;
            this.viewModelFactory = viewModelFactory;
        }

        public ViewModelCollection([NotNull] ObservableStack<TModel> source,
            [NotNull] Func<TModel, TViewModel> viewModelFactory)
            : base(source.Select(viewModelFactory))
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (viewModelFactory == null) throw new ArgumentNullException(nameof(viewModelFactory));

            source.CollectionChanged += OnSourceCollectionChanged;
            this.viewModelFactory = viewModelFactory;
        }

        #endregion

        #region Methods

        void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        Insert(e.NewStartingIndex + i, viewModelFactory((TModel) e.NewItems[i]));
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (e.OldItems.Count == 1)
                        Move(e.OldStartingIndex, e.NewStartingIndex);
                    else
                    {
                        List<TViewModel> items = this.Skip(e.OldStartingIndex).Take(e.OldItems.Count).ToList();
                        for (int i = 0; i < e.OldItems.Count; i++)
                            RemoveAt(e.OldStartingIndex);

                        for (int i = 0; i < items.Count; i++)
                            Insert(e.NewStartingIndex + i, items[i]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        RemoveAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        RemoveAt(e.OldStartingIndex);
                    goto case NotifyCollectionChangedAction.Add;

                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    foreach (object newItem in e.NewItems)
                        Add(viewModelFactory((TModel) newItem));
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        #endregion
    }
}
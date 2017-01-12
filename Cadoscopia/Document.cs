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
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Cadoscopia.DatabaseServices;
using JetBrains.Annotations;

namespace Cadoscopia
{
    public abstract class Document : INotifyPropertyChanged, IXmlSerializable
    {
        #region Fields

        Uri uri;

        #endregion

        #region Properties

        public ICommand CommandInProgress { get; internal set; }

        public Database Database { get; private set; }

        [CanBeNull]
        public string Name => Uri != null ? Path.GetFileNameWithoutExtension(Uri.AbsolutePath) : null;

        [CanBeNull]
        public Uri Uri
        {
            get { return uri; }
            set
            {
                if (Equals(value, uri)) return;
                uri = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
            }
        }

        #endregion

        protected Document([NotNull] Database database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            Database = database;
        }

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        public XmlSchema GetSchema()
        {
            return null;
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void ReadXml(XmlReader reader);

        public abstract void WriteXml(XmlWriter writer);

        #endregion
    }
}
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

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Cadoscopia.Parametric.SketchServices
{
    // ReSharper disable once UseNameofExpression
    [DebuggerDisplay("{Value}")]
    public class Parameter : INotifyPropertyChanging, IXmlSerializable
    {
        #region Fields

        double value;

        #endregion

        #region Properties

        [XmlAttribute]
        [DefaultValue(0.0)]
        public double Value
        {
            get { return value; }
            set
            {
                OnPropertyChanging();
                this.value = value;
            }
        }

        #endregion

        #region Events

        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Constructors

        public Parameter()
        {
        }

        public Parameter(double value)
        {
            this.value = value;
        }

        #endregion

        #region Methods

        public XmlSchema GetSchema()
        {
            return null;
        }

        protected void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        public void ReadXml(XmlReader reader)
        {
            string attribute = reader.GetAttribute(nameof(Value));
            if (attribute != null)
                Value = double.Parse(attribute, CultureInfo.InvariantCulture);
            reader.ReadStartElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(nameof(Value),
                Value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
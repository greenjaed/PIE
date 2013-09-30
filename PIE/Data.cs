using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    class Data
    {
        protected Data parentData;
        //public Byte[] dataBytes { get; set; }
        public long customStart { get; set; }
        public long start { get { return _start; } }
        protected long _start;
        public long size { get { return _size; } }
        protected long _size;
        public IByteProvider dataByteProvider { get { return _dataByteProvider; } }
        protected IByteProvider _dataByteProvider;
        protected HexBox display;

        public Data(DynamicFileByteProvider fileByteProvider)
        {
            this._dataByteProvider = fileByteProvider;
            _size = fileByteProvider.Length;
        }

        public Data(Data parentData, long start, long size)
        {
            this._start = start;
            this._size = size;
            this.parentData = parentData;
        }

        private void setByteProvider()
        {
            List<Byte> bytes = new List<byte>();

            for (int i = 0; i < _size; ++i)
                bytes.Add(parentData._dataByteProvider.ReadByte(_start + i));
            _dataByteProvider = new DynamicByteProvider(bytes);

        }

        public void fillAddresses(ToolStripComboBox addrSelector)
        {
            addrSelector.Items.Clear();
            addrSelector.Items.AddRange(new string[] { "0", _start.ToString("X") });
            if (customStart != 0)
                addrSelector.Items.Add(customStart.ToString("X"));
        }

        //Displays the data
        public virtual void Display(Control.ControlCollection displays)
        {
            if (display == null)
                display = displays["displayHexBox"] as HexBox;
            if (_dataByteProvider == null)
                setByteProvider();
            display.ByteProvider = _dataByteProvider;
            display.Visible = true;
        }

        //Hides the data
        public virtual void Hide()
        {
            display.ByteProvider = null;
            display.Visible = false;
        }

        public virtual void Cut()
        {
            display.Cut();
        }

        public virtual void Paste()
        {
            display.Paste();
        }

        public virtual void Copy()
        {
            display.Copy();
        }

        public virtual void save()
        {
            DynamicByteProvider temp = _dataByteProvider as DynamicByteProvider;
            IByteProvider tempParent = parentData._dataByteProvider;
            if (temp != null && temp.HasChanges())
            {
                tempParent.DeleteBytes(_start, _size);
                tempParent.InsertBytes(_start, temp.Bytes.ToArray());
            }
        }

    }
}

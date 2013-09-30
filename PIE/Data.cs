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
        public long start { get; protected set; }
        public long size { get; protected set; }
        public IByteProvider dataByteProvider { get; protected set; }
        protected HexBox display;

        public Data(DynamicFileByteProvider fileByteProvider)
        {
            this.dataByteProvider = fileByteProvider;
            size = fileByteProvider.Length;
        }

        public Data(Data parentData, long start, long size)
        {
            this.start = start;
            this.size = size;
            this.parentData = parentData;
        }

        private void setByteProvider()
        {
            List<Byte> bytes = new List<byte>();

            for (int i = 0; i < size; ++i)
                bytes.Add(parentData.dataByteProvider.ReadByte(start + i));
            dataByteProvider = new DynamicByteProvider(bytes);

        }

        public void fillAddresses(ToolStripComboBox addrSelector)
        {
            addrSelector.Items.Clear();
            addrSelector.Items.AddRange(new string[] { "0", start.ToString("X") });
            if (customStart != 0)
                addrSelector.Items.Add(customStart.ToString("X"));
        }

        //Displays the data
        public virtual void Display(Control.ControlCollection displays)
        {
            if (display == null)
                display = displays["displayHexBox"] as HexBox;
            if (dataByteProvider == null)
                setByteProvider();
            display.ByteProvider = dataByteProvider;
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
            DynamicByteProvider temp = dataByteProvider as DynamicByteProvider;
            IByteProvider tempParent = parentData.dataByteProvider;
            if (temp != null && temp.HasChanges())
            {
                tempParent.DeleteBytes(start, size);
                tempParent.InsertBytes(start, temp.Bytes.ToArray());
            }
        }

    }
}

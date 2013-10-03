using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    public class Data
    {
        protected Data parentData;
        //public Byte[] dataBytes { get; set; }
        public long customStart { get; set; }
        public long start { get; protected set; }
        public long size { get; protected set; }
        public long end { get; protected set; }
        public IByteProvider dataByteProvider { get; protected set; }
        protected HexBox display;

        public Data(DynamicFileByteProvider fileByteProvider)
        {
            this.dataByteProvider = fileByteProvider;
            size = fileByteProvider.Length;
            end = size - 1;
        }

        public Data(Data parentData, long start, long size)
        {
            this.start = start;
            this.size = size;
            end = (start + size) - 1;
            this.parentData = parentData;
        }

        public void setByteProvider()
        {
            List<Byte> bytes = new List<byte>();

            //can happen if a project file has just been opened
            if (parentData.dataByteProvider == null)
                parentData.setByteProvider();

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

        public void resize(long start, long end)
        {
            this.start = start;
            this.end = end;
            size = (end - start) + 1;
            dataByteProvider = null;
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

        /*checks the Data object passed in against all nodes of the same tier
         *if the new Data object's data range is found overlapping with any existing objects,
         *it returns false.  Otherwise the method returns true.
         */
        public static bool IsTaken(TreeNode toSlice, Data toCheck)
        {
            return IsTaken(toSlice, toCheck.start, toCheck.end);
        }

        public static bool IsTaken(TreeNode beingSliced, long start, long end)
        {
            TreeNodeCollection currentTier = beingSliced.Nodes;
            Data currentData;

            foreach (TreeNode d in currentTier)
            {
                currentData = d.Tag as Data;

                //if they don't overlap, continue
                if (end < currentData.start || start > currentData.end)
                    continue;
                else
                    return true;
            }
            return false;
        }
    }
}

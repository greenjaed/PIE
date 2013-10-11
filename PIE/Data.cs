using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Be.Windows.Forms;

namespace PIE
{
    public class Data
    {
        protected Data parentData;
        //public Byte[] dataBytes { get; set; }
        public long customStart { get; set; }
        public long lastStart { get; protected set; }
        public long start { get; protected set; }
        public long size { get; protected set; }
        public long end { get; protected set; }
        [XmlIgnore]
        public IByteProvider dataByteProvider { get; protected set; }
        [XmlIgnore]
        public bool isChanged
        {
            get { return dataByteProvider != null ? dataByteProvider.HasChanges() : false; }
        }
        [XmlIgnore]
        public HexBox display { protected get; set; }

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
            end = start + size - 1;
            this.parentData = parentData;
            lastStart = start;
        }

        private void SetByteProvider()
        {
            List<Byte> bytes = new List<byte>();

            //can happen if a project file has just been opened
            if (parentData.dataByteProvider == null)
                parentData.SetByteProvider();

            for (int i = 0; i < size; ++i)
                bytes.Add(parentData.dataByteProvider.ReadByte(start + i));
            dataByteProvider = new DynamicByteProvider(bytes);
            dataByteProvider.LengthChanged += new EventHandler(dataByteProvider_LengthChanged);
        }

        void dataByteProvider_LengthChanged(object sender, EventArgs e)
        {
            if (dataByteProvider.Length > size)
                dataByteProvider.DeleteBytes(size, dataByteProvider.Length - size);
            else if (dataByteProvider.Length < size)
                dataByteProvider.InsertBytes(dataByteProvider.Length, new byte[size - dataByteProvider.Length]);
        }

        public void FillAddresses(ToolStripComboBox addrSelector)
        {
            addrSelector.Items.Clear();
            addrSelector.Items.Add("0");
            if (start != 0)
                addrSelector.Items.Add(start.ToString("X"));
            if (customStart != 0)
                addrSelector.Items.Add(customStart.ToString("X"));
            addrSelector.Items.Add(size.ToString("X"));
            if (addrSelector.Text != "")
                addrSelector.Text = lastStart.ToString("X");
        }

        //Displays the data
        public virtual void Display()
        {
            if (dataByteProvider == null)
                SetByteProvider();
            display.ByteProvider = dataByteProvider;
            display.LineInfoOffset = lastStart;
            display.Visible = true;
        }

        public void ChangeOffset(long offset)
        {
            display.LineInfoOffset = offset;
            lastStart = offset;
            if (offset != 0 || offset != start)
                customStart = offset;
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

        public virtual void Delete()
        {
            dataByteProvider.DeleteBytes(display.SelectionStart, display.SelectionLength);
        }

        public void Resize(TreeNode node, long start, long end)
        {
            this.start = start;
            this.end = end;
            size = (end - start) + 1;
            if (parentData != null)
                dataByteProvider = null;
            
            foreach (TreeNode t in node.Nodes)
                resize(t, start, end);
            for (int i = 0; i < node.Nodes.Count; ++i)
            {
                while (i < node.Nodes.Count && node.Nodes[i].Tag == null)
                    node.Nodes[i].Remove();
            }
        }

        private void resize(TreeNode node, long start, long end)
        {
            Data current = node.Tag as Data;
            long nStart = current.start;
            long nEnd = current.end;

            if (current.end < start || current.start > end)
                node.Tag = null;
            else
            {
                if (current.start < start)
                    nStart = start;
                if (current.end > end)
                    nEnd = end;
                if (nStart != current.start || nEnd != current.end)
                    current.Resize(node, nStart, nEnd);
            }
        }

        public virtual void Save()
        {
            Save(true);
        }

        //saves the data and propagates the data to the parent
        public virtual void Save(bool propagateUp)
        {
            if (dataByteProvider.HasChanges())
            {
                DynamicByteProvider temp = dataByteProvider as DynamicByteProvider;
                
                //if Data is the root, save the changes
                if (temp == null)
                    dataByteProvider.ApplyChanges();
                //otherwise, propagate the changes up to the parent(s)
                else
                {
                    if (propagateUp)
                        SaveUp(temp);
                    else
                    {
                        parentData.dataByteProvider.DeleteBytes(start, size);
                        parentData.dataByteProvider.InsertBytes(start, temp.Bytes.ToArray());
                    }
                }
            }
        }

        private void SaveUp(DynamicByteProvider temp)
        {
            Byte[] changes = temp.Bytes.ToArray(); //the changes
            Data parent = this.parentData; //the parent
            IByteProvider tempParent; //temporary parent data
            long currentStart = start; //the start address to insert at
            bool hadChanges;

            do
            {
                hadChanges = parent.isChanged;
                tempParent = parent.dataByteProvider;
                tempParent.DeleteBytes(currentStart, size);
                tempParent.InsertBytes(currentStart, changes);
                //if the only changes were those that occurred when inserting the new data, apply them
                if (!hadChanges)
                    parent.dataByteProvider.ApplyChanges();
                currentStart += parent.start;
                parent = parent.parentData;
            } while (parent != null);

            temp.ApplyChanges();
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

                if (start > currentData.end)
                    continue;
                else if (end < currentData.start)
                    break;
                else
                    return true;
            }
            return false;
        }

        public void invalidate()
        {
            dataByteProvider = null;
        }
    }
}

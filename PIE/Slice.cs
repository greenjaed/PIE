using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Be.Windows.Forms;

namespace PIE
{
    [DataContract]
    public class Slice
    {
        protected Slice parentSlice;
        [DataMember]
        public long customStart { get; set; }
        [DataMember]
        public long lastStart { get; protected set; }
        [DataMember]
        public long start { get; protected set; }
        [DataMember]
        public long size { get; protected set; }
        [DataMember]
        public long end { get; protected set; }
        public IByteProvider dataByteProvider { get; protected set; }
        public bool isChanged
        {
            get { return dataByteProvider != null ? dataByteProvider.HasChanges() : false; }
        }
        public HexBox display { get; set; }

        public Slice()
        {

        }

        public Slice(Slice source, Slice parent)
        {
            start = source.start;
            size = source.size;
            end = source.end;
            parentSlice = parent;
        }

        public Slice(DynamicFileByteProvider fileByteProvider)
        {
            this.dataByteProvider = fileByteProvider;
            size = fileByteProvider.Length;
            end = size - 1;
        }

        public Slice(Slice parentData, long start, long size)
        {
            this.start = start;
            this.size = size;
            end = start + size - 1;
            this.parentSlice = parentData;
            lastStart = start;
        }

        //updates the main slice ONLY
        public void updateMainSlice(DynamicFileByteProvider fileByteProvider)
        {
            if (parentSlice == null)
                dataByteProvider = fileByteProvider;
        }

        private byte[] getBytes(long start, long size)
        {
            if (dataByteProvider != null)
            {
                byte[] bytes = new byte[size];

                for (int i = 0; i < size; ++i)
                    bytes[i] = dataByteProvider.ReadByte(start + i);
                return bytes;
            }
            else
                return parentSlice.getBytes(start + this.start, size);
        }

        private void setBytes(long start, byte[] toInsert)
        {
            if (dataByteProvider != null)
            {
                dataByteProvider.DeleteBytes(start, toInsert.Length);
                dataByteProvider.InsertBytes(start, toInsert);
            }
            else
                parentSlice.setBytes(start + this.start, toInsert);
        }

        private void SetByteProvider(byte[] bytes)
        {
            if (dataByteProvider != null)
                dataByteProvider.LengthChanged -= dataByteProvider_LengthChanged;
            dataByteProvider = new DynamicByteProvider(bytes);
            dataByteProvider.LengthChanged += new EventHandler(dataByteProvider_LengthChanged);
        }

        protected void dataByteProvider_LengthChanged(object sender, EventArgs e)
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
                SetByteProvider(parentSlice.getBytes(start, size));
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
            if (parentSlice != null)
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
            Slice current = node.Tag as Slice;
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
                        parentSlice.setBytes(start, temp.Bytes.ToArray());
                }
            }
        }

        private void SaveUp(DynamicByteProvider temp)
        {
            Byte[] changes = temp.Bytes.ToArray(); //the changes
            Slice parent = this.parentSlice; //the parent
            IByteProvider tempParent; //temporary parent data
            long currentStart = start; //the start address to insert at
            bool hadChanges;

            do
            {
                tempParent = parent.dataByteProvider;
                if (tempParent != null)
                {
                    hadChanges = parent.isChanged;
                    tempParent.DeleteBytes(currentStart, size);
                    tempParent.InsertBytes(currentStart, changes);
                    //if the only changes were those that occurred when inserting the new data, apply them
                    if (!hadChanges)
                        parent.dataByteProvider.ApplyChanges();
                }
                currentStart += parent.start;
                parent = parent.parentSlice;
            } while (parent != null);

            temp.ApplyChanges();
        }

        /*checks the Data object passed in against all nodes of the same tier
         *if the new Data object's data range is found overlapping with any existing objects,
         *it returns false.  Otherwise the method returns true.
         */
        public static bool IsTaken(TreeNode toSlice, Slice toCheck)
        {
            return IsTaken(toSlice, toCheck.start, toCheck.end);
        }

        public static bool IsTaken(TreeNode beingSliced, long start, long end)
        {
            TreeNodeCollection currentTier = beingSliced.Nodes;
            Slice currentData;

            foreach (TreeNode d in currentTier)
            {
                currentData = d.Tag as Slice;

                if (start > currentData.end)
                    continue;
                else if (end < currentData.start)
                    break;
                else
                    return true;
            }
            return false;
        }

        public void Invalidate()
        {
            dataByteProvider = null;
        }

        public void Serialize(XmlWriter writer)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(Slice));
            dcs.WriteObject(writer, this);
        }

        public virtual void Export(string fileName)
        {
            using (FileStream export = new FileStream(fileName, FileMode.Create))
            {
                byte[] sliceBytes = (dataByteProvider as DynamicByteProvider).Bytes.ToArray();
                export.Write(sliceBytes, 0, sliceBytes.Length);
            }
        }

        public virtual void Import(string fileName)
        {
            long readLength;
            byte[] bytes;
            using (FileStream import = new FileStream(fileName, FileMode.Open))
            {
                readLength = Math.Min(size, import.Length);
                bytes = new byte[readLength];
                import.Read(bytes, 0, (int) readLength);
                SetByteProvider(bytes);
                if (readLength < size)
                    dataByteProvider.InsertBytes(readLength, new byte[size - readLength]);
            }
        }

        public static Slice Deserialize(XmlReader reader)
        {
            DataContractSerializer deserializer = new DataContractSerializer(typeof(Slice));
            return (Slice) deserializer.ReadObject(reader);
        }
    }
}

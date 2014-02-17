using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Be.Windows.Forms;

namespace PIE
{
    [DataContract]
    public class Slice
    {
        //a custom start address for displaying the data
        [DataMember]
        public long customStart { get; set; }
        //the HexBox showing the data
        public HexBox display { get; set; }
        //the end address in parentSlice where the data comes from
        [DataMember]
        public long end { get; protected set; }
        //indicates if the slice has been changed or not. if no data, returns false
        public bool isChanged
        {
            get { return dataByteProvider != null ? dataByteProvider.HasChanges() : false; }
        }
        //the last selected start address
        [DataMember]
        public long lastStart { get; protected set; }
        //stores notes about the slice
        [DataMember]
        public string notes { get; set; }
        //the slice the current slice is contained in
        protected Slice parentSlice;
        //the size of the slice
        [DataMember]
        public long size { get; protected set; }
        //the start address in parentSlice where the data comes from
        [DataMember]
        public long start { get; protected set; }
        //the data itself
        public IByteProvider dataByteProvider { get; protected set; }

        //required for serialization
        public Slice()
        {

        }

        //copy constructor with a different parent
        public Slice(Slice source, Slice parent)
        {
            customStart = source.customStart;
            lastStart = source.lastStart;
            start = source.start;
            size = source.size;
            end = source.end;
            notes = source.notes;
            parentSlice = parent;
        }

        //constructs the main slice with the fileByteProvider
        public Slice(DynamicFileByteProvider fileByteProvider)
        {
            this.dataByteProvider = fileByteProvider;
            size = fileByteProvider.Length;
            end = size - 1;
        }

        //constructs all other slices
        public Slice(Slice parentSlice, long start, long size)
        {
            this.start = start;
            this.size = size;
            end = start + size - 1;
            this.parentSlice = parentSlice;
            lastStart = start;
        }

        //updates the main slice ONLY
        public void updateMainSlice(DynamicFileByteProvider fileByteProvider)
        {
            if (parentSlice == null)
                dataByteProvider = fileByteProvider;
        }

        //gets the bytes for the slice. if none are present, retrieves them from the parent
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

        //recurses up the slices until a nonempty dataByteProvider is found and saves toInsert to it
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

        //sets dataByteProvider
        private void SetByteProvider(byte[] bytes)
        {
            if (dataByteProvider != null)
                dataByteProvider.LengthChanged -= dataByteProvider_LengthChanged;
            dataByteProvider = new DynamicByteProvider(bytes);
            dataByteProvider.LengthChanged += new EventHandler(dataByteProvider_LengthChanged);
        }

        //when editing a slice, it must stay the same size
        //this event will never be associated with the main slice
        protected void dataByteProvider_LengthChanged(object sender, EventArgs e)
        {
            if (dataByteProvider.Length > size)
                dataByteProvider.DeleteBytes(size, dataByteProvider.Length - size);
            else if (dataByteProvider.Length < size)
                dataByteProvider.InsertBytes(dataByteProvider.Length, new byte[size - dataByteProvider.Length]);
        }

        //adds the start addresses to addrSelector 
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

        //changes the address offset
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

        //resizes the slice and internal slices
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

        //recursively resizes internal slices
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

        //saves the data if propagateUp is true, propagates the data to the parent
        public virtual void Save(bool propagateUp)
        {
            if (dataByteProvider.HasChanges())
            {
                dataByteProvider.ApplyChanges();
                DynamicByteProvider temp = dataByteProvider as DynamicByteProvider;

                //if Data is not the root, propagate the changes up the slice(s)
                if (temp != null)
                {
                    byte[] bytes = temp.Bytes.ToArray();

                    if (propagateUp)
                        SaveUp(bytes);
                    else
                        parentSlice.setBytes(start, bytes);
                }
            }
        }

        //saves any changes and applies the changes up the slices
        private void SaveUp(byte[] changes)
        {
            Slice parent = this.parentSlice; //the parent
            IByteProvider tempParent; //temporary parent data
            long currentStart = start; //the start address to insert at
            bool hadChanges; //indicates the state of the data before insertion

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
        }

        /*checks the Slice object passed in against all slices of the same tier
         *if the new Slice object's data range is found overlapping with any existing objects,
         *it returns false.  Otherwise it returns true.
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

        //serializes the slice
        public virtual void Serialize(XmlDictionaryWriter writer)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(Slice));
            dcs.WriteObject(writer, this);
        }

        //deserializes a Slice and returns it
        public static Slice Deserialize(XmlDictionaryReader reader)
        {
            DataContractSerializer deserializer = new DataContractSerializer(typeof(Slice));
            return (Slice) deserializer.ReadObject(reader);
        }

        //exports the slice
        public virtual void Export(string fileName)
        {
            using (FileStream export = new FileStream(fileName, FileMode.Create))
            {
                byte[] sliceBytes = (dataByteProvider as DynamicByteProvider).Bytes.ToArray();
                export.Write(sliceBytes, 0, sliceBytes.Length);
            }
        }

        //imports a slice.
        //If the data is bigger than the slice, the data is truncated
        //if the data is smaller than the slice, the slice is padded
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
    }
}

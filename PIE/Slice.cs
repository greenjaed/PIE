using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Be.Windows.Forms;
using System.Collections.Generic;

namespace PIE
{
    //Describes an object for partitioning, viewing, and editing data
    [DataContract]
    public class Slice
    {
        //a custom start address for displaying the data
        [DataMember]
        protected long CustomStart;
        //the end address in parentSlice where the data comes from
        [DataMember]
        public long End { get; protected set; }
        //indicates if the slice has been changed or not. if no data, returns false
        public bool IsChanged
        {
            get { return DataByteProvider != null ? DataByteProvider.HasChanges() : false; }
        }
        //the last selected start address
        [DataMember]
        protected long LastStart;
        //stores notes about the slice
        [DataMember]
        public string Notes { get; set; }
        //the slice the current one is contained in
        protected Slice ParentSlice;
        //the size of the slice
        [DataMember]
        public long Size { get; protected set; }
        //the start address in parentSlice where the data comes from
        [DataMember]
        public long Start { get; protected set; }
        //the data itself
        protected IByteProvider DataByteProvider;

        public IByteProvider Data
        {
            get
            {
                if (DataByteProvider == null)
                {
                    setByteProvider(ParentSlice.getBytes(Start, Size));
                }
                return DataByteProvider;
            }
        }

        public Slice Parent
        {
            set
            {
                if (value != null)
                {
                    ParentSlice = value;
                }
            }
        }

        public virtual long Offset
        {
            get
            {
                return LastStart;
            }
            set
            {
                LastStart = value;
                if (value != 0 || value != Start)
                {
                    CustomStart = value;
                }
            }
        }

        //required for serialization
        public Slice()  { }

        public Slice(Slice toCopy) : this(toCopy, toCopy.ParentSlice) { }

        public Slice(Slice source, Slice parent)
        {
            CustomStart = source.CustomStart;
            LastStart = source.LastStart;
            Start = source.Start;
            Size = source.Size;
            End = source.End;
            Notes = source.Notes;
            ParentSlice = source.ParentSlice;
            ParentSlice = parent;
        }

        //constructs the main slice with the fileByteProvider
        public Slice(DynamicFileByteProvider fileByteProvider)
        {
            this.DataByteProvider = fileByteProvider;
            Size = fileByteProvider.Length;
            End = Size - 1;
        }

        //constructs all other slices
        public Slice(Slice parentSlice, long start, long size)
        {
            this.Start = start;
            this.Size = size;
            End = start + size - 1;
            this.ParentSlice = parentSlice;
            LastStart = start;
        }

        //updates the main slice ONLY
        public void SetMainSlice(DynamicFileByteProvider fileByteProvider)
        {
            if (ParentSlice == null)
            {
                DataByteProvider = fileByteProvider;
            }
        }

        //gets the bytes for the slice. if none are present, retrieves them from the parent
        private byte[] getBytes(long start, long size)
        {
            if (DataByteProvider != null)
            {
                byte[] bytes = new byte[size];

                for (int i = 0; i < size; ++i)
                {
                    bytes[i] = DataByteProvider.ReadByte(start + i);
                }
                return bytes;
            }
            else
            {
                return ParentSlice.getBytes(start + this.Start, size);
            }
        }

        //recurses up the slices until a nonempty dataByteProvider is found and saves toInsert to it
        private void setBytes(long start, byte[] toInsert)
        {
            if (DataByteProvider != null)
            {
                DataByteProvider.DeleteBytes(start, toInsert.Length);
                DataByteProvider.InsertBytes(start, toInsert);
            }
            else
            {
                ParentSlice.setBytes(start + this.Start, toInsert);
            }
        }

        protected void setByteProvider()
        {
            setByteProvider(ParentSlice.getBytes(Start, Size));
        }

        private void setByteProvider(byte[] bytes)
        {
            if (DataByteProvider != null)
            {
                DataByteProvider.LengthChanged -= dataByteProvider_LengthChanged;
            }
            DataByteProvider = new DynamicByteProvider(bytes);
            DataByteProvider.LengthChanged += new EventHandler(dataByteProvider_LengthChanged);
        }

        //when editing a slice, it must stay the same size
        //The main slice will never trigger this event
        protected void dataByteProvider_LengthChanged(object sender, EventArgs e)
        {
            if (DataByteProvider.Length > Size)
            {
                DataByteProvider.DeleteBytes(Size, DataByteProvider.Length - Size);
            }
            else if (DataByteProvider.Length < Size)
            {
                DataByteProvider.InsertBytes(DataByteProvider.Length, new byte[Size - DataByteProvider.Length]);
            }
        }

        //adds the start addresses to addrSelector 
        public string[] Addresses()
        {
            List<long> addresses = new List<long>();
            addresses.Add(LastStart);
            addresses.Add(CustomStart);
            addresses.Add(Start);
            addresses.Add(Size);
            addresses.Add(0L);
            return addresses.Distinct().Select(a => a.ToString("X")).ToArray();
        }

        //resizes the slice and internal slices
        public void Resize(TreeNode node, long start, long end)
        {
            this.Start = start;
            this.End = end;
            Size = (end - start) + 1;
            if (ParentSlice != null)
            {
                DataByteProvider = null;
            }

            foreach (TreeNode t in node.Nodes)
            {
                resize(t, start, end);
            }
            for (int i = 0; i < node.Nodes.Count; ++i)
            {
                while (i < node.Nodes.Count && node.Nodes[i].Tag == null)
                {
                    node.Nodes[i].Remove();
                }
            }
        }

        //recursively resizes internal slices
        private void resize(TreeNode node, long start, long end)
        {
            Slice current = node.Tag as Slice;
            long nStart = current.Start;
            long nEnd = current.End;

            if (current.End < start || current.Start > end)
            {
                node.Tag = null;
            }
            else
            {
                if (current.Start < start)
                {
                    nStart = start;
                }
                if (current.End > end)
                {
                    nEnd = end;
                }
                if (nStart != current.Start || nEnd != current.End)
                {
                    current.Resize(node, nStart, nEnd);
                }
            }
        }

        //saves the data.  If propagateUp is true, propagates the data to the parent
        public virtual void Save(bool propagateUp = true)
        {
            if (DataByteProvider.HasChanges())
            {
                DataByteProvider.ApplyChanges();
                DynamicByteProvider byteProvider = DataByteProvider as DynamicByteProvider;

                //if Data is not the root, propagate the changes up the slice(s)
                if (byteProvider != null)
                {
                    byte[] bytes = byteProvider.Bytes.ToArray();

                    if (propagateUp)
                    {
                        saveUp(bytes);
                    }
                    else
                    {
                        ParentSlice.setBytes(Start, bytes);
                    }
                }
            }
        }

        //Changes parent and addresses of slice
        public void Merge(Slice parent, long offset)
        {
            ParentSlice = parent;
            Start -= offset;
            End -= offset;
        }

        //Dissolves the parent slice and moves this one up a level
        public void Split()
        {
            Start += ParentSlice.Start;
            End += ParentSlice.Start;
            ParentSlice = ParentSlice.ParentSlice;
        }

        //saves any changes and applies the changes up the slices
        private void saveUp(byte[] changes)
        {
            Slice parent = this.ParentSlice;
            IByteProvider parentByteProvider;
            long currentStart = Start;
            bool hadChanges;

            do
            {
                parentByteProvider = parent.DataByteProvider;
                if (parentByteProvider != null)
                {
                    hadChanges = parent.IsChanged;
                    parentByteProvider.DeleteBytes(currentStart, Size);
                    parentByteProvider.InsertBytes(currentStart, changes);
                    //if the only changes were those that occurred when inserting the new data, apply them
                    if (!hadChanges)
                    {
                        parent.DataByteProvider.ApplyChanges();
                    }
                }
                currentStart += parent.Start;
                parent = parent.ParentSlice;
            } while (parent != null);
        }

        //Wrapper for IsTaken
        public static bool IsTaken(TreeNode toSlice, Slice toCheck)
        {
            return IsTaken(toSlice, toCheck.Start, toCheck.End);
        }

        /*checks the Slice object passed in against all slices of the same tier
         *if the new Slice object's data range is found overlapping with any existing objects,
         *it returns false.  Otherwise it returns true.
         */
        public static bool IsTaken(TreeNode beingSliced, long start, long end)
        {
            TreeNodeCollection currentTier = beingSliced.Nodes;
            Slice currentData;

            foreach (TreeNode d in currentTier)
            {
                currentData = d.Tag as Slice;

                if (start > currentData.End)
                {
                    continue;
                }
                else if (end < currentData.Start)
                {
                    break;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public void Invalidate()
        {
            DataByteProvider = null;
        }

        public virtual void Serialize(XmlDictionaryWriter writer)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(Slice));
            dcs.WriteObject(writer, this);
        }

        public static Slice Deserialize(XmlDictionaryReader reader)
        {
            DataContractSerializer deserializer = new DataContractSerializer(typeof(Slice));
            return (Slice) deserializer.ReadObject(reader);
        }

        //exports the slice to a file
        public virtual void Export(string fileName)
        {
            using (FileStream export = new FileStream(fileName, FileMode.Create))
            {
                byte[] sliceBytes = (DataByteProvider as DynamicByteProvider).Bytes.ToArray();
                export.Write(sliceBytes, 0, sliceBytes.Length);
            }
        }

        //imports a slice.
        //If the data is bigger than the slice, the data is truncated
        //if the data is smaller than the slice, the slice is padded with 0s
        public virtual void Import(string fileName)
        {
            long readLength;
            byte[] bytes;
            using (FileStream import = new FileStream(fileName, FileMode.Open))
            {
                readLength = Math.Min(Size, import.Length);
                bytes = new byte[readLength];
                import.Read(bytes, 0, (int) readLength);
                setByteProvider(bytes);
                if (readLength < Size)
                {
                    DataByteProvider.InsertBytes(readLength, new byte[Size - readLength]);
                }
            }
        }

        public void Insert(long position, int amount)
        {
            DataByteProvider.InsertBytes(position, new byte[amount]);
        }

        public void Clear(long position, int amount)
        {
            DataByteProvider.RemoveBytes(position, amount);
            Insert(position, amount);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    //stores information about the data in  a table column
    [DataContract]
    public class ColumnDescriptor
    {
        //The column name
        [DataMember]
        public string Name;
        //The data type
        [DataMember]
        public string DataType;
        //The size of the data type, in bytes
        [DataMember]
        public int Size;
        //for ints: how the int is formatted
        [DataMember]
        public IntFormat IntFormat;
        //for fixed: the size of the fraction, in bits
        [DataMember]
        public int Fraction;

        public override string ToString()
        {
            return Name;
        }
    }

    public enum IntFormat
    {
        None,
        Signed,
        Hex
    }

    //A slice of data interpreted as a table
    [DataContract]
    public class TableSlice : Slice
    {
        //The table holding the data
        private DataTable sourceTable;
        //The info describing the columns
        [DataMember]
        public ColumnDescriptor[] ColumnInfo { get; protected set; }
        //array of converters to help serialize/deserialize data
        public ICrossConverter[] DataConverters { get; protected set; }
        //the number of bytes in a row
        public int RowLength { get; protected set; }
        //the number of cells in the partial row
        public int PartialRowIndex { get; protected set; }
        //indicates if the table needs to be regenerated
        private bool regenerateConverters;
        //Gets the current offset
        public override long Offset
        {
            get
            {
                return base.Offset;
            }
            set
            {
                base.Offset = value;
                //In order to display the new offset, the table needs to be regenerated
                regenerateTable(value);
            }
        }

        public DataTable Table
        {
            get
            {
                if (sourceTable == null)
                {
                    regenerateTable(Offset);
                }
                return sourceTable;
            }
        }

        public TableSlice()
            : base()
        {

        }

        //copy constructor to convert between classes
        public TableSlice(Slice toCopy)
            : base(toCopy)
        {

        }

        public TableSlice(Slice source, Slice parent)
            : base(source, parent)
        {

        }

        public TableSlice(Slice parentSlice, long start, long size)
            : base(parentSlice, start, size)
        {

        }

        private void regenerateTable(long seedValue)
        {
            var table = createTable(seedValue);
            int count = sourceTable.Columns.Count;
            foreach (DataRow row in sourceTable.Rows)
            {
                DataRow copy = table.NewRow();
                for (int i = 1; i < count; ++i)
                {
                    copy[i] = row[i];
                }
                table.Rows.Add(copy);
            }
            sourceTable = table;
        }

        //creates the data converters used to generate the table
        private void getConverters()
        {
            DataConverters = new ICrossConverter[ColumnInfo.Length];
            for (int i = 0; i < DataConverters.Length; ++i)
            {
                DataConverters[i] = CrossConverterFactory.SelectConverter(ColumnInfo[i]);
            }
        }

        //calculates the row length
        public int calcRowLength()
        {
            RowLength = 0;
            foreach (ColumnDescriptor di in ColumnInfo)
            {
                RowLength += di.Size >> 3;
            }
            return RowLength;
        }

        //converts the bytes to their respective type string representations and puts them in a table
        private void fillTable()
        {
            Byte[] data = (DataByteProvider as DynamicByteProvider).Bytes.ToArray();
            int index = 0;
            DataRow rowNew;

            //Disabling event handler while mucking around with the table
            sourceTable.ColumnChanging -= sourceTable_ColumnChanging;
            while (index + RowLength <= Size)
            {
                rowNew = sourceTable.NewRow();
                for (int i = 0; i < ColumnInfo.Length; ++i)
                {
                    //array placement is offset because the first row is the address
                    rowNew[i + 1] = DataConverters[i].ToString(data, index);
                    index += ColumnInfo[i].Size >> 3;
                }
                sourceTable.Rows.Add(rowNew);
            }
            //If there's a partial row's worth of data, put it into a row
            if (index < Size)
            {
                rowNew = sourceTable.NewRow();
                for (int i = 0; i < ColumnInfo.Length; ++i)
                {
                    if (index + (ColumnInfo[i].Size >> 3) > Size)
                    {
                        rowNew[i + 1] = "partial";
                        PartialRowIndex = i;
                        break;
                    }
                    rowNew[i + 1] = DataConverters[i].ToString(data, index);
                    index += ColumnInfo[i].Size >> 3;
                }
                sourceTable.Rows.Add(rowNew);
            }
            sourceTable.ColumnChanging += new DataColumnChangeEventHandler(sourceTable_ColumnChanging);
        }

        //Displays the ColumnForm and gets the edited columns
        public bool EditColumns()
        {
            ColumnForm columnForm = new ColumnForm(this);
            bool result = columnForm.ShowDialog() == DialogResult.OK;
            regenerateConverters = result;
            return result;
        }

        //creates the columnInfo array and creates the columns
        public void AddColumns(IEnumerable<ColumnDescriptor> columns)
        {
            ColumnInfo = columns.ToArray();
            calcRowLength();
        }

        //initializes the dataTable
        private DataTable createTable(long seedValue)
        {
            DataTable freshTable = new DataTable();
            if (sourceTable != null)
            {
                sourceTable.ColumnChanging -= sourceTable_ColumnChanging;
            }
            freshTable.ColumnChanging += new DataColumnChangeEventHandler(sourceTable_ColumnChanging);
            DataColumn addrColumn = new DataColumn("Address", typeof(int));
            addrColumn.AutoIncrement = true;
            addrColumn.AutoIncrementSeed = seedValue;
            addrColumn.AutoIncrementStep = RowLength;
            addrColumn.ReadOnly = true;
            freshTable.Columns.Add(addrColumn);
            foreach (ColumnDescriptor columnInfo in ColumnInfo)
            {
                freshTable.Columns.Add(columnInfo.Name, typeof(string));
            }
            if (regenerateConverters)
            {
                DataConverters = new ICrossConverter[ColumnInfo.Length];
                for (int i = 0; i < ColumnInfo.Length; ++i)
                {
                    DataConverters[i] = CrossConverterFactory.SelectConverter(ColumnInfo[i]);
                }
                regenerateConverters = false;
            }
            return freshTable;
        }

        //when the table is altered, move the changes to the data source
        private void sourceTable_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            byte[] changes;
            DynamicByteProvider data = DataByteProvider as DynamicByteProvider;
            if (DataConverters[e.Column.Ordinal - 1].TryToBytes(e.ProposedValue as string, out changes))
            {
                int insertPoint = (int)e.Row["Address"] - (int)Offset;
                for (int i = 1; i < e.Column.Ordinal; ++i)
                {
                    insertPoint += ColumnInfo[i - 1].Size;
                }
                //if the entry point is invalid, clear the data
                if (insertPoint > Size - 1)
                {
                    e.ProposedValue = string.Empty;
                    return;
                }
                data.Bytes.RemoveRange(insertPoint, changes.Length);
                data.Bytes.InsertRange(insertPoint, changes);
            }
            else
            {
                e.ProposedValue = e.Row[e.Column];
            }
        }

        public int OffsetOfCell(int columnIndex)
        {
            return ColumnInfo.Take(columnIndex).Select(c => c.Size).Sum();
        }

        public void UpdateData(byte[] data)
        {
            DataByteProvider = new DynamicFileByteProvider(data);
        }
    }
}

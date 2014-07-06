using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Be.Windows.Forms;

namespace PIE
{
    //stores information about the data in  a table column
    [DataContract]
    public class DataInfo
    {
        //The column name
        [DataMember]
        public string name;
        //The data type
        [DataMember]
        public string dataType;
        //The size of the data type, in bytes
        [DataMember]
        public int size;
        //for ints: how the int is formatted
        [DataMember]
        public IntFormat intFormat;
        //for fixed: the size of the fraction, in bits
        [DataMember]
        public int fraction;

        public override string ToString()
        {
            return name;
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
        [DataMember]
        private DataTable sourceTable;
        //The info describing the columns
        public DataInfo[] ColumnInfo { get; protected set; }
        //the control the table will be displayed in
        public DataGridView tableDisplay;
        //array of converters to help serialize/deserialize data
        public ICrossConverter[] dataConverter { get; protected set; }
        //the number of bytes in a row
        private int rowLength;
        //the number of cells in the partial row
        private int partialRowIndex;
        //indicates if the table needs to be regenerated
        private bool regenerate;
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
                tableDisplay.DataSource = null;
                createColumns();
                fillTable();
                setTable();
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

        //sets the data for the dataGridView, sets up the address column, and disables extra cells
        private void setTable()
        {
            tableDisplay.DataSource = sourceTable;
            tableDisplay.Columns[0].DefaultCellStyle.Format = "X";
            tableDisplay.Columns[0].Frozen = true;
            if (partialRowIndex > 0)
            {
                for (int i = ColumnInfo.Length + 1 - partialRowIndex; i <= ColumnInfo.Length; ++i)
                {
                    DataGridViewCell extra = tableDisplay.Rows[tableDisplay.Rows.Count - 1].Cells[i];
                    extra.ReadOnly = true;
                    extra.Style.BackColor = System.Drawing.Color.Gray;
                }
            }
        }

        //creates the data converters used to generate the table
        private void getConverters()
        {
            dataConverter = new ICrossConverter[ColumnInfo.Length];
            for (int i = 0; i < dataConverter.Length; ++i)
                dataConverter[i] = CrossConverterSelector.SelectConverter(ColumnInfo[i]);
        }

        //calculates the row length
        private void calcRowLength()
        {
            rowLength = 0;
            foreach (DataInfo di in ColumnInfo)
                rowLength += di.size >> 3;
        }

        //converts the bytes to their respective type string representations and puts them in a table
        private void fillTable()
        {
            Byte[] data = (dataByteProvider as DynamicByteProvider).Bytes.ToArray();
            int index = 0;
            DataRow rowNew;

            //Disabling event handler while mucking around with the table
            sourceTable.ColumnChanging -= sourceTable_ColumnChanging;
            while (index + rowLength <= size)
            {
                rowNew = sourceTable.NewRow();
                for (int i = 0; i < ColumnInfo.Length; ++i)
                {
                    //array placement is offset because the first row is the address
                    rowNew[i + 1] = dataConverter[i].ToString(data, index);
                    index += ColumnInfo[i].size >> 3;
                }
                sourceTable.Rows.Add(rowNew);
            }
            //If there's a partial row's worth of data, put it into a row
            if (index < size)
            {
                rowNew = sourceTable.NewRow();
                for (int i = 0; i < ColumnInfo.Length; ++i)
                {
                    if (index + (ColumnInfo[i].size >> 3) > size)
                    {
                        rowNew[i + 1] = "partial";
                        partialRowIndex = i;
                        break;
                    }
                    rowNew[i + 1] = dataConverter[i].ToString(data, index);
                    index += ColumnInfo[i].size >> 3;
                }
                sourceTable.Rows.Add(rowNew);
            }
            sourceTable.ColumnChanging += new DataColumnChangeEventHandler(sourceTable_ColumnChanging);
        }

        //Displays the table
        public override void Display()
        {
            if (dataByteProvider == null)
                setByteProvider();
            if (regenerate)
            {
                fillTable();
                regenerate = false;
            }
            setTable();
            tableDisplay.Visible = true;
            tableDisplay.BringToFront();
        }

        //Hides the table
        public override void Hide()
        {
            tableDisplay.DataSource = null;
            tableDisplay.Visible = false;
            tableDisplay.SendToBack();
        }

        //Displays the ColumnForm and gets the edited columns
        public bool EditColumns()
        {
            ColumnForm columnForm = new ColumnForm(this);
            bool result = columnForm.ShowDialog() == DialogResult.OK;
            regenerate = result;
            return result;
        }

        //creates the columnInfo array and creates the columns
        public void AddColumns(ListBox.ObjectCollection columns)
        {
            ColumnInfo = new DataInfo[columns.Count];
            columns.CopyTo(ColumnInfo, 0);
            calcRowLength();
            createColumns();
        }

        //initializes the dataTable
        private void createColumns()
        {
            if (sourceTable != null)
                sourceTable.ColumnChanging -= sourceTable_ColumnChanging;
            sourceTable = new DataTable();
            sourceTable.ColumnChanging += new DataColumnChangeEventHandler(sourceTable_ColumnChanging);
            dataConverter = new ICrossConverter[ColumnInfo.Length];
            DataColumn addrColumn = new DataColumn("Address", typeof(int));
            addrColumn.AutoIncrement = true;
            addrColumn.AutoIncrementSeed = this.lastStart;
            addrColumn.AutoIncrementStep = rowLength;
            addrColumn.ReadOnly = true;
            sourceTable.Columns.Add(addrColumn);
            for (int i = 0; i < ColumnInfo.Length; ++i)
            {
                sourceTable.Columns.Add(ColumnInfo[i].name, typeof(string));
                dataConverter[i] = CrossConverterSelector.SelectConverter(ColumnInfo[i]);
            }
        }

        //when the table is altered, move the changes to the data source
        void sourceTable_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            byte[] changes;
            DynamicByteProvider data = dataByteProvider as DynamicByteProvider;
            if (dataConverter[e.Column.Ordinal - 1].ToBytes(e.ProposedValue as string, out changes))
            {
                int insertPoint = (int)e.Row["Address"] - (int)Offset;
                for (int i = 1; i < e.Column.Ordinal; ++i)
                    insertPoint += ColumnInfo[i - 1].size;
                //if the entry point is invalid, clear the data
                if (insertPoint > size - 1)
                {
                    e.ProposedValue = "";
                    return;
                }
                data.Bytes.RemoveRange(insertPoint, changes.Length);
                data.Bytes.InsertRange(insertPoint, changes);
            }
            else
                e.ProposedValue = e.Row[e.Column];
        }

        public override void ScrollToAddress(long address)
        {
            tableDisplay.FirstDisplayedScrollingRowIndex = (int) address / rowLength;
        }

        public override void Copy()
        {
            base.Copy();
        }

        public override void SelectAll()
        {
            tableDisplay.SelectAll();
        }
    }
}

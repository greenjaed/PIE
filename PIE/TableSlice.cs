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
    [DataContract]
    public class DataInfo
    {
        [DataMember]
        public string name;
        [DataMember]
        public string dataType;
        [DataMember]
        public int size;
        [DataMember]
        public IntFormat intFormat;
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

    [DataContract]
    public class TableSlice : Slice
    {
        [DataMember]
        private DataTable sourceTable;
        public DataInfo[] ColumnInfo { get; protected set; }
        public DataGridView tableDisplay;
        public ICrossConverter[] dataConverter { get; protected set; }
        private int rowLength;
        private bool regenerate;

        public override long Offset
        {
            get
            {
                return base.Offset;
            }
            set
            {
                base.Offset = value;
                tableDisplay.DataSource = null;
                CreateColumns();
                fillTable();
                tableDisplay.DataSource = sourceTable;
                tableDisplay.Columns[0].DefaultCellStyle.Format = "X";
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

        private void getConverters()
        {
            dataConverter = new ICrossConverter[ColumnInfo.Length];
            for (int i = 0; i < dataConverter.Length; ++i)
                dataConverter[i] = CrossConverterSelector.SelectConverter(ColumnInfo[i]);
        }

        private void calcRowLength()
        {
            rowLength = 0;
            foreach (DataInfo di in ColumnInfo)
                rowLength += di.size >> 3;
        }

        private void fillTable()
        {
            Byte[] data = (dataByteProvider as DynamicByteProvider).Bytes.ToArray();
            //converts the bytes to their respective type string representations and puts them in a table
            int index = 0;
            DataRow rowNew;

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

            if (index < size)
            {
                rowNew = sourceTable.NewRow();
                for (int i = 0; i < ColumnInfo.Length; ++i)
                {
                    if (index + (ColumnInfo[i].size >> 3) > size)
                        break;
                    index += ColumnInfo[i].size >> 3;
                }
                sourceTable.Rows.Add(rowNew);
            }
            sourceTable.ColumnChanging += new DataColumnChangeEventHandler(sourceTable_ColumnChanging);
        }

        public override void Display()
        {
            if (dataByteProvider == null)
                SetByteProvider();
            if (regenerate)
                fillTable();
            tableDisplay.DataSource = sourceTable;
            tableDisplay.Columns[0].DefaultCellStyle.Format = "X";
            tableDisplay.Visible = true;
            tableDisplay.BringToFront();
        }

        public override void Hide()
        {
            tableDisplay.DataSource = null;
            tableDisplay.Visible = false;
            tableDisplay.SendToBack();
        }

        public bool EditColumns()
        {
            ColumnForm columnForm = new ColumnForm(this);
            bool result = columnForm.ShowDialog() == DialogResult.OK;
            regenerate = result;
            return result;
        }

        public void AddColumns(ListBox.ObjectCollection columns)
        {
            ColumnInfo = new DataInfo[columns.Count];
            columns.CopyTo(ColumnInfo, 0);
            calcRowLength();
            CreateColumns();
        }

        private void CreateColumns()
        {
            if (sourceTable != null)
            {
                sourceTable.ColumnChanging -= sourceTable_ColumnChanging;
            }
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

        void sourceTable_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            byte[] changes;
            DynamicByteProvider data = dataByteProvider as DynamicByteProvider;
            if (dataConverter[e.Column.Ordinal - 1].ToBytes(e.ProposedValue as string, out changes))
            {
                int insertPoint = (int)e.Row["Address"] - (int)Offset;
                for (int i = 1; i < e.Column.Ordinal; ++i)
                    insertPoint += ColumnInfo[i - 1].size;
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

        public override void scrollToAddress(long address)
        {
            tableDisplay.FirstDisplayedScrollingRowIndex = (int) address / rowLength;
        }
    }
}

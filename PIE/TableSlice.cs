using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace PIE
{
    public class DataInfo
    {
        public string name;
        public string dataType;
        public int size;
        public bool signed;
        public bool hex;
        public int fraction;

        public override string ToString()
        {
            return name;
        }
    }


    [DataContract]
    public class TableSlice : Slice
    {
        public DataGridView tableDisplay;

        public DataGridViewColumn[] Columns { get; protected set; }
        [DataMember]
        public DataInfo[] ColumnInfo { get; protected set; }

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

        public override void Display()
        {
            if (tableDisplay == null)
                return;
            //converts the bytes to their respective type string representations and puts them in a table
            tableDisplay.BringToFront();
        }

        public bool EditColumns()
        {
            ColumnForm columnForm = new ColumnForm(this);
            return columnForm.ShowDialog() == DialogResult.OK ? true : false;
        }

        public void AddColumns(ListBox.ObjectCollection columns)
        {
            ColumnInfo = new DataInfo[columns.Count];
            columns.CopyTo(ColumnInfo, 0);
            Columns = new DataGridViewColumn[columns.Count];
            for (int i = 0; i < columns.Count; ++i)
            {
                Columns[i].CellTemplate = new DataGridViewTextBoxCell();
                ColumnInfo[i].name = i.ToString();
                Columns[i].HeaderText = ColumnInfo[i].name;
            }
        }
    }
}

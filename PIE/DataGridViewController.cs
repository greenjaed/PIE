using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    class DataGridViewController : ISliceController
    {
        private PIEForm MainController;
        private DataGridView View;
        private TableSlice ModelSlice;
        private BindingSource ViewBinder;

        public Slice Model
        {
            get
            {
                return ModelSlice;
            }
            set
            {
                if (value != ModelSlice)
                {
                    ModelSlice = value as TableSlice;
                    //do stuff;
                }
            }
        }

        public long Offset
        {
            get
            {
                if (ModelSlice != null)
                {
                    return ModelSlice.Offset;
                }
                return -1;
            }
            set
            {
                if (ModelSlice != null)
                {
                    ModelSlice.Offset = value;
                    ViewBinder.DataSource = ModelSlice.Table;
                }
            }
        }

        public DataGridViewController(PIEForm mainController, DataGridView view)
        {
            MainController = mainController;
            View = view;
            ViewBinder = new BindingSource();
        }

        private void SetUpEventHandlers()
        {
            View.ColumnAdded += new DataGridViewColumnEventHandler(View_ColumnAdded);
            View.SelectionChanged += new EventHandler(View_SelectionChanged);
        }

        private void View_SelectionChanged(object sender, EventArgs e)
        {
            MainController.ToggleEnable(View.SelectedCells.Count > 0);
            UpdatePosition();
        }

        private void View_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        public void Display()
        {
            configureView();
            View.Visible = true;
            View.BringToFront();
        }

        public void Hide()
        {
            View.DataSource = null;
            View.Visible = false;
            View.SendToBack();
        }

        public void Copy()
        {
            throw new NotImplementedException();
        }

        public void Cut()
        {
            throw new NotImplementedException();
        }

        public void Paste()
        {
            throw new NotImplementedException();
        }

        public void PasteOver()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void ScrollToAddress(long address)
        {
            View.FirstDisplayedScrollingRowIndex = (int)address / ModelSlice.RowLength;
        }

        public void SelectAll()
        {
            View.SelectAll();
            View.SelectedColumns[0].Selected = false;
        }

        public void UpdatePosition()
        {
            if (ModelSlice != null)
            {
                var selectedCells = View.SelectedCells.Cast<DataGridViewCell>();
                MainController.CurrentPosition = LookUpPosition(selectedCells.First());
                long currentPosition = MainController.CurrentPosition;
                MainController.PositionDisplay.Text = currentPosition.ToString("X")
                    + (MainController.IsSelecting ? string.Format("-{0:X}", currentPosition + LookUpPosition(selectedCells.Last())) : string.Empty);
            }
        }

        private long LookUpPosition(DataGridViewCell cell)
        {
            long address = View.Rows[cell.RowIndex].Cells[0].Value;
            if (cell.ColumnIndex > 1)
            {
                address += ModelSlice.OffsetOfCell(cell.ColumnIndex - 1);
            }
            return address;
        }

        private void configureView()
        {
            ViewBinder.DataSource = ModelSlice.Table;
            View.DataSource = ViewBinder;
            View.Columns[0].DefaultCellStyle.Format = "X";
            View.Columns[0].Frozen = true;
            int partialRowIndex = ModelSlice.PartialRowIndex;
            var columnInfo = ModelSlice.ColumnInfo;

            if (partialRowIndex > 0)
            {
                for (int i = columnInfo.Length + 1 - partialRowIndex; i <= columnInfo.Length; ++i)
                {
                    DataGridViewCell extra = View.Rows[View.Rows.Count - 1].Cells[i];
                    extra.ReadOnly = true;
                    extra.Style.BackColor = System.Drawing.Color.Gray;
                }
            }
        }
    }
}

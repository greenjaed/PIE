using System;
using System.Windows.Forms;
using Be.Windows.Forms;
using System.IO;

namespace PIE
{
    class HexBoxController : ISliceController
    {
        private PIEForm MainController;
        private HexBox View;
        private Slice ModelSlice;

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
                    ModelSlice = value;
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
                View.LineInfoOffset = value;
                ModelSlice.Offset = value;
            }
        }

        public HexBoxController(PIEForm mainController, HexBox view)
        {
            MainController = mainController;
            View = view;
            SetUpEventHandlers();
        }

        private void SetUpEventHandlers()
        {
            View.Copied += new EventHandler(View_Copied);
            View.CurrentLineChanged += new EventHandler(View_CurrentLineChanged);
            View.CurrentPositionInLineChanged += new EventHandler(View_CurrentPositionInLineChanged);
            View.InsertActiveChanged += new EventHandler(View_InsertActiveChanged);
            View.SelectionLengthChanged += new EventHandler(View_SelectionLengthChanged);
        }

        private void View_SelectionLengthChanged(object sender, EventArgs e)
        {
            MainController.ToggleEnable(View.CanCopy());
            UpdatePosition();
        }

        private void View_InsertActiveChanged(object sender, EventArgs e)
        {
            MainController.InsertRemove.Text = View.InsertActive ? "Insert" : "Overwrite";
        }

        private void View_CurrentPositionInLineChanged(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        private void View_CurrentLineChanged(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        private void View_Copied(object sender, EventArgs e)
        {
            MainController.TogglePaste(true);
        }

        public void UpdatePosition()
        {
            if (ModelSlice != null)
            {
                MainController.CurrentPosition = (View.CurrentLine - 1) * View.BytesPerLine
                    + (View.CurrentPositionInLine - 1)
                    + ModelSlice.Offset;
                long currentPosition = MainController.CurrentPosition;
                MainController.PositionDisplay.Text = currentPosition.ToString("X")
                    + (MainController.IsSelecting ? string.Format("-{0:X}", currentPosition + View.SelectionLength - 1) : string.Empty);
            }
        }

        public void Display()
        {
            View.ByteProvider = ModelSlice.Data;
            View.LineInfoOffset = ModelSlice.Offset;
            View.Visible = true;
            View.BringToFront();
        }

        public void Hide()
        {
            View.ByteProvider = null;
            View.Visible = false;
            View.SendToBack();
        }

        public void Copy()
        {
            View.Copy();
        }

        public void Cut()
        {
            View.Cut();
        }

        public void Paste()
        {
            View.Paste();
        }

        public void PasteOver()
        {
            IDataObject da = Clipboard.GetDataObject();
            long size = 0;
            long start = (View.SelectionLength > 0 ? View.SelectionStart : MainController.CurrentPosition) - ModelSlice.Offset;

            //get the size of the data to paste
            //if it's copied hex:
            if (da.GetDataPresent("BinaryData"))
            {
                MemoryStream ms = (MemoryStream)da.GetData("BinaryData");
                size = ms.Length;
            }
            //if it's copied text:
            else if (da.GetDataPresent(typeof(string)))
            {
                string buffer = (string)da.GetData(typeof(string));
                size = buffer.Length;
            }

            //if there is data to paste, delete the bytes and insert the new ones
            if (size > 0)
            {
                ModelSlice.Data.DeleteBytes(start, size);
                Paste();
            }
        }

        public void Delete()
        {
            ModelSlice.Data.DeleteBytes(View.SelectionStart, View.SelectionLength);
        }

        public void Insert()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void ScrollToAddress(long address)
        {
            View.ScrollByteIntoView(address);
        }

        public void SelectAll()
        {
            View.SelectAll();
        }
    }
}

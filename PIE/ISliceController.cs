using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PIE
{
    public interface ISliceController
    {
        Slice Model { get; set; }
        long Offset { get; set; }
        void Display();
        void Hide();
        void Copy();
        void Cut();
        void Paste();
        void PasteOver();
        void Delete();
        void ScrollToAddress(long address);
        void SelectAll();
        void UpdatePosition();
    }
}

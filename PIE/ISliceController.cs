using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PIE.Slices;

namespace PIE
{
    public interface ISliceController
    {
        /// <summary>
        /// Gets or sets the model. The Model is the slice being displayed and edited.
        /// </summary>
        /// <value>The slice.</value>
        Slice Model { get; set; }
        /// <summary>
        /// Gets or sets the offset to the displayed address for the slice.
        /// </summary>
        /// <value>The offset.</value>
        long Offset { get; set; }
        /// <summary>
        /// Displays the slice.
        /// </summary>
        void Display();
        /// <summary>
        /// Hides the slice.
        /// </summary>
        void Hide();
        /// <summary>
        /// Copies the selected data.
        /// </summary>
        void Copy();
        /// <summary>
        /// Cuts the selected data.
        /// </summary>
        void Cut();
        /// <summary>
        /// Paste-inserts the data in the clipboard.
        /// </summary>
        void Paste();
        /// <summary>
        /// Paste-overwrites the data in the clipboard.
        /// </summary>
        void PasteOver();
        /// <summary>
        /// Deletes the selected data.
        /// </summary>
        void Delete();
        /// <summary>
        /// Inserts the specified amount of empty data.
        /// </summary>
        /// <param name="amount">Amount.</param>
        void Insert(int amount);
        /// <summary>
        /// Empties the data in the selection.
        /// </summary>
        void Clear();
        /// <summary>
        /// Scrolls to address.
        /// </summary>
        /// <param name="address">Address.</param>
        void ScrollToAddress(long address);
        /// <summary>
        /// Selects all data.
        /// </summary>
        void SelectAll();
        /// <summary>
        /// Updates the displayed position.
        /// </summary>
        void UpdatePosition();
        /// <summary>
        /// Performs any editing action for the view
        /// </summary>
        void Edit();
    }
}

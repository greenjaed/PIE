﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using PIE.Slices;

namespace PIE
{
    public partial class ColumnForm : Form
    {
        private TableSlice Slice;

        public ColumnForm()
        {
            InitializeComponent();
        }

        public ColumnForm(TableSlice slice)
        {
            InitializeComponent();
            this.Slice = slice;
            if (slice.ColumnInfo != null)
            {
                columnListBox.Items.AddRange(slice.ColumnInfo);
                okButton.Enabled = true;
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            int index = columnListBox.SelectedIndex;
            if (index > 0)
            {
                object temp = columnListBox.SelectedItem;
                columnListBox.Items.Remove(temp);
                columnListBox.Items.Insert(index - 1, temp);
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            int index = columnListBox.SelectedIndex;
            if (index < columnListBox.Items.Count - 1)
            {
                object temp = columnListBox.SelectedItem;
                columnListBox.Items.Remove(temp);
                columnListBox.Items.Insert(index + 1, temp);
            }

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Slice.AddColumns(columnListBox.Items.Cast<ColumnDescriptor>());
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (columnListBox.SelectedItem != null)
            {
                columnListBox.Items.Remove(columnListBox.SelectedItem);
            }
            if (columnListBox.Items.Count == 0)
            {
                removeButton.Enabled = false;
                okButton.Enabled = false;
            }
        }

        private void columnListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            editButton.Enabled = true;
            removeButton.Enabled = true;
            upButton.Enabled = columnListBox.SelectedIndex == 0 ? false : true;
            downButton.Enabled = columnListBox.SelectedIndex == columnListBox.Items.Count - 1 ? false : true;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            ColumnEditForm add = new ColumnEditForm();
            if (add.ShowDialog(this) == DialogResult.OK)
            {
                columnListBox.Items.Add(add.Column);
                okButton.Enabled = true;
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            ColumnEditForm edit = new ColumnEditForm(columnListBox.SelectedItem as ColumnDescriptor);
            if (edit.ShowDialog(this) == DialogResult.OK)
            {
                int index = columnListBox.SelectedIndex;
                columnListBox.Items.RemoveAt(index);
                columnListBox.Items.Insert(index, edit.Column);
            }
        }

        private void ColumnForm_Load(object sender, EventArgs e)
        {
            if (columnListBox.Items.Count == 0)
            {
                ColumnEditForm columnNew = new ColumnEditForm();
                if (columnNew.ShowDialog(this) == DialogResult.OK)
                {
                    columnListBox.Items.Add(columnNew.Column);
                    okButton.Enabled = true;
                }
            }
        }
    }
}

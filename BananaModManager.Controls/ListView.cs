using System;
using System.Drawing;
using System.Windows.Forms;

namespace BananaModManager.Controls
{
    public partial class ListView : System.Windows.Forms.ListView
    {
        private SelectedListViewItemCollection _draggedItems;

        public ListView()
        {
            MouseDown += GrabItem;
            MouseMove += DragItem;
            MouseUp += DropItem;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void DragItem(object sender, MouseEventArgs e)
        {
            if (_draggedItems == null)
                return;

            foreach (ListViewItem draggedItem in _draggedItems)
                draggedItem.Font = new Font(draggedItem.Font, FontStyle.Bold);
            Cursor = Cursors.SizeAll;
        }


        private void GrabItem(object sender, MouseEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control) || ModifierKeys.HasFlag(Keys.Shift))
                return;

            var hoveredItem = GetItemAt(0, e.Y);

            if (hoveredItem == null)
                return;

            if (!SelectedItems.Contains(hoveredItem))
            {
                SelectedItems.Clear();
                hoveredItem.Selected = true;
                Select();
            }

            _draggedItems = SelectedItems;
        }

        private void DropItem(object sender, MouseEventArgs e)
        {
            if (_draggedItems == null)
                return;

            var hoveredItem = GetItemAt(0,
                Math.Min(e.Y, Items[Items.Count - 1].GetBounds(ItemBoundsPortion.Entire).Bottom - 1));

            if (hoveredItem == null)
                return;

            var bounds = hoveredItem.GetBounds(ItemBoundsPortion.Entire);

            foreach (ListViewItem draggedItem in _draggedItems)
                if (draggedItem != hoveredItem)
                {
                    Items.Remove(draggedItem);
                    if (e.Y < bounds.Top + bounds.Height / 2)
                        Items.Insert(hoveredItem.Index, draggedItem);
                    else
                        Items.Insert(hoveredItem.Index + 1, draggedItem);
                }

            foreach (ListViewItem item in Items) item.Font = new Font(item.Font, FontStyle.Regular);

            _draggedItems = null;

            Cursor = Cursors.Default;
        }
    }
}
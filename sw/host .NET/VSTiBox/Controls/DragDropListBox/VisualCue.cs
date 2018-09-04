using System;
using System.Drawing;
using System.Windows.Forms;

namespace VSTiBox.DragDropListBox
{
	/// <summary>
	/// Horizontal line, which indicates the current drop position.
	/// </summary>
	/// <remarks>In order to be able to delete the visual cue properly, we need to draw in inverted mode. Since
	/// Microsoft has not included this option into the BCL, we need to fall back on the Win32 API.</remarks>
	public class VisualCue
	{
		/// <summary>
		/// Indicates that no visual cue has been drawn.
		/// </summary>
		public const int NoVisualCue = -1;

		private ListBox _listBox;
		private int _index = NoVisualCue;

		/// <summary>
		/// Initializes a new instance of the VisualCue class.
		/// </summary>
		/// <param name="listBox">ListBox or control derived from ListBox which owns the visual cue.</param>
		public VisualCue(ListBox listBox)
		{
			_listBox = listBox;
		}

		/// <summary>
		/// Gets the index of the item before which the visual cue has been drawn or NoVisualCue if no visual cue has been drawn.
		/// </summary>
		public int Index
		{
			get { return _index; }
		}

		/// <summary>
		/// Clears the visual cue, if some visual cue is visible.
		/// </summary>
		public void Clear()
		{
			if (_index != NoVisualCue) {
				Draw(_index);  // Draws in inverted mode and thus deletes the visual cue;
				_index = NoVisualCue;
			}
		}

		/// <summary>
		/// Draws a visual cue before (above) the list item indicated by itemIndex. If itemIndex is greater than
		/// the index of the last item of the list, then the visual cue is drawn after (below) the last list item.
		/// </summary>
		/// <remarks>Remembers the passed index for later use by the Clear method and for the Index property.</remarks>
		/// <param name="itemIndex">Index of a list item.</param>
		public void Draw(int itemIndex)
		{
			Rectangle rect;
			Point l1p1, l1p2, l2p1, l2p2;

			if (_listBox.Sorted) { // Draw vertical cue if the list is sorted, since items could just be dropped anywhere.
				rect = _listBox.ClientRectangle;
				rect = _listBox.RectangleToScreen(rect); // We need screen coordinates.
				l1p1 = new Point(rect.Left, rect.Top);
				l1p2 = new Point(rect.Left, rect.Bottom);
				l2p1 = new Point(rect.Left + 1, rect.Top);
				l2p2 = new Point(rect.Left + 1, rect.Bottom);
			} else { // Draw horizontal line.
				// Figure out where to draw the cue.
				if (_listBox.Items.Count == 0) { // No current item available. Get the client rectangle instead.
					rect = _listBox.ClientRectangle;
				} else if (itemIndex < _listBox.Items.Count) { // Somewhere in the list, this is our current item.
					rect = _listBox.GetItemRectangle(itemIndex);
				} else { // Somewhere after the last item. Let's draw the visual cue just after the last item in the list.
					rect = _listBox.GetItemRectangle(_listBox.Items.Count - 1); // Take the last items rectangle...
					rect.Y += rect.Height; // ... and move it down by the item height.
				}
				rect.Y -= 1; // Center the 2-pixel line around the calculated position.
				if (rect.Y < _listBox.ClientRectangle.Y) { // Make sure we draw inside of the client rectangle.
					rect.Y = _listBox.ClientRectangle.Y;
				}
				rect = _listBox.RectangleToScreen(rect); // We need screen coordinates.
				l1p1 = new Point(rect.Left, rect.Top);
				l1p2 = new Point(rect.Right, rect.Top);
				l2p1 = new Point(rect.Left, rect.Top + 1);
				l2p2 = new Point(rect.Right, rect.Top + 1);
			}
			IntPtr hdc = UnsafeNativeMethods.GetDC(IntPtr.Zero); // Get device context.
			UnsafeNativeMethods.SetROP2(hdc, UnsafeNativeMethods.R2_NOT); // Switch to inverted mode.
			UnsafeNativeMethods.MoveToEx(hdc, l1p1.X, l1p1.Y, IntPtr.Zero);
			UnsafeNativeMethods.LineTo(hdc, l1p2.X, l1p2.Y);
			UnsafeNativeMethods.MoveToEx(hdc, l2p1.X, l2p1.Y, IntPtr.Zero);
			UnsafeNativeMethods.LineTo(hdc, l2p2.X, l2p2.Y);
			UnsafeNativeMethods.ReleaseDC(IntPtr.Zero, hdc); // Release device context.
			_index = itemIndex;
		}
	}
}

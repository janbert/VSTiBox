using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace VSTiBox.DragDropListBox
{
	/// <summary>
	/// Win 32 API stuff used in this Project.
	/// </summary>
	internal static class UnsafeNativeMethods
	{
		[DllImport("gdi32.dll", EntryPoint = "SetROP2", CallingConvention = CallingConvention.StdCall)]
		public extern static int SetROP2(IntPtr hdc, int fnDrawMode);

		[DllImport("user32.dll", EntryPoint = "GetDC", CallingConvention = CallingConvention.StdCall)]
		public extern static IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll", EntryPoint = "ReleaseDC", CallingConvention = CallingConvention.StdCall)]
		public extern static IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll", EntryPoint = "MoveToEx", CallingConvention = CallingConvention.StdCall)]
		public extern static bool MoveToEx(IntPtr hdc, int x, int y, IntPtr lpPoint);

		[DllImport("gdi32.dll", EntryPoint = "LineTo", CallingConvention = CallingConvention.StdCall)]
		public extern static bool LineTo(IntPtr hdc, int x, int y);

		public const int R2_NOT = 6;  // Inverted drawing mode

	}
}

using System;

using System.Runtime.InteropServices;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class WinAPI
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hwnd, string lpstring);

        [DllImport("Gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("Gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("Gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hDC, int index);

        [DllImport("Kernel32.dll")]
        public static extern uint GetACP();

        public static IntPtr CreateDCFromHWND(IntPtr hwnd)
        {
            var hDC = GetDC(hwnd);
            try
            {
                return CreateCompatibleDC(hDC);
            }
            finally
            {
                ReleaseDC(hwnd, hDC);
            }
        }

        public static IntPtr CreateDCFromHWND(int hwnd) => CreateDCFromHWND(new IntPtr(hwnd));

    }
}

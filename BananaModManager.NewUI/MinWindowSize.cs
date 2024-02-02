using System;
using System.Runtime.InteropServices;

namespace BananaModManager.NewUI;

public static class MinWindowSize
{
    private static int _minWidth;
    private static int _minHeight;

    // Define the delegate for the subclass procedure
    private delegate IntPtr SubclassProcDelegate(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData);

    // Le subclass procedure
    private static IntPtr SubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData)
    {
        // Check if the message is WM_GETMINMAXINFO
        if (uMsg == 0x0024)
        {
            // Get the DPI value of the window
            uint dpi = GetDpiForWindow(hWnd);

            // Calculate the scaling factor
            // Blame Microsoft for the 96 magic number
            var scalingFactor = (float)dpi / 96;

            // Get the MINMAXINFO structure from the lParam
            var mmi = Marshal.PtrToStructure<MinMaxInfo>(lParam);

            // Set the minimum size of the window, scaled by the DPI value
            mmi.ptMinTrackSize.x = (int)(_minWidth * scalingFactor);
            mmi.ptMinTrackSize.y = (int)(_minHeight * scalingFactor);

            // Copy the modified structure back to the lParam
            Marshal.StructureToPtr(mmi, lParam, false);
        }
        // Call the original window procedure
        return DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    // Define the structs needed
    [StructLayout(LayoutKind.Sequential)]
    private struct MinMaxInfo
    {
        public Point ptReserved;
        public Point ptMaxSize;
        public Point ptMaxPosition;
        public Point ptMinTrackSize;
        public Point ptMaxTrackSize;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int x;
        public int y;
    }

    // Import the functions
    [DllImport("comctl32.dll")]
    private static extern bool SetWindowSubclass(IntPtr hWnd, SubclassProcDelegate pfnSubclass, IntPtr uIdSubclass, IntPtr dwRefData);

    [DllImport("comctl32.dll")]
    private static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
    [DllImport("User32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hWnd);

    // Society if C# didn't have a garbage collector
    private static readonly SubclassProcDelegate _pleaseDoNotGarbageCollect = SubclassProc;

    // This is where the fun happens
    public static void Set(IntPtr hWnd, int minWidth, int minHeight)
    {
        _minWidth = minWidth;
        _minHeight = minHeight;
        SetWindowSubclass(hWnd, _pleaseDoNotGarbageCollect, (IntPtr) 1, IntPtr.Zero);
    }
}

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Ducksoft.SOA.Common.Utilities
{
    /// <summary>
    /// Utility class which holds windows native API methods in HR4 application.
    /// </summary>
    public static class NativeApiHelper
    {
        public enum MSI_ERROR : int
        {
            ERROR_SUCCESS = 0,
            ERROR_MORE_DATA = 234,
            ERROR_NO_MORE_ITEMS = 259,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_BAD_CONFIGURATION = 1610,
        }

        [Flags]
        public enum MoveFileFlags
        {
            MOVEFILE_REPLACE_EXISTING = 0x00000001,
            MOVEFILE_COPY_ALLOWED = 0x00000002,
            MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004,
            MOVEFILE_WRITE_THROUGH = 0x00000008,
            MOVEFILE_CREATE_HARDLINK = 0x00000010,
            MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x00000020
        }

        public const int SW_RESTORE = 9;

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 MsiGetProductInfo(string product, string property,
            [Out] StringBuilder valueBuf, ref Int32 len);

        [DllImport("msi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int MsiEnumProducts(int iProductIndex, StringBuilder lpProductBuf);
        [DllImport("msi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern UInt32 MsiInstallProduct(string packagePath, string commandLine);
        [DllImport("msi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern UInt32 MsiReinstallProduct(string product, int reinstallMode);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, int wEnable);

        //Bring window to the front,Implemenetd for DAE291(Show log button)
        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [DllImport("User32.dll")]
        public static extern bool IsIconic(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName,
           MoveFileFlags dwFlags);
    }
}

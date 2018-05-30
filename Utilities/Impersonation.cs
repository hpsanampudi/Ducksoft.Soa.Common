using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace Ducksoft.SOA.Common.Utilities
{
    /// <summary>
    /// Reference: http://stackoverflow.com/questions/125341/how-do-you-do-impersonation-in-net
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class Impersonation : IDisposable
    {
        /// <summary>
        /// The handle
        /// </summary>
        private readonly SafeTokenHandle _handle;

        /// <summary>
        /// The context
        /// </summary>
        private readonly WindowsImpersonationContext _context;

        /// <summary>
        /// The logo N32 logon new credentials
        /// </summary>
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        /// <summary>
        /// Initializes a new instance of the <see cref="Impersonation"/> class.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="System.ApplicationException"></exception>
        public Impersonation(string domain, string username, string password)
        {
            var isValidUser = LogonUser(username, domain, password, LOGON32_LOGON_NEW_CREDENTIALS,
                0, out _handle);

            if (!isValidUser)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw (new ApplicationException($"Could not impersonate the elevated user." +
                    $" LogonUser returned error code {errorCode}."));
            }

            _context = WindowsIdentity.Impersonate(_handle.DangerousGetHandle());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _context?.Undo();
            _context.Dispose();
            _handle.Dispose();
        }

        /// <summary>
        /// Logons the user.
        /// </summary>
        /// <param name="lpszUsername">The LPSZ username.</param>
        /// <param name="lpszDomain">The LPSZ domain.</param>
        /// <param name="lpszPassword">The LPSZ password.</param>
        /// <param name="dwLogonType">Type of the dw logon.</param>
        /// <param name="dwLogonProvider">The dw logon provider.</param>
        /// <param name="phToken">The ph token.</param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain,
            string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid" />
        public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            /// <summary>
            /// Prevents a default instance of the <see cref="SafeTokenHandle"/> class from being created.
            /// </summary>
            private SafeTokenHandle()
                : base(true) { }

            /// <summary>
            /// Closes the handle.
            /// </summary>
            /// <param name="handle">The handle.</param>
            /// <returns></returns>
            [DllImport("kernel32.dll")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);

            /// <summary>
            /// When overridden in a derived class, executes the code required to free the handle.
            /// </summary>
            /// <returns>
            /// true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.
            /// </returns>
            protected override bool ReleaseHandle()
            {
                return (CloseHandle(handle));
            }
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BiF.DAL.Extensions
{
    public static class DataTools
    {

        public static SecureString Secure(this string value) {
            if (value == null)
                throw new ArgumentNullException("value");

            SecureString securePassword = new SecureString();
            foreach (char c in value)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }

        public static string Unsecure(this SecureString secureValue) {
            IntPtr valuePtr = IntPtr.Zero;
            try {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(secureValue);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

    }
}

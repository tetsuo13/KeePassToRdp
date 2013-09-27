using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace KeePassToRdp
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// RDP password encryption code from
    /// http://www.remkoweijnen.nl/blog/2007/10/18/how-rdp-passwords-are-encrypted/#comment-562
    /// </remarks>
    class RdpPassword
    {
        private const int CryptProtectUiForbidden = 0x1;

        // Wrapper for the NULL handle or pointer.
        static private IntPtr NullPtr = ((IntPtr)((int)(0)));

        // Wrapper for DPAPI CryptProtectData function.
        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CryptProtectData(ref DATA_BLOB pPlainText,
            [MarshalAs(UnmanagedType.LPWStr)]string szDescription, IntPtr pEntroy, IntPtr pReserved, IntPtr pPrompt,
            int dwFlags, ref DATA_BLOB pCipherText);

        // BLOB structure used to pass data to DPAPI functions.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        private static void InitBlob(byte[] data, ref DATA_BLOB blob)
        {
            blob.pbData = Marshal.AllocHGlobal(data.Length);
            if (blob.pbData == IntPtr.Zero)
            {
                throw new Exception("Unable to allocate buffer for BLOB data.");
            }

            blob.cbData = data.Length;
            Marshal.Copy(data, 0, blob.pbData, data.Length);
        }

        public static string EncryptPassword(string password)
        {
            byte[] pwba = Encoding.Unicode.GetBytes(password);
            DATA_BLOB dataIn = new DATA_BLOB();
            DATA_BLOB dataOut = new DATA_BLOB();
            StringBuilder epwsb = new StringBuilder();

            try
            {
                try
                {
                    InitBlob(pwba, ref dataIn);
                }
                catch (Exception)
                {
                }

                bool success = CryptProtectData(ref dataIn, "psw", NullPtr, NullPtr, NullPtr,
                    CryptProtectUiForbidden, ref dataOut);

                if (!success)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Exception("CryptProtectData failed", new Win32Exception(errorCode));
                }

                byte[] epwba = new byte[dataOut.cbData];
                Marshal.Copy(dataOut.pbData, epwba, 0, dataOut.cbData);

                // Convert hex data to hex characters (suitable for a string).
                for (int i = 0; i < dataOut.cbData; i++)
                {
                    epwsb.Append(Convert.ToString(epwba[i], 16).PadLeft(2, '0').ToUpper());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unable to encrypt data.", ex);
            }
            finally
            {
                if (dataIn.pbData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(dataIn.pbData);
                }

                if (dataOut.pbData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(dataOut.pbData);
                }
            }
            return epwsb.ToString();
        }
    }
}

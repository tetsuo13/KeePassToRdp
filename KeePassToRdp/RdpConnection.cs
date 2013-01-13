// KeePass to RDP.
// Copyright (C) 2013  Andrei Nicholson
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KeePassToRdp
{
    /// <summary>
    /// Create a saved RDP file for MSTSC to use. Removed after usage.
    /// </summary>
    /// 
    /// <remarks>
    /// RDP password encryption code from
    /// http://www.remkoweijnen.nl/blog/2007/10/18/how-rdp-passwords-are-encrypted/#comment-562
    /// </remarks>
    public class RdpConnection
    {
        private const int CryptProtectUiForbidden = 0x1;

        // Wrapper for the NULL handle or pointer.
        static private IntPtr NullPtr = ((IntPtr)((int)(0)));

        // Wrapper for DPAPI CryptProtectData function.
        [DllImport("crypt32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool CryptProtectData(ref DATA_BLOB pPlainText,
                                                    [MarshalAs(UnmanagedType.LPWStr)]string szDescription,
                                                    IntPtr pEntroy,
                                                    IntPtr pReserved,
                                                    IntPtr pPrompt,
                                                    int dwFlags,
                                                    ref DATA_BLOB pCipherText);

        // BLOB structure used to pass data to DPAPI functions.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]

        internal struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        public static void Launch(Client c)
        {
            string rdpFile = "";

            try
            {
                rdpFile = CreateRdpFile(c);
                LaunchRdc(rdpFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Launch MSTSC using an RDP file. Delete the file afterwards.
        /// </summary>
        /// 
        /// <remarks>
        /// RDP file is deleted a few seconds after MSTSC is lauched. This can
        /// be error prone and exposes the risk of the file being moved
        /// elsewhere and saved. Contains login credentials, so this file
        /// should be treated as a security risk and deleted immediately.
        /// </remarks>
        /// <param name="rdpFile">Path to RDP file</param>
        private static void LaunchRdc(string rdpFile)
        {
            Process rdc = new Process();
            string exe = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");

            if (exe != null)
            {
                rdc.StartInfo.FileName = exe;
                rdc.StartInfo.Arguments = rdpFile;

                try
                {
                    rdc.Start();
                    Thread.Sleep(5000);

                    try
                    {
                        File.Delete(rdpFile);
                    }
                    catch (Exception)
                    {
                        // Let OS prune it eventually.
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Problem starting mstsc: " + e.Message);
                }
            }
        }

        private static string CreateRdpFile(Client c)
        {
            List<string> contents = new List<string>() {
                "screen mode id:i:2",
                "use multimon:i:0",
                "session bpp:i:16",
                "compression:i:1",
                "keyboardhook:i:2",
                "audiocapturemode:i:0",
                "videoplaybackmode:i:1",
                "connection type:i:4",
                "displayconnectionbar:i:1",
                "disable wallpaper:i:1",
                "allow font smoothing:i:0",
                "allow desktop composition:i:0",
                "disable full window drag:i:1",
                "disable menu anims:i:1",
                "disable themes:i:0",
                "disable cursor setting:i:0",
                "bitmapcachepersistenable:i:1",
                "audiomode:i:0",
                "redirectprinters:i:1",
                "redirectcomports:i:0",
                "redirectsmartcards:i:1",
                "redirectclipboard:i:1",
                "redirectposdevices:i:0",
                "redirectdirectx:i:1",
                "autoreconnection enabled:i:1",
                "authentication level:i:2",
                "prompt for credentials:i:0",
                "negotiate security layer:i:1",
                "remoteapplicationmode:i:0",
                "alternate shell:s:",
                "shell working directory:s:",
                "gatewayhostname:s:",
                "gatewayusagemethod:i:4",
                "gatewaycredentialssource:i:4",
                "gatewayprofileusagemethod:i:0",
                "promptcredentialonce:i:1",
                "use redirection server name:i:0",
                "drivestoredirect:s:"
            };

            contents.Add("winposstr:s:" + WindowPosition());
            contents.Add("desktopwidth:i:" + SystemInformation.VirtualScreen.Width.ToString());
            contents.Add("desktopheight:i:" + SystemInformation.VirtualScreen.Height.ToString());
            contents.Add("full address:s:" + c.GetUrl());
            contents.Add("username:s:" + c.GetUserName());
            contents.Add("password 51:b:" + EncryptPassword(c.GetPassword()));

            string rdpFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(rdpFile, String.Join(Environment.NewLine, contents.ToArray()));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw new Exception();
            }

            return rdpFile;
        }

        /// <summary>
        /// String form of the WINDOWSPOS structure.
        /// </summary>
        /// 
        /// <remarks>
        /// Uses full screen. Opts to put window on second monitor if found.
        /// </remarks>
        /// <returns></returns>
        private static string WindowPosition()
        {
            List<int> winposstr = new List<int>() { 2 };
            int left;
            int top;
            int bottom;

            if (Screen.AllScreens.Length > 1)
            {
                left = Screen.AllScreens[1].Bounds.Left;
                top = Screen.AllScreens[1].Bounds.Top;
                bottom = Screen.AllScreens[1].Bounds.Bottom;
            }
            else
            {
                left = 0;
                top = 0;
                bottom = Screen.AllScreens[0].Bounds.Bottom;
            }

            winposstr.Add(3);
            winposstr.Add(left);
            winposstr.Add(top);
            winposstr.Add(SystemInformation.VirtualScreen.Width);
            winposstr.Add(bottom);

            return String.Join(",", winposstr);
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

        private static string EncryptPassword(string password)
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

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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace KeePassToRdp
{
    /// <summary>
    /// Create a saved RDP file for MSTSC to use. Removed after usage.
    /// </summary>
    public class RdpConnection
    {
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
            string exe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mstsc.exe");

            rdc.StartInfo.FileName = exe;

            // Quote entire path since the connection name may have spaces.
            rdc.StartInfo.Arguments = String.Format(@"""{0}""", rdpFile);

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
            contents.Add("full address:s:" + FullAddress(c, c.GetUrl()));
            contents.Add("username:s:" + c.GetUserName());
            contents.Add("password 51:b:" + RdpPassword.EncryptPassword(c.GetPassword()));

            string rdpFile = Path.Combine(Path.GetTempPath(), UniqueConnectionFileName(c.GetSafeTitle(), c.GetUrl()));

            try
            {
                File.WriteAllText(rdpFile, String.Join(Environment.NewLine, contents.ToArray()));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw e;
            }

            return rdpFile;
        }

        /// <summary>
        /// Return the full address including any selected options.
        /// </summary>
        /// <param name="client">Client with settings to check</param>
        /// <param name="url">URL to start with</param>
        /// <returns></returns>
        private static string FullAddress(Client client, string url)
        {
            List<string> fullAddress = new List<string>() { url };

            if (client.settings.Admin)
            {
                fullAddress.Add("/admin");
            }

            if (client.settings.Public)
            {
                fullAddress.Add("/public");
            }

            return String.Join(" ", fullAddress);
        }

        /// <summary>
        /// Use the client name in conjunction with an incrementing counter.
        /// This is used as the RDC connection name so that each instance of
        /// the same client is given a unique name in the taskbar.
        /// </summary>
        /// <remarks>
        /// Attempts to query all running applications to find a window title
        /// like "clientName - IP - Remote Desktop Connection". A number is
        /// appended to the client name.
        /// </remarks>
        /// <param name="clientName">Client name</param>
        /// <param name="clientAddress">IP address of client</param>
        /// <returns>Filename used for RDC connection</returns>
        private static string UniqueConnectionFileName(string clientName, string clientAddress)
        {
            // Replace characters which will cause issues for the file name.
            string filenameSafeClientName = clientName.Replace('.', '-');

            Regex existingClient = new Regex(String.Format(@"^{0}-?(\d+)?\s?- {1} - Remote Desktop Connection$",
                Regex.Escape(filenameSafeClientName), Regex.Escape(clientAddress)));
            Process[] processList = Process.GetProcesses();
            string connectionName = filenameSafeClientName;
            int nextInstanceId = 0;

            foreach (Process process in processList)
            {
                Match matches = existingClient.Match(process.MainWindowTitle);

                if (String.IsNullOrEmpty(process.MainWindowTitle) || !matches.Success)
                {
                    continue;
                }

                if (!String.IsNullOrEmpty(matches.Groups[1].Value))
                {
                    try
                    {
                        nextInstanceId = Int32.Parse(matches.Groups[1].Value) + 1;
                    }
                    catch (Exception e)
                    {
                    }
                }
                else
                {
                    nextInstanceId = 2;
                }
            }

            string filename;

            if (nextInstanceId == 0)
            {
                filename = connectionName;
            }
            else
            {
                filename = String.Format("{0}-{1}", connectionName, nextInstanceId.ToString());
            }

            return String.Format("{0}.tmp", filename);
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
    }
}

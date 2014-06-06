#region License
// KeePass to RDP.
// Copyright (C) 2013-2014 Andrei Nicholson
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
#endregion

using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace KeePassToRdp
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            this.Version.DataContext = new
            {
                Version = "Version " + Assembly.GetEntryAssembly().GetName().Version
            };

            Assembly kp = Assembly.Load(KeePassToRdp.Properties.Resources.KeePassLib);
            this.ComponentListView.Items.Add(new
            {
                Component = "KeePassLib", Version = kp.GetName().Version.ToString()
            });
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

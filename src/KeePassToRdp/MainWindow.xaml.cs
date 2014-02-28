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

using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace KeePassToRdp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CompositeKey key = new CompositeKey();
        private string dbFile;
        private Clients clients;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void OpenDbButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "KeePass Database (*.kdbx)|*.kdbx";
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                dbFile = dialog.FileName;
                SetupDatabase(dbFile, true);
            }
        }

        public void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SetupDatabase(dbFile, false);
        }

        private void SetupDatabase(string file, bool loadDb)
        {
            try
            {
                ReadDatabase(file, loadDb);

                if (ServerList.Items.Count > 0)
                {
                    ServerList.IsEnabled = true;
                    ToggleOptions(false);
                }
            }
            catch (KeePassLib.Keys.InvalidCompositeKeyException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ReadDatabase(string dbPath, bool loadDb)
        {
            PwDatabase db = InitializeDatabase(dbPath, loadDb);

            DatabaseNameLabel.Content = String.IsNullOrEmpty(db.Name) ? System.IO.Path.GetFileName(dbPath) : db.Name;
            DatabaseNameLabel.FontStyle = System.Windows.FontStyles.Normal;

            PopulateCombobox(db);

            db.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <seealso href="https://stackoverflow.com/questions/3544616/wpf-combobox-option-group-optgroup-type-behaviour">
        /// StackOverflow: WPF ComboBox “option group (optGroup)” type behaviour
        /// </seealso>
        private void PopulateCombobox(PwDatabase db)
        {
            clients = new Clients();

            foreach (PwEntry entry in db.RootGroup.GetEntries(true))
            {
                if (!clients.ValidRdpEntry(entry))
                {
                    continue;
                }

                clients.Add(entry.ParentGroup.GetFullPath(Clients.GroupSeparator, false), entry.Strings);
            }

            ServerList.SelectionChanged -= ServerList_SelectionChanged;
            ServerList.SelectedIndex = -1;

            ServerList.ItemsSource = clients.GetComboBoxRange();

            ServerList.SelectionChanged += ServerList_SelectionChanged;
        }

        private PwDatabase InitializeDatabase(string dbPath, bool loadDb)
        {
            if (key.UserKeyCount != 0 && !loadDb)
            {
                return OpenKeePass();
            }

            PasswordWindow form = new PasswordWindow(Path.GetFileName(dbPath));
            form.Owner = this;
            form.ShowDialog();

            if (form.DialogResult.HasValue && form.DialogResult.Value)
            {
                if (key.UserKeyCount == 0 && loadDb)
                {
                    key.AddUserKey(new KcpPassword(form.PasswordEntry.Password));
                }

                return OpenKeePass();
            }

            throw new Exception("Master password entry canceled");
        }

        private PwDatabase OpenKeePass()
        {
            IOConnectionInfo info = new IOConnectionInfo()
            {
                Path = dbFile
            };
            PwDatabase db = new PwDatabase();
            db.Open(info, key, null);
            RefreshButton.IsEnabled = true;
            return db;
        }

        private void LaunchButton_Click(object sender, EventArgs e)
        {
            RdpConnection.Launch(clients.GetClient(((ComboBoxItem)ServerList.SelectedItem).Value));
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            SetupDatabase(dbFile, false);
        }

        private void ServerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedClientIndex = ((ComboBoxItem)ServerList.SelectedItem).Value;
            Client client = clients.GetClient(selectedClientIndex);
            ToggleOptions(true, client);
        }

        private void ToggleOptions(bool enabled)
        {
            LaunchConnectionButton.IsEnabled = enabled;

            // Disable firing the checked event while changing the boxes. Only
            // want to uncheck the boxes for visual response, not change the
            // setting for the client.
            CheckBoxAdmin.Checked -= CheckBoxAdmin_Checked;
            CheckBoxPublic.Checked -= CheckBoxPublic_Checked;

            CheckBoxAdmin.IsEnabled = enabled;
            CheckBoxAdmin.IsChecked = false;
            CheckBoxPublic.IsEnabled = enabled;
            CheckBoxPublic.IsChecked = false;

            CheckBoxAdmin.Checked += CheckBoxAdmin_Checked;
            CheckBoxPublic.Checked += CheckBoxPublic_Checked;
        }

        private void ToggleOptions(bool enabled, Client client)
        {
            ToggleOptions(enabled);

            CheckBoxAdmin.IsChecked = client.settings.Admin;
            CheckBoxPublic.IsChecked = client.settings.Public;
        }

        private void CheckBoxAdmin_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = CheckBoxAdmin.IsChecked.HasValue ? CheckBoxAdmin.IsChecked.Value : false;
            clients.ChangeSettingAdmin(((ComboBoxItem)ServerList.SelectedItem).Value, isChecked);
        }

        private void CheckBoxPublic_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = CheckBoxPublic.IsChecked.HasValue ? CheckBoxPublic.IsChecked.Value : false;
            clients.ChangeSettingPublic(((ComboBoxItem)ServerList.SelectedItem).Value, isChecked);
        }
    }
}

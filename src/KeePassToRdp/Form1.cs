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

using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace KeePassToRdp
{
    public partial class Form1 : Form
    {
        private CompositeKey key = new CompositeKey();
        private string dbFile;

        public Form1()
        {
            InitializeComponent();
        }

        private void opendbButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "KeePass Database (*.kdbx)|*.kdbx";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                dbFile = openFileDialog1.FileName;
                SetupDatabase(dbFile, true);
            }
        }

        private void SetupDatabase(string file, bool loadDb)
        {
            try
            {
                ReadDatabase(file, loadDb);
                serverList.Enabled = true;
                launchButton.Enabled = false;
            }
            catch (KeePassLib.Keys.InvalidCompositeKeyException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception)
            {
            }
        }

        private void ReadDatabase(string dbPath, bool loadDb)
        {
            PwDatabase db = InitializeDatabase(dbPath, loadDb);

            label1.Text = String.IsNullOrEmpty(db.Name) ? System.IO.Path.GetFileName(dbPath) : db.Name;
            label1.Font = new Font(label1.Font.Name, label1.Font.Size, FontStyle.Regular);

            PopulateCombobox(db);

            db.Close();
        }

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

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(clients.GetComboBoxRange());
            comboBox1.UpdateWidth();
        }

        private PwDatabase InitializeDatabase(string dbPath, bool loadDb)
        {
            if (key.UserKeyCount != 0 && !loadDb)
            {
                return OpenKeePass();
            }

            PasswordForm form = new PasswordForm(System.IO.Path.GetFileName(dbPath));

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                if (key.UserKeyCount == 0 && loadDb)
                {
                    key.AddUserKey(new KcpPassword(form.password.GetPassword()));
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
            refreshButton.Enabled = true;
            return db;
        }

        private void launchButton_Click(object sender, EventArgs e)
        {
            RdpConnection.Launch(clients.GetClient(((ClientComboBoxItem)comboBox1.SelectedItem).Value));
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            SetupDatabase(dbFile, false);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enabled = false;

            if (comboBox1.SelectedItem != null && ((ClientComboBoxItem)comboBox1.SelectedItem).Selectable)
            {
                enabled = true;
            }

            this.launchButton.Enabled = enabled;
        }
    }
}

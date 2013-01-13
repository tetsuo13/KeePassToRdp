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
using System.Windows.Forms;

namespace KeePassToRdp
{
    public partial class PasswordForm : Form
    {
        public PasswordEntry password = new PasswordEntry();

        public PasswordForm(string dbName)
        {
            InitializeComponent();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Text = "Open Database - " + dbName;
            this.passwordBox.UseSystemPasswordChar = true;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void passwordBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                okButton.PerformClick();
                e.Handled = true;
            }
        }

        void passwordBox_TextChanged(object sender, System.EventArgs e)
        {
            password.SetPassword(passwordBox.Text);
        }
    }
}

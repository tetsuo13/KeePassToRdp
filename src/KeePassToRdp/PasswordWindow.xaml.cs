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

using System.Windows;
using System.Windows.Input;

namespace KeePassToRdp
{
    /// <summary>
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    /// <remarks>
    /// Showing and hiding the password via a toggle button like is used in
    /// the KeePass "Open Database" master password dialog is accomplished
    /// using an initial <b>PasswordBox</b> object along with a
    /// <b>TextBox</b>. The <b>TextBox</b> control is exactly the same
    /// dimensions and positioned in the same spot as the <b>PasswordBox</b>
    /// control but is initially hidden. When input is received it is copied
    /// into the other control so that each control is up to date with the
    /// latest character typed. When the toggle button is activated, the
    /// <b>PasswordBox</b> control is hidden and the <b>TextBox</b> control is
    /// shown. This gives the appearance of being able to remove PasswordChar.
    /// </remarks>
    public partial class PasswordWindow : Window
    {
        private bool passwordVisible;

        public PasswordWindow(string dbName)
        {
            InitializeComponent();
            passwordVisible = false;
            PasswordEntry.Focus();
            this.Title += " " + dbName;
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public void PasswordEntry_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TextInput.TextChanged -= TextInput_TextChanged;
            TextInput.Text = PasswordEntry.Password;
            TextInput.TextChanged += TextInput_TextChanged;
        }

        private void TextInput_TextChanged(object sender, RoutedEventArgs e)
        {
            PasswordEntry.PasswordChanged -= PasswordEntry_PasswordChanged;
            PasswordEntry.Password = TextInput.Text;
            PasswordEntry.PasswordChanged += PasswordEntry_PasswordChanged;
        }

        public void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (passwordVisible)
            {
                TextInput.Visibility = Visibility.Hidden;
                PasswordEntry.Visibility = Visibility.Visible;
                ShowPasswordButton.ToolTip = "Show password";
                PasswordEntry.Focus();
            }
            else
            {
                TextInput.Visibility = Visibility.Visible;
                PasswordEntry.Visibility = Visibility.Hidden;
                ShowPasswordButton.ToolTip = "Hide password";
                TextInput.Focus();
                TextInput.CaretIndex = TextInput.Text.Length;
            }

            passwordVisible = !passwordVisible;
        }

        private void ShowPasswordButton_Checked(object sender, RoutedEventArgs e)
        {
            //this.PasswordEntry.Opacity = 0;
        }

        private void DualPasswordEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButton_Click(sender, null);
                e.Handled = true;
            }
        }
    }
}

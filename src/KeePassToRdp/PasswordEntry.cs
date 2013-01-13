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
using System.Runtime.InteropServices;
using System.Text;
using System.Security;

namespace KeePassToRdp
{
    public class PasswordEntry
    {
        private SecureString password = new SecureString();

        public void SetPassword(string p)
        {
            StringBuilder sb = new StringBuilder(p);

            for (int i = 0; i < sb.Length; i++)
            {
                password.AppendChar(sb[i]);
            }
        }

        public string GetPassword()
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(password);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
    }
}

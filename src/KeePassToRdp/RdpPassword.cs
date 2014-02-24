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

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KeePassToRdp
{
    public class RdpPassword
    {
        private static string ToHexString(byte[] bytes)
        {
            if (bytes == null)
            {
                return String.Empty;
            }
            return bytes.Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat("{0:x2}", b)).ToString();
        }

        public static string EncryptPassword(string password)
        {
            byte[] data = Encoding.Unicode.GetBytes(password);
            return ToHexString(ProtectedData.Protect(data, new byte[0], DataProtectionScope.CurrentUser));
        }
    }
}

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
using KeePassLib.Collections;
using System;

namespace KeePassToRdp
{
    /// <summary>
    /// Represents a single entry from the KeePass database.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Token string to indicate an RDP entry.
        /// </summary>
        private const string EntryToken = "rdp";

        /// <summary>
        /// Number representing the group this entry is assigned to.
        /// </summary>
        public int groupId { get; private set; }

        private ProtectedStringDictionary dict;

        public Client(int groupId, ProtectedStringDictionary dict)
        {
            this.groupId = groupId;
            this.dict = dict;
        }

        private string ReadTag(string tag)
        {
            return dict.ReadSafe(tag);
        }

        public string GetTitle()
        {
            return ReadTag("Title");
        }

        public string GetUserName()
        {
            return ReadTag("UserName");
        }

        public string GetPassword()
        {
            return ReadTag("Password");
        }

        public string GetUrl()
        {
            return ReadTag("URL");
        }

        /// <summary>
        /// Search the title, notes, and tags attributes for the key token.
        /// </summary>
        /// <param name="entry">Database entry</param>
        /// <returns>True if entry appears to represent RDP info</returns>
        public static bool ValidRdpEntry(PwEntry entry)
        {
            string[] keys = new string[] {"Title", "Notes", "Tags"};

            foreach (string key in keys)
            {
                if (entry.Strings.ReadSafe(key).ToLower().Contains(EntryToken))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

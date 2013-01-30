﻿// KeePass to RDP.
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KeePassToRdp
{
    public class Clients
    {
        /// <summary>
        /// Separator used for group breadcrumbs during display.
        /// </summary>
        public const string GroupSeparator = " / ";

        /// <summary>
        /// Prefix used for entry title during display.
        /// </summary>
        public const string ClientPrefix = "    ";

        private List<Client> clientList = new List<Client>();
        private List<string> groups = new List<string>();
        private Regex tokenMatch = new Regex(@"\brdp\b", RegexOptions.IgnoreCase);

        public void Add(string group, ProtectedStringDictionary dict)
        {
            clientList.Add(new Client(FindOrCreateGroup(group), dict));
        }

        public Client GetClient(int clientId)
        {
            return clientList[clientId];
        }

        private int FindOrCreateGroup(string needle)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].Equals(needle))
                {
                    return i;
                }
            }
            groups.Add(needle);
            return groups.Count - 1;
        }

        public ClientComboBoxItem[] GetComboBoxRange()
        {
            int lastGroupId = -1;
            ClientComboBoxItem item;
            List<ClientComboBoxItem> items = new List<ClientComboBoxItem>();

            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].groupId != lastGroupId)
                {
                    item = new ClientComboBoxItem();
                    item.Text = groups[clientList[i].groupId];
                    item.Selectable = false;
                    item.Value = -1;
                    lastGroupId = clientList[i].groupId;
                    items.Add(item);
                }

                item = new ClientComboBoxItem();
                item.Text = Clients.ClientPrefix + clientList[i].GetTitle();
                item.Value = i;
                item.Selectable = true;
                items.Add(item);
            }

            return items.ToArray();
        }

        /// <summary>
        /// Search the title, notes, and tags attributes for the key token.
        /// </summary>
        /// <param name="entry">Database entry</param>
        /// <returns>True if entry appears to represent RDP info</returns>
        public bool ValidRdpEntry(PwEntry entry)
        {
            if (EntryStringContainsToken(entry))
            {
                return true;
            }

            if (EntryTagsContainToken(entry))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Search the Strings property of an entry for the token.
        /// </summary>
        /// <param name="entry">Database entry</param>
        /// <returns>True if an entry string contains the token</returns>
        private bool EntryStringContainsToken(PwEntry entry)
        {
            string[] keys = new string[] { "Title", "Notes" };

            foreach (string key in keys)
            {
                if (tokenMatch.IsMatch(entry.Strings.ReadSafe(key)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Search all tags for entry for the token.
        /// </summary>
        /// <param name="entry">Database entry</param>
        /// <returns>True if any one tag contains the token</returns>
        private bool EntryTagsContainToken(PwEntry entry)
        {
            foreach (string tag in entry.Tags)
            {
                if (tokenMatch.IsMatch(tag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KeePassLib;
using KeePassLib.Collections;

namespace KeePassToRdp
{
    /// <summary>
    /// Organizes all clients within their respective groups.
    /// </summary>
    /// <remarks>
    /// A dictionary is used to maintain the group and all of its clients. The
    /// groups are sorted and all clients within are also sorted.
    ///
    /// Throughout the scope of the object, it's assumed that the population
    /// of the list (<seealso cref="Clients.Add"/>) will occur and then only
    /// reads (<seealso cref="GetClient"/>) will occur thereafter. This is
    /// important since GetComboBoxRange will increment a counter for the
    /// clients and use that as their ID. When retrieving a client,
    /// <seealso cref="GetClient"/>, the requested ID must correspond to that
    /// client's position. Adding any subsequent clients will throw off the
    /// IDs since the list is sorted.
    /// </remarks>
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

        private readonly Regex TokenMatch = new Regex(@"\brdp\b", RegexOptions.IgnoreCase);

        private SortedList<string, List<Client>> clientList = new SortedList<string, List<Client>>();

        public void Add(string group, ProtectedStringDictionary dict)
        {
            Client client = new Client(dict);

            if (!clientList.ContainsKey(group))
            {
                clientList.Add(group, new List<Client> { client });
            }
            else
            {
                clientList[group].Add(client);
                clientList[group] = clientList[group].OrderBy(x => x.GetTitle()).ToList();
            }
        }

        public Client GetClient(int clientId)
        {
            // Flatten all of the lists. Each group would've been its own list
            // of Clients.
            List<Client> clients = clientList.Values.SelectMany(x => x).ToList();

            if (clientId < clients.Count)
            {
                return clients[clientId];
            }

            throw new KeyNotFoundException();
        }

        public ClientComboBoxItem[] GetComboBoxRange()
        {
            List<ClientComboBoxItem> items = new List<ClientComboBoxItem>();
            int itemNumber = 0;

            foreach (KeyValuePair<string, List<Client>> pair in clientList)
            {
                items.Add(new ClientComboBoxItem
                {
                    Text = pair.Key,
                    Selectable = false,
                    Value = 0
                });

                foreach (Client client in pair.Value)
                {
                    items.Add(new ClientComboBoxItem
                    {
                        Text = Clients.ClientPrefix + client.GetTitle(),
                        Selectable = true,
                        Value = itemNumber
                    });

                    itemNumber++;
                }
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
                if (TokenMatch.IsMatch(entry.Strings.ReadSafe(key)))
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
                if (TokenMatch.IsMatch(tag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

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

namespace KeePassToRdp
{
    /// <summary>
    /// Attributes for a single combobox item.
    /// </summary>
    public class ClientComboBoxItem
    {
        /// <summary>
        /// Accessor which will return the value.
        /// </summary>
        public const string ValueMember = "Value";

        /// <summary>
        /// Accessor which will return the displayed text.
        /// </summary>
        public const string DisplayMember = "Text";

        public int Value { get; set; }
        public string Text { get; set; }
        public bool Selectable { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}

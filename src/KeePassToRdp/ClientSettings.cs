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

namespace KeePassToRdp
{
    public class ClientSettings
    {
        /// <summary>
        /// Connects you to a session for administering the server.
        /// </summary>
        public bool Admin { get; set; }

        /// <summary>
        /// Runs Remote Desktop in public mode. In public mode, passwords and
        /// bitmaps are not cached.
        /// </summary>
        public bool Public { get; set; }
    }
}

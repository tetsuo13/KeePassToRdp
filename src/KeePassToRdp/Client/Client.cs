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

using System.IO;
using KeePassLib.Collections;

namespace KeePassToRdp
{
    /// <summary>
    /// Represents a single entry from the KeePass database.
    /// </summary>
    public class Client
    {
        private ProtectedStringDictionary dict;

        /// <summary>
        /// Title tag safe for a filename.
        /// </summary>
        private string safeTitle;

        public Client(ProtectedStringDictionary dict)
        {
            this.dict = dict;
            safeTitle = ReplaceInvalidChars(GetTitle());
        }

        /// <summary>
        /// Replace characters which are invalid for filenames or paths.
        /// </summary>
        /// <param name="title"></param>
        private string ReplaceInvalidChars(string tag)
        {
            const string replacementChar = "-";
            string invalidCharacters = new string(Path.GetInvalidFileNameChars()) +
                new string(Path.GetInvalidPathChars());
            string safeTag = tag;

            foreach (char c in invalidCharacters)
            {
                safeTag = safeTag.Replace(c.ToString(), replacementChar);
            }

            return safeTag;
        }

        private string ReadTag(string tag)
        {
            return dict.ReadSafe(tag);
        }

        public string GetSafeTitle()
        {
            return safeTitle;
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
    }
}

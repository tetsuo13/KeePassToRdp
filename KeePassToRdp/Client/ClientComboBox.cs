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
using System.Drawing;
using System.Windows.Forms;

namespace KeePassToRdp
{
    /// <summary>
    /// Mimics the look of the HTML OPTGROUP tag within SELECT.
    /// </summary>
    ///
    /// <remarks>
    /// Lacks the most important trait: cannot hover over groups. Currently
    /// you can and even select the groups but
    /// <see cref="OnSelectedIndexChanged"/> will prevent the event from
    /// registering.
    /// </remarks>
    public class ClientComboBox : ComboBox
    {
        public ClientComboBox()
        {
            this.ValueMember = ClientComboBoxItem.ValueMember;
            this.DisplayMember = ClientComboBoxItem.DisplayMember;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            e.DrawBackground();

            if (this.Items.Count == 0 || e.Index == -1 || e.Index >= this.Items.Count)
            {
                return;
            }

            string text = this.Items[e.Index].ToString();
            Brush textColor;
            Font textFont;

            if (!text.StartsWith(Clients.ClientPrefix))
            {
                textFont = new Font(e.Font.Name, e.Font.Size, FontStyle.Bold);
            }
            else
            {
                textFont = e.Font;
            }

            if (e.State != (DrawItemState.NoAccelerator | DrawItemState.NoFocusRect))
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
                textColor = new SolidBrush(SystemColors.HighlightText);
            }
            else
            {
                textColor = new SolidBrush(SystemColors.WindowText);
            }

            e.Graphics.DrawString(text, textFont, textColor, e.Bounds);
            e.DrawFocusRectangle();
        }

        public void UpdateWidth()
        {
            int maxWidth = 0;
            int currentWidth = 0;

            foreach (var item in this.Items)
            {
                currentWidth = TextRenderer.MeasureText(item.ToString(), this.Font).Width;

                if (currentWidth > maxWidth)
                {
                    maxWidth = currentWidth;
                }
            }

            this.Width = maxWidth + SystemInformation.VerticalScrollBarWidth;
        }

        /// <summary>
        /// Prevent selecting groups.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (this.SelectedItem == null)
            {
                return;
            }

            ClientComboBoxItem combo = (ClientComboBoxItem)this.SelectedItem;

            if (!combo.Selectable)
            {
                this.SelectedIndex = -1;
                return;
            }
        }
    }
}

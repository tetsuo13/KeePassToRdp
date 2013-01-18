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

using System.Windows.Forms;

namespace KeePassToRdp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.opendbButton = new System.Windows.Forms.Button();
            this.comboBox1 = new KeePassToRdp.ClientComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dbOptions = new System.Windows.Forms.GroupBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.serverList = new System.Windows.Forms.GroupBox();
            this.launchButton = new System.Windows.Forms.Button();
            this.dbOptions.SuspendLayout();
            this.serverList.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.InitialDirectory = "Desktop";
            // 
            // opendbButton
            // 
            this.opendbButton.Location = new System.Drawing.Point(85, 55);
            this.opendbButton.Name = "opendbButton";
            this.opendbButton.Size = new System.Drawing.Size(97, 23);
            this.opendbButton.TabIndex = 0;
            this.opendbButton.Text = "Open Database";
            this.opendbButton.UseVisualStyleBackColor = true;
            this.opendbButton.Click += new System.EventHandler(this.opendbButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DisplayMember = "Text";
            this.comboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(16, 26);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.ValueMember = "Value";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(107, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "None loaded";
            // 
            // dbOptions
            // 
            this.dbOptions.Controls.Add(this.refreshButton);
            this.dbOptions.Controls.Add(this.label2);
            this.dbOptions.Controls.Add(this.opendbButton);
            this.dbOptions.Controls.Add(this.label1);
            this.dbOptions.Location = new System.Drawing.Point(12, 12);
            this.dbOptions.Name = "dbOptions";
            this.dbOptions.Size = new System.Drawing.Size(371, 93);
            this.dbOptions.TabIndex = 3;
            this.dbOptions.TabStop = false;
            this.dbOptions.Text = "Database Options";
            // 
            // refreshButton
            // 
            this.refreshButton.Enabled = false;
            this.refreshButton.Location = new System.Drawing.Point(188, 55);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(97, 23);
            this.refreshButton.TabIndex = 4;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Current database:";
            // 
            // serverList
            // 
            this.serverList.Controls.Add(this.launchButton);
            this.serverList.Controls.Add(this.comboBox1);
            this.serverList.Enabled = false;
            this.serverList.Location = new System.Drawing.Point(12, 122);
            this.serverList.Name = "serverList";
            this.serverList.Size = new System.Drawing.Size(371, 106);
            this.serverList.TabIndex = 4;
            this.serverList.TabStop = false;
            this.serverList.Text = "Server List";
            // 
            // launchButton
            // 
            this.launchButton.Location = new System.Drawing.Point(119, 66);
            this.launchButton.Name = "launchButton";
            this.launchButton.Size = new System.Drawing.Size(129, 23);
            this.launchButton.TabIndex = 2;
            this.launchButton.Text = "Launch Connection";
            this.launchButton.UseVisualStyleBackColor = true;
            this.launchButton.Click += new System.EventHandler(this.launchButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 242);
            this.Controls.Add(this.serverList);
            this.Controls.Add(this.dbOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "KeePassToRDP";
            this.dbOptions.ResumeLayout(false);
            this.dbOptions.PerformLayout();
            this.serverList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenFileDialog openFileDialog1;
        private Button opendbButton;
        private Clients clients;
        private Label label1;
        private GroupBox dbOptions;
        private Label label2;
        private GroupBox serverList;
        private Button launchButton;
        private ClientComboBox comboBox1;
        private Button refreshButton;
    }
}

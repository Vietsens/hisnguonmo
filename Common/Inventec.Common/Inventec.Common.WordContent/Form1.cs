/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using Inventec.Common.WordContent;
using System.IO;

namespace Inventec.Common.WordContent
{
    /// <summary>
    /// just testing
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button load;
        private WinWordControl.WinWordControl winWordControl1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Restore;
        private System.Windows.Forms.Button close;
        private IContainer components;

        string fileName;
        string templateFileName;
        Inventec.Common.SignLibrary.ADO.InputADO emrInputADO;
        private Timer timer2;
        Dictionary<string, object> templateKey;


        public Form1()
        {
            InitializeComponent();
        }

        public Form1(WordContentADO wordContentADO)
        {
            InitializeComponent();
            try
            {
                if (wordContentADO != null)
                {
                    this.fileName = wordContentADO.FileName;
                    this.templateFileName = wordContentADO.TemplateFileName;
                    this.emrInputADO = wordContentADO.EmrInputADO;
                    this.templateKey = wordContentADO.TemplateKey;
                }
                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                catch (Exception ex)
                { }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// cleanuup ressources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // just to be shure!
            winWordControl1.CloseControl();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.load = new System.Windows.Forms.Button();
            this.winWordControl1 = new WinWordControl.WinWordControl();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.Restore = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // load
            // 
            this.load.Location = new System.Drawing.Point(592, 24);
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(56, 32);
            this.load.TabIndex = 1;
            this.load.Text = "load";
            this.load.Click += new System.EventHandler(this.load_Click);
            // 
            // winWordControl1
            // 
            this.winWordControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.winWordControl1.Location = new System.Drawing.Point(0, 0);
            this.winWordControl1.Name = "winWordControl1";
            this.winWordControl1.Size = new System.Drawing.Size(586, 389);
            this.winWordControl1.TabIndex = 2;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "WordDateien (*.doc)|*.doc|Word (*.docx)|*.docx";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(592, 152);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 32);
            this.button1.TabIndex = 3;
            this.button1.Text = "PreActivate";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Restore
            // 
            this.Restore.Location = new System.Drawing.Point(592, 208);
            this.Restore.Name = "Restore";
            this.Restore.Size = new System.Drawing.Size(56, 32);
            this.Restore.TabIndex = 4;
            this.Restore.Text = "Restore Word";
            this.Restore.Click += new System.EventHandler(this.Restore_Click);
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(592, 72);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(56, 32);
            this.close.TabIndex = 5;
            this.close.Text = "Close";
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(672, 389);
            this.Controls.Add(this.close);
            this.Controls.Add(this.Restore);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.winWordControl1);
            this.Controls.Add(this.load);
            this.Name = "Form1";
            this.Text = "Xem in";
            this.Activated += new System.EventHandler(this.OnActivate);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }
        #endregion


        private void OnActivate(object sender, System.EventArgs e)
        {

        }

        private void load_Click(object sender, System.EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                winWordControl1.LoadDocument(openFileDialog1.FileName);
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            winWordControl1.PreActivate();
        }

        private void Restore_Click(object sender, System.EventArgs e)
        {
            winWordControl1.RestoreWord();
        }

        private void close_Click(object sender, System.EventArgs e)
        {
            winWordControl1.CloseControl();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                timer2.Stop();
                if (!String.IsNullOrEmpty(this.fileName) && File.Exists(this.fileName))
                    winWordControl1.LoadDocument(this.fileName);
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Duong dan file khong hop le hoac file khong ton tai____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer2.Interval = 1000;
            timer2.Enabled = true;
            timer2.Start();
        }
    }
}

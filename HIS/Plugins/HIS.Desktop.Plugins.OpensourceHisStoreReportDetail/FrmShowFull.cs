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
using DevExpress.XtraLayout;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.OpensourceHisStoreReportDetail
{
    public partial class FrmShowFull : Form
    {
        bool isShowTextDocument = true;
        string ShowTextDetail = "";
        string pathIMGReport = "";
        public FrmShowFull(bool isShowText, string Text, string pathIMG)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            //this.WindowState = FormWindowState.Maximized;
            //this.Width = 1079;
            //this.Height = 720;
            this.ShowTextDetail = Text;
            this.pathIMGReport = pathIMG;
            this.isShowTextDocument = isShowText;
            InitializeComponent();
            SetCaptionByLanguageKey();
            FillDataToForm();
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.OpensourceHisStoreReportDetail.Resources.Lang", typeof(FrmShowFull).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FrmShowFull.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToForm()
        {
            try
            {
                if (isShowTextDocument == false)
                {
                    layoutControl1.Clear();
                    var layoutControlItem = new LayoutControlItem();

                    // Set the control to be managed by the LayoutControlItem
                    layoutControl1.Dock = DockStyle.Fill;
                    layoutControlGroup1.Size = MaximumSize;
                    PictureBox showImage = new PictureBox();
                    showImage.SizeMode = PictureBoxSizeMode.Zoom;
                    showImage.Dock = DockStyle.Fill;
                    showImage.MaximumSize = MaximumSize;
                    //showDocument.Controls. = true;
                    layoutControlItem.TextVisible = false;
                    layoutControlItem.Control = showImage;
                    if (pathIMGReport != null)
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] imageBytes = webClient.DownloadData(pathIMGReport);
                            using (var ms = new System.IO.MemoryStream(imageBytes))
                            {
                                showImage.Image = Image.FromStream(ms);
                            }
                        }
                    }
                    layoutControlGroup1.AddItem(layoutControlItem);
                }
                else
                {
                    layoutControl1.Clear();
                    var layoutControlItem = new LayoutControlItem();

                    // Set the control to be managed by the LayoutControlItem
                    layoutControl1.Dock = DockStyle.Fill;
                    layoutControlGroup1.Size = MaximumSize;
                    WebBrowser showDocument = new WebBrowser();
                    showDocument.Dock = DockStyle.Fill;
                    showDocument.MaximumSize = MaximumSize;
                    //showDocument.Controls. = true;
                    showDocument.DocumentText = ShowTextDetail;
                    layoutControlItem.TextVisible = false;
                    layoutControlItem.Control = showDocument;
                    layoutControlGroup1.AddItem(layoutControlItem);
                    //layoutControl1.Controls.Add(showDocument);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void FrmShowFull_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.IO;
using System.Net;

namespace HIS.Desktop.Plugins.OpensourceHisStore.CardItem
{
    public partial class CardItem : UserControl
    {
        public event EventHandler Click;
        HisStoreServiceReportSDO currentReport = new HisStoreServiceReportSDO();
        Inventec.Desktop.Common.Modules.Module moduleData;
        private Timer resizeTimer;
        private bool isMouseHovering;
        public CardItem(HisStoreServiceReportSDO reportDetail, Inventec.Desktop.Common.Modules.Module moduleData)
        {
            this.moduleData = moduleData;
            this.currentReport = reportDetail;
            InitializeComponent();
            SetCaptionByLanguageKey();
            LoadDetail(reportDetail);
            

            // Khởi tạo Timer
            resizeTimer = new Timer();
            resizeTimer.Interval = 100; // Thời gian giữa các lần thay đổi kích thước
            resizeTimer.Tick += new EventHandler(ResizeTimer_Tick);

            foreach (Control control in this.Controls)
            {
                control.Click += Control_Click;
                //control.MouseEnter += new EventHandler(CardItem_MouseEnter);
                //control.MouseLeave += new EventHandler(CardItem_MouseLeave);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.OpensourceHisStore.Resources.Lang", typeof(CardItem).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("CardItem.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtServiceName.Text = Inventec.Common.Resource.Get.Value("CardItem.txtServiceName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtServiceCode.Text = Inventec.Common.Resource.Get.Value("CardItem.txtServiceCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSumary.Text = Inventec.Common.Resource.Get.Value("CardItem.txtSumary.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtDepartment.Text = Inventec.Common.Resource.Get.Value("CardItem.txtDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtRate.Text = Inventec.Common.Resource.Get.Value("CardItem.txtRate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPrice.Text = Inventec.Common.Resource.Get.Value("CardItem.txtPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Control_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.currentReport != null)
                {
                    
                    List<object> listArgs = new List<object>();
                    HisStoreServiceReportSDO dataReport = new HisStoreServiceReportSDO();
                    if (currentReport != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisStoreServiceReportSDO>(dataReport, currentReport);
                        listArgs.Add(dataReport);
                    }
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.OpensourceHisStoreReportDetail", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDetail(HisStoreServiceReportSDO reportDetail)
        {
            try
            {
                if (reportDetail != null)
                {
                    this.layoutControl1.Appearance.Control.BorderColor = Color.Black;
                    this.layoutControl1.Appearance.Control.Options.UseBorderColor = true;

                    txtServiceCode.Text = reportDetail.ServiceCode.ToString();
                    txtServiceName.Text = reportDetail.ServiceName.ToString();
                    var isBuy = reportDetail.IsInstalled ?? false;
                    if (isBuy)
                    {
                        txtPrice.Text = "Đã tải";
                    }
                    else
                    {
                        if (reportDetail.Price == 0)
                        {
                            txtPrice.Text = "Miễn phí";
                        }
                        else
                        {
                            decimal price = Convert.ToDecimal(reportDetail.Price);
                            txtPrice.Text = price.ToString("#,##0");
                        }
                    }
                    txtDepartment.Text = reportDetail.ServiceCategoryName.ToString();
                    txtRate.Text = (reportDetail.AverageRating ?? 0).ToString() + "★ |";
                    txtSumary.Text = !string.IsNullOrEmpty(reportDetail.Summary) ? (reportDetail.Summary.Length > 130 ? reportDetail.Summary.Substring(0,130).ToString() +"..." : reportDetail.Summary) : "Summary";
                    if (!String.IsNullOrEmpty(reportDetail.AuthorLogoUrl))
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] imageBytes = webClient.DownloadData(reportDetail.AuthorLogoUrl);
                            using (var ms = new System.IO.MemoryStream(imageBytes))
                            {
                                ImageAuthor.Image = Image.FromStream(ms);
                            }
                        }
                    }
                    else
                    {
                        string pathLocal = GetPathDefault();
                        ImageAuthor.Image = Image.FromFile(pathLocal);
                    }


                    if (!String.IsNullOrEmpty(reportDetail.ImageUrl))
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] imageBytes = webClient.DownloadData(reportDetail.ImageUrl);
                            using (var ms = new System.IO.MemoryStream(imageBytes))
                            {
                                ImageUrl.Image = Image.FromStream(ms);
                            }
                        }
                    }
                    else
                    {
                        string pathLocal = GetPathDefault();
                        ImageUrl.Image = Image.FromFile(pathLocal);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string GetPathDefault()
        {
            string imageDefaultPath = string.Empty;
            try
            {
                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                imageDefaultPath = localPath + "\\Img\\ImageStorage\\notImage.jpg";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return imageDefaultPath;
        }

        private void CardItem_MouseEnter(object sender, EventArgs e)
        {
        //    try
        //    {
        //        isMouseHovering = true;
        //        resizeTimer.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        }

        private void CardItem_MouseLeave(object sender, EventArgs e)
        {
        //    try
        //    {
        //        isMouseHovering = false;
        //        resizeTimer.Start();
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        }
        private void ResizeTimer_Tick(object sender, EventArgs e)
        {
            if (isMouseHovering)
            {
                if (this.Width < 350 && this.Height < 350) // Giới hạn kích thước tối đa
                {
                    this.Size = new Size(this.Width + 10, this.Height + 10);
                }
                else
                {
                    resizeTimer.Stop();
                }
            }
            else
            {
                if (this.Width > 300 && this.Height > 300) // Giới hạn kích thước tối thiểu
                {
                    this.Size = new Size(this.Width - 10, this.Height - 10);
                }
                else
                {
                    resizeTimer.Stop();
                }
            }
        }
        //private void LoadData(string CategoryCode, string CategoryName,int index)
        //{
        //    try
        //    {
        //        int buttonWidth = 250;
        //        int buttonHeight = 40;
        //        int padding = 10;
        //        int startX = 12;
        //        int startY = 45;
        //        SimpleButton btnAll = new DevExpress.XtraEditors.SimpleButton();
        //        btnAll.Location = new Point(startX, startY + (buttonHeight + padding));
        //        btnAll.Name = CategoryCode;
        //        btnAll.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
        //        btnAll.StyleController = this.layoutControl1;
        //        btnAll.TabIndex = index;
        //        btnAll.Text = CategoryName;
        //        this.layoutControl1.Controls.Add(btnAll);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
    }
}

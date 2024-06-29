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
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using MOS.SDO;
using SAR.SDO;
using System.IO;
using Inventec.Common.SignLibrary;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Net;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.OpensourceHisStoreReportDetail
{
    public partial class frmDetailMarketReport : DevExpress.XtraEditors.XtraForm
    {
        List<LicenseSDO> Licenses = new List<LicenseSDO>();
        HisStoreServiceReportDetailSDO ListData;
        HisStoreServiceReportBuySDO ListReportBuySDO;
        HisStoreServiceReportSDO resultData = null;
        Inventec.Desktop.Common.Modules.Module moduleData;
        public frmDetailMarketReport(Inventec.Desktop.Common.Modules.Module moduleData, HisStoreServiceReportSDO listData)
        {
            InitializeComponent();
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;
                this.WindowState = FormWindowState.Maximized;
                this.moduleData = moduleData;
                this.Text = moduleData.text;
                this.resultData = listData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDetailMarketReport_Load(object sender, EventArgs e)
        {
            try
            {
            //WaitingManager.Show();
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            GetData();
            FillDataToControl();
            SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.OpensourceHisStoreReportDetail.Resources.Lang", typeof(frmDetailMarketReport).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPurchase.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.btnPurchase.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetData()
        {
            try
            {
                CommonParam param = new CommonParam();
                //var a = new BackendAdapter(param).Get<List<HisStoreServiceReportDetailSDO>>("api/HisStoreService/ReportGetDetail", ApiConsumers.MosConsumer, filter, param);
                ListData = new BackendAdapter(param).Get<HisStoreServiceReportDetailSDO>("api/HisStoreService/ReportGetDetail", ApiConsumers.MosConsumer, resultData.ServiceCode, param);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListData),
                    ListData) 
                );

                if (ListData != null)
                {
                    decimal? formatPrice = ListData.Price;
                    if (formatPrice.HasValue)
                    {
                        string formattedPrice = formatPrice.Value.ToString("N0");
                        txtPrice.Text = formattedPrice;
                    }
                    else
                    {
                        txtPrice.Text = "";
                    }
                    //txtPrice.Text = ListData.Price.ToString();
                    //decimal price = ListData.Price ?? 0;
                    txtCodeReport.Text = ListData.ServiceCode;
                    txtNameReport.Text = ListData.ServiceName;
                    txtTypeReport.Text = ListData.ServiceCategoryName;
                    txtAuthor.Text = ListData.AuthorFullName;
                  
                    txtPurchaseNum.Text = ListData.BuyingCount.ToString();
                    if (ListData.IsPurchased == true)
                    { 
                        txtPurchaseStatus.Text = "Đã mua";
                        txtPurchaseStatus.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
                        btnPurchase.Enabled = false;
                    }
                    else
                    {
                        txtPurchaseStatus.Text = "Chưa mua";
                        btnPurchase.Enabled = true;
                    }
                    txtDescribe.Text = ListData.Description;
                    if (ListData.DescriptionUrl != null)
                    {
                        string tempFilePath = Path.Combine(Path.GetTempPath(), "temp.pdf");
                       
                        if (File.Exists(tempFilePath))
                        {
                            // Nếu tồn tại, xóa tệp
                            File.Delete(tempFilePath);
                        }
                        // Tải tệp PDF từ URL
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(new Uri(ListData.DescriptionUrl), tempFilePath);
                        }
                        // Hiển thị tệp PDF trong PdfViewer
                        pdfViewer.LoadDocument(tempFilePath);
                        pdfViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToWidth;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn muốn mua báo cáo này?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    HisStoreServiceReportBuyInputSDO filter = new HisStoreServiceReportBuyInputSDO();
                    filter.ServiceCode = resultData.ServiceCode;
                    ListReportBuySDO = new BackendAdapter(param).Post<HisStoreServiceReportBuySDO>("api/HisStoreService/ReportBuy", ApiConsumers.MosConsumer, filter, param);
                    if (ListReportBuySDO != null)
                    {
                        if (ListReportBuySDO.Licenses != null && ListReportBuySDO.Licenses.Count > 0)
                        {
                            //GetData();
                            //FillDataToControl();
                            Licenses.AddRange(ListReportBuySDO.Licenses);
                            //MessageManager.Show(this, param, true);
                            CreateReport();
                            WaitingManager.Hide();
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Goi api huy yeu cau mua bao cao that bai____Api uri:api/HisStoreService/ReportBuy");
                            WaitingManager.Hide();
                            MessageManager.Show(this, param, false);
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Goi api huy yeu cau mua bao cao that bai____Api uri:api/HisStoreService/ReportBuy");
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, false);
                    }
                }
                else
                    return;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    
        private void CreateReport()
        {
            try 
            {
                CommonParam param = new CommonParam();
                SarReportTypeMakeFromStoreSDO filter = new SarReportTypeMakeFromStoreSDO();
                filter.ReportTypeCode = ListReportBuySDO.ServiceCode;
                filter.SqlScript = ListReportBuySDO.SqlScript;
                filter.Description = ListReportBuySDO.Description;
                filter.ReportTypeName = ListReportBuySDO.ServiceName;
                filter.StoreTemplateUrl = ListReportBuySDO.TemplateUrl;
                bool suscess = new BackendAdapter(param).Post<bool>("api/SarReportType/MakeFromStore", ApiConsumers.SarConsumer, filter, param);
                if (suscess == true)
                {
                    CreSDALICENSE();
                }
                else
                {
                    MessageManager.Show(this, param, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreSDALICENSE()
        {
            try
            {
                List<SDA_LICENSE> lstSda = new List<SDA_LICENSE>();
                if (this.Licenses != null && this.Licenses.Count > 0)
                {
                    foreach (var item in Licenses)
                    {
                        SDA_LICENSE sda = new SDA_LICENSE();
                        sda.LICENSE = item.LicenseCode;
                        sda.APP_CODE = item.AppCode;
                        sda.CLIENT_CODE = item.SubCode;
                        sda.EXPIRED_DATE = item.ExpiredTime;
                        sda.FEATURE_CODE = resultData.ServiceCode;
                        lstSda.Add(sda);
                    }
                }
                if (lstSda != null && lstSda.Count >= 0)
                {
                        CommonParam paramCommon = new CommonParam();
                        //SdaLicenseFilter filter = new SdaLicenseFilter();
                        List<SDA_LICENSE> resuilt = new List<SDA_LICENSE>();
                        resuilt = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<List<SDA_LICENSE>>("api/SdaLicense/CreateList", ApiConsumers.SdaConsumer, lstSda, paramCommon);
                        if (resuilt != null && resuilt.Count > 0)
                        {
                            MessageManager.Show(this, paramCommon, true);
                            GetData();
                            FillDataToControl();
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("api/SdaLicense/CreateList khong hoat dong." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resuilt), resuilt));
                            MessageManager.Show(this, paramCommon, false);
                        }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDetailMarketReport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                 if (e.Control && e.KeyCode == Keys.B)
                    {
                        btnPurchase_Click(null, null);
                    }
            }
            catch (Exception ex)
            {
               Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDetailMarketReport_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                pdfViewer.CloseDocument();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

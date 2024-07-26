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
using SAR.SDO;
using System.IO;
using Inventec.Common.SignLibrary;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Net;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using MOS.SDO;
using MOS.Filter;
using DevExpress.XtraCharts;

namespace HIS.Desktop.Plugins.OpensourceHisStoreReportDetail
{
    public partial class frmDetailMarketReport : DevExpress.XtraEditors.XtraForm
    {
        List<LicenseSDO> Licenses = new List<LicenseSDO>();
        HisStoreServiceReportDetailSDO ListData;
        HisStoreServiceReportBuySDO ListReportBuySDO;
        HisStoreServiceReportSDO resultData = null;
        List<string> lstImageUrls;
        ReportReviewResultSDO lstEvaluate;
        Inventec.Desktop.Common.Modules.Module moduleData;
        long? yourRating;
        bool IsAcctive = false;
        private int currentIndex = 0;
        public frmDetailMarketReport(Inventec.Desktop.Common.Modules.Module moduleData, HisStoreServiceReportSDO listData)
        {
            InitializeComponent();
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;
                //this.WindowState = FormWindowState.Maximized;
                //this.AutoScroll = false;
                this.moduleData = moduleData;
                this.Text = moduleData.text;
                this.resultData = listData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private int currentPage = 0;
        private int pageSize = 10;
        //private int totalLoadedData = 0;
        private void frmDetailMarketReport_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                GetData();
                SetCaptionByLanguageKey();
                LoadDataForm();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPaging(object param, bool isLoadMore)
        {
            try
            {
                //get data comment
                Inventec.Common.Logging.LogSystem.Debug("Load data to list");
                int limit = ((CommonParam)param).Limit ?? 0;
                int start = isLoadMore ? 0 : ((CommonParam)param).Start ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<ReportCommentGetPagingResultSDO>> apiResult = null;

                ReportCommentGetPagingFilter Filter = new ReportCommentGetPagingFilter();
                Filter.ServiceCode = resultData.ServiceCode;
                Filter.ParentId = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<ReportCommentGetPagingResultSDO>>("api/HisStoreService/ReportCommentGetPaging", ApiConsumers.MosConsumer, Filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null)
                    {
                        LoadUC(data);
                    }
                }
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
                this.labelControl8.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.labelControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl7.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.labelControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.labelControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.labelControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.labelControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.showFullDescribe.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.showFullDescribe.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox2.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.groupBox2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox1.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.groupBox1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPurchase.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.btnPurchase.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSummary.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.txtSummary.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblAverageRating.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.lblAverageRating.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtAuthor.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.txtAuthor.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTypeReport.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.txtTypeReport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtNameReport.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.txtNameReport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtCodeReport.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.txtCodeReport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblNumAllRatings.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.lblNumAllRatings.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblAverageRatings.Text = Inventec.Common.Resource.Get.Value("frmDetailMarketReport.lblAverageRatings.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                    txtSummary.Text = ListData.Summary;
                    txtCodeReport.Text = ListData.ServiceCode;
                    txtNameReport.Text = ListData.ServiceName;
                    txtTypeReport.Text = ListData.ServiceCategoryName;
                    txtAuthor.Text = ListData.AuthorFullName;
                    lblAverageRating.Text = ListData.AverageRating.ToString();
                    lblNumAllRatings.Text = string.Format("{0} Ratings", ListData.NumOfReview);
                    //lblDescribe.Text = ListData.Description;
                    //lblDescribe.AutoSizeInLayoutControl = true;
                    //lblDescribe.Text = !string.IsNullOrEmpty(ListData.Description) ? (ListData.Description.Length > 2000 ? ListData.Description.Substring(0, 2000).ToString() + "..." : ListData.Description) : "Description";
                    txtSummary.Text = ListData.Summary.ToString();
                    yourRating = ListData.YourRating;
                    lstImageUrls = ListData.ImageUrls;
                    showDetaiDescribe.DocumentText = ListData.Description;
                    if (ListData.IsInstalled == true)
                    {
                        IsAcctive = true;
                    }

                    if (!String.IsNullOrEmpty(ListData.AuthorLogoUrl))
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] imageBytes = webClient.DownloadData(ListData.AuthorLogoUrl);
                            using (var ms = new System.IO.MemoryStream(imageBytes))
                            {
                                picAuthor.Image = Image.FromStream(ms);
                            }
                        }
                    }

                    if (lstImageUrls.Count > 0)
                    {
                        string firstImageUrl = lstImageUrls[0];
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] imageBytes = webClient.DownloadData(firstImageUrl);
                            using (var ms = new System.IO.MemoryStream(imageBytes))
                            {
                                picReportIMG.Image = Image.FromStream(ms);
                            }
                        }
                    }
                    LoadRating();
                    LoadChartRating();
                }

            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadChartRating()
        {
            try
            {
                long NumOf1 = ListData.NumOfRating1 ?? 0;
                long NumOf2 = ListData.NumOfRating2 ?? 0;
                long NumOf3 = ListData.NumOfRating3 ?? 0;
                long NumOf4 = ListData.NumOfRating4 ?? 0;
                long NumOf5 = ListData.NumOfRating5 ?? 0;
                long NumOfAll = ListData.NumOfReview ?? 0;

                chartRating.Series.Clear();
                Series series = new Series("Ratings", ViewType.Bar);

                if (NumOfAll != null && NumOfAll > 0)
                {
                    // Thêm các điểm dữ liệu vào Series
                    series.Points.Add(new SeriesPoint("1", new double[] { (NumOf1 * 100) / NumOfAll }));
                    series.Points.Add(new SeriesPoint("2", new double[] { (NumOf2 * 100) / NumOfAll }));
                    series.Points.Add(new SeriesPoint("3", new double[] { (NumOf3 * 100) / NumOfAll }));
                    series.Points.Add(new SeriesPoint("4", new double[] { (NumOf4 * 100) / NumOfAll }));
                    series.Points.Add(new SeriesPoint("5", new double[] { (NumOf5 * 100) / NumOfAll }));

                    // Thêm Series vào ChartControl
                    chartRating.Series.Add(series);
                }

                // Lấy đối tượng XYDiagram từ ChartControl
                XYDiagram diagram = (XYDiagram)chartRating.Diagram;

                // Thiết lập thuộc tính Rotated để làm cho biểu đồ nằm ngang
                diagram.Rotated = true;
                diagram.AxisX.Label.Visible = false;
                diagram.AxisY.Label.Visible = false;
                //// Tùy chỉnh trục X và Y
                //diagram.AxisX.Title.Text = "Percentage";
                //diagram.AxisX.Title.Visible = true;
                //diagram.AxisY.Title.Text = "Ratings";
                //diagram.AxisY.Title.Visible = true;

                // Tùy chỉnh hiển thị thanh ngang
                BarSeriesView view = (BarSeriesView)series.View;
                view.BarWidth = 0.5;

                // Tùy chỉnh màu sắc của các thanh
                series.Points[0].Color = Color.Orange;
                series.Points[1].Color = Color.Orange;
                series.Points[2].Color = Color.Orange;
                series.Points[3].Color = Color.Orange;
                series.Points[4].Color = Color.Orange;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadUC(List<ReportCommentGetPagingResultSDO> lstComment)
        {
            try
            {
                int rowIndex = tableComment.ColumnCount;
                int columnIndex = 0;
                if (lstComment.Count > 0)
                {
                    foreach (var item in lstComment)
                    {
                        bool isShowRepply = false;
                        UCItemComment ItemComment = new UCItemComment(item, this.moduleData, resultData, isShowRepply,
                            null) { Dock = DockStyle.Fill };
                        tableComment.Controls.Add(ItemComment, columnIndex, rowIndex);
                        //columnIndex++;
                        //ItemComment.Click += new EventHandler(CardItem_Click);
                         rowIndex++;
                         tableComment.RowCount = rowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                  Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadDataForm()
        {
            FillDataToControl();
            tableComment.DataBindings.Clear();
            CommonParam param = new CommonParam();
            param.Limit = pageSize;
            //param.Count = totalLoadedData;
            LoadPaging(param, false);
        }


        private void LoadRating()
        {
            try
            {

                if (ListData.YourRating == 1)
                {
                    Star1();
                }
                else if (ListData.YourRating == 2)
                {
                    Star2();
                }
                else if (ListData.YourRating == 3)
                {
                    Star3();
                }
                else if (ListData.YourRating == 4)
                {
                    Star4();
                }
                else if (ListData.YourRating == 5)
                {
                    Star5();
                }
                else
                {
                    picStar1.Image = Properties.Resources.StarNotRated;
                    picStar2.Image = Properties.Resources.StarNotRated;
                    picStar3.Image = Properties.Resources.StarNotRated;
                    picStar4.Image = Properties.Resources.StarNotRated;
                    picStar5.Image = Properties.Resources.StarNotRated;
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
                if (ListData.IsInstalled == true)
                {
                    if (MessageBox.Show("Bạn đã tải báo cáo này. Bạn muốn tiếp tục tải lại?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ReportBuy();
                    }
                    else
                        return;
                }
                else if (ListData.FeeType == "FREE")
                {
                    if (MessageBox.Show("Bạn muốn tải báo cáo này?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ReportBuy();
                    }
                    else
                        return;
                }
                else if (ListData.FeeType == "PAID")
                {
                    if (MessageBox.Show("Bạn muốn mua báo cáo này?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ReportBuy();
                    }
                    else
                        return;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReportBuy()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisStoreServiceReportBuyInputSDO InputSDO = new HisStoreServiceReportBuyInputSDO();
                InputSDO.ServiceCode = resultData.ServiceCode;
                ListReportBuySDO = new BackendAdapter(param).Post<HisStoreServiceReportBuySDO>("api/HisStoreService/ReportDownload", ApiConsumers.MosConsumer, InputSDO, param);
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
                        Inventec.Common.Logging.LogSystem.Warn("Goi api huy yeu cau mua bao cao that bai____Api uri:api/HisStoreService/ReportDownload");
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
                SarReportTypeMakeFromStoreSDO updateSDO = new SarReportTypeMakeFromStoreSDO();
                updateSDO.ReportTypeCode = ListReportBuySDO.ServiceCode;
                updateSDO.SqlScript = ListReportBuySDO.SqlScript;
                updateSDO.Description = ListReportBuySDO.Summary;
                updateSDO.ReportTypeName = ListReportBuySDO.ServiceName;
                updateSDO.StoreTemplateUrl = ListReportBuySDO.TemplateUrl;
                bool suscess = new BackendAdapter(param).Post<bool>("api/SarReportType/MakeFromStore", ApiConsumers.SarConsumer, updateSDO, param);
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

        private void picStar1_Click(object sender, EventArgs e)
        {
            try
            {
                yourRating = 1;
                if (IsAcctive == true)
                {
                    if (Evaluate(yourRating) == true)
                    {
                        //Star1();
                        GetData();
                        FillDataToControl();
                    }
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void picStar2_Click(object sender, EventArgs e)
        {
            try
            {
                yourRating = 2;
                if (IsAcctive == true)
                {
                    if (Evaluate(yourRating) == true)
                    {
                        //Star2();
                        GetData();
                        FillDataToControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void picStar3_Click(object sender, EventArgs e)
        {
            try
            {
                yourRating = 3;
                if (IsAcctive == true)
                {
                    //picStar1.Image = Properties.Resources.StarRated;
                    if (Evaluate(yourRating) == true)
                    {
                        //Star3();
                        GetData();
                        FillDataToControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void picStar4_Click(object sender, EventArgs e)
        {
            try
            {
                yourRating = 4;
                if (IsAcctive == true)
                {
                    if (Evaluate(yourRating) == true)
                    {
                        //Star4();
                        GetData();
                        FillDataToControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void picStar5_Click(object sender, EventArgs e)
        {
            try
            {
                yourRating = 5;
                if (IsAcctive == true)
                {
                    if (Evaluate(yourRating) == true)
                    {
                        //Star5();
                        GetData();
                        FillDataToControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Star1()
        {
            try
            {
                picStar1.Image = Properties.Resources.StarRated;
                picStar2.Image = Properties.Resources.StarNotRated;
                picStar3.Image = Properties.Resources.StarNotRated;
                picStar4.Image = Properties.Resources.StarNotRated;
                picStar5.Image = Properties.Resources.StarNotRated;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void Star2()
        {
            try
            {
                picStar1.Image = Properties.Resources.StarRated;
                picStar2.Image = Properties.Resources.StarRated;
                picStar3.Image = Properties.Resources.StarNotRated;
                picStar4.Image = Properties.Resources.StarNotRated;
                picStar5.Image = Properties.Resources.StarNotRated;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void Star3()
        {
            try
            {
                picStar1.Image = Properties.Resources.StarRated;
                picStar2.Image = Properties.Resources.StarRated;
                picStar3.Image = Properties.Resources.StarRated;
                picStar4.Image = Properties.Resources.StarNotRated;
                picStar5.Image = Properties.Resources.StarNotRated;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Star4()
        {
            try
            {
                picStar1.Image = Properties.Resources.StarRated;
                picStar2.Image = Properties.Resources.StarRated;
                picStar3.Image = Properties.Resources.StarRated;
                picStar4.Image = Properties.Resources.StarRated;
                picStar5.Image = Properties.Resources.StarNotRated;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Star5()
        {
            try
            {
                picStar1.Image = Properties.Resources.StarRated;
                picStar2.Image = Properties.Resources.StarRated;
                picStar3.Image = Properties.Resources.StarRated;
                picStar4.Image = Properties.Resources.StarRated;
                picStar5.Image = Properties.Resources.StarRated;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool Evaluate(long? Rating)
        {
            bool sucssess = true;
            try
            {
                WaitingManager.Show();
                CommonParam paramCommon = new CommonParam();
                ReportReviewInputSDO filter = new ReportReviewInputSDO();
                filter.ServiceCode = resultData.ServiceCode;
                filter.Rating = Rating ?? 0;
                lstEvaluate = new BackendAdapter(paramCommon).Post<ReportReviewResultSDO>("api/HisStoreService/ReportReview", ApiConsumers.MosConsumer, filter, paramCommon);
                if (lstEvaluate != null)
                {
                    MessageManager.Show(this, paramCommon, true);
                }
                else
                {
                    MessageManager.Show(this, paramCommon, false);
                }
                WaitingManager.Hide();
                return sucssess;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void picPrev_Click(object sender, EventArgs e)
        {
            try
            {
                currentIndex = (currentIndex + 1) % lstImageUrls.Count;
                string currentImageUrl = lstImageUrls[currentIndex];

                if (lstImageUrls.Count > 0)
                {
                    // string firstImageUrl = lstImageUrls[0];
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] imageBytes = webClient.DownloadData(currentImageUrl);
                        using (var ms = new System.IO.MemoryStream(imageBytes))
                        {
                            picReportIMG.Image = Image.FromStream(ms);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void picNext_Click(object sender, EventArgs e)
        {
            try
            {
                // currentIndex = (currentIndex + lstImageUrls.Count) % lstImageUrls.Count;
                currentIndex = (currentIndex + 1) % lstImageUrls.Count;
                string currentImageUrl = lstImageUrls[currentIndex];

                if (lstImageUrls.Count > 0)
                {
                    // string firstImageUrl = lstImageUrls[0];
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] imageBytes = webClient.DownloadData(currentImageUrl);
                        using (var ms = new System.IO.MemoryStream(imageBytes))
                        {
                            picReportIMG.Image = Image.FromStream(ms);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShowComment_Click(object sender, EventArgs e)
        {
            try
            {
                frmListComment ListComment = new frmListComment(resultData, this.moduleData, null, false);
                ListComment.Show();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void showFullDescribe_Click(object sender, EventArgs e)
        {
            try
            {
                FrmShowFull showFullDetail = new FrmShowFull(true, ListData.Description, null);
                showFullDetail.Show();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void picReportIMG_Click(object sender, EventArgs e)
        {
            try
            {
                if (picReportIMG.Image != null)
                {
                    string currentImageUrl = lstImageUrls[currentIndex];
                    FrmShowFull showFullDetail = new FrmShowFull(false, null, currentImageUrl);
                    showFullDetail.Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



    }
}

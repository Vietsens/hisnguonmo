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
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Controls.Session;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Reflection;
using MOS.Filter;
using MOS.SDO;
using DevExpress.Data;
using DevExpress.XtraLayout;
using System.Drawing.Drawing2D;
using Inventec.CustomControls;

namespace HIS.Desktop.Plugins.OpensourceHisStore.frmHisStore
{
    public partial class frmHisStore : DevExpress.XtraEditors.XtraForm
    {
        #region Init variables
        Inventec.Desktop.Common.Modules.Module moduleData;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomID = 0;
        long roomTypeID = 0;
        bool firtLoad = true;
        HisStoreServiceReportFilter filter = new HisStoreServiceReportFilter();
        #endregion
        public frmHisStore(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.roomID = moduleData.RoomId;
            this.roomTypeID = moduleData.RoomTypeId;
        }
        #region FormAction
        private void frmHisStore_Load(object sender, EventArgs e)
        {
            try
            {
                SetDeaultValue();
                //FillDataToControl();
                SetCaptionByLanguageKey();
                CreateButtons();
                loadPrice();
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                //RegisterMouseWheelEvent(this.listReport);

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void RegisterMouseWheelEvent(Control control)
        {
            try
            {
                control.MouseWheel += TablePanel_MouseWheel;

                // Đăng ký sự kiện cho các Control con
                foreach (Control child in control.Controls)
                {
                    RegisterMouseWheelEvent(child);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void TablePanel_MouseWheel(object sender, MouseEventArgs e)
        {
            // Điều chỉnh giá trị VerticalScroll
            if (this.listReport.VerticalScroll.Visible)
            {
                int scrollChange = e.Delta > 0 ? -20 : 20; // Điều chỉnh giá trị cuộn theo nhu cầu của bạn
                this.listReport.VerticalScroll.Value = Math.Max(0, Math.Min(this.listReport.VerticalScroll.Maximum, this.listReport.VerticalScroll.Value + scrollChange));
                this.listReport.PerformLayout();
            }
        }
        private void frmHisStore_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    //b.PerformClick();
                }

            }
            catch (Exception ex )
            {
                LogSystem.Warn(ex.Message);
                throw;
            }
        }
        #endregion
        #region setLanguage resource
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.OpensourceHisStore.Resources.Lang", typeof(frmHisStore).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisStore.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisStore.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlReport.Text = Inventec.Common.Resource.Get.Value("frmHisStore.layoutControlReport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisStore.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisStore.txtSearchValue.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisStore.txtSearchValue.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlCategory.Text = Inventec.Common.Resource.Get.Value("frmHisStore.layoutControlCategory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisStore.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cbboPrice.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisStore.cbboPrice.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
        #region load data
        public class PriceOption
        {                          
            public int ID { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }
        List<PriceOption> priceOptions = new List<PriceOption>
                {
                    new PriceOption { ID = 0, Name = "Tất cả",Value ="" },
                    new PriceOption { ID = 1, Name = "Miễn phí",Value ="FREE" },
                    new PriceOption { ID = 2, Name = "Trả phí" ,Value ="PAID"}
                };
        private void loadPrice()
        {
            try
            {
                
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 190, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "ID", columnInfos, false, 100);
                ControlEditorLoader.Load(cbboPrice, priceOptions, controlEditorADO);
                cbboPrice.EditValue = 0;

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private List<PNSimpleButton> buttonList = new List<PNSimpleButton>();
        private void CreateButtons()
        {
            List<HisStoreServiceReportCategorySDO> data = new List<HisStoreServiceReportCategorySDO>();
            data.Add(new HisStoreServiceReportCategorySDO { ServiceCategoryCode = "ALL", ServiceCategoryName = "Tất Cả" });

            Inventec.Common.Logging.LogSystem.Debug("Load category type");
            CommonParam param = new CommonParam();
            MOS.Filter.HisStoreServiceReportFilter filter = new HisStoreServiceReportFilter();
            Inventec.Common.Logging.LogSystem.Debug("Begin call API : api/HisStoreService/ReportCategoryGet");
            var rs = new BackendAdapter(param).Get<List<MOS.SDO.HisStoreServiceReportCategorySDO>>("api/HisStoreService/ReportCategoryGet", ApiConsumers.MosConsumer, filter, param);
            if (rs != null) data.AddRange(rs);

            int buttonWidth = 150; // Độ rộng của button
            int buttonHeight = 30; // Độ cao của button
            int startX = 30; // Vị trí bắt đầu của button theo trục X
            int startY = 10; // Vị trí bắt đầu của button theo trục Y
            int paddingX = 5; // Khoảng cách giữa các button theo trục X
            int paddingY = 10; // Khoảng cách giữa các button theo trục Y
            int columns = Screen.PrimaryScreen.Bounds.Width / (buttonWidth + paddingX);
            this.layoutControlCategory.AutoScroll = true;
            for (int i = 0; i < data.Count; i++)
            {
                PNSimpleButton button = new PNSimpleButton();
                button.Name = data[i].ServiceCategoryCode; // Đặt tên cho button
                button.Text = data[i].ServiceCategoryName; // Đặt nội dung cho button
                button.Size = new Size(buttonWidth, buttonHeight); // Đặt kích thước cho button
                button.Location = new Point(startX + (i % columns) * (buttonWidth + paddingX), startY + (i / columns) * (buttonHeight + paddingY)); // Tính toán vị trí của button
                button.BackColor = Color.White;
                button.BorderRadius = 20;
                button.BorderSize = 1;
                button.TextColor = Color.Black;
                button.Padding = new Padding(1);
                button.Font = new Font("Tahoma", 10);
                button.Click += (sender, e) =>
                {
                    foreach (var btn in buttonList)
                    {
                        if (btn == sender)
                        {
                            btn.TextColor = Color.Blue; 
                            btn.BorderColor = Color.Blue;
                            if (!btn.Name.Equals("ALL")) this.filter.ServiceCategoryCode = btn.Name;
                            else this.filter.ServiceCategoryCode = "";
                            FillDataToControl();
                            this.firtLoad = false;
                        }
                        else
                        {
                            btn.ForeColor = Color.Black;
                            btn.BorderColor = Color.White;
                        }
                    }
                };
                buttonList.Add(button);
                layoutControlCategory.Controls.Add(button);
                this.layoutControlCategory.Padding = new Padding(15);

            }
            if (buttonList.Count > 0)
            {
                buttonList[0].PerformClick();
                buttonList[0].Focus();

            }


        }
        
        
        private int currentPage = 0;
        private int pageSize = 12; 
        private int totalLoadedData = 0;

        private void FillDataToControl()
        {
            try
            {
                if (firtLoad) return;
                WaitingManager.Show();
                listReport.Controls.Clear(); 
                currentPage = 0;
                totalLoadedData = 0; 

                CommonParam param = new CommonParam();
                param.Limit = pageSize;
                param.Count = totalLoadedData;

                listReport.DataBindings.Clear();
                LoadPaging(param,false);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }


        private void LoadPaging(object param, bool isLoadMore)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Load data to list");
                int limit = ((CommonParam)param).Limit ?? 0;
                int start = isLoadMore ? 0 : ((CommonParam)param).Start ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<HisStoreServiceReportSDO>> apiResult = null;
                SetFilter();

                Inventec.Common.Logging.LogSystem.Debug("Filter to load data, Filter :"+LogUtil.TraceData("____filter:",this.filter));
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HisStoreServiceReportSDO>>("api/HisStoreService/ReportGet", ApiConsumers.MosConsumer, this.filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    
                    if (data != null)
                    {
                        if (isLoadMore)
                        {
                            FillToUCControl(data);
                        }
                        else
                        {
                            listReport.Controls.Clear();
                            FillToUCControl(data);
                        }
                        rowCount = data.Count;
                        dataTotal = apiResult.Param.Count ?? 0;
                        totalLoadedData += data.Count;
                    }
                    else
                    {
                       
                        MessageManager.Show(this, paramCommon, false);
                    }
                }
                
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }

            catch (Exception ex)
            {
                LogSystem.Error(ex.Message);
                throw;
            }
        }


        private void LoadMoreData()
        {
            try
            {
                if (totalLoadedData < dataTotal)
                {
                    CommonParam param = new CommonParam();
                    param.Limit = pageSize + totalLoadedData;
                    param.Start = totalLoadedData;
                    LoadPaging(param, true);
                }
                
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void FillToUCControl(List<HisStoreServiceReportSDO> data)
        {
            try
            {
                
                data = data.OrderByDescending(s => s.AverageRating).ThenBy(o => o.ServiceCode).ToList();
                int rowIndex = listReport.RowCount;
                int columnIndex = 0;
                listReport.Controls.Clear();
                listReport.RowStyles.Clear();
                this.listReport.AutoScroll = true;
                this.listReport.AutoSize = false;
                this.listReport.HorizontalScroll.Visible = false;
                foreach (var item in data)
                {
                    CardItem.CardItem cardItem = new CardItem.CardItem(item, this.moduleData) { Dock = DockStyle.Fill };
                    listReport.Controls.Add(cardItem, columnIndex, rowIndex);
                    columnIndex++;
                    
                    if (columnIndex >= listReport.ColumnCount)
                    {
                        columnIndex = 0;
                        rowIndex++;
                    }
                }
                listReport.RowCount = rowIndex + 1;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void CardItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Click");
        }
        private void SetFilter()
        {
            if (!string.IsNullOrEmpty(txtSearchValue.Text.Trim()))
            {
                this.filter.Keyword = txtSearchValue.Text.Trim();
            }
            else this.filter.Keyword = "";
        }
            
        private void SetDeaultValue()
        {
            try
            {
                txtSearchValue.Text = "";
                cbboPrice.EditValue  = 0;
                cbboPrice.Text = priceOptions.Where(s => s.ID == Convert.ToInt64(cbboPrice.EditValue)).Select(o => o.Name).FirstOrDefault();
    
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex.Message);
                throw;
            }
        }
        #endregion
        #region handle click
        private void listReport_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion
        #region handle value change
        private void cbboPrice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbboPrice.EditValue != null)
                {
                    int price = Convert.ToInt32(cbboPrice.EditValue);
                    this.filter.FeeType = priceOptions.Where(s => s.ID == price).Select(o => o.Value).FirstOrDefault();
                    if(!this.firtLoad)FillDataToControl();
                    
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }



        private void txtSearchValue_KeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        private void listReport_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                Panel panel = sender as Panel;

                if (panel != null)
                {
                    if (panel.VerticalScroll.Value + panel.ClientSize.Height >= panel.VerticalScroll.Maximum)
                    {
                        
                        LoadMoreData();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}

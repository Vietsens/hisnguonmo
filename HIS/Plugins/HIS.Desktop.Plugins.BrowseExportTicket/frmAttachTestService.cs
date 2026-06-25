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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.BrowseExportTicket.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BrowseExportTicket
{
    public partial class frmAttachTestService : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private long treatmentId;
        private List<long> preCheckedSereServIds;
        private List<AttachTestServiceADO> allTests = new List<AttachTestServiceADO>();

        /// <summary>Trạng thái checkbox "chọn tất cả" ở header cột Chọn.</summary>
        private bool isCheckedAll = false;

        /// <summary>
        /// Danh sách dịch vụ xét nghiệm người dùng đã tích chọn, đọc bởi form cha sau khi DialogResult = OK.
        /// </summary>
        public List<AttachTestServiceADO> SelectedTestServices { get; private set; }
        #endregion

        #region Constructor
        public frmAttachTestService(long treatmentId, List<long> preCheckedSereServIds)
        {
            InitializeComponent();
            this.treatmentId = treatmentId;
            this.preCheckedSereServIds = preCheckedSereServIds ?? new List<long>();
            this.SelectedTestServices = new List<AttachTestServiceADO>();
        }
        #endregion

        #region Load
        private void frmAttachTestService_Load(object sender, EventArgs e)
        {
            try
            {
                this.KeyPreview = true;
                SetIcon();
                SetCaptionByLanguageKey();
                LoadData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BrowseExportTicket.Resources.Lang", typeof(frmAttachTestService).Assembly);

                this.Text = GetLang("frmAttachTestService.Text", this.Text);
                this.btnChon.Text = GetLang("frmAttachTestService.btnChon.Text", this.btnChon.Text);
                this.btnSearch.Text = GetLang("frmAttachTestService.btnSearch.Text", this.btnSearch.Text);
                this.txtBarcode.Properties.NullValuePrompt = GetLang("frmAttachTestService.txtBarcode.NullValuePrompt", this.txtBarcode.Properties.NullValuePrompt);
                this.txtKeyword.Properties.NullValuePrompt = GetLang("frmAttachTestService.txtKeyword.NullValuePrompt", this.txtKeyword.Properties.NullValuePrompt);
                this.gcStt.Caption = GetLang("frmAttachTestService.gcStt.Caption", this.gcStt.Caption);
                this.gcCheck.Caption = GetLang("frmAttachTestService.gcCheck.Caption", this.gcCheck.Caption);
                this.gcServiceReqCode.Caption = GetLang("frmAttachTestService.gcServiceReqCode.Caption", this.gcServiceReqCode.Caption);
                this.gcBarcode.Caption = GetLang("frmAttachTestService.gcBarcode.Caption", this.gcBarcode.Caption);
                this.gcServiceCode.Caption = GetLang("frmAttachTestService.gcServiceCode.Caption", this.gcServiceCode.Caption);
                this.gcServiceName.Caption = GetLang("frmAttachTestService.gcServiceName.Caption", this.gcServiceName.Caption);
                this.gcAmount.Caption = GetLang("frmAttachTestService.gcAmount.Caption", this.gcAmount.Caption);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLang(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }
        #endregion

        #region LoadData
        private void LoadData()
        {
            try
            {
                WaitingManager.Show();
                allTests = new List<AttachTestServiceADO>();

                HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                serviceReqFilter.TREATMENT_ID = this.treatmentId;
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN };
                var serviceReqs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, serviceReqFilter, null);

                if (serviceReqs == null || serviceReqs.Count == 0)
                {
                    BindGrid(allTests);
                    WaitingManager.Hide();
                    return;
                }

                serviceReqs = serviceReqs.Where(o => o.PARENT_ID == null).ToList();
                if (serviceReqs.Count == 0)
                {
                    BindGrid(allTests);
                    WaitingManager.Hide();
                    return;
                }

                var barcodeByServiceReqId = new Dictionary<long, string>();
                foreach (var sr in serviceReqs)
                {
                    barcodeByServiceReqId[sr.ID] = sr.BARCODE;
                }

                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.SERVICE_REQ_IDs = serviceReqs.Select(o => o.ID).ToList();
                var sereServs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, null);

                if (sereServs != null && sereServs.Count > 0)
                {
                    foreach (var ss in sereServs)
                    {
                        AttachTestServiceADO ado = new AttachTestServiceADO();
                        ado.SERE_SERV_ID = ss.ID;
                        ado.SERVICE_ID = ss.SERVICE_ID;
                        ado.SERVICE_REQ_ID = ss.SERVICE_REQ_ID;
                        ado.TDL_SERVICE_REQ_CODE = ss.TDL_SERVICE_REQ_CODE;
                        ado.TDL_SERVICE_CODE = ss.TDL_SERVICE_CODE;
                        ado.TDL_SERVICE_NAME = ss.TDL_SERVICE_NAME;
                        ado.AMOUNT = ss.AMOUNT;
                        if (ss.SERVICE_REQ_ID.HasValue && barcodeByServiceReqId.ContainsKey(ss.SERVICE_REQ_ID.Value))
                        {
                            ado.BARCODE = barcodeByServiceReqId[ss.SERVICE_REQ_ID.Value];
                        }
                        ado.IsCheck = this.preCheckedSereServIds.Contains(ss.ID);
                        allTests.Add(ado);
                    }
                }

                BindGrid(allTests);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BindGrid(List<AttachTestServiceADO> data)
        {
            try
            {
                gridViewTest.BeginUpdate();
                gridControlTest.DataSource = data;
                gridViewTest.EndUpdate();
                RecomputeHeaderCheckState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTest_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.ListSourceRowIndex >= 0 && e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Search
        private void Search()
        {
            try
            {
                gridViewTest.PostEditor();
                List<AttachTestServiceADO> data = allTests;

                string barcode = txtBarcode.Text.Trim();
                if (!string.IsNullOrEmpty(barcode))
                {
                    data = data.Where(o => o.BARCODE == barcode).ToList();
                }

                string keyword = txtKeyword.Text.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    string kw = keyword.ToLower();
                    data = data.Where(o => (o.TDL_SERVICE_CODE ?? "").ToLower().Contains(kw)
                        || (o.TDL_SERVICE_NAME ?? "").ToLower().Contains(kw)
                        || (o.TDL_SERVICE_REQ_CODE ?? "").ToLower().Contains(kw)).ToList();
                }

                BindGrid(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Search();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Search();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region CheckAll
        /// <summary>
        /// Vẽ checkbox "chọn tất cả" ở header cột Chọn.
        /// </summary>
        private void gridViewTest_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column == gcCheck)
                {
                    e.Info.Caption = string.Empty;
                    e.Info.InnerElements.Clear();
                    e.Painter.DrawObject(e.Info);
                    DrawHeaderCheckBox(e.Cache, GetHeaderCheckBoxBounds(e.Bounds), isCheckedAll);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private System.Drawing.Rectangle GetHeaderCheckBoxBounds(System.Drawing.Rectangle headerBounds)
        {
            int size = 16;
            int x = headerBounds.X + (headerBounds.Width - size) / 2;
            int y = headerBounds.Y + (headerBounds.Height - size) / 2;
            return new System.Drawing.Rectangle(x, y, size, size);
        }

        private void DrawHeaderCheckBox(DevExpress.Utils.Drawing.GraphicsCache cache, System.Drawing.Rectangle bounds, bool isChecked)
        {
            try
            {
                DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info = repositoryItemCheckEdit1.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
                DevExpress.XtraEditors.Drawing.CheckEditPainter painter = repositoryItemCheckEdit1.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
                if (info == null || painter == null)
                {
                    return;
                }
                info.EditValue = isChecked;
                info.Bounds = bounds;
                info.CalcViewInfo(null);
                DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, bounds);
                painter.Draw(args);
                args.Cache = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Click vào header cột Chọn → đảo trạng thái chọn tất cả các dòng đang hiển thị.
        /// </summary>
        private void gridControlTest_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left)
                {
                    return;
                }
                DevExpress.XtraGrid.Views.Grid.GridView view = gridControlTest.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null)
                {
                    return;
                }
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = view.CalcHitInfo(new System.Drawing.Point(e.X, e.Y));
                if (hit.InColumn && hit.Column == gcCheck)
                {
                    isCheckedAll = !isCheckedAll;
                    SetCheckAll(isCheckedAll);
                    gridControlTest.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đồng bộ trạng thái header khi người dùng tích/bỏ tích từng dòng.
        /// </summary>
        private void gridViewTest_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column == gcCheck)
                {
                    RecomputeHeaderCheckState();
                    gridControlTest.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đặt IsCheck cho toàn bộ dòng đang hiển thị (theo bộ lọc tìm kiếm hiện tại).
        /// </summary>
        private void SetCheckAll(bool isChecked)
        {
            try
            {
                gridViewTest.PostEditor();
                List<AttachTestServiceADO> data = gridControlTest.DataSource as List<AttachTestServiceADO>;
                if (data == null || data.Count == 0)
                {
                    return;
                }
                foreach (var item in data)
                {
                    item.IsCheck = isChecked;
                }
                gridViewTest.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RecomputeHeaderCheckState()
        {
            try
            {
                List<AttachTestServiceADO> data = gridControlTest.DataSource as List<AttachTestServiceADO>;
                isCheckedAll = data != null && data.Count > 0 && data.All(o => o.IsCheck);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Choose
        private void btnChon_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewTest.PostEditor();
                gridViewTest.UpdateCurrentRow();
                this.SelectedTestServices = allTests.Where(o => o.IsCheck).ToList();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.S))
                {
                    btnChon_Click(this.btnChon, EventArgs.Empty);
                    return true;
                }
                if (keyData == (Keys.Control | Keys.F))
                {
                    txtBarcode.Focus();
                    txtBarcode.SelectAll();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
    }
}

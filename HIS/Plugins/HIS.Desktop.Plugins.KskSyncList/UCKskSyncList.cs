/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.KskSyncList.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.KskSyncList
{
    public partial class UCKskSyncList : UserControlBase
    {
        #region Declare
        // Khoa cau hinh ket noi cong QD1551 theo vien (muc 3.3 PTTK_44350)
        private const string CONFIG_KEY__CONNECTION_INFO = "MOS.HIS_KSK_SYNC.CONNECTION_INFO";

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }

        private bool isNotLoadWhileChangeControlStateInFirst;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        SettingSignADO SettingSignADO { get; set; }
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        #endregion

        #region Constructor / Load
        public UCKskSyncList(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
        }

        private void UCKskSyncList_Load(object sender, EventArgs e)
        {
            try
            {
                timer.Interval = 100;
                timer.Tick += Timer_Tick;
                SetCaptionByLanguageKey();
                InitComboKskType();
                SetDefaultControl();
                InitControlState();
                FillDataToGrid();
                LoadDicRefresh();
                txtKeyWord.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private Method
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.KskSyncList.Resources.Lang", typeof(HIS.Desktop.Plugins.KskSyncList.UCKskSyncList).Assembly);
                // Cac caption mac dinh da dat trong Designer (tieng Viet). Neu co ban dich
                // trong Lang.{culture}.resx thi override; khong co thi giu nguyen mac dinh.
                // Caption cua o loc nam tren LayoutControlItem (khong con LabelControl rieng).
                SetLayoutText(lciKskType, "lblKskType.Text");
                SetLayoutText(lciConclusionFrom, "lblConclusionFrom.Text");
                SetLayoutText(lciConclusionTo, "lblConclusionTo.Text");
                SetLayoutText(lciSyncStatus, "lblSyncStatus.Text");
                SetText(btnSearch, "btnSearch.Text");
                SetText(btnRefresh, "btnRefresh.Text");
                SetText(btnPreview, "btnPreview.Text");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetText(Control ctrl, string key)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                if (!string.IsNullOrEmpty(value)) ctrl.Text = value;
            }
            catch { }
        }

        private void SetLayoutText(DevExpress.XtraLayout.BaseLayoutItem item, string key)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                if (!string.IsNullOrEmpty(value)) item.Text = value;
            }
            catch { }
        }

        private void InitComboKskType()
        {
            try
            {
                cboKskType.Properties.Items.Clear();
                cboKskType.Properties.Items.Add(new KskTypeADO(0, "(Tất cả loại)"));
                foreach (var t in KskTypeADO.GetHisKskTypes())
                {
                    cboKskType.Properties.Items.Add(t);
                }
                cboKskType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                dtConclusionFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartMonth() ?? 0));
                dtConclusionTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                txtKeyWord.Text = "";
                txtPatientCode.Text = "";
                txtTreatmentCode.Text = "";
                if (cboKskType.Properties.Items.Count > 0) cboKskType.SelectedIndex = 0;
                cboSyncStatus.SelectedIndex = 0;
                btnSync.Enabled = false;
                UpdateSyncBadge(0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging.pagingGrid != null)
                    pageSize = ucPaging.pagingGrid.PageSize;
                else
                    pageSize = (int)ConfigApplications.NumPageSize;

                LoadGridData(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                HisKskSyncViewFilter filter = new HisKskSyncViewFilter();
                SetFilter(ref filter);
                gridView1.BeginUpdate();
                ApiResultObject<List<V_HIS_KSK_SYNC>> apiResult =
                    new BackendAdapter(paramCommon).GetRO<List<V_HIS_KSK_SYNC>>("api/HisKskSync/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    gridControl1.DataSource = (data != null && data.Count > 0) ? data : null;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();

                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckDigit(string s)
        {
            try
            {
                if (string.IsNullOrEmpty(s)) return false;
                return s.All(char.IsDigit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void SetFilter(ref HisKskSyncViewFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 10 && CheckDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = txtTreatmentCode.Text;
                }
                else if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && CheckDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = txtPatientCode.Text;
                }
                else
                {
                    filter.ORDER_FIELD = "CONCLUSION_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.KEY_WORD = txtKeyWord.Text.Trim();

                    KskTypeADO kskType = cboKskType.SelectedItem as KskTypeADO;
                    if (kskType != null && kskType.KSK_TYPE_ID > 0)
                        filter.KSK_TYPE_ID = kskType.KSK_TYPE_ID;

                    if (dtConclusionFrom.EditValue != null && dtConclusionFrom.DateTime != DateTime.MinValue)
                        filter.CONCLUSION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtConclusionFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtConclusionTo.EditValue != null && dtConclusionTo.DateTime != DateTime.MinValue)
                        filter.CONCLUSION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtConclusionTo.EditValue).ToString("yyyyMMdd") + "235959");

                    switch (cboSyncStatus.SelectedIndex)
                    {
                        case 1: filter.SYNC_RESULT_TYPE = 1; break; // Chua dong bo
                        case 2: filter.SYNC_RESULT_TYPE = 2; break; // Da dong bo
                        case 3: filter.SYNC_RESULT_TYPE = 3; break; // That bai
                        default: filter.SYNC_RESULT_TYPE = null; break; // Tat ca
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Trang thai dong bo (text + mau)
        private static int GetSyncType(V_HIS_KSK_SYNC row)
        {
            int t = 0;
            try { t = Convert.ToInt32(row.SYNC_RESULT_TYPE); }
            catch { t = 0; }
            if (t == 0) t = 1; // Chua tung day (LEFT JOIN null) => Chua dong bo
            return t;
        }

        private static string GetSyncTypeText(int t)
        {
            switch (t)
            {
                case 2: return "Đã đồng bộ";
                case 3: return "Thất bại";
                case 4: return "Có chỉnh sửa";
                default: return "Chưa đồng bộ";
            }
        }
        #endregion

        #region Grid events
        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!(e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)) return;
                V_HIS_KSK_SYNC data = (V_HIS_KSK_SYNC)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data == null) return;

                if (e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1 + (ucPaging.pagingGrid.CurrentPage - 1) * (ucPaging.pagingGrid.PageSize);
                }
                else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                {
                    try
                    {
                        string dobStr = data.TDL_PATIENT_DOB.ToString();
                        if (GetPropInt(data, "TDL_PATIENT_IS_HAS_NOT_DAY_DOB") == 1)
                            e.Value = dobStr.Length >= 4 ? dobStr.Substring(0, 4) : dobStr;
                        else
                            e.Value = FormatTimeNumber(data.TDL_PATIENT_DOB);
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                }
                else if (e.Column.FieldName == "CONCLUSION_TIME_STR")
                {
                    e.Value = FormatTimeNumber(data.CONCLUSION_TIME);
                }
                else if (e.Column.FieldName == "SYNC_RESULT_TYPE_STR")
                {
                    e.Value = GetSyncTypeText(GetSyncType(data));
                }
                else if (e.Column.FieldName == "SYNC_TIME_STR")
                {
                    e.Value = FormatTimeNumber(data.SYNC_TIME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Dinh dang so thoi gian (yyyyMMddHHmmss / yyyyMMdd) -> chuoi ngay, doc lap
        // kieu cot backend (long? / decimal? / int?). Null hoac 0 -> rong.
        private static string FormatTimeNumber(object o)
        {
            try
            {
                long v = (o == null) ? 0 : Convert.ToInt64(o);
                if (v <= 0) return "";
                return Inventec.Common.DateTime.Convert.TimeNumberToDateString(v);
            }
            catch { return ""; }
        }

        private static int GetPropInt(object obj, string name)
        {
            try
            {
                var p = obj.GetType().GetProperty(name);
                if (p == null) return 0;
                var v = p.GetValue(obj, null);
                return v == null ? 0 : Convert.ToInt32(v);
            }
            catch { return 0; }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column != colSyncStatus) return;
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_KSK_SYNC data = (V_HIS_KSK_SYNC)view.GetRow(e.RowHandle);
                if (data == null) return;
                int t = GetSyncType(data);
                e.Appearance.Options.UseForeColor = true;
                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                if (t == 2)
                    e.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 150, 60);    // xanh la - Da dong bo
                else if (t == 3)
                    e.Appearance.ForeColor = System.Drawing.Color.FromArgb(210, 40, 40);   // do - That bai
                else
                    e.Appearance.ForeColor = System.Drawing.Color.FromArgb(220, 140, 0);   // cam - Chua dong bo
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int count = gridView1.GetSelectedRows().Count(rh => rh >= 0);
                btnSync.Enabled = count > 0;
                UpdateSyncBadge(count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateSyncBadge(int count)
        {
            try { btnSync.Text = "Đồng bộ lên cổng  (" + count + ")"; }
            catch { }
        }
        #endregion

        #region Buttons / keys
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try { FillDataToGrid(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try { SetDefaultControl(); FillDataToGrid(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void txt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try { btnSearch_Click(null, null); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try { btnRefresh_Click(null, null); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
        #endregion

        #region Ky so (control state + frmSetting)
        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.currentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkSign.Name)
                        {
                            SettingSignADO = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingSignADO>(item.VALUE);
                            chkSign.Checked = SettingSignADO != null && !string.IsNullOrEmpty(SettingSignADO.SerialNumber);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                chkSign.Checked = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst) return;
                timer.Start();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                if (chkSign.Checked)
                {
                    frmSetting frm = new frmSetting(SettingSignADO, (result) => { SettingSignADO = (SettingSignADO)result; });
                    frm.ShowDialog();
                    if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                        chkSign.Checked = false;
                }
                SaveControlStateSign();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void SaveControlStateSign()
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO cs = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                    ? this.currentControlStateRDO.Where(o => o.KEY == chkSign.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault()
                    : null;
                if (cs != null)
                {
                    cs.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                }
                else
                {
                    cs = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    cs.KEY = chkSign.Name;
                    cs.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                    cs.MODULE_LINK = this.currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(cs);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
        #endregion

        #region Xem du lieu se day (Scene 3)
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                V_HIS_KSK_SYNC row = GetFirstChosenRow();
                if (row == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn (hoặc bấm vào) một hồ sơ để xem trước dữ liệu sẽ đẩy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ShowPreview(row);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void ShowPreview(V_HIS_KSK_SYNC row)
        {
            try
            {
                KskSyncProcessor processor = new KskSyncProcessor(GetConnectionInfo(), chkSign.Checked, SettingSignADO);
                string content = processor.BuildPreview(row);
                Preview.frmKskSyncPreview frm = new Preview.frmKskSyncPreview(row, content);
                frm.ShowDialog();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private V_HIS_KSK_SYNC GetFirstChosenRow()
        {
            var selected = gridView1.GetSelectedRows().Where(rh => rh >= 0).ToList();
            if (selected.Count > 0)
                return (V_HIS_KSK_SYNC)gridView1.GetRow(selected[0]);
            var focused = gridView1.GetFocusedRow() as V_HIS_KSK_SYNC;
            return focused;
        }
        #endregion

        #region Dong bo (Scene 2 -> 4 -> 5)
        private string GetConnectionInfo()
        {
            try
            {
                var cfg = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY == CONFIG_KEY__CONNECTION_INFO).FirstOrDefault();
                return cfg != null ? cfg.VALUE : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>BR7 - an toan da vien: chua cau hinh ket noi cong => khong day (Scene 5).</summary>
        private bool VerifyConnectionConfigured()
        {
            if (!string.IsNullOrWhiteSpace(GetConnectionInfo())) return true;
            XtraMessageBox.Show(
                "Chưa có cấu hình kết nối Cổng dữ liệu Y tế cho cơ sở này." + Environment.NewLine +
                "Vui lòng liên hệ bộ phận quản trị để khai báo thông tin kết nối (tài khoản, địa chỉ cổng, chứng thư số) trước khi đẩy dữ liệu.",
                "Không thể đồng bộ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                var rows = gridView1.GetSelectedRows().Where(rh => rh >= 0)
                    .Select(rh => (V_HIS_KSK_SYNC)gridView1.GetRow(rh)).Where(r => r != null).ToList();
                if (rows.Count == 0) return;
                SyncRecords(rows);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void repositoryItemButtonEdit_PUSH_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = gridView1.GetFocusedRow() as V_HIS_KSK_SYNC;
                if (row == null) return;
                SyncRecords(new List<V_HIS_KSK_SYNC>() { row });
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void SyncRecords(List<V_HIS_KSK_SYNC> rows)
        {
            try
            {
                // Scene 5: chua cau hinh ket noi cong -> bao loi, khong day
                if (!VerifyConnectionConfigured()) return;

                // Ky so duoc bat nhung chua chon chung thu
                if (chkSign.Checked && (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber)))
                {
                    XtraMessageBox.Show("Bạn đã bật Ký số nhưng chưa chọn chứng thư số. Vui lòng chọn chứng thư trước khi đẩy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                WaitingManager.Show();
                long syncTime = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));

                // Build + ky so + goi cong QD1551 (thu vien BD_046 - muc 3.4)
                KskSyncProcessor processor = new KskSyncProcessor(GetConnectionInfo(), chkSign.Checked, SettingSignADO);
                List<KskSyncResultADO> results = processor.PushList(rows, syncTime);

                // Luu trang thai day vao HIS_KSK_SYNC (muc 3.2.2)
                SaveSyncResult(results);

                WaitingManager.Hide();

                // Scene 4: hop thoai tong hop ket qua day lo
                SyncResult.frmKskSyncResult frm = new SyncResult.frmKskSyncResult(results);
                frm.ShowDialog();

                FillDataToGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveSyncResult(List<KskSyncResultADO> results)
        {
            try
            {
                if (results == null || results.Count == 0) return;
                CommonParam param = new CommonParam();
                new BackendAdapter(param).Post<int>("api/HisKskSync/SaveSyncResult", ApiConsumers.MosConsumer, results, SessionManager.ActionLostToken, param);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Refresh dictionary
        private void LoadDicRefresh()
        {
            try
            {
                if (GlobalVariables.DicRefreshData == null)
                    GlobalVariables.DicRefreshData = new Dictionary<string, RefeshReference>();
                if (currentModule != null && !GlobalVariables.DicRefreshData.ContainsKey(currentModule.RoomId.ToString()))
                    GlobalVariables.DicRefreshData.Add(currentModule.RoomId.ToString(), (HIS.Desktop.Common.RefeshReference)FillDataToGrid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}

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
using HIS.Desktop.ADO;
using HIS.Desktop.Plugins.KskSyncList.ADO;
using HIS.UC.SettingSignInfo;
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
        // Khoa cau hinh ket noi cong QD2062 theo vien (muc 3.3 PTTK_44350)
        private const string CONFIG_KEY__CONNECTION_INFO = "MOS.HIS_KSK_SYNC.CONNECTION_INFO";

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }

        private bool isNotLoadWhileChangeControlStateInFirst;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        // Cau hinh cong lien thong (btnSettings). Luu local qua ControlState key = btnSettings.Name.
        private KskSyncTargetADO syncTarget = new KskSyncTargetADO();
        private bool bytConfigAvailable;   // MOS.HIS_KSK_SYNC.CONNECTION_INFO co du lieu
        private bool hsskConfigAvailable;  // MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO co du lieu
        private bool hocConfigAvailable;   // MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO co du lieu
        private bool hccConfigAvailable;   // MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO co du lieu
        private bool vlgConfigAvailable;   // MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO co du lieu
        private bool sytConfigAvailable;   // MOS.HIS_KSK_SYNC.SYT_HCM_CONNECTION_INFO co du lieu
        private bool hasSavedSyncState;    // da co trang thai check luu truoc do
        // Trang thai luu co chua truong SyncVlg chua? JSON luu tu BAN CU (truoc khi co cong VLG) khong co
        // truong nay -> deserialize ra false "gia" -> phai auto-tick theo config MOT LAN khi vien vua khai
        // key VLG (khong thi vien Vinh Long deploy xong bam Dong bo ngay se day THIEU cong tinh ma van
        // bao thanh cong). User tick tay lan dau la JSON moi co truong nay -> ton trong lua chon user.
        private bool vlgStateSaved;

        /// <summary>Trạng thái đã lưu CÓ trường SyncSytHcm chưa — để biết khi nào cần tự tích một lần.</summary>
        private bool sytStateSaved;
        private string exportXmlPath = ""; // duong dan xuat XML (luu local qua ControlState theo key btnExportPath)
        // Khai bao noi chi so can lam sang voi chi tieu mau M3 (cong SYT TP.HCM) — luu local qua ControlState.
        private string sytClsMapJson = "";
        private const string CONTROL_STATE_KEY__SYT_CLS_MAP = "KskSytClsMap";
        private const string CONFIG_KEY__HSSK_HN_2062_CONNECTION_INFO = "MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO";
        private const string CONFIG_KEY__HSSK_HOC_2062_CONNECTION_INFO = "MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO";
        private const string CONFIG_KEY__HSSK_HCC_2062_CONNECTION_INFO = "MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO";
        private const string CONFIG_KEY__VLG_2062_CONNECTION_INFO = "MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO";
        private const string CONFIG_KEY__SYT_HCM_CONNECTION_INFO = "MOS.HIS_KSK_SYNC.SYT_HCM_CONNECTION_INFO";
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
                timer.Tick -= Timer_Tick;   // tranh dang ky trung neu UC Load lai
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
                // Reload lam mat lua chon cu -> tat nut Dong bo cho den khi chon lai (tranh enable ao sau khi Tim).
                btnSync.Enabled = false;
                UpdateSyncBadge(0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        // ===== TEST FAKE: 4 dong luoi gia — 3001/3002 (>=18, co/khong CKDT_), 3003 (tre <6), 3004 (nguoi <18). =====
        private static List<V_HIS_KSK_SYNC> BuildFakeGridRows()
        {
            return new List<V_HIS_KSK_SYNC>
            {
                MakeFakeRow(KskSyncProcessor.FAKE_SR_HAS_CKDT, "000000003001", "TEST0001", "NGUYỄN VĂN CÓ CKDT", KskSyncProcessor.FAKE_CONCLUDER_LOGINNAME, "KSK trên 18 tuổi", 19900101000000L),
                MakeFakeRow(KskSyncProcessor.FAKE_SR_NO_CKDT,  "000000003002", "TEST0002", "TRẦN THỊ KHÔNG CKDT", KskSyncProcessor.FAKE_CONCLUDER_LOGINNAME, "KSK trên 18 tuổi", 19900101000000L),
                MakeFakeRow(KskSyncProcessor.FAKE_SR_UNDER6,   "000000003003", "TEST0003", "LÊ BẢO AN (TRẺ <6)", KskSyncProcessor.FAKE_CONCLUDER_LOGINNAME, "KSK trẻ dưới 6 tuổi", 20220615000000L),
                MakeFakeRow(KskSyncProcessor.FAKE_SR_UNDER18,  "000000003004", "TEST0004", "PHẠM GIA HÂN (<18)", KskSyncProcessor.FAKE_CONCLUDER_LOGINNAME, "KSK người dưới 18 tuổi", 20120310000000L)
            };
        }

        private static V_HIS_KSK_SYNC MakeFakeRow(long sr, string treCode, string patCode, string name, string concLogin, string kskTypeName, long dob)
        {
            var row = new V_HIS_KSK_SYNC();
            var map = new Dictionary<string, object>
            {
                { "SERVICE_REQ_ID", sr }, { "TDL_TREATMENT_ID", sr }, { "TDL_TREATMENT_CODE", treCode },
                { "TDL_PATIENT_CODE", patCode }, { "TDL_PATIENT_NAME", name }, { "TDL_PATIENT_DOB", dob },
                { "KSK_TYPE_NAME", kskTypeName }, { "CONCLUSION", "Đủ sức khỏe" },
                { "CONCLUDER_USERNAME", "Bác sĩ Fake" }, { "CONCLUDER_LOGINNAME", concLogin },
                { "CONCLUSION_TIME", 20260101080000L }, { "EXECUTE_ROOM_NAME", "Phòng khám Fake" },
                { "SYNC_RESULT_TYPE", (short)1 }
            };
            foreach (var kv in map) SetPropSafe(row, kv.Key, kv.Value);
            return row;
        }

        /// <summary>Set property qua reflection (an toan, tu convert kieu). Prop khong ton tai/read-only -> bo qua.</summary>
        private static void SetPropSafe(object obj, string prop, object value)
        {
            try
            {
                if (obj == null) return;
                var p = obj.GetType().GetProperty(prop);
                if (p == null || !p.CanWrite) return;
                object v = value;
                if (value != null)
                {
                    var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    if (!t.IsInstanceOfType(value)) v = Convert.ChangeType(value, t);
                }
                p.SetValue(obj, v, null);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
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
                List<V_HIS_KSK_SYNC> data = (apiResult != null) ? apiResult.Data : null;
                if (KskSyncProcessor.USE_FAKE_DATA) data = BuildFakeGridRows();   // TEST: nap 2 dong fake (1 co CKDT_, 1 khong)
                if (data != null && data.Count > 0)
                {
                    gridControl1.DataSource = data;
                    rowCount = data.Count;
                    dataTotal = KskSyncProcessor.USE_FAKE_DATA ? data.Count : (apiResult != null && apiResult.Param != null ? apiResult.Param.Count ?? 0 : 0);
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
                btnSync.Enabled = count > 0 && CanSync();
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
                            chkSign.Checked = IsSignSettingValid(SettingSignADO);
                        }
                        else if (item.KEY == btnSettings.Name)
                        {
                            var t = Newtonsoft.Json.JsonConvert.DeserializeObject<KskSyncTargetADO>(item.VALUE);
                            if (t != null)
                            {
                                syncTarget = t;
                                hasSavedSyncState = true;
                                // JSON ban cu khong co truong SyncVlg -> chua co lua chon user cho cong VLG.
                                vlgStateSaved = HasJsonField(item.VALUE, "SyncVlg");
                                sytStateSaved = HasJsonField(item.VALUE, "SyncSytHcm");
                            }
                        }
                        else if (item.KEY == btnExportPath.Name)
                        {
                            exportXmlPath = item.VALUE ?? "";
                        }
                        else if (item.KEY == CONTROL_STATE_KEY__SYT_CLS_MAP)
                        {
                            sytClsMapJson = item.VALUE ?? "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                chkSign.Checked = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            LoadSyncTargetAvailability();
            // AN TOAN DA VIEN: nut "Noi chi so CLS" chi phuc vu cong SYT TP.HCM nen chi hien voi
            // vien da khai bao cau hinh cong do; vien khac khong thay nut nay.
            // KHÔNG dời nút bằng mã. Vị trí các nút đã đặt sẵn trong tệp bố cục (Designer): nút
            // "Nối chỉ số" ở cuối hàng 1, nút "Đồng bộ lên cổng" ở cuối hàng 2. Gọi Move lúc chạy sẽ
            // sắp lại cây bố cục và kéo theo hai hậu quả đã gặp: nút Đồng bộ bị đẩy lên hàng 1 tách
            // khỏi nút Xuất XML, và phần lưới bên dưới không còn giãn hết chiều cao.
            lciBtnClsMap.Visibility = sytConfigAvailable
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        // Cau hinh ky so hop le (giu tich checkbox). Form ky so chi tra ve ado (non-null) khi bam Luu.
        // - USB token (nhu cac chuc nang khac): bat buoc co SerialNumber (chung thu da chon).
        // - HSM (mac dinh cua form, tai khoan QD2062 sandbox): khong dung SerialNumber
        //   (dung he thong/ma ky/secret key) -> da luu cau hinh HSM la hop le.
        private static bool IsSignSettingValid(SettingSignADO ado)
        {
            if (ado == null) return false;
            if (ado.IsHsm) return true;
            return !string.IsNullOrEmpty(ado.SerialNumber);
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
                    if (!IsSignSettingValid(SettingSignADO))
                        chkSign.Checked = false;
                }
                else
                {
                    SettingSignADO = null;
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

        #region Cai dat cong lien thong (btnSettings -> PopupControlContainer)
        private DevExpress.XtraBars.PopupControlContainer popupSync;
        private DevExpress.XtraEditors.CheckEdit chkSyncByt;
        private DevExpress.XtraEditors.CheckEdit chkSyncHssk;
        private DevExpress.XtraEditors.CheckEdit chkSyncHoc;
        private DevExpress.XtraEditors.CheckEdit chkSyncHcc;
        private DevExpress.XtraEditors.CheckEdit chkSyncVlg;
        private DevExpress.XtraEditors.CheckEdit chkSyncSytHcm;

        /// <summary>Dựng PopupControlContainer (như nút cài đặt in AssignService) chứa các checkbox chọn cổng liên thông.</summary>
        private void EnsureSyncPopup()
        {
            if (popupSync != null) return;

            chkSyncByt = new DevExpress.XtraEditors.CheckEdit();
            chkSyncByt.Properties.Caption = "Liên thông KSK BYT (2062/QĐ-BYT)";
            chkSyncByt.Checked = syncTarget != null && syncTarget.SyncByt;

            chkSyncHssk = new DevExpress.XtraEditors.CheckEdit();
            chkSyncHssk.Properties.Caption = "Liên thông HSSK (2062/QĐ-BYT)";
            chkSyncHssk.Checked = syncTarget != null && syncTarget.SyncHssk;

            chkSyncHoc = new DevExpress.XtraEditors.CheckEdit();
            chkSyncHoc.Properties.Caption = "Liên thông HOC";
            chkSyncHoc.Checked = syncTarget != null && syncTarget.SyncHoc;

            chkSyncHcc = new DevExpress.XtraEditors.CheckEdit();
            chkSyncHcc.Properties.Caption = "Liên thông HCC (2062/QĐ-BYT)";
            chkSyncHcc.Checked = syncTarget != null && syncTarget.SyncHcc;
            chkSyncSytHcm = new DevExpress.XtraEditors.CheckEdit();
            chkSyncSytHcm.Properties.Caption = "Liên thông KSK Sở Y tế TP.HCM (mẫu M3)";
            chkSyncSytHcm.Checked = syncTarget != null && syncTarget.SyncSytHcm;

            chkSyncVlg = new DevExpress.XtraEditors.CheckEdit();
            chkSyncVlg.Properties.Caption = "Liên thông KDLYT Vĩnh Long (2062/QĐ-BYT)";
            chkSyncVlg.Checked = syncTarget != null && syncTarget.SyncVlg;

            // Bố cục chuẩn bằng LayoutControl: mỗi checkbox = 1 LayoutControlItem (ẩn text item, dùng caption checkbox).
            DevExpress.XtraLayout.LayoutControl lc = new DevExpress.XtraLayout.LayoutControl();
            lc.Dock = System.Windows.Forms.DockStyle.Fill;
            lc.BeginUpdate();
            DevExpress.XtraLayout.LayoutControlItem lciByt = (DevExpress.XtraLayout.LayoutControlItem)lc.Root.AddItem();
            lciByt.Control = chkSyncByt;
            lciByt.TextVisible = false;
            // Chỉ hiển thị checkbox khi config tương ứng có dữ liệu.
            lciByt.Visibility = bytConfigAvailable
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            DevExpress.XtraLayout.LayoutControlItem lciHssk = (DevExpress.XtraLayout.LayoutControlItem)lc.Root.AddItem();
            lciHssk.Control = chkSyncHssk;
            lciHssk.TextVisible = false;
            lciHssk.Visibility = hsskConfigAvailable
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            DevExpress.XtraLayout.LayoutControlItem lciHoc = (DevExpress.XtraLayout.LayoutControlItem)lc.Root.AddItem();
            lciHoc.Control = chkSyncHoc;
            lciHoc.TextVisible = false;
            lciHoc.Visibility = hocConfigAvailable
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            DevExpress.XtraLayout.LayoutControlItem lciHcc = (DevExpress.XtraLayout.LayoutControlItem)lc.Root.AddItem();
            lciHcc.Control = chkSyncHcc;
            lciHcc.TextVisible = false;
            lciHcc.Visibility = hccConfigAvailable
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            DevExpress.XtraLayout.LayoutControlItem lciVlg = (DevExpress.XtraLayout.LayoutControlItem)lc.Root.AddItem();
            lciVlg.Control = chkSyncVlg;
            lciVlg.TextVisible = false;
            lciVlg.Visibility = vlgConfigAvailable
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            // Cổng Sở Y tế TP.HCM — đặt SAU các cổng đang có để không đổi thứ tự các dòng cũ.
            DevExpress.XtraLayout.LayoutControlItem lciSyt =
                (DevExpress.XtraLayout.LayoutControlItem)lc.Root.AddItem();
            lciSyt.Control = chkSyncSytHcm;
            lciSyt.TextVisible = false;
            lciSyt.Visibility = sytConfigAvailable
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lc.Root.GroupBordersVisible = false;
            lc.EndUpdate();

            // Gắn handler SAU khi set Checked ban đầu để không kích hoạt lưu thừa.
            chkSyncByt.CheckedChanged += SyncTarget_CheckedChanged;
            chkSyncHssk.CheckedChanged += SyncTarget_CheckedChanged;
            chkSyncHoc.CheckedChanged += SyncTarget_CheckedChanged;
            chkSyncHcc.CheckedChanged += SyncTarget_CheckedChanged;
            chkSyncVlg.CheckedChanged += SyncTarget_CheckedChanged;
            chkSyncSytHcm.CheckedChanged += SyncTarget_CheckedChanged;

            // Chiều cao popup CO GIÃN theo số cổng THỰC SỰ hiển thị (config có dữ liệu) — mỗi item 1 dòng.
            int visibleCount = (bytConfigAvailable ? 1 : 0) + (hsskConfigAvailable ? 1 : 0)
                             + (hocConfigAvailable ? 1 : 0) + (hccConfigAvailable ? 1 : 0)
                             + (vlgConfigAvailable ? 1 : 0) + (sytConfigAvailable ? 1 : 0);
            if (visibleCount < 1) visibleCount = 1;
            const int ROW_HEIGHT = 30;   // chiều cao 1 dòng checkbox (đủ để không hiện scroll)
            const int PADDING = 14;      // padding trên/dưới của LayoutControl 
            int popupHeight = visibleCount * ROW_HEIGHT + PADDING;

            popupSync = new DevExpress.XtraBars.PopupControlContainer();
            popupSync.Name = "popupControlContainerSync";
            popupSync.Manager = this.barManager1;
            popupSync.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            popupSync.Size = new System.Drawing.Size(320, popupHeight);
            popupSync.Controls.Add(lc);
            popupSync.Visible = false;
            this.Controls.Add(popupSync);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (!bytConfigAvailable && !hsskConfigAvailable && !hocConfigAvailable && !hccConfigAvailable
                    && !vlgConfigAvailable && !sytConfigAvailable)
                {
                    XtraMessageBox.Show(
                        "Chưa cấu hình cổng liên thông khám sức khỏe cho cơ sở " + GetCurrentBranchName() + "." + Environment.NewLine +
                        "Vui lòng cấu hình MOS.HIS_KSK_SYNC.CONNECTION_INFO (Liên thông KSK BYT), " +
                        "MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO (Liên thông HSSK), " +
                        "MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO (Liên thông HOC), " +
                        "MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO (Liên thông HCC) " +
                        "hoặc MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO (Liên thông KDLYT Vĩnh Long).",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                EnsureSyncPopup();
                System.Drawing.Point p = btnSettings.PointToScreen(new System.Drawing.Point(0, btnSettings.Height + 2));
                popupSync.ShowPopup(p);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void SyncTarget_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (syncTarget == null) syncTarget = new KskSyncTargetADO();
                syncTarget.SyncByt = chkSyncByt.Checked;
                syncTarget.SyncHssk = chkSyncHssk.Checked;
                if (chkSyncHoc != null) syncTarget.SyncHoc = chkSyncHoc.Checked;
                if (chkSyncHcc != null) syncTarget.SyncHcc = chkSyncHcc.Checked;
                if (chkSyncVlg != null) syncTarget.SyncVlg = chkSyncVlg.Checked;
                if (chkSyncSytHcm != null) syncTarget.SyncSytHcm = chkSyncSytHcm.Checked;
                SaveControlStateTarget();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>
        /// Mở bảng khai báo nối chỉ số cận lâm sàng → chỉ tiêu mẫu M4. Khai báo lưu tại máy trạm.
        /// </summary>
        private void btnClsMap_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new HIS.Desktop.Plugins.KskSyncList.TestIndexMap.frmKskSytClsMap(
                    this.sytClsMapJson, (json) => SaveControlStateClsMap(json)))
                {
                    frm.ShowDialog();
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>
        /// Lưu khai báo nối chỉ số (JSON) qua ControlState, key = KskSytClsMap.
        /// BẮT BUỘC ghi bằng DANH SÁCH ĐẦY ĐỦ currentControlStateRDO: ControlStateWorker.SetData xóa
        /// mọi key của màn hình không nằm trong danh sách truyền vào (sẽ mất cấu hình ký số / chọn cổng).
        /// </summary>
        private void SaveControlStateClsMap(string json)
        {
            try
            {
                this.sytClsMapJson = json ?? "";

                HIS.Desktop.Library.CacheClient.ControlStateRDO cs = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                    ? this.currentControlStateRDO.Where(o => o.KEY == CONTROL_STATE_KEY__SYT_CLS_MAP && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault()
                    : null;
                if (cs != null)
                {
                    cs.VALUE = this.sytClsMapJson;
                }
                else
                {
                    cs = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    cs.KEY = CONTROL_STATE_KEY__SYT_CLS_MAP;
                    cs.VALUE = this.sytClsMapJson;
                    cs.MODULE_LINK = this.currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(cs);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Lưu trạng thái check cổng liên thông (JSON) qua ControlState, key = btnSettings.Name.</summary>
        private void SaveControlStateTarget()
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO cs = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                    ? this.currentControlStateRDO.Where(o => o.KEY == btnSettings.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault()
                    : null;
                string val = Newtonsoft.Json.JsonConvert.SerializeObject(syncTarget);
                if (cs != null)
                {
                    cs.VALUE = val;
                }
                else
                {
                    cs = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    cs.KEY = btnSettings.Name;
                    cs.VALUE = val;
                    cs.MODULE_LINK = this.currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(cs);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>JSON (chuỗi lưu ControlState) có chứa trường tên <paramref name="field"/> không.</summary>
        private static bool HasJsonField(string json, string field)
        {
            try
            {
                if (string.IsNullOrEmpty(json)) return false;
                var jo = Newtonsoft.Json.Linq.JObject.Parse(json);
                return jo[field] != null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return false; }
        }

        /// <summary>
        /// Đọc VALUE của 1 key HIS_CONFIG theo CHI NHÁNH đang làm việc (null nếu không có).
        /// Nhiều cơ sở chung 1 DB -> cùng key có nhiều bản ghi, phân biệt bằng BRANCH_ID.
        /// </summary>
        private string GetConfigValue(string key)
        {
            return KskBranchConfig.GetValue(key);
        }

        /// <summary>Tên chi nhánh (cơ sở) đang làm việc — dùng cho thông báo cấu hình thiếu.</summary>
        private string GetCurrentBranchName()
        {
            return KskBranchConfig.CurrentBranchName();
        }

        /// <summary>
        /// Lấy chuỗi cấu hình cổng HOC → TTYTQG từ HIS_CONFIG
        /// (khóa <c>MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO</c>). Null/rỗng nếu chưa cấu hình.
        /// </summary>
        private string GetHocConnectionInfo()
        {
            return GetConfigValue(CONFIG_KEY__HSSK_HOC_2062_CONNECTION_INFO);
        }

        /// <summary>
        /// Lấy chuỗi cấu hình cổng HCC (Health Care Center) từ HIS_CONFIG
        /// (khóa <c>MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO</c>). Null/rỗng nếu chưa cấu hình.
        /// Định dạng (các trường cách '|', cùng họ với cổng HOC — xem <see cref="KskHccConfigParser"/>):
        /// MaCsyt|Username|Password|ReceiverId|DataType|Version|TokenUrl|PushUrl|PrivateKey
        /// </summary>
        private string GetHccConnectionInfo()
        {
            return GetConfigValue(CONFIG_KEY__HSSK_HCC_2062_CONNECTION_INFO);
        }

        /// <summary>
        /// Lấy chuỗi cấu hình cổng KDLYT Vĩnh Long (Cổng tiếp nhận) từ HIS_CONFIG
        /// (khóa <c>MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO</c>). Null/rỗng nếu chưa cấu hình
        /// (viện không dùng cổng Vĩnh Long thì để trống khóa này — không ảnh hưởng viện khác).
        /// Định dạng (các trường cách '|' — xem <see cref="KskVlgConfigParser"/>):
        /// MaDonVi|Username|Password|TokenUrl|PushUrl
        /// </summary>
        private string GetVlgConnectionInfo()
        {
            return GetConfigValue(CONFIG_KEY__VLG_2062_CONNECTION_INFO);
        }

        /// <summary>
        /// Xác định cổng nào có cấu hình + auto-tích. ƯU TIÊN trạng thái đã lưu (hasSavedSyncState):
        /// chỉ auto-tích theo config khi CHƯA có trạng thái lưu trước đó. Config rỗng -> bỏ tích.
        /// </summary>
        private void LoadSyncTargetAvailability()
        {
            try
            {
                bytConfigAvailable = !string.IsNullOrWhiteSpace(GetConfigValue(CONFIG_KEY__CONNECTION_INFO));
                hsskConfigAvailable = !string.IsNullOrWhiteSpace(GetConfigValue(CONFIG_KEY__HSSK_HN_2062_CONNECTION_INFO));
                hocConfigAvailable = !string.IsNullOrWhiteSpace(GetHocConnectionInfo());
                hccConfigAvailable = !string.IsNullOrWhiteSpace(GetHccConnectionInfo());
                vlgConfigAvailable = !string.IsNullOrWhiteSpace(GetVlgConnectionInfo());
                sytConfigAvailable = !string.IsNullOrWhiteSpace(GetConfigValue(CONFIG_KEY__SYT_HCM_CONNECTION_INFO));

                // Tom tat cong nao co cau hinh THEO CHI NHANH dang lam viec — soi nhanh khi vien bao
                // "da khai cau hinh ma khong tich duoc cong". Chi tiet tung key: marker "KskBranchConfig:".
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "KskSyncList: chi nhanh dang lam viec={0} ({1}) | co cau hinh: BYT={2}, HSSK={3},"
                    + " HOC={4}, HCC={5}, VLG={6}, SYT_HCM={7}",
                    KskBranchConfig.CurrentBranchId(), KskBranchConfig.CurrentBranchName(),
                    bytConfigAvailable, hsskConfigAvailable, hocConfigAvailable,
                    hccConfigAvailable, vlgConfigAvailable, sytConfigAvailable));

                if (syncTarget == null) syncTarget = new KskSyncTargetADO();
                if (!hasSavedSyncState)
                {
                    syncTarget.SyncByt = bytConfigAvailable;
                    syncTarget.SyncHssk = hsskConfigAvailable;
                    syncTarget.SyncHoc = hocConfigAvailable;
                    syncTarget.SyncHcc = hccConfigAvailable;
                    syncTarget.SyncVlg = vlgConfigAvailable;
                    syncTarget.SyncSytHcm = sytConfigAvailable;
                }
                else
                {
                    // Trang thai luu tu BAN CU (chua co truong tuong ung) -> tu tich theo cau hinh MOT
                    // LAN nhu lan dau, tranh vien nang cap xong day thieu cong.
                    // Hai cong xet RIENG: vien co the da luu trang thai co SyncVlg nhung chua co
                    // SyncSytHcm, gop dieu kien thi cong con lai khong bao gio duoc tu tich.
                    if (!vlgStateSaved) syncTarget.SyncVlg = vlgConfigAvailable;
                    if (!sytStateSaved) syncTarget.SyncSytHcm = sytConfigAvailable;
                }
                if (!bytConfigAvailable) syncTarget.SyncByt = false;
                if (!hsskConfigAvailable) syncTarget.SyncHssk = false;
                if (!hocConfigAvailable) syncTarget.SyncHoc = false;
                if (!hccConfigAvailable) syncTarget.SyncHcc = false;
                if (!vlgConfigAvailable) syncTarget.SyncVlg = false;
                // Nut "Cap nhat KQ cong VLg" chi hien voi vien co cau hinh cong Vinh Long.
                // SWAP CAP voi EmptySpaceItem cung vi tri: item Never bi LOAI khoi layout (cac o loc se
                // gian ra lap cho) — nen vien KHONG config VLG phai giu EmptySpace de giao dien Y HET cu.
                lciBtnVlgStatus.Visibility = vlgConfigAvailable
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                // (26/08 Designer dong bo tu may dev da bo EmptySpaceItem "emptyFilterTop" va them btnClsMap — khong con swap cap.)
                if (!sytConfigAvailable) syncTarget.SyncSytHcm = false;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Có ít nhất 1 cổng liên thông được cấu hình -> mới cho phép Đồng bộ.</summary>
        private bool CanSync()
        {
            return bytConfigAvailable || hsskConfigAvailable || hocConfigAvailable || hccConfigAvailable
                || vlgConfigAvailable || sytConfigAvailable;
        }
        #endregion

        #region Xem du lieu se day (Scene 3)
        // Nut "Xuất XML": xuat file XML cho cac dong TICH (neu co) hoac TOAN BO ket qua tim kiem (khong phan trang).
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try { ExportXml(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void ShowPreview(V_HIS_KSK_SYNC row)
        {
            try
            {
                // Preview: truyen CA 5 config de MA_GTIN_CSKCB co the fallback sang SenderId cong HSSK/HCC/VLG.
                KskSyncProcessor processor = new KskSyncProcessor(
                    GetConfigValue(CONFIG_KEY__CONNECTION_INFO),
                    GetConfigValue(CONFIG_KEY__HSSK_HN_2062_CONNECTION_INFO),
                    GetHocConnectionInfo(),
                    GetHccConnectionInfo(),
                    GetVlgConnectionInfo(),
                    true, hsskConfigAvailable, hocConfigAvailable, hccConfigAvailable, vlgConfigAvailable,
                    chkSign.Checked, SettingSignADO);
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
        /// <summary>Chuỗi cấu hình cổng BYT của chi nhánh đang làm việc.</summary>
        private string GetConnectionInfo()
        {
            return GetConfigValue(CONFIG_KEY__CONNECTION_INFO);
        }

        /// <summary>BR7 - an toan da vien: chua cau hinh ket noi cong => khong day (Scene 5).</summary>
        private bool VerifyConnectionConfigured()
        {
            if (!string.IsNullOrWhiteSpace(GetConnectionInfo())) return true;
            XtraMessageBox.Show(
                "Chưa có cấu hình kết nối Cổng dữ liệu Y tế cho cơ sở " + GetCurrentBranchName() + "." + Environment.NewLine +
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

        // Click vao cot "Đẩy" (colPush - nut mui ten Up) -> day rieng ho so dong do.
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                var row = gridView1.GetRow(e.RowHandle) as V_HIS_KSK_SYNC;
                if (row == null) return;
                if (e.Column == colPush)
                    SyncRecords(new List<V_HIS_KSK_SYNC>() { row });         // cot "Đẩy" -> day rieng ho so
                else if (colPreview != null && e.Column == colPreview)
                    ShowPreview(row);                                        // cot "Xem" (eye) -> xem du lieu se day
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        // Ket qua chay tien trinh nen (push + luu) tra ve UI thread.
        private class SyncOutcome
        {
            public List<KskSyncResultADO> Results { get; set; }
            public bool SaveOk { get; set; }
            public string SaveError { get; set; }
        }

        private void SyncRecords(List<V_HIS_KSK_SYNC> rows)
        {
            try
            {
                if (rows == null || rows.Count == 0) return;

                // Cong dich = cong da CHON (popup settings) VA co cau hinh. base64 XML dung CHUNG cho BYT/HSSK/HOC;
                // cong HCC dung base64 rieng (mac dinh json/base64 theo tai lieu HCC);
                // cong VLG (KDLYT Vinh Long) gui XML truc tiep (khong base64) qua KskVlgPusher.
                bool toByt = syncTarget != null && syncTarget.SyncByt && bytConfigAvailable;
                bool toHssk = syncTarget != null && syncTarget.SyncHssk && hsskConfigAvailable;
                bool toHoc = syncTarget != null && syncTarget.SyncHoc && hocConfigAvailable;
                bool toHcc = syncTarget != null && syncTarget.SyncHcc && hccConfigAvailable;
                bool toVlg = syncTarget != null && syncTarget.SyncVlg && vlgConfigAvailable;

                bool toSytHcm = syncTarget != null && syncTarget.SyncSytHcm && sytConfigAvailable;

                // Scene 5: chua chon/chua cau hinh cong nao -> bao loi, khong day.
                // Tinh CA cong VLG va cong So Y te TP.HCM: chi tich rieng mot trong hai van phai day
                // duoc, khong bi cac cong khac chan.
                if (!toByt && !toHssk && !toHoc && !toHcc && !toVlg && !toSytHcm)
                {
                    XtraMessageBox.Show(
                        "Chưa chọn cổng liên thông có cấu hình để đẩy dữ liệu." + Environment.NewLine +
                        "Vui lòng bấm nút Cài đặt để chọn cổng (KSK BYT / HSSK / HOC / HCC / "
                        + "KDLYT Vĩnh Long / KSK Sở Y tế TP.HCM) đã được cấu hình.",
                        "Không thể đồng bộ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ky so ap dung cho MOI cong dung ban tin XML (BYT/HSSK/HOC + HCC che do xml) — ban tin base64
                // dung chung nen tich ky so la cong nao cung nhan ban tin da ky -> validate KHONG con theo BYT.
                if (chkSign.Checked && !IsSignSettingValid(SettingSignADO))
                {
                    XtraMessageBox.Show("Bạn đã bật Ký số nhưng chưa cấu hình chứng thư/chữ ký số. Vui lòng cấu hình trước khi đẩy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // CKS_NGUOI_KET_LUAN luon ky bang HSM cua nguoi ket luan (khong phu thuoc USB token/HSM cua
                // the CKS_BENH_VIEN) -> da tich ky so la doi hoi ho so nao cung co nguoi ket luan.
                if (chkSign.Checked && SettingSignADO != null)
                {
                    string missConcluderMsg;
                    if (!KskSyncProcessor.AllHaveConcluder(rows, out missConcluderMsg))
                    {
                        XtraMessageBox.Show(missConcluderMsg, "Thiếu người kết luận", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Chup input tren UI thread (KHONG cham control tu background thread).
                string connectionInfo = GetConfigValue(CONFIG_KEY__CONNECTION_INFO);
                string hsskConnectionInfo = GetConfigValue(CONFIG_KEY__HSSK_HN_2062_CONNECTION_INFO);
                string hocConnectionInfo = GetHocConnectionInfo();
                string hccConnectionInfo = GetHccConnectionInfo();
                string vlgConnectionInfo = GetVlgConnectionInfo();
                bool sign = chkSign.Checked;
                SettingSignADO signSettingLocal = this.SettingSignADO;
                long syncTime = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                List<V_HIS_KSK_SYNC> rowsLocal = rows;

                // Day cong + luu chay o TIEN TRINH NEN -> UI khong bi treo (van hien spinner, van thao tac duoc).
                WaitingManager.Show();
                SetSyncUiBusy(true);

                var worker = new System.ComponentModel.BackgroundWorker();
                worker.DoWork += (s, e) =>
                {
                    // Build + ky so + goi cong QD2062 (thu vien BD_046 - muc 3.4), roi luu trang thai.
                    // Day dong thoi BYT (toByt) / HSSK (toHssk) / HOC (toHoc) — base64 XML dung CHUNG —
                    // va/hoac HCC (toHcc) voi payload rieng theo data_type cua cau hinh HCC,
                    // va/hoac VLG (toVlg — KDLYT Vinh Long) voi XML truc tiep qua KskVlgPusher.
                    KskSyncProcessor processor = new KskSyncProcessor(
                        connectionInfo, hsskConnectionInfo, hocConnectionInfo, hccConnectionInfo, vlgConnectionInfo,
                        toByt, toHssk, toHoc, toHcc, toVlg, sign, signSettingLocal);
                    // Cổng thứ năm — Sở Y tế TP.HCM. Đặt qua thuộc tính để KHÔNG đổi chữ ký hàm khởi
                    // tạo mà 4 cổng cũ đang dùng.
                    processor.PushSytHcm = toSytHcm;
                    // Bảng khai báo nối chỉ số cận lâm sàng — nguồn của khối cận lâm sàng.
                    processor.SytClsMapJson = this.sytClsMapJson;
                    // PushList: đẩy TỪNG hồ sơ 1 + LƯU trạng thái ngay từng hồ sơ (không lưu batch lần 2 nữa).
                    List<KskSyncResultADO> results = processor.PushList(rowsLocal, syncTime);
                    e.Result = new SyncOutcome { Results = results, SaveOk = processor.SaveAllOk, SaveError = processor.SaveError };
                };
                worker.RunWorkerCompleted += (s, e) =>
                {
                    try
                    {
                        WaitingManager.Hide();
                        if (e.Error != null)
                        {
                            Inventec.Common.Logging.LogSystem.Error(e.Error);
                            XtraMessageBox.Show("Lỗi khi đồng bộ lên cổng." + Environment.NewLine + e.Error.Message,
                                "Đồng bộ thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        SyncOutcome outcome = e.Result as SyncOutcome;
                        if (outcome == null) return;

                        if (!outcome.SaveOk)
                        {
                            XtraMessageBox.Show(
                                "Không lưu được trạng thái đồng bộ (mã lỗi / lý do) vào hệ thống." + Environment.NewLine + (outcome.SaveError ?? ""),
                                "Lưu trạng thái thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        // Scene 4: hop thoai tong hop ket qua day lo
                        SyncResult.frmKskSyncResult frm = new SyncResult.frmKskSyncResult(outcome.Results);
                        frm.ShowDialog();

                        FillDataToGrid();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    finally
                    {
                        SetSyncUiBusy(false);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                SetSyncUiBusy(false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Khoa/mo control khi dang day cong (tranh double-click, giu UI phan hoi). Chay tren UI thread.
        private void SetSyncUiBusy(bool busy)
        {
            try
            {
                btnPreview.Enabled = !busy;
                btnSearch.Enabled = !busy;
                btnRefresh.Enabled = !busy;
                btnVlgStatus.Enabled = !busy;
                chkSign.Enabled = !busy;
                gridControl1.Enabled = !busy;

                if (busy)
                {
                    btnSync.Enabled = false;
                }
                else
                {
                    // Khoi phuc trang thai nut Dong bo theo so ho so dang chon.
                    int count = gridView1.GetSelectedRows().Count(rh => rh >= 0);
                    btnSync.Enabled = count > 0 && CanSync();
                    UpdateSyncBadge(count);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        // (Đã bỏ PersistSyncResult: việc LƯU trạng thái do KskSyncProcessor.PushList thực hiện batch 1 lần
        //  qua List<HIS_KSK_SYNC> — không còn lưu từ UC bằng List<KskSyncResultADO>.)

        /// <summary>
        /// Nút "Cập nhật KQ cổng VLg": tra cứu kết quả xử lý THẬT trên Cổng tiếp nhận Vĩnh Long
        /// (cổng tiếp nhận bất đồng bộ — "Đã đồng bộ" mới là "đã tiếp nhận") và cập nhật trạng thái hồ sơ:
        /// ĐẠT (VALID) giữ Đã đồng bộ + ghi chú; KHÔNG ĐẠT (INVALID) chuyển Thất bại + lý do lỗi của cổng.
        /// Hồ sơ tra cứu = dòng TÍCH; không tích thì toàn bộ hồ sơ ĐÃ TỪNG ĐẨY trên trang hiện tại.
        /// </summary>
        private void btnVlgStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (!vlgConfigAvailable) return;

                // Chi tra cuu ho so DA TUNG DAY (GetSyncType != 1) — ke ca o nhanh TICK, tranh dong
                // "Chua dong bo" bi bao "That bai: chua co tren cong" gay hieu nham + request thua.
                var rows = gridView1.GetSelectedRows().Where(rh => rh >= 0)
                    .Select(rh => gridView1.GetRow(rh) as V_HIS_KSK_SYNC)
                    .Where(r => r != null && GetSyncType(r) != 1).ToList();
                if (rows.Count == 0)
                {
                    var ds = gridControl1.DataSource as List<V_HIS_KSK_SYNC>;
                    if (ds != null)
                        rows = ds.Where(r => r != null && GetSyncType(r) != 1).ToList();
                }
                if (rows.Count == 0)
                {
                    XtraMessageBox.Show(
                        "Không có hồ sơ đã đẩy để tra cứu (hồ sơ 'Chưa đồng bộ' được bỏ qua)." + Environment.NewLine +
                        "Tích chọn hồ sơ đã đẩy cần tra, hoặc để trống để tra toàn bộ hồ sơ đã đẩy trên trang hiện tại.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Chup input tren UI thread.
                string vlgConnectionInfo = GetVlgConnectionInfo();
                List<V_HIS_KSK_SYNC> rowsLocal = rows;

                WaitingManager.Show();
                SetSyncUiBusy(true);

                var worker = new System.ComponentModel.BackgroundWorker();
                worker.DoWork += (s, ev) =>
                {
                    // Processor chi can cau hinh VLG — cac cong khac khong tham gia tra cuu.
                    KskSyncProcessor processor = new KskSyncProcessor(null, null, null, null, vlgConnectionInfo,
                        false, false, false, false, true, false, null);
                    List<KskSyncResultADO> results = processor.UpdateVlgStatuses(rowsLocal);
                    ev.Result = new SyncOutcome { Results = results, SaveOk = processor.SaveAllOk, SaveError = processor.SaveError };
                };
                worker.RunWorkerCompleted += (s, ev) =>
                {
                    try
                    {
                        WaitingManager.Hide();
                        if (ev.Error != null)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ev.Error);
                            XtraMessageBox.Show("Lỗi khi tra cứu kết quả trên cổng." + Environment.NewLine + ev.Error.Message,
                                "Tra cứu thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        SyncOutcome outcome = ev.Result as SyncOutcome;
                        if (outcome == null) return;
                        if (!outcome.SaveOk)
                        {
                            XtraMessageBox.Show(
                                "Không lưu được trạng thái cập nhật vào hệ thống." + Environment.NewLine + (outcome.SaveError ?? ""),
                                "Lưu trạng thái thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        SyncResult.frmKskSyncResult frm = new SyncResult.frmKskSyncResult(outcome.Results);
                        frm.ShowDialog();
                        FillDataToGrid();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    finally
                    {
                        SetSyncUiBusy(false);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                SetSyncUiBusy(false);
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        #region Xuat XML (btnExportPath + duong dan)
        private DevExpress.XtraBars.PopupControlContainer popupExport;
        private DevExpress.XtraEditors.TextEdit txtExportPath;

        private void btnExportPath_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureExportPathPopup();
                if (txtExportPath != null) txtExportPath.Text = exportXmlPath ?? "";
                System.Drawing.Point p = btnExportPath.PointToScreen(new System.Drawing.Point(0, btnExportPath.Height + 2));
                popupExport.ShowPopup(p);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Popup chuan LayoutControl: label "Đường dẫn xuất XML:" + textEdit + nut folder chon thu muc.</summary>
        private void EnsureExportPathPopup()
        {
            if (popupExport != null) return;

            var lbl = new DevExpress.XtraEditors.LabelControl();
            lbl.Text = "Đường dẫn xuất XML:";
            txtExportPath = new DevExpress.XtraEditors.TextEdit();
            txtExportPath.Text = exportXmlPath ?? "";
            var btnBrowse = new DevExpress.XtraEditors.SimpleButton();
            var fimg = KskSyncIcons.Folder();
            if (fimg != null) btnBrowse.Image = fimg;
            btnBrowse.ToolTip = "Chọn thư mục xuất XML";
            btnBrowse.Click += btnBrowseExportPath_Click;

            var lc = new DevExpress.XtraLayout.LayoutControl();
            lc.Dock = System.Windows.Forms.DockStyle.Fill;
            lc.BeginUpdate();
            var lciLbl = (DevExpress.XtraLayout.LayoutControlItem)lc.Root.AddItem();
            lciLbl.Control = lbl; lciLbl.TextVisible = false;
            var lciTxt = (DevExpress.XtraLayout.LayoutControlItem)lc.Root.AddItem();
            lciTxt.Control = txtExportPath; lciTxt.TextVisible = false;
            var lciBtn = (DevExpress.XtraLayout.LayoutControlItem)lc.Root.AddItem();
            lciBtn.Control = btnBrowse; lciBtn.TextVisible = false;
            lciBtn.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            lciBtn.MaxSize = new System.Drawing.Size(28, 24);
            lciBtn.MinSize = new System.Drawing.Size(28, 24);
            // Bo cuc: label tren cung; textEdit + nut folder cung 1 hang duoi label.
            lciTxt.Move(lciLbl, DevExpress.XtraLayout.Utils.InsertType.Bottom);
            lciBtn.Move(lciTxt, DevExpress.XtraLayout.Utils.InsertType.Right);
            lc.Root.GroupBordersVisible = false;
            lc.EndUpdate();

            popupExport = new DevExpress.XtraBars.PopupControlContainer();
            popupExport.Name = "popupControlContainerExport";
            popupExport.Manager = this.barManager1;
            popupExport.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            popupExport.Size = new System.Drawing.Size(380, 64);
            popupExport.Controls.Add(lc);
            popupExport.Visible = false;
            this.Controls.Add(popupExport);
        }

        private void btnBrowseExportPath_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
                {
                    dlg.Description = "Chọn thư mục xuất XML";
                    if (!string.IsNullOrWhiteSpace(txtExportPath.Text) && System.IO.Directory.Exists(txtExportPath.Text))
                        dlg.SelectedPath = txtExportPath.Text;
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        txtExportPath.Text = dlg.SelectedPath;
                        exportXmlPath = dlg.SelectedPath;
                        SaveControlStateExportPath();
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Luu duong dan xuat XML local qua ControlState (key = btnExportPath.Name).</summary>
        private void SaveControlStateExportPath()
        {
            try
            {
                if (this.controlStateWorker == null) return;
                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                var cs = this.currentControlStateRDO
                    .Where(o => o.KEY == btnExportPath.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault();
                if (cs != null) cs.VALUE = exportXmlPath ?? "";
                else
                {
                    cs = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    cs.KEY = btnExportPath.Name;
                    cs.VALUE = exportXmlPath ?? "";
                    cs.MODULE_LINK = this.currentModule.ModuleLink;
                    this.currentControlStateRDO.Add(cs);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Xuat XML: co tich dong -> chi xuat cac dong tich; khong tich -> xuat TOAN BO theo API tim kiem
        /// (khong phan trang). Chay o tien trinh nen; moi ho so 1 file trong thu muc da thiet lap.
        /// </summary>
        private void ExportXml()
        {
            // Tich ky so ma chua cau hinh chung thu -> CHAN, khong xuat file khong co CKS_ ma bao "thanh cong".
            if (chkSign.Checked && !IsSignSettingValid(SettingSignADO))
            {
                XtraMessageBox.Show("Bạn đã bật Ký số nhưng chưa cấu hình chứng thư/chữ ký số. Vui lòng cấu hình trước khi xuất XML.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(exportXmlPath))
            {
                XtraMessageBox.Show("Chưa thiết lập đường dẫn xuất XML. Vui lòng bấm nút thư mục để chọn đường dẫn.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnExportPath_Click(null, null);
                return;
            }
            try
            {
                if (!System.IO.Directory.Exists(exportXmlPath)) System.IO.Directory.CreateDirectory(exportXmlPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Đường dẫn xuất XML không hợp lệ / không tạo được:" + Environment.NewLine + exportXmlPath,
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Chup input tren UI thread
            List<V_HIS_KSK_SYNC> selectedRows = gridView1.GetSelectedRows().Where(rh => rh >= 0)
                .Select(rh => gridView1.GetRow(rh) as V_HIS_KSK_SYNC).Where(r => r != null).ToList();
            bool exportAll = selectedRows.Count == 0;
            HisKskSyncViewFilter filter = null;
            if (exportAll) { filter = new HisKskSyncViewFilter(); SetFilter(ref filter); }  // SetFilter doc control -> UI thread
            string dir = exportXmlPath;
            string bytInfo = GetConfigValue(CONFIG_KEY__CONNECTION_INFO);
            string hsskInfo = GetConfigValue(CONFIG_KEY__HSSK_HN_2062_CONNECTION_INFO);
            string hocInfo = GetHocConnectionInfo();
            string hccInfo = GetHccConnectionInfo();
            string vlgInfo = GetVlgConnectionInfo();
            bool sign = chkSign.Checked;
            SettingSignADO signLocal = this.SettingSignADO;
            bool hsskAvail = this.hsskConfigAvailable;
            bool hocAvail = this.hocConfigAvailable;
            bool hccAvail = this.hccConfigAvailable;
            bool vlgAvail = this.vlgConfigAvailable;

            WaitingManager.Show();
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.DoWork += (s, ev) =>
            {
                List<V_HIS_KSK_SYNC> rows = exportAll ? FetchAllRows(filter) : selectedRows;
                if (rows == null || rows.Count == 0) { ev.Result = new object[] { 0, 0, 0, null }; return; }
                KskSyncProcessor processor = new KskSyncProcessor(bytInfo, hsskInfo, hocInfo, hccInfo, vlgInfo,
                    true, hsskAvail, hocAvail, hccAvail, vlgAvail, sign, signLocal);
                int failed; string err;
                int ok = processor.ExportXmlFiles(rows, dir, out failed, out err);
                ev.Result = new object[] { ok, failed, rows.Count, err };
            };
            worker.RunWorkerCompleted += (s, ev) =>
            {
                try
                {
                    WaitingManager.Hide();
                    if (ev.Error != null)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ev.Error);
                        XtraMessageBox.Show("Lỗi khi xuất XML." + Environment.NewLine + ev.Error.Message,
                            "Xuất XML thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    object[] r = ev.Result as object[];
                    int ok = (r != null) ? Convert.ToInt32(r[0]) : 0, failed = (r != null) ? Convert.ToInt32(r[1]) : 0, total = (r != null) ? Convert.ToInt32(r[2]) : 0;
                    string err = (r != null && r.Length > 3) ? r[3] as string : null;
                    // Loi validate (VD thieu nguoi ket luan khi ky so) -> bao va dung.
                    if (!string.IsNullOrEmpty(err))
                    {
                        XtraMessageBox.Show(err, "Không thể xuất XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (total == 0)
                    {
                        XtraMessageBox.Show("Không có hồ sơ để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    XtraMessageBox.Show(string.Format(
                        "Đã xuất {0}/{1} hồ sơ ra XML{2}.{3}Khi ký số, hồ sơ được gom theo người kết luận (mỗi người 1 file).{3}Thư mục: {4}", ok, total,
                        (failed > 0 ? (" (" + failed + " hồ sơ lỗi/thiếu dữ liệu)") : ""), Environment.NewLine, dir),
                        "Xuất XML", MessageBoxButtons.OK,
                        (failed > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information));
                }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
            };
            worker.RunWorkerAsync();
        }

        /// <summary>Goi api/HisKskSync/GetView voi filter da dung (KHONG phan trang) -> toan bo ho so.</summary>
        private List<V_HIS_KSK_SYNC> FetchAllRows(HisKskSyncViewFilter filter)
        {
            try
            {
                CommonParam param = new CommonParam();   // khong truyen Start/Limit -> lay toan bo (khong phan trang)
                ApiResultObject<List<V_HIS_KSK_SYNC>> rs =
                    new BackendAdapter(param).GetRO<List<V_HIS_KSK_SYNC>>("api/HisKskSync/GetView", ApiConsumers.MosConsumer, filter, param);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                return (rs != null && rs.Data != null) ? rs.Data : new List<V_HIS_KSK_SYNC>();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); return new List<V_HIS_KSK_SYNC>(); }
        }
        #endregion
    }
}

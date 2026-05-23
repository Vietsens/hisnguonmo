/* IVT
 * @Project : hisnguonmo
 * Việc 45072 — Fill 4 ICD, TG xử lý, Vô cảm, Máy, Cách thức, Kết luận... khi click row.
 * Cũng wire DanhSachYLenh button + ControlState cho chkKT_v45072.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.ADO;
using HIS.Desktop.Utilities;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    public partial class UCSurgServiceReqExecute2 : HIS.Desktop.Utility.UserControlBase
    {
        // Cache 2 entities lần cuối click — dùng cho Save (V_HIS_SERE_SERV_PTTT là view chỉ đọc)
        internal V_HIS_SERE_SERV_PTTT currentSereServPttt_v45072 { get; set; }
        internal HIS_SERE_SERV_EXT currentSereServExt_v45072 { get; set; }

        /// <summary>
        /// Việc 45072 — Wire tất cả events cho các control _v45072.
        /// Gọi từ constructor.
        /// </summary>
        private void Wire45072Events()
        {
            try
            {
                if (cboPtttTemp_v45072 != null)
                {
                    cboPtttTemp_v45072.EditValueChanged += CboPtttTemp_v45072_EditValueChanged;
                    cboPtttTemp_v45072.ButtonClick += CboPtttTemp_v45072_ButtonClick;
                }
                if (cboEmotionLess_v45072 != null)
                    cboEmotionLess_v45072.ButtonClick += CboEmotionLess_v45072_ButtonClick;
                if (cboMachine_v45072 != null)
                    cboMachine_v45072.ButtonClick += CboMachine_v45072_ButtonClick;
                if (btnSavePtttTemp_v45072 != null)
                    btnSavePtttTemp_v45072.Click += BtnSavePtttTemp_v45072_Click;
                if (btnDanhSachYLenh_v45072 != null)
                    btnDanhSachYLenh_v45072.Click += BtnDanhSachYLenh_v45072_Click;
                if (chkKT_v45072 != null)
                    chkKT_v45072.CheckedChanged += ChkKT_v45072_CheckedChanged;
                if (spnTimeProcess_v45072 != null)
                    spnTimeProcess_v45072.EditValueChanged += SpnTimeProcess_v45072_EditValueChanged;
                if (dteStart != null)
                    dteStart.EditValueChanged += DteStart_v45072_EditValueChanged;

                // Wire grid trái: custom unbound + popup menu
                if (gridView1 != null)
                    gridView1.CustomUnboundColumnData += GridView1_CustomUnbound_v45072;
                WirePopupMenu_v45072();

                // Việc 45072 — Setup DataSource cho cboEmotionLess_v45072 + cboMachine_v45072 (TuanLN báo bug: combo Vô cảm phải map HIS_EMOTIONLESS_METHOD)
                InitLookupCombos_v45072();

                // Việc 45072 — TuanLN báo: CĐ phụ + ICD9 phụ PHẢI cho nhập NHIỀU ICD (giống Execute SecondaryIcd)
                // Không chặn ký tự ';', cho phép multi-value cách nhau bởi ';'
                // Wire popup chọn nhiều ICD khi nhấn F1 + sync multi-value code↔name
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        /// <summary>
        /// Việc 45072 — Flag chống infinite loop khi 2-way sync code↔combo Vô cảm.
        /// </summary>
        private bool isSyncingEmotionLess_v45072 = false;

        /// <summary>
        /// Việc 45072 — Khi user chọn cbo Vô cảm → fill txt code tương ứng.
        /// </summary>
        private void CboEmotionLess_v45072_SyncCode(object sender, EventArgs e)
        {
            if (isSyncingEmotionLess_v45072) return;
            try
            {
                isSyncingEmotionLess_v45072 = true;
                if (cboEmotionLess_v45072 == null || txtEmotionLessCode_v45072 == null) return;
                if (cboEmotionLess_v45072.EditValue == null)
                {
                    txtEmotionLessCode_v45072.Text = string.Empty;
                    return;
                }
                long emoId;
                if (!long.TryParse(cboEmotionLess_v45072.EditValue.ToString(), out emoId)) return;
                var emo = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>()
                    .FirstOrDefault(o => o.ID == emoId);
                txtEmotionLessCode_v45072.Text = emo != null ? emo.EMOTIONLESS_METHOD_CODE : string.Empty;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            finally { isSyncingEmotionLess_v45072 = false; }
        }

        /// <summary>
        /// Việc 45072 — Khi user gõ mã Vô cảm → tìm trong cache → set cbo.
        /// </summary>
        private void TxtEmotionLessCode_v45072_SyncCombo(object sender, EventArgs e)
        {
            if (isSyncingEmotionLess_v45072) return;
            try
            {
                isSyncingEmotionLess_v45072 = true;
                if (txtEmotionLessCode_v45072 == null || cboEmotionLess_v45072 == null) return;
                var code = (txtEmotionLessCode_v45072.Text ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(code))
                {
                    cboEmotionLess_v45072.EditValue = null;
                    return;
                }
                var emo = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>()
                    .FirstOrDefault(o => o.EMOTIONLESS_METHOD_CODE != null
                                      && o.EMOTIONLESS_METHOD_CODE.Equals(code, StringComparison.OrdinalIgnoreCase));
                cboEmotionLess_v45072.EditValue = emo != null ? (object)emo.ID : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            finally { isSyncingEmotionLess_v45072 = false; }
        }

        /// <summary>
        /// Việc 45072 — Setup DataSource cho 2 LookUpEdit mới (cboEmotionLess + cboMachine).
        /// cboEmotionLess: map HIS_EMOTIONLESS_METHOD (Vô cảm chính — 4 phương pháp mổ + Không tê).
        /// cboMachine: map HIS_MACHINE (Máy thực hiện).
        /// </summary>
        private void InitLookupCombos_v45072()
        {
            try
            {
                // Combo Vô cảm — adapt pattern Execute (ComboEmotionlessMothod):
                //   IS_ACTIVE=1 AND (IS_FIRST==1 OR (IS_FIRST!=1 AND IS_SECOND!=1))
                // Mục đích: chỉ hiển thị các phương pháp vô cảm CHÍNH (loại trừ những phương pháp chỉ dùng cho "Phương pháp 2").
                if (cboEmotionLess_v45072 != null)
                {
                    var emoList = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>()
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                 && (o.IS_FIRST == 1 || (o.IS_FIRST != 1 && o.IS_SECOND != 1)))
                        .OrderBy(o => o.EMOTIONLESS_METHOD_CODE)
                        .ToList();
                    cboEmotionLess_v45072.Properties.DataSource = emoList;
                    cboEmotionLess_v45072.Properties.DisplayMember = "EMOTIONLESS_METHOD_NAME";
                    cboEmotionLess_v45072.Properties.ValueMember = "ID";
                    cboEmotionLess_v45072.Properties.Columns.Clear();
                    cboEmotionLess_v45072.Properties.Columns.Add(
                        new DevExpress.XtraEditors.Controls.LookUpColumnInfo("EMOTIONLESS_METHOD_CODE", "Mã", 80));
                    cboEmotionLess_v45072.Properties.Columns.Add(
                        new DevExpress.XtraEditors.Controls.LookUpColumnInfo("EMOTIONLESS_METHOD_NAME", "Tên", 250));
                    cboEmotionLess_v45072.Properties.PopupWidth = 350;
                    cboEmotionLess_v45072.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoFilter;
                    cboEmotionLess_v45072.Properties.AutoSearchColumnIndex = 1;
                    cboEmotionLess_v45072.Properties.ImmediatePopup = true;

                    // Việc 45072 — 2-way sync code↔combo (TuanLN y/c layout giống Phương pháp TT)
                    cboEmotionLess_v45072.EditValueChanged += CboEmotionLess_v45072_SyncCode;
                    if (txtEmotionLessCode_v45072 != null)
                        txtEmotionLessCode_v45072.EditValueChanged += TxtEmotionLessCode_v45072_SyncCombo;
                }

                // Combo Máy thực hiện — load HIS_MACHINE IS_ACTIVE=1
                if (cboMachine_v45072 != null)
                {
                    var machineList = BackendDataWorker.Get<HIS_MACHINE>()
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        .OrderBy(o => o.MACHINE_CODE)
                        .ToList();
                    cboMachine_v45072.Properties.DataSource = machineList;
                    cboMachine_v45072.Properties.DisplayMember = "MACHINE_NAME";
                    cboMachine_v45072.Properties.ValueMember = "ID";
                    cboMachine_v45072.Properties.Columns.Clear();
                    cboMachine_v45072.Properties.Columns.Add(
                        new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MACHINE_CODE", "Mã", 100));
                    cboMachine_v45072.Properties.Columns.Add(
                        new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MACHINE_NAME", "Tên", 300));
                    cboMachine_v45072.Properties.PopupWidth = 400;
                    cboMachine_v45072.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoFilter;
                    cboMachine_v45072.Properties.AutoSearchColumnIndex = 1;
                    cboMachine_v45072.Properties.ImmediatePopup = true;
                }

                // Việc 45072 — Setup 4 cbo ICD (y/c TuanLN: làm tương tự Vô cảm)
                InitLookupIcd_v45072();

                // Việc 45072 — Wire F1 trên cbo phụ → mở plugin SecondaryIcd chọn nhiều bệnh phụ
                WireF1OpenSecondaryIcd_v45072();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Setup 4 cbo ICD/ICD9 + 2-way sync code↔combo:
        /// - CĐ chính: txtIcdCode_v45072 ↔ cboIcdName_v45072 (HIS_ICD) — single-select
        /// - CĐ phụ: cboIcdText_v45072 — multi-value (TextEditStyle=Standard, F1 mở SecondaryIcd)
        /// - ICD9 chính: txtIcdCmCode_v45072 ↔ cboIcdCmName_v45072 (HIS_ICD_CM) — single-select
        /// - ICD9 phụ: cboIcdCmText_v45072 — multi-value (TextEditStyle=Standard, F1 mở SecondaryIcd)
        /// </summary>
        private void InitLookupIcd_v45072()
        {
            try
            {
                var icdList = BackendDataWorker.Get<HIS_ICD>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.ICD_CODE)
                    .ToList();
                var icdCmList = BackendDataWorker.Get<HIS_ICD_CM>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.ICD_CM_CODE)
                    .ToList();

                // CĐ chính + CĐ phụ — dùng HIS_ICD
                SetupLookupIcd_v45072(cboIcdName_v45072, icdList, "ICD_CODE", "ICD_NAME");
                SetupLookupIcd_v45072(cboIcdText_v45072, icdList, "ICD_CODE", "ICD_NAME");

                // ICD9 chính + ICD9 phụ — dùng HIS_ICD_CM
                SetupLookupIcd_v45072(cboIcdCmName_v45072, icdCmList, "ICD_CM_CODE", "ICD_CM_NAME");
                SetupLookupIcd_v45072(cboIcdCmText_v45072, icdCmList, "ICD_CM_CODE", "ICD_CM_NAME");

                // Việc 45072 — 2 cbo PHỤ cần TextEditStyle.Standard để cho phép set Text multi-value (cách ';')
                if (cboIcdText_v45072 != null)
                    cboIcdText_v45072.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                if (cboIcdCmText_v45072 != null)
                    cboIcdCmText_v45072.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

                // Việc 45072 — TuanLN báo: ICD code gõ thường hay hoa đều tự chuyển in HOA
                if (txtIcdCode_v45072 != null)
                    txtIcdCode_v45072.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                if (txtIcdSubCode_v45072 != null)
                    txtIcdSubCode_v45072.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                if (txtIcdCmCode_v45072 != null)
                    txtIcdCmCode_v45072.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                if (txtIcdCmSubCode_v45072 != null)
                    txtIcdCmSubCode_v45072.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;

                // Việc 45072 — 2-way sync code↔combo cho cả 4 cặp (click vào combo + gõ key đều hoạt động)
                // Phụ: khi gõ multi-value 'a00;a01' → fill name 'Bệnh tả;Sốt thương hàn' (parse từng code, lookup, ghép)
                WireIcdSync_v45072(txtIcdCode_v45072, cboIcdName_v45072, icdList, "ICD_CODE", "ICD_NAME");
                WireIcdSync_v45072(txtIcdSubCode_v45072, cboIcdText_v45072, icdList, "ICD_CODE", "ICD_NAME");
                WireIcdSync_v45072(txtIcdCmCode_v45072, cboIcdCmName_v45072, icdCmList, "ICD_CM_CODE", "ICD_CM_NAME");
                WireIcdSync_v45072(txtIcdCmSubCode_v45072, cboIcdCmText_v45072, icdCmList, "ICD_CM_CODE", "ICD_CM_NAME");

                // Việc 45072 — Wire ButtonClick (nút x) cho CĐ chính + ICD9-CM chính để clear giá trị
                if (cboIcdName_v45072 != null)
                    cboIcdName_v45072.ButtonClick += CboIcdName_v45072_ButtonClick;
                if (cboIcdCmName_v45072 != null)
                    cboIcdCmName_v45072.ButtonClick += CboIcdCmName_v45072_ButtonClick;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Click nút x trên CĐ chính → clear ICD chính.
        /// EditValueChanged đã wire trong WireIcdSync_v45072 sẽ tự đồng bộ txtIcdCode_v45072 về rỗng.
        /// </summary>
        private void CboIcdName_v45072_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboIcdName_v45072.EditValue = null;
                    if (txtIcdCode_v45072 != null) txtIcdCode_v45072.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Click nút x trên ICD9-CM chính → clear ICD9-CM chính.
        /// </summary>
        private void CboIcdCmName_v45072_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboIcdCmName_v45072.EditValue = null;
                    if (txtIcdCmCode_v45072 != null) txtIcdCmCode_v45072.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Click nút x trên combo Mẫu PTTT → clear template selection.
        /// Các field đã fill từ template (Vô cảm, Cách thức, Kết luận, Mô tả, Ghi chú) GIỮ NGUYÊN —
        /// user có thể đã sửa hoặc muốn dùng tiếp data đó, KHÔNG ép xoá kèm.
        /// </summary>
        private void CboPtttTemp_v45072_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboPtttTemp_v45072.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Click nút x trên combo Vô cảm → clear EditValue.
        /// CboEmotionLess_v45072_SyncCode đã wire trên EditValueChanged sẽ tự xử lý đồng bộ.
        /// </summary>
        private void CboEmotionLess_v45072_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboEmotionLess_v45072.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Click nút x trên combo Máy thực hiện → clear EditValue.
        /// </summary>
        private void CboMachine_v45072_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboMachine_v45072.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Setup LookUpEdit thành "free text" mode để hỗ trợ multi-value ICD (cách ';').
        /// Disable popup grid, cho phép gõ tự do; user nhấn F1 để mở plugin SecondaryIcd chọn nhiều ICD.
        /// </summary>
        private void SetupMultiValueIcdEdit_v45072(DevExpress.XtraEditors.LookUpEdit cbo)
        {
            if (cbo == null) return;
            try
            {
                // Xoá DataSource (nếu đã set ở SetupLookupIcd) để combo không hiện popup data
                cbo.Properties.DataSource = null;
                cbo.Properties.Columns.Clear();
                // Standard = TextEdit-like: user gõ tự do, EditValue = string raw
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.ImmediatePopup = false;
                // KHÔNG show button drop-down — chỉ cho phép gõ + F1
                cbo.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Wire phím F1 trên CĐ phụ + ICD9 phụ → mở plugin HIS.Desktop.Plugins.SecondaryIcd.
        /// Pattern adapt từ Execute (txtIcdText_KeyUp + GetStringIcds).
        /// Plugin con trả về (codes, names) qua DelegateRefeshIcdChandoanphu → fill 2 control.
        /// </summary>
        private void WireF1OpenSecondaryIcd_v45072()
        {
            try
            {
                // Việc 45072 — Wire F1 trên CẢ txt code lẫn cbo name (LookUpEdit có thể chặn F1 internal)
                if (txtIcdSubCode_v45072 != null)
                    txtIcdSubCode_v45072.KeyUp += CboIcdText_v45072_KeyUp;
                if (cboIcdText_v45072 != null)
                    cboIcdText_v45072.KeyUp += CboIcdText_v45072_KeyUp;
                if (txtIcdCmSubCode_v45072 != null)
                    txtIcdCmSubCode_v45072.KeyUp += CboIcdCmText_v45072_KeyUp;
                if (cboIcdCmText_v45072 != null)
                    cboIcdCmText_v45072.KeyUp += CboIcdCmText_v45072_KeyUp;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboIcdText_v45072_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                    OpenSecondaryIcdPopup_v45072(isIcdCm: false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboIcdCmText_v45072_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                    OpenSecondaryIcdPopup_v45072(isIcdCm: true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Mở plugin SecondaryIcd để chọn nhiều ICD.
        /// isIcdCm = false → CĐ phụ (HIS_ICD), true → ICD9 phụ (HIS_ICD_CM).
        /// TODO: Plugin SecondaryIcd mặc định dùng HIS_ICD; nếu cần lọc HIS_ICD_CM thì plugin SecondaryIcd
        /// hiện chưa hỗ trợ — tạm thời dùng chung. Tester verify lại với TuanLN.
        /// </summary>
        private void OpenSecondaryIcdPopup_v45072(bool isIcdCm)
        {
            try
            {
                var moduleData = GlobalVariables.currentModuleRaws
                    .FirstOrDefault(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd");
                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    XtraMessageBox.Show(
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay),
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string currentCodes, currentNames;
                HIS.Desktop.Common.DelegateRefeshIcdChandoanphu callback;
                if (isIcdCm)
                {
                    currentCodes = txtIcdCmSubCode_v45072 != null ? (txtIcdCmSubCode_v45072.Text ?? string.Empty) : string.Empty;
                    currentNames = cboIcdCmText_v45072 != null ? (cboIcdCmText_v45072.Text ?? string.Empty) : string.Empty;
                    callback = GetStringIcdCms_v45072;
                }
                else
                {
                    currentCodes = txtIcdSubCode_v45072 != null ? (txtIcdSubCode_v45072.Text ?? string.Empty) : string.Empty;
                    currentNames = cboIcdText_v45072 != null ? (cboIcdText_v45072.Text ?? string.Empty) : string.Empty;
                    callback = GetStringIcds_v45072;
                }

                var ado = new HIS.Desktop.ADO.SecondaryIcdADO(callback, currentCodes, currentNames);
                var listArgs = new List<object> { ado };
                // Việc 45072 — Plugin SecondaryIcd: truyền thêm `true` vào args để load HIS_ICD_CM thay vì HIS_ICD (pattern Execute line 3118)
                if (isIcdCm) listArgs.Add(true);
                var instance = PluginInstance.GetPluginInstance(
                    PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId),
                    listArgs);
                if (instance == null) return;
                if (instance is Form) ((Form)instance).Show(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Callback từ plugin SecondaryIcd trả về codes + names cho CĐ phụ.
        /// Pattern Execute (line 2192-2210): chỉ set khi delegate có giá trị, KHÔNG xóa text cũ.
        /// </summary>
        private void GetStringIcds_v45072(string delegateIcdCodes, string delegateIcdNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateIcdCodes) && txtIcdSubCode_v45072 != null)
                    txtIcdSubCode_v45072.Text = delegateIcdCodes;
                if (!string.IsNullOrEmpty(delegateIcdNames) && cboIcdText_v45072 != null)
                    cboIcdText_v45072.Text = delegateIcdNames;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Callback từ plugin SecondaryIcd trả về codes + names cho ICD9 phụ.
        /// Pattern Execute (line 3130-3148): chỉ set khi delegate có giá trị.
        /// </summary>
        private void GetStringIcdCms_v45072(string delegateIcdCodes, string delegateIcdNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateIcdCodes) && txtIcdCmSubCode_v45072 != null)
                    txtIcdCmSubCode_v45072.Text = delegateIcdCodes;
                if (!string.IsNullOrEmpty(delegateIcdNames) && cboIcdCmText_v45072 != null)
                    cboIcdCmText_v45072.Text = delegateIcdNames;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Helper setup 1 LookUpEdit ICD/ICD_CM (DataSource + 2 columns Mã/Tên).
        /// ValueMember = CODE (vì DB lưu CODE, không lưu ID).
        /// Case-INSENSITIVE search trong popup grid (TuanLN y/c: gõ "a01" match "A01").
        /// </summary>
        private void SetupLookupIcd_v45072(DevExpress.XtraEditors.LookUpEdit cbo,
            System.Collections.IList dataSource, string codeMember, string nameMember)
        {
            if (cbo == null) return;
            try
            {
                cbo.Properties.DataSource = dataSource;
                cbo.Properties.DisplayMember = nameMember;
                cbo.Properties.ValueMember = codeMember;
                cbo.Properties.Columns.Clear();
                cbo.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(codeMember, "Mã", 80));
                cbo.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameMember, "Tên", 350));
                cbo.Properties.PopupWidth = 450;
                cbo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoFilter;
                cbo.Properties.AutoSearchColumnIndex = 1;
                cbo.Properties.ImmediatePopup = true;
                cbo.Properties.CaseSensitiveSearch = false;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Việc 45072 — Flag chống loop khi sync 4 cặp ICD.
        /// </summary>
        private bool isSyncingIcd_v45072 = false;

        /// <summary>
        /// Việc 45072 — Wire 2-way sync code↔combo cho 1 cặp ICD.
        /// Gõ code (case-INSENSITIVE) → tìm trong list → set combo theo CODE đúng case DB.
        /// Chọn combo → fill code vào textbox.
        /// </summary>
        private void WireIcdSync_v45072(DevExpress.XtraEditors.TextEdit txtCode,
            DevExpress.XtraEditors.LookUpEdit cbo,
            System.Collections.IList dataSource, string codeMember, string nameMember)
        {
            if (txtCode == null || cbo == null) return;
            try
            {
                cbo.EditValueChanged += (s, e) =>
                {
                    if (isSyncingIcd_v45072) return;
                    try
                    {
                        isSyncingIcd_v45072 = true;
                        var val = cbo.EditValue as string;
                        txtCode.Text = string.IsNullOrEmpty(val) ? string.Empty : val;
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                    finally { isSyncingIcd_v45072 = false; }
                };
                // Việc 45072 — Sync chỉ trigger khi user nhấn ENTER hoặc LEAVE (rời focus) — KHÔNG sync khi gõ từng ký tự
                // Lý do: nếu sync mỗi keystroke, khi user xóa cbo.Text thì sync sẽ không re-trigger được trừ khi xóa txt code rồi gõ lại
                EventHandler doSyncTextToCombo = (s2, e2) =>
                {
                    if (isSyncingIcd_v45072) return;
                    try
                    {
                        isSyncingIcd_v45072 = true;
                        var code = (txtCode.Text ?? string.Empty).Trim();
                        if (string.IsNullOrEmpty(code))
                        {
                            cbo.EditValue = null;
                            cbo.Text = string.Empty;
                            return;
                        }
                        // Multi-value: split, lookup từng code, ghép NAME vào cbo.Text
                        if (code.Contains(";") || code.Contains(","))
                        {
                            char sep = code.Contains(";") ? ';' : ',';
                            var codes = code.Split(sep);
                            var names = new List<string>();
                            foreach (var c in codes)
                            {
                                var t = (c ?? string.Empty).Trim();
                                if (string.IsNullOrEmpty(t)) { names.Add(string.Empty); continue; }
                                string foundName = string.Empty;
                                foreach (var item in dataSource)
                                {
                                    if (item == null) continue;
                                    var pCode = item.GetType().GetProperty(codeMember);
                                    var pName = item.GetType().GetProperty(nameMember);
                                    if (pCode == null || pName == null) continue;
                                    var ic = pCode.GetValue(item, null) as string;
                                    if (ic != null && ic.Equals(t, StringComparison.OrdinalIgnoreCase))
                                    {
                                        foundName = pName.GetValue(item, null) as string ?? string.Empty;
                                        break;
                                    }
                                }
                                names.Add(foundName);
                            }
                            cbo.EditValue = null;
                            cbo.Text = string.Join(sep.ToString(), names);
                            return;
                        }
                        // Single value: lookup case-INSENSITIVE, set EditValue đúng case DB
                        string matchedCode = null;
                        foreach (var item in dataSource)
                        {
                            if (item == null) continue;
                            var propInfo = item.GetType().GetProperty(codeMember);
                            if (propInfo == null) continue;
                            var itemCode = propInfo.GetValue(item, null) as string;
                            if (itemCode != null && itemCode.Equals(code, StringComparison.OrdinalIgnoreCase))
                            {
                                matchedCode = itemCode;
                                break;
                            }
                        }
                        cbo.EditValue = matchedCode != null ? (object)matchedCode : (object)code;
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                    finally { isSyncingIcd_v45072 = false; }
                };

                // Trigger sync khi nhấn ENTER
                txtCode.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        doSyncTextToCombo(s, e);
                        e.Handled = true;
                    }
                };
                // Trigger sync khi user rời ô (Tab/click ô khác)
                txtCode.Leave += (s, e) => doSyncTextToCombo(s, e);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Việc 45072 — Click row: lookup V_HIS_SERE_SERV_PTTT (đã load qua sp field _LoadData) +
        /// HIS_SERE_SERV_EXT, fill các control.
        /// </summary>
        private void FillExtendedDataWhenClickRow(SereServView1ADO row)
        {
            try
            {
                if (row == null) return;

                // 1. Load HIS_SERE_SERV_EXT theo SERE_SERV_ID
                HIS_SERE_SERV_EXT extData = LoadSereServExt_v45072(row.ID);
                currentSereServExt_v45072 = extData;
                currentSereServPttt_v45072 = this.sp; // sp loaded trong LoadDataPttt() ở _LoadData.cs (V_HIS_SERE_SERV_PTTT)

                // 2. Fill 4 ICD từ sp (V_HIS_SERE_SERV_PTTT) — adapt pattern Execute (FillDataToCboIcd / FillDataToCboIcdCm).
                if (this.sp != null)
                {
                    // CĐ chính (HIS_ICD) — verify code có trong cache rồi set
                    FillIcd_v45072(txtIcdCode_v45072, cboIcdName_v45072, this.sp.ICD_CODE, isIcdCm: false);
                    // Việc 45072 — CĐ phụ: multi-value, dùng .Text (vì TextEditStyle.Standard)
                    if (txtIcdSubCode_v45072 != null) txtIcdSubCode_v45072.Text = this.sp.ICD_SUB_CODE ?? "";
                    if (cboIcdText_v45072 != null) cboIcdText_v45072.Text = this.sp.ICD_TEXT ?? string.Empty;

                    // ICD9 chính (HIS_ICD_CM) — BUG FIX: dùng HIS_ICD_CM, KHÔNG phải HIS_ICD
                    FillIcd_v45072(txtIcdCmCode_v45072, cboIcdCmName_v45072, this.sp.ICD_CM_CODE, isIcdCm: true);
                    // Việc 45072 — ICD9 phụ: multi-value, dùng .Text
                    if (txtIcdCmSubCode_v45072 != null) txtIcdCmSubCode_v45072.Text = this.sp.ICD_CM_SUB_CODE ?? "";
                    if (cboIcdCmText_v45072 != null) cboIcdCmText_v45072.Text = this.sp.ICD_CM_TEXT ?? string.Empty;

                    if (cboEmotionLess_v45072 != null) cboEmotionLess_v45072.EditValue = this.sp.EMOTIONLESS_METHOD_ID;
                    // Việc 45072 — Cách thức (MANNER) — adapt pattern Execute (___Load.cs line 925/941):
                    //   Nếu sereServPTTT có MANNER → dùng MANNER
                    //   Ngược lại (chưa có record PTTT, hoặc record có nhưng MANNER null) → fallback TDL_SERVICE_NAME
                    if (txtManner_v45072 != null)
                        txtManner_v45072.Text = !string.IsNullOrWhiteSpace(this.sp.MANNER)
                            ? this.sp.MANNER
                            : (row.TDL_SERVICE_NAME ?? "");
                }
                else
                {
                    // Việc 45072 — Chưa có bản ghi PTTT (sp null) → DEFAULT ICD từ V_HIS_SERVICE_REQ (sere-serv cha).
                    // Pattern adapt từ SurgServiceReqExecute.SetIcdFromServiceReq: ICD chính + sub + text từ serviceReq.
                    // User chọn/sửa sau đó → SaveData_v45072 đọc từ control hiện tại nên thay đổi được lưu đúng.
                    ClearExtendedControls_v45072();
                    DefaultIcdFromServiceReq_v45072();
                    // Khi chưa có PTTT record → Cách thức mặc định = tên DV chỉ định (Execute pattern: sereServ.TDL_SERVICE_NAME)
                    if (txtManner_v45072 != null)
                        txtManner_v45072.Text = row.TDL_SERVICE_NAME ?? "";
                }

                // 3. Fill từ HIS_SERE_SERV_EXT
                if (extData != null)
                {
                    if (cboMachine_v45072 != null) cboMachine_v45072.EditValue = extData.MACHINE_ID;
                    if (txtConclude_v45072 != null) txtConclude_v45072.Text = extData.CONCLUDE ?? "";
                    if (txtInstructionNote_v45072 != null) txtInstructionNote_v45072.Text = extData.INSTRUCTION_NOTE ?? "";
                    if (txtDescription_v45072 != null) txtDescription_v45072.Text = extData.DESCRIPTION ?? "";
                    if (txtNote_v45072 != null) txtNote_v45072.Text = extData.NOTE ?? "";

                    // Fill grid BEGIN_TIME_STR / END_TIME_STR theo extData
                    row.BEGIN_TIME_STR = extData.BEGIN_TIME.HasValue
                        ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(extData.BEGIN_TIME.Value)
                        : "";
                    row.END_TIME_STR = extData.END_TIME.HasValue
                        ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(extData.END_TIME.Value)
                        : "";
                    if (gridControl1 != null) gridControl1.RefreshDataSource();
                }

                // 4. Begin/End time logic
                ApplyBeginEndTime_v45072(row, extData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Load HIS_SERE_SERV_EXT theo SERE_SERV_ID qua API.
        /// </summary>
        private HIS_SERE_SERV_EXT LoadSereServExt_v45072(long sereServId)
        {
            try
            {
                if (sereServId <= 0) return null;
                CommonParam param = new CommonParam();
                var filter = new HisSereServExtFilter();
                filter.SERE_SERV_ID = sereServId;
                var lst = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>(
                    HisRequestUriStore.MOSHIS_HIS_SERE_SERV_EXT_GET, ApiConsumers.MosConsumer, filter, param);
                if (lst != null && lst.Count > 0) return lst.First();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// Việc 45072 — Fill 1 cặp ICD (txt + cbo) — adapt pattern Execute (FillDataToCboIcd).
        /// Kiểm tra ICD_CODE tồn tại trong cache → set CẢ txt và cbo (đồng bộ).
        /// Nếu code không tồn tại → set txt = code raw, cbo = null.
        /// isIcdCm = true → lookup HIS_ICD_CM, false → lookup HIS_ICD.
        /// </summary>
        private void FillIcd_v45072(DevExpress.XtraEditors.TextEdit txtCode,
            DevExpress.XtraEditors.LookUpEdit cbo, string code, bool isIcdCm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    if (txtCode != null) txtCode.Text = "";
                    if (cbo != null) cbo.EditValue = null;
                    return;
                }
                code = code.Trim();
                if (isIcdCm)
                {
                    var icd = BackendDataWorker.Get<HIS_ICD_CM>()
                        .FirstOrDefault(o => o.ICD_CM_CODE == code);
                    if (icd != null)
                    {
                        if (txtCode != null) txtCode.Text = icd.ICD_CM_CODE;
                        if (cbo != null) cbo.EditValue = icd.ICD_CM_CODE;
                    }
                    else
                    {
                        if (txtCode != null) txtCode.Text = code;
                        if (cbo != null) cbo.EditValue = null;
                    }
                }
                else
                {
                    var icd = BackendDataWorker.Get<HIS_ICD>()
                        .FirstOrDefault(o => o.ICD_CODE == code);
                    if (icd != null)
                    {
                        if (txtCode != null) txtCode.Text = icd.ICD_CODE;
                        if (cbo != null) cbo.EditValue = icd.ICD_CODE;
                    }
                    else
                    {
                        if (txtCode != null) txtCode.Text = code;
                        if (cbo != null) cbo.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Default ICD từ V_HIS_SERVICE_REQ + HIS_SERVICE khi chưa có bản ghi PTTT.
        /// Adapt 2 pattern của SurgServiceReqExecute:
        ///   - SetIcdFromServiceReq: CĐ chính + CĐ phụ từ serviceReq.ICD_CODE/ICD_NAME/ICD_SUB_CODE/ICD_TEXT
        ///   - SetDefaultCboICD9CmChinh: ICD9-CM chính từ HIS_SERVICE.ICD_CM_ID (lookup theo currentRow.SERVICE_ID)
        /// V_HIS_SERVICE_REQ không có field ICD_CM_* — ICD9-CM phụ để trống.
        /// </summary>
        private void DefaultIcdFromServiceReq_v45072()
        {
            try
            {
                var sr = this.currentServiceReq_v45072;
                if (sr != null)
                {
                    // CĐ chính — lookup HIS_ICD theo CODE, set cả txt + cbo
                    FillIcd_v45072(txtIcdCode_v45072, cboIcdName_v45072, sr.ICD_CODE, isIcdCm: false);

                    // CĐ phụ — text raw (multi-value TextEditStyle=Standard)
                    if (txtIcdSubCode_v45072 != null) txtIcdSubCode_v45072.Text = sr.ICD_SUB_CODE ?? "";
                    if (cboIcdText_v45072 != null) cboIcdText_v45072.Text = sr.ICD_TEXT ?? string.Empty;
                }

                // ICD9-CM chính — lookup HIS_SERVICE.ICD_CM_ID theo SERVICE_ID của row
                if (this.currentRow != null && this.currentRow.SERVICE_ID > 0)
                {
                    var service = BackendDataWorker.Get<HIS_SERVICE>()
                        .FirstOrDefault(o => o.ID == this.currentRow.SERVICE_ID);
                    if (service != null && service.ICD_CM_ID.HasValue)
                    {
                        var icdCm = BackendDataWorker.Get<HIS_ICD_CM>()
                            .FirstOrDefault(o => o.ID == service.ICD_CM_ID.Value);
                        if (icdCm != null)
                        {
                            if (txtIcdCmCode_v45072 != null) txtIcdCmCode_v45072.Text = icdCm.ICD_CM_CODE;
                            if (cboIcdCmName_v45072 != null) cboIcdCmName_v45072.EditValue = icdCm.ICD_CM_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClearExtendedControls_v45072()
        {
            try
            {
                if (txtIcdCode_v45072 != null) txtIcdCode_v45072.Text = "";
                if (cboIcdName_v45072 != null) cboIcdName_v45072.EditValue = null;
                if (txtIcdSubCode_v45072 != null) txtIcdSubCode_v45072.Text = "";
                // Việc 45072 — CĐ phụ/ICD9 phụ: multi-value, clear bằng .Text
                if (cboIcdText_v45072 != null) cboIcdText_v45072.Text = string.Empty;
                if (txtIcdCmCode_v45072 != null) txtIcdCmCode_v45072.Text = "";
                if (cboIcdCmName_v45072 != null) cboIcdCmName_v45072.EditValue = null;
                if (txtIcdCmSubCode_v45072 != null) txtIcdCmSubCode_v45072.Text = "";
                if (cboIcdCmText_v45072 != null) cboIcdCmText_v45072.Text = string.Empty;
                if (cboEmotionLess_v45072 != null) cboEmotionLess_v45072.EditValue = null;
                if (txtManner_v45072 != null) txtManner_v45072.Text = "";
                if (cboMachine_v45072 != null) cboMachine_v45072.EditValue = null;
                if (txtConclude_v45072 != null) txtConclude_v45072.Text = "";
                if (txtInstructionNote_v45072 != null) txtInstructionNote_v45072.Text = "";
                if (txtDescription_v45072 != null) txtDescription_v45072.Text = "";
                if (txtNote_v45072 != null) txtNote_v45072.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Logic Begin/End time theo config TakeIntrucionTimeByServiceReq.
        /// </summary>
        private void ApplyBeginEndTime_v45072(SereServView1ADO row, HIS_SERE_SERV_EXT extData)
        {
            try
            {
                // BEGIN_TIME
                if (extData != null && extData.BEGIN_TIME.HasValue)
                {
                    var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(extData.BEGIN_TIME.Value);
                    if (dt.HasValue) dteStart.DateTime = dt.Value;
                }
                else
                {
                    string cfg = Config.HisConfigCFG.TakeIntrucionTimeByServiceReq;
                    bool isProc = false, isSurg = false;
                    if (row != null && row.SERVICE_ID > 0)
                    {
                        var svc = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == row.SERVICE_ID);
                        if (svc != null)
                        {
                            isProc = svc.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT;
                            isSurg = svc.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT;
                        }
                    }
                    if (cfg == "1" && isProc && row != null)
                    {
                        var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.INTRUCTION_TIME);
                        if (dt.HasValue) dteStart.DateTime = dt.Value;
                    }
                    else if (cfg == "2" && (isProc || isSurg) && row != null)
                    {
                        var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.INTRUCTION_TIME);
                        if (dt.HasValue) dteStart.DateTime = dt.Value;
                    }
                    else if (cfg == "3" && (isProc || isSurg))
                    {
                        dteStart.DateTime = DateTime.Now;
                    }
                }

                // END_TIME
                if (extData != null && extData.END_TIME.HasValue)
                {
                    var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(extData.END_TIME.Value);
                    if (dt.HasValue) dteFinish.DateTime = dt.Value;
                }
                else
                {
                    RecomputeDteFinish_v45072();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Tính dteFinish = dteStart + spnTimeProcess (phút).
        /// </summary>
        private void RecomputeDteFinish_v45072()
        {
            try
            {
                if (dteStart == null || dteStart.EditValue == null || dteStart.DateTime == DateTime.MinValue) return;
                if (spnTimeProcess_v45072 == null || spnTimeProcess_v45072.EditValue == null) return;
                decimal mins = 0;
                decimal.TryParse(spnTimeProcess_v45072.EditValue.ToString(), out mins);
                if (mins <= 0) return;
                dteFinish.DateTime = dteStart.DateTime.AddMinutes((double)mins);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SpnTimeProcess_v45072_EditValueChanged(object sender, EventArgs e)
        {
            try { RecomputeDteFinish_v45072(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void DteStart_v45072_EditValueChanged(object sender, EventArgs e)
        {
            try { RecomputeDteFinish_v45072(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #region Danh sách y lệnh (button btnDanhSachYLenh)
        /// <summary>
        /// Việc 45072 — Mở plugin HIS.Desktop.Plugins.ServiceReqList với treatmentId của row đang chọn.
        /// </summary>
        private void BtnDanhSachYLenh_v45072_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentRow == null || !currentRow.TDL_TREATMENT_ID.HasValue || currentRow.TDL_TREATMENT_ID <= 0)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.ChuaChonYLenh,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var targetModule = GlobalVariables.currentModuleRaws
                    .FirstOrDefault(o => o.ModuleLink == ModuleLinkString.ServiceReqList);
                if (targetModule == null || !targetModule.IsPlugin || targetModule.ExtensionInfo == null)
                {
                    XtraMessageBox.Show(
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay),
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var treatment = new HIS_TREATMENT { ID = currentRow.TDL_TREATMENT_ID ?? 0 };

                var listArgs = new List<object>();
                listArgs.Add(treatment);

                var instance = PluginInstance.GetPluginInstance(
                    PluginInstance.GetModuleWithWorkingRoom(targetModule, this.moduleData.RoomId, this.moduleData.RoomTypeId),
                    listArgs);

                if (instance is Form)
                    ((Form)instance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Việc 45072 — Helper Build SDO (gọi từ btnSave_Click trong _Right.cs)

        /// <summary>
        /// Việc 45072 — Bổ sung 4 ICD + EMOTIONLESS_METHOD_ID + MANNER vào HIS_SERE_SERV_PTTT.
        /// PHẢI gọi SAU khi đã set các field cũ (PTTT_GROUP_ID, PTTT_METHOD_ID, ...).
        /// </summary>
        internal void FillPtttFields_v45072(HIS_SERE_SERV_PTTT pttt)
        {
            try
            {
                if (pttt == null || currentRow == null) return;
                // Việc 45072 — BUG FIX: COPY field cũ từ V_HIS_SERE_SERV_PTTT (this.sp) sang entity mới
                // Nếu không map, các field DB cũ (như PCI, STENTING, EYE, SKIN...) sẽ NULL → BE update null → mất data.
                // Pattern Execute (ProcessSereServPttt line 568-572) — bắt buộc.
                if (this.sp != null)
                {
                    try { Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_PTTT>(pttt, this.sp); }
                    catch (Exception exMap) { Inventec.Common.Logging.LogSystem.Warn(exMap); }
                }
                pttt.SERE_SERV_ID = currentRow.ID;
                pttt.TDL_TREATMENT_ID = currentRow.TDL_TREATMENT_ID;

                // ICD chính — adapt pattern Execute (Plus_ICD.ChangecboChanDoanTD):
                //   txtIcdCode = ICD_CODE, cbo.EditValue chứa ICD_CODE (string), ICD_NAME lookup HIS_ICD theo ICD_CODE.
                // Việc 45072 — BUG FIX: PHẢI explicit clear khi text rỗng (TuanLN báo: ICD chính bị mất sau save).
                // Vì MAP from sp ở line đầu set ICD_CODE/ICD_NAME từ DB cũ → nếu không clear, save sẽ giữ data cũ.
                if (txtIcdCode_v45072 != null && !string.IsNullOrWhiteSpace(txtIcdCode_v45072.Text))
                {
                    string code = txtIcdCode_v45072.Text.Trim().ToUpper();
                    var icd = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE == code);
                    if (icd != null)
                    {
                        pttt.ICD_CODE = icd.ICD_CODE;
                        pttt.ICD_NAME = icd.ICD_NAME;
                    }
                    else
                    {
                        pttt.ICD_CODE = code;
                        pttt.ICD_NAME = null;   // Clear NAME nếu code không tìm thấy
                    }
                }
                else
                {
                    pttt.ICD_CODE = null;       // Explicit clear khi user xóa text
                    pttt.ICD_NAME = null;
                }
                // Việc 45072 — CĐ phụ: multi-value (codes cách ';'), lưu trực tiếp từ .Text — KHÔNG lookup HIS_ICD
                if (txtIcdSubCode_v45072 != null) pttt.ICD_SUB_CODE = (txtIcdSubCode_v45072.Text ?? "").Trim();
                if (cboIcdText_v45072 != null) pttt.ICD_TEXT = (cboIcdText_v45072.Text ?? string.Empty).Trim();

                // ICD9 chính (phẫu thuật thủ thuật) — BUG FIX: phải lookup HIS_ICD_CM, KHÔNG phải HIS_ICD.
                // Adapt pattern Execute (ChangecboIcdCmTD): cbo.EditValue chứa ICD_CM_CODE (string), ICD_CM_NAME lookup HIS_ICD_CM.
                // Việc 45072 — BUG FIX: explicit clear khi text rỗng.
                if (txtIcdCmCode_v45072 != null && !string.IsNullOrWhiteSpace(txtIcdCmCode_v45072.Text))
                {
                    string codeCm = txtIcdCmCode_v45072.Text.Trim().ToUpper();
                    var icdCm = BackendDataWorker.Get<HIS_ICD_CM>().FirstOrDefault(o => o.ICD_CM_CODE == codeCm);
                    if (icdCm != null)
                    {
                        pttt.ICD_CM_CODE = icdCm.ICD_CM_CODE;
                        pttt.ICD_CM_NAME = icdCm.ICD_CM_NAME;
                    }
                    else
                    {
                        pttt.ICD_CM_CODE = codeCm;
                        pttt.ICD_CM_NAME = null;
                    }
                }
                else
                {
                    pttt.ICD_CM_CODE = null;
                    pttt.ICD_CM_NAME = null;
                }
                // Việc 45072 — ICD9 phụ: multi-value, lưu trực tiếp từ .Text — KHÔNG lookup HIS_ICD_CM
                if (txtIcdCmSubCode_v45072 != null) pttt.ICD_CM_SUB_CODE = (txtIcdCmSubCode_v45072.Text ?? "").Trim();
                if (cboIcdCmText_v45072 != null) pttt.ICD_CM_TEXT = (cboIcdCmText_v45072.Text ?? string.Empty).Trim();

                // Vô cảm chính (EMOTIONLESS_METHOD_ID — khác với cboEmotionLessMethod đã set ở field cũ)
                // Việc 45072 — BUG FIX: explicit clear khi user clear combo
                if (cboEmotionLess_v45072 != null && cboEmotionLess_v45072.EditValue != null)
                {
                    long emoId;
                    if (long.TryParse(cboEmotionLess_v45072.EditValue.ToString(), out emoId))
                        pttt.EMOTIONLESS_METHOD_ID = emoId;
                    else
                        pttt.EMOTIONLESS_METHOD_ID = null;
                }
                else
                {
                    pttt.EMOTIONLESS_METHOD_ID = null;
                }
                if (txtManner_v45072 != null) pttt.MANNER = (txtManner_v45072.Text ?? "").Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — Bổ sung MACHINE_ID/CODE, INSTRUCTION_NOTE, CONCLUDE, DESCRIPTION, NOTE vào HIS_SERE_SERV_EXT.
        /// PHẢI gọi SAU khi đã set BEGIN_TIME / END_TIME / SERE_SERV_ID.
        /// </summary>
        internal void FillExtFields_v45072(HIS_SERE_SERV_EXT ext)
        {
            try
            {
                if (ext == null || currentRow == null) return;
                ext.SERE_SERV_ID = currentRow.ID;

                if (cboMachine_v45072 != null && cboMachine_v45072.EditValue != null)
                {
                    long mId;
                    if (long.TryParse(cboMachine_v45072.EditValue.ToString(), out mId))
                    {
                        ext.MACHINE_ID = mId;
                        var machine = BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == mId);
                        if (machine != null) ext.MACHINE_CODE = machine.MACHINE_CODE;
                    }
                }
                if (txtInstructionNote_v45072 != null) ext.INSTRUCTION_NOTE = (txtInstructionNote_v45072.Text ?? "").Trim();
                if (txtConclude_v45072 != null) ext.CONCLUDE = (txtConclude_v45072.Text ?? "").Trim();
                if (txtDescription_v45072 != null) ext.DESCRIPTION = (txtDescription_v45072.Text ?? "").Trim();
                if (txtNote_v45072 != null) ext.NOTE = (txtNote_v45072.Text ?? "").Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Việc 45072 — IsFinished tính theo:
        ///  - Nếu config ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR == "1" && user không phải BS → false
        ///  - Ngược lại: (chkKT_v45072.Checked && dteFinish có giá trị)
        /// PURE function — KHÔNG show UI. Caller (btnSave_Click) tự show cảnh báo qua CheckCanFinishByDoctorRole_v45072.
        /// </summary>
        internal bool ComputeIsFinished_v45072()
        {
            try
            {
                bool chkOk = chkKT_v45072 != null && chkKT_v45072.Checked;
                bool dtOk = dteFinish != null && dteFinish.EditValue != null && dteFinish.DateTime != DateTime.MinValue;
                if (!chkOk || !dtOk) return false;

                // Nếu config bật: chỉ cho phép finish khi user là BS
                string allow = Config.HisConfigCFG.AllowFinishWhenAccountIsDoctor;
                if (allow == "1" && !IsCurrentUserDoctor_v45072())
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Việc 45072 — Kiểm tra user hiện tại có phải bác sĩ không (IS_DOCTOR=1).
        /// </summary>
        internal bool IsCurrentUserDoctor_v45072()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var emp = BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == loginName);
                return (emp != null && emp.IS_DOCTOR.HasValue && emp.IS_DOCTOR.Value == 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Việc 45072 — Kiểm tra điều kiện finish theo role bác sĩ. Gọi từ btnSave_Click TRƯỚC khi save.
        /// Trả về true nếu OK tiếp tục save; false nếu user check KT nhưng không có quyền BS — hiển thị cảnh báo, dừng save.
        /// </summary>
        internal bool CheckCanFinishByDoctorRole_v45072()
        {
            try
            {
                if (chkKT_v45072 == null || !chkKT_v45072.Checked) return true; // Không check KT → không cần check role
                string allow = Config.HisConfigCFG.AllowFinishWhenAccountIsDoctor;
                if (allow != "1") return true;
                if (IsCurrentUserDoctor_v45072()) return true;

                // User check KT nhưng không phải BS → cảnh báo + dừng
                XtraMessageBox.Show(
                    Resources.ResourceMessage.TaiKhoanKhongPhaiBacSi,
                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true; // Lỗi check → cho phép save (fail-safe)
            }
        }

        #endregion

        #region ControlState cho chkKT_v45072

        /// <summary>
        /// Việc 45072 — Khởi tạo ControlState: đọc state đã lưu cho chkKT_v45072.
        /// </summary>
        private void InitControlState_v45072()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData(moduleLink_v45072);
                if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                {
                    foreach (var item in currentControlStateRDO)
                    {
                        if (chkKT_v45072 != null && item.KEY == chkKT_v45072.Name)
                            chkKT_v45072.Checked = item.VALUE == "1";
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                isNotLoadWhileChangeControlStateInFirst = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChkKT_v45072_CheckedChanged(object sender, EventArgs e)
        {
            if (isNotLoadWhileChangeControlStateInFirst) return;
            try
            {
                if (chkKT_v45072 == null) return;
                var item = currentControlStateRDO != null
                    ? currentControlStateRDO.FirstOrDefault(o => o.KEY == chkKT_v45072.Name && o.MODULE_LINK == moduleLink_v45072)
                    : null;
                if (item != null)
                {
                    item.VALUE = chkKT_v45072.Checked ? "1" : "";
                }
                else
                {
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = chkKT_v45072.Name,
                        MODULE_LINK = moduleLink_v45072,
                        VALUE = chkKT_v45072.Checked ? "1" : ""
                    });
                }
                if (controlStateWorker != null)
                    controlStateWorker.SetData(currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}

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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    /// <summary>
    /// Form cấu hình màn chờ thu ngân 3 (mở từ menu phòng thu ngân): điều kiện lọc tivi, làm mới, tùy chỉnh hiển thị,
    /// nút Lưu (áp dụng ngay cho tivi đang mở) và nút Mở / Đóng màn hình trên màn hình mở rộng.
    /// </summary>
    public partial class frmConfigCashierV3 : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module module;
        private V_HIS_ROOM room;
        private string cashierRoomName;
        private readonly ConfigStateStore store = new ConfigStateStore();
        private WaitingScreenConfigADO cfg;
        private bool isInit = true;

        public frmConfigCashierV3(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.module = module;
        }

        private void frmConfigCashierV3_Load(object sender, EventArgs e)
        {
            try
            {
                this.isInit = true;
                this.room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.module.RoomId);
                this.cashierRoomName = ResolveCashierRoomName(this.room);
                lblRoomValue.Text = this.cashierRoomName;

                string departmentName = this.room != null ? this.room.DEPARTMENT_NAME : "";
                long? departmentId = this.room != null ? (long?)this.room.DEPARTMENT_ID : null;
                this.cfg = this.store.Load(departmentName, departmentId);
                LogSystem.Info(string.Format(
                    "CallPatientCashierV3 | Mo form cau hinh: RoomId={0}, ROOM_CODE={1}, phong tim thay={2}, DEPARTMENT_ID cua phong={3}, cfg: AlwaysToday={4}, InDate {5}-{6}, DepartmentIds=[{7}], TreatmentTypeIds=[{8}], PatientTypeIds=[{9}], DebtStatus={10}, ServiceGroupKeys=[{11}], SplitCdha={12}, RefreshSeconds={13}",
                    this.module.RoomId, this.room != null ? this.room.ROOM_CODE : "", this.room != null, departmentId,
                    this.cfg.AlwaysToday, this.cfg.InDateFrom, this.cfg.InDateTo,
                    string.Join(",", this.cfg.DepartmentIds), string.Join(",", this.cfg.TreatmentTypeIds), string.Join(",", this.cfg.PatientTypeIds),
                    this.cfg.DebtStatus, string.Join(",", this.cfg.ServiceGroupKeys), this.cfg.SplitCdhaByExecuteGroup, this.cfg.RefreshSeconds));

                LoadCatalogs(this.cfg.SplitCdhaByExecuteGroup);
                FillControls(this.cfg);
                UpdateOpenButtonText();
                this.isInit = false;

                if (this.cfg.AutoOpen && FindOpenScreen() == null)
                {
                    OpenScreen(this.cfg);
                }
            }
            catch (Exception ex)
            {
                this.isInit = false;
                LogSystem.Error(ex);
            }
        }

        internal static string ResolveCashierRoomName(V_HIS_ROOM room)
        {
            try
            {
                if (room == null) return "";
                var cashierRooms = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>();
                V_HIS_CASHIER_ROOM cashierRoom = cashierRooms != null ? cashierRooms.FirstOrDefault(o => o.ROOM_ID == room.ID) : null;
                if (cashierRoom != null && !string.IsNullOrWhiteSpace(cashierRoom.CASHIER_ROOM_NAME))
                {
                    return cashierRoom.CASHIER_ROOM_NAME;
                }
                return room.ROOM_NAME ?? "";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return room != null ? (room.ROOM_NAME ?? "") : "";
            }
        }

        #region Danh mục cho các ô chọn

        private void LoadCatalogs(bool splitCdha)
        {
            try
            {
                cboDepartments.Properties.Items.Clear();
                var departments = BackendDataWorker.Get<HIS_DEPARTMENT>();
                if (departments != null)
                {
                    foreach (var d in departments.Where(o => o.IS_ACTIVE == 1).OrderBy(o => o.DEPARTMENT_NAME))
                    {
                        cboDepartments.Properties.Items.Add(d.ID.ToString(), d.DEPARTMENT_NAME, CheckState.Unchecked, true);
                    }
                }

                cboTreatmentTypes.Properties.Items.Clear();
                var treatmentTypes = BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                if (treatmentTypes != null)
                {
                    foreach (var t in treatmentTypes.Where(o => o.IS_ACTIVE == 1).OrderBy(o => o.ID))
                    {
                        cboTreatmentTypes.Properties.Items.Add(t.ID.ToString(), t.TREATMENT_TYPE_NAME, CheckState.Unchecked, true);
                    }
                }

                cboPatientTypes.Properties.Items.Clear();
                var patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (patientTypes != null)
                {
                    foreach (var p in patientTypes.Where(o => o.IS_ACTIVE == 1).OrderBy(o => o.PATIENT_TYPE_NAME))
                    {
                        cboPatientTypes.Properties.Items.Add(p.ID.ToString(), p.PATIENT_TYPE_NAME, CheckState.Unchecked, true);
                    }
                }

                ReloadServiceGroupItems(splitCdha);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ReloadServiceGroupItems(bool splitCdha)
        {
            try
            {
                cboServiceGroups.Properties.Items.Clear();
                foreach (var g in ServiceGroupCatalog.GetFilterItems(splitCdha))
                {
                    cboServiceGroups.Properties.Items.Add(g.Key, g.Name, CheckState.Unchecked, true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private static void SetCheckedValues(CheckedComboBoxEdit cbo, IEnumerable<string> values)
        {
            HashSet<string> set = new HashSet<string>(values ?? new string[0]);
            foreach (CheckedListBoxItem item in cbo.Properties.Items)
            {
                item.CheckState = (item.Value != null && set.Contains(item.Value.ToString())) ? CheckState.Checked : CheckState.Unchecked;
            }
            cbo.RefreshEditValue();
        }

        private static List<string> GetCheckedValues(CheckedComboBoxEdit cbo)
        {
            List<string> result = new List<string>();
            foreach (CheckedListBoxItem item in cbo.Properties.Items)
            {
                if (item.CheckState == CheckState.Checked && item.Value != null)
                {
                    result.Add(item.Value.ToString());
                }
            }
            return result;
        }

        private static List<long> ToIds(IEnumerable<string> values)
        {
            List<long> result = new List<long>();
            foreach (string v in values)
            {
                long id;
                if (long.TryParse(v, out id)) result.Add(id);
            }
            return result;
        }

        #endregion

        #region Đổ / đọc control

        private void FillControls(WaitingScreenConfigADO c)
        {
            try
            {
                txtDepartmentTitle.Text = c.DepartmentTitle ?? "";
                chkAlwaysToday.Checked = c.AlwaysToday;
                dtFrom.DateTime = c.InDateFrom.HasValue ? ParseDate(c.InDateFrom.Value) : DateTime.Now.Date;
                dtTo.DateTime = c.InDateTo.HasValue ? ParseDate(c.InDateTo.Value) : DateTime.Now.Date;
                dtFrom.Enabled = !c.AlwaysToday;
                dtTo.Enabled = !c.AlwaysToday;

                SetCheckedValues(cboDepartments, c.DepartmentIds.Select(o => o.ToString()));
                SetCheckedValues(cboTreatmentTypes, c.TreatmentTypeIds.Select(o => o.ToString()));
                SetCheckedValues(cboPatientTypes, c.PatientTypeIds.Select(o => o.ToString()));
                cboDebtStatus.SelectedIndex = (c.DebtStatus >= 0 && c.DebtStatus <= 2) ? c.DebtStatus : 0;
                chkSplitCdha.Checked = c.SplitCdhaByExecuteGroup;
                SetCheckedValues(cboServiceGroups, c.ServiceGroupKeys);

                spnRefresh.Value = c.RefreshSeconds;
                spnMaxRows.Value = c.MaxRows;
                spnPageSeconds.Value = c.PageSeconds;

                spnTitleFont.Value = (decimal)c.TitleFontSize;
                spnListFont.Value = (decimal)c.ListFontSize;
                spnCallingFont.Value = (decimal)c.CallingFontSize;
                clrTitle.Color = ParseColor(c.TitleColor, WaitingScreenConfigADO.DEFAULT_TITLE_COLOR);
                clrList.Color = ParseColor(c.ListColor, WaitingScreenConfigADO.DEFAULT_LIST_COLOR);
                clrCalling.Color = ParseColor(c.CallingColor, WaitingScreenConfigADO.DEFAULT_CALLING_COLOR);
                clrBack.Color = ParseColor(c.BackColor, WaitingScreenConfigADO.DEFAULT_BACK_COLOR);
                FillExtraColors(c);
                chkExtendMonitor.Checked = c.ShowOnExtendMonitor;
                chkAutoOpen.Checked = c.AutoOpen;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private WaitingScreenConfigADO ReadControls()
        {
            WaitingScreenConfigADO c = new WaitingScreenConfigADO();
            c.DepartmentTitle = (txtDepartmentTitle.Text ?? "").Trim();
            c.AlwaysToday = chkAlwaysToday.Checked;
            c.InDateFrom = dtFrom.EditValue != null ? (long?)long.Parse(dtFrom.DateTime.ToString("yyyyMMdd") + "000000") : null;
            c.InDateTo = dtTo.EditValue != null ? (long?)long.Parse(dtTo.DateTime.ToString("yyyyMMdd") + "235959") : null;
            c.DepartmentIds = ToIds(GetCheckedValues(cboDepartments));
            c.TreatmentTypeIds = ToIds(GetCheckedValues(cboTreatmentTypes));
            c.PatientTypeIds = ToIds(GetCheckedValues(cboPatientTypes));
            c.DebtStatus = cboDebtStatus.SelectedIndex < 0 ? 0 : cboDebtStatus.SelectedIndex;
            c.ServiceGroupKeys = GetCheckedValues(cboServiceGroups);
            c.SplitCdhaByExecuteGroup = chkSplitCdha.Checked;
            c.RefreshSeconds = (int)spnRefresh.Value;
            c.MaxRows = (int)spnMaxRows.Value;
            c.PageSeconds = (int)spnPageSeconds.Value;
            c.TitleFontSize = (float)spnTitleFont.Value;
            c.ListFontSize = (float)spnListFont.Value;
            c.CallingFontSize = (float)spnCallingFont.Value;
            c.TitleColor = ColorToHtml(clrTitle.Color, WaitingScreenConfigADO.DEFAULT_TITLE_COLOR);
            c.ListColor = ColorToHtml(clrList.Color, WaitingScreenConfigADO.DEFAULT_LIST_COLOR);
            c.CallingColor = ColorToHtml(clrCalling.Color, WaitingScreenConfigADO.DEFAULT_CALLING_COLOR);
            c.BackColor = ColorToHtml(clrBack.Color, WaitingScreenConfigADO.DEFAULT_BACK_COLOR);
            c.ListTitleBackColor = ColorToHtml(clrListTitleBack.Color, WaitingScreenConfigADO.DEFAULT_LIST_TITLE_BACK_COLOR);
            c.ColumnHeaderBackColor = ColorToHtml(clrColumnHeaderBack.Color, WaitingScreenConfigADO.DEFAULT_COLUMN_HEADER_BACK_COLOR);
            c.RowAltBackColor = ColorToHtml(clrRowAltBack.Color, WaitingScreenConfigADO.DEFAULT_ROW_ALT_BACK_COLOR);
            c.CallBarBackColor = ColorToHtml(clrCallBarBack.Color, WaitingScreenConfigADO.DEFAULT_CALL_BAR_BACK_COLOR);
            c.CallBarBackColor2 = ColorToHtml(clrCallBarBack2.Color, WaitingScreenConfigADO.DEFAULT_CALL_BAR_BACK_COLOR2);
            c.CallTitleColor = ColorToHtml(clrCallTitle.Color, WaitingScreenConfigADO.DEFAULT_CALL_TITLE_COLOR);
            c.CallRoomColor = ColorToHtml(clrCallRoom.Color, WaitingScreenConfigADO.DEFAULT_CALL_ROOM_COLOR);
            c.ShowOnExtendMonitor = chkExtendMonitor.Checked;
            c.AutoOpen = chkAutoOpen.Checked;
            c.DisplayVersion = WaitingScreenConfigADO.CURRENT_DISPLAY_VERSION;
            c.FilterVersion = WaitingScreenConfigADO.CURRENT_FILTER_VERSION;
            c.Normalize();
            return c;
        }

        private static DateTime ParseDate(long value)
        {
            try
            {
                string s = value.ToString();
                if (s.Length >= 8)
                {
                    return new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)), int.Parse(s.Substring(6, 2)));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return DateTime.Now.Date;
        }

        /// <summary>Màu thẻ "Danh sách chờ" và thanh "Đang gọi".</summary>
        private void FillExtraColors(WaitingScreenConfigADO c)
        {
            clrListTitleBack.Color = ParseColor(c.ListTitleBackColor, WaitingScreenConfigADO.DEFAULT_LIST_TITLE_BACK_COLOR);
            clrColumnHeaderBack.Color = ParseColor(c.ColumnHeaderBackColor, WaitingScreenConfigADO.DEFAULT_COLUMN_HEADER_BACK_COLOR);
            clrRowAltBack.Color = ParseColor(c.RowAltBackColor, WaitingScreenConfigADO.DEFAULT_ROW_ALT_BACK_COLOR);
            clrCallBarBack.Color = ParseColor(c.CallBarBackColor, WaitingScreenConfigADO.DEFAULT_CALL_BAR_BACK_COLOR);
            clrCallBarBack2.Color = ParseColor(c.CallBarBackColor2, WaitingScreenConfigADO.DEFAULT_CALL_BAR_BACK_COLOR2);
            clrCallTitle.Color = ParseColor(c.CallTitleColor, WaitingScreenConfigADO.DEFAULT_CALL_TITLE_COLOR);
            clrCallRoom.Color = ParseColor(c.CallRoomColor, WaitingScreenConfigADO.DEFAULT_CALL_ROOM_COLOR);
        }

        /// <summary>ColorPickEdit chưa chọn trả về màu rỗng / trong suốt → dùng mặc định thay vì lưu chuỗi rỗng.</summary>
        private static string ColorToHtml(Color color, string fallback)
        {
            try
            {
                if (color.IsEmpty || color.A == 0) return fallback;
                return ColorTranslator.ToHtml(Color.FromArgb(color.R, color.G, color.B));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return fallback;
            }
        }

        internal static Color ParseColor(string html, string fallback)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(html)) return ColorTranslator.FromHtml(html);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return ColorTranslator.FromHtml(fallback);
        }

        #endregion

        #region Sự kiện

        private void chkAlwaysToday_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                dtFrom.Enabled = !chkAlwaysToday.Checked;
                dtTo.Enabled = !chkAlwaysToday.Checked;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void chkSplitCdha_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit) return;
                List<string> selected = GetCheckedValues(cboServiceGroups);
                ReloadServiceGroupItems(chkSplitCdha.Checked);
                SetCheckedValues(cboServiceGroups, selected);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnResetDisplay_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingScreenConfigADO c = ReadControls();
                c.ResetDisplayToDefault();
                spnTitleFont.Value = (decimal)c.TitleFontSize;
                spnListFont.Value = (decimal)c.ListFontSize;
                spnCallingFont.Value = (decimal)c.CallingFontSize;
                clrTitle.Color = ParseColor(c.TitleColor, WaitingScreenConfigADO.DEFAULT_TITLE_COLOR);
                clrList.Color = ParseColor(c.ListColor, WaitingScreenConfigADO.DEFAULT_LIST_COLOR);
                clrCalling.Color = ParseColor(c.CallingColor, WaitingScreenConfigADO.DEFAULT_CALLING_COLOR);
                clrBack.Color = ParseColor(c.BackColor, WaitingScreenConfigADO.DEFAULT_BACK_COLOR);
                FillExtraColors(c);
                lblStatus.Text = "Đã đưa cỡ chữ, màu về mặc định theo mẫu viện (bấm Lưu để áp dụng)";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private bool ValidateBeforeSave(WaitingScreenConfigADO c)
        {
            if (!c.AlwaysToday && c.InDateFrom.HasValue && c.InDateTo.HasValue && c.InDateFrom.Value > c.InDateTo.Value)
            {
                XtraMessageBox.Show("Ngày vào từ phải nhỏ hơn hoặc bằng ngày vào đến", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            // Không lọc khoa là mặc định (08/10/2026) nên không hỏi xác nhận khi để trống ô Khoa
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingScreenConfigADO c = ReadControls();
                if (!ValidateBeforeSave(c)) return;
                if (!this.store.Save(c))
                {
                    XtraMessageBox.Show("Lưu cấu hình thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.cfg = c;
                frmWaitingScreenCashierV3 screen = FindOpenScreen();
                if (screen != null)
                {
                    screen.ApplyConfig(c);
                }
                lblStatus.Text = string.Format("Đã lưu lúc {0:HH:mm:ss}{1}", DateTime.Now, screen != null ? " — tivi đã áp dụng" : "");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                frmWaitingScreenCashierV3 screen = FindOpenScreen();
                if (screen != null)
                {
                    screen.Close();
                    UpdateOpenButtonText();
                    return;
                }
                WaitingScreenConfigADO c = ReadControls();
                if (!ValidateBeforeSave(c)) return;
                this.store.Save(c);
                this.cfg = c;
                OpenScreen(c);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region Mở / đóng tivi

        private static frmWaitingScreenCashierV3 FindOpenScreen()
        {
            try
            {
                return Application.OpenForms.OfType<frmWaitingScreenCashierV3>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }

        private void OpenScreen(WaitingScreenConfigADO c)
        {
            try
            {
                frmWaitingScreenCashierV3 frm = new frmWaitingScreenCashierV3(this.module, this.room, this.cashierRoomName, c);
                frm.FormClosed += delegate(object s, FormClosedEventArgs args) { UpdateOpenButtonText(); };
                if (c.ShowOnExtendMonitor)
                {
                    ShowFormProcessor.ShowFormInExtendMonitor(frm);
                }
                else
                {
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.Size = new Size(1280, 720);
                    frm.Show();
                }
                UpdateOpenButtonText();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void UpdateOpenButtonText()
        {
            try
            {
                if (this.IsDisposed) return;
                btnOpen.Text = FindOpenScreen() != null ? "Đóng màn hình" : "Mở màn hình";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}

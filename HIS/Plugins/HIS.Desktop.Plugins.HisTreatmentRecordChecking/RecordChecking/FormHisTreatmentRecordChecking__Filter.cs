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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    /// <summary>
    /// Task 53180 - filter by ordering doctor.
    /// The screen has two working modes, driven solely by cboRequestDoctor:
    ///   empty    -> mode 1, review a single record by its code (unchanged behaviour)
    ///   selected -> mode 2, review every order of that doctor across records
    /// See rules QT-01 .. QT-08 in the business document.
    /// </summary>
    public partial class FormHisTreatmentRecordChecking
    {
        #region Declare

        /// <summary>Value of cboTreatmentStatus meaning the record has NOT been finished yet.</summary>
        private const long TREATMENT_STATUS__NOT_FINISHED = 0;

        /// <summary>Value of cboTreatmentStatus meaning the record HAS been finished.</summary>
        private const long TREATMENT_STATUS__FINISHED = 1;

        /// <summary>Maximum number of days allowed for one query (QT-04).</summary>
        private const int MAX_FILTER_DAYS = 31;

        /// <summary>Employees offered by cboRequestDoctor.</summary>
        private List<HIS_EMPLOYEE> listRequestDoctor;

        /// <summary>Shows validation errors on the filter editors.</summary>
        private DXErrorProvider dxErrorProviderFilter = new DXErrorProvider();

        #endregion

        /// <summary>
        /// True when the user picked a doctor, i.e. mode 2 (QT-01).
        /// </summary>
        private bool IsFilterByDoctorMode()
        {
            try
            {
                return cboRequestDoctor.EditValue != null
                    && !string.IsNullOrWhiteSpace(cboRequestDoctor.EditValue.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>Login name of the selected doctor, empty in mode 1.</summary>
        private string GetSelectedDoctorLoginName()
        {
            try
            {
                if (IsFilterByDoctorMode()) return cboRequestDoctor.EditValue.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>
        /// Loads the doctor combo from the local cache - no API call.
        /// </summary>
        private void InitComboRequestDoctor()
        {
            try
            {
                // TDL_USERNAME is the display name denormalised from ACS_USER;
                // HIS_EMPLOYEE itself has no name column.
                listRequestDoctor = BackendDataWorker.Get<HIS_EMPLOYEE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                             && !string.IsNullOrWhiteSpace(o.LOGINNAME))
                    .OrderBy(o => o.TDL_USERNAME)
                    .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", GetLangValue("ComboColumn.LoginName"), 110, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", GetLangValue("ComboColumn.EmployeeName"), 200, 2));

                ControlEditorADO controlEditorADO =
                    new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 330);
                ControlEditorLoader.Load(cboRequestDoctor, listRequestDoctor, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Loads the two fixed options of the record status combo (QT-06).
        /// </summary>
        private void InitComboTreatmentStatus()
        {
            try
            {
                List<TreatmentStatusADO> data = new List<TreatmentStatusADO>();
                data.Add(new TreatmentStatusADO(TREATMENT_STATUS__NOT_FINISHED, GetLangValue("TreatmentStatus.NotFinished")));
                data.Add(new TreatmentStatusADO(TREATMENT_STATUS__FINISHED, GetLangValue("TreatmentStatus.Finished")));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 150, 1));

                ControlEditorADO controlEditorADO =
                    new ControlEditorADO("NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboTreatmentStatus, data, controlEditorADO);

                cboTreatmentStatus.EditValue = TREATMENT_STATUS__NOT_FINISHED;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Default period: first day of the current month .. today.
        /// </summary>
        private void SetDefaultFilterValue()
        {
            try
            {
                DateTime today = DateTime.Now;
                dtFromDate.DateTime = new DateTime(today.Year, today.Month, 1);
                dtToDate.DateTime = today;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Enables / disables the editors according to the current mode.
        /// Mode 2 marks the mandatory captions in maroon and locks the approval buttons,
        /// because closing a record stays a mode 1 action.
        /// </summary>
        private void ApplyModeUI()
        {
            try
            {
                bool byDoctor = IsFilterByDoctorMode();

                TxtTreatmentCode.Enabled = !byDoctor;

                dtFromDate.Enabled = byDoctor;
                dtToDate.Enabled = byDoctor;
                cboTreatmentStatus.Enabled = byDoctor;

                Color captionColor = byDoctor ? Color.Maroon : Color.Empty;
                SetRequiredCaption(lciFromDate, byDoctor, captionColor);
                SetRequiredCaption(lciToDate, byDoctor, captionColor);
                SetRequiredCaption(lciTreatmentStatus, byDoctor, captionColor);

                // Patient columns only make sense when several records are listed at once.
                Gv_IR_PatientCode.Visible = byDoctor;
                Gv_IR_PatientName.Visible = byDoctor;
                Gv_IR_TreatmentCode.Visible = byDoctor;

                // Mode 1 shows one record, so there is nothing to page through.
                lciPaging.Visibility = byDoctor
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                // Vung thong tin benh nhan chi co nghia khi dang xem MOT ho so.
                LcgPatientInfo.Enabled = !byDoctor;

                if (byDoctor)
                {
                    btnDat.Enabled = false;
                    btnKhongDat.Enabled = false;
                    btnDuyet.Enabled = false;
                    btnHuyDuyet.Enabled = false;
                    SetDefaultValueControl();
                }
                else
                {
                    BackToTreatmentMode();
                }

                ResetFilterValidation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dua man hinh ve Cach 1 sau khi xoa o Bac si chi dinh.
        /// Bo du lieu cua Cach 2 con dong lai trong bo nho va tren 3 luoi, tra con tro ve o Ma ho so
        /// de nguoi dung go/quet ma tim tiep ngay - truoc day khong lam nen nhin nhu "khong tim lai duoc".
        /// </summary>
        private void BackToTreatmentMode()
        {
            try
            {
                ListDataInfoRecord = new List<ADO.InfoRecordADO>();
                CurrentDataInfoRecord = new List<ADO.InfoRecordADO>();
                CurrentInfoRecord = new List<ADO.InfoRecordADO>();
                ListDocument = new List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>();

                SetDefaultValueControl();

                // Thanh phan trang da duoc an bang lciPaging.Visibility o ApplyModeUI.
                // KHONG set ucPaging.Visible = false: LayoutControl se de lai o trong khi hien lai.

                TxtTreatmentCode.Focus();
                TxtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Paints the caption of a mandatory editor.</summary>
        private void SetRequiredCaption(DevExpress.XtraLayout.LayoutControlItem item, bool isRequired, Color color)
        {
            try
            {
                if (item == null) return;
                item.AppearanceItemCaption.ForeColor = color;
                item.AppearanceItemCaption.Options.UseForeColor = isRequired;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Clears every validation marker of the filter row.</summary>
        private void ResetFilterValidation()
        {
            try
            {
                dxErrorProviderFilter.ClearErrors();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Validates the mandatory filters of mode 2 (QT-02, QT-03, QT-04).
        /// Returns false when the search must be blocked.
        /// </summary>
        private bool ValidateFilterByDoctor()
        {
            bool result = false;
            try
            {
                ResetFilterValidation();

                if (!IsFilterByDoctorMode()) return true;   // mode 1 has no mandatory filter

                string requiredMessage = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                    HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);

                // QT-02: period and record status become mandatory once a doctor is picked
                if (dtFromDate.EditValue == null)
                {
                    dxErrorProviderFilter.SetError(dtFromDate, requiredMessage, ErrorType.Warning);
                    dtFromDate.Focus();
                    return false;
                }

                if (dtToDate.EditValue == null)
                {
                    dxErrorProviderFilter.SetError(dtToDate, requiredMessage, ErrorType.Warning);
                    dtToDate.Focus();
                    return false;
                }

                if (cboTreatmentStatus.EditValue == null)
                {
                    dxErrorProviderFilter.SetError(cboTreatmentStatus, requiredMessage, ErrorType.Warning);
                    cboTreatmentStatus.Focus();
                    return false;
                }

                DateTime fromDate = dtFromDate.DateTime.Date;
                DateTime toDate = dtToDate.DateTime.Date;

                // QT-03
                if (fromDate > toDate)
                {
                    dxErrorProviderFilter.SetError(dtFromDate,
                        Resources.ResourceMessage.TuNgayPhaiNhoHonDenNgay, ErrorType.Warning);
                    dtFromDate.Focus();
                    return false;
                }

                // QT-04
                if ((toDate - fromDate).TotalDays + 1 > MAX_FILTER_DAYS)
                {
                    if (XtraMessageBox.Show(
                            Resources.ResourceMessage.KhoangThoiGianVuotQua31Ngay,
                            HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                                HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return false;
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Start of the selected period as yyyyMMdd000000.</summary>
        private long GetFilterFromTime()
        {
            try
            {
                return Convert.ToInt64(dtFromDate.DateTime.ToString("yyyyMMdd") + "000000");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        /// <summary>End of the selected period as yyyyMMdd235959.</summary>
        private long GetFilterToTime()
        {
            try
            {
                return Convert.ToInt64(dtToDate.DateTime.ToString("yyyyMMdd") + "235959");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        /// <summary>True when the user asked for finished records only (QT-06).</summary>
        private bool IsFilterFinishedTreatment()
        {
            try
            {
                if (cboTreatmentStatus.EditValue == null) return false;
                return Convert.ToInt64(cboTreatmentStatus.EditValue) == TREATMENT_STATUS__FINISHED;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private void cboRequestDoctor_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ApplyModeUI();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nut xoa tren o Bac si chi dinh.
        /// DevExpress chi ve san glyph cho ButtonPredefines.Delete, KHONG tu xoa gia tri -
        /// phai tu gan EditValue = null, neu khong nguoi dung khong thoat duoc Cach 2.
        /// Gan null se kich EditValueChanged -> ApplyModeUI() dua man hinh ve Cach 1.
        /// </summary>
        private void cboRequestDoctor_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button == null
                    || e.Button.Kind != DevExpress.XtraEditors.Controls.ButtonPredefines.Delete) return;

                if (cboRequestDoctor.EditValue == null) return;

                cboRequestDoctor.ClosePopup();
                cboRequestDoctor.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Quick filters (QT-11, QT-12)

        /// <summary>
        /// Applies the two quick filters to the order list.
        /// MUST run after ProcessDocumentStatus(), because both filters read DOC_STATUS.
        ///   chkNoDocument      -> keep only NoDocument                       (QT-11)
        ///   chkNotFullySigned  -> keep everything except FullySigned         (QT-12)
        /// Both ticked -> only NoDocument survives, which matches QT-08.
        /// </summary>
        private List<ADO.InfoRecordADO> ApplyQuickFilters(List<ADO.InfoRecordADO> records)
        {
            try
            {
                if (records == null || records.Count == 0) return records;

                if (chkNoDocument.Checked)
                {
                    records = records
                        .Where(o => o.DOC_STATUS == EnumRecordDocumentStatus.NoDocument)
                        .ToList();
                }

                if (chkNotFullySigned.Checked)
                {
                    records = records
                        .Where(o => o.DOC_STATUS != EnumRecordDocumentStatus.FullySigned)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return records;
        }

        /// <summary>
        /// Persists the checked state of one check box so it survives a restart.
        /// </summary>
        private void SaveCheckBoxState(CheckEdit chk)
        {
            try
            {
                if (chk == null || controlStateWorker == null) return;

                var item = (currentControlStateRDO != null)
                    ? currentControlStateRDO.FirstOrDefault(o => o.KEY == chk.Name && o.MODULE_LINK == MODULE_LINK)
                    : null;

                if (item != null)
                {
                    item.VALUE = chk.Checked ? "1" : "0";
                }
                else
                {
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                    currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = chk.Name,
                        MODULE_LINK = MODULE_LINK,
                        VALUE = chk.Checked ? "1" : "0"
                    });
                }

                controlStateWorker.SetData(currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkNoDocument_CheckedChanged(object sender, EventArgs e)
        {
            // Blocked while InitControlState() restores the saved state.
            if (IsLoadFirstForm) return;

            try
            {
                SaveCheckBoxState(chkNoDocument);
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkNotFullySigned_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLoadFirstForm) return;

            try
            {
                SaveCheckBoxState(chkNotFullySigned);
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }

    /// <summary>Single option of the record status combo (QT-06).</summary>
    internal class TreatmentStatusADO
    {
        public long ID { get; set; }
        public string NAME { get; set; }

        public TreatmentStatusADO(long id, string name)
        {
            this.ID = id;
            this.NAME = name;
        }
    }
}

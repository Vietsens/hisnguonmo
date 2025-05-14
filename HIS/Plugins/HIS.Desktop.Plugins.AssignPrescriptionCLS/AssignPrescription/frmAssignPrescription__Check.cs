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
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private bool CheckExistMedicinePaymentLimit(string medicineTypeCode)
        {
            bool result = false;
            try
            {
                string medicePaymentLimit = HisConfigCFG.MedicineHasPaymentLimitBHYT.ToLower();
                if (!String.IsNullOrEmpty(medicePaymentLimit))
                {
                    string[] medicineArr = medicePaymentLimit.Split(',');
                    if (medicineArr != null && medicineArr.Length > 0)
                    {
                        if (medicineArr.Contains(medicineTypeCode.ToLower()))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckMaxInPrescription(MediMatyTypeADO mediMaTy, decimal? _amount)
        {
            bool result = true;
            try
            {

                decimal? amount = null;
                if (mediMaTy != null && mediMaTy.ALERT_MAX_IN_PRESCRIPTION.HasValue)
                {
                    List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                    if (mediMatyTypeADOs == null)
                        mediMatyTypeADOs = new List<MediMatyTypeADO>();

                    amount = mediMatyTypeADOs.Where(o => o.ID == mediMaTy.ID && o.PrimaryKey != mediMaTy.PrimaryKey).Sum(o => o.AMOUNT)
                        + _amount;

                    if (amount > mediMaTy.ALERT_MAX_IN_PRESCRIPTION.Value)
                    {
                        DialogResult myResult;
                        string notice = "";
                        if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            notice = ResourceMessage.ThuocXKeQuaSoLuongChoPhepBanCoMuonBoSung;                            
                        }
                        else if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            notice = ResourceMessage.VatTuXKeQuaSoLuongChoPhepBanCoMuonBoSung;
                        }
                        myResult = MessageBox.Show(String.Format(notice, mediMaTy.MEDICINE_TYPE_NAME, amount - mediMaTy.ALERT_MAX_IN_PRESCRIPTION.Value, mediMaTy.SERVICE_UNIT_NAME), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != System.Windows.Forms.DialogResult.Yes)
                        {
                            result = false;
                        }
                        Inventec.Common.Logging.LogSystem.Debug("CheckMaxInPrescription:valid=" + result + "____" + String.Format(notice, mediMaTy.MEDICINE_TYPE_NAME, amount - mediMaTy.ALERT_MAX_IN_PRESCRIPTION.Value, mediMaTy.SERVICE_UNIT_NAME));
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckOddConvertUnit(MediMatyTypeADO mediMaTy, decimal? _amount)
        {
            bool result = true;
            try
            {
                if (mediMaTy != null && mediMaTy.CONVERT_RATIO.HasValue)
                {
                    decimal? amount = _amount / mediMaTy.CONVERT_RATIO;
                    var phanthapphan = amount - ((int)amount);
                    if (phanthapphan.ToString().Length > 8)
                    {
                        MessageBox.Show(String.Format(ResourceMessage.SoLuongBiLePhanThapPhanToiDaX, _amount, mediMaTy.CONVERT_RATIO, amount));
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckMaMePackage(MediMatyTypeADO mediMaTy)
        {
            bool result = true;
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                if (mediMatyTypeADOs == null || mediMatyTypeADOs.Count == 0) return true;

                var existsMameType = mediMatyTypeADOs.Any(o => !(o.IsAssignPackage.HasValue && o.IsAssignPackage.Value) && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT));
                var existsMame = mediMatyTypeADOs.Any(o => (o.IsAssignPackage.HasValue && o.IsAssignPackage.Value) && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT));

                if (mediMaTy != null && (mediMaTy.IsAssignPackage.HasValue && mediMaTy.IsAssignPackage.Value) && existsMameType)
                {
                    string mameWarm = String.Join(",", mediMatyTypeADOs.Where(o => !(o.IsAssignPackage.HasValue && o.IsAssignPackage.Value) && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)).Select(o => o.MEDICINE_TYPE_NAME));
                    MessageBox.Show(String.Format(ResourceMessage.DanhSachThuocDaKeDaCoThuocTheoXKhongTheBoSungThemThuocYZ, mameWarm, "loại", mediMaTy.MEDICINE_TYPE_NAME, "lô"));
                    result = false;
                }
                else if (mediMaTy != null && !(mediMaTy.IsAssignPackage.HasValue && mediMaTy.IsAssignPackage.Value) && existsMame)
                {
                    string mameWarm = String.Join(",", mediMatyTypeADOs.Where(o => (o.IsAssignPackage.HasValue && o.IsAssignPackage.Value) && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)).Select(o => o.MEDICINE_TYPE_NAME));
                    MessageBox.Show(String.Format(ResourceMessage.DanhSachThuocDaKeDaCoThuocTheoXKhongTheBoSungThemThuocYZ, mameWarm, "lô", mediMaTy.MEDICINE_TYPE_NAME, "loại"));
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckGenderMediMaty(MediMatyTypeADO mediMaTy)
        {
            bool result = true;
            try
            {
                if (currentTreatmentWithPatientType != null && mediMaTy != null && mediMaTy.TDL_GENDER_ID.HasValue)
                {
                    if (currentTreatmentWithPatientType.TDL_PATIENT_GENDER_ID != mediMaTy.TDL_GENDER_ID)
                    {
                        HIS_GENDER gender = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>()
                            .FirstOrDefault(o => o.ID == mediMaTy.TDL_GENDER_ID);
                        if (gender != null)
                        {
                            MessageBox.Show(String.Format(ResourceMessage.ThuocVatTuChiSuDungChoGioiTinhX, mediMaTy.MEDICINE_TYPE_NAME, gender.GENDER_NAME), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            result = false;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Kiểm tra hẹn khám sớm
        /// #13751
        /// </summary>
        private void CheckAppoinmentEarly()
        {
            try
            {
                if (GlobalStore.IsCabinet || GlobalStore.IsTreatmentIn)
                {
                    return;
                }

                if (this.currentTreatmentWithPatientType != null
                    && this.currentTreatmentWithPatientType.PREVIOUS_APPOINTMENT_TIME.HasValue)
                {
                    DateTime dtAppoinmentTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentTreatmentWithPatientType.PREVIOUS_APPOINTMENT_TIME.Value).Value;
                    if (dtAppoinmentTime > DateTime.Now)
                    {
                        TimeSpan ts = new TimeSpan();
                        ts = (TimeSpan)(dtAppoinmentTime.Date - DateTime.Now.Date);
                        if (ts.TotalDays > 0)
                        {
                            lblNotice.Text = String.Format(ResourceMessage.BenhNhanDiKamSomTruocXNgay, ts.TotalDays);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

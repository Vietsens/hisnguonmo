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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription
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
                            notice = "Thuốc {0} kê quá số lượng cho phép ({1} {2}). Bạn có muốn bố sung hay không?";
                        }
                        else if (mediMaTy.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            notice = "Vật tư {0} kê quá số lượng cho phép ({1} {2}). Bạn có muốn bố sung hay không?";
                        }
                        myResult = MessageBox.Show(String.Format(notice, mediMaTy.MEDICINE_TYPE_NAME, amount - mediMaTy.ALERT_MAX_IN_PRESCRIPTION.Value, mediMaTy.SERVICE_UNIT_NAME), "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (myResult != System.Windows.Forms.DialogResult.OK)
                        {
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
                        MessageBox.Show(String.Format("Số lượng bị lẻ phần thập phân tối đa là 6. {0}/{1} = {2}", _amount, mediMaTy.CONVERT_RATIO, amount));
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
                            MessageBox.Show(String.Format("Thuốc/vật tư {0} chỉ sử dụng cho giới tính {1}.", mediMaTy.MEDICINE_TYPE_NAME, gender.GENDER_NAME), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
}

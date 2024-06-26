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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT
{
    /// <summary>
    /// - Y/c:
    ///Bổ sung cấu hình, để cho phép thông báo đối với BN nếu thỏa mãn các điều kiện:
    ///+ BN ko phải BHYT 
    ///+ Phòng làm việc là phòng cấp cứu
    ///Nội dung thông báo "Bệnh nhân đã tạm ứng XXXX đồng, bạn có muốn tiếp tục không" (nếu chưa tạm ứng thì hiển thị XXX = 0)
    ///Nếu chọn có thì tiếp tục vào màn hình chỉ định dịch vụ
    ///Nếu chọn không thì không xử lý gì, đóng màn hình chỉ định dịch vụ lại
    /// </summary>
    public class AlertHospitalFeeNotBHYTManager
    {
        public AlertHospitalFeeNotBHYTManager() { }

        public bool Run(long treatmentId, long patientTypeId, long roomId)
        {
            bool success = true;
            try
            {
                ConfigCFG.LoadConfig();
                if (ConfigCFG.IsAlertHospitalFeeNotBHYT)
                {
                    bool condiValid = true;

                    condiValid = condiValid && patientTypeId > 0;
                    condiValid = condiValid && roomId > 0;
                    condiValid = condiValid && patientTypeId != ConfigCFG.PatientTypeId__BHYT;

                    var roomXL = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == roomId);
                    condiValid = condiValid && (roomXL != null && roomXL.IS_EMERGENCY == ConfigCFG.valueInt__true);

                    if (condiValid)
                    {
                        string numDepositByPatient = GetTotalDepositByPatient(treatmentId);

                        if (MessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.AlertHospitalFeeNotBHYT), numDepositByPatient), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            success = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        public bool Run(decimal? depositByPatient, long patientTypeId, long roomId)
        {
            bool success = true;
            try
            {
                ConfigCFG.LoadConfig();
                if (ConfigCFG.IsAlertHospitalFeeNotBHYT)
                {
                    bool condiValid = true;

                    condiValid = condiValid && patientTypeId > 0;
                    condiValid = condiValid && roomId > 0;
                    condiValid = condiValid && patientTypeId != ConfigCFG.PatientTypeId__BHYT;

                    var roomXL = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == roomId);
                    condiValid = condiValid && (roomXL != null && roomXL.IS_EMERGENCY == ConfigCFG.valueInt__true);

                    if (condiValid)
                    {
                        string numDepositByPatient = Inventec.Common.Number.Convert.NumberToString(depositByPatient ?? 0, ConfigApplications.NumberSeperator);

                        if (MessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.AlertHospitalFeeNotBHYT), numDepositByPatient), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            success = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        string GetTotalDepositByPatient(long treatmentId)
        {
            CommonParam paramCommon = new CommonParam();
            HisTreatmentFeeViewFilter treatFilter = new HisTreatmentFeeViewFilter();
            treatFilter.ORDER_DIRECTION = "MODIFY_TIME";
            treatFilter.ORDER_FIELD = "DESC";
            treatFilter.ID = treatmentId;
            //treatFilter.BRANCH_ID = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch.ID;
            //treatFilter.PATIENT_CODE__EXACT = code;

            var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, treatFilter, paramCommon).FirstOrDefault();
            if (result != null)
            {
                return Inventec.Common.Number.Convert.NumberToString(result.TOTAL_DEPOSIT_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
            }
            return "0";
        }
    }
}

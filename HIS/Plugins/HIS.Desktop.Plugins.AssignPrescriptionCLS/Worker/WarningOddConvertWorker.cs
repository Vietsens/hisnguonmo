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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.Worker
{
    internal class WarningOddConvertWorker
    {
        /// <summary>
        ///  #20327
        /// - Bổ sung cấu hình hệ thống: "HIS.Desktop.Plugins.AssignPrescription.IsWarningOddConvertAmount": "1: Cảnh báo khi kê đơn nếu số lượng quy đổi bị lẻ" 
        ///- Khi bật cấu hình này thì xử lý:
        ///+ Khi kê đơn, nếu người dùng chọn thuốc và nhấn nút "bổ sung", và thuốc này có đơn vị quy đổi thì hệ thống tính ra số lượng quy đổi (= số lượng * tỷ lệ quy đổi). 
        ///+ Nếu số lượng quy đổi ko phải là 1 số nguyên thì hiển thị cảnh báo: "Số lượng kê bị lẻ "XXX" (1 XXX = T YYYY). Bạn có muốn kê không?". Trong đó: XXX là đơn vị quy đổi, T là tỷ lệ quy đổi, YYY là đơn vị gốc.
        ///vd: "Số lượng kê bị lẻ "lọ" (1 lọ = 400 UI). Bạn có muốn kê không?" 
        ///Nếu người dùng đồng ý thì thực hiện bổ sung, còn ko thì không thực hiện gì.
        /// </summary>
        /// <returns>true/false</returns>
        internal static bool CheckWarningOddConvertAmount(MediMatyTypeADO currentMedicineTypeADOForEdit, decimal amount, Action<bool, bool> ResetFocusMediMaty)
        {
            bool valid = true;
            string messageErr = "";
            try
            {
                if (HisConfigCFG.IsWarningOddConvertAmount)
                {
                    decimal? CONVERT_RATIO = currentMedicineTypeADOForEdit != null ? currentMedicineTypeADOForEdit.CONVERT_RATIO : null;
                    string CONVERT_UNIT_NAME = currentMedicineTypeADOForEdit != null ? currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME : "";
                    string SERVICE_UNIT_NAME = currentMedicineTypeADOForEdit != null ? currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME : "";
                    bool? IsUseOrginalUnitForPres = currentMedicineTypeADOForEdit != null ? currentMedicineTypeADOForEdit.IsUseOrginalUnitForPres : null;

                    if ((CONVERT_RATIO ?? 0) > 0 && (IsUseOrginalUnitForPres ?? false) == false)
                    {
                        var amountWarning = amount / CONVERT_RATIO.Value;
                        valid = amountWarning == (long)amountWarning;

                        if (!valid)
                        {
                            messageErr = string.Format(ResourceMessage.WarnKeDonCoSoLuongQuyDoiLe, SERVICE_UNIT_NAME, SERVICE_UNIT_NAME, CONVERT_RATIO, CONVERT_UNIT_NAME);
                            DialogResult myResult = MessageBox.Show(messageErr, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            valid = (myResult == DialogResult.Yes);
                        }
                    }
                    if (!valid)
                    {
                        if (ResetFocusMediMaty != null)
                        {
                            //ReSetDataInputAfterAdd__MedicinePage();//Bỏ comment nếu cần reset lại các ô nhập thông tin thuốc cần bổ sung TODO
                            ResetFocusMediMaty(true, true);
                        }
                        Inventec.Common.Logging.LogSystem.Debug("CheckWarningOddConvertAmount. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid)
                            + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageErr), messageErr)
                            + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CONVERT_RATIO), CONVERT_RATIO)
                            + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CONVERT_UNIT_NAME), CONVERT_UNIT_NAME)
                            + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SERVICE_UNIT_NAME), SERVICE_UNIT_NAME)
                            + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IsUseOrginalUnitForPres), IsUseOrginalUnitForPres)
                            );
                    }                    
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

    }
}

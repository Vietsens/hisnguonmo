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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.DelegateRegister
{
    public interface IRegisterHisConfig
    {
        string GetIsShowCheckExpired();
        bool GetIsCheckHeinCard();
        bool GetIsCheckPreviousDebt();
        bool GetIsCheckPreviousPrescription();
        string GetIsDefaultRightRouteType();
        bool GetVisibilityControl();
        bool GetIsObligatoryTranferMediOrg();
        bool GetIsSyncHID();
        bool GetIsWarningOverExamBhyt();

        /// <summary>
        /// - Cấu hình chế độ kiểm tra thẻ bhyt trên cổng bhxh. Đặt 1 là tự động kiểm tra khi tìm thấy bệnh nhân cũ, đặt số khác là không kiểm tra.
        /// </summary>
        bool GetIsCheckExamHistory();
        string GetAutoCheckIcd();
        string GetPatientTypeCode__BHYT();
        long GetPatientTypeId__BHYT();
        string GetPatientTypeCode__QN();
        long GetPatientTypeId__QN();
        string GetIsPrintAfterSave();
        string GetIsVisibleBill();
        bool GetIsSetDefaultDepositPrice();
        List<string> GetExecuteRoomShow();

        /// <summary>
        /// true - Bắt buộc nhập thông tin Người nhà, quan hệ, địa chỉ với trẻ em nhỏ hơn 6t
        /// false - Không bắt buộc nhập thông tin Người nhà, quan hệ, địa chỉ với mọi đối tượng
        /// </summary>
        bool GetMustHaveNCSInfoForChild();

        MOS.EFMODEL.DataModels.HIS_GENDER GetGenderBase();
        MOS.EFMODEL.DataModels.HIS_CAREER GetCareerBase();
        MOS.EFMODEL.DataModels.HIS_CAREER GetCareerHS();
        MOS.EFMODEL.DataModels.HIS_CAREER GetCareerUnder6Age();
        SDA.EFMODEL.DataModels.SDA_NATIONAL GetNationalBase();
        SDA.EFMODEL.DataModels.SDA_ETHNIC GetEthinicBase();
    }
}

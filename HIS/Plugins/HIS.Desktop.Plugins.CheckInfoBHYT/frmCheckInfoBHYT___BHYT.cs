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
using His.Bhyt.InsuranceExpertise;
using His.Bhyt.InsuranceExpertise.LDO;
using MOS.EFMODEL.DataModels;
using System;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;


namespace HIS.Desktop.Plugins.CheckInfoBHYT
{
    public partial class frmCheckInfoBHYT : HIS.Desktop.Utility.FormBase
    {
        private async Task CheckTTFull(V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter, string nameCb, string cccdCb, string api)
        {
            rsDataBHYT = new ResultDataADO();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(String.Format("Tên cán bộ:{0}", nameCb));
                Inventec.Common.Logging.LogSystem.Debug(String.Format("CCCD cán bộ:{0}", cccdCb));
                Inventec.Common.Logging.LogSystem.Debug(String.Format("Tên api:{0}", api));

                ApiInsuranceExpertise apiInsuranceExpertise = new ApiInsuranceExpertise();
                apiInsuranceExpertise.ApiEgw = api;
                CheckHistoryLDO checkHistoryLDO = new CheckHistoryLDO();
                checkHistoryLDO.maThe = _patientTypeAlter.HEIN_CARD_NUMBER;
                checkHistoryLDO.ngaySinh = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_HisPatient.DOB);
                checkHistoryLDO.hoTen = _HisPatient.VIR_PATIENT_NAME;
                checkHistoryLDO.cccdCb = cccdCb;
                checkHistoryLDO.hoTenCb = nameCb;
                if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                    || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                    || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                {
                    rsDataBHYT.ResultHistoryLDO = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                }
            }
            catch (Exception ex)
            {
                rsDataBHYT = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public class GenderConvert
        {
            public static string TextToNumber(string ge)
            {
                return (ge == "Nữ") ? "2" : "1";
            }

            public static string HisToHein(string ge)
            {
                return (ge == "1") ? "2" : "1";
            }

            public static long HeinToHisNumber(string ge)
            {
                return (ge == "1" ? IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE : IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE);
            }
        }
    }
}

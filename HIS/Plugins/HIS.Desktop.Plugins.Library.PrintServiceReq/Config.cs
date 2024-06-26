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
using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq
{
    class Config
    {
        internal const string mps = "HIS.Desktop.Plugins.Library.PrintServiceReq.Mps";

        internal const string HIS_DEPOSIT__PRICE_FOR_BHYT_KEY = "HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT"; //tính tiền dịch vụ cần tạm ứng(ct MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPriceForBhytOutPatient) ở chức năng tạm ứng dịch vụ theo dịch vụ.
        internal const string mpsGroup = "HIS.Desktop.Plugins.Library.PrintServiceReq.Mps.PrintGroup_340";

        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT

        internal const string CONFIG_KEY__AssignServicePrint_CODE = "HIS.Desktop.Plugins.ServiceReq.PrintSplitByParent.ServiceReqTypeCode";
        internal const string CONFIG_KEY__PRINT_BARCODE_NO_ZERO = "HIS.Desktop.Library.Print.BacodeNoZero";
        internal const string OptionPrintDifferenceMps = "HIS.Desktop.Plugins.ServiceReq.PrintOption.IsUsingDifferenceTempForDifferenceSubTypes";
        internal const string OptionPrintXetNghiemDiffMps = "HIS.Desktop.Plugins.ServiceReq.Test.PrintSplitByType";
        internal const string OptionCurrentNumOrder = "HIS.Desktop.ServiceReq.PrintCurrentNumOrder";
        internal const string GroupByExecuteDepartment = "HIS.Desktop.Plugins.Library.PrintServiceReq.GroupByDepartment";

        internal const string OptionMergePrint_IsmergeCFG = "HIS.Desktop.Plugins.OptionMergePrint.Ismerge";

        internal const string LisIntegrationTypeCFG = "MOS.LIS.INTEGRATION_TYPE";

        internal const string Sugr_PrintSplitByType = "HIS.Desktop.Plugins.ServiceReq.Sugr.PrintSplitByType";
        internal const string CFG_HisTranReqQRCodeTreatmentPrint = "HIS.Desktop.Plugins.AssignService.HisTranReqQRCodeTreatmentPrint";

        internal static bool HisTranReqQRCodeTreatmentPrint;
        internal static bool BARCODE_NO_ZERO;
        internal static bool IsmergePrint;

        internal static long PatientTypeId__BHYT;

        internal static List<string> departmentCodeTestGroup;

        internal static string SugrPrintSplitByType; 

        internal static void LoadConfig()
        {
            try
            {
                PatientTypeId__BHYT = GetPatientTypeByCode(HisConfigs.Get<string>(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT)).ID;
                BARCODE_NO_ZERO = HisConfigs.Get<string>(CONFIG_KEY__PRINT_BARCODE_NO_ZERO) == "1";
                IsmergePrint = HisConfigs.Get<string>(OptionMergePrint_IsmergeCFG) == "1";
                HisTranReqQRCodeTreatmentPrint = HisConfigs.Get<string>(CFG_HisTranReqQRCodeTreatmentPrint) == "1";
                var data = HisConfigs.Get<string>(GroupByExecuteDepartment);
                if (!String.IsNullOrWhiteSpace(data))
                {
                    departmentCodeTestGroup = data.Split(',').ToList();
                }

                SugrPrintSplitByType = HisConfigs.Get<string>(Sugr_PrintSplitByType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
        }
    }
}

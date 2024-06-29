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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.UCHeniInfo.Data
{
    internal class DataStore
    {
        internal static List<HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO> MediOrgForHasDobCretidentials { get; set; }
        internal static List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> HeinRightRouteTypes { get; set; }

        //private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        //private const string CONFIG_KEY__PATIENT_TYPE_CODE__QN = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.QN"; // Doi tuong Quan Nhan
        //private const string CONFIG_KEY__IS_OBLIGATORY_TRANFER_MEDI_ORG = "HIS.UC.UCHein.IS_OBLIGATORY_TRANFER_MEDI_ORG";

        //private const string CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE = "CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE";

        //internal static string PatientTypeCode__BHYT;
        //internal static long PatientTypeId__BHYT;
        //internal static string PatientTypeCode__QN;
        //internal static long PatientTypeId__QN;
        //internal static string PatientTypeCode__ByKeyConfig;
        //internal static long PatientTypeID_Default;

        //internal static bool IsObligatory { get; set; }
        //internal static long PatientTypeId { get; set; }

        //private const string valueString__true = "1";

        //internal static List<MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST> BhytBlackLists {get;set;}
        //internal static List<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST> BhytWhiteLists { get; set; }

        //internal static void LoadConfig()
        //{
        //    try
        //    {
        //        PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
        //        PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
        //        PatientTypeCode__QN = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__QN);
        //        PatientTypeId__QN = GetPatientTypeByCode(PatientTypeCode__QN).ID;

        //        BhytWhiteLists = BackendDataWorker.Get<HIS_BHYT_WHITELIST>();
        //        BhytBlackLists = BackendDataWorker.Get<HIS_BHYT_BLACKLIST>();

        //        PatientTypeCode__ByKeyConfig = ConfigApplicationWorker.Get<string>(CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE);
        //        PatientTypeID_Default = GetPatientTypeByCode(PatientTypeCode__ByKeyConfig).ID;

        //        IsObligatory = (GetValue(CONFIG_KEY__IS_OBLIGATORY_TRANFER_MEDI_ORG) == valueString__true);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
        //{
        //    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
        //    try
        //    {
        //        result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }

        //    return result ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
        //}

        //private static string GetValue(string key)
        //{
        //    try
        //    {
        //        return HisConfigs.Get<string>(key);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //    return "";
        //}
    }
}

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
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.UCTransPati.Config
{
    class DataStore
    {
        private const string CONFIG_KEY__TIEP_DON_HIEN_THI_THONG_TIN_THEM = "CONFIG_KEY__TIEP_DON_HIEN_THI_THONG_TIN_THEM";
        private const string HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG = "HIS.UC.UCHein.IS_OBLIGATORY_TRANFER_MEDI_ORG";

        internal static List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG> MediOrgs { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_ICD> Icds { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> TranPatiForms { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> TranPatiReasons { get; set; }
        internal static List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> HeinRightRouteTypes { get; set; }

        internal static bool IsVisibleSomeControl { get; set; }
        internal static bool IsObligatoryTranferMediOrg { get; set; }
        internal static long ObligatoryTranferMediOrg { get; set; }

        internal static void LoadConfig()
        {
            try
            {
                TranPatiForms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>();
                TranPatiReasons = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>();
                MediOrgs = BackendDataWorker.Get<HIS_MEDI_ORG>();
                Icds = BackendDataWorker.Get<HIS_ICD>();
                HeinRightRouteTypes = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeStore.Get();

                try
                {
                    IsVisibleSomeControl = (ConfigApplicationWorker.Get<string>(CONFIG_KEY__TIEP_DON_HIEN_THI_THONG_TIN_THEM) == GlobalVariables.CommonStringTrue);
                    IsObligatoryTranferMediOrg = (GetValue(HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG) == GlobalVariables.CommonStringTrue);
                    ObligatoryTranferMediOrg = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG));
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static string GetValue(string key)
        {
            try
            {
                return HisConfigs.Get<string>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return "";
        }
    }
}

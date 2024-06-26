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
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisAccountBookList.Base
{
    class GlobalStore
    {
        public static SAR.EFMODEL.DataModels.SAR_REPORT_TYPE ReportType = new SAR.EFMODEL.DataModels.SAR_REPORT_TYPE();

        public static SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE ReportTemplate = new SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE();

        public static string REPORT_TYPE_CODE_BC_CHI_TIET_SU_DUNG_QUYEN_SO_THU_CHI = "MRS00249";

        internal const short IS_TRUE = (short)1;

        internal static void ListOfReportType()
        {
            try
            {
                var apiReportType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>().Where(o => o.REPORT_TYPE_CODE == REPORT_TYPE_CODE_BC_CHI_TIET_SU_DUNG_QUYEN_SO_THU_CHI).FirstOrDefault();
                if (apiReportType != null)
                {
                    GlobalStore.ReportType = apiReportType;
                    var apiTemplate = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE>().Where(o => o.REPORT_TYPE_ID == apiReportType.ID).FirstOrDefault();
                    if (apiTemplate != null)
                    {
                        GlobalStore.ReportTemplate = apiTemplate;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                throw;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> PatientType;
        public static List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> ListPatientType
        {
            get
            {
                if (PatientType == null || PatientType.Count == 0)
                {
                    PatientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                return PatientType;
            }
            set
            {
                PatientType = value;
            }
        }
     
        internal static List<string> TypeFilters = null;

        internal void LoadTypeFilter()
        {
            try
            {
                TypeFilters = new List<string>();
                TypeFilters.Add(THUONG);
                TypeFilters.Add(DICH_VU);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string THUONG
        {
            get
            {
                return Inventec.Common.Resource.Get.Value(
                    "THUONG",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
        }
        string DICH_VU
        {
            get
            {
                return Inventec.Common.Resource.Get.Value(
                    "DICH_VU",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
        }

        public static List<BillTypeADO> listBillType = null;

        internal void CreateBillType()
        {
            try
            {
                LoadTypeFilter();
                listBillType = new List<Base.BillTypeADO>();
                for (int i = 0; i < Base.GlobalStore.TypeFilters.Count; i++)
                {
                    Base.BillTypeADO item = new Base.BillTypeADO();
                    item.ID = i + 1;
                    item.BillTypeName = Base.GlobalStore.TypeFilters[i];
                    listBillType.Add(item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                listBillType = null;
            }
        }
        
    }
}

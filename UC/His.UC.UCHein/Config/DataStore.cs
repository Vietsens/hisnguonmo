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

namespace His.UC.UCHein.Config
{
    class DataStore
    {
        internal static List<His.UC.UCHein.Data.MediOrgADO> MediOrgs { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG> MediOrgForHasDobCretidentials { get; set; }
        internal static List<MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData> LiveAreas { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_ICD> Icds { get; set; }
        internal static List<His.UC.UCHein.Data.IcdADO> IcdADOs { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> TranPatiForms { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> TranPatiReasons { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_GENDER> Genders { get; set; }
        internal static List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> HeinRightRouteTypes { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> TreatmentTypes { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> PatientTypes { get; set; }
    }
}

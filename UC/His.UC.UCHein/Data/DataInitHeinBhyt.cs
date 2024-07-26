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
using Inventec.Common.QrCodeBHYT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.Data
{
    public class DataInitHeinBhyt
    {
        public DataInitHeinBhyt() { }

        public string Template { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG> MediOrgs { get; set; }
        public List<MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData> LiveAreas { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_ICD> Icds { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> TranPatiForms { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> TranPatiReasons { get; set; }
        public List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> HeinRightRouteTypes { get; set; }
        public string MEDI_ORG_CODE__CURRENT { get; set; }
        public List<string> MEDI_ORG_CODES__ACCEPTs { get; set; }
        public string HEIN_LEVEL_CODE__CURRENT { get; set; }
        public long TREATMENT_TYPE_ID__EXAM { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> TreatmentTypes { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> PatientTypes { get; set; }
        public long PATIENT_TYPE_ID__BHYT { get; set; }
        public bool IsChild { get; set; }
        public long PatientTypeId { get; set; }
        public long isVisibleControl { get; set; }
        public string IsShowCheckKhongKTHSD { get; set; }
        public string SYS_MEDI_ORG_CODE { get; set; }
        public bool IsNotRequiredRightTypeInCaseOfHavingAreaCode { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST> BhytWhiteLists { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST> BhytBlackLists { get; set; }
        public long ExceedDayAllow { get; set; }
        public string AutoCheckIcd { get; set; }
        public bool IsDefaultRightRouteType { get; set; }
        public bool IsEdit { get; set; }
        public bool IsTempQN { get; set; }
        public bool IsObligatoryTranferMediOrg { get; set; }//#4862  
        public string ObligatoryTranferMediOrg { get; set; }
        public bool IsDungTuyenCapCuuByTime { get; set; }//#4862
        public bool IsAutoSelectEmergency { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_GENDER> Genders { get; set; }
        public FillDataPatientSDOToRegisterForm FillDataPatientSDOToRegisterForm { get; set; }
        public SetFocusMoveOut SetFocusMoveOut { get; set; }
        public SetShortcutKeyDown SetShortcutKeyDown { get; set; }
        public ProcessFillDataCareerUnder6AgeByHeinCardNumber ProcessFillDataCareerUnder6AgeByHeinCardNumber { get; set; }
        public DelegateAutoCheckCC AutoCheckCC { get; set; }
        public CheckExamHistoryByHeinCardNumber CheckExamHistory { get; set; }
        public DelegateSetRelativeAddress SetRelativeAddress { get; set; }
        public Action ActChangePatientDob { get; set; }
        public bool IsSampleDepartment { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4 HisTreatment { get; set; }
        public Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        public bool IsInitFromCallPatientTypeAlter { get; set; }
        public long PatientId { get; set; }
    }
}

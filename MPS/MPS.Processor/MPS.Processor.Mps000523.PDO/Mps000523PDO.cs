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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000523.PDO
{
    /// <summary>
    /// Giấy khám sức khỏe định kỳ cho người cao tuổi — "Mẫu 4" of the HCMC health department KSK program
    /// (form body: Mẫu 03, TT 25/2026/TT-BYT). Source = KSK over-18 record (HIS_KSK_OVER_EIGHTEEN) + its M4 record
    /// (HIS_KSK_SYT_HCM, 1-1 by KSK_OVER_EIGHTEEN_ID) + HIS_KSK_GENERAL + HIS_DHST.
    /// The processor has no API/cache access, so the plugin resolves portal catalog ids, doctor names,
    /// the interview JSON and lab values beforehand (dictionaries below).
    /// </summary>
    public partial class Mps000523PDO : RDOBase
    {
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public HIS_KSK_OVER_EIGHTEEN HisKskOverEighteen { get; set; }
        public HIS_KSK_SYT_HCM HisKskSytHcm { get; set; }
        public HIS_KSK_GENERAL HisKskGeneral { get; set; }
        public HIS_DHST HisDhst { get; set; }
        public V_HIS_TREATMENT_4 Treatment { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> ExamRanks { get; set; }

        /// <summary>KSK service request entity (optional) — keys SREQ_*.</summary>
        public HIS_SERVICE_REQ KskServiceReq { get; set; }
        /// <summary>Patient entity (optional) — keys PATIENT_*, current address (HT_*).</summary>
        public HIS_PATIENT KskPatient { get; set; }

        /// <summary>Personal history of the form (22 rows), ordered by HIS_DISEASE_TYPE code.</summary>
        public List<Mps000523HistoryADO> PersonalHistories { get; set; }

        /// <summary>HIS_KSK_SYT_HCM.INTERVIEW_JSON parsed: key -> raw value (numbers as invariant strings).</summary>
        public Dictionary<string, string> InterviewAnswers { get; set; }

        /// <summary>
        /// Display names of portal/HIS catalog values, keyed by the FIELD that stores the value:
        /// JSON keys (rltc_*, rlla_*, ttsy_metmoi, benh_*_cskcb, noi_cong_tac, noi_cong_tac_xa_phuong) and
        /// columns (SYT_PAYSOURCE_ID, SYT_PAY_SOURCE_DETAIL_ID, SYT_EXAM_PLACE_ID, SYT_SUGGEST_ID, TOOTH_xx,
        /// KSK_PAY_SOURCE, PATHOLOGICAL_HISTORY_FAMILY).
        /// </summary>
        public Dictionary<string, string> ResolvedNames { get; set; }

        /// <summary>LOGINNAME -> user name of the doctors referenced by the record.</summary>
        public Dictionary<string, string> EmployeeNames { get; set; }

        /// <summary>Lab values keyed by the portal field code (xnm_slhc, shm_creatinin, xnnt_ph...), from the CLS map of the workstation.</summary>
        public Dictionary<string, string> ClsValues { get; set; }

        /// <summary>"Đối tượng" already resolved by the plugin ("code. name; ...").</summary>
        public string PatientTypesText { get; set; }

        public Mps000523PDO(
            V_HIS_SERVICE_REQ hisServiceReq,
            HIS_KSK_OVER_EIGHTEEN hisKskOverEighteen,
            HIS_KSK_SYT_HCM hisKskSytHcm,
            HIS_KSK_GENERAL hisKskGeneral,
            HIS_DHST hisDhst,
            V_HIS_TREATMENT_4 treatment,
            List<HIS_HEALTH_EXAM_RANK> examRanks)
        {
            try
            {
                this.HisServiceReq = hisServiceReq;
                this.HisKskOverEighteen = hisKskOverEighteen;
                this.HisKskSytHcm = hisKskSytHcm;
                this.HisKskGeneral = hisKskGeneral;
                this.HisDhst = hisDhst;
                this.Treatment = treatment;
                this.ExamRanks = examRanks;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    /// <summary>One personal-history row (HIS_PERIOD_DRIVER_DITY joined with HIS_DISEASE_TYPE).</summary>
    public class Mps000523HistoryADO
    {
        public string DISEASE_TYPE_CODE { get; set; }
        public string DISEASE_TYPE_NAME { get; set; }
        /// <summary>"1" = Có, "0" = Không, null = not answered.</summary>
        public string IS_YES_NO { get; set; }
    }
}

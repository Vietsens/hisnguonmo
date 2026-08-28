using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000499.PDO
{
    public class Mps000499PDO : RDOBase
    {
        public HIS_KSK_OCCUPATIONAL HisKskOccupational { get; set; }
        public V_HIS_TREATMENT_4 HisTreatment { get; set; }
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public V_HIS_DHST HisDhst { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> ExamRank { get; set; }
        /// <summary>Y lệnh KSK (entity HIS_SERVICE_REQ) — tùy chọn; processor đổ key prefix SREQ_.</summary>
        public HIS_SERVICE_REQ KskServiceReq { get; set; }
        /// <summary>Bệnh nhân (HIS_PATIENT) — tùy chọn; processor đổ key prefix PATIENT_.</summary>
        public HIS_PATIENT KskPatient { get; set; }
        /// <summary>
        /// Kết luận theo bệnh (ICD-10) của lượt khám — lưu ở HIS_KSK_GENERAL cùng SERVICE_REQ_ID
        /// (UC "Kết luận theo bệnh ICD-10" dùng chung cho mọi tab KSK).
        /// Tùy chọn; processor đổ key CONCLUSION_ICD_* + object tag {KskGeneral.x}.
        /// </summary>
        public HIS_KSK_GENERAL HisKskGeneral { get; set; }

        public Mps000499PDO(
            HIS_KSK_OCCUPATIONAL hisKskOccupational,
            V_HIS_TREATMENT_4 hisTreatment,
            V_HIS_SERVICE_REQ hisServiceReq,
            V_HIS_DHST hisDhst,
            List<HIS_HEALTH_EXAM_RANK> examRank)
        {
            try
            {
                this.HisKskOccupational = hisKskOccupational;
                this.HisTreatment = hisTreatment;
                this.HisServiceReq = hisServiceReq;
                this.HisDhst = hisDhst;
                this.ExamRank = examRank;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

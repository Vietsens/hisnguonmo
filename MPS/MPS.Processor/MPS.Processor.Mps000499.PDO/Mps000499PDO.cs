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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000507.PDO
{
    public partial class Mps000507PDO : RDOBase
    {
        public HIS_KSK_GENERAL HisKskGeneral { get; set; }
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public HIS_DHST HisDhst { get; set; }
        public V_HIS_TREATMENT_4 Treatment { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> ExamRanks { get; set; }
        public List<V_HIS_DISEASE_DETAIL> DiseaseDetails { get; set; }
        public List<HIS_DISEASE_DETAIL_RESULT> DiseaseDetailResults { get; set; }

        public Mps000507PDO(
            HIS_KSK_GENERAL hisKskGeneral,
            V_HIS_SERVICE_REQ hisServiceReq,
            HIS_DHST hisDhst,
            V_HIS_TREATMENT_4 treatment,
            List<HIS_HEALTH_EXAM_RANK> examRanks,
            List<V_HIS_DISEASE_DETAIL> diseaseDetails,
            List<HIS_DISEASE_DETAIL_RESULT> diseaseDetailResults)
        {
            try
            {
                this.HisKskGeneral = hisKskGeneral;
                this.HisServiceReq = hisServiceReq;
                this.HisDhst = hisDhst;
                this.Treatment = treatment;
                this.ExamRanks = examRanks;
                this.DiseaseDetails = diseaseDetails;
                this.DiseaseDetailResults = diseaseDetailResults;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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

        public List<V_HIS_SERE_SERV> SereServs { get; set; }
        public List<HIS_SERVICE> HisServices { get; set; }
        public List<HIS_SERE_SERV_EXT> SereSErvExts { get; set; }
        public List<V_HIS_TEST_INDEX> TestIndexs { get; set; }
        public List<V_HIS_SERE_SERV_TEIN> SereServTeins { get; set; }

        public List<V_HIS_EMPLOYEE> Employees { get; set; }

        public Mps000507PDO(
            HIS_KSK_GENERAL hisKskGeneral,
            V_HIS_SERVICE_REQ hisServiceReq,
            HIS_DHST hisDhst,
            V_HIS_TREATMENT_4 treatment,
            List<HIS_HEALTH_EXAM_RANK> examRanks,
            List<V_HIS_DISEASE_DETAIL> diseaseDetails,
            List<HIS_DISEASE_DETAIL_RESULT> diseaseDetailResults)
            : this(hisKskGeneral, hisServiceReq, hisDhst, treatment, examRanks, diseaseDetails, diseaseDetailResults,
                   null, null, null, null, null, null)
        {
        }

        public Mps000507PDO(
            HIS_KSK_GENERAL hisKskGeneral,
            V_HIS_SERVICE_REQ hisServiceReq,
            HIS_DHST hisDhst,
            V_HIS_TREATMENT_4 treatment,
            List<HIS_HEALTH_EXAM_RANK> examRanks,
            List<V_HIS_DISEASE_DETAIL> diseaseDetails,
            List<HIS_DISEASE_DETAIL_RESULT> diseaseDetailResults,
            List<V_HIS_SERE_SERV> sereServs,
            List<HIS_SERVICE> hisServices,
            List<HIS_SERE_SERV_EXT> sereSErvExts,
            List<V_HIS_TEST_INDEX> testIndexs,
            List<V_HIS_SERE_SERV_TEIN> sereServTeins)
            : this(hisKskGeneral, hisServiceReq, hisDhst, treatment, examRanks, diseaseDetails, diseaseDetailResults,
                   sereServs, hisServices, sereSErvExts, testIndexs, sereServTeins, null)
        {
        }

        public Mps000507PDO(
            HIS_KSK_GENERAL hisKskGeneral,
            V_HIS_SERVICE_REQ hisServiceReq,
            HIS_DHST hisDhst,
            V_HIS_TREATMENT_4 treatment,
            List<HIS_HEALTH_EXAM_RANK> examRanks,
            List<V_HIS_DISEASE_DETAIL> diseaseDetails,
            List<HIS_DISEASE_DETAIL_RESULT> diseaseDetailResults,
            List<V_HIS_SERE_SERV> sereServs,
            List<HIS_SERVICE> hisServices,
            List<HIS_SERE_SERV_EXT> sereSErvExts,
            List<V_HIS_TEST_INDEX> testIndexs,
            List<V_HIS_SERE_SERV_TEIN> sereServTeins,
            List<V_HIS_EMPLOYEE> employees)
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
                this.SereServs = sereServs;
                this.HisServices = hisServices;
                this.SereSErvExts = sereSErvExts;
                this.TestIndexs = testIndexs;
                this.SereServTeins = sereServTeins;
                this.Employees = employees;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

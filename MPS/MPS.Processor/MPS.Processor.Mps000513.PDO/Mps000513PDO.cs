using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000513.PDO
{
    public partial class Mps000513PDO : RDOBase
    {
        public V_HIS_SPECIALIST_EXAM currentExam { get; set; }

        public HIS_TREATMENT currentTreatment { get; set; }
        public Mps000513PDO(V_HIS_SPECIALIST_EXAM currentExam, HIS_TREATMENT currentTreatment)
        {
            try
            {
                this.currentExam = currentExam;
                this.currentTreatment = currentTreatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

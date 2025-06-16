using HIS.Desktop.Common;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ADO
{
    public class AnalyzeImageADO
    {
        public AnalyzeImageADO()
        {
            
        }
        public long TreatmentId { get; set; }   
        public TimeSpan? TimeOut { get; set; }
        public DelegateSelectData DelSelectData { get; set; }
        public List<HIS_SERE_SERV> SereServs { get; set; }

        public AnalyzeImageADO(long treatmentId, TimeSpan? timeOut, DelegateSelectData delSelectData, List<HIS_SERE_SERV> SereServs)
        {
            this.TreatmentId = treatmentId;
            this.TimeOut = timeOut;
            this.DelSelectData = delSelectData;
            this.SereServs = SereServs;
        }
    }
}

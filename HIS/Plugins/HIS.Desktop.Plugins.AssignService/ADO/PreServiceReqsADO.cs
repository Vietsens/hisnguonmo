using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignService.ADO
{
    class PreServiceReqsADO : V_HIS_SERVICE_REQ_6
    {
        public bool IsReqPicked { get; set; } = false;  // Trạng thái checkbox

        // Khóa gom nhóm theo ngày chỉ định (yyyyMMdd) - dùng để group + sắp xếp ngày giảm dần
        public long INTRUCTION_DATE_KEY { get; set; }

        // Ngày chỉ định hiển thị trên dòng cha: dd/MM/yyyy
        public string INTRUCTION_DATE_str { get; set; }

        public PreServiceReqsADO(V_HIS_SERVICE_REQ_6 source)
        {
            if (source == null) return;
            Inventec.Common.Mapper.DataObjectMapper.Map<PreServiceReqsADO>(this, source);

            // INTRUCTION_TIME dạng yyyyMMddHHmmss (long) -> ngày yyyyMMdd
            this.INTRUCTION_DATE_KEY = this.INTRUCTION_TIME / 1000000;
            int yyyy = (int)(this.INTRUCTION_DATE_KEY / 10000);
            int mm = (int)((this.INTRUCTION_DATE_KEY / 100) % 100);
            int dd = (int)(this.INTRUCTION_DATE_KEY % 100);
            this.INTRUCTION_DATE_str = this.INTRUCTION_DATE_KEY > 0
                ? string.Format("{0:00}/{1:00}/{2:0000}", dd, mm, yyyy)
                : "";
        }
    }
}

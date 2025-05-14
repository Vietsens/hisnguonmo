using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.ADO
{
    public class ServiceReqGroupByDateADO : HisServiceReqGroupByDateSDO
    {
        public long TREELIST_ID { get; set; }
        public long PARENT_ID { get; set; }
        public long TRACKING_TIME { get; set; }
        public long TRACKING_ID { get; set; }
        public bool isParent { get; set; }
        public string DATETIME_DISPLAY { get; set; }
        public ServiceReqGroupByDateADO(HisServiceReqGroupByDateSDO data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisServiceReqGroupByDateSDO>(this, data);
                    this.isParent = true;
                    this.TREELIST_ID = data.InstructionDate ?? 0;
                    this.DATETIME_DISPLAY = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.InstructionDate ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public ServiceReqGroupByDateADO(HisServiceReqGroupByDateSDO data, bool isChild, long trackingTime, List<long> listTreeListIDs)
        {
            try
            {
                if (data != null && !isChild)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisServiceReqGroupByDateSDO>(this, data);
                    this.isParent = true;
                    long num = data.InstructionDate ?? 0;
                    while (listTreeListIDs.Contains(num))
                    {
                        num++;
                    }
                    this.TREELIST_ID = num;
                    this.DATETIME_DISPLAY = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.InstructionDate ?? 0);
                }
                else if (data != null && isChild && trackingTime > 0)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisServiceReqGroupByDateSDO>(this, data);
                    this.isParent = false;
                    long num = trackingTime;
                    while (listTreeListIDs.Contains(num))
                    {
                        num++;
                    }
                    this.TREELIST_ID = num;
                    if (data.InstructionDate != null)
                    {
                        string str = data.InstructionDate.ToString().Substring(0, 8) + "000000";
                        this.PARENT_ID = Convert.ToInt64(str);
                    }
                    string dayHospitalize = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(trackingTime);
                    this.DATETIME_DISPLAY = (System.String.Format("{0:dd/MM/yyyy HH:mm}", dayHospitalize)).Substring(0, (System.String.Format("{0:dd/MM/yyyy HH:mm}", dayHospitalize)).Length - 3);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public ServiceReqGroupByDateADO()
        {

        }
    }
}

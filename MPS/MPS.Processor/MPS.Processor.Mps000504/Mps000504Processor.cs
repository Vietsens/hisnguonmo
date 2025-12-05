using Inventec.Core;
using MPS.Processor.Mps000504.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000504
{
    public class Mps000504Processor : AbstractProcessor
    {
        Mps000504PDO rdo;

        //private List<Mps000504PDO> listSereServ = new List<Mps000504PDO>();

        public Mps000504Processor(CommonParam param, PrintData printData)
    : base(param, printData)
        {
            rdo = (Mps000504PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessPrintLogData();
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                //Inventec.Common.Mapper.DataObjectMapper.Map<Mps000504PDO>(listSereServ, rdo.HisSereServ);
                objectTag.AddObjectData(store, "SereServs", rdo.HisSereServ.Where(o=>o.TDL_INTRUCTION_TIME > rdo.fromDateReq && o.TDL_INTRUCTION_TIME < rdo.toDateReq).ToList());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (this.rdo.Treatment != null)
                {
                    AddObjectKeyIntoListkey(this.rdo.Treatment, false);
                    SetSingleKey(new KeyValue(Mps000504ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Treatment.TDL_HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000504ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Treatment.TDL_HEIN_CARD_TO_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000504ExtendSingleKey.STR_HEIN_CARD_NUMBER, rdo.Treatment.TDL_HEIN_CARD_NUMBER));
                    SetSingleKey(new KeyValue(Mps000504ExtendSingleKey.GENDER_NAME, rdo.Treatment.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue("PATIENT_CODE", rdo.Treatment.TDL_PATIENT_CODE));
                    if (rdo.toDateReq == long.MaxValue)
                    {
                        SetSingleKey(new KeyValue("TIME_TO_STR", ""));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue("TIME_FROM_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)rdo.fromDateReq)));
                    }
                    if (rdo.fromDateReq == long.MinValue)
                    {
                        SetSingleKey(new KeyValue("TIME_FROM_STR", ""));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue("TIME_TO_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)rdo.toDateReq)));
                    }
                    SetSingleKey(new KeyValue("DOB", rdo.Treatment.TDL_PATIENT_DOB));

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

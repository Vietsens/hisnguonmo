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
                objectTag.AddObjectData(store, "Surcharge", SurchargeProcess()); // PTTK 2656 - mục 4.2.8

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        // PTTK 2656 - mục 4.2.8: tạo danh sách dòng phụ phí (SURCHARGE_AMOUNT > 0)
        private List<MPS.Processor.Mps000504.ADO.SurchargeADO> SurchargeProcess()
        {
            List<MPS.Processor.Mps000504.ADO.SurchargeADO> result = new List<MPS.Processor.Mps000504.ADO.SurchargeADO>();
            try
            {
                if (rdo.SurchargePayforms == null || rdo.SurchargePayforms.Count == 0)
                    return result;

                int stt = 1;
                foreach (var item in rdo.SurchargePayforms.Where(o => (o.SURCHARGE_AMOUNT ?? 0) > 0).OrderBy(o => o.SORT_ORDER ?? 0))
                {
                    result.Add(new MPS.Processor.Mps000504.ADO.SurchargeADO()
                    {
                        STT = stt++,
                        SURCHARGE_NAME = item.SURCHARGE_NAME,
                        AMOUNT = 1,
                        SURCHARGE_AMOUNT = item.SURCHARGE_AMOUNT ?? 0,
                        SORT_ORDER = item.SORT_ORDER
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                // PTTK 2656 - mục 4.2.8: key tổng phụ phí + nhãn section (504 là danh sách phẳng, không đánh số section)
                decimal totalSurcharge = (rdo.SurchargePayforms != null) ? rdo.SurchargePayforms.Where(o => (o.SURCHARGE_AMOUNT ?? 0) > 0).Sum(o => o.SURCHARGE_AMOUNT ?? 0) : 0;
                int surchargeCount = (rdo.SurchargePayforms != null) ? rdo.SurchargePayforms.Count(o => (o.SURCHARGE_AMOUNT ?? 0) > 0) : 0;
                SetSingleKey(new KeyValue("TOTAL_SURCHARGE", Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalSurcharge, 0)));
                SetSingleKey(new KeyValue("TOTAL_SURCHARGE_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(totalSurcharge).ToString())));
                SetSingleKey(new KeyValue("SURCHARGE_COUNT", surchargeCount));
                SetSingleKey(new KeyValue("SURCHARGE_SECTION_LABEL", surchargeCount > 0 ? "Phụ phí" : ""));

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
                        SetSingleKey(new KeyValue("TIME_FROM_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)rdo.fromDateReq)));
                    }
                    if (rdo.fromDateReq == long.MinValue)
                    {
                        SetSingleKey(new KeyValue("TIME_FROM_STR", ""));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue("TIME_TO_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)rdo.toDateReq)));
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

using Inventec.Core;
using MPS.Processor.Mps000504.ADO;
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
    public partial class Mps000504Processor : AbstractProcessor
    {
        Mps000504PDO rdo;

        // PTTK 2883 - muc 2: du lieu gom nhom theo khoa/phong xu ly (port tu Mps000304 — temp 6556)
        private List<SereServADO> sereServADOs_ExeRoom { get; set; }
        private List<HeinServiceTypeADO> heinServiceTypeADOs_ExeRoom { get; set; }
        private List<HeinServiceTypeADO> HeinServiceTypeBeds_ExeRoom { get; set; }
        private List<PatyAlterBhytADO> patyAlterBHYTADOs_ExeRoom { get; set; }
        private List<MedicineLineADO> medicineLineADOs_ExeRoom { get; set; }
        private List<GroupDepartmentADO> GroupDepartments_ExeRoom { get; set; }
        private List<GroupDepartmentADO> GroupDepartments_DepaRoom { get; set; }

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

                // PTTK 2883 - muc 2: keys gom nhom theo khoa/phong xu ly nhu Mps000304 (temp 6556):
                // ReqExeDepaRoom, ReqExeRoom, HeinServiceTypeExeRoom, MedicineLineExeRoom,
                // HeinServiceTypeBedExeRoom, ServiceExeRoom, PatyAlterBHYTExeRoom
                if (rdo.SereServs != null && rdo.SereServs.Count > 0 && rdo.PatientTypeCFG != null && rdo.TreatmentView != null)
                {
                    // Chi lay dich vu trong khoang thoi gian loc [fromDateReq, toDateReq]
                    rdo.SereServs = rdo.SereServs
                        .Where(o => o.TDL_INTRUCTION_TIME >= (rdo.fromDateReq ?? 0)
                                 && o.TDL_INTRUCTION_TIME <= (rdo.toDateReq ?? long.MaxValue))
                        .ToList();
                    DataInputProcess_ExeRoom();
                    GroupDisplayProcess_ExeRoom();
                }
                else
                {
                    // Behavior chua truyen input ExeRoom -> xuat danh sach rong de temp dung key khong bi loi
                    this.sereServADOs_ExeRoom = new List<SereServADO>();
                    this.heinServiceTypeADOs_ExeRoom = new List<HeinServiceTypeADO>();
                    this.HeinServiceTypeBeds_ExeRoom = new List<HeinServiceTypeADO>();
                    this.patyAlterBHYTADOs_ExeRoom = new List<PatyAlterBhytADO>();
                    this.medicineLineADOs_ExeRoom = new List<MedicineLineADO>();
                    this.GroupDepartments_ExeRoom = new List<GroupDepartmentADO>();
                    this.GroupDepartments_DepaRoom = new List<GroupDepartmentADO>();
                }

                objectTag.AddObjectData(store, "ReqExeDepaRoom", GroupDepartments_DepaRoom);
                objectTag.AddObjectData(store, "ReqExeRoom", GroupDepartments_ExeRoom);
                objectTag.AddObjectData(store, "HeinServiceTypeExeRoom", heinServiceTypeADOs_ExeRoom);
                objectTag.AddObjectData(store, "HeinServiceTypeBedExeRoom", HeinServiceTypeBeds_ExeRoom);
                objectTag.AddObjectData(store, "ServiceExeRoom", sereServADOs_ExeRoom);
                objectTag.AddObjectData(store, "PatyAlterBHYTExeRoom", patyAlterBHYTADOs_ExeRoom);
                objectTag.AddObjectData(store, "MedicineLineExeRoom", medicineLineADOs_ExeRoom);

                objectTag.AddRelationship(store, "ReqExeRoom", "ServiceExeRoom", "GROUP_ROOM_ID__ExeRoom", "GROUP_ROOM_ID__ExeRoom");
                objectTag.AddRelationship(store, "ReqExeRoom", "HeinServiceTypeExeRoom", "GROUP_ROOM_ID__ExeRoom", "GROUP_ROOM_ID__ExeRoom");
                objectTag.AddRelationship(store, "ReqExeRoom", "HeinServiceTypeBedExeRoom", "GROUP_ROOM_ID__ExeRoom", "GROUP_ROOM_ID__ExeRoom");
                objectTag.AddRelationship(store, "ReqExeRoom", "MedicineLineExeRoom", "GROUP_ROOM_ID__ExeRoom", "GROUP_ROOM_ID__ExeRoom");
                objectTag.AddRelationship(store, "HeinServiceTypeExeRoom", "ServiceExeRoom", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeExeRoom", "HeinServiceTypeBedExeRoom", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeExeRoom", "MedicineLineExeRoom", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "MedicineLineExeRoom", "ServiceExeRoom", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLineExeRoom", "HeinServiceTypeBedExeRoom", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeBedExeRoom", "ServiceExeRoom", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");

                objectTag.AddRelationship(store, "PatyAlterBHYTExeRoom", "ServiceExeRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTExeRoom", "HeinServiceTypeExeRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTExeRoom", "HeinServiceTypeBedExeRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTExeRoom", "MedicineLineExeRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTExeRoom", "ReqExeRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTExeRoom", "ReqExeDepaRoom", "KEY", "KEY_PATY_ALTER");

                objectTag.AddRelationship(store, "ReqExeDepaRoom", "ReqExeRoom", "GROUP_DEPARTMENT_ID__DepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom");
                objectTag.AddRelationship(store, "ReqExeDepaRoom", "ServiceExeRoom", "GROUP_DEPARTMENT_ID__DepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom");
                objectTag.AddRelationship(store, "ReqExeDepaRoom", "HeinServiceTypeExeRoom", "GROUP_DEPARTMENT_ID__DepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom");
                objectTag.AddRelationship(store, "ReqExeDepaRoom", "HeinServiceTypeBedExeRoom", "GROUP_DEPARTMENT_ID__DepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom");
                objectTag.AddRelationship(store, "ReqExeDepaRoom", "MedicineLineExeRoom", "GROUP_DEPARTMENT_ID__DepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom");

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

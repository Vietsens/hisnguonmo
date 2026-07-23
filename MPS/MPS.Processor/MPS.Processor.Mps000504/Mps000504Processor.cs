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

        // PTTK 2883 - muc 2: du lieu gom nhom theo khoa/phong xu ly (port tu Mps000508 — bo mau 697)
        private PatientADO patientADO { get; set; }
        private List<PatyAlterBhytADO> patyAlterBHYTADOs { get; set; }
        private List<SereServADO> sereServADOs { get; set; }
        private List<HeinServiceTypeADO> heinServiceTypeADOs { get; set; }
        private List<MedicineLineADO> medicineLineADOs { get; set; }
        private List<HeinServiceTypeADO> HeinServiceTypeBeds { get; set; }
        private List<CDHACount> CDHACountList { get; set; }
        private List<OtherSourceADO> ListOtherSource = new List<OtherSourceADO>();

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

                // PTTK 2883 - muc 2: bo key gom nhom theo khoa/phong xu ly nhu Mps000508 (bo mau 697 —
                // thay the 6556 vien khong dung nua): Service, HeinServiceType, PatyAlterBHYT(All),
                // MedicineLine, HeinServiceTypeBed, CDHACountList, ServiceExeRoom, ServiceExeRoomByDepa,
                // ServiceGroupByDepa, ServiceGroupByRoom, HeinServiceTypeExeRoom, HeinServiceTypeByDepa
                if (rdo.SereServs != null && rdo.SereServs.Count > 0 && rdo.PatientTypeCFG != null && rdo.TreatmentView != null)
                {
                    // Chi lay dich vu trong khoang thoi gian loc [fromDateReq, toDateReq]
                    rdo.SereServs = rdo.SereServs
                        .Where(o => o.TDL_INTRUCTION_TIME >= (rdo.fromDateReq ?? 0)
                                 && o.TDL_INTRUCTION_TIME <= (rdo.toDateReq ?? long.MaxValue))
                        .ToList();
                    DataInputProcess();
                    GroupDisplayProcess();
                    ExeRoomProcess();
                }

                // Behavior cu chua truyen input (hoac khong co DV trong khoang loc) -> danh sach rong,
                // temp co dung key cung khong loi; bieu in phang giu nguyen nhu cu.
                if (sereServADOs == null) sereServADOs = new List<SereServADO>();
                if (heinServiceTypeADOs == null) heinServiceTypeADOs = new List<HeinServiceTypeADO>();
                if (patyAlterBHYTADOs == null) patyAlterBHYTADOs = new List<PatyAlterBhytADO>();
                if (medicineLineADOs == null) medicineLineADOs = new List<MedicineLineADO>();
                if (HeinServiceTypeBeds == null) HeinServiceTypeBeds = new List<HeinServiceTypeADO>();
                if (CDHACountList == null) CDHACountList = new List<CDHACount>();
                if (sereServADOs_ExeRoom == null) sereServADOs_ExeRoom = new List<SereServADO>();
                if (heinServiceTypeADOs_ExeRoom == null) heinServiceTypeADOs_ExeRoom = new List<HeinServiceTypeADO>();
                if (heinServiceTypeADOs_ExeRoomByDepa == null) heinServiceTypeADOs_ExeRoomByDepa = new List<HeinServiceTypeADO>();
                if (ServiceGroupByDepa == null) ServiceGroupByDepa = new List<GroupDepartmentADO>();
                if (ServiceGroupByRoom == null) ServiceGroupByRoom = new List<GroupDepartmentADO>();
                if (sereServADOs_ExeRoomByDepa == null) sereServADOs_ExeRoomByDepa = new List<SereServADO>();

                // Bo key nen (nhu Mps000508)
                objectTag.AddObjectData(store, "HeinServiceType", heinServiceTypeADOs);
                objectTag.AddObjectData(store, "Service", sereServADOs);
                objectTag.AddObjectData(store, "PatyAlterBHYT", patyAlterBHYTADOs);
                objectTag.AddObjectData(store, "PatyAlterBHYTAll", patyAlterBHYTADOs);
                objectTag.AddObjectData(store, "MedicineLine", medicineLineADOs);
                objectTag.AddObjectData(store, "HeinServiceTypeBed", HeinServiceTypeBeds);
                objectTag.AddObjectData(store, "CDHACountList", this.CDHACountList);

                objectTag.AddRelationship(store, "PatyAlterBHYT", "Service", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "HeinServiceType", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "HeinServiceTypeBed", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "MedicineLine", "KEY", "KEY_PATY_ALTER");

                objectTag.AddRelationship(store, "HeinServiceType", "Service", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "HeinServiceTypeBed", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "MedicineLine", "ID", "HEIN_SERVICE_TYPE_ID");

                objectTag.AddRelationship(store, "MedicineLine", "Service", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLine", "HeinServiceTypeBed", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeBed", "Service", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");

                #region Bo gom theo khoa/phong xu ly (ExeRoom) — giong Mps000508. Template khong dung thi vo hai.
                // ServiceExeRoom = bo dich vu dedup CO phong (khac "Service" goc dedup khong phong).
                objectTag.AddObjectData(store, "ServiceExeRoom", sereServADOs_ExeRoom);
                // ServiceExeRoomByDepa = bo dich vu chi tiet dedup KHONG phong (gom theo khoa) -> template gom theo khoa bind ten nay de so luong cong don qua cac phong.
                objectTag.AddObjectData(store, "ServiceExeRoomByDepa", sereServADOs_ExeRoomByDepa);
                objectTag.AddObjectData(store, "ServiceGroupByDepa", this.ServiceGroupByDepa);
                objectTag.AddObjectData(store, "ServiceGroupByRoom", this.ServiceGroupByRoom);
                objectTag.AddObjectData(store, "HeinServiceTypeExeRoom", heinServiceTypeADOs_ExeRoom.OrderBy(o => o.NUM_ORDER ?? 99999999).ToList());
                // Loai dich vu gom theo KHOA (khong tach phong) - template gom theo khoa bind ten nay thay cho HeinServiceTypeExeRoom.
                objectTag.AddObjectData(store, "HeinServiceTypeByDepa", heinServiceTypeADOs_ExeRoomByDepa.OrderBy(o => o.NUM_ORDER ?? 99999999).ToList());

                // Nested: ServiceGroupByDepa (khoa) -> ServiceGroupByRoom (phong) -> HeinServiceTypeExeRoom (loai dv) -> ServiceExeRoom (chi tiet).
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "ServiceExeRoom", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "ServiceGroupByRoom", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "ServiceExeRoom", "GROUP_ROOM_ID", "GROUP_ROOM_ID");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "HeinServiceTypeExeRoom", "GROUP_ROOM_ID", "GROUP_ROOM_ID__ExeRoom");
                objectTag.AddRelationship(store, "HeinServiceTypeExeRoom", "ServiceExeRoom", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeExeRoom", "MedicineLine", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeExeRoom", "HeinServiceTypeBed", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "HeinServiceTypeExeRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "ServiceExeRoom", "KEY", "KEY_PATY_ALTER");
                // Subtotal khoa/phong phai gan theo tung doi tuong BHYT - neu thieu, tong tien bi cong gop & lap qua moi doi tuong.
                objectTag.AddRelationship(store, "PatyAlterBHYT", "ServiceGroupByDepa", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "ServiceGroupByRoom", "KEY", "KEY_PATY_ALTER");

                // Nhanh gom theo KHOA: ServiceGroupByDepa (khoa) -> HeinServiceTypeByDepa (loai dv gom theo khoa) -> ServiceExeRoomByDepa (chi tiet gom theo khoa).
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "HeinServiceTypeByDepa", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeByDepa", "MedicineLine", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeByDepa", "HeinServiceTypeBed", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "HeinServiceTypeByDepa", "KEY", "KEY_PATY_ALTER");

                // Chi tiet gom theo KHOA (dedup KHONG phong): moi (dv, khoa, loai) chi 1 dong, so luong da cong qua cac phong.
                objectTag.AddRelationship(store, "HeinServiceTypeByDepa", "ServiceExeRoom", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "ServiceExeRoomByDepa", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeByDepa", "ServiceExeRoomByDepa", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "ServiceExeRoomByDepa", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "MedicineLine", "ServiceExeRoomByDepa", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeBed", "ServiceExeRoomByDepa", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");

                // Phu ngang quan he cua "Service" goc cho "ServiceExeRoom": breakdown thuoc / giuong duoi moi dich vu.
                objectTag.AddRelationship(store, "MedicineLine", "ServiceExeRoom", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeBed", "ServiceExeRoom", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");
                #endregion

                objectTag.SetUserFunction(store, "ReplaceValue", new ReplaceValueFunction());

                objectTag.AddObjectData(store, "OtherPaySource", this.ListOtherSource);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        // PTTK 2883 - muc 2: tong tien theo nguon khac chi tra (port tu Mps000508)
        private void ProcessOtherSource(List<SereServADO> sereServADOs)
        {
            try
            {
                if (sereServADOs != null && sereServADOs.Count > 0 && rdo.ListOtherPaySource != null && rdo.ListOtherPaySource.Count > 0)
                {
                    this.ListOtherSource = new List<OtherSourceADO>();
                    var otherGroup = sereServADOs.GroupBy(o => o.OTHER_PAY_SOURCE_ID).ToList();
                    foreach (var item in otherGroup)
                    {
                        var otherPaySource = rdo.ListOtherPaySource.FirstOrDefault(o => o.ID == item.Key);
                        if (otherPaySource != null)
                        {
                            OtherSourceADO ado = new OtherSourceADO();
                            ado.OTHER_PAY_SOURCE_CODE = otherPaySource.OTHER_PAY_SOURCE_CODE;
                            ado.OTHER_PAY_SOURCE_NAME = otherPaySource.OTHER_PAY_SOURCE_NAME;
                            ado.TOTAL_PRICE = item.Sum(s => s.OTHER_SOURCE_PRICE ?? 0);
                            ado.TOTAL_PRICE_STR = Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(ado.TOTAL_PRICE).ToString());
                            this.ListOtherSource.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.ListOtherSource = new List<OtherSourceADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // PTTK 2883 - muc 2: user function cho template (port tu Mps000508)
        class ReplaceValueFunction : FlexCel.Report.TFlexCelUserFunction
        {
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                try
                {
                    string value = parameters[0] + "";
                    if (!String.IsNullOrEmpty(value))
                    {
                        value = value.Replace(';', '/');
                    }
                    return value;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    return parameters[0];
                }
            }
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

using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000508.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000508.ADO;
using System.IO;
using HIS.Desktop.Common.BankQrCode;

namespace MPS.Processor.Mps000508
{
    public partial class Mps000508Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        private List<PatyAlterBhytADO> patyAlterBHYTADOs { get; set; }
        private List<SereServADO> sereServADOs { get; set; }
        private List<HeinServiceTypeADO> heinServiceTypeADOs { get; set; }
        private List<MedicineLineADO> medicineLineADOs { get; set; }
        private List<HeinServiceTypeADO> HeinServiceTypeBeds { get; set; }
        private List<CDHACount> CDHACountList { get; set; }
        private List<OtherSourceADO> ListOtherSource = new List<OtherSourceADO>();
        private PrintData printData;

        Mps000508PDO rdo;
        public Mps000508Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000508PDO)rdoBase;
            this.printData = printData;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.Treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000508ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetImageKey()
        {
            try
            {
                bool isBhytAndAvtNull = true;
                if (rdo.Patient != null && !String.IsNullOrEmpty(rdo.Patient.AVATAR_URL))
                {
                    SetSingleImage(Mps000508ExtendSingleKey.IMG_AVATAR, rdo.Patient.AVATAR_URL);
                    isBhytAndAvtNull = false;
                }

                if (rdo.CurrentPatyAlter != null && !String.IsNullOrEmpty(rdo.CurrentPatyAlter.BHYT_URL))
                {
                    SetSingleImage(Mps000508ExtendSingleKey.IMG_BHYT, rdo.CurrentPatyAlter.BHYT_URL);
                    isBhytAndAvtNull = false;
                }
                else if (rdo.Patient != null && !String.IsNullOrEmpty(rdo.Patient.BHYT_URL))
                {
                    SetSingleImage(Mps000508ExtendSingleKey.IMG_BHYT, rdo.Patient.BHYT_URL);
                    isBhytAndAvtNull = false;
                }

                if (isBhytAndAvtNull)
                {
                    SetSingleKey(Mps000508ExtendSingleKey.AVT_AND_BHYT_NULL, "1");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetSingleImage(string key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                if (stream != null)
                {
                    SetSingleKey(new KeyValue(key, stream.ToArray()));
                }
                else
                {
                    SetSingleKey(new KeyValue(key, ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag(); 
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                DataInputProcess();
                GroupDisplayProcess(); 
                ExeRoomProcess();  
                ProcessSingleKey(); 
                SetQrCode();
                SetBarcodeKey();
                SetTreatmentQrCodeBase();
                SetImageKey();
                ProcessPrintLogData();
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                if (sereServADOs == null || sereServADOs.Count == 0)
                    return false;
                if (CDHACountList == null)
                {
                    CDHACountList = new List<CDHACount>();
                }

                if (heinServiceTypeADOs == null)
                {
                    heinServiceTypeADOs = new List<HeinServiceTypeADO>();
                }

                if (sereServADOs == null)
                {
                    sereServADOs = new List<SereServADO>();
                }

                if (patyAlterBHYTADOs == null)
                {
                    patyAlterBHYTADOs = new List<PatyAlterBhytADO>();
                }

                if (medicineLineADOs == null)
                {
                    medicineLineADOs = new List<MedicineLineADO>();
                }

                if (HeinServiceTypeBeds == null)
                {
                    HeinServiceTypeBeds = new List<HeinServiceTypeADO>();
                }

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
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

                objectTag.AddObjectData(store, "Surcharge", SurchargeProcess()); // PTTK 2656 

                #region Bộ gom theo phòng xử lý (ExeRoom) - port từ Mps000512 / Mps000304. Template không dùng thì vô hại.
                if (sereServADOs_ExeRoom == null) sereServADOs_ExeRoom = new List<SereServADO>();
                if (heinServiceTypeADOs_ExeRoom == null) heinServiceTypeADOs_ExeRoom = new List<HeinServiceTypeADO>();
                if (ServiceGroupByDepa == null) ServiceGroupByDepa = new List<MPS.Processor.Mps000508.ADO.GroupDepartmentADO>();
                if (ServiceGroupByRoom == null) ServiceGroupByRoom = new List<MPS.Processor.Mps000508.ADO.GroupDepartmentADO>();

                // ServiceExeRoom = bộ dịch vụ dedup CÓ phòng (khác "Service" gốc dedup không phòng).  
                objectTag.AddObjectData(store, "ServiceExeRoom", sereServADOs_ExeRoom);
                objectTag.AddObjectData(store, "ServiceGroupByDepa", this.ServiceGroupByDepa);
                objectTag.AddObjectData(store, "ServiceGroupByRoom", this.ServiceGroupByRoom);
                objectTag.AddObjectData(store, "HeinServiceTypeExeRoom", heinServiceTypeADOs_ExeRoom);

                // Nested: ServiceGroupByDepa (khoa) -> ServiceGroupByRoom (phòng) -> HeinServiceTypeExeRoom (loại dv) -> ServiceExeRoom (chi tiết). 
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "ServiceExeRoom", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "ServiceGroupByRoom", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "ServiceExeRoom", "GROUP_ROOM_ID", "GROUP_ROOM_ID");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "HeinServiceTypeExeRoom", "GROUP_ROOM_ID", "GROUP_ROOM_ID__ExeRoom");
                objectTag.AddRelationship(store, "HeinServiceTypeExeRoom", "ServiceExeRoom", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeExeRoom", "MedicineLine", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeExeRoom", "HeinServiceTypeBed", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "HeinServiceTypeExeRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "ServiceExeRoom", "KEY", "KEY_PATY_ALTER");
                // Subtotal khoa/phòng phải gắn theo từng đối tượng BHYT - nếu thiếu, tổng tiền bị cộng gộp & lặp qua mọi đối tượng (giống Mps000304).
                objectTag.AddRelationship(store, "PatyAlterBHYT", "ServiceGroupByDepa", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "ServiceGroupByRoom", "KEY", "KEY_PATY_ALTER");

                // Phủ ngang quan hệ của "Service" gốc cho "ServiceExeRoom": breakdown thuốc / giường dưới mỗi dịch vụ.
                // MedicineLine / HeinServiceTypeBed dùng chung (không theo phòng) - master theo ID nên resolve đúng theo từng dòng. 
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

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

        private decimal? GetCoPaidAccumulateAmount(object patyAlter)
        {
            try
            {
                if (patyAlter == null) return null;
                var prop = patyAlter.GetType().GetProperty("CO_PAID_ACCUMULATE_AMOUNT");
                if (prop != null)
                {
                    var v = prop.GetValue(patyAlter, null);
                    if (v != null)
                    {
                        return Convert.ToDecimal(v);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private static string GetStringPropertyByReflection(object source, string propertyName)
        {
            try
            {
                if (source == null) return null;
                var prop = source.GetType().GetProperty(propertyName);
                if (prop != null)
                {
                    return prop.GetValue(source, null) as string;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private void SetSingleKey697_Section11()
        {
            try
            {
                string heinPatientTypeCode = GetStringPropertyByReflection(rdo.Treatment, "HEIN_PATIENT_TYPE_CODE");
                if (String.IsNullOrWhiteSpace(heinPatientTypeCode))
                {
                    heinPatientTypeCode = GetStringPropertyByReflection(rdo.CurrentPatyAlter, "HEIN_PATIENT_TYPE_CODE");
                }
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.HEIN_PATIENT_TYPE_CODE_697, heinPatientTypeCode ?? ""));

                if (rdo.Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.MEDI_ORG_NAME_697, rdo.Treatment.MEDI_ORG_NAME));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TRANSFER_IN_MEDI_ORG_NAME_697, rdo.Treatment.TRANSFER_IN_MEDI_ORG_NAME));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TRANSFER_IN_MEDI_ORG_CODE_697, rdo.Treatment.TRANSFER_IN_MEDI_ORG_CODE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetSingleKey697_CoPaidAccumulate()
        {
            try
            {
                decimal? amount = GetCoPaidAccumulateAmount(rdo.CurrentPatyAlter);

                if ((!amount.HasValue || amount.Value <= 0)
                    && rdo.PatientTypeAlterAlls != null
                    && rdo.PatientTypeAlterAlls.Count > 0)
                {
                    var latest = rdo.PatientTypeAlterAlls
                        .Where(o => o.IS_ACTIVE == 1)
                        .OrderByDescending(o => o.LOG_TIME)
                        .FirstOrDefault();
                    if (latest != null)
                    {
                        amount = GetCoPaidAccumulateAmount(latest);
                    }
                }

                if (amount.HasValue && amount.Value > 0)
                {
                    string amountStr = Inventec.Common.Number.Convert.NumberToStringRoundAuto(amount.Value, 0);
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CO_PAID_ACCUMULATE_AMOUNT_697, amount.Value));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CO_PAID_ACCUMULATE_AMOUNT_STR_697, amountStr));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CO_PAID_ACCUMULATE_AMOUNT, amount.Value));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CO_PAID_ACCUMULATE_AMOUNT_STR, amountStr));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CO_PAID_ACCUMULATE_AMOUNT_697, ""));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CO_PAID_ACCUMULATE_AMOUNT_STR_697, ""));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CO_PAID_ACCUMULATE_AMOUNT, ""));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CO_PAID_ACCUMULATE_AMOUNT_STR, ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetSingleKey697_TypeIndex()
        {
            try
            {
                if (rdo.CurrentPatyAlter == null) return;
                string typeIndex697 = "";
                switch (rdo.CurrentPatyAlter.TREATMENT_TYPE_ID)
                {
                    case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM:
                        typeIndex697 = "01";
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU:
                        typeIndex697 = "02";
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU:
                        typeIndex697 = "03";
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY:
                        typeIndex697 = "04";
                        break;
                    default:
                        typeIndex697 = "";
                        break;
                }
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TYPE_INDEX_697, typeIndex697));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetSingleKey697_TreatmentResult()
        {
            try
            {
                if (rdo.Treatment == null) return;

                string nameResult = !String.IsNullOrWhiteSpace(rdo.Treatment.TREATMENT_END_TYPE_NAME)
                    ? rdo.Treatment.TREATMENT_END_TYPE_NAME
                    : rdo.Treatment.TREATMENT_RESULT_NAME;
                string codeResult = !String.IsNullOrWhiteSpace(rdo.Treatment.TREATMENT_END_TYPE_CODE)
                    ? rdo.Treatment.TREATMENT_END_TYPE_CODE
                    : rdo.Treatment.TREATMENT_RESULT_CODE;

                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TREATMENT_RESULT_NAME_697, nameResult ?? ""));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TREATMENT_RESULT_CODE_697, codeResult ?? ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<SurchargeADO> SurchargeProcess()

        {

            List<SurchargeADO> r = new List<SurchargeADO>();

            try {

                if (rdo.SurchargePayforms == null || rdo.SurchargePayforms.Count == 0) return r;

                int stt = 1;

                foreach (var item in rdo.SurchargePayforms.Where(o => (o.SURCHARGE_AMOUNT ?? 0) > 0).OrderBy(o => o.SORT_ORDER ?? 0))

                    r.Add(new SurchargeADO { STT = stt++, SURCHARGE_NAME = item.SURCHARGE_NAME, AMOUNT = 1, SURCHARGE_AMOUNT = item.SURCHARGE_AMOUNT ?? 0, SORT_ORDER = item.SORT_ORDER });

            } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }

            return r;

        }


        void ProcessSingleKey()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("508 reach 879, ICD=" + (rdo.Treatment != null ? rdo.Treatment.ICD_CODE : "null"));

                AddObjectKeyIntoListkey<PatientADO>(patientADO);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment, false);
                
                if (rdo.SingleKeyValue != null)
                {
                    if (!String.IsNullOrEmpty(rdo.SingleKeyValue.roomName))
                    {
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.ROOM_NAME, rdo.SingleKeyValue.roomName));
                    }

                    if (!String.IsNullOrEmpty(rdo.SingleKeyValue.departmentName))
                    {
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.DEPARTMENT_NAME, rdo.SingleKeyValue.departmentName));
                    }

                    AddObjectKeyIntoListkey(rdo.SingleKeyValue, false);
                }

                if (rdo.Treatment != null)
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TREATMENT_CODE, rdo.Treatment.TREATMENT_CODE));

                if (rdo.CurrentPatyAlter != null)
                {
                    int? typeIndex = null;
                    string typeName = "";

                    HIS_TREATMENT_TYPE treatmentType = rdo.TreatmentTypes != null ? rdo.TreatmentTypes.FirstOrDefault(o => o.ID == rdo.CurrentPatyAlter.TREATMENT_TYPE_ID) : null;

                    if (treatmentType != null)
                    {
                        try
                        {
                            typeIndex = (int)treatmentType.ID;
                            typeName = treatmentType.TREATMENT_TYPE_NAME.ToUpper();
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        switch (rdo.CurrentPatyAlter.TREATMENT_TYPE_ID)
                        {
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM:
                                typeIndex = 1;
                                typeName = "KHÁM BỆNH";
                                break;
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU:
                                typeIndex = 2;
                                typeName = "ĐIỀU TRỊ NGOẠI TRÚ";
                                break;
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU:
                                typeIndex = 3;
                                typeName = "ĐIỀU TRỊ NỘI TRÚ";
                                break;
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__NHANTHUOC:
                                typeIndex = 6;
                                typeName = "NHẬN THUỐC THEO HẸN (KHÔNG KHÁM BỆNH)";
                                break;
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__TYTXA:
                                typeIndex = 5;
                                typeName = "ĐIỀU TRỊ LƯU TẠI TYT XÃ, PKĐKKV";
                                break;
                            default:
                                typeIndex = null;
                                typeName = "(KHÔNG XÁC ĐỊNH ĐƯỢC ĐỐI TƯỢNG)";
                                break;
                        }
                    }
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TYPE_INDEX, typeIndex));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TYPE_NAME, typeName));

                    if (rdo.CurrentPatyAlter.FREE_CO_PAID_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.FREE_CO_PAID_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.FREE_CO_PAID_TIME.Value)));
                    }
                }

                SetSingleKey697_TypeIndex();

                if (patyAlterBHYTADOs != null && patyAlterBHYTADOs.Count > 0)
                {
                    string heinMediOrgCode = string.Join(";", patyAlterBHYTADOs.Select(s => s.HEIN_MEDI_ORG_CODE).Distinct().OrderBy(o => o).ToList());
                    string heinMediOrgName = string.Join(";", patyAlterBHYTADOs.Select(s => s.HEIN_MEDI_ORG_NAME).Distinct().OrderBy(o => o).ToList());

                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.HEIN_MEDI_ORG_CODE, heinMediOrgCode));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.HEIN_MEDI_ORG_NAME, heinMediOrgName));

                    PatyAlterBhytADO patyAlterBhytADO = patyAlterBHYTADOs.OrderBy(o => o.LOG_TIME).FirstOrDefault(o => !String.IsNullOrEmpty(o.HEIN_CARD_NUMBER));
                    if (patyAlterBhytADO != null)
                    {
                        HIS_BRANCH branch = rdo.Branch != null ? rdo.Branch : new HIS_BRANCH();
                        string province = !String.IsNullOrWhiteSpace(patyAlterBhytADO.HEIN_MEDI_ORG_CODE) ? patyAlterBhytADO.HEIN_MEDI_ORG_CODE.Substring(0, 2) : "";
                        HIS_MEDI_ORG mediOrg = rdo.ListMediOrg != null && rdo.ListMediOrg.Count > 0 ? rdo.ListMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == patyAlterBhytADO.HEIN_MEDI_ORG_CODE) : new HIS_MEDI_ORG();

                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.IS_HEIN, "X"));
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.HEIN_CARD_ADDRESS, patyAlterBhytADO.ADDRESS));

                        string RIGHT_ROUTE_TYPE_NAME_CC = "";
                        string RIGHT_ROUTE_TYPE_NAME = "";
                        string NOT_RIGHT_ROUTE_TYPE_NAME = "";
                        string RIGHT_ROUTE_TYPE_NAME_TT = "";

                        if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                            {
                                RIGHT_ROUTE_TYPE_NAME_CC = "X";
                            }
                            else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER)
                            {
                                RIGHT_ROUTE_TYPE_NAME_TT = "X";
                            }
                            else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)
                            {
                                RIGHT_ROUTE_TYPE_NAME = "X";
                            }
                            else if (patyAlterBhytADO.HEIN_MEDI_ORG_CODE == branch.HEIN_MEDI_ORG_CODE || (!string.IsNullOrWhiteSpace(branch.ACCEPT_HEIN_MEDI_ORG_CODE) && branch.ACCEPT_HEIN_MEDI_ORG_CODE.Contains(patyAlterBhytADO.HEIN_MEDI_ORG_CODE)))
                            {
                                RIGHT_ROUTE_TYPE_NAME = "X";
                            }
                            else if (branch.HEIN_LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT || branch.HEIN_LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE
                                )
                            {
                                if (province == branch.HEIN_PROVINCE_CODE && mediOrg != null && (mediOrg.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT || mediOrg.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE))
                                {
                                    RIGHT_ROUTE_TYPE_NAME_TT = "X";
                                }
                                else
                                {
                                    NOT_RIGHT_ROUTE_TYPE_NAME = "X";
                                }
                            }
                            else
                            {
                                RIGHT_ROUTE_TYPE_NAME = "X";
                                if (rdo.Treatment.MEDI_ORG_CODE != rdo.CurrentPatyAlter.HEIN_MEDI_ORG_CODE)
                                    SetSingleKey(new KeyValue("THONG_TUYEN", "X"));
                            }
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)
                        {
                            NOT_RIGHT_ROUTE_TYPE_NAME = "X";
                        }

                        SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME_CC", RIGHT_ROUTE_TYPE_NAME_CC));
                        SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME", RIGHT_ROUTE_TYPE_NAME));
                        SetSingleKey(new KeyValue("NOT_RIGHT_ROUTE_TYPE_NAME", NOT_RIGHT_ROUTE_TYPE_NAME));
                        SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME_TT", RIGHT_ROUTE_TYPE_NAME_TT));
                    }
                    else
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.IS_NOT_HEIN, "X"));
                }
                else
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.IS_NOT_HEIN, "X"));

                //SetSingleKey697_Section11();
                //SetSingleKey697_CoPaidAccumulate();
                if (rdo.PatientTypeAlterAlls != null && rdo.CurrentPatyAlter != null)
                {
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CO_PAID_ACCUMULATE_AMOUNT, rdo.PatientTypeAlterAlls.Where(o => o.ID == rdo.CurrentPatyAlter.ID).FirstOrDefault().CO_PAID_ACCUMULATE_AMOUNT));
                }
                
                if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                {
                    if (rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME ?? 0)));
                    }

                    if (rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1] != null && rdo.DepartmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1].DEPARTMENT_NAME));
                    }
                }

                if (rdo.Treatment != null)
                {
                    if (rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (rdo.Treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.OUT_TIME.Value)));
                    }

                    if (rdo.Treatment.END_DEPARTMENT_ID.HasValue)
                    {
                        HIS_DEPARTMENT department = rdo.Departments != null ? rdo.Departments.FirstOrDefault(o => o.ID == rdo.Treatment.END_DEPARTMENT_ID.Value) : null;
                        if (department != null)
                        {
                            SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.DEPARTMENT_BHYT_CODE, department.BHYT_CODE));
                            SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.END_DEPARTMENT_CODE, department.DEPARTMENT_CODE));
                            SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.END_DEPARTMENT_NAME, department.DEPARTMENT_NAME));
                        }
                    }

                    int? genderIndex = null;
                    string genderName;
                    switch (rdo.Treatment.TDL_PATIENT_GENDER_ID)
                    {
                        case IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE:
                            genderIndex = 1;
                            genderName = rdo.Treatment.TDL_PATIENT_GENDER_NAME;
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE:
                            genderIndex = 2;
                            genderName = rdo.Treatment.TDL_PATIENT_GENDER_NAME;
                            break;
                        default:
                            genderIndex = 3;
                            genderName = "Không xác định";
                            break;
                    }

                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.GENDER_INDEX, genderIndex));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.GENDER_NAME, genderName));

                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.Treatment.TRANSFER_IN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.Treatment.TRANSFER_IN_MEDI_ORG_NAME));

                    if (rdo.Treatment.TREATMENT_RESULT_ID.HasValue || rdo.Treatment.TREATMENT_RESULT_ID.HasValue)
                    {
                        int treatmentResultIndex = 2;
                        if (rdo.Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO
                            || rdo.Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI
                            || rdo.Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                            || rdo.Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                            || rdo.Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV)
                        {
                            treatmentResultIndex = 1;
                            if ((rdo.Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO
                            || rdo.Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI) &&
                                (rdo.Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN))
                            {
                                treatmentResultIndex = 2;
                            }
                        }
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TREATMENT_RESULT_INDEX, treatmentResultIndex));
                    }

                    SetSingleKey697_TreatmentResult();

                    if (rdo.Treatment.OUT_TIME.HasValue)
                    {
                        long inTime = 0;
                        if (rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                        {
                            inTime = rdo.Treatment.CLINICAL_IN_TIME.Value;
                        }
                        else
                        {
                            inTime = rdo.Treatment.IN_TIME;
                        }

                        System.DateTime? dateBefore = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(inTime);
                        System.DateTime? dateAfter = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo.Treatment.OUT_TIME.Value);
                        if (dateBefore != null && dateAfter != null)
                        {
                            TimeSpan difference = dateAfter.Value - dateBefore.Value;

                            if (difference.Days > 1 || (difference.Days == 1 && (difference.Hours >= 1 || difference.Minutes >= 1 || difference.Seconds >= 1)))
                            {
                                if (rdo.Treatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                                {
                                    DateTime before = new DateTime(dateBefore.Value.Year, dateBefore.Value.Month, dateBefore.Value.Day);
                                    DateTime after = new DateTime(dateAfter.Value.Year, dateAfter.Value.Month, dateAfter.Value.Day);

                                    int diffDay = (int)(after - before).TotalDays + 1;

                                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TREATMENT_DAY_COUNT_6556, diffDay));
                                }
                            }
                            else if (rdo.Treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                            {
                                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TREATMENT_DAY_COUNT_6556, 1));
                            }
                        }
                    }
                }

                string executeRoomExam = "";
                string executeRoomExamFirst = "";
                string executeRoomExamLast = "";

                decimal thanhtien_tong = 0;
                decimal thanhtienBH_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;
                long totalNumberFilm = 0;
                decimal tongtienbenhnhantutra = 0;
                decimal tongTienBenhNhan = 0;

                decimal tongtienbenhnhantutra_new = 0;
                decimal thanhtien_tong_new = 0;

                if (sereServADOs != null && sereServADOs.Count > 0)
                {
                    var sereServExamADOs = sereServADOs.Where(o => o.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__EXAM_ID).OrderBy(o => o.CREATE_TIME).ToList();

                    if (sereServExamADOs != null && sereServExamADOs.Count > 0)
                    {
                        executeRoomExamFirst = sereServExamADOs[0].EXECUTE_ROOM_NAME;
                        executeRoomExamLast = sereServExamADOs[sereServExamADOs.Count - 1].EXECUTE_ROOM_NAME;

                        foreach (var sereServExamADO in sereServExamADOs)
                        {
                            executeRoomExam += sereServExamADO.EXECUTE_ROOM_NAME + ", ";
                        }
                    }

                    thanhtienBH_tong = sereServADOs.Sum(o => o.TOTAL_PRICE_BHYT);
                    thanhtien_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    bhytthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT) ?? 0;
                    nguonkhac_tong = sereServADOs.Sum(o => o.OTHER_SOURCE_PRICE) ?? 0;
                    tongtienbenhnhantutra = sereServADOs.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);

                    thanhtien_tong_new = sereServADOs.Sum(o => o.TOTAL_PRICE_VP);
                    tongtienbenhnhantutra_new = sereServADOs.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);

                    totalNumberFilm = (long)sereServADOs.Sum(o => ((o.NUMBER_OF_FILM ?? 0) * o.AMOUNT));
                    if (totalNumberFilm > 0)
                    {
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_NUMBER_FILM, totalNumberFilm));
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_NUMBER_FILM_STR, String.Format("Bệnh nhân đã nhận đủ số phim . Số phim {0}", totalNumberFilm)));
                    }

                    tongTienBenhNhan = sereServADOs.Sum(o => o.TOTAL_PRICE_PATIENT_NO_PAY_RATE ?? 0);
                }
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_DAY, rdo.SingleKeyValue.TOTAL_DAY));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.EXECUTE_ROOM_EXAM, executeRoomExam));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.FIRST_EXAM_ROOM_NAME, executeRoomExamFirst));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.LAST_EXAM_ROOM_NAME, executeRoomExamLast));

                // PTTK 2656 - muc 4.2.8
                decimal totalSurcharge = (rdo.SurchargePayforms != null) ? rdo.SurchargePayforms.Where(o => (o.SURCHARGE_AMOUNT ?? 0) > 0).Sum(o => o.SURCHARGE_AMOUNT ?? 0) : 0;
                int surchargeCount = (rdo.SurchargePayforms != null) ? rdo.SurchargePayforms.Count(o => (o.SURCHARGE_AMOUNT ?? 0) > 0) : 0;
                int surchargeSectionNo = (heinServiceTypeADOs != null ? heinServiceTypeADOs.Count : 0) + 1;
                thanhtien_tong += totalSurcharge;
                tongtienbenhnhantutra += totalSurcharge;
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_SURCHARGE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalSurcharge, 0)));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_SURCHARGE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(totalSurcharge).ToString())));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.SURCHARGE_COUNT, surchargeCount));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.SURCHARGE_SECTION_NO, surchargeSectionNo));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.SURCHARGE_SECTION_LABEL, surchargeCount > 0 ? (surchargeSectionNo + ". Phu phi") : ""));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtienBH_tong, 0)));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_PATIENT_SELF, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongtienbenhnhantutra, 0)));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                decimal tongNguoiBenhTra_697 = bnthanhtoan_tong + tongtienbenhnhantutra;
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_PATIENT_ALL_697, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongNguoiBenhTra_697, 0)));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_PATIENT_ALL_TEXT_697, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(tongNguoiBenhTra_697).ToString())));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_PATIENT_NO_PAY_RATE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongTienBenhNhan, 0)));
                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_PRICE_PATIENT_NO_PAY_RATE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(tongTienBenhNhan).ToString())));

                SetSingleKey(new KeyValue("TOTAL_PRICE_NEW", Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong_new, 0)));
                SetSingleKey(new KeyValue("TOTAL_PATIENT_PRICE_LEFT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongtienbenhnhantutra_new, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_NEW_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong_new).ToString())));
                if (rdo.TreatmentFees != null && rdo.TreatmentFees.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.TreatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));

                    decimal totalPrice = 0;
                    decimal totalHeinPrice = 0;
                    decimal totalPatientPrice = 0;
                    decimal totalDeposit = 0;
                    decimal totalBill = 0;
                    decimal totalBillTransferAmount = 0;
                    decimal totalRepay = 0;
                    decimal exemption = 0;
                    decimal depositPlus = 0;
                    decimal total_obtained_price = 0;

                    totalPrice = rdo.TreatmentFees[0].TOTAL_PRICE ?? 0;
                    totalHeinPrice = rdo.TreatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                    totalPatientPrice = rdo.TreatmentFees[0].TOTAL_PATIENT_PRICE ?? 0;
                    totalDeposit = rdo.TreatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                    totalBill = rdo.TreatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                    totalBillTransferAmount = rdo.TreatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                    exemption = 0;
                    totalRepay = rdo.TreatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                    total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);
                    decimal transfer = totalPatientPrice - total_obtained_price;
                    depositPlus = transfer;

                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TREATMENT_FEE_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0)));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TREATMENT_FEE_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPatientPrice, 0)));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TREATMENT_FEE_TOTAL_OBTAINED_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(total_obtained_price, 0)));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TREATMENT_FEE_TRANSFER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(transfer, 0)));
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(rdo.TreatmentFees[0], false);
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0")); 
                }

                SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                

                if (rdo.CurrentPatyAlter != null)
                {
                    AddObjectKeyIntoListkey<PatyAlterBhytADO>(DataRawProcess.PatyAlterBHYTRawToADO(rdo.CurrentPatyAlter, rdo.Branch, rdo.TreatmentTypes), false);
                    if (rdo.CurrentPatyAlter.HEIN_CARD_FROM_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.HEIN_CARD_FROM_TIME ?? 0)));
                    if (rdo.CurrentPatyAlter.HEIN_CARD_TO_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.HEIN_CARD_TO_TIME ?? 0)));
                    if (rdo.CurrentPatyAlter.JOIN_5_YEAR_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.JOIN_5_YEAR_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.JOIN_5_YEAR_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.RATIO_STR, DataRawProcess.GetDefaultHeinRatioForView(rdo.CurrentPatyAlter.HEIN_CARD_NUMBER, rdo.CurrentPatyAlter.HEIN_TREATMENT_TYPE_CODE, rdo.Branch != null ? rdo.Branch.HEIN_LEVEL_CODE : null, rdo.CurrentPatyAlter.RIGHT_ROUTE_CODE)));
                    SetSingleKey(new KeyValue(Mps000508ExtendSingleKey.LIVE_AREA_CODE, rdo.CurrentPatyAlter.LIVE_AREA_CODE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetQrCode()
        {
            try
            {
                if (rdo.TransReq != null && rdo.ListHisConfigPaymentQrCode != null && rdo.ListHisConfigPaymentQrCode.Count > 0)
                {
                    var data = QrCodeProcessor.CreateQrImage(rdo.TransReq, rdo.ListHisConfigPaymentQrCode);
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            SetSingleKey(new KeyValue(item.Key, item.Value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã điều trị: " + rdo.Treatment.TREATMENT_CODE;
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.Treatment != null)
                {
                    string printTypeCode = printData.printTypeCode;
                    if (rdo.Treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        printTypeCode = "Mps000509";
                    }
                    result = String.Format("{0} TREATMENT_CODE:{1}", printTypeCode, rdo.Treatment.TREATMENT_CODE);
                }

            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

    }
}

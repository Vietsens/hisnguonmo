/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
using MPS.Processor.Mps000306.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000306.ADO;
using System.IO;

namespace MPS.Processor.Mps000306
{
    public partial class Mps000306Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        private List<PatyAlterBhytADO> patyAlterBHYTADOs { get; set; }
        private List<SereServADO> sereServADOs { get; set; }
        private List<HeinServiceTypeADO> heinServiceTypeADOs { get; set; }
        private List<MedicineLineADO> medicineLineADOs { get; set; }
        private List<HeinServiceTypeADO> HeinServiceTypeBeds { get; set; }
        private List<GroupDepartmentADO> GroupDepartments { get; set; }
        private List<OtherSourceADO> ListOtherSource = new List<OtherSourceADO>();

        private List<PatyAlterBhytADO> patyAlterBHYTADOs_DepaRoom { get; set; }
        private List<GroupDepartmentADO> ServiceGroupByDepa { get; set; }
        private List<GroupDepartmentADO> ServiceGroupByRoom { get; set; }
        private List<HeinServiceTypeADO> heinServiceTypeADOs_DepaRoom { get; set; }
        private List<HeinServiceTypeADO> HeinServiceTypeBeds_DepaRoom { get; set; }
        private List<MedicineLineADO> medicineLineADOs_DepaRoom { get; set; }
        private List<SereServADO> sereServADOs_DepaRoom { get; set; }

        private List<SereServADO> sereServADOs_ExeRoom { get; set; }
        private List<HeinServiceTypeADO> heinServiceTypeADOs_ExeRoom { get; set; }
        private List<HeinServiceTypeADO> HeinServiceTypeBeds_ExeRoom { get; set; }
        private List<PatyAlterBhytADO> patyAlterBHYTADOs_ExeRoom { get; set; }
        private List<MedicineLineADO> medicineLineADOs_ExeRoom { get; set; }
        private List<GroupDepartmentADO> GroupDepartments_ExeRoom { get; set; }
        private List<GroupDepartmentADO> GroupDepartments_DepaRoom { get; set; }

        private List<SereServADO> sereServADOs_WithBhyt { get; set; }
        Mps000306PDO rdo;
        public Mps000306Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000306PDO)rdoBase;
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

                dicImage.Add(Mps000306ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
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
                    SetSingleImage(Mps000306ExtendSingleKey.IMG_AVATAR, rdo.Patient.AVATAR_URL);
                    isBhytAndAvtNull = false;
                }

                if (rdo.CurrentPatyAlter != null && !String.IsNullOrEmpty(rdo.CurrentPatyAlter.BHYT_URL))
                {
                    SetSingleImage(Mps000306ExtendSingleKey.IMG_BHYT, rdo.CurrentPatyAlter.BHYT_URL);
                    isBhytAndAvtNull = false;
                }
                else if (rdo.Patient != null && !String.IsNullOrEmpty(rdo.Patient.BHYT_URL))
                {
                    SetSingleImage(Mps000306ExtendSingleKey.IMG_BHYT, rdo.Patient.BHYT_URL);
                    isBhytAndAvtNull = false;
                }

                if (isBhytAndAvtNull)
                {
                    SetSingleKey(Mps000306ExtendSingleKey.AVT_AND_BHYT_NULL, "1");
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

                #region Issue 58420: Bổ sung danh sách khoa và chi tiết dịch vụ mới gom nhóm dịch vụ theo khoa xử lý và phòng xử lý
                DataInputProcess_DepaRoom();
                GroupDisplayProcess_DepaRoom();
                #endregion

                #region Issue 65834: Bổ sung danh sách gom nhóm theo phòng
                DataInputProcess_ExeRoom();
                GroupDisplayProcess_ExeRoom();
                #endregion

                DataInputProcess_WithBhyt();

                ProcessSingleKey();
                SetBarcodeKey();
                SetImageKey();
                if (sereServADOs == null || sereServADOs.Count == 0)
                    return false;

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "HeinServiceType", heinServiceTypeADOs);
                objectTag.AddObjectData(store, "Service", sereServADOs);
                objectTag.AddObjectData(store, "PatyAlterBHYT", patyAlterBHYTADOs);
                objectTag.AddObjectData(store, "PatyAlterBHYTAll", patyAlterBHYTADOs.Where(o => !String.IsNullOrEmpty(o.HEIN_CARD_NUMBER)).ToList());
                objectTag.AddObjectData(store, "MedicineLine", medicineLineADOs);
                objectTag.AddObjectData(store, "HeinServiceTypeBed", HeinServiceTypeBeds);
                objectTag.AddObjectData(store, "ReqExeDepartment", GroupDepartments);

                objectTag.AddRelationship(store, "PatyAlterBHYT", "Service", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "HeinServiceType", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "HeinServiceTypeBed", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "MedicineLine", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYT", "ReqExeDepartment", "KEY", "KEY_PATY_ALTER");

                objectTag.AddRelationship(store, "ReqExeDepartment", "Service", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ReqExeDepartment", "HeinServiceType", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ReqExeDepartment", "HeinServiceTypeBed", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ReqExeDepartment", "MedicineLine", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");

                objectTag.AddRelationship(store, "HeinServiceType", "Service", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "HeinServiceTypeBed", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "MedicineLine", "ID", "HEIN_SERVICE_TYPE_ID");

                objectTag.AddRelationship(store, "MedicineLine", "Service", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLine", "HeinServiceTypeBed", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeBed", "Service", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");

                #region Bổ sung danh sách chi tiết có dịch vụ BHYT và có phụ thu
                objectTag.AddObjectData(store, "ServiceWithPrimary", sereServADOs_WithBhyt);
                objectTag.AddObjectData(store, "HeinServiceTypeWithPrimary", heinServiceTypeADOs_WithBhyt);
                objectTag.AddObjectData(store, "HeinServiceTypeBedWithPrimary", HeinServiceTypeBeds_WithBhyt);
                objectTag.AddObjectData(store, "PatyAlterBHYTWithPrimary", patyAlterBHYTADOs_WithBhyt);
                objectTag.AddObjectData(store, "ReqExeDepartmentWithPrimary", GroupDepartments_WithBhyt);

                objectTag.AddRelationship(store, "PatyAlterBHYTWithPrimary", "ServiceWithPrimary", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTWithPrimary", "HeinServiceTypeWithPrimary", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTWithPrimary", "HeinServiceTypeBedWithPrimary", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTWithPrimary", "ReqExeDepartmentWithPrimary", "KEY", "KEY_PATY_ALTER");

                objectTag.AddRelationship(store, "ReqExeDepartmentWithPrimary", "ServiceWithPrimary", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ReqExeDepartmentWithPrimary", "HeinServiceTypeWithPrimary", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ReqExeDepartmentWithPrimary", "HeinServiceTypeBedWithPrimary", "GROUP_DEPARTMENT_ID", "GROUP_DEPARTMENT_ID");

                objectTag.AddRelationship(store, "HeinServiceTypeWithPrimary", "ServiceWithPrimary", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeWithPrimary", "HeinServiceTypeBedWithPrimary", "ID", "PARENT_ID");

                objectTag.AddRelationship(store, "HeinServiceTypeBedWithPrimary", "ServiceWithPrimary", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");
                #endregion

                #region Issue 58420: Bổ sung danh sách khoa và chi tiết dịch vụ mới gom nhóm dịch vụ theo khoa xử lý và phòng xử lý
                objectTag.AddObjectData(store, "ServiceGroupByDepa", ServiceGroupByDepa);
                objectTag.AddObjectData(store, "ServiceGroupByRoom", ServiceGroupByRoom);
                objectTag.AddObjectData(store, "HeinServiceTypeDepaRoom", heinServiceTypeADOs_DepaRoom);
                objectTag.AddObjectData(store, "HeinServiceTypeBedDepaRoom", HeinServiceTypeBeds_DepaRoom);
                objectTag.AddObjectData(store, "ServiceDepaRoom", sereServADOs_DepaRoom);
                objectTag.AddObjectData(store, "PatyAlterBHYTDepaRoom", patyAlterBHYTADOs_DepaRoom);
                objectTag.AddObjectData(store, "MedicineLineDepaRoom", medicineLineADOs_DepaRoom);

                objectTag.AddRelationship(store, "ServiceGroupByDepa", "ServiceDepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "ServiceGroupByRoom", "GROUP_DEPARTMENT_ID__DepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "HeinServiceTypeDepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "HeinServiceTypeBedDepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom");
                objectTag.AddRelationship(store, "ServiceGroupByDepa", "MedicineLineDepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom", "GROUP_DEPARTMENT_ID__DepaRoom");

                objectTag.AddRelationship(store, "ServiceGroupByRoom", "ServiceDepaRoom", "GROUP_ROOM_ID__ExeRoom", "GROUP_ROOM_ID__ExeRoom");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "HeinServiceTypeDepaRoom", "GROUP_ROOM_ID__ExeRoom", "GROUP_ROOM_ID__ExeRoom");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "HeinServiceTypeBedDepaRoom", "GROUP_ROOM_ID__ExeRoom", "GROUP_ROOM_ID__ExeRoom");
                objectTag.AddRelationship(store, "ServiceGroupByRoom", "MedicineLineDepaRoom", "GROUP_ROOM_ID__ExeRoom", "GROUP_ROOM_ID__ExeRoom");
                objectTag.AddRelationship(store, "HeinServiceTypeDepaRoom", "ServiceDepaRoom", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeDepaRoom", "HeinServiceTypeBedDepaRoom", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeDepaRoom", "MedicineLineDepaRoom", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "MedicineLineDepaRoom", "ServiceDepaRoom", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLineDepaRoom", "HeinServiceTypeBedDepaRoom", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "HeinServiceTypeBedDepaRoom", "ServiceDepaRoom", "ID", "HEIN_SERVICE_TYPE_PARENT_1_ID");

                objectTag.AddRelationship(store, "PatyAlterBHYTDepaRoom", "ServiceDepaRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTDepaRoom", "HeinServiceTypeDepaRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTDepaRoom", "HeinServiceTypeBedDepaRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTDepaRoom", "MedicineLineDepaRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTDepaRoom", "ServiceGroupByRoom", "KEY", "KEY_PATY_ALTER");
                objectTag.AddRelationship(store, "PatyAlterBHYTDepaRoom", "ServiceGroupByDepa", "KEY", "KEY_PATY_ALTER");
                #endregion

                #region Issue 65834: Bổ sung danh sách gom nhóm theo phòng
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

        void ProcessSingleKey()
        {
            try
            {

                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.RATIO_STR, rdo.SingleKeyValue.ratio));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TREATMENT_CODE, rdo.Treatment.TREATMENT_CODE));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.USERNAME_RETURN_RESULT, rdo.SingleKeyValue.userNameReturnResult));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.STATUS_TREATMENT_OUT, rdo.SingleKeyValue.statusTreatmentOut));

                if (rdo.CurrentPatyAlter != null)
                {
                    int? typeIndex = null;
                    string typeName;

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
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TYPE_INDEX, typeIndex));
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TYPE_NAME, typeName));

                    if (rdo.CurrentPatyAlter.FREE_CO_PAID_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.FREE_CO_PAID_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.FREE_CO_PAID_TIME.Value)));
                    }
                }


                if (rdo.PatientTypeAlterAlls != null && rdo.PatientTypeAlterAlls.Count > 0)
                {
                    string heinMediOrgCode = null;
                    string heinMediOrgName = null;
                    foreach (var item in rdo.PatientTypeAlterAlls)
                    {
                        heinMediOrgCode += (item.HEIN_MEDI_ORG_CODE + (rdo.PatientTypeAlterAlls.IndexOf(item) < rdo.PatientTypeAlterAlls.Count() - 1 ? "; " : ""));
                        heinMediOrgName += (item.HEIN_MEDI_ORG_NAME + (rdo.PatientTypeAlterAlls.IndexOf(item) < rdo.PatientTypeAlterAlls.Count() - 1 ? "; " : ""));
                    }

                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.HEIN_MEDI_ORG_CODE, heinMediOrgCode));
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.HEIN_MEDI_ORG_NAME, heinMediOrgName));


                    HIS_PATIENT_TYPE_ALTER patyAlterBhytADO = rdo.PatientTypeAlterAlls.OrderBy(o => o.LOG_TIME).FirstOrDefault(o => !String.IsNullOrEmpty(o.HEIN_CARD_NUMBER));
                    if (patyAlterBhytADO != null)
                    {
                        HIS_BRANCH branch = rdo.Branch != null ? rdo.Branch : new HIS_BRANCH();
                        string province = !String.IsNullOrWhiteSpace(patyAlterBhytADO.HEIN_MEDI_ORG_CODE) ? patyAlterBhytADO.HEIN_MEDI_ORG_CODE.Substring(0, 2) : "";
                        HIS_MEDI_ORG mediOrg = rdo.ListMediOrg != null && rdo.ListMediOrg.Count > 0 ? rdo.ListMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == patyAlterBhytADO.HEIN_MEDI_ORG_CODE) : new HIS_MEDI_ORG();

                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.IS_HEIN, "X"));
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.HEIN_CARD_ADDRESS, patyAlterBhytADO.ADDRESS));

                        string RIGHT_ROUTE_TYPE_NAME_CC = "";
                        string RIGHT_ROUTE_TYPE_NAME = "";
                        string NOT_RIGHT_ROUTE_TYPE_NAME = "";
                        string RIGHT_ROUTE_TYPE_NAME_TT = "";

                        if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
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
                        else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                        {
                            NOT_RIGHT_ROUTE_TYPE_NAME = "X";
                        }

                        SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME_CC", RIGHT_ROUTE_TYPE_NAME_CC));
                        SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME", RIGHT_ROUTE_TYPE_NAME));
                        SetSingleKey(new KeyValue("NOT_RIGHT_ROUTE_TYPE_NAME", NOT_RIGHT_ROUTE_TYPE_NAME));
                        SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME_TT", RIGHT_ROUTE_TYPE_NAME_TT));
                    }
                    else
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.IS_NOT_HEIN, "X"));
                }
                else
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                {
                    if (rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME ?? 0)));
                    }
                    if (rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1] != null && rdo.DepartmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1].DEPARTMENT_NAME));
                    }

                }

                if (!String.IsNullOrEmpty(rdo.SingleKeyValue.roomName))
                {
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.ROOM_NAME, rdo.SingleKeyValue.roomName));
                }

                if (rdo.Treatment != null)
                {
                    if (rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (rdo.Treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.OUT_TIME.Value)));
                    }

                    if (rdo.Treatment.END_DEPARTMENT_ID.HasValue)
                    {
                        HIS_DEPARTMENT department = rdo.Departments.FirstOrDefault(o => o.ID == rdo.Treatment.END_DEPARTMENT_ID.Value);
                        if (department != null)
                        {
                            SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.DEPARTMENT_BHYT_CODE, department.BHYT_CODE));
                            SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.END_DEPARTMENT_CODE, department.DEPARTMENT_CODE));
                            SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.END_DEPARTMENT_NAME, department.DEPARTMENT_NAME));
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

                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.GENDER_INDEX, genderIndex));
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.GENDER_NAME, genderName));

                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.Treatment.TRANSFER_IN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.Treatment.TRANSFER_IN_MEDI_ORG_NAME));

                    //Tình trạng ra viện
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
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TREATMENT_RESULT_INDEX, treatmentResultIndex));
                    }
                    if (rdo.Treatment.IS_PAUSE == 1 && rdo.listTransaction != null && rdo.listTransaction.Count > 0)
                    {
                        var lstTransactions = rdo.listTransaction.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && o.IS_CANCEL != 1 && o.IS_DIRECTLY_BILLING != 1 && o.SALE_TYPE_ID == null).ToList();
                        if (lstTransactions != null && lstTransactions.Count > 0)
                        {
                            var transaction = lstTransactions.OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                            SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.LAST_TRANSACTION_AMOUNT, transaction.AMOUNT));
                        }
                    }
                }

                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_DAY, rdo.SingleKeyValue.today));

                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.SingleKeyValue.currentDateSeparateFullTime));



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
                    nguonkhac_tong = 0;
                    tongtienbenhnhantutra = sereServADOs.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);

                    thanhtien_tong_new = sereServADOs.Sum(o => o.TOTAL_PRICE_VP);
                    tongtienbenhnhantutra_new = sereServADOs.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);

                    totalNumberFilm = (long)sereServADOs.Sum(o => ((o.NUMBER_OF_FILM ?? 0) * o.AMOUNT));
                    if (totalNumberFilm > 0)
                    {
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_NUMBER_FILM, totalNumberFilm));
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_NUMBER_FILM_STR, String.Format("Bệnh nhân đã nhận đủ số phim . Số phim {0}", totalNumberFilm)));
                    }
                }

                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.EXECUTE_ROOM_EXAM, executeRoomExam));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.FIRST_EXAM_ROOM_NAME, executeRoomExamFirst));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.LAST_EXAM_ROOM_NAME, executeRoomExamLast));


                if (!String.IsNullOrEmpty(rdo.SingleKeyValue.departmentName))
                {
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.DEPARTMENT_NAME, rdo.SingleKeyValue.departmentName));
                }

                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_PRICE_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtienBH_tong, 0)));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_PRICE_PATIENT_SELF, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongtienbenhnhantutra, 0)));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                SetSingleKey(new KeyValue("TOTAL_PRICE_NEW", Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong_new, 0)));
                SetSingleKey(new KeyValue("TOTAL_PATIENT_PRICE_LEFT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongtienbenhnhantutra_new, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_NEW_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong_new).ToString())));

                if (rdo.TreatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.TreatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));

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

                    totalPrice = rdo.TreatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                    totalHeinPrice = rdo.TreatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                    totalPatientPrice = rdo.TreatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                    totalDeposit = rdo.TreatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                    totalBill = rdo.TreatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                    totalBillTransferAmount = rdo.TreatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                    exemption = 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                    totalRepay = rdo.TreatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                    total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                    decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan
                    depositPlus = transfer;//Nop them

                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TREATMENT_FEE_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0)));
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TREATMENT_FEE_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPatientPrice, 0)));
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TREATMENT_FEE_TOTAL_OBTAINED_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(total_obtained_price, 0)));
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TREATMENT_FEE_TRANSFER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(transfer, 0)));
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(rdo.TreatmentFees[0], false);
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }
                SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                AddObjectKeyIntoListkey<PatientADO>(patientADO);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment, false);

                if (rdo.CurrentPatyAlter != null)
                {
                    AddObjectKeyIntoListkey<PatyAlterBhytADO>(DataRawProcess.PatyAlterBHYTRawToADO(rdo.CurrentPatyAlter, rdo.Branch, rdo.TreatmentTypes), false);
                    if (rdo.CurrentPatyAlter.HEIN_CARD_FROM_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.HEIN_CARD_FROM_TIME ?? 0)));
                    if (rdo.CurrentPatyAlter.HEIN_CARD_TO_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.HEIN_CARD_TO_TIME ?? 0)));
                    if (rdo.CurrentPatyAlter.JOIN_5_YEAR_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.JOIN_5_YEAR_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.JOIN_5_YEAR_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.RATIO_STR, DataRawProcess.GetDefaultHeinRatioForView(rdo.CurrentPatyAlter.HEIN_CARD_NUMBER, rdo.CurrentPatyAlter.HEIN_TREATMENT_TYPE_CODE, rdo.Branch.HEIN_LEVEL_CODE, rdo.CurrentPatyAlter.RIGHT_ROUTE_CODE)));
                    SetSingleKey(new KeyValue(Mps000306ExtendSingleKey.LIVE_AREA_CODE, rdo.CurrentPatyAlter.LIVE_AREA_CODE));

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
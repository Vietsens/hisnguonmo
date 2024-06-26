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
using MPS.Processor.Mps000125.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000125.ADO;

namespace MPS.Processor.Mps000125
{
    public partial class Mps000125Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        private PatyAlterBhytADO patyAlterBHYTADO { get; set; }
        private Dictionary<DicKey.SERE_SERV, List<SereServADO>> dicSereServ { get; set; }
        private Dictionary<DicKey.HEIN_SERVICE_TYPE, List<HeinServiceTypeADO>> dicHeinServiceType { get; set; }
        private Dictionary<DicKey.DEPARTMENT, List<DepartmentADO>> dicDepartment { get; set; }

        Mps000125PDO rdo;
        public Mps000125Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000125PDO)rdoBase;
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

                dicImage.Add(Mps000125ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                ProcessGroupSereServ();
                ProcessSingleKey();
                SetBarcodeKey();

                if ((dicSereServ[DicKey.SERE_SERV.NOT_HIGHTECH_VTTT] == null || dicSereServ[DicKey.SERE_SERV.NOT_HIGHTECH_VTTT].Count == 0) && (dicSereServ[DicKey.SERE_SERV.HIGHTECH] == null || dicSereServ[DicKey.SERE_SERV.HIGHTECH].Count == 0) && (dicSereServ[DicKey.SERE_SERV.VTTT] == null || dicSereServ[DicKey.SERE_SERV.VTTT].Count == 0))
                    return false;

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "Services", dicSereServ[DicKey.SERE_SERV.NOT_HIGHTECH_VTTT]);
                objectTag.AddObjectData(store, "ServiceGroups", dicHeinServiceType[DicKey.HEIN_SERVICE_TYPE.NOT_HIGHTECH]);
                objectTag.AddObjectData(store, "Departments", dicDepartment[DicKey.DEPARTMENT.NOT_HIGHTECH]);
                objectTag.AddRelationship(store, "ServiceGroups", "Services", "ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceGroups", "Departments", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "Departments", "Services", "ID", "TDL_REQUEST_DEPARTMENT_ID");
               // objectTag.AddRelationship(store, "ServiceGroups", "Services", "ID", "TDL_HEIN_SERVICE_TYPE_ID");

                objectTag.AddObjectData(store, "HightServiceGroups", dicHeinServiceType[DicKey.HEIN_SERVICE_TYPE.HIGHTECH]);
                objectTag.AddObjectData(store, "HightTechServices", dicSereServ[DicKey.SERE_SERV.HIGHTECH]);
                objectTag.AddObjectData(store, "HightTechDepartments", dicDepartment[DicKey.DEPARTMENT.HIGHTECH]);
                objectTag.AddObjectData(store, "ServiceInHightTechVTTTs", dicSereServ[DicKey.SERE_SERV.VTTT]);

                objectTag.AddRelationship(store, "HightServiceGroups", "HightTechServices", "ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HightTechServices", "ServiceInHightTechVTTTs", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "HightTechDepartments", "ServiceInHightTechVTTTs", "ID", "TDL_REQUEST_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "HightTechDepartments", "HightTechServices", "ID", "TDL_REQUEST_DEPARTMENT_ID");

                objectTag.SetUserFunction(store, "ReplaceValue", new ReplaceValueFunction());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
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

                if (rdo.Bills != null && rdo.Bills.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.CASHIER_LOGINNAME, rdo.Bills[0].CASHIER_LOGINNAME));
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TRANSACTION_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Bills[0].CREATE_TIME.Value)));
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.CASHIER_USERNAME, rdo.Bills[0].CASHIER_USERNAME));
                }
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.RATIO_STR, rdo.SingleKeyValue.ratio));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TREATMENT_CODE, rdo.Treatment.TREATMENT_CODE));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.USERNAME_RETURN_RESULT, rdo.SingleKeyValue.userNameReturnResult));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.STATUS_TREATMENT_OUT, rdo.SingleKeyValue.statusTreatmentOut));
                if (patyAlterBHYTADO != null)
                {
                    if (patyAlterBHYTADO.IS_HEIN != null)
                        SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.IS_HEIN, "X"));
                    else
                        SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.HEIN_CARD_ADDRESS, patyAlterBHYTADO.ADDRESS));
                }
                else
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                {
                    if (rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME ?? 0)));
                    }
                    if (rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1] != null && rdo.DepartmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1].DEPARTMENT_NAME));
                    }
                }

                if (rdo.Treatment != null)
                {
                    if (rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (rdo.Treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.OUT_TIME.Value)));
                    }
                }
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_DAY, rdo.SingleKeyValue.today));

                if (rdo.Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.Treatment.TRANSFER_IN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.Treatment.TRANSFER_IN_MEDI_ORG_NAME));
                }

                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.SingleKeyValue.currentDateSeparateFullTime));

                if (!String.IsNullOrEmpty(rdo.SingleKeyValue.departmentName))
                {
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.DEPARTMENT_NAME, rdo.SingleKeyValue.departmentName));
                }

                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;
                decimal tienbenhnhanphaitra = 0;

                if (dicSereServ[DicKey.SERE_SERV.ALL] != null && dicSereServ[DicKey.SERE_SERV.ALL].Count > 0)
                {
                    string executeRoomExam = "";
                    string executeRoomExamFirst = "";
                    string executeRoomExamLast = "";

                    var sereServExamADOs = dicSereServ[DicKey.SERE_SERV.ALL].Where(o => o.SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__EXAM_ID).OrderBy(o => o.CREATE_TIME).ToList();

                    if (sereServExamADOs != null && sereServExamADOs.Count > 0)
                    {
                        executeRoomExamFirst = sereServExamADOs[0].EXECUTE_ROOM_NAME;
                        executeRoomExamLast = sereServExamADOs[sereServExamADOs.Count - 1].EXECUTE_ROOM_NAME;
                        foreach (var sereServExamADO in sereServExamADOs)
                        {
                            executeRoomExam += sereServExamADO.EXECUTE_ROOM_NAME + ", ";
                        }
                    }

                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.EXECUTE_ROOM_EXAM, executeRoomExam));
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.FIRST_EXAM_ROOM_NAME, executeRoomExamFirst));
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.LAST_EXAM_ROOM_NAME, executeRoomExamLast));

                    thanhtien_tong = dicSereServ[DicKey.SERE_SERV.ALL].Sum(o => o.TOTAL_PRICE_BHYT);
                    bhytthanhtoan_tong = dicSereServ[DicKey.SERE_SERV.ALL].Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = dicSereServ[DicKey.SERE_SERV.ALL].Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT) ?? 0;
                    tienbenhnhanphaitra = dicSereServ[DicKey.SERE_SERV.ALL].Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    nguonkhac_tong = 0;
                }

                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_PRICE_PATIENT_SEFT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tienbenhnhanphaitra, 0)));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_PRICE_PATIENT_SEFT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(tienbenhnhanphaitra).ToString())));
                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (rdo.TreatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.TreatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                SetSingleKey(new KeyValue(Mps000125ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                AddObjectKeyIntoListkey<PatientADO>(patientADO);
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBHYTADO);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using MPS.Processor.Mps000224.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000224.ADO;

namespace MPS.Processor.Mps000224
{
    public partial class Mps000224Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        private List<PatyAlterBhytADO> patyAlterBHYTADOs { get; set; }
        private List<SereServADO> sereServADOs { get; set; }
        private Dictionary<DicKey.TYPE, List<SereServADO>> dicSereServ { get; set; }
        private List<HeinServiceTypeADO> heinServiceTypeADOs { get; set; }

        Mps000224PDO rdo;
        public Mps000224Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000224PDO)rdoBase;
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

                dicImage.Add(Mps000224ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
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
                //HeinServiceTypeProcess();
                ProcessSingleKey();
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                SetBarcodeKey();
                if (sereServADOs == null || sereServADOs.Count == 0)
                    return false;
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "Service", dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT]);
                objectTag.AddObjectData(store, "ServicePTTT", dicSereServ[DicKey.TYPE.SERE_SERV_PTTT]);
                objectTag.AddObjectData(store, "ServiceOther", dicSereServ[DicKey.TYPE.SERE_SERV_OTHER]);
                objectTag.AddObjectData(store, "PatyAlterBHYT", patyAlterBHYTADOs);
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

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.Treatment != null)
                {
                    string treatmentCode = rdo.Treatment.TREATMENT_CODE;
                    string serviceReqCode = "";
                    string serviceCode = "";
                    if (dicSereServ[DicKey.TYPE.SERE_SERV_PTTT] != null && dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Count() > 0)
                    {
                        serviceReqCode = string.Join(", ", dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Select(o => o.TDL_SERVICE_REQ_CODE).ToList());
                        serviceCode = string.Join(", ", dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Select(o => o.SERVICE_CODE).ToList());
                    }
                    result = String.Format("{0} TREATMENT_CODE: {1} SERVICE_REQ_CODE: {2} SERVICE_CODE: {3}", printTypeCode, treatmentCode, serviceReqCode, serviceCode);
                }

            }
            catch (Exception ex)
            {
                result = "";
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
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.DEPARTMENT_NAME, rdo.SingleKeyValue.departmentName));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.EXECUTE_TIME_STR, rdo.SingleKeyValue.executeTimeStr));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.RATIO_STR, rdo.SingleKeyValue.ratio));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TREATMENT_CODE, rdo.Treatment.TREATMENT_CODE));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.USERNAME_RETURN_RESULT, rdo.SingleKeyValue.userNameReturnResult));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.STATUS_TREATMENT_OUT, rdo.SingleKeyValue.statusTreatmentOut));

                if (patyAlterBHYTADOs != null && patyAlterBHYTADOs.Count > 0)
                {
                    string heinMediOrgCode = null;
                    string heinMediOrgName = null;
                    foreach (var item in patyAlterBHYTADOs)
                    {
                        heinMediOrgCode += (item.HEIN_MEDI_ORG_CODE + (patyAlterBHYTADOs.IndexOf(item) < patyAlterBHYTADOs.Count() - 1 ? "; " : ""));
                        heinMediOrgName += (item.HEIN_MEDI_ORG_NAME + (patyAlterBHYTADOs.IndexOf(item) < patyAlterBHYTADOs.Count() - 1 ? "; " : ""));
                    }

                    SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.HEIN_MEDI_ORG_CODE, heinMediOrgCode));
                    SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.HEIN_MEDI_ORG_NAME, heinMediOrgName));


                    PatyAlterBhytADO patyAlterBhytADO = patyAlterBHYTADOs.OrderBy(o => o.LOG_TIME).FirstOrDefault(o => !String.IsNullOrEmpty(o.HEIN_CARD_NUMBER));
                    if (patyAlterBhytADO != null)
                    {
                        SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.IS_HEIN, "X"));
                        SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.HEIN_CARD_ADDRESS, patyAlterBhytADO.ADDRESS));
                        if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                            {
                                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                            }
                            else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                            {
                                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                            }
                            else
                            {
                                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                            }
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                        {
                            SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                        }
                    }
                    else
                        SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.IS_NOT_HEIN, "X"));
                }
                else
                    SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.Treatment != null)
                {
                    if (rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (rdo.Treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.OUT_TIME.Value)));
                    }
                }

                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_DAY, rdo.SingleKeyValue.today));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.SingleKeyValue.currentDateSeparateFullTime));

                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                decimal thanhtien_tong_pttt = 0;
                decimal bhytthanhtoan_tong_pttt = 0;
                decimal bnthanhtoan_tong_pttt = 0;

                decimal thanhtien_tong_has_parent = 0;
                decimal bhytthanhtoan_tong_has_parent = 0;
                decimal bnthanhtoan_tong_has_parent = 0;


                decimal thanhtien_tong_pttt_other = 0;
                decimal bhytthanhtoan_tong_pttt_other = 0;
                decimal bnthanhtoan_tong_pttt_other = 0;
                decimal thanhtien_tong_other = 0;
                decimal bhytthanhtoan_tong_other = 0;
                decimal nguonkhac_tong_other = 0;
                decimal bnthanhtoan_tong_other = 0;

                string titleServiceName = "";
                string titleHeinServiceTypeName = "";

                if (dicSereServ[DicKey.TYPE.SERE_SERV_PTTT] != null && dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Count > 0)
                {
                    thanhtien_tong_pttt = dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    bhytthanhtoan_tong_pttt = dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    bnthanhtoan_tong_pttt = dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);

                    thanhtien_tong += dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    bhytthanhtoan_tong += (dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0));
                    bnthanhtoan_tong += (dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0));
                    nguonkhac_tong += (dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.OTHER_SOURCE_PRICE ?? 0));
                    titleServiceName = dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].FirstOrDefault().SERVICE_NAME.ToUpper();
                    titleHeinServiceTypeName = dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].FirstOrDefault().HEIN_SERVICE_TYPE_NAME;

                    thanhtien_tong_pttt_other += dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    bhytthanhtoan_tong_pttt_other += dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    bnthanhtoan_tong_pttt_other += dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);

                    thanhtien_tong_other += dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    bhytthanhtoan_tong_other += (dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0));
                    bnthanhtoan_tong_other += (dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0));
                    nguonkhac_tong_other += (dicSereServ[DicKey.TYPE.SERE_SERV_PTTT].Sum(o => o.OTHER_SOURCE_PRICE ?? 0));
                }
                else
                {
                    titleServiceName = "KHÔNG TÌM THẤY DỊCH VỤ PHẪU THUẬT THỦ THUẬT";
                    titleHeinServiceTypeName = "không tìm thấy loại dịch vụ";
                }

                if (dicSereServ[DicKey.TYPE.SERE_SERV_OTHER] != null && dicSereServ[DicKey.TYPE.SERE_SERV_OTHER].Count > 0)
                {
                    thanhtien_tong_pttt_other += dicSereServ[DicKey.TYPE.SERE_SERV_OTHER].Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    bhytthanhtoan_tong_pttt_other += dicSereServ[DicKey.TYPE.SERE_SERV_OTHER].Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    bnthanhtoan_tong_pttt_other += dicSereServ[DicKey.TYPE.SERE_SERV_OTHER].Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);

                    thanhtien_tong_other += dicSereServ[DicKey.TYPE.SERE_SERV_OTHER].Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    bhytthanhtoan_tong_other += (dicSereServ[DicKey.TYPE.SERE_SERV_OTHER].Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0));
                    bnthanhtoan_tong_other += (dicSereServ[DicKey.TYPE.SERE_SERV_OTHER].Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0));
                    nguonkhac_tong_other += (dicSereServ[DicKey.TYPE.SERE_SERV_OTHER].Sum(o => o.OTHER_SOURCE_PRICE ?? 0));
                }

                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TITLE_SERVICE_NAME, titleServiceName));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.HEIN_SERVICE_TYPE_NAME, titleHeinServiceTypeName));

                if (dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT] != null && dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Count > 0)
                {
                    thanhtien_tong_has_parent = dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    bhytthanhtoan_tong_has_parent = dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    bnthanhtoan_tong_has_parent = dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);

                    thanhtien_tong += dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    bhytthanhtoan_tong += (dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0));
                    bnthanhtoan_tong += (dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0));
                    nguonkhac_tong += (dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.OTHER_SOURCE_PRICE ?? 0));

                    thanhtien_tong_other += dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    bhytthanhtoan_tong_other += (dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0));
                    bnthanhtoan_tong_other += (dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0));
                    nguonkhac_tong_other += (dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT].Sum(o => o.OTHER_SOURCE_PRICE ?? 0));
                }

                if (!String.IsNullOrEmpty(rdo.SingleKeyValue.departmentName))
                {
                    SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.DEPARTMENT_NAME, rdo.SingleKeyValue.departmentName));
                }

                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_PTTT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong_pttt, 0)));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_HEIN_PTTT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong_pttt, 0)));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_PATIENT_PTTT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong_pttt, 0)));

                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_HAS_PARENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong_has_parent, 0)));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_HEIN_HAS_PARENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong_has_parent, 0)));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_PATIENT_HAS_PARENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong_has_parent, 0)));


                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));


                SetSingleKey(new KeyValue("TOTAL_PRICE_PTTT_OTHER", Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong_pttt_other, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_HEIN_PTTT_OTHER", Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong_pttt_other, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_PATIENT_PTTT_OTHER", Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong_pttt_other, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_WITH_OTHER", Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong_other, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_HEIN_OTHER", Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong_other, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_PATIENT_OTHER", Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong_other, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_OTHER_NEW", Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong_other, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_WITH_OTHER_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong_other).ToString())));
                SetSingleKey(new KeyValue("TOTAL_PRICE_HEIN_OTHER_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong_other).ToString())));
                SetSingleKey(new KeyValue("TOTAL_PRICE_PATIENT_OTHER_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong_other).ToString())));
                SetSingleKey(new KeyValue("TOTAL_PRICE_OTHER_NEW_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong_other).ToString())));

                SetSingleKey(new KeyValue(Mps000224ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                AddObjectKeyIntoListkey<PatientADO>(patientADO);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

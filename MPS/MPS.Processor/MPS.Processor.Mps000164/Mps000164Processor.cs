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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000164.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000164
{
    public class Mps000164Processor : AbstractProcessor
    {
        Mps000164PDO rdo;
        public Mps000164Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000164PDO)rdoBase;
        }

        private void DataInputProcess()
        {
            try
            {
                //Patient ADO
                rdo.patientADO = DataRawProcess.PatientRawToADO(rdo.patient);

                rdo.patyAlterBHYTADO = DataRawProcess.PatyAlterBHYTRawToADO(rdo.patyAlterBHYT);

                rdo.sereServADOs = new List<SereServADO>();
                //SereServADO
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.sereServs
                                          select new SereServADO(r));

                //sereServ Vien Phi
                var sereServVPGroups = sereServADOTemps
                    .Where(o => o.PATIENT_TYPE_CODE != rdo.patientType.PATIENT_TYPE_BHYT_CODE
                    && o.IS_NO_EXECUTE == null
                    && o.IS_EXPEND == null
                    && o.AMOUNT > 0)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.IS_EXPEND
                    }).ToList();

                foreach (var sereServVPGroup in sereServVPGroups)
                {
                    SereServADO sereServ = sereServVPGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServVPGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_PRICE = sereServVPGroup.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServVPGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServVPGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    rdo.sereServADOs.Add(sereServ);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HeinServiceTypeProcess()
        {
            try
            {
                rdo.heinServiceTypes = new List<SereServADO>();

                var sereServBHYTGroups = rdo.sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => o.TDL_HEIN_SERVICE_TYPE_ID).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    SereServADO heinServiceType = new SereServADO();

                    V_HIS_SERE_SERV sereServBHYT = sereServBHYTGroup.FirstOrDefault();
                    if (sereServBHYT.TDL_HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        heinServiceType.TDL_HEIN_SERVICE_TYPE_ID = sereServBHYT.TDL_HEIN_SERVICE_TYPE_ID.Value;
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServBHYT.HEIN_SERVICE_TYPE_NAME;
                    }
                    else
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }

                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE) ?? -999;
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                    rdo.heinServiceTypes.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TREATMENT_CODE, rdo.treatment.TREATMENT_CODE));

                if (rdo.patyAlterBHYTADO != null)
                {
                    if (rdo.patyAlterBHYTADO.IS_HEIN != null)
                        SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.IS_HEIN, "X"));
                    else
                        SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    //Dia chi the
                    SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.patyAlterBHYTADO.ADDRESS));
                }
                else
                    SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    if (rdo.departmentTrans[0].DEPARTMENT_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].DEPARTMENT_IN_TIME.Value)));
                    }
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null && rdo.departmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, rdo.departmentTrans[rdo.departmentTrans.Count - 1].DEPARTMENT_NAME));
                    }
                }

                if (rdo.treatment != null && rdo.treatment.OUT_TIME.HasValue)
                {
                    SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.treatment.OUT_TIME.Value)));
                    SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_DAY, rdo.today));
                }

                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.treatment.TRANSFER_IN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.treatment.TRANSFER_IN_MEDI_ORG_NAME));
                }


                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.currentDateSeparateFullTime));

                if (!String.IsNullOrEmpty(rdo.departmentName))
                {
                    SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
                }

                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (rdo.sereServADOs != null && rdo.sereServADOs.Count > 0)
                {
                    thanhtien_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_PRICE) ?? -999;
                    bhytthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;
                }

                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (rdo.treatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                SetSingleKey(new KeyValue(Mps000164ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);
                if (rdo.patyAlterBHYTADO != null)
                    AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.patyAlterBHYTADO, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.treatment, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.treatment != null && !String.IsNullOrEmpty(rdo.treatment.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.treatment.TREATMENT_CODE);
                    barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatment.IncludeLabel = false;
                    barcodeTreatment.Width = 120;
                    barcodeTreatment.Height = 40;
                    barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatment.IncludeLabel = true;

                    dicImage.Add(Mps000164ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
                }
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
                DataInputProcess();
                HeinServiceTypeProcess();
                SetSingleKey();
                SetBarcodeKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "HeinServiceType", rdo.heinServiceTypes);
                objectTag.AddObjectData(store, "Service", rdo.sereServADOs);
                objectTag.AddRelationship(store, "HeinServiceType", "Service", "TDL_HEIN_SERVICE_TYPE_ID", "TDL_HEIN_SERVICE_TYPE_ID");

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}

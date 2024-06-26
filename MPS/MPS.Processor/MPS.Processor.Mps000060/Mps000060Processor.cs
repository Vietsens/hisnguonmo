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
using MPS.Processor.Mps000060.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000060
{
    public class Mps000060Processor : AbstractProcessor
    {
        Mps000060PDO rdo;
        public Mps000060Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000060PDO)rdoBase;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.PatyAlterBhyt != null)
                {
                    if (rdo.PatyAlterBhyt.IS_HEIN != null)
                        SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.IS_HEIN, "X"));
                    else
                        SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }
                }
                else
                    SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TREATMENT_CODE, rdo.currentTreatment.TREATMENT_CODE));
                }

                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].LOG_TIME)));
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null)
                    {
                        SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[rdo.departmentTrans.Count - 1].LOG_TIME)));
                    }
                }

                if (rdo.hisTranPati != null)
                {
                    SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.hisTranPati.MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.hisTranPati.MEDI_ORG_NAME));
                }

                SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.currentDateSeparateFullTime));


                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (rdo.sereServADO != null && rdo.sereServADO.Count > 0)
                {
                    thanhtien_tong = rdo.sereServADO.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    bhytthanhtoan_tong = rdo.sereServADO.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = rdo.sereServADO.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;
                }

                SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (rdo.treatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                SetSingleKey(new KeyValue(Mps000060ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                AddObjectKeyIntoListkey<PatientADO>(rdo.Patient);
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentTreatment);
                //GlobalQuery.AddObjectKeyIntoListkey<List<ServiceGroupPrintADO>>(highTechServiceReports, keyValues);
                //GlobalQuery.AddObjectKeyIntoListkey<List<SereServADO>>(hightTechServices, keyValues);
                //GlobalQuery.AddObjectKeyIntoListkey<List<V_HIS_SERE_SERV>>(highTechDepartments, keyValues);
                //GlobalQuery.AddObjectKeyIntoListkey<List<SereServADO>>(serviceInHightTechs, keyValues);
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
                if (rdo.currentTreatment != null && !String.IsNullOrEmpty(rdo.currentTreatment.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(Mps000060ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                SetBarcodeKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ServiceGroups", rdo.HeinServiceTypes);
                objectTag.AddObjectData(store, "Services", rdo.sereServADO);
                objectTag.AddRelationship(store, "ServiceGroups", "Services", "ID", "HEIN_SERVICE_TYPE_ID");

                objectTag.AddObjectData(store, "HightServiceGroups", rdo.highTechServiceReports);
                objectTag.AddObjectData(store, "HightTechServices", rdo.hightTechServices);
                objectTag.AddObjectData(store, "HightTechDepartments", rdo.highTechDepartments);
                objectTag.AddObjectData(store, "ServicesInHightTech", rdo.serviceInHightTechs);

                objectTag.AddRelationship(store, "HightServiceGroups", "HightTechServices", "ID", "HEIN_SERVICE_TYPE_ID");
                //objectTag.AddRelationship(store, "HightTechServices", "HightTechDepartments", "REQUEST_DEPARTMENT_ID", "REQUEST_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "HightTechServices", "ServicesInHightTech", "SERVICE_ID", "SERVICE_PACKAGE_ID");

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

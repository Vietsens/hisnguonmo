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
using MPS.Processor.Mps000087.PDO;

namespace MPS.Processor.Mps000087
{
    class Mps000087Processor : AbstractProcessor
    {
        Mps000087PDO rdo;
        public Mps000087Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000087PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000087ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
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

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "rdo.ServiceGroups", rdo.ServiceGroups);
                objectTag.AddObjectData(store, "Services", rdo.sereServADOs);
                objectTag.AddRelationship(store, "rdo.ServiceGroups", "Services", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");

                //var streamResult = store.OutStream();
                //if (streamResult != null)
                //{
                //    streamResult.Position = 0;
                //    PrintPreview(streamResult, this.fileName);
                //}

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                //GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReq, keyValues);
                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.RATIO, rdo.ratio));
                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TREATMENT_CODE, rdo.currentTreatment.TREATMENT_CODE));

                if (rdo.PatyAlterBhyt != null)
                {
                    if (rdo.PatyAlterBhyt.IS_HEIN != null)
                        SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.IS_HEIN, "X"));
                    else
                        SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }
                }
                else
                    SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.IS_NOT_HEIN, "X"));

                //SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.ICD_MAIN_CODE, rdo.currentTreatment.ICD_CODE));
                //SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.ICD_MAIN_TEXT, rdo.currentTreatment.ICD_NAME));

                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].LOG_TIME)));
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null && rdo.departmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[rdo.departmentTrans.Count - 1].LOG_TIME)));
                        //Số ngày điều trị
                        SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_DAY, rdo.totalDay));

                    }

                    //Thời gian vào khoa
                    //Thời gian vào khoa
                    List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTranTemps = new List<V_HIS_DEPARTMENT_TRAN>();
                    foreach (var departmentTran in rdo.departmentTrans)
                    {
                        if (departmentTran != null)
                            departmentTranTemps.Add(departmentTran);

                    }

                    var departmentIns = departmentTranTemps.Where(o => o.IN_OUT == 1).OrderByDescending(o => o.LOG_TIME).ToList();
                    if (departmentIns != null && departmentIns.Count > 0)
                    {
                        var timeDepartmentIn = departmentIns[0].LOG_TIME;
                        SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TIME_DEPARTMENT_IN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeDepartmentIn)));
                    }

                    if (departmentTranTemps.Count == rdo.departmentTrans.Count)
                    {
                        var departmentOuts = rdo.departmentTrans.Where(o => o.IN_OUT == 2).OrderByDescending(o => o.LOG_TIME).ToList();
                        if (departmentOuts != null && departmentOuts.Count > 0)
                        {
                            var timeDepartmentOut = departmentOuts[0].LOG_TIME;

                            SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TIME_DEPARTMENT_OUT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeDepartmentOut)));
                        }
                    }
                }



                if (rdo.hisTranPati != null)
                {
                    SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.hisTranPati.MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.hisTranPati.MEDI_ORG_NAME));
                }

                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.currentDateSeparateFullTime));


                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (rdo.sereServADOs != null && rdo.sereServADOs.Count > 0)
                {
                    thanhtien_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    bhytthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;
                }

                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (rdo.treatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                SetSingleKey(new KeyValue(Mps000087ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName.ToUpper()));


                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentTreatment, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ProcessGroupSereServ()
        {
            try
            {
                rdo.ServiceGroups = new List<SereServGroupPlusADO>();
                var sServceReportGroups = rdo.sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER).GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                foreach (var sServceReportGroup in sServceReportGroups)
                {
                    List<SereServGroupPlusADO> subSServiceReportGroup = sServceReportGroup.ToList<SereServGroupPlusADO>();
                    SereServGroupPlusADO itemSServiceReportGroup = subSServiceReportGroup.First();
                    if (itemSServiceReportGroup.HEIN_SERVICE_TYPE_ID != null)
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = rdo.serviceReports.Where(o => o.ID == subSServiceReportGroup.First().HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                    else
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = "Khác";
                    itemSServiceReportGroup.TOTAL_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    itemSServiceReportGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    itemSServiceReportGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    rdo.ServiceGroups.Add(itemSServiceReportGroup);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

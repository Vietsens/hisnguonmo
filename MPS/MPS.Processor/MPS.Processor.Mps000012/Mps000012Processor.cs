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
using MPS.Processor.Mps000012.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000012
{
    public class Mps000012Processor : AbstractProcessor
    {
        Mps000012PDO rdo;
        public Mps000012Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000012PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000012ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000012ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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

                SetBarcodeKey();
                SetSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ServiceMedicines", rdo.currentExpMestMedicines);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (!String.IsNullOrEmpty(rdo.requestDepartmentName))
                {
                    SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.REQUEST_DEPARTMENT_NAME, rdo.requestDepartmentName)));
                }

                SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.FINISH_TIME_TRAN_PATI, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.maxUserTime))));

                rdo.lstDepartmentTran = rdo.lstDepartmentTran.OrderBy(o => o.LOG_TIME).ToList();
                if (rdo.lstDepartmentTran != null && rdo.lstDepartmentTran.Count > 0)
                {
                    SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.CREATE_TIME_TRAN_PATI, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.lstDepartmentTran[0].LOG_TIME))));
                }
                else
                {
                    SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.CREATE_TIME_TRAN_PATI, "")));
                    SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.FINISH_TIME_TRAN_PATI, "")));
                }

                if (rdo.ExamServiceReqs != null)
                    SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ExamServiceReqs.INTRUCTION_TIME))));
                if (rdo.vHisExamServiceReq1 != null)
                    SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.vHisExamServiceReq1.INTRUCTION_TIME))));

                if (rdo.currentExpMestMedicines != null && rdo.currentExpMestMedicines.Count > 0)
                {
                    long maxUseTimeTo = rdo.currentExpMestMedicines.Max(a => a.USE_TIME_TO ?? 0);
                    SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxUseTimeTo))));

                    SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.TREATMENT_OUT_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxUseTimeTo))));
                }
                else
                {
                    if (rdo.currentTreatment != null && rdo.currentTreatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.TREATMENT_OUT_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.OUT_TIME.Value))));
                    }
                }

                if (rdo.Dhsts != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.Dhsts);
                }
                if (rdo.currentTreatment != null)
                {
                    if (string.IsNullOrEmpty(rdo.currentTreatment.ICD_MAIN_TEXT))
                    {
                        SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.ICD_TREATMENT_NAME, rdo.currentTreatment.ICD_NAME)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.ICD_TREATMENT_NAME, rdo.currentTreatment.ICD_MAIN_TEXT)));
                    }

                    SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.ICD_TREATMENT_CODE, rdo.currentTreatment.ICD_CODE)));
                    SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.ICD_TREATMENT_TEXT, rdo.currentTreatment.ICD_TEXT)));
                }
                if (rdo.tranPaties != null && rdo.tranPaties.TRAN_PATI_TYPE_ID == 1)
                {
                    if (string.IsNullOrEmpty(rdo.tranPaties.ICD_NAME))
                    {
                        SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.ICD_NGT_TEXT, rdo.tranPaties.ICD_NAME)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.ICD_NGT_TEXT, rdo.tranPaties.ICD_MAIN_TEXT)));
                    }
                }

                if (rdo.currentTreatment != null)
                {

                    if (rdo.currentTreatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.TREATMENT_IN_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.CLINICAL_IN_TIME.Value))));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000012ExtendSingleKey.TREATMENT_IN_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.IN_TIME))));
                    }
                }

                if (rdo.ExamServiceReqs != null)
                    AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ>(rdo.ExamServiceReqs, false);
                if (rdo.vHisExamServiceReq1 != null)
                    AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ_1>(rdo.vHisExamServiceReq1, false);
                if (rdo.PatyAlterBhyt != null)
                {
                    SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.HEIN_CARD_NUMBER, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER));
                    SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME)));
                    SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME)));
                    SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo.PatyAlterBhyt.HEIN_MEDI_ORG_CODE));

                    SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.IS_HEIN, "X"));
                    if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }
                    //Dia chi the
                    SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                }
                else
                    SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.Patient != null)
                {
                    SetSingleKey(new KeyValue(Mps000012ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB)));
                }

                AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(rdo.tranPaties, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentTreatment, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(rdo.sereServs, false);
                AddObjectKeyIntoListkey<V_HIS_PATY_ALTER_BHYT>(rdo.PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

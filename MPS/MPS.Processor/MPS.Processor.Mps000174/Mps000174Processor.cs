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
using MPS.Processor.Mps000174.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000174
{
    class Mps000174Processor : AbstractProcessor
    {
        Mps000174PDO rdo;

        public Mps000174Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000174PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;
                dicImage.Add(Mps000174ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetSingleKey()
        {
            try
            {
                if (rdo.patientTypeAlter != null && !String.IsNullOrEmpty(rdo.patientTypeAlter.HEIN_CARD_NUMBER))
                {
                    SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.HEIN_CARD_NUMBER, rdo.patientTypeAlter.HEIN_CARD_NUMBER));
                    SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.patientTypeAlter.HEIN_CARD_TO_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo.patientTypeAlter.HEIN_MEDI_ORG_CODE));

                    SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.IS_HEIN, "X"));
                    if (rdo.patientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.patientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.patientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.patientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }
                    //Dia chi the
                    SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.patientTypeAlter.ADDRESS));
                }
                else
                    SetSingleKey(new KeyValue(Mps000174ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (!String.IsNullOrEmpty(rdo.RequestDepartmentName))
                {
                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.REQUEST_DEPARTMENT_NAME, rdo.RequestDepartmentName)));
                }

                if (rdo.ado != null)
                {
                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.TREATMENT_RESULT_CODE, rdo.ado.TREATMENT_RESULT_CODE)));
                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.TREATMENT_RESULT_NAME, rdo.ado.TREATMENT_RESULT_NAME)));
                }

                rdo.DepartmentTrans = rdo.DepartmentTrans.OrderBy(o => o.DEPARTMENT_IN_TIME).ToList();
                if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                {
                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.CREATE_TIME_TRAN_PATI, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME ?? 0))));
                }
                else
                {
                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.CREATE_TIME_TRAN_PATI, "")));
                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.FINISH_TIME_TRAN_PATI, "")));
                }

                if (rdo.ServiceReq != null)
                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReq.INTRUCTION_TIME))));

                long maxUseTimeTo = 0;
                if (rdo.ExpMestMedicines != null && rdo.ExpMestMedicines.Count > 0)
                {
                    maxUseTimeTo = rdo.ExpMestMedicines.Max(a => a.USE_TIME_TO ?? 0);
                    if (maxUseTimeTo > 0)
                    {
                        SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxUseTimeTo))));
                        SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.TREATMENT_OUT_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxUseTimeTo))));
                    }
                }
                if (rdo.Treatment != null)
                {
                    if (maxUseTimeTo == 0 && rdo.Treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.TREATMENT_OUT_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.OUT_TIME.Value))));
                    }
                    if (string.IsNullOrEmpty(rdo.Treatment.ICD_NAME) && !String.IsNullOrEmpty(rdo.Treatment.TRANSFER_IN_ICD_CODE))
                    {
                        HIS_ICD icd = rdo.Icds.FirstOrDefault(o => rdo.Treatment.TRANSFER_IN_ICD_CODE == o.ICD_CODE);
                        SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.ICD_TREATMENT_NAME, icd != null ? icd.ICD_NAME : "")));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.ICD_TREATMENT_NAME, rdo.Treatment.ICD_NAME)));
                    }
                    if (rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.TREATMENT_IN_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value))));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.TREATMENT_IN_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.IN_TIME))));
                    }
                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.ICD_TREATMENT_CODE, rdo.Treatment.ICD_CODE)));
                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.ICD_TREATMENT_TEXT, rdo.Treatment.ICD_TEXT)));
                }

                if (rdo.Treatment != null)
                {
                    if (!String.IsNullOrEmpty(rdo.Treatment.TRANSFER_IN_ICD_CODE) && rdo.Icds != null && rdo.Icds.Count > 0)
                    {
                        HIS_ICD icd = rdo.Icds.FirstOrDefault(o => rdo.Treatment.TRANSFER_IN_ICD_CODE == o.ICD_CODE);
                        SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.ICD_NGT_TEXT, icd != null ? icd.ICD_NAME : "")));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.ICD_NGT_TEXT, rdo.Treatment.TRANSFER_IN_ICD_NAME)));
                    }
                }

                AddObjectKeyIntoListkey<MPS.Processor.Mps000174.PDO.Mps000174PDO.PatientADO>(new MPS.Processor.Mps000174.PDO.Mps000174PDO.PatientADO(rdo.Patient), false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReq, false);
                AddObjectKeyIntoListkey<HIS_DHST>(rdo.Dhst, false);
                if (rdo.Dhst != null)
                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.DHST_NOTE, rdo.Dhst.NOTE)));
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.Treatment, false);
                AddObjectKeyIntoListkey<MPS.Processor.Mps000174.PDO.Mps000174PDO.Mps000174ADO>(rdo.ado, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.patientTypeAlter);
                
                if (rdo.ListSereServPTTT != null && rdo.ListSereServPTTT.Count > 0)
                {
                    string pttt = "";
                    var groupSS = rdo.ListSereServPTTT.GroupBy(o => o.SERVICE_ID).ToList();
                    foreach (var item in groupSS)
                    {
                        pttt += item.FirstOrDefault().TDL_SERVICE_NAME + " " + string.Format("({0:0.####}", item.Sum(o => o.AMOUNT)) + " lần);";
                    }

                    SetSingleKey((new KeyValue(Mps000174ExtendSingleKey.SERE_SERV_PTTTs, pttt)));
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
                DataInputProcess();
                SetBarcodeKey();
                SetSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                rdo.ExpMestMedicines = rdo.ExpMestMedicines != null ? rdo.ExpMestMedicines : new List<V_HIS_EXP_MEST_MEDICINE>();
                objectTag.AddObjectData(store, "ServiceMedicines", rdo.ExpMestMedicines);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        internal void DataInputProcess()
        {
            try
            {

                var expMestMedicineGroups = rdo.ExpMestMedicines.GroupBy(o =>
                    new { o.MEDICINE_TYPE_ID, o.MEDICINE_ID, o.PRICE, o.IS_EXPEND });
                rdo.ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                foreach (var expMestMedicineGroup in expMestMedicineGroups)
                {
                    V_HIS_EXP_MEST_MEDICINE expMestMedicine = expMestMedicineGroup.First();
                    expMestMedicine.AMOUNT = expMestMedicineGroup.Sum(o => o.AMOUNT);
                    rdo.ExpMestMedicines.Add(expMestMedicine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using MPS.Processor.Mps000382.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000382
{
    public class Mps000382Processor : AbstractProcessor
    {
        Mps000382PDO rdo;
        public Mps000382Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000382PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.currentTreatment != null)
                {
                    if (!String.IsNullOrEmpty(rdo.currentTreatment.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TDL_PATIENT_CODE);
                        barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatientCode.IncludeLabel = false;
                        barcodePatientCode.Width = 120;
                        barcodePatientCode.Height = 40;
                        barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatientCode.IncludeLabel = true;

                        dicImage.Add(Mps000011ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.currentTreatment.TREATMENT_CODE))
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

                        dicImage.Add(Mps000011ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
                    }
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
                SetBarcodeKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetBarcodeKey();
                SetSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "Reasons", rdo.tranpatiReasonADOs);
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
                if (rdo.PatyAlterBhyt != null)
                {
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.HEIN_CARD_NUMBER, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo.PatyAlterBhyt.HEIN_MEDI_ORG_CODE));

                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.IS_HEIN, "X"));
                    if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }
                    //Dia chi the
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                }
                else
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.IN_TIME)));
                    if (rdo.currentTreatment.OUT_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.OUT_TIME.Value)));

                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.MEDI_ORG_TO_NAME, rdo.currentTreatment.MEDI_ORG_NAME));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.DAU_HIEU_LAM_SANG, rdo.currentTreatment.CLINICAL_NOTE));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.XET_NGHIEM, rdo.currentTreatment.SUBCLINICAL_RESULT));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.THUOC_DA_DUNG, rdo.currentTreatment.TREATMENT_METHOD));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.HUONG_DIEU_TRI, rdo.currentTreatment.TREATMENT_DIRECTION));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.TINH_TRANG, rdo.currentTreatment.PATIENT_CONDITION));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.PHUONG_TIEN_CHUYEN, rdo.currentTreatment.TRANSPORT_VEHICLE));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.NGUOI_HO_TONG, rdo.currentTreatment.TRANSPORTER));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.OUT_CODE, rdo.currentTreatment.OUT_CODE));
                    //SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.currentTreatment.TDL_PATIENT_DOB)));

                    if (rdo.currentTreatment.TDL_PATIENT_DOB.ToString().Length > 4)
                    {
                        SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.DOB, rdo.currentTreatment.TDL_PATIENT_DOB));
                        SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.currentTreatment.TDL_PATIENT_DOB)));
                        SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.STR_YEAR, rdo.currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                        SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.currentTreatment.TDL_PATIENT_DOB)));
                    }

                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.ADDRESS, rdo.currentTreatment.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.VIR_ADDRESS, rdo.currentTreatment.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.CAREER_NAME, rdo.currentTreatment.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.PATIENT_CODE, rdo.currentTreatment.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.GENDER_NAME, rdo.currentTreatment.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.MILITARY_RANK_NAME, rdo.currentTreatment.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.VIR_PATIENT_NAME, rdo.currentTreatment.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.NATIONAL_NAME, rdo.currentTreatment.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.WORK_PLACE, rdo.currentTreatment.TDL_PATIENT_WORK_PLACE));
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.WORK_PLACE_NAME, rdo.currentTreatment.TDL_PATIENT_WORK_PLACE_NAME));
                }

                if (rdo.tranPatiForms != null)
                {
                    SetSingleKey(new KeyValue(Mps000011ExtendSingleKey.HINH_THUC_CHUYEN, rdo.tranPatiForms.TRAN_PATI_FORM_NAME));
                }

                AddObjectKeyIntoListkey<PatientADO>(rdo.PatientADO);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentTreatment, true);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<Mps000382ADO>(rdo.Mps000382ADO, false);
                AddObjectKeyIntoListkey<HIS_TRAN_PATI_TECH>(rdo._TranPatiTech, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

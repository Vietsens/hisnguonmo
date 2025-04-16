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
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000010.PDO;
using System.Linq;

namespace MPS.Processor.Mps000010
{
    class Mps000010Processor : AbstractProcessor
    {
        Mps000010PDO rdo;
        public Mps000010Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000010PDO)rdoBase;
        }

        private void SetBarcodeKey()
        {
            try
            {

                if (rdo.currentTreatment != null)
                {
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

                        dicImage.Add(Mps000010ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
                    }

                    if (!String.IsNullOrEmpty(rdo.currentTreatment.STORE_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.STORE_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 200;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000010ExtendSingleKey.STORE_CODE_BAR, barcodeTreatment);
                    }

                    if (!String.IsNullOrEmpty(rdo.currentTreatment.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeAppointment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                        barcodeAppointment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeAppointment.IncludeLabel = false;
                        barcodeAppointment.Width = 120;
                        barcodeAppointment.Height = 40;
                        barcodeAppointment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeAppointment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeAppointment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeAppointment.IncludeLabel = true;

                        dicImage.Add(Mps000010ExtendSingleKey.APPOINTMENT_BARCODE, barcodeAppointment);
                    }
                }

                if (rdo.Patient != null && !String.IsNullOrEmpty(rdo.Patient.PATIENT_CODE))
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

                    dicImage.Add(Mps000010ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
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
            bool result = true;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetBarcodeKey();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);

                objectTag.AddObjectData(store, "AppointmentServs", rdo._AppointmentServs != null ? rdo._AppointmentServs : new List<V_HIS_APPOINTMENT_SERV>());

                barCodeTag.ProcessData(store, dicImage);
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
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.HEIN_CARD_NUMBER, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo.PatyAlterBhyt.HEIN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.HEIN_MEDI_ORG_NAME, rdo.PatyAlterBhyt.HEIN_MEDI_ORG_NAME));
                }

                if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.METHOD, rdo.currentTreatment.TREATMENT_METHOD));

                    //SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.DATE_TIME_APPOINT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.APPOINTMENT_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.END_ORDER, rdo.currentTreatment.END_CODE));

                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.IN_TIME)));

                    //SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.APPOINTMENT_CODE_STR, rdo.currentTreatment.TREATMENT_CODE));
                    //SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.APPOINTMENT_DESC_STR, rdo.currentTreatment.APPOINTMENT_DESC));
                    //SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ADVISE_STR, rdo.currentTreatment.ADVISE));


                    if (rdo.currentTreatment.OUT_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.OUT_TIME.Value)));

                    if (rdo.currentTreatment.TDL_PATIENT_DOB.ToString().Length > 4)
                    {
                        SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.DOB, rdo.currentTreatment.TDL_PATIENT_DOB));
                        SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.currentTreatment.TDL_PATIENT_DOB)));
                        SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.STR_YEAR, rdo.currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                        SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.currentTreatment.TDL_PATIENT_DOB)));
                    }

                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ADDRESS, rdo.currentTreatment.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.VIR_ADDRESS, rdo.currentTreatment.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.CAREER_NAME, rdo.currentTreatment.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.PATIENT_CODE, rdo.currentTreatment.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.GENDER_NAME, rdo.currentTreatment.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.MILITARY_RANK_NAME, rdo.currentTreatment.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.VIR_PATIENT_NAME, rdo.currentTreatment.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.NATIONAL_NAME, rdo.currentTreatment.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.WORK_PLACE, rdo.currentTreatment.TDL_PATIENT_WORK_PLACE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.WORK_PLACE_NAME, rdo.currentTreatment.TDL_PATIENT_WORK_PLACE_NAME));
                }

                SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                string _Names = "";
                string _Codes = "";
                string _Ids = "";
                if (rdo._AppointmentServs != null && rdo._AppointmentServs.Count > 0)
                {
                    List<V_HIS_APPOINTMENT_SERV> _sss = rdo._AppointmentServs.OrderBy(p => p.SERVICE_TYPE_CODE).ToList();
                    List<string> _serviceNames = _sss.Select(p => p.SERVICE_NAME).ToList();
                    List<string> _serviceCodes = _sss.Select(p => p.SERVICE_CODE).ToList();
                    _Names = string.Join(";", _serviceNames);
                    _Codes = string.Join(";", _serviceCodes);
                }

                HIS_APPOINTMENT_PERIOD appPeriod = new HIS_APPOINTMENT_PERIOD();
                if (rdo.currentTreatment.APPOINTMENT_PERIOD_ID.HasValue && rdo.ListAppointmentPeriod != null && rdo.ListAppointmentPeriod.Count > 0)
                {
                    appPeriod = rdo.ListAppointmentPeriod.FirstOrDefault(o => o.ID == rdo.currentTreatment.APPOINTMENT_PERIOD_ID);
                }

                if (rdo.ServiceReq != null && rdo.ServiceReq.APPOINTMENT_TIME != null)
                {
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.DATE_TIME_APPOINT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ServiceReq.APPOINTMENT_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ADVISE_STR, rdo.ServiceReq.ADVISE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.APPOINTMENT_CODE_STR, rdo.ServiceReq.APPOINTMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.APPOINTMENT_DESC_STR, rdo.ServiceReq.APPOINTMENT_DESC));
                }
                else if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.DATE_TIME_APPOINT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.APPOINTMENT_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ADVISE_STR, rdo.currentTreatment.ADVISE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.APPOINTMENT_CODE_STR, rdo.currentTreatment.APPOINTMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.APPOINTMENT_DESC_STR, rdo.currentTreatment.APPOINTMENT_DESC));
                }

                if (rdo.ServiceReq != null && rdo.ServiceReq.EXAM_END_TYPE == 3)
                {
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.EXECUTE_USERNAME_STR, rdo.ServiceReq.EXECUTE_USERNAME));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.EXECUTE_LOGINNAME_STR, rdo.ServiceReq.EXECUTE_LOGINNAME));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ICD_CODE_STR, rdo.ServiceReq.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ICD_NAME_STR, rdo.ServiceReq.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ICD_SUB_CODE_STR, rdo.ServiceReq.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ICD_TEXT_STR, rdo.ServiceReq.ICD_TEXT));
                }
                else if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ICD_CODE_STR, rdo.currentTreatment.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ICD_NAME_STR, rdo.currentTreatment.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ICD_SUB_CODE_STR, rdo.currentTreatment.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.ICD_TEXT_STR, rdo.currentTreatment.ICD_TEXT));
                }
                if (!string.IsNullOrEmpty(rdo.Mps000010ADO.APPOINTMENT_SERVICE_CODES))
                {
                    _Codes = rdo.Mps000010ADO.APPOINTMENT_SERVICE_CODES;
                }
                if (!string.IsNullOrEmpty(rdo.Mps000010ADO.APPOINTMENT_SERVICE_NAMES))
                {
                    _Names = rdo.Mps000010ADO.APPOINTMENT_SERVICE_NAMES;
                }
                if (!string.IsNullOrEmpty(rdo.Mps000010ADO.APPOINTMENT_EXAM_ROOM_IDS))
                {
                    _Ids = rdo.Mps000010ADO.APPOINTMENT_EXAM_ROOM_IDS;
                }

                SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.APPOINTMENT_SERVICE_NAMES, _Names));
                SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.APPOINTMENT_SERVICE_CODES, _Codes));
                SetSingleKey(new KeyValue(Mps000010ExtendSingleKey.APPOINTMENT_EXAM_ROOM_IDS, _Ids));

                AddObjectKeyIntoListkey<PatientADO>(rdo.Patient, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<Mps000010ADO>(rdo.Mps000010ADO, false);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentTreatment, false);
                AddObjectKeyIntoListkey<HIS_APPOINTMENT_PERIOD>(appPeriod, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}


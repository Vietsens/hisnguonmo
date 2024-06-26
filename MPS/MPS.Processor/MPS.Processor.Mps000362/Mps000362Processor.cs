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
using MPS.Processor.Mps000362.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000362
{
    public partial class Mps000362Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        private PatyAlterBhytADO patyAlter { get; set; }

        Mps000362PDO rdo;
        public Mps000362Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000362PDO)rdoBase;
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

                dicImage.Add(Mps000362ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                ProcessSingleKey();
                SetBarcodeKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
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
                SetSingleKey(new KeyValue(Mps000362ExtendSingleKey.RATIO_STR, rdo.SingleKeyValue.RatioText));
                SetSingleKey(new KeyValue(Mps000362ExtendSingleKey.EXECUTE_DEPARTMENT_NAME, rdo.SingleKeyValue.ExecuteDepartmentName));
                SetSingleKey(new KeyValue(Mps000362ExtendSingleKey.EXECUTE_ROOM_NAME, rdo.SingleKeyValue.ExecuteRoomName));

                if (rdo.Treatment != null)
                {
                    if (rdo.Treatment.CLINICAL_IN_TIME != null)
                    {

                        SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value))));

                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.IN_TIME))));
                    }
                }

                if (rdo.ExamServiceReq != null)
                    SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ExamServiceReq.INTRUCTION_TIME))));
                if (!String.IsNullOrEmpty(rdo.Treatment.TRANSFER_IN_MEDI_ORG_CODE))
                {
                    SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.ICD_NGT_TEXT, rdo.Treatment.TRANSFER_IN_ICD_NAME)));
                }
                SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.ICD_DEPARTMENT_TRAN, rdo.SingleKeyValue.Icd_Name)));

                if (rdo.PatyAlter != null)
                {
                    SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.PatyAlter.ADDRESS)));
                    SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlter.HEIN_CARD_FROM_TIME.ToString()))));
                    SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.HEIN_CARD_TO_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlter.HEIN_CARD_TO_TIME.ToString()))));
                }

                SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.LOGIN_USER_NAME, rdo.SingleKeyValue.Username)));
                SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.LOGIN_NAME, rdo.SingleKeyValue.LoginName)));


                if (rdo._currentPatient != null)
                {
                    SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._currentPatient.DOB))));
                }

                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.Treatment, false);
                AddObjectKeyIntoListkey<PatientADO>(patientADO, false);
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlter, false);
                if (rdo.ExamServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ExamServiceReq, false);
                }
                AddObjectKeyIntoListkey<HIS_DHST>(rdo.DHST, false);
                if (rdo.DHST != null)
                {
                    SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.DHST_NOTE, rdo.DHST.NOTE)));
                    string strBmiDisplayText = "", strLeatherArea = "";
                    FillDataToBmiAndLeatherArea(rdo.DHST, ref strBmiDisplayText, ref strLeatherArea);
                    SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.DHST_BMI, strBmiDisplayText)));
                    SetSingleKey((new KeyValue(Mps000362ExtendSingleKey.DHST_LEATHER_AREA, strLeatherArea)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToBmiAndLeatherArea(MOS.EFMODEL.DataModels.HIS_DHST dhst, ref string strBmiDisplayText, ref string strLeatherArea)
        {
            try
            {
                decimal bmi = 0;
                if (dhst.HEIGHT.HasValue && dhst.HEIGHT.Value != 0 && dhst.WEIGHT.HasValue)
                {
                    bmi = ((dhst.WEIGHT ?? 0)) / ((dhst.HEIGHT.Value / 100) * (dhst.HEIGHT.Value / 100));
                    double leatherArea = 0.007184 * Math.Pow((double)dhst.HEIGHT.Value, 0.725) * Math.Pow((double)dhst.WEIGHT.Value, 0.425);
                    string sText = Math.Round(bmi, 2) + "";
                    strLeatherArea = Math.Round(leatherArea, 2) + "";
                }

                if (bmi < 16)
                {
                    strBmiDisplayText = "(Gầy độ III)";
                }
                else if (16 <= bmi && bmi < 17)
                {
                    strBmiDisplayText = "(Gầy độ II)";
                }
                else if (17 <= bmi && bmi < (decimal)18.5)
                {
                    strBmiDisplayText = "(Gầy độ I)";
                }
                else if ((decimal)18.5 <= bmi && bmi < 25)
                {
                    strBmiDisplayText = "(Bình thường)";
                }
                else if (25 <= bmi && bmi < 30)
                {
                    strBmiDisplayText = "(Thừa cân)";
                }
                else if (30 <= bmi && bmi < 35)
                {
                    strBmiDisplayText = "(Béo phì độ I)";
                }
                else if (35 <= bmi && bmi < 40)
                {
                    strBmiDisplayText = "(Béo phì độ II)";
                }
                else if (40 < bmi)
                {
                    strBmiDisplayText = "(Béo phì độ III)";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

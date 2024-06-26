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
using MPS.Processor.Mps000309.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000309
{
    public partial class Mps000309Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        private PatyAlterBhytADO patyAlter { get; set; }

        Mps000309PDO rdo;
        public Mps000309Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000309PDO)rdoBase;
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

                dicImage.Add(Mps000309ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                if (rdo.PatyAlter != null)
                {
                    SetSingleKey((new KeyValue(Mps000309ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlter.HEIN_CARD_FROM_TIME.ToString()))));
                    SetSingleKey((new KeyValue(Mps000309ExtendSingleKey.HEIN_CARD_TO_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlter.HEIN_CARD_TO_TIME.ToString()))));
                }

                SetSingleKey((new KeyValue(Mps000309ExtendSingleKey.LOGIN_USER_NAME, rdo.SingleKeyValue.Username)));
                SetSingleKey((new KeyValue(Mps000309ExtendSingleKey.LOGIN_NAME, rdo.SingleKeyValue.LoginName)));
                SetSingleKey((new KeyValue(Mps000309ExtendSingleKey.GENDER_MALE_TICK, (rdo._currentPatient.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE) ? "X" : "")));
                SetSingleKey((new KeyValue(Mps000309ExtendSingleKey.GENDER_FEMALE_TICK, (rdo._currentPatient.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE) ? "X" : "")));
                SetSingleKey((new KeyValue(Mps000309ExtendSingleKey.GENDER_UNKNOWN_TICK, (rdo._currentPatient.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__UNKNOWN) ? "X" : "")));
                AddObjectKeyIntoListkey<PatientADO>(patientADO);
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlter, false);
                AddObjectKeyIntoListkey<HIS_DHST>(rdo.DHST != null ? rdo.DHST : new HIS_DHST(), false);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.Treatment, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

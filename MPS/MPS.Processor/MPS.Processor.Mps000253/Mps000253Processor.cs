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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000253.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000253
{
    public class Mps000253Processor : AbstractProcessor
    {
        Mps000253PDO rdo;
        List<AllergenicADO> listAdo;

        public Mps000253Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000253PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.treatment.TREATMENT_CODE);
                barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatmentCode.IncludeLabel = false;
                barcodeTreatmentCode.Width = 120;
                barcodeTreatmentCode.Height = 40;
                barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatmentCode.IncludeLabel = true;

                dicImage.Add(Mps000253ExtendSingleKey.TREATMENT_CODE_BARCODE, barcodeTreatmentCode);
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

                SetBarcodeKey();
                SetSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                listAdo = new List<AllergenicADO>();
                if (rdo.allergenic != null && rdo.allergenic.Count > 0)
                {
                    foreach (var item in rdo.allergenic)
                    {
                        AllergenicADO ado = new AllergenicADO(item);
                        listAdo.Add(ado);
                    }
                }

                objectTag.AddObjectData(store, "ListAllergenic", listAdo);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.treatment, false);
                AddObjectKeyIntoListkey<V_HIS_ALLERGY_CARD>(rdo.allergyCard, false);
                AddObjectKeyIntoListkey<Mps000253ADO>(rdo.mps000253ADO, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.patient, false);

                if (rdo.patient != null)
                {
                    string cmndCCCD = "";
                    if (!string.IsNullOrEmpty(rdo.patient.CCCD_NUMBER))
                    {
                        cmndCCCD = rdo.patient.CCCD_NUMBER;
                    }
                    else
                    {
                        cmndCCCD = rdo.patient.CMND_NUMBER;
                    }

                    SetSingleKey(new KeyValue(Mps000253ExtendSingleKey.CCCD_CMND_NUMBER, cmndCCCD));
                }

                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000253ExtendSingleKey.TREATMENT_CODE_BARCODE, rdo.treatment.TREATMENT_CODE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

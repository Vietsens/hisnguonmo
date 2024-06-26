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
using MPS.Processor.Mps000358.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000358
{
    public partial class Mps000358Processor : AbstractProcessor
    {
        Mps000358PDO rdo;
        public Mps000358Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000358PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000358ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodeTreatment);
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

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                //if (rdo != null && rdo._ChmsExpMest != null)
                //    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}", printTypeCode, rdo.Patient.PATIENT_CODE, rdo.Patient., countMedicine, countMaterial, countBlood);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                if (rdo.Patient != null)
                {
                    int? genderIndex = null;
                    string genderName;
                    switch (rdo.Patient.GENDER_ID)
                    {
                        case IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE:
                            genderIndex = 1;
                            genderName = rdo.Patient.GENDER_NAME;
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE:
                            genderIndex = 2;
                            genderName = rdo.Patient.GENDER_NAME;
                            break;
                        default:
                            genderIndex = 3;
                            genderName = "Không xác định";
                            break;
                    }
                    SetSingleKey(new KeyValue(Mps000358ExtendSingleKey.GENDER_INDEX, genderIndex));
                    SetSingleKey(new KeyValue(Mps000358ExtendSingleKey.GENDER_NAME, genderName));
                    SetSingleKey(new KeyValue(Mps000358ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Patient.DOB)));
                    SetSingleKey(new KeyValue(Mps000358ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB)));

                }

                SetSingleKey(new KeyValue(Mps000358ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.SingleKeyValue.currentDateSeparateFullTime));
                SetSingleKey(new KeyValue(Mps000358ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

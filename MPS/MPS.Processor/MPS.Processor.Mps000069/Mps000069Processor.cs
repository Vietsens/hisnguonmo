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
using MPS.Processor.Mps000069.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.Processor.Mps000069.PDO;
using System.Drawing;
using FlexCel.Report;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000069
{
    class Mps000069Processor : AbstractProcessor
    {
        Mps000069PDO rdo;

        public Mps000069Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000069PDO)rdoBase;
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

                dicImage.Add(Mps000069ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                //Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.departmentTran.TREATMENT_CODE);
                //barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodeTreatment.IncludeLabel = false;
                //barcodeTreatment.Width = 120;
                //barcodeTreatment.Height = 40;
                //barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodeTreatment.IncludeLabel = true;

                //dicImage.Add(Mps000069ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "Cares", rdo.lstCareViewPrintADO);
                objectTag.AddObjectData(store, "Care", rdo.lstCareDescription);
                objectTag.AddObjectData(store, "InstructionDescription", rdo.lstInstructionDescription);
                objectTag.AddObjectData(store, "CareDetails", rdo.lstCareDetailViewPrintADO);
                objectTag.AddObjectData(store, "Creators", rdo._Creator);
                //objectTag.AddRelationship(store, "Creators", "CareDetails", "", "");

                objectTag.SetUserFunction(store, "FuncSameTitleRow", new CustomerFuncMergeSameData(rdo.lstCareViewPrintADO, 1));
                objectTag.SetUserFunction(store, "FuncSameTitleCol", new CustomerFuncMergeSameData(rdo.lstCareViewPrintADO, 2));
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        #region Orders Implementation
        class CustomerFuncMergeSameData : TFlexCelUserFunction
        {
            List<CareViewPrintADO> ListCares;
            int SameType;
            public CustomerFuncMergeSameData(List<CareViewPrintADO> listCare, int sameType)
            {
                ListCares = listCare;
                SameType = sameType;
            }
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                bool result = false;
                try
                {
                    int rowIndex = (int)parameters[0];
                    if (rowIndex >= 0)
                    {
                        switch (SameType)
                        {
                            case 1:
                                if (rowIndex == 0 || rowIndex == 1 || rowIndex == 2 || rowIndex == 3 || rowIndex == 9 || rowIndex == 10 || rowIndex == 11 || rowIndex == 16 || rowIndex == 17 || rowIndex == 18)
                                {
                                    result = true;
                                }
                                else
                                {
                                    result = false;
                                }
                                break;
                            case 2:
                                if (rowIndex == 5 || rowIndex == 6 || rowIndex == 7 || rowIndex == 8 || rowIndex == 13 || rowIndex == 14 || rowIndex == 15)
                                {
                                    result = true;
                                }
                                else
                                {
                                    result = false;
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                return result;
            }
        }
        #endregion

        void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentTreatment, false);
                AddObjectKeyIntoListkey<Mps000069ADO>(rdo.mps000069ADO, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.Patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

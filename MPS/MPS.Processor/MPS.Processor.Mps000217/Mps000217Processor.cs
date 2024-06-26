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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000217.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000217
{
    class Mps000217Processor : AbstractProcessor
    {
        Mps000217PDO rdo;
        public Mps000217Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000217PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetBarcodeKey();
                ProcessSingleKey();

                singleTag.ProcessData(store, singleValueDictionary);
                singleTag.ProcessData(store, rdo._SingKey217.DIC_OTHER_KEY);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "Report", rdo._Mrs00067RDOs);
                objectTag.AddObjectData(store, "ListMedicine", rdo._ListMedicines);
                Mrs00067RDO nr = new Mrs00067RDO();
                if(rdo._Mrs00067RDOs==null || rdo._Mrs00067RDOs.Count==0)
                {
                    nr.EXP_MEST_TYPE_NAME = "Tồn cuối kỳ";
                    nr.EXECUTE_DATE_STR = string.Join(", ", rdo._ListMedicines.Select(o => o.EXECUTE_DATE_STR).Distinct().ToList());
                    nr.PACKAGE_NUMBER = string.Join(", ", rdo._ListMedicines.Select(o => o.PACKAGE_NUMBER).Distinct().ToList());
                    nr.EXPIRED_DATE_STR = string.Join(", ", rdo._ListMedicines.Select(o => o.EXPIRED_DATE_STR).Distinct().ToList());
                    nr.BEGIN_AMOUNT = rdo._ListMedicines.Sum(s => s.BEGIN_AMOUNT);
                    nr.END_AMOUNT = rdo._ListMedicines.Sum(s => s.END_AMOUNT);

                    nr.IMP_PRICE = rdo._ListMedicines.OrderBy(o => o.MEDICINE_ID).Select(p => p.IMP_PRICE).LastOrDefault();
                    nr.IMP_VAT_RATIO = rdo._ListMedicines.OrderBy(o => o.MEDICINE_ID).Select(p => p.IMP_VAT_RATIO).LastOrDefault();
                }    
                
                objectTag.AddObjectData(store, "SumMedicine", new List<Mrs00067RDO>() { nr }) ;
                objectTag.AddObjectData(store, "ReportByPackage", rdo._MedicineByPakages);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<SingKey217>(rdo._SingKey217, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetBarcodeKey()
        {
            try
            {
                //Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo._BcsExpMest.EXP_MEST_CODE);
                //barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodePatientCode.IncludeLabel = false;
                //barcodePatientCode.Width = 120;
                //barcodePatientCode.Height = 40;
                //barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodePatientCode.IncludeLabel = true;

                //dicImage.Add(Mps000215ExtendSingleKey.EXP_MEST_CODE_BARCODE, barcodePatientCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

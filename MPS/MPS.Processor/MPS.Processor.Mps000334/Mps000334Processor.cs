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
using MPS.Processor.Mps000334.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000334
{
    class Mps000334Processor : AbstractProcessor
    {
        Mps000334PDO rdo;
        public Mps000334Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000334PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                SetSingleKey();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Medicines", rdo._Medicines);
                barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<string> GetListStringApprovalLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpTimeLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => p.EXP_TIME != null)
                      .GroupBy(o => o.EXP_TIME)
                      .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                      .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {

                // SetSingleKey(new KeyValue(Mps000334ExtendSingleKey.SUM_TOTAL_PRICE_AFTER_DISCOUNT_NOT_ROUND_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0}", sumAfterDiscountNotRound))));

                if (rdo.Transaction != null)
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo.Transaction, false);
                if (rdo._vHisVaccinations != null)
                    AddObjectKeyIntoListkey<V_HIS_VACCINATION>(rdo._vHisVaccinations, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.Transaction != null && !String.IsNullOrEmpty(rdo.Transaction.TRANSACTION_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(rdo.Transaction.TRANSACTION_CODE);
                    barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcode.IncludeLabel = false;
                    barcode.Width = 120;
                    barcode.Height = 40;
                    barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcode.IncludeLabel = true;

                    dicImage.Add(Mps000334ExtendSingleKey.TRANSACTION_CODE_BAR, barcode);
                }

                if (rdo._vHisVaccinations != null && !String.IsNullOrEmpty(rdo._vHisVaccinations.VACCINATION_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(rdo._vHisVaccinations.VACCINATION_CODE);
                    barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcode.IncludeLabel = false;
                    barcode.Width = 120;
                    barcode.Height = 40;
                    barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcode.IncludeLabel = true;

                    dicImage.Add(Mps000334ExtendSingleKey.VACCINATION_CODE_BAR, barcode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = this.LogDataExpMests(rdo._vHisVaccinations.TDL_PATIENT_CODE, rdo._Medicines.Select(s => s.MEDICINE_TYPE_CODE).ToList(), "");
                log += "TRANSACTION_CODE: " + rdo.Transaction.TRANSACTION_CODE;
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null)
                {
                    if (rdo.Transaction != null)
                    {
                        result = String.Format("{0}_{1}_{2}_{3}", this.printTypeCode, rdo.Transaction.ACCOUNT_BOOK_CODE, rdo.Transaction.TRANSACTION_CODE, rdo.Transaction.NUM_ORDER);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string LogDataExpMests(string treatmentCode, List<string> expMestCodes, string message)
        {
            string result = "";
            try
            {
                result += message + ". ";

                if (!String.IsNullOrWhiteSpace(treatmentCode))
                {
                    result += string.Format("PATIENT_CODE: {0}. ", treatmentCode);
                }

                if (expMestCodes != null)
                {
                    foreach (var expMestCode in expMestCodes)
                    {
                        if (!String.IsNullOrWhiteSpace(expMestCode))
                        {
                            result += string.Format("PATIENT_TYPE_CODE: {0}. ", expMestCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

    }
}

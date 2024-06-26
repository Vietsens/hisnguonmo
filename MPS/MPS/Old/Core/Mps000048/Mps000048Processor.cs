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
using AutoMapper;
using FlexCel.Report;
using Inventec.Common.Logging;
using MPS.ADO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Core.Mps000048
{
    class Mps000048Processor : ProcessorBase, IProcessorPrint
    {
        Mps000048RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;
        List<ExpMestMedicinePrintADO> ExpMestManuMedicineSumForPrints = new List<ExpMestMedicinePrintADO>();

        internal Mps000048Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000048RDO)data;
            this.fileName = fileName;
            store = new Inventec.Common.FlexCellExport.Store();
            dicImage = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
        }

        bool IProcessorPrint.Run()
        {
            bool result = false;
            bool valid = true;
            try
            {
                SetCommonSingleKey();
                rdo.SetSingleKey();
                SetSingleKey();

                //Cac ham dac thu khac cua rdo
                SetBarcodeKey();

                store.SetCommonFunctions();

                //Process list input data to list ADO for print
                ProcessListADO();

                //Ham xu ly cac doi tuong su dung trong thu vien flexcelexport
                valid = valid && ProcessData();
                if (valid)
                {
                    using (MemoryStream streamResult = store.OutStream())
                    {
                        //Print preview
                        result = PrintPreview(streamResult, this.fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrExpMest.EXP_MEST_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000048ExtendSingleKey.EXP_MEST_CODE_BAR, barcodePatientCode);

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
        protected bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ExpMestAggregates", ExpMestManuMedicineSumForPrints);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessListADO()
        {
            try
            {
                ExpMestManuMedicineSumForPrints = new List<ExpMestMedicinePrintADO>();
                List<ExpMestMedicinePrintADO> lstPrintADO = new List<ExpMestMedicinePrintADO>();
                if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Approved)
                {
                    lstPrintADO = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                }
                else if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Draft || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Request || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Rejected)
                {
                    lstPrintADO = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                }
                var query = lstPrintADO.AsQueryable();

                query = query
               .Where(o =>
                   (
                       rdo.IsMedicine
                       && rdo.ServiceUnitIds.Contains(o.SERVICE_UNIT_ID)
                       && (o.MEDICINE_USE_FORM_ID == null || (o.MEDICINE_USE_FORM_ID != null && rdo.UseFormIds.Contains(o.MEDICINE_USE_FORM_ID ?? 0)))
                       && o.IS_MEDICINE == true
                       //&& o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE
                       )
                   );

                if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                {
                    query = query.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0));
                }

                var expMestManuMedicineTemps = query.ToList();

                if (expMestManuMedicineTemps != null)
                {
                    foreach (var item in expMestManuMedicineTemps)
                    {
                        var expMestDetailOld = ExpMestManuMedicineSumForPrints.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID && o.IMP_PRICE == item.IMP_PRICE);
                        if (expMestDetailOld == null)
                        {
                            if (MPS.PrintConfig.VHisMedicineTypes != null && MPS.PrintConfig.VHisMedicineTypes.Count > 0)
                            {
                                var medicineData = MPS.PrintConfig.VHisMedicineTypes.FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                                if (medicineData != null)
                                {
                                    item.DESCRIPTION = medicineData.DESCRIPTION;
                                    item.CONCENTRA = medicineData.CONCENTRA;
                                    item.PACKING_TYPE_NAME = medicineData.PACKING_TYPE_NAME;
                                    item.CONCENTRA_PACKING_TYPE_NAME = medicineData.CONCENTRA;
                                    if (!String.IsNullOrEmpty(medicineData.PACKING_TYPE_NAME))
                                    {
                                        item.CONCENTRA_PACKING_TYPE_NAME += ";" + medicineData.PACKING_TYPE_NAME;
                                    }
                                }
                            }
                            if (item.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported)
                            {
                                item.AMOUNT_EXPORTED = item.AMOUNT;
                            }
                            else
                            {
                                item.AMOUNT_EXPORTED = 0;
                            }
                            ExpMestManuMedicineSumForPrints.Add(item);
                        }
                        else
                        {
                            expMestDetailOld.AMOUNT += item.AMOUNT;
                            if (item.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported)
                            {
                                expMestDetailOld.AMOUNT_EXPORTED += item.AMOUNT;
                            }
                        }
                    }
                    foreach (var item in ExpMestManuMedicineSumForPrints)
                    {
                        item.AMOUNT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(item.AMOUNT).ToString());
                        item.AMOUNT_EXPORTED_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(item.AMOUNT_EXPORTED ?? 0).ToString());
                    }
                }

                if (ExpMestManuMedicineSumForPrints != null && ExpMestManuMedicineSumForPrints.Count > 0)
                {
                    ExpMestManuMedicineSumForPrints = ExpMestManuMedicineSumForPrints.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

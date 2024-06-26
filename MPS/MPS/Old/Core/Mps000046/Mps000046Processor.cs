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

namespace MPS.Core.Mps000046
{
    class Mps000046Processor : ProcessorBase, IProcessorPrint
    {
        Mps000046RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;
        List<ExpMestMedicinePrintADO> ExpMestManuMedicineSumForPrints = new List<ExpMestMedicinePrintADO>();

        internal Mps000046Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000046RDO)data;
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
                Inventec.Common.BarcodeLib.Barcode expMestCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrExpMest.EXP_MEST_CODE);
                expMestCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                expMestCodeBar.IncludeLabel = false;
                expMestCodeBar.Width = 120;
                expMestCodeBar.Height = 40;
                expMestCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                expMestCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                expMestCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                expMestCodeBar.IncludeLabel = true;

                dicImage.Add(Mps000046ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBar);

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
                List<ExpMestMedicinePrintADO> lstPrintRequestADO = new List<ExpMestMedicinePrintADO>();
                List<ExpMestMedicinePrintADO> lstPrintExecuteADO = new List<ExpMestMedicinePrintADO>();
                lstPrintExecuteADO = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                lstPrintRequestADO = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Approved)
                {
                    lstPrintADO = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                }
                else if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Draft || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Request || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Rejected)
                {
                    lstPrintADO = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                }
                var query = lstPrintADO.AsQueryable();
                if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                {
                    query = query.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0));
                }
                var medicineSumForPrints = query
                     .Where(o =>
                     (
                         (
                             rdo.IsMedicine
                             && o.IS_MEDICINE == true
                             && rdo.ServiceUnitIds.Contains(o.SERVICE_UNIT_ID)
                             && (o.MEDICINE_USE_FORM_ID == null || (o.MEDICINE_USE_FORM_ID != null && rdo.UseFormIds.Contains(o.MEDICINE_USE_FORM_ID ?? 0)))
                         )
                         ||
                         (
                             rdo.Ismaterial
                             && o.IS_MEDICINE == false
                             && o.IS_CHEMICAL_SUBSTANCE != IMSys.DbConfig.HIS_RS.HIS_MATERIAL_TYPE.IS_CHEMICAL_SUBSTANCE
                             && rdo.ServiceUnitIds.Contains(o.SERVICE_UNIT_ID)
                         )
                         ||
                         (
                             rdo.IsChemicalSustance
                             && o.IS_MEDICINE == false
                             && rdo.ServiceUnitIds.Contains(o.SERVICE_UNIT_ID)
                             && o.IS_CHEMICAL_SUBSTANCE == IMSys.DbConfig.HIS_RS.HIS_MATERIAL_TYPE.IS_CHEMICAL_SUBSTANCE
                         )
                     ))
                     .ToList();

                if (medicineSumForPrints != null && medicineSumForPrints.Count > 0)
                {
                    var groupData = medicineSumForPrints.GroupBy(p => p.MEDICINE_TYPE_ID).ToList();
                    foreach (var itemGroup in groupData)
                    {
                        ExpMestMedicinePrintADO data = new ExpMestMedicinePrintADO();
                        Mapper.CreateMap<ExpMestMedicinePrintADO, ExpMestMedicinePrintADO>();
                        data = Mapper.Map<ExpMestMedicinePrintADO, ExpMestMedicinePrintADO>(itemGroup.FirstOrDefault());

                        var soLo = itemGroup.ToList();
                        var soLoGr = soLo.GroupBy(p => p.MEDICINE_ID).ToList();
                        foreach (var itemSoLo in soLoGr)
                        {
                            if (String.IsNullOrEmpty(itemSoLo.FirstOrDefault().PACKAGE_NUMBER))
                                continue;
                            data.PACKAGE_NUMBER += itemSoLo.FirstOrDefault().PACKAGE_NUMBER;
                        }
                        if (MPS.PrintConfig.VHisMedicineTypes != null && MPS.PrintConfig.VHisMedicineTypes.Count > 0)
                        {
                            var medicineData = MPS.PrintConfig.VHisMedicineTypes.FirstOrDefault(o => o.ID == data.MEDICINE_TYPE_ID);
                            if (medicineData != null)
                            {
                                data.DESCRIPTION = medicineData.DESCRIPTION;
                                data.CONCENTRA = medicineData.CONCENTRA;
                                data.PACKING_TYPE_NAME = medicineData.PACKING_TYPE_NAME;
                                data.CONCENTRA_PACKING_TYPE_NAME = medicineData.PACKING_TYPE_NAME;
                                if (!String.IsNullOrEmpty(medicineData.PACKING_TYPE_NAME))
                                {
                                    data.CONCENTRA_PACKING_TYPE_NAME += ";" + medicineData.PACKING_TYPE_NAME;
                                }
                            }
                        }
                        ExpMestManuMedicineSumForPrints.Add(data);
                    }




                    //Dictionary<long, List<string>> dicLo = new Dictionary<long, List<string>>();
                    //foreach (var item in medicineSumForPrints)
                    //{
                    //    var expMestDetailOld = ExpMestManuMedicineSumForPrints.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID && o.IS_MEDICINE == item.IS_MEDICINE);
                    //    if (expMestDetailOld != null)
                    //    {
                    //        if (!String.IsNullOrEmpty(item.PACKAGE_NUMBER))
                    //        {
                    //            if (!dicLo.ContainsKey(item.MEDICINE_TYPE_ID))
                    //            {
                    //                dicLo[item.MEDICINE_TYPE_ID] = new List<string>();
                    //                dicLo[item.MEDICINE_TYPE_ID].Add(item.PACKAGE_NUMBER);
                    //                var data = dicLo[item.MEDICINE_TYPE_ID];
                    //                if (true)
                    //                {

                    //                }
                    //            }
                    //            else
                    //            {
                    //                dicLo[item.MEDICINE_TYPE_ID].Add(item.PACKAGE_NUMBER);

                    //            }
                    //            //expMestDetailOld.PACKAGE_NUMBER += ", " + item.PACKAGE_NUMBER;
                    //        }
                    //        continue;
                    //    }
                    //    else
                    //    {
                    //        ExpMestMedicinePrintADO data = new ExpMestMedicinePrintADO();
                    //        Mapper.CreateMap<ExpMestMedicinePrintADO, ExpMestMedicinePrintADO>();
                    //        data = Mapper.Map<ExpMestMedicinePrintADO, ExpMestMedicinePrintADO>(item);
                    //        if (MPS.PrintConfig.VHisMedicineTypes != null && MPS.PrintConfig.VHisMedicineTypes.Count > 0)
                    //        {
                    //            var medicineData = MPS.PrintConfig.VHisMedicineTypes.FirstOrDefault(o => o.ID == data.MEDICINE_TYPE_ID);
                    //            if (medicineData != null)
                    //            {
                    //                data.DESCRIPTION = medicineData.DESCRIPTION;
                    //                data.CONCENTRA = medicineData.CONCENTRA;
                    //                data.PACKING_TYPE_NAME = medicineData.PACKING_TYPE_NAME;
                    //                data.CONCENTRA_PACKING_TYPE_NAME = medicineData.PACKING_TYPE_NAME;
                    //                if (!String.IsNullOrEmpty(medicineData.PACKING_TYPE_NAME))
                    //                {
                    //                    data.CONCENTRA_PACKING_TYPE_NAME += ";" + medicineData.PACKING_TYPE_NAME;
                    //                }
                    //            }
                    //        }
                    //        ExpMestManuMedicineSumForPrints.Add(data);
                    //    }
                    //}

                    foreach (var item in ExpMestManuMedicineSumForPrints)
                    {
                        var checkInRequest = lstPrintRequestADO.Where(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID).ToList(); ;
                        if (checkInRequest != null && checkInRequest.Count > 0)
                        {
                            item.AMOUNT_REQUEST = checkInRequest.Sum(p => p.AMOUNT);
                        }
                        var checkInExecute = lstPrintExecuteADO.Where(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID).ToList();
                        if (checkInExecute != null && checkInExecute.Count > 0)
                        {
                            if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported)
                            {
                                item.AMOUNT_EXPORTED = checkInExecute.Sum(p => p.AMOUNT);
                            }
                            item.AMOUNT_EXCUTE = checkInExecute.Sum(p => p.AMOUNT);
                        }
                        item.AMOUNT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.AMOUNT_REQUEST ?? 0)));
                        item.AMOUNT_EXPORTED_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.AMOUNT_EXPORTED ?? 0)));
                        item.AMOUNT_EXCUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.AMOUNT_EXCUTE ?? 0)));
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

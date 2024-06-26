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

namespace MPS.Core.Mps000078
{
    class Mps000078Processor : ProcessorBase, IProcessorPrint
    {
        Mps000078RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;
        List<ImpMestMedicinePrintADO> ImpMestManuMedicineSumForPrints = new List<ImpMestMedicinePrintADO>();

        internal Mps000078Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000078RDO)data;
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
                Inventec.Common.BarcodeLib.Barcode impMestCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrImpMest.IMP_MEST_CODE);
                impMestCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                impMestCodeBar.IncludeLabel = false;
                impMestCodeBar.Width = 120;
                impMestCodeBar.Height = 40;
                impMestCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                impMestCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                impMestCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                impMestCodeBar.IncludeLabel = true;

                dicImage.Add(Mps000078ExtendSingleKey.IMP_MEST_CODE_BARCODE, impMestCodeBar);

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
                objectTag.AddObjectData(store, "ImpMestAggregates", ImpMestManuMedicineSumForPrints);

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
                ImpMestManuMedicineSumForPrints = new List<ImpMestMedicinePrintADO>();
                var query = rdo.MedicineImpMestTypeADOs.AsQueryable();
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
                         )
                         ||
                         (
                             rdo.Ismaterial
                             && o.IS_MEDICINE == false
                             && rdo.ServiceUnitIds.Contains(o.SERVICE_UNIT_ID)
                         )
                     ))
                     .ToList();

                if (medicineSumForPrints != null)
                {
                    foreach (var item in medicineSumForPrints)
                    {
                        var expMestDetailOld = ImpMestManuMedicineSumForPrints.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID && o.IMP_PRICE == item.IMP_PRICE);
                        if (expMestDetailOld == null)
                        {
                            ImpMestMedicinePrintADO data = new ImpMestMedicinePrintADO();
                            Mapper.CreateMap<ImpMestMedicinePrintADO, ImpMestMedicinePrintADO>();
                            data = Mapper.Map<ImpMestMedicinePrintADO, ImpMestMedicinePrintADO>(item);

                            if (item.IMP_MEST_STT_ID == rdo.HisImpMestSttId__Imported)
                            {
                                data.AMOUNT_IMPORTED = item.AMOUNT;
                            }
                            if (item.IMP_MEST_STT_ID == rdo.HisImpMestSttId__Approved)
                            {
                                data.AMOUNT_APPROVED = item.AMOUNT;
                            }

                            ImpMestManuMedicineSumForPrints.Add(data);
                        }
                        else
                        {
                            expMestDetailOld.AMOUNT += item.AMOUNT;
                            if (item.IMP_MEST_STT_ID == rdo.HisImpMestSttId__Imported)
                            {
                                if (expMestDetailOld.AMOUNT_IMPORTED == null)
                                    expMestDetailOld.AMOUNT_IMPORTED = 0;
                                expMestDetailOld.AMOUNT_IMPORTED += item.AMOUNT;
                            }
                            if (item.IMP_MEST_STT_ID == rdo.HisImpMestSttId__Approved)
                            {
                                if (expMestDetailOld.AMOUNT_APPROVED == null)
                                    expMestDetailOld.AMOUNT_APPROVED = 0;
                                expMestDetailOld.AMOUNT_APPROVED += item.AMOUNT;
                            }
                        }
                    }

                    foreach (var item in ImpMestManuMedicineSumForPrints)
                    {
                        item.AMOUNT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(item.AMOUNT).ToString());
                        item.AMOUNT_IMPORTED_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(item.AMOUNT_IMPORTED ?? 0).ToString());
                        item.AMOUNT_APPROVED_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(item.AMOUNT_APPROVED ?? 0).ToString());
                        item.PRICE_EXPORTED = item.AMOUNT * item.IMP_PRICE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

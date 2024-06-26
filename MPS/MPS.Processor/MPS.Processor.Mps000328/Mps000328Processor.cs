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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000328.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000328
{
    public class Mps000328Processor : AbstractProcessor
    {
        Mps000328PDO rdo;

        public Mps000328Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000328PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetBarcodeKey();
                SetSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Branchs", rdo._HisBranchs);
                objectTag.AddObjectData(store, "Suppliers", rdo._HisSuppliers);
                objectTag.AddObjectData(store, "ImpMestPays", rdo._HisImpMestPays);
                objectTag.AddObjectData(store, "ImpMests", rdo._ImpMests);
                objectTag.AddObjectData(store, "Medicines", rdo._Medicines);
                objectTag.AddObjectData(store, "Materials", rdo._Materials);

                objectTag.AddRelationship(store, "ImpMests", "Medicines", "ID", "IMP_MEST_ID");
                objectTag.AddRelationship(store, "ImpMests", "Materials", "ID", "IMP_MEST_ID");

                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode impMestProposeCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo._ImpMestPropose.IMP_MEST_PROPOSE_CODE);
                impMestProposeCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                impMestProposeCodeBar.IncludeLabel = false;
                impMestProposeCodeBar.Width = 120;
                impMestProposeCodeBar.Height = 40;
                impMestProposeCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                impMestProposeCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                impMestProposeCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                impMestProposeCodeBar.IncludeLabel = true;

                dicImage.Add(Mps000328ExtendSingleKey.IMP_MEST_PROPOSE_CODE_BARCODE, impMestProposeCodeBar);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                if (this.rdo._ImpMestPropose != null)
                {
                    AddObjectKeyIntoListkey(this.rdo._ImpMestPropose, false);
                    SetSingleKey(new KeyValue(Mps000328ExtendSingleKey.CONTRACT_DATE, rdo._HisSuppliers[0].CONTRACT_DATE));
                    SetSingleKey(new KeyValue(Mps000328ExtendSingleKey.CONTRACT_NUM, rdo._HisSuppliers[0].CONTRACT_NUM));
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
                if (rdo != null && rdo._ImpMestPropose != null)
                {
                    log = LogDataExpMest(rdo._ImpMestPropose.IMP_MEST_PROPOSE_CODE, "", "");
                }
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
                if (rdo != null && rdo._ImpMestPropose != null)
                    result = String.Format("{0}_{1}_{2}", printTypeCode, rdo._ImpMestPropose.IMP_MEST_PROPOSE_CODE, "Phiếu đề nghị thanh toán nhà cung cấp");
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

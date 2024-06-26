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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000275.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using MOS.EFMODEL.DataModels;
using System.Text;
using System.Linq;
using HIS.Desktop.LocalStorage.BackendData;

namespace MPS.Processor.Mps000275
{
    class Mps000275Processor : AbstractProcessor
    {
        Mps000275PDO rdo;
        List<SereServRationPlusADO> sereServRationPlusADOs = new List<SereServRationPlusADO>();

        public Mps000275Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000275PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo != null)
                {
                    if (rdo._vServiceReqs != null && rdo._vServiceReqs.Count > 0)
                    {
                        if (!String.IsNullOrWhiteSpace(rdo._vServiceReqs.First().TDL_PATIENT_CODE))
                        {
                            Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(rdo._vServiceReqs.First().TDL_PATIENT_CODE);
                            barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                            barcode.IncludeLabel = false;
                            barcode.Width = 120;
                            barcode.Height = 40;
                            barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                            barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                            barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                            barcode.IncludeLabel = true;

                            dicImage.Add(Mps000275ExtendSingleKey.BARCODE_PATIENT_CODE, barcode);
                        }

                        if (!String.IsNullOrWhiteSpace(rdo._vServiceReqs.First().SERVICE_REQ_CODE))
                        {
                            Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(rdo._vServiceReqs.First().SERVICE_REQ_CODE);
                            barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                            barcode.IncludeLabel = false;
                            barcode.Width = 120;
                            barcode.Height = 40;
                            barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                            barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                            barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                            barcode.IncludeLabel = true;

                            dicImage.Add(Mps000275ExtendSingleKey.BARCODE_SERVICE_REQ_CODE, barcode);
                        }
                        if (!String.IsNullOrWhiteSpace(rdo._vServiceReqs.First().TREATMENT_CODE))
                        {
                            Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(rdo._vServiceReqs.First().TREATMENT_CODE);
                            barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                            barcode.IncludeLabel = false;
                            barcode.Width = 120;
                            barcode.Height = 40;
                            barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                            barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                            barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                            barcode.IncludeLabel = true;

                            dicImage.Add(Mps000275ExtendSingleKey.BARCODE_TREATMENT_CODE, barcode);
                        }
                    }
                }
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
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                SetBarcodeKey();
                SetSingleKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                rdo._vServiceReqs = rdo._vServiceReqs.OrderBy(o => o.ID).ToList();
                objectTag.AddObjectData(store, "ServiceReqs", rdo._vServiceReqs);
                objectTag.AddObjectData(store, "SereServs", sereServRationPlusADOs);
                objectTag.AddRelationship(store, "ServiceReqs", "SereServs", "ID", "SERVICE_REQ_ID");

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
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo._vServiceReqs.FirstOrDefault());
                sereServRationPlusADOs = new List<SereServRationPlusADO>();
                var serviceAlls = BackendDataWorker.Get<V_HIS_SERVICE>();
                foreach (var ss in rdo._SereServRations)
                {
                    SereServRationPlusADO sereServRationPlusADO = new SereServRationPlusADO();
                    sereServRationPlusADO.AMOUNT = ss.AMOUNT;
                    sereServRationPlusADO.DISCOUNT = ss.DISCOUNT;
                    sereServRationPlusADO.ID = ss.ID;
                    sereServRationPlusADO.INSTRUCTION_NOTE = ss.INSTRUCTION_NOTE;
                    sereServRationPlusADO.IS_ACTIVE = ss.IS_ACTIVE;
                    sereServRationPlusADO.MODIFIER = ss.MODIFIER;
                    sereServRationPlusADO.CREATE_TIME = ss.CREATE_TIME;
                    sereServRationPlusADO.MODIFY_TIME = ss.MODIFY_TIME;
                    sereServRationPlusADO.CREATOR = ss.CREATOR;
                    sereServRationPlusADO.PATIENT_TYPE_ID = ss.PATIENT_TYPE_ID;
                    sereServRationPlusADO.SERVICE_ID = ss.SERVICE_ID;
                    sereServRationPlusADO.SERVICE_REQ_ID = ss.SERVICE_REQ_ID;
                    var sv = serviceAlls.Where(o => o.ID == ss.SERVICE_ID).FirstOrDefault();
                    sereServRationPlusADO.SERVICE_CODE = sv != null ? sv.SERVICE_CODE : "";
                    sereServRationPlusADO.SERVICE_NAME = sv != null ? sv.SERVICE_NAME : "";
                    sereServRationPlusADO.SERVICE_UNIT_NAME = sv != null ? sv.SERVICE_UNIT_NAME : "";
                    var sereServ = rdo._vSereServs.FirstOrDefault(o => o.SERVICE_ID == ss.SERVICE_ID && o.SERVICE_REQ_ID == ss.SERVICE_REQ_ID);
                    if (sereServ != null)
                    {
                        sereServRationPlusADO.PRICE = sereServ.PRICE;
                        sereServRationPlusADO.VAT_RATIO = sereServ.VAT_RATIO;
                        sereServRationPlusADO.TOTAL_PRICE = sereServ.VIR_TOTAL_PRICE ?? null;
                    }

                    //sereServRationPlusADO.TOTAL_PRICE = (ss.AMOUNT * ss.PRICE * (1 + (ss.VAT_RATIO ?? 0))) - (ss.DISCOUNT ?? 0);
                    if (rdo._ListPatientType != null)
                    {
                        sereServRationPlusADO.PATIENT_TYPE_NAME = rdo._ListPatientType.Where(o => o.ID == ss.PATIENT_TYPE_ID).Select(o => o.PATIENT_TYPE_NAME).FirstOrDefault();
                    }

                    sereServRationPlusADOs.Add(sereServRationPlusADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

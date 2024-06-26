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
using MPS.Processor.Mps000434.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000434
{
    public class Mps000434Processor : AbstractProcessor
    {
        Mps000434PDO rdo;

        List<Mps000434ADO> listData = new List<Mps000434ADO>();
        public Mps000434Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000434PDO)rdoBase;
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

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetBarcodeKey();
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ListData", this.listData);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.VHisTreatment != null && !String.IsNullOrEmpty(rdo.VHisTreatment.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTransactionCode = new Inventec.Common.BarcodeLib.Barcode(rdo.VHisTreatment.TREATMENT_CODE);
                    barcodeTransactionCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTransactionCode.IncludeLabel = false;
                    barcodeTransactionCode.Width = 120;
                    barcodeTransactionCode.Height = 40;
                    barcodeTransactionCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTransactionCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTransactionCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTransactionCode.IncludeLabel = true;

                    dicImage.Add("TREATMENT_CODE_BAR", barcodeTransactionCode);
                }
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
                if (rdo.VHisTreatment != null)
                {
                    AddObjectKeyIntoListkey(rdo.VHisTreatment, false);
                }

                if (rdo.ListExpMestMedicine != null && rdo.ListExpMestMedicine.Count > 0)
                {
                    if (rdo.Mps000434PdoCFG != null && rdo.Mps000434PdoCFG.ConfigKeyMergerData == 1)
                    {
                        var group = rdo.ListExpMestMedicine.GroupBy(g => new { g.MEDICINE_ID, g.CONCENTRA }).ToList();
                        foreach (var item in group)
                        {
                            Mps000434ADO ado = new Mps000434ADO(item.First());
                            ado.AMOUNT = item.Sum(s => s.AMOUNT);
                            ado.VIR_PRICE = item.Sum(s => s.VIR_PRICE);
                            ado.CREATE_TIME = item.Min(o => o.CREATE_TIME);
                            this.listData.Add(ado);
                        }
                    }
                    else
                    {
                        var group = rdo.ListExpMestMedicine.GroupBy(g => new { g.MEDICINE_TYPE_ID, g.CONCENTRA }).ToList();
                        foreach (var item in group)
                        {
                            Mps000434ADO ado = new Mps000434ADO(item.First());
                            ado.AMOUNT = item.Sum(s => s.AMOUNT);
                            ado.VIR_PRICE = item.Sum(s => s.VIR_PRICE);
                            ado.CREATE_TIME = item.Min(o => o.CREATE_TIME);
                            this.listData.Add(ado);
                        }
                    }
                }

                if (rdo.ListExpMestMaterial != null && rdo.ListExpMestMaterial.Count > 0)
                {
                    if (rdo.Mps000434PdoCFG != null && rdo.Mps000434PdoCFG.ConfigKeyMergerData == 1)
                    {
                        var group = rdo.ListExpMestMaterial.GroupBy(g => new { g.MATERIAL_ID, g.IS_CHEMICAL_SUBSTANCE }).ToList();
                        foreach (var item in group)
                        {
                            Mps000434ADO ado = new Mps000434ADO(item.First());
                            ado.AMOUNT = item.Sum(s => s.AMOUNT);
                            ado.VIR_PRICE = item.Sum(s => s.VIR_PRICE);
                            ado.CREATE_TIME = item.Min(o => o.CREATE_TIME);
                            this.listData.Add(ado);
                        }
                    }
                    else
                    {
                        var group = rdo.ListExpMestMaterial.GroupBy(g => new { g.MATERIAL_TYPE_ID, g.IS_CHEMICAL_SUBSTANCE }).ToList();
                        foreach (var item in group)
                        {
                            Mps000434ADO ado = new Mps000434ADO(item.First());
                            ado.AMOUNT = item.Sum(s => s.AMOUNT);
                            ado.VIR_PRICE = item.Sum(s => s.VIR_PRICE);
                            ado.CREATE_TIME = item.Min(o => o.CREATE_TIME);
                            this.listData.Add(ado);
                        }
                    }
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
                if (rdo.VHisTreatment != null)
                {
                    List<string> mess = new List<string>();
                    if (rdo.ListExpMestMedicine != null && rdo.ListExpMestMedicine.Count > 0)
                    {
                        var group = rdo.ListExpMestMedicine.GroupBy(o => o.MEDICINE_TYPE_CODE).ToList();

                        foreach (var item in group)
                        {
                            mess.Add(string.Format("{0}({1})", item.Key, item.Sum(s => s.AMOUNT)));
                        }
                    }

                    if (rdo.ListExpMestMaterial != null && rdo.ListExpMestMaterial.Count > 0)
                    {
                        var group = rdo.ListExpMestMaterial.GroupBy(o => o.MATERIAL_TYPE_CODE).ToList();

                        foreach (var item in group)
                        {
                            mess.Add(string.Format("{0}({1})", item.Key, item.Sum(s => s.AMOUNT)));
                        }
                    }

                    log = string.Format("Mã hồ sơ:{0}. Thuốc, vật tư bị hủy: {1}", rdo.VHisTreatment.TREATMENT_CODE, string.Join("|", mess));
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
                if (rdo != null)
                {
                    string treatmentCode = "";

                    if (rdo != null && rdo.VHisTreatment != null)
                    {
                        treatmentCode = rdo.VHisTreatment.TREATMENT_CODE;
                    }

                    List<string> mess = new List<string>();
                    if (rdo.ListExpMestMedicine != null && rdo.ListExpMestMedicine.Count > 0)
                    {
                        var group = rdo.ListExpMestMedicine.GroupBy(o => o.MEDICINE_TYPE_CODE).OrderBy(o => o.Key).ToList();

                        foreach (var item in group)
                        {
                            mess.Add(string.Format("{0}({1})", item.Key, item.Sum(s => s.AMOUNT)));
                        }
                    }

                    if (rdo.ListExpMestMaterial != null && rdo.ListExpMestMaterial.Count > 0)
                    {
                        var group = rdo.ListExpMestMaterial.GroupBy(o => o.MATERIAL_TYPE_CODE).OrderBy(o => o.Key).ToList();

                        foreach (var item in group)
                        {
                            mess.Add(string.Format("{0}({1})", item.Key, item.Sum(s => s.AMOUNT)));
                        }
                    }

                    result = String.Format("{0} {1} {2}", printTypeCode, treatmentCode, string.Join("|", mess));
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

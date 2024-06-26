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
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000084.PDO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System.Linq;

namespace MPS.Processor.Mps000084
{
    class Mps000084Processor : AbstractProcessor
    {
        Mps000084PDO rdo;

        public Mps000084Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000084PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.ImpMest != null && rdo.ImpMest.IMP_MEST_CODE != null)
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeImpMest = new Inventec.Common.BarcodeLib.Barcode(rdo.ImpMest.IMP_MEST_CODE);
                    barcodeImpMest.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeImpMest.IncludeLabel = false;
                    barcodeImpMest.Width = 200;
                    barcodeImpMest.Height = 100;
                    barcodeImpMest.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeImpMest.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeImpMest.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeImpMest.IncludeLabel = true;

                    dicImage.Add(Mps000084ExtendSingleKey.BARCODE_IMP_MEST_CODE, barcodeImpMest);
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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetBarcodeKey();
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ListMedicine", rdo.listMrs000084ADO);
                objectTag.AddObjectData(store, "LstMedicine", rdo.listMrs000084ADO);
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
                decimal sumExpPrice = 0;
                decimal sumImpPrice = 0;
                rdo.listMrs000084ADO = new List<Mps000084ADO>();

                var listExpMestMate = new List<V_HIS_EXP_MEST_MATERIAL>();
                var listExpMestMedi = new List<V_HIS_EXP_MEST_MEDICINE>();
                var listImpMestMate = new List<V_HIS_IMP_MEST_MATERIAL>();
                var listImpMestMedi = new List<V_HIS_IMP_MEST_MEDICINE>();

                if (rdo.listExpMestMaterial != null && rdo.listExpMestMaterial.Count > 0)
                {
                    var groups = rdo.listExpMestMaterial.GroupBy(g => new { g.MATERIAL_ID }).ToList();
                    foreach (var group in groups)
                    {
                        V_HIS_EXP_MEST_MATERIAL mate = new V_HIS_EXP_MEST_MATERIAL();
                        mate = group.First();
                        mate.AMOUNT = group.Sum(s => s.AMOUNT);
                        listExpMestMate.Add(mate);
                    }
                }

                if (rdo.listExpMestMedicine != null && rdo.listExpMestMedicine.Count > 0)
                {
                    var groups = rdo.listExpMestMedicine.GroupBy(g => new { g.MEDICINE_ID }).ToList();
                    foreach (var group in groups)
                    {
                        V_HIS_EXP_MEST_MEDICINE medi = new V_HIS_EXP_MEST_MEDICINE();
                        medi = group.First();
                        medi.AMOUNT = group.Sum(s => s.AMOUNT);
                        listExpMestMedi.Add(medi);
                    }
                }

                if (rdo.listImpMestMaterial != null && rdo.listImpMestMaterial.Count > 0)
                {
                    var groups = rdo.listImpMestMaterial.GroupBy(g => g.MATERIAL_ID).ToList();
                    foreach (var group in groups)
                    {
                        V_HIS_IMP_MEST_MATERIAL mate = new V_HIS_IMP_MEST_MATERIAL();
                        mate = group.First();
                        mate.AMOUNT = group.Sum(s => s.AMOUNT);
                        listImpMestMate.Add(mate);
                    }
                }

                if (rdo.listImpMestMedicine != null && rdo.listImpMestMedicine.Count > 0)
                {
                    var groups = rdo.listImpMestMedicine.GroupBy(g => g.MEDICINE_ID).ToList();
                    foreach (var group in groups)
                    {
                        V_HIS_IMP_MEST_MEDICINE medi = new V_HIS_IMP_MEST_MEDICINE();
                        medi = group.First();
                        medi.AMOUNT = group.Sum(s => s.AMOUNT);
                        listImpMestMedi.Add(medi);
                    }
                }

                if (listImpMestMedi != null && listImpMestMedi.Count > 0)
                {
                    foreach (var item in listImpMestMedi)
                    {
                        Mps000084ADO mps000084ADO = new Mps000084ADO(item);
                        if (listExpMestMedi != null && listExpMestMedi.Count > 0)
                        {
                            var checkExistMedicine = listExpMestMedi.FirstOrDefault(o => o.MEDICINE_ID == item.MEDICINE_ID);

                            if (checkExistMedicine != null)
                            {
                                mps000084ADO.AMOUNT = checkExistMedicine.AMOUNT;
                                mps000084ADO.EXP_PRICE = (checkExistMedicine.PRICE ?? 0);
                                mps000084ADO.DISCOUNT = checkExistMedicine.DISCOUNT;
                                mps000084ADO.EXP_VAT_RATIO = (checkExistMedicine.VAT_RATIO ?? 0);
                                mps000084ADO.EXP_TOTAL_PRICE = checkExistMedicine.AMOUNT * mps000084ADO.EXP_PRICE * (1 + mps000084ADO.EXP_VAT_RATIO) - (checkExistMedicine.DISCOUNT ?? 0);
                                sumExpPrice += (checkExistMedicine.AMOUNT * mps000084ADO.EXP_PRICE * (1 + mps000084ADO.EXP_VAT_RATIO) - (checkExistMedicine.DISCOUNT ?? 0));
                                mps000084ADO.ACTIVE_INGR_BHYT_CODE = checkExistMedicine.ACTIVE_INGR_BHYT_CODE;
                                mps000084ADO.ACTIVE_INGR_BHYT_NAME = checkExistMedicine.ACTIVE_INGR_BHYT_NAME;

                            }
                        }
                        sumImpPrice += (1 + item.IMP_VAT_RATIO) * item.AMOUNT * item.IMP_PRICE;

                        rdo.listMrs000084ADO.Add(mps000084ADO);
                    }
                }
                if (listImpMestMate != null && listImpMestMate.Count > 0)
                {
                    foreach (var item in listImpMestMate)
                    {
                        Mps000084ADO mps000084ADO = new Mps000084ADO(item);
                        if (listExpMestMate != null && listExpMestMate.Count > 0)
                        {
                            var checkExistMaterial = listExpMestMate.FirstOrDefault(o => o.MATERIAL_ID == item.MATERIAL_ID);
                            if (checkExistMaterial != null)
                            {
                                mps000084ADO.AMOUNT = checkExistMaterial.AMOUNT;
                                mps000084ADO.EXP_PRICE = (checkExistMaterial.PRICE ?? 0);
                                mps000084ADO.DISCOUNT = checkExistMaterial.DISCOUNT;
                                mps000084ADO.EXP_VAT_RATIO = (checkExistMaterial.VAT_RATIO ?? 0);
                                mps000084ADO.EXP_TOTAL_PRICE = checkExistMaterial.AMOUNT * mps000084ADO.EXP_PRICE * (1 + mps000084ADO.EXP_VAT_RATIO) - (checkExistMaterial.DISCOUNT ?? 0);
                                sumExpPrice += (checkExistMaterial.AMOUNT * mps000084ADO.EXP_PRICE * (1 + mps000084ADO.EXP_VAT_RATIO) - (checkExistMaterial.DISCOUNT ?? 0));
                            }
                        }
                        sumImpPrice += (1 + item.IMP_VAT_RATIO) * item.AMOUNT * item.IMP_PRICE;
                        rdo.listMrs000084ADO.Add(mps000084ADO);
                    }
                }
                AddObjectKeyIntoListkey<V_HIS_IMP_MEST>(rdo.ImpMest, false);
                AddObjectKeyIntoListkey<SingleKey>(rdo.singleKey, false);
                SetSingleKey(new KeyValue(Mps000084ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo.ImpMest.IMP_TIME ?? 0))));
                SetSingleKey(new KeyValue(Mps000084ExtendSingleKey.SUM_EXP_PRICE_NUM,sumExpPrice));
                SetSingleKey(new KeyValue(Mps000084ExtendSingleKey.SUM_EXP_PRICE, Inventec.Common.Number.Convert.NumberToString(sumExpPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                SetSingleKey(new KeyValue(Mps000084ExtendSingleKey.SUM_IMP_PRICE_NUM, sumImpPrice));
                SetSingleKey(new KeyValue(Mps000084ExtendSingleKey.SUM_IMP_PRICE, Inventec.Common.Number.Convert.NumberToString(sumImpPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumImpPrice));
                SetSingleKey(new KeyValue(Mps000084ExtendSingleKey.SUM_EXP_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                if (rdo.ImpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000084ExtendSingleKey.VIR_PATIENT_NAME, rdo.ImpMest.TDL_PATIENT_NAME));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

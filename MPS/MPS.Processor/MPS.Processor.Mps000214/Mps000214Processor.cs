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
using MPS.Processor.Mps000214.PDO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System.Linq;

namespace MPS.Processor.Mps000214
{
    class Mps000214Processor : AbstractProcessor
    {
        Mps000214PDO rdo;

        public Mps000214Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000214PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {

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
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
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
                decimal sumExpPrice = 0, sumImpPrice = 0;
                rdo.listMrs000084ADO = new List<Mps000214ADO>();

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

                Inventec.Common.Logging.LogSystem.Debug("SetSingleKey listImpMestMedi " + Inventec.Common.Logging.LogUtil.TraceData("", listImpMestMedi));
                if (listImpMestMedi != null && listImpMestMedi.Count > 0)
                {
                    foreach (var item in listImpMestMedi)
                    {
                        Mps000214ADO mps000084ADO = new Mps000214ADO(item);
                        if (listExpMestMedi != null && listExpMestMedi.Count > 0)
                        {
                            var checkExistMedicine = listExpMestMedi.FirstOrDefault(o => o.MEDICINE_ID == item.MEDICINE_ID);

                            if (checkExistMedicine != null)
                            {
                                mps000084ADO.EXP_PRICE = (checkExistMedicine.PRICE ?? 0);
                                mps000084ADO.DISCOUNT = checkExistMedicine.DISCOUNT;
                                mps000084ADO.EXP_VAT_RATIO = (checkExistMedicine.VAT_RATIO ?? 0);
                                mps000084ADO.EXP_TOTAL_PRICE = mps000084ADO.AMOUNT * mps000084ADO.EXP_PRICE * (1 + mps000084ADO.EXP_VAT_RATIO) - (checkExistMedicine.DISCOUNT ?? 0);
                                sumExpPrice += (mps000084ADO.AMOUNT * mps000084ADO.EXP_PRICE * (1 + mps000084ADO.EXP_VAT_RATIO) - (checkExistMedicine.DISCOUNT ?? 0));
                                sumImpPrice += mps000084ADO.IMP_TOTAL_PRICE;
                            }
                        }

                        rdo.listMrs000084ADO.Add(mps000084ADO);
                    }
                }
                if (listImpMestMate != null && listImpMestMate.Count > 0)
                {
                    foreach (var item in listImpMestMate)
                    {
                        Mps000214ADO mps000214ADO = new Mps000214ADO(item);
                        if (listExpMestMate != null && listExpMestMate.Count > 0)
                        {
                            var checkExistMaterial = listExpMestMate.FirstOrDefault(o => o.MATERIAL_ID == item.MATERIAL_ID);
                            if (checkExistMaterial != null)
                            {
                                mps000214ADO.EXP_PRICE = (checkExistMaterial.PRICE ?? 0);
                                mps000214ADO.DISCOUNT = checkExistMaterial.DISCOUNT;
                                mps000214ADO.EXP_VAT_RATIO = (checkExistMaterial.VAT_RATIO ?? 0);
                                mps000214ADO.EXP_TOTAL_PRICE = mps000214ADO.AMOUNT * mps000214ADO.EXP_PRICE * (1 + mps000214ADO.EXP_VAT_RATIO) - (checkExistMaterial.DISCOUNT ?? 0);
                                sumExpPrice += (mps000214ADO.AMOUNT * mps000214ADO.EXP_PRICE * (1 + mps000214ADO.EXP_VAT_RATIO) - (checkExistMaterial.DISCOUNT ?? 0));
                            }
                        }
                        rdo.listMrs000084ADO.Add(mps000214ADO);
                    }
                }
                string SumEmpPriceSeparate = Inventec.Common.Number.Convert.NumberToString(sumExpPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                string SumImpPriceSeparate = Inventec.Common.Number.Convert.NumberToString(sumImpPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                AddObjectKeyIntoListkey<V_HIS_IMP_MEST>(rdo.ImpMest, false);
                SetSingleKey(new KeyValue(Mps000214ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo.ImpMest.IMP_TIME ?? 0))));
                SetSingleKey(new KeyValue(Mps000214ExtendSingleKey.SUM_EXP_PRICE, sumExpPrice));
                SetSingleKey(new KeyValue(Mps000214ExtendSingleKey.SUM_EXP_PRICE_SEPARATE, SumEmpPriceSeparate));
                SetSingleKey(new KeyValue(Mps000214ExtendSingleKey.SUM_IMP_PRICE, sumImpPrice));
                SetSingleKey(new KeyValue(Mps000214ExtendSingleKey.SUM_IMP_PRICE_SEPARATE, SumImpPriceSeparate));
                string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumExpPrice));
                SetSingleKey(new KeyValue(Mps000214ExtendSingleKey.SUM_EXP_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));
                AddObjectKeyIntoListkey<HIS_EXP_MEST>(rdo.ExpMest, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

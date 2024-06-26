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
using MOS.EFMODEL.DataModels;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000084
{
    /// <summary>
    /// In yeu cau kham.
    /// Dau vao:
    /// PatyAlterBhyt: doi tuong the bhyt
    /// ServiceReq: yeu cau dich vu
    /// PatientType: doi tuong benh nhan
    /// </summary>
    public class Mps000084RDO : RDOBase
    {
        internal HIS_IMP_MEST ImpMest { get; set; }
        internal V_HIS_MOBA_IMP_MEST MobaImpMest { get; set; }
        internal List<Mps000084ADO> listMrs000084ADO;
        internal MOS.EFMODEL.DataModels.V_HIS_SALE_EXP_MEST saleExpMest { get; set; }

        List<V_HIS_IMP_MEST_MEDICINE> listImpMestMedicine = null;
        List<V_HIS_IMP_MEST_MATERIAL> listImpMestMaterial = null;
        List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine = null;
        List<V_HIS_EXP_MEST_MATERIAL> listExpMestMaterial = null;

        public Mps000084RDO(
            HIS_IMP_MEST ImpMest,
            V_HIS_MOBA_IMP_MEST MobaImpMest,
            List<V_HIS_IMP_MEST_MEDICINE> hisImpMestMedicines,
            List<V_HIS_IMP_MEST_MATERIAL> hisImpMestMaterials,
            List<V_HIS_EXP_MEST_MEDICINE> _listExpMestMedicine,
            List<V_HIS_EXP_MEST_MATERIAL> _listExpMestMaterial,
            MOS.EFMODEL.DataModels.V_HIS_SALE_EXP_MEST _saleExpMest
            )
        {
            try
            {
                this.ImpMest = ImpMest;
                this.MobaImpMest = MobaImpMest;
                this.listImpMestMedicine = hisImpMestMedicines;
                this.listImpMestMaterial = hisImpMestMaterials;
                this.listExpMestMedicine = _listExpMestMedicine;
                this.listExpMestMaterial = _listExpMestMaterial;
                this.saleExpMest = _saleExpMest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                decimal sumExpPrice = 0;
                listMrs000084ADO = new List<Mps000084ADO>();
                if (listImpMestMedicine != null && listImpMestMedicine.Count > 0)
                {
                    // sắp xếp theo tăng dần của id 
                    listImpMestMedicine = listImpMestMedicine.OrderBy(o => o.ID).ToList();
                    foreach (var item in listImpMestMedicine)
                    {
                        Mps000084ADO mps000084ADO = new ADO.Mps000084ADO(item);
                        if (this.listExpMestMedicine != null && this.listExpMestMedicine.Count > 0)
                        {
                            var checkExistMedicine = this.listExpMestMedicine.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID);

                            if (checkExistMedicine != null)
                            {
                                mps000084ADO.EXP_PRICE = (checkExistMedicine.PRICE ?? 0);
                                mps000084ADO.DISCOUNT = checkExistMedicine.DISCOUNT;
                                mps000084ADO.EXP_VAT_RATIO = (checkExistMedicine.VAT_RATIO ?? 0);
                                mps000084ADO.EXP_TOTAL_PRICE = checkExistMedicine.AMOUNT * mps000084ADO.EXP_PRICE * (1 + mps000084ADO.EXP_VAT_RATIO) - (checkExistMedicine.DISCOUNT ?? 0);
                                sumExpPrice += (checkExistMedicine.AMOUNT * mps000084ADO.EXP_PRICE * (1 + mps000084ADO.EXP_VAT_RATIO) - (checkExistMedicine.DISCOUNT ?? 0));
                            }
                        }

                        listMrs000084ADO.Add(mps000084ADO);
                    }
                }
                if (listImpMestMaterial != null && listImpMestMaterial.Count > 0)
                {
                    // sắp xếp theo tăng dần của id
                    listImpMestMaterial = listImpMestMaterial.OrderBy(p => p.ID).ToList();

                    foreach (var item in listImpMestMaterial)
                    {
                        Mps000084ADO mps000084ADO = new ADO.Mps000084ADO(item);
                        if (listExpMestMaterial != null && listExpMestMaterial.Count > 0)
                        {
                            var checkExistMaterial = listExpMestMaterial.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID);
                            if (checkExistMaterial != null)
                            {
                                mps000084ADO.EXP_PRICE = (checkExistMaterial.PRICE ?? 0);
                                mps000084ADO.DISCOUNT = checkExistMaterial.DISCOUNT;
                                mps000084ADO.EXP_VAT_RATIO = (checkExistMaterial.VAT_RATIO ?? 0);
                                mps000084ADO.EXP_TOTAL_PRICE = checkExistMaterial.AMOUNT * mps000084ADO.EXP_PRICE * (1 + mps000084ADO.EXP_VAT_RATIO) - (checkExistMaterial.DISCOUNT ?? 0);
                                sumExpPrice += (checkExistMaterial.AMOUNT * mps000084ADO.EXP_PRICE * (1 + mps000084ADO.EXP_VAT_RATIO) - (checkExistMaterial.DISCOUNT ?? 0));
                            }
                        }
                        listMrs000084ADO.Add(mps000084ADO);
                    }
                }
                GlobalQuery.AddObjectKeyIntoListkey<HIS_IMP_MEST>(ImpMest, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_MOBA_IMP_MEST>(MobaImpMest, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SALE_EXP_MEST>(this.saleExpMest, keyValues, false);

                keyValues.Add(new KeyValue(Mps000084ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((ImpMest.IMP_TIME ?? 0))));
                keyValues.Add(new KeyValue(Mps000084ExtendSingleKey.SUM_EXP_PRICE, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumExpPrice)));
                string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumExpPrice));
                keyValues.Add(new KeyValue(Mps000084ExtendSingleKey.SUM_EXP_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}

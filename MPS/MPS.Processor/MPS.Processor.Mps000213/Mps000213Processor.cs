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
using MPS.Processor.Mps000213.PDO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System.Linq;
using HIS.Desktop.LocalStorage.ConfigApplication;

namespace MPS.Processor.Mps000213
{
    class Mps000213Processor : AbstractProcessor
    {
        Mps000213PDO rdo;

        public Mps000213Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000213PDO)rdoBase;
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
                objectTag.AddObjectData(store, "ListBlood", rdo.listMrs000213ADO);
                objectTag.AddObjectData(store, "LstBlood", rdo.listMrs000213ADO);
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
                rdo.listMrs000213ADO = new List<Mps000213ADO>();
                var listExpMestMedi = new List<V_HIS_IMP_MEST_BLOOD>();
                if (rdo.listExpMestBlood != null && rdo.listExpMestBlood.Count > 0)
                {
                    foreach (var item in rdo.listExpMestBlood)
                    {
                        Mps000213ADO mps000084ADO = new Mps000213ADO(item);
                        sumExpPrice += (mps000084ADO.IMP_PRICE * (1 + mps000084ADO.IMP_VAT_RATIO));
                        rdo.listMrs000213ADO.Add(mps000084ADO);
                    }
                }
                AddObjectKeyIntoListkey<V_HIS_IMP_MEST>(rdo.ImpMest, false);
                SetSingleKey(new KeyValue(Mps000213ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((rdo.ImpMest.IMP_TIME ?? 0))));
                SetSingleKey(new KeyValue(Mps000213ExtendSingleKey.SUM_EXP_PRICE, Inventec.Common.Number.Convert.NumberToString(sumExpPrice, ConfigApplications.NumberSeperator)));
                string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundAuto(sumExpPrice, ConfigApplications.NumberSeperator));
                SetSingleKey(new KeyValue(Mps000213ExtendSingleKey.SUM_EXP_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

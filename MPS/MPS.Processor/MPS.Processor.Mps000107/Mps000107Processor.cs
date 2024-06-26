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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000107.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000107
{
    public class Mps000107Processor : AbstractProcessor
    {
        Mps000107PDO rdo;
        public Mps000107Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000107PDO)rdoBase;
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
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);

                List<PrintFullRowData> printFullRowDatas = new List<PrintFullRowData>();
                for (int i = 0; i < 50; i++)
                {
                    printFullRowDatas.Add(new PrintFullRowData(""));
                }

                objectTag.AddObjectData(store, "ExpMestBlty1", rdo.HisExpMestBltys);
                objectTag.AddObjectData(store, "ExpMestBlty2", rdo.HisExpMestBltys);
                objectTag.AddObjectData(store, "Blood1", rdo.ListBloodADO);
                objectTag.AddObjectData(store, "Blood2", rdo.ListBloodADO);
                objectTag.AddObjectData(store, "PrintFullRows1", printFullRowDatas);
                objectTag.AddObjectData(store, "PrintFullRows2", printFullRowDatas);
                objectTag.SetUserFunction(store, "FuncFixPrintFullRow", new FuncFixPrintFullRow(rdo.ListBloodADO, rdo.HisExpMestBltys.Count()));

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.HisServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000107ExtendSingleKey.PATIENT_CODE, rdo.HisServiceReq.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000107ExtendSingleKey.VIR_PATIENT_NAME, rdo.HisServiceReq.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000107ExtendSingleKey.DOB_YEAR_STR, rdo.HisServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000107ExtendSingleKey.GENDER_NAME, rdo.HisServiceReq.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000107ExtendSingleKey.VIR_ADDRESS, rdo.HisServiceReq.TDL_PATIENT_ADDRESS));
                    string icdText = null;
                    if (!String.IsNullOrEmpty(rdo.HisServiceReq.ICD_TEXT))
                    {
                        icdText = rdo.HisServiceReq.ICD_TEXT;
                    }
                    else
                    {
                        icdText = rdo.HisServiceReq.ICD_NAME;
                    }
                    SetSingleKey(new KeyValue(Mps000107ExtendSingleKey.ICD_MAIN_TEXT, icdText));
                }

                if (rdo.HisExpMestBloods != null && rdo.HisExpMestBloods.Count > 0)
                {
                    decimal VirTotalPrice = rdo.HisExpMestBloods.Sum(o => (o.IMP_PRICE * (o.IMP_VAT_RATIO + 1)));
                    string VirTotalPriceSeparate = Inventec.Common.Number.Convert.NumberToString(VirTotalPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    SetSingleKey(new KeyValue(Mps000107ExtendSingleKey.VIR_TOTAL_PRICE, VirTotalPrice));
                    SetSingleKey(new KeyValue(Mps000107ExtendSingleKey.VIR_TOTAL_PRICE_SEPARATE, VirTotalPriceSeparate));
                    SetSingleKey(new KeyValue(Mps000107ExtendSingleKey.VIR_TOTAL_PRICE_OTHER, 0));

                    rdo.ListBloodADO = (from r in rdo.HisExpMestBloods select new ExpMestBloodADO(r)).ToList();
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                }

                if (rdo.HisExpMestBltys == null)
                {
                    rdo.HisExpMestBltys = new List<V_HIS_EXP_MEST_BLTY_REQ>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    internal class FuncFixPrintFullRow : TFlexCelUserFunction
    {
        IList list;
        int countType;
        internal FuncFixPrintFullRow(IList list, int countType)
        {
            this.list = list;
            this.countType = countType;
        }

        public override object Evaluate(object[] parameters)
        {
            bool result = false;
            int count = 0;
            try
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                if (this.list is IList)
                {
                    count = this.list.Count + this.countType;
                }

                int rowPosition = Convert.ToInt32(parameters[0]);
                int maxRowCount = Convert.ToInt32(parameters[1]);

                if (count < maxRowCount)
                {
                    int rowCountRuntime = maxRowCount - count;
                    if (rowPosition > rowCountRuntime)
                    {
                        result = true;
                    }
                }
                else
                    result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }

    internal class PrintFullRowData
    {
        public string Data { get; set; }
        public PrintFullRowData(string data)
        {
            this.Data = data;
        }
    }
}

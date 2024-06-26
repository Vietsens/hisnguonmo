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
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000422.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000422
{
    public class Mps000422Processor : AbstractProcessor
    {
        Mps000422PDO rdo;
        List<Mps000422ADO> listAdo = new List<Mps000422ADO>();
        public Mps000422Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000422PDO)rdoBase;
        }


        public override bool ProcessData()
        {
            bool result = false;
            try
            {

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                //đọc template trong MPS
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessListData();
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListExpMest", this.listAdo);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.listAdo), this.listAdo));
                result = true;
            }
            catch (Exception ex)
            {
                return result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessListData()
        {
            try
            {
                if (rdo != null)
                {
                    if (rdo._ListMestBLTYReq != null)
                    {
                        foreach (var item in rdo._ListMestBLTYReq)
                        {
                            int amount = (int)item.AMOUNT;
                            for (int i = 0; i < amount; i++)
                            {
                                this.listAdo.Add(new Mps000422ADO(rdo._SaleExpMest, item, i + 1));
                            }
                            this.listAdo.Add(new Mps000422ADO(rdo._SaleExpMest, item, null));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSingleKey()
        {
            try
            {
                long sum = 0;

                if (this.rdo._ListMestBLTYReq != null)
                {
                    foreach (var item in this.rdo._ListMestBLTYReq)
                    {
                        sum += item.AMOUNT;
                    }
                    SetSingleKey(new KeyValue(Mps000422ExtendSingleKey.SUM, sum));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

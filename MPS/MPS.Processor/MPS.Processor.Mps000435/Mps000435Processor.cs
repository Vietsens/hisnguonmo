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
using MPS.Processor.Mps000435.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000435
{
    public class Mps000435Processor : AbstractProcessor
    {
        Mps000435PDO rdo;

        List<Mps000435ADO> listData = new List<Mps000435ADO>();
        List<Mps000435ADO> listTrayNumber = new List<Mps000435ADO>();
        public Mps000435Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000435PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListData", this.listData);
                objectTag.AddObjectData(store, "listTrayNumber", this.listTrayNumber);
                objectTag.AddRelationship(store, "listTrayNumber", "ListData", "TRAY_NUMBER", "TRAY_NUMBER");
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
                foreach (var item in this.rdo.ListSampleAggrPrint)
                {
                    Mps000435ADO ado = new Mps000435ADO();

                    var expandoDict = item as IDictionary<string, object>;
                    System.Reflection.PropertyInfo[] pis = Inventec.Common.Repository.Properties.Get<Mps000435ADO>();
                    foreach (var pi in pis)
                    {
                        if (expandoDict.ContainsKey(pi.Name))
                            pi.SetValue(ado, expandoDict[pi.Name]);
                    }

                    ado.AGGREGATE_BARCODE = (expandoDict["AGGREGATE_BARCODE"] ?? "").ToString();
                    listData.Add(ado);
                    if (!listTrayNumber.Exists(k => k.TRAY_NUMBER == ado.TRAY_NUMBER))
                        listTrayNumber.Add(ado);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listTrayNumber), listTrayNumber)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listData), listData));

                //kiểm tra danh sách và tạo key để ẩn hiện cột tương ứng.
                List<string> listKeys = new List<string>();
                System.Reflection.PropertyInfo[] pii = Inventec.Common.Repository.Properties.Get<Mps000435ADO>();
                foreach (var pi in pii)
                {
                    if (pi.Name.Contains("BARCODE_"))
                    {
                        foreach (var item in listData)
                        {
                            var bar = pi.GetValue(item);
                            if (bar != null && !String.IsNullOrWhiteSpace(bar.ToString()) && !listKeys.Contains(pi.Name))
                            {
                                listKeys.Add(pi.Name);
                            }
                        }
                    }
                }

                foreach (string key in listKeys)
                {
                    SetSingleKey(new KeyValue(key, key));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlexCel.Report;
using Inventec.Common.Logging;

namespace MPS.Core.Mps000115
{
    class Mps000115Processor : ProcessorBase, IProcessorPrint
    {
        Mps000115RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;

        internal Mps000115Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000115RDO)data;
            this.fileName = fileName;
            store = new Inventec.Common.FlexCellExport.Store();
        }

        bool IProcessorPrint.Run()
        {
            bool result = false;
            bool valid = true;
            try
            {
                SetCommonSingleKey();
                rdo.SetSingleKey();
                SetSingleKey();

                store.SetCommonFunctions();

                //Ham xu ly cac doi tuong su dung trong thu vien flexcelexport
                valid = valid && ProcessData();
                if (valid)
                {
                    using (MemoryStream streamResult = store.OutStream())
                    {
                        //Print preview
                        result = PrintPreview(streamResult, this.fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool ProcessData()
        {
            bool result = false;
            try
            {
                SetSingleKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);

                List<PrintFullRowData> printFullRowDatas = new List<PrintFullRowData>();
                for (int i = 0; i < 50; i++)
                {
                    printFullRowDatas.Add(new PrintFullRowData(""));
                }

                objectTag.AddObjectData(store, "Print1", rdo._listInvoiceDetails);
                objectTag.AddObjectData(store, "Print2", rdo._listInvoiceDetails);
                objectTag.AddObjectData(store, "Print3", rdo._listInvoiceDetails);

                objectTag.AddObjectData(store, "PrintRow1", printFullRowDatas);
                objectTag.AddObjectData(store, "PrintRow2", printFullRowDatas);
                objectTag.AddObjectData(store, "PrintRow3", printFullRowDatas);
                objectTag.SetUserFunction(store, "FuncFixPrintFullRow", new FuncFixPrintFullRow(rdo._listInvoiceDetails));


                objectTag.AddObjectData(store, "sereServGroups1", rdo._listInvoiceDetailsADOs);
                objectTag.AddObjectData(store, "sereServGroups2", rdo._listInvoiceDetailsADOs);
                objectTag.AddObjectData(store, "sereServGroups3", rdo._listInvoiceDetailsADOs);

                objectTag.AddObjectData(store, "PayForm1", rdo._payForm);
                objectTag.AddObjectData(store, "PayForm2", rdo._payForm);
                objectTag.AddObjectData(store, "PayForm3", rdo._payForm);

                objectTag.AddObjectData(store, "totalNextPages", rdo._totalNextPageSdos);
                objectTag.AddObjectData(store, "totalNextPages1", rdo._totalNextPageSdos);
                objectTag.AddObjectData(store, "totalNextPages2", rdo._totalNextPageSdos);
                store.SetCommonFunctions();
                objectTag.AddRelationship(store, "totalNextPages", "sereServGroups1", "Id", "PageId");
                objectTag.AddRelationship(store, "totalNextPages1", "sereServGroups2", "Id", "PageId");
                objectTag.AddRelationship(store, "totalNextPages2", "sereServGroups3", "Id", "PageId");
                store.SetCommonFunctions();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }

    public class PrintFullRowData
    {
        public string Data { get; set; }
        public PrintFullRowData(string data)
        {
            this.Data = data;
        }
    }

    internal class FuncFixPrintFullRow : TFlexCelUserFunction
    {
        IList list;
        internal FuncFixPrintFullRow(IList list)
        {
            this.list = list;
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
                    count = this.list.Count;
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
                LogSystem.Debug(ex);
            }

            return result;
        }
    }
}

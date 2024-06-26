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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000089
{
    class Mps000089Processor : ProcessorBase, IProcessorPrint
    {
        Mps000089RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;

        internal Mps000089Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000089RDO)data;
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
                        streamResult.Position = 0;
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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "MedicineTypes1", rdo.listAdo);
                objectTag.AddObjectData(store, "MedicineTypes2", rdo.listAdo);
                objectTag.AddObjectData(store, "MedicineTypes3", rdo.listAdo);
                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData21", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData22", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData31", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData32", new CalculateMergerData());
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }

    class CalculateMergerData : TFlexCelUserFunction
    {
        long medicineTypeId = 0;

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
            bool result = false;
            try
            {
                long mediId = Convert.ToInt64(parameters[0]);

                if (mediId > 0)
                {
                    if (this.medicineTypeId == mediId)
                    {
                        return true;
                    }
                    else
                    {
                        this.medicineTypeId = mediId;
                        return false;
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
    }
}

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
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MPS.Core.Mps000189
{
    class Mps000189Processor : ProcessorBase, IProcessorPrint
    {
        Mps000189RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;


        internal Mps000189Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000189RDO)data;
            this.fileName = fileName;
            store = new Inventec.Common.FlexCellExport.Store();
            dicImage = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
        }

        bool IProcessorPrint.Run()
        {
            bool result = false;
            bool valid = true;
            try
            {
                //Xu ly du lieu tho
                rdo.DataInputProcess();
                if ((rdo.sereServExamADOs == null || rdo.sereServExamADOs.Count == 0)
                    && (rdo.sereServServiceADOs == null || rdo.sereServServiceADOs.Count == 0))
                    return false;
                rdo.HeinServiceTypeProcess();
                SetCommonSingleKey();
                rdo.SetSingleKey();
                SetSingleKey();

                //Cac ham dac thu khac cua rdo
                SetBarcodeKey();

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

        internal void SetBarcodeKey()
        {
            try
            {

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000189ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
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
        protected bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "HeinServiceType", rdo.heinServiceTypes);
                objectTag.AddObjectData(store, "HeinServiceExamType", rdo.heinServiceExamTypes);
                objectTag.AddObjectData(store, "ExecuteRoom", rdo.executeRooms);
                objectTag.AddObjectData(store, "RequestRoom", rdo.requestRooms);
                objectTag.AddObjectData(store, "ServiceExam", rdo.sereServExamADOs);
                objectTag.AddObjectData(store, "Service", rdo.sereServServiceADOs);
                objectTag.AddRelationship(store, "ExecuteRoom", "HeinServiceExamType", "EXECUTE_ROOM_ID", "EXECUTE_ROOM_ID");
                objectTag.AddRelationship(store, "ExecuteRoom", "ServiceExam", "EXECUTE_ROOM_ID", "EXECUTE_ROOM_ID");
                objectTag.AddRelationship(store, "HeinServiceExamType", "ServiceExam", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "RequestRoom", "HeinServiceType", "REQUEST_ROOM_ID", "REQUEST_ROOM_ID");
                objectTag.AddRelationship(store, "RequestRoom", "Service", "REQUEST_ROOM_ID", "REQUEST_ROOM_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "Service", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.SetUserFunction(store, "ReplaceValue", new ReplaceValueFunction());

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

    class ReplaceValueFunction : FlexCel.Report.TFlexCelUserFunction
    {
        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

            try
            {
                string value = parameters[0] + "";
                if (!String.IsNullOrEmpty(value))
                {
                    value = value.Replace(';', '/');
                }
                return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return parameters[0];
            }
        }
    }
}

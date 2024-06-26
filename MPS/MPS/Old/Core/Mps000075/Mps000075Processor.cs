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
using System.Linq;


namespace MPS.Core.Mps000075
{
    class Mps000075Processor : ProcessorBase, IProcessorPrint
    {
        Mps000075RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;


        internal Mps000075Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000075RDO)data;
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
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000075ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

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
                // remove hightTech service
                //List<long> highTechServiceIds = rdo.hightTechServices.Select(o => o.SERVICE_ID).ToList();
                //var serviceInHighTechFilters = rdo.serviceInHightTechs.Where(o => highTechServiceIds.Contains(o.SERVICE_PACKAGE_ID??0)).ToList();
                //List<long> serviceInHighTechFilterIds = serviceInHighTechFilters.Select(o => o.ID).ToList();
                //rdo.sereServADOs.RemoveAll(o => serviceInHighTechFilterIds.Contains(o.ID));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ServiceGroups", rdo.ListGroupServiceTypeADO);
                objectTag.AddObjectData(store, "Services", rdo.sereServADOs);
                objectTag.AddObjectData(store, "Departments", rdo.departmentADOs);

                objectTag.AddObjectData(store, "HightServiceGroups", rdo.highTechServiceReports);
                objectTag.AddObjectData(store, "HightTechServices", rdo.hightTechServices);
                objectTag.AddObjectData(store, "HightTechDepartments", rdo.highTechDepartments);
                objectTag.AddObjectData(store, "ServicesInHightTech", rdo.serviceInHightTechs);

                objectTag.AddRelationship(store, "Departments", "Services", "DEPARTMENT__SERVICE_GROUP__ID", "DEPARTMENT__SERVICE_GROUP__ID");
                objectTag.AddRelationship(store, "ServiceGroups", "Departments", "ID", "HEIN_SERVICE_TYPE_ID");

                objectTag.AddRelationship(store, "HightServiceGroups", "HightTechServices", "ID", "HEIN_SERVICE_TYPE_ID");
                //objectTag.AddRelationship(store, "HightTechServices", "HightTechDepartments", "REQUEST_DEPARTMENT_ID", "REQUEST_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "HightTechServices", "ServicesInHightTech", "SERVICE_ID", "SERVICE_PACKAGE_ID");

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
}

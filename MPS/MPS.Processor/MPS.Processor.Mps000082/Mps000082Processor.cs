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
using Inventec.Common.Logging;
using Inventec.Core;
using MPS.Processor.Mps000082.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000082
{
    class Mps000082Processor : AbstractProcessor
    {
        Mps000082PDO rdo;
        public Mps000082Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000082PDO)rdoBase;
        }

        
        
        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentHisTreatment.TREATMENT_CODE);
                barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatmentCode.IncludeLabel = false;
                barcodeTreatmentCode.Width = 120;
                barcodeTreatmentCode.Height = 40;
                barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatmentCode.IncludeLabel = true;

                dicImage.Add(Mps000082ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);

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
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "Services", rdo.sereServNotHiTechs);
                objectTag.AddObjectData(store, "ServiceGroups", rdo.ServiceGroups);
                objectTag.AddObjectData(store, "Departments", rdo.DepartmentGroups);
                objectTag.AddObjectData(store, "HightTechDepartments", rdo.HightTechDepartmentGroups);
                objectTag.AddObjectData(store, "ServiceVTTTDepartments", rdo.ServiceVTTTDepartmentGroup);
                objectTag.AddObjectData(store, "ServiceHightTechs", rdo.sereServHitechs);
                objectTag.AddObjectData(store, "ServiceVTTTs", rdo.sereServVTTTs);

                objectTag.AddRelationship(store, "ServiceGroups", "Departments", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "Departments", "Services", "REQUEST_DEPARTMENT_ID", "REQUEST_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroups", "Services", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceGroups", "HightTechDepartments", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceGroups", "ServiceHightTechs", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");

                objectTag.AddRelationship(store, "HightTechDepartments", "ServiceHightTechs", "EXECUTE_DEPARTMENT_ID", "EXECUTE_DEPARTMENT_ID");

                objectTag.AddRelationship(store, "ServiceGroups", "ServiceVTTTDepartments", "HEIN_SERVICE_TYPE_ID", "DEPARTMENT__GROUP_SERVICE_REPORT");

                objectTag.AddRelationship(store, "ServiceVTTTDepartments", "ServiceVTTTs", "REQUEST_DEPARTMENT_ID", "REQUEST_DEPARTMENT_ID");

                objectTag.AddRelationship(store, "ServiceGroups", "ServiceVTTTs", "HEIN_SERVICE_TYPE_ID", "SERE_SERV__GROUP_SERVICE_REPORT");





                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        class CustomerFuncRownumberData : TFlexCelUserFunction
        {
            public CustomerFuncRownumberData()
            {
            }
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                long result = 0;
                try
                {
                    long rownumber = Convert.ToInt64(parameters[0]);
                    result = (rownumber + 1);
                }
                catch (Exception ex)
                {
                    LogSystem.Debug(ex);
                }

                return result;
            }
        }
    }
}

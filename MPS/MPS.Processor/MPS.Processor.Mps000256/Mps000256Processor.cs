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
using MPS.Processor.Mps000256.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000256
{
    public class Mps000256Processor : AbstractProcessor
    {
        Mps000256PDO rdo;
        List<Mps000256SDO> ListExport = new List<Mps000256SDO>();
        List<Mps000256SDO> ListExportParent = new List<Mps000256SDO>();
        public Mps000256Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000256PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = true;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                ProcessorDataExport();
                SetSingleKey();

                if (ListExportParent != null && ListExportParent.Count > 0)
                {
                    ListExportParent = ListExportParent.OrderBy(o => o.SERVICE_TYPE_CODE).ToList();
                }

                if (ListExport != null && ListExport.Count > 0)
                {
                    ListExport = ListExport.OrderBy(o => o.SERVICE_CODE_PARENT).ThenBy(o => o.SERVICE_CODE).ToList();
                }

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Export", ListExport);
                objectTag.AddObjectData(store, "ExportParent", ListExportParent);
                objectTag.AddObjectData(store, "ServicePaty", rdo.ListVServicePaty);
                objectTag.AddRelationship(store, "ExportParent", "Export", "SERVICE_TYPE_ID", "SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "Export", "ServicePaty", "ID", "SERVICE_ID");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void SetSingleKey()
        {

        }

        private void ProcessorDataExport()
        {
            try
            {
                if (rdo != null && rdo.ListVService != null)
                {
                    Dictionary<long, Mps000256SDO> dicPatient = new Dictionary<long, Mps000256SDO>();
                    foreach (var service in rdo.ListVService)
                    {
                        if (service == null) continue;

                        Mps000256SDO sdo = new Mps000256SDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000256SDO>(sdo, service);
                        if (service.PACKAGE_ID.HasValue && rdo.ListPackage != null)
                        {
                            var package = rdo.ListPackage.FirstOrDefault(o => o.ID == service.PACKAGE_ID.Value);
                            sdo.PACKAGE_CODE = package != null ? package.PACKAGE_CODE : "";
                            sdo.PACKAGE_NAME = package != null ? package.PACKAGE_NAME : "";
                        }

                        if (service.PTTT_GROUP_ID.HasValue && rdo.ListPtttGroup != null)
                        {
                            var ptttGroup = rdo.ListPtttGroup.FirstOrDefault(o => o.ID == service.PTTT_GROUP_ID.Value);
                            sdo.PTTT_GROUP_CODE = ptttGroup != null ? ptttGroup.PTTT_GROUP_CODE : "";
                            sdo.PTTT_GROUP_NAME = ptttGroup != null ? ptttGroup.PTTT_GROUP_NAME : "";
                        }

                        if (service.PTTT_METHOD_ID.HasValue && rdo.ListPtttMethod != null)
                        {
                            var ptttMethod = rdo.ListPtttMethod.FirstOrDefault(o => o.ID == service.PTTT_METHOD_ID.Value);
                            sdo.PTTT_METHOD_CODE = ptttMethod != null ? ptttMethod.PTTT_METHOD_CODE : "";
                            sdo.PTTT_METHOD_NAME = ptttMethod != null ? ptttMethod.PTTT_METHOD_NAME : "";
                        }

                        if (rdo.Mps000256ADO != null && rdo.ListVServicePaty != null)
                        {
                            if (rdo.Mps000256ADO.PATIENT_TYPE_ID__BHYT.HasValue)
                            {
                                var patyBhyt = rdo.ListVServicePaty.FirstOrDefault(o => o.SERVICE_ID == service.ID && o.PATIENT_TYPE_ID == rdo.Mps000256ADO.PATIENT_TYPE_ID__BHYT.Value);
                                if (patyBhyt != null) sdo.BHYT_PRICE = patyBhyt.PRICE;
                            }

                            if (rdo.Mps000256ADO.PATIENT_TYPE_ID__VP.HasValue)
                            {
                                var patyVp = rdo.ListVServicePaty.FirstOrDefault(o => o.SERVICE_ID == service.ID && o.PATIENT_TYPE_ID == rdo.Mps000256ADO.PATIENT_TYPE_ID__VP.Value);
                                if (patyVp != null) sdo.VP_PRICE = patyVp.PRICE;
                            }
                        }

                        if (service.PARENT_ID.HasValue)
                        {
                            var parent = rdo.ListVService.FirstOrDefault(o => o.ID == service.PARENT_ID);
                            sdo.SERVICE_CODE_PARENT = parent != null ? parent.SERVICE_CODE : "";
                            sdo.SERVICE_NAME_PARENT = parent != null ? parent.SERVICE_NAME : "";
                        }
                        if ( rdo.ListVServicePaty == null || rdo.ListVServicePaty.Count == 0 || (rdo.ListVServicePaty != null && rdo.ListVServicePaty.FirstOrDefault(o=>o.SERVICE_ID == service.ID) == null))
                        {
                            rdo.ListVServicePaty.Add(new MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY() { SERVICE_ID = service.ID});
                        }

                        ListExport.Add(sdo);

                        if (!dicPatient.ContainsKey(service.SERVICE_TYPE_ID))
                        {
                            Mps000256SDO parent = new Mps000256SDO();

                            parent.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                            parent.SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                            parent.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                            dicPatient.Add(service.SERVICE_TYPE_ID, parent);
                        }
                    }

                    if (dicPatient.Count > 0)
                    {
                        ListExportParent = dicPatient.Values.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

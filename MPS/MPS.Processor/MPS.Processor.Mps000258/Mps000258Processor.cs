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
using MPS.Processor.Mps000258.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000258
{
    public class Mps000258Processor : AbstractProcessor
    {
        Mps000258PDO rdo;
        List<Mps000258SDO> ListExport = new List<Mps000258SDO>();
        List<Mps000258SDO> ListExportType = new List<Mps000258SDO>();
        List<Mps000258SDO> ListExportBranch = new List<Mps000258SDO>();

        public Mps000258Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000258PDO)rdoBase;
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

                if (ListExportType != null && ListExportType.Count > 0)
                {
                    ListExportType = ListExportType.OrderBy(o => o.SERVICE_TYPE_CODE).ToList();
                }

                if (ListExport != null && ListExport.Count > 0)
                {
                    ListExport = ListExport.OrderBy(o => o.SERVICE_CODE).ToList();
                }

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ExportBranch", ListExportBranch);
                objectTag.AddObjectData(store, "ExportType", ListExportType);
                objectTag.AddObjectData(store, "Export", ListExport);
                objectTag.AddRelationship(store, "ExportBranch", "ExportType", "BRANCH_ID", "BRANCH_ID");
                objectTag.AddRelationship(store, "ExportType", "Export", "SERVICE_TYPE_ID", "SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ExportBranch", "Export", "BRANCH_ID", "BRANCH_ID");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void ProcessorDataExport()
        {
            try
            {
                if (rdo != null && rdo.ListServicePaty != null && rdo.ListServicePaty.Count > 0)
                {
                    ListExport = new List<Mps000258SDO>();
                    ListExportType = new List<Mps000258SDO>();
                    ListExportBranch = new List<Mps000258SDO>();

                    Dictionary<long, int> dicPatientType = new Dictionary<long, int>();
                    if (rdo.ListPatientType != null && rdo.ListPatientType.Count > 0)
                    {
                        //rdo.ListPatientType = rdo.ListPatientType.OrderBy(o => o.ID).ToList();

                        List<long> patientTypeId = rdo.ListServicePaty.Select(s => s.PATIENT_TYPE_ID).Distinct().OrderBy(o => o).ToList();

                        int i = 1;
                        foreach (var item in patientTypeId)
                        {
                            if (i > 15) break;

                            if (rdo.ListPatientType.Exists(o => o.ID == item))
                            {
                                if (!dicPatientType.ContainsKey(item))
                                {
                                    dicPatientType[item] = i;
                                    i++;
                                }
                            }
                        }
                    }

                    var groupBranch = rdo.ListServicePaty.GroupBy(o => o.BRANCH_ID).ToList();
                    foreach (var branch in groupBranch)
                    {
                        Mps000258SDO sdobranch = new Mps000258SDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000258SDO>(sdobranch, branch.First());
                        ListExportBranch.Add(sdobranch);

                        var groupType = branch.GroupBy(o => o.SERVICE_TYPE_ID).ToList();
                        foreach (var types in groupType)
                        {
                            Mps000258SDO sdoType = new Mps000258SDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<Mps000258SDO>(sdoType, types.First());
                            ListExportType.Add(sdoType);

                            var groupServicePaty = types.GroupBy(o => o.SERVICE_ID).ToList();
                            foreach (var servicePatys in groupServicePaty)
                            {
                                Dictionary<int, Mps000258SDO> dicServPaty = new Dictionary<int, Mps000258SDO>();
                                //co nhieu chinh sach gia.
                                var servicePatyGroupByPaty = servicePatys.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                                foreach (var groupByPaty in servicePatyGroupByPaty)
                                {
                                    int i = 1;
                                    foreach (var item in groupByPaty)
                                    {
                                        if (!dicPatientType.ContainsKey(item.PATIENT_TYPE_ID)) continue;

                                        if (!dicServPaty.ContainsKey(i))
                                        {
                                            dicServPaty[i] = new Mps000258SDO();
                                        }

                                        Mps000258SDO sdoService = dicServPaty[i];
                                        if (sdoService.ID <= 0)
                                        {
                                            Inventec.Common.Mapper.DataObjectMapper.Map<Mps000258SDO>(sdoService, item);
                                            V_HIS_SERVICE service = rdo.ListService != null ? rdo.ListService.FirstOrDefault(o => o.ID == item.SERVICE_ID) : new V_HIS_SERVICE();
                                            if (service != null)
                                            {
                                                sdoService.HEIN_SERVICE_BHYT_CODE = service.HEIN_SERVICE_BHYT_CODE;
                                                sdoService.HEIN_SERVICE_BHYT_NAME = service.HEIN_SERVICE_BHYT_NAME;
                                                sdoService.PARENT_ID = service.PARENT_ID;
                                                sdoService.IS_LEAF = service.IS_LEAF;

                                                sdoService.HEIN_LIMIT_PRICE = service.HEIN_LIMIT_PRICE;
                                                sdoService.HEIN_LIMIT_PRICE_OLD = service.HEIN_LIMIT_PRICE_OLD;
                                                sdoService.HEIN_LIMIT_RATIO = service.HEIN_LIMIT_RATIO;
                                                sdoService.HEIN_LIMIT_RATIO_OLD = service.HEIN_LIMIT_RATIO_OLD;
                                                sdoService.HEIN_LIMIT_PRICE_IN_TIME = service.HEIN_LIMIT_PRICE_IN_TIME;
                                                sdoService.HEIN_LIMIT_PRICE_INTR_TIME = service.HEIN_LIMIT_PRICE_INTR_TIME;
                                            }
                                        }

                                        //if (!sdoService.IS_LEAF.HasValue || sdoService.IS_LEAF != 1) continue;

                                        try
                                        {
                                            System.Reflection.PropertyInfo typeName = typeof(Mps000258SDO).GetProperty("PATIENT_TYPE_NAME_" + dicPatientType[item.PATIENT_TYPE_ID]);
                                            typeName.SetValue(sdoService, item.PATIENT_TYPE_NAME);

                                            System.Reflection.PropertyInfo price = typeof(Mps000258SDO).GetProperty("PRICE_" + dicPatientType[item.PATIENT_TYPE_ID]);
                                            price.SetValue(sdoService, item.PRICE);

                                            System.Reflection.PropertyInfo vat = typeof(Mps000258SDO).GetProperty("VAT_RATIO_" + dicPatientType[item.PATIENT_TYPE_ID]);
                                            vat.SetValue(sdoService, item.VAT_RATIO);
                                        }
                                        catch (Exception ex)
                                        {
                                            Inventec.Common.Logging.LogSystem.Error(ex);
                                        }

                                        dicServPaty[i] = sdoService;
                                        i++;
                                    }
                                }

                                if (dicServPaty != null && dicServPaty.Count > 0)
                                {
                                    ListExport.AddRange(dicServPaty.Values);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                if (ListExport != null && ListExport.Count > 0)
                {
                    var patientTypeName1 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_1));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_1, patientTypeName1 != null ? patientTypeName1.PATIENT_TYPE_NAME_1 : ""));

                    var patientTypeName2 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_2));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_2, patientTypeName2 != null ? patientTypeName2.PATIENT_TYPE_NAME_2 : ""));

                    var patientTypeName3 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_3));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_3, patientTypeName3 != null ? patientTypeName3.PATIENT_TYPE_NAME_3 : ""));

                    var patientTypeName4 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_4));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_4, patientTypeName4 != null ? patientTypeName4.PATIENT_TYPE_NAME_4 : ""));

                    var patientTypeName5 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_5));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_5, patientTypeName5 != null ? patientTypeName5.PATIENT_TYPE_NAME_5 : ""));

                    var patientTypeName6 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_6));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_6, patientTypeName6 != null ? patientTypeName6.PATIENT_TYPE_NAME_6 : ""));

                    var patientTypeName7 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_7));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_7, patientTypeName7 != null ? patientTypeName7.PATIENT_TYPE_NAME_7 : ""));

                    var patientTypeName8 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_8));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_8, patientTypeName8 != null ? patientTypeName8.PATIENT_TYPE_NAME_8 : ""));

                    var patientTypeName9 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_9));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_9, patientTypeName9 != null ? patientTypeName9.PATIENT_TYPE_NAME_9 : ""));

                    var patientTypeName10 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_10));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_10, patientTypeName10 != null ? patientTypeName10.PATIENT_TYPE_NAME_10 : ""));

                    var patientTypeName11 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_11));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_11, patientTypeName11 != null ? patientTypeName11.PATIENT_TYPE_NAME_11 : ""));

                    var patientTypeName12 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_12));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_12, patientTypeName12 != null ? patientTypeName12.PATIENT_TYPE_NAME_12 : ""));

                    var patientTypeName13 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_13));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_13, patientTypeName13 != null ? patientTypeName13.PATIENT_TYPE_NAME_13 : ""));

                    var patientTypeName14 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_14));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_14, patientTypeName14 != null ? patientTypeName14.PATIENT_TYPE_NAME_14 : ""));

                    var patientTypeName15 = ListExport.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.PATIENT_TYPE_NAME_15));
                    SetSingleKey(new KeyValue(Mps000528ExtendSingleKey.PATIENT_TYPE_NAME_15, patientTypeName15 != null ? patientTypeName10.PATIENT_TYPE_NAME_15 : ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

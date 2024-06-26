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
using MPS.Processor.Mps000486.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000486
{
     class Mps000486Processor : AbstractProcessor
    {
        Mps000486PDO rdo;
        List<SereServADO> lstAdo = new List<SereServADO>();
        public Mps000486Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000486PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "serviceReq", rdo.listServiceReq.OrderBy(o=>o.INTRUCTION_TIME).ThenBy(o=>o.SERVICE_REQ_CODE).ToList());
                objectTag.AddObjectData(store, "Service", lstAdo);
                objectTag.AddRelationship(store, "serviceReq", "Service", "ID", "SERVICE_REQ_ID");
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        void ProcessSingleKey()
        {
            try
            {
                if(rdo.listServiceReqMety != null && rdo.listServiceReqMety.Count > 0)
                {
                    foreach (var item in rdo.listServiceReqMety)
                    {
                        SereServADO ado = new SereServADO(item);
                        if(ado.MEDI_MATE_TYPE_ID != null)
                        {
                            var checkType = rdo.listVMedicineType.FirstOrDefault(o=>o.ID == ado.MEDI_MATE_TYPE_ID);
                            if(checkType != null)
                            {
                                ado.MEDI_MATE_TYPE_CODE = checkType.MEDICINE_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_NAME = checkType.MEDICINE_TYPE_NAME;
                                ado.ACTIVE_INGR_BHYT_CODE = checkType.ACTIVE_INGR_BHYT_CODE;
                                ado.ACTIVE_INGR_BHYT_NAME = checkType.ACTIVE_INGR_BHYT_NAME;
                                ado.CONCENTRA = checkType.CONCENTRA;
                                ado.MANUFACTURER_CODE = checkType.MANUFACTURER_CODE;
                                ado.MANUFACTURER_NAME = checkType.MANUFACTURER_NAME;
                                ado.HEIN_SERVICE_TYPE_CODE = checkType.HEIN_SERVICE_TYPE_CODE;
                                ado.HEIN_SERVICE_TYPE_NAME = checkType.HEIN_SERVICE_TYPE_NAME;
                                ado.MEDICINE_LINE_CODE = checkType.MEDICINE_LINE_CODE;
                                ado.MEDICINE_LINE_NAME = checkType.MEDICINE_LINE_NAME;
                                ado.HEIN_SERVICE_BHYT_CODE = checkType.HEIN_SERVICE_BHYT_CODE;
                                ado.HEIN_SERVICE_BHYT_NAME = checkType.HEIN_SERVICE_BHYT_NAME;
                                ado.DESCRIPTION = checkType.DESCRIPTION;
                            }    
                        }
                        if (ado.MEDICINE_USE_FORM_ID != null)
                        {
                            var checkType = rdo.listVMedicineType.FirstOrDefault(o => o.ID == ado.MEDICINE_USE_FORM_ID);
                            if (checkType != null)
                            {
                                ado.MEDICINE_USE_FORM_CODE = checkType.MEDICINE_USE_FORM_CODE;
                                ado.MEDICINE_USE_FORM_NAME = checkType.MEDICINE_USE_FORM_NAME;
                            }
                        }
                        lstAdo.Add(ado);
                    }
                }
                if (rdo.listServiceReqMaty != null && rdo.listServiceReqMaty.Count > 0)
                {
                    foreach (var item in rdo.listServiceReqMaty)
                    {
                        SereServADO ado = new SereServADO(item);
                        if (ado.MEDI_MATE_TYPE_ID != null)
                        {
                            var checkType = rdo.listVMaterialType.FirstOrDefault(o => o.ID == ado.MEDI_MATE_TYPE_ID);
                            if (checkType != null)
                            {
                                ado.MEDI_MATE_TYPE_CODE = checkType.MATERIAL_TYPE_MAP_CODE;
                                ado.MEDI_MATE_TYPE_NAME = checkType.MATERIAL_TYPE_MAP_NAME;
                                ado.CONCENTRA = checkType.CONCENTRA;
                                ado.MANUFACTURER_CODE = checkType.MANUFACTURER_CODE;
                                ado.MANUFACTURER_NAME = checkType.MANUFACTURER_NAME;
                                ado.HEIN_SERVICE_TYPE_CODE = checkType.HEIN_SERVICE_TYPE_CODE;
                                ado.HEIN_SERVICE_TYPE_NAME = checkType.HEIN_SERVICE_TYPE_NAME;
                                ado.HEIN_SERVICE_BHYT_CODE = checkType.HEIN_SERVICE_BHYT_CODE;
                                ado.HEIN_SERVICE_BHYT_NAME = checkType.HEIN_SERVICE_BHYT_NAME;
                                ado.DESCRIPTION = checkType.DESCRIPTION;
                            }
                        }
                        lstAdo.Add(ado);
                    }
                }
                if (rdo.listVSereServ2 != null && rdo.listVSereServ2.Count > 0)
                {
                    foreach (var item in rdo.listVSereServ2)
                    {
                        SereServADO ado = new SereServADO(item);
                        lstAdo.Add(ado);
                    }
                }
                AddObjectKeyIntoListkey<Mps000486ADO>(rdo.ado486, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

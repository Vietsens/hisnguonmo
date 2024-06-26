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
using MPS.Processor.Mps000471.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000471
{
    public partial class Mps000471Processor : AbstractProcessor
    {
        List<V_HIS_SERE_SERV> lstSereServ = new List<V_HIS_SERE_SERV>();
        List<V_HIS_SERVICE_REQ> lstServiceReq = new List<V_HIS_SERVICE_REQ>();
        List<HIS_SERE_SERV_EXT> lstSereServExt = new List<HIS_SERE_SERV_EXT>();
        List<SereServADO> lstSereServADO = new List<SereServADO>();
        Mps000471PDO rdo;
        public Mps000471Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000471PDO)rdoBase;
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
                SetSingleKey();
                ProcessDataNew();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("ListTreatment_____", lstServiceReq));
                objectTag.AddObjectData(store, "ListTreatment", lstServiceReq);
                objectTag.AddObjectData(store, "ListServiceReq", rdo.serviceReq);
                objectTag.AddObjectData(store, "ListService", lstSereServADO);
                objectTag.AddRelationship(store, "ListTreatment", "ListServiceReq", "TREATMENT_ID", "TREATMENT_ID");
                objectTag.AddRelationship(store, "ListServiceReq", "ListService", "ID", "SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ListTreatment", "ListService", "TREATMENT_ID", "TDL_TREATMENT_ID");
                singleTag.ProcessData(store, singleValueDictionary);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessDataNew()
        {
            try
            {
                if (rdo.serviceReq != null && rdo.serviceReq.Count() > 0)
                {
                    var ssServiceReqs = rdo.serviceReq.GroupBy(o => o.TREATMENT_ID);
                    if (ssServiceReqs != null && ssServiceReqs.Count() > 0)
                    {
                        foreach (var item in ssServiceReqs)
                        {
                            lstServiceReq.Add(item.FirstOrDefault());
                        }

                    }

                    if (rdo.sereServ != null && rdo.sereServ.Count() > 0)
                    {
                        foreach (var item in rdo.sereServ)
                        {
                            var sereServExts = rdo.sereServExt.Where(o => o.SERE_SERV_ID == item.ID);
                            var serviceReqs = lstServiceReq.Where(o => o.ID == item.SERVICE_REQ_ID);
                            SereServADO ado = new SereServADO();
                            if (sereServExts != null && sereServExts.Count() > 0)
                                ado = new SereServADO(item, sereServExts.First(), serviceReqs.First());
                            else
                                ado = new SereServADO(item, serviceReqs.First());
                            lstSereServADO.Add(ado);
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

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000037.PDO
{
    public partial class Mps000037PDO : RDOBase
    {
        public Mps000037PDO(
            List<V_HIS_SERE_SERV> sereServs_All,
            V_HIS_SERVICE_REQ lstServiceReq,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            HIS_TREATMENT hisTreatment,
            Mps000037ADO mps000037ADO,
            List<V_HIS_SERVICE_REQ> listServiceReqPrint,
            List<HIS_SERE_SERV_EXT> hisSereServExt,
            List<V_HIS_SERVICE> listService)
        {
            try
            {
                this.SereServs_All = sereServs_All;
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.lstServiceReq = lstServiceReq;
                this.currentHisTreatment = hisTreatment;
                this.Mps000037ADO = mps000037ADO;
                this.ListServiceReqPrint = listServiceReqPrint;
                this.SereServExt = hisSereServExt;
                this.ListService = listService;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000037PDO(
    List<V_HIS_SERE_SERV> sereServs_All,
    V_HIS_SERVICE_REQ lstServiceReq,
    V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
    HIS_TREATMENT hisTreatment,
    Mps000037ADO mps000037ADO,
    List<V_HIS_SERVICE_REQ> listServiceReqPrint,
    List<HIS_SERE_SERV_EXT> hisSereServExt,
    List<V_HIS_SERVICE> listService,
    List<HIS_SERVICE_REQ_TYPE> lstServiceReqType) : this(sereServs_All, lstServiceReq, V_HIS_PATIENT_TYPE_ALTER, hisTreatment, mps000037ADO, listServiceReqPrint, hisSereServExt, listService)
        {
            try
            {
                this.ListServiceReqType = lstServiceReqType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

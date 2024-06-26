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

namespace MPS.Processor.Mps000275.PDO
{
    public partial class Mps000275PDO : RDOBase
    {
        public Mps000275PDO() { }

        public Mps000275PDO(
            List<V_HIS_SERVICE_REQ> _vserviceReqs,
            List<V_HIS_SERE_SERV> _vsereServs,
            List<HIS_SERE_SERV_RATION> _sereServRations,
            List<HIS_SERE_SERV_EXT> _sereServExts)
        {
            try
            {
                this._vServiceReqs = _vserviceReqs;
                this._vSereServs = _vsereServs;
                this._SereServRations = _sereServRations;
                this._SereServExts = _sereServExts;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000275PDO(
            List<V_HIS_SERVICE_REQ> _vserviceReqs,
            List<V_HIS_SERE_SERV> _vsereServs,
            List<HIS_SERE_SERV_RATION> _sereServRations,
            List<HIS_SERE_SERV_EXT> _sereServExts,
            List<HIS_PATIENT_TYPE> _listPatientType)
        {
            try
            {
                this._vServiceReqs = _vserviceReqs;
                this._vSereServs = _vsereServs;
                this._SereServRations = _sereServRations;
                this._SereServExts = _sereServExts;
                this._ListPatientType = _listPatientType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000275PDO(
            V_HIS_SERVICE_REQ _vserviceReq,
            List<V_HIS_SERE_SERV> _vsereServs)
        {
            try
            {
                this._vServiceReqs = new List<V_HIS_SERVICE_REQ>();
                this._vServiceReqs.Add(_vserviceReq);
                this._vSereServs = _vsereServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

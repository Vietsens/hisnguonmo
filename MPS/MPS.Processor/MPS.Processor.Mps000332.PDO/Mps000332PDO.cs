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
using SAR.EFMODEL.DataModels;

namespace MPS.Processor.Mps000332.PDO
{
    public partial class Mps000332PDO : RDOBase
    {

        public HIS_TREATMENT Treatment { get; set; }
        public HIS_SERVICE_REQ ServiceReq { get; set; }
        public V_HIS_SERE_SERV SereServ { get; set; }
        public HIS_SERE_SERV_EXT SereServExt { get; set; }
        public V_HIS_SERE_SERV_PTTT SereServPttt { get; set; }
        public List<SAR_FORM_DATA> _SarFormDatas { get; set; }
        public string RequestDepartmentName { get; set; }
        public Mps000332PDO() { }
        public Mps000332PDO(
           HIS_TREATMENT _treatment,
           HIS_SERVICE_REQ _serviceReq,
           V_HIS_SERE_SERV _sereServ,
            HIS_SERE_SERV_EXT _sereServExt,
            V_HIS_SERE_SERV_PTTT _sereServPttt,
            List<SAR_FORM_DATA> _sarFormDatas)
        {
            try
            {
                this.Treatment = _treatment;
                this.ServiceReq = _serviceReq;
                this.SereServ = _sereServ;
                this.SereServExt = _sereServExt;
                this.SereServPttt = _sereServPttt;
                this._SarFormDatas = _sarFormDatas;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

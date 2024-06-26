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

namespace MPS.Processor.Mps000015.PDO
{
    public class Mps000015PDO : RDOBase
    {
        public V_HIS_PATIENT_TYPE_ALTER _PatyAlterBhyt { get; set; }
        public HIS_SERVICE_REQ _ServiceReq { get; set; }
        public List<Mps000015ADO> _SereServs { get; set; }
        public SingleKeys _SingleKeys { get; set; }

        public Mps000015PDO(
            V_HIS_PATIENT_TYPE_ALTER _HisPatyAlterBhyt,
            HIS_SERVICE_REQ _HisServiceReq,
            List<Mps000015ADO> _HisSereServs,
            SingleKeys _singleKeys
            )
        {
            try
            {
                this._PatyAlterBhyt = _HisPatyAlterBhyt;
                this._ServiceReq = _HisServiceReq;
                this._SereServs = _HisSereServs;
                this._SingleKeys = _singleKeys;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class Mps000015ADO : HIS_SERE_SERV
    {
        public string DESCRIPTION { get; set; }
        public string CONCLUDE { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }

        public Mps000015ADO() { }

        public Mps000015ADO(HIS_SERE_SERV data, string str1, string str2)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<Mps000015ADO>(this, data);
                    this.SERVICE_NAME = data.TDL_SERVICE_NAME;
                    this.SERVICE_CODE = data.TDL_SERVICE_CODE;
                }
                this.DESCRIPTION = str1;
                this.CONCLUDE = str2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class SingleKeys
    {
        public string BED_NAME { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public decimal Ratio { get; set; }

    }
}

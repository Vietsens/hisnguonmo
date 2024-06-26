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

namespace MPS.Processor.Mps000116.PDO
{
    public partial class Mps000116PDO : RDOBase
    {
        public MOS.SDO.WorkPlaceSDO _WorkPlace { get; set; }
        public List<Mps000116ADO> _Mps000116ADOs { get; set; }
        public long _IntructionTime { get; set; }
        public V_HIS_BED_LOG _vHisBedLog { get; set; }

        public Mps000116PDO() { }

        public Mps000116PDO(
            V_HIS_TREATMENT currentTreatment,
            List<Mps000116ADO> _mps000116ADOs,
            MOS.SDO.WorkPlaceSDO _workPlace,
            V_HIS_BED_LOG _vhisBedLog,
            long _intructionTime,
            SingleKeys _singleKeys
            )
        {
            try
            {
                this._Treatment = currentTreatment;
                this._Mps000116ADOs = _mps000116ADOs;
                this._WorkPlace = _workPlace;
                this._vHisBedLog = _vhisBedLog;
                this._IntructionTime = _intructionTime;
                this._SingleKeys = _singleKeys;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

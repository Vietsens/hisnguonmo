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

namespace MPS.Processor.Mps000497.PDO
{
    public class Mps000497PDO : RDOBase
    {
        public List<HIS_RATION_SUM> _hisRationSums { get; set; }
        public List<V_HIS_SERE_SERV_RATION> _hisSereServRation { get; set; }
        public List<HIS_RATION_GROUP> _hisRationGroup { get; set; }
        public List<HIS_DEPARTMENT> _hisDepartment { get; set; }

        public Mps000497PDO(List<HIS_RATION_SUM> hisRationSums, List<V_HIS_SERE_SERV_RATION> hisSereServRation, List<HIS_RATION_GROUP> hisRationGroup, List<HIS_DEPARTMENT> hisDepartment)
        {
            try
            {
                this._hisRationSums = hisRationSums;
                this._hisSereServRation = hisSereServRation;
                this._hisRationGroup = hisRationGroup;
                this._hisDepartment = hisDepartment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

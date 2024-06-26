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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using MOS.SDO;
namespace MPS.Processor.Mps000453.PDO
{
    public partial class Mps000453PDO : RDOBase
    {
        public HIS_KSK_UNDER_EIGHTEEN HisKskUnderEighteen { get; set; }
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public HIS_DHST HisDhst { get; set; }
        public List<HIS_VACCINE_TYPE> lstVaccineType { get; set; }
        public List<HIS_KSK_UNEI_VATY> lstUneiVaty { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> examRank { get; set; }
        public Mps000453PDO(
            HIS_KSK_UNDER_EIGHTEEN HisKskUnderEighteen,
           V_HIS_SERVICE_REQ HisServiceReq,
           HIS_DHST HisDhst,
            List<HIS_VACCINE_TYPE> lstVaccineType,
            List<HIS_KSK_UNEI_VATY> lstUneiVaty,
            List<HIS_HEALTH_EXAM_RANK> examRank
            )
        {
            try
            {
                this.HisKskUnderEighteen = HisKskUnderEighteen;
                this.HisServiceReq = HisServiceReq;
                this.HisDhst = HisDhst;
                this.lstVaccineType = lstVaccineType;
                this.lstUneiVaty = lstUneiVaty;
                this.examRank = examRank;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

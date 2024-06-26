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

namespace MPS.Processor.Mps000316.PDO
{
    public class PrescriptionADO
    {
        public List<HIS_SERVICE_REQ> HisServiceReq { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> VHisExpMestMedicine { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> VHisExpMestMaterial { get; set; }
        public List<HIS_SERVICE_REQ_METY> VHisServiceReqMety { get; set; }
        public List<HIS_SERVICE_REQ_MATY> VHisServiceReqMaty { get; set; }

        public PrescriptionADO() { }

        public PrescriptionADO(PrescriptionADO data) 
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<PrescriptionADO>(this, data);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }
    }
}

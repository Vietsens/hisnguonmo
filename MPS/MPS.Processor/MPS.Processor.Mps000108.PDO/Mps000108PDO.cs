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

namespace MPS.Processor.Mps000108.PDO
{
    public partial class Mps000108PDO : RDOBase
    {
        public const string printTypeCode = "Mps000108";

        public Mps000108PDO(HIS_EXP_MEST expMest, List<V_HIS_EXP_MEST_BLTY_REQ_1> hisExpMestBltys, V_HIS_TREATMENT treatment, V_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                this.ExpMest = expMest;
                this.HisExpMestBltys = hisExpMestBltys;
                this.Treatment = treatment;
                this.ServiceReq = serviceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000108PDO(HIS_EXP_MEST expMest, List<V_HIS_EXP_MEST_BLTY_REQ_1> hisExpMestBltys, V_HIS_TREATMENT treatment, V_HIS_SERVICE_REQ serviceReq,List<V_HIS_TREATMENT_BED_ROOM> _lstTreatmentBedRoom,List<V_HIS_SERE_SERV_1> _lstSereServ1)
        {
            try
            {
                this.ExpMest = expMest;
                this.HisExpMestBltys = hisExpMestBltys;
                this.Treatment = treatment;
                this.ServiceReq = serviceReq;
                this.treatmentBedRooms = _lstTreatmentBedRoom;
                this.sereServ1s = _lstSereServ1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000108PDO(HIS_EXP_MEST expMest, List<V_HIS_EXP_MEST_BLTY_REQ_1> hisExpMestBltys, V_HIS_TREATMENT treatment, V_HIS_SERVICE_REQ serviceReq, List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloodList)
        {
            try
            {
                this.ExpMest = expMest;
                this.HisExpMestBltys = hisExpMestBltys;
                this.Treatment = treatment;
                this.ServiceReq = serviceReq;
                this.ExpMestBloodList = _ExpMestBloodList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000108PDO(HIS_EXP_MEST expMest, List<V_HIS_EXP_MEST_BLTY_REQ_1> hisExpMestBltys, V_HIS_TREATMENT treatment, V_HIS_SERVICE_REQ serviceReq, List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloodList, List<V_HIS_TREATMENT_BED_ROOM> _lstTreatmentBedRoom, List<V_HIS_SERE_SERV_1> _lstSereServ1)
        {
            try
            {
                this.ExpMest = expMest;
                this.HisExpMestBltys = hisExpMestBltys;
                this.Treatment = treatment;
                this.ServiceReq = serviceReq;
                this.ExpMestBloodList = _ExpMestBloodList;
                this.treatmentBedRooms = _lstTreatmentBedRoom;
                this.sereServ1s = _lstSereServ1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

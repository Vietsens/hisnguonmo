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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestCreate.ADO
{
    class ResultImpMestADO
    {
        public HisImpMestManuSDO HisManuSDO { get; set; }
        public HisImpMestInitSDO HisInitSDO { get; set; }
        public HisImpMestInveSDO HisInveSDO { get; set; }
        public HisImpMestOtherSDO HisOtherSDO { get; set; }

        public List<HisMedicineWithPatySDO> HisMedicineSDOs { get; set; }
        public List<HisMaterialWithPatySDO> HisMaterialSDOs { get; set; }

        public long ImpMestTypeId { get; set; }
        public long ImpMestSttId { get; set; }
        public HIS_IMP_MEST ImpMestUpdate { get; set; }

        public ResultImpMestADO()
        {

        }

        public ResultImpMestADO(HisImpMestManuSDO manuSDO)
        {
            this.HisManuSDO = manuSDO;
            this.HisMaterialSDOs = manuSDO.ManuMaterials;
            this.HisMedicineSDOs = manuSDO.ManuMedicines;
            this.ImpMestTypeId = manuSDO.ImpMest.IMP_MEST_TYPE_ID;
            this.ImpMestSttId = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
            this.ImpMestUpdate = manuSDO.ImpMest;
        }

        public ResultImpMestADO(HisImpMestInitSDO initSDO)
        {
            this.HisInitSDO = initSDO;
            this.HisMaterialSDOs = initSDO.InitMaterials;
            this.HisMedicineSDOs = initSDO.InitMedicines;
            this.ImpMestTypeId = initSDO.ImpMest.IMP_MEST_TYPE_ID;
            this.ImpMestSttId = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
            this.ImpMestUpdate = initSDO.ImpMest;
        }

        public ResultImpMestADO(HisImpMestInveSDO inveSDO)
        {
            this.HisInveSDO = inveSDO;
            this.HisMaterialSDOs = inveSDO.InveMaterials;
            this.HisMedicineSDOs = inveSDO.InveMedicines;
            this.ImpMestTypeId = inveSDO.ImpMest.IMP_MEST_TYPE_ID;
            this.ImpMestSttId = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
            this.ImpMestUpdate = inveSDO.ImpMest;
        }

        public ResultImpMestADO(HisImpMestOtherSDO otherSDO)
        {
            this.HisOtherSDO = otherSDO;
            this.HisMaterialSDOs = otherSDO.OtherMaterials;
            this.HisMedicineSDOs = otherSDO.OtherMedicines;
            this.ImpMestTypeId = otherSDO.ImpMest.IMP_MEST_TYPE_ID;
            this.ImpMestSttId = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
            this.ImpMestUpdate = otherSDO.ImpMest;
        }
    }
}

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

namespace MPS.Processor.Mps000199.PDO
{
    public partial class Mps000199PDO : RDOBase
    {
        public const string printTypeCode = "Mps000199";

        public Mps000199PDO() { }

        public Mps000199PDO(
            V_HIS_IMP_MEST ImpMest,
            List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines,
            List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials,
            List<V_HIS_IMP_MEST_BLOOD> impMestBloods,
            List<V_HIS_IMP_MEST_USER> listIpmMestUser
            )
        {
            this._ImpMestMaterials = impMestMaterials;
            this._ImpMestMedicines = impMestMedicines;
            this._ListIpmMestUser = listIpmMestUser;
            this._ImpMestBloods = impMestBloods;

            if (ImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
            {
                this._initImpMest = (V_HIS_IMP_MEST)ImpMest;
            }
            else if (ImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
            {
                this._inveImpMest = (V_HIS_IMP_MEST)ImpMest;
            }
            else if (ImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
            {
                this._otherImpMest = (V_HIS_IMP_MEST)ImpMest;
            }
        }

        public Mps000199PDO(
            V_HIS_IMP_MEST ImpMest,
            List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines,
            List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials,
            List<V_HIS_IMP_MEST_BLOOD> impMestBloods,
            List<V_HIS_IMP_MEST_USER> listIpmMestUser,
            List<MedicalContractADO> listMedicalContract
            )
        {
            this._ImpMestMaterials = impMestMaterials;
            this._ImpMestMedicines = impMestMedicines;
            this._ListIpmMestUser = listIpmMestUser;
            this._ImpMestBloods = impMestBloods;

            if (ImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
            {
                this._initImpMest = (V_HIS_IMP_MEST)ImpMest;
            }
            else if (ImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
            {
                this._inveImpMest = (V_HIS_IMP_MEST)ImpMest;
            }
            else if (ImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
            {
                this._otherImpMest = (V_HIS_IMP_MEST)ImpMest;
            }
            this._ListMedicalContract = listMedicalContract;
        }
    }
}

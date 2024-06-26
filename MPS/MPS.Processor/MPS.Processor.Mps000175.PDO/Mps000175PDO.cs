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

namespace MPS.Processor.Mps000175.PDO
{
    public partial class Mps000175PDO : RDOBase
    {
        public List<Mps000175ADO> listAdo = new List<Mps000175ADO>();
        public List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        public List<HIS_EXP_MEST> _ExpMests_Print { get; set; }
        public keyTitles keyName;
        public Mps000175Config ConfigMps175 { get; set; }
        public List<V_HIS_MATERIAL_TYPE> _MaterialTypes { get; set; }
        public List<V_HIS_EXP_MEST> ListAggrExpMest { get; set; }

        public Mps000175PDO() { }

        public Mps000175PDO(
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            V_HIS_EXP_MEST aggrExpMest,
            List<HIS_EXP_MEST> _expMests_Print,
            HIS_DEPARTMENT department,
            List<long> serviceUnitIds,
            List<long> roomIds,
            keyTitles _key,
            Mps000175Config _configMps175,
            List<V_HIS_MATERIAL_TYPE> materialTypes
            )
        {
            try
            {
                this._ExpMestMaterials = _expMestMaterials;
                this.AggrExpMest = aggrExpMest;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this.ServiceUnitIds = serviceUnitIds;
                this.RoomIds = roomIds;
                this.keyName = _key;
                this.ConfigMps175 = _configMps175;
                if (ConfigMps175 == null) ConfigMps175 = new Mps000175Config();
                this._MaterialTypes = materialTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000175PDO(
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            V_HIS_EXP_MEST aggrExpMest,
            List<HIS_EXP_MEST> _expMests_Print,
            HIS_DEPARTMENT department,
            List<long> serviceUnitIds,
            List<long> roomIds,
            keyTitles _key,
            Mps000175Config _configMps175,
            List<V_HIS_MATERIAL_TYPE> materialTypes,
            List<V_HIS_EXP_MEST> _listAggrExpMest
            )
        {
            try
            {
                this._ExpMestMaterials = _expMestMaterials;
                this.AggrExpMest = aggrExpMest;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this.ServiceUnitIds = serviceUnitIds;
                this.RoomIds = roomIds;
                this.keyName = _key;
                this.ConfigMps175 = _configMps175;
                if (ConfigMps175 == null) ConfigMps175 = new Mps000175Config();
                this._MaterialTypes = materialTypes;
                this.ListAggrExpMest = _listAggrExpMest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

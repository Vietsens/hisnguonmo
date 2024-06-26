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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000325.PDO
{
    public partial class Mps000325PDO : RDOBase
    {
        public List<Mps000325ADO> listAdo = new List<Mps000325ADO>();
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        public List<HIS_EXP_MEST> _ExpMests_Print { get; set; }
        public Mps000325Config ConfigMps325 { get; set; }
        public keyTitles keyName;
        public Mps000325PDO() { }

        public Mps000325PDO(
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            V_HIS_EXP_MEST aggrExpMest,
            List<HIS_EXP_MEST> _expMests_Print,
            HIS_DEPARTMENT department,
            List<long> serviceUnitIds,
            List<long> useFormIds,
            List<long> roomIds,
            List<V_HIS_MEDICINE_TYPE> vHisMedicineTypes,
            Mps000325Config _configMps325
            )
        {
            try
            {
                this._ExpMestMedicines = _expMestMedicines;
                this.AggrExpMest = aggrExpMest;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this.ServiceUnitIds = serviceUnitIds;
                this.UseFormIds = useFormIds;
                this.RoomIds = roomIds;
                this._MedicineTypes = vHisMedicineTypes;
                this.ConfigMps325 = _configMps325;
                if (ConfigMps325 == null) ConfigMps325 = new Mps000325Config();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000325PDO(
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            V_HIS_EXP_MEST aggrExpMest,
            List<HIS_EXP_MEST> _expMests_Print,
            HIS_DEPARTMENT department,
            List<long> serviceUnitIds,
            List<long> useFormIds,
            List<long> roomIds,
            List<V_HIS_MEDICINE_TYPE> vHisMedicineTypes,
             keyTitles keyTitles,
            Mps000325Config _configMps325
            )
        {
            try
            {
                this._ExpMestMedicines = _expMestMedicines;
                this.AggrExpMest = aggrExpMest;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this.ServiceUnitIds = serviceUnitIds;
                this.UseFormIds = useFormIds;
                this.RoomIds = roomIds;
                this._MedicineTypes = vHisMedicineTypes;
                this.ConfigMps325 = _configMps325;
                if (ConfigMps325 == null) ConfigMps325 = new Mps000325Config();
                this.keyName = keyTitles;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

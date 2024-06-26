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
using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MPS.ProcessorBase;
using MPS.Processor.Mps000046.PDO;

namespace MPS.Processor.Mps000046.PDO
{
    public partial class Mps000046PDO : RDOBase
    {
        public List<Mps000046ADO> listAdo = new List<Mps000046ADO>();
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        public List<HIS_EXP_MEST> _ExpMests_Print { get; set; }
        public keyTitles keyName;
        public long _ConfigKeyMERGER_DATA { get; set; }
        public long PatientTypeId__BHYT { get; set; }
        public long _ConfigKeyOderOption { get; set; }

        public Mps000046PDO() { }

        public Mps000046PDO(
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            V_HIS_EXP_MEST aggrExpMest,
            List<HIS_EXP_MEST> _expMests_Print,
            HIS_DEPARTMENT department,
            List<long> serviceUnitIds,
            List<long> useFormIds,
            List<long> roomIds,
            bool isMedicine,
            bool ismaterial,
            bool isChemicalSustance,
            long HisExpMestSttId__Approved,
            long HisExpMestSttId__Exported,
            List<V_HIS_MEDICINE_TYPE> vHisMedicineTypes,
            keyTitles _key,
            long _configKeyMERGER_DATA,
            long patientTypeId__BHYT,
            long ConfigKeyOderOption
            )
        {
            try
            {
                this._ExpMestMedicines = _expMestMedicines;
                this._ExpMestMaterials = _expMestMaterials;
                this.AggrExpMest = aggrExpMest;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this.ServiceUnitIds = serviceUnitIds;
                this.UseFormIds = useFormIds;
                this.RoomIds = roomIds;
                this.IsMedicine = isMedicine;
                this.Ismaterial = ismaterial;
                this.IsChemicalSustance = isChemicalSustance;
                this._ExpMestSttId__Approved = HisExpMestSttId__Approved;
                this._ExpMestSttId__Exported = HisExpMestSttId__Exported;
                this._MedicineTypes = vHisMedicineTypes;
                this.keyName = _key;
                this._ConfigKeyMERGER_DATA = _configKeyMERGER_DATA;
                this.PatientTypeId__BHYT = patientTypeId__BHYT;
                this._ConfigKeyOderOption = ConfigKeyOderOption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

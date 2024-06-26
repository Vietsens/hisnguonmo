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

namespace MPS.Processor.Mps000078.PDO
{
    public partial class Mps000078PDO : RDOBase
    {
        public long HisImpMestSttId__Imported;
        public long HisImpMestSttId__Approved;
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes;
        public List<HIS_IMP_MEST> _ImpMests_Print { get; set; }
        public List<HIS_EXP_MEST> _MobaExpMests { get; set; }

        public Mps000078PDO() { }

        public Mps000078PDO(
            List<V_HIS_IMP_MEST_MEDICINE> _impMestMedicines,
            List<V_HIS_IMP_MEST_MATERIAL> _impMestMaterials,
            V_HIS_IMP_MEST aggrImpMest,
            HIS_DEPARTMENT department,
            List<long> serviceUnitIds,
            List<long> useFormIds,
            List<long> roomIds,
            bool isMedicine,
            bool ismaterial,
            long HisImpMestSttId__Imported,
            long HisImpMestSttId__Approved,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes,
            IsTittle keyNameTitles,
            List<HIS_IMP_MEST> impMests_Print
            )
        {
            try
            {
                this._ImpMestMedicines = _impMestMedicines;
                this._ImpMestMaterials = _impMestMaterials;
                this.AggrImpMest = aggrImpMest;
                this.Department = department;
                this.ServiceUnitIds = serviceUnitIds;
                this.UseFormIds = useFormIds;
                this.RoomIds = roomIds;
                this.IsMedicine = isMedicine;
                this.Ismaterial = ismaterial;
                this.HisImpMestSttId__Imported = HisImpMestSttId__Imported;
                this.HisImpMestSttId__Approved = HisImpMestSttId__Approved;
                this._MedicineTypes = _medicineTypes;
                this.keyNameTitles = keyNameTitles;
                this._ImpMests_Print = impMests_Print;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000078PDO(
            List<V_HIS_IMP_MEST_MEDICINE> _impMestMedicines,
            List<V_HIS_IMP_MEST_MATERIAL> _impMestMaterials,
            V_HIS_IMP_MEST aggrImpMest,
            HIS_DEPARTMENT department,
            List<long> serviceUnitIds,
            List<long> useFormIds,
            List<long> roomIds,
            bool isMedicine,
            bool ismaterial,
            long HisImpMestSttId__Imported,
            long HisImpMestSttId__Approved,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes,
            IsTittle keyNameTitles,
            List<HIS_IMP_MEST> impMests_Print,
            List<HIS_EXP_MEST> mobaExpMests
            )
        {
            try
            {
                this._ImpMestMedicines = _impMestMedicines;
                this._ImpMestMaterials = _impMestMaterials;
                this.AggrImpMest = aggrImpMest;
                this.Department = department;
                this.ServiceUnitIds = serviceUnitIds;
                this.UseFormIds = useFormIds;
                this.RoomIds = roomIds;
                this.IsMedicine = isMedicine;
                this.Ismaterial = ismaterial;
                this.HisImpMestSttId__Imported = HisImpMestSttId__Imported;
                this.HisImpMestSttId__Approved = HisImpMestSttId__Approved;
                this._MedicineTypes = _medicineTypes;
                this.keyNameTitles = keyNameTitles;
                this._ImpMests_Print = impMests_Print;
                this._MobaExpMests = mobaExpMests;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public enum IsTittle
    {
        TongHop,
        ThuocThuong,
        VatTu,
        Corticoid,
        KhangSinh,
        Lao,
        DichTruyen,
        TienChat
    }
}

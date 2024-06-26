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

namespace MPS.Processor.Mps000048.PDO
{
    public partial class Mps000048PDO : RDOBase
    {
        public List<V_HIS_MEDI_STOCK> _MediStocks { get; set; }
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_MATERIAL_TYPE> _MaterialTypes { get; set; }
        public List<V_HIS_BLOOD_TYPE> _BloodTypes { get; set; }
        public List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs { get; set; }
        public List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs { get; set; }
        public List<HIS_EXP_MEST_BLTY_REQ> _ExpMestBltyReqs { get; set; }
        public long keyMert = 1;
        public long _keyPhieuTra = 0;

        public Mps000048PDO() { }

        public Mps000048PDO(
            V_HIS_EXP_MEST chmsExpMest,
            List<V_HIS_EXP_MEST_MEDICINE> listMedicine,
            List<V_HIS_EXP_MEST_MATERIAL> listMaterial,
            List<V_HIS_EXP_MEST_BLOOD> listBlood,
            List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs,
            List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs,
            List<HIS_EXP_MEST_BLTY_REQ> _expMestBltyReqs,
            string _Req_Department_name,
            string _Req_Room_Name,
            string _Exp_Department_Name,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export,
            List<V_HIS_MEDI_STOCK> listMediStock,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes,
            List<V_HIS_MATERIAL_TYPE> _materialTypes,
            List<V_HIS_BLOOD_TYPE> _bloodTypes,
            string keyNames,
            long keyMert,
            long keyPhieuTra
            )
        {
            this._ChmsExpMest = chmsExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this._Bloods = listBlood;
            this._ExpMestMetyReqs = _expMestMetyReqs;
            this._ExpMestMatyReqs = _expMestMatyReqs;
            this._ExpMestBltyReqs = _expMestBltyReqs;
            this.Req_Department_Name = _Req_Department_name;
            this.Req_Room_Name = _Req_Room_Name;
            this.Exp_Department_Name = _Exp_Department_Name;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
            this._MediStocks = listMediStock;
            this._MedicineTypes = _medicineTypes;
            this._MaterialTypes = _materialTypes;
            this._BloodTypes = _bloodTypes;
            this.KeyNames = keyNames;
            this.keyMert = keyMert;
            this._keyPhieuTra = keyPhieuTra;
        }
    }
}

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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000215.PDO
{
    public partial class Mps000215PDO : RDOBase
    {
        public List<V_HIS_MEDI_STOCK> _MediStocks { get; set; }
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_MATERIAL_TYPE> _MaterialTypes { get; set; }
        public List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs { get; set; }
        public List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs { get; set; }
        public keyTitles _keyTitles { get; set; }
        public V_HIS_EXP_MEST _BcsExpMest;
        public List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;
        public List<V_HIS_EXP_MEST_MATERIAL> _Materials = null;
        public List<HIS_TREATMENT> ListTreatment { get; set; }
        public HisExpMestBcsMoreInfoSDO _BcsMoreInfoSDO { get; set; }
        public long OderOptionKey;
        public Mps000215PDO() { }

        public Mps000215PDO(
            V_HIS_EXP_MEST chmsExpMest,
            List<V_HIS_EXP_MEST_MEDICINE> listMedicine,
            List<V_HIS_EXP_MEST_MATERIAL> listMaterial,
            List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs,
            List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs,
            List<V_HIS_MEDI_STOCK> listMediStock,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes,
            List<V_HIS_MATERIAL_TYPE> _materialTypes,
            keyTitles _KeyTitles
            )
        {
            this._BcsExpMest = chmsExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this._ExpMestMetyReqs = _expMestMetyReqs;
            this._ExpMestMatyReqs = _expMestMatyReqs;
            this._MediStocks = listMediStock;
            this._MedicineTypes = _medicineTypes;
            this._MaterialTypes = _materialTypes;
            this._keyTitles = _KeyTitles;
        }

        public Mps000215PDO(
            V_HIS_EXP_MEST chmsExpMest,
            List<V_HIS_EXP_MEST_MEDICINE> listMedicine,
            List<V_HIS_EXP_MEST_MATERIAL> listMaterial,
            List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs,
            List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs,
            List<V_HIS_MEDI_STOCK> listMediStock,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes,
            List<V_HIS_MATERIAL_TYPE> _materialTypes,
            keyTitles _KeyTitles,
            List<HIS_TREATMENT> _ListTreatment
            )
        {
            this._BcsExpMest = chmsExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this._ExpMestMetyReqs = _expMestMetyReqs;
            this._ExpMestMatyReqs = _expMestMatyReqs;
            this._MediStocks = listMediStock;
            this._MedicineTypes = _medicineTypes;
            this._MaterialTypes = _materialTypes;
            this._keyTitles = _KeyTitles;
            this.ListTreatment = _ListTreatment;
        }

        public Mps000215PDO(
            V_HIS_EXP_MEST chmsExpMest,
            List<V_HIS_EXP_MEST_MEDICINE> listMedicine,
            List<V_HIS_EXP_MEST_MATERIAL> listMaterial,
            List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs,
            List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs,
            List<V_HIS_MEDI_STOCK> listMediStock,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes,
            List<V_HIS_MATERIAL_TYPE> _materialTypes,
            keyTitles _KeyTitles,
            List<HIS_TREATMENT> _ListTreatment,
            long _OderOptionKey
            )
        {
            this._BcsExpMest = chmsExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this._ExpMestMetyReqs = _expMestMetyReqs;
            this._ExpMestMatyReqs = _expMestMatyReqs;
            this._MediStocks = listMediStock;
            this._MedicineTypes = _medicineTypes;
            this._MaterialTypes = _materialTypes;
            this._keyTitles = _KeyTitles;
            this.ListTreatment = _ListTreatment;
            this.OderOptionKey = _OderOptionKey;
        }

        public Mps000215PDO(
            V_HIS_EXP_MEST chmsExpMest,
            List<V_HIS_EXP_MEST_MEDICINE> listMedicine,
            List<V_HIS_EXP_MEST_MATERIAL> listMaterial,
            List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs,
            List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs,
            List<V_HIS_MEDI_STOCK> listMediStock,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes,
            List<V_HIS_MATERIAL_TYPE> _materialTypes,
            keyTitles _KeyTitles,
            List<HIS_TREATMENT> _ListTreatment,
            HisExpMestBcsMoreInfoSDO moreInfoSDO
            )
        {
            this._BcsExpMest = chmsExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this._ExpMestMetyReqs = _expMestMetyReqs;
            this._ExpMestMatyReqs = _expMestMatyReqs;
            this._MediStocks = listMediStock;
            this._MedicineTypes = _medicineTypes;
            this._MaterialTypes = _materialTypes;
            this._keyTitles = _KeyTitles;
            this.ListTreatment = _ListTreatment;
            this._BcsMoreInfoSDO = moreInfoSDO;
        }

        public Mps000215PDO(
          V_HIS_EXP_MEST chmsExpMest,
          List<V_HIS_EXP_MEST_MEDICINE> listMedicine,
          List<V_HIS_EXP_MEST_MATERIAL> listMaterial,
          List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs,
          List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs,
          List<V_HIS_MEDI_STOCK> listMediStock,
          List<V_HIS_MEDICINE_TYPE> _medicineTypes,
          List<V_HIS_MATERIAL_TYPE> _materialTypes,
          keyTitles _KeyTitles,
          List<HIS_TREATMENT> _ListTreatment,
          HisExpMestBcsMoreInfoSDO moreInfoSDO,
         long _OderOptionKey
    )
        {
            this._BcsExpMest = chmsExpMest;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this._ExpMestMetyReqs = _expMestMetyReqs;
            this._ExpMestMatyReqs = _expMestMatyReqs;
            this._MediStocks = listMediStock;
            this._MedicineTypes = _medicineTypes;
            this._MaterialTypes = _materialTypes;
            this._keyTitles = _KeyTitles;
            this.ListTreatment = _ListTreatment;
            this._BcsMoreInfoSDO = moreInfoSDO;
            this.OderOptionKey = _OderOptionKey;
        }
    }
}

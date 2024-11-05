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

namespace MPS.Processor.Mps000086.PDO
{
    public partial class Mps000086PDO : RDOBase
    {
        public List<V_HIS_MEDI_STOCK> _MediStocks { get; set; }
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_MATERIAL_TYPE> _MaterialTypes { get; set; }
        public List<V_HIS_BLOOD_TYPE> _BloodTypes { get; set; }
        public List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs { get; set; }
        public List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs { get; set; }
        public List<HIS_EXP_MEST_BLTY_REQ> _ExpMestBltyReqs { get; set; }
        public List<HIS_MEDICINE_USE_FORM> _MedicineUserForms { get; set; }
        public long _keyMert = 1;
        public long _keyPhieuTra = 0;
        public List<HIS_BLOOD_ABO> _BloodABOs = null;
        public List<HIS_BLOOD_RH> _BloodRHs = null;
        public long OrderKey = 0;
        public List<V_HIS_MEDICINE_PATY> mediPaty { get; set; }
        public List<V_HIS_MATERIAL_PATY> matePaty { get; set; }
        public List<HIS_CONFIG> listConfig { get; set; }

        public Mps000086PDO() { }

        public Mps000086PDO(
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
            long keyPhieuTra,
            List<HIS_MEDICINE> _medicine,
            List<HIS_MATERIAL> _material,
            List<HIS_BLOOD> _blood
            )
        {
            this._ChmsExpMest = chmsExpMest;
            this._ExpMestMaterials = listMaterial;
            this._ExpMestMedicines = listMedicine;
            this._ExpMestBloods = listBlood;
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
            this._keyMert = keyMert;
            this._keyPhieuTra = keyPhieuTra;
            this._Medicines = _medicine;
            this._Materials = _material;
            this._Bloods = _blood;
        }

        public Mps000086PDO(
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
    long keyPhieuTra,
    List<HIS_MEDICINE> _medicine,
    List<HIS_MATERIAL> _material,
    List<HIS_BLOOD> _blood,
    long _OrderKey
    )
        {
            this._ChmsExpMest = chmsExpMest;
            this._ExpMestMaterials = listMaterial;
            this._ExpMestMedicines = listMedicine;
            this._ExpMestBloods = listBlood;
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
            this._keyMert = keyMert;
            this._keyPhieuTra = keyPhieuTra;
            this._Medicines = _medicine;
            this._Materials = _material;
            this._Bloods = _blood;
            this.OrderKey = _OrderKey;
        }

        public Mps000086PDO(
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
            long keyPhieuTra,
            List<HIS_MEDICINE> _medicine,
            List<HIS_MATERIAL> _material,
            List<HIS_BLOOD> _blood,
            long _OrderKey,
            List<HIS_MEDICINE_USE_FORM> _medicineUserForms
            )
        {
            this._ChmsExpMest = chmsExpMest;
            this._ExpMestMaterials = listMaterial;
            this._ExpMestMedicines = listMedicine;
            this._ExpMestBloods = listBlood;
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
            this._keyMert = keyMert;
            this._keyPhieuTra = keyPhieuTra;
            this._Medicines = _medicine;
            this._Materials = _material;
            this._Bloods = _blood;
            this.OrderKey = _OrderKey;
            this._MedicineUserForms = _medicineUserForms;
        }

        public Mps000086PDO(
            V_HIS_EXP_MEST chmsExpMest,
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicine,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterial,
            List<V_HIS_EXP_MEST_BLOOD> expMestBlood,
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
            List<HIS_BLOOD_ABO> _bloodABOs,
            List<HIS_BLOOD_RH> _bloodRHs,
            string keyNames,
            long keyMert,
            long keyPhieuTra,
            List<HIS_MEDICINE> _medicine,
            List<HIS_MATERIAL> _material,
            List<HIS_BLOOD> _blood
            )
        {
            this._ChmsExpMest = chmsExpMest;
            this._ExpMestMaterials = _expMestMaterial;
            this._ExpMestMedicines = _expMestMedicine;
            this._ExpMestBloods = expMestBlood;
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
            this._BloodABOs = _bloodABOs;
            this._BloodRHs = _bloodRHs;
            this.KeyNames = keyNames;
            this._keyMert = keyMert;
            this._keyPhieuTra = keyPhieuTra;
            this._Medicines = _medicine;
            this._Materials = _material;
            this._Bloods = _blood;
        }

        public Mps000086PDO(
    V_HIS_EXP_MEST chmsExpMest,
    List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicine,
    List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterial,
    List<V_HIS_EXP_MEST_BLOOD> expMestBlood,
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
    List<HIS_BLOOD_ABO> _bloodABOs,
    List<HIS_BLOOD_RH> _bloodRHs,
    string keyNames,
    long keyMert,
    long keyPhieuTra,
    List<HIS_MEDICINE> _medicine,
    List<HIS_MATERIAL> _material,
    List<HIS_BLOOD> _blood,
            long _OrDerKey
    )
        {
            this._ChmsExpMest = chmsExpMest;
            this._ExpMestMaterials = _expMestMaterial;
            this._ExpMestMedicines = _expMestMedicine;
            this._ExpMestBloods = expMestBlood;
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
            this._BloodABOs = _bloodABOs;
            this._BloodRHs = _bloodRHs;
            this.KeyNames = keyNames;
            this._keyMert = keyMert;
            this._keyPhieuTra = keyPhieuTra;
            this._Medicines = _medicine;
            this._Materials = _material;
            this._Bloods = _blood;
            this.OrderKey = _OrDerKey;
        }
        //178567
        // bo sung gia ban vao
        public Mps000086PDO(
        V_HIS_EXP_MEST chmsExpMest,
        List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicine,
        List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterial,
        List<V_HIS_EXP_MEST_BLOOD> expMestBlood,
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
        List<HIS_BLOOD_ABO> _bloodABOs,
        List<HIS_BLOOD_RH> _bloodRHs,
        string keyNames,
        long keyMert,
        long keyPhieuTra,
        List<HIS_MEDICINE> _medicine,
        List<HIS_MATERIAL> _material,
        List<HIS_BLOOD> _blood,
        long _OrDerKey,
        List<HIS_MEDICINE_USE_FORM> _medicineUserForms,
        List<V_HIS_MEDICINE_PATY> medicinePaty,
        List<V_HIS_MATERIAL_PATY> materialPaty,
        List<HIS_CONFIG> config
        )
        {
            this._ChmsExpMest = chmsExpMest;
            this._ExpMestMaterials = _expMestMaterial;
            this._ExpMestMedicines = _expMestMedicine;
            this._ExpMestBloods = expMestBlood;
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
            this._BloodABOs = _bloodABOs;
            this._BloodRHs = _bloodRHs;
            this.KeyNames = keyNames;
            this._keyMert = keyMert;
            this._keyPhieuTra = keyPhieuTra;
            this._Medicines = _medicine;
            this._Materials = _material;
            this._Bloods = _blood;
            this.OrderKey = _OrDerKey;
            this.mediPaty = medicinePaty;
            this.matePaty = materialPaty;
            this.listConfig = config;
            this._MedicineUserForms = _medicineUserForms;
        }
    }
}

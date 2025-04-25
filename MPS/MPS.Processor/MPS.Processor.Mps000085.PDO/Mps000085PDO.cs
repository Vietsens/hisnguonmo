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

namespace MPS.Processor.Mps000085.PDO
{     
    public partial class Mps000085PDO : RDOBase
    {
        public List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines = null;
        public List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterials = null;
        public List<HIS_MEDICINE> _Medicines = null;
        public List<HIS_MATERIAL> _Materials = null;
        //public List<ImpMestMedicineADO> _ImpMestMedicineAdos = null;
        //public List<ImpMestMaterialADO> _ImpMestMaterialAdos = null;
        public List<V_HIS_IMP_MEST_USER> _ImpMestUserPrint = null;
        public List<Mps000085ADO> listAdo = new List<Mps000085ADO>();
        public List<RoleADO> roleAdo = new List<RoleADO>();

        public List<MedicalContractADO> _ListMedicalContract = null;

        //public Mps000085PDO(V_HIS_IMP_MEST impMest, List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines, List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials)
        //{
        //    this._ImpMest = impMest;
        //    // this._ManuImpMest = manuImpMest;
        //    this._ImpMestMaterials = impMestMaterials;
        //    this._ImpMestMedicines = impMestMedicines;
        //}

        //public Mps000085PDO(V_HIS_IMP_MEST impMest, List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines, List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials, List<V_HIS_IMP_MEST_USER> listImpMestUserPrint)
        //{
        //    this._ImpMest = impMest;
        //    // this._ManuImpMest = manuImpMest;
        //    this._ImpMestMaterials = impMestMaterials;
        //    this._ImpMestMedicines = impMestMedicines;
        //    this._ImpMestUserPrint = listImpMestUserPrint;
        //}

        public Mps000085PDO() { }

        public Mps000085PDO(V_HIS_IMP_MEST impMest, List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines, List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials, List<V_HIS_IMP_MEST_USER> listImpMestUserPrint, List<HIS_MEDICINE> medicine, List<HIS_MATERIAL> material, HIS_SUPPLIER supplier)
        {
            this._ImpMest = impMest;
            this._supplier = supplier;
            this._Medicines = medicine;
            this._Materials = material;
            this._ImpMestMaterials = impMestMaterials;
            this._ImpMestMedicines = impMestMedicines;
            this._ImpMestUserPrint = listImpMestUserPrint;
        }

        public Mps000085PDO(V_HIS_IMP_MEST impMest, List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines, List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials, List<V_HIS_IMP_MEST_USER> listImpMestUserPrint, List<HIS_MEDICINE> medicine, List<HIS_MATERIAL> material, HIS_SUPPLIER supplier, List<MedicalContractADO> listMedicalContract)
        {
            this._ImpMest = impMest;
            this._supplier = supplier;
            this._Medicines = medicine;
            this._Materials = material;
            this._ImpMestMaterials = impMestMaterials;
            this._ImpMestMedicines = impMestMedicines;
            this._ImpMestUserPrint = listImpMestUserPrint;
            this._ListMedicalContract = listMedicalContract;
        }

        public Mps000085PDO(V_HIS_IMP_MEST impMest, List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines, List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials, List<V_HIS_IMP_MEST_USER> listImpMestUserPrint, List<HIS_MEDICINE> medicine, List<HIS_MATERIAL> material, HIS_SUPPLIER supplier, List<MedicalContractADO> listMedicalContract, List<V_HIS_IMP_MEST_BLOOD> lstMestBlook)
        {
            this._ImpMest = impMest;
            this._supplier = supplier;
            this._Medicines = medicine;
            this._Materials = material;
            this._ImpMestMaterials = impMestMaterials;
            this._ImpMestMedicines = impMestMedicines;
            this._ImpMestUserPrint = listImpMestUserPrint;
            this._ListMedicalContract = listMedicalContract;
            this._ListImpMestBlood = lstMestBlook;
        }

        //public Mps000085PDO(V_HIS_IMP_MEST impMest, List<ImpMestMedicineADO> impMestMedicineAdos, List<ImpMestMaterialADO> impMestMaterialAdos, List<V_HIS_IMP_MEST_USER> listImpMestUserPrint, V_HIS_BID bid)
        //{
        //    this._ImpMest = impMest;
        //    this._bid = bid;
        //    this._ImpMestMaterialAdos = impMestMaterialAdos;
        //    this._ImpMestMedicineAdos = impMestMedicineAdos;
        //    this._ImpMestUserPrint = listImpMestUserPrint;
        //}
    }
}

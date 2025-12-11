using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000505.PDO
{
    public partial class Mps000505PDO : RDOBase
    {
        public List<V_HIS_IMP_MEST> vHisImpMests = null;
        public List<V_HIS_IMP_MEST_MEDICINE> vHisImpMestMedicines = null;
        public List<V_HIS_IMP_MEST_MATERIAL> vHisImpMestMaterials = null;
        public List<V_HIS_IMP_MEST_BLOOD> vHisImpMestBloods = null;
        public List<HIS_MEDICINE> hisMedicine = null;
        public List<HIS_MATERIAL> hisMaterial = null;
        public List<V_HIS_IMP_MEST_USER> vHisImpMestUsers = null;
        public List<V_HIS_MEDICINE_PATY> vHisMedicinePatys = null;
        public List<V_HIS_MATERIAL_PATY> vHisMAterialPatis = null;
        public List<HIS_SUPPLIER> hisSuppliers = null;
        public List<MedicalContractADO> medicalContractADOs = null;
        public List<Mps000505ADO> listAdo = new List<Mps000505ADO>();

        public Mps000505PDO()
        {
            
        }

        public Mps000505PDO(
            List<V_HIS_IMP_MEST> vHisImpMests, 
            List<V_HIS_IMP_MEST_MEDICINE> vHisImpMestMedicines,
            List<V_HIS_IMP_MEST_MATERIAL> vHisImpMestMaterials,
            List<V_HIS_IMP_MEST_BLOOD> vHisImpMestBloods,
            List<HIS_MEDICINE> hisMedicine,
            List<HIS_MATERIAL> hisMaterial,
            List<V_HIS_IMP_MEST_USER> vHisImpMestUsers,
            List<V_HIS_MEDICINE_PATY> vHisMedicinePatys,
            List<V_HIS_MATERIAL_PATY> vHisMAterialPatis,
            List<HIS_SUPPLIER> hisSuppliers,
            List<MedicalContractADO> medicalContractADOs
            )
        {
            this.vHisImpMests = vHisImpMests;
            this.vHisImpMestMedicines = vHisImpMestMedicines;
            this.vHisImpMestMaterials = vHisImpMestMaterials;
            this.vHisImpMestBloods = vHisImpMestBloods;
            this.hisMedicine = hisMedicine;
            this.hisMaterial = hisMaterial;
            this.vHisImpMestUsers = vHisImpMestUsers;
            this.vHisMedicinePatys = vHisMedicinePatys;
            this.vHisMAterialPatis = vHisMAterialPatis;
            this.hisSuppliers = hisSuppliers;
            this.medicalContractADOs = medicalContractADOs;
        }
    }
}

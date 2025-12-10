using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000505.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000505
{
    class Mps000505Processor : AbstractProcessor
    {
        Mps000505PDO rdo;
        public Mps000505Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000505PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();

                objectTag.AddObjectData(store, "Mps000505ADO", rdo.listAdo);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                if (this.rdo.vHisImpMestMedicines != null && this.rdo.vHisImpMestMedicines.Count > 0)
                {
                    this.rdo.vHisImpMestMedicines = this.rdo.vHisImpMestMedicines.OrderBy(o => o.ID).ToList();
                    foreach (var item in this.rdo.vHisImpMestMedicines)
                    {
                        Mps000505ADO ado = new Mps000505ADO();
                        ado.MEDI_MATE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                        ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        ado.NATIONAL_NAME = item.NATIONAL_NAME;
                        ado.BATCH_REGISTER_NUMBER = item.REGISTER_NUMBER;
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        ado.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.EXPIRED_DATE ?? 0);
                        ado.PRICE = item.PRICE ?? 0;
                        ado.AMOUNT = item.AMOUNT;
                        ado.BATCH_REGISTER_NUMBER = item.MEDICINE_REGISTER_NUMBER;
                        ado.BATCH_MANUFACTURER_CODE = item.MEDICINE_MANUFACTURER_CODE;
                        ado.BATCH_MANUFACTURER_NAME = item.MEDICINE_MANUFACTURER_NAME;
                        if (!item.PRICE.HasValue)
                            continue;
                        ado.PRICE_AMOUNT = item.PRICE.Value * item.AMOUNT;

                        var medicine = rdo.hisMedicine != null ? rdo.hisMedicine.FirstOrDefault(o => o.ID == item.MEDICINE_ID) : null;
                        var medicinePaties = rdo.vHisMedicinePatys != null ? rdo.vHisMedicinePatys.Where(o => o.MEDICINE_ID == item.MEDICINE_ID).ToList() : new List<V_HIS_MEDICINE_PATY>();
                        Dictionary<string, decimal> dicPrice = new Dictionary<string, decimal>();
                        if (medicine != null)
                        {
                            foreach (var paty in medicinePaties)
                            {
                                decimal price = 0;
                                if (medicine.IS_SALE_EQUAL_IMP_PRICE != 1)
                                {
                                    if (item.TDL_IMP_UNIT_ID.HasValue)
                                    {
                                        price = (paty.IMP_UNIT_EXP_PRICE ?? 0) * (1 + (paty.EXP_VAT_RATIO));
                                    }
                                    else
                                    {
                                        price = (paty.EXP_PRICE) * (1 + (paty.EXP_VAT_RATIO));
                                    }
                                }
                                else
                                {
                                    if (item.TDL_IMP_UNIT_ID.HasValue)
                                    {
                                        price = (medicine.IMP_UNIT_PRICE ?? 0) * (1 + (item.IMP_VAT_RATIO));
                                    }
                                    else
                                    {
                                        price = (medicine.IMP_PRICE) * (1 + (item.IMP_VAT_RATIO));
                                    }
                                }
                                if (!dicPrice.ContainsKey(paty.PATIENT_TYPE_CODE))
                                {
                                    dicPrice[paty.PATIENT_TYPE_CODE] = price;
                                }
                            }
                        }
                        ado.DicMediMate = dicPrice;

                        if (rdo.hisMedicine != null && rdo.hisMedicine.Count > 0)
                        {
                            ado.TDL_BID_GROUP_CODE = rdo.hisMedicine.FirstOrDefault(o => o.ID == item.MEDICINE_ID).TDL_BID_GROUP_CODE;
                            ado.TDL_BID_NUM_ORDER = rdo.hisMedicine.FirstOrDefault(o => o.ID == item.MEDICINE_ID).TDL_BID_NUM_ORDER;
                            ado.TDL_BID_NUMBER = rdo.hisMedicine.FirstOrDefault(o => o.ID == item.MEDICINE_ID).TDL_BID_NUMBER;
                            ado.TDL_BID_PACKAGE_CODE = rdo.hisMedicine.FirstOrDefault(o => o.ID == item.MEDICINE_ID).TDL_BID_PACKAGE_CODE;
                            ado.TDL_BID_YEAR = rdo.hisMedicine.FirstOrDefault(o => o.ID == item.MEDICINE_ID).TDL_BID_YEAR;
                            ado.VIR_IMP_PRICE = rdo.hisMedicine.FirstOrDefault(o => o.ID == item.MEDICINE_ID).VIR_IMP_PRICE;
                        }

                        if (rdo.medicalContractADOs != null && rdo.medicalContractADOs.Count > 0)
                        {
                            V_HIS_MEDICAL_CONTRACT MedicalContract = rdo.medicalContractADOs.FirstOrDefault(o => o.MEDICINE_ID == item.MEDICINE_ID);
                            if (MedicalContract != null)
                            {
                                ado.MEDICAL_CONTRACT_CODE = MedicalContract.MEDICAL_CONTRACT_CODE;
                                ado.MEDICAL_CONTRACT_NAME = MedicalContract.MEDICAL_CONTRACT_NAME;
                                ado.DOCUMENT_SUPPLIER_NAME = MedicalContract.DOCUMENT_SUPPLIER_NAME;
                                ado.VENTURE_AGREENING = MedicalContract.VENTURE_AGREENING;
                            }
                        }

                        if (this.rdo.vHisImpMests != null && this.rdo.vHisImpMests.Count > 0)
                        {
                            var impMest = this.rdo.vHisImpMests.FirstOrDefault(o => o.ID == item.IMP_MEST_ID);
                            if (impMest != null) 
                            {
                                ado.IMP_DATE = impMest.IMP_DATE;
                                ado.DOCUMENT_NUMBER = impMest.DOCUMENT_NUMBER;
                                ado.SUPPLIER_ID = impMest.SUPPLIER_ID;
                                ado.MEDI_STOCK_ID = impMest.CHMS_MEDI_STOCK_ID ?? 0;
                                ado.APPROVAL_TIME = impMest.APPROVAL_TIME;
                                var supplier = this.rdo.hisSuppliers.FirstOrDefault(o => o.ID == impMest.SUPPLIER_ID);
                                if (supplier != null)
                                {
                                    ado.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                                    ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                                }
                            }
                        }
                        this.rdo.listAdo.Add(ado);
                    }
                }
                if (this.rdo.vHisImpMestMaterials != null && this.rdo.vHisImpMestMaterials.Count > 0)
                {
                    this.rdo.vHisImpMestMaterials = this.rdo.vHisImpMestMaterials.OrderBy(o => o.ID).ToList();
                    foreach (var item in this.rdo.vHisImpMestMaterials) 
                    {
                        Mps000505ADO ado = new Mps000505ADO();
                        ado.MEDI_MATE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        ado.NATIONAL_NAME = item.NATIONAL_NAME;
                        //ado.BATCH_REGISTER_NUMBER = item.REGISTER_NUMBER;
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        ado.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.EXPIRED_DATE ?? 0);
                        ado.PRICE = item.PRICE ?? 0;
                        ado.AMOUNT = item.AMOUNT;
                        ado.BATCH_REGISTER_NUMBER = item.MATERIAL_REGISTER_NUMBER;
                        ado.BATCH_MANUFACTURER_CODE = item.MATERIAL_MANUFACTURER_CODE;
                        ado.BATCH_MANUFACTURER_NAME = item.MATERIAL_MANUFACTURER_NAME;
                        if (!item.PRICE.HasValue)
                            continue;
                        ado.PRICE_AMOUNT = item.PRICE.Value * item.AMOUNT;

                        var material = rdo.hisMaterial != null ? rdo.hisMaterial.FirstOrDefault(o => o.ID == item.MATERIAL_ID) : null;
                        var materialPaties = rdo.vHisMAterialPatis != null ? rdo.vHisMAterialPatis.Where(o => o.MATERIAL_ID == item.MATERIAL_ID).ToList() : new List<V_HIS_MATERIAL_PATY>();
                        Dictionary<string, decimal> dicPrice = new Dictionary<string, decimal>();
                        if (material != null)
                        {
                            foreach (var paty in materialPaties)
                            {
                                decimal price = 0;
                                if (material.IS_SALE_EQUAL_IMP_PRICE != 1)
                                {
                                    if (item.TDL_IMP_UNIT_ID.HasValue)
                                    {
                                        price = (paty.IMP_UNIT_EXP_PRICE ?? 0) * (1 + (paty.EXP_VAT_RATIO));
                                    }
                                    else
                                    {
                                        price = (paty.EXP_PRICE) * (1 + (paty.EXP_VAT_RATIO));
                                    }
                                }
                                else
                                {
                                    if (item.TDL_IMP_UNIT_ID.HasValue)
                                    {
                                        price = (material.IMP_UNIT_PRICE ?? 0) * (1 + (item.IMP_VAT_RATIO));
                                    }
                                    else
                                    {
                                        price = (material.IMP_PRICE) * (1 + (item.IMP_VAT_RATIO));
                                    }
                                }
                                if (!dicPrice.ContainsKey(paty.PATIENT_TYPE_CODE))
                                {
                                    dicPrice[paty.PATIENT_TYPE_CODE] = price;
                                }
                            }
                        }
                        ado.DicMediMate = dicPrice;

                        if (rdo.hisMaterial != null && rdo.hisMaterial.Count > 0)
                        {
                            ado.TDL_BID_GROUP_CODE = rdo.hisMaterial.FirstOrDefault(o => o.ID == item.MATERIAL_ID).TDL_BID_GROUP_CODE;
                            ado.TDL_BID_NUM_ORDER = rdo.hisMaterial.FirstOrDefault(o => o.ID == item.MATERIAL_ID).TDL_BID_NUM_ORDER;
                            ado.TDL_BID_NUMBER = rdo.hisMaterial.FirstOrDefault(o => o.ID == item.MATERIAL_ID).TDL_BID_NUMBER;
                            ado.TDL_BID_PACKAGE_CODE = rdo.hisMaterial.FirstOrDefault(o => o.ID == item.MATERIAL_ID).TDL_BID_PACKAGE_CODE;
                            ado.TDL_BID_YEAR = rdo.hisMaterial.FirstOrDefault(o => o.ID == item.MATERIAL_ID).TDL_BID_YEAR;
                            ado.VIR_IMP_PRICE = rdo.hisMaterial.FirstOrDefault(o => o.ID == item.MATERIAL_ID).VIR_IMP_PRICE;
                        }

                        if (rdo.medicalContractADOs != null && rdo.medicalContractADOs.Count > 0)
                        {
                            V_HIS_MEDICAL_CONTRACT MedicalContract = rdo.medicalContractADOs.FirstOrDefault(o => o.MATERIAL_ID == item.MATERIAL_ID);
                            if (MedicalContract != null)
                            {
                                ado.MEDICAL_CONTRACT_CODE = MedicalContract.MEDICAL_CONTRACT_CODE;
                                ado.MEDICAL_CONTRACT_NAME = MedicalContract.MEDICAL_CONTRACT_NAME;
                                ado.DOCUMENT_SUPPLIER_NAME = MedicalContract.DOCUMENT_SUPPLIER_NAME;
                                ado.VENTURE_AGREENING = MedicalContract.VENTURE_AGREENING;
                            }
                        }

                        if (this.rdo.vHisImpMests != null && this.rdo.vHisImpMests.Count > 0)
                        {
                            var impMest = this.rdo.vHisImpMests.FirstOrDefault(o => o.ID == item.IMP_MEST_ID);
                            if (impMest != null)
                            {
                                ado.IMP_DATE = impMest.IMP_DATE;
                                ado.DOCUMENT_NUMBER = impMest.DOCUMENT_NUMBER;
                                ado.SUPPLIER_ID = impMest.SUPPLIER_ID;
                                ado.MEDI_STOCK_ID = impMest.CHMS_MEDI_STOCK_ID ?? 0;
                                ado.APPROVAL_TIME = impMest.APPROVAL_TIME;
                                var supplier = this.rdo.hisSuppliers.FirstOrDefault(o => o.ID == impMest.SUPPLIER_ID);
                                if (supplier != null)
                                {
                                    ado.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                                    ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                                }
                            }
                        }
                        this.rdo.listAdo.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

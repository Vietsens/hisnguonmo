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
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.HisMedicalContractImport.ADO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;

namespace HIS.Desktop.Plugins.HisMedicalContractImport
{
    public partial class frmHisMedicalContractImport
    {
        private List<V_HIS_BID_MATERIAL_TYPE> storeBidMaterialType = null;
        private List<V_HIS_BID_MEDICINE_TYPE> storeBidMedicineType = null;
        private void ProcessSave()
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                Dictionary<string, HIS_MEDICAL_CONTRACT> dicMedicalContract_ForProcess = new Dictionary<string, HIS_MEDICAL_CONTRACT>();
                Dictionary<string, List<HIS_MEDI_CONTRACT_MATY>> dicMediContractMatys_ForProcess = new Dictionary<string, List<HIS_MEDI_CONTRACT_MATY>>();
                Dictionary<string, List<HIS_MEDI_CONTRACT_METY>> dicMediContractMetys_ForProcess = new Dictionary<string, List<HIS_MEDI_CONTRACT_METY>>();

                if (this._MedicalContractAdos != null && this._MedicalContractAdos.Count > 0)
                {
                    foreach (var item in this._MedicalContractAdos)
                    {
                        if (!String.IsNullOrEmpty(item.ERROR))
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại dữ liệu lỗi", "Thông báo");
                            return;
                        }
                        HIS_MEDICAL_CONTRACT processMedicalContract = new HIS_MEDICAL_CONTRACT();

                        if (dicMedicalContract_ForProcess.ContainsKey(item.MEDICAL_CONTRACT_CODE)
                            && dicMedicalContract_ForProcess[item.MEDICAL_CONTRACT_CODE] != null)
                        {
                            processMedicalContract = dicMedicalContract_ForProcess[item.MEDICAL_CONTRACT_CODE];
                        }
                        else
                        {
                            processMedicalContract.MEDICAL_CONTRACT_CODE = item.MEDICAL_CONTRACT_CODE;
                            processMedicalContract.SUPPLIER_ID = item.SUPPLIER_ID ?? 0;
                            processMedicalContract.BID_ID = item.BID_ID;
                            processMedicalContract.VALID_FROM_DATE = item.VALID_FROM_DATE;
                            processMedicalContract.VALID_TO_DATE = item.VALID_TO_DATE;
                            processMedicalContract.VENTURE_AGREENING = item.VENTURE_AGREENING;
                            processMedicalContract.DOCUMENT_SUPPLIER_ID = item.DOCUMENT_SUPPLIER_ID;

                            dicMedicalContract_ForProcess.Add(item.MEDICAL_CONTRACT_CODE, processMedicalContract);
                        }

                        if (item.isMedicine)
                        {
                            if (!dicMediContractMetys_ForProcess.ContainsKey(item.MEDICAL_CONTRACT_CODE))
                            {
                                dicMediContractMetys_ForProcess.Add(item.MEDICAL_CONTRACT_CODE, new List<HIS_MEDI_CONTRACT_METY>());
                            }

                            HIS_MEDI_CONTRACT_METY processMediContractMety = new HIS_MEDI_CONTRACT_METY();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDI_CONTRACT_METY>(processMediContractMety, item);

                            var medicineType = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE);
                            if (medicineType != null)
                            {
                                processMediContractMety.MEDICINE_TYPE_ID = medicineType.ID;
                                var bidMedicineType = GetBIDMedicineType(item.BID_ID, medicineType.ID);
                                if (bidMedicineType != null)
                                    processMediContractMety.BID_MEDICINE_TYPE_ID = bidMedicineType.ID;
                            }

                            var medicineUseForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(p => p.MEDICINE_USE_FORM_CODE == item.MEDICINE_USE_FORM_CODE);
                            processMediContractMety.MEDICINE_USE_FORM_ID = medicineUseForm != null ? medicineUseForm.ID : 0;

                            var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(p => p.MANUFACTURER_CODE == item.MANUFACTURER_CODE);
                            if (manufacturer != null)
                            {
                                processMediContractMety.MANUFACTURER_ID = manufacturer.ID;
                            }
                            else
                                processMediContractMety.MANUFACTURER_ID = null;
                            

                            //processMediContractMety.AMOUNT = item.AMOUNT;
                            //processMediContractMety.CONTRACT_PRICE = item.CONTRACT_PRICE;
                            //processMediContractMety.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                            //processMediContractMety.MEDICINE_REGISTER_NUMBER = item.MEDICINE_REGISTER_NUMBER;
                            //processMediContractMety.NATIONAL_NAME = item.NATIONAL_NAME;
                            //processMediContractMety.BID_NUMBER = item.BID_NUMBER;
                            //processMediContractMety.BID_GROUP_CODE = item.BID_GROUP_CODE;
                            //processMediContractMety.IMP_EXPIRED_DATE = item.IMP_EXPIRED_DATE;
                            //processMediContractMety.EXPIRED_DATE = item.EXPIRED_DATE;
                            //processMediContractMety.CONCENTRA = item.CONCENTRA;
                            //processMediContractMety.NOTE = item.NOTE;
                            //processMediContractMety.MONTH_LIFESPAN = item.MONTH_LIFESPAN;
                            //processMediContractMety.DAY_LIFESPAN = item.DAY_LIFESPAN;
                            //processMediContractMety.HOUR_LIFESPAN = item.HOUR_LIFESPAN;

                            dicMediContractMetys_ForProcess[item.MEDICAL_CONTRACT_CODE].Add(processMediContractMety);
                        }
                        else
                        {
                            if (!dicMediContractMatys_ForProcess.ContainsKey(item.MEDICAL_CONTRACT_CODE))
                            {
                                dicMediContractMatys_ForProcess.Add(item.MEDICAL_CONTRACT_CODE, new List<HIS_MEDI_CONTRACT_MATY>());
                            }

                            HIS_MEDI_CONTRACT_MATY processMediContractMaty = new HIS_MEDI_CONTRACT_MATY();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDI_CONTRACT_MATY>(processMediContractMaty, item);

                            var materialType = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.MATERIAL_TYPE_CODE == item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE);
                            if (materialType != null)
                            {
                                processMediContractMaty.MATERIAL_TYPE_ID = materialType.ID;

                                var bidMaterialType = GetBIDMaterialType(item.BID_ID, materialType.ID);
                                if (bidMaterialType != null)
                                    processMediContractMaty.BID_MATERIAL_TYPE_ID = bidMaterialType.ID;
                            }

                            var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(p => p.MANUFACTURER_CODE == item.MANUFACTURER_CODE);
                            if (manufacturer != null)
                            {
                                processMediContractMaty.MANUFACTURER_ID = manufacturer.ID;
                            }
                            else
                                processMediContractMaty.MANUFACTURER_ID = null;
                            //processMediContractMaty.AMOUNT = item.AMOUNT;
                            //processMediContractMaty.CONTRACT_PRICE = item.CONTRACT_PRICE;
                            //processMediContractMaty.NATIONAL_NAME = item.NATIONAL_NAME;
                            //processMediContractMaty.BID_NUMBER = item.BID_NUMBER;
                            //processMediContractMaty.BID_GROUP_CODE = item.BID_GROUP_CODE;
                            //processMediContractMaty.IMP_EXPIRED_DATE = item.IMP_EXPIRED_DATE;
                            //processMediContractMaty.EXPIRED_DATE = item.EXPIRED_DATE;
                            //processMediContractMaty.CONCENTRA = item.CONCENTRA;
                            //processMediContractMaty.NOTE = item.NOTE;
                            //processMediContractMaty.MONTH_LIFESPAN = item.MONTH_LIFESPAN;
                            //processMediContractMaty.DAY_LIFESPAN = item.DAY_LIFESPAN;
                            //processMediContractMaty.HOUR_LIFESPAN = item.HOUR_LIFESPAN;

                            dicMediContractMatys_ForProcess[item.MEDICAL_CONTRACT_CODE].Add(processMediContractMaty);
                        }
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }

                List<HIS_MEDICAL_CONTRACT> processListDatas = new List<HIS_MEDICAL_CONTRACT>();

                foreach (var item in dicMedicalContract_ForProcess)
                {
                    HIS_MEDICAL_CONTRACT processdata = new HIS_MEDICAL_CONTRACT();
                    processdata = item.Value;
                    if (dicMediContractMetys_ForProcess.ContainsKey(item.Key))
                        processdata.HIS_MEDI_CONTRACT_METY = dicMediContractMetys_ForProcess[item.Key];
                    else
                        processdata.HIS_MEDI_CONTRACT_METY = new List<HIS_MEDI_CONTRACT_METY>();
                    if (dicMediContractMatys_ForProcess.ContainsKey(item.Key))
                        processdata.HIS_MEDI_CONTRACT_MATY = dicMediContractMatys_ForProcess[item.Key];
                    else
                        processdata.HIS_MEDI_CONTRACT_MATY = new List<HIS_MEDI_CONTRACT_MATY>();
                    processListDatas.Add(processdata);
                }

                CommonParam param = new CommonParam();
                var apiResult = new BackendAdapter(param).Post<List<HIS_MEDICAL_CONTRACT>>(HisRequestUriStore.MOS_HIS_MEDICAL_CONTRACT_IMPORT, ApiConsumers.MosConsumer, processListDatas, param);
                WaitingManager.Hide();
                if (apiResult != null && apiResult.Count > 0)
                {
                    success = true;
                    btnSave.Enabled = false;
                }

                MessageManager.Show(this.ParentForm, param, success);
                if (success)
                {
                    if (this._DelegateRefresh != null)
                    {
                        this._DelegateRefresh();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_BID_MATERIAL_TYPE GetBIDMaterialType(long? bidID, long materialTypeId)
        {
            V_HIS_BID_MATERIAL_TYPE result = null;
            try
            {
                if (this.storeBidMaterialType != null && this.storeBidMaterialType.Count() > 0)
                {
                    result = this.storeBidMaterialType.FirstOrDefault(o => o.BID_ID == bidID && o.MATERIAL_TYPE_ID == materialTypeId);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private V_HIS_BID_MEDICINE_TYPE GetBIDMedicineType(long? bidID, long medicineTypeId)
        {
            V_HIS_BID_MEDICINE_TYPE result = null;
            try
            {
                if (this.storeBidMedicineType != null && this.storeBidMedicineType.Count > 0)
                {
                    result = this.storeBidMedicineType.FirstOrDefault(o => o.BID_ID == bidID && o.MEDICINE_TYPE_ID == medicineTypeId);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.DispenseMedicine.ADO;
using HIS.Desktop.Plugins.DispenseMedicine.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DispenseMedicine
{
    public partial class frmDispenseMedicine : FormBase
    {

        private void LoadDispenseMedicineEdit(long? dispenseId)
        {
            try
            {
                if (dispenseId.HasValue && this.action == ACTION.UPDATE)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    expMestFilter.DISPENSE_ID = dispenseId;
                    List<HIS_EXP_MEST> expMests = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                    if (expMests == null || expMests.Count == 0)
                    {
                        throw new Exception("Khong tim thay expMests");
                    }

                    //Lay thong tin kho
                    this.mediStock = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == expMests.FirstOrDefault().MEDI_STOCK_ID);

                    HisMediStockFilter mediStockFilter = new HisMediStockFilter();
                    mediStockFilter.ID = expMests.FirstOrDefault().MEDI_STOCK_ID;
                    this.mediStock = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>>("api/HisMediStock/Get", ApiConsumers.MosConsumer, mediStockFilter, param).FirstOrDefault();

                    HisImpMestFilter impMestFilter = new HisImpMestFilter();
                    impMestFilter.DISPENSE_ID = dispenseId;
                    List<HIS_IMP_MEST> impMests = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, impMestFilter, param);

                    if (impMests == null || impMests.Count == 0)
                    {
                        throw new Exception("Khong tim thay impMests");
                    }
                    WaitingManager.Hide();
                    LoadImpMestToGrid(impMests, expMests);
                    LoadThuocThanhPhamTuExpMest(impMests.FirstOrDefault());
                    LoadThuocChePhamTuImpMest(expMests.FirstOrDefault());
                    btnRefesh.Enabled = false;

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnabledButtonPrint()
        {
            try
            {
                if (hisDispenseResultSDO == null || hisDispenseResultSDO.HisExpMest == null)
                {
                    btnPrint.Enabled = false;
                }
                else
                {
                    btnPrint.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadThuocThanhPhamTuExpMest(HIS_IMP_MEST impMest)
        {
            if (impMest != null)
            {
                CommonParam param = new CommonParam();
                HisImpMestMedicineView4Filter filter = new HisImpMestMedicineView4Filter();
                filter.IMP_MEST_ID = impMest.ID;
                List<V_HIS_IMP_MEST_MEDICINE_4> impMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE_4>>("api/HisImpMestMedicine/GetView4", ApiConsumers.MosConsumer, filter, param);

                if (impMestMedicines == null || impMestMedicines.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Không tìm thấy danh sách thuốc thành phẩm");
                    return;
                }


                txtThuocThanhPham.Text = impMestMedicines.FirstOrDefault().MEDICINE_TYPE_NAME;
                spinMetyAmount.Value = impMestMedicines.FirstOrDefault().AMOUNT;

                HIS_SERVICE_UNIT serviceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>()
                    .FirstOrDefault(o => o.ID == impMestMedicines.FirstOrDefault().TDL_SERVICE_UNIT_ID);

                lblTPUnitName.Text = serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : null;
                txtPackageNumber.Text = impMestMedicines.FirstOrDefault().PACKAGE_NUMBER;
                if (impMestMedicines.FirstOrDefault().EXPIRED_DATE.HasValue)
                {
                    dtExpTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(impMestMedicines.FirstOrDefault().EXPIRED_DATE ?? 0) ?? DateTime.Now;
                }

                if (this.mediStock != null)
                {
                    txtMediStockName.Text = this.mediStock.MEDI_STOCK_NAME;
                }


                HisMedicineFilter medicineFilter = new HisMedicineFilter();
                medicineFilter.ID = impMestMedicines.FirstOrDefault().MEDICINE_ID;
                HIS_MEDICINE medicine = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param).FirstOrDefault();
                if (medicine != null)
                {
                    txtHeinDocumentNumber.Text = medicine.TDL_BID_NUMBER;
                }
                InitEnabledAction(ACTION.UPDATE);
                LoadMedicinePaty(impMestMedicines.FirstOrDefault().MEDICINE_ID);
            }

        }

        private void LoadThuocChePhamTuImpMest(HIS_EXP_MEST expMest)
        {
            try
            {
                if (expMest != null)
                {
                    dispenseMetyMatyADOs = new List<DispenseMedyMatyADO>();

                    CommonParam param = new CommonParam();
                    HisExpMestMedicineView2Filter impMestMedicicneFilter = new HisExpMestMedicineView2Filter();
                    impMestMedicicneFilter.EXP_MEST_ID = expMest.ID;
                    List<V_HIS_EXP_MEST_MEDICINE_2> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE_2>>("api/HisExpMestMedicine/GetView2", ApiConsumers.MosConsumer, impMestMedicicneFilter, param);

                    if (expMestMedicines != null && expMestMedicines.Count > 0)
                    {
                        foreach (var item in expMestMedicines)
                        {
                            DispenseMedyMatyADO dispenseMedyMaty = new DispenseMedyMatyADO();
                            dispenseMedyMaty.Amount = item.AMOUNT;
                            dispenseMedyMaty.OldAmount = item.AMOUNT;
                            if (item.TDL_MEDICINE_TYPE_ID.HasValue)
                                dispenseMedyMaty.PreparationMediMatyTypeId = item.TDL_MEDICINE_TYPE_ID.Value;
                            dispenseMedyMaty.PreparationMediMatyTypeName = item.MEDICINE_TYPE_NAME;
                            dispenseMedyMaty.ServiceTypeId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                            HIS_SERVICE_UNIT serviceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>()
.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                            dispenseMedyMaty.ServiceUnitName = serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : null;
                            dispenseMetyMatyADOs.Add(dispenseMedyMaty);
                        }
                    }

                    HisExpMestMaterialView2Filter expMestMaterialFilter = new HisExpMestMaterialView2Filter();
                    expMestMaterialFilter.EXP_MEST_ID = expMest.ID;
                    List<V_HIS_EXP_MEST_MATERIAL_2> expMestMaterials = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL_2>>("api/HisExpMestMaterial/GetView2", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                    if (expMestMaterials != null && expMestMaterials.Count > 0)
                    {
                        foreach (var item in expMestMaterials)
                        {
                            DispenseMedyMatyADO dispenseMedyMaty = new DispenseMedyMatyADO();
                            dispenseMedyMaty.Amount = item.AMOUNT;
                            dispenseMedyMaty.OldAmount = item.AMOUNT;
                            if (item.TDL_MATERIAL_TYPE_ID.HasValue)
                                dispenseMedyMaty.PreparationMediMatyTypeId = item.TDL_MATERIAL_TYPE_ID.Value;
                            dispenseMedyMaty.PreparationMediMatyTypeName = item.MATERIAL_TYPE_NAME;
                            dispenseMedyMaty.ServiceTypeId = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                            HIS_SERVICE_UNIT serviceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>()
.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                            dispenseMedyMaty.ServiceUnitName = serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : null;
                            dispenseMetyMatyADOs.Add(dispenseMedyMaty);
                        }
                    }


                    gridControlDSChePham.DataSource = dispenseMetyMatyADOs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMedicinePaty(long medicineId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicinePatyFilter filter = new HisMedicinePatyFilter();
                filter.MEDICINE_ID = medicineId;
                List<V_HIS_MEDICINE_PATY> medicinePatys = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY>>("api/HisMedicinePaty/GetView", ApiConsumers.MosConsumer, filter, param);

                if (medicinePatys != null && medicinePatys.Count > 0 && medicinePatyADOs != null && medicinePatyADOs.Count > 0)
                {
                    foreach (var item in medicinePatys)
                    {
                        MedicinePatyADO medicinePatyADO = medicinePatyADOs.FirstOrDefault(o => o.PatientTypeId == item.PATIENT_TYPE_ID);
                        medicinePatyADO.PatientTypeName = item.PATIENT_TYPE_NAME;
                        medicinePatyADO.Price = item.EXP_PRICE;
                        medicinePatyADO.Vat = item.EXP_VAT_RATIO * 100;
                        medicinePatyADO.Id = item.ID;
                        medicinePatyADO.MedicineId = item.MEDICINE_ID;
                    }

                    gridControlPaty.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMedicinePaty(List<HIS_MEDICINE_PATY> hisMedicinePatys)
        {
            try
            {
                if (hisMedicinePatys != null && hisMedicinePatys.Count > 0)
                {
                    var patientTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                    foreach (var item in hisMedicinePatys)
                    {
                        MedicinePatyADO medicinePatyADO = medicinePatyADOs.FirstOrDefault(o => o.PatientTypeId == item.PATIENT_TYPE_ID);
                        if (medicinePatyADO != null)
                        {
                            medicinePatyADO.Price = item.EXP_PRICE;
                            medicinePatyADO.Vat = item.EXP_VAT_RATIO * 100;
                            medicinePatyADO.Id = item.ID;
                            HIS_PATIENT_TYPE patientType = patientTypes.FirstOrDefault(o => o.ID == item.PATIENT_TYPE_ID);
                            medicinePatyADO.PatientTypeName = patientType.PATIENT_TYPE_NAME;
                        }
                    }
                    gridControlPaty.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

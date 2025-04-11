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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Controls.EditorLoader;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.Icd.ADO;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        private void LoadMediStockFromRoomId()
        {
            try
            {
                CommonParam param = new CommonParam();
                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.roomId);
                if (this.mediStock == null)
                    throw new Exception("mediStock is null");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataExpMestByEdit()
        {
            try
            {
                if (this.expMestId != null && this.expMestId.Value > 0)
                {
                    List<long> expMestIds = new List<long>();
                    expMestIds.Add(this.expMestId.Value);
                    List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = null;
                    List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = null;
                    V_HIS_EXP_MEST expMest = null;
                    List<Action> methods = new List<Action>();
                    methods.Add(() => { expMestMedicines = GetExpMestMedicineByExpMest(expMestIds); });
                    methods.Add(() => { expMestMaterials = GetExpMestMaterialByExpMest(expMestIds); });
                    methods.Add(() => { expMest = GetExpMestView(); });
                    ThreadCustomManager.MultipleThreadWithJoin(methods);

                    List<V_HIS_EXP_MEST> expMestList = new List<V_HIS_EXP_MEST>();
                    expMestList.Add(expMest);
                    //expMestDones_.ID = expMest.ID;
                    LoadDataFromExpMest(expMest, true);
                    LoadMediMateBeanByExpMestMediMate(expMestList, expMestMedicines, expMestMaterials, null, false);

                    moduleAction = GlobalDataStore.ModuleAction.EDIT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataExpMestByEditMulti(List<V_HIS_EXP_MEST> ExpMestList, HisExpMestForSaleSDO expMestForSaleSDO, bool IsSearch)
        {
            try
            {
                if (ExpMestList != null && ExpMestList.Count > 0)
                {
                    List<long> expMestIds = new List<long>();
                    expMestIds.AddRange(ExpMestList.Select(o => o.ID));
                    List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = null;
                    List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = null;
                    //List<Action> methods = new List<Action>();
                    //methods.Add(() => { expMestMedicines = GetExpMestMedicineByExpMest(expMestIds); });
                    //methods.Add(() => { expMestMaterials = GetExpMestMaterialByExpMest(expMestIds); });
                    //ThreadCustomManager.MultipleThreadWithJoin(methods);
                    //expMestMedicines =  GetExpMestMedicineByExpMest(expMestIds);
                    //expMestMaterials = GetExpMestMaterialByExpMest(expMestIds);
                    if (IsSearch && expMestForSaleSDO != null && expMestForSaleSDO.ViewMedicines != null && expMestForSaleSDO.ViewMedicines.Count > 0)
                    {
                        expMestMedicines = expMestForSaleSDO.ViewMedicines.Where(o => expMestIds.Contains(o.EXP_MEST_ID ?? -1)).ToList();
                    }
                    else
                    {
                        expMestMedicines = GetExpMestMedicineByExpMest(expMestIds);
                    }
                    if (IsSearch && expMestForSaleSDO != null && expMestForSaleSDO.ViewMaterials != null && expMestForSaleSDO.ViewMaterials.Count > 0)
                    {
                        expMestMaterials = expMestForSaleSDO.ViewMaterials.Where(o => expMestIds.Contains(o.EXP_MEST_ID ?? -1)).ToList();
                    }
                    else
                    {
                        expMestMaterials = GetExpMestMaterialByExpMest(expMestIds);
                    }
                    //expMestDones_.ID = ExpMestList[0].ID;
                    LoadDataFromExpMest(ExpMestList[0], true);
                    LoadMediMateBeanByExpMestMediMate(ExpMestList, expMestMedicines, expMestMaterials, expMestForSaleSDO, IsSearch);

                    this.resultSDO = new HisExpMestSaleListResultSDO();
                    this.resultSDO.ExpMestSdos = new List<HisExpMestSaleResultSDO>();
                    foreach (var item in ExpMestList)
                    {
                        HisExpMestSaleResultSDO hisExpMestResultSDO = new HisExpMestSaleResultSDO();
                        hisExpMestResultSDO.ExpMest = item;
                        this.resultSDO.ExpMestSdos.Add(hisExpMestResultSDO);
                    }

                    moduleAction = GlobalDataStore.ModuleAction.EDIT;
                    btnCancelExport.Enabled = true;
                    btnSavePrint.Enabled = false;
                    btnSave.Enabled = false;
                    SetLabelSave(this.moduleAction);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMediMateBeanByExpMestMediMate(List<V_HIS_EXP_MEST> expMestList, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials, HisExpMestForSaleSDO expMestForSaleSDO, bool IsSearch)
        {
            try
            {
                List<MediMateTypeADO> mediMateTypeADOs = new List<MediMateTypeADO>();
                if (expMestMedicines != null && expMestMedicines.Count > 0)
                {
                    expMestMedicineIds = expMestMedicines.Select(o => o.ID).ToList();

                    List<HIS_MEDICINE_BEAN> medicineBeans = new List<HIS_MEDICINE_BEAN>();
                    if (IsSearch && expMestForSaleSDO != null)
                    {
                        medicineBeans = expMestForSaleSDO.MedicineBeans.Where(o => expMestMedicineIds.Contains(o.EXP_MEST_MEDICINE_ID ?? -1)).ToList();
                    }
                    else
                    {
                        CommonParam param = new CommonParam();
                        HisMedicineBeanFilter medicineBeanFilter = new HisMedicineBeanFilter();
                        medicineBeanFilter.EXP_MEST_MEDICINE_IDs = expMestMedicineIds;
                        medicineBeans = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_BEAN>>(RequestUriStore.HIS_MEDICINE_BEAN__GET, ApiConsumers.MosConsumer, medicineBeanFilter, param);
                    }

                    foreach (var expMest in expMestList)
                    {
                        var expMestMedicineFilter = expMestMedicines.Where(o => o.EXP_MEST_ID == expMest.ID).ToList();
                        if (expMestMedicineFilter != null && expMestMedicineFilter.Count > 0)
                        {
                            var medicineBeanChild = medicineBeans.Where(o => expMestMedicineFilter.Select(p => p.ID).Contains(o.EXP_MEST_MEDICINE_ID ?? -1)).ToList();
                            mediMateTypeADOs.AddRange(from r in expMestMedicineFilter select new MediMateTypeADO(r, expMest, medicineBeanChild));
                        }
                    }
                }

                if (expMestMaterials != null && expMestMaterials.Count > 0)
                {
                    expMestMaterialIds = expMestMaterials.Select(o => o.ID).ToList();

                    List<HIS_MATERIAL_BEAN> materialBeans = new List<HIS_MATERIAL_BEAN>();
                    if (IsSearch && expMestForSaleSDO != null)
                    {
                        materialBeans = expMestForSaleSDO.MaterialBeans.Where(o => expMestMaterialIds.Contains(o.EXP_MEST_MATERIAL_ID ?? -1)).ToList();
                    }
                    else
                    {
                        CommonParam param = new CommonParam();
                        HisMaterialBeanFilter materialBeanFilter = new HisMaterialBeanFilter();
                        materialBeanFilter.EXP_MEST_MATERIAL_IDs = expMestMaterialIds;
                        materialBeans = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_BEAN>>(RequestUriStore.HIS_MATERIAL_BEAN__GET, ApiConsumers.MosConsumer, materialBeanFilter, param);
                    }

                    foreach (var expMest in expMestList)
                    {
                        var expMestMaterialFilter = expMestMaterials.Where(o => o.EXP_MEST_ID == expMest.ID).ToList();
                        if (expMestMaterialFilter != null && expMestMaterialFilter.Count > 0)
                        {
                            var materialBeanChild = materialBeans.Where(o => expMestMaterialFilter.Select(p => p.ID).Contains(o.EXP_MEST_MATERIAL_ID ?? -1)).ToList();
                            mediMateTypeADOs.AddRange(from r in expMestMaterialFilter select new MediMateTypeADO(r, expMest, materialBeanChild));
                        }
                    }
                }

                if (mediMateTypeADOs != null && mediMateTypeADOs.Count > 0)
                {
                    var dicMediMateAdoTmp = dicMediMateAdo;
                    dicMediMateAdo = new Dictionary<long, List<MediMateTypeADO>>();
                    foreach (var mediMateTypeADO in mediMateTypeADOs)
                    {
                        mediMateTypeADO.AVAILABLE_AMOUNT = (mediMateTypeADO.OLD_AMOUNT ?? 0);
                        if (mediMateTypeADO.IsMedicine)
                        {
                            HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == mediMateTypeADO.MEDI_MATE_TYPE_ID);
                            if (mediInStockSDO != null)
                            {
                                mediMateTypeADO.AVAILABLE_AMOUNT += (mediInStockSDO.AvailableAmount ?? 0);
                                if (spinAmount.Value > mediInStockSDO.AvailableAmount)
                                {
                                    mediMateTypeADO.IsExceedsAvailable = true;
                                }
                            }
                            else
                            {
                                if (!mediMateTypeADO.OLD_AMOUNT.HasValue)
                                    mediMateTypeADO.IsNotInStock = true;
                            }
                        }
                        if (mediMateTypeADO.IsMaterial)
                        {
                            HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == mediMateTypeADO.MEDI_MATE_TYPE_ID && !mediMateTypeADO.OLD_AMOUNT.HasValue);
                            if (mateInStockSDO != null)
                            {
                                mediMateTypeADO.AVAILABLE_AMOUNT += (mateInStockSDO.AvailableAmount ?? 0);
                                if (spinAmount.Value > mateInStockSDO.AvailableAmount)
                                {
                                    mediMateTypeADO.IsExceedsAvailable = true;
                                }
                            }
                            else
                            {
                                if (!mediMateTypeADO.OLD_AMOUNT.HasValue)
                                    mediMateTypeADO.IsNotInStock = true;
                            }
                        }
                        if (dicMediMateAdo.ContainsKey(mediMateTypeADO.MEDI_MATE_TYPE_ID))
                            dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID].Add(mediMateTypeADO);
                        else
                        {
                            dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID] = new List<MediMateTypeADO>();
                            if (dicMediMateAdoTmp != null && dicMediMateAdoTmp.Count > 0 && dicMediMateAdoTmp.ContainsKey(mediMateTypeADO.MEDI_MATE_TYPE_ID))
                                mediMateTypeADO.ClientSessionKey = dicMediMateAdoTmp[mediMateTypeADO.MEDI_MATE_TYPE_ID][0].ClientSessionKey;
                            else
                                mediMateTypeADO.ClientSessionKey = clientSessionKey;
                            dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID].Add(mediMateTypeADO);
                        }
                    }

                    //gridControlExpMestDetail.DataSource = dicMediMateAdo.Select(o => o.Value);
                }
                SetAvaliable0MediMateStock();
                //SetTotalPriceExpMestDetail();
                LoadDataToGridExpMestDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetAvaliable0MediMateStock()
        {
            try
            {
                if (dicMediMateAdo != null)
                {
                    var datas = dicMediMateAdo.Select(p => p.Value).SelectMany(p => p).Distinct().ToList();
                    List<HisMedicineTypeInStockSDO> medicineInStocks = new List<HisMedicineTypeInStockSDO>();
                    List<HisMaterialTypeInStockSDO> materialInStocks = new List<HisMaterialTypeInStockSDO>();
                    if (this.mediInStocks != null && this.mediInStocks.Count > 0)
                    {
                        medicineInStocks = this.mediInStocks.Where(o =>
                   datas.Select(p => p.MEDI_MATE_TYPE_ID).Contains(o.Id) || o.AvailableAmount > 0).ToList();
                    }

                    //gridControlMedicineInStock.DataSource = medicineInStocks;
                    if (this.mateInStocks != null && this.mateInStocks.Count > 0)
                    {
                        materialInStocks = this.mateInStocks.Where(o =>
                       datas.Select(p => p.MEDI_MATE_TYPE_ID).Contains(o.Id) || o.AvailableAmount > 0).ToList();
                    }

                    //gridControlMaterialInStock.DataSource = materialInStocks;

                    SetDataGridMediMaty(medicineInStocks, materialInStocks);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataFromExpMest(V_HIS_EXP_MEST expMest, bool _IsShow)
        {
            try
            {
                if (expMest != null)
                {
                    CommonParam param = new CommonParam();
                    HisExpMestFilter _filter = new HisExpMestFilter();
                    _filter.ID = expMest.ID;

                    var result = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, _filter, param).FirstOrDefault();
                    //expMestDones = result.ID;
                    // expMestDones_ = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, _filter, param).FirstOrDefault();
                    checkIsVisitor.Enabled = false;
                    if (_IsShow && expMest.PRESCRIPTION_ID.HasValue)
                    {
                        txtPrescriptionCode.Text = expMest.TDL_SERVICE_REQ_CODE;
                    }
                    else if (_IsShow)
                    {
                        checkIsVisitor.CheckState = CheckState.Checked;
                    }

                    txtVirPatientName.Text = expMest.TDL_PATIENT_NAME;
                    cboPatientType.EditValue = expMest.SALE_PATIENT_TYPE_ID;

                    if (expMest.TDL_PATIENT_GENDER_ID.HasValue)
                    {
                        cboGender.EditValue = expMest.TDL_PATIENT_GENDER_ID.Value;
                    }

                    if (expMest.TDL_PATIENT_DOB.HasValue)
                    {
                        //dtPatientDob.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(expMest.TDL_PATIENT_DOB.Value) ?? DateTime.Now;
                        if (expMest.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        {
                            txtPatientDob.Text = expMest.TDL_PATIENT_DOB.Value.ToString().Substring(0, 4);
                        }
                        else
                        {
                            txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(expMest.TDL_PATIENT_DOB.Value);
                        }
                    }

                    txtAddress.Text = expMest.TDL_PATIENT_ADDRESS;
                    if (expMest.DISCOUNT.HasValue && expMest.DISCOUNT.Value > 0)
                    {
                        spinAmount.Value = expMest.DISCOUNT.Value * 100;
                    }
                    //txtIcdCode.Text = result.ICD_CODE;
                    //var icdId = BackendDataWorker.Get<HIS_ICD>().Where(o => o.ICD_CODE == result.ICD_CODE).FirstOrDefault();
                    //if (icdId != null)
                    //{
                    //    cboIcds.EditValue = icdId.ID;
                    //}
                    IcdInputADO inputIcd = new IcdInputADO();
                    inputIcd.ICD_NAME = result.ICD_NAME;
                    inputIcd.ICD_CODE = result.ICD_CODE;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputIcd), inputIcd));
                    Inventec.Common.Logging.LogSystem.Debug("ucIcd " + ucIcd);
                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, inputIcd);
                    }
                    txtSubIcdCode.Text = result.ICD_SUB_CODE;
                    txtIcd.Text = result.ICD_TEXT;
                    txtLoginName.Text = expMest.TDL_PRESCRIPTION_REQ_LOGINNAME;
                    txtPresUser.Text = expMest.TDL_PRESCRIPTION_REQ_USERNAME;
                    if (expMest.TDL_INTRUCTION_TIME.HasValue)
                    {
                        dtIntructionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(expMest.TDL_INTRUCTION_TIME.Value).Value;
                    }

                    lblExpMestCode.Text = _IsShow ? expMest.EXP_MEST_CODE : "";//trong trường hợp nhập mã điều trị thì không fill thông tin vào ô mã y lệnh
                    SetControlByExpMest(expMest.EXP_MEST_CODE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_EXP_MEST GetExpMestView()
        {
            V_HIS_EXP_MEST result = null;
            try
            {
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this.expMestId;
                result = new BackendAdapter(new CommonParam())
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, new CommonParam()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<V_HIS_EXP_MEST_MEDICINE> GetExpMestMedicineByExpMest(List<long> expMestIds)
        {
            List<V_HIS_EXP_MEST_MEDICINE> result = null;
            try
            {
                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_IDs = expMestIds;
                result = new BackendAdapter(new CommonParam())
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<V_HIS_EXP_MEST_MATERIAL> GetExpMestMaterialByExpMest(List<long> expMestIds)
        {
            List<V_HIS_EXP_MEST_MATERIAL> result = null;
            try
            {
                HisExpMestMaterialViewFilter medicineFilter = new HisExpMestMaterialViewFilter();
                medicineFilter.EXP_MEST_IDs = expMestIds;
                result = new BackendAdapter(new CommonParam())
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadDataToComboUser()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.InitComboUser(); }));
                }
                else
                {
                    this.InitComboUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load data Medi va Mate co su dung Thread Join
        /// </summary>
        private void LoadMediMateFromMediStock()
        {
            try
            {
                List<Action> methods = new List<Action>();
                methods.Add(LoadMedicineTypeFromStock);
                methods.Add(LoadMaterialTypeFromStock);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataGridMediMaty(List<HisMedicineTypeInStockSDO> _medis, List<HisMaterialTypeInStockSDO> matys)
        {
            try
            {
                List<MediMateTypeADO> _Datas = new List<MediMateTypeADO>();
                gridControlMediMaty.BeginUpdate();
                if (_medis != null && _medis.Count > 0)
                {
                    _Datas.AddRange((from s in _medis select new MediMateTypeADO(s)).ToList());
                }
                if (matys != null && matys.Count > 0)
                {
                    _Datas.AddRange((from s in matys select new MediMateTypeADO(s)).ToList());
                }
                gridControlMediMaty.DataSource = _Datas;
                gridControlMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMedicineTypeFromStock()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineTypeStockViewFilter medicineFilter = new HisMedicineTypeStockViewFilter();
                medicineFilter.MEDI_STOCK_ID = mediStock.ID;
                medicineFilter.IS_AVAILABLE = true;
                medicineFilter.IS_ACTIVE = true;
                this.mediInStocks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisMedicineTypeInStockSDO>>("api/HisMedicineType/GetInStockMedicineType", ApiConsumers.MosConsumer, medicineFilter, param);
                //.Where(o => o.IsBusiness == 1)
                //.ToList();
                if (this.mediInStocks != null && this.mediInStocks.Count > 0)
                {
                    this.mediInStocks = this.mediInStocks.Where(o => o.IsBusiness == 1).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMaterialTypeFromStock()
        {
            try
            {
                HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                mateFilter.MEDI_STOCK_ID = mediStock.ID;
                mateFilter.IS_AVAILABLE = true;
                mateFilter.IS_ACTIVE = true;
                this.mateInStocks = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>("api/HisMaterialType/GetInStockMaterialType", ApiConsumers.MosConsumer, mateFilter, null);
                //.Where(o => o.IsBusiness == 1)
                //.ToList();
                if (this.mateInStocks != null && this.mateInStocks.Count > 0)
                {
                    this.mateInStocks = this.mateInStocks.Where(o => o.IsBusiness == 1).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Grid Medi & Mate
        /// Load Combo PatientType
        /// </summary>
        private void LoadDataToControl()
        {
            try
            {
                if (this.mediStock != null)
                {
                    txtExpMediStock.Text = this.mediStock.MEDI_STOCK_NAME;
                }

                LoadDataToCboPatientType();
                LoadDataToCboGender();
                LoadDataToComboPayForm();

                //Load patient from cfg
                string patientTypeCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.PatientTypeCodeDefault);
                if (!String.IsNullOrEmpty(patientTypeCode))
                {
                    this.patientTypeConfig = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == patientTypeCode);
                    if (patientTypeConfig != null)
                        cboPatientType.EditValue = patientTypeConfig.ID;
                }
                spinDiscount.EditValue = null;
                spinDiscountRatio.EditValue = null;
                checkImpExpPrice.Enabled = this.currentMediMate != null ? true : false;
                dtIntructionTime.DateTime = DateTime.Now;
                spinDayNum.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboPatientType()
        {
            try
            {
                //thì chỉ hiển thị ra các đối tượng có trong chính sách giá/vật tư (medicine_paty và material_paty)
                //--> Khi đó, với danh mục thuốc/vật tư của nhà thuốc, người dùng chỉ khai báo chính sách giá cho đối tượng mua thuốc
                List<long> patientTypeIds = new List<long>();
                List<HIS_MEDICINE_PATY> medicinePatys = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_PATY>();
                if (medicinePatys != null && medicinePatys.Count > 0)
                {
                    patientTypeIds.AddRange(medicinePatys.Select(o => o.PATIENT_TYPE_ID));
                }

                List<HIS_MATERIAL_PATY> materialPatys = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_PATY>();
                if (materialPatys != null && materialPatys.Count > 0)
                {
                    patientTypeIds.AddRange(materialPatys.Select(o => o.PATIENT_TYPE_ID));
                }
                //Bỏ trùng
                patientTypeIds = patientTypeIds.Distinct().ToList();

                cboPatientType.Properties.DataSource = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => patientTypeIds.Contains(o.ID)).ToList();
                cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ValueMember = "ID";
                cboPatientType.Properties.ForceInitialize();
                cboPatientType.Properties.Columns.Clear();
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_CODE", "", 50));
                cboPatientType.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_NAME", "", 120));
                cboPatientType.Properties.ShowHeader = false;
                cboPatientType.Properties.ImmediatePopup = true;
                cboPatientType.Properties.DropDownRows = 10;
                cboPatientType.Properties.PopupWidth = 170;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboGender()
        {
            try
            {
                var gender = BackendDataWorker.Get<HIS_GENDER>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 10, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 110);
                ControlEditorLoader.Load(cboGender, gender, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboCashierRoom()
        {
            try
            {
                var userRooms = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId());
                var cashierRooms = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && userRooms != null && userRooms.Any(a => a.ROOM_ID == o.ROOM_ID))
                    .OrderBy(p => p.CASHIER_ROOM_NAME).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_NAME", "", 180, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CASHIER_ROOM_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBillCashierRoom, cashierRooms, controlEditorADO);
                if (cboBillCashierRoom.EditValue == null && cashierRooms != null && cashierRooms.Count > 0)
                {
                    cboBillCashierRoom.EditValue = cashierRooms[0].ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboBillAccountBook()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 180, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBillAccountBook, null, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboPayForm()
        {
            try
            {
                List<HIS_PAY_FORM> lData = lData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 110);
                ControlEditorLoader.Load(cboPayForm, lData, controlEditorADO);

                cboPayForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM;
                var payform = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == (long)cboPayForm.EditValue);
                if (payform != null)
                {
                    CheckPayFormTienMatChuyenKhoan(payform);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Create SessionKey
        /// </summary>
        /// <param name="length"> Length Character</param>
        /// <returns></returns>
        private string CreateSessionKey(int length)
        {
            string result = null;
            try
            {
                Random rd = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                result = new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[rd.Next(s.Length)]).ToArray());
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadDataToGridExpMestDetail()
        {
            try
            {
                if (dicMediMateAdo != null)
                {
                    List<MediMateTypeADO> dataSources = dicMediMateAdo.Select(p => p.Value).SelectMany(p => p).Distinct().ToList();
                    // List<MediMateTypeADO> dataSources = (List<MediMateTypeADO>)dicMediMateAdo.Select(o => o.Value).ToList();

                    SetTotalPriceExpMestDetail();
                    string dem = "";

                    var check = dataSources.Select(o => o.EXP_MEST_ID).Distinct().ToList();
                    foreach (var item in check)
                    {
                        dem += dataSources.Where(o => o.EXP_MEST_ID == item).FirstOrDefault().PresNumber + ";";
                    }
                    dem = dem.EndsWith(";") ? dem.Substring(0, dem.Length - 1) : dem;

                    lblPresNumber.Text = dem;
                    ProcessSetDataTrees(dataSources);
                    SetTotalPriceExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTotalPriceExpMestDetail()
        {
            try
            {
                decimal? price = 0;
                decimal? totalPriceDiscount = 0;
                if (dicMediMateAdo != null)
                {
                    var datas = dicMediMateAdo.Select(p => p.Value).SelectMany(p => p).Distinct().ToList();
                    DiscountDisplayProcess(this.discountFocus, this.discountRatioFocus, spinDiscount, spinDiscountRatio, datas.Sum(p => p.TOTAL_PRICE ?? 0));
                    decimal? totalPrice = datas.Sum(p => p.TOTAL_PRICE ?? 0);// dicMediMateAdo.Sum(o => (o.Value.TOTAL_PRICE ?? 0));

                    List<PriceDetailsADO> lstADO = new List<PriceDetailsADO>();

                    var dataTrees = dicMediMateAdo.Select(o => o.Value).ToList();
                    var dataCoverts = dataTrees.SelectMany(p => p).Distinct().OrderByDescending(o => o.TOTAL_PRICE).ToList();
                    var dataGroups = dataCoverts.GroupBy(p => p.SERVICE_REQ_CODE).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        PriceDetailsADO ado = new PriceDetailsADO();
                        decimal? x = 0;
                        foreach (var i in item)
                        {
                            if (spinDiscountRatio.EditValue != null)
                            {
                                x += (i.TOTAL_PRICE ?? 0) - (i.TOTAL_PRICE ?? 0) * spinDiscountRatio.Value / 100;
                            }
                            else
                            {
                                x += i.TOTAL_PRICE ?? 0;
                            }
                        }
                        ado.PRICE = CalculReceivable(x ?? 0) ?? 0;
                        lstADO.Add(ado);
                    }
                    if (lstADO != null && lstADO.Count > 0)
                    {
                        price = CalculReceivable(lstADO.Sum(p => p.PRICE));
                    }
                    if (spinDiscountRatio.EditValue != null)
                    {
                        totalPriceDiscount = totalPrice * spinDiscountRatio.Value / 100;
                    }
                    totalPayPrice = totalPrice - totalPriceDiscount;
                    lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice ?? 0, ConfigApplications.NumberSeperator);
                    //lblTotalDiscountPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPriceDiscount ?? 0, ConfigApplications.NumberSeperator);
                    lblPayPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPayPrice ?? 0, ConfigApplications.NumberSeperator);
                }
                else
                {
                    totalPayPrice = 0;
                }

                this.CalculTotalReceivable((price ?? 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculTotalReceivable(decimal totalPayPrice)
        {
            try
            {
                this.totalReceivable = 0;
                if (chkCreateBill.Checked && chkRoundPrice.Checked && totalPayPrice > 0)
                {
                    int b = (int)spinBaseValue.Value;
                    if (b > 0)
                    {
                        int n = b.ToString().Length;
                        int y = (int)(totalPayPrice % (int)(Math.Pow(10, n)));
                        if (y >= b)
                        {
                            this.totalReceivable = ((int)(totalPayPrice / (int)Math.Pow(10, n)) + 1) * (int)Math.Pow(10, n);
                        }
                        else
                        {
                            this.totalReceivable = ((int)(totalPayPrice / (int)Math.Pow(10, n))) * (int)Math.Pow(10, n);
                        }
                    }
                    else
                    {
                        this.totalReceivable = totalPayPrice;
                    }
                }

                lblTotalReceivable.Text = Inventec.Common.Number.Convert.NumberToString(this.totalReceivable, ConfigApplications.NumberSeperator);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlAfterAddClick()
        {
            try
            {
                spinExpPrice.EditValue = null;
                txtTutorial.Text = "";
                spinDiscountDetail.EditValue = null;
                spinDiscountDetailRatio.EditValue = null;
                btnAdd.Enabled = false;
                spinExpVatRatio.EditValue = null;
                txtNote.Text = "";
                spinAmount.EditValue = "";
                this.currentMediMate = null;
                this.discountDetailFocus = false;
                this.discountDetailRatioFocus = false;
                checkImpExpPrice.Enabled = false;
                checkImpExpPrice.CheckState = CheckState.Unchecked;
                this.Action = GlobalDataStore.ActionAdd;

                if (txtMediMatyForPrescription.Enabled)
                {
                    txtMediMatyForPrescription.Focus();
                    txtMediMatyForPrescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetControl()
        {
            try
            {
                checkImpExpPrice.CheckState = CheckState.Unchecked;
                spinExpPrice.EditValue = null;
                txtTutorial.Text = "";
                spinDiscountDetail.EditValue = null;
                spinDiscountDetailRatio.EditValue = null;
                btnAdd.Enabled = false;
                spinExpVatRatio.EditValue = null;
                txtNote.Text = "";
                spinAmount.EditValue = "";
                this.currentMediMate = null;
                this.discountDetailFocus = false;
                this.discountDetailRatioFocus = false;
                checkImpExpPrice.Enabled = false;
                checkImpExpPrice.CheckState = CheckState.Unchecked;
                this.Action = GlobalDataStore.ActionAdd;
                //if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ExpmestSaleCreate__CashierRoomId <= 0)
                //{
                //    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ExpmestSaleCreate__CashierRoomId = this.taiQuayTrongGioID;
                //}
                //cboCashierRoom.EditValue = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ExpmestSaleCreate__CashierRoomId;
                spinTransferAmount.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetAllControl()
        {
            try
            {
                if (btnQr.Visible)
                    btnQr.Enabled = false;
                this.currentMediMateFocus = null;
                cboPatientType.EditValue = null;
                if (this.patientTypeConfig != null)
                {
                    cboPatientType.EditValue = patientTypeConfig.ID;
                }

                this.moduleAction = GlobalDataStore.ModuleAction.ADD;
                checkIsVisitor.CheckState = CheckState.Unchecked;
                txtPrescriptionCode.Text = "";
                txtTreatmentCode.Text = "";
                txtVirPatientName.Text = "";
                cboGender.EditValue = null;
                txtMediMatyForPrescription.Text = "";
                txtPatientDob.EditValue = null;
                dtPatientDob.EditValue = null;
                txtAge.Text = "";
                cboAge.EditValue = null;
                txtAddress.Text = "";
                txtDescription.Text = "";
                this.serviceReq = null;
                this.expMestId = null;
                this.Patient = null;
                lblTotalPrice.Text = "";
                lblPayPrice.Text = "";
                lblPresNumber.Text = "";
                lblTotalReceivable.Text = "";
                txtPatientCode.Text = "";
                txtPatientPhone.Text = "";
                txtEmail.Text = "";
                treeListMediMate.DataSource = null;
                this.discountFocus = false;
                this.discountRatioFocus = false;
                this.discountDetailFocus = false;
                this.discountDetailRatioFocus = false;
                if (dicMediMateAdo != null)
                    dicMediMateAdo.Clear();
                treeListResult.DataSource = null;
                this.expMestMaterialIds = null;
                this.expMestMedicineIds = null;
                checkIsVisitor.Enabled = true;
                txtPresUser.Text = "";
                txtLoginName.Text = "";
                btnSaleBill.Enabled = true;
                btnDebt.Enabled = false;
                btnAdd.Enabled = true;
                btnSave.Enabled = true;
                btnSavePrint.Enabled = true;
                cboTHX.EditValue = null;
                txtMaTHX.Text = "";
                lblTransactionCode.Text = "";
                btnCancelExport.Enabled = false;
                this.icdProcessor.Reload(ucIcd, null);
                this.icdProcessor.ResetValidate(ucIcd);
                this.icdProcessor.SetRequired(ucIcd, false);
                txtIcd.Text = "";
                //txtIcdCode.Text = "";
                txtSubIcdCode.Text = "";
                //cboCashierRoom.EditValue = GlobalVariables.ExpmestSaleCreate__CashierRoomId;
                //this.expMestResult = new HisExpMestResultSDO();
                //this.expMestResult.ExpMest = new HIS_EXP_MEST();
                if (chkCreateBill.Checked && cboBillAccountBook.EditValue != null)
                {
                    spinBillNumOrder.Value = setDataToDicNumOrderInAccountBook(this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboBillAccountBook.EditValue)));
                }
                else
                {
                    spinBillNumOrder.Text = "";
                }
                if (cboPayForm.EditValue == null)
                    cboPayForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM;
                spinTransferAmount.EditValue = null;
                this.SetLabelSave(GlobalDataStore.ModuleAction.ADD);
                LoadMediMateFromMediStock();
                LoadDataToControl();
                ResetControlAfterAddClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableByCheckIsVisitor()
        {
            try
            {
                if (checkIsVisitor.Checked)
                {
                    txtPrescriptionCode.Enabled = false;
                    txtTreatmentCode.Enabled = false;
                    btnSearchPres.Enabled = false;
                    txtVirPatientName.Enabled = true;
                    cboGender.Enabled = true;
                    txtPatientDob.Enabled = true;
                    txtAddress.Enabled = true;
                    txtLoginName.Enabled = true;
                    txtPresUser.Enabled = true;
                    //txtDescription.Enabled = true;
                    txtMaTHX.Enabled = true;
                    cboTHX.Enabled = true;
                    txtPatientCode.Enabled = true;
                    txtPatientPhone.Enabled = true;
                    txtEmail.Enabled = true;
                }
                else
                {
                    txtPrescriptionCode.Enabled = true;
                    txtTreatmentCode.Enabled = true;
                    btnSearchPres.Enabled = true;
                    txtVirPatientName.Enabled = false;
                    cboGender.Enabled = false;
                    txtPatientDob.Enabled = false;
                    txtAddress.Enabled = false;
                    txtLoginName.Enabled = false;
                    txtPresUser.Enabled = false;
                    //txtDescription.Enabled = false;
                    txtMaTHX.Enabled = false;
                    cboTHX.Enabled = false;
                    txtPatientCode.Enabled = false;
                    txtPatientPhone.Enabled = false;
                    txtEmail.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanMedicineAll(Dictionary<long, MediMateTypeADO> dic, string ssKey = null)
        {
            try
            {
                if (dic != null)
                {
                    List<MediMateTypeADO> medicines = dic.Select(o => o.Value).Where(o => o.IsMedicine == true && o.IsExceedsAvailable == false && o.IsNotInStock == false).ToList();
                    if (medicines != null && medicines.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();
                        foreach (var medicine in medicines)
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = ssKey;
                            takeBeanSDO.BeanIds = null;
                            takeBeanSDO.Amount = medicine.EXP_AMOUNT ?? 0;
                            takeBeanSDO.MediStockId = this.mediStock.ID;
                            takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                            takeBeanSDO.TypeId = medicine.MEDI_MATE_TYPE_ID;
                            takeBeanSDOs.Add(takeBeanSDO);
                        }
                        Inventec.Common.Logging.LogSystem.Debug("INPUT___TakeMedicineBeanListResultSDO" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));
                        List<TakeMedicineBeanListResultSDO> takeMedicines = new BackendAdapter(param)
                     .Post<List<TakeMedicineBeanListResultSDO>>(RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEANLIST, ApiConsumers.MosConsumer, takeBeanSDOs, param);
                        Inventec.Common.Logging.LogSystem.Debug("OUTPUT___TakeMedicineBeanListResultSDO" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeMedicines), takeMedicines));

                        if (takeMedicines == null || takeMedicines.Count == 0)
                        {
                            param.Messages[0] = "(Thuốc) " + param.Messages[0];
                            MessageManager.Show(param, false);
                        }
                        foreach (var takeMedicine in takeMedicines)
                        {
                            if (takeMedicine.Request != null && takeMedicine.Result != null && dic[takeMedicine.Request.TypeId] != null)
                            {
                                List<HIS_MEDICINE_BEAN> medicineBeans = takeMedicine.Result.ToList();
                                var medicineBeanGroups = medicineBeans.GroupBy(o => new { o.TDL_MEDICINE_TYPE_ID });
                                foreach (var g in medicineBeanGroups)
                                {
                                    List<long> beanIds = g != null ? g.Select(o => o.ID).ToList() : null;
                                    dic[takeMedicine.Request.TypeId].BeanIds = beanIds;
                                    dic[takeMedicine.Request.TypeId].TOTAL_PRICE = g.Sum(s => (s.AMOUNT * s.TDL_MEDICINE_IMP_PRICE * (1 + s.TDL_MEDICINE_IMP_VAT_RATIO)));
                                    dic[takeMedicine.Request.TypeId].EXP_PRICE = Math.Round((dic[takeMedicine.Request.TypeId].TOTAL_PRICE ?? 0) / dic[takeMedicine.Request.TypeId].EXP_AMOUNT ?? 0, 4);
                                    dic[takeMedicine.Request.TypeId].EXP_VAT_RATIO = 0;
                                    if (dicMediMateAdo.ContainsKey(takeMedicine.Request.TypeId))
                                    {
                                        dicMediMateAdo[takeMedicine.Request.TypeId].Add(dic[takeMedicine.Request.TypeId]);
                                    }
                                    else
                                    {
                                        dicMediMateAdo[takeMedicine.Request.TypeId] = new List<MediMateTypeADO>();
                                        dicMediMateAdo[takeMedicine.Request.TypeId].Add(dic[takeMedicine.Request.TypeId]);
                                    }
                                }
                            }
                        }
                    }

                    //
                    List<MediMateTypeADO> medicineIsExceedsAvailableOrIsNotInStocks = dic.Select(o => o.Value).Where(o => o.IsMedicine == true && (o.IsExceedsAvailable || o.IsNotInStock)).ToList();
                    if (medicineIsExceedsAvailableOrIsNotInStocks != null && medicineIsExceedsAvailableOrIsNotInStocks.Count > 0)
                    {
                        foreach (var medicineIsExceedsAvailableOrIsNotInStock in medicineIsExceedsAvailableOrIsNotInStocks)
                        {
                            if (dicMediMateAdo.ContainsKey(medicineIsExceedsAvailableOrIsNotInStock.MEDI_MATE_TYPE_ID))
                            {
                                dicMediMateAdo[medicineIsExceedsAvailableOrIsNotInStock.MEDI_MATE_TYPE_ID].Add(medicineIsExceedsAvailableOrIsNotInStock);
                            }
                            else
                            {
                                dicMediMateAdo[medicineIsExceedsAvailableOrIsNotInStock.MEDI_MATE_TYPE_ID] = new List<MediMateTypeADO>();
                                dicMediMateAdo[medicineIsExceedsAvailableOrIsNotInStock.MEDI_MATE_TYPE_ID].Add(medicineIsExceedsAvailableOrIsNotInStock);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanMaterialAll(Dictionary<long, MediMateTypeADO> dic, string ssKey)
        {
            try
            {
                if (dic != null)
                {
                    List<MediMateTypeADO> materials = dic.Select(o => o.Value).Where(o => o.IsMaterial == true && o.IsExceedsAvailable == false && o.IsNotInStock == false).ToList();
                    if (materials != null && materials.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        List<TakeBeanSDO> takeBeanSDOs = new List<TakeBeanSDO>();

                        foreach (var material in materials)
                        {
                            TakeBeanSDO takeBeanSDO = new TakeBeanSDO();
                            takeBeanSDO.ClientSessionKey = ssKey;

                            Inventec.Common.Logging.LogSystem.Error("ssKEY__MATERIAL_:" + material.SERVICE_REQ_CODE + "___" + ssKey);
                            takeBeanSDO.BeanIds = null;
                            takeBeanSDO.Amount = material.EXP_AMOUNT ?? 0;
                            takeBeanSDO.MediStockId = this.mediStock.ID;
                            takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                            takeBeanSDO.TypeId = material.MEDI_MATE_TYPE_ID;
                            takeBeanSDOs.Add(takeBeanSDO);
                        }
                        Inventec.Common.Logging.LogSystem.Debug("INPUT___TakeMaterialBeanListResultSDO" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeBeanSDOs), takeBeanSDOs));

                        List<TakeMaterialBeanListResultSDO> takeMaterials = new BackendAdapter(param)
                    .Post<List<TakeMaterialBeanListResultSDO>>(RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEANLIST, ApiConsumers.MosConsumer, takeBeanSDOs, param);
                        Inventec.Common.Logging.LogSystem.Debug("OUTPUT___TakeMaterialBeanListResultSDO" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => takeMaterials), takeMaterials));
                        if (takeMaterials == null || takeMaterials.Count == 0)
                        {
                            param.Messages[0] = "(Vật tư) " + param.Messages[0];
                            MessageManager.Show(param, false);
                        }

                        foreach (var takeMaterial in takeMaterials)
                        {
                            if (takeMaterial.Request != null && takeMaterial.Result != null)
                            {
                                List<HIS_MATERIAL_BEAN> materialBeans = takeMaterial.Result.ToList();
                                var materialBeanGroups = materialBeans.GroupBy(o => new { o.TDL_MATERIAL_TYPE_ID });
                                foreach (var g in materialBeanGroups)
                                {
                                    List<long> beanIds = g != null ? g.Select(o => o.ID).ToList() : null;
                                    dic[takeMaterial.Request.TypeId].BeanIds = beanIds;
                                    dic[takeMaterial.Request.TypeId].TOTAL_PRICE = g.Sum(s => (s.AMOUNT * s.TDL_MATERIAL_IMP_PRICE * (1 + s.TDL_MATERIAL_IMP_VAT_RATIO)));
                                    dic[takeMaterial.Request.TypeId].EXP_PRICE = Math.Round((dic[takeMaterial.Request.TypeId].TOTAL_PRICE ?? 0) / dic[takeMaterial.Request.TypeId].EXP_AMOUNT ?? 0, 4);
                                    dic[takeMaterial.Request.TypeId].EXP_VAT_RATIO = 0;
                                    if (dicMediMateAdo.ContainsKey(takeMaterial.Request.TypeId))
                                    {
                                        dicMediMateAdo[takeMaterial.Request.TypeId].Add(dic[takeMaterial.Request.TypeId]);
                                    }
                                    else
                                    {
                                        dicMediMateAdo[takeMaterial.Request.TypeId] = new List<MediMateTypeADO>();
                                        dicMediMateAdo[takeMaterial.Request.TypeId].Add(dic[takeMaterial.Request.TypeId]);
                                    }
                                }
                            }
                        }
                    }

                    List<MediMateTypeADO> matecineIsExceedsAvailableOrIsNotInStocks = dic.Select(o => o.Value).Where(o => o.IsMaterial == true && (o.IsExceedsAvailable || o.IsNotInStock)).ToList();
                    if (matecineIsExceedsAvailableOrIsNotInStocks != null && matecineIsExceedsAvailableOrIsNotInStocks.Count > 0)
                    {
                        foreach (var matecineIsExceedsAvailableOrIsNotInStock in matecineIsExceedsAvailableOrIsNotInStocks)
                        {
                            if (dicMediMateAdo.ContainsKey(matecineIsExceedsAvailableOrIsNotInStock.MEDI_MATE_TYPE_ID))
                            {
                                dicMediMateAdo[matecineIsExceedsAvailableOrIsNotInStock.MEDI_MATE_TYPE_ID].Add(matecineIsExceedsAvailableOrIsNotInStock);
                            }
                            else
                            {
                                dicMediMateAdo[matecineIsExceedsAvailableOrIsNotInStock.MEDI_MATE_TYPE_ID] = new List<MediMateTypeADO>();
                                dicMediMateAdo[matecineIsExceedsAvailableOrIsNotInStock.MEDI_MATE_TYPE_ID].Add(matecineIsExceedsAvailableOrIsNotInStock);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckAvaliableAndMedicineInStock(long medicineTypeId, string medicineTypeName, decimal amount, ref bool next, ref bool reject)
        {
            try
            {
                next = true;
                reject = false;

                if (this.mediInStocks == null)
                {
                    next = false;
                    return;
                }

                HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == medicineTypeId);
                if (mediInStockSDO == null)
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show(String.Format("Thuốc {0} không tồn tại trong kho. Bạn có muốn tiếp tục", medicineTypeName), "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (myResult != DialogResult.OK)
                    {
                        next = false;
                        return;
                    }
                    else
                    {
                        next = true;
                        reject = true;
                    }
                }
                else
                {
                    if (mediInStockSDO.AvailableAmount < amount)
                    {
                        DialogResult myResult;
                        myResult = MessageBox.Show(String.Format("Thuốc {0} không đủ số lượng khả dụng trong kho. Bạn có muốn tiếp tục", medicineTypeName), "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (myResult != DialogResult.OK)
                        {
                            next = false;
                            return;
                        }
                        else
                        {
                            next = true;
                            reject = true;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                next = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckAvaliableAndMaterialInStock(long medicineTypeId, string medicineTypeName, decimal amount, ref bool next, ref bool reject)
        {
            try
            {
                next = true;
                reject = false;

                if (this.mateInStocks == null)
                {
                    next = false;
                    return;
                }

                HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == medicineTypeId);
                if (mateInStockSDO == null)
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show(String.Format("Vật tư {0} không tồn tại trong kho. Bạn có muốn tiếp tục", medicineTypeName), "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (myResult != DialogResult.OK)
                    {
                        next = false;
                        return;
                    }
                    else
                    {
                        next = true;
                        reject = true;
                    }
                }
                else
                {
                    if (mateInStockSDO.AvailableAmount < amount)
                    {
                        DialogResult myResult;
                        myResult = MessageBox.Show(String.Format("Vật tư {0} không đủ số lượng khả dụng trong kho. Bạn có muốn tiếp tục", medicineTypeName), "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (myResult != DialogResult.OK)
                        {
                            next = false;
                            return;
                        }
                        else
                        {
                            next = true;
                            reject = true;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                next = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientInfoFromPrescription(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                if (serviceReq != null)
                {
                    txtPatientCode.Text = serviceReq.TDL_PATIENT_CODE;
                    txtPatientPhone.Text = serviceReq.TDL_PATIENT_PHONE;
                    txtVirPatientName.Text = serviceReq.TDL_PATIENT_NAME;
                    txtAddress.Text = serviceReq.TDL_PATIENT_ADDRESS;
                    cboGender.EditValue = serviceReq.TDL_PATIENT_GENDER_ID;
                    if (serviceReq.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtPatientDob.Text = serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.TDL_PATIENT_DOB);
                    }

                    txtLoginName.Text = serviceReq.REQUEST_LOGINNAME;
                    txtPresUser.Text = serviceReq.REQUEST_USERNAME;
                }
                else
                {
                    txtPatientCode.Text = "";
                    txtPatientPhone.Text = "";
                    txtVirPatientName.Text = "";
                    txtAddress.Text = "";
                    cboGender.EditValue = null;
                    txtPatientDob.EditValue = null;
                    txtLoginName.Text = "";
                    txtPresUser.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckStockIsBusiness()
        {
            try
            {
                if (this.mediStock != null && this.mediStock.IS_BUSINESS != 1)
                {
                    cboPatientType.Enabled = false;
                    checkIsVisitor.Enabled = false;
                    txtPrescriptionCode.Enabled = false;
                    txtVirPatientName.Enabled = false;
                    cboGender.Enabled = false;
                    txtPatientDob.Enabled = false;
                    txtAddress.Enabled = false;
                    txtMaTHX.Enabled = false;
                    cboTHX.Enabled = false;
                    txtDescription.Enabled = false;
                    txtMediMatyForPrescription.Enabled = false;
                    treeListMediMate.Enabled = false;
                    treeListResult.Enabled = false;
                    spinAmount.Enabled = false;
                    checkImpExpPrice.Enabled = false;
                    spinExpPrice.Enabled = false;
                    spinExpVatRatio.Enabled = false;
                    spinDiscount.Enabled = false;
                    spinTransferAmount.Enabled = false;
                    txtTutorial.Enabled = false;
                    txtNote.Enabled = false;
                    btnAdd.Enabled = false;
                    btnSavePrint.Enabled = false;
                    btnSave.Enabled = false;
                    btnNew.Enabled = false;
                    btnSaleBill.Enabled = false;
                    btnDebt.Enabled = false;
                    ddBtnPrint.Enabled = false;
                    spinDiscountRatio.Enabled = false;
                    spinDiscountDetailRatio.Enabled = false;
                    spinDiscountDetail.Enabled = false;
                    lblTotalPrice.Enabled = false;
                    lblPresNumber.Enabled = false;
                    //cboCashierRoom.EditValue = null;
                    MessageBox.Show(String.Format("Kho {0} không phải là kho kinh doanh", this.mediStock.MEDI_STOCK_NAME), "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load người chỉ định
        private async Task InitComboUser()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                //if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                //{
                //    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                //}
                //else
                //{
                //    CommonParam paramCommon = new CommonParam();
                //    dynamic filter = new System.Dynamic.ExpandoObject();
                //    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                //    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                //}
                datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                //var listLoginName = BackendDataWorker.Get<HIS_EMPLOYEE>().Select(o => o.LOGINNAME).ToList();

                var listLoginName = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(d => d.IS_DOCTOR == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => o.LOGINNAME).ToList();
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                if (listLoginName != null && listLoginName.Count > 0)
                {
                    datas = datas.Where(o => listLoginName.Contains(o.LOGINNAME)).ToList();
                }
                this.RebuildControlContainerUser(datas);

                Control topParent = this;
                while (topParent.Parent != null)
                {
                    x += topParent.Left;
                    y += topParent.Top;
                    topParent = topParent.Parent;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void RebuildControlContainerUser(List<ACS_USER> datas)
        {
            try
            {
                gridViewPopupUser.BeginUpdate();
                gridViewPopupUser.Columns.Clear();
                popupControlContainer1.Width = 300;
                popupControlContainer1.Height = 90;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "LOGINNAME";
                col1.Caption = "Tên đăng nhập";
                col1.Width = 100;
                col1.VisibleIndex = 1;
                gridViewPopupUser.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "USERNAME";
                col2.Caption = "Họ tên";
                col2.Width = 200;
                col2.VisibleIndex = 2;
                gridViewPopupUser.Columns.Add(col2);

                gridViewPopupUser.GridControl.DataSource = datas;
                gridViewPopupUser.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

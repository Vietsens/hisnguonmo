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
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Controls.EditorLoader;
using ACS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ExpMestSaleCreateV2
{
    public partial class UCExpMestSaleCreateV2 : UserControl
    {
        private void LoadMediStockFromRoomId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMediStockFilter filter = new HisMediStockFilter();
                filter.ROOM_ID = this.roomId;
                this.mediStock = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>>("api/HisMediStock/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                if (this.expMestId.HasValue && this.expMestId.Value > 0)
                {
                    List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = null;
                    List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = null;
                    V_HIS_EXP_MEST expMest = null;
                    List<Action> methods = new List<Action>();
                    methods.Add(() => { expMestMedicines = GetExpMestMedicineByExpMest(); });
                    methods.Add(() => { expMestMaterials = GetExpMestMaterialByExpMest(); });
                    methods.Add(() => { expMest = GetExpMestView(); });
                    ThreadCustomManager.MultipleThreadWithJoin(methods);

                    LoadMediMateBeanByExpMestMediMate(expMest, expMestMedicines, expMestMaterials);
                    LoadDataFromExpMest(expMest);
                    SetTotalPriceExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMediMateBeanByExpMestMediMate(V_HIS_EXP_MEST expMest, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials)
        {
            try
            {
                List<MediMateTypeADO> mediMateTypeADOs = new List<MediMateTypeADO>();
                if (expMestMedicines != null && expMestMedicines.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    expMestMedicineIds = expMestMedicines.Select(o => o.ID).ToList();
                    HisMedicineBeanFilter medicineBeanFilter = new HisMedicineBeanFilter();
                    medicineBeanFilter.EXP_MEST_MEDICINE_IDs = expMestMedicineIds;
                    List<HIS_MEDICINE_BEAN> medicineBeans = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_BEAN>>(RequestUriStore.HIS_MEDICINE_BEAN__GET, ApiConsumers.MosConsumer, medicineBeanFilter, param);
                    mediMateTypeADOs.AddRange(from r in expMestMedicines select new MediMateTypeADO(r, expMest, medicineBeans));
                }

                if (expMestMaterials != null && expMestMaterials.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    expMestMaterialIds = expMestMaterials.Select(o => o.ID).ToList();
                    HisMaterialBeanFilter materialBeanFilter = new HisMaterialBeanFilter();
                    materialBeanFilter.EXP_MEST_MATERIAL_IDs = expMestMaterialIds;
                    List<HIS_MATERIAL_BEAN> medicineBeans = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_BEAN>>(RequestUriStore.HIS_MATERIAL_BEAN__GET, ApiConsumers.MosConsumer, materialBeanFilter, param);
                    mediMateTypeADOs.AddRange(from r in expMestMaterials select new MediMateTypeADO(r, medicineBeans));
                }

                if (mediMateTypeADOs != null && mediMateTypeADOs.Count > 0)
                {
                    dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();
                    var groupByType = mediMateTypeADOs.GroupBy(o => new { o.MEDI_MATE_TYPE_ID, o.IsMaterial, o.IsMedicine }).ToList();
                    foreach (var type in groupByType)
                    {
                        MediMateTypeADO mediMateTypeADO = new MediMateTypeADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MediMateTypeADO>(mediMateTypeADO, type.First());
                        mediMateTypeADO.EXP_AMOUNT = type.Sum(s => s.EXP_AMOUNT);
                        mediMateTypeADO.BeanIds = new List<long>();
                        foreach (var item in type)
                        {
                            if (item.BeanIds != null && item.BeanIds.Count > 0)
                            {
                                mediMateTypeADO.BeanIds.AddRange(item.BeanIds);
                            }
                        }

                        var odlAmount = type.Where(o => o.OLD_AMOUNT.HasValue).ToList();
                        if (odlAmount != null && odlAmount.Count > 0)
                        {
                            mediMateTypeADO.OLD_AMOUNT = odlAmount.Sum(s => s.OLD_AMOUNT ?? 0);
                        }

                        if (mediMateTypeADO.IsMedicine)
                        {
                            HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == mediMateTypeADO.MEDI_MATE_TYPE_ID);
                            if (mediInStockSDO != null)
                            {
                                mediMateTypeADO.AVAILABLE_AMOUNT = mediInStockSDO.AvailableAmount;
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
                        else if (mediMateTypeADO.IsMaterial)
                        {
                            HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == mediMateTypeADO.MEDI_MATE_TYPE_ID && !mediMateTypeADO.OLD_AMOUNT.HasValue);
                            if (mateInStockSDO != null)
                            {
                                mediMateTypeADO.AVAILABLE_AMOUNT = mateInStockSDO.AvailableAmount;
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

                        dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID] = mediMateTypeADO;
                    }

                    gridControlExpMestDetail.DataSource = dicMediMateAdo.Select(o => o.Value);
                }
                SetAvaliable0MediMateStock();
                SetTotalPriceExpMestDetail();
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
                    List<HisMedicineTypeInStockSDO> medicineInStocks = this.mediInStocks.Where(o =>
                        dicMediMateAdo.Select(p => p.Value.MEDI_MATE_TYPE_ID).Contains(o.Id) || o.AvailableAmount > 0).ToList();
                    gridControlMedicineInStock.DataSource = medicineInStocks;

                    List<HisMaterialTypeInStockSDO> materialInStocks = this.mateInStocks.Where(o =>
                       dicMediMateAdo.Select(p => p.Value.MEDI_MATE_TYPE_ID).Contains(o.Id) || o.AvailableAmount > 0).ToList();
                    gridControlMaterialInStock.DataSource = materialInStocks;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataFromExpMest(V_HIS_EXP_MEST expMest)
        {
            try
            {
                if (expMest != null)
                {
                    checkIsVisitor.Enabled = false;
                    if (expMest.PRESCRIPTION_ID.HasValue)
                    {
                        txtPrescriptionCode.Text = expMest.TDL_SERVICE_REQ_CODE;
                    }
                    else
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
                    txtLoginName.Text = expMest.TDL_PRESCRIPTION_REQ_LOGINNAME;
                    txtPresUser.Text = expMest.TDL_PRESCRIPTION_REQ_USERNAME;

                    if (expMest.TDL_INTRUCTION_TIME.HasValue)
                    {
                        dtIntructionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(expMest.TDL_INTRUCTION_TIME.Value).Value;
                    }

                    lblExpMestCode.Text = expMest.EXP_MEST_CODE;
                    txtTdlPatientAccountNumber.Text = expMest.TDL_PATIENT_ACCOUNT_NUMBER;
                    txtTdlPatientTaxCode.Text = expMest.TDL_PATIENT_TAX_CODE;
                    txtTdlPatientWorkPlace.Text = expMest.TDL_PATIENT_WORK_PLACE;
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

        private List<V_HIS_EXP_MEST_MEDICINE> GetExpMestMedicineByExpMest()
        {
            List<V_HIS_EXP_MEST_MEDICINE> result = null;
            try
            {
                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_ID = this.expMestId;
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

        private List<V_HIS_EXP_MEST_MATERIAL> GetExpMestMaterialByExpMest()
        {
            List<V_HIS_EXP_MEST_MATERIAL> result = null;
            try
            {
                HisExpMestMaterialViewFilter medicineFilter = new HisExpMestMaterialViewFilter();
                medicineFilter.EXP_MEST_ID = this.expMestId;
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

        private void LoadMedicineTypeFromStock()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineTypeStockViewFilter medicineFilter = new HisMedicineTypeStockViewFilter();
                medicineFilter.MEDI_STOCK_ID = mediStock.ID;
                medicineFilter.IS_AVAILABLE = true;
                medicineFilter.IS_ACTIVE = true;
                this.mediInStocks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisMedicineTypeInStockSDO>>("api/HisMedicineType/GetInStockMedicineType", ApiConsumers.MosConsumer, medicineFilter, param)
                    .Where(o => o.IsBusiness == 1)
                    .ToList();
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
                this.mateInStocks = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>("api/HisMaterialType/GetInStockMaterialType", ApiConsumers.MosConsumer, mateFilter, null)
                    .Where(o => o.IsBusiness == 1)
                    .ToList();
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
                //Load data to GridControl
                if (this.mediInStocks != null)
                {
                    gridControlMedicineInStock.DataSource = mediInStocks;
                }
                if (this.mateInStocks != null)
                {
                    gridControlMaterialInStock.DataSource = mateInStocks;
                }


                if (this.mediStock != null)
                {
                    txtExpMediStock.Text = this.mediStock.MEDI_STOCK_NAME;
                }

                gridViewMedicineInStock.SelectRow(-1);
                gridViewMaterialInStock.SelectRow(-1);

                LoadDataToCboPatientType();
                LoadDataToCboGender();
                LoadDataToCboSampleForm();

                //Load patient from cfg
                string patientTypeCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.PatientTypeCodeDefault);
                if (!String.IsNullOrEmpty(patientTypeCode))
                {
                    HIS_PATIENT_TYPE patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == patientTypeCode);
                    if (patientType != null)
                        cboPatientType.EditValue = patientType.ID;
                }
                spinDiscount.EditValue = null;
                spinDiscountRatio.EditValue = null;
                checkImpExpPrice.Enabled = this.currentMediMate != null ? true : false;
                dtIntructionTime.DateTime = DateTime.Now;
                spinDayNum.EditValue = null;
                oldDayNum = 1;
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
                cboGender.Properties.DataSource = BackendDataWorker.Get<HIS_GENDER>();
                cboGender.Properties.DisplayMember = "GENDER_NAME";
                cboGender.Properties.ValueMember = "ID";
                cboGender.Properties.ForceInitialize();
                cboGender.Properties.Columns.Clear();
                cboGender.Properties.Columns.Add(new LookUpColumnInfo("GENDER_CODE", "", 50));
                cboGender.Properties.Columns.Add(new LookUpColumnInfo("GENDER_NAME", "", 100));
                cboGender.Properties.ShowHeader = false;
                cboGender.Properties.ImmediatePopup = true;
                cboGender.Properties.DropDownRows = 10;
                cboGender.Properties.PopupWidth = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboSampleForm()
        {
            try
            {
                cboSampleForm.Properties.DataSource = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>();
                cboSampleForm.Properties.DisplayMember = "EXP_MEST_TEMPLATE_NAME";
                cboSampleForm.Properties.ValueMember = "ID";
                cboSampleForm.Properties.ForceInitialize();
                cboSampleForm.Properties.Columns.Clear();
                cboSampleForm.Properties.Columns.Add(new LookUpColumnInfo("EXP_MEST_TEMPLATE_CODE", "", 50));
                cboSampleForm.Properties.Columns.Add(new LookUpColumnInfo("EXP_MEST_TEMPLATE_NAME", "", 200));
                cboSampleForm.Properties.ShowHeader = false;
                cboSampleForm.Properties.ImmediatePopup = true;
                cboSampleForm.Properties.DropDownRows = 10;
                cboSampleForm.Properties.PopupWidth = 250;
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
                    gridControlExpMestDetail.DataSource = dicMediMateAdo.Select(o => o.Value);
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
                if (dicMediMateAdo != null)
                {
                    DiscountDisplayProcess(this.discountFocus, this.discountRatioFocus, spinDiscount, spinDiscountRatio, dicMediMateAdo.Sum(o => o.Value.TOTAL_PRICE ?? 0));

                    decimal? totalPrice = dicMediMateAdo.Sum(o => (o.Value.TOTAL_PRICE ?? 0));
                    decimal? totalPayPrice = 0;
                    decimal? totalPriceDiscount = 0;
                    if (spinDiscountRatio.EditValue != null)
                    {
                        totalPriceDiscount = totalPrice * spinDiscountRatio.Value / 100;
                    }
                    totalPayPrice = totalPrice - totalPriceDiscount;

                    lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice ?? 0, ConfigApplications.NumberSeperator);
                    lblTotalDiscountPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPriceDiscount ?? 0, ConfigApplications.NumberSeperator);
                    lblPayPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPayPrice ?? 0, ConfigApplications.NumberSeperator);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ResetControlAfterAddClick()
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

                if (xtraTabControlMain.SelectedTabPageIndex == 0)
                {
                    txtKeyworkMedicineInStock.Focus();
                    txtKeyworkMedicineInStock.SelectAll();
                }
                if (xtraTabControlMain.SelectedTabPageIndex == 1)
                {
                    txtKeyworkMaterialInStock.Focus();
                    txtKeyworkMaterialInStock.SelectAll();
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
                cboPatientType.EditValue = null;
                this.moduleAction = GlobalDataStore.ModuleAction.ADD;
                checkIsVisitor.CheckState = CheckState.Unchecked;
                txtPrescriptionCode.Text = "";
                txtSampleForm.Text = "";
                cboSampleForm.EditValue = null;
                txtVirPatientName.Text = "";
                cboGender.EditValue = null;
                txtKeyworkMedicineInStock.Text = "";
                txtKeyworkMaterialInStock.Text = "";
                txtPatientDob.EditValue = null;
                dtPatientDob.EditValue = null;
                txtAge.Text = "";
                cboAge.EditValue = null;
                txtAddress.Text = "";
                txtDescription.Text = "";
                txtTdlPatientAccountNumber.Text = "";
                txtTdlPatientTaxCode.Text = "";
                txtTdlPatientWorkPlace.Text = "";
                lblPayPrice.Text = "";
                this.serviceReq = null;
                this.expMestResult = null;
                this.expMestId = null;
                lblTotalPrice.Text = "";
                gridControlExpMestDetail.DataSource = null;
                this.discountFocus = false;
                this.discountRatioFocus = false;
                this.discountDetailFocus = false;
                this.discountDetailRatioFocus = false;
                dicMediMateAdo.Clear();
                gridControlExpMestMedicine.DataSource = null;
                gridControlExpMestMaterial.DataSource = null;
                this.expMestMaterialIds = null;
                this.expMestMedicineIds = null;
                checkIsVisitor.Enabled = true;
                txtPresUser.Text = "";
                txtLoginName.Text = "";
                btnSaleBill.Enabled = true;
                btnAdd.Enabled = true;
                btnSave.Enabled = true;
                //btnSavePrint.Enabled = true;
                this.expMestResult = new HisExpMestResultSDO();
                this.expMestResult.ExpMest = new HIS_EXP_MEST();
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
                    txtVirPatientName.Enabled = true;
                    cboGender.Enabled = true;
                    txtPatientDob.Enabled = true;
                    txtAddress.Enabled = true;
                    txtLoginName.Enabled = true;
                    txtPresUser.Enabled = true;
                    //txtDescription.Enabled = true;
                    txtMaTHX.Enabled = true;
                    cboTHX.Enabled = true;
                }
                else
                {
                    txtPrescriptionCode.Enabled = true;
                    txtVirPatientName.Enabled = false;
                    cboGender.Enabled = false;
                    txtPatientDob.Enabled = false;
                    txtAddress.Enabled = false;
                    txtLoginName.Enabled = false;
                    txtPresUser.Enabled = false;
                    //txtDescription.Enabled = false;
                    txtMaTHX.Enabled = false;
                    cboTHX.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridDetailByTemplate(HIS_EXP_MEST_TEMPLATE data)
        {
            try
            {
                if (data != null)
                {
                    ReleaseAllAndResetGrid();
                    Dictionary<long, MediMateTypeADO> dicMediMateTempAdo = new Dictionary<long, MediMateTypeADO>();

                    WaitingManager.Show();
                    HisEmteMedicineTypeViewFilter medicineFilter = new HisEmteMedicineTypeViewFilter();
                    medicineFilter.EXP_MEST_TEMPLATE_ID = data.ID;
                    var medicineTemps = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EMTE_MEDICINE_TYPE>>(HisRequestUriStore.HIS_EMTE_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, null);

                    HisEmteMaterialTypeViewFilter materialFilter = new HisEmteMaterialTypeViewFilter();
                    materialFilter.EXP_MEST_TEMPLATE_ID = data.ID;
                    var materialTemps = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EMTE_MATERIAL_TYPE>>(HisRequestUriStore.HIS_EMTE_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, materialFilter, null);

                    WaitingManager.Hide();

                    if (materialTemps.Count == 0 && medicineTemps.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy thông tin thuốc vật tư theo mẫu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (medicineTemps != null && medicineTemps.Count > 0)
                    {
                        foreach (var medicineTemp in medicineTemps)
                        {

                            MediMateTypeADO ado = new MediMateTypeADO(medicineTemp);
                            HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == medicineTemp.MEDICINE_TYPE_ID);
                            if (mediInStockSDO != null)
                            {
                                ado.NATIONAL_NAME = mediInStockSDO.NationalName;
                                ado.MANUFACTURER_NAME = mediInStockSDO.ManufacturerName;
                                ado.REGISTER_NUMBER = mediInStockSDO.RegisterNumber;
                                ado.AVAILABLE_AMOUNT = mediInStockSDO.AvailableAmount;
                                if (medicineTemp.AMOUNT > mediInStockSDO.AvailableAmount)
                                {
                                    ado.IsExceedsAvailable = true;
                                }
                                ado.ACTIVE_INGR_BHYT_CODE = mediInStockSDO.ActiveIngrBhytCode;
                                ado.ACTIVE_INGR_BHYT_NAME = mediInStockSDO.ActiveIngrBhytName;
                                ado.CONCENTRA = mediInStockSDO.Concentra;
                            }
                            else
                            {
                                ado.IsNotInStock = true;
                            }

                            if (cboPatientType.EditValue != null)
                            {
                                var listServicePaty = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_PATY>();
                                if (listServicePaty != null)
                                {
                                    var paty = listServicePaty.FirstOrDefault(o => o.PATIENT_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString())
                                        && o.SERVICE_ID == medicineTemp.SERVICE_ID);
                                    if (paty != null)
                                    {
                                        ado.EXP_PRICE = paty.PRICE;
                                        ado.EXP_VAT_RATIO = paty.VAT_RATIO;
                                    }
                                }
                            }

                            if (dicMediMateTempAdo.ContainsKey(ado.MEDI_MATE_TYPE_ID))
                            {
                                ado.EXP_AMOUNT += dicMediMateAdo[ado.MEDI_MATE_TYPE_ID].EXP_AMOUNT;
                                ado.TOTAL_PRICE = ado.EXP_AMOUNT * ado.EXP_PRICE;
                            }

                            dicMediMateTempAdo[ado.MEDI_MATE_TYPE_ID] = ado;
                        }
                    }


                    if (materialTemps != null && materialTemps.Count > 0)
                    {
                        foreach (var materialTemp in materialTemps)
                        {

                            MediMateTypeADO ado = new MediMateTypeADO(materialTemp);
                            HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == materialTemp.MATERIAL_TYPE_ID);
                            if (mateInStockSDO != null)
                            {
                                ado.NATIONAL_NAME = mateInStockSDO.NationalName;
                                ado.MANUFACTURER_NAME = mateInStockSDO.ManufacturerName;
                                ado.AVAILABLE_AMOUNT = mateInStockSDO.AvailableAmount;
                                if (mateInStockSDO.AvailableAmount < materialTemp.AMOUNT)
                                {
                                    ado.IsExceedsAvailable = true;
                                }
                            }
                            else
                            {
                                ado.IsNotInStock = true;
                            }

                            if (cboPatientType.EditValue != null)
                            {
                                var listServicePaty = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_PATY>();
                                if (listServicePaty != null)
                                {
                                    var paty = listServicePaty.FirstOrDefault(o => o.PATIENT_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString())
                                        && o.SERVICE_ID == materialTemp.SERVICE_ID);
                                    if (paty != null)
                                    {
                                        ado.EXP_PRICE = paty.PRICE;
                                        ado.EXP_VAT_RATIO = paty.VAT_RATIO;
                                    }
                                }
                            }

                            if (dicMediMateTempAdo.ContainsKey(ado.MEDI_MATE_TYPE_ID))
                            {
                                ado.EXP_AMOUNT += dicMediMateAdo[ado.MEDI_MATE_TYPE_ID].EXP_AMOUNT;
                                ado.TOTAL_PRICE = ado.EXP_AMOUNT * ado.EXP_PRICE;
                            }

                            dicMediMateTempAdo[ado.MEDI_MATE_TYPE_ID] = ado;
                        }
                    }

                    //Take bean
                    TakeBeanMedicineAll(dicMediMateTempAdo);
                    TakeBeanMaterialAll(dicMediMateTempAdo);
                    gridControlExpMestDetail.DataSource = dicMediMateAdo.Select(o => o.Value);
                    SetTotalPriceExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TakeBeanMedicineAll(Dictionary<long, MediMateTypeADO> dic)
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
                            takeBeanSDO.ClientSessionKey = this.clientSessionKey;
                            takeBeanSDO.BeanIds = null;
                            takeBeanSDO.Amount = medicine.EXP_AMOUNT;
                            takeBeanSDO.MediStockId = this.mediStock.ID;
                            takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                            takeBeanSDO.TypeId = medicine.MEDI_MATE_TYPE_ID;
                            takeBeanSDOs.Add(takeBeanSDO);
                        }

                        List<TakeMedicineBeanListResultSDO> takeMedicines = new BackendAdapter(param)
                    .Post<List<TakeMedicineBeanListResultSDO>>(RequestUriStore.HIS_MEDICINE_BEAN__TAKEBEANLIST, ApiConsumers.MosConsumer, takeBeanSDOs, param);
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
                                    dic[takeMedicine.Request.TypeId].EXP_PRICE = Math.Round((dic[takeMedicine.Request.TypeId].TOTAL_PRICE ?? 0) / dic[takeMedicine.Request.TypeId].EXP_AMOUNT, 4);
                                    dic[takeMedicine.Request.TypeId].EXP_VAT_RATIO = 0;
                                    dicMediMateAdo[takeMedicine.Request.TypeId] = dic[takeMedicine.Request.TypeId];
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
                            dicMediMateAdo[medicineIsExceedsAvailableOrIsNotInStock.MEDI_MATE_TYPE_ID] = medicineIsExceedsAvailableOrIsNotInStock;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TakeBeanMaterialAll(Dictionary<long, MediMateTypeADO> dic)
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
                            takeBeanSDO.ClientSessionKey = this.clientSessionKey;
                            takeBeanSDO.BeanIds = null;
                            takeBeanSDO.Amount = material.EXP_AMOUNT;
                            takeBeanSDO.MediStockId = this.mediStock.ID;
                            takeBeanSDO.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                            takeBeanSDO.TypeId = material.MEDI_MATE_TYPE_ID;
                            takeBeanSDOs.Add(takeBeanSDO);
                        }

                        List<TakeMaterialBeanListResultSDO> takeMaterials = new BackendAdapter(param)
                    .Post<List<TakeMaterialBeanListResultSDO>>(RequestUriStore.HIS_MATERIAL_BEAN__TAKEBEANLIST, ApiConsumers.MosConsumer, takeBeanSDOs, param);
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
                                    dic[takeMaterial.Request.TypeId].EXP_PRICE = Math.Round((dic[takeMaterial.Request.TypeId].TOTAL_PRICE ?? 0) / dic[takeMaterial.Request.TypeId].EXP_AMOUNT, 4);
                                    dic[takeMaterial.Request.TypeId].EXP_VAT_RATIO = 0;
                                    dicMediMateAdo[takeMaterial.Request.TypeId] = dic[takeMaterial.Request.TypeId];
                                }
                            }
                        }
                    }

                    List<MediMateTypeADO> matecineIsExceedsAvailableOrIsNotInStocks = dic.Select(o => o.Value).Where(o => o.IsMaterial == true && (o.IsExceedsAvailable || o.IsNotInStock)).ToList();
                    if (matecineIsExceedsAvailableOrIsNotInStocks != null && matecineIsExceedsAvailableOrIsNotInStocks.Count > 0)
                    {
                        foreach (var matecineIsExceedsAvailableOrIsNotInStock in matecineIsExceedsAvailableOrIsNotInStocks)
                        {
                            dicMediMateAdo[matecineIsExceedsAvailableOrIsNotInStock.MEDI_MATE_TYPE_ID] = matecineIsExceedsAvailableOrIsNotInStock;
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

        private HIS_TREATMENT GetTreatmentById(long treatmentId)
        {
            HIS_TREATMENT result = null;
            try
            {
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = treatmentId;
                result = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadPatientInfoFromPrescription(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                if (serviceReq != null)
                {
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
                    var treatment = GetTreatmentById(serviceReq.TREATMENT_ID);
                    txtTdlPatientAccountNumber.Text = treatment != null ? treatment.TDL_PATIENT_ACCOUNT_NUMBER : "";
                    txtTdlPatientTaxCode.Text = treatment != null ? treatment.TDL_PATIENT_TAX_CODE : "";
                    txtTdlPatientWorkPlace.Text = treatment != null ? treatment.TDL_PATIENT_WORK_PLACE : "";
                }
                else
                {
                    txtVirPatientName.Text = "";
                    txtAddress.Text = "";
                    cboGender.EditValue = null;
                    txtPatientDob.EditValue = null;
                    txtLoginName.Text = "";
                    txtPresUser.Text = null;
                    txtTdlPatientAccountNumber.Text = "";
                    txtTdlPatientTaxCode.Text = "";
                    txtTdlPatientWorkPlace.Text = "";
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
                    txtSampleForm.Enabled = false;
                    cboSampleForm.Enabled = false;
                    txtVirPatientName.Enabled = false;
                    cboGender.Enabled = false;
                    txtPatientDob.Enabled = false;
                    txtAddress.Enabled = false;
                    txtMaTHX.Enabled = false;
                    cboTHX.Enabled = false;
                    txtDescription.Enabled = false;
                    xtraTabControlMain.Enabled = false;
                    gridControlExpMestDetail.Enabled = false;
                    xtraTabControlExpMest.Enabled = false;
                    spinAmount.Enabled = false;
                    checkImpExpPrice.Enabled = false;
                    spinExpPrice.Enabled = false;
                    spinExpVatRatio.Enabled = false;
                    spinDiscount.Enabled = false;
                    txtTutorial.Enabled = false;
                    txtNote.Enabled = false;
                    btnAdd.Enabled = false;
                    //btnSavePrint.Enabled = false;
                    btnSave.Enabled = false;
                    btnNew.Enabled = false;
                    btnSaleBill.Enabled = false;
                    ddBtnPrint.Enabled = false;
                    spinDiscountRatio.Enabled = false;
                    spinDiscountDetailRatio.Enabled = false;
                    spinDiscountDetail.Enabled = false;
                    lblTotalPrice.Enabled = false;
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
                Inventec.Common.Logging.LogSystem.Info("InitComboUser. 1");
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
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

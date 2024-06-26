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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ImpMestViewDetail.ADO;
using HIS.Desktop.Plugins.ImpMestViewDetail.Base;
using HIS.Desktop.Plugins.ImpMestViewDetail.Config;
using HIS.Desktop.Utility;
using IMSys.DbConfig.HIS_RS;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestViewDetail.ImpMestViewDetail
{
    public partial class frmImpMestViewDetail : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        long ImpMestId;
        long IMP_MEST_TYPE_ID;
        long ImpMestSttId;
        List<ImpMestMaterialSDODetail> impMestMaterials;
        List<ImpMestMedicineSDODetail> impMestMedicines;
        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines;
        List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials;
        List<V_HIS_IMP_MEST_MATERIAL> vimpMestMaterials;
        List<ImpMestBloodSDODetail> impMestBloods;
        V_HIS_IMP_MEST impMest;
        public const string HIS_INIT_IMP_MEST_GETVIEW = "api/HisInitImpMest/GetView";
        public const string HIS_INVE_IMP_MEST_GETVIEW = "api/HisInveImpMest/GetView";
        public const string HIS_OTHER_IMP_MEST_GETVIEW = "api/HisOtherImpMest/GetView";
        Inventec.Desktop.Common.Modules.Module moduleData;
        DelegateSelectData delegateSelectData = null;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        List<V_HIS_MEDICINE_TYPE> medicineTypes;
        List<V_HIS_MATERIAL_TYPE> materialTypes;
        List<HIS_MEDICINE> Medicines;
        List<HIS_MATERIAL> Materials;
        List<HIS_MEDICINE_PATY> MedicinePaty;
        List<HIS_MATERIAL_PATY> MaterialPaty;
        bool IsAdmin = false;
        int positionHandleControl = -1;
        internal bool IsAllowDuplicateDocument = false;
        #endregion

        #region Construct
        /// <summary>
        /// contructor
        /// </summary>
        /// <param name="_impMestId">id phieu nhap</param>
        /// <param name="_impMestTypeId">id loai nhap</param>
        /// <param name="_impMestSttId">id trang thai nhap</param>
        public frmImpMestViewDetail(long _impMestId, long _impMestTypeId, long _impMestSttId, Inventec.Desktop.Common.Modules.Module moduleData, DelegateSelectData _delegateSelectData)
        {
            try
            {
                InitializeComponent();
                ImpMestId = _impMestId;
                this.IMP_MEST_TYPE_ID = _impMestTypeId;
                this.moduleData = moduleData;
                ImpMestSttId = _impMestSttId;
                this.delegateSelectData = _delegateSelectData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmBill_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                SetIcon();
                IsAdmin = CheckEmployIsAdmin();
                SetDataToCommonControl();
                EnableGridColumn(IsAdmin);
                SetCaptionByLanguageKey();
                LoadMobaExpMest();
                loadDataToGridMaterial();
                loadDataToGridMedicine();
                LoadDataToGridBlood();
                InitComboBloodType(GetBloodType());
                medicineTypes = new List<V_HIS_MEDICINE_TYPE>();
                materialTypes = new List<V_HIS_MATERIAL_TYPE>();
                if (this.impMestMedicines != null && this.impMestMedicines.Count() > 0)
                {
                    var bidIdMedicines = this.impMestMedicines.Select(o => o.BID_ID ?? 0).Distinct().ToList();
                    var medicineTypeIds = this.impMestMedicines.Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                    medicineTypes = FilterMedicineType(bidIdMedicines, medicineTypeIds);
                    InitComboMedicineType(medicineTypes);
                }

                if (this.impMestMaterials != null && this.impMestMaterials.Count() > 0)
                {
                    var bidIdMaterials = this.impMestMaterials.Select(o => o.BID_ID ?? 0).Distinct().ToList();
                    var materialTypeIds = this.impMestMaterials.Select(o => o.MATERIAL_TYPE_ID).Distinct().ToList();
                    materialTypes = FilterMaterialType(bidIdMaterials, materialTypeIds);
                    InitComboMaterialType(materialTypes);
                }

                ShowTab();
                GetControlAcs();
                EnableButton();
                InitMenuToButtonPrint();
                ValidateControlForm();
                WaitingManager.Hide();
                ItemGridLookUpEdit_BloodName.CloseUpKey = new DevExpress.Utils.KeyShortcut(Keys.Enter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateControlForm()
        {
            try
            {
                if (Config.HisConfigCFG.MustEnterDocumentNumberAndDocumentDate && impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    LciDocumentNumber.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutDocumentDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidControl(TxtDocumentNumber);
                    ValidControlDate();//
                }
                else
                {
                    LciDocumentNumber.AppearanceItemCaption.ForeColor = Color.Black;
                    layoutDocumentDate.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProvider1.RemoveControlError(TxtDocumentNumber);
                    dxValidationProvider1.RemoveControlError(dtDocumentDate);
                    dxValidationProvider1.RemoveControlError(txtDocumentDate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDate()
        {
            try
            {
                Validation.DocumentDateValidationRule docDateRule = new Validation.DocumentDateValidationRule();
                docDateRule.txtDocumentDate = txtDocumentDate;
                docDateRule.dtDocumentDate = dtDocumentDate;
                dxValidationProvider1.SetValidationRule(dtDocumentDate, docDateRule);
                dxValidationProvider1.SetValidationRule(txtDocumentDate, docDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region private function
        private List<V_HIS_BLOOD_TYPE> GetBloodType()
        {
            List<V_HIS_BLOOD_TYPE> result = new List<V_HIS_BLOOD_TYPE>();
            try
            {
                result = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool CheckEmployIsAdmin()
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisEmployeeFilter hisEmployeeFilter = new HisEmployeeFilter();
                hisEmployeeFilter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var employee = new BackendAdapter(param).Get<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumer.ApiConsumers.MosConsumer, hisEmployeeFilter, param).FirstOrDefault();
                if (employee != null && employee.IS_ADMIN == 1)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void EnableGridColumn(bool IsAdmin)
        {
            try
            {
                TxtDocumentNumber.Enabled = this.impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
                dtDocumentDate.Enabled = this.impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
                txtDescription.Enabled = this.impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
                SpDocumentPrice.Enabled = this.impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
                TxtDeliverer.Enabled = this.impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
                SpDiscount.Enabled = this.impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
                SpDiscountRatio.Enabled = this.impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
                txtDescription.Enabled = this.impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
                panelControl1.Enabled = this.impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_MEDICINE_TYPE> FilterMedicineType(List<long> bidIds, List<long> medicineTypeIds)
        {
            List<V_HIS_MEDICINE_TYPE> result = new List<V_HIS_MEDICINE_TYPE>();
            try
            {
                if (bidIds != null && bidIds.Count() > 0)
                {
                    CommonParam param = new CommonParam();
                    // get bid_medicine_type
                    MOS.Filter.HisBidMedicineTypeViewFilter bidMedicineTypeFilter = new HisBidMedicineTypeViewFilter();
                    bidMedicineTypeFilter.BID_IDs = bidIds;
                    var bidMedicineTypes = new BackendAdapter(param).Get<List<V_HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/GetView", ApiConsumer.ApiConsumers.MosConsumer, bidMedicineTypeFilter, param);
                    if (bidMedicineTypes != null && bidMedicineTypes.Count() > 0)
                    {
                        result = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => bidMedicineTypes.Select(p => p.MEDICINE_TYPE_ID).Contains(o.ID)).ToList();
                        // add thêm thuốc nếu không có trong thầu
                        var checkNotExistMedicines = medicineTypeIds != null && medicineTypeIds.Count() > 0 ? medicineTypeIds.Where(o => !bidMedicineTypes.Select(p => p.MEDICINE_TYPE_ID).Contains(o)).ToList() : null;
                        if (checkNotExistMedicines != null && checkNotExistMedicines.Count() > 0)
                        {
                            var medicineTypeAdds = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => checkNotExistMedicines.Contains(o.ID)).ToList();
                            if (medicineTypeAdds != null && medicineTypeAdds.Count() > 0)
                                result.AddRange(medicineTypeAdds);
                        }

                    }
                    else
                    {
                        result = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    }
                }
                else
                {
                    result = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                }
                // filter is_active
                result = result != null && result.Count() > 0 ?
                    result.ToList()
                    : result;
            }
            catch (Exception ex)
            {
                result = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<V_HIS_MATERIAL_TYPE> FilterMaterialType(List<long> bidIds, List<long> materialTypeIds)
        {
            List<V_HIS_MATERIAL_TYPE> result = new List<V_HIS_MATERIAL_TYPE>();
            try
            {
                if (bidIds != null && bidIds.Count() > 0)
                {
                    CommonParam param = new CommonParam();
                    // get bid_medicine_type
                    MOS.Filter.HisBidMaterialTypeViewFilter bidMaterialTypeFilter = new HisBidMaterialTypeViewFilter();
                    bidMaterialTypeFilter.BID_IDs = bidIds;
                    var bidMaterialTypes = new BackendAdapter(param).Get<List<V_HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/GetView", ApiConsumer.ApiConsumers.MosConsumer, bidMaterialTypeFilter, param);
                    if (bidMaterialTypes != null && bidMaterialTypes.Count() > 0)
                    {
                        result = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => bidMaterialTypes.Select(p => p.MATERIAL_TYPE_ID).Contains(o.ID)).ToList();
                        // add thêm vật tư nếu không có trong thầu
                        var checkNotExistMaterials = materialTypeIds != null && materialTypeIds.Count() > 0 ? materialTypeIds.Where(o => !bidMaterialTypes.Select(p => p.MATERIAL_TYPE_ID).Contains(o)).ToList() : null;
                        if (checkNotExistMaterials != null && checkNotExistMaterials.Count() > 0)
                        {
                            var materialTypeAdds = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => checkNotExistMaterials.Contains(o.ID)).ToList();
                            if (materialTypeAdds != null && materialTypeAdds.Count() > 0)
                                result.AddRange(materialTypeAdds);
                        }
                    }
                    else
                    {
                        result = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    }
                }
                else
                {
                    result = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                }

                // filter is_active
                result = result != null && result.Count() > 0 ?
                    result.ToList()
                    : result;
            }
            catch (Exception ex)
            {
                result = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitComboMedicineType(List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> medicineTypes)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.ItemGridLookUpEdit_MedicineName, medicineTypes, controlEditorADO);

                List<ColumnInfo> disColumns = new List<ColumnInfo>();
                disColumns.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 100, 1));
                disColumns.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 250, 2));
                ControlEditorADO disADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", disColumns, false, 350);
                ControlEditorLoader.Load(this.repositoryItemGridLookUpEdit_Medicine_Disable, medicineTypes, disADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMaterialType(List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> materialTypes)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.ItemGridLookUpEdit_MaterialName, materialTypes, controlEditorADO);

                List<ColumnInfo> disColumns = new List<ColumnInfo>();
                disColumns.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 100, 1));
                disColumns.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 250, 2));
                ControlEditorADO disADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", disColumns, false, 350);
                ControlEditorLoader.Load(this.repositoryItemGridLookUpEdit_Material_Disable, materialTypes, disADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBloodType(List<MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE> materialTypes)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BLOOD_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.ItemGridLookUpEdit_BloodName, materialTypes, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetControlAcs()
        {
            try
            {
                CommonParam param = new CommonParam();
                ACS.SDO.AcsTokenLoginSDO tokenLoginSDOForAuthorize = new ACS.SDO.AcsTokenLoginSDO();
                tokenLoginSDOForAuthorize.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                tokenLoginSDOForAuthorize.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;

                var acsAuthorize = new BackendAdapter(param).Get<ACS.SDO.AcsAuthorizeSDO>(HIS.Desktop.ApiConsumer.AcsRequestUriStore.ACS_TOKEN__AUTHORIZE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, tokenLoginSDOForAuthorize, param);

                if (acsAuthorize != null)
                {
                    controlAcs = acsAuthorize.ControlInRoles.ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowTab()
        {
            try
            {
                if (impMestMedicines != null && impMestMedicines.Count > 0) xtraTabControl1.SelectedTabPageIndex = 0;
                else if (impMestMaterials != null && impMestMaterials.Count > 0) xtraTabControl1.SelectedTabPageIndex = 1;
                else if (impMestBloods != null && impMestBloods.Count > 0) xtraTabControl1.SelectedTabPageIndex = 2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void resetControl()
        {
            try
            {
                gridControlMaterial.DataSource = null;
                gridControlMedicine.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMobaExpMest()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (impMest.MOBA_EXP_MEST_ID == null)
                {
                    return;
                }

                MOS.Filter.HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                hisExpMestMedicineViewFilter.EXP_MEST_ID = impMest.MOBA_EXP_MEST_ID;
                this.expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, param);

                MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialViewFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                expMestMaterialViewFilter.EXP_MEST_ID = impMest.MOBA_EXP_MEST_ID;
                this.expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialViewFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToCommonControl()
        {
            try
            {
                MOS.Filter.HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();
                impMestViewFilter.ID = this.ImpMestId;
                var impMests = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, impMestViewFilter, new CommonParam());
                if (impMests != null && impMests.Count > 0)
                {
                    impMest = impMests.FirstOrDefault();
                    lblImpMedistock.Text = impMest.MEDI_STOCK_CODE + " - " + impMest.MEDI_STOCK_NAME;
                    lblImpMestCode.Text = impMest.IMP_MEST_CODE;
                    lblImpTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(impMest.IMP_TIME ?? 0);
                    lblImpUserName.Text = impMest.IMP_LOGINNAME + " - " + impMest.IMP_USERNAME;

                    TxtDocumentNumber.Text = impMest.DOCUMENT_NUMBER;
                    SpDocumentPrice.EditValue = impMest.DOCUMENT_PRICE;
                    TxtDeliverer.Text = impMest.DELIVERER;
                    txtDescription.Text = impMest.DESCRIPTION;
                    SpDiscount.EditValue = impMest.DISCOUNT;
                    SpDiscountRatio.EditValue = (impMest.DISCOUNT_RATIO ?? 0) * 100;
                    txtDocumentDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(impMest.DOCUMENT_DATE ?? 0);
                    dtDocumentDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(impMest.DOCUMENT_DATE ?? 0) ?? DateTime.MinValue;
                }
                else
                {
                    lblImpUserName.Text = "";
                    lblImpTime.Text = "";
                    lblImpMestCode.Text = "";
                    lblImpMedistock.Text = "";
                    TxtDocumentNumber.Text = "";
                    SpDocumentPrice.EditValue = "";
                    TxtDeliverer.Text = "";
                    txtDescription.Text = "";
                    SpDiscount.EditValue = null;
                    SpDiscountRatio.EditValue = null;
                    txtDocumentDate.Text = null;
                    dtDocumentDate.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // load dữ liệu vào grid thuốc
        private void loadDataToGridMedicine()
        {
            CommonParam param = new CommonParam();
            try
            {
                impMestMedicines = new List<ImpMestMedicineSDODetail>();

                MOS.Filter.HisImpMestMedicineViewFilter filter = new HisImpMestMedicineViewFilter();
                filter.IMP_MEST_ID = this.ImpMestId;
                var impMestMedicineAPIs = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                if (impMestMedicineAPIs != null && impMestMedicineAPIs.Count > 0)
                {
                    this.Medicines = new List<HIS_MEDICINE>();
                    List<long> mediIds = impMestMedicineAPIs.Select(o => o.MEDICINE_ID).Distinct().ToList();
                    int skip = 0;
                    while (mediIds.Count - skip > 0)
                    {
                        var ids = mediIds.Skip(skip).Take(500).ToList();
                        skip += 500;
                        MOS.Filter.HisMedicineFilter filterMedi = new MOS.Filter.HisMedicineFilter();
                        filterMedi.IDs = ids;
                        CommonParam paramMedi = new CommonParam();
                        var medis = new BackendAdapter(paramMedi).Get<List<HIS_MEDICINE>>("/api/HisMedicine/Get", ApiConsumers.MosConsumer, filterMedi, paramMedi);
                        if (medis != null && medis.Count > 0)
                        {
                            this.Medicines.AddRange(medis);
                        }
                    }

                    //Load du lieu chinh sach gia
                    if (this.Medicines != null && this.Medicines.Count > 0)
                    {
                        MOS.Filter.HisMedicinePatyFilter filterMediPaty = new MOS.Filter.HisMedicinePatyFilter();
                        CommonParam paramMediPaty = new CommonParam();
                        filterMediPaty.MEDICINE_IDs = this.Medicines.Select(o => o.ID).Distinct().ToList();
                        this.MedicinePaty = new BackendAdapter(paramMediPaty).Get<List<HIS_MEDICINE_PATY>>("/api/HisMedicinePaty/Get", ApiConsumers.MosConsumer, filterMediPaty, paramMediPaty);
                    }
                }

                foreach (var item in impMestMedicineAPIs)
                {
                    //Don gia ban bhyt và VP 
                    ImpMestMedicineSDODetail impMestMedicine = new ImpMestMedicineSDODetail(item);
                    var datamedi = this.Medicines.FirstOrDefault<HIS_MEDICINE>(o => o.ID == item.MEDICINE_ID);
                    if (datamedi != null && datamedi.IS_SALE_EQUAL_IMP_PRICE != 1)
                    {
                        var datamedipatyBHYT = this.MedicinePaty.FirstOrDefault(o => o.MEDICINE_ID == item.MEDICINE_ID && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT);
                        var datamedipatyVP = this.MedicinePaty.FirstOrDefault(o => o.MEDICINE_ID == item.MEDICINE_ID && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__VP);
                        if (datamedipatyBHYT != null)
                        {
                            if (item.TDL_IMP_UNIT_ID.HasValue)
                            {
                                impMestMedicine.HienPrice = (datamedipatyBHYT.IMP_UNIT_EXP_PRICE ?? 0) * (1 + datamedipatyBHYT.EXP_VAT_RATIO);
                            }
                            else
                            {
                                impMestMedicine.HienPrice = datamedipatyBHYT.EXP_PRICE * (1 + datamedipatyBHYT.EXP_VAT_RATIO);
                            }
                        }
                        if (datamedipatyVP != null)
                        {
                            if (item.TDL_IMP_UNIT_ID.HasValue)
                            {
                                impMestMedicine.PriceVP = (datamedipatyVP.IMP_UNIT_EXP_PRICE ?? 0) * (1 + datamedipatyVP.EXP_VAT_RATIO);
                            }
                            else
                            {
                                impMestMedicine.PriceVP = datamedipatyVP.EXP_PRICE * (1 + datamedipatyVP.EXP_VAT_RATIO);
                            }
                        }
                    }
                    else if (datamedi != null && datamedi.IS_SALE_EQUAL_IMP_PRICE == 1)
                    {
                        if (item.TDL_IMP_UNIT_ID.HasValue)
                        {
                            impMestMedicine.HienPrice = (datamedi.IMP_UNIT_PRICE ?? 0) * (1 + datamedi.IMP_VAT_RATIO);
                            impMestMedicine.PriceVP = (datamedi.IMP_UNIT_PRICE ?? 0) * (1 + datamedi.IMP_VAT_RATIO);
                        }
                        else
                        {
                            impMestMedicine.HienPrice = datamedi.IMP_PRICE * (1 + datamedi.IMP_VAT_RATIO);
                            impMestMedicine.PriceVP = datamedi.IMP_PRICE * (1 + datamedi.IMP_VAT_RATIO);
                        }
                    }
                    impMestMedicines.Add(impMestMedicine);
                }

                // sắp xếp theo thứ tự tăng dần id
                if (impMestMedicines != null && impMestMedicines.Count > 0)
                {
                    impMestMedicines = impMestMedicines.OrderBy(o => o.ID).ToList();
                    var impMestMedicineGroup = impMestMedicines.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_ID, o.IMP_PRICE, o.IMP_VAT_RATIO, o.SERVICE_UNIT_ID }).Select(y =>
                  new
                  {
                      MEDICINE_TYPE_ID = y.First().MEDICINE_TYPE_ID,
                      AMOUNT = y.Sum(o => o.AMOUNT),
                      MEDICINE_ID = y.First().MEDICINE_ID,
                      IMP_PRICE = y.First().IMP_PRICE,
                      IMP_VAT_RATIO = y.First().IMP_VAT_RATIO,
                      SERVICE_UNIT_ID = y.First().SERVICE_UNIT_ID
                  }).ToList();
                    impMestMedicines = impMestMedicines.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_ID, o.IMP_PRICE, o.IMP_VAT_RATIO, o.SERVICE_UNIT_ID }).Select(o => o.First()).ToList();
                    foreach (var item in impMestMedicines)
                    {
                        var medicineCheck = impMestMedicineGroup.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID && o.MEDICINE_ID == item.MEDICINE_ID && o.IMP_VAT_RATIO == item.IMP_VAT_RATIO && o.SERVICE_UNIT_ID == item.SERVICE_UNIT_ID);
                        if (medicineCheck != null)
                        {
                            item.AMOUNT = medicineCheck.AMOUNT;
                        }

                        if (this.expMestMedicines != null && this.expMestMedicines.Count > 0)
                        {
                            var checkMobaExpMestMedicine = this.expMestMedicines.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID);
                            if (checkMobaExpMestMedicine != null)
                            {
                                item.PRICE = checkMobaExpMestMedicine.PRICE;
                                item.VAT_RATIO = checkMobaExpMestMedicine.VAT_RATIO;
                            }
                        }

                        item.AMOUNT_OLD = item.AMOUNT;
                    }
                }
                gridControlMedicine.DataSource = impMestMedicines;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // load dữ liệu vào grid vật tư
        private void loadDataToGridMaterial()
        {
            CommonParam param = new CommonParam();
            try
            {
                MOS.Filter.HisImpMestMaterialViewFilter filter = new HisImpMestMaterialViewFilter();
                filter.IMP_MEST_ID = this.ImpMestId;
                var impMestMaterialAPIs = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, filter, param);

                impMestMaterials = new List<ImpMestMaterialSDODetail>();
                if (impMestMaterialAPIs != null && impMestMaterialAPIs.Count > 0)
                {
                    this.Materials = new List<HIS_MATERIAL>();
                    List<long> materialId = impMestMaterialAPIs.Select(o => o.MATERIAL_ID).Distinct().ToList();
                    int skip = 0;
                    while (materialId.Count - skip > 0)
                    {
                        var ids = materialId.Skip(skip).Take(500).ToList();
                        skip += 500;
                        MOS.Filter.HisMaterialFilter filterMaTer = new MOS.Filter.HisMaterialFilter();
                        CommonParam paramMater = new CommonParam();
                        filterMaTer.IDs = ids;
                        var material = new BackendAdapter(paramMater).Get<List<HIS_MATERIAL>>("/api/HisMaterial/Get", ApiConsumers.MosConsumer, filterMaTer, paramMater);
                        if (material != null && material.Count > 0)
                        {
                            this.Materials.AddRange(material);
                        }
                    }

                    //load du lieu chinh sach gia
                    if (this.Materials != null && this.Materials.Count > 0)
                    {
                        MOS.Filter.HisMaterialPatyFilter filterMaterPaty = new MOS.Filter.HisMaterialPatyFilter();
                        CommonParam paramMaterPaty = new CommonParam();
                        filterMaterPaty.MATERIAL_IDs = this.Materials.Select(o => o.ID).Distinct().ToList();
                        this.MaterialPaty = new BackendAdapter(paramMaterPaty).Get<List<HIS_MATERIAL_PATY>>("/api/HisMaterialPaty/Get", ApiConsumers.MosConsumer, filterMaterPaty, paramMaterPaty);
                    }
                }
                foreach (var item in impMestMaterialAPIs)
                {
                    ImpMestMaterialSDODetail ImpMestMaterialSDODetail = new ImpMestMaterialSDODetail(item);
                    var dataMater = this.Materials.FirstOrDefault<HIS_MATERIAL>(o => o.ID == item.MATERIAL_ID);
                    if (dataMater != null && dataMater.IS_SALE_EQUAL_IMP_PRICE != 1)
                    {
                        var dataMatepatyBHYT = this.MaterialPaty.FirstOrDefault(o => o.MATERIAL_ID == item.MATERIAL_ID && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT);
                        var dataMatepatyVP = this.MaterialPaty.FirstOrDefault(o => o.MATERIAL_ID == item.MATERIAL_ID && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__VP);
                        if (dataMatepatyBHYT != null)
                        {
                            if (item.TDL_IMP_UNIT_ID.HasValue)
                            {
                                ImpMestMaterialSDODetail.HienPrice = (dataMatepatyBHYT.IMP_UNIT_EXP_PRICE ?? 0) * (1 + dataMatepatyBHYT.EXP_VAT_RATIO);
                            }
                            else
                            {
                                ImpMestMaterialSDODetail.HienPrice = dataMatepatyBHYT.EXP_PRICE * (1 + dataMatepatyBHYT.EXP_VAT_RATIO);
                            }
                        }
                        if (dataMatepatyVP != null)
                        {
                            if (item.TDL_IMP_UNIT_ID.HasValue)
                            {
                                ImpMestMaterialSDODetail.PriceVP = (dataMatepatyVP.IMP_UNIT_EXP_PRICE ?? 0) * (1 + dataMatepatyVP.EXP_VAT_RATIO);
                            }
                            else
                            {
                                ImpMestMaterialSDODetail.PriceVP = dataMatepatyVP.EXP_PRICE * (1 + dataMatepatyVP.EXP_VAT_RATIO);
                            }
                        }
                    }
                    else if (dataMater != null && dataMater.IS_SALE_EQUAL_IMP_PRICE == 1)
                    {
                        if (item.TDL_IMP_UNIT_ID.HasValue)
                        {
                            ImpMestMaterialSDODetail.HienPrice = (dataMater.IMP_UNIT_PRICE ?? 0) * (1 + dataMater.IMP_VAT_RATIO);
                            ImpMestMaterialSDODetail.PriceVP = (dataMater.IMP_UNIT_PRICE ?? 0) * (1 + dataMater.IMP_VAT_RATIO);
                        }
                        else
                        {
                            ImpMestMaterialSDODetail.HienPrice = dataMater.IMP_PRICE * (1 + dataMater.IMP_VAT_RATIO);
                            ImpMestMaterialSDODetail.PriceVP = dataMater.IMP_PRICE * (1 + dataMater.IMP_VAT_RATIO);
                        }
                    }

                    impMestMaterials.Add(ImpMestMaterialSDODetail);
                }

                // sắp xếp theo id của thuốc
                if (impMestMaterials != null && impMestMaterials.Count > 0)
                {
                    impMestMaterials = impMestMaterials.OrderBy(o => o.ID).ToList();

                    var impMestMaterialGroup = impMestMaterials.GroupBy(o => new { o.MATERIAL_TYPE_ID, o.MATERIAL_ID, o.IMP_PRICE, o.IMP_VAT_RATIO, o.SERVICE_UNIT_ID }).Select(y =>
                    new
                    {
                        MATERIAL_TYPE_ID = y.First().MATERIAL_TYPE_ID,
                        AMOUNT = y.Sum(o => o.AMOUNT),
                        IMP_UNIT_AMOUNT = y.Sum(o => o.IMP_UNIT_AMOUNT ?? 0),
                        MATERIAL_ID = y.First().MATERIAL_ID,
                        IMP_PRICE = y.First().IMP_PRICE,
                        IMP_VAT_RATIO = y.First().IMP_VAT_RATIO,
                        SERVICE_UNIT_ID = y.First().SERVICE_UNIT_ID
                    }).ToList();
                    impMestMaterials = impMestMaterials.GroupBy(o => new { o.MATERIAL_TYPE_ID, o.MATERIAL_ID, o.IMP_PRICE, o.IMP_VAT_RATIO, o.SERVICE_UNIT_ID }).Select(o => o.First()).ToList();
                    foreach (var item in impMestMaterials)
                    {
                        var materialCheck = impMestMaterialGroup.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID && o.MATERIAL_ID == item.MATERIAL_ID && o.IMP_VAT_RATIO == item.IMP_VAT_RATIO && o.SERVICE_UNIT_ID == item.SERVICE_UNIT_ID);
                        if (materialCheck != null)
                        {
                            item.AMOUNT = materialCheck.AMOUNT;
                        }
                        if (this.expMestMaterials != null && this.expMestMaterials.Count > 0)
                        {
                            var checkMobaExpMestMaterial = this.expMestMaterials.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID);
                            if (checkMobaExpMestMaterial != null)
                            {
                                item.PRICE = checkMobaExpMestMaterial.PRICE;
                                item.VAT_RATIO = checkMobaExpMestMaterial.VAT_RATIO;
                            }
                        }
                        item.AMOUNT_OLD = item.AMOUNT;
                    }
                }
                gridControlMaterial.DataSource = impMestMaterials;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //load máu vào grid
        private void LoadDataToGridBlood()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisImpMestBloodViewFilter filter = new MOS.Filter.HisImpMestBloodViewFilter();
                filter.IMP_MEST_ID = this.ImpMestId;
                var impMestBloodAPIs = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_BLOOD>>(HisRequestUriStore.HIS_IMP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                impMestBloods = new List<ImpMestBloodSDODetail>();

                foreach (var item in impMestBloodAPIs)
                {
                    ImpMestBloodSDODetail result = new ImpMestBloodSDODetail(item);
                    impMestBloods.Add(result);
                }

                // sắp xếp theo thứ tự tăng dần của id
                if (impMestBloods != null && impMestBloods.Count > 0)
                {
                    impMestBloods = impMestBloods.OrderBy(o => o.ID).ToList();
                }
                gridControlBlood.DataSource = impMestBloods;
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event handler
        private void gridViewMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImpMestMedicineSDODetail dataRow = (ImpMestMedicineSDODetail)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "PRICE_DISPLAY")
                        {
                            try
                            {
                                if (dataRow.PRICE == null)
                                {
                                    e.Value = null;
                                }
                                else
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                                }

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_PRICE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE_IMP")
                        {
                            try
                            {
                                if (impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC
                                       && dataRow.DOCUMENT_PRICE.HasValue)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.DOCUMENT_PRICE.Value, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                                }
                                else
                                {
                                    decimal impPrice = dataRow.IMP_PRICE;
                                    decimal amount = dataRow.AMOUNT;
                                    decimal valueTotal = (impPrice * amount * (1 + dataRow.IMP_VAT_RATIO));
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(valueTotal, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_IMP_PRICE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency(dataRow.IMP_VAT_RATIO * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.VAT_RATIO ?? 0) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "PROFIT_RATIO_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.PROFIT_RATIO ?? 0) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong PROFIT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.IMP_PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.EXPIRED_DATE_EDIT ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImpMestMaterialSDODetail dataRow = (ImpMestMaterialSDODetail)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TOTAL_IMP_PRICE")
                        {
                            try
                            {
                                if (impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC
                                          && dataRow.DOCUMENT_PRICE.HasValue)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.DOCUMENT_PRICE.Value, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                                }
                                else
                                {
                                    decimal ImpPrice = dataRow.IMP_PRICE;
                                    decimal amount = dataRow.AMOUNT;
                                    decimal impVatRatio = dataRow.IMP_VAT_RATIO;
                                    decimal valueTotal = (ImpPrice * amount * (1 + impVatRatio));
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(valueTotal, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_PRICE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency(dataRow.IMP_VAT_RATIO * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.VAT_RATIO ?? 0) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "PROFIT_RATIO_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.PROFIT_RATIO ?? 0) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong PROFIT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.IMP_PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.EXPIRED_DATE_EDIT ?? 0);
                        }
                        else if (e.Column.FieldName == "PRICE_DISPLAY")
                        {
                            if (dataRow.PRICE == null)
                            {
                                e.Value = null;
                            }
                            else
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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

        private void gridViewBlood_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImpMestBloodSDODetail dataRow = (ImpMestBloodSDODetail)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        decimal AMOUNT = 0, IMP_PRICE = 0, IMP_VAT_RATIO = 0, sumPrice = 0;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                        {
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.IMP_PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            //TODO
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXPIRED_DATE ?? 0);
                        }
                        else if (e.Column.FieldName == "VAT_RATIO_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.VAT_RATIO ?? 0) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "SUM_PRICE_DISPLAY")
                        {
                            AMOUNT = 1;
                            IMP_PRICE = dataRow.IMP_PRICE;
                            IMP_VAT_RATIO = (dataRow.VAT_RATIO_100 ?? 0);
                            sumPrice = AMOUNT * IMP_PRICE * (IMP_VAT_RATIO + 100) / 100;
                            e.Value = Inventec.Common.Number.Convert.NumberToString(sumPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridViewMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);

                int medicineGroupId = Inventec.Common.TypeConvert.Parse.ToInt16((vw.GetRowCellValue(e.RowHandle, "MEDICINE_GROUP_ID") ?? "").ToString());// là thuốc gây nghiện
                if (medicineGroupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medicineGroupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                    e.Appearance.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRoleUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ImpMestId > 0)
                {
                    Inventec.Desktop.Common.Message.WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisRoleUser").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisRoleUser");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        MOS.EFMODEL.DataModels.HIS_IMP_MEST ado = new HIS_IMP_MEST();
                        ado.ID = this.ImpMestId;
                        ado.IMP_MEST_TYPE_ID = this.IMP_MEST_TYPE_ID;
                        ado.IMP_MEST_STT_ID = this.ImpMestSttId;
                        listArgs.Add(ado);
                        listArgs.Add(this.impMest);
                        listArgs.Add(this.impMestMaterials);
                        listArgs.Add(this.impMestMedicines);
                        listArgs.Add(this.impMestBloods);
                        listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                    }
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (EVImportMest, this.impMest);

                EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataAfterSave(apiresul);
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        V_HIS_IMP_MEST UpdateImpmest(MOS.EFMODEL.DataModels.HIS_IMP_MEST impMest, V_HIS_IMP_MEST impMestView)
        {
            try
            {
                MOS.Filter.HisImpMestViewFilter impMestFilter = new HisImpMestViewFilter();
                impMestFilter.ID = impMest.ID;
                var impMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, impMestFilter, new CommonParam());
                if (impMests != null && impMests.Count > 0)
                {
                    impMestView = impMests.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return impMestView;
        }

        private void FillDataAfterSave(object prescription)
        {
            try
            {
                if (prescription != null && prescription is HIS_IMP_MEST)
                {
                    this.impMest = UpdateImpmest((HIS_IMP_MEST)prescription, this.impMest);
                    this.ImpMestSttId = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                    SetDataToCommonControl();
                    this.EnableButton();
                    EnableGridColumn(IsAdmin);
                    loadDataToGridMaterial();
                    loadDataToGridMedicine();
                    LoadDataToGridBlood();
                    ValidateControlForm();
                    ShowTab();
                    InitMenuToButtonPrint();
                }
                if (prescription != null)
                {
                    delegateSelectData(prescription);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void EnableButton()
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_MEDI_STOCK medistock = null;

                Boolean btnApproveStt = true;
                Boolean btnImportStt = true;

                if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null)

                    btnApproveStt = true;
                else
                    btnApproveStt = false;


                if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnImport) != null)
                    btnImportStt = true;
                else
                    btnImportStt = false;

                // lấy kho đang làm việc
                if (this.moduleData != null)
                {
                    medistock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == this.moduleData.RoomId).FirstOrDefault();
                }
                if (medistock != null
                    && this.impMest.MEDI_STOCK_ID == medistock.ID
                    && this.impMest.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST
                    && this.impMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT
                    && btnApproveStt)
                    btnApproval.Enabled = true;
                else
                    btnApproval.Enabled = false;

                if (medistock != null && this.impMest.MEDI_STOCK_ID == medistock.ID
                          && this.impMest.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL
                          && this.impMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT
                          && this.impMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT
                          && btnImportStt)
                {
                    btnImport.Enabled = true;
                }
                else
                {
                    btnImport.Enabled = false;
                }

                btnSave.Enabled = controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == ControlCode.BtnSave);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (this.impMest != null && this.impMest.HAS_IDENTITY_MATERIAL == 1 && HisConfigCFG.IDENTITY_MATERIAL_OPTION == "2")
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.IdentityMaterialInformation").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.IdentityMaterialInformation'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        moduleData.RoomId = this.moduleData.RoomId;
                        moduleData.RoomTypeId = this.moduleData.RoomTypeId;
                        List<object> listArgs = new List<object>();
                        listArgs.Add(true);
                        listArgs.Add(this.impMest.ID);
                        listArgs.Add((HIS.Desktop.Common.DelegateImpTime)ProcessImpMest);
                        listArgs.Add(moduleData);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
                else
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonThucNhapDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (this.impMest != null)
                        {
                            WaitingManager.Show();
                            MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                            Inventec.Common.Mapper.DataObjectMapper.Map
                                <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                                (data, this.impMest);
                            data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                            var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                                ("api/HisImpMest/Import", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            if (apiresult != null)
                            {
                                success = true;
                                FillDataAfterSave(apiresult);
                            }
                            WaitingManager.Hide();
                            #region Show message
                            MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void ProcessImpMest(long? impTime)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (this.impMest != null)
                {
                    WaitingManager.Show();
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST data = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map
                        <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        (data, this.impMest);
                    data.IMP_TIME = Inventec.Common.DateTime.Get.Now();
                    data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                        ("api/HisImpMest/Import", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresult != null)
                    {
                        success = true;
                        FillDataAfterSave(apiresult);
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                CommonParam param = new CommonParam();
            }
        }

        private void bbtnApproval_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnApproval.Enabled)
                {
                    btnApproval_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                {
                    btnImport_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var focus = (ImpMestMedicineSDODetail)gridViewMedicine.GetFocusedRow();
                if (focus != null)
                {
                    if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        if (gridViewMedicine.EditingValue is DateTime)
                        {
                            var dt = (DateTime)gridViewMedicine.EditingValue;
                            if (dt == null || dt == DateTime.MinValue)
                            {
                                focus.EXPIRED_DATE_EDIT = null;
                            }
                            else if ((Convert.ToInt64(dt.ToString("yyyyMMdd") + "235959")) < (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Hạn sử dụng không được nhỏ hơn ngày hiện tại");
                                return;
                            }
                            else
                            {
                                focus.EXPIRED_DATE_EDIT = Convert.ToInt64(dt.ToString("yyyyMMdd") + "235959");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ItemGridLookUpEdit_MedicineName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                GridLookUpEdit lookupEdit = sender as GridLookUpEdit;

                if (e.CloseMode == PopupCloseMode.Normal && lookupEdit != null)
                {
                    var MedicineTypeSearch = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == Int64.Parse(lookupEdit.EditValue.ToString()));
                    if (MedicineTypeSearch != null)
                    {
                        gridViewMedicine.SetFocusedRowCellValue(gridColumnMedicineTypeName, MedicineTypeSearch.ID);
                        gridViewMedicine.SetFocusedRowCellValue(gridColumnMedicineTypeCode, MedicineTypeSearch.MEDICINE_TYPE_CODE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ItemTextEdit_MedicineTypeCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var focus = (ImpMestMedicineSDODetail)gridViewMedicine.GetFocusedRow();
                TextEdit txtMedicineType = sender as TextEdit;
                if (e.KeyCode == Keys.Enter && !String.IsNullOrWhiteSpace(txtMedicineType.Text) && this.medicineTypes != null && this.medicineTypes.Count() > 0)
                {
                    var MedicineTypeSearch = this.medicineTypes.FirstOrDefault(o => o.MEDICINE_TYPE_CODE.ToLower() == txtMedicineType.Text.Trim().ToLower());
                    if (MedicineTypeSearch != null)
                    {
                        gridViewMedicine.SetFocusedRowCellValue(gridColumnMedicineTypeName, MedicineTypeSearch.ID);
                        gridViewMedicine.SetFocusedRowCellValue(gridColumnMedicineTypeCode, MedicineTypeSearch.MEDICINE_TYPE_CODE);
                    }
                    else
                    {
                        gridViewMedicine.SetFocusedRowCellValue(gridColumnMedicineTypeName, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ItemTextEdit_MaterialCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var focus = (ImpMestMaterialSDODetail)gridViewMaterial.GetFocusedRow();
                TextEdit txtMaterialType = sender as TextEdit;
                if (e.KeyCode == Keys.Enter && !String.IsNullOrWhiteSpace(txtMaterialType.Text) && this.materialTypes != null && this.materialTypes.Count() > 0)
                {
                    var MaterialTypeSearch = this.materialTypes.FirstOrDefault(o => o.MATERIAL_TYPE_CODE.ToLower() == txtMaterialType.Text.Trim().ToLower());
                    if (MaterialTypeSearch != null)
                    {
                        gridViewMaterial.SetFocusedRowCellValue(gridColumnMateMaterialTypeName, MaterialTypeSearch.ID);
                        gridViewMaterial.SetFocusedRowCellValue(gridColumnMateMaterialTypeCode, MaterialTypeSearch.MATERIAL_TYPE_CODE);
                    }
                    else
                    {
                        gridViewMaterial.SetFocusedRowCellValue(gridColumnMateMaterialTypeName, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ItemGridLookUpEdit_MaterialName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                GridLookUpEdit lookupEdit = sender as GridLookUpEdit;

                if (e.CloseMode == PopupCloseMode.Normal && lookupEdit != null)
                {
                    var MaterialTypeSearch = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == Int64.Parse(lookupEdit.EditValue.ToString()));
                    if (MaterialTypeSearch != null)
                    {
                        gridViewMaterial.SetFocusedRowCellValue(gridColumnMateMaterialTypeName, MaterialTypeSearch.ID);
                        gridViewMaterial.SetFocusedRowCellValue(gridColumnMateMaterialTypeCode, MaterialTypeSearch.MATERIAL_TYPE_CODE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ItemTextEdit_BloodCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var focus = (ImpMestBloodSDODetail)gridViewBlood.GetFocusedRow();
                TextEdit txtBloodType = sender as TextEdit;
                if (e.KeyCode == Keys.Enter && !String.IsNullOrWhiteSpace(txtBloodType.Text))
                {
                    var BloodTypeSearch = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(o => o.BLOOD_TYPE_CODE.ToLower() == txtBloodType.Text.Trim().ToLower());
                    if (BloodTypeSearch != null)
                    {
                        gridViewBlood.SetFocusedRowCellValue(gridColumnBloodBloodTypeName, BloodTypeSearch.ID);
                        gridViewBlood.SetFocusedRowCellValue(gridColumnBloodBloodTypeCode, BloodTypeSearch.BLOOD_TYPE_CODE);
                    }
                    else
                    {
                        gridViewBlood.SetFocusedRowCellValue(gridColumnBloodBloodTypeName, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ItemGridLookUpEdit_BloodName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                GridLookUpEdit lookupEdit = sender as GridLookUpEdit;

                if (e.CloseMode == PopupCloseMode.Normal && lookupEdit != null)
                {
                    var BloodTypeSearch = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(o => o.ID == Int64.Parse(lookupEdit.EditValue.ToString()));
                    if (BloodTypeSearch != null)
                    {
                        gridViewBlood.SetFocusedRowCellValue(gridColumnBloodBloodTypeName, BloodTypeSearch.ID);
                        gridViewBlood.SetFocusedRowCellValue(gridColumnBloodBloodTypeCode, BloodTypeSearch.BLOOD_TYPE_CODE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ImpMestMedicineSDODetail data = view.GetFocusedRow() as ImpMestMedicineSDODetail;
                if (view.Columns.ColumnByFieldName("MEDICINE_TYPE_ID_EDIT") == gridColumnMedicineTypeName && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null && data.MEDICINE_TYPE_ID_EDIT == 0 && !data.TDL_IMP_UNIT_ID.HasValue)
                    {
                        editor.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "MEDICINE_TYPE_ID_EDIT" || e.ColumnName == "PRICE" || e.ColumnName == "VAT_RATIO_100" || e.ColumnName == "AMOUNT")
                {
                    this.gridViewMedicine_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewMedicine.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlMedicine.DataSource as List<ImpMestMedicineSDODetail>;
                var row = listDatas[index];
                if (e.ColumnName == "MEDICINE_TYPE_ID_EDIT")
                {
                    if (row.MEDICINE_TYPE_ID_EDIT == 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Trường dữ liệu bắt buộc";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                if (e.ColumnName == "PRICE")
                {
                    if (row.PRICE < 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Giá trị không được âm";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                if (e.ColumnName == "VAT_RATIO_100")
                {
                    if (row.VAT_RATIO_100 < 0 || row.VAT_RATIO_100 > 100)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Giá trị trong khoảng [0 - 100]";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                if (e.ColumnName == "AMOUNT")
                {
                    if (row.AMOUNT <= 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Giá trị > 0";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewMaterial.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlMaterial.DataSource as List<ImpMestMaterialSDODetail>;
                var row = listDatas[index];
                if (e.ColumnName == "MATERIAL_TYPE_ID_EDIT")
                {
                    if (row.MATERIAL_TYPE_ID_EDIT == 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Trường dữ liệu bắt buộc";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                if (e.ColumnName == "PRICE")
                {
                    if (row.PRICE < 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Giá trị không được âm";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                if (e.ColumnName == "VAT_RATIO_100")
                {
                    if (row.VAT_RATIO_100 < 0 || row.VAT_RATIO_100 > 100)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Giá trị trong khoảng [0 - 100]";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                if (e.ColumnName == "AMOUNT")
                {
                    if (row.AMOUNT <= 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Giá trị > 0";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBlood_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewBlood.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlMedicine.DataSource as List<ImpMestBloodSDODetail>;
                var row = listDatas[index];
                if (e.ColumnName == "BLOOD_TYPE_ID_EDIT")
                {
                    if (row.BLOOD_TYPE_ID_EDIT == 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Trường dữ liệu bắt buộc";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                if (e.ColumnName == "PRICE")
                {
                    if (row.PRICE < 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Giá trị không được âm";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                if (e.ColumnName == "VAT_RATIO_100")
                {
                    if (row.VAT_RATIO_100 < 0 || row.VAT_RATIO_100 > 100)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Giá trị trong khoảng [0 - 100]";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.gridViewMedicine.IsEditing)
                    this.gridViewMedicine.CloseEditor();

                if (this.gridViewMedicine.FocusedRowModified)
                    this.gridViewMedicine.UpdateCurrentRow();

                if (this.gridViewMaterial.IsEditing)
                    this.gridViewMaterial.CloseEditor();

                if (this.gridViewMaterial.FocusedRowModified)
                    this.gridViewMaterial.UpdateCurrentRow();

                if (this.gridViewBlood.IsEditing)
                    this.gridViewBlood.CloseEditor();

                if (this.gridViewBlood.FocusedRowModified)
                    this.gridViewBlood.UpdateCurrentRow();

                if (!btnSave.Enabled)
                {
                    return;
                }

                if (this.impMest.IMP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chỉ cho phép sửa với phiếu đã nhập", Resources.ResourceMessage.ThongBao);
                    return;
                }

                if (this.impMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && this.impMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK && this.impMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK && this.impMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chỉ cho phép sửa với các loại phiếu nhập: Nhập NCC, Nhập kiểm kê, Nhập đầu kỳ, Nhập khác", Resources.ResourceMessage.ThongBao);
                    return;
                }

                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                String errMessage = "";

                List<ImpMestMedicineSDODetail> impMestMedicines = (List<ImpMestMedicineSDODetail>)gridViewMedicine.DataSource;
                List<ImpMestMaterialSDODetail> impMestMaterials = (List<ImpMestMaterialSDODetail>)gridViewMaterial.DataSource;
                List<ImpMestBloodSDODetail> impMestBloods = (List<ImpMestBloodSDODetail>)gridViewBlood.DataSource;

                List<ImpMestMedicineSDODetail> impMestMedicineCheckCode = impMestMedicines.Where(o => o.MEDICINE_TYPE_ID_EDIT == 0).ToList();
                List<ImpMestMedicineSDODetail> impMestMedicineCheckPrice = impMestMedicines.Where(o => o.PRICE < 0).ToList();

                List<ImpMestMedicineSDODetail> impMestMedicineCheckAMOUNT = impMestMedicines.Where(o => o.AMOUNT <= 0).ToList();

                List<ImpMestMedicineSDODetail> impMestMedicineCheckVAT = impMestMedicines.Where(o => o.VAT_RATIO_100 < 0 || o.VAT_RATIO_100 > 100).ToList();

                List<ImpMestMaterialSDODetail> impMestMaterialCheckCode = impMestMaterials.Where(o => o.MATERIAL_TYPE_ID_EDIT == 0).ToList();
                List<ImpMestMaterialSDODetail> impMestMaterialCheckPrice = impMestMaterials.Where(o => o.PRICE < 0).ToList();
                List<ImpMestMaterialSDODetail> impMestMaterialCheckAmount = impMestMaterials.Where(o => o.AMOUNT <= 0).ToList();
                List<ImpMestMaterialSDODetail> impMestMaterialCheckVAT = impMestMaterials.Where(o => o.VAT_RATIO_100 < 0 || o.VAT_RATIO_100 > 100).ToList();

                List<ImpMestBloodSDODetail> impMestBloodCheckCode = impMestBloods.Where(o => o.BLOOD_TYPE_ID_EDIT == 0).ToList();
                List<ImpMestBloodSDODetail> impMestBloodCheckPrice = impMestBloods.Where(o => o.PRICE < 0).ToList();
                List<ImpMestBloodSDODetail> impMestBloodCheckVAT = impMestBloods.Where(o => o.VAT_RATIO_100 < 0 || o.VAT_RATIO_100 > 100).ToList();

                if (impMestMedicineCheckCode != null && impMestMedicineCheckCode.Count() > 0)
                {
                    errMessage += " mã thuốc: ";
                    errMessage += String.Join("; ", impMestMedicineCheckCode.Select(o => o.MEDICINE_TYPE_CODE));
                }
                if (impMestMedicineCheckPrice != null && impMestMedicineCheckPrice.Count() > 0)
                {
                    errMessage += " đơn giá thuốc: ";
                    errMessage += String.Join("; ", impMestMedicineCheckPrice.Select(o => o.PRICE));
                }
                if (impMestMedicineCheckVAT != null && impMestMedicineCheckVAT.Count() > 0)
                {
                    errMessage += " VAT thuốc: ";
                    errMessage += String.Join("; ", impMestMedicineCheckVAT.Select(o => o.VAT_RATIO_100));
                }
                if (impMestMedicineCheckAMOUNT != null && impMestMedicineCheckAMOUNT.Count() > 0)
                {
                    errMessage += " số lượng thuốc: ";
                    errMessage += String.Join("; ", impMestMedicineCheckAMOUNT.Select(o => o.AMOUNT));
                }

                if (impMestMaterialCheckCode != null && impMestMaterialCheckCode.Count() > 0)
                {
                    errMessage += " mã vật tư: ";
                    errMessage += String.Join("; ", impMestMaterialCheckCode.Select(o => o.MATERIAL_TYPE_CODE));
                }
                if (impMestMaterialCheckPrice != null && impMestMaterialCheckPrice.Count() > 0)
                {
                    errMessage += " đơn giá vật tư: ";
                    errMessage += String.Join("; ", impMestMaterialCheckPrice.Select(o => o.PRICE));
                }
                if (impMestMaterialCheckVAT != null && impMestMaterialCheckVAT.Count() > 0)
                {
                    errMessage += " VAT vật tư: ";
                    errMessage += String.Join("; ", impMestMaterialCheckVAT.Select(o => o.VAT_RATIO_100));
                }
                if (impMestMaterialCheckAmount != null && impMestMaterialCheckAmount.Count() > 0)
                {
                    errMessage += " số lượng vật tư: ";
                    errMessage += String.Join("; ", impMestMaterialCheckAmount.Select(o => o.AMOUNT));
                }
                if (impMestBloodCheckCode != null && impMestBloodCheckCode.Count() > 0)
                {
                    errMessage += " mã máu: ";
                    errMessage += String.Join("; ", impMestBloodCheckCode.Select(o => o.BLOOD_TYPE_CODE));
                }
                if (impMestBloodCheckPrice != null && impMestBloodCheckPrice.Count() > 0)
                {
                    errMessage += " đơn giá máu: ";
                    errMessage += String.Join("; ", impMestBloodCheckPrice.Select(o => o.PRICE));
                }
                if (impMestBloodCheckVAT != null && impMestBloodCheckVAT.Count() > 0)
                {
                    errMessage += " VAT giá máu: ";
                    errMessage += String.Join("; ", impMestBloodCheckVAT.Select(o => o.VAT_RATIO_100));
                }

                if (!String.IsNullOrWhiteSpace(errMessage))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu không hợp lệ: " + errMessage, Resources.ResourceMessage.ThongBao);
                    return;
                }

                List<HisImpMestBloodSDO> impMestBloodSDOs = new List<HisImpMestBloodSDO>();
                if (impMestBloods != null && impMestBloods.Count() > 0)
                {
                    foreach (var item in impMestBloods)
                    {
                        if (item.BLOOD_TYPE_ID_EDIT == item.BLOOD_TYPE_ID && item.VAT_RATIO_100_OLD == item.VAT_RATIO_100 && item.IMP_PRICE == item.IMP_PRICE_OLD && item.PACKAGE_NUMBER == item.PACKAGE_NUMBER_EDIT && item.EXPIRED_DATE == item.EXPIRED_DATE_EDIT)
                            continue;

                        HisImpMestBloodSDO hisImpMestBloodSDO = new HisImpMestBloodSDO();
                        hisImpMestBloodSDO.BloodTypeId = item.BLOOD_TYPE_ID_EDIT;
                        hisImpMestBloodSDO.Id = item.ID;
                        hisImpMestBloodSDO.ImpPrice = item.IMP_PRICE;
                        hisImpMestBloodSDO.ImpVatRatio = (item.VAT_RATIO_100 ?? 0) / 100;
                        //hisImpMestBloodSDO.ExpireDate = item.EXPIRED_DATE_EDIT;
                        //hisImpMestBloodSDO.PackageNumber = item.PACKAGE_NUMBER_EDIT;
                        impMestBloodSDOs.Add(hisImpMestBloodSDO);
                    }
                }

                List<HisImpMestMaterialSDO> impMestMaterialSDOs = new List<HisImpMestMaterialSDO>();
                if (impMestMaterials != null && impMestMaterials.Count() > 0)
                {
                    foreach (var item in impMestMaterials)
                    {
                        if (item.MATERIAL_TYPE_ID_EDIT == item.MATERIAL_TYPE_ID && item.VAT_RATIO_100_OLD == item.VAT_RATIO_100 && item.PRICE == item.PRICE_OLD && item.PACKAGE_NUMBER == item.PACKAGE_NUMBER_EDIT && item.EXPIRED_DATE == item.EXPIRED_DATE_EDIT && item.TOTAL_PRICE == item.TOTAL_PRICE_OLD)
                            continue;

                        HisImpMestMaterialSDO hisImpMestMaterialSDO = new HisImpMestMaterialSDO();
                        hisImpMestMaterialSDO.MaterialTypeId = item.MATERIAL_TYPE_ID_EDIT;
                        hisImpMestMaterialSDO.Amount = item.AMOUNT;
                        hisImpMestMaterialSDO.Id = item.ID;
                        hisImpMestMaterialSDO.ImpPrice = item.PRICE ?? 0;
                        hisImpMestMaterialSDO.ImpVatRatio = (item.VAT_RATIO_100 ?? 0) / 100;
                        hisImpMestMaterialSDO.ExpireDate = item.EXPIRED_DATE_EDIT;
                        hisImpMestMaterialSDO.PackageNumber = item.PACKAGE_NUMBER;
                        if (impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                        {
                            hisImpMestMaterialSDO.DocumentPrice = Inventec.Common.TypeConvert.Parse.ToInt64(Math.Round(item.TOTAL_PRICE, 0).ToString());
                        }

                        impMestMaterialSDOs.Add(hisImpMestMaterialSDO);
                    }
                }

                List<HisImpMestMedicineSDO> impMestMedicineSDOs = new List<HisImpMestMedicineSDO>();
                if (impMestMedicines != null && impMestMedicines.Count() > 0)
                {
                    foreach (var item in impMestMedicines)
                    {
                        if (item.MEDICINE_TYPE_ID_EDIT == item.MEDICINE_TYPE_ID && item.VAT_RATIO_100_OLD == item.VAT_RATIO_100 && item.PRICE == item.PRICE_OLD && item.PACKAGE_NUMBER == item.PACKAGE_NUMBER_EDIT && item.EXPIRED_DATE == item.EXPIRED_DATE_EDIT && item.TOTAL_PRICE == item.TOTAL_PRICE_OLD && item.TEMPERATURE_OLD == item.TEMPERATURE)
                            continue;

                        HisImpMestMedicineSDO hisImpMestMedicineSDO = new HisImpMestMedicineSDO();
                        hisImpMestMedicineSDO.MedicineTypeId = item.MEDICINE_TYPE_ID_EDIT;
                        hisImpMestMedicineSDO.Amount = item.AMOUNT;
                        hisImpMestMedicineSDO.Id = item.ID;
                        hisImpMestMedicineSDO.ImpPrice = item.PRICE ?? 0;
                        hisImpMestMedicineSDO.ImpVatRatio = (item.VAT_RATIO_100 ?? 0) / 100;
                        hisImpMestMedicineSDO.ExpireDate = item.EXPIRED_DATE_EDIT;
                        hisImpMestMedicineSDO.PackageNumber = item.PACKAGE_NUMBER;
                        hisImpMestMedicineSDO.Temperature = item.TEMPERATURE;
                        if (impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                        {
                            hisImpMestMedicineSDO.DocumentPrice = Inventec.Common.TypeConvert.Parse.ToInt64(Math.Round(item.TOTAL_PRICE, 0).ToString());
                        }

                        impMestMedicineSDOs.Add(hisImpMestMedicineSDO);
                    }
                }

                CommonParam param = new CommonParam();
                bool success = false;
                HisImpMestUpdateDetailSDO hisImpMestUpdateDetailSDO = new MOS.SDO.HisImpMestUpdateDetailSDO();
                hisImpMestUpdateDetailSDO.ImpMestBloods = new List<HisImpMestBloodSDO>();
                hisImpMestUpdateDetailSDO.ImpMestId = this.impMest.ID;
                hisImpMestUpdateDetailSDO.ImpMestMaterials = new List<HisImpMestMaterialSDO>();
                hisImpMestUpdateDetailSDO.ImpMestMedicines = new List<HisImpMestMedicineSDO>();
                hisImpMestUpdateDetailSDO.ReqestRoomId = moduleData.RoomId;

                hisImpMestUpdateDetailSDO.ImpMestBloods = impMestBloodSDOs;
                hisImpMestUpdateDetailSDO.ImpMestMaterials = impMestMaterialSDOs;
                hisImpMestUpdateDetailSDO.ImpMestMedicines = impMestMedicineSDOs;

                hisImpMestUpdateDetailSDO.Deliverer = TxtDeliverer.Text;
                hisImpMestUpdateDetailSDO.Description = txtDescription.Text;
                hisImpMestUpdateDetailSDO.DocumentNumber = TxtDocumentNumber.Text;

                HisImpMestFilter filter = new HisImpMestFilter();
                filter.DOCUMENT_NUMBER__EXACT = hisImpMestUpdateDetailSDO.DocumentNumber;
                filter.SUPPLIER_ID = this.impMest.SUPPLIER_ID;

                var apiResult = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_IMP_MEST>>("api/HisImpMest/Get",
                   ApiConsumers.MosConsumer, filter, param);

                apiResult = apiResult != null ? apiResult.Where(o => o.ID != this.impMest.ID && (o.DOCUMENT_NUMBER != "" || o.DOCUMENT_NUMBER != null)).ToList() : null;
                List<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK> mediStocks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>();
                var IdStock = apiResult != null ? apiResult.Select(item => item.MEDI_STOCK_ID).Distinct().ToList() : null;
                var stockName = mediStocks != null ? mediStocks.Where(it => IdStock.Contains(it.ID)).ToList() : null;
                if (apiResult != null && apiResult.Count > 0 && (TxtDocumentNumber.Text != null && TxtDocumentNumber.Text != ""))
                {
                    string messCode = string.Join(", ", apiResult.Where(it => it.DOCUMENT_NUMBER == TxtDocumentNumber.Text).Select(item => item.IMP_MEST_CODE).Distinct());
                    string stock = string.Join(", ", stockName.Select(it => it.MEDI_STOCK_NAME).Distinct());
                    this.IsAllowDuplicateDocument = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AllowDuplicateDocumentNumberInTheSameSupplier") == "1" ? true : false;

                    if (IsAllowDuplicateDocument)
                    {
                        if (messCode != null)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(
                            "Tồn tại phiếu nhập " + messCode + " có số chứng từ " + hisImpMestUpdateDetailSDO.DocumentNumber + " tại " + stock,
                            Resources.ResourceMessage.ThongBaoTrungSoChungtu,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                if (SpDiscount.EditValue != null)
                                {
                                    hisImpMestUpdateDetailSDO.DiscountPrice = SpDiscount.Value;
                                }

                                if (SpDiscountRatio.EditValue != null)
                                {
                                    hisImpMestUpdateDetailSDO.DiscountRatio = SpDiscountRatio.Value / 100;
                                }

                                if (dtDocumentDate.EditValue != null && dtDocumentDate.DateTime != DateTime.MinValue)
                                {
                                    hisImpMestUpdateDetailSDO.DocumentDate = Convert.ToInt64(dtDocumentDate.DateTime.ToString("yyyyMMddHHmmss"));
                                }

                                if (SpDocumentPrice.EditValue != null)
                                {
                                    hisImpMestUpdateDetailSDO.DocumentPrice = SpDocumentPrice.Value;
                                }

                                if ((hisImpMestUpdateDetailSDO.ImpMestBloods == null || hisImpMestUpdateDetailSDO.ImpMestBloods.Count() == 0)
                                    && (hisImpMestUpdateDetailSDO.ImpMestMaterials == null || hisImpMestUpdateDetailSDO.ImpMestMaterials.Count() == 0)
                                    && (hisImpMestUpdateDetailSDO.ImpMestMedicines == null || hisImpMestUpdateDetailSDO.ImpMestMedicines.Count() == 0)
                                    && hisImpMestUpdateDetailSDO.Deliverer == impMest.DELIVERER
                                    && hisImpMestUpdateDetailSDO.Description == impMest.DESCRIPTION
                                    && hisImpMestUpdateDetailSDO.DiscountPrice == impMest.DISCOUNT
                                    && hisImpMestUpdateDetailSDO.DiscountRatio == impMest.DISCOUNT_RATIO
                                    && hisImpMestUpdateDetailSDO.DocumentDate == impMest.DOCUMENT_DATE
                                    && hisImpMestUpdateDetailSDO.DocumentNumber == impMest.DOCUMENT_NUMBER
                                    && hisImpMestUpdateDetailSDO.DocumentPrice == impMest.DOCUMENT_PRICE
                                    )
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu để sửa", Resources.ResourceMessage.ThongBao);
                                    return;
                                }

                                var res = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestResultSDO>("api/HisImpMest/UpdateDetail", ApiConsumers.MosConsumer, hisImpMestUpdateDetailSDO, param);
                                if (res != null)
                                {
                                    success = true;
                                    loadDataToGridMaterial();
                                    loadDataToGridMedicine();
                                    LoadDataToGridBlood();
                                    FillDataAfterSave(res);
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại phiếu nhập" + messCode +
                            " có số chứng từ " + hisImpMestUpdateDetailSDO.DocumentNumber + " tại " +
                            stock, Resources.ResourceMessage.ThongBaoTrungSoChungtu);
                        return;
                    }
                }
                if (SpDiscount.EditValue != null)
                {
                    hisImpMestUpdateDetailSDO.DiscountPrice = SpDiscount.Value;
                }

                if (SpDiscountRatio.EditValue != null)
                {
                    hisImpMestUpdateDetailSDO.DiscountRatio = SpDiscountRatio.Value / 100;
                }

                if (dtDocumentDate.EditValue != null && dtDocumentDate.DateTime != DateTime.MinValue)
                {
                    hisImpMestUpdateDetailSDO.DocumentDate = Convert.ToInt64(dtDocumentDate.DateTime.ToString("yyyyMMddHHmmss"));
                }

                if (SpDocumentPrice.EditValue != null)
                {
                    hisImpMestUpdateDetailSDO.DocumentPrice = SpDocumentPrice.Value;
                }

                if ((hisImpMestUpdateDetailSDO.ImpMestBloods == null || hisImpMestUpdateDetailSDO.ImpMestBloods.Count() == 0)
                    && (hisImpMestUpdateDetailSDO.ImpMestMaterials == null || hisImpMestUpdateDetailSDO.ImpMestMaterials.Count() == 0)
                    && (hisImpMestUpdateDetailSDO.ImpMestMedicines == null || hisImpMestUpdateDetailSDO.ImpMestMedicines.Count() == 0)
                    && hisImpMestUpdateDetailSDO.Deliverer == impMest.DELIVERER
                    && hisImpMestUpdateDetailSDO.Description == impMest.DESCRIPTION
                    && hisImpMestUpdateDetailSDO.DiscountPrice == impMest.DISCOUNT
                    && hisImpMestUpdateDetailSDO.DiscountRatio == impMest.DISCOUNT_RATIO
                    && hisImpMestUpdateDetailSDO.DocumentDate == impMest.DOCUMENT_DATE
                    && hisImpMestUpdateDetailSDO.DocumentNumber == impMest.DOCUMENT_NUMBER
                    && hisImpMestUpdateDetailSDO.DocumentPrice == impMest.DOCUMENT_PRICE
                    )
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu để sửa", Resources.ResourceMessage.ThongBao);
                    return;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("hisImpMestUpdateDetailSDO__:", hisImpMestUpdateDetailSDO));
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestResultSDO>("api/HisImpMest/UpdateDetail",
                    ApiConsumers.MosConsumer, hisImpMestUpdateDetailSDO, param);
                if (rs != null)
                {
                    success = true;
                    loadDataToGridMaterial();
                    loadDataToGridMedicine();
                    LoadDataToGridBlood();
                    FillDataAfterSave(rs);
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "MATERIAL_TYPE_ID_EDIT" || e.ColumnName == "PRICE" || e.ColumnName == "VAT_RATIO_100" || e.ColumnName == "AMOUNT")
                {
                    this.gridViewMaterial_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBlood_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "BLOOD_TYPE_ID_EDIT" || e.ColumnName == "PRICE" || e.ColumnName == "VAT_RATIO_100" || e.ColumnName == "AMOUNT")
                {
                    this.gridViewBlood_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ItemGridLookUpEdit_MedicineName_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                if (e.Value != null && HisConfigCFG.IS_JOIN_NAME_WITH_CONCENTRA)
                {
                    long id = Convert.ToInt64(e.Value);
                    V_HIS_MEDICINE_TYPE type = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == id);
                    if (type != null)
                    {
                        e.DisplayText = String.Format("{0} {1}", type.MEDICINE_TYPE_NAME, type.CONCENTRA);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtDocumentDate.EditValue = dt;
                        dtDocumentDate.Update();
                        //txtDeliverer.Focus();
                        //txtDeliverer.SelectAll();
                        dtDocumentDate.Visible = true;
                        dtDocumentDate.Focus();
                        dtDocumentDate.ShowPopup();
                    }
                    else
                    {
                        dtDocumentDate.Visible = true;
                        dtDocumentDate.Focus();
                        dtDocumentDate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = Resources.ResourceMessage.NguoiDungNhapNgayKhongHopLe;
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDocumentDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                    SpDocumentPrice.Focus();
                    SpDocumentPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_Leave(object sender, EventArgs e)
        {
            try
            {
                dtDocumentDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                SpDocumentPrice.Focus();
                SpDocumentPrice.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtDocumentDate.Text))
                    {
                        dtDocumentDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                        SpDocumentPrice.Focus();
                        SpDocumentPrice.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtDocumentDate.EditValue = dt;
                        dtDocumentDate.Update();
                        SpDocumentPrice.Focus();
                        SpDocumentPrice.SelectAll();
                    }
                    else
                    {
                        dtDocumentDate.Visible = true;
                        dtDocumentDate.Focus();
                        dtDocumentDate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();

                if (!String.IsNullOrEmpty(currentValue))
                {
                    int day = Int16.Parse(currentValue.Substring(0, 2));
                    int month = Int16.Parse(currentValue.Substring(3, 2));
                    int year = Int16.Parse(currentValue.Substring(6, 4));
                    if (day < 0 || day > 31 || month < 0 || month > 12 || year < 1000 || year > DateTime.Now.Year)
                    {
                        //e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDate_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtDocumentDate.Visible = false;
                    if (dtDocumentDate.DateTime != DateTime.MinValue)
                    {
                        txtDocumentDate.Text = dtDocumentDate.DateTime.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        txtDocumentDate.Text = "";
                    }
                    SpDocumentPrice.Focus();
                    SpDocumentPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDocumentDate.Visible = false;
                    dtDocumentDate.Update();
                    txtDocumentDate.Text = dtDocumentDate.DateTime.ToString("dd/MM/yyyy");
                    SpDocumentPrice.Focus();
                    SpDocumentPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtDocumentNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDocumentDate.Focus();
                    txtDocumentDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SpDocumentPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtDeliverer.Focus();
                    TxtDeliverer.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtDeliverer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SpDiscount.Focus();
                    SpDiscount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SpDiscount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SpDiscountRatio.Focus();
                    SpDiscountRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SpDiscountRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var focus = (ImpMestMaterialSDODetail)gridViewMaterial.GetFocusedRow();
                if (focus != null && e.Column.FieldName == "EXPIRED_DATE_STR" && gridViewMaterial.EditingValue is DateTime)
                {
                    var dt = (DateTime)gridViewMaterial.EditingValue;
                    if (dt == null || dt == DateTime.MinValue)
                    {
                        focus.EXPIRED_DATE_EDIT = null;
                    }
                    else if ((Convert.ToInt64(dt.ToString("yyyyMMdd") + "235959")) < (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Hạn sử dụng không được nhỏ hơn ngày hiện tại");
                        return;
                    }
                    else
                    {
                        focus.EXPIRED_DATE_EDIT = Convert.ToInt64(dt.ToString("yyyyMMdd") + "235959");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ImpMestMaterialSDODetail data = view.GetFocusedRow() as ImpMestMaterialSDODetail;
                if (view.Columns.ColumnByFieldName("MATERIAL_TYPE_ID_EDIT") == gridColumnMedicineTypeName && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null && data.MATERIAL_TYPE_ID_EDIT == 0 && !data.TDL_IMP_UNIT_ID.HasValue)
                    {
                        editor.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                ImpMestMedicineSDODetail data = (ImpMestMedicineSDODetail)gridViewMedicine.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "MEDICINE_TYPE_ID_EDIT")
                    {
                        if (data.TDL_IMP_UNIT_ID.HasValue)
                        {
                            e.RepositoryItem = repositoryItemGridLookUpEdit_Medicine_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = ItemGridLookUpEdit_MedicineName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                ImpMestMaterialSDODetail data = (ImpMestMaterialSDODetail)gridViewMaterial.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "MATERIAL_TYPE_ID_EDIT")
                    {
                        if (data.TDL_IMP_UNIT_ID.HasValue)
                        {
                            e.RepositoryItem = repositoryItemGridLookUpEdit_Material_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = ItemGridLookUpEdit_MaterialName;
                        }
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

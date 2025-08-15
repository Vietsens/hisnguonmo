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
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.Controls.Session;
using System.Drawing;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using HIS.Desktop.Common;
using System.Globalization;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using SDA.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Utilities.Extensions;
using MOS.SDO;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate
{
    public partial class frmMaterialTypeCreate : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int positionHandleControlMedicineTypeInfo = -1;
        int ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;// mac dinh la them
        long? materialTypeId = null;
        Inventec.Desktop.Common.Modules.Module module;
        MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE currentVHisMaterialTypeDTODefault;
        //MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE currentVHisMaterialTypeDTODefault_;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE currentVHisServiceDTODefault;
        V_HIS_SERVICE currentRightClick;
        HIS_MATERIAL_TYPE resultData = null;
        DelegateSelectData delegateSelect;

        HIS.UC.National.NationalProcessor nationalProcessor;
        UserControl ucNational;
        V_HIS_MATERIAL_TYPE HisMaterial;
        internal List<ADO.VHisServicePatyADO> lsVHisServicePaty { get; set; }
        internal List<ADO.VHisServicePatyADO> lsVHisServicePatyBegin = new List<ADO.VHisServicePatyADO>();
        internal List<HIS_SERVICE_PATY> ServicePatyCreate { get; set; }
        internal List<HIS_SERVICE_PATY> ServicePatyUpdate { get; set; }
        HIS_BRANCH BranchID;

        private List<HIS_DEPARTMENT> BlockDepartment__Seleced = new List<HIS_DEPARTMENT>();
        private List<long> oldBlockDepartmentIds = null;
        private List<HIS_MATERIAL_TYPE> MaterialTypeMap__Seleced = new List<HIS_MATERIAL_TYPE>();
        private List<long> oldMaterialTypeMapIds = null;
        //  private List<long> oldMaterialTypeMapIds_ = null;

        //
        private List<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT> ServiceUnitList { get; set; }
        private List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE> MaterialTypeList { get; set; }

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.MaterialTypeCreate";

        #endregion

        #region Contructor
        public frmMaterialTypeCreate(Inventec.Desktop.Common.Modules.Module _module, long? _materialTypeId, int _actionType, DelegateSelectData _delegateSelect)
            : base(_module)
        {
            try
            {
                InitializeComponent();
                this.materialTypeId = _materialTypeId;
                this.module = _module;
                delegateSelect = _delegateSelect;
                // mac dinh la them moi du lieu, neu sua thi gan lai actionType
                if (materialTypeId != null && materialTypeId > 0)
                {
                    this.ActionType = _actionType;
                }
                else
                {
                    rdoUpdateAll.Enabled = false;
                    rdoUpdateNotFee.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        void SendDataAfterSave()
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(resultData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMaterialTypeCreate_Load(object sender, EventArgs e)
        {
            try
            {

                WaitingManager.Show(this);
                this.BranchID = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch;
                InitControlState();
                InitUcNational();
                FillDataToControlsForm();
                ValidataForm();
                InitCheck(cboBlockDepartment, SelectionGrid__BlockDepartment);
                InitCombo(cboBlockDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>(), "DEPARTMENT_NAME", "ID");
                SetIcon();
                SetCaptionByLanguageKey();
                SetDataToControl();
                FillBlockDepartment();
                FillDataToGridConrolServicePaty();
                InitComboMaterialTypeMapId();
                InitComboMaterialTypeMapId_();


                if (chkVatTu.Checked == false)
                {
                    this.lciMaterialTypeMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.LayCboAnhXa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    FillMaterialTypeMap();

                }
                if (chkVatTu.Checked == true)
                {
                    this.LayCboAnhXa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciMaterialTypeMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    FillMaterialTypeMap_();
                }
                if (chkIsBusiness.Checked)
                {
                    chkIS_DRUG_STORE.Enabled = true;

                }
                else
                {
                    chkIS_DRUG_STORE.Enabled = false;
                    chkIS_DRUG_STORE.Checked = false;
                }

                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    var mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.module.RoomId);
                    chkIsBusiness.Checked = mediStock != null && mediStock.IS_BUSINESS == 1;
                }
                WaitingManager.Hide();
            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "chkVatTu")
                        {
                            chkVatTu.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitUcNational()
        {
            try
            {
                panelControlNational.Dock = DockStyle.Fill;
                nationalProcessor = new HIS.UC.National.NationalProcessor();
                HIS.UC.National.ADO.NationalInitADO ado = new HIS.UC.National.ADO.NationalInitADO();
                ado.DelegateNextFocus = NextFocusNational;
                ado.Width = 440;
                ado.Height = 24;
                //ado.IsColor = true;
                ado.DataNationals = BackendDataWorker.Get<SDA_NATIONAL>();
                //ado.AutoCheckNational = AutoCheckIcd == "1";
                ucNational = (UserControl)nationalProcessor.Run(ado);

                if (ucNational != null)
                {
                    this.panelControlNational.Controls.Add(ucNational);
                    ucNational.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextFocusNational()
        {
            try
            {
                txtManufactureCode.Focus();
                txtManufactureCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetNullToSpinControl()
        {
            try
            {
                spinImpPrice.EditValue = null;
                spinImpVatRatio.EditValue = null;
                spinLastExpPrice.EditValue = null;
                spinLastExpVatPrice.EditValue = null;
                spinInternalPrice.EditValue = null;
                spinAlertMinInStock.EditValue = null;
                spinAlertExpiredDate.EditValue = null;
                spinNumOrder.EditValue = null;
                spinAlertMaxInPrescription.EditValue = null;
                spinAlertMaxInDay.EditValue = null;
                chkMaxReuseCount.CheckState = CheckState.Unchecked;
                ChkIsFilm.CheckState = CheckState.Unchecked;
                ChkIsSizeRequired.CheckState = CheckState.Unchecked;
                spinMaxReuseCount.EditValue = null;
                spinMaxReuseCount.Enabled = false;
                cboFileSize.Enabled = false;
                ChkIsNoHeinLimitForSpaceil.CheckState = CheckState.Unchecked;
                txtHeinLimitPrice.EditValue = null;
                txtHienLimitPriceOld.EditValue = null;
                cboImpUnit.EditValue = null;
                spUnitConvertRatio.EditValue = null;
                spReuseFee.EditValue = null;
                spPricingMaxReuseCount.EditValue = null;
                lciPricingMaxReuseCount.Enabled = false;
                lciReuseFee.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        async Task SetDataToControl()
        {
            try
            {
                spUnitConvertRatio.Enabled = false;
                if (txtMaterialType.Enabled)
                {
                    txtMaterialType.Focus();
                    txtMaterialType.SelectAll();
                }
                else
                {
                    txtMedicineTypeCode.Focus();
                    txtMedicineTypeCode.SelectAll();
                }
                if (this.ActionType == GlobalVariables.ActionEdit)
                {
                    chkIsBusiness.Checked = true;
                    txtMaterialType.Enabled = true;
                    cboMaterialType.Enabled = true;
                }
                else
                {
                    chkIsBusiness.Checked = false;
                    txtMaterialType.Enabled = false;
                    cboMaterialType.Enabled = false;
                }
                if (this.materialTypeId > 0 && this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    btnRefresh.Enabled = false;
                    rdoUpdateNotFee.CheckState = CheckState.Checked;
                    CommonParam param = new CommonParam();
                    chkIsBusiness.Checked = false;
                    //Load Current materialType
                    MOS.Filter.HisMaterialTypeViewFilter hisMaterialTypeViewFilter = new MOS.Filter.HisMaterialTypeViewFilter();
                    hisMaterialTypeViewFilter.ID = this.materialTypeId;
                    currentVHisMaterialTypeDTODefault = new BackendAdapter(param).Get<List<V_HIS_MATERIAL_TYPE>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, hisMaterialTypeViewFilter, param).SingleOrDefault();

                    //CommonParam pramservice = new CommonParam();
                    //HisServiceViewFilter filter = new HisServiceViewFilter();
                    //filter.ID = currentVHisMaterialTypeDTODefault.SERVICE_ID;
                    //currentVHisServiceDTODefault = new BackendAdapter(pramservice).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumers.MosConsumer, filter, pramservice).SingleOrDefault();
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("+++++++" + Inventec.Common.Logging.LogUtil.GetMemberName(() => currentVHisServiceDTODefault), currentVHisServiceDTODefault));
                    FillDataMedicineTypeToControl(currentVHisMaterialTypeDTODefault);

                    btnSave.Enabled = (currentVHisMaterialTypeDTODefault.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);


                }
                else
                {
                    SetNullToSpinControl();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillBlockDepartment()
        {
            try
            {
                if (this.materialTypeId > 0 && this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    HisMestMatyDepaFilter filter = new HisMestMatyDepaFilter();
                    filter.MATERIAL_TYPE_ID = this.materialTypeId;
                    filter.HAS_MEDI_STOCK_ID = false;

                    List<HIS_MEST_MATY_DEPA> matyDepas = new BackendAdapter(new CommonParam()).Get<List<HIS_MEST_MATY_DEPA>>("api/HisMestMatyDepa/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    this.oldBlockDepartmentIds = matyDepas != null ? matyDepas.Where(o => o.IS_JUST_PRESCRIPTION != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(s => s.DEPARTMENT_ID).Distinct().ToList() : null;
                }
                GridCheckMarksSelection gridCheckMark = cboBlockDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBlockDepartment.Properties.View);
                    if (this.oldBlockDepartmentIds != null && this.oldBlockDepartmentIds.Count > 0)
                    {
                        List<HIS_DEPARTMENT> seleceds = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => this.oldBlockDepartmentIds.Contains(o.ID)).ToList();
                        gridCheckMark.SelectAll(seleceds);

                        string displayText = String.Join(", ", seleceds.Select(s => s.DEPARTMENT_NAME).ToList());
                        cboBlockDepartment.Text = displayText;
                        cboBlockDepartment.ToolTip = displayText;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillMaterialTypeMap_()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("__________________1");
                if (currentVHisMaterialTypeDTODefault != null && currentVHisMaterialTypeDTODefault.MATERIAL_TYPE_MAP_ID.HasValue && this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    Inventec.Common.Logging.LogSystem.Error("__________________" + currentVHisMaterialTypeDTODefault.MATERIAL_TYPE_MAP_ID.Value);
                    Inventec.Common.Logging.LogSystem.Error("__________________1.1");
                    HisMaterialTypeMapFilter filter_ = new HisMaterialTypeMapFilter();
                    filter_.ID = currentVHisMaterialTypeDTODefault.MATERIAL_TYPE_MAP_ID.Value;
                    Inventec.Common.Logging.LogSystem.Error("__________________1.2");

                    var materials_ = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL_TYPE_MAP>>("api/HisMaterialTypeMap/Get", ApiConsumer.ApiConsumers.MosConsumer, filter_, null);
                    Inventec.Common.Logging.LogSystem.Error("__________________materials_          :   " + materials_);

                    cboAnhXa.EditValue = currentVHisMaterialTypeDTODefault.MATERIAL_TYPE_MAP_ID;
                    //cboAnhXa.EditValue = materials_.FirstOrDefault().ID;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillMaterialTypeMap()
        {
            try
            {

                Inventec.Common.Logging.LogSystem.Error("__________________1");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("____________________________________________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => currentVHisMaterialTypeDTODefault), currentVHisMaterialTypeDTODefault));
                if (currentVHisMaterialTypeDTODefault != null && currentVHisMaterialTypeDTODefault.MATERIAL_TYPE_MAP_ID.HasValue && this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {

                    HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                    Inventec.Common.Logging.LogSystem.Error("__________________1");
                    filter.MATERIAL_TYPE_MAP_ID = currentVHisMaterialTypeDTODefault.MATERIAL_TYPE_MAP_ID.Value;
                    Inventec.Common.Logging.LogSystem.Error("__________________2");
                    var materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL_TYPE>>("api/HisMaterialType/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    Inventec.Common.Logging.LogSystem.Error("__________________3");





                    if (materials != null && materials.Count > 0)
                    {
                        oldMaterialTypeMapIds = materials.Select(s => s.ID).ToList();
                        Inventec.Common.Logging.LogSystem.Error("__________________4");
                    }

                }
                if (oldMaterialTypeMapIds == null) return;
                var mtSelected = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(o => oldMaterialTypeMapIds.Contains(o.ID)).ToList();

                //cboAnhXa.EditValue = mtSelected_.Select(s => s.MATERIAL_TYPE_MAP_NAME);
                GridCheckMarksSelection gridCheckMark = cboMaterialTypeMapId.Properties.Tag as GridCheckMarksSelection;
                Inventec.Common.Logging.LogSystem.Error("__________________5");
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboMaterialTypeMapId.Properties.View);
                    if (oldMaterialTypeMapIds != null && oldMaterialTypeMapIds.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Error("__________________6");
                        //cboMaterialTypeMapId.Properties.DataSource = null;
                        //List<HIS_MATERIAL_TYPE> datas = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                        //datas = datas.OrderBy(o => seleceds.Contains(o)).ToList();
                        //cboMaterialTypeMapId.Properties.DataSource = datas;
                        gridCheckMark.SelectAll(mtSelected);
                        string displayText = String.Join(", ", mtSelected.Select(s => s.MATERIAL_TYPE_NAME).ToList());
                        cboMaterialTypeMapId.Text = displayText;
                        cboMaterialTypeMapId.ToolTip = displayText;
                        Inventec.Common.Logging.LogSystem.Error("__________________7");
                    }

                }


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

        private void FillDataMedicineTypeToControl(V_HIS_MATERIAL_TYPE hIS_MEDICINE_TYPE)
        {
            try
            {
                txtMaterialType.Text = hIS_MEDICINE_TYPE.MATERIAL_TYPE_CODE;
                cboMaterialType.EditValue = hIS_MEDICINE_TYPE.ID;
                txtMedicineTypeCode.Text = hIS_MEDICINE_TYPE.MATERIAL_TYPE_CODE;
                txtMedicineTypeName.Text = hIS_MEDICINE_TYPE.MATERIAL_TYPE_NAME;
                txtServiceUnitCode.Text = hIS_MEDICINE_TYPE.SERVICE_UNIT_CODE;
                cboServiceUnit.EditValue = hIS_MEDICINE_TYPE.SERVICE_UNIT_ID;
                cboMaterialTypeParent.EditValue = hIS_MEDICINE_TYPE.PARENT_ID;
                //cboMaterialTypeMapId.EditValue = hIS_MEDICINE_TYPE.MATERIAL_TYPE_MAP_ID;
                //txtMaterialTypeMapId.Text = hIS_MEDICINE_TYPE.MATERIAL_TYPE_MAP_CODE;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("///////////////" + Inventec.Common.Logging.LogUtil.GetMemberName(() => hIS_MEDICINE_TYPE.MATERIAL_TYPE_MAP_NAME), hIS_MEDICINE_TYPE.MATERIAL_TYPE_MAP_NAME));
                txtManufactureCode.Text = hIS_MEDICINE_TYPE.MANUFACTURER_CODE;
                cboManufacture.EditValue = hIS_MEDICINE_TYPE.MANUFACTURER_ID;
                txtRegisterNumber.Text = hIS_MEDICINE_TYPE.REGISTER_NUMBER;
                spinImpPrice.EditValue = hIS_MEDICINE_TYPE.IMP_PRICE;
                spinImpVatRatio.EditValue = hIS_MEDICINE_TYPE.IMP_VAT_RATIO != null ? hIS_MEDICINE_TYPE.IMP_VAT_RATIO * 100 : null;
                spinLastExpPrice.EditValue = hIS_MEDICINE_TYPE.LAST_EXP_PRICE;
                spinLastExpVatPrice.EditValue = hIS_MEDICINE_TYPE.LAST_EXP_VAT_RATIO != null ? hIS_MEDICINE_TYPE.LAST_EXP_VAT_RATIO * 100 : null;
                spinInternalPrice.EditValue = hIS_MEDICINE_TYPE.INTERNAL_PRICE;
                txtConcentra.Text = hIS_MEDICINE_TYPE.CONCENTRA;
                txtRecordTransation.Text = hIS_MEDICINE_TYPE.RECORDING_TRANSACTION;
                txtPackingTypeCode.Text = hIS_MEDICINE_TYPE.PACKING_TYPE_NAME;
                txtMaterialGroupBHYT.Text = hIS_MEDICINE_TYPE.MATERIAL_GROUP_BHYT;
                txtModel.Text = hIS_MEDICINE_TYPE.MODEL_CODE;

                if (hIS_MEDICINE_TYPE.IS_STOP_IMP == 1)
                {
                    chkIsStopImp.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsStopImp.CheckState = CheckState.Unchecked;
                }
                if (hIS_MEDICINE_TYPE.IS_SIZE_REQUIRED == 1)
                {
                    ChkIsSizeRequired.CheckState = CheckState.Checked;
                }
                else
                {
                    ChkIsSizeRequired.CheckState = CheckState.Unchecked;
                }
                if (hIS_MEDICINE_TYPE.IS_CHEMICAL_SUBSTANCE == 1)
                {
                    chkIsChemiscalSubstance.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsChemiscalSubstance.CheckState = CheckState.Unchecked;
                }

                if (hIS_MEDICINE_TYPE.IS_OUT_PARENT_FEE == 1)
                {
                    chkCPNG.CheckState = CheckState.Checked;
                }
                else
                {
                    chkCPNG.CheckState = CheckState.Unchecked;
                }

                if (hIS_MEDICINE_TYPE.IS_BUSINESS == 1)
                {
                    chkIsBusiness.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsBusiness.CheckState = CheckState.Unchecked;
                }

                if (hIS_MEDICINE_TYPE.IS_AUTO_EXPEND == 1)
                {
                    chkAutoExpend.CheckState = CheckState.Checked;
                }
                else
                {
                    chkAutoExpend.CheckState = CheckState.Unchecked;
                }

                if (hIS_MEDICINE_TYPE.IS_DRUG_STORE == 1)
                {
                    chkIS_DRUG_STORE.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIS_DRUG_STORE.CheckState = CheckState.Unchecked;
                }
                if (hIS_MEDICINE_TYPE.IS_REQUIRE_HSD == 1)
                {
                    chkIsExprireDate.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsExprireDate.CheckState = CheckState.Unchecked;
                }
                if (hIS_MEDICINE_TYPE.IS_ALLOW_EXPORT_ODD == 1)
                {
                    chkIsAllowExportOdd.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsAllowExportOdd.CheckState = CheckState.Unchecked;
                }

                if (hIS_MEDICINE_TYPE.IS_MUST_PREPARE == 1)
                {
                    chkIsMustPrepare.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsMustPrepare.CheckState = CheckState.Unchecked;
                }

                if (hIS_MEDICINE_TYPE.IS_RAW_MATERIAL == 1)
                {
                    chkIsRawMaterial.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsRawMaterial.CheckState = CheckState.Unchecked;
                }

                if (hIS_MEDICINE_TYPE.IS_ALLOW_ODD == 1)
                {
                    chkIsAllowOdd.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsAllowOdd.CheckState = CheckState.Unchecked;
                }
                chkDisplay.CheckState = hIS_MEDICINE_TYPE.IS_NOT_SHOW_TRACKING == 1 ? CheckState.Checked : CheckState.Unchecked;

                // get service
                MOS.Filter.HisServiceFilter serviceFilter = new HisServiceFilter();
                serviceFilter.ID = hIS_MEDICINE_TYPE.SERVICE_ID;
                var service = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE>>("api/HisService/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceFilter, new
                 CommonParam());
                if (service != null && service.Count > 0)
                {

                    if (service.FirstOrDefault().IS_NO_HEIN_LIMIT_FOR_SPECIAL == 1)
                    {
                        ChkIsNoHeinLimitForSpaceil.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        ChkIsNoHeinLimitForSpaceil.CheckState = CheckState.Unchecked;
                    }
                    if (service.FirstOrDefault().IS_OUT_OF_DRG == 1)
                    {
                        chkNgoaiDRG.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        chkNgoaiDRG.CheckState = CheckState.Unchecked;
                    }
                    if (service.FirstOrDefault().OTHER_PAY_SOURCE_ID != null)
                    {
                        txtIsSupported.Enabled = true;
                        txtBhytWhiteListCodes.Enabled = true;
                        cboCTK.EditValue = service.FirstOrDefault().OTHER_PAY_SOURCE_ID.Value;
                    }
                    if (service.FirstOrDefault().OTHER_PAY_SOURCE_ICDS != null)
                    {
                        txtIsSupported.Text = service.FirstOrDefault().OTHER_PAY_SOURCE_ICDS;
                    }
                    if (service.FirstOrDefault().BHYT_WHITELIST_CODES != null)
                    {
                        txtBhytWhiteListCodes.Text = service.FirstOrDefault().BHYT_WHITELIST_CODES;
                    }
                }

                dtHeinLimitPriceInTime.EditValue = hIS_MEDICINE_TYPE.HEIN_LIMIT_PRICE_IN_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hIS_MEDICINE_TYPE.HEIN_LIMIT_PRICE_IN_TIME ?? 0) : null;
                dtHSD.EditValue = hIS_MEDICINE_TYPE.LAST_EXPIRED_DATE != null ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hIS_MEDICINE_TYPE.LAST_EXPIRED_DATE ?? 0) : null;
                cboImpUnit.EditValue = hIS_MEDICINE_TYPE.IMP_UNIT_ID ?? null;
                if (cboImpUnit.EditValue != null)
                    spUnitConvertRatio.Value = hIS_MEDICINE_TYPE.IMP_UNIT_CONVERT_RATIO ?? 1;
                else
                    spUnitConvertRatio.EditValue = null;
                txtHeinOrder.Text = hIS_MEDICINE_TYPE.HEIN_ORDER;
                txtHeinServiceBhytCode.Text = hIS_MEDICINE_TYPE.HEIN_SERVICE_BHYT_CODE;
                txtHeinServiceBhytName.Text = hIS_MEDICINE_TYPE.HEIN_SERVICE_BHYT_NAME;
                if (hIS_MEDICINE_TYPE.HEIN_LIMIT_PRICE != null)
                    txtHeinLimitPrice.EditValue = hIS_MEDICINE_TYPE.HEIN_LIMIT_PRICE;
                else
                    txtHeinLimitPrice.EditValue = null;
                if (hIS_MEDICINE_TYPE.HEIN_LIMIT_PRICE_OLD != null)
                    txtHienLimitPriceOld.EditValue = hIS_MEDICINE_TYPE.HEIN_LIMIT_PRICE_OLD;
                else
                    txtHienLimitPriceOld.EditValue = null;

                txtHeinLimitRatioOld.Text = hIS_MEDICINE_TYPE.HEIN_LIMIT_RATIO_OLD != null ? string.Format("{0:0.####}", (hIS_MEDICINE_TYPE.HEIN_LIMIT_RATIO_OLD ?? 0) * 100) : "";
                txtHeinLimitRatio.Text = hIS_MEDICINE_TYPE.HEIN_LIMIT_RATIO != null ? string.Format("{0:0.####}", (hIS_MEDICINE_TYPE.HEIN_LIMIT_RATIO ?? 0) * 100) : "";
                spinAlertMinInStock.EditValue = hIS_MEDICINE_TYPE.ALERT_MIN_IN_STOCK;
                spinAlertExpiredDate.EditValue = hIS_MEDICINE_TYPE.ALERT_EXPIRED_DATE;
                spinAlertMaxInPrescription.EditValue = hIS_MEDICINE_TYPE.ALERT_MAX_IN_PRESCRIPTION;
                spinAlertMaxInDay.EditValue = hIS_MEDICINE_TYPE.ALERT_MAX_IN_DAY;
                spinNumOrder.EditValue = hIS_MEDICINE_TYPE.NUM_ORDER;

                var medicineTypeParent = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>().SingleOrDefault(o => o.ID == hIS_MEDICINE_TYPE.PARENT_ID);


                HIS.UC.National.ADO.NationalInputADO ado = new UC.National.ADO.NationalInputADO();

                var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME.ToUpper().Trim() == (hIS_MEDICINE_TYPE.NATIONAL_NAME ?? "").ToUpper().Trim());
                if (national != null)
                {
                    if (nationalProcessor != null && ucNational != null)
                    {
                        ado.NATIONAL_CODE = national.NATIONAL_CODE;
                        ado.NATIONAL_NAME = national.NATIONAL_NAME;
                        nationalProcessor.SetValue(ucNational, ado);
                    }
                }
                else
                {
                    ado.NATIONAL_NAME = hIS_MEDICINE_TYPE.NATIONAL_NAME;
                    nationalProcessor.SetValue(ucNational, ado);
                }

                if (hIS_MEDICINE_TYPE.IS_STENT == 1)
                {
                    chkIsStent.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsStent.CheckState = CheckState.Unchecked;
                }
                if (hIS_MEDICINE_TYPE.IS_OUT_HOSPITAL == 1)
                {
                    chkIsOutHospital.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsOutHospital.CheckState = CheckState.Unchecked;
                }
                if (hIS_MEDICINE_TYPE.IS_SALE_EQUAL_IMP_PRICE == 1)
                {
                    chkIsSaleEqualImpPrice.CheckState = CheckState.Checked;
                }
                else
                {
                    chkIsSaleEqualImpPrice.CheckState = CheckState.Unchecked;
                }


                cboHeinServiceType.EditValue = hIS_MEDICINE_TYPE.HEIN_SERVICE_TYPE_ID;

                if (hIS_MEDICINE_TYPE.TDL_GENDER_ID != null)
                {
                    var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == hIS_MEDICINE_TYPE.TDL_GENDER_ID);
                    if (gender != null)
                    {
                        cboGender.EditValue = gender.ID;

                    }
                }
                else
                    cboGender.EditValue = null;


                if (hIS_MEDICINE_TYPE.IS_FILM.HasValue && hIS_MEDICINE_TYPE.IS_FILM.Value == 1)
                {
                    cboFileSize.Enabled = true;
                    cboFileSize.EditValue = hIS_MEDICINE_TYPE.FILM_SIZE_ID;
                    ChkIsFilm.CheckState = CheckState.Checked;
                }
                else
                {
                    cboFileSize.Enabled = false;
                    cboFileSize.EditValue = null;
                    ChkIsFilm.CheckState = CheckState.Unchecked;
                }

                if (hIS_MEDICINE_TYPE.IS_CONSUMABLE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    chkIsExpendCTTM.Checked = true;
                }
                else
                {
                    chkIsExpendCTTM.Checked = false;
                }
                if (hIS_MEDICINE_TYPE.IS_REUSABLE == 1)
                {
                    spinMaxReuseCount.Enabled = true;
                    lciReuseFee.Enabled = true;
                    lciPricingMaxReuseCount.Enabled = true;
                    spinMaxReuseCount.EditValue = hIS_MEDICINE_TYPE.MAX_REUSE_COUNT;
                    spReuseFee.EditValue = hIS_MEDICINE_TYPE.REUSE_FEE;
                    spPricingMaxReuseCount.EditValue = hIS_MEDICINE_TYPE.PRICING_MAX_REUSE_COUNT;
                    chkMaxReuseCount.Checked = true;
                }
                else
                {
                    spinMaxReuseCount.Enabled = false;
                    spinMaxReuseCount.EditValue = null;
                    lciReuseFee.Enabled = false;
                    lciPricingMaxReuseCount.Enabled = false;
                    spPricingMaxReuseCount.EditValue = null;
                    spReuseFee.EditValue = null;
                    chkMaxReuseCount.Checked = false;
                }
                if (hIS_MEDICINE_TYPE.IS_IDENTITY_MANAGEMENT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    chkIsIdentityManagement.Checked = true;
                }
                else
                {
                    chkIsIdentityManagement.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load giữ liệu lên các combobox
        /// </summary>
        public void FillDataToControlsForm()
        {
            try
            {
                InitServiceUnit();
                InitMedicineTypeParent();
                InitManufacture();
                InitHeinServiceType();
                InitComboGender();
                //InitComboMaterialTypeMapId();
                LoadDatatoComboPatientType();
                InitHisFilmSeze();
                InitMaterialType();
                InitNguonCTK();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SuccessLog(HIS_MATERIAL_TYPE result)
        {
            try
            {
                if (result != null)
                {
                    //string message = String.Format(EXE.LOGIC.Base.EventLogUtil.SetLog(His.EventLog.Message.Enum.SuaThongTinBenhNhan), result.PATIENT_CODE, result.VIR_PATIENT_NAME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(result.DOB), cboGender.Text, currentHisPatientTypeAlter.TREATMENT_ID, currentHisPatientTypeAlter.TREATMENT_TYPE_NAME);
                    //His.EventLog.Logger.Log(LOGIC.LocalStore.GlobalStore.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event
        #region ---Click
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceUnit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceUnit.Properties.Buttons[1].Visible = false;
                    cboServiceUnit.EditValue = null;
                    txtServiceUnitCode.Text = "";
                    txtServiceUnitCode.Focus();
                    txtServiceUnitCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataMaterial()
        {
            try
            {
                if (cboServiceUnit.EditValue != null)
                {
                    HisMaterial = new V_HIS_MATERIAL_TYPE();
                    HisMaterial.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceUnit.EditValue.ToString());
                    HisMaterial.SERVICE_UNIT_CODE = txtServiceUnitCode.Text.Trim();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboMedicineTypeParent_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                SetDataMaterial();
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMaterialTypeParent.Properties.Buttons[1].Visible = false;
                    cboMaterialTypeParent.EditValue = null;
                    txtServiceUnitCode.Focus();
                    txtServiceUnitCode.SelectAll();
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateSelectData)refreshDataToMaterial);
                    listArgs.Add((V_HIS_MATERIAL_TYPE)HisMaterial);
                    if (this.module == null)
                    {
                        CallModule callModule = new CallModule(CallModule.MaterialTypeCreateParent, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.MaterialTypeCreateParent, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboManufacture_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboManufacture.Properties.Buttons[1].Visible = false;
                    cboManufacture.EditValue = null;
                    txtManufactureCode.Text = "";
                    txtManufactureCode.Focus();
                    txtManufactureCode.SelectAll();
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateSelectData)refreshDataToManufacturerCombo);
                    if (this.module == null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisManufacturer, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisManufacturer, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void refreshDataToMaterial(object MaterialType)
        {
            try
            {
                if (MaterialType != null && MaterialType is MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 100, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 150);
                    MOS.Filter.HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IS_LEAF = null;
                    CommonParam param = new CommonParam();
                    var data = new BackendAdapter(param).Get<List<HIS_MATERIAL_TYPE>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET, ApiConsumers.MosConsumer, filter, param);
                    ControlEditorLoader.Load(cboMaterialTypeParent, data, controlEditorADO);
                    cboMaterialTypeParent.EditValue = ((MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE)MaterialType).ID;

                }
            }
            catch (Exception ex)
            {

                LogSession.Warn(ex);
            }
        }

        private void refreshDataToManufacturerCombo(object manufacturer)
        {
            try
            {
                if (manufacturer != null && manufacturer is MOS.EFMODEL.DataModels.HIS_MANUFACTURER)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MANUFACTURER_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("MANUFACTURER_NAME", "", 100, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MANUFACTURER_NAME", "ID", columnInfos, false, 150);
                    MOS.Filter.HisManufacturerFilter filter = new HisManufacturerFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    CommonParam param = new CommonParam();
                    var hisManufacturers = new BackendAdapter(param).Get<List<HIS_MANUFACTURER>>(HisRequestUriStore.HIS_MANUFACTURER_GET, ApiConsumers.MosConsumer, filter, param);
                    ControlEditorLoader.Load(cboManufacture, hisManufacturers, controlEditorADO);
                    cboManufacture.EditValue = ((MOS.EFMODEL.DataModels.HIS_MANUFACTURER)manufacturer).ID;
                    txtManufactureCode.Text = ((MOS.EFMODEL.DataModels.HIS_MANUFACTURER)manufacturer).MANUFACTURER_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void UpdateHeinServiceType(MOS.EFMODEL.DataModels.HIS_SERVICE service)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtHeinServiceBhytCode.Text))
                {
                    service.HEIN_SERVICE_BHYT_CODE = txtHeinServiceBhytCode.Text.Trim();
                    service.HEIN_SERVICE_BHYT_NAME = txtHeinServiceBhytName.Text.Trim();
                    service.HEIN_ORDER = txtHeinOrder.Text;
                }
                if (cboCTK.EditValue != null)
                {
                    service.OTHER_PAY_SOURCE_ID = long.Parse(cboCTK.EditValue.ToString());
                }
                else
                {
                    service.OTHER_PAY_SOURCE_ID = null;
                }
                if (!String.IsNullOrEmpty(txtIsSupported.Text))
                {
                    service.OTHER_PAY_SOURCE_ICDS = txtIsSupported.Text;
                }
                if (!String.IsNullOrEmpty(txtBhytWhiteListCodes.Text))
                {
                    service.BHYT_WHITELIST_CODES = txtBhytWhiteListCodes.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdatePatientDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE materialType)
        {
            try
            {
                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE, MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>();
                    materialType = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>(currentVHisMaterialTypeDTODefault);
                    materialType.ID = currentVHisMaterialTypeDTODefault.ID;
                }

                materialType.HIS_SERVICE = new HIS_SERVICE();
                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit && currentVHisMaterialTypeDTODefault != null)
                {
                    materialType.HIS_SERVICE.ID = currentVHisMaterialTypeDTODefault.SERVICE_ID;
                    materialType.HIS_SERVICE.SERVICE_UNIT_ID = currentVHisMaterialTypeDTODefault.SERVICE_UNIT_ID;
                    materialType.HIS_SERVICE.SERVICE_TYPE_ID = currentVHisMaterialTypeDTODefault.SERVICE_TYPE_ID;

                }
                materialType.MATERIAL_TYPE_CODE = txtMedicineTypeCode.Text.Trim();
                materialType.MATERIAL_TYPE_NAME = txtMedicineTypeName.Text;

                if (spinAlertExpiredDate.EditValue != null)
                {
                    materialType.ALERT_EXPIRED_DATE = (long)spinAlertExpiredDate.Value;
                }
                else materialType.ALERT_EXPIRED_DATE = null;

                if (spinAlertMaxInPrescription.EditValue != null)
                {
                    materialType.ALERT_MAX_IN_PRESCRIPTION = (decimal)spinAlertMaxInPrescription.Value;
                }
                else materialType.ALERT_MAX_IN_PRESCRIPTION = null;
                if (spinAlertMaxInDay.EditValue != null)
                {
                    materialType.ALERT_MAX_IN_DAY = (decimal)spinAlertMaxInDay.Value;
                }
                else materialType.ALERT_MAX_IN_DAY = null;

                materialType.CONCENTRA = txtConcentra.Text.Trim();
                materialType.MODEL_CODE = txtModel.Text.Trim();
                materialType.RECORDING_TRANSACTION = txtRecordTransation.Text.Trim();
                materialType.REGISTER_NUMBER = txtRegisterNumber.Text.Trim();

                if (spinNumOrder.EditValue != null)
                {
                    materialType.NUM_ORDER = (long)spinNumOrder.Value;
                }
                else materialType.NUM_ORDER = null;

                if (chkIsChemiscalSubstance.CheckState == CheckState.Checked)
                {
                    materialType.IS_CHEMICAL_SUBSTANCE = 1;
                }
                else
                {
                    materialType.IS_CHEMICAL_SUBSTANCE = null;
                }
                if (ChkIsSizeRequired.CheckState == CheckState.Checked)
                {
                    materialType.IS_SIZE_REQUIRED = 1;
                }
                else
                {
                    materialType.IS_SIZE_REQUIRED = null;
                }

                if (chkIsRawMaterial.CheckState == CheckState.Checked)
                {
                    materialType.IS_RAW_MATERIAL = 1;
                }
                else
                {
                    materialType.IS_RAW_MATERIAL = null;
                }

                materialType.PACKING_TYPE_NAME = this.txtPackingTypeCode.Text.Trim();
                materialType.MATERIAL_GROUP_BHYT = this.txtMaterialGroupBHYT.Text.Trim();

                if (!string.IsNullOrEmpty(txtHeinLimitRatio.Text))
                {
                    materialType.HIS_SERVICE.HEIN_LIMIT_RATIO = CustomParse.ConvertCustom(txtHeinLimitRatio.Text) / 100;
                }
                else materialType.HIS_SERVICE.HEIN_LIMIT_RATIO = null;

                if (!string.IsNullOrEmpty(txtHeinLimitRatioOld.Text))
                {
                    materialType.HIS_SERVICE.HEIN_LIMIT_RATIO_OLD = CustomParse.ConvertCustom(txtHeinLimitRatioOld.Text) / 100;
                }
                else materialType.HIS_SERVICE.HEIN_LIMIT_RATIO_OLD = null;

                if (dtHeinLimitPriceInTime.EditValue != null && dtHeinLimitPriceInTime.DateTime != DateTime.MinValue)
                    materialType.HIS_SERVICE.HEIN_LIMIT_PRICE_IN_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtHeinLimitPriceInTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                else
                    materialType.HIS_SERVICE.HEIN_LIMIT_PRICE_IN_TIME = null;

                if (dtHSD.EditValue != null && dtHSD.DateTime != DateTime.MinValue)
                    materialType.LAST_EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtHSD.EditValue).ToString("yyyyMMddHHmm") + "00");
                else
                    materialType.LAST_EXPIRED_DATE = null;

                if (cboImpUnit.EditValue != null)
                {
                    materialType.IMP_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboImpUnit.EditValue.ToString());
                    materialType.IMP_UNIT_CONVERT_RATIO = spUnitConvertRatio.Value;
                }
                else
                {
                    materialType.IMP_UNIT_ID = null;
                    materialType.IMP_UNIT_CONVERT_RATIO = null;
                }

                materialType.HIS_SERVICE.HEIN_SERVICE_BHYT_CODE = string.IsNullOrEmpty(txtHeinServiceBhytCode.Text) ? "" : txtHeinServiceBhytCode.Text.Trim();

                materialType.HIS_SERVICE.IS_NO_HEIN_LIMIT_FOR_SPECIAL = (short)(ChkIsNoHeinLimitForSpaceil.Checked == true ? 1 : 0);
                materialType.HIS_SERVICE.IS_OUT_PARENT_FEE = (short)(chkCPNG.Checked == true ? 1 : 0);
                UpdateHeinServiceType(materialType.HIS_SERVICE);

                materialType.HIS_SERVICE.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceUnit.EditValue ?? "").ToString());

                if (txtHeinLimitPrice.EditValue != null)
                {
                    materialType.HIS_SERVICE.HEIN_LIMIT_PRICE = (decimal)txtHeinLimitPrice.Value;
                }
                else
                    materialType.HIS_SERVICE.HEIN_LIMIT_PRICE = null;

                if (txtHienLimitPriceOld.EditValue != null)
                {
                    materialType.HIS_SERVICE.HEIN_LIMIT_PRICE_OLD = (decimal)txtHienLimitPriceOld.Value;
                }
                else
                    materialType.HIS_SERVICE.HEIN_LIMIT_PRICE_OLD = null;

                if (spinImpPrice.EditValue != null)
                {
                    materialType.IMP_PRICE = (decimal)spinImpPrice.Value;
                }
                else materialType.IMP_PRICE = null;
                if (spinLastExpPrice.EditValue != null)
                {
                    materialType.LAST_EXP_PRICE = (decimal)spinLastExpPrice.Value;
                }
                else
                {
                    materialType.LAST_EXP_PRICE = null;
                }

                if (spinLastExpVatPrice.EditValue != null)
                {
                    materialType.LAST_EXP_VAT_RATIO = (decimal)spinLastExpVatPrice.Value / 100;
                }
                else
                {
                    materialType.LAST_EXP_VAT_RATIO = null;
                }
                if (spinImpVatRatio.EditValue != null)
                {
                    materialType.IMP_VAT_RATIO = (decimal)spinImpVatRatio.Value / 100;
                }
                else materialType.IMP_VAT_RATIO = null;
                if (spinInternalPrice.EditValue != null)
                {
                    materialType.INTERNAL_PRICE = (decimal)spinInternalPrice.Value;
                }
                else materialType.INTERNAL_PRICE = null;
                if (chkIsStopImp.CheckState == CheckState.Checked)
                    materialType.IS_STOP_IMP = 1;
                else
                    materialType.IS_STOP_IMP = null;
                if (chkIsExprireDate.CheckState == CheckState.Checked)
                    materialType.IS_REQUIRE_HSD = 1;
                else
                    materialType.IS_REQUIRE_HSD = null;

                if (chkAutoExpend.CheckState == CheckState.Checked)
                    materialType.IS_AUTO_EXPEND = 1;
                else
                    materialType.IS_AUTO_EXPEND = null;


                if (chkIS_DRUG_STORE.CheckState == CheckState.Checked)
                    materialType.IS_DRUG_STORE = 1;
                else
                    materialType.IS_DRUG_STORE = null;

                if (chkIsBusiness.CheckState == CheckState.Checked)
                    materialType.IS_BUSINESS = 1;
                else
                    materialType.IS_BUSINESS = null;
                if (chkNgoaiDRG.CheckState == CheckState.Checked)
                {
                    materialType.HIS_SERVICE.IS_OUT_OF_DRG = 1;
                }
                else
                {
                    materialType.HIS_SERVICE.IS_OUT_OF_DRG = null;
                }
                if (chkIsAllowExportOdd.CheckState == CheckState.Checked)
                    materialType.IS_ALLOW_EXPORT_ODD = 1;
                else
                    materialType.IS_ALLOW_EXPORT_ODD = null;

                if (chkIsMustPrepare.CheckState == CheckState.Checked)
                    materialType.IS_MUST_PREPARE = 1;
                else
                    materialType.IS_MUST_PREPARE = null;

                if (chkIsAllowOdd.CheckState == CheckState.Checked)
                    materialType.IS_ALLOW_ODD = 1;
                else
                    materialType.IS_ALLOW_ODD = null;

                if (chkIsSaleEqualImpPrice.CheckState == CheckState.Checked)
                    materialType.IS_SALE_EQUAL_IMP_PRICE = 1;
                else
                    materialType.IS_SALE_EQUAL_IMP_PRICE = null;

                if (cboManufacture.EditValue != null)
                {
                    materialType.MANUFACTURER_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboManufacture.EditValue ?? "").ToString());
                }
                else
                {
                    materialType.MANUFACTURER_ID = null;
                }

                if (cboMaterialTypeParent.EditValue != null)
                {
                    materialType.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMaterialTypeParent.EditValue ?? "").ToString());
                }
                else
                {
                    materialType.PARENT_ID = null;
                }

                if (currentVHisMaterialTypeDTODefault != null && currentVHisMaterialTypeDTODefault.MATERIAL_TYPE_MAP_ID.HasValue)
                {
                    materialType.MATERIAL_TYPE_MAP_ID = currentVHisMaterialTypeDTODefault.MATERIAL_TYPE_MAP_ID.Value;
                }
                else if (cboAnhXa.EditValue != null)
                {
                    materialType.MATERIAL_TYPE_MAP_ID = long.Parse(cboAnhXa.EditValue.ToString());
                }
                else
                {
                    materialType.MATERIAL_TYPE_MAP_ID = null;
                }

                if (ucNational != null && nationalProcessor != null)
                {
                    var nati = (HIS.UC.National.ADO.NationalInputADO)nationalProcessor.GetValue(ucNational);
                    materialType.NATIONAL_NAME = nati.NATIONAL_NAME;
                }
                else materialType.NATIONAL_NAME = null;

                if (chkIsStopImp.CheckState == CheckState.Checked)
                {
                    materialType.IS_STOP_IMP = 1;
                }
                else
                {
                    materialType.IS_STOP_IMP = null;
                }

                if (chkIsStent.CheckState == CheckState.Checked)
                {
                    materialType.IS_STENT = 1;
                }
                else
                {
                    materialType.IS_STENT = null;
                }

                if (chkIsOutHospital.CheckState == CheckState.Checked)
                {
                    materialType.IS_OUT_HOSPITAL = 1;
                }
                else
                {
                    materialType.IS_OUT_HOSPITAL = null;
                }
                if (chkDisplay.CheckState == CheckState.Checked)
                    materialType.IS_NOT_SHOW_TRACKING = 1;
                else
                    materialType.IS_NOT_SHOW_TRACKING = null;

                if (spinAlertMinInStock.EditValue != null)
                {
                    materialType.ALERT_MIN_IN_STOCK = spinAlertMinInStock.Value;
                }
                else
                    materialType.ALERT_MIN_IN_STOCK = null;

                if (cboHeinServiceType.EditValue != null)
                {
                    materialType.HIS_SERVICE.HEIN_SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboHeinServiceType.EditValue.ToString());
                }
                else materialType.HIS_SERVICE.HEIN_SERVICE_TYPE_ID = null;

                if (cboGender.EditValue != null)
                {
                    materialType.TDL_GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString());
                    materialType.HIS_SERVICE.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString());
                }
                else
                {
                    materialType.TDL_GENDER_ID = null;
                    materialType.HIS_SERVICE.GENDER_ID = null;
                }

                if (chkMaxReuseCount.CheckState == CheckState.Checked)
                {
                    materialType.IS_REUSABLE = 1;
                }
                else
                {
                    materialType.IS_REUSABLE = null;
                }

                if (spinMaxReuseCount.EditValue != null)
                {
                    materialType.MAX_REUSE_COUNT = (long)spinMaxReuseCount.Value;
                }
                else materialType.MAX_REUSE_COUNT = null;
                if (spReuseFee.EditValue != null)
                {
                    materialType.REUSE_FEE = spReuseFee.Value;
                }
                else materialType.REUSE_FEE = null;
                if (spPricingMaxReuseCount.EditValue != null)
                {
                    materialType.PRICING_MAX_REUSE_COUNT = spPricingMaxReuseCount.Value;
                }
                else materialType.PRICING_MAX_REUSE_COUNT = null;

                if (ChkIsFilm.CheckState == CheckState.Checked)
                {
                    materialType.IS_FILM = 1;
                }
                else
                {
                    materialType.IS_FILM = null;
                }

                if (chkIsExpendCTTM.CheckState == CheckState.Checked)
                {
                    materialType.IS_CONSUMABLE = 1;
                }
                else
                {
                    materialType.IS_CONSUMABLE = null;
                }
                if (chkIsIdentityManagement.CheckState == CheckState.Checked)
                {
                    materialType.IS_IDENTITY_MANAGEMENT = 1;
                }
                else
                {
                    materialType.IS_IDENTITY_MANAGEMENT = null;
                }
                if (cboFileSize.EditValue != null)
                {
                    materialType.FILM_SIZE_ID = (long)cboFileSize.EditValue;
                }
                else materialType.FILM_SIZE_ID = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// validate dữ liệu trước khi lưu
        /// </summary>
        /// <returns></returns>
        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandleControlMedicineTypeInfo = -1;

                if (!dxValidationProvider1.Validate())
                    return;
                if (!(bool)nationalProcessor.ValidationNational(ucNational))
                    return;
                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionView)
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE currentMedicineTypeDTO = new MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE();

                UpdatePatientDTOFromDataForm(ref currentMedicineTypeDTO);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMedicineTypeDTO), currentMedicineTypeDTO));

                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    resultData = new BackendAdapter(param).Post<HIS_MATERIAL_TYPE>(HisRequestUriStore.HIS_MATERIAL_TYPE_CREATE, ApiConsumers.MosConsumer, currentMedicineTypeDTO, param);
                }
                else if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    MOS.SDO.HisMaterialTypeSDO materialTypeSdo = new MOS.SDO.HisMaterialTypeSDO();
                    materialTypeSdo.HisMaterialType = currentMedicineTypeDTO;
                    UpdateData(materialTypeSdo);
                    resultData = new BackendAdapter(param).Post<HIS_MATERIAL_TYPE>("api/HisMaterialType/UpdateSdo", ApiConsumers.MosConsumer, materialTypeSdo, param);
                }

                if (resultData != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                    btnRefresh.Enabled = true;
                    txtMedicineTypeCode.Text = resultData.MATERIAL_TYPE_CODE;
                    WaitingManager.Hide();
                    SuccessLog(resultData);
                    BackendDataWorker.Reset<HIS_MATERIAL_TYPE>();
                    SendDataAfterSave();
                    InitMedicineTypeParent();
                    MOS.Filter.HisMaterialTypeViewFilter materialTypeViewFilter = new HisMaterialTypeViewFilter();
                    materialTypeViewFilter.ID = resultData.ID;
                    this.currentVHisMaterialTypeDTODefault = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>>("api/HisMaterialType/GetView", ApiConsumer.ApiConsumers.MosConsumer, materialTypeViewFilter, param).FirstOrDefault();


                    BackendDataWorker.Reset<V_HIS_SERVICE>();
                }

                if (Check())
                {
                    SaveProcessorsHisServicePaty(ref param);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.Luuchinhsachgiathatbai, Resources.ResourceMessage.Thongbao, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (success)
                {
                    this.SaveBlockDepartment(ref param);
                    this.SaveMaterialTypeMap(ref param);
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void UpdateData(MOS.SDO.HisMaterialTypeSDO materilaTypeSdo)
        {
            try
            {
                if (materilaTypeSdo != null)
                {
                    if (!rdoUpdateAll.Checked && !rdoUpdateNotFee.Checked)
                    {
                        materilaTypeSdo.UpdateSereServ = null;
                    }
                    else if (rdoUpdateNotFee.Checked)
                    {
                        materilaTypeSdo.UpdateSereServ = false;
                    }
                    else if (rdoUpdateAll.Checked)
                    {
                        materilaTypeSdo.UpdateSereServ = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---PreviewKeyDown
        private void txtMedicineTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineTypeName.Focus();
                    txtMedicineTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMaterialTypeParent.Focus();
                    cboMaterialTypeParent.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceUnitCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadServiceUnit(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinLastExpVatPrice_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnSave.Enabled)
                        btnSave.Focus();
                    else
                        btnRefresh.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            e.Handled = true;
        }

        private void txtAtc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinNumOrder.Focus();
                    spinNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtHSD.Focus();
                    dtHSD.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineTypeProprietaryName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtManufactureCode.Focus();
                    txtManufactureCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtManufactureCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadManufacturer(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpVatRatio.Focus();
                    spinImpVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpVatRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinInternalPrice.Focus();
                    spinInternalPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinInternalPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSaleEqualImpPrice.Focus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConcentra_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinNumOrder.Focus();
                    spinNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPackingTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRegisterNumber.Focus();
                    txtRegisterNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAlertMinInStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBlockDepartment.Focus();
                    cboBlockDepartment.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAntibiotic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsStopImp.Focus();
                    chkIsStopImp.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsStopImp_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCPNG.Focus();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAlertExpiredDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinAlertMaxInDay.Focus();
                    spinAlertMaxInDay.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceUnit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboServiceUnit.Text))
                    {
                        string key = cboServiceUnit.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().Where(o => o.SERVICE_UNIT_CODE.ToLower().Contains(key) || o.SERVICE_UNIT_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboServiceUnit.EditValue = listData.First().ID;
                            txtServiceUnitCode.Text = listData.First().SERVICE_UNIT_CODE;
                            cboHeinServiceType.Focus();
                            cboHeinServiceType.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboServiceUnit.Focus();
                        cboServiceUnit.ShowPopup();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cboMaterialTypeParent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bool valid = false;
                if (!String.IsNullOrEmpty(cboServiceUnit.Text))
                {
                    string key = cboMaterialTypeParent.Text.ToLower();
                    var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>().Where(o => o.MATERIAL_TYPE_CODE.ToLower().Contains(key) || o.MATERIAL_TYPE_NAME.ToLower().Contains(key)).ToList();
                    if (listData != null && listData.Count == 1)
                    {
                        valid = true;
                        cboMaterialTypeParent.EditValue = listData.First().ID;
                        txtServiceUnitCode.Focus();
                        txtServiceUnitCode.SelectAll();
                    }
                }
                if (!valid)
                {
                    cboMaterialTypeParent.Focus();
                    cboMaterialTypeParent.ShowPopup();
                }
            }
        }

        private void chkMaxReuseCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lciReuseFee.Enabled)
                        spReuseFee.Focus();
                    else if (lciSizeRequired.Enabled)
                        ChkIsSizeRequired.Focus();
                    else if (spinMaxReuseCount.Enabled)
                        spinMaxReuseCount.Focus();
                    else if (lciPricingMaxReuseCount.Enabled)
                        spPricingMaxReuseCount.Focus();
                    else
                        ChkIsFilm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinMaxReuseCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lciPricingMaxReuseCount.Enabled)
                        spPricingMaxReuseCount.Focus();
                    else
                        ChkIsFilm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAllowOdd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsAllowExportOdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaterialGroupBHYT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMaterialTypeMapId.Focus();
                    //cboMaterialTypeMapId.SelectAll();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboManufacture_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bool valid = false;
                if (!String.IsNullOrEmpty(cboManufacture.Text))
                {
                    string key = cboManufacture.Text.ToLower();
                    var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>().Where(o => o.MANUFACTURER_CODE.ToLower().Contains(key) || o.MANUFACTURER_NAME.ToLower().Contains(key)).ToList();
                    if (listData != null && listData.Count == 1)
                    {
                        valid = true;
                        cboManufacture.EditValue = listData.First().ID;
                        txtManufactureCode.Text = listData.First().MANUFACTURER_CODE;
                    }
                }
                if (!valid)
                {
                    cboManufacture.Focus();
                    cboManufacture.ShowPopup();
                }
            }
        }

        private void chkIsStent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsOutHospital.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHeinServiceType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboHeinServiceType.Text))
                    {
                        string key = cboHeinServiceType.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>().Where(o => o.HEIN_SERVICE_TYPE_CODE.ToLower().Contains(key) || o.HEIN_SERVICE_TYPE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboHeinServiceType.EditValue = listData.First().ID;
                            txtHeinOrder.Focus();
                            txtHeinOrder.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboHeinServiceType.Focus();
                        cboHeinServiceType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsChemiscalSubstance_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsExpendCTTM.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCPNG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsExprireDate.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsExprireDate.Focus();
                    //e.Handled = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsExprireDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsAllowOdd.Focus();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAutoExpend_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpPrice.Focus();
                    spinImpPrice.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitPriceOld_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLimitRatioOld.Focus();
                    txtHeinLimitRatioOld.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitRatioOld_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLimitPrice.Focus();
                    txtHeinLimitPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLimitRatioOld.Focus();
                    txtHeinLimitRatioOld.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsBusiness_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsMustPrepare.Focus();

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsSaleEqualImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinLastExpPrice.Focus();
                    spinLastExpPrice.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAllowExportOdd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsBusiness.Focus();

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsRawMaterial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNgoaiDRG.Focus();
                    chkNgoaiDRG.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinServiceBhytCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinServiceBhytName.Focus();
                    txtHeinServiceBhytName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinServiceBhytName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaterialGroupBHYT.Focus();
                    txtMaterialGroupBHYT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsMustPrepare_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAutoExpend.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---KeyUp

        #endregion

        #region ---Close
        private void cboServiceUnit_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboServiceUnit.EditValue != null)
                    {
                        var serviceUnit = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceUnit.EditValue ?? "").ToString()));
                        if (serviceUnit != null)
                        {
                            txtServiceUnitCode.Text = serviceUnit.SERVICE_UNIT_CODE;
                            cboServiceUnit.Properties.Buttons[1].Visible = true;
                            cboHeinServiceType.Focus();
                            cboHeinServiceType.SelectAll();
                        }
                        else
                        {
                            cboServiceUnit.Focus();
                            cboServiceUnit.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineTypeParent_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMaterialTypeParent.EditValue != null)
                    {
                        cboMaterialTypeParent.Properties.Buttons[1].Visible = true;
                        txtServiceUnitCode.Focus();
                        txtServiceUnitCode.SelectAll();
                    }
                    else
                    {
                        cboMaterialTypeParent.Focus();
                        cboMaterialTypeParent.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboManufacture_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboManufacture.EditValue != null)
                    {
                        var manufacturer = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboManufacture.EditValue ?? "").ToString()));
                        if (manufacturer != null)
                        {
                            txtManufactureCode.Text = manufacturer.MANUFACTURER_CODE;
                            cboManufacture.Properties.Buttons[1].Visible = true;
                            txtPackingTypeCode.Focus();
                            txtPackingTypeCode.SelectAll();
                        }
                        else
                        {
                            cboManufacture.Focus();
                            cboManufacture.ShowPopup();
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

        #region ---ValidationFailed
        private void dxValidationProviderPatientInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlMedicineTypeInfo == -1)
                {
                    positionHandleControlMedicineTypeInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlMedicineTypeInfo > edit.TabIndex)
                {
                    positionHandleControlMedicineTypeInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cboHeinServiceType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHeinServiceType.Properties.Buttons[1].Visible = true;
                    cboHeinServiceType.EditValue = null;
                    txtHeinOrder.Focus();
                    txtHeinOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHeinServiceType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHeinServiceType.EditValue != null)
                    {
                        cboHeinServiceType.Properties.Buttons[1].Visible = true;
                        txtHeinOrder.Focus();
                        txtHeinOrder.SelectAll();
                    }
                    else
                    {
                        cboHeinServiceType.Focus();
                        cboHeinServiceType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnMaterialPaty_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                MOS.EFMODEL.DataModels.V_HIS_SERVICE service = new V_HIS_SERVICE();
                if (this.currentVHisMaterialTypeDTODefault != null || this.resultData != null)
                {
                    // get service by service_id
                    MOS.Filter.HisServiceViewFilter serviceViewFilter = new HisServiceViewFilter();
                    if (this.resultData != null)
                    {
                        serviceViewFilter.ID = this.resultData.SERVICE_ID;
                    }
                    else if (this.currentVHisMaterialTypeDTODefault != null)
                    {
                        serviceViewFilter.ID = currentVHisMaterialTypeDTODefault.SERVICE_ID;
                    }

                    var services = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceViewFilter, new CommonParam());

                    if (services != null && services.Count > 0)
                    {
                        service = services.FirstOrDefault();
                    }
                }

                listArgs.Add(service);
                if (this.module == null)
                {
                    CallModule callModule = new CallModule(CallModule.HisServicePatyList, 0, 0, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisServicePatyList, this.module.RoomId, this.module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinServiceBhytCode.Focus();
                    txtHeinServiceBhytCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void dtHeinLimitPriceInTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    ChkIsNoHeinLimitForSpaceil.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnRefresh.Enabled)
                    btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                DefaultGroup();
                ResetFormData();
                currentVHisMaterialTypeDTODefault = null;
                resultData = null;
                //this.ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;// mac dinh la them

                materialTypeId = null;
                delegateSelect = null;
                HisMaterial = null;
                lsVHisServicePatyBegin = null;
                lsVHisServicePaty = null;
                btnSave.Enabled = true;
                ServicePatyCreate = null;
                this.ServicePatyUpdate = null;
                FillDataToGridConrolServicePaty();
                if (ucNational != null)
                {
                    nationalProcessor.Reload(ucNational, null);
                }
                txtMaterialType.Text = "";
                chkIsOutHospital.Checked = false;
                txtMaterialType.Enabled = false;
                cboMaterialType.EditValue = null;
                cboMaterialType.EditValue = false;
                cboCTK.EditValue = null;
                txtIsSupported.Text = "";
                dxValidationProvider1.RemoveControlError(txtIsSupported);
                txtBhytWhiteListCodes.Text = "";
                dxValidationProvider1.RemoveControlError(txtBhytWhiteListCodes);
                this.ActionType = GlobalVariables.ActionAdd;
                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    var mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.module.RoomId);
                    chkIsBusiness.Checked = mediStock != null && mediStock.IS_BUSINESS == 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #region ---Refresh---
        private void ResetFormDatagroup1()
        {
            try
            {
                if (!layoutControl4.IsInitialized) return;
                layoutControl4.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl4.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;

                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl4.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormDatagroup2()
        {
            try
            {
                if (!layoutControl5.IsInitialized) return;
                layoutControl5.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl5.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;

                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl5.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormDatagroup3()
        {
            try
            {
                if (!layoutControl6.IsInitialized) return;
                layoutControl6.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl6.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl6.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormDatagroup4()
        {
            try
            {
                if (!layoutControl8.IsInitialized) return;
                layoutControl8.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl8.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;

                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl5.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormDatagroup5()
        {
            try
            {
                if (!layoutControl7.IsInitialized) return;
                layoutControl7.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl7.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            this.nationalProcessor.SetValue(this.ucNational, "");
                            dxValidationProvider1.RemoveControlError(fomatFrm);
                            txtMedicineTypeCode.Focus();
                            txtMedicineTypeCode.SelectAll();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl7.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                ResetFormDatagroup1();
                ResetFormDatagroup2();
                ResetFormDatagroup3();
                ResetFormDatagroup4();
                ResetFormDatagroup5();
                this.ActionType = GlobalVariables.ActionAdd;

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                chkIsBusiness.Checked = false;
                chkNgoaiDRG.Checked = false;
                chkDisplay.Checked = false;
                chkIsMustPrepare.Checked = false;
                chkAutoExpend.Checked = false;
                chkIS_DRUG_STORE.Checked = false;
                chkIS_DRUG_STORE.Enabled = false;
                chkIsChemiscalSubstance.Checked = false;
                ChkIsSizeRequired.Checked = false;
                chkIsStent.Checked = false;
                chkIsRawMaterial.Checked = false;
                chkIsStopImp.Checked = false;
                chkCPNG.Checked = false;
                chkIsExprireDate.Checked = false;
                chkIsAllowOdd.Checked = false;
                chkIsAllowExportOdd.Checked = false;
                SetNullToSpinControl();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void DefaultGroup()
        {
            try
            {
                ResetControl(spinImpPrice);
                ResetControl(spinLastExpPrice);
                ResetControl(spinLastExpVatPrice);
                ResetControl(spinImpVatRatio);
                ResetControl(spinInternalPrice);
                ResetControl(txtHeinLimitRatioOld);
                ResetControl(txtHeinLimitRatio);
                ResetControl(dtHeinLimitPriceInTime);
                ResetControl(dtHSD);
                rdoUpdateAll.Checked = false;
                rdoUpdateNotFee.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ResetControl(BaseEdit control)
        {
            try
            {
                control.EditValue = null;
                control.ResetText();
                dxValidationProvider1.RemoveControlError(control);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoUpdateAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoUpdateAll.Checked)
                    rdoUpdateNotFee.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void rdoUpdateNotFee_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoUpdateNotFee.Checked)
                    rdoUpdateAll.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                BackendDataWorker.Reset<V_HIS_SERVICE>();
                BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
                BackendDataWorker.Reset<V_HIS_MATERIAL_TYPE>();
                BackendDataWorker.Reset<V_HIS_SERVICE_ROOM>();
                BackendDataWorker.Reset<V_HIS_MATERIAL_PATY>();
                BackendDataWorker.Reset<MedicineMaterialTypeComboADO>();
                MessageManager.Show("Xử lý thành công");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_PATY)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_MATERIAL_TYPE)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_ROOM)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_MATERIAL_PATY)).ToString(), false);
                MessageManager.Show("Xử lý thành công");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboGender.EditValue != null)
                    {
                        spinAlertMaxInPrescription.Focus();
                        spinAlertMaxInPrescription.SelectAll();

                    }
                    else
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGender.EditValue = null;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboGender.EditValue != null)
                {
                    cboGender.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboGender.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAlertMaxInPrescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinAlertExpiredDate.Focus();
                    spinAlertExpiredDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMaxReuseCount_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkMaxReuseCount.Checked == true)
                {
                    lciSizeRequired.Enabled = true;
                    ChkIsSizeRequired.Enabled = true;
                    spinMaxReuseCount.Enabled = true;
                    lciSpinMaxReuseCount.Enabled = true;
                    lciReuseFee.Enabled = true;
                    spReuseFee.Enabled = true;
                    lciPricingMaxReuseCount.Enabled = true;
                    spPricingMaxReuseCount.Enabled = true;
                }
                else
                {
                    if (chkIsIdentityManagement.Checked)
                    {
                        lciSizeRequired.Enabled = true;
                        ChkIsSizeRequired.Enabled = true;
                    }
                    else
                    {
                        lciSizeRequired.Enabled = false;
                        ChkIsSizeRequired.Enabled = false;
                        ChkIsSizeRequired.Checked = false;
                    }
                    lciSpinMaxReuseCount.Enabled = false;
                    spinMaxReuseCount.Enabled = false;
                    spinMaxReuseCount.EditValue = null;
                    lciReuseFee.Enabled = false;
                    spReuseFee.Enabled = false;
                    lciPricingMaxReuseCount.Enabled = false;
                    spPricingMaxReuseCount.Enabled = false;
                    spReuseFee.EditValue = null;
                    spPricingMaxReuseCount.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChkIsNoHeinLimitForSpaceil_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGender.Focus();
                    cboGender.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void txtMaterialTypeMapId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
        //            LoadCboMaterialTypeMapId(strValue);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void LoadCboMaterialTypeMapId(string text)
        //{
        //    try
        //    {
        //        List<HIS_MATERIAL_TYPE_MAP> listResult = new List<HIS_MATERIAL_TYPE_MAP>();
        //        listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE_MAP>().Where(o => (o.MATERIAL_TYPE_MAP_CODE != null && o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.MATERIAL_TYPE_MAP_CODE.StartsWith(text))).ToList();

        //        if (listResult.Count == 1)
        //        {
        //            cboMaterialTypeMapId.EditValue = listResult[0].ID;
        //            cboMaterialTypeParent.Properties.Buttons[1].Visible = true;
        //            txtMaterialTypeMapId.Text = listResult[0].MATERIAL_TYPE_MAP_CODE;
        //            if (nationalProcessor != null && ucNational != null)
        //            {
        //                nationalProcessor.FocusControl(ucNational);
        //            }
        //        }
        //        else if (listResult.Count > 1)
        //        {
        //            cboMaterialTypeMapId.EditValue = null;
        //            cboMaterialTypeMapId.Focus();
        //            cboMaterialTypeMapId.ShowPopup();
        //        }
        //        else
        //        {
        //            cboMaterialTypeMapId.EditValue = null;
        //            cboMaterialTypeMapId.Focus();
        //            cboMaterialTypeMapId.ShowPopup();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void cboMaterialTypeMapId_ButtonClick(object sender, ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Button.Kind == ButtonPredefines.Delete)
        //        {
        //            cboMaterialTypeMapId.Properties.Buttons[1].Visible = false;
        //            cboMaterialTypeMapId.EditValue = null;
        //            txtMaterialTypeMapId.Text = "";
        //            txtMaterialTypeMapId.Focus();
        //            txtMaterialTypeMapId.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void cboMaterialTypeMapId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            bool valid = false;
        //            if (!String.IsNullOrEmpty(cboMaterialTypeMapId.Text))
        //            {
        //                string key = cboMaterialTypeMapId.Text.ToLower();
        //                var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE_MAP>().Where(o => o.MATERIAL_TYPE_MAP_CODE.ToLower().Contains(key) || o.MATERIAL_TYPE_MAP_NAME.ToLower().Contains(key)).ToList();
        //                if (listData != null && listData.Count == 1)
        //                {
        //                    valid = true;
        //                    cboMaterialTypeMapId.EditValue = listData.First().ID;
        //                    txtMaterialTypeMapId.Text = listData.First().MATERIAL_TYPE_MAP_CODE;
        //                    this.nationalProcessor.FocusControl(ucNational);
        //                }
        //            }
        //            if (!valid)
        //            {
        //                cboMaterialTypeMapId.Focus();
        //                cboMaterialTypeMapId.ShowPopup();
        //            }
        //        }
        //        else
        //        {
        //            cboMaterialTypeMapId.Focus();
        //            cboMaterialTypeMapId.ShowPopup();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void cboMaterialTypeMapId_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    this.nationalProcessor.FocusControl(ucNational);
                    //if (cboMaterialTypeMapId.EditValue != null)
                    //{
                    //    var MaterialTypeMapID = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE_MAP>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMaterialTypeMapId.EditValue ?? "").ToString()));
                    //    if (MaterialTypeMapID != null)
                    //    {
                    //        txtMaterialTypeMapId.Text = MaterialTypeMapID.MATERIAL_TYPE_MAP_CODE;
                    //        cboMaterialTypeMapId.Properties.Buttons[1].Visible = true;
                    //        this.nationalProcessor.FocusControl(ucNational);

                    //    }
                    //    else
                    //    {
                    //        cboMaterialTypeMapId.Focus();
                    //        cboMaterialTypeMapId.ShowPopup();
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinLastExpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinLastExpVatPrice.Focus();
                    spinLastExpVatPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void textEdit14_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboFileSize.Text))
                    {
                        string key = cboFileSize.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_FILM_SIZE>().Where(o => o.FILM_SIZE_CODE.ToLower().Contains(key) || o.FILM_SIZE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboFileSize.EditValue = listData.First().ID;
                            txtHeinLimitRatio.Focus();
                            txtHeinLimitRatio.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboFileSize.Focus();
                        cboFileSize.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinLimitPriceInTime_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtHeinLimitPriceInTime.EditValue != null)
                        ChkIsNoHeinLimitForSpaceil.Focus();
                    else
                    {
                        dtHeinLimitPriceInTime.Focus();
                        dtHeinLimitPriceInTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboGender.EditValue != null)
                    {
                        spinAlertMaxInPrescription.Focus();
                        spinAlertMaxInPrescription.SelectAll();
                    }
                    else
                    {
                        cboGender.Focus();
                        cboGender.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private async Task FillDataToGridConrolServicePaty()
        {
            try
            {
                List<ADO.VHisServicePatyADO> lstHisPaty = new List<ADO.VHisServicePatyADO>();
                grdViewHisServicePaty.BeginUpdate();
                grdControlHisServicePaty.DataSource = null;
                CommonParam param = new CommonParam();
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>> data = null;
                HisServicePatyViewFilter filter = new HisServicePatyViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (this.resultData != null)
                {
                    filter.SERVICE_ID = resultData.SERVICE_ID;
                    data = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>>(
                    "api/HisServicePaty/GetView", ApiConsumers.MosConsumer, filter, param);
                }
                else if (this.currentVHisMaterialTypeDTODefault != null)
                {
                    filter.SERVICE_ID = currentVHisMaterialTypeDTODefault.SERVICE_ID;
                    data = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>>(
                   "api/HisServicePaty/GetView", ApiConsumers.MosConsumer, filter, param);
                }
                if (data != null)
                {
                    var Result = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>)data.Data;
                    if (Result != null && Result.Count > 0)
                    {
                        int i = 0;
                        foreach (var item in Result)
                        {
                            ADO.VHisServicePatyADO HisServicePaty = new ADO.VHisServicePatyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.VHisServicePatyADO>(HisServicePaty, item);
                            if (i == 0)

                                HisServicePaty.Action = GlobalVariables.ActionAdd;

                            else
                                HisServicePaty.Action = GlobalVariables.ActionEdit;
                            HisServicePaty.VAT_RATIO = HisServicePaty.VAT_RATIO != null ? HisServicePaty.VAT_RATIO * 100 : 0;
                            lstHisPaty.Add(HisServicePaty);

                            i++;
                        }
                        if (lstHisPaty != null && lstHisPaty.Count > 0)
                        {

                            this.lsVHisServicePaty = lstHisPaty;
                            grdControlHisServicePaty.DataSource = lstHisPaty;
                        }
                    }
                    else
                    {
                        ADO.VHisServicePatyADO HisServicePaty = new ADO.VHisServicePatyADO();
                        HisServicePaty.Action = GlobalVariables.ActionAdd;
                        HisServicePaty.BRANCH_ID = this.BranchID.ID;
                        HisServicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                        lstHisPaty.Add(HisServicePaty);
                        grdControlHisServicePaty.DataSource = null;
                        grdControlHisServicePaty.DataSource = lstHisPaty;
                        this.lsVHisServicePaty = lstHisPaty;
                    }
                }
                else
                {
                    if (chkIsBusiness.CheckState == CheckState.Checked)
                    {
                        List<HIS_PATIENT_TYPE> listPatientTypeForSale = new List<HIS_PATIENT_TYPE>();
                        var queryData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_FOR_SALE_EXP == 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                        if (queryData != null && queryData.Count() > 0)
                        {
                            listPatientTypeForSale = queryData.ToList();
                        }
                        foreach (var item in listPatientTypeForSale)
                        {
                            ADO.VHisServicePatyADO HisServicePaty = new ADO.VHisServicePatyADO();
                            HisServicePaty.Action = GlobalVariables.ActionAdd;
                            HisServicePaty.PATIENT_TYPE_ID = item.ID;
                            HisServicePaty.BRANCH_ID = this.BranchID.ID;
                            HisServicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                            lstHisPaty.Add(HisServicePaty);
                        }
                        if (lstHisPaty.Count == 0)
                        {
                            ADO.VHisServicePatyADO HisServicePaty = new ADO.VHisServicePatyADO();
                            HisServicePaty.Action = GlobalVariables.ActionAdd;
                            HisServicePaty.BRANCH_ID = this.BranchID.ID;
                            HisServicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                            lstHisPaty.Add(HisServicePaty);
                        }
                        grdControlHisServicePaty.DataSource = null;
                        grdControlHisServicePaty.DataSource = lstHisPaty;
                        this.lsVHisServicePaty = lstHisPaty;
                    }
                    else
                    {


                        ADO.VHisServicePatyADO HisServicePaty = new ADO.VHisServicePatyADO();
                        HisServicePaty.Action = GlobalVariables.ActionAdd;
                        HisServicePaty.BRANCH_ID = this.BranchID.ID;
                        HisServicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                        lstHisPaty.Add(HisServicePaty);
                        grdControlHisServicePaty.DataSource = null;
                        grdControlHisServicePaty.DataSource = lstHisPaty;
                        this.lsVHisServicePaty = lstHisPaty;
                    }
                }
                grdViewHisServicePaty.EndUpdate();

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private bool Check()
        {
            bool result = true;
            try
            {

                if (this.lsVHisServicePaty == null || this.lsVHisServicePaty.Count == 0)
                    throw new ArgumentNullException("Du lieu dau vao khong hop le. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lsVHisServicePaty), lsVHisServicePaty));

                var groupPatientType = from p in this.lsVHisServicePaty
                                       group p by new
                                       {
                                           p.PATIENT_TYPE_ID,
                                           p.BRANCH_ID,
                                       } into g
                                       select new { Key = g.Key, CareDetail = g.ToList() };
                if (groupPatientType != null && groupPatientType.Count() > 0)
                {
                    foreach (var item in groupPatientType)
                    {
                        if (item.CareDetail.Count > 1)
                        {
                            result = false;
                            //  MessageBox.Show("Lưu chính sách giá thất bại, không thể lưu cùng loại đối tượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //param.Messages.Add("Dữ liệu chi tiết không thể cùng loại đối tượng");
                            break;
                        }
                    }
                }

                //#region Show message
                //MessageManager.Show(param, null);
                //#endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SaveProcessorsHisServicePaty(ref CommonParam param)
        {
            try
            {

                DataUpdateAndInsert();
                WaitingManager.Show();
                if (this.ServicePatyCreate != null && this.ServicePatyCreate.Count > 0)
                {
                    CommonParam commonpara = new CommonParam();
                    var resultData1 = new BackendAdapter(commonpara).Post<List<HIS_SERVICE_PATY>>(
                   "api/HisServicePaty/CreateList", ApiConsumers.MosConsumer, ServicePatyCreate, commonpara);
                    if (resultData1 != null)
                    {
                        ServicePatyCreate = null;
                        BackendDataWorker.Reset<HIS_SERVICE_PATY>();
                    }
                    else if ((commonpara.Messages != null && commonpara.Messages.Count > 0)
                        || (commonpara.BugCodes != null && commonpara.BugCodes.Count > 0))
                    {
                        param.Messages.AddRange(commonpara.Messages);
                        param.BugCodes.AddRange(commonpara.BugCodes);
                    }
                }
                if (this.ServicePatyUpdate != null && this.ServicePatyUpdate.Count > 0)
                {
                    CommonParam commonparam = new CommonParam();
                    var resultData = new BackendAdapter(commonparam).Post<List<HIS_SERVICE_PATY>>(
                   "api/HisServicePaty/UpdateList", ApiConsumers.MosConsumer, ServicePatyUpdate, commonparam);
                    if (resultData != null)
                    {
                        ServicePatyUpdate = null;
                        BackendDataWorker.Reset<HIS_SERVICE_PATY>();
                    }
                    else if ((commonparam.Messages != null && commonparam.Messages.Count > 0)
                        || (commonparam.BugCodes != null && commonparam.BugCodes.Count > 0))
                    {
                        param.Messages.AddRange(commonparam.Messages);
                        param.BugCodes.AddRange(commonparam.BugCodes);
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SaveBlockDepartment(ref CommonParam param)
        {
            try
            {
                WaitingManager.Show();
                List<long> selectedDepartmentIds = new List<long>();
                List<long> oldDepartmentIds = new List<long>();

                if (this.oldBlockDepartmentIds != null)
                {
                    oldDepartmentIds = this.oldBlockDepartmentIds;
                }

                if (this.BlockDepartment__Seleced != null)
                {
                    selectedDepartmentIds = this.BlockDepartment__Seleced.Select(s => s.ID).Distinct().ToList();
                }

                if (selectedDepartmentIds.Count == 0 && oldDepartmentIds.Count == 0)
                {
                    return;
                }

                if (selectedDepartmentIds.Exists(e => !oldDepartmentIds.Contains(e))
                    || oldDepartmentIds.Exists(e => !selectedDepartmentIds.Contains(e)))
                {
                    CommonParam commonpara = new CommonParam();
                    HisMestMatyDepaByMaterialSDO sdo = new HisMestMatyDepaByMaterialSDO();
                    sdo.MaterialTypeId = resultData.ID;
                    sdo.DepartmentIds = selectedDepartmentIds;
                    var rs = new BackendAdapter(commonpara).Post<bool>("api/HisMestMatyDepa/CreateByMaterial", ApiConsumers.MosConsumer, sdo, commonpara);
                    if (!rs && (commonpara.Messages != null && commonpara.Messages.Count > 0)
                        || (commonpara.BugCodes != null && commonpara.BugCodes.Count > 0))
                    {
                        param.Messages.AddRange(commonpara.Messages);
                        param.BugCodes.AddRange(commonpara.BugCodes);
                    }

                    this.oldBlockDepartmentIds = new List<long>();
                    if (selectedDepartmentIds.Count > 0)
                    {
                        this.oldBlockDepartmentIds.AddRange(selectedDepartmentIds);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SaveMaterialTypeMap(ref CommonParam param)
        {
            try
            {
                WaitingManager.Show();
                List<long> selectedMaterialIds = new List<long>();
                List<long> oldMaterialIds = new List<long>();

                if (this.oldMaterialTypeMapIds != null)
                {
                    oldMaterialIds = this.oldMaterialTypeMapIds;
                }

                if (this.MaterialTypeMap__Seleced != null)
                {
                    selectedMaterialIds = this.MaterialTypeMap__Seleced.Select(s => s.ID).Distinct().ToList();
                }

                if (selectedMaterialIds.Count == 0 && oldMaterialIds.Count == 0)
                {
                    return;
                }

                if (selectedMaterialIds.Exists(e => !oldMaterialIds.Contains(e))
                    || oldMaterialIds.Exists(e => !selectedMaterialIds.Contains(e)))
                {
                    HisMaterialTypeUpdateMapSDO sdo = new HisMaterialTypeUpdateMapSDO();
                    sdo.MaterialTypeId = resultData.ID;
                    sdo.MapMaterialTypeIds = selectedMaterialIds;
                    var rs = new BackendAdapter(param).Post<HIS_MATERIAL_TYPE>("api/HisMaterialType/UpdateMap", ApiConsumers.MosConsumer, sdo, param);
                    if (rs != null)
                    {
                        currentVHisMaterialTypeDTODefault.MATERIAL_TYPE_MAP_ID = rs.MATERIAL_TYPE_MAP_ID;
                        this.oldMaterialTypeMapIds = new List<long>();
                        if (selectedMaterialIds.Count > 0)
                        {
                            this.oldMaterialTypeMapIds.AddRange(selectedMaterialIds);
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void DataUpdateAndInsert()
        {
            try
            {
                if (this.resultData != null && this.lsVHisServicePaty != null && this.lsVHisServicePaty.Count > 0)
                {
                    this.ServicePatyCreate = new List<HIS_SERVICE_PATY>();
                    this.ServicePatyUpdate = new List<HIS_SERVICE_PATY>();
                    foreach (var item in this.lsVHisServicePaty)
                    {
                        if (item.ID > 0 && item.PATIENT_TYPE_ID > 0 && resultData.SERVICE_ID > 0 && item.PRICE != null && item.VAT_RATIO >= 0 && item.VAT_RATIO <= 100)
                        {
                            HIS_SERVICE_PATY data = new HIS_SERVICE_PATY();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_PATY>(data, item);
                            data.VAT_RATIO = item.VAT_RATIO != null ? item.VAT_RATIO / 100 : 0;
                            this.ServicePatyUpdate.Add(data);
                        }

                        else if (resultData.SERVICE_ID > 0 && item.PATIENT_TYPE_ID > 0 && item.PRICE != null && item.VAT_RATIO >= 0 && item.VAT_RATIO <= 100)
                        {
                            HIS_SERVICE_PATY data = new HIS_SERVICE_PATY();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_PATY>(data, item);
                            data.VAT_RATIO = item.VAT_RATIO != null ? item.VAT_RATIO / 100 : 0;
                            //data.BRANCH_ID = this.BranchID;
                            data.SERVICE_ID = resultData.SERVICE_ID;
                            this.ServicePatyCreate.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnItem_Add_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                ADO.VHisServicePatyADO VHisSerVicePaty = new ADO.VHisServicePatyADO();
                VHisSerVicePaty.Action = GlobalVariables.ActionEdit;
                VHisSerVicePaty.BRANCH_ID = this.BranchID.ID;
                VHisSerVicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                this.lsVHisServicePaty.Add(VHisSerVicePaty);
                grdControlHisServicePaty.DataSource = null;
                grdControlHisServicePaty.DataSource = this.lsVHisServicePaty;

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewHisServicePaty_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ADO.VHisServicePatyADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (ADO.VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "BtnAddDelete")
                    {
                        if (data.Action == GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = btnItem_Add;
                        }
                        else
                        {
                            e.RepositoryItem = btnItem_Delete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnItem_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    var HisServicePaty = (ADO.VHisServicePatyADO)grdViewHisServicePaty.GetFocusedRow();
                    if (HisServicePaty != null)
                    {
                        success = true;
                        this.lsVHisServicePaty.Remove(HisServicePaty);
                        if (HisServicePaty.ID > 0)
                        {
                            HIS_SERVICE_PATY data = new HIS_SERVICE_PATY();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_PATY>(data, HisServicePaty);
                            success = new BackendAdapter(param).Post<bool>("api/HisServicePaty/Delete", ApiConsumers.MosConsumer, data, param);
                            if (success)
                            {
                                BackendDataWorker.Reset<HIS_SERVICE_PATY>();
                            }
                        }
                        grdControlHisServicePaty.DataSource = null;
                        grdControlHisServicePaty.DataSource = this.lsVHisServicePaty;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private async Task LoadDatatoComboPatientType()
        {
            try
            {
                this.lsVHisServicePaty = new List<ADO.VHisServicePatyADO>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 150);
                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                if (chkIsBusiness.Checked)
                {
                    if (data != null && data.Count > 0)
                    {
                        var data_ = data.Where(o => o.IS_FOR_SALE_EXP == 1 && o.IS_ACTIVE == 1).ToList();
                        if (data_ != null && data_.Count > 0)
                        {
                            this.lsVHisServicePaty.Clear();
                            foreach (var item in data_)
                            {
                                ADO.VHisServicePatyADO VHisSerVicePaty = new ADO.VHisServicePatyADO();
                                VHisSerVicePaty.Action = GlobalVariables.ActionEdit;
                                VHisSerVicePaty.BRANCH_ID = this.BranchID.ID;
                                VHisSerVicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                                VHisSerVicePaty.PATIENT_TYPE_ID = item.ID;
                                this.lsVHisServicePaty.Add(VHisSerVicePaty);
                                grdControlHisServicePaty.BeginUpdate();
                                grdControlHisServicePaty.DataSource = null;
                                grdControlHisServicePaty.DataSource = this.lsVHisServicePaty;
                                grdControlHisServicePaty.EndUpdate();
                            }
                            ControlEditorLoader.Load(repositoryItemGridLookUpPatientType, data_, controlEditorADO);
                        }
                        else
                        {
                            this.lsVHisServicePaty.Clear();

                            ADO.VHisServicePatyADO VHisSerVicePaty = new ADO.VHisServicePatyADO();
                            VHisSerVicePaty.Action = GlobalVariables.ActionEdit;
                            VHisSerVicePaty.BRANCH_ID = this.BranchID.ID;
                            VHisSerVicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                            this.lsVHisServicePaty.Add(VHisSerVicePaty);
                            grdControlHisServicePaty.BeginUpdate();
                            grdControlHisServicePaty.DataSource = null;
                            grdControlHisServicePaty.DataSource = this.lsVHisServicePaty;
                            grdControlHisServicePaty.EndUpdate();
                            ControlEditorLoader.Load(repositoryItemGridLookUpPatientType, data.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                        }
                    }
                }
                else
                {
                    this.lsVHisServicePaty.Clear();

                    ADO.VHisServicePatyADO VHisSerVicePaty = new ADO.VHisServicePatyADO();
                    VHisSerVicePaty.Action = GlobalVariables.ActionEdit;
                    VHisSerVicePaty.BRANCH_ID = this.BranchID.ID;
                    VHisSerVicePaty.BRANCH_NAME = this.BranchID.BRANCH_NAME;
                    this.lsVHisServicePaty.Add(VHisSerVicePaty);
                    grdControlHisServicePaty.BeginUpdate();
                    grdControlHisServicePaty.DataSource = null;
                    grdControlHisServicePaty.DataSource = this.lsVHisServicePaty;
                    grdControlHisServicePaty.EndUpdate();
                    ControlEditorLoader.Load(repositoryItemGridLookUpPatientType, data.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ChkIsFilm_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChkIsFilm.CheckState == CheckState.Checked)
                {
                    cboFileSize.Enabled = true;
                }
                else
                {
                    cboFileSize.Enabled = false;
                    cboFileSize.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewHisServicePaty_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {

                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkIsFilm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ChkIsFilm.CheckState == CheckState.Checked)
                    {
                        cboFileSize.Focus();
                        cboFileSize.SelectAll();
                    }
                    else
                    {
                        chkIsStent.Focus();
                        // chkIsExpendCTTM.Select();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboFileSize_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboFileSize.Properties.Buttons[1].Visible = true;
                    cboFileSize.EditValue = null;
                    txtHeinLimitRatio.Focus();
                    txtHeinLimitRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFileSize_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboFileSize.EditValue != null)
                    {
                        txtHeinLimitRatio.Focus();
                        txtHeinLimitRatio.SelectAll();
                    }
                    else
                    {
                        cboFileSize.Focus();
                        cboFileSize.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewHisServicePaty_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {

                    GridView view = sender as GridView;
                    GridColumn onOrderCol = view.Columns["PATIENT_TYPE_ID"];
                    GridColumn onOrderColPrice = view.Columns["PRICE"];
                    GridColumn onOrderColVatRatio = view.Columns["VAT_RATIO"];
                    var data = (ADO.VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data.PATIENT_TYPE_ID < 0)
                    {

                        view.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc");
                    }

                    if (data.PRICE < 0)
                    {

                        view.SetColumnError(onOrderColPrice, "Trường dữ liệu bắt buộc");
                    }
                    if (data.VAT_RATIO < 0)
                    {

                        view.SetColumnError(onOrderColVatRatio, "Trường dữ liệu bắt buộc");
                    }
                    else
                    {
                        if (data.VAT_RATIO > 100)
                        {
                            view.SetColumnError(onOrderColVatRatio, "Giá trị trong khoảng 0-100");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdViewHisServicePaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

        }

        private void txtHienLimitPriceOld_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtHeinLimitPriceInTime.Focus();
                    dtHeinLimitPriceInTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void spinAlertMaxInDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinAlertMinInStock.Focus();
                    spinAlertMinInStock.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtHeinLimitPrice_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHienLimitPriceOld.Focus();
                    txtHienLimitPriceOld.SelectAll();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHSD_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (dtHSD.EditValue != null)
                    {
                        txtRecordTransation.Focus();
                        txtRecordTransation.SelectAll();
                    }
                    else
                    {
                        dtHSD.Focus();
                        dtHSD.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtHSD_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtHSD.EditValue != null)
                    {
                        txtRecordTransation.Focus();
                        txtRecordTransation.SelectAll();
                    }
                    else
                    {
                        dtHSD.Focus();
                        dtHSD.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void textEdit2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var strEdit = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    var data = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(o => o.MATERIAL_TYPE_CODE.ToUpper().Trim() == strEdit.ToUpper().Trim()).ToList();
                    if (data != null && data.Count == 1)
                    {
                        txtMaterialType.Text = data[0].MATERIAL_TYPE_CODE;
                        cboMaterialType.EditValue = data[0].ID;
                        txtMedicineTypeCode.Focus();
                        txtMedicineTypeCode.SelectAll();
                    }
                    else
                    {
                        cboMaterialType.Focus();
                        cboMaterialType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboMaterialType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMaterialType.EditValue != null)
                    {
                        var materialtype = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMaterialType.EditValue.ToString() ?? ""));
                        if (materialtype != null)
                        {
                            txtMaterialType.Text = materialtype.MATERIAL_TYPE_CODE;
                            cboMaterialType.EditValue = materialtype.ID;
                            txtMedicineTypeCode.Focus();
                            txtMedicineTypeCode.SelectAll();
                        }
                        else
                        {
                            cboMaterialType.Focus();
                            cboMaterialType.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboMaterialType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(cboMaterialType.Text))
                    {
                        var materialtype = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(o => o.MATERIAL_TYPE_NAME.ToUpper().Trim().Contains(cboMaterialType.Text.ToUpper().Trim())).ToList();
                        if (materialtype != null && materialtype.Count == 1)
                        {
                            txtMaterialType.Text = materialtype[0].MATERIAL_TYPE_CODE;
                            cboMaterialType.EditValue = materialtype[0].ID;
                            txtMedicineTypeCode.Focus();
                            txtMedicineTypeCode.SelectAll();
                        }
                        else
                        {
                            cboMaterialType.Focus();
                            cboMaterialType.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboMaterialType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.materialTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMaterialType.EditValue.ToString() ?? "");
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);

                currentVHisMaterialTypeDTODefault = null;
                resultData = null;
                // ActionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;// mac dinh la them
                HisMaterial = null;
                lsVHisServicePatyBegin = null;
                lsVHisServicePaty = null;
                btnSave.Enabled = true;
                ServicePatyCreate = null;
                this.ServicePatyUpdate = null;
                if (ucNational != null)
                {
                    nationalProcessor.Reload(ucNational, null);
                }
                SetDataToControl();
                FillDataToGridConrolServicePaty();

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtRecordTransation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboImpUnit.Focus();
                    cboImpUnit.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboBlockDepartment_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string display = "";
                foreach (var item in this.BlockDepartment__Seleced)
                {
                    if (display.Trim().Length > 0)
                    {
                        display += ", " + item.DEPARTMENT_NAME;
                    }
                    else
                        display = item.DEPARTMENT_NAME;
                }
                e.DisplayText = display;
                cboBlockDepartment.ToolTip = display;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__BlockDepartment(object sender, EventArgs e)
        {
            try
            {
                BlockDepartment__Seleced = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        BlockDepartment__Seleced.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboBlockDepartment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboCTK.Focus();
                    cboCTK.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMaterialTypeMapId_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string display = "";
                if (MaterialTypeMap__Seleced.Count < 100)
                {
                    foreach (var item in this.MaterialTypeMap__Seleced)
                    {
                        if (display.Trim().Length > 0)
                        {
                            display += ", " + item.MATERIAL_TYPE_NAME;
                        }
                        else
                            display = item.MATERIAL_TYPE_NAME;
                    }
                }
                else
                {
                    var select = MaterialTypeMap__Seleced.Take(100).ToList();
                    display = string.Join(", ", select.Select(s => s.MATERIAL_TYPE_NAME).ToList()) + " ...";
                }

                e.DisplayText = display;
                cboMaterialTypeMapId.ToolTip = display;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsExpendCTTM_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsIdentityManagement.Focus();
                    // chkIsOutHospital.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsOutHospital_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLimitRatio.Focus();
                    txtHeinLimitRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsBusiness_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNgoaiDRG.Focus();
                    chkNgoaiDRG.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkVatTu.Checked == false)
                {
                    this.lciMaterialTypeMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.LayCboAnhXa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    FillMaterialTypeMap();

                }
                if (chkVatTu.Checked == true)
                {
                    this.LayCboAnhXa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciMaterialTypeMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    FillMaterialTypeMap_();
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "chkVatTu" && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkVatTu.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "chkVatTu";
                    csAddOrUpdate.VALUE = (chkVatTu.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCTK_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCTK.EditValue != null)
                    {
                        cboCTK.Properties.Buttons[1].Visible = true;
                        txtIsSupported.Focus();
                        txtIsSupported.SelectAll();
                    }
                    else
                    {
                        chkIsStopImp.Focus();
                        chkIsStopImp.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCTK_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCTK.Properties.Buttons[1].Visible = true;
                    cboCTK.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCTK_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (cboCTK.EditValue != null)
                    {
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OTHER_PAY_SOURCE>().Where(o => o.ID == long.Parse(cboCTK.EditValue.ToString())).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboCTK.EditValue = listData.First().ID;
                            txtIsSupported.Focus();
                            txtIsSupported.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboCTK.Focus();
                        cboCTK.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCTK_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCTK.EditValue != null)
                {
                    txtIsSupported.Enabled = true;
                    txtBhytWhiteListCodes.Enabled = true;
                    ValidMaxlengthTxtIsSupported();
                    ValidMaxlengthTxtBhytCodes();
                }
                else
                {
                    txtIsSupported.Text = null;
                    txtBhytWhiteListCodes.Text = null;
                    txtIsSupported.Enabled = false;
                    txtBhytWhiteListCodes.Enabled = false;
                    dxValidationProvider1.SetValidationRule(txtIsSupported, null);
                    dxValidationProvider1.SetValidationRule(txtBhytWhiteListCodes, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtIsSupported_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsStopImp.Focus();
                    chkIsStopImp.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex); ;
            }
        }

        private void chkIsBusiness_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsBusiness.Checked)
                {
                    chkIS_DRUG_STORE.Enabled = true;

                }
                else
                {
                    chkIS_DRUG_STORE.Enabled = false;
                    chkIS_DRUG_STORE.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkAutoExpend_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                //if (chkAutoExpend.Checked)
                //{
                //    chkIS_DRUG_STORE.Enabled = true;
                //}
                //else
                //{
                //    chkIS_DRUG_STORE.Enabled = false;
                //    chkIS_DRUG_STORE.Checked = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkDisplay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIS_DRUG_STORE.Focus();

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpUnit_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboImpUnit.EditValue != null)
                    spUnitConvertRatio.Enabled = true;
                else
                {
                    spUnitConvertRatio.Enabled = false;
                    spUnitConvertRatio.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboImpUnit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboImpUnit.EditValue = null;
                    spUnitConvertRatio.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpUnit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboImpUnit.EditValue == null)
                        chkIsChemiscalSubstance.Focus();
                    else
                    {
                        spUnitConvertRatio.Focus();
                        spUnitConvertRatio.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spUnitConvertRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtModel.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spUnitConvertRatio_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spUnitConvertRatio.Enabled == true)
                {
                    if (!String.IsNullOrWhiteSpace(spUnitConvertRatio.Text))
                    {
                        if (spUnitConvertRatio.Text.IndexOf('0') == spUnitConvertRatio.Text.Length - 1)
                        {
                            string a = spUnitConvertRatio.Text.Substring(0, spUnitConvertRatio.Text.Length - 1);
                            spUnitConvertRatio.Text = a;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spinAlertMaxInDay_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

        }

        private void spinAlertMaxInPrescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spinImpVatRatio_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spinLastExpVatPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtRegisterNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtConcentra.Focus();
                    txtConcentra.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAlertMinInStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtHienLimitPriceOld_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spinAlertExpiredDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spinMaxReuseCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spUnitConvertRatio_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtModel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsChemiscalSubstance.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void chkIsIdentityManagement_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ChkIsSizeRequired.Focus();
                    // chkIsOutHospital.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNgoaiDRG_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spinMaxReuseCount.Focus();
            }
        }

        private void ChkIsSizeRequired_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinMaxReuseCount.Enabled)
                        spinMaxReuseCount.Focus();
                    else if (lciPricingMaxReuseCount.Enabled)
                        spPricingMaxReuseCount.Focus();
                    else
                        ChkIsFilm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsIdentityManagement_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsIdentityManagement.Checked || chkMaxReuseCount.Checked)
                {
                    lciSizeRequired.Enabled = true;
                    ChkIsSizeRequired.Enabled = true;
                }
                else
                {
                    lciSizeRequired.Enabled = false;
                    ChkIsSizeRequired.Enabled = false;
                    ChkIsSizeRequired.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spReuseFee_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lciSizeRequired.Enabled)
                        ChkIsSizeRequired.Focus();
                    else if (spinMaxReuseCount.Enabled)
                        spinMaxReuseCount.Focus();
                    else if (lciPricingMaxReuseCount.Enabled)
                        spPricingMaxReuseCount.Focus();
                    else
                        ChkIsFilm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spPricingMaxReuseCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ChkIsFilm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spReuseFee_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spPricingMaxReuseCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar != ',' && e.KeyChar != '.' && !Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
                if ((e.KeyChar == '.' && (sender as NumericUpDown).Text.IndexOf('.') > -1) || (e.KeyChar == ',' && (sender as NumericUpDown).Text.IndexOf(',') > -1))
                {
                    // Nếu có nhiều hơn một dấu chấm, hủy sự kiện KeyPress
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spinMaxReuseCount_Leave(object sender, EventArgs e)
        {
            try
            {
                if (spinMaxReuseCount.EditValue != null)
                    spPricingMaxReuseCount.EditValue = spinMaxReuseCount.Value;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}


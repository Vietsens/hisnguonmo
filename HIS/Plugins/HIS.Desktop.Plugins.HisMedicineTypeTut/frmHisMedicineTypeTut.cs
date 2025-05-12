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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisMedicineTypeTut.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisMedicineTypeTut
{
    public partial class frmHisMedicineTypeTut : HIS.Desktop.Utility.FormBase
    {
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int ActionType = -1;
        int positionHandle = -1;
        PagingGrid pagingGrid;
        long medicineTypeTutId;
        string serviceUnitName;
        MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_TUT currentData;
        Inventec.Desktop.Common.Modules.Module moduleData;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        ACS_USER loginName = null;
        private HIS_MEDICINE_TYPE_TUT medicineTypeTut;
        List<V_HIS_MEDICINE_TYPE_TUT> listMedicineTypeTut = new List<V_HIS_MEDICINE_TYPE_TUT>();
        internal MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_TUT currentHisMedicine { get; set; }
        List<V_HIS_MEDICINE_TYPE_TUT> dataMedicineTypeTut { get; set; }
        string LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

        public frmHisMedicineTypeTut(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmHisMedicineTypeTut(HIS_MEDICINE_TYPE_TUT medicineTypeTut, Inventec.Desktop.Common.Modules.Module moduleData)
        {
            try
            {
                this.Text = moduleData.text;
                this.medicineTypeTut = medicineTypeTut;
                this.moduleData = moduleData;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisMedicineTypeTut_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
                LoadDataToComboMedicineType();
                // LoadDataToComboMedicineUseForm(null);
                LoadDataToComboHut();
                if ((this.ActionType == GlobalVariables.ActionEdit || this.ActionType == GlobalVariables.ActionView) && this.currentHisMedicine != null)
                {
                    LoadDataDebateDiagnostic(this.currentHisMedicine);//Load du lieu san co
                }
                txtLoginName.Text = LoginName;
                ValidateForm();
                this.ActionType = GlobalVariables.ActionAdd;
                ResetFormData();
                EnableControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMedicineTypeTut.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMedicineTypeTut.frmHisMedicineTypeTut).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtFind.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.txtFind.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.barButtonItemEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.barButtonItemAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemCancel.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.barButtonItemCancel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHTU.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.cboHTU.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMedicineType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.cboMedicineType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicineType.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.lciMedicineType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMorning.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.lciMorning.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNoon.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.lciNoon.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAfterNoon.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.lciAfterNoon.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEvening.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.lciEvening.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicineTypeTut.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDebateDiagnostic(V_HIS_MEDICINE_TYPE_TUT medicineTypeTutNew)
        {
            try
            {
                txtMedicineType.EditValue = medicineTypeTutNew.MEDICINE_TYPE_CODE;
                cboMedicineType.EditValue = medicineTypeTutNew.MEDICINE_TYPE_ID;
                txtLoginName.EditValue = medicineTypeTutNew.LOGINNAME;
                //txtMedicineUseForm.EditValue = medicineTypeTutNew.MEDICINE_USE_FORM_CODE;
                // cboMedicineUseForm.EditValue = medicineTypeTutNew.MEDICINE_USE_FORM_ID;
                cboHTU.EditValue = medicineTypeTutNew.HTU_ID;
                txtMorning.EditValue = medicineTypeTutNew.MORNING;
                txtNoon.EditValue = medicineTypeTutNew.NOON;
                txtAfterNoon.EditValue = medicineTypeTutNew.AFTERNOON;
                txtEvening.EditValue = medicineTypeTutNew.EVENING;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                //ValidationSingleControl(txtLoginName);
                ValidationSingleControl(cboMedicineType);
                ValidationSingleControl(txtMedicineType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format(ResourceMessage.ChuaNhapTruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void LoadDataToComboLoginName()
        //{
        //    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //    //columnInfos.Add(new ColumnInfo("USERNAME", "", 100, 1));
        //    columnInfos.Add(new ColumnInfo("LOGINNAME", "", 250, 2));
        //    ControlEditorADO controlEditorADO = new ControlEditorADO("LOGINNAME", "LOGINNAME", columnInfos, false, 350);
        //    ControlEditorLoader.Load(cboLoginName, BackendDataWorker.Get<ACS_USER>(), controlEditorADO);
        //}

        private void LoadDataToComboHut()
        {
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            //columnInfos.Add(new ColumnInfo("HTU_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("HTU_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("HTU_NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(cboHTU, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HTU>(), controlEditorADO);
        }

        private void LoadDataToComboMedicineUseForm(long? medicineUseFormId)
        {
            //List<HIS_MEDICINE_USE_FORM> medicineUseForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>();
            ////List<HIS_MEDICINE_USE_FORM> medicineUseForm = new BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>();
            //if (medicineUseFormId.HasValue)
            //{
            //    medicineUseForm = medicineUseForm.Where(o => o.ID == medicineUseFormId).ToList();
            //}
            //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            //columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_CODE", "", 100, 1));
            //columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 250, 2));
            //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 350);
            //ControlEditorLoader.Load(cboMedicineUseForm, medicineUseForm, controlEditorADO);
        }

        private void LoadDataToComboMedicineType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMedicineType, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                int numPageSize;
                numPageSize = (this.ucPaging1.pagingGrid != null) ? this.ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                this.FillDataToGridMedicineTypeTut(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                this.ucPaging1.Init(this.FillDataToGridMedicineTypeTut, param, numPageSize, this.gridControlMedicineTypeTut);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMedicineTypeTut(object data)
        {
            try
            {
                WaitingManager.Show();
                start = ((CommonParam)data).Start ?? 0;
                limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_MEDICINE_TYPE_TUT>> apiResult = null;
                HisMedicineTypeTutViewFilter filter = new HisMedicineTypeTutViewFilter();
                filter.ORDER_DIRECTION = "MODIFY_TIME";
                filter.ORDER_FIELD = "DESC";
                filter.KEY_WORD = txtFind.Text;
                filter.CREATOR = LoginName;
                listMedicineTypeTut = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICINE_TYPE_TUT>>(
                    "api/HisMedicineTypeTut/GetView", ApiConsumers.MosConsumer, filter, null);

                apiResult = new BackendAdapter(param).GetRO<List<V_HIS_MEDICINE_TYPE_TUT>>(
                    "api/HisMedicineTypeTut/GetView", ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null)
                {
                    dataMedicineTypeTut = (List<V_HIS_MEDICINE_TYPE_TUT>)apiResult.Data;
                    dataMedicineTypeTut = dataMedicineTypeTut.Where(o => o.LOGINNAME == LoginName).ToList();
                    //var apiResults = apiResult.Data.Where(o => o.LOGINNAME == LoginName).ToList();
                    gridControlMedicineTypeTut.DataSource = null;
                    gridControlMedicineTypeTut.DataSource = dataMedicineTypeTut;
                    rowCount = (dataMedicineTypeTut == null ? 0 : dataMedicineTypeTut.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicineTypeTut_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                // LoadDataToComboMedicineUseForm(null);
                var row = (V_HIS_MEDICINE_TYPE_TUT)gridViewMedicineTypeTut.GetFocusedRow();
                currentData = row as V_HIS_MEDICINE_TYPE_TUT;
                if (row != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().SingleOrDefault(o => o.ID == row.MEDICINE_TYPE_ID);
                    if (gt != null)
                    {
                        serviceUnitName = gt.SERVICE_UNIT_NAME;
                    }
                    ChangedDataRow(row);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangedDataRow(V_HIS_MEDICINE_TYPE_TUT data)
        {
            try
            {
                if (data != null)
                {
                    this.ActionType = GlobalVariables.ActionEdit;
                    FillDataToEditorControl(data);
                    EnableControlChanged(this.ActionType);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToEditorControl(V_HIS_MEDICINE_TYPE_TUT data)
        {
            try
            {
                if (data != null)
                {
                    txtDayCount.EditValue = data.DAY_COUNT;
                    medicineTypeTutId = data.ID;
                    cboHTU.EditValue = data.HTU_ID;
                    txtLoginName.EditValue = data.LOGINNAME;
                    cboMedicineType.EditValue = data.MEDICINE_TYPE_ID;
                    //cboMedicineUseForm.EditValue = data.MEDICINE_USE_FORM_ID;
                    txtMorning.EditValue = data.MORNING;
                    txtNoon.EditValue = data.NOON;
                    txtAfterNoon.EditValue = data.AFTERNOON;
                    txtEvening.EditValue = data.EVENING;
                    txtMedicineType.EditValue = data.MEDICINE_TYPE_CODE;
                    txtHD.Text = data.TUTORIAL;
                    txtCachDung.Text = data.HTU_TEXT;
                    // txtMedicineUseForm.EditValue = data.MEDICINE_USE_FORM_CODE;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicineTypeTut_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_MEDICINE_TYPE_TUT data = (V_HIS_MEDICINE_TYPE_TUT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + start;
                            }
                            catch (Exception ex)
                            {

                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri STT", ex);
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

        private void gridViewMedicineTypeTut_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    // V_HIS_MEDICINE_TYPE_TUT data = (V_HIS_MEDICINE_TYPE_TUT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewMedicineTypeTut.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());

                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnUnLock : btnLock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDELETE : btnDELETE_Disable);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGridControl();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //var datanew = dataMedicineTypeTut.Where(o => o.MEDICINE_TYPE_CODE.ToUpper().Contains(txtFind.Text.ToUpper())).ToList();
                var datanew = dataMedicineTypeTut.Where(o => (o.MEDICINE_TYPE_CODE).ToUpper().Contains(txtFind.Text.ToUpper()) || (o.MEDICINE_TYPE_NAME).ToUpper().Contains(txtFind.Text.ToUpper())).ToList();
                //var datanew2 = dataMedicineTypeTut.Where(o => o.MEDICINE_TYPE_NAME.ToUpper().Contains(txtFind.Text.ToUpper())).ToList();
                gridControlMedicineTypeTut.DataSource = null;
                gridControlMedicineTypeTut.DataSource = datanew;
                //gridControlMedicineTypeTut.DataSource = datanew2;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                //LoadDataToComboMedicineUseForm(null);
                txtMedicineType.EditValue = null;
                cboMedicineType.EditValue = null;
                //txtMedicineUseForm.EditValue = null;
                // cboMedicineUseForm.EditValue = null;
                txtMorning.EditValue = null;
                txtNoon.EditValue = null;
                txtAfterNoon.EditValue = null;
                txtEvening.EditValue = null;
                cboHTU.EditValue = null;
                txtLoginName.EditValue = LoginName;
                txtHD.EditValue = null;
                txtCachDung.EditValue = null;
                txtDayCount.EditValue = null;
                spinAmount.EditValue = null;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDICINE_TYPE_TUT medicineTypeTut = new HIS_MEDICINE_TYPE_TUT();
            bool notHandler = false;
            try
            {
                WaitingManager.Show();
                V_HIS_MEDICINE_TYPE_TUT dataMedicineTypeTut = (V_HIS_MEDICINE_TYPE_TUT)gridViewMedicineTypeTut.GetFocusedRow();
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_MEDICINE_TYPE_TUT data = new HIS_MEDICINE_TYPE_TUT();
                    data.ID = dataMedicineTypeTut.ID;
                    WaitingManager.Show();
                    medicineTypeTut = new BackendAdapter(param).Post<HIS_MEDICINE_TYPE_TUT>(
                        "api/HisMedicineTypeTut/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                    if (medicineTypeTut != null)
                    {
                        notHandler = true;
                        btnEdit.Enabled = false;
                        FillDataToGridControl();
                    }
                }
                else
                {
                    return;
                }
                MessageManager.Show(this.ParentForm, param, notHandler);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HIS_MEDICINE_TYPE_TUT medicineTypeTut = new HIS_MEDICINE_TYPE_TUT();
                bool notHandler = false;
                V_HIS_MEDICINE_TYPE_TUT dataMedicineTypeTut = (V_HIS_MEDICINE_TYPE_TUT)gridViewMedicineTypeTut.GetFocusedRow();
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonMoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_MEDICINE_TYPE_TUT data = new HIS_MEDICINE_TYPE_TUT();
                    data.ID = dataMedicineTypeTut.ID;
                    WaitingManager.Show();
                    medicineTypeTut = new BackendAdapter(param).Post<HIS_MEDICINE_TYPE_TUT>(
                        "api/HisMedicineTypeTut/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                    if (medicineTypeTut != null)
                    {
                        notHandler = true;
                        btnEdit.Enabled = true;
                        FillDataToGridControl();
                    }
                }
                else
                {
                    return;
                }
                MessageManager.Show(this.ParentForm, param, notHandler);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDELETE_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var data = (V_HIS_MEDICINE_TYPE_TUT)gridViewMedicineTypeTut.GetFocusedRow();
                V_HIS_MEDICINE_TYPE_TUT rowData = data as V_HIS_MEDICINE_TYPE_TUT;
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisMedicineTypeTut/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
                else
                {
                    return;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcess()
        {
            try
            {
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                {
                    return;
                }
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HIS_MEDICINE_TYPE_TUT updateDTO = new HIS_MEDICINE_TYPE_TUT();

                UpdateDTOFromDataForm(ref updateDTO);
                if (updateDTO.DAY_COUNT == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Số ngày dùng phải lớn hơn 0.", "Thông báo");
                    return;
                }
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>(
                        "api/HisMedicineTypeTut/Create", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    if (medicineTypeTutId > 0)
                    {
                        updateDTO.ID = medicineTypeTutId;

                        if (listMedicineTypeTut.FirstOrDefault(o => o.MEDICINE_TYPE_ID == updateDTO.MEDICINE_TYPE_ID) != null && listMedicineTypeTut.FirstOrDefault(o => o.MEDICINE_TYPE_ID == updateDTO.MEDICINE_TYPE_ID).ID != medicineTypeTutId)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Trùng loại thuốc trên từ điển", "Thông báo");
                            return;
                        }
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>(
                        "api/HisMedicineTypeTut/Update", ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            // UpdateRowDataAfterEdit(resultData);
                        }
                    }
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateRowDataAfterEdit(HIS_MEDICINE_TYPE_TUT data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_TUT)gridViewMedicineTypeTut.GetFocusedRow();
                if (rowData != null)
                {
                    //data = AutoMapper.Mapper.Map<V_HIS_SERVICE_PATY, V_HIS_SERVICE_PATY>(rowData);
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_TUT>(rowData, data);
                    gridViewMedicineTypeTut.RefreshRow(gridViewMedicineTypeTut.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_MEDICINE_TYPE_TUT updateDTO)
        {
            try
            {
                if (cboHTU.EditValue != null) updateDTO.HTU_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboHTU.EditValue ?? "0").ToString());
                if (cboMedicineType.EditValue != null) updateDTO.MEDICINE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineType.EditValue ?? "0").ToString());
                //if (cboMedicineUseForm.EditValue != null) updateDTO.MEDICINE_USE_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineUseForm.EditValue ?? "0").ToString());
                //if (cboLoginName.EditValue != null) updateDTO.LOGINNAME = (cboLoginName.EditValue ?? "0").ToString();
                updateDTO.MORNING = (string)txtMorning.EditValue;
                updateDTO.NOON = (string)txtNoon.EditValue;
                updateDTO.AFTERNOON = (string)txtAfterNoon.EditValue;
                updateDTO.EVENING = (string)txtEvening.EditValue;
                updateDTO.LOGINNAME = (string)txtLoginName.EditValue;
                updateDTO.DAY_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64((txtDayCount.EditValue).ToString());
                updateDTO.TUTORIAL = (string)txtHD.Text;
                updateDTO.HTU_TEXT = (string)txtCachDung.Text;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMedicineType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtMedicineType.Text))
                    {
                        string key = Inventec.Common.String.Convert.UnSignVNese(txtMedicineType.Text.ToLower().Trim());
                        var data = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(o => Inventec.Common.String.Convert.UnSignVNese(o.MEDICINE_TYPE_CODE.ToLower()).Contains(key)).ToList();


                        List<HIS_MEDICINE_TYPE> results = data != null ? (data.Count == 1 ? data : data.Where(o => o.MEDICINE_TYPE_CODE.ToLower() == key).ToList()) : null;
                        if (results != null && results.Count == 1)
                        {
                            valid = true;
                            txtMedicineType.Text = results.First().MEDICINE_TYPE_CODE;
                            cboMedicineType.EditValue = results.First().ID;
                            cboHTU.Focus();
                            cboHTU.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboMedicineType.Focus();
                        cboMedicineType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedicineType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {

                    CalculateAmount();
                    BaseEdit baseEdit = sender as BaseEdit;
                    if (baseEdit != null && baseEdit.EditorContainsFocus)
                        SetHuongDanFromSoLuongNgay();
                    if (cboMedicineType.EditValue != null && cboMedicineType.EditValue != cboMedicineType.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineType.EditValue.ToString()));
                        if (gt != null)
                        {
                            serviceUnitName = gt.SERVICE_UNIT_NAME;
                            txtMedicineType.Text = gt.MEDICINE_TYPE_CODE;
                            // LoadDataToComboMedicineUseForm(gt.MEDICINE_USE_FORM_ID);
                            cboHTU.Focus();
                        }
                    }
                    else
                    {
                        cboHTU.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMedicineType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineType.EditValue.ToString()));
                        if (gt != null)
                        {
                            serviceUnitName = gt.SERVICE_UNIT_NAME;
                            cboHTU.Focus();
                            cboMedicineType.ShowPopup();
                        }
                    }
                    else
                    {
                        cboMedicineType.ShowPopup();
                    }
                }
                else
                {
                    cboMedicineType.ShowPopup();
                }
                CalculateAmount();
                BaseEdit baseEdit = sender as BaseEdit;
                if (baseEdit != null && baseEdit.EditorContainsFocus)
                    SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMedicineUseForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //    try
            //    {
            //        if (e.KeyCode == Keys.Enter)
            //        {
            //            bool valid = false;
            //            if (!String.IsNullOrEmpty(txtMedicineUseForm.Text))
            //            {
            //                string key = txtMedicineUseForm.Text.Trim().ToLower();
            //                var data = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().Where(o => o.MEDICINE_USE_FORM_CODE.ToLower().Contains(key) ||
            //                    o.MEDICINE_USE_FORM_NAME.ToLower().Contains(key)).ToList();
            //                if (data != null && data.Count == 1)
            //                {
            //                    valid = true;
            //                    txtMedicineUseForm.Text = data.First().MEDICINE_USE_FORM_CODE;
            //                    cboMedicineUseForm.EditValue = data.First().ID;
            //                    cboHTU.Focus();
            //                    //txtServiceName.SelectAll();
            //                }
            //            }
            //            if (!valid)
            //            {
            //                cboMedicineUseForm.Focus();
            //                cboMedicineUseForm.ShowPopup();
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //        Inventec.Common.Logging.LogSystem.Error(ex);
            //    }
        }

        private void cboMedicineUseForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            //try
            //{
            //    if (e.CloseMode == PopupCloseMode.Normal)
            //    {

            //        CalculateAmount();
            //        SetHuongDanFromSoLuongNgay();
            //        if (cboMedicineUseForm.EditValue != null && cboMedicineUseForm.EditValue != cboMedicineUseForm.OldEditValue)
            //        {
            //            MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineUseForm.EditValue.ToString()));
            //            if (gt != null)
            //            {
            //                txtMedicineUseForm.Text = gt.MEDICINE_USE_FORM_CODE;
            //                cboHTU.Focus();
            //            }
            //        }
            //        else
            //        {
            //            cboHTU.Focus();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboMedicineUseForm_KeyUp(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        if (cboMedicineUseForm.EditValue != null)
            //        {
            //            MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineUseForm.EditValue.ToString()));
            //            if (gt != null)
            //            {
            //                cboHTU.Focus();
            //            }
            //        }
            //        else
            //        {
            //            cboMedicineUseForm.ShowPopup();
            //        }
            //    }
            //    else
            //    {
            //        cboMedicineUseForm.ShowPopup();
            //    }
            //    CalculateAmount();
            //    SetHuongDanFromSoLuongNgay();
            //}
            //catch (Exception ex)
            //{

            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void cboHTU_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHTU.EditValue != null && cboHTU.EditValue != cboHTU.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_HTU gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HTU>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboHTU.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtDayCount.Focus();
                        }
                    }
                    else
                    {
                        txtDayCount.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTU_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHTU.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_HTU gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HTU>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboHTU.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtDayCount.Focus();
                        }
                    }
                    else
                    {
                        cboHTU.ShowPopup();
                    }
                }
                else
                {
                    cboHTU.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMorning_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNoon.Focus();
                    txtNoon.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNoon_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAfterNoon.Focus();
                    txtAfterNoon.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAfterNoon_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEvening.Focus();
                    txtEvening.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEvening_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled == true)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMorning_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SpinValidating(object sender, CancelEventArgs e)
        {
            try
            {
                decimal amountInput = 0;
                string vl = (sender as DevExpress.XtraEditors.TextEdit).Text;
                try
                {
                    if (vl.Contains("/"))
                    {
                        vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        var arrNumber = vl.Split('/');
                        if (arrNumber != null && arrNumber.Count() > 1)
                        {
                            amountInput = Convert.ToDecimal(arrNumber[0]) / Convert.ToDecimal(arrNumber[1]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    amountInput = 0;
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNoon_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAfterNoon_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEvening_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMorning_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                CalculateAmount();
                BaseEdit baseEdit = sender as BaseEdit;
                if (baseEdit != null && baseEdit.EditorContainsFocus)
                    SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetHuongDanFromSoLuongNgay()
        {
            try
            {

                //string serviceUnitName = "";
                bool isUse = false;
                if (cboMedicineType.EditValue != null)
                {
                    string huongDan = "";
                    string format__NgayUong = ResourceMessage.NgayUong;
                    string format___NgayXVienBuoiYZ = ResourceMessage._NgayXVienBuoiYZ;
                    string format__Sang = ResourceMessage.Sang;
                    string format__Trua = ResourceMessage.Trua;
                    string format__Chieu = ResourceMessage.Chieu;
                    string format__Toi = ResourceMessage.Toi;
                    string format__NgaySuDung = ResourceMessage.NgaySuDung;
                    string strSeperator = ", ";
                    int solan = 0;
                    string buoiChon = "";
                    string soNgayDung = "";
                    double sang, trua, chieu, toi, ngayDung = 0;
                    sang = GetValueSpin(txtMorning.Text);
                    trua = GetValueSpin(txtNoon.Text);
                    chieu = GetValueSpin(txtAfterNoon.Text);
                    toi = GetValueSpin(txtEvening.Text);
                    ngayDung = GetValueSpin(txtDayCount.Text);
                    if (sang > 0
                    || trua > 0
                    || chieu > 0
                    || toi > 0
                        && ngayDung > 0)
                    {
                        if (sang > 0) { solan += 1; buoiChon = ResourceMessage.BuoiSang; }
                        if (trua > 0) { solan += 1; buoiChon = ResourceMessage.BuoiTrua; }
                        if (chieu > 0) { solan += 1; buoiChon = ResourceMessage.BuoiChieu; }
                        if (toi > 0) { solan += 1; buoiChon = ResourceMessage.BuoiToi; }
                        if (ngayDung > 0)
                        {
                            soNgayDung = ResourceMessage.NGayDung;
                        }
                        double tong_s_t_c_t = (sang + trua + chieu + toi);
                        if (solan == 1)
                        {
                            if ((int)tong_s_t_c_t == tong_s_t_c_t)
                            {
                                huongDan = (!String.IsNullOrEmpty(spinAmount.Text) ? String.Format(format___NgayXVienBuoiYZ, " ", " " + (int)tong_s_t_c_t, serviceUnitName, buoiChon) : "");
                                huongDan += (ngayDung > 0 ? ((String.IsNullOrEmpty(huongDan) ? "" : strSeperator) + String.Format(format__NgaySuDung, ConvertNumber.Dec2frac(ngayDung), "Ngày")) : "");
                            }
                            else
                            {
                                huongDan = (!String.IsNullOrEmpty(spinAmount.Text) ? String.Format(format___NgayXVienBuoiYZ, ConvertNumber.Dec2frac(tong_s_t_c_t), serviceUnitName, buoiChon) : "");
                            }
                        }
                        else
                        {
                            if ((int)tong_s_t_c_t == tong_s_t_c_t)
                            {
                                huongDan = (tong_s_t_c_t > 0 ? String.Format(format__NgayUong, " ", " " + (int)tong_s_t_c_t, serviceUnitName, solan) : "");
                            }
                            else
                            {
                                huongDan = (tong_s_t_c_t > 0 ? String.Format(format__NgayUong, ConvertNumber.Dec2frac(tong_s_t_c_t), serviceUnitName, solan) : "");
                                //(String.IsNullOrEmpty(cboMedicineUseForm.Text) ? "" : " " + cboMedicineUseForm.Text + " "),
                            }

                            huongDan += (sang > 0 ? String.Format(format__Sang, ConvertNumber.Dec2frac(sang), serviceUnitName) : "");
                            huongDan += (trua > 0 ? ((String.IsNullOrEmpty(huongDan) ? "" : strSeperator) + String.Format(format__Trua, ConvertNumber.Dec2frac(trua), serviceUnitName)) : "");
                            huongDan += (chieu > 0 ? ((String.IsNullOrEmpty(huongDan) ? "" : strSeperator) + String.Format(format__Chieu, ConvertNumber.Dec2frac(chieu), serviceUnitName)) : "");
                            huongDan += (toi > 0 ? ((String.IsNullOrEmpty(huongDan) ? "" : strSeperator) + String.Format(format__Toi, ConvertNumber.Dec2frac(toi), serviceUnitName)) : "");
                            huongDan += (ngayDung > 0 ? ((String.IsNullOrEmpty(huongDan) ? "" : strSeperator) + String.Format(format__NgaySuDung, ConvertNumber.Dec2frac(ngayDung), "Ngày")) : "");
                        }

                        huongDan += (!String.IsNullOrEmpty(cboHTU.Text) ? " (" + cboHTU.Text + ")" : "");
                    }
                    huongDan = huongDan.Replace("  ", " ");
                    txtHD.Text = huongDan;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateAmount()
        {
            try
            {
                double sang, trua, chieu, toi = 0;
                sang = GetValueSpin(txtMorning.Text);
                trua = GetValueSpin(txtNoon.Text);
                chieu = GetValueSpin(txtAfterNoon.Text);
                toi = GetValueSpin(txtEvening.Text);
                if (sang > 0
                    || trua > 0
                    || chieu > 0
                    || toi > 0)
                {
                    long roomTypeId = 0;
                    if (moduleData != null && moduleData.RoomId > 0 && moduleData.RoomTypeId > 0)
                    {
                        roomTypeId = moduleData.RoomTypeId;
                    }

                    double tong_s_t_c_t = (sang + trua + chieu + toi);
                    spinAmount.Text = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)((double)(txtDayCount.Value) * (tong_s_t_c_t)), 2) + "";
                }
                else
                {
                    spinAmount.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private double GetValueSpin(string strValue)
        {
            double value = 0;
            try
            {
                if (!String.IsNullOrEmpty(strValue))
                {
                    string vl = strValue;
                    vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    if (vl.Contains("/"))
                    {
                        var arrNumber = vl.Split('/');
                        if (arrNumber != null && arrNumber.Count() > 1)
                        {
                            value = Convert.ToDouble(arrNumber[0]) / Convert.ToDouble(arrNumber[1]);
                        }
                    }
                    else
                    {
                        value = Convert.ToDouble(vl);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }

        private void txtNoon_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalculateAmount();
                BaseEdit baseEdit = sender as BaseEdit;
                if (baseEdit != null && baseEdit.EditorContainsFocus)
                    SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAfterNoon_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalculateAmount();
                BaseEdit baseEdit = sender as BaseEdit;
                if (baseEdit != null && baseEdit.EditorContainsFocus)
                    SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEvening_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalculateAmount();
                BaseEdit baseEdit = sender as BaseEdit;
                if (baseEdit != null && baseEdit.EditorContainsFocus)
                    SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDayCount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalculateAmount();
                BaseEdit baseEdit = sender as BaseEdit;
                if (baseEdit != null && baseEdit.EditorContainsFocus)
                    SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDayCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMorning.Focus();
                    txtMorning.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

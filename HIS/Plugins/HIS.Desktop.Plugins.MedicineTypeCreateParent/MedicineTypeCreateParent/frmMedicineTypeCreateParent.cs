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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Common;
using Inventec.UC.Paging;
using Inventec.Common.Logging;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.MedicineTypeCreateParent.MedicineTypeCreateParent
{
    public partial class frmMedicineTypeCreateParent : HIS.Desktop.Utility.FormBase
    {
        #region ---Declare
        PagingGrid pagingGrid;
        Inventec.Desktop.Common.Modules.Module moduleData;
        DelegateSelectData delegateSelect = null;
        V_HIS_MEDICINE_TYPE MedicineType;
        #endregion
        public frmMedicineTypeCreateParent(Inventec.Desktop.Common.Modules.Module moduleData, DelegateSelectData _delegateSelect)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this.pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                delegateSelect = _delegateSelect;
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

        public frmMedicineTypeCreateParent(Inventec.Desktop.Common.Modules.Module moduleData, DelegateSelectData _delegateSelect, V_HIS_MEDICINE_TYPE madicine)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this.pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                delegateSelect = _delegateSelect;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    if (madicine != null)
                    {
                        this.MedicineType = madicine;
                    }
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

        private void frmMedicineTypeCreateParent_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefautData();
                ValidateForm();
                SetCaptionByLanguageKey();
                FillDataToCombo();
                if (this.MedicineType != null)
                {
                    txtServiceUnitCode.Text = MedicineType.SERVICE_UNIT_CODE;
                    if (MedicineType.SERVICE_UNIT_ID > 0)
                    {
                        cboServiceUnit.EditValue = MedicineType.SERVICE_UNIT_ID;
                        txtServiceUnitCode.Text = MedicineType.SERVICE_UNIT_CODE;
                    }
                    else
                    {
                        var ServiceUnit = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().FirstOrDefault();
                        txtServiceUnitCode.Text = ServiceUnit.SERVICE_UNIT_CODE;
                        cboServiceUnit.EditValue = ServiceUnit.ID;
                    }

                    txtMedicineUserFormCode.Text = MedicineType.MEDICINE_USE_FORM_CODE;
                    if (MedicineType.MEDICINE_USE_FORM_ID > 0)
                    {
                        cboMedicineUseForm.EditValue = MedicineType.MEDICINE_USE_FORM_ID;
                        txtMedicineUserFormCode.Text = MedicineType.MEDICINE_USE_FORM_CODE;
                    }
                    else
                    {
                        var MedicineUserForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault();
                        txtMedicineUserFormCode.Text = MedicineUserForm.MEDICINE_USE_FORM_CODE;
                        cboMedicineUseForm.EditValue = MedicineUserForm.ID;
                    }
                    if (MedicineType.MEDICINE_LINE_ID > 0)
                        cboMedicineLine.EditValue = MedicineType.MEDICINE_LINE_ID;
                    else
                        cboMedicineLine.EditValue = BackendDataWorker.Get<HIS_MEDICINE_LINE>().FirstOrDefault().ID;
                }
               
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #region ---set data
        private void FillDataToCombo()
        {
            try
            {
                InitMedincineUseForm();
                InitMedicineLine();
                InitServiceUnit();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetDefautData()
        {
            try
            {
                txtMedicineTypeCode.Text = "";
                txtMedicineTypeName.Text = "";
                txtMedicineUserFormCode.Text = "";
                txtServiceUnitCode.Text = "";
                cboMedicineLine.EditValue = null;
                cboMedicineUseForm.EditValue = null;
                cboServiceUnit.EditValue = null;

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }


        private void ValidateForm()
        {
            try
            {
                ValidationControlTextEditName();
                ValidationControlTextEditCode();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditCode()
        {
            try
            {
                ValidateMaxlength validRule = new ValidateMaxlength();
                validRule.required = true;
                validRule.txtEdit = txtMedicineTypeCode;
                validRule.maxLenght = 25;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtMedicineTypeCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationControlTextEditName()
        {
            try
            {
                ValidateMaxlength validRule = new ValidateMaxlength();
                validRule.required = true;
                validRule.txtEdit = txtMedicineTypeName;
                validRule.maxLenght = 500;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtMedicineTypeName, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = moduleData.text;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void RestFromData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized)
                    return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
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
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMedicineUseForm(string _medicineUseFormCode)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> listResult = new List<HIS_MEDICINE_USE_FORM>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().Where(o => (o.MEDICINE_USE_FORM_CODE != null && o.MEDICINE_USE_FORM_CODE.StartsWith(_medicineUseFormCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboMedicineUseForm, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboMedicineUseForm.EditValue = listResult[0].ID;
                    txtMedicineUserFormCode.Text = listResult[0].MEDICINE_USE_FORM_CODE;
                    txtServiceUnitCode.Focus();
                    txtServiceUnitCode.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboMedicineUseForm.EditValue = null;
                    cboMedicineUseForm.Focus();
                    cboMedicineUseForm.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboMedicineUseForm.EditValue = null;
                    cboMedicineUseForm.Focus();
                    cboMedicineUseForm.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region ---Load dat to combo
        private void InitMedincineUseForm()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboMedicineUseForm, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitServiceUnit()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboServiceUnit, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMedicineLine()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_LINE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_LINE_NAME", "", 100, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_LINE_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboMedicineLine, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion



        void SetDataToMedicineLine(object _medicineLine)
        {
            try
            {
                if (_medicineLine != null && _medicineLine is MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDICINE_LINE_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("MEDICINE_LINE_NAME", "", 100, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_LINE_NAME", "ID", columnInfos, false, 150);
                    MOS.Filter.HisMedicineLineFilter filter = new HisMedicineLineFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    CommonParam param = new CommonParam();
                    var heinServiceBhyts = new BackendAdapter(param).Get<List<HIS_MEDICINE_LINE>>(HisRequestUriStore.HisMedicineLine_Get, ApiConsumers.MosConsumer, filter, param);
                    ControlEditorLoader.Load(cboMedicineLine, heinServiceBhyts, controlEditorADO);
                    cboMedicineLine.EditValue = ((MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE)_medicineLine).ID;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RefreshDataToMedicineUseForm(object medicineUseForm)
        {
            try
            {
                if (medicineUseForm != null && medicineUseForm is MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 100, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 150);
                    MOS.Filter.HisMedicineUseFormFilter filter = new HisMedicineUseFormFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    CommonParam param = new CommonParam();
                    var hisMedicineUseForms = new BackendAdapter(param).Get<List<HIS_MEDICINE_USE_FORM>>(HisRequestUriStore.HisMedicineUseForm_Get, ApiConsumers.MosConsumer, filter, param);
                    ControlEditorLoader.Load(cboMedicineUseForm, hisMedicineUseForms, controlEditorADO);
                    cboMedicineUseForm.EditValue = ((MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM)medicineUseForm).ID;
                    txtMedicineUserFormCode.Text = ((MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM)medicineUseForm).MEDICINE_USE_FORM_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceUnit(string _serviceUnitCode)
        {
            try
            {
                List<HIS_SERVICE_UNIT> listResult = new List<HIS_SERVICE_UNIT>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().Where(o => (o.SERVICE_UNIT_CODE != null && o.SERVICE_UNIT_CODE.StartsWith(_serviceUnitCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboServiceUnit, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboServiceUnit.EditValue = listResult[0].ID;
                    txtServiceUnitCode.Text = listResult[0].SERVICE_UNIT_CODE;
                    cboMedicineLine.Focus();
                    cboMedicineLine.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboServiceUnit.EditValue = null;
                    cboServiceUnit.Focus();
                    cboServiceUnit.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboServiceUnit.EditValue = null;
                    cboServiceUnit.Focus();
                    cboServiceUnit.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RefeshDataAfterSave(HIS_MEDICINE_TYPE data)
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UpdateDTOFromDataToForm(HIS_MEDICINE_TYPE data)
        {
            try
            {
                data.MEDICINE_TYPE_CODE = txtMedicineTypeCode.Text.Trim();
                data.MEDICINE_TYPE_NAME = txtMedicineTypeName.Text.Trim();
                data.HIS_SERVICE = new HIS_SERVICE();
                if (cboMedicineLine.EditValue != null)
                {
                    data.MEDICINE_LINE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineLine.EditValue.ToString());
                }
                
                if (cboServiceUnit.EditValue != null)
                {
                    data.TDL_SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceUnit.EditValue.ToString());
                    data.HIS_SERVICE.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceUnit.EditValue.ToString());
                }
                
                if (cboMedicineUseForm.EditValue != null)
                {
                    data.MEDICINE_USE_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineUseForm.EditValue.ToString());
                }
                
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SaveProcessor()
        {
            try
            {
                bool success = false;
                if (!btnAdd.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HIS_MEDICINE_TYPE UpdateDTO = new HIS_MEDICINE_TYPE();
                UpdateDTOFromDataToForm(UpdateDTO);

                var result = new BackendAdapter(param).Post<HIS_MEDICINE_TYPE>(HisRequestUriStore.CreateParent, ApiConsumers.MosConsumer, UpdateDTO, param);
                if (result != null)
                {
                    success = true;
                    BackendDataWorker.Reset<HIS_MEDICINE_TYPE>();
                    RefeshDataAfterSave(result);
                    RestFromData();
                    this.Close();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }




        #endregion
        #region ---Click
        #region ---ItemClick
        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }


        #endregion



        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {

                RestFromData();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                txtMedicineTypeCode.Focus();
                txtMedicineTypeCode.SelectAll();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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

                LogSystem.Error(ex);
            }
        }

        private void txtMedicineTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineUserFormCode.Focus();
                    txtMedicineUserFormCode.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtMedicineUserFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadMedicineUseForm(strValue);
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
        #endregion
        #region --- even Combo
        private void cboMedicineUseForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMedicineUseForm.Properties.Buttons[1].Visible = false;
                    cboMedicineUseForm.EditValue = null;
                    txtMedicineUserFormCode.Text = "";
                    txtServiceUnitCode.Focus();
                    txtServiceUnitCode.SelectAll();
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateSelectData)RefreshDataToMedicineUseForm);
                    if (this.moduleData == null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisMedicineUseForm, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisMedicineUseForm, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineUseForm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMedicineUseForm.EditValue != null)
                    {
                        var medicineUseForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineUseForm.EditValue ?? "").ToString()));
                        if (medicineUseForm != null)
                        {
                            txtMedicineUserFormCode.Text = medicineUseForm.MEDICINE_USE_FORM_CODE;
                            cboMedicineUseForm.Properties.Buttons[1].Visible = true;
                            txtServiceUnitCode.Focus();
                            txtServiceUnitCode.SelectAll();
                        }
                        else
                        {
                            cboMedicineUseForm.Focus();
                            cboMedicineUseForm.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineUseForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboMedicineUseForm.Text))
                    {
                        string key = cboMedicineUseForm.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().Where(o => o.MEDICINE_USE_FORM_CODE.ToLower().Contains(key) || o.MEDICINE_USE_FORM_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboMedicineUseForm.EditValue = listData.First().ID;
                            txtMedicineUserFormCode.Text = listData.First().MEDICINE_USE_FORM_CODE;
                            txtServiceUnitCode.Focus();
                            txtServiceUnitCode.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboMedicineUseForm.Focus();
                        cboMedicineUseForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                            cboMedicineLine.Focus();
                            cboMedicineLine.SelectAll();

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
                        }
                    }
                    if (!valid)
                    {
                        cboServiceUnit.Focus();
                        cboServiceUnit.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineLine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMedicineLine.Properties.Buttons[1].Visible = false;
                    cboMedicineLine.EditValue = null;

                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateSelectData)SetDataToMedicineLine);
                    if (this.moduleData == null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisMedicineLine, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisMedicineLine, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineLine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMedicineLine.EditValue != null)
                    {
                        var medicineLine = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineLine.EditValue ?? "").ToString()));
                        if (medicineLine != null)
                        {
                            btnAdd.Focus();

                        }
                        else
                        {
                            cboMedicineLine.Focus();
                            cboMedicineLine.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineLine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboMedicineLine.Text))
                    {
                        string key = cboMedicineLine.Text.ToLower();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>().Where(o => o.MEDICINE_LINE_CODE.ToLower().Contains(key) || o.MEDICINE_LINE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboMedicineLine.EditValue = listData.First().ID;
                            btnAdd.Focus();
                        }
                    }
                    if (!valid)
                    {
                        cboMedicineLine.Focus();
                        cboMedicineLine.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void bbtnF2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bbtnSave_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void bbtnRefresh_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }


    }
}

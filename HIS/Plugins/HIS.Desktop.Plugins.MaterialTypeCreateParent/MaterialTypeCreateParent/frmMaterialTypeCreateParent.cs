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

namespace HIS.Desktop.Plugins.MaterialTypeCreateParent.MaterialTypeCreateParent
{
    public partial class frmMaterialTypeCreateParent : HIS.Desktop.Utility.FormBase
    {
        #region --- Declare
        PagingGrid pagingGrid;
        Inventec.Desktop.Common.Modules.Module moduleData;
        DelegateSelectData delegateSelect = null;
        V_HIS_MATERIAL_TYPE Materialtype;
        #endregion
        public frmMaterialTypeCreateParent(Inventec.Desktop.Common.Modules.Module moduleData, DelegateSelectData _delegateSelect)
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
        public frmMaterialTypeCreateParent(Inventec.Desktop.Common.Modules.Module moduleData, DelegateSelectData _delegateSelect, V_HIS_MATERIAL_TYPE Material)
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
                    if (Material != null)
                    {
                        this.Materialtype = Material;
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
        #region --- Set data
        private void InitServiceUnit()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 100, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboServiceUnit, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMaterialTypeCreateParent_Load(object sender, EventArgs e)
        {
            try
            {

                InitServiceUnit();
                SetDefautData();
                ValidateForm();
                SetCaptionByLanguageKey();
                if (Materialtype!=null)
                {
                    txtServiceUnitCode.Text = Materialtype.SERVICE_UNIT_CODE;
                    cboServiceUnit.EditValue = Materialtype.SERVICE_UNIT_ID;
                }
                else
                {
                    var ServiceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault();
                    txtServiceUnitCode.Text = ServiceUnit.SERVICE_UNIT_CODE;
                    cboServiceUnit.EditValue = ServiceUnit.ID;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
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

        private void SetDefautData()
        {
            try
            {
                txtMetrialTypeCode.Text = "";
                txtMetrialTypeName.Text = "";
                txtServiceUnitCode.Text = "";
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
                ValidationControlTextEditCode();
                ValidationControlTextEditName();
                
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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
                if (listResult.Count == 1)
                {
                    cboServiceUnit.EditValue = listResult[0].ID;
                    txtServiceUnitCode.Text = listResult[0].SERVICE_UNIT_CODE;
                    btnAdd.Focus();
                }
                else if (listResult.Count > 1)
                {
                    cboServiceUnit.EditValue = null;
                    cboServiceUnit.Focus();
                    cboServiceUnit.ShowPopup();
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

        private void ValidationControlTextEditCode()
        {
            try
            {
                ValidateMaxlength validRule = new ValidateMaxlength();
                validRule.required = true;
                validRule.txtEdit = txtMetrialTypeCode;
                validRule.maxLenght = 25;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtMetrialTypeCode, validRule);
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
                validRule.txtEdit = txtMetrialTypeName;
                validRule.maxLenght = 500;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtMetrialTypeName, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataToForm(HIS_MATERIAL_TYPE data)
        {
            try
            {
                data.MATERIAL_TYPE_CODE = txtMetrialTypeCode.Text.Trim();
                data.MATERIAL_TYPE_NAME = txtMetrialTypeName.Text.Trim();
                data.HIS_SERVICE = new HIS_SERVICE();
                if (cboServiceUnit.EditValue != null)
                {
                    data.TDL_SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceUnit.EditValue.ToString());
                    data.HIS_SERVICE.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceUnit.EditValue.ToString());
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
                HIS_MATERIAL_TYPE UpdateDTO = new HIS_MATERIAL_TYPE();
                UpdateDTOFromDataToForm(UpdateDTO);

                var Result = new BackendAdapter(param).Post<HIS_MATERIAL_TYPE>(HisRequestUriStore.CreateParent, ApiConsumers.MosConsumer, UpdateDTO, param);
                if (Result != null)
                {
                    BackendDataWorker.Reset<HIS_MATERIAL_TYPE>();
                    RefeshDataAfterSave(Result);
                    success = true;
                    RestFromData();
                    this.Close();
                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);

            }
        }
        void RefeshDataAfterSave(HIS_MATERIAL_TYPE data)
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
        #endregion
        #region ---PreviewKeyDown
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

                LogSystem.Error(ex);
            }
        }

        private void txtMetrialTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMetrialTypeName.Focus();
                    txtMetrialTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtMetrialTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceUnitCode.Focus();
                    txtServiceUnitCode.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---EvenCombo
        private void cboServiceUnit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboServiceUnit.EditValue != null)
                    {
                        var serviceUnit = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceUnit.EditValue ?? "").ToString()));
                        if (serviceUnit != null)
                        {
                            txtServiceUnitCode.Text = serviceUnit.SERVICE_UNIT_CODE;
                            cboServiceUnit.Properties.Buttons[1].Visible = true;

                            btnAdd.Focus();

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
        #endregion
        #region --- Click

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

        private void bbtnRestFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtMetrialTypeCode.Focus();
                txtMetrialTypeCode.SelectAll();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
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
                txtMetrialTypeCode.Focus();
                txtMetrialTypeCode.SelectAll();
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

        private void bbtnRest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

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
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using EMR.Desktop.Plugins.DocumentEdit.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EMR.Filter;
using DevExpress.XtraEditors.Controls;
using EMR.SDO;

namespace EMR.Desktop.Plugins.DocumentEdit
{
    public partial class frmDocumentEdit : FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        private HIS.Desktop.Common.RefeshReference refeshData;
        long EmrDocumentId = 0;
        EMR_DOCUMENT currentEmrDocument { get; set; }
        List<EMR_DOCUMENT_TYPE> listEmrDocumentTypes;
        List<EMR_DOCUMENT_GROUP> listEmrDocumentGroups;

        int positionHandle = -1;

        public frmDocumentEdit()
        {
            InitializeComponent();
        }

        public frmDocumentEdit(Inventec.Desktop.Common.Modules.Module module, long _emrDocumentId, HIS.Desktop.Common.RefeshReference _refeshData)
            : base(module)
        {

            InitializeComponent();
            currentModule = module;
            this.EmrDocumentId = _emrDocumentId;
            this.refeshData = _refeshData;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                if (currentModule != null)
                {
                    this.Text = currentModule.text;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmDocumentEdit_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboDocumentType();
                InitComboDocumentGroup();
                SetDefaultValueControl();
                ValidateMaxLength(txtDocumentName, 500);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                if (EmrDocumentId > 0)
                {
                    CommonParam param = new CommonParam();
                    EmrDocumentFilter filter = new EmrDocumentFilter();
                    filter.ID = EmrDocumentId;
                    var emrDocument = new BackendAdapter(param)
                            .Get<List<EMR_DOCUMENT>>("api/EmrDocument/Get", ApiConsumers.EmrConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (emrDocument != null)
                    {
                        currentEmrDocument = emrDocument.FirstOrDefault();
                        txtDocumentName.Text = currentEmrDocument.DOCUMENT_NAME;
                        cboDocumentType.EditValue = currentEmrDocument.DOCUMENT_TYPE_ID;
                        cboDocumentGroup.EditValue = currentEmrDocument.DOCUMENT_GROUP_ID;
                        dtDocumentTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentEmrDocument.DOCUMENT_TIME ?? 0) ?? DateTime.Now;
                    }
                }
                else
                {
                    currentEmrDocument = null;
                    txtDocumentName.Text = null;
                    cboDocumentType.EditValue = null;
                    cboDocumentGroup.EditValue = null;
                    dtDocumentTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitComboDocumentType()
        {
            try
            {
                listEmrDocumentTypes = BackendDataWorker.Get<EMR_DOCUMENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.EMR_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDocumentType, listEmrDocumentTypes, controlEditorADO);
                cboDocumentType.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void InitComboDocumentGroup()
        {
            try
            {
                listEmrDocumentGroups = BackendDataWorker.Get<EMR_DOCUMENT_GROUP>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.EMR_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDocumentGroup, listEmrDocumentGroups, controlEditorADO);
                cboDocumentGroup.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.EmrDocumentId > 0)
                {
                    this.positionHandle = -1;
                    if (!dxValidationProvider1.Validate())
                        return;
                    bool success = false;
                    CommonParam param = new CommonParam();
                    EmrDocumentInfoSDO emrDocumentInfoSDO = new EmrDocumentInfoSDO();
                    emrDocumentInfoSDO.DocumentId = EmrDocumentId;
                    emrDocumentInfoSDO.DocumentName = txtDocumentName.Text.Trim();
                    if (cboDocumentType.EditValue != null)
                    {
                        emrDocumentInfoSDO.DocumentTypeId = Convert.ToInt64(cboDocumentType.EditValue.ToString());
                    }
                    if (cboDocumentGroup.EditValue != null)
                    {
                        emrDocumentInfoSDO.DocumentGroupId = Convert.ToInt64(cboDocumentGroup.EditValue.ToString());
                    }
                    if (dtDocumentTime.EditValue != null && dtDocumentTime.DateTime != DateTime.MinValue)
                    {
                        emrDocumentInfoSDO.DocumentTime = Convert.ToInt64(dtDocumentTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                    }
                    var apiResult = new BackendAdapter(param).Post<EMR_DOCUMENT>("api/EmrDocument/UpdateInfoSdo", ApiConsumers.EmrConsumer, emrDocumentInfoSDO, param);

                    if (apiResult != null)
                    {
                        success = true;
                        if (refeshData != null)
                            refeshData();
                        this.Close();
                    }

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidateMaxLength(BaseControl control, int maxlength)
        {
            try
            {
                ValidationMaxLength valid = new ValidationMaxLength();
                valid.textEdit = control;
                valid.maxLength = maxlength;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void cboDocumentType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDocumentType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentGroup_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDocumentGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDocumentName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDocumentType.Focus();
                    cboDocumentType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDocumentGroup.Focus();
                    cboDocumentGroup.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDocumentTime.Focus();
                    dtDocumentTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDocumentTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

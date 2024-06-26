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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Configuration;
using System.Drawing;
using System.Resources;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;


namespace HIS.Desktop.Plugins.MaterialType.Form
{
    public partial class frmLock : HIS.Desktop.Utility.FormBase
    {
        DelegateSelectData delegateSelectData;
        HIS_MATERIAL_TYPE materialType;
        int positionHandle = -1;

        public frmLock(HIS_MATERIAL_TYPE _medicineType, DelegateSelectData _delegateSelectData)
        {
            InitializeComponent();
            this.materialType = _medicineType;
            this.delegateSelectData = _delegateSelectData;
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmLock
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                MaterialTypeList.Resources.ResourceLangManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MaterialType.Resources.Lang", typeof(frmLock).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt              
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmLock.btnSave.Text", MaterialTypeList.Resources.ResourceLangManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmLock.layoutControlItem1.Text", MaterialTypeList.Resources.ResourceLangManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmLock.bar1.Text", MaterialTypeList.Resources.ResourceLangManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmLock.bbtnSave.Caption", MaterialTypeList.Resources.ResourceLangManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmLock.Text", MaterialTypeList.Resources.ResourceLangManager.LanguageResource, LanguageManager.GetCulture());              
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
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                CommonParam param = new CommonParam();
                bool success = false;
                MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE result = null;
                WaitingManager.Show();
                materialType.LOCKING_REASON = this.txtReason.Text;
                Inventec.Common.Adapter.BackendAdapter adapter = new Inventec.Common.Adapter.BackendAdapter(param);
                result = adapter.Post<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>("api/HisMaterialType/Lock", ApiConsumers.MosConsumer, materialType, param);
                if (result != null)
                {
                    success = true;
                    if (this.delegateSelectData != null)
                    {
                        this.delegateSelectData(result);
                        this.Close();
                    }
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
        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmLock_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                ValidationMaxlength(txtReason, 1000);
                SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxlength(MemoEdit control, int maxLength)
        {
            try
            {
                ValidateMaxLength validRule = new ValidateMaxLength();
                validRule.maxLength = maxLength;
                validRule.memoEdit = control;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
    }
}

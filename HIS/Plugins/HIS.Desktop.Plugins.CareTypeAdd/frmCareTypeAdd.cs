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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.CareTypeAdd
{
    public partial class frmCareTypeAdd : HIS.Desktop.Utility.FormBase
    {
        int positionHandleControl = -1;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        internal HIS.Desktop.Common.DelegateReturnSuccess refreshData;

        public frmCareTypeAdd()
        {
            InitializeComponent();
        }

        public frmCareTypeAdd(Inventec.Desktop.Common.Modules.Module currentModule, HIS.Desktop.Common.DelegateReturnSuccess refreshData)
		:base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this.refreshData = refreshData;
        }

        private void frmCareTypeAdd_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                ValidControl();
                txtCareTypeCode.Focus();
                txtCareTypeCode.SelectAll();
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
                ////Khoi tao doi tuong resource
                HIS.Desktop.Plugins.CareTypeAdd.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CareTypeAdd.Resources.Lang", typeof(HIS.Desktop.Plugins.CareTypeAdd.frmCareTypeAdd).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCareTypeAdd.layoutControl1.Text", HIS.Desktop.Plugins.CareTypeAdd.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveAdd.Text = Inventec.Common.Resource.Get.Value("frmCareTypeAdd.btnSaveAdd.Text", HIS.Desktop.Plugins.CareTypeAdd.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCareTypeCode.Text = Inventec.Common.Resource.Get.Value("frmCareTypeAdd.lciCareTypeCode.Text", HIS.Desktop.Plugins.CareTypeAdd.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCareTypeName.Text = Inventec.Common.Resource.Get.Value("frmCareTypeAdd.lciCareTypeName.Text", HIS.Desktop.Plugins.CareTypeAdd.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmCareTypeAdd.bar1.Text", HIS.Desktop.Plugins.CareTypeAdd.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmCareTypeAdd.barButtonItem1.Caption", HIS.Desktop.Plugins.CareTypeAdd.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCareTypeAdd.Text", HIS.Desktop.Plugins.CareTypeAdd.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void Process()
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_CARE_TYPE careType = new MOS.EFMODEL.DataModels.HIS_CARE_TYPE();
                careType.CARE_TYPE_CODE = txtCareTypeCode.Text.Trim();
                careType.CARE_TYPE_NAME = txtCareTypeName.Text.Trim();

                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();

                var rs = new BackendAdapter(param).Post<HIS_CARE_TYPE>(HisRequestUriStore.HIS_CARE_TYPE_CREATE, ApiConsumers.MosConsumer, careType, param);
                WaitingManager.Hide();
                if (rs != null && rs.ID > 0)
                {
                    success = true;
                    this.refreshData(success);
                    this.Close();
                }
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void btnSaveAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;

                Process();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCareTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCareTypeName.Focus();
                    txtCareTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCareTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSaveAdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.ServiceGroup
{
    public partial class FormServiceGroupCreate : FormBase
    {
        List<SereServADO> ServiceCheckeds;
        private int positionHandle = -1;

        public FormServiceGroupCreate(Inventec.Desktop.Common.Modules.Module module, List<SereServADO> serviceCheckeds)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.ServiceCheckeds = serviceCheckeds;
                this.IsUseApplyFormClosingOption = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormServiceGroupCreate_Load(object sender, EventArgs e)
        {
            try
            {
                this.btnSave.Enabled = this.ServiceCheckeds != null && this.ServiceCheckeds.Count > 0;

                ValidateForm();
                SetCaptionByLanguageKey();
                txtServiceGroupCode.Focus();
                txtServiceGroupCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }




        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormServiceGroupCreate
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(FormServiceGroupCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormServiceGroupCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceGroupCode.Text = Inventec.Common.Resource.Get.Value("FormServiceGroupCreate.lciServiceGroupCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceGroupName.Text = Inventec.Common.Resource.Get.Value("FormServiceGroupCreate.lciServiceGroupName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("FormServiceGroupCreate.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPublic.Properties.Caption = Inventec.Common.Resource.Get.Value("FormServiceGroupCreate.chkPublic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPublic.Text = Inventec.Common.Resource.Get.Value("FormServiceGroupCreate.lciPublic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormServiceGroupCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormServiceGroupCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("FormServiceGroupCreate.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormServiceGroupCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




        private void ValidateForm()
        {
            try
            {
                ControlMaxLengthValidationRule ptttTempCodeValidate = new ControlMaxLengthValidationRule();
                ptttTempCodeValidate.editor = txtServiceGroupCode;
                ptttTempCodeValidate.maxLength = 6;
                ptttTempCodeValidate.IsRequired = true;
                ptttTempCodeValidate.ErrorText = "Trường dữ liệu nhập quá dài";
                ptttTempCodeValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtServiceGroupCode, ptttTempCodeValidate);

                ControlMaxLengthValidationRule ptttTempNameValidate = new ControlMaxLengthValidationRule();
                ptttTempNameValidate.editor = txtServiceGroupName;
                ptttTempNameValidate.maxLength = 100;
                ptttTempNameValidate.IsRequired = true;
                ptttTempNameValidate.ErrorText = "Trường dữ liệu nhập quá dài";
                ptttTempNameValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtServiceGroupName, ptttTempNameValidate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.btnSave.Enabled)
                    return;

                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                bool success = false;
                CommonParam param = new CommonParam();

                HIS_SERVICE_GROUP serviceGroup = new HIS_SERVICE_GROUP();
                SaveServiceGroupProcess(ref serviceGroup);
                var serviceGroupResult = new BackendAdapter(param).Post<HIS_SERVICE_GROUP>("/api/HisServiceGroup/Create", ApiConsumer.ApiConsumers.MosConsumer, serviceGroup, param);
                if (serviceGroupResult != null)
                {
                    success = true;
                    this.Close();
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveServiceGroupProcess(ref HIS_SERVICE_GROUP serviceGroup)
        {
            try
            {
                serviceGroup.SERVICE_GROUP_CODE = txtServiceGroupCode.Text.Trim();
                serviceGroup.SERVICE_GROUP_NAME = txtServiceGroupName.Text.Trim();
                serviceGroup.DESCRIPTION = mmDescription.Text.Trim();

                if (chkPublic.Checked)
                {
                    serviceGroup.IS_PUBLIC = 1;
                }
                else
                {
                    serviceGroup.IS_PUBLIC = null;
                }

                if (ServiceCheckeds != null && ServiceCheckeds.Count > 0)
                {
                    serviceGroup.HIS_SERV_SEGR = new List<HIS_SERV_SEGR>();
                    foreach (var item in ServiceCheckeds)
                    {
                        HIS_SERV_SEGR data = new HIS_SERV_SEGR();

                        data.SERVICE_ID = item.SERVICE_ID;
                        data.AMOUNT = item.AMOUNT;
                        if (item.IsExpend.HasValue && item.IsExpend.Value)
                        {
                            data.IS_EXPEND = 1;
                        }
                        data.NOTE = item.InstructionNote;

                        serviceGroup.HIS_SERV_SEGR.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceGroupName.Focus();
                    txtServiceGroupName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceGroupName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    mmDescription.Focus();
                    mmDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

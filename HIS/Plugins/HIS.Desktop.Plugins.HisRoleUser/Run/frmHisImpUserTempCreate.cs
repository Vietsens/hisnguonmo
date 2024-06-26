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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisRoleUser.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisRoleUser.Run
{
    public partial class frmHisImpUserTempCreate : HIS.Desktop.Utility.FormBase
    {
        public List<RoleUserAdo> listRoleUserAdo = new List<RoleUserAdo>();
        public RefeshReference refeshData;
        int positionHandle = -1;

        const int THUOC = 1;
        const int VATTU = 2;
        const int THUOC_DM = 3;
        const int VATTU_DM = 4;
        const int THUOC_TUTUC = 5;


        public frmHisImpUserTempCreate(List<RoleUserAdo> listRoleUserAdo, RefeshReference refeshData)
        {
            InitializeComponent();

            this.listRoleUserAdo = listRoleUserAdo;
            this.refeshData = refeshData;
        }

        private void dxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandle == -1)
                {
                    this.positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandle > edit.TabIndex)
                {
                    this.positionHandle = edit.TabIndex;
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

        private void ValidControls()
        {
            try
            {
                //this.ValidationSingleControl(this.txtImpUserTempCode, 10, true);
                this.ValidationSingleControl(this.txtImpUserTempName, 200, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, int? maxLength, [Optional] bool IsRequest)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequest;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave__ImpUserTemp_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            WaitingManager.Show();
            try
            {
                this.positionHandle = -1;
                if (!this.dxValidationProvider.Validate())
                {
                    WaitingManager.Hide();
                    return;
                }
                if (this.listRoleUserAdo == null || this.listRoleUserAdo.Count == 0)
                {
                    MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
                    WaitingManager.Hide();
                    return;
                }
                else
                {
                    HIS_IMP_USER_TEMP ImpUserTempCreate = new HIS_IMP_USER_TEMP();

                    ImpUserTempCreate.IMP_USER_TEMP_NAME = this.txtImpUserTempName.Text;
                    ImpUserTempCreate.HIS_IMP_USER_TEMP_DT = new List<HIS_IMP_USER_TEMP_DT>();
                    HIS_IMP_USER_TEMP ImpUserTempUpdate = GetImpUserTempByName(ImpUserTempCreate.IMP_USER_TEMP_NAME);
                    foreach (var item in this.listRoleUserAdo)
                    {
                        MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP_DT impUserTempDt = new MOS.EFMODEL.DataModels.HIS_IMP_USER_TEMP_DT();
                        impUserTempDt.DESCRIPTION = item.DESCRIPTION;
                        impUserTempDt.EXECUTE_ROLE_ID = item.EXECUTE_ROLE_ID;
                        impUserTempDt.LOGINNAME = item.LOGINNAME;
                        impUserTempDt.USERNAME = item.USERNAME;
                        ImpUserTempCreate.HIS_IMP_USER_TEMP_DT.Add(impUserTempDt);

                    }
                    HIS_IMP_USER_TEMP ImpUserTempResult = null;

                    if (ImpUserTempUpdate != null && ImpUserTempUpdate.ID>0)
                    {
                        WaitingManager.Hide();
                        if (MessageBox.Show("Trùng tên. Bạn có muốn lưu?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            ImpUserTempResult = new BackendAdapter(param).Post<HIS_IMP_USER_TEMP>("api/HisImpUserTemp/Create", ApiConsumers.MosConsumer, ImpUserTempCreate, ProcessLostToken, param);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        ImpUserTempResult = new BackendAdapter(param).Post<HIS_IMP_USER_TEMP>("api/HisImpUserTemp/Create", ApiConsumers.MosConsumer, ImpUserTempCreate, ProcessLostToken, param);
                    }
                   
                        WaitingManager.Hide();
                        if (ImpUserTempResult != null)
                        {
                            success = true;
                            if (this.refeshData != null)
                                this.refeshData();

                            this.Close();
                        }

                    MessageManager.Show(this, param, success);
                }

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private HIS_IMP_USER_TEMP GetImpUserTempByName(string ImpUserTempName)
        {
            HIS_IMP_USER_TEMP result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisImpUserTempFilter impUserTempFilter = new MOS.Filter.HisImpUserTempFilter();
                impUserTempFilter.KEY_WORD = ImpUserTempName;
                List<HIS_IMP_USER_TEMP> rs = new BackendAdapter(param).Get<List<HIS_IMP_USER_TEMP>>("api/HisImpUserTemp/Get", ApiConsumers.MosConsumer, impUserTempFilter, param);
                if (rs.Count > 0)
                {
                    result = rs.FirstOrDefault(o => o.IMP_USER_TEMP_NAME == ImpUserTempName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

        private void ProcessDataInputRoleUserTemplate(long ImpUserTempId)
        {
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                //////Khoi tao doi tuong resource
                //HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.ResourceLanguageManager.LanguagefrmHisImpUserTempCreate = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription.frmHisImpUserTempCreate).Assembly);

                //////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                //this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisImpUserTempCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmHisImpUserTempCreate, LanguageManager.GetCulture());
                //this.btnSave__ImpUserTemp.Text = Inventec.Common.Resource.Get.Value("frmHisImpUserTempCreate.btnSave__ImpUserTemp.Text", Resources.ResourceLanguageManager.LanguagefrmHisImpUserTempCreate, LanguageManager.GetCulture());
                //this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisImpUserTempCreate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguagefrmHisImpUserTempCreate, LanguageManager.GetCulture());
                //this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHisImpUserTempCreate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguagefrmHisImpUserTempCreate, LanguageManager.GetCulture());
                //this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisImpUserTempCreate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmHisImpUserTempCreate, LanguageManager.GetCulture());
                //this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisImpUserTempCreate.bar1.Text", Resources.ResourceLanguageManager.LanguagefrmHisImpUserTempCreate, LanguageManager.GetCulture());
                //this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmHisImpUserTempCreate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguagefrmHisImpUserTempCreate, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmHisImpUserTempCreate.Text", Resources.ResourceLanguageManager.LanguagefrmHisImpUserTempCreate, LanguageManager.GetCulture());
                //this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisImpUserTempCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguagefrmHisImpUserTempCreate, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmHisImpUserTempCreate_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                this.ValidControls();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.btnSave__ImpUserTemp_Click(null, null);
        }

        private void txtImpUserTempCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtImpUserTempName.Focus();
                    this.txtImpUserTempName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtImpUserTempName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //this.txtImpUserTempDescription.Focus();
                    //this.txtImpUserTempDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtImpUserTempDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //this.chkPublic.Properties.FullFocusRect = true;
                    //this.chkPublic.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPublic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnSave__ImpUserTemp.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

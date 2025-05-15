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
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.EmrDocument.Base;
using SDA.SDO;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.EmrDocument
{
    public partial class FrmReason : Form
    {
        bool check = false;
        HIS.Desktop.Common.DelegateReturnSuccess dataSelect;
        List<EmrDocumentADO> listDataTrue = new System.Collections.Generic.List<EmrDocumentADO>();
        int positionHandleControlLeft = -1;
        public FrmReason(HIS.Desktop.Common.DelegateReturnSuccess dataSelect_, List<EmrDocumentADO> listDataTrue_)
        {
            InitializeComponent();
            this.dataSelect = dataSelect_;
            this.listDataTrue = listDataTrue_;
        }

        private void FrmReason_Load(object sender, EventArgs e)
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                this.SetCaptionByLanguageKey();

                ValidateNull controlEdit = new ValidateNull();
                controlEdit.textEdit = txtReason;

                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationcheck.SetValidationRule(txtReason, controlEdit);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {

            try
            {
                this.positionHandleControlLeft = -1;
                if (!dxValidationcheck.Validate())
                {
                    return;
                }
                //if (string.IsNullOrEmpty(txtReason.Text) || string.IsNullOrWhiteSpace(txtReason.Text))
                //{
                //    return;
                //}
                WaitingManager.Show();
                string reason = string.Format("Chi tiết bệnh án.Lý do xuất: {0}", txtReason.Text);
                List<string> message = new List<string>();
                foreach (var item in listDataTrue)
                {
                   // var doc = BackendDataWorker.Get<EMR_DOCUMENT_TYPE>().FirstOrDefault(o => o.ID == item.DOCUMENT_TYPE_ID);
                    string mes = string.Format(" \r\nDOCUMENT_TYPE: {0}. DOCUMENT_CODE: {1}. DOCUMENT_NAME: {2}. TREATMENT_CODE: {3}. PATIENT_NAME: {4}. ", item.DOCUMENT_TYPE_NAME, item.DOCUMENT_CODE, item.DOCUMENT_NAME, item.TREATMENT_CODE, item.VIR_PATIENT_NAME);
                    message.Add(mes);
                }
                string sendmessage = reason + string.Join(" | ", message);
                string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SdaEventLogCreate eventlog = new SdaEventLogCreate();
                check = eventlog.Create(login, null, true, sendmessage);
                dataSelect(check);
                if (check == true)
                {
                    MessageManager.ShowAlert(this.ParentForm, "Thông báo", "Xử lý thành công!", 3);
                    this.Close();
                }
                else
                {
                    MessageManager.ShowAlert(this.ParentForm, "Thông báo", "Xử lý thất bại!", 3);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                dataSelect(false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void barSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FrmReason_Leave(object sender, System.EventArgs e)
        {

            try
            {
                dataSelect(check);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FrmReason_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                dataSelect(check);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationcheck_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                MemoEdit edit = e.InvalidControl as MemoEdit;
                if (edit == null)
                    return;

                MemoEditViewInfo viewInfo = edit.GetViewInfo() as MemoEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
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

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FrmReason
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource

                Resources.ResourceLanguageManager.LanguageResource__FrmReason = new ResourceManager("HIS.Desktop.Plugins.EmrDocument.Resources.Lang", typeof(FrmReason).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FrmReason.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__FrmReason, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FrmReason.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource__FrmReason, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FrmReason.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource__FrmReason, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FrmReason.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__FrmReason, LanguageManager.GetCulture());
                this.barSave.Caption = Inventec.Common.Resource.Get.Value("FrmReason.barSave.Caption", Resources.ResourceLanguageManager.LanguageResource__FrmReason, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FrmReason.Text", Resources.ResourceLanguageManager.LanguageResource__FrmReason, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

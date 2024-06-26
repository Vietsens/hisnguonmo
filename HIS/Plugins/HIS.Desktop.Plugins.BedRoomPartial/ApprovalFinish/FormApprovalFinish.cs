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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Plugins.BedRoomPartial;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.BedRoomPartial.ApprovalFinish
{
    public partial class FormApprovalFinish : Form
    {
        private RefeshData RefeshDataSave;
        private L_HIS_TREATMENT_BED_ROOM CurrentTreatment;
        private int positionHandleControl = -1;

        public FormApprovalFinish(L_HIS_TREATMENT_BED_ROOM treatment, RefeshData RefeshDataDelegate)
        {
            InitializeComponent();

            CurrentTreatment = treatment;
            RefeshDataSave = RefeshDataDelegate;
        }

        private void FormApprovalFinish_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                this.SetCaptionByLanguageKey();
                this.TxtNote.Text = CurrentTreatment.APPROVE_FINISH_NOTE;
                ValidateControlNote();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateControlNote()
        {
            try
            {
                ControlMaxLengthValidationRule rule = new ControlMaxLengthValidationRule();
                rule.editor = TxtNote;
                rule.maxLength = 500;
                rule.IsRequired = true;
                rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(TxtNote, rule);
            }
            catch (Exception ex)
            {
                    Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }
                bool success = false;
                CommonParam param = new CommonParam();
                HisTreatmentApproveFinishSDO sdo = new HisTreatmentApproveFinishSDO();
                sdo.TreatmentId = CurrentTreatment.TREATMENT_ID;
                sdo.ApproveFinishNote = TxtNote.Text;

                var _Treatment = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/ApproveFinish", ApiConsumers.MosConsumer, sdo, param);
                if (_Treatment != null)
                {
                    if (RefeshDataSave != null)
                    {
                        RefeshDataSave();
                    }

                    this.Close();
                    success = true;
                }

                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormApprovalFinish
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__FormApprovalFinish = new ResourceManager("HIS.Desktop.Plugins.BedRoomPartial.Resources.Lang", typeof(FormApprovalFinish).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormApprovalFinish.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__FormApprovalFinish, LanguageManager.GetCulture());
                this.BtnSave.Text = Inventec.Common.Resource.Get.Value("FormApprovalFinish.BtnSave.Text", Resources.ResourceLanguageManager.LanguageResource__FormApprovalFinish, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FormApprovalFinish.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource__FormApprovalFinish, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormApprovalFinish.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__FormApprovalFinish, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("FormApprovalFinish.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource__FormApprovalFinish, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormApprovalFinish.Text", Resources.ResourceLanguageManager.LanguageResource__FormApprovalFinish, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormApprovalFinish_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                positionHandleControl = 0;
                CurrentTreatment = null;
                RefeshDataSave = null;
                this.BtnSave.Click -= new System.EventHandler(this.BtnSave_Click);
                this.barBtnSave.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSave_ItemClick);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.FormApprovalFinish_Load);
                dxValidationProvider1 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barBtnSave = null;
                bar1 = null;
                barManager1 = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                TxtNote = null;
                BtnSave = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

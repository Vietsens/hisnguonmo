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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
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
using MOS.SDO;
using MOS.EFMODEL;
using MOS.Filter;
using AutoMapper;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using Inventec.UC.Login;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.HisBedRoomOut
{
    public partial class FormHisBedRoomOut : HIS.Desktop.Utility.FormBase
    {

        #region Declare

        int positionHandleControl = -1;
        long treatmentId;
        MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM _treatmentBedRoom;
        HisTreatmentBedRoomSDO _hisTreatmentBedRoomSDO;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<L_HIS_TREATMENT_BED_ROOM> _listTreatmentBedRoom = new List<L_HIS_TREATMENT_BED_ROOM>();

        #endregion

        #region Contructor

        public FormHisBedRoomOut(Inventec.Desktop.Common.Modules.Module module, L_HIS_TREATMENT_BED_ROOM treatmentBedRoom)
		:base(module)
        {
            try
            {
                InitializeComponent();
                this._treatmentBedRoom = treatmentBedRoom;
                this.treatmentId = treatmentBedRoom.TREATMENT_ID;
                //this.this.treatmentId = this.treatmentId;
                this.currentModule = module;
                ValidateForm();              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        #endregion

        #region Method

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBedRoomOut.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBedRoomOut.FormHisBedRoomOut).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomOut.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barbtnSave.Caption = Inventec.Common.Resource.Get.Value("FormHisBedRoomOut.barbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomOut.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomOut.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomOut.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomOut.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomOut.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
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

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
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
                ValidationSingleControl(dtLogTime, dxValidationProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateData(ref V_HIS_TREATMENT_BED_ROOM hisTreatmentBedRoomProcess)
        {
            try
            {
                hisTreatmentBedRoomProcess.REMOVE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                hisTreatmentBedRoomProcess.TREATMENT_ID = this.treatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        private void LoadDefautLoadForm()
        {
            try
            {
                dtLogTime.EditValue = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        #endregion

        #region Event

        private void barbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtLogTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool success = false;
            CommonParam param = new CommonParam();
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();

                

                MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM hisTreatmentBedRoomProcess = new MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT_BED_ROOM>(hisTreatmentBedRoomProcess, this._treatmentBedRoom);

                UpdateData(ref hisTreatmentBedRoomProcess);
               
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TREATMENT_BED_ROOM>(HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_TREATMENT_BEDROOM_REMOVE, ApiConsumers.MosConsumer, hisTreatmentBedRoomProcess, param);
                if (rs != null)
                {
                    success = true;
                    this.Close();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                HIS.Desktop.LibraryMessage.MessageUtil.SetParam(param, LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }
            #region Show message
            MessageManager.Show(this, param, success);
            #endregion

            #region Process has exception
            SessionManager.ProcessTokenLost(param);
            #endregion

            if (success)
            {
                BackendDataWorker.Reset<V_HIS_TREATMENT_BED_ROOM>();
                this.Close();
            }
        }

        private void HisBedRoomOut_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                SetCaptionByLanguageKey();
                LoadDefautLoadForm();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        #endregion

    }
}

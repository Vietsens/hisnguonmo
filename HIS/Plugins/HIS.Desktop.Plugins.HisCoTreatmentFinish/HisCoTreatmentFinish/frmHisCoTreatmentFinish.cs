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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.SDO;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisCoTreatmentFinish.ValidationRule;
using System.Resources;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;

namespace HIS.Desktop.Plugins.HisCoTreatmentFinish.HisCoTreatmentFinish
{
    public partial class frmHisCoTreatmentFinish : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        DelegateSelectData delegateSelect;
        Inventec.Desktop.Common.Modules.Module moduleData;
        int positionHandle = -1;
        long coTreatmentId;
        V_HIS_CO_TREATMENT coTreatment { get; set; }

        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;

        #endregion

        #region Construct
        public frmHisCoTreatmentFinish(long _coTreatmentId, Inventec.Desktop.Common.Modules.Module _moduleData, DelegateSelectData _delegateSelect)
            : base(_moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = _moduleData;
                this.delegateSelect = _delegateSelect;
                this.coTreatmentId = _coTreatmentId;
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
        #endregion

        #region Private method
        private void frmHisCoTreatmentCreate_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguage();
                ValidControlStartTime();
                dtStartTime.DateTime = DateTime.Now;
                LoadDataDefault();
                InitUcSecondaryIcd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataDefault()
        {
            try
            {
                HisCoTreatmentViewFilter filter = new HisCoTreatmentViewFilter();
                filter.ID = coTreatmentId;

                coTreatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_CO_TREATMENT>>("api/HisCoTreatment/GetView", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                if (coTreatment != null)
                {
                    HisTreatmentViewFilter ft = new HisTreatmentViewFilter();
                    ft.ID = coTreatment.TDL_TREATMENT_ID;
                    var treatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, ft, null).FirstOrDefault();
                    if (treatment != null)
                    {
                        lblMainIcd.Text = String.Format("{0} - {1}", treatment.ICD_CODE, treatment.ICD_NAME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList());
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.Width = 420;
                ado.Height = 24;
                //ado.TextLblIcd = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__LCI_ICD_TEXT");
                ado.TextLblIcd = "CĐ ĐT kết hợp:";
                ado.TootiplciIcdSubCode = "Chẩn đoán điều trị kết hợp";
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisCoTreatmentFinish.Resources.Lang", typeof(HIS.Desktop.Plugins.HisCoTreatmentFinish.HisCoTreatmentFinish.frmHisCoTreatmentFinish).Assembly);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisCoTreatmentFinish.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStartTime.Text = Inventec.Common.Resource.Get.Value("frmHisCoTreatmentFinish.lciStartTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !string.IsNullOrEmpty(moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlStartTime()
        {
            try
            {
                StartTimeValidationRule reasonRule = new StartTimeValidationRule();
                reasonRule.dtCancelTime = dtStartTime;
                dxValidationProviderEditorInfo.SetValidationRule(dtStartTime, reasonRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                WaitingManager.Show();
                HisCoTreatmentFinishSDO hisCoTreatmentSDO = new HisCoTreatmentFinishSDO();
                hisCoTreatmentSDO.Id = this.coTreatmentId;
                hisCoTreatmentSDO.RequestRoomId = this.moduleData.RoomId;
                if (dtStartTime.DateTime != DateTime.MinValue)
                {
                    hisCoTreatmentSDO.FinishTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStartTime.DateTime) ?? 0;
                }
                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        hisCoTreatmentSDO.IcdSubCode = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        hisCoTreatmentSDO.IcdText = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CO_TREATMENT>(HisRequestUriStore.MOSHIS_CO_TREATMENT_FINISH, ApiConsumers.MosConsumer, hisCoTreatmentSDO, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                }
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    if (this.delegateSelect != null)
                    {
                        this.delegateSelect(result);
                    }
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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

        private void txtDepartment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        #endregion

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        #region Public method
        #endregion

        #region Shortcut
        #endregion

    }
}

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
using System.Resources;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.ConfigApplication;

namespace HIS.Desktop.Plugins.HisCoTreatmentCreate.HisCoTreatmentCreate
{
    public partial class frmHisCoTreatmentCreate : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        DelegateSelectData delegateSelect;
        Inventec.Desktop.Common.Modules.Module moduleData;
        long treatmentId;
        int positionHandle = -1;
        HIS_CO_TREATMENT coTreatmentToPrint { get; set; }
        #endregion

        #region Construct
        public frmHisCoTreatmentCreate(long _treatmentId, Inventec.Desktop.Common.Modules.Module _moduleData, DelegateSelectData _delegateSelect)
            : base(_moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = _moduleData;
                this.delegateSelect = _delegateSelect;
                this.treatmentId = _treatmentId;
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
                InitComboDepartment();
                ValidateGridLookupWithTextEdit(this.cboDepartment, this.txtDepartment);
                ValidateMaxlengthMemoEdit(txtNote, 1000);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateMaxlengthMemoEdit(MemoEdit txt, int maxLength)
        {
            try
            {
                ValidateMaxlength validRule = new ValidateMaxlength();
                validRule.mme = txt;
                validRule.maxLength = maxLength;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDepartment()
        {
            try
            {
                if (this.moduleData != null && this.moduleData.RoomId > 0)
                {
                    var selectRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                    var departmentFilters = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID != selectRoom.DEPARTMENT_ID && o.IS_CLINICAL == 1 && o.IS_ACTIVE == 1).ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                    ControlEditorLoader.Load(this.cboDepartment, departmentFilters, controlEditorADO);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisCoTreatmentCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.HisCoTreatmentCreate.HisCoTreatmentCreate.frmHisCoTreatmentCreate).Assembly);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisCoTreatmentCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                lciDepartmentCode.Text = Inventec.Common.Resource.Get.Value("frmHisCoTreatmentCreate.lciDepartmentCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                HisCoTreatmentSDO hisCoTreatmentSDO = new HisCoTreatmentSDO();
                hisCoTreatmentSDO.DepartmentId = Inventec.Common.TypeConvert.Parse.ToInt32((cboDepartment.EditValue ?? "").ToString());
                hisCoTreatmentSDO.RequestRoomId = this.moduleData.RoomId;
                hisCoTreatmentSDO.TreatmentId = this.treatmentId;
                hisCoTreatmentSDO.CotreatmentRequest = txtNote.Text;

                coTreatmentToPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CO_TREATMENT>(HisRequestUriStore.MOSHIS_CO_TREATMENT_CREATE, ApiConsumers.MosConsumer, hisCoTreatmentSDO, param);
                WaitingManager.Hide();
                if (coTreatmentToPrint != null)
                {
                    success = true;
                }
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    if (this.delegateSelect != null)
                    {
                        this.delegateSelect(coTreatmentToPrint);
                    }
                    btnPrint.Enabled = true;

                    if (!btnPrint.Visible)
                    {
                        this.Close();
                    }
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

        void LoadDepartmentCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDepartment.EditValue = null;
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboAccountBook);
                }
                else
                {
                    var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDepartment.EditValue = data[0].ID;
                            txtDepartment.Text = data[0].DEPARTMENT_CODE;
                            btnSave.Focus();
                        }
                        else if (data.Count > 1)
                        {
                            cboDepartment.EditValue = null;
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboAccountBook);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null && cboDepartment.EditValue != cboDepartment.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEPARTMENT accountBook = BackendDataWorker.Get<HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString()));
                        if (accountBook != null)
                        {
                            txtDepartment.Text = accountBook.DEPARTMENT_CODE;
                            btnSave.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDepartmentCombo(strValue, false);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Public method
        #endregion

        #region Shortcut
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
        #endregion

        private void bbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled) return;

                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000488", delegateRunPrintTemplte);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool delegateRunPrintTemplte(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case "Mps000488":
                            InChuyenDieuTriKetHop(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InChuyenDieuTriKetHop(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                HisCoTreatmentViewFilter ft = new HisCoTreatmentViewFilter();
                ft.ID = coTreatmentToPrint.ID;
                var vCoTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_CO_TREATMENT>>("api/HisCoTreatment/GetView", ApiConsumers.MosConsumer, ft, null).First();


                MPS.Processor.Mps000488.PDO.Mps000488PDO rdo = new MPS.Processor.Mps000488.PDO.Mps000488PDO(vCoTreatment);
                MPS.ProcessorBase.Core.PrintData printdata = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { };
                }
                else
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { };
                }
                result = MPS.MpsPrinter.Run(printdata);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

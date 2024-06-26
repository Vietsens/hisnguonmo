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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TreatmentLog.Validate;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
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

namespace HIS.Desktop.Plugins.TreatmentLog.Popup.CoTreatment
{
    public partial class frmCoTreatment : HIS.Desktop.Utility.FormBase
    {
        V_HIS_CO_TREATMENT currentData;
        int positionHandle = -1;
        HIS.Desktop.Common.DelegateRefreshData delegateRefresh;
        HIS_CO_TREATMENT resultData;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        public frmCoTreatment(V_HIS_CO_TREATMENT data, HIS.Desktop.Common.DelegateRefreshData _delegateRefresh, Inventec.Desktop.Common.Modules.Module currentModule) : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentData = data;
                this.delegateRefresh = _delegateRefresh;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmCoTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.currentData.START_TIME != null)
                {
                    ValidationSingleControl(dtStartTime);
                    dtStartTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentData.START_TIME ?? 0);
                }
                else
                {
                    dtStartTime.Enabled = false;
                }

                if (this.currentData.FINISH_TIME != null)
                {
                    ValidationSingleControl(dtFinishTime);
                    dtFinishTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentData.FINISH_TIME ?? 0);
                }
                else
                {
                    dtFinishTime.Enabled = false;
                }
                ValidationNote();
                memNote.Text = currentData.COTREATMENT_REQUEST;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidationNote()
        {
            try
            {
                ValidateMaxLength valid = new ValidateMaxLength();
                valid.maxLength = 1000;
                valid.memoEdit = memNote;
                dxValidationProvider1.SetValidationRule(memNote, valid);
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
                bool success = false;
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HIS_CO_TREATMENT coTreatment = new HIS_CO_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_CO_TREATMENT>(coTreatment, this.currentData);
                if (dtStartTime.EditValue == null)
                {
                    coTreatment.START_TIME = null;
                }
                else
                {
                    coTreatment.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStartTime.DateTime);
                }

                if (dtFinishTime.EditValue != null)
                {
                    coTreatment.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinishTime.DateTime);
                }
                else
                {
                    coTreatment.FINISH_TIME = null;
                }

                if (coTreatment.FINISH_TIME != null && coTreatment.START_TIME != null && coTreatment.FINISH_TIME < coTreatment.START_TIME)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Resources.ResourceMessage.ThoiGianKetThucPhaiLonHonThoiGianBatDau), "Thông báo");
                    return;
                }
                coTreatment.COTREATMENT_REQUEST = memNote.Text.Trim();
                resultData = new BackendAdapter(param).Post<HIS_CO_TREATMENT>("api/HisCoTreatment/Update", ApiConsumer.ApiConsumers.MosConsumer, coTreatment, param);
                if (resultData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_CO_TREATMENT>(currentData, resultData);
                    success = true;
                    if (this.delegateRefresh != null)
                    {
                        this.delegateRefresh();
                    }
                    if(!layoutControlItem5.Visible)
                        this.Close();
                }
                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
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
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate("Mps000488", DelegateRunPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool DelegateRunPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (printTypeCode == "Mps000488")
                {
                    Mps000488(printTypeCode, fileName, ref result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000488(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                MPS.Processor.Mps000488.PDO.Mps000488PDO Mps000488PDO = new MPS.Processor.Mps000488.PDO.Mps000488PDO(currentData);
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000488PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000488PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnPrint.Enabled)
                {
                    btnPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
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

namespace HIS.Desktop.Plugins.AggrHospitalFees
{
    public partial class frmReCancel : Form
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_TRANSACTION transactionPrint;
        Action<bool> IsSuc;
        private int positionHandle;

        public frmReCancel(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_TRANSACTION transactionPrint,Action<bool> IsSuc)
        {
            InitializeComponent();
            SetIconFrm();
            this.currentModule = currentModule;
            this.transactionPrint = transactionPrint;
            this.IsSuc = IsSuc;
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
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate())
                    return;
                CommonParam param = new CommonParam();
                MOS.SDO.HisTransactionRequestCancelSDO sdo = new MOS.SDO.HisTransactionRequestCancelSDO();
                sdo.TransactionId = transactionPrint.ID;
                sdo.WorkingRoomId = currentModule.RoomId;
                sdo.CancelReqReason = memCancel.Text.Trim();
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/RequestCancel", ApiConsumers.MosConsumer, sdo, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                if (rs != null)
                {
                    IsSuc(true);
                    transactionPrint = rs;
                    if (transactionPrint.CANCEL_REQ_STT == IMSys.DbConfig.HIS_RS.CANCEL_REQ_STT.ID__CANCEL_REQ)
                        btnPrint.Enabled = true;
                }
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, rs != null);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintHuyHoaDon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if(btnPrint.Enabled)
                    btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmReCancel_Load(object sender, EventArgs e)
        {
            try
            {
                var executeRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(s => s.ID == currentModule.RoomId);
                lbReqRoom.Text = executeRoom.ROOM_NAME;
                lblReqDepartment.Text = executeRoom.DEPARTMENT_NAME;
                lblTransactionCode.Text = transactionPrint.TRANSACTION_CODE;
                lblTreatmentCode.Text = transactionPrint.TREATMENT_CODE;
                lblPatientName.Text = transactionPrint.TDL_PATIENT_NAME;
                lblAmount.Text = Inventec.Common.Number.Convert.NumberToString(transactionPrint.AMOUNT, ConfigApplications.NumberSeperator);
                memCancel.Text = transactionPrint.CANCEL_REQ_REASON;
                if (transactionPrint.CANCEL_REQ_STT == IMSys.DbConfig.HIS_RS.CANCEL_REQ_STT.ID__CANCEL_REQ)
                    btnPrint.Enabled = true;
                Validation.ValidateMaxLength vld = new Validation.ValidateMaxLength();
                vld.maxLength = 500;
                vld.memoEdit = memCancel;
                dxValidationProvider1.SetValidationRule(memCancel, vld);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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

        private void PrintHuyHoaDon()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate("Mps000487", this.DelegateRunPrinter);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000487":
                        InPhieuHuyHoaDon(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuHuyHoaDon(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                MPS.Processor.Mps000487.PDO.Mps000487PDO rdo = new MPS.Processor.Mps000487.PDO.Mps000487PDO(transactionPrint);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.transactionPrint.TREATMENT_CODE != null ? this.transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

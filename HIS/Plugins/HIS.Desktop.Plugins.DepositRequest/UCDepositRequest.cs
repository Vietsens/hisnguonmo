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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ListDepositRequest;
using MOS.EFMODEL.DataModels;
using HIS.UC.ListDepositRequest.ADO;
using DevExpress.XtraEditors;
using HIS.Desktop.Print;
using HIS.Desktop.LibraryMessage;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Plugins.DepositRequest.Resources;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.DepositRequest.Config;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraBars;
using HIS.Desktop.ADO;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.DepositRequest
{
    public partial class UCDepositRequest : UserControlBase
    {
        ListDepositRequestProcessor listDepositReqProcessor = null;
        UserControl ucRequestDeposit = null;
        MOS.EFMODEL.DataModels.V_HIS_DEPOSIT_REQ adepositreq;
        List<V_HIS_DEPOSIT_REQ> listDepositReq = new List<V_HIS_DEPOSIT_REQ>();
        List<V_HIS_DEPOSIT_REQ> currentlistDepositReq = new List<V_HIS_DEPOSIT_REQ>();
        V_HIS_DEPOSIT_REQ depositReq = new V_HIS_DEPOSIT_REQ();
        V_HIS_TRANSACTION transaction;
        V_HIS_DEPOSIT_REQ currentdepositReq = new V_HIS_DEPOSIT_REQ();
        ListDepositRequestInitADO listDepositRequestADO = new ListDepositRequestInitADO();
        internal MOS.EFMODEL.DataModels.V_HIS_TRANSACTION HisDeposit { get; set; }
        private V_HIS_DEPOSIT_REQ depositReqPrint { get; set; }
        int positionHandleLeft = -1;
        internal int action = -1;
        SendResultToOtherForm sendResultToOtherForm;
        bool isPrintNow = false;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentID;
        string accountBookCode;
        bool isUpdate = false;
        bool isSupplement = false;
        long roomId;
        long roomTypeId;
        int positionHandleControl = -1;
        List<HIS_CONFIG> listConfig = new List<HIS_CONFIG>();

        public UCDepositRequest(Inventec.Desktop.Common.Modules.Module module, List<V_HIS_DEPOSIT_REQ> treatmentID)
            : base(module)
        {
            InitializeComponent();
            try
            {
                CommonParam param = new CommonParam();
                positionHandleLeft = -1;
                InitListDepositReqGrid();
                this.currentModule = module;
                roomId = module.RoomId;
                roomTypeId = module.RoomTypeId;
                this.currentlistDepositReq = treatmentID;
                currentdepositReq = (V_HIS_DEPOSIT_REQ)listDepositReqProcessor.GetSelectRow(ucRequestDeposit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlEditor(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                txtDescription.Text = data.DESCRIPTION;
                txtAmount.Text = Inventec.Common.Number.Convert.NumberToString(data.AMOUNT, ConfigApplications.NumberSeperator);
                txtEditReqCode.Text = data.DEPOSIT_REQ_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCDepositRequest_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                positionHandleLeft = -1;
                cboStatus.SelectedIndex = 1;
                FillDataToGrid();
                ValidControls();

                SetCaptionByLanguageKey();

                LoadCombo();
                HisConfigCFG.LoadConfig();
                SetDefaultTransactionTime();
                SetDefaultCreateQR();
                FillDataToControlEditor(currentdepositReq);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
         
        private void SetDefaultCreateQR()
        {
            try
            {
                listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(s => s.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(s.VALUE)).ToList();
                if (listConfig.Count > 0 && listConfig != null)
                {
                    layoutbtnQRCe.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutbtnQRCe.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                if (currentdepositReq.DEPOSIT_ID != null)
                {
                    btnCreateQR.Enabled = false;
                }
                else
                    btnCreateQR.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetDefaultTransactionTime()
        {
            try
            {
                dtTransactionTime.DateTime = DateTime.Now;
                if (HisConfigCFG.IsEditTransactionTimeCFG != null && HisConfigCFG.IsEditTransactionTimeCFG.Equals("1"))
                {
                    lciTransactionTime.Enabled = true;
                }
                else
                {
                    lciTransactionTime.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadCombo()
        {
            loadAccountBook();
            loadPayForm();
            InitComboAccountBook();
            InitComboPayForm();
            LoadAccountBookCombo(accountBookCode);
            LoadDataToForm(listDepositReq[0]);
            SetDefaultAccountBookForUser();
            SetDefaultPayFormForUser();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                if (!String.IsNullOrEmpty(txtReqCode.Text))
                {
                    if (checkDigit(txtReqCode.Text))
                        FillDataToGrid();
                    else
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.MaYeuCauHoacMaDieuTriKhongHopLe));
                }
                else
                {
                    FillDataToGrid();
                }

                if (listDepositReq != null && listDepositReq.Count > 0)
                    LoadDataToForm(listDepositReq[0]);
                else
                    LoadDataToForm(null);

                SetDefaultAccountBookForUser();
                SetDefaultPayFormForUser();
                Grid_RowCellClick(currentdepositReq);
                currentdepositReq = (V_HIS_DEPOSIT_REQ)listDepositReqProcessor.GetSelectRow(ucRequestDeposit);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DepositRequest.Resources.Lang", typeof(HIS.Desktop.Plugins.DepositRequest.UCDepositRequest).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtReqCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCDepositRequest.txtReqCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCDepositRequest.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSavePrint.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.btnSavePrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("UCDepositRequest.cboPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("UCDepositRequest.cboAccountBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTransactionTime.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UCDepositRequest.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleLeft == -1)
                {
                    positionHandleLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }

                if (positionHandleLeft > edit.TabIndex)
                {
                    positionHandleLeft = edit.TabIndex;
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

        private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadAccountBookCombo(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null && cboAccountBook.EditValue != cboAccountBook.OldEditValue)
                    {
                        txtPayFormCode.Focus();
                        txtPayFormCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    loadPayFormCombo(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPayForm.EditValue != null && cboPayForm.EditValue != cboPayForm.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_PAY_FORM commune = ListPayForm.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString()));
                        if (commune != null)
                        {
                            txtPayFormCode.Text = commune.PAY_FORM_CODE;
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                    }
                }
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
                if (!btnSave.Enabled) return;

                SaveProcess(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void txtReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Grid_PrintClick(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                if (data != null)
                {
                    depositReqPrint = data;
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__YEU_CAU_TAM_UNG__MPS000091, DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSavePrint.Enabled) return;

                SaveProcess(true);
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

                isPrintNow = false;
                Grid_PrintClick(currentdepositReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        void Regcode(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F2)
                {
                    txtReqCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
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
                btnSavePrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtReqCode_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtReqCode_Click(object sender, EventArgs e)
        {
            txtReqCode.Focus();
        }

        private void dtTransactionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountBookCode.Focus();
                    txtAccountBookCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                txtAccountBookCode.Text = "";
                SpNumOrder.EditValue = null;
                SpNumOrder.Enabled = false;

                if (cboAccountBook.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK accountBook = ListAccountBook.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));
                    if (accountBook != null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("cboAccountBook_EditValueChanged");
                        txtAccountBookCode.Text = accountBook.ACCOUNT_BOOK_CODE;
                        txtTotalFromNumberOder.Text = accountBook.TOTAL + "/" + accountBook.FROM_NUM_ORDER + "/" + (int)(accountBook.CURRENT_NUM_ORDER ?? 0);

                        if (accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            SpNumOrder.Enabled = true;
                            SpNumOrder.EditValue = setDataToDicNumOrderInAccountBook(accountBook);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal setDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            decimal result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
            try
            {
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                        }

                        CommonParam param = new CommonParam();
                        MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new MOS.Filter.HisAccountBookViewFilter();
                        hisAccountBookViewFilter.ID = accountBook.ID;
                        var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(ApiConsumer.HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                        if (accountBooks != null && accountBooks.Count > 0)
                        {
                            var accountBookNew = accountBooks.FirstOrDefault();
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, accountBookNew.CURRENT_NUM_ORDER ?? 0);
                            result = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                        }
                    }
                    else
                    {
                        result = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                    }
                }
                else
                {
                    result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private HIS_CONFIG selectedConfig = new HIS_CONFIG();
        private void btnCreateQR_Click(object sender, EventArgs e)
        {
            try
            {
                if (listConfig != null)
                {
                    if (listConfig.Count > 1)
                    {
                        popupMenu1.ClearLinks();
                        foreach (var item in listConfig)
                        {
                            string key = "";
                            string value = item.KEY;
                            int index = value.IndexOf("Info");
                            if (index > 0)
                            {
                                var shotkey = value.Substring(0, index);
                                string[] parts = shotkey.Split('.');
                                if (parts.Length > 0)
                                {
                                    key = parts[parts.Length - 1]; // Lấy phần cuối cùng sau khi tách
                                }
                            }
                            else
                            {
                                key = item.KEY;
                            }


                            BarButtonItem btnOption = new BarButtonItem(null, key);
                            btnOption.ItemClick += (s, args) =>
                            {

                                selectedConfig = item;
                                List<object> listArgs = new List<object>();
                                TransReqQRADO adoqr = new TransReqQRADO();
                                adoqr.TreatmentId = currentdepositReq.TREATMENT_ID;
                                adoqr.ConfigValue = selectedConfig;
                                adoqr.TransReqId = CreateReqType.Deposit;
                                adoqr.DepositReq = currentdepositReq;
                                listArgs.Add(adoqr);
                                LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR ; KEY: " + selectedConfig.KEY);

                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                            };
                            popupMenu1.AddItem(btnOption);
                        }
                        popupMenu1.Manager = barManager1;
                        popupMenu1.ShowPopup(Control.MousePosition);
                    }
                    else
                    {
                        selectedConfig = listConfig[0];
                        List<object> listArgs = new List<object>();
                        TransReqQRADO adoqr = new TransReqQRADO();
                        adoqr.TreatmentId = currentdepositReq.TREATMENT_ID;
                        adoqr.ConfigValue = selectedConfig;
                        adoqr.TransReqId = CreateReqType.Deposit;
                        adoqr.DepositReq = currentdepositReq;
                        listArgs.Add(adoqr);
                        LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + selectedConfig.KEY);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                    }
                }

                FillDataToGrid();
               // if()
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi thuc hien thanh toan QR tam thu: " + ex);
            }
        }
    }
}

using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LibraryMessage = Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.Controls.Session;

namespace HIS.Desktop.Plugins.TransactionList
{
    public partial class frmTransactionReasonEdit : FormBase
    {
        public delegate void DelegateReloadAfterUpdate();

        private readonly V_HIS_TRANSACTION currentTransaction;
        private readonly long currentRoomId;
        private readonly DelegateReloadAfterUpdate reloadCallback;
        private List<HIS_TRANSACTION_REASON> reasonList;

        public frmTransactionReasonEdit(V_HIS_TRANSACTION transaction, long roomId, DelegateReloadAfterUpdate reload)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.StartPosition = FormStartPosition.CenterScreen;
                this.currentTransaction = transaction;
                this.currentRoomId = roomId;
                this.reloadCallback = reload;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    ApplicationStoreLocation.ApplicationStartupPath,
                    ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmTransactionReasonEdit_Load(object sender, EventArgs e)
        {
            try
            {
                LoadComboReason();
                SetDefaultValue();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboReason()
        {
            try
            {
                reasonList = BackendDataWorker.Get<HIS_TRANSACTION_REASON>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();

                List<ColumnInfo> columns = new List<ColumnInfo>();
                columns.Add(new ColumnInfo("TRANSACTION_REASON_CODE", "Mã", 100, 1));
                columns.Add(new ColumnInfo("TRANSACTION_REASON_NAME", "Tên", 250, 2));
                ControlEditorADO ado = new ControlEditorADO(
                    "TRANSACTION_REASON_NAME", "ID", columns, false, 350);
                ControlEditorLoader.Load(cboReason, reasonList, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                if (currentTransaction != null && currentTransaction.TRANSACTION_REASON_ID.HasValue)
                {
                    cboReason.EditValue = currentTransaction.TRANSACTION_REASON_ID.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                var resource = Base.ResourceLangManager.LanguageFrmTransactionList;
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                if (resource == null) return;

                this.Text = Inventec.Common.Resource.Get.Value("frmTransactionReasonEdit.Text", resource, culture);
                this.lciReason.Text = Inventec.Common.Resource.Get.Value("frmTransactionReasonEdit.lciReason.Text", resource, culture);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmTransactionReasonEdit.btnSave.Text", resource, culture);
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmTransactionReasonEdit.btnCancel.Text", resource, culture);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ValidateForm()
        {
            dxValidationProvider1.RemoveControlError(cboReason);
            if (cboReason.EditValue == null || string.IsNullOrEmpty(cboReason.EditValue.ToString()))
            {
                dxValidationProvider1.SetError(
                    cboReason,
                    LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc),
                    DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentTransaction == null) return;
                if (!ValidateForm()) return;

                bool success = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                MOS.SDO.HisTransactionUpdateInfoSDO ado = new MOS.SDO.HisTransactionUpdateInfoSDO();
                ado.TransactionId = currentTransaction.ID;
                ado.TransactionReasonId = Convert.ToInt64(cboReason.EditValue);
                ado.RequestRoomId = currentRoomId;

                var dataUpdate = new BackendAdapter(param).Post<HIS_TRANSACTION>(
                    "api/HisTransaction/UpdateInfo",
                    ApiConsumers.MosConsumer,
                    ado,
                    param);

                WaitingManager.Hide();
                success = (dataUpdate != null);
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);

                if (success)
                {
                    if (reloadCallback != null) reloadCallback();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

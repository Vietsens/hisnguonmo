/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.TreatmentAppointment.ADO;
using HIS.Desktop.Plugins.TreatmentAppointment.SelectZaloTemplate;
using HIS.Desktop.Plugins.TreatmentAppointment.SendZaloResult;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentAppointment
{
    public partial class frmTreatmentAppointment
    {
        #region Constants
        private const string CONFIG_KEY_ZALO_ENABLE = "MOS.SMS.ZALO_ENABLE";
        #endregion

        #region Fields
        private int currentZaloEnable;
        #endregion

        #region Init/Visibility
        /// <summary>
        /// Đọc config MOS.SMS.ZALO_ENABLE từ HIS_CONFIG cache + ẩn/hiện nút btnSendZalo + cột checkbox.
        /// Gọi trong Load event sau SetCaptionByLanguageKey.
        /// </summary>
        private void InitZaloSendVisibility()
        {
            try
            {
                this.currentZaloEnable = ReadZaloEnableConfig();
                bool isEnabled = this.currentZaloEnable == (int)EnumZaloEnable.OneSms
                    || this.currentZaloEnable == (int)EnumZaloEnable.FnsZns;

                this.lciBtnSendZalo.Visibility = isEnabled
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                this.gridColumnSelected.Visible = isEnabled;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private int ReadZaloEnableConfig()
        {
            try
            {
                string raw = HisConfigs.Get<string>(CONFIG_KEY_ZALO_ENABLE);
                int value;
                if (!string.IsNullOrWhiteSpace(raw) && int.TryParse(raw.Trim(), out value))
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return (int)EnumZaloEnable.Disabled;
        }
        #endregion

        #region Button Handler
        private void btnSendZalo_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedTreatments = GetSelectedTreatments();
                if (selectedTreatments == null || selectedTreatments.Count == 0)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessageLang.VuiLongChonItNhatMotBenhNhan,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (this.currentZaloEnable != (int)EnumZaloEnable.OneSms
                    && this.currentZaloEnable != (int)EnumZaloEnable.FnsZns)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessageLang.ChucNangGuiZaloChuaDuocBat,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dialog = new frmSelectZaloTemplate(selectedTreatments);
                if (dialog.ShowDialog(this) != DialogResult.OK
                    || string.IsNullOrWhiteSpace(dialog.SelectedTemplateId))
                {
                    return;
                }

                SendZaloMessages(selectedTreatments, dialog.SelectedTemplateId);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region Send Process
        private List<TreatmentAppointmentADO> GetSelectedTreatments()
        {
            try
            {
                var dataSource = gridControlTreatmentAppointment.DataSource as List<TreatmentAppointmentADO>;
                if (dataSource == null) return new List<TreatmentAppointmentADO>();
                return dataSource.Where(o => o != null && o.IsSelected).ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return new List<TreatmentAppointmentADO>();
            }
        }

        private void SendZaloMessages(List<TreatmentAppointmentADO> selected, string templateId)
        {
            CommonParam param = new CommonParam();
            try
            {
                var filter = new SendAppointmentZaloFilter
                {
                    TreatmentIds = selected.Select(o => o.ID).ToList(),
                    TemplateId = templateId
                };

                LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<SendAppointmentZaloResultADO>(
                    HisRequestUriStore.MOSHIS_HIS_TREATMENT_SEND_APPOINTMENT_ZALO,
                    ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();

                bool success = result != null && result.TotalSuccess > 0;
                ShowSendResultDialog(result);

                if (success)
                {
                    FillDataToGridControl();
                }

                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void ShowSendResultDialog(SendAppointmentZaloResultADO result)
        {
            try
            {
                if (result == null)
                {
                    XtraMessageBox.Show(
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBKQXLYCCuaFrontendThatBai),
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var dialog = new frmSendZaloResult(result))
                {
                    dialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}

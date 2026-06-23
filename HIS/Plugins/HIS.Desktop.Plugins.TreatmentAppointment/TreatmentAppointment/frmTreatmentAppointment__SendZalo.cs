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
        private bool selectAllHeaderWired;
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

                // Gắn checkbox "chọn tất cả" ở header cột tích chọn (chỉ gắn 1 lần, khi bật Zalo).
                if (isEnabled && !this.selectAllHeaderWired)
                {
                    this.selectAllHeaderWired = true;
                    this.gridViewTreatmentAppointment.CustomDrawColumnHeader += this.gridViewTreatmentAppointment_SelectAll_CustomDrawColumnHeader;
                    this.gridViewTreatmentAppointment.MouseDown += this.gridViewTreatmentAppointment_SelectAll_MouseDown;
                    this.gridViewTreatmentAppointment.CellValueChanged += this.gridViewTreatmentAppointment_Selected_CellValueChanged;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #region Select-all header checkbox
        /// <summary>Vẽ checkbox ở header cột tích chọn; trạng thái = đã tích hết tất cả dòng hay chưa.</summary>
        private void gridViewTreatmentAppointment_SelectAll_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != this.gridColumnSelected) return;

                e.Painter.DrawObject(e.Info); // vẽ nền header mặc định

                System.Windows.Forms.VisualStyles.CheckBoxState state = IsAllRowsSelected()
                    ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal
                    : System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal;

                System.Drawing.Size glyph = System.Windows.Forms.CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
                System.Drawing.Point pt = new System.Drawing.Point(
                    e.Bounds.X + (e.Bounds.Width - glyph.Width) / 2,
                    e.Bounds.Y + (e.Bounds.Height - glyph.Height) / 2);
                System.Windows.Forms.CheckBoxRenderer.DrawCheckBox(e.Graphics, pt, state);

                e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Click vào header cột tích chọn => đảo trạng thái chọn tất cả dòng trong trang.</summary>
        private void gridViewTreatmentAppointment_SelectAll_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;

                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.HitTest == DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitTest.Column && hi.Column == this.gridColumnSelected)
                {
                    ToggleSelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Khi tích/bỏ 1 dòng => vẽ lại header để checkbox header đồng bộ.</summary>
        private void gridViewTreatmentAppointment_Selected_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column == this.gridColumnSelected)
                {
                    this.gridControlTreatmentAppointment.Invalidate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private bool IsAllRowsSelected()
        {
            var ds = this.gridControlTreatmentAppointment.DataSource as List<TreatmentAppointmentADO>;
            return ds != null && ds.Count > 0 && ds.All(o => o != null && o.IsSelected);
        }

        private void ToggleSelectAll()
        {
            var ds = this.gridControlTreatmentAppointment.DataSource as List<TreatmentAppointmentADO>;
            if (ds == null || ds.Count == 0) return;

            bool newValue = !IsAllRowsSelected();
            this.gridViewTreatmentAppointment.BeginUpdate();
            foreach (var o in ds)
            {
                if (o != null) o.IsSelected = newValue;
            }
            this.gridViewTreatmentAppointment.EndUpdate();
            this.gridViewTreatmentAppointment.RefreshData();
            this.gridControlTreatmentAppointment.Invalidate();
        }
        #endregion

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

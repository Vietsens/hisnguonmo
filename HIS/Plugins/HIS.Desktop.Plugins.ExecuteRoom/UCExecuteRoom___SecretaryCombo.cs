/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * vCong42464 - Thêm trường Thư ký để tính lương khi Xử lý khám
 */
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExecuteRoom
{
    public partial class UCExecuteRoom
    {
        private const string SECRETARY_CONTROL_STATE_KEY = "cboSecretaryUserName";

        /// <summary>
        /// Load danh sách ACS_USER đang hoạt động vào combo Thư ký.
        /// Dữ liệu lấy từ BackendDataWorker (RAM cache), không gọi thêm API.
        /// </summary>
        private void LoadSecretaryCombo()
        {
            try
            {
                List<ACS_USER> listUser = BackendDataWorker.Get<ACS_USER>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 120, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 220, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 360);
                ControlEditorLoader.Load(cboSecretaryUserName, listUser, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đọc loginname thư ký đã chọn lần gần nhất (lưu cục bộ theo người dùng).
        /// Gọi sau LoadSecretaryCombo trong UCExecuteRoom_Load.
        /// </summary>
        private void InitSecretaryControlState()
        {
            try
            {
                if (this.currentControlStateRDO == null || this.currentControlStateRDO.Count == 0) return;

                var item = this.currentControlStateRDO
                    .FirstOrDefault(o => o.KEY == SECRETARY_CONTROL_STATE_KEY && o.MODULE_LINK == ModuleLinkName);
                if (item == null || string.IsNullOrEmpty(item.VALUE)) return;

                ACS_USER user = BackendDataWorker.Get<ACS_USER>()
                    .FirstOrDefault(o => o.LOGINNAME == item.VALUE && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                if (user == null) return;

                cboSecretaryUserName.EditValue = user.LOGINNAME;
                txtSecretaryLoginName.Text = user.LOGINNAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lưu loginname hiện tại vào ControlState để lần mở sau tự điền lại.
        /// </summary>
        private void SaveSecretaryControlState(string loginName)
        {
            try
            {
                var item = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                    ? this.currentControlStateRDO.FirstOrDefault(o => o.KEY == SECRETARY_CONTROL_STATE_KEY && o.MODULE_LINK == ModuleLinkName)
                    : null;

                if (item != null)
                {
                    item.VALUE = loginName ?? "";
                }
                else
                {
                    item = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    item.KEY = SECRETARY_CONTROL_STATE_KEY;
                    item.VALUE = loginName ?? "";
                    item.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(item);
                }

                if (this.controlStateWorker == null)
                    this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tên đăng nhập thư ký hiện tại (null nếu chưa chọn).
        /// </summary>
        internal string GetSecretaryLoginName()
        {
            try
            {
                if (cboSecretaryUserName == null || cboSecretaryUserName.EditValue == null) return null;
                string loginName = cboSecretaryUserName.EditValue.ToString();
                return string.IsNullOrWhiteSpace(loginName) ? null : loginName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Tên đầy đủ thư ký hiện tại, tra từ BackendDataWorker theo loginname.
        /// </summary>
        internal string GetSecretaryUserName()
        {
            try
            {
                string loginName = GetSecretaryLoginName();
                if (string.IsNullOrEmpty(loginName)) return null;
                ACS_USER user = BackendDataWorker.Get<ACS_USER>()
                    .FirstOrDefault(o => o.LOGINNAME == loginName);
                return user != null ? user.USERNAME : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private void cboSecretaryUserName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode != PopupCloseMode.Normal) return;
                if (cboSecretaryUserName.EditValue == null)
                {
                    txtSecretaryLoginName.Text = "";
                    SaveSecretaryControlState("");
                    return;
                }
                string loginName = cboSecretaryUserName.EditValue.ToString();
                ACS_USER user = BackendDataWorker.Get<ACS_USER>()
                    .FirstOrDefault(o => o.LOGINNAME == loginName);
                if (user != null)
                {
                    txtSecretaryLoginName.Text = user.LOGINNAME;
                    SaveSecretaryControlState(user.LOGINNAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSecretaryUserName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboSecretaryUserName.EditValue == null) return;
                    string loginName = cboSecretaryUserName.EditValue.ToString();
                    ACS_USER user = BackendDataWorker.Get<ACS_USER>()
                        .FirstOrDefault(o => o.LOGINNAME == loginName);
                    if (user != null)
                    {
                        txtSecretaryLoginName.Text = user.LOGINNAME;
                        SaveSecretaryControlState(user.LOGINNAME);
                    }
                }
                else if (e.KeyCode != Keys.Tab && e.KeyCode != Keys.Escape)
                {
                    cboSecretaryUserName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSecretaryUserName_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSecretaryUserName.EditValue = null;
                    txtSecretaryLoginName.Text = "";
                    SaveSecretaryControlState("");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSecretaryLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter) return;

                string code = (txtSecretaryLoginName.Text ?? "").Trim();
                if (string.IsNullOrEmpty(code))
                {
                    cboSecretaryUserName.EditValue = null;
                    SaveSecretaryControlState("");
                    return;
                }

                List<ACS_USER> matches = BackendDataWorker.Get<ACS_USER>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                && o.LOGINNAME != null
                                && o.LOGINNAME.Equals(code, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (matches.Count == 1)
                {
                    cboSecretaryUserName.EditValue = matches[0].LOGINNAME;
                    txtSecretaryLoginName.Text = matches[0].LOGINNAME;
                    SaveSecretaryControlState(matches[0].LOGINNAME);
                }
                else
                {
                    cboSecretaryUserName.EditValue = null;
                    cboSecretaryUserName.Focus();
                    cboSecretaryUserName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

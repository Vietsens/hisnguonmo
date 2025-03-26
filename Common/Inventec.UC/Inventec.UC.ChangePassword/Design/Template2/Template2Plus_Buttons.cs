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
using Inventec.Core;
using Inventec.UC.ChangePassword.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Design.Template2
{
    internal partial class Template2
    {

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool valid = true;
                bool success = false;
                try
                {
                    if (!dxValidationProvider1.Validate())
                    {
                        txtPreviousPass.Focus();
                        txtPreviousPass.SelectAll();
                        return;
                    }

                    waitLoad = new WaitDialogForm(Process.MessageUtil.GetMessage(Message.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm), Process.MessageUtil.GetMessage(Message.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm));
                    if (txtRetypePass.Text.CompareTo(txtNewPass.Text) > 0)
                    {
                        valid = false;
                        param.Messages.Add(MessageUtil.GetMessage(Message.Message.Enum.NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac));
                        waitLoad.Dispose();
                    }
                    if (valid)
                    {
                        var tokenData = TokenClient.clientTokenManager.GetTokenData();
                        if (tokenData != null) new TokenManager().SetConsunmer(tokenData.TokenCode);
                        Process.TokenManager token = new Process.TokenManager(param);
                        if (token.ChangePassword(txtPreviousPass.Text, txtNewPass.Text))
                        {
                            waitLoad.Dispose();
                            param.Messages.Add(" Lưu ý: mật khẩu này áp dụng cho việc đăng nhập vào toàn bộ các phần mềm trong hệ thống. Phần mềm sẽ tự động đăng xuất. Bạn vui lòng thực hiện đăng nhập lại bằng mật khẩu mới để tiếp tục sử dụng.");
                            success = true;
                        }
                        else
                        {
                            waitLoad.Dispose();
                            param.Messages.Add(MessageUtil.GetMessage(Message.Message.Enum.NguoiDungNhapTaiKhoanHoacMatKhauKhongChinhXacDeDangNhap));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    waitLoad.Dispose();
                }

                #region Show message
                ResultManager.ShowMessage(param, success);
                #endregion

                #region Process has exception
                if (_HasException != null) _HasException(param);
                #endregion

                if (success)
                {
                    try
                    {
                        if (_ChangeSuccess != null) _ChangeSuccess();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    txtNewPass.Text = "";
                    txtPreviousPass.Text = "";
                    txtRetypePass.Text = "";
                    txtPreviousPass.Focus();
                    txtPreviousPass.SelectAll();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

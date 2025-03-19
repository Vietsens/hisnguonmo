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
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.UC.Login.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Login.Design.Template1
{
    internal partial class Template1
    {

        internal void btnLogin_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                UCD.LoginSuccessUCD dataLoginSuccess = new UCD.LoginSuccessUCD();
                dataLoginSuccess.LOGINNAME = txtLoginName.Text.Trim();
                dataLoginSuccess.PASSWORD = txtPassword.Text.Trim();
                dataLoginSuccess.LANGUAGE = (cbbLanguage.EditValue != null ? cbbLanguage.EditValue.ToString().ToLower() : LanguageWorker.languageVi);
                try
                {
                    dataLoginSuccess.BranchId = ((cboBranch.EditValue ?? "").ToString() != "" ? (long?)(cboBranch.EditValue) : null);
                }
                catch { }

                bool valid = true;
                //valid = valid && CheckValidLogin(param);
                if (dataLoginSuccess.LOGINNAME == "")
                {
                    valid = false;
                    param.Messages.Add(MessageUtil.GetMessage(Message.Message.Enum.NguoiDungChuaNhapTaiKhoanDeDangNhap));
                    txtLoginName.Focus();
                }
                else if (dataLoginSuccess.PASSWORD == "")
                {
                    valid = false;
                    param.Messages.Add(MessageUtil.GetMessage(Message.Message.Enum.NguoiDungChuaNhapMatKhauDeDangNhap));
                    txtPassword.Focus();
                }
                else if (dataLoginSuccess.BranchId == null || dataLoginSuccess.BranchId == 0)
                {
                    valid = false;
                    param.Messages.Add(MessageUtil.GetMessage(Message.Message.Enum.NguoiDungChuaChonChiNhanh));
                    cboBranch.Focus();
                }
                if (valid)
                {
                    bool success = new TokenManager(param).Login(dataLoginSuccess.LOGINNAME, dataLoginSuccess.PASSWORD);
                    //Goi api dang nhap
                    if (success)
                    {
                        if (this._LoginInfor != null) _LoginInfor(dataLoginSuccess);
                    }
                    else
                    {
                        ResultManager.ShowMessage(param, success);
                        param.Messages.Clear();
                        param.BugCodes.Clear();
                    }
                }
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                btnConfig.Enabled = true;
                btnLogin.Enabled = true;
                LogSystem.Error(ex);
                param.Messages.Add(MessageUtil.GetMessage(Message.Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong));
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(MessageUtil.GetMessage(Message.Message.Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong));
            }
            catch (Exception ex)
            {
                btnConfig.Enabled = true;
                btnLogin.Enabled = true;
                LogSystem.Error(ex);
                param.Messages.Add(MessageUtil.GetMessage(Message.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat));
            }

            string message = Base.MessageUtil.GetMessageAlert(param);
            if (!String.IsNullOrEmpty(message))
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(message, MessageUtil.GetMessage(Message.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("Application_End. Time=" + DateTime.Now.ToString("yyyyMMddhhmmss"));
                TokenManager token = new TokenManager(); token.Logout();
                Application.Exit();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (this._BtnConfig_Click != null)
                    this._BtnConfig_Click();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

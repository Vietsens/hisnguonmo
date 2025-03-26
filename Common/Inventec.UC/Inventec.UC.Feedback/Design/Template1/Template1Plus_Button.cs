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
using Inventec.Core;
using Inventec.UC.Feedback.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Design.Template1
{
    internal partial class Template1
    {
        private void btnSend_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                SDA.EFMODEL.DataModels.SDA_FEEDBACK dataForm = new SDA.EFMODEL.DataModels.SDA_FEEDBACK();
                dataForm.AUTHOR = txtAuthor.Text;
                dataForm.CONTENT = txtContent.Text;
                dataForm.TITLE = txtTitle.Text;
                var tokenData = TokenClient.clientTokenManager.GetTokenData();
                if (tokenData != null) new TokenManager().SetConsunmer(tokenData.TokenCode);
                Inventec.UC.Feedback.Sda.SdaFeedback.Create.SdaFeedbackLogic logic = new Sda.SdaFeedback.Create.SdaFeedbackLogic(param);
                if (logic.Create(dataForm) != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thông tin phản hồi của bạn đã được gửi đến chúng tôi, cảm ơn vì sự đóng góp của bạn!", "Thông báo");
                    success = true;
                }

                #region Show message
                ResultManager.ShowMessage(param, success);
                #endregion

                #region Process has exception
                if (_HasException != null) _HasException(param);
                ////SessionManager.ProcessTokenLost(param);
                #endregion

                if (_Close != null) _Close();
            }
            catch (Exception ex)
            {
                MessageUtil.SetMessage(param, Message.Message.Enum.HeThongTBKQXLYCCuaFrontendThatBai);
                ResultManager.ShowMessage(param, null);
                if (_Close != null) _Close();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSend_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSend_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

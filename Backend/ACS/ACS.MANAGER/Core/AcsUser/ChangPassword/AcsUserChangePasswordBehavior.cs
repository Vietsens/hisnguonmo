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
using ACS.EFMODEL.DataModels;
using ACS.MANAGER.Base;
using ACS.MANAGER.Core.Check;
using ACS.SDO;
using Inventec.Core;
using System;

namespace ACS.MANAGER.Core.AcsUser.ChangePassword
{
    class AcsUserChangePasswordBehaviorEv : BeanObjectBase, IAcsUserChangePassword
    {
        AcsUserChangePasswordSDO entity;

        internal AcsUserChangePasswordBehaviorEv(CommonParam param, AcsUserChangePasswordSDO data)
            : base(param)
        {
            entity = data;
        }

        bool IAcsUserChangePassword.Run()
        {
            bool result = false;
            try
            {
                ACS_USER user = new ACS_USER();
                CommonParam commonParam = new CommonParam();
                if (AcsUserCheckVerifyValidDataForLogin.Verify(commonParam, ref user, entity.LOGIN_NAME, entity.PASSWORD__OLD))
                {
                    if (user != null)
                    {
                        string encryptPassword = new MOS.EncryptPassword.Cryptor().EncryptPassword(entity.PASSWORD__NEW, user.LOGINNAME);
                        user.PASSWORD = encryptPassword;
                        if (DAOWorker.AcsUserDAO.Update(user))
                        {
                            result = true;
                        }
                        else
                        {
                            user.PASSWORD = "";
                            Inventec.Common.Logging.LogSystem.Warn("Update du lieu user that bai. Doi mat khau that bai. Can kiem tra lai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => user), user));
                            ACS.MANAGER.Base.BugUtil.SetBugCode(param, ACS.LibraryBug.Bug.Enum.Common__CapNhatThatBai);
                        }
                    }
                    else
                    {
                        ACS.MANAGER.Base.BugUtil.SetBugCode(param, ACS.LibraryBug.Bug.Enum.Common__ThieuThongTinBatBuoc);
                        Inventec.Common.Logging.LogSystem.Warn("Kiem tra du lieu user khong hop le. Doi mat khau that bai. Can kiem tra lai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => entity), entity));
                    }
                }
                else
                {
                    ACS.MANAGER.Base.MessageUtil.SetMessage(param, LibraryMessage.Message.Enum.Core_AcsUser_MatKhauCuKhongChinhXac);
                }
                if (result) this.AddActivityLog(user);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                result = false;
            }
            return result;
        }


        private void AddActivityLog(ACS_USER user)
        {
            try
            {
                ACS_ACTIVITY_LOG log = new ACS_ACTIVITY_LOG();
                log.ACTIVITY_TIME = Inventec.Common.DateTime.Get.Now().Value;
                log.ACTIVITY_TYPE_ID = IMSys.DbConfig.ACS_RS.ACS_ACTIVITY_TYPE.ID__CHANGE_PASS;
                log.EMAIL = user.EMAIL;
                log.EXECUTE_LOGINNAME = Inventec.Token.ResourceSystem.ResourceTokenManager.GetLoginName();
                log.LOGINNAME = user.LOGINNAME;
                log.MOBILE = user.MOBILE;
                log.USERNAME = user.USERNAME;
                var token = Inventec.Token.ResourceSystem.ResourceTokenManager.GetUserData();
                log.APPLICATION_CODE = (token != null) ? token.ApplicationCode : Inventec.Token.ResourceSystem.ResourceTokenManager.GetApplicationCode();
                try
                {
                    log.IP_ADDRESS = Inventec.Token.ResourceSystem.ResourceTokenManager.GetRequestAddress();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                ActivityProcessor.ActivityLogCache.Push(log);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

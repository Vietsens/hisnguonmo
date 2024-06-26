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
using DevExpress.XtraTab;
using Inventec.Core;
using Inventec.Desktop.Controls;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Token;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace HIS.Desktop.Controls.Session
{
    public class SessionManager
    {
        const string frmLoginName = "frmLogin";
        const string frmMainName = "frmMain";
        const int timeWatingForError = 3000;
        static DevExpress.Utils.WaitDialogForm waitLoad = null;

        public static void ActionLostToken()
        {
            try
            {
                CommonParam param = new CommonParam();
                param.HasException = true;
                ProcessTokenLostBase(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void ProcessTokenLost(CommonParam param)
        {
            try
            {
                if (param != null && param.HasException)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Goi api server tra ve param co truong HasException = true____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("ProcessTokenLost fail.", ex);
            }
        }

        public static void ProcessTokenLostBase(CommonParam param)
        {
            try
            {
                //ẩn biểu tượng chờ load khi thông báo
                WaitingManager.Hide();
                MessageManager.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai));

                if (!Inventec.Desktop.Common.Token.TokenManager.Logout())
                    Inventec.Common.Logging.LogSystem.Warn("TokenManager.Logout");

                System.Diagnostics.Process.Start(Application.ExecutablePath);
                GlobalVariables.isLogouter = true;
                GlobalVariables.IsLostToken = true;

                //close this one
                Inventec.Common.Logging.LogSystem.Info("ProcessTokenLostBase");

                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("ProcessTokenLost fail.", ex);
            }
        }

        static void OpenAndFocusToLoginForm()
        {
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                Form f = Application.OpenForms[i];
                try
                {
                    if (f.Name == frmLoginName)
                    {
                        f.Show();
                        f.Activate();
                        f.Focus();

                        Type classType = f.GetType();
                        MethodInfo methodInfo = classType.GetMethod("LoadOnFocus");
                        methodInfo.Invoke(f, null);

                    }
                }
                catch
                {
                    f.Invoke(new MethodInvoker(delegate
                    {
                        f.Show();
                        f.Activate();
                        f.Focus();

                        Type classType = f.GetType();
                        MethodInfo methodInfo = classType.GetMethod("LoadOnFocus");
                        methodInfo.Invoke(f, null);
                    }));
                }
            }
        }

        static void CloseAllFormOpened()
        {
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                Form f = Application.OpenForms[i];
                if (f.Name != frmLoginName)
                {
                    try
                    {
                        f.Close();
                    }
                    catch
                    {
                        if (!f.IsDisposed) f.Invoke(new MethodInvoker(delegate() { f.Close(); }));
                    }
                }
            }
        }

        public static Form GetFormMain()
        {
            try
            {
                return GetFormByName(frmMainName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("GetFormMain fail.", ex);
            }
            return null;
        }

        public static XtraTabControl GetTabControlMain()
        {
            Form main = GetFormMain();
            var tabMain = (XtraTabControl)(Inventec.Desktop.Controls.ControlWorker.GetControlByNameInParentControl("tabControlMain", main));
            Inventec.Common.Logging.LogSystem.Debug((main != null ? main.Name : "GetFormMain is null") + "__" + (tabMain != null ? tabMain.Name : "tabControlMain is null"));
            return tabMain;
        }

        public static Control GetCurrentPage()
        {
            try
            {
                var controlInPages = GetTabControlMain().SelectedTabPage.Controls;
                if (controlInPages != null && controlInPages.Count > 0)
                    return controlInPages[0];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return null;
        }

        public static Form GetFormLogin()
        {
            try
            {
                return GetFormByName(frmLoginName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("GetFormLogin fail.", ex);
            }
            return null;
        }

        public static Form GetFormByName(string formName)
        {
            try
            {
                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                {
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form f = Application.OpenForms[i];
                        if (f.Name == formName)
                        {
                            return f;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("GetFormMain fail.", ex);
            }
            return null;
        }
    }
}

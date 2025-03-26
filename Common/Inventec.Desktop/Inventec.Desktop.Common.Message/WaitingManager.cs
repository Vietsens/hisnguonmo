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
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Common.Message
{
    public class WaitingManager
    {
        private static bool IsStop { get; set; }
        private static int pendingTime = int.Parse(ConfigurationManager.AppSettings.Get("Inventec.Desktop.Common.Message.PendingTime") ?? "2000");

        private static bool IsShow { get; set; }

        private static object IsLock = new object();

        private static SplashScreenManager CurrentSplashScreenManager;

        public static void Show()
        {
            try
            {
                if (CurrentSplashScreenManager != null)
                {
                    //Inventec.Common.Logging.LogSystem.Info("Show IsSplashFormVisible: " + CurrentSplashScreenManager.IsSplashFormVisible);
                }
                //Đếm thời gian xử lý
                //nếu nhỏ hơn 1 giây thì không hiển thị form chờ
                //nếu lớn hơn 1 giây thì sẽ hiển thị form chờ
                //TODO

                IsShow = true;
                IsStop = false;

                Thread show = new Thread(CountTime);
                try
                {
                    show.Start();
                }
                catch (Exception)
                {
                    show.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private static void CountTime()
        {
            try
            {
                Thread.Sleep(pendingTime);
                //Inventec.Common.Logging.LogSystem.Info("CountTime__" + IsStop.ToString());
                if (!IsStop && IsShow)
                {
                    ShowWaitForm(null);
                }
                else
                    CloseForm();
            }
            catch (Exception ex)
            {
                CloseForm();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static void Show(int frameCount)
        {
            try
            {
                CloseForm();
                ShowWaitForm(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        public static void Show(Form formParent)
        {
            try
            {
                IsShow = true;
                IsStop = false;

                Thread show = new Thread(CountTimeWithParent);
                try
                {
                    show.Start(formParent);
                }
                catch (Exception)
                {
                    show.Abort();
                }
            }
            catch (Exception ex)
            {
                CloseForm();
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private static void CountTimeWithParent(object obj)
        {
            try
            {
                Thread.Sleep(pendingTime);
                //Inventec.Common.Logging.LogSystem.Info("CountTime__" + IsStop.ToString());
                if (!IsStop && IsShow)
                {
                    ShowWaitForm((Form)obj);
                }
                else
                    CloseForm();
            }
            catch (Exception ex)
            {
                CloseForm();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static void ShowWaitForm(Form formParent)
        {
            try
            {
                if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                    CloseForm();

                if (CurrentSplashScreenManager == null || CurrentSplashScreenManager.Properties.ParentForm != formParent)
                {
                    lock (IsLock)
                    {
                        if (CurrentSplashScreenManager != null)
                        {
                            if (CurrentSplashScreenManager.IsSplashFormVisible)
                            {
                                CurrentSplashScreenManager.CloseWaitForm();
                            }
                            CurrentSplashScreenManager.Dispose();
                        }
                    }

                    CurrentSplashScreenManager = new SplashScreenManager(formParent, typeof(frmWaitForm), true, true, false);
                }

                lock (IsLock)
                {
                    if (CurrentSplashScreenManager.IsSplashFormVisible)
                    {
                        CurrentSplashScreenManager.CloseWaitForm();
                    }
                    CurrentSplashScreenManager.ShowWaitForm();
                    IsShow = true;
                }
            }
            catch (Exception ex)
            {
                CloseForm();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void Hide()
        {
            try
            {
                CloseForm();
                IsStop = true;
            }
            catch (Exception ex)
            {
                CloseForm();
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        public static void CloseForm()
        {
            try
            {
                if (CurrentSplashScreenManager != null && CurrentSplashScreenManager.IsSplashFormVisible)
                {
                    CurrentSplashScreenManager.CloseWaitForm();
                }
                IsShow = false;
                //Inventec.Common.Logging.LogSystem.Info("CloseForm IsSplashFormVisible: " + CurrentSplashScreenManager.IsSplashFormVisible);

                Thread check = new Thread(CheckSplashScreenManager);
                try
                {
                    check.Start();
                }
                catch (Exception)
                {
                    check.Abort();
                }
            }
            catch (Exception ex)
            {
                CloseFormByName("frmWaitForm");
                //Inventec.Common.Logging.LogSystem.Error(ex);
                //Inventec.Common.Logging.LogSystem.Debug("CloseForm__" + IsStop.ToString());
            }
        }

        private static void CheckSplashScreenManager()
        {
            try
            {
                Thread.Sleep(pendingTime * 2);
                if (CurrentSplashScreenManager != null && CurrentSplashScreenManager.IsSplashFormVisible)
                {
                    //Inventec.Common.Logging.LogSystem.Info("Check IsSplashFormVisible: " + CurrentSplashScreenManager.IsSplashFormVisible);
                    CurrentSplashScreenManager.CloseWaitForm();
                }
            }
            catch (Exception ex)
            {
                if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                    SplashScreenManager.CloseDefaultSplashScreen();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static void CloseFormByName(string formName)//"frmWaitForm"
        {
            try
            {
                foreach (Form item in Application.OpenForms)
                {
                    if (item.Name == formName)
                    {
                        item.Close();
                        //Inventec.Common.Logging.LogSystem.Debug("CloseForm__CloseFormByName__frmWaitForm");
                    }
                }
            }
            catch (Exception exx)
            {
                //Inventec.Common.Logging.LogSystem.Error(exx);
            }
        }
    }
}

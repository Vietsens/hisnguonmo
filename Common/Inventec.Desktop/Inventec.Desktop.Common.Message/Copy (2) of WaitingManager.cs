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
        public static bool IsStop { get; set; }
        static int pendingTime = int.Parse(ConfigurationManager.AppSettings.Get("Inventec.Desktop.Common.Message.PendingTime") ?? "2000");

        public static void Show()
        {
            try
            {
                //Đếm thời gian xử lý
                //nếu nhỏ hơn 1 giây thì không hiển thị form chờ
                //nếu lớn hơn 1 giây thì sẽ hiển thị form chờ
                //TODO
                IsStop = true;
                Thread time = new Thread(CountTime);
                time.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private static void CountTime(object obj)
        {
            Thread.Sleep(pendingTime);
            //Inventec.Common.Logging.LogSystem.Info("CountTime__" + IsStop.ToString());
            if (IsStop)
            {
                //Hide();
                ShowWaitForm(null);
            }
            else
                CloseForm();
        }

        public static void Show(int frameCount)
        {
            try
            {
                Hide();
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
                Hide();
                ShowWaitForm(formParent);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private static void ShowWaitForm(Form formParent)
        {
            try
            {
                if (formParent == null)
                {
                    foreach (Form item in Application.OpenForms)
                    {
                        if (item.Focused)
                        {
                            formParent = item;
                        }
                    }
                }
                if (formParent == null)
                {
                    formParent = Form.ActiveForm;
                }
                if (formParent == null)
                {
                    formParent = Application.OpenForms[Application.OpenForms.Count - 1];
                }

                if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                    CloseForm();

                SplashScreenManager.ShowForm(formParent, typeof(frmWaitForm), true, true, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void Hide()
        {
            try
            {
                IsStop = false;
                CloseForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        public static void CloseForm()
        {
            try
            {
                if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                {
                    SplashScreenManager.CloseForm(true, true);
                    //Inventec.Common.Logging.LogSystem.Info("Default__" + SplashScreenManager.Default == null ? "" : SplashScreenManager.Default.IsSplashFormVisible.ToString());
                }
                else
                {
                    SplashScreenManager.CloseForm(true, true);
                }
            }
            catch (Exception ex)
            {
                CloseFormByName("frmWaitForm");
                //Inventec.Common.Logging.LogSystem.Error(ex);
                //Inventec.Common.Logging.LogSystem.Debug("CloseForm__" + IsStop.ToString());
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

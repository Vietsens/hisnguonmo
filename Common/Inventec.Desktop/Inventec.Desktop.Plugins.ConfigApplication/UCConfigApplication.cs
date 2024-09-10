using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.ConfigApplication
{
    public partial class UCConfigApplication : UserControl
    {
        Inventec.UC.ConfigApplication.Refesh refeshData;
        public UCConfigApplication(Inventec.UC.ConfigApplication.Refesh _refeshData)
        {
            InitializeComponent();
            refeshData = _refeshData;
        }

        //void RefeshData()
        //{
        //    try
        //    {
        //        Inventec.Desktop.LocalStorage.ConfigApplication.Load.Init();
        //        var formMain = SessionManager.GetFormMain();
        //        if (formMain != null)
        //        {
        //            Type classType = formMain.GetType();
        //            MethodInfo methodInfo = classType.GetMethod("ResetAllTabpageToDefault");
        //            methodInfo.Invoke(formMain, null);
        //            //formMain.ResetAllTabpageToDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Error(ex);
        //    }
        //}

        private void UCConfigApplication_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.UC.ConfigApplication.UCConfigApplication ucConfigApplication = new Inventec.UC.ConfigApplication.UCConfigApplication(LanguageManager.GetLanguage(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), refeshData);
                ucConfigApplication.Dock = DockStyle.Fill;
                this.Controls.Add(ucConfigApplication);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}

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

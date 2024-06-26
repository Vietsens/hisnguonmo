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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReportAll
{
  internal partial class UCReportAll : DevExpress.XtraEditors.XtraUserControl
  {
    Inventec.UC.ListReports.MainListReports MainListReports;
    UserControl UserControl;    

    private void meShow()
    {
      try
      {
        Inventec.UC.ListReports.Data.InitData dataInit = new Inventec.UC.ListReports.Data.InitData
        (ApiConsumers.SarConsumer, ApiConsumers.SdaConsumer, ApiConsumers.AcsConsumer,
        Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager, ConfigApplications.NumPageSize,
        "APP.ico",
        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        MainListReports = new Inventec.UC.ListReports.MainListReports();
        UserControl = new UserControl();
        UserControl = MainListReports.Init(Inventec.UC.ListReports.MainListReports.EnumTemplate.TEMPLATE3,
          dataInit);
        UserControl.Dock = DockStyle.Fill;
        UCReportList.Controls.Add(UserControl);
        MainListReports.MeShowUC(UCReportList);
        if (!MainListReports.SetDelegateProcessHasException(UserControl, ProcessHasException))
          Inventec.Common.Logging.LogSystem.Debug("Loi setDelegateProcessHasException cho UCReportList");

        Inventec.UC.ListReports.Base.ResouceManager.ResourceLanguageManager();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Warn(ex);
      }
    }

    private void ProcessHasException(CommonParam param)
    {
      try
      {
        SessionManager.ProcessTokenLost(param);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    #region Keyboard
    public void ListRefesh()
    {
      try
      {
        MainListReports.ButtonRefreshClick(UserControl);
      }
      catch (Exception ex)
      {
        LogSystem.Warn(ex);
      }
    }

    public void ListSearch()
    {
      try
      {
        MainListReports.ButtonSearchClick(UserControl);
      }
      catch (Exception ex)
      {
        LogSystem.Warn(ex);
      }
    }
    #endregion
  }
}

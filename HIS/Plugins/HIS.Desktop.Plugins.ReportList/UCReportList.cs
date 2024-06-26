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
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReportList
{
    public partial class UCReportList : UserControl
    {
        Inventec.UC.ListReports.MainListReports MainReportList;
        UserControl ReportListControl;

        public UCReportList()
        {
            InitializeComponent();
            MeShow();
        }

        public void MeShow()
        {
            try
            {
                Inventec.UC.ListReports.Data.InitData dataInit = new Inventec.UC.ListReports.Data.InitData(ApiConsumers.SarConsumer, ApiConsumers.SdaConsumer, ApiConsumers.AcsConsumer, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager, ConfigApplications.NumPageSize, "APP.ico", LanguageManager.GetCulture());
                MainReportList = new Inventec.UC.ListReports.MainListReports();
                ReportListControl = new UserControl();
                ReportListControl = MainReportList.Init(Inventec.UC.ListReports.MainListReports.EnumTemplate.TEMPLATE2, dataInit);
                this.Controls.Add(ReportListControl);
                MainReportList.MeShowUC(ReportListControl);
                if (!MainReportList.SetDelegateProcessHasException(ReportListControl, ProcessHasException)) Inventec.Common.Logging.LogSystem.Debug("Loi setDelegateProcessHasException cho UCReportList");

                Inventec.UC.ListReports.Base.ResouceManager.ResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        public void Refesh()
        {
            try
            {
                MainReportList.ButtonRefreshClick(ReportListControl);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public void Search()
        {
            try
            {
                MainReportList.ButtonSearchClick(ReportListControl);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}

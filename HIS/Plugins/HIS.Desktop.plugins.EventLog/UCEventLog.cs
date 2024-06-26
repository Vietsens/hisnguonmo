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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Logging;
using Inventec.Core;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;

namespace HIS.Desktop.Plugins.EventLog
{
    public partial class UCEventLog : UserControl
    {
        Inventec.UC.EventLogControl.ProcessHasException processHasException;
        KeyCodeADO keyCodeADO = new KeyCodeADO();
        public UCEventLog()
        {
            InitializeComponent();
            // processHasException = _processHasException;
        }

        public UCEventLog(KeyCodeADO ado)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            // this.processHasException = exceptionApi;
            keyCodeADO = ado;
        }

        private void UCEventLog_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.UC.EventLogControl.MainEventLog ucEventLog = new Inventec.UC.EventLogControl.MainEventLog();
                Inventec.UC.EventLogControl.Data.DataInit initData = new Inventec.UC.EventLogControl.Data.DataInit(HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer, ConfigApplications.NumPageSize, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                initData.impMestCode = keyCodeADO.impMestCode;
                initData.expMestCode = keyCodeADO.expMestCode;
                initData.patientCode = keyCodeADO.patientCode;
                initData.serviceRequestCode = keyCodeADO.serviceRequestCode;
                initData.treatmentCode = keyCodeADO.treatmentCode;
                initData.bidNumber = keyCodeADO.bidNumber;
                string cfgUriElasticSearch = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.UriElasticSearchService");                              
                initData.UriElasticSearchServer = cfgUriElasticSearch;

                var uc = ucEventLog.Init(Inventec.UC.EventLogControl.MainEventLog.EnumTemplate.TEMPLATE2, initData);
                uc.Dock = DockStyle.Fill;
                this.Controls.Add(uc);
                ucEventLog.SetDelegateHasException(uc, processHasException);

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}

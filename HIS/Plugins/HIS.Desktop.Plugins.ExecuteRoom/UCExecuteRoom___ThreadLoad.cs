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
using System.Collections;
using Inventec.Desktop.Plugins.ExecuteRoom.Delegate;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.UC.TreeSereServ7V2;
using HIS.Desktop.Plugins.ExecuteRoom.Base;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using AutoMapper;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraBars;
using Inventec.Desktop.Plugins.ExecuteRoom;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.ExecuteRoom.ADO;

namespace HIS.Desktop.Plugins.ExecuteRoom
{
    public partial class UCExecuteRoom : UserControlBase
    {
        private void LoadSereServByTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServFilter filter = new HisSereServFilter();
                filter.TREATMENT_ID = this.currentHisServiceReq.TREATMENT_ID;
                SereServCurrentTreatment = new BackendAdapter(param)
                    .Get<List<ADOserserv7>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SereServCurrentTreatment), SereServCurrentTreatment));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceReqByTreatment()
        {
            try
            {
                ServiceReqCurrentTreatment = null;
                if (this.currentHisServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqLViewFilter filter = new HisServiceReqLViewFilter();
                    filter.TREATMENT_ID = this.currentHisServiceReq.TREATMENT_ID;
                    ServiceReqCurrentTreatment = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>>("api/HisServiceReq/GetLView", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadTreatment4ByServiceReq()
        {
            try
            {
                currentTreatment4 = null;
                if (this.currentHisServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                    filter.ID = this.currentHisServiceReq.TREATMENT_ID;
                    var apiResult = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumers.MosConsumer, filter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        currentTreatment4 = apiResult[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void ReloadMachineCounter()
        {
            try
            {
                LogSystem.Debug("ReloadMachineCounter. 1");
                if (this.executeRoom == null
                    || this.executeRoom.IS_EXAM == (short)1
                    || this.executeRoom.IS_SURGERY == (short)1
                    || this.executeRoom.IS_USE_KIOSK == (short)1)
                {
                    return;
                }

                if (HisConfigCFG.IsMachineWarningOption != "1" && HisConfigCFG.IsMachineWarningOption != "2")
                {
                    return;
                }
                LogSystem.Debug("ReloadMachineCounter. 2");
                HisMachineCounterFilter filter = new HisMachineCounterFilter();
                if (HisConfigCFG.PatientTypeOption == "1")
                    filter.IS_BHYT = true;

                List<HisMachineCounterSDO> sdos = await new BackendAdapter(new CommonParam()).GetAsync<List<HisMachineCounterSDO>>("api/HisMachine/GetCounter", ApiConsumers.MosConsumer, filter, null);
                LogSystem.Debug("ReloadMachineCounter. 3");
                GlobalVariables.MachineCounterSdos = sdos;
                LogSystem.Debug("ReloadMachineCounter. 5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

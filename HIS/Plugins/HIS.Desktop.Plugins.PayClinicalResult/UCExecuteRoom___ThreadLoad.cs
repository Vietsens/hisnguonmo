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
using HIS.UC.TreeSereServ7;
using HIS.Desktop.Plugins.PayClinicalResult.Base;
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

namespace HIS.Desktop.Plugins.PayClinicalResult
{
    public partial class UCExecuteRoom : HIS.Desktop.Utility.UserControlBase
    {
        private void LoadSereServByTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServFilter filter = new HisSereServFilter();
                filter.TREATMENT_ID = this.currentHisServiceReq.TREATMENT_ID;
                SereServCurrentTreatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);
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
                if (this.currentHisServiceReq != null
                        && this.currentHisServiceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.TREATMENT_ID = this.currentHisServiceReq.TREATMENT_ID;
                    ServiceReqCurrentTreatment = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDSereServ2()
        {
            try
            {
                CommonParam param = new CommonParam();
                DHisSereServ2Filter _sereServ2Filter = new DHisSereServ2Filter();
                _sereServ2Filter.TREATMENT_ID = this.currentHisServiceReq.TREATMENT_ID;
                DSereServ2s = new BackendAdapter(param).Get<List<DHisSereServ2>>("api/HisSereServ/GetDHisSereServ2", ApiConsumers.MosConsumer, _sereServ2Filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

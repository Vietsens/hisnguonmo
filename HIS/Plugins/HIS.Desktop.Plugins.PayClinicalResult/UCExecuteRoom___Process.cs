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

namespace HIS.Desktop.Plugins.PayClinicalResult
{
    public partial class UCExecuteRoom : HIS.Desktop.Utility.UserControlBase
    {
        private void CancelFinish(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                var serviceReq = new BackendAdapter(param)
                .Post<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UNFINISH, ApiConsumers.MosConsumer, serviceReqInput.ID, param);
                if (serviceReq != null)
                {
                    success = true;
                    //Reload data
                    foreach (var item in serviceReqs)
                    {
                        if (item.ID == serviceReq.ID)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(item, serviceReq);
                            break;
                        }
                    }
                    gridControlServiceReq.RefreshDataSource();
                    SetTextButtonExecute(serviceReq);
                }

                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void StartEvent(ref bool isStart, HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                //Bắt đầu
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HIS_SERVICE_REQ serviceReqResult = new BackendAdapter(param)
                .Post<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_START, ApiConsumers.MosConsumer, serviceReqInput.ID, param);
                WaitingManager.Hide();
                if (serviceReqResult == null)
                {
                    #region Show message
                    ResultManager.ShowMessage(param, null);
                    #endregion
                    return;
                }
                else
                {
                    LoadServiceReqCount();
                    foreach (var item in serviceReqs)
                    {
                        if (item.ID == currentHisServiceReq.ID)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(item, serviceReqResult);
                            break;
                        }
                    }
                    if (serviceReqInput.ID == currentHisServiceReq.ID)
                        currentHisServiceReq.SERVICE_REQ_STT_ID = serviceReqResult.SERVICE_REQ_STT_ID;
                    gridControlServiceReq.RefreshDataSource();
                    SetTextButtonExecute(currentHisServiceReq);
                }
                isStart = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}

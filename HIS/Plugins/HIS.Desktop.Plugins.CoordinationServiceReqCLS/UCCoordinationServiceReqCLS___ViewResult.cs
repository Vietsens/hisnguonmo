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
using HIS.Desktop.Plugins.CoordinationServiceReqCLS.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS
{
    public partial class UCCoordinationServiceReqCLS
    {
        /// <summary>
        /// Nút "Xem kết quả" — TÁI SỬ DỤNG module có sẵn, định tuyến theo EXE_SERVICE_MODULE_ID.
        /// Xử lý tương tự repositoryItemButtonView_ButtonClick của HIS.Desktop.Plugins.ServiceReqList.
        /// </summary>
        private void repoBtnView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HisServiceReqGetServiceReqCLSSDO srRow = gridViewServiceReq.GetFocusedRow() as HisServiceReqGetServiceReqCLSSDO;
                if (srRow == null) return;

                WaitingManager.Show();

                V_HIS_SERVICE_REQ serviceReq = GetServiceReqView(srRow.ID);
                if (serviceReq == null)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.YLenhKhongTonTai,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                HIS_SERE_SERV sereServRow = GetFirstSereServ(srRow.ID);
                if (sereServRow == null)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.ChuaCoKetQuaDeXem,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                RouteViewResult(serviceReq, sereServRow);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Định tuyến mở module xem kết quả theo loại module thực hiện (giữ nguyên logic ServiceReqList).</summary>
        private void RouteViewResult(V_HIS_SERVICE_REQ serviceReq, HIS_SERE_SERV sereServRow)
        {
            try
            {
                List<object> sendObj = new List<object>();

                if (serviceReq.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__KHAM)
                {
                    sendObj.Add(sereServRow.ID);
                    CallChildModule(ModuleLinkString.ExamServiceReqResult, sendObj);
                }
                else if (serviceReq.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__XN)
                {
                    if (serviceReq.IS_ANTIBIOTIC_RESISTANCE == 1)
                    {
                        sendObj.Add(sereServRow);
                        CallChildModule(ModuleLinkString.SereServTeinBacterium, sendObj);
                    }
                    else
                    {
                        sendObj.Add(sereServRow);
                        CallChildModule(ModuleLinkString.SereServTein, sendObj);
                    }
                }
                else if (serviceReq.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__XULYXN
                    || serviceReq.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__PHCN
                    || serviceReq.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__XULYDV
                    || (serviceReq.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__PTTT && sereServRow.IS_SENT_EXT == 1))
                {
                    sendObj.Add(sereServRow.ID);
                    CallChildModule(ModuleLinkString.ServiceReqResultView, sendObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Mở plugin con qua ModuleExt (tái sử dụng module có sẵn).</summary>
        private void CallChildModule(string moduleLinkChild, List<object> data)
        {
            try
            {
                new CallModule(moduleLinkChild, currentModule.RoomId, currentModule.RoomTypeId, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Lấy view y lệnh để biết loại module thực hiện.</summary>
        private V_HIS_SERVICE_REQ GetServiceReqView(long serviceReqId)
        {
            try
            {
                if (serviceReqId <= 0) return null;
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = serviceReqId;
                var result = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(
                    RequestUriStore.HIS_SERVICE_REQ_GETVIEW,
                    ApiConsumers.MosConsumer, filter, param);
                return result != null ? result.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>Lấy dịch vụ đầu tiên (còn hiệu lực) của y lệnh để xem kết quả.</summary>
        private HIS_SERE_SERV GetFirstSereServ(long serviceReqId)
        {
            try
            {
                if (serviceReqId <= 0) return null;
                CommonParam param = new CommonParam();
                HisSereServFilter filter = new HisSereServFilter();
                filter.SERVICE_REQ_ID = serviceReqId;
                var result = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(
                    RequestUriStore.HIS_SERE_SERV_GET,
                    ApiConsumers.MosConsumer, filter, param);
                return (result != null && result.Any()) ? result.First() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}

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
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS
{
    public partial class UCCoordinationServiceReqCLS
    {
        /// <summary>
        /// Nút "Lưu" trên từng dòng y lệnh — lưu người xem + hướng giải quyết (mục 5.4).
        /// Gọi api/HisServiceReq/UpdateCoordination { Id, SolutionDes }.
        /// </summary>
        private void repoBtnSave_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                // Commit giá trị đang nhập ở ô memo hướng giải quyết
                gridViewServiceReq.CloseEditor();
                gridViewServiceReq.UpdateCurrentRow();

                HisServiceReqGetServiceReqCLSSDO srRow = gridViewServiceReq.GetFocusedRow() as HisServiceReqGetServiceReqCLSSDO;
                if (srRow == null) return;

                UpdateCoordination(srRow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateCoordination(HisServiceReqGetServiceReqCLSSDO srRow)
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();

                HisServiceReqUpdateCoordinationSDO dto = new HisServiceReqUpdateCoordinationSDO();
                dto.Id = srRow.ID;
                dto.SolutionDes = srRow.SOLUTION_DES;

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => dto), dto));

                var result = new BackendAdapter(param).Post<bool>(
                    RequestUriStore.HIS_SERVICE_REQ_UPDATE_COORDINATION,
                    ApiConsumers.MosConsumer, dto, param);

                bool success = result;

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);

                if (success)
                {
                    // Ghi nhận người xem = tài khoản đang đăng nhập và cập nhật hiển thị lưới trái
                    srRow.VIEW_LOGINNAME = this.loginName;
                    UpdatePatientSolutionDisplay(srRow);
                    gridControlServiceReq.RefreshDataSource();
                    gridControlPatient.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(
                    "UpdateCoordination thất bại."
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => srRow), srRow),
                    ex);
            }
        }

        /// <summary>Đồng bộ hướng giải quyết hiển thị ở dòng bệnh nhân (lưới trái) sau khi lưu.</summary>
        private void UpdatePatientSolutionDisplay(HisServiceReqGetServiceReqCLSSDO srRow)
        {
            try
            {
                if (listPatient == null || srRow == null) return;
                var patient = listPatient.FirstOrDefault(o => o.TREATMENT_CODE == srRow.TREATMENT_CODE);
                if (patient != null)
                {
                    patient.SOLUTION_DES = srRow.SOLUTION_DES;
                    // Có ít nhất 1 y lệnh của điều trị có SOLUTION_DES → "Đã xử lý", ngược lại "Chưa xử lý"
                    bool hasSolution = patient.ServiceReqs != null
                        && patient.ServiceReqs.Any(o => !string.IsNullOrWhiteSpace(o.SOLUTION_DES));
                    patient.SolutionDesDisplay = hasSolution
                        ? Resources.ResourceMessage.DaXuLy
                        : Resources.ResourceMessage.ChuaXuLy;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

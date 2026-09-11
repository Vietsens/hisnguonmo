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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PaanExecuteList.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteList
{
    /// <summary>
    /// Mo man hinh Tra ket qua khi bam nut "Xu ly (Ctrl X)".
    ///
    /// NGUYEN TAC: giu nguyen hanh vi nhu man hinh cu
    /// "Xu ly yeu cau kham/cls/pttt" (HIS.Desktop.Plugins.ExecuteRoom).
    /// Khong tao man Tra ket qua rieng cho Giai phau benh.
    ///
    /// Cach lam: tra bang HIS_EXE_SERVICE_MODULE theo EXE_SERVICE_MODULE_ID
    /// cua y lenh de biet mo module nao. Day la co che DU LIEU, khong fix cung
    /// cho Giai phau benh, nen y lenh Giai phau benh se tu mo dung
    /// HIS.Desktop.Plugins.ServiceExecute nhu hien tai.
    /// Tham chieu ban goc: ExecuteRoom\UCExecuteRoom___Load.cs:2894-2937
    /// </summary>
    public partial class UCPaanExecuteList
    {
        /// <summary>Xu ly benh nhan dang chon tren luoi.</summary>
        private void ProcessSelectedRow()
        {
            try
            {
                PaanSereServADO row = GetFocusedRow();
                if (row == null)
                {
                    MessageBox.Show("Vui lòng chọn một dòng trong danh sách!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!row.SERVICE_REQ_ID.HasValue)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SERVICE_REQ_ID rong, khong mo duoc man Tra ket qua. SERE_SERV_ID = " + row.ID);
                    return;
                }

                OpenServiceExecute(row.SERVICE_REQ_ID.Value);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Lay dong dang duoc chon tren luoi.</summary>
        private PaanSereServADO GetFocusedRow()
        {
            try
            {
                if (gridViewPaan == null) return null;

                int handle = gridViewPaan.FocusedRowHandle;
                if (handle < 0) return null;

                return gridViewPaan.GetRow(handle) as PaanSereServADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Mo man hinh Tra ket qua cho y lenh chi dinh.
        /// Lam theo dung trinh tu cua man hinh cu.
        /// </summary>
        private void OpenServiceExecute(long serviceReqId)
        {
            try
            {
                WaitingManager.Show();

                // 1. Lay ban ghi y lenh day du. Lay nguyen ban ghi (khong dung
                //    ColumnParams loc bot cot) de chac chan man Tra ket qua co du
                //    thong tin no can. Chi 1 dong nen khong nang.
                V_HIS_SERVICE_REQ serviceReq = GetVServiceReqById(serviceReqId);
                if (serviceReq == null)
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Không lấy được thông tin y lệnh!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Tra bang anh xa de biet mo module nao.
                if (!serviceReq.EXE_SERVICE_MODULE_ID.HasValue)
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Dịch vụ này chưa được cấu hình màn hình xử lý!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Inventec.Common.Logging.LogSystem.Warn(
                        "EXE_SERVICE_MODULE_ID rong tai SERVICE_REQ_ID = " + serviceReqId);
                    return;
                }

                List<HIS_EXE_SERVICE_MODULE> exeServiceModules =
                    BackendDataWorker.Get<HIS_EXE_SERVICE_MODULE>();
                HIS_EXE_SERVICE_MODULE exeServiceModule =
                    (exeServiceModules != null && exeServiceModules.Count > 0)
                    ? exeServiceModules.FirstOrDefault(o => o.ID == serviceReq.EXE_SERVICE_MODULE_ID.Value)
                    : null;

                if (exeServiceModule == null)
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Không tìm thấy module!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Inventec.Common.Logging.LogSystem.Error(
                        "Khong tim thay HIS_EXE_SERVICE_MODULE co ID = " + serviceReq.EXE_SERVICE_MODULE_ID.Value);
                    return;
                }

                // 3. Tim module da nap tuong ung.
                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws
                        .Where(o => o.ModuleLink == exeServiceModule.MODULE_LINK)
                        .FirstOrDefault();

                if (moduleData == null)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(
                        "Khong tim thay moduleLink = " + exeServiceModule.MODULE_LINK);
                    MessageBox.Show("Không tìm thấy màn hình xử lý!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(
                        "Module khong phai plugin hoac thieu ExtensionInfo: " + exeServiceModule.MODULE_LINK);
                    return;
                }

                // 4. Mo man hinh Tra ket qua trong tab moi.
                long roomId = currentModule != null ? currentModule.RoomId : 0;
                long roomTypeId = currentModule != null ? currentModule.RoomTypeId : 0;

                Inventec.Desktop.Common.Modules.Module openModule =
                    PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId);

                string dob = serviceReq.TDL_PATIENT_DOB > 0
                    ? serviceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4)
                    : Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.TDL_PATIENT_DOB);

                List<object> listArgs = new List<object>();
                listArgs.Add(new ServiceExecuteADO(serviceReq, ReloadAfterExecute));

                var instance = PluginInstance.GetPluginInstance(openModule, listArgs);
                if (instance == null)
                {
                    WaitingManager.Hide();
                    throw new ArgumentNullException("instance", "Khong khoi tao duoc man hinh xu ly");
                }

                HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(
                    SessionManager.GetTabControlMain(),
                    openModule.ExtensionInfo.Code + serviceReq.SERVICE_REQ_CODE,
                    serviceReq.SERVICE_REQ_CODE + " - " + serviceReq.TDL_PATIENT_NAME
                        + " - " + dob + " - " + serviceReq.TDL_PATIENT_GENDER_NAME,
                    (UserControl)instance,
                    openModule,
                    null);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Lay mot ban ghi y lenh theo ID, dang V_HIS_SERVICE_REQ.
        /// Man Tra ket qua (ServiceExecute) nhan dung kieu nay.
        /// Khac voi GetServiceReqById trong UCPaanExecuteList___Bridge.cs -
        /// ham do tra ve L_HIS_SERVICE_REQ cho cac nut va menu chuot phai.
        /// </summary>
        private V_HIS_SERVICE_REQ GetVServiceReqById(long serviceReqId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = serviceReqId;

                var data = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(
                    PaanRequestUriStore.HIS_SERVICE_REQ_GET_VIEW,
                    ApiConsumers.MosConsumer,
                    filter,
                    param);

                return data != null ? data.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Nap lai danh sach sau khi man Tra ket qua luu xong.
        /// Truyen sang man Tra ket qua qua ServiceExecuteADO.RefreshData.
        ///
        /// Chu ky PHAI khop delegate HIS.Desktop.Common.DelegateRefresh
        /// (HIS.Desktop.Common\Delegate.cs:35), tuc nhan mot tham so
        /// V_HIS_SERVICE_REQ. Khong dung tham so nay, chi nap lai ca danh sach.
        /// </summary>
        private void ReloadAfterExecute(V_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

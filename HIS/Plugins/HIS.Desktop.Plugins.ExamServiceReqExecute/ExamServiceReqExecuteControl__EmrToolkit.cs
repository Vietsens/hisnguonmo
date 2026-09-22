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
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrToolkitImport;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Config;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    /// <summary>
    /// Mở chức năng xem kết quả cận lâm sàng do cơ sở khác chia sẻ trên cổng EMRToolkit.
    /// Việc đẩy kết quả của viện lên cổng do tiến trình nền trong MOS đảm nhiệm, không làm ở đây.
    /// </summary>
    public partial class ExamServiceReqExecuteControl
    {
        /// <summary>Module đích mở khi bấm nút</summary>
        private const string MODULE_LINK__SERE_SERV_TEIN = "HIS.Desktop.Plugins.SereServTein";

        /// <summary>
        /// Ẩn/hiện nút theo cấu hình. Gọi trong Load — viện không liên thông thì không thấy nút.
        /// </summary>
        private void InitEmrToolkitButton()
        {
            try
            {
                this.btnEmrToolkit.Visible =
                    EmrToolkitSubclinicalProcessor.IsViewEnable(EmrToolkitSubclinicalType.TEST);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnEmrToolkit_Click(object sender, EventArgs e)
        {
            try
            {
                OpenEmrToolkitProcess();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Xác định phiếu xét nghiệm của lần khám hiện tại rồi mở chức năng kết quả xét nghiệm.
        /// Nhiều phiếu thì cho người dùng chọn phiếu cần xem.
        /// </summary>
        private void OpenEmrToolkitProcess()
        {
            try
            {
                if (this.treatment == null)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongCoThongTinDieuTri,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                            HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                List<HIS_SERVICE_REQ> serviceReqs = GetTestServiceReqs();
                if (serviceReqs.Count == 0)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongCoChiDinhXetNghiem,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                            HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                HIS_SERVICE_REQ selected = serviceReqs.Count == 1 ? serviceReqs[0] : ChooseServiceReq(serviceReqs);
                if (selected == null) return;

                long sereServId = GetFirstSereServId(selected.ID);
                if (sereServId <= 0)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongCoChiDinhXetNghiem,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                            HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                OpenSereServTein(sereServId);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Các phiếu xét nghiệm của lần điều trị hiện tại, mới nhất trước</summary>
        private List<HIS_SERVICE_REQ> GetTestServiceReqs()
        {
            CommonParam param = new CommonParam();
            try
            {
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.TREATMENT_ID = this.treatment.ID;
                filter.SERVICE_REQ_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN };
                filter.ORDER_FIELD = "INTRUCTION_TIME";
                filter.ORDER_DIRECTION = "DESC";

                List<HIS_SERVICE_REQ> data = new BackendAdapter(param)
                    .Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);

                return data ?? new List<HIS_SERVICE_REQ>();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return new List<HIS_SERVICE_REQ>();
            }
        }

        /// <summary>
        /// Chọn phiếu khi lần khám có nhiều chỉ định xét nghiệm.
        /// Dùng hộp chọn đơn giản để không phải thêm một form mới cho thao tác một bước.
        /// </summary>
        private HIS_SERVICE_REQ ChooseServiceReq(List<HIS_SERVICE_REQ> serviceReqs)
        {
            try
            {
                using (frmChooseServiceReqEmrToolkit frm = new frmChooseServiceReqEmrToolkit(serviceReqs))
                {
                    return frm.ShowDialog(this) == DialogResult.OK ? frm.SelectedServiceReq : null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>Chức năng kết quả xét nghiệm nhận vào một dịch vụ, lấy dịch vụ đầu của phiếu</summary>
        private long GetFirstSereServId(long serviceReqId)
        {
            CommonParam param = new CommonParam();
            try
            {
                HisSereServFilter filter = new HisSereServFilter();
                filter.SERVICE_REQ_ID = serviceReqId;

                List<HIS_SERE_SERV> data = new BackendAdapter(param)
                    .Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);

                HIS_SERE_SERV sereServ = data != null
                    ? data.Where(o => o.IS_NO_EXECUTE != 1).OrderBy(o => o.ID).FirstOrDefault()
                    : null;
                return sereServ != null ? sereServ.ID : 0;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return 0;
            }
        }

        /// <summary>Mở plugin kết quả xét nghiệm — người dùng chuyển sang tab liên thông để tra cứu</summary>
        private void OpenSereServTein(long sereServId)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == MODULE_LINK__SERE_SERV_TEIN).FirstOrDefault();
                if (moduleData == null)
                {
                    LogSystem.Error("khong tim thay moduleLink = " + MODULE_LINK__SERE_SERV_TEIN);
                    return;
                }

                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    LogSystem.Error("Module " + MODULE_LINK__SERE_SERV_TEIN + " khong phai plugin");
                    return;
                }

                WaitingManager.Show();
                try
                {
                    long roomId = this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0;
                    long roomTypeId = this.currentModuleBase != null ? this.currentModuleBase.RoomTypeId : 0;

                    List<object> listArgs = new List<object>();
                    listArgs.Add(sereServId);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId));

                    object instance = PluginInstance.GetPluginInstance(
                        PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (instance == null) throw new ArgumentNullException("instance is null");

                    WaitingManager.Hide();

                    if (instance is Form) ((Form)instance).ShowDialog();
                }
                finally
                {
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
    }
}

using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.KskServiceEditList.ADO;
using HIS.Desktop.Plugins.KskServiceEditList.Worker;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskServiceEditList
{
    public partial class frmKskServiceEditList
    {
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.isSaving) return;
            try
            {
                this.GridViewService.PostEditor();
                this.GridViewService.UpdateCurrentRow();
                if (!this.ValidateRequired()) return;

                HisKskServiceEditSDO sdo = this.BuildSdo();
                if (!this.ValidateChanges(sdo)) return;

                if (XtraMessageBox.Show(String.Format(Resources.ResourceMessage.XacNhanApDung, this.treatments.Count),
                    this.GetTitleMessage(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                this.isSaving = true;
                this.btnSave.Enabled = false;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("KskServiceEdit.Input", new
                {
                    sdo.KskContractId,
                    TreatmentCount = this.treatments.Count,
                    sdo.DeleteServiceIds,
                    sdo.ChangeRooms,
                    sdo.AddServices
                }));

                WaitingManager.Show();
                KskServiceEditBatchResultADO result = new KskServiceEditWorker().Run(sdo, this.treatments);
                WaitingManager.Hide();

                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}",
                    "HIS.Desktop.Plugins.KskServiceEditList", "KskServiceEdit", sdo.Loginname,
                    Inventec.Common.Logging.LogUtil.TraceData("Summary", result.Summary)));

                frmKskServiceEditResult frmResult = new frmKskServiceEditResult(this.currentModule, result);
                frmResult.ShowDialog();

                if (result.Summary.SuccessTreatment > 0)
                {
                    if (this.refeshReference != null) this.refeshReference();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.isSaving = false;
                if (!this.IsDisposed) this.btnSave.Enabled = true;
            }
        }

        private bool ValidateRequired()
        {
            bool valid = true;
            try
            {
                this.dxErrorProvider1.ClearErrors();
                string required = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                if (this.cboLogin.EditValue == null)
                {
                    this.dxErrorProvider1.SetError(this.cboLogin, required, ErrorType.Warning);
                    valid = false;
                }
                if (this.dtIntructionTime.EditValue == null || this.dtIntructionTime.DateTime == DateTime.MinValue)
                {
                    this.dxErrorProvider1.SetError(this.dtIntructionTime, required, ErrorType.Warning);
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                valid = false;
            }
            return valid;
        }

        /// <summary>
        /// Chuyển trạng thái lưới thành thao tác (giống UpdataDataForProcess của AssignServiceEdit):
        /// - Bỏ tick dịch vụ đang có -> Xóa
        /// - Tick dịch vụ chưa có / tick hẳn dịch vụ một phần BN có -> Thêm (BN đã có được bỏ qua)
        /// - Đổi "Phòng thực hiện" của dịch vụ tất cả BN đang có -> Đổi phòng
        /// </summary>
        private HisKskServiceEditSDO BuildSdo()
        {
            HisKskServiceEditSDO sdo = new HisKskServiceEditSDO();
            sdo.KskContractId = this.kskContract.ID;
            sdo.RequestRoomId = this.currentModule.RoomId;
            sdo.Loginname = this.cboLogin.EditValue.ToString();
            ACS_USER user = this.listAcsUser != null ? this.listAcsUser.FirstOrDefault(o => o.LOGINNAME == sdo.Loginname) : null;
            sdo.Username = user != null ? user.USERNAME : "";
            sdo.IntructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtIntructionTime.DateTime.ToString("yyyyMMddHHmm") + "00");

            sdo.DeleteServiceIds = this.allServiceRows
                .Where(o => o.IsExisting && o.IsChecked == false)
                .Select(o => o.SERVICE_ID).ToList();
            sdo.ChangeRooms = this.allServiceRows
                .Where(o => o.IsExisting && !o.IsPartial && o.IsChecked == true && o.IsRoomChanged && o.RoomId.HasValue)
                .Select(o => new KskServiceChangeRoomSDO { ServiceId = o.SERVICE_ID, NewRoomId = o.RoomId.Value }).ToList();
            sdo.AddServices = this.allServiceRows
                .Where(o => o.IsChecked == true && (!o.IsExisting || o.IsPartial) && o.KSK_ID.HasValue)
                .Select(o => new KskServiceAddSDO { KskId = o.KSK_ID.Value, ServiceId = o.SERVICE_ID, RoomId = o.RoomId }).ToList();
            return sdo;
        }

        private bool ValidateChanges(HisKskServiceEditSDO sdo)
        {
            // Dịch vụ tick thêm nhưng không thuộc nhóm dịch vụ KSK của hợp đồng -> không có giá/nhóm để chỉ định
            List<string> notInKsk = this.allServiceRows
                .Where(o => o.IsChecked == true && (!o.IsExisting || o.IsPartial) && !o.KSK_ID.HasValue)
                .Select(o => o.SERVICE_NAME).ToList();
            if (notInKsk.Any())
            {
                XtraMessageBox.Show(String.Format(Resources.ResourceMessage.DichVuKhongThuocNhomKsk, String.Join(", ", notInKsk)), this.GetTitleMessage());
                return false;
            }

            if (!sdo.DeleteServiceIds.Any() && !sdo.ChangeRooms.Any() && !sdo.AddServices.Any())
            {
                XtraMessageBox.Show(Resources.ResourceMessage.KhongCoThayDoi, this.GetTitleMessage());
                return false;
            }

            // Hợp đồng hết hiệu lực -> không thêm dịch vụ / đổi phòng (giống Import)
            bool hasAddOrMove = sdo.AddServices.Any() || sdo.ChangeRooms.Any();
            long now = Inventec.Common.DateTime.Get.Now() ?? 0;
            if (hasAddOrMove && this.kskContract.EXPIRY_DATE.HasValue && (this.kskContract.EXPIRY_DATE.Value + 235959) < now)
            {
                XtraMessageBox.Show(Resources.ResourceMessage.HopDongHetHieuLuc, this.GetTitleMessage());
                return false;
            }
            return true;
        }
    }
}

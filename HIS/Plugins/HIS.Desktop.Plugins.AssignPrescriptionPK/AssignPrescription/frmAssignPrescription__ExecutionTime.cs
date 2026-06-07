/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tính năng "TG thực hiện" (UsedTime) cho từng thuốc/vật tư khi kê đơn.
 * Dùng control dteUsedTime ĐÃ CÓ SẴN trên form (DateEdit, format dd/MM/yyyy HH:mm).
 * - Khi Bổ sung: lấy giá trị dteUsedTime -> gán vào dòng (ExecutionTime), sau đó CLEAR ô để nhập lần mới.
 * - Grid danh sách: hiển thị cột "TG thực hiện" (dd/MM/yyyy HH:mm).
 * - Double-click dòng: fill TG thực hiện lên ô dteUsedTime, cho sửa lại.
 * - Khi lưu: map ExecutionTime -> UsedTime của PresMedicineSDO/PresMaterialSDO.
 * - Validate: TG thực hiện không được nhỏ hơn thời gian chỉ định / ngày dự trù -> icon vàng + tooltip.
 */
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using Inventec.Desktop.Common.Message;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription
    {
        /// <summary>
        /// TG thực hiện đang nhập ở ô dteUsedTime (yyyyMMddHHmmss). Null = chưa chọn.
        /// </summary>
        public long? CurrentExecutionTime
        {
            get
            {
                try
                {
                    if (this.dteUsedTime != null && this.dteUsedTime.EditValue != null
                        && this.dteUsedTime.DateTime != DateTime.MinValue)
                    {
                        return Inventec.Common.TypeConvert.Parse.ToInt64(this.dteUsedTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return null;
            }
        }

        /// <summary>
        /// Thêm cột "TG thực hiện" vào grid danh sách thuốc/vật tư (gọi 1 lần khi load form).
        /// Không tạo thêm control nhập — ô nhập dùng dteUsedTime có sẵn trên form.
        /// </summary>
        internal void InitExecutionTimeControl()
        {
            try
            {
                // Nút Delete trên ô TG thực hiện -> xóa giá trị
                if (this.dteUsedTime != null)
                {
                    this.dteUsedTime.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.dteUsedTime_ButtonClick);
                    this.dteUsedTime.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.dteUsedTime_ButtonClick);
                }

                if (this.gridViewServiceProcess == null) return;
                if (this.gridViewServiceProcess.Columns["grcExecutionTime__TabMedicine"] != null) return;

                DevExpress.XtraGrid.Columns.GridColumn col = this.gridViewServiceProcess.Columns.AddField("ExecutionTimeDisplay");
                col.Name = "grcExecutionTime__TabMedicine";
                col.Caption = "TG thực hiện";
                col.OptionsColumn.AllowEdit = false;
                col.OptionsColumn.ReadOnly = true;
                col.Width = 115;
                col.Visible = true;
                col.VisibleIndex = this.gridViewServiceProcess.Columns.Count; // đặt ở cuối
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Click nút Delete trên ô TG thực hiện -> xóa giá trị.
        /// </summary>
        private void dteUsedTime_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.dteUsedTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Fill TG thực hiện của 1 dòng lên ô dteUsedTime (double-click sửa).
        /// </summary>
        internal void FillExecutionTimeToDetail(MediMatyTypeADO ado)
        {
            try
            {
                if (this.dteUsedTime == null) return;
                if (ado != null && ado.ExecutionTime.HasValue && ado.ExecutionTime.Value > 0)
                {
                    DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ado.ExecutionTime.Value);
                    this.dteUsedTime.EditValue = dt.HasValue ? (object)dt.Value : null;
                }
                else
                {
                    this.dteUsedTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Clear ô TG thực hiện sau khi Bổ sung xong, sẵn sàng cho lần nhập mới.
        /// </summary>
        internal void ResetExecutionTimeDetail()
        {
            try
            {
                if (this.dteUsedTime != null)
                    this.dteUsedTime.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lấy thời gian chỉ định để đối chiếu TG thực hiện:
        /// ưu tiên của từng dòng (IntructionTimeSelecteds/IntructionTime),
        /// nếu dòng chưa có thì fallback về thời gian chỉ định của đợt kê (form).
        /// </summary>
        private long GetInstructionTimeRefForCheck(MediMatyTypeADO item)
        {
            long insRef = 0;
            try
            {
                // 2) Thời gian chỉ định của đợt kê (UC date - form level)
                if (insRef <= 0 && this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0)
                    insRef = this.intructionTimeSelecteds.Where(o => o > 0).DefaultIfEmpty(0).Min();
                if (insRef <= 0 && this.InstructionTime > 0)
                    insRef = this.InstructionTime;
                // 3) Thời gian chỉ định theo từng thuốc ở vùng chi tiết (TG chỉ định - per medi)
                if (insRef <= 0)
                {
                    var mediList = this.UcDateGetValueForMedi();
                    if (mediList != null && mediList.Count > 0)
                        insRef = mediList.Where(o => o > 0).DefaultIfEmpty(0).Min();
                }
                if (insRef <= 0 && this.intructionTimeSelectedsForMedi != null && this.intructionTimeSelectedsForMedi.Count > 0)
                    insRef = this.intructionTimeSelectedsForMedi.Where(o => o > 0).DefaultIfEmpty(0).Min();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return insRef;
        }

        /// <summary>
        /// Validate TG thực hiện của từng dòng: không được nhỏ hơn thời gian chỉ định / ngày dự trù.
        /// Set ErrorType.Warning + message để hiển thị icon vàng + tooltip trên cột.
        /// </summary>
        internal void ValidateExecutionTime(MediMatyTypeADO item)
        {
            try
            {
                if (item == null) return;
                item.ErrorMessageExecutionTime = "";
                item.ErrorTypeExecutionTime = ErrorType.None;

                if (!(item.ExecutionTime.HasValue && item.ExecutionTime.Value > 0))
                    return;

                long insRef = GetInstructionTimeRefForCheck(item);
                long duTruRef = this.UseTime; // ngày dự trù (yyyyMMdd000000)

                bool invalid = (insRef > 0 && item.ExecutionTime.Value < insRef)
                            || (duTruRef > 0 && item.ExecutionTime.Value < duTruRef);
                if (invalid)
                {
                    item.ErrorMessageExecutionTime = ExecutionTimeWarningMessage();
                    item.ErrorTypeExecutionTime = ErrorType.Warning;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra TG thực hiện ngay khi Bổ sung (dựa trên ô dteUsedTime + thời gian chỉ định/dự trù hiện tại).
        /// True = hợp lệ (cho thêm dòng); False = vi phạm (chặn thêm).
        /// </summary>
        internal bool IsExecutionTimeValidAtAdd()
        {
            try
            {
                long? exec = this.CurrentExecutionTime;
                if (!exec.HasValue || exec.Value <= 0)
                    return true; // không nhập TG thực hiện thì không chặn

                long insRef = GetInstructionTimeRefForCheck(this.currentMedicineTypeADOForEdit);
                long duTruRef = this.UseTime;

                if ((insRef > 0 && exec.Value < insRef) || (duTruRef > 0 && exec.Value < duTruRef))
                    return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
        }

        private string ExecutionTimeWarningMessage()
        {
            try
            {
                string msg = ResourceMessage.ThoiGianThucHienKhongDuocNhoHonThoiGianChiDinhDuTru;
                if (!string.IsNullOrEmpty(msg))
                    return msg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "Thời gian thực hiện không được nhỏ hơn thời gian chỉ định/ dự trù";
        }
    }
}

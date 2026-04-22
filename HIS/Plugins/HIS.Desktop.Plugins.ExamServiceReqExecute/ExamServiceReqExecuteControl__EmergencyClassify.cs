/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * PTTK_19083: Phân loại cấp cứu 1 — chỉ xử lý cho combo cboEmergencyClassifyId1
 * (đã khai báo trong Designer của ExamServiceReqExecuteControl).
 *
 * - Bind data: HIS_PATIENT_CLASSIFY lọc IS_EMERGENCY=1, IS_ACTIVE=1
 * - Visibility: config MOS.HIS_TREATMENT.EMERGENCY_CLASSIFY=1 AND phòng cấp cứu
 * - Load value: từ HIS_TREATMENT.EMERGENCY_CLASSIFY_ID_1 khi mở hồ sơ cũ
 * - Get value: cho ProcessExamServiceReqDTO map vào SDO
 *
 * Phân loại cấp cứu 2 (Nhập viện / Kết thúc điều trị) nằm trong UCHospitalize &
 * UCExamTreatmentFinish — các UC tự quản lý, plugin KHÔNG đụng.
 */
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private List<HIS_PATIENT_CLASSIFY> lstEmergencyClassify;
        private bool isEmergencyRoom = false;

        /// <summary>
        /// Bind danh mục phân loại cấp cứu vào cboEmergencyClassifyId1.
        /// Nguồn: HIS_PATIENT_CLASSIFY (cache RAM, fallback API) — lọc IS_EMERGENCY=1, IS_ACTIVE=1.
        /// </summary>
        private async Task InitComboEmergencyClassify()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<HIS_PATIENT_CLASSIFY>())
                {
                    lstEmergencyClassify = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    HisPatientClassifyFilter filter = new HisPatientClassifyFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    lstEmergencyClassify = await new BackendAdapter(paramCommon).GetAsync<List<HIS_PATIENT_CLASSIFY>>(
                        "api/HisPatientClassify/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (lstEmergencyClassify != null)
                        BackendDataWorker.UpdateToRam(typeof(HIS_PATIENT_CLASSIFY), lstEmergencyClassify, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                var emergencyList = (lstEmergencyClassify ?? new List<HIS_PATIENT_CLASSIFY>())
                    .Where(o => o.IS_EMERGENCY == 1
                                && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_CLASSIFY_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_CLASSIFY_NAME", "Tên", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_CLASSIFY_NAME", "ID", columnInfos, false, 300);

                if (this.cboEmergencyClassifyId1 != null)
                    ControlEditorLoader.Load(this.cboEmergencyClassifyId1, emergencyList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ẩn/hiện layout item chứa cboEmergencyClassifyId1 theo:
        /// (config EMERGENCY_CLASSIFY bật) AND (phòng hiện tại IS_EMERGENCY=1).
        /// Tìm LayoutControlItem bằng GetItemByControl — không phụ thuộc tên item.
        /// </summary>
        private void UpdateEmergencyClassifyVisibility()
        {
            try
            {
                if (this.cboEmergencyClassifyId1 == null) return;

                bool show = false;
                if (HisConfigCFG.IsEnableEmergencyClassify && this.moduleData != null)
                {
                    var execRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>()
                        .FirstOrDefault(o => o.ROOM_ID == this.moduleData.RoomId);
                    show = execRoom != null && execRoom.IS_EMERGENCY == 1;
                }
                isEmergencyRoom = show;

                var layoutItem = this.layoutControl3 != null
                    ? this.layoutControl3.GetItemByControl(this.cboEmergencyClassifyId1) as LayoutControlItem
                    : null;
                if (layoutItem != null)
                    layoutItem.Visibility = show ? LayoutVisibility.Always : LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load giá trị đã lưu trong HIS_TREATMENT.EMERGENCY_CLASSIFY_ID_1 vào combo (khi mở hồ sơ cũ).
        /// </summary>
        private void LoadEmergencyClassifyValues()
        {
            try
            {
                if (this.treatment == null || this.cboEmergencyClassifyId1 == null) return;
                this.cboEmergencyClassifyId1.EditValue = this.treatment.EMERGENCY_CLASSIFY_ID_1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Helper: phòng hiện tại có phải phòng cấp cứu không (config bật + V_HIS_EXECUTE_ROOM.IS_EMERGENCY=1).
        /// Dùng truyền flag cho UC.Hospitalize + UC.ExamTreatmentFinish qua InitADO.
        /// </summary>
        internal bool IsEmergencyClassifyRoom()
        {
            return isEmergencyRoom && HisConfigCFG.IsEnableEmergencyClassify;
        }

        /// <summary>
        /// Lấy ID phân loại cấp cứu 1 cho ProcessExamServiceReqDTO.
        /// Trả null nếu combo không hiển thị (phòng không cấp cứu / config tắt) hoặc BS chưa chọn.
        /// </summary>
        private long? GetEmergencyClassifyId1()
        {
            try
            {
                if (!isEmergencyRoom || !HisConfigCFG.IsEnableEmergencyClassify) return null;
                if (this.cboEmergencyClassifyId1 == null || this.cboEmergencyClassifyId1.EditValue == null) return null;
                return Inventec.Common.TypeConvert.Parse.ToInt64(this.cboEmergencyClassifyId1.EditValue.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}

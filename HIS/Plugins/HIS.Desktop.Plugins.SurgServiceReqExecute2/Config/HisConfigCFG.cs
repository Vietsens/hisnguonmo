/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Việc 45072 — Bổ sung các config:
 *  - AutoDeleteEmrDocumentWhenEditReq: cho phép xóa văn bản EMR đã ký khi hủy kết thúc
 *  - IsHasConnectionEmr: hệ thống có kết nối EMR
 *  - TakeIntrucionTimeByServiceReq: chế độ lấy thời gian bắt đầu khi click row
 *  - AllowFinishWhenAccountIsDoctor: chỉ cho kết thúc khi tài khoản là bác sĩ
 */
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2.Config
{
    class HisConfigCFG
    {
        public static bool IsUsingExecuteRoomPayment
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.EPAYMENT.IS_USING_EXECUTE_ROOM_PAYMENT") == "1";
            }
        }
        public static bool IsNotRequiredPtttExecuteRole
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.IsNotRequiredPtttExecuteRole") != "1";
            }
        }
        public static string ProcessTimeMustBeLessThanMaxTotalProcessTime
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ProcessTimeMustBeLessThanMaxTotalProcessTime");
            }
        }
        internal static long PatientTypeId__BHYT
        {
            get
            {
                long result = -1;
                try
                {
                    string code = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                    if (!String.IsNullOrEmpty(code))
                        result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code).ID;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                return result;
            }
        }
        internal static string StartTimeMustBeGreaterThanInstructionTime
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.StartTimeMustBeGreaterThanInstructionTime");
            }
        }

        /// <summary>
        /// Việc 45072 — "1" = tự động xóa văn bản EMR ký số khi hủy kết thúc y lệnh.
        /// </summary>
        public static string AutoDeleteEmrDocumentWhenEditReq
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AutoDeleteEmrDocumentWhenEditReq");
            }
        }

        /// <summary>
        /// Việc 45072 — true nếu hệ thống có kết nối EMR (đọc từ config HIS.Desktop.IsHasConnectionEmr = "1").
        /// </summary>
        public static bool IsHasConnectionEmr
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.IsHasConnectionEmr") == "1";
            }
        }

        /// <summary>
        /// Việc 45072 — Cơ chế lấy thời gian bắt đầu khi click row (1/2/3 — xem mô tả trong _Extended).
        /// </summary>
        public static string TakeIntrucionTimeByServiceReq
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.TakeIntrucionTimeByServiceReq");
            }
        }

        /// <summary>
        /// Việc 45072 — "1" = chỉ cho phép kết thúc y lệnh khi tài khoản đăng nhập là bác sĩ.
        /// </summary>
        public static string AllowFinishWhenAccountIsDoctor
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR");
            }
        }
    }
}

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

        public static string AutoDeleteEmrDocumentWhenEditReq
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AutoDeleteEmrDocumentWhenEditReq");
            }
        }

        public static bool IsHasConnectionEmr
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.IsHasConnectionEmr") == "1";
            }
        }

        public static string TakeIntrucionTimeByServiceReq
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.TakeIntrucionTimeByServiceReq");
            }
        }

        public static string AllowFinishWhenAccountIsDoctor
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR");
            }
        }

        /// <summary>
        /// Bắt buộc nhập phương pháp Vô cảm.
        /// 1 = bắt buộc với Phẫu thuật (chặn); 2 = bắt buộc với Phẫu thuật (chặn) + cảnh báo Có/Không với Thủ thuật; khác = không kiểm tra.
        /// </summary>
        public static string RequiredEmotionlessMethodOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.RequiredEmotionlessMethodOption");
            }
        }

        /// <summary>
        /// Cách load danh sách Máy thực hiện.
        /// 1 = theo HIS_SERVICE_MACHINE (IS_ACTIVE=1, SERVICE_ID = dịch vụ đang xử lý);
        /// 2 = theo HIS_MACHINE (ID phòng đang mở nằm trong ROOM_IDS); khác = giống 2.
        /// </summary>
        public static string HisMachineShowOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.HisMachine_ShowOption");
            }
        }
    }
}

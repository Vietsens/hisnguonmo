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
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.HisExpMestTemplate
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__AssignPrescription__SplitOffset = "HIS.Desktop.Plugins.AssignPrescription.SplitOffset";
        internal const string TUTORIAL_FORMAT = "HIS.Desktop.Plugins.AssignPrescription.TutorialFormat";
        private const string CONFIG_KEY__PRESCRIPTION_ATC_CODE_OVERLAP_WARNING_OPTION = "HIS.DESKTOP.PRESCRIPTION.ATC_CODE_OVERLAP.WARNING_OPTION";
        private const string CONFIG_KEY__AmountDecimalNumber = "HIS.Desktop.Plugins.AssignPrescriptionPK.AmountDecimalNumber";
        private const string CONFIG_KEY__HIS_Desktop_Plugins_AssignPrescription_IsNotAutoGenerateTutorial = "HIS.Desktop.Plugins.AssignPrescription.IsNotAutoGenerateTutorial";
        internal const string TUTORIAL_NUMBER_IS_FRAC = "HIS.Desktop.Plugins.AssignPrescription.TutorialNumberIsFrac";

        /// <summary>
        /// Cấu hình tách phần bù thuốc/vật tư (phần để làm tròn trong trường hợp kê lẻ)
        /// Khi cấu hình hệ thống trên có giá trị = 1 thì với các trường hợp có nghiệp vụ xử lý làm tròn số lượng thuốc/vật tư (vd: người dùng kê số lượng lẻ, nhưng thuốc/vật tư cấu hình không cho phép kê lẻ) thì thực hiện xử lý:
        ///- Trong trường hợp kê đơn mới, thì gửi thông tin kê thành 2 dòng:
        ///+ 1 dòng là số lượng người dùng kê
        ///+ 1 dòng là số lượng phần bù, và có đánh dấu là IsNotPres = true
        /// </summary>
        internal static string SplitOffset;

        /// <summary>
        /// Cấu hình định dạng hướng dẫn sử dụng ở màn hình kê đơn
        /// - 1: Ngày <đường dùng> <số lượng tổng số 1 ngày> <đơn vị> / số lần : thời điểm trong ngày : số lượng <Dạng dùng>
        /// Ví dụ: ngày uống 4 viên/ 4 lần: sáng 1, trưa 1, chiều 1, tối 1 sau ăn
        /// - 2: Ngày <đường dùng> số lần : thời điểm trong ngày : số lượng <Dạng dùng>
        /// Ví dụ: ngày uống 4 lần: sáng 1, trưa 1, chiều 1, tối 1 sau ăn
        /// (Không nhập sáng trưa chiều tối sẽ không ra hướng dẫn sử dụng)
        /// - 3:
        /// + Đối với màn hình kê đơn không phải YHCT: <Số lượng> <đơn vị>/lần * <Số lần>/ngày
        /// Ví dụ: Thuốc A được kê là Ngày uống 2 viên chia 2 lần, sáng 1 viên, chiều 1 viên thì trên xml thể hiện: 1 viên/lần * 2 lần/ngày
        /// + Đối với màn hình kê đơn YHCT: <số lượng> <đơn vị> * 1 thang * <số ngày>
        /// Ví dụ: Thuốc thang A: 12g/thang, uống 5 thang: Trên xml thể hiện: 12g*1 thang*5 ngày
        /// - 4: < Đường dùng> <Tổng số ngày> ngày. Ngày <đường dùng> <số lượng tổng số 1 ngày> <đơn vị> / số lần : thời điểm trong ngày : số lượng <Dạng dùng>
        /// ví dụ : Uống 4 ngày. Ngày uống 4 lần sáng 1, trưa 1, chiều 1, tối 1 sau ăn
        /// </summary>
        internal static long TutorialFormat;

        internal static string AtcCodeOverlarWarningOption;
        public static int AmountDecimalNumber { get; set; }

        /// <summary>
        /// 1: Không tự động sinh trường HDSD khi kê đơn
        /// </summary>
        internal static bool IsNotAutoGenerateTutorial;

        internal static bool IsTutorialNumberIsFrac;
        
        internal static void LoadConfig()
        {
            try
            {
                AtcCodeOverlarWarningOption = GetValue(CONFIG_KEY__PRESCRIPTION_ATC_CODE_OVERLAP_WARNING_OPTION);

                SplitOffset = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__AssignPrescription__SplitOffset);
             
                TutorialFormat = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(TUTORIAL_FORMAT));

                AmountDecimalNumber = Inventec.Common.TypeConvert.Parse.ToInt32(GetValue(CONFIG_KEY__AmountDecimalNumber));

                IsNotAutoGenerateTutorial = GetValue(CONFIG_KEY__HIS_Desktop_Plugins_AssignPrescription_IsNotAutoGenerateTutorial) == GlobalVariables.CommonStringTrue;

                IsTutorialNumberIsFrac = GetValue(TUTORIAL_NUMBER_IS_FRAC) == GlobalVariables.CommonStringTrue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisExecuteRoom.RoomConfigOption
{
    internal static class RoomConfigOption
    {

        internal class RoomOptionItem
        {
            internal RoomOptionItem(Option option)
            {
                this.Option = option;
                this.Code = option.ToString();
                this.Name = option.Description();
                this.ToolTip = option.ToolTipOption();
            }

            public string Code { get; set; }
            public string Name { get; set; }
            public string ToolTip { get; set; }
            public Option Option { get; set; }
        }
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        internal sealed class ToolTipOptionAttribute : Attribute
        {
            public string ToolTip { get; }
            public ToolTipOptionAttribute(string toolTip)
            {
                ToolTip = toolTip;
            }
        }
        internal enum Option
        {
            [Description("Phòng cấp cứu")]
            IsEmergency,                // 
            [Description("Là phòng khám")]
            IsExam,                     // Là phòng khám
            [Description("Mặc định hao phí khi khám thêm")]
            IsAutoExpendAddExam,        // Mặc định hao phí khi khám thêm
            [Description("Phòng chuyên khoa")]
            IsSpeciality,               // Phòng chuyên khoa
            [Description("Không nhập ICD")]
            IsAllowNoICD,               // Không nhập ICD
            [Description("Là phòng Kiosk")]
            IsUseKiosk,                 // Là phòng kios
            [Description("Tạm dừng")]
            IsPause,                    // Tạm dừng
            [ToolTipOption("Nếu bạn sử dụng tính năng này thì bắt buộc phải thực hiện Thiết lập phòng yêu cầu")]
            [Description("Giới hạn chỉ định phòng thực hiện")]
            IsRestrictExecuteRoom,      // Giới hạn chỉ định phòng thực hiện
            [ToolTipOption("Nếu bạn sử dụng tính năng này thì bắt buộc phải thực hiện cấu hình thuốc được phép")]
            [Description("Giới hạn sử dụng thuốc")]
            IsRestrictMedicineType,     // Giới hạn sử dụng thuốc
            [ToolTipOption("Nếu bạn sử dụng tính năng này thì bắt buộc phải thực hiện cấu hình thời gian hoạt")]
            [Description("Giới hạn thời gian hoạt động")]
            IsRestrictTime,             // Giới hạn thời gian hoạt động
            [ToolTipOption("Tạm ngừng không cho phép người dùng chỉ định dịch vụ tới phòng này")]
            [Description("Tạm dừng chỉ định")]
            IsPauseEnclitic,            // Tạm dừng chỉ định
            [Description("Là phòng tiêm chủng")]
            IsVaccine,                  // Là phòng khám tiêm chủng
            [Description("Phòng uống Vitamin A")]
            IsVitaminA,                 // Phòng uống Vitamin A
            [ToolTipOption("Nếu sử dụng tính năng này bắt buộc phải thiết lập dịch vụ - phòng")]
            [Description("Giới hạn yêu cầu, thực hiện dịch vụ")]
            IsRestrictReqService,       // Giới hạn yêu cầu, thực hiện dịch vụ. Tooltip: Nếu sử dụng tính năng này bắt buộc phải thiết lập dịch vụ - phòng.
            [ToolTipOption("Giới hạn đối tượng bệnh nhân")]
            [Description("Nếu sử dụng tính năng này bắt buộc phải thiết lập Phòng xử lý - đối tượng bệnh nhân")]
            IsRestrictPatientType,      // Giới hạn đối tượng bệnh nhân. Tooltip: Nếu sử dụng tính năng này bắt buộc phải thiết lập Phòng xử lý - đối tượng bệnh nhân
            [Description("Không cần chọn dịch vụ")]
            AllowNotChooseService,      // Không cần chọn dịch vụ.
            [ToolTipOption("Chỉ chọn được khi đã chọn <b>Là phòng khám<b>")]
            [Description("Cấp số thứ tự theo khung giờ khám")]
            IsBlockNumOrder,            // Cấp số thứ tự theo khung giờ khám
            [Description("Là phòng mổ")]
            IsSurgery,                  // Là phòng mổ.
            [Description("Phải duyệt mổ")]
            [ToolTipOption("Chỉ chọn được khi đã chọn <b>Là phòng mổ<b>")]
            MustBeApprovedSurgery       // Phải duyệt mổ. chỉ enable nếu chọn "Là phòng mổ". Lưu thông tin vào MUST_BE_APPROVED_SURGERY
        }
        private static string Description(this Enum value)
        {
            if (value == null) return null;
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();

        }
        private static string ToolTipOption(this Option value)
        {
            // Remove null check for value, since Option is a non-nullable enum
            FieldInfo fi = value.GetType().GetField(value.ToString());
            ToolTipOptionAttribute[] attributes =
                (ToolTipOptionAttribute[])fi.GetCustomAttributes(
                typeof(ToolTipOptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].ToolTip;
            else
                return null;
        }
        public static short? Any(this List<RoomOptionItem> list, Option option)
        {
            if (list == null || list.Count == 0) return null;
            return list.Any(x => x.Option == option) ? (short)1 : (short?)null;
        }
        public static void Add(this List<RoomOptionItem> list, RoomConfigOption.Option option, short? value)
        {
            if (value.HasValue && value.Value == 1)
            {
                if (list == null)
                {
                    list = new List<RoomOptionItem>();
                }
                list.Add(new RoomOptionItem(option));
            }
        }
    }
}

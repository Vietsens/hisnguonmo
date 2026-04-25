using System;
using System.Resources;

namespace EMR.Desktop.Plugins.EmrExamCategory.Resource
{
    internal class ResourceMessage
    {
        static readonly ResourceManager languageMessage =
            new ResourceManager(
                "EMR.Desktop.Plugins.EmrExamCategory.Resource.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        private static string GetMessage(string key, string fallback)
        {
            try
            {
                var value = Inventec.Common.Resource.Get.Value(
                    key,
                    languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                return !string.IsNullOrEmpty(value) ? value : fallback;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return fallback;
        }

        /// <summary>Mã loại xét nghiệm không được để trống</summary>
        internal static string CategoryCodeRequired
        {
            get { return GetMessage("CategoryCodeRequired", "Mã loại xét nghiệm không được để trống"); }
        }

        /// <summary>Tên loại xét nghiệm không được để trống</summary>
        internal static string CategoryNameRequired
        {
            get { return GetMessage("CategoryNameRequired", "Tên loại xét nghiệm không được để trống"); }
        }

        /// <summary>Mã loại xét nghiệm bị trùng: {0}</summary>
        internal static string CategoryCodeDuplicated(string codes)
        {
            return string.Format(
                GetMessage("CategoryCodeDuplicated", "Mã loại xét nghiệm bị trùng: {0}"),
                codes);
        }

        /// <summary>Pattern không được để trống</summary>
        internal static string RulePatternRequired
        {
            get { return GetMessage("RulePatternRequired", "Pattern không được để trống"); }
        }

        /// <summary>Kiểu match phải là PREFIX, CONTAINS hoặc REGEX</summary>
        internal static string RuleMatchTypeInvalid
        {
            get { return GetMessage("RuleMatchTypeInvalid", "Kiểu match phải là PREFIX, CONTAINS hoặc REGEX"); }
        }

        /// <summary>Pattern REGEX không hợp lệ: {0}</summary>
        internal static string RuleRegexInvalid(string pattern)
        {
            return string.Format(
                GetMessage("RuleRegexInvalid", "Pattern REGEX không hợp lệ: {0}"),
                pattern);
        }

        /// <summary>Extract key không được để trống</summary>
        internal static string RuleKeyExtractorRequired
        {
            get { return GetMessage("RuleKeyExtractorRequired", "Extract key không được để trống"); }
        }

        /// <summary>Extract key sai định dạng: {0}</summary>
        internal static string RuleKeyExtractorInvalid(string extractor)
        {
            return string.Format(
                GetMessage("RuleKeyExtractorInvalid", "Extract key sai định dạng (phải là seg[N] hoặc seg[N]+seg[M]): {0}"),
                extractor);
        }

        /// <summary>Loại xét nghiệm không được để trống</summary>
        internal static string RuleExamCategoryRequired
        {
            get { return GetMessage("RuleExamCategoryRequired", "Loại xét nghiệm không được để trống"); }
        }

        /// <summary>Thứ tự ưu tiên rule bị trùng trong loại: {0}</summary>
        internal static string RuleNumOrderDuplicated(string categoryName)
        {
            return string.Format(
                GetMessage("RuleNumOrderDuplicated", "Thứ tự ưu tiên rule bị trùng trong loại: {0}"),
                categoryName);
        }

        /// <summary>Vui lòng chọn loại xét nghiệm trước khi thêm rule</summary>
        internal static string SelectCategoryBeforeAddRule
        {
            get { return GetMessage("SelectCategoryBeforeAddRule", "Vui lòng chọn loại xét nghiệm trước khi thêm rule"); }
        }

        /// <summary>Thứ tự loại xét nghiệm phải >= 1 và <= {0}</summary>
        internal static string CategoryNumOrderInvalid(int maxOrder)
        {
            return string.Format(
                GetMessage("CategoryNumOrderInvalid", "Thứ tự loại xét nghiệm phải >= 1 và <= {0}"),
                maxOrder);
        }

        /// <summary>Thứ tự ưu tiên rule phải >= 1</summary>
        internal static string RuleNumOrderInvalid
        {
            get { return GetMessage("RuleNumOrderInvalid", "Thứ tự ưu tiên rule phải >= 1"); }
        }
    }
}

/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;

namespace HIS.Desktop.Plugins.HemodialysisDispensary.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.HemodialysisDispensary.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Vui lòng chọn bệnh nhân đã xếp lịch chạy thận.</summary>
        internal static string VuiLongChonBenhNhan
        {
            get { return GetValue("VuiLongChonBenhNhan"); }
        }

        /// <summary>Vui lòng chọn y lệnh bác sĩ.</summary>
        internal static string VuiLongChonYLenhBacSi
        {
            get { return GetValue("VuiLongChonYLenhBacSi"); }
        }

        /// <summary>Toa bác sĩ đã hết — yêu cầu tái khám kê toa mới.</summary>
        internal static string ToaBacSiDaHet
        {
            get { return GetValue("ToaBacSiDaHet"); }
        }

        private static string GetValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }
    }
}

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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransDepartment.Config
{
    class ConfigKey
    {
        public const string KEY__MOS_TREATMENT_ALLOW_MANY_TREATMENT_OPENING_OPTION = "MOS.TREATMENT.ALLOW_MANY_TREATMENT_OPENING_OPTION";
        public const string KEY__CHECK_REQUIRED_SERVICE = "HIS.Desktop.Plugins.TransDepartment.CheckRequiredService";
        //// Viec 59683: 1 = cho phep nguoi dung sua Thoi gian chuyen khoa tai man Chuyen khoa (tao moi),
        //// kiem tra voi thoi gian vao khoa + lich su giuong tai khoa hien tai. Khac 1/khong khai bao -> nhu cu.
        public const string KEY__MOS_HIS_DEPARTMENT_TRAN_ALLOW_EDIT_TRANSFER_TIME = "MOS.HIS_DEPARTMENT_TRAN.ALLOW_EDIT_TRANSFER_TIME";
        internal static string AllowManyOpeningOption;
        internal static bool AllowEditTransferTime;

        //// Danh sach SERVICE_TYPE_CODE (VD: XN|CDHA|PT) chan chuyen khoa khi benh nhan con dich vu
        //// thuoc loai do chua hoan thanh. Khong khai bao/de trong -> khong kiem tra.
        internal static string CheckRequiredService;
        internal static List<string> CheckRequiredServiceTypeCodes = new List<string>();

        internal static void GetConfigKey()
        {
            try
            {
                AllowManyOpeningOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MOS_TREATMENT_ALLOW_MANY_TREATMENT_OPENING_OPTION);
                CheckRequiredService = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__CHECK_REQUIRED_SERVICE);
                AllowEditTransferTime = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MOS_HIS_DEPARTMENT_TRAN_ALLOW_EDIT_TRANSFER_TIME) ?? "").Trim() == "1";
                CheckRequiredServiceTypeCodes = string.IsNullOrWhiteSpace(CheckRequiredService)
                    ? new List<string>()
                    : CheckRequiredService.Split('|').Select(o => (o ?? "").Trim().ToUpper()).Where(o => o.Length > 0).Distinct().ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedRoomPartial.Key
{
    class HisConfigCFG
    {
        private const string IS_ShowResultWhenReqComplete = "HIS.Desktop.Plugins.ContentSubclinical.ShowResultWhenReqComplete";
        private const string AI_ConnectionInfo = "HIS.Desktop.AI.ConnectionInfo";
        private const string ASSIGN_BED_OPTION = "HIS.Desktop.Plugins.AssignBed.Option";
        internal static string AIConnectionInfo
        {
            get
            {
                var AIConec = HisConfigs.Get<string>(AI_ConnectionInfo);
                return AIConec;
            }
        }
        internal static int AssignBedOption
        {
            get
            {
                try
                {
                    var optStr = HisConfigs.Get<string>(ASSIGN_BED_OPTION);
                    int opt;
                    if (!int.TryParse(optStr, out opt) || (opt != 1 && opt != 2))
                    {
                        opt = 1;
                    }
                    return opt;
                }
                catch
                {
                    return 1;
                }
            }
        }
        /// <summary>
        /// Bat hien thi don thuoc du tru theo ngay du tru tren man Buong benh (QT-11).
        /// Mac dinh tat de khong anh huong cac don vi dang su dung.
        /// </summary>
        internal static bool ShowAnticipatePresByUseDate
        {
            get
            {
                try
                {
                    return HisConfigs.Get<string>(
                        Key.HisConfigKeys.HIS_CONFIG_KEY__ShowAnticipatePresByUseDate) == "1";
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Format hien thi cua cot so luong (HIS.Desktop.AmountDecimalNumber = N so thap phan).
        /// Tra ve null khi key khong khai bao (hoac khong phai so nguyen 0..9): giu nguyen hien thi cu.
        /// </summary>
        internal static string AmountFormatString
        {
            get
            {
                try
                {
                    string configValue = HisConfigs.Get<string>(
                        Key.HisConfigKeys.HIS_CONFIG_KEY__AmountDecimalNumber);
                    if (String.IsNullOrWhiteSpace(configValue))
                        return null;

                    int decimalNumber;
                    if (!int.TryParse(configValue.Trim(), out decimalNumber) || decimalNumber < 0 || decimalNumber > 9)
                        return null;

                    return decimalNumber > 0 ? "#,##0." + new string('0', decimalNumber) : "#,##0";
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    return null;
                }
            }
        }

        internal static string IsShowResultWhenReqComplete
        {
            get
            {
                var ptBHYT = HisConfigs.Get<string>(IS_ShowResultWhenReqComplete);
                return ptBHYT;
            }
        }
        internal static long PatientTypeId__BHYT
        {
            get
            {
                var ptBHYT = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.PATIENT_TYPE_CODE == HisConfigs.Get<string>(Key.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT)).FirstOrDefault();
                return ptBHYT != null ? ptBHYT.ID : 0;
            }
        }

        internal static string PatientTypeCode__BHYT
        {
            get
            {
                var ptBHYT = HisConfigs.Get<string>(Key.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                return ptBHYT;
            }
        }

        internal static string PatientTypeCode__VP
        {
            get
            {
                var ptVP = HisConfigs.Get<string>(Key.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                return ptVP;
            }
        }

        /// <summary>
        /// Cau hinh canh bao Loai van ban bat buoc phai hoan thanh khi benh nhan vao khoa.
        ///
        /// Gia tri dang "{so phut}|{co kiem ca benh nhan cu}" — xem RequiredDocumentConfigADO:
        ///   "30"   -> chi kiem benh nhan vao khoa tu luc bat cau hinh tro di (mac dinh);
        ///   "30|1" -> kiem ca benh nhan da nam khoa tu truoc khi bat cau hinh.
        ///
        /// Moi truong hop khai bao sai deu tra ve CheckMinutes = 0, tuc khong kiem tra —
        /// trang thai mac dinh an toan cho vien chua bat. KHONG bao gio nem ngoai le.
        /// </summary>
        internal static RequiredDocumentConfigADO RequiredDocumentConfig
        {
            get
            {
                var result = new RequiredDocumentConfigADO();
                result.CheckMinutes = 0;
                result.IsCheckPatientAdmittedBeforeConfig = false;
                try
                {
                    var raw = HisConfigs.Get<string>(Key.HisConfigKeys.HIS_CONFIG_KEY__RequiredDocument);
                    if (string.IsNullOrWhiteSpace(raw))
                        return result;

                    string[] parts = raw.Split('|');

                    int minutes;
                    if (!int.TryParse((parts[0] ?? "").Trim(), out minutes) || minutes <= 0)
                        return result;
                    result.CheckMinutes = minutes;

                    // Chi dung "1" moi bo luat hoi to. Thieu doan sau hoac ghi gia tri khac
                    // deu giu luat hoi to (mac dinh an toan).
                    if (parts.Length > 1)
                        result.IsCheckPatientAdmittedBeforeConfig = (parts[1] ?? "").Trim() == "1";

                    return result;
                }
                catch
                {
                    result.CheckMinutes = 0;
                    result.IsCheckPatientAdmittedBeforeConfig = false;
                    return result;
                }
            }
        }
    }
}

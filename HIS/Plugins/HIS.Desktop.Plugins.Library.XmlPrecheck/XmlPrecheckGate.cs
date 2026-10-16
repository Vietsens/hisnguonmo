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
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.Library.XmlPrecheck.Base;
using Inventec.Common.Logging;
using System;

namespace HIS.Desktop.Plugins.Library.XmlPrecheck
{
    /// <summary>
    /// vCong MDInsight ban 3.0 - Cong tac cua tinh nang kiem tra loi XML, doc tu HIS_CONFIG.
    ///
    /// Ba khoa, kiem o CA HAI TANG (man hinh an/hien nut, va tang xu ly thoat som) - quy tac QT-30:
    ///  1. HIS.MDINSIGHT.CONNECTION_INFO               - dia chi, nguong, tai khoan theo co so (QT-01)
    ///  2. HIS.Desktop.Plugins.TreatmentFinish.XmlPrecheck - cong tac rieng cua man Ket thuc dieu tri, mac dinh TAT
    ///  3. HIS.Desktop.Plugins.TreatmentFinish.SaveTemp    - khoa SAN CO bat nut Luu tam; khong co Luu tam thi
    ///     ho so chua co thoi gian ra vien truoc khi ket thuc, khong co gi de kiem
    ///
    /// Tham chieu: PTTK muc B.2.5.7.
    /// </summary>
    public class XmlPrecheckGate
    {
        public const string KEY_CONNECTION_INFO = "HIS.MDINSIGHT.CONNECTION_INFO";
        public const string KEY_TREATMENT_FINISH_PRECHECK = "HIS.Desktop.Plugins.TreatmentFinish.XmlPrecheck";
        public const string KEY_TREATMENT_FINISH_SAVE_TEMP = "HIS.Desktop.Plugins.TreatmentFinish.SaveTemp";

        /// <summary>Cau hinh ket noi da doc. Null khi khoa 1 trong.</summary>
        public MdInsightConfig Config { get; private set; }

        /// <summary>Khoa 1 khai du dia chi va it nhat mot doan tai khoan hop le</summary>
        public bool IsConnectionConfigured
        {
            get { return this.Config != null && this.Config.IsValidConfig; }
        }

        /// <summary>Khoa 2 = 1</summary>
        public bool IsTreatmentFinishSwitchOn { get; private set; }

        /// <summary>Khoa 3 = 1</summary>
        public bool IsSaveTempOn { get; private set; }

        /// <summary>Du ca ba khoa - dieu kien de man Ket thuc dieu tri hien nut kiem so bo</summary>
        public bool IsTreatmentFinishEnabled
        {
            get { return this.IsConnectionConfigured && this.IsTreatmentFinishSwitchOn && this.IsSaveTempOn; }
        }

        private XmlPrecheckGate()
        {
        }

        /// <summary>Doc ba khoa tu bo nho dem cau hinh cua may tram. Khong phat sinh truy van nao.</summary>
        public static XmlPrecheckGate Read()
        {
            XmlPrecheckGate gate = new XmlPrecheckGate();
            try
            {
                string connectionInfo = HisConfigs.Get<string>(KEY_CONNECTION_INFO);
                gate.Config = string.IsNullOrWhiteSpace(connectionInfo) ? null : new MdInsightConfig(connectionInfo);
                gate.IsTreatmentFinishSwitchOn = HisConfigs.Get<string>(KEY_TREATMENT_FINISH_PRECHECK) == "1";
                gate.IsSaveTempOn = HisConfigs.Get<string>(KEY_TREATMENT_FINISH_SAVE_TEMP) == "1";

                if (gate.IsTreatmentFinishSwitchOn && !gate.IsTreatmentFinishEnabled)
                {
                    //Khai cong tac ma thieu dieu kien khac thi ghi ro cho quan tri, khong bao nguoi dung
                    LogSystem.Warn("XmlPrecheck - Khoa " + KEY_TREATMENT_FINISH_PRECHECK + " dang bat nhung"
                        + (gate.IsConnectionConfigured ? "" : " khoa ket noi " + KEY_CONNECTION_INFO + " chua khai du;")
                        + (gate.IsSaveTempOn ? "" : " khoa " + KEY_TREATMENT_FINISH_SAVE_TEMP + " chua bat;")
                        + " -> kiem so bo tai man Ket thuc dieu tri KHONG kich hoat.");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                gate.Config = null;
                gate.IsTreatmentFinishSwitchOn = false;
                gate.IsSaveTempOn = false;
            }
            return gate;
        }
    }
}

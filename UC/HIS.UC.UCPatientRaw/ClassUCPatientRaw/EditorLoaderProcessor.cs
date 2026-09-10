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
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCPatientRaw.ClassUCPatientRaw
{
    public partial class EditorLoaderProcessor
    {
        internal static void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal static void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Init combo nghe nghiep: Ma + Ten + Nhom cap 2/3/4 theo key cau hinh
        /// MOS.HIS_CAREER.IS_SHOW_LEVEL_2/3/4 (rong/khac 1 = an, mac dinh an nhu cu)
        /// </summary>
        internal static void InitComboCareer(Control cboEditor, object data)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CAREER_CODE", "", 100, 1));
                popupWidth += 100;
                columnInfos.Add(new ColumnInfo("CAREER_NAME", "", 250, 2));
                popupWidth += 250;
                if (CareerLevelConfig.IsShowLevel2)
                {
                    columnInfos.Add(new ColumnInfo("LEVEL2_NAME", "", 180, 3));
                    popupWidth += 180;
                }
                if (CareerLevelConfig.IsShowLevel3)
                {
                    columnInfos.Add(new ColumnInfo("LEVEL3_NAME", "", 180, 4));
                    popupWidth += 180;
                }
                if (CareerLevelConfig.IsShowLevel4)
                {
                    columnInfos.Add(new ColumnInfo("LEVEL4_NAME", "", 180, 5));
                    popupWidth += 180;
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO("CAREER_NAME", "ID", columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    /// <summary>
    /// Doc key cau hinh hien thi cap 2/3/4 tren combo nghe nghiep (cache 1 lan)
    /// </summary>
    internal class CareerLevelConfig
    {
        private const string CONFIG_KEY__IS_SHOW_LEVEL_2 = "MOS.HIS_CAREER.IS_SHOW_LEVEL_2";
        private const string CONFIG_KEY__IS_SHOW_LEVEL_3 = "MOS.HIS_CAREER.IS_SHOW_LEVEL_3";
        private const string CONFIG_KEY__IS_SHOW_LEVEL_4 = "MOS.HIS_CAREER.IS_SHOW_LEVEL_4";

        private static bool? isShowLevel2;
        private static bool? isShowLevel3;
        private static bool? isShowLevel4;

        internal static bool IsShowLevel2
        {
            get
            {
                if (!isShowLevel2.HasValue)
                {
                    isShowLevel2 = GetConfig(CONFIG_KEY__IS_SHOW_LEVEL_2);
                }
                return isShowLevel2.Value;
            }
        }

        internal static bool IsShowLevel3
        {
            get
            {
                if (!isShowLevel3.HasValue)
                {
                    isShowLevel3 = GetConfig(CONFIG_KEY__IS_SHOW_LEVEL_3);
                }
                return isShowLevel3.Value;
            }
        }

        internal static bool IsShowLevel4
        {
            get
            {
                if (!isShowLevel4.HasValue)
                {
                    isShowLevel4 = GetConfig(CONFIG_KEY__IS_SHOW_LEVEL_4);
                }
                return isShowLevel4.Value;
            }
        }

        private static bool GetConfig(string key)
        {
            bool result = false;
            try
            {
                result = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(key) == "1";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }
            return result;
        }
    }
}

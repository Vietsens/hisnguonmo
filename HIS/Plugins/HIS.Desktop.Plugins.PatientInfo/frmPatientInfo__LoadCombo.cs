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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PatientInfo
{
    public partial class frmPatientInfo : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Combo nghe nghiep: tieu de cot Ma / Ten nghe nghiep + cot Nhom cap 2/3/4 theo key cau hinh
        /// MOS.HIS_CAREER.IS_SHOW_LEVEL_2/3/4 (rong/khac 1 = an, mac dinh an nhu cu) - viec 56781
        /// </summary>
        void InitComboCareer(LookUpEdit cboEditor, object datasource)
        {
            try
            {
                cboEditor.Properties.DataSource = null;
                cboEditor.Properties.DataSource = datasource;
                cboEditor.Properties.DisplayMember = "CAREER_NAME";
                cboEditor.Properties.ValueMember = "ID";
                cboEditor.Properties.ForceInitialize();
                cboEditor.Properties.Columns.Clear();
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo("CAREER_CODE", "Mã", 100));
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo("CAREER_NAME", "Tên nghề nghiệp", 250));
                int popupWidth = 350;
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_CAREER.IS_SHOW_LEVEL_2") == "1")
                {
                    cboEditor.Properties.Columns.Add(new LookUpColumnInfo("LEVEL2_NAME", "Nhóm cấp 2", 180));
                    popupWidth += 180;
                }
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_CAREER.IS_SHOW_LEVEL_3") == "1")
                {
                    cboEditor.Properties.Columns.Add(new LookUpColumnInfo("LEVEL3_NAME", "Nhóm cấp 3", 180));
                    popupWidth += 180;
                }
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_CAREER.IS_SHOW_LEVEL_4") == "1")
                {
                    cboEditor.Properties.Columns.Add(new LookUpColumnInfo("LEVEL4_NAME", "Nhóm cấp 4", 180));
                    popupWidth += 180;
                }
                cboEditor.Properties.ShowHeader = true;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.Properties.DropDownRows = 20;
                cboEditor.Properties.PopupWidth = popupWidth;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FillDataToLookupedit(LookUpEdit cboEditor, string displayMember, string valueMember, string displayCodeMember, object datasource)
        {
            try
            {
                cboEditor.Properties.DataSource = null;
                cboEditor.Properties.DataSource = datasource;
                cboEditor.Properties.DisplayMember = displayMember;
                cboEditor.Properties.ValueMember = valueMember;
                cboEditor.Properties.ForceInitialize();
                cboEditor.Properties.Columns.Clear();
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayCodeMember, "", 50));
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayMember, "", 100));
                cboEditor.Properties.ShowHeader = false;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.Properties.DropDownRows = 20;
                cboEditor.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        void FillDataToLookupedit1(LookUpEdit cboEditor, string valueMember, string displayCodeMember, object datasource)
        {
            try
            {
                cboEditor.Properties.DataSource = null;
                cboEditor.Properties.DataSource = datasource;
                cboEditor.Properties.DisplayMember = displayCodeMember;
                cboEditor.Properties.ValueMember = valueMember;
                cboEditor.Properties.ForceInitialize();
                cboEditor.Properties.Columns.Clear();
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayCodeMember, "", 50));
                cboEditor.Properties.ShowHeader = false;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.Properties.DropDownRows = 10;
                cboEditor.Properties.PopupWidth = 50;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        void FillDataToLookupedit(DevExpress.XtraEditors.LookUpEdit cboEditor, string displayMember, string valueMember, object datasource)
        {
            try
            {
                cboEditor.Properties.DataSource = datasource;
                cboEditor.Properties.DisplayMember = displayMember;
                cboEditor.Properties.ValueMember = valueMember;
                cboEditor.Properties.ForceInitialize();
                cboEditor.Properties.Columns.Clear();
                //cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayCodeMember, "", 50));
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayMember, "", 100));
                cboEditor.Properties.ShowHeader = false;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.Properties.DropDownRows = 20;
                cboEditor.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FocusShowPopup(DateEdit cboEditor)
        {
            cboEditor.Focus();
            cboEditor.ShowPopup();
        }

        private void FocusShowPopup(LookUpEdit cboEditor)
        {
            cboEditor.Focus();
            cboEditor.ShowPopup();
        }
        private void FocusMoveText(TextEdit txt)
        {
            try
            {
                txt.Focus();
                txt.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}

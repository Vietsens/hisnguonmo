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
using MOS.EFMODEL.DataModels;
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
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    public partial class frmPatientUpdate : HIS.Desktop.Utility.FormBase
    {
        void FillDataToLookupedit(DevExpress.XtraEditors.LookUpEdit cboEditor, string displayMember, string valueMember, string displayCodeMember, object datasource)
        {
            try
            {
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
        void FillDataToGridLookupedit(DevExpress.XtraEditors.GridLookUpEdit cboEditor, string displayMember, string valueMember, string displayCodeMember, object datasource)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(displayCodeMember, "", 100, 1));
                columnInfos.Add(new ColumnInfo(displayMember, "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboEditor, datasource, controlEditorADO);
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
        void FillDataToLookupedit(DevExpress.XtraEditors.LookUpEdit cboEditor, string displayMember, string valueMember, object datasource, int PopupWidth)
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
                cboEditor.Properties.PopupWidth = PopupWidth;
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

        private void FocusShowPopup(GridLookUpEdit cboEditor, GridView gridView)
        {
            cboEditor.Focus();
            cboEditor.ShowPopup();
            gridView.FocusedRowHandle = 0;
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

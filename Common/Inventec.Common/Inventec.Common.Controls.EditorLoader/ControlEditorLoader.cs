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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Controls.EditorLoader
{
    public class ControlEditorLoader
    {
        public static void Load(object control, object dataSource, ControlEditorADO controlEditorADO)
        {
            try
            {
                if (control is DevExpress.XtraEditors.LookUpEdit)
                {
                    LoadDataToLookUpEdit((DevExpress.XtraEditors.LookUpEdit)control, dataSource, controlEditorADO);
                }
                else if (control is DevExpress.XtraEditors.GridLookUpEdit)
                {
                    LoadDataToGridLookUpEdit((DevExpress.XtraEditors.GridLookUpEdit)control, dataSource, controlEditorADO);
                }
                else if (control is DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit)
                {
                    LoadDataToLookUpEdit((DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit)control, dataSource, controlEditorADO);
                }
                else if (control is DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit)
                {
                    LoadDataToGridLookUpEdit((DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit)control, dataSource, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static void LoadDataToLookUpEdit(DevExpress.XtraEditors.LookUpEdit cboEditor, object dataSource, ControlEditorADO controlEditorADO)
        {
            try
            {
                cboEditor.Properties.DataSource = dataSource;
                cboEditor.Properties.DisplayMember = controlEditorADO.DisplayMember;
                cboEditor.Properties.ValueMember = controlEditorADO.ValueMember;
                cboEditor.Properties.ForceInitialize();

                cboEditor.Properties.Columns.Clear();
                foreach (var columnInfo in controlEditorADO.ColumnInfos)
                {
                    var formType = DevExpress.Utils.FormatType.None;
                    if (columnInfo.formatType != null)
                    {
                        switch (columnInfo.formatType)
                        {
                            case ColumnInfo.FormatType.None:
                                formType = DevExpress.Utils.FormatType.None;
                                break;
                            case ColumnInfo.FormatType.Numeric:
                                formType = DevExpress.Utils.FormatType.Numeric;
                                break;
                            case ColumnInfo.FormatType.DateTime:
                                formType = DevExpress.Utils.FormatType.DateTime;
                                break;
                            case ColumnInfo.FormatType.Custom:
                                formType = DevExpress.Utils.FormatType.Custom;
                                break;
                            default:
                                break;
                        }
                    }
                    var horzAlignment = DevExpress.Utils.HorzAlignment.Default;
                    switch (columnInfo.horzAlignment)
                    {
                        case ColumnInfo.HorzAlignment.Default:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Default;
                            break;
                        case ColumnInfo.HorzAlignment.Near:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Near;
                            break;
                        case ColumnInfo.HorzAlignment.Center:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Center;
                            break;
                        case ColumnInfo.HorzAlignment.Far:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Far;
                            break;
                        default:
                            break;
                    }

                    cboEditor.Properties.Columns.Add(new LookUpColumnInfo(columnInfo.fieldName, columnInfo.caption, columnInfo.width, formType, columnInfo.formatString, columnInfo.visible, horzAlignment));
                }

                cboEditor.Properties.ShowHeader = controlEditorADO.ShowHeader;
                cboEditor.Properties.ImmediatePopup = controlEditorADO.ImmediatePopup;
                cboEditor.Properties.DropDownRows = (controlEditorADO.DropDownRows == 0 ? ControlEditorADO.DEFAULT__DROP_DOWN_ROW : controlEditorADO.DropDownRows);
                cboEditor.Properties.PopupWidth = (controlEditorADO.PopupWidth == 0 ? ControlEditorADO.DEFAULT__POPUP_WIDTH : controlEditorADO.PopupWidth);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static void LoadDataToGridLookUpEdit(DevExpress.XtraEditors.GridLookUpEdit cboEditor, object dataSource, ControlEditorADO controlEditorADO)
        {
            try
            {
                cboEditor.Properties.DataSource = dataSource;
                cboEditor.Properties.DisplayMember = controlEditorADO.DisplayMember;
                cboEditor.Properties.ValueMember = controlEditorADO.ValueMember;

                cboEditor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboEditor.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboEditor.Properties.ImmediatePopup = controlEditorADO.ImmediatePopup;
                cboEditor.ForceInitialize();
                cboEditor.Properties.View.Columns.Clear();

                foreach (var columnInfo in controlEditorADO.ColumnInfos)
                {
                    var horzAlignment = DevExpress.Utils.HorzAlignment.Default;
                    switch (columnInfo.horzAlignment)
                    {
                        case ColumnInfo.HorzAlignment.Default:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Default;
                            break;
                        case ColumnInfo.HorzAlignment.Near:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Near;
                            break;
                        case ColumnInfo.HorzAlignment.Center:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Center;
                            break;
                        case ColumnInfo.HorzAlignment.Far:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Far;
                            break;
                        default:
                            break;
                    }

                    GridColumn aColumnCode = cboEditor.Properties.View.Columns.AddField(columnInfo.fieldName);
                    aColumnCode.Caption = columnInfo.caption;
                    aColumnCode.Visible = columnInfo.visible;
                    aColumnCode.VisibleIndex = columnInfo.VisibleIndex;
                    aColumnCode.Width = (columnInfo.width == 0 ? ControlEditorADO.DEFAULT__COLUMN_WIDTH : columnInfo.width);
                    aColumnCode.AppearanceCell.TextOptions.HAlignment = horzAlignment;
                    aColumnCode.OptionsColumn.FixedWidth = columnInfo.FixedWidth;
                }

                cboEditor.Properties.View.OptionsView.ColumnAutoWidth = false;
                cboEditor.Properties.View.OptionsView.ShowIndicator = false;
                cboEditor.Properties.View.OptionsView.ShowGroupPanel = false;
                cboEditor.Properties.PopupFormSize = new System.Drawing.Size(controlEditorADO.PopupWidth + 20, 200);
                cboEditor.Properties.View.OptionsView.ShowColumnHeaders = controlEditorADO.ShowHeader;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static void LoadDataToLookUpEdit(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cboEditor, object dataSource, ControlEditorADO controlEditorADO)
        {
            try
            {
                cboEditor.DataSource = dataSource;
                cboEditor.DisplayMember = controlEditorADO.DisplayMember;
                cboEditor.ValueMember = controlEditorADO.ValueMember;
                cboEditor.ForceInitialize();

                cboEditor.Columns.Clear();
                foreach (var columnInfo in controlEditorADO.ColumnInfos)
                {
                    var formType = DevExpress.Utils.FormatType.None;
                    if (columnInfo.formatType != null)
                    {
                        switch (columnInfo.formatType)
                        {
                            case ColumnInfo.FormatType.None:
                                formType = DevExpress.Utils.FormatType.None;
                                break;
                            case ColumnInfo.FormatType.Numeric:
                                formType = DevExpress.Utils.FormatType.Numeric;
                                break;
                            case ColumnInfo.FormatType.DateTime:
                                formType = DevExpress.Utils.FormatType.DateTime;
                                break;
                            case ColumnInfo.FormatType.Custom:
                                formType = DevExpress.Utils.FormatType.Custom;
                                break;
                            default:
                                break;
                        }
                    }
                    var horzAlignment = DevExpress.Utils.HorzAlignment.Default;
                    switch (columnInfo.horzAlignment)
                    {
                        case ColumnInfo.HorzAlignment.Default:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Default;
                            break;
                        case ColumnInfo.HorzAlignment.Near:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Near;
                            break;
                        case ColumnInfo.HorzAlignment.Center:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Center;
                            break;
                        case ColumnInfo.HorzAlignment.Far:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Far;
                            break;
                        default:
                            break;
                    }

                    cboEditor.Columns.Add(new LookUpColumnInfo(columnInfo.fieldName, columnInfo.caption, columnInfo.width, formType, columnInfo.formatString, columnInfo.visible, horzAlignment));
                }

                cboEditor.ShowHeader = controlEditorADO.ShowHeader;
                cboEditor.ImmediatePopup = controlEditorADO.ImmediatePopup;
                cboEditor.DropDownRows = (controlEditorADO.DropDownRows == 0 ? ControlEditorADO.DEFAULT__DROP_DOWN_ROW : controlEditorADO.DropDownRows);
                cboEditor.PopupWidth = (controlEditorADO.PopupWidth == 0 ? ControlEditorADO.DEFAULT__POPUP_WIDTH : controlEditorADO.PopupWidth);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static void LoadDataToGridLookUpEdit(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cboEditor, object dataSource, ControlEditorADO controlEditorADO)
        {
            try
            {
                cboEditor.DataSource = dataSource;
                cboEditor.DisplayMember = controlEditorADO.DisplayMember;
                cboEditor.ValueMember = controlEditorADO.ValueMember;

                cboEditor.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboEditor.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboEditor.ImmediatePopup = controlEditorADO.ImmediatePopup;
                //cboEditor.ForceInitialize();
                cboEditor.View.Columns.Clear();

                foreach (var columnInfo in controlEditorADO.ColumnInfos)
                {
                    var horzAlignment = DevExpress.Utils.HorzAlignment.Default;

                    switch (columnInfo.horzAlignment)
                    {
                        case ColumnInfo.HorzAlignment.Default:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Default;
                            break;
                        case ColumnInfo.HorzAlignment.Near:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Near;
                            break;
                        case ColumnInfo.HorzAlignment.Center:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Center;
                            break;
                        case ColumnInfo.HorzAlignment.Far:
                            horzAlignment = DevExpress.Utils.HorzAlignment.Far;
                            break;
                        default:
                            break;
                    }

                    GridColumn aColumnCode = cboEditor.View.Columns.AddField(columnInfo.fieldName);
                    var formType = DevExpress.Utils.FormatType.None;
                    if (columnInfo.formatType != null)
                    {
                        switch (columnInfo.formatType)
                        {
                            case ColumnInfo.FormatType.None:
                                formType = DevExpress.Utils.FormatType.None;
                                break;
                            case ColumnInfo.FormatType.Numeric:
                                formType = DevExpress.Utils.FormatType.Numeric;
                                break;
                            case ColumnInfo.FormatType.DateTime:
                                formType = DevExpress.Utils.FormatType.DateTime;
                                break;
                            case ColumnInfo.FormatType.Custom:
                                formType = DevExpress.Utils.FormatType.Custom;
                                break;
                            default:
                                break;
                        }
                    }
                    aColumnCode.DisplayFormat.FormatType = formType;
                    if (!String.IsNullOrEmpty(columnInfo.formatString))
                    {
                        aColumnCode.DisplayFormat.FormatString = columnInfo.formatString;
                    }
                    aColumnCode.Caption = columnInfo.caption;
                    aColumnCode.Visible = columnInfo.visible;
                    aColumnCode.VisibleIndex = columnInfo.VisibleIndex;
                    aColumnCode.Width = (columnInfo.width == 0 ? ControlEditorADO.DEFAULT__COLUMN_WIDTH : columnInfo.width);
                    aColumnCode.AppearanceCell.TextOptions.HAlignment = horzAlignment;
                    aColumnCode.OptionsColumn.FixedWidth = columnInfo.FixedWidth;
                }
                if (controlEditorADO.PopupWidth > 0)
                {
                    cboEditor.PopupFormWidth = controlEditorADO.PopupWidth;
                }
                cboEditor.View.OptionsView.ColumnAutoWidth = true;
                cboEditor.View.OptionsView.ShowColumnHeaders = controlEditorADO.ShowHeader;
                cboEditor.View.OptionsView.ShowIndicator = false;
                cboEditor.View.OptionsView.ShowGroupPanel = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

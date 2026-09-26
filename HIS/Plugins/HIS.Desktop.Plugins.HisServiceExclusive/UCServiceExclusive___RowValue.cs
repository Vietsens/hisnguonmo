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
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.Plugins.HisServiceExclusive.ADO;
using HIS.Desktop.Plugins.Library.CheckServiceExclusive.ADO;
using HIS.UC.Service;
using HIS.UC.Service.ADO;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisServiceExclusive
{
    /// <summary>
    /// Task 57452 - test feedback 23/09/2026 (tester: "chuyen muc xu ly + ghi chu + con su dung len bang, de luu theo
    /// tung dong, khong luu chung"): the right grid carries 3 editable columns "Muc xu ly" / "Con su dung" / "Ghi chu"
    /// so every exclusive pair of the selected base service is declared and saved with its own values.
    ///
    /// The grid is the shared HIS.UC.Service control, which is NOT modified:
    ///  - the 3 columns are UNBOUND (field names not present in ServiceADO); their values live in dicRowValue,
    ///  - value -> cell through ServiceInitADO.ServiceGrid_CustomUnboundColumnData (the UC forwards IsGetData only),
    ///  - cell -> value: the UC IGNORES IsSetData of unbound columns and CellValueChanged of an unbound column carries the value
    ///    RE-READ from the cell (i.e. the old one) - so the edited value is caught by a CustomUnboundColumnData handler attached
    ///    to the grid view returned by UCServiceProcessor.GetGridControl (IsSetData only, our 3 columns only),
    ///  - editors are attached to the columns through the same GetGridControl once the UC has created them.
    /// Verified by an automated harness on the real HIS.UC.Service grid (25/09/2026).
    /// </summary>
    public partial class UCServiceExclusive
    {
        #region Declare - per-row values
        private const string EXCL_HANDLE_TYPE_FIELD = "EXCL_HANDLE_TYPE_ID";
        private const string EXCL_IS_ACTIVE_FIELD = "EXCL_IS_ACTIVE";
        private const string EXCL_NOTE_FIELD = "EXCL_NOTE";

        /// <summary>Same limit as the former note memo (HIS_SERVICE_EXCLUSIVE.NOTE)</summary>
        private const int EXCL_NOTE_MAX_LENGTH = 2000;

        /// <summary>
        /// Values of the right grid rows, key = id of the OTHER service of the pair. Rebuilt from the database each time the
        /// checked state of the grid is rebuilt (base service picked, page/search changed, after save), so unsaved edits follow
        /// the same rule as unsaved ticks.
        /// </summary>
        private Dictionary<long, ExclusiveRowValueADO> dicRowValue = new Dictionary<long, ExclusiveRowValueADO>();

        private List<HandleTypeItem> handleTypeItems;
        private RepositoryItemLookUpEdit repositoryItemHandleType;
        private RepositoryItemCheckEdit repositoryItemIsActive;
        private RepositoryItemTextEdit repositoryItemNote;
        private bool isRowValueEditorReady;
        #endregion

        #region Columns + editors
        /// <summary>Add the 3 editable per-row columns and wire the value callbacks of the shared grid</summary>
        private void AddRowValueColumns(ServiceInitADO ado)
        {
            try
            {
                ado.ServiceGrid_CustomUnboundColumnData = ExclusiveGrid_CustomUnboundColumnData;
                ado.gridView_CellValueChanged = ExclusiveGrid_CellValueChanged;

                HIS.UC.Service.ServiceColumn colHandleType = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colHandleType.Caption"), EXCL_HANDLE_TYPE_FIELD, 90, true);
                colHandleType.VisibleIndex = 4;
                colHandleType.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colHandleType.Tooltip = GetLang("UCServiceExclusive.colHandleType.Tooltip");
                ado.ListServiceColumn.Add(colHandleType);

                HIS.UC.Service.ServiceColumn colIsActive = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colIsActive.Caption"), EXCL_IS_ACTIVE_FIELD, 75, true);
                colIsActive.VisibleIndex = 5;
                colIsActive.UnboundColumnType = DevExpress.Data.UnboundColumnType.Boolean;
                ado.ListServiceColumn.Add(colIsActive);

                HIS.UC.Service.ServiceColumn colNote = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colNote.Caption"), EXCL_NOTE_FIELD, 200, true);
                colNote.VisibleIndex = 6;
                colNote.UnboundColumnType = DevExpress.Data.UnboundColumnType.String;
                colNote.Tooltip = GetLang("UCServiceExclusive.colNote.Tooltip");
                ado.ListServiceColumn.Add(colNote);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>The shared grid creates its columns in its own Load -> attach the editors right after it</summary>
        private void ucGridControlExclusive_Load(object sender, EventArgs e)
        {
            try
            {
                SetupRowValueEditors();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private GridView GetExclusiveGridView()
        {
            GridView result = null;
            try
            {
                if (exclusiveProcessor != null && ucGridControlExclusive != null)
                {
                    GridControl grid = exclusiveProcessor.GetGridControl(ucGridControlExclusive) as GridControl;
                    if (grid != null)
                    {
                        result = grid.MainView as GridView;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Attach the editors to the 3 columns (idempotent, does nothing until the shared grid has created its columns)</summary>
        private void SetupRowValueEditors()
        {
            try
            {
                if (isRowValueEditorReady)
                {
                    return;
                }

                GridView view = GetExclusiveGridView();
                if (view == null || view.GridControl == null)
                {
                    return;
                }

                GridColumn colHandleType = view.Columns[EXCL_HANDLE_TYPE_FIELD];
                GridColumn colIsActive = view.Columns[EXCL_IS_ACTIVE_FIELD];
                GridColumn colNote = view.Columns[EXCL_NOTE_FIELD];
                if (colHandleType == null || colIsActive == null || colNote == null)
                {
                    return;
                }

                repositoryItemHandleType = new RepositoryItemLookUpEdit();
                repositoryItemHandleType.DataSource = handleTypeItems ?? new List<HandleTypeItem>();
                repositoryItemHandleType.DisplayMember = "NAME";
                repositoryItemHandleType.ValueMember = "ID";
                repositoryItemHandleType.NullText = "";
                repositoryItemHandleType.ShowHeader = false;
                repositoryItemHandleType.ShowFooter = false;
                repositoryItemHandleType.TextEditStyle = TextEditStyles.DisableTextEditor;
                repositoryItemHandleType.Columns.Add(new LookUpColumnInfo("NAME", ""));
                repositoryItemHandleType.EditValueChanged += repositoryItemRowValue_EditValueChanged;

                repositoryItemIsActive = new RepositoryItemCheckEdit();
                // Unticked row -> cell value null: draw it as an empty (unchecked) box, not the blue "indeterminate" state
                repositoryItemIsActive.AllowGrayed = false;
                repositoryItemIsActive.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
                repositoryItemIsActive.CheckedChanged += repositoryItemRowValue_EditValueChanged;

                repositoryItemNote = new RepositoryItemTextEdit();
                repositoryItemNote.MaxLength = EXCL_NOTE_MAX_LENGTH;
                repositoryItemNote.NullValuePrompt = GetLang("UCServiceExclusive.txtNote.NullValuePrompt");

                view.GridControl.RepositoryItems.AddRange(new RepositoryItem[] { repositoryItemHandleType, repositoryItemIsActive, repositoryItemNote });
                colHandleType.ColumnEdit = repositoryItemHandleType;
                colIsActive.ColumnEdit = repositoryItemIsActive;
                colNote.ColumnEdit = repositoryItemNote;

                // The shared UC only answers IsGetData -> store the edited value ourselves (see class summary)
                view.CustomUnboundColumnData += ExclusiveGridView_CustomUnboundColumnData_SetData;

                isRowValueEditorReady = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Post the combo / check value at once (not only when the cell is left)</summary>
        private void repositoryItemRowValue_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridView view = GetExclusiveGridView();
                if (view != null)
                {
                    view.PostEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Commit the cell being edited (typed note...) before the grid data is read on Save</summary>
        private void PostExclusiveGridEditor()
        {
            try
            {
                GridView view = GetExclusiveGridView();
                if (view != null)
                {
                    view.CloseEditor();
                    view.UpdateCurrentRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Value <-> cell
        /// <summary>Cell value of the 3 per-row columns: empty on rows that are not ticked</summary>
        private void ExclusiveGrid_CustomUnboundColumnData(V_HIS_SERVICE data, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e == null || !e.IsGetData || e.Column == null)
                {
                    return;
                }
                string fieldName = e.Column.FieldName;
                if (fieldName != EXCL_HANDLE_TYPE_FIELD && fieldName != EXCL_IS_ACTIVE_FIELD && fieldName != EXCL_NOTE_FIELD)
                {
                    return;
                }

                ServiceADO row = data as ServiceADO;
                if (row == null || !row.checkWarning || row.ID == serviceIdChecked)
                {
                    e.Value = null;
                    return;
                }

                ExclusiveRowValueADO value = GetRowValue(row.ID);
                if (fieldName == EXCL_HANDLE_TYPE_FIELD)
                {
                    e.Value = value.HandleTypeId;
                }
                else if (fieldName == EXCL_IS_ACTIVE_FIELD)
                {
                    e.Value = value.IsActive;
                }
                else
                {
                    e.Value = value.Note;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Store the edited value of a row (IsSetData of our 3 unbound columns). Editing a row that is not ticked ticks it:
        /// declaring a handle type / note for a service means choosing it as an exclusive service. The row is ticked BEFORE the
        /// grid re-reads the cell, so the re-read returns the value just stored.
        /// </summary>
        private void ExclusiveGridView_CustomUnboundColumnData_SetData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e == null || !e.IsSetData || e.Column == null)
                {
                    return;
                }
                string fieldName = e.Column.FieldName;
                if (fieldName != EXCL_HANDLE_TYPE_FIELD && fieldName != EXCL_IS_ACTIVE_FIELD && fieldName != EXCL_NOTE_FIELD)
                {
                    return;
                }

                ServiceADO row = e.Row as ServiceADO;
                if (row == null || row.ID == serviceIdChecked)
                {
                    return;
                }

                ExclusiveRowValueADO value = GetOrCreateRowValue(row.ID);
                if (fieldName == EXCL_HANDLE_TYPE_FIELD)
                {
                    long handleTypeId = e.Value != null ? Inventec.Common.TypeConvert.Parse.ToInt64(e.Value.ToString()) : 0;
                    value.HandleTypeId = handleTypeId == (long)HandleType.Block ? (short)HandleType.Block : (short)HandleType.Warning;
                }
                else if (fieldName == EXCL_IS_ACTIVE_FIELD)
                {
                    value.IsActive = e.Value is bool && (bool)e.Value;
                }
                else
                {
                    value.Note = NormalizeNote(e.Value != null ? e.Value.ToString() : null);
                }

                if (!row.checkWarning)
                {
                    row.checkWarning = true;
                    row.checkService = false;
                    row.checkServiceNotUse = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>After an edit of our 3 columns: repaint the row (the tick column may have been set by the edit)</summary>
        private void ExclusiveGrid_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null || e == null || e.Column == null)
                {
                    return;
                }
                string fieldName = e.Column.FieldName;
                if (fieldName == EXCL_HANDLE_TYPE_FIELD || fieldName == EXCL_IS_ACTIVE_FIELD || fieldName == EXCL_NOTE_FIELD)
                {
                    view.RefreshRow(e.RowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Row values
        /// <summary>
        /// Load the row values from the pairs of the selected base service. A pair may exist in both directions (A,B) and (B,A)
        /// (constraint UK1 is one-directional): take the most severe handle type, in use when any record is in use and the
        /// first non-empty note - the next save writes these values to every record of the pair.
        /// </summary>
        private void BuildRowValues(Dictionary<long, List<HIS_SERVICE_EXCLUSIVE>> mapped)
        {
            try
            {
                dicRowValue = new Dictionary<long, ExclusiveRowValueADO>();
                if (mapped == null)
                {
                    return;
                }

                foreach (var kv in mapped)
                {
                    List<HIS_SERVICE_EXCLUSIVE> pairs = kv.Value;
                    if (pairs == null || pairs.Count == 0)
                    {
                        continue;
                    }

                    ExclusiveRowValueADO value = new ExclusiveRowValueADO();
                    value.HandleTypeId = pairs.Any(o => o.HANDLE_TYPE_ID == (short)HandleType.Block)
                        ? (short)HandleType.Block
                        : (short)HandleType.Warning;
                    value.IsActive = pairs.Any(o => (o.IS_ACTIVE ?? IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    HIS_SERVICE_EXCLUSIVE withNote = pairs.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.NOTE));
                    value.Note = withNote != null ? NormalizeNote(withNote.NOTE) : null;
                    dicRowValue[kv.Key] = value;
                }
            }
            catch (Exception ex)
            {
                dicRowValue = new Dictionary<long, ExclusiveRowValueADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Default values of a newly ticked row (document 3342): Warning, in use, no note</summary>
        private ExclusiveRowValueADO CreateDefaultRowValue()
        {
            ExclusiveRowValueADO result = new ExclusiveRowValueADO();
            result.HandleTypeId = (short)HandleType.Warning;
            result.IsActive = true;
            result.Note = null;
            return result;
        }

        /// <summary>Values of a row (defaults when the row has no stored value yet). Does not store anything.</summary>
        private ExclusiveRowValueADO GetRowValue(long serviceId)
        {
            ExclusiveRowValueADO result = null;
            try
            {
                if (dicRowValue != null)
                {
                    dicRowValue.TryGetValue(serviceId, out result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result ?? CreateDefaultRowValue();
        }

        private ExclusiveRowValueADO GetOrCreateRowValue(long serviceId)
        {
            if (dicRowValue == null)
            {
                dicRowValue = new Dictionary<long, ExclusiveRowValueADO>();
            }
            ExclusiveRowValueADO result;
            if (!dicRowValue.TryGetValue(serviceId, out result) || result == null)
            {
                result = CreateDefaultRowValue();
                dicRowValue[serviceId] = result;
            }
            return result;
        }

        private string NormalizeNote(string note)
        {
            return String.IsNullOrWhiteSpace(note) ? null : note.Trim();
        }

        private short ToIsActiveValue(ExclusiveRowValueADO value)
        {
            return value != null && value.IsActive
                ? IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                : IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE;
        }

        /// <summary>Record of a pair differs from the values declared on its row -> must be updated</summary>
        private bool IsPairDifferent(HIS_SERVICE_EXCLUSIVE pair, ExclusiveRowValueADO value)
        {
            return pair.HANDLE_TYPE_ID != value.HandleTypeId
                || (pair.NOTE ?? "") != (NormalizeNote(value.Note) ?? "")
                || (pair.IS_ACTIVE ?? IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) != ToIsActiveValue(value);
        }

        /// <summary>The shared record panel (handle type / in use / note for every pair) is replaced by the per-row columns</summary>
        private void HideRecordPanel()
        {
            try
            {
                lciHandleType.Visibility = LayoutVisibility.Never;
                lciIsActive.Visibility = LayoutVisibility.Never;
                lciNote.Visibility = LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}

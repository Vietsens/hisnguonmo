using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.UC
{
    /// <summary>
    /// Helper: setup GridView hien thi danh sach benh dang doc (PARENT_TYPE = 3, 4, 5).
    /// 3 cot: STT | Ten benh (DISPLAY_NAME) | Co (IS_CHECKED)
    /// RepositoryItems tao 1 lan trong SetupGridView, reuse trong CustomRowCellEdit.
    /// </summary>
    public static class DiseaseDetailGridHelper
    {
        /// <summary>Luu tru RepositoryItems tao san cho moi gridView</summary>
        private class GridRepoStore
        {
            public RepositoryItemTextEdit RiTextOther { get; set; }
            public RepositoryItemCheckEdit RiCheck { get; set; }
            public RepositoryItemTextEdit RiEmpty { get; set; }
        }

        #region Setup Grid
        /// <summary>
        /// Setup 3 columns cho gridView. Goi 1 lan trong Init.
        /// Pre-create RepositoryItems de reuse — KHONG tao moi trong CustomRowCellEdit.
        /// </summary>
        public static void SetupGridView(GridView gridView)
        {
            try
            {
                if (gridView == null) return;

                gridView.BeginUpdate();
                gridView.Columns.Clear();

                // STT — KHONG cho sua
                var gcSTT = new GridColumn();
                gcSTT.Caption = "STT";
                gcSTT.FieldName = "STT";
                gcSTT.VisibleIndex = 0;
                gcSTT.Width = 40;
                gcSTT.OptionsColumn.AllowEdit = false;
                gcSTT.OptionsColumn.AllowFocus = false;
                gcSTT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gcSTT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gridView.Columns.Add(gcSTT);

                // Ten benh (DISPLAY_NAME)
                var gcName = new GridColumn();
                gcName.Caption = "Tên bệnh";
                gcName.FieldName = "DISPLAY_NAME";
                gcName.VisibleIndex = 1;
                gcName.Width = 300;
                gcName.OptionsColumn.AllowEdit = true;
                gridView.Columns.Add(gcName);

                // Co (IS_CHECKED)
                var gcCheck = new GridColumn();
                gcCheck.Caption = "Có";
                gcCheck.FieldName = "IS_CHECKED";
                gcCheck.VisibleIndex = 2;
                gcCheck.Width = 40;
                gcCheck.OptionsColumn.AllowEdit = true;
                gcCheck.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gcCheck.AppearanceHeader.Options.UseTextOptions = true;
                gcCheck.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gcCheck.AppearanceCell.Options.UseTextOptions = true;
                gridView.Columns.Add(gcCheck);

                // === Pre-create RepositoryItems 1 lan — reuse trong CustomRowCellEdit ===
                var store = new GridRepoStore();

                store.RiTextOther = new RepositoryItemTextEdit();
                store.RiTextOther.MaxLength = 500;

                store.RiCheck = new RepositoryItemCheckEdit();
                store.RiCheck.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

                store.RiEmpty = new RepositoryItemTextEdit();
                store.RiEmpty.ReadOnly = true;
                store.RiEmpty.NullText = "";
                store.RiEmpty.Appearance.BackColor = Color.White;
                store.RiEmpty.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

                if (gridView.GridControl != null)
                {
                    gridView.GridControl.RepositoryItems.Add(store.RiTextOther);
                    gridView.GridControl.RepositoryItems.Add(store.RiCheck);
                    gridView.GridControl.RepositoryItems.Add(store.RiEmpty);
                }

                gridView.Tag = store;

                // Grid options
                gridView.OptionsBehavior.Editable = true;
                gridView.OptionsBehavior.ReadOnly = false;
                gridView.OptionsView.ShowGroupPanel = false;
                gridView.OptionsView.ShowIndicator = false;
                gridView.OptionsView.ShowAutoFilterRow = false;
                gridView.OptionsView.ColumnAutoWidth = true;
                gridView.OptionsCustomization.AllowGroup = false;
                gridView.OptionsCustomization.AllowSort = false;
                gridView.OptionsCustomization.AllowColumnMoving = false;
                gridView.OptionsCustomization.AllowFilter = false;
                gridView.OptionsSelection.EnableAppearanceFocusedCell = false;

                // Events
                gridView.CustomRowCellEdit += GridView_CustomRowCellEdit;
                gridView.ShowingEditor += GridView_ShowingEditor;
                gridView.RowCellStyle += GridView_RowCellStyle;
                gridView.CellValueChanging += GridView_CellValueChanging;

                gridView.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Unsubscribe events khi dong form — goi tu ProcessDisposeModuleDataAfterClose.
        /// </summary>
        public static void DetachEvents(GridView gridView)
        {
            try
            {
                if (gridView == null) return;
                gridView.CustomRowCellEdit -= GridView_CustomRowCellEdit;
                gridView.ShowingEditor -= GridView_ShowingEditor;
                gridView.RowCellStyle -= GridView_RowCellStyle;
                gridView.CellValueChanging -= GridView_CellValueChanging;
                gridView.Tag = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Assign RepositoryItem da tao san — KHONG new object.
        /// </summary>
        private static void GridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return;
                var store = view.Tag as GridRepoStore;
                if (store == null) return;
                var row = view.GetRow(e.RowHandle) as ADO.DiseaseDetailGridADO;
                if (row == null) return;

                if (e.Column.FieldName == "DISPLAY_NAME" && row.HAS_OTHER)
                    e.RepositoryItem = store.RiTextOther;

                if (e.Column.FieldName == "IS_CHECKED")
                    e.RepositoryItem = row.HAS_CHECKBOX ? (RepositoryItem)store.RiCheck : store.RiEmpty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// CHI cho edit: DISPLAY_NAME khi IS_OTHER=1, IS_CHECKED khi HAS_CHECKBOX=true.
        /// </summary>
        private static void GridView_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return;
                var row = view.GetRow(view.FocusedRowHandle) as ADO.DiseaseDetailGridADO;
                if (row == null) { e.Cancel = true; return; }

                string fieldName = view.FocusedColumn != null ? view.FocusedColumn.FieldName : "";
                e.Cancel = true;

                if (fieldName == "DISPLAY_NAME" && row.HAS_OTHER)
                    e.Cancel = false;
                else if (fieldName == "IS_CHECKED" && row.HAS_CHECKBOX)
                    e.Cancel = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Chan xoa prefix "Name: " khi user sua cot DISPLAY_NAME.
        /// Chi fire khi user THUC SU edit — KHONG fire moi cell repaint.
        /// </summary>
        private static void GridView_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "DISPLAY_NAME") return;
                var view = sender as GridView;
                if (view == null) return;
                var row = view.GetRow(e.RowHandle) as ADO.DiseaseDetailGridADO;
                if (row == null || !row.HAS_OTHER) return;

                string prefix = GetPrefix(row);
                string newVal = (e.Value ?? "").ToString();
                if (!newVal.StartsWith(prefix))
                {
                    view.SetRowCellValue(e.RowHandle, e.Column, prefix);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dong khong co checkbox → o trang.
        /// </summary>
        private static void GridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return;
                var row = view.GetRow(e.RowHandle) as ADO.DiseaseDetailGridADO;
                if (row == null) return;

                if (e.Column.FieldName == "IS_CHECKED" && !row.HAS_CHECKBOX)
                {
                    e.Appearance.ForeColor = Color.White;
                    e.Appearance.BackColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static string GetPrefix(ADO.DiseaseDetailGridADO row)
        {
            string name = (row != null ? row.DISEASE_NAME : "") ?? "";
            return name.Trim() + ": ";
        }
        #endregion

        #region Load Data
        public static List<ADO.DiseaseDetailGridADO> LoadToGrid(
            GridControl gridControl,
            GridView gridView,
            List<V_HIS_DISEASE_DETAIL> allDetails,
            long parentType)
        {
            var adoList = new List<ADO.DiseaseDetailGridADO>();
            try
            {
                if (gridView == null || allDetails == null) return adoList;

                var filtered = allDetails
                    .Where(d => d.PARENT_TYPE == parentType)
                    .OrderBy(d => d.NUM_ORDER_TYPE)
                    .ThenBy(d => d.NUM_ORDER_DETAIL)
                    .ToList();

                int stt = 0;
                foreach (var detail in filtered)
                {
                    stt++;
                    string name = (detail.NAME ?? "").Trim();
                    bool hasOther = (detail.IS_OTHER ?? 0) == 1;
                    adoList.Add(new ADO.DiseaseDetailGridADO
                    {
                        DISEASE_DETAIL_ID = detail.ID,
                        STT = stt,
                        DISEASE_NAME = name,
                        DISPLAY_NAME = hasOther ? name + ": " : name,
                        IS_CHECKED = false,
                        OTHER_TEXT = "",
                        HAS_CHECKBOX = (detail.IS_CHECKBOX ?? 0) == 1,
                        HAS_OTHER = hasOther,
                        GROUP_NAME = detail.DISEASE_TYPE_NAME ?? "",
                        NUM_ORDER_TYPE = detail.NUM_ORDER_TYPE ?? 0,
                        NUM_ORDER_DETAIL = detail.NUM_ORDER_DETAIL ?? 0
                    });
                }

                gridView.BeginUpdate();
                gridControl.DataSource = adoList;
                gridView.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return adoList;
        }

        public static void ApplyResults(
            GridView gridView,
            List<ADO.DiseaseDetailGridADO> adoList,
            List<HIS_DISEASE_DETAIL_RESULT> results)
        {
            try
            {
                if (adoList == null || results == null) return;

                var resultDict = results
                    .Where(r => r.DISEASE_DETAIL_ID != null)
                    .GroupBy(r => r.DISEASE_DETAIL_ID.Value)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.ID).First());

                foreach (var ado in adoList)
                {
                    HIS_DISEASE_DETAIL_RESULT result;
                    if (resultDict.TryGetValue(ado.DISEASE_DETAIL_ID, out result))
                    {
                        if (ado.HAS_CHECKBOX)
                            ado.IS_CHECKED = (result.IS_CHECK ?? 0) == 1;
                        if (ado.HAS_OTHER && !string.IsNullOrEmpty(result.OTHER))
                        {
                            ado.OTHER_TEXT = result.OTHER;
                            ado.DISPLAY_NAME = GetPrefix(ado) + result.OTHER;
                        }
                    }
                }

                if (gridView != null)
                    gridView.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Collect Results
        public static List<ADO.DiseaseDetailResultADO> CollectResults(
            List<ADO.DiseaseDetailGridADO> adoList,
            long? kskGeneralId = null)
        {
            var rows = new List<ADO.DiseaseDetailResultADO>();
            try
            {
                if (adoList == null) return rows;

                foreach (var ado in adoList)
                {
                    if (!ado.HAS_CHECKBOX && !ado.HAS_OTHER) continue;

                    string otherText = null;
                    if (ado.HAS_OTHER)
                    {
                        string prefix = GetPrefix(ado);
                        string displayText = (ado.DISPLAY_NAME ?? "");
                        if (displayText.StartsWith(prefix) && displayText.Length > prefix.Length)
                            otherText = displayText.Substring(prefix.Length).Trim();
                        if (string.IsNullOrEmpty(otherText)) otherText = null;
                    }

                    rows.Add(new ADO.DiseaseDetailResultADO
                    {
                        DISEASE_DETAIL_ID = ado.DISEASE_DETAIL_ID,
                        IS_CHECK = ado.HAS_CHECKBOX && ado.IS_CHECKED ? 1 : 0,
                        OTHER = otherText,
                        KSK_GENERAL_ID = kskGeneralId
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rows;
        }

        public static void ResetAll(List<ADO.DiseaseDetailGridADO> adoList, GridView gridView)
        {
            try
            {
                if (adoList == null) return;
                foreach (var ado in adoList)
                {
                    ado.IS_CHECKED = false;
                    ado.OTHER_TEXT = "";
                    ado.DISPLAY_NAME = ado.HAS_OTHER ? GetPrefix(ado) : ado.DISEASE_NAME;
                }
                if (gridView != null) gridView.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}

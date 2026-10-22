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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Plugins.PublicMedicineMultiPatient.ADO;
using HIS.Desktop.Plugins.PublicMedicineMultiPatient.Base;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient
{
    public partial class frmPublicMedicineMultiPatient : HIS.Desktop.Utility.FormBase
    {
        List<TreatmentBedRoomADO> treatmentBedRoomADOs;

        /// <summary>
        /// Patient list = rows of the bed room screen filter without paging, sorted by bed room then name.
        /// The patient focused on the bed room screen is checked on open.
        /// </summary>
        private void LoadPatientList()
        {
            try
            {
                WaitingManager.Show();
                List<L_HIS_TREATMENT_BED_ROOM> data = null;
                if (this.treatmentBedRoomFilter != null)
                {
                    PublicMedicineDataLoader loader = new PublicMedicineDataLoader();
                    data = loader.GetTreatmentBedRooms(this.treatmentBedRoomFilter);
                    if (loader.HasError)
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(this, loader.Param, false);
                    }
                }
                else if (this.focusedTreatmentBedRoom != null)
                {
                    data = new List<L_HIS_TREATMENT_BED_ROOM> { this.focusedTreatmentBedRoom };
                }

                this.treatmentBedRoomADOs = (data ?? new List<L_HIS_TREATMENT_BED_ROOM>())
                    .Select(o => new TreatmentBedRoomADO(o))
                    .OrderBy(o => o.BED_ROOM_NAME)
                    .ThenBy(o => o.TDL_PATIENT_FIRST_NAME)
                    .ToList();

                if (this.focusedTreatmentBedRoom != null)
                {
                    TreatmentBedRoomADO focused = this.treatmentBedRoomADOs.FirstOrDefault(o => o.ID == this.focusedTreatmentBedRoom.ID)
                        ?? this.treatmentBedRoomADOs.FirstOrDefault(o => o.TREATMENT_ID == this.focusedTreatmentBedRoom.TREATMENT_ID);
                    if (focused != null)
                        focused.IsChecked = true;
                }

                this.gridViewPatient.BeginUpdate();
                try
                {
                    this.gridControlPatient.DataSource = this.treatmentBedRoomADOs;
                }
                finally
                {
                    this.gridViewPatient.EndUpdate();
                }
                UpdateSelectedCount();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateSelectedCount()
        {
            try
            {
                int total = this.treatmentBedRoomADOs != null ? this.treatmentBedRoomADOs.Count : 0;
                int selected = this.treatmentBedRoomADOs != null ? this.treatmentBedRoomADOs.Count(o => o.IsChecked) : 0;
                this.lblSelectedCount.Text = String.Format(GetLanguageValue("frmPublicMedicineMultiPatient.lblSelectedCount.Text"), selected, total);
                this.gridViewPatient.InvalidateColumnHeader(this.gcCheck);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Rows currently shown on the grid (after the find panel filter)
        /// </summary>
        private List<TreatmentBedRoomADO> GetVisibleRows()
        {
            List<TreatmentBedRoomADO> result = new List<TreatmentBedRoomADO>();
            for (int i = 0; i < this.gridViewPatient.RowCount; i++)
            {
                int rowHandle = this.gridViewPatient.GetVisibleRowHandle(i);
                if (!this.gridViewPatient.IsDataRow(rowHandle))
                    continue;
                TreatmentBedRoomADO row = this.gridViewPatient.GetRow(rowHandle) as TreatmentBedRoomADO;
                if (row != null)
                    result.Add(row);
            }
            return result;
        }

        private bool IsAllVisibleChecked()
        {
            List<TreatmentBedRoomADO> visibleRows = GetVisibleRows();
            return visibleRows.Count > 0 && visibleRows.All(o => o.IsChecked);
        }

        /// <summary>
        /// Header check box: checks every visible row, or unchecks them all when they are already checked
        /// </summary>
        private void ToggleAllVisibleRows()
        {
            List<TreatmentBedRoomADO> visibleRows = GetVisibleRows();
            bool isChecked = !(visibleRows.Count > 0 && visibleRows.All(o => o.IsChecked));
            foreach (TreatmentBedRoomADO row in visibleRows)
            {
                row.IsChecked = isChecked;
            }
            this.gridViewPatient.RefreshData();
            UpdateSelectedCount();
        }

        private void ToggleRow(int rowHandle)
        {
            if (!this.gridViewPatient.IsDataRow(rowHandle))
                return;
            TreatmentBedRoomADO row = this.gridViewPatient.GetRow(rowHandle) as TreatmentBedRoomADO;
            if (row == null)
                return;
            row.IsChecked = !row.IsChecked;
            this.gridViewPatient.RefreshRow(rowHandle);
            UpdateSelectedCount();
        }

        private void gridViewPatient_Click(object sender, EventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null)
                    return;
                Point point = view.GridControl.PointToClient(Control.MousePosition);
                GridHitInfo hitInfo = view.CalcHitInfo(point);
                if (hitInfo.Column != this.gcCheck)
                    return;
                if (hitInfo.InColumn)
                {
                    ToggleAllVisibleRows();
                }
                else if (hitInfo.InRowCell)
                {
                    ToggleRow(hitInfo.RowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatient_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    ToggleRow(this.gridViewPatient.FocusedRowHandle);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Find panel: the whole typed phrase (not word by word) inside treatment code, patient code, patient name or bed,
        /// ignoring case and Vietnamese diacritics. Bed room, gender, date of birth are not searched.
        /// </summary>
        private void gridViewPatient_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            try
            {
                string key = NormalizeSearchText((this.gridViewPatient.FindFilterText ?? "").Trim().Trim('"'));
                if (String.IsNullOrEmpty(key) || this.treatmentBedRoomADOs == null || e.ListSourceRow < 0 || e.ListSourceRow >= this.treatmentBedRoomADOs.Count)
                    return;
                TreatmentBedRoomADO row = this.treatmentBedRoomADOs[e.ListSourceRow];
                e.Visible = row != null
                    && (NormalizeSearchText(row.TREATMENT_CODE).Contains(key)
                        || NormalizeSearchText(row.TDL_PATIENT_CODE).Contains(key)
                        || NormalizeSearchText(row.TDL_PATIENT_NAME).Contains(key)
                        || NormalizeSearchText(row.BED_NAME).Contains(key));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lower case, đ to d, diacritics removed, runs of white space collapsed to one space
        /// </summary>
        private static string NormalizeSearchText(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                return "";
            string decomposed = text.Trim().ToLower().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(decomposed.Length);
            bool lastSpace = false;
            foreach (char c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                    continue;
                if (Char.IsWhiteSpace(c))
                {
                    if (!lastSpace)
                        sb.Append(' ');
                    lastSpace = true;
                    continue;
                }
                lastSpace = false;
                sb.Append(c);
            }
            return sb.ToString();
        }

        private void gridViewPatient_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                this.gridViewPatient.InvalidateColumnHeader(this.gcCheck);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatient_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null || e.Column != this.gcCheck)
                    return;
                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);
                DrawCheckBox(e.Graphics, e.Bounds, IsAllVisibleChecked());
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DrawCheckBox(Graphics g, Rectangle r, bool isChecked)
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info = this.repositoryItemCheckEditPatient.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter = this.repositoryItemCheckEditPatient.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = isChecked;
            info.Bounds = r;
            info.CalcViewInfo(g);
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, new DevExpress.Utils.Drawing.GraphicsCache(g), r);
            painter.Draw(args);
            args.Cache.Dispose();
        }

        private void gridViewPatient_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Checked patients in grid order, one row per treatment
        /// </summary>
        private List<TreatmentBedRoomADO> GetCheckedTreatments()
        {
            List<TreatmentBedRoomADO> result = new List<TreatmentBedRoomADO>();
            if (this.treatmentBedRoomADOs == null)
                return result;
            HashSet<long> treatmentIds = new HashSet<long>();
            foreach (TreatmentBedRoomADO row in this.treatmentBedRoomADOs.Where(o => o.IsChecked))
            {
                if (treatmentIds.Add(row.TREATMENT_ID))
                    result.Add(row);
            }
            return result;
        }
    }
}

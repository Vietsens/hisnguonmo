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
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CashCollect
{
    /// <summary>
    /// "Cashier room" filter (multi-select, same behaviour as "Account book") - task 55505.
    /// Filtered on the backend through HisTransactionViewFilter.CASHIER_ROOM_IDs; nothing selected = all rooms.
    /// </summary>
    public partial class UCCashCollect : HIS.Desktop.Utility.UserControlBase
    {
        List<V_HIS_CASHIER_ROOM> cashierRoomCollection;
        List<V_HIS_CASHIER_ROOM> CashierRoomSelecteds = new List<V_HIS_CASHIER_ROOM>();

        private void InitCashierRoomCheck()
        {
            try
            {
                GridCheckMarksSelection1 gridCheck = new GridCheckMarksSelection1(cboCashierRoom.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection1.SelectionChangedEventHandler(SelectionGrid__CashierRoom);
                cboCashierRoom.Properties.Tag = gridCheck;
                cboCashierRoom.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboCashierRoom()
        {
            try
            {
                cashierRoomCollection = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>();
                cashierRoomCollection = cashierRoomCollection != null
                    ? cashierRoomCollection.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.CASHIER_ROOM_CODE).ToList()
                    : new List<V_HIS_CASHIER_ROOM>();

                cboCashierRoom.Properties.DataSource = cashierRoomCollection;
                cboCashierRoom.Properties.DisplayMember = "CASHIER_ROOM_NAME";
                cboCashierRoom.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn colCode = cboCashierRoom.Properties.View.Columns.AddField("CASHIER_ROOM_CODE");
                colCode.VisibleIndex = 1;
                colCode.Width = 80;
                colCode.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn colName = cboCashierRoom.Properties.View.Columns.AddField("CASHIER_ROOM_NAME");
                colName.VisibleIndex = 2;
                colName.Width = 220;
                colName.Caption = "";
                cboCashierRoom.Properties.PopupFormWidth = 320;
                cboCashierRoom.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboCashierRoom.Properties.View.OptionsSelection.MultiSelect = true;
                ClearCashierRoomSelection();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__CashierRoom(object sender, EventArgs e)
        {
            try
            {
                CashierRoomSelecteds = new List<V_HIS_CASHIER_ROOM>();
                foreach (V_HIS_CASHIER_ROOM rv in (sender as GridCheckMarksSelection1).Selection)
                {
                    if (rv != null)
                        CashierRoomSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>IDs of the selected cashier rooms; null = do not filter by room.</summary>
        private List<long> GetSelectedCashierRoomIds()
        {
            List<long> result = null;
            try
            {
                if (CashierRoomSelecteds != null && CashierRoomSelecteds.Count > 0)
                {
                    result = CashierRoomSelecteds.Select(o => o.ID).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ClearCashierRoomSelection()
        {
            try
            {
                GridCheckMarksSelection1 gridCheckMark = cboCashierRoom.Properties.Tag as GridCheckMarksSelection1;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboCashierRoom.Properties.View);
                }
                cboCashierRoom.EditValue = null;
                cboCashierRoom.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCashierRoom_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                GridCheckMarksSelection1 gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as GridCheckMarksSelection1 : (sender as RepositoryItemSearchLookUpEdit).Tag as GridCheckMarksSelection1;
                if (gridCheckMark == null) return;
                List<string> names = new List<string>();
                foreach (V_HIS_CASHIER_ROOM rv in gridCheckMark.Selection)
                {
                    if (rv != null) names.Add(rv.CASHIER_ROOM_NAME);
                }
                e.DisplayText = String.Join(", ", names);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCashierRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    btnFind.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCashierRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && CashierRoomSelecteds != null && CashierRoomSelecteds.Count > 0)
                {
                    btnFind.Focus();
                }
                else
                {
                    cboCashierRoom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

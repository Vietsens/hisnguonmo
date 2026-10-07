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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.CashCollect
{
    /// <summary>
    /// Search box of the multi-select combos (Phòng thu ngân, Sổ thu chi) - task 55505.
    /// The DevExpress 15.2 find panel ORs the typed words, so typing more words never narrowed the list
    /// (tester: "chỉ tìm được từ đầu tiên"). A row now matches when its code + name contains every typed word,
    /// case and diacritic insensitive.
    /// </summary>
    public partial class UCCashCollect : HIS.Desktop.Utility.UserControlBase
    {
        private void CashierRoomView_CustomRowFilter(object sender, RowFilterEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null || String.IsNullOrWhiteSpace(view.FindFilterText)) return;
                if (cashierRoomCollection == null || e.ListSourceRow < 0 || e.ListSourceRow >= cashierRoomCollection.Count) return;

                V_HIS_CASHIER_ROOM room = cashierRoomCollection[e.ListSourceRow];
                e.Visible = ContainsAllWords(room.CASHIER_ROOM_CODE + " " + room.CASHIER_ROOM_NAME, view.FindFilterText);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BookCollectionView_CustomRowFilter(object sender, RowFilterEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null || String.IsNullOrWhiteSpace(view.FindFilterText)) return;
                if (bookCollection == null || e.ListSourceRow < 0 || e.ListSourceRow >= bookCollection.Count) return;

                HIS_ACCOUNT_BOOK book = bookCollection[e.ListSourceRow];
                e.Visible = ContainsAllWords(book.ACCOUNT_BOOK_CODE + " " + book.ACCOUNT_BOOK_NAME, view.FindFilterText);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>True when the text contains every word of the keyword (case and Vietnamese diacritic insensitive).</summary>
        internal static bool ContainsAllWords(string text, string keyword)
        {
            string source = Inventec.Common.String.Convert.UnSignVNese((text ?? "").ToLower());
            string[] words = Inventec.Common.String.Convert.UnSignVNese((keyword ?? "").ToLower()).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return words.All(word => source.Contains(word));
        }
    }
}

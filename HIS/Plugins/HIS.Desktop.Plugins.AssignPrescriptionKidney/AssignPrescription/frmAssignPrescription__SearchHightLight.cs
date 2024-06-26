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
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections;
using System.Drawing;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {

        #region HightLight result while search medi maty
        private string GetStringWithoutQuotes(string findText)
        {
            string stringWithoutQuotes = findText.ToLower().Replace("\"", string.Empty);
            return stringWithoutQuotes;
        }

        private int FindSubStringStartPosition(string dispalyText, string findText)
        {
            string stringWithoutQuotes = GetStringWithoutQuotes(findText);
            int index = dispalyText.ToLower().IndexOf(stringWithoutQuotes);
            return index;
        }

        private bool HiglightSubString(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e, string findText)
        {
            int index = FindSubStringStartPosition(e.DisplayText, findText);
            if (index == -1)
            {
                return false;
            }

            e.Appearance.FillRectangle(e.Cache, e.Bounds);
            e.Cache.Paint.DrawMultiColorString(e.Cache, e.Bounds, e.DisplayText, GetStringWithoutQuotes(findText),
                e.Appearance, Color.Indigo, Color.Gold, true, index);
            return true;
        }

        private void OnCustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.OptionsFind.HighlightFindResults && !txtMediMatyForPrescription.Text.Equals(string.Empty))
            {
                CriteriaOperator op = ConvertFindPanelTextToCriteriaOperator(txtMediMatyForPrescription.Text, view, false);
                if (op is GroupOperator)
                {
                    string findText = txtMediMatyForPrescription.Text;
                    if (HiglightSubString(e, findText))
                        e.Handled = true;
                }
                else if (op is FunctionOperator)
                {
                    FunctionOperator func = op as FunctionOperator;
                    CriteriaOperator colNameOperator = func.Operands[0];
                    string colName = colNameOperator.LegacyToString().Replace("[", string.Empty).Replace("]", string.Empty);
                    if (!e.Column.FieldName.StartsWith(colName)) return;

                    CriteriaOperator valueOperator = func.Operands[1];
                    string findText = valueOperator.LegacyToString().ToLower().Replace("'", "");
                    if (HiglightSubString(e, findText))
                        e.Handled = true;
                }

            }
        }

        public static CriteriaOperator ConvertFindPanelTextToCriteriaOperator(string findPanelText, GridView view, bool applyPrefixes)
        {
            if (!string.IsNullOrEmpty(findPanelText))
            {
                FindSearchParserResults parseResult = new FindSearchParser().Parse(findPanelText, GetFindToColumnsCollection(view));
                if (applyPrefixes)
                    parseResult.AppendColumnFieldPrefixes();

                return DxFtsContainsHelperAlt.Create(parseResult, FilterCondition.Contains, false);
            }
            return null;
        }

        private static ICollection GetFindToColumnsCollection(GridView view)
        {
            System.Reflection.MethodInfo mi = typeof(ColumnView).GetMethod("GetFindToColumnsCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return mi.Invoke(view, null) as ICollection;
        }
        #endregion

    }
}

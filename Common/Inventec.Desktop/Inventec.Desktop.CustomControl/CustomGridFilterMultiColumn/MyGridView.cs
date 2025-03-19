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

using DevExpress.Data;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using System.Reflection;
using DevExpress.XtraGrid.Columns;
using System.Collections.Generic;
using System.Text;
using DevExpress.Data.Filtering;
using System.Globalization;

namespace Inventec.Desktop.CustomControl
{
    public class MyGridView
        : GridView
    {
        internal static readonly string ViewNameValue = typeof(MyGridView).Name;

        public MyGridView(GridControl ownerGrid)
            : base(ownerGrid)
        {
            this.CustomUnboundColumnData += MyGridView_CustomUnboundColumnData;
        }

        public MyGridView()
        {
            this.CustomUnboundColumnData += MyGridView_CustomUnboundColumnData;
        }

        protected override string ViewName
        {
            get { return ViewNameValue; }
        }

        public event EventHandler<RowErrorEventArgs> CustomRowError;
        public event EventHandler<RowColumnErrorEventArgs> CustomRowColumnError;

        protected override BaseGridController CreateDataController()
        {
            if (requireDataControllerType == DataControllerType.AsyncServerMode)
            {
                return new AsyncServerModeDataController();
            }
            if (requireDataControllerType == DataControllerType.ServerMode)
            {
                return new ServerModeDataController();
            }
            if (requireDataControllerType == DataControllerType.RegularNoCurrencyManager)
            {
                return new MyGridDataController(this);
            }
            return new MyCurrencyDataController(this);
        }

        protected internal virtual void FillRowError(int handle, ErrorInfo errorInfo)
        {
            var handler = CustomRowError;
            if (handler != null)
            {
                handler(this, new RowErrorEventArgs(errorInfo, handle));
            }
        }

        protected internal virtual void FillRowColumnError(int handle, string column, ErrorInfo errorInfo)
        {
            var handler = CustomRowColumnError;
            if (handler != null)
            {
                handler(this, new RowColumnErrorEventArgs(errorInfo, handle, column));
            }
        }


        void MyGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {


                if (!String.IsNullOrWhiteSpace(e.Column.FieldName) && e.Column.FieldName.Contains("Unb"))
                {
                    string field = e.Column.FieldName.Substring(0, e.Column.FieldName.IndexOf("Unb"));
                    var nv = e.Row;
                    Type type = nv.GetType();
                    PropertyInfo[] pi = type.GetProperties();
                    foreach (var ipi in pi)
                    {
                        if (ipi.Name == field && ipi.Name != "IsChecked")
                        {
                            var val = ipi.GetValue(nv);// (e.Row as nhanvien).tennv.ToString();
                            if (val != null)
                            {
                                string processedString = MyGridView.RemoveDiacritics(val.ToString(), true);
                                e.Value = val + processedString;
                                //Inventec.Common.Logging.LogSystem.Info(val + processedString);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected override void RefreshVisibleColumnsList()
        {
            base.RefreshVisibleColumnsList();
            // add required unbound columns
            foreach (GridColumn column in VisibleColumns)
            {
                string name = column.FieldName + "Unb";
                GridColumn col = Columns.ColumnByFieldName(name);
                if (col != null) continue;
                GridColumn unboundCol = Columns.AddField(column.FieldName + "Unb");
                unboundCol.UnboundType = DevExpress.Data.UnboundColumnType.String;
                column.FieldNameSortGroup = unboundCol.FieldName;
                column.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            }
        }

        public static IEnumerable<char> RemoveDiacriticsEnum(string src, bool compatNorm, Func<char, char> customFolding)
        {
            foreach (char c in src.Normalize(compatNorm ? NormalizationForm.FormKD : NormalizationForm.FormD))
            {
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.NonSpacingMark:
                    case UnicodeCategory.SpacingCombiningMark:
                    case UnicodeCategory.EnclosingMark:
                        //do nothing
                        break;
                    default:

                        yield return customFolding(c);
                        break;
                }
            }
        }
        public static IEnumerable<char> RemoveDiacriticsEnum(string src, bool compatNorm)
        {
            return RemoveDiacritics(src, compatNorm, c => c);
        }
        public static string RemoveDiacritics(string src, bool compatNorm, Func<char, char> customFolding)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in RemoveDiacriticsEnum(src, compatNorm, customFolding))
                sb.Append(c);
            return sb.ToString().Replace('Đ','D').Replace('đ','d');
        }
        public static string RemoveDiacritics(string src, bool compatNorm)
        {
            return RemoveDiacritics(src, compatNorm, c => c);
        }

        protected override ColumnFilterInfo CreateFilterRowInfo(GridColumn column, object _value)
        {
            string strVal = _value == null ? null : _value.ToString();
            if (_value == null || strVal == string.Empty) return ColumnFilterInfo.Empty;
            strVal = MyGridView.RemoveDiacritics(strVal, true);
            AutoFilterCondition condition = ResolveAutoFilterCondition(column);
            CriteriaOperator op = CreateAutoFilterCriterion(column, condition, _value, strVal);
            return new ColumnFilterInfo(ColumnFilterType.AutoFilter, _value, op, string.Empty);
        }
    }
}

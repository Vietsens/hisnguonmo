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
using DevExpress.Spreadsheet;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ExcelImport
{
    internal class Coordinate
    {
        public int row;
        public int column;
        public CellValue cellValue;
        public string Key;
        //public int columnMergeCount;

        public Coordinate(int rIndex, int cIndex, CellValue value)
        {
            row = rIndex;
            column = cIndex;
            cellValue = value;
            Key = SetKey(value);
        }

        internal static string SetKey(CellValue value)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(Tag.IMPORT_TAG))
                {
                    string startWrapKey = "{";
                    string endWrapKey = "}";
                    string[] arrPiece = System.Text.RegularExpressions.Regex.Split(value.TextValue, Tag.IMPORT_TAG);
                    if (arrPiece != null && arrPiece.Count() > 0)
                    {
                        for (int i = 0, count = arrPiece.Count(); i < count; i++)
                        {
                            if (arrPiece[i].Contains(startWrapKey))
                            {
                                int start = arrPiece[i].IndexOf(startWrapKey) + 1; //-1 de lay ca dau .
                                int end = arrPiece[i].LastIndexOf(endWrapKey) + 1;
                                result = arrPiece[i].Substring(start, end - (start + 1));
                            }
                            else
                            {
                                result += arrPiece[i];
                            }
                        }
                    }
                }
                else
                {
                    result = value.TextValue;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = value.TextValue; //Khong remove duoc, giu nguyen goc
            }
            return result;
        }

        internal static Coordinate GetFirstCoordinate(List<Coordinate> listCoordinate)
        {
            try
            {
                Coordinate result = new Coordinate(999999, 999999, "");
                bool found = false;
                foreach (Coordinate ec in listCoordinate)
                {
                    if ((ec.row < result.row) || ((ec.row == result.row) && (ec.column < result.column)))
                    {
                        result = ec;
                        found = true;
                    }
                }
                if (found)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        internal static Coordinate GetLastCoordinate(List<Coordinate> listCoordinate)
        {
            try
            {
                Coordinate result = new Coordinate(0, 0, "");
                bool found = false;
                foreach (Coordinate ec in listCoordinate)
                {
                    if ((ec.row > result.row) || ((ec.row == result.row) && (ec.column > result.column)))
                    {
                        result = ec;
                        found = true;
                    }
                }
                if (found)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        internal static void SetRowIndex(List<Coordinate> listCoordinate, int row)
        {
            try
            {
                foreach (var item in listCoordinate)
                {
                    item.row = row;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void SetColumnIndex(List<Coordinate> listCoordinate, int column)
        {
            try
            {
                foreach (var item in listCoordinate)
                {
                    item.column = column;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void ChangeRowIndex(List<Coordinate> listCoordinate, int from, int count)
        {
            try
            {
                if (listCoordinate != null)
                {
                    List<Coordinate> listUpdate = listCoordinate.Where(o => o.row >= from).ToList();
                    if (listUpdate != null)
                    {
                        foreach (var item in listUpdate)
                        {
                            item.row += count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void ChangeColumnIndex(List<Coordinate> listCoordinate, int from, int count)
        {
            try
            {
                if (listCoordinate != null)
                {
                    List<Coordinate> listUpdate = listCoordinate.Where(o => o.column >= from).ToList();
                    if (listUpdate != null)
                    {
                        foreach (var item in listUpdate)
                        {
                            item.column += count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void ChangeRowIndex(Dictionary<string, Coordinate> dictionary, int from, int count)
        {
            try
            {
                if (dictionary != null && dictionary.Count > 0)
                {
                    List<Coordinate> listCoordinate = dictionary.Values.ToList();
                    if (listCoordinate != null)
                    {
                        List<Coordinate> listUpdate = listCoordinate.Where(o => o.row >= from).ToList();
                        if (listUpdate != null)
                        {
                            foreach (var item in listUpdate)
                            {
                                item.row += count;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void ChangeColumnIndex(Dictionary<string, Coordinate> dictionary, int from, int count)
        {
            try
            {
                if (dictionary != null && dictionary.Count > 0)
                {
                    List<Coordinate> listCoordinate = dictionary.Values.ToList();
                    if (listCoordinate != null)
                    {
                        List<Coordinate> listUpdate = listCoordinate.Where(o => o.row >= from).ToList();
                        if (listUpdate != null)
                        {
                            foreach (var item in listUpdate)
                            {
                                item.column += count;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}

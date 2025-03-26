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
using DevExpress.XtraSpreadsheet;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ExcelImport
{
    public class Import
    {
        public string FilePath { get; set; }
        internal List<Coordinate> listAllTag = new List<Coordinate>();
        internal List<String> listKey = new List<string>();
        public DevExpress.Spreadsheet.Worksheet workSheet;
        DevExpress.Spreadsheet.WorksheetCollection worksheets;
        private double RowHeightUnit { get; set; }

        public Import()
        {

        }

       
        public bool ReadFileExcel(string path)
        {
            bool result = false;
            try
            {
                FilePath = path;
                byte[] byteArr = File.ReadAllBytes(FilePath);
                if (byteArr.Length > 0)
                {
                    using (MemoryStream Stream = new MemoryStream())
                    {
                        Stream.Write(byteArr, 0, (int)byteArr.Length);
                        Stream.Position = 0;
                        SpreadsheetControl spreadSheetControl = new SpreadsheetControl();
                        spreadSheetControl.AllowDrop = false;
                        spreadSheetControl.LoadDocument(Stream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                        DevExpress.Spreadsheet.IWorkbook workBook = spreadSheetControl.Document;
                        workBook.BeginUpdate();
                        workBook.Unit = DevExpress.Office.DocumentUnit.Document;
                        worksheets = workBook.Worksheets;
                        //workSheet = worksheets[nameSheet];//spreadSheetControl.ActiveWorksheet;
                        //RowHeightUnit = workSheet.Cells[0, 0].RowHeight;
                    }
                    result = true;
                    //if (workSheet != null && RowHeightUnit > 0)
                    //{
                    //    if (CollectTagImport())
                    //    {
                    //        result = true;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public List<T> Get<T>(int index)
        {
            List<T> result = new List<T>();
            try
            {
                if (index <= worksheets.Count)
                {
                    workSheet = worksheets[index];
                    RowHeightUnit = workSheet.Cells[0, 0].RowHeight;
                    if (workSheet != null && RowHeightUnit > 0)
                    {
                        if (CollectTagImport() && CheckIndex())
                        {
                            for (int i = listAllTag[0].row + 1; i <= workSheet.Rows.LastUsedIndex; i++)
                            {
                                //int index = listAllTag[0].column;
                                Type type = typeof(T);
                                var data = (T)Activator.CreateInstance(type);
                                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get(type);

                                foreach (var item in listAllTag)
                                {
                                    foreach (var ipi in pi)
                                    {
                                        if (ipi.Name == item.Key)
                                        {
                                            CellValue c = workSheet.GetCellValue(item.column, i);
                                            if (c != null)
                                            {
                                                if (ipi.PropertyType.Equals(typeof(short)) || ipi.PropertyType.Equals(typeof(short?)))
                                                {
                                                    if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                    {
                                                        ipi.SetValue(data, null);
                                                    }
                                                    else
                                                    {
                                                        ipi.SetValue(data, (c.TextValue != null) ? short.Parse(c.TextValue) : (short)(c.NumericValue));
                                                    }
                                                }
                                                else if (ipi.PropertyType.Equals(typeof(long)) || ipi.PropertyType.Equals(typeof(long?)))
                                                {
                                                    if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                    {
                                                        ipi.SetValue(data, null);
                                                    }
                                                    else
                                                    {
                                                        ipi.SetValue(data, (c.TextValue != null) ? long.Parse(c.TextValue) : (long)(c.NumericValue));
                                                    }
                                                }
                                                else if (ipi.PropertyType.Equals(typeof(decimal)) || ipi.PropertyType.Equals(typeof(decimal?)))
                                                {
                                                    if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                    {
                                                        ipi.SetValue(data, null);
                                                    }
                                                    else
                                                    {
                                                        ipi.SetValue(data, (c.TextValue != null) ? decimal.Parse(c.TextValue) : (decimal)(c.NumericValue));
                                                    }
                                                }
                                                else if (ipi.PropertyType.Equals(typeof(int)) || ipi.PropertyType.Equals(typeof(int?)))
                                                {
                                                    if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                    {
                                                        ipi.SetValue(data, null);
                                                    }
                                                    else
                                                    {
                                                        ipi.SetValue(data, (c.TextValue != null) ? int.Parse(c.TextValue) : (int)(c.NumericValue));
                                                    }
                                                }
                                                else
                                                {
                                                    ipi.SetValue(data, (c.IsText) ? c.TextValue : ((c.IsNumeric) ? c.NumericValue.ToString() : ""));
                                                }
                                                //index += 1;
                                            }
                                        }
                                    }
                                }
                                result.Add(data);
                            }
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workSheet), workSheet));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public List<T> GetWithCheck<T>(int index)
        {
            List<T> result = new List<T>();
            try
            {
                if (index <= worksheets.Count)
                {
                    workSheet = worksheets[index];
                    RowHeightUnit = workSheet.Cells[0, 0].RowHeight;
                    if (workSheet != null && RowHeightUnit > 0)
                    {
                        if (CollectTagImport() && CheckIndex())
                        {
                            for (int i = listAllTag[0].row + 1; i <= workSheet.Rows.LastUsedIndex; i++)
                            {
                                try
                                {
                                    //int index = listAllTag[0].column;
                                    Type type = typeof(T);
                                    var data = (T)Activator.CreateInstance(type);
                                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get(type);

                                    foreach (var item in listAllTag)
                                    {
                                        try
                                        {
                                            foreach (var ipi in pi)
                                            {
                                                try
                                                {
                                                    if (ipi.Name == item.Key)
                                                    {
                                                        CellValue c = workSheet.GetCellValue(item.column, i);
                                                        if (c != null)
                                                        {
                                                            if (ipi.PropertyType.Equals(typeof(short)) || ipi.PropertyType.Equals(typeof(short?)))
                                                            {
                                                                if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                                {
                                                                    ipi.SetValue(data, null);
                                                                }
                                                                else
                                                                {
                                                                    ipi.SetValue(data, (c.TextValue != null) ? short.Parse(c.TextValue) : (short)(c.NumericValue));
                                                                }
                                                            }
                                                            else if (ipi.PropertyType.Equals(typeof(long)) || ipi.PropertyType.Equals(typeof(long?)))
                                                            {
                                                                if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                                {
                                                                    ipi.SetValue(data, null);
                                                                }
                                                                else
                                                                {
                                                                    ipi.SetValue(data, (c.TextValue != null) ? long.Parse(c.TextValue) : (long)(c.NumericValue));
                                                                }
                                                            }
                                                            else if (ipi.PropertyType.Equals(typeof(decimal)) || ipi.PropertyType.Equals(typeof(decimal?)))
                                                            {
                                                                if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                                {
                                                                    ipi.SetValue(data, null);
                                                                }
                                                                else
                                                                {
                                                                    ipi.SetValue(data, (c.TextValue != null) ? decimal.Parse(c.TextValue) : (decimal)(c.NumericValue));
                                                                }
                                                            }
                                                            else if (ipi.PropertyType.Equals(typeof(int)) || ipi.PropertyType.Equals(typeof(int?)))
                                                            {
                                                                if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                                {
                                                                    ipi.SetValue(data, null);
                                                                }
                                                                else
                                                                {
                                                                    ipi.SetValue(data, (c.TextValue != null) ? int.Parse(c.TextValue) : (int)(c.NumericValue));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ipi.SetValue(data, (c.IsText) ? c.TextValue : ((c.IsNumeric) ? c.NumericValue.ToString() : ""));
                                                            }
                                                            //index += 1;
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Inventec.Common.Logging.LogSystem.Warn(ex);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Inventec.Common.Logging.LogSystem.Warn(ex);
                                        }


                                    }
                                    result.Add(data);
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn(ex);
                                }


                            }
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workSheet), workSheet));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public List<T> GetPlus<T>()
        {
            List<T> result = new List<T>();
            try
            {
                workSheet = worksheets[0];
                RowHeightUnit = workSheet.Cells[0, 0].RowHeight;
                if (workSheet != null && RowHeightUnit > 0)
                {
                    if (CollectTagImport() && CheckIndex())
                    {
                        for (int i = listAllTag[0].row + 1; i <= workSheet.Rows.LastUsedIndex; i++)
                        {
                            //int index = listAllTag[0].column;
                            Type type = typeof(T);
                            var data = (T)Activator.CreateInstance(type);
                            System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get(type);

                            foreach (var item in listAllTag)
                            {
                                foreach (var ipi in pi)
                                {
                                    if (ipi.Name == item.Key)
                                    {
                                        CellValue c = workSheet.GetCellValue(item.column, i);
                                        if (c != null)
                                        {
                                            if (c.IsDateTime && (ipi.PropertyType.Equals(typeof(DateTime)) || ipi.PropertyType.Equals(typeof(DateTime?))))
                                            {
                                                ipi.SetValue(data, c.DateTimeValue);
                                            }
                                            else if (ipi.PropertyType.Equals(typeof(DateTime)) || ipi.PropertyType.Equals(typeof(DateTime?)))
                                            {
                                                if (!String.IsNullOrEmpty(c.TextValue))
                                                {
                                                    ipi.SetValue(data, DateTime.ParseExact(c.TextValue, "o", System.Globalization.CultureInfo.InvariantCulture));
                                                }
                                            }
                                            else if (ipi.PropertyType.Equals(typeof(short)) || ipi.PropertyType.Equals(typeof(short?)))
                                            {
                                                if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                {
                                                    ipi.SetValue(data, null);
                                                }
                                                else
                                                {
                                                    ipi.SetValue(data, (c.TextValue != null) ? short.Parse(c.TextValue) : (short)(c.NumericValue));
                                                }
                                            }
                                            else if (ipi.PropertyType.Equals(typeof(long)) || ipi.PropertyType.Equals(typeof(long?)))
                                            {
                                                if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                {
                                                    ipi.SetValue(data, null);
                                                }
                                                else
                                                {
                                                    ipi.SetValue(data, (c.TextValue != null) ? long.Parse(c.TextValue) : (long)(c.NumericValue));
                                                }
                                            }
                                            else if (ipi.PropertyType.Equals(typeof(decimal)) || ipi.PropertyType.Equals(typeof(decimal?)))
                                            {
                                                if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                {
                                                    ipi.SetValue(data, null);
                                                }
                                                else
                                                {
                                                    ipi.SetValue(data, (c.TextValue != null) ? decimal.Parse(c.TextValue) : (decimal)(c.NumericValue));
                                                }
                                            }
                                            else if (ipi.PropertyType.Equals(typeof(int)) || ipi.PropertyType.Equals(typeof(int?)))
                                            {
                                                if (c.IsEmpty && ipi.PropertyType.IsGenericType && ipi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                {
                                                    ipi.SetValue(data, null);
                                                }
                                                else
                                                {
                                                    ipi.SetValue(data, (c.TextValue != null) ? int.Parse(c.TextValue) : (int)(c.NumericValue));
                                                }
                                            }
                                            else
                                            {
                                                ipi.SetValue(data, (c.IsText) ? c.TextValue : ((c.IsNumeric) ? c.NumericValue.ToString() : ""));
                                            }
                                            //index += 1;
                                        }
                                    }
                                }
                            }
                            result.Add(data);
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workSheet), workSheet));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        //

        private bool CollectTagImport()
        {
            bool result = false;
            try
            {
                if (workSheet != null)
                {
                    for (int r = 0, rCount = workSheet.Rows.LastUsedIndex; r <= rCount; r++)
                    {
                        if (listAllTag.Count > 0)
                        {
                            break;
                        }
                        for (int c = 0, cCount = workSheet.Columns.LastUsedIndex; c <= cCount; c++)
                        {
                            CellValue cellValue = workSheet.GetCellValue(c, r);
                            if (cellValue != null && !cellValue.IsEmpty && cellValue.IsText && cellValue.TextValue.Contains(Tag.IMPORT_TAG))
                            {
                                Coordinate coor = new Coordinate(r, c, cellValue);
                                listAllTag.Add(coor);
                            }
                        }
                    }
                    if (listAllTag != null && listAllTag.Count == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong quet duoc tag nao trong file template. TAG=" + Tag.TAG + ".");
                    }
                    result = true;
                }
                else
                {
                    LogSystem.Warn("workSheet = null --> Khong xu ly quet tag duoc. Kiem tra lai khau doc template de khoi tao workSheet truoc do co the chua thanh cong.");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
                listAllTag.Clear();
            }
            return result;
        }

        private bool CheckIndex()
        {
            bool result = false;
            try
            {
                if (listAllTag.Count > 1)
                {
                    for (int i = 0; i < listAllTag.Count-1; i++)
                    {
                        for (int j = i+1; j < listAllTag.Count; j++)
                        {
                            if (listAllTag[i].row != listAllTag[j].row)
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

    }
}

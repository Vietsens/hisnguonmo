/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Phase 3 — Read cell values + named ranges from a FlexCel-rendered workbook.
 *
 * After FlexCel renders the Excel template (placeholders replaced, formulas evaluated by
 * FlexCel's Recalc), the JSON renderer can call these helpers to extract values that
 * couldn't be obtained from singleValueDictionary alone — e.g. =SUM ranges, =VLOOKUP,
 * conditional cells. All FlexCel access is wrapped in try-catch so a missing API or a
 * non-matching cell silently fails the lookup and lets the pipe fallback try another option.
 */
using System;
using FlexCel.XlsAdapter;

namespace Inventec.Common.JsonExport
{
    public static class CellRefResolver
    {
        /// <summary>
        /// Read a cell value by A1-style address. Sheet name optional (defaults to first sheet).
        /// Examples: "A1", "B10", "Sheet1!E5", "'My Sheet'!A1".
        /// Returns null when the address is invalid, the sheet is missing, or the workbook is null.
        /// </summary>
        public static object ReadCell(XlsFile workbook, string address)
        {
            if (workbook == null || string.IsNullOrEmpty(address)) return null;
            try
            {
                string sheetName;
                int row, col;
                if (!TryParseAddress(address, out sheetName, out row, out col)) return null;

                int sheetIdx = ResolveSheetIndex(workbook, sheetName);
                if (sheetIdx <= 0) return null;

                int prevActive = workbook.ActiveSheet;
                try
                {
                    workbook.ActiveSheet = sheetIdx;
                    return workbook.GetCellValue(row, col);
                }
                finally
                {
                    try { workbook.ActiveSheet = prevActive; } catch { /* ignore */ }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("CellRefResolver.ReadCell failed for '" + address + "': " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Read the value of a named range. Returns the value of the top-left cell of the range,
        /// which matches the user's expectation for single-cell named references.
        /// Returns null if the name is missing or FlexCel doesn't expose a compatible API.
        /// </summary>
        public static object ReadNamedRange(XlsFile workbook, string name)
        {
            if (workbook == null || string.IsNullOrEmpty(name)) return null;
            try
            {
                // FlexCel exposes named ranges via GetNamedRange(name) returning TXlsNamedRange.
                // Use reflection to stay loose with the exact return shape across versions.
                var method = workbook.GetType().GetMethod("GetNamedRange", new Type[] { typeof(string) });
                if (method == null) return null;
                object range = method.Invoke(workbook, new object[] { name });
                if (range == null) return null;

                int sheetIdx = ReadIntProperty(range, new[] { "SheetIndex", "RangeSheet", "Sheet" }, workbook.ActiveSheet);
                int top = ReadIntProperty(range, new[] { "Top", "RangeFirstRow", "Row1", "FirstRow" }, 0);
                int left = ReadIntProperty(range, new[] { "Left", "RangeFirstCol", "Col1", "FirstCol" }, 0);
                if (top <= 0 || left <= 0) return null;

                int prevActive = workbook.ActiveSheet;
                try
                {
                    workbook.ActiveSheet = sheetIdx > 0 ? sheetIdx : workbook.ActiveSheet;
                    return workbook.GetCellValue(top, left);
                }
                finally
                {
                    try { workbook.ActiveSheet = prevActive; } catch { /* ignore */ }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("CellRefResolver.ReadNamedRange failed for '" + name + "': " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Force FlexCel to recompute formulas. Wrap in try-catch because not all FlexCel
        /// versions expose the same method name.
        /// </summary>
        public static void TryRecalc(XlsFile workbook)
        {
            if (workbook == null) return;
            try
            {
                var m = workbook.GetType().GetMethod("Recalc", Type.EmptyTypes);
                if (m != null) m.Invoke(workbook, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("CellRefResolver.TryRecalc failed: " + ex.Message);
            }
        }

        // -------------------------------------------------------------------------------
        // Address parsing: split "Sheet!A1" or "'Sheet name'!A1" or just "A1" into parts.
        // Excel rows/cols are 1-based.
        // -------------------------------------------------------------------------------
        public static bool TryParseAddress(string address, out string sheetName, out int row, out int col)
        {
            sheetName = null;
            row = 0;
            col = 0;
            if (string.IsNullOrEmpty(address)) return false;
            string addr = address.Trim();

            int bang = addr.IndexOf('!');
            if (bang > 0)
            {
                sheetName = addr.Substring(0, bang).Trim();
                if (sheetName.StartsWith("'") && sheetName.EndsWith("'") && sheetName.Length >= 2)
                {
                    sheetName = sheetName.Substring(1, sheetName.Length - 2).Replace("''", "'");
                }
                addr = addr.Substring(bang + 1).Trim();
            }

            // Strip leading $ from absolute refs ($A$1)
            addr = addr.Replace("$", "");

            int i = 0;
            int colNum = 0;
            while (i < addr.Length && char.IsLetter(addr[i]))
            {
                colNum = colNum * 26 + (char.ToUpperInvariant(addr[i]) - 'A' + 1);
                i++;
            }
            if (colNum == 0 || i == addr.Length) return false;

            string rowStr = addr.Substring(i);
            int rowNum;
            if (!int.TryParse(rowStr, out rowNum) || rowNum <= 0) return false;

            row = rowNum;
            col = colNum;
            return true;
        }

        private static int ResolveSheetIndex(XlsFile workbook, string sheetName)
        {
            if (string.IsNullOrEmpty(sheetName)) return workbook.ActiveSheet > 0 ? workbook.ActiveSheet : 1;
            try
            {
                int count = workbook.SheetCount;
                for (int i = 1; i <= count; i++)
                {
                    string name = workbook.GetSheetName(i);
                    if (string.Equals(name, sheetName, StringComparison.OrdinalIgnoreCase)) return i;
                }
            }
            catch { /* fall through */ }
            return 0;
        }

        private static int ReadIntProperty(object obj, string[] propertyNames, int fallback)
        {
            if (obj == null) return fallback;
            var type = obj.GetType();
            foreach (var name in propertyNames)
            {
                try
                {
                    var pi = type.GetProperty(name);
                    if (pi != null)
                    {
                        var v = pi.GetValue(obj, null);
                        if (v != null) return Convert.ToInt32(v);
                    }
                    var fi = type.GetField(name);
                    if (fi != null)
                    {
                        var v = fi.GetValue(obj);
                        if (v != null) return Convert.ToInt32(v);
                    }
                }
                catch { /* try next name */ }
            }
            return fallback;
        }
    }
}

/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Reverse of JsonTemplateRenderer: scan an Excel template (.xlsx) used by MPS print
 * processors, find all <#KEY;> placeholders + <#list.property;> bindings, emit a
 * render-ready JSON skeleton designers can use as a starting point.
 *
 * Patterns based on a full scan of 680 production MPS templates:
 *   Data key (single): <#IDENT;>              e.g. <#TDL_PATIENT_NAME;>, <#Age;>
 *   List binding:      <#IDENT.IDENT;>        e.g. <#SereServs.ServiceName;>
 *                  OR  <#IDENT.IDENT>         e.g. <#HeinServiceType.NUM_ORDER>  (no-semi variant
 *                                                                                  used by ~400 templates)
 *   List meta (skip):  <#List.#rowpos>        e.g. <#SereServs.#rowpos>, .#rowcount
 *   FlFunc (skip):     <#FlFuncXxx(...)>      ends with `)>` — regex naturally rejects
 *   Control (skip):    <#Row Height(...)>     spaces/parens — regex naturally rejects
 *   Quoted (skip):     <#'Auto Merge';>       FlexCel auto-merge marker — quote not in identifier
 *
 * Strict identifier patterns mean we don't need a keyword blacklist: anything that
 * isn't a clean <#identifier(;?)> or <#identifier.identifier(;?)> is automatically excluded.
 * Inner data placeholders inside FlFunc(...) are still captured because regex.Matches advances
 * past unmatched positions and finds the inner <#KEY;> on its next iteration.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FlexCel.XlsAdapter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Inventec.Common.JsonExport
{
    /// <summary>
    /// Statistics + skeleton produced by ExtractWithReport. Used by the CLI to print a
    /// summary and by tests to assert on collected keys.
    /// </summary>
    public class ExtractionReport
    {
        public string JsonSkeleton { get; set; }
        public List<string> SingleKeys { get; set; }
        public Dictionary<string, List<string>> ListProperties { get; set; }
        public List<string> SkippedSheets { get; set; }
        public int SheetsScanned { get; set; }
        public int CellsScanned { get; set; }

        public ExtractionReport()
        {
            SingleKeys = new List<string>();
            ListProperties = new Dictionary<string, List<string>>();
            SkippedSheets = new List<string>();
        }
    }

    public static class JsonTemplateExtractor
    {
        // List binding: <#ListName.PropertyName;> OR <#ListName.PropertyName>
        // Semi is optional because both forms appear in real templates.
        private static readonly Regex ListPropPattern = new Regex(
            @"<#([A-Za-z_][A-Za-z0-9_]*)\.([A-Za-z_][A-Za-z0-9_]*);?>",
            RegexOptions.Compiled);

        // Single data key: <#IDENT;>  (only with semi — single keys without semi don't appear
        // in real data templates; the no-semi forms found are all control tags or list meta)
        private static readonly Regex SingleKeyPattern = new Regex(
            @"<#([A-Za-z_][A-Za-z0-9_]*);>",
            RegexOptions.Compiled);

        // Sheets that Inventec.Common.FlexCelExport generates as code-driven config — never
        // contain designer placeholders we want to surface in the skeleton.
        private static readonly HashSet<string> SkipSheetNames =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Template_Key",
                "Config_Image"
            };

        public static string Extract(string xlsxFilePath)
        {
            if (string.IsNullOrEmpty(xlsxFilePath))
            {
                throw new ArgumentException("xlsxFilePath is required", "xlsxFilePath");
            }
            if (!File.Exists(xlsxFilePath))
            {
                throw new FileNotFoundException("Excel template not found", xlsxFilePath);
            }
            using (var fs = File.OpenRead(xlsxFilePath))
            {
                return Extract(fs);
            }
        }

        public static string Extract(Stream xlsxStream)
        {
            return ExtractWithReport(xlsxStream).JsonSkeleton;
        }

        /// <summary>
        /// Extract + write to file. If <paramref name="overwrite"/> is false and the output
        /// file already exists, returns false without touching the existing file.
        /// </summary>
        public static bool ExtractToFile(string xlsxFilePath, string outputJsonPath, bool overwrite)
        {
            if (string.IsNullOrEmpty(outputJsonPath))
            {
                throw new ArgumentException("outputJsonPath is required", "outputJsonPath");
            }
            if (File.Exists(outputJsonPath) && !overwrite) return false;
            try
            {
                string json = Extract(xlsxFilePath);
                if (string.IsNullOrEmpty(json)) return false;
                File.WriteAllText(outputJsonPath, json, new UTF8Encoding(false));
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("JsonTemplateExtractor.ExtractToFile", ex);
                return false;
            }
        }

        public static ExtractionReport ExtractWithReport(string xlsxFilePath)
        {
            if (string.IsNullOrEmpty(xlsxFilePath))
            {
                throw new ArgumentException("xlsxFilePath is required", "xlsxFilePath");
            }
            if (!File.Exists(xlsxFilePath))
            {
                throw new FileNotFoundException("Excel template not found", xlsxFilePath);
            }
            using (var fs = File.OpenRead(xlsxFilePath))
            {
                return ExtractWithReport(fs);
            }
        }

        public static ExtractionReport ExtractWithReport(Stream xlsxStream)
        {
            var report = new ExtractionReport();
            var singleKeys = new List<string>();
            var singleSet = new HashSet<string>();
            var listProps = new Dictionary<string, List<string>>();
            var listPropSets = new Dictionary<string, HashSet<string>>();

            var xls = new XlsFile(true);
            xls.Open(xlsxStream);

            int sheetCount = xls.SheetCount;
            for (int sheetIdx = 1; sheetIdx <= sheetCount; sheetIdx++)
            {
                string sheetName;
                try { sheetName = xls.GetSheetName(sheetIdx); }
                catch { sheetName = "(unknown)"; }

                if (SkipSheetNames.Contains(sheetName))
                {
                    report.SkippedSheets.Add(sheetName);
                    continue;
                }

                report.SheetsScanned++;
                xls.ActiveSheet = sheetIdx;

                int maxRow = SafeGetRowCount(xls);
                int maxCol = SafeGetColCount(xls);

                for (int row = 1; row <= maxRow; row++)
                {
                    for (int col = 1; col <= maxCol; col++)
                    {
                        object value;
                        try { value = xls.GetCellValue(row, col); }
                        catch { continue; }
                        if (value == null) continue;

                        // FlexCel returns string for plain-text cells and TRichString for cells
                        // with any formatting (bold/color/mixed font). Other types (double, DateTime,
                        // bool, formula) cannot contain placeholders, so skip them without ToString().
                        string s;
                        if (value is string)
                        {
                            s = (string)value;
                        }
                        else if (value is FlexCel.Core.TRichString)
                        {
                            s = value.ToString();
                        }
                        else
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(s)) continue;
                        // Cheap pre-filter — most cells have no placeholder
                        if (s.IndexOf("<#", StringComparison.Ordinal) < 0) continue;

                        report.CellsScanned++;
                        VisitCell(s, singleKeys, singleSet, listProps, listPropSets);
                    }
                }
            }

            report.SingleKeys = singleKeys;
            report.ListProperties = listProps;
            report.JsonSkeleton = BuildSkeleton(singleKeys, listProps);
            return report;
        }

        /// <summary>
        /// Scan a single cell's text. List pattern picks up X.Y bindings (both ;> and >
        /// closings). Single pattern picks up plain identifiers with ;> closing. Strict
        /// regexes mean control tags, FlFunc, row meta, quoted bodies, etc. are silently
        /// excluded — no blacklist needed.
        /// </summary>
        private static void VisitCell(
            string cellContent,
            List<string> singleKeys,
            HashSet<string> singleSet,
            Dictionary<string, List<string>> listProps,
            Dictionary<string, HashSet<string>> listPropSets)
        {
            foreach (Match m in ListPropPattern.Matches(cellContent))
            {
                string listName = m.Groups[1].Value;
                string propName = m.Groups[2].Value;

                HashSet<string> set;
                if (!listPropSets.TryGetValue(listName, out set))
                {
                    set = new HashSet<string>();
                    listPropSets[listName] = set;
                    listProps[listName] = new List<string>();
                }
                if (set.Add(propName))
                {
                    listProps[listName].Add(propName);
                }
            }

            foreach (Match m in SingleKeyPattern.Matches(cellContent))
            {
                string key = m.Groups[1].Value;
                if (singleSet.Add(key))
                {
                    singleKeys.Add(key);
                }
            }
        }

        internal static string BuildSkeleton(List<string> singleKeys, Dictionary<string, List<string>> listProps)
        {
            var root = new JObject();
            foreach (var k in singleKeys)
            {
                root[k] = new JValue("<#" + k + ";>");
            }
            foreach (var kvp in listProps)
            {
                var item = new JObject();
                foreach (var p in kvp.Value)
                {
                    item[p] = new JValue("<#" + p + ";>");
                }
                var arr = new JArray();
                arr.Add(item);
                root[kvp.Key] = arr;
            }
            return root.ToString(Formatting.Indented);
        }

        // FlexCel exposes used-range bounds via properties whose exact names vary by version.
        // Reflection lookup + fallback constants keep us compatible while preventing runaway
        // scans on huge sparse sheets.
        private const int FallbackMaxRow = 2000;
        private const int FallbackMaxCol = 256;

        private static int SafeGetRowCount(XlsFile xls)
        {
            return TryGetIntProperty(xls, "RowCount", FallbackMaxRow);
        }

        private static int SafeGetColCount(XlsFile xls)
        {
            return TryGetIntProperty(xls, "ColCount", FallbackMaxCol);
        }

        private static int TryGetIntProperty(object target, string propertyName, int fallback)
        {
            if (target == null) return fallback;
            try
            {
                var pi = target.GetType().GetProperty(propertyName);
                if (pi == null) return fallback;
                var v = pi.GetValue(target, null);
                if (v == null) return fallback;
                int n = Convert.ToInt32(v);
                return n > 0 ? n : fallback;
            }
            catch
            {
                return fallback;
            }
        }
    }
}

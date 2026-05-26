/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Reverse of XtraReportExport: scan a DevExpress XtraReport template (.repx) used by MPS
 * print processors, find every [FieldName] reference inside <Expression> nodes and attributes,
 * emit a render-ready JSON skeleton compatible with JsonTemplateRenderer (which expects
 * <#KEY;> syntax in the .json template).
 *
 * Patterns based on a survey of production .repx files:
 *   Single field:    [PULSE], [TRACKING_TIME_STR], [BLOOD_PRESSURE_MAX]
 *   Inside Iif/expr: Iif([PULSE] > 0, 'P=' + [PULSE], '') → both [PULSE] hits captured
 *   Dotted (list):   [ServiceReqs.SERVICE_NAME] — uncommon but Tracked by ListPattern
 *
 * .repx is plain XML (UTF-8 BOM, root <XtraReportsLayoutSerializer>) — no ZIP. We use
 * XmlDocument with namespace-agnostic XPath (local-name() based) because DevExpress versions
 * may attach different namespaces to the serializer.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Inventec.Common.JsonExport
{
    public static class XtraReportTemplateExtractor
    {
        // List binding: [ListName.PropertyName] (rare in XtraReport but supported by DevExpress
        // when the report has a hierarchical data source).
        private static readonly Regex ListPropPattern = new Regex(
            @"\[([A-Za-z_][A-Za-z0-9_]*)\.([A-Za-z_][A-Za-z0-9_]*)\]",
            RegexOptions.Compiled);

        // Single field reference: [IDENT]. Must NOT contain dots (dotted form is matched above)
        // and must NOT contain operators / quotes / parens (those are expression syntax around
        // the field, not the field itself).
        private static readonly Regex SingleKeyPattern = new Regex(
            @"\[([A-Za-z_][A-Za-z0-9_]*)\]",
            RegexOptions.Compiled);

        public static string Extract(string repxFilePath)
        {
            return ExtractWithReport(repxFilePath).JsonSkeleton;
        }

        public static ExtractionReport ExtractWithReport(string repxFilePath)
        {
            if (string.IsNullOrEmpty(repxFilePath))
                throw new ArgumentNullException("repxFilePath");
            if (!File.Exists(repxFilePath))
                throw new FileNotFoundException("XtraReport template not found", repxFilePath);

            var report = new ExtractionReport();
            var singleKeys = new List<string>();
            var singleSet = new HashSet<string>();
            var listProps = report.ListProperties;
            var listPropSets = new Dictionary<string, HashSet<string>>();

            var doc = new XmlDocument();
            // PreserveWhitespace not needed — we only read text content / attribute values.
            doc.Load(repxFilePath);

            // .repx records each band as a separate XML tree level — count root child elements
            // (Bands, Parameters, ComponentStorage, etc.) as "sheets scanned" for parity with
            // the Excel extractor's report shape.
            if (doc.DocumentElement != null)
            {
                report.SheetsScanned = doc.DocumentElement.ChildNodes.Count;
            }

            // Visit every Expression element (regardless of namespace). DevExpress serializes
            // formulas both as direct text content of <Expression> AND as attribute values on
            // properties named Expression* — we cover both.
            VisitExpressionElements(doc, singleKeys, singleSet, listProps, listPropSets, report);
            VisitExpressionAttributes(doc, singleKeys, singleSet, listProps, listPropSets, report);

            report.SingleKeys = singleKeys;
            report.JsonSkeleton = JsonTemplateExtractor.BuildSkeleton(singleKeys, listProps);
            return report;
        }

        /// <summary>
        /// Find all elements whose local name is "Expression" and run regex against their
        /// inner text. Uses local-name() XPath so namespace prefix changes don't break the scan.
        /// </summary>
        private static void VisitExpressionElements(
            XmlDocument doc,
            List<string> singleKeys,
            HashSet<string> singleSet,
            Dictionary<string, List<string>> listProps,
            Dictionary<string, HashSet<string>> listPropSets,
            ExtractionReport report)
        {
            var nodes = doc.SelectNodes("//*[local-name()='Expression']");
            if (nodes == null) return;
            foreach (XmlNode node in nodes)
            {
                string text = node.InnerText;
                if (string.IsNullOrEmpty(text)) continue;
                if (text.IndexOf('[') < 0) continue;
                report.CellsScanned++;
                VisitExpressionText(text, singleKeys, singleSet, listProps, listPropSets);
            }
        }

        /// <summary>
        /// Some XtraReport versions store the expression as an attribute (e.g.
        /// &lt;ExpressionBinding Expression="[FOO]" /&gt;) rather than a nested element.
        /// Walk the entire DOM and inspect any attribute whose local name contains "Expression".
        /// </summary>
        private static void VisitExpressionAttributes(
            XmlDocument doc,
            List<string> singleKeys,
            HashSet<string> singleSet,
            Dictionary<string, List<string>> listProps,
            Dictionary<string, HashSet<string>> listPropSets,
            ExtractionReport report)
        {
            var walker = doc.GetElementsByTagName("*");
            foreach (XmlNode node in walker)
            {
                if (node.Attributes == null) continue;
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.LocalName.IndexOf("Expression", StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    string text = attr.Value;
                    if (string.IsNullOrEmpty(text) || text.IndexOf('[') < 0) continue;
                    report.CellsScanned++;
                    VisitExpressionText(text, singleKeys, singleSet, listProps, listPropSets);
                }
            }
        }

        /// <summary>
        /// Apply the dotted-list pattern first (more specific), then the single-key pattern.
        /// Both patterns are non-overlapping for any given substring because they require
        /// different inner shapes.
        /// </summary>
        private static void VisitExpressionText(
            string text,
            List<string> singleKeys,
            HashSet<string> singleSet,
            Dictionary<string, List<string>> listProps,
            Dictionary<string, HashSet<string>> listPropSets)
        {
            foreach (Match m in ListPropPattern.Matches(text))
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

            foreach (Match m in SingleKeyPattern.Matches(text))
            {
                string key = m.Groups[1].Value;
                if (singleSet.Add(key))
                {
                    singleKeys.Add(key);
                }
            }
        }
    }
}

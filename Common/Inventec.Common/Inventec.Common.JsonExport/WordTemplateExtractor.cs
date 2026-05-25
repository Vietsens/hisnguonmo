/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Reverse of TemplaterExport: scan a Word template (.docx) used by MPS print processors,
 * find designer placeholders and emit a render-ready JSON skeleton.
 *
 * Production templates use TWO placeholder syntaxes side-by-side — we scan for both:
 *
 *   1) FlexCel-style: <#KEY;> / <#List.Prop;>
 *      Used by ~half of the existing .docx files (e.g. Mps000354__MauKetQua__CLS.docx).
 *      Same syntax as Excel templates.
 *
 *   2) Templater bracket-style: [[KEY]] / [[List.Prop]]
 *      Used by the other half (e.g. MPS000403_BanCamKetNamGiuongTuNguyen.docx,
 *      MPS000410_BenhAnDieuTriNgoaiTruPTTTPhongKhamPTTT.docx — has 47 [[…]] keys).
 *      Native ONLYOFFICE Templater syntax.
 *
 * When a designer changes formatting (bold, color, font) in the middle of a placeholder,
 * Word splits the run — e.g. <w:r><w:t>&lt;#</w:t></w:r><w:r><w:t>KEY;&gt;</w:t></w:r>
 * or <w:r><w:t>[[</w:t></w:r><w:r><w:t>KEY</w:t></w:r><w:r><w:t>]]</w:t></w:r>.
 * To handle this we concatenate every descendant <w:t> within each paragraph (<w:p>)
 * before running the regexes — Word automatically un-escapes &lt;/&gt; via InnerText,
 * so by the time the regex runs we see literal "<#KEY;>" / "[[KEY]]" again.
 *
 * Strict identifier shape in each regex naturally rejects FlFunc(), control tags,
 * quoted markers, etc. — no blacklist needed.
 *
 * Output always uses <#KEY;> syntax in the generated .json (matches what
 * JsonTemplateRenderer expects), regardless of which source syntax the .docx used.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Inventec.Common.JsonExport
{
    public static class WordTemplateExtractor
    {
        // FlexCel-style list binding: <#ListName.PropertyName;> OR <#ListName.PropertyName>
        private static readonly Regex HashListPattern = new Regex(
            @"<#([A-Za-z_][A-Za-z0-9_]*)\.([A-Za-z_][A-Za-z0-9_]*);?>",
            RegexOptions.Compiled);

        // FlexCel-style single key: <#IDENT;>  (semi required to skip control tags / list meta)
        private static readonly Regex HashSinglePattern = new Regex(
            @"<#([A-Za-z_][A-Za-z0-9_]*);>",
            RegexOptions.Compiled);

        // Templater bracket-style list binding: [[ListName.PropertyName]]
        private static readonly Regex BracketListPattern = new Regex(
            @"\[\[([A-Za-z_][A-Za-z0-9_]*)\.([A-Za-z_][A-Za-z0-9_]*)\]\]",
            RegexOptions.Compiled);

        // Templater bracket-style single key: [[IDENT]]
        private static readonly Regex BracketSinglePattern = new Regex(
            @"\[\[([A-Za-z_][A-Za-z0-9_]*)\]\]",
            RegexOptions.Compiled);

        // Word OOXML namespace for paragraphs / runs / text nodes.
        private const string WNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        // Entries inside the .docx ZIP that we scan. document.xml is required; header/footer
        // are optional but commonly contain placeholders (organization name, page number).
        // Other entries (comments.xml, endnotes.xml, styles.xml...) are skipped — they don't
        // hold designer placeholders in the templates we surveyed.
        private static readonly Regex ScanEntryPattern = new Regex(
            @"^word/(document|header\d*|footer\d*)\.xml$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string Extract(string docxFilePath)
        {
            return ExtractWithReport(docxFilePath).JsonSkeleton;
        }

        public static ExtractionReport ExtractWithReport(string docxFilePath)
        {
            if (string.IsNullOrEmpty(docxFilePath))
                throw new ArgumentNullException("docxFilePath");
            if (!File.Exists(docxFilePath))
                throw new FileNotFoundException("Word template not found", docxFilePath);

            var report = new ExtractionReport();
            var singleKeys = new List<string>();
            var singleSet = new HashSet<string>();
            var listProps = report.ListProperties; // shared dict, populated in place
            var listPropSets = new Dictionary<string, HashSet<string>>();

            using (var zip = ZipFile.OpenRead(docxFilePath))
            {
                foreach (var entry in zip.Entries)
                {
                    if (!ScanEntryPattern.IsMatch(entry.FullName))
                    {
                        // Track skipped XML parts for verbose reporting (reuses SkippedSheets
                        // slot — the report shape is shared with the Excel/XtraReport extractors).
                        if (entry.FullName.StartsWith("word/", StringComparison.OrdinalIgnoreCase)
                            && entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        {
                            report.SkippedSheets.Add(entry.FullName);
                        }
                        continue;
                    }

                    report.SheetsScanned++;
                    using (var stream = entry.Open())
                    {
                        VisitXmlPart(stream, singleKeys, singleSet, listProps, listPropSets, report);
                    }
                }
            }

            report.SingleKeys = singleKeys;
            report.JsonSkeleton = JsonTemplateExtractor.BuildSkeleton(singleKeys, listProps);
            return report;
        }

        /// <summary>
        /// Walk one Word XML part: load DOM, find every &lt;w:p&gt; paragraph, concatenate
        /// descendant &lt;w:t&gt; text, run placeholder regex. Paragraph-level concat is the
        /// minimum scope that reassembles split placeholders introduced by mid-string format
        /// changes, while keeping unrelated text in separate paragraphs from getting joined.
        /// </summary>
        private static void VisitXmlPart(
            Stream stream,
            List<string> singleKeys,
            HashSet<string> singleSet,
            Dictionary<string, List<string>> listProps,
            Dictionary<string, HashSet<string>> listPropSets,
            ExtractionReport report)
        {
            var doc = new XmlDocument();
            doc.Load(stream);
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("w", WNamespace);

            var paragraphs = doc.SelectNodes("//w:p", nsmgr);
            if (paragraphs == null) return;

            var sb = new StringBuilder();
            foreach (XmlNode p in paragraphs)
            {
                sb.Length = 0;
                var textNodes = p.SelectNodes(".//w:t", nsmgr);
                if (textNodes == null || textNodes.Count == 0) continue;

                foreach (XmlNode t in textNodes)
                {
                    sb.Append(t.InnerText);
                }
                string paragraphText = sb.ToString();
                // Cheap pre-filter: skip paragraphs without either placeholder marker.
                bool hasHash = paragraphText.IndexOf("<#", StringComparison.Ordinal) >= 0;
                bool hasBracket = paragraphText.IndexOf("[[", StringComparison.Ordinal) >= 0;
                if (!hasHash && !hasBracket) continue;

                report.CellsScanned++;
                VisitParagraphText(paragraphText, singleKeys, singleSet, listProps, listPropSets);
            }
        }

        /// <summary>
        /// Apply all 4 regexes to the concatenated paragraph text — both FlexCel-style
        /// (&lt;#…;&gt;) and Templater bracket-style ([[…]]). List bindings (dotted form)
        /// run before single-key patterns in each style; the dotted pattern is more specific
        /// and the single-key regex rejects identifiers containing dots, so no double-counting.
        /// </summary>
        private static void VisitParagraphText(
            string text,
            List<string> singleKeys,
            HashSet<string> singleSet,
            Dictionary<string, List<string>> listProps,
            Dictionary<string, HashSet<string>> listPropSets)
        {
            AddListMatches(HashListPattern.Matches(text), listProps, listPropSets);
            AddListMatches(BracketListPattern.Matches(text), listProps, listPropSets);
            AddSingleMatches(HashSinglePattern.Matches(text), singleKeys, singleSet);
            AddSingleMatches(BracketSinglePattern.Matches(text), singleKeys, singleSet);
        }

        private static void AddListMatches(
            MatchCollection matches,
            Dictionary<string, List<string>> listProps,
            Dictionary<string, HashSet<string>> listPropSets)
        {
            foreach (Match m in matches)
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
        }

        private static void AddSingleMatches(
            MatchCollection matches,
            List<string> singleKeys,
            HashSet<string> singleSet)
        {
            foreach (Match m in matches)
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

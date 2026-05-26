using System;
using System.IO;
using System.Text;
using Inventec.Common.JsonExport;

namespace Inventec.Common.JsonExport.Cli
{
    /// <summary>
    /// CLI wrapper around the three template extractors. Dispatches by file extension:
    ///   .xlsx / .xls  → <see cref="JsonTemplateExtractor"/>      (Excel / FlexCel)
    ///   .docx / .doc  → <see cref="WordTemplateExtractor"/>      (Word / Templater)
    ///   .repx         → <see cref="XtraReportTemplateExtractor"/> (DevExpress XtraReport)
    ///
    /// Two modes:
    ///   - Interactive (no args): prompts for the template path, runs extraction, pauses on exit.
    ///     Designer can double-click the exe — no command-line knowledge needed.
    ///   - CLI (with args): automation-friendly flow with flags + exit codes.
    /// Help (-h / --help) prints usage in either mode.
    /// </summary>
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }
            catch { /* console may not support utf8 — fall back silently */ }

            if (args != null && (HasFlag(args, "-h") || HasFlag(args, "--help")))
            {
                PrintUsage();
                return 1;
            }

            if (args == null || args.Length == 0)
            {
                return RunInteractive();
            }

            return RunCli(args);
        }

        // ===================================================================
        // Interactive mode
        // ===================================================================

        private static int RunInteractive()
        {
            PrintBanner();

            string templatePath = PromptForTemplatePath();
            if (string.IsNullOrEmpty(templatePath))
            {
                Console.WriteLine("Đã hủy.");
                PressEnterToExit();
                return 1;
            }

            string outPath = Path.ChangeExtension(templatePath, ".json");
            if (File.Exists(outPath))
            {
                bool overwrite = PromptYesNo("File output đã tồn tại: " + outPath + Environment.NewLine +
                                             "Ghi đè? (y/n)");
                if (!overwrite)
                {
                    Console.WriteLine("Đã hủy — không ghi đè file cũ.");
                    PressEnterToExit();
                    return 0;
                }
            }

            try
            {
                var report = DispatchExtract(templatePath);
                if (report == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Định dạng không hỗ trợ: " + Path.GetExtension(templatePath));
                    Console.WriteLine("Hỗ trợ: .xlsx, .xls, .docx, .doc, .repx");
                    PressEnterToExit();
                    return 1;
                }
                File.WriteAllText(outPath, report.JsonSkeleton, new UTF8Encoding(false));
                Console.WriteLine();
                Console.WriteLine("[OK] " + Path.GetFileName(templatePath) + " -> " + outPath);
                Console.WriteLine("     " + report.SingleKeys.Count + " single keys, " +
                                  report.ListProperties.Count + " list binding" +
                                  (report.ListProperties.Count == 1 ? "" : "s") +
                                  " (cells: " + report.CellsScanned + ")");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Lỗi khi extract: " + ex.Message);
                PressEnterToExit();
                return 3;
            }

            PressEnterToExit();
            return 0;
        }

        /// <summary>
        /// Pick the right extractor based on file extension. Returns null when the extension
        /// is not supported (caller surfaces the error message). FileNotFoundException is
        /// thrown by each extractor when the path doesn't exist — kept consistent so callers
        /// don't need to pre-check.
        /// </summary>
        private static ExtractionReport DispatchExtract(string path)
        {
            string ext = Path.GetExtension(path);
            if (ext == null) return null;
            ext = ext.ToLowerInvariant();
            switch (ext)
            {
                case ".xlsx":
                case ".xls":
                    return JsonTemplateExtractor.ExtractWithReport(path);
                case ".docx":
                case ".doc":
                    return WordTemplateExtractor.ExtractWithReport(path);
                case ".repx":
                    return XtraReportTemplateExtractor.ExtractWithReport(path);
                default:
                    return null;
            }
        }

        private static string PromptForTemplatePath()
        {
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                Console.Write("Nhập đường dẫn file template (.xlsx / .docx / .repx) [bỏ trống để thoát]: ");
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) return null;

                line = line.Trim();
                // Strip surrounding quotes — designers often paste path with quotes when
                // it contains spaces.
                if (line.Length >= 2 && line[0] == '"' && line[line.Length - 1] == '"')
                {
                    line = line.Substring(1, line.Length - 2);
                }

                if (File.Exists(line)) return line;

                Console.WriteLine("Không tìm thấy file: " + line);
                if (attempt < 3)
                {
                    Console.WriteLine("Thử lại (còn " + (3 - attempt) + " lần)...");
                }
            }
            Console.WriteLine("Đã thử 3 lần không thành công.");
            return null;
        }

        private static bool PromptYesNo(string question)
        {
            Console.WriteLine(question);
            Console.Write("> ");
            string line = Console.ReadLine();
            if (line == null) return false;
            string t = line.Trim().ToLowerInvariant();
            return t == "y" || t == "yes" || t == "có" || t == "co";
        }

        private static void PressEnterToExit()
        {
            Console.WriteLine();
            Console.WriteLine("Nhấn Enter để thoát...");
            try { Console.ReadLine(); } catch { /* console may be closed */ }
        }

        private static void PrintBanner()
        {
            Console.WriteLine("JsonExtractor — Phân tích template MPS (.xlsx/.docx/.repx) → JSON skeleton");
            Console.WriteLine("─────────────────────────────────────────────────────────────────────────");
            Console.WriteLine();
        }

        // ===================================================================
        // CLI mode (existing behavior — preserved for automation)
        // ===================================================================

        private static int RunCli(string[] args)
        {
            string templatePath = args[0];
            if (!File.Exists(templatePath))
            {
                Console.Error.WriteLine("File not found: " + templatePath);
                return 2;
            }

            bool verbose = HasFlag(args, "-v") || HasFlag(args, "--verbose");
            bool useStdout = HasFlag(args, "--stdout");
            bool force = HasFlag(args, "--force");
            string outPath = GetOption(args, "-o");
            if (outPath == null) outPath = GetOption(args, "--output");
            if (outPath == null) outPath = Path.ChangeExtension(templatePath, ".json");

            ExtractionReport report;
            try
            {
                report = DispatchExtract(templatePath);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Extraction failed: " + ex.Message);
                return 3;
            }
            if (report == null)
            {
                Console.Error.WriteLine("Unsupported file extension: " + Path.GetExtension(templatePath));
                Console.Error.WriteLine("Supported: .xlsx, .xls, .docx, .doc, .repx");
                return 1;
            }

            if (useStdout)
            {
                Console.Write(report.JsonSkeleton);
                PrintSummary(report, templatePath, null, verbose);
                return 0;
            }

            if (File.Exists(outPath) && !force)
            {
                Console.Error.WriteLine("Output file already exists: " + outPath);
                Console.Error.WriteLine("Use --force to overwrite.");
                return 4;
            }

            try
            {
                File.WriteAllText(outPath, report.JsonSkeleton, new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Cannot write output: " + ex.Message);
                return 4;
            }

            PrintSummary(report, templatePath, outPath, verbose);
            return 0;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("JsonExtractor — extract JSON skeleton from MPS template");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  JsonExtractor.exe                           Interactive mode (prompts for path)");
            Console.WriteLine("  JsonExtractor.exe <template-path> [opts]    CLI mode");
            Console.WriteLine();
            Console.WriteLine("Supported template formats:");
            Console.WriteLine("  .xlsx / .xls   Excel (FlexCel)         — placeholders <#KEY;>");
            Console.WriteLine("  .docx / .doc   Word (Templater)        — placeholders <#KEY;> (same syntax)");
            Console.WriteLine("  .repx          XtraReport (DevExpress) — placeholders [FIELD] in <Expression>");
            Console.WriteLine();
            Console.WriteLine("Output: JSON skeleton with <#KEY;> placeholders (the syntax JsonTemplateRenderer expects).");
            Console.WriteLine();
            Console.WriteLine("Options (CLI mode):");
            Console.WriteLine("  -o, --output <path>   Output file path (default: same name with .json extension)");
            Console.WriteLine("  -v, --verbose         Print detailed list of keys + lists found");
            Console.WriteLine("      --stdout          Print JSON to stdout instead of writing a file");
            Console.WriteLine("      --force           Overwrite existing output file");
            Console.WriteLine("  -h, --help            Show this help");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  JsonExtractor.exe");
            Console.WriteLine("  JsonExtractor.exe Mps000312.xlsx");
            Console.WriteLine("  JsonExtractor.exe Mps000275_BieuMau.docx -v");
            Console.WriteLine("  JsonExtractor.exe Mps000062__ToDieuTri.repx -o C:\\out\\schema.json --force");
            Console.WriteLine();
            Console.WriteLine("Exit codes:  0=ok  1=bad args / unsupported extension  2=file not found  3=extract failed  4=write failed");
        }

        private static void PrintSummary(ExtractionReport report, string templatePath, string outPath, bool verbose)
        {
            int totalSingle = report.SingleKeys.Count;
            int totalLists = report.ListProperties.Count;

            string templateName = Path.GetFileName(templatePath);
            if (outPath != null)
            {
                Console.Error.WriteLine("[OK] " + templateName + " -> " + outPath);
            }
            // "sheets scanned" + "cells with placeholder" labels stay generic — Word counts
            // XML parts as "sheets" and paragraphs with placeholders as "cells"; XtraReport
            // counts root sections / expression nodes. The numbers are useful for spotting
            // empty templates without the label needing per-format wording.
            Console.Error.WriteLine("     " + totalSingle + " single keys, " +
                totalLists + " list binding" + (totalLists == 1 ? "" : "s") +
                " (sheets scanned: " + report.SheetsScanned + ", cells with placeholder: " + report.CellsScanned + ")");

            if (verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("Single keys:");
                foreach (var k in report.SingleKeys)
                {
                    Console.Error.WriteLine("  - " + k);
                }
                Console.Error.WriteLine();
                Console.Error.WriteLine("List bindings:");
                foreach (var kvp in report.ListProperties)
                {
                    Console.Error.WriteLine("  - " + kvp.Key + " (" + kvp.Value.Count + " properties)");
                    foreach (var p in kvp.Value)
                    {
                        Console.Error.WriteLine("      ." + p);
                    }
                }
                if (report.SkippedSheets.Count > 0)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("Skipped sheets:");
                    foreach (var s in report.SkippedSheets)
                    {
                        Console.Error.WriteLine("  - " + s);
                    }
                }
            }
        }

        private static bool HasFlag(string[] args, string flag)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], flag, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        private static string GetOption(string[] args, string optionName)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }
            return null;
        }
    }
}

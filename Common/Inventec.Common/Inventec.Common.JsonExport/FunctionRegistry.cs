/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Function library for JSON template renderer.
 *
 * Phase 1: evaluate
 * Phase 2: if, ifnull, concat, sum, count, avg, min, max, fmt, date, substr, upper, lower, trim
 *
 * A function call has the form &lt;#FN(arg1, arg2, ...);&gt;. The FunctionRegistry handles
 * arg parsing and dispatch. Each argument may be a literal (quoted string, number, bool),
 * a single placeholder, or a mixed template — FunctionContext.ResolveArgAsValue handles
 * the dispatch uniformly.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace Inventec.Common.JsonExport
{
    /// <summary>
    /// Result of a single placeholder/function resolution. Marked failed when the placeholder
    /// cannot be resolved (missing key, function error, child placeholder missing...).
    /// The pipe fallback chain stops at the first non-failed option.
    /// </summary>
    internal class ResolveResult
    {
        public bool Failed;
        public object Value;

        public static ResolveResult Fail() { return new ResolveResult { Failed = true }; }
        public static ResolveResult Ok(object v) { return new ResolveResult { Failed = false, Value = v }; }
    }

    /// <summary>
    /// Resolve a placeholder BODY (text between &lt;# and ;&gt;). The delegate is provided
    /// by the renderer and carries the current SingleKeys + itemContext closure.
    /// </summary>
    internal delegate ResolveResult ResolveBodyDelegate(string placeholderBody);

    /// <summary>
    /// Context passed to function handlers. Carries the data sources and helper methods
    /// for resolving function arguments. Built fresh per call by the renderer.
    /// </summary>
    internal class FunctionContext
    {
        public IDictionary<string, object> SingleKeys;
        public IDictionary<string, IEnumerable<object>> ListData;
        public object ItemContext;
        public ResolveBodyDelegate Resolver;
        public FlexCel.XlsAdapter.XlsFile Workbook;

        /// <summary>
        /// Resolve a raw function argument. Argument may be:
        ///   - quoted string literal: "abc"
        ///   - numeric literal: 123, 1.5
        ///   - boolean literal: true / false
        ///   - single placeholder: &lt;#KEY;&gt;
        ///   - mixed text + placeholders
        /// Returns failure only when a contained placeholder cannot be resolved.
        /// Empty raw arg → returns empty string OK (not failure).
        /// </summary>
        public ResolveResult ResolveArgAsValue(string raw)
        {
            if (raw == null) return ResolveResult.Ok("");
            string trimmed = raw.Trim();
            if (trimmed.Length == 0) return ResolveResult.Ok("");

            // Quoted string literal — unescape \" and \\
            if (trimmed.Length >= 2 && trimmed[0] == '"' && trimmed[trimmed.Length - 1] == '"')
            {
                string content = trimmed.Substring(1, trimmed.Length - 2);
                content = content.Replace("\\\"", "\"").Replace("\\\\", "\\");
                return ResolveResult.Ok(content);
            }

            // Boolean literals (only if the whole arg is the literal — case-sensitive per JSON)
            if (trimmed == "true") return ResolveResult.Ok(true);
            if (trimmed == "false") return ResolveResult.Ok(false);

            // Numeric literal
            decimal d;
            if (decimal.TryParse(trimmed, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
            {
                // Heuristic: don't swallow strings that happen to start with digits but contain placeholders
                if (trimmed.IndexOf(PlaceholderParser.OPEN_TAG, StringComparison.Ordinal) < 0)
                {
                    return ResolveResult.Ok(d);
                }
            }

            // Template — tokenize and resolve
            var tokens = PlaceholderParser.Tokenize(trimmed);
            if (tokens.Count == 1 && tokens[0].IsPlaceholder)
            {
                return Resolver(tokens[0].Text);
            }
            if (tokens.Count == 1 && !tokens[0].IsPlaceholder)
            {
                return ResolveResult.Ok(tokens[0].Text);
            }
            var sb = new System.Text.StringBuilder();
            foreach (var t in tokens)
            {
                if (!t.IsPlaceholder)
                {
                    sb.Append(t.Text);
                    continue;
                }
                var inner = Resolver(t.Text);
                if (inner.Failed) return ResolveResult.Fail();
                sb.Append(StringifyValue(inner.Value));
            }
            return ResolveResult.Ok(sb.ToString());
        }

        /// <summary>
        /// Get an arg as a raw identifier — strips surrounding quotes if quoted, otherwise
        /// returns the trimmed text. Used by aggregators (list name / field name) and by
        /// format/date for the format-string arg.
        /// </summary>
        public string GetRawName(string raw)
        {
            if (raw == null) return "";
            string trimmed = raw.Trim();
            if (trimmed.Length >= 2 && trimmed[0] == '"' && trimmed[trimmed.Length - 1] == '"')
            {
                return trimmed.Substring(1, trimmed.Length - 2)
                    .Replace("\\\"", "\"").Replace("\\\\", "\\");
            }
            return trimmed;
        }

        internal static string StringifyValue(object v)
        {
            if (v == null) return "";
            if (v is IFormattable)
            {
                return ((IFormattable)v).ToString(null, CultureInfo.InvariantCulture);
            }
            return v.ToString();
        }

        internal static bool TryGetPropertyValue(object item, string propertyName, out object value)
        {
            value = null;
            if (item == null || string.IsNullOrEmpty(propertyName)) return false;
            Type t = item.GetType();
            PropertyInfo pi = t.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi == null)
            {
                FieldInfo fi = t.GetField(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (fi == null) return false;
                value = fi.GetValue(item);
                return true;
            }
            value = pi.GetValue(item, null);
            return true;
        }

        internal static bool TryToDecimal(object v, out decimal result)
        {
            result = 0;
            if (v == null) return false;
            try
            {
                result = Convert.ToDecimal(v, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                decimal d;
                if (decimal.TryParse(v.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                {
                    result = d;
                    return true;
                }
                return false;
            }
        }
    }

    internal static class FunctionRegistry
    {
        public static bool IsKnown(string functionName)
        {
            if (string.IsNullOrEmpty(functionName)) return false;
            switch (functionName)
            {
                case "evaluate":
                case "if":
                case "ifnull":
                case "concat":
                case "sum":
                case "count":
                case "avg":
                case "min":
                case "max":
                case "fmt":
                case "date":
                case "substr":
                case "upper":
                case "lower":
                case "trim":
                case "cell":
                case "named":
                    return true;
                default:
                    return CustomFunctions != null && CustomFunctions.ContainsKey(functionName);
            }
        }

        public static ResolveResult Invoke(string functionName, List<string> rawArgs, FunctionContext ctx)
        {
            switch (functionName)
            {
                case "evaluate": return InvokeEvaluate(rawArgs, ctx);
                case "if":       return InvokeIf(rawArgs, ctx);
                case "ifnull":   return InvokeIfNull(rawArgs, ctx);
                case "concat":   return InvokeConcat(rawArgs, ctx);
                case "sum":      return InvokeAggregate(rawArgs, ctx, Aggregate.Sum);
                case "count":    return InvokeCount(rawArgs, ctx);
                case "avg":      return InvokeAggregate(rawArgs, ctx, Aggregate.Avg);
                case "min":      return InvokeAggregate(rawArgs, ctx, Aggregate.Min);
                case "max":      return InvokeAggregate(rawArgs, ctx, Aggregate.Max);
                case "fmt":      return InvokeFmt(rawArgs, ctx);
                case "date":     return InvokeDate(rawArgs, ctx);
                case "substr":   return InvokeSubstr(rawArgs, ctx);
                case "upper":    return InvokeStringTransform(rawArgs, ctx, s => s.ToUpperInvariant());
                case "lower":    return InvokeStringTransform(rawArgs, ctx, s => s.ToLowerInvariant());
                case "trim":     return InvokeStringTransform(rawArgs, ctx, s => s.Trim());
                case "cell":     return InvokeCell(rawArgs, ctx);
                case "named":    return InvokeNamed(rawArgs, ctx);
                default:
                    CustomFunctionHandler custom;
                    if (CustomFunctions != null && CustomFunctions.TryGetValue(functionName, out custom))
                    {
                        try { return custom(rawArgs, ctx); }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Custom function '" + functionName + "' threw: " + ex.Message);
                            return ResolveResult.Fail();
                        }
                    }
                    return ResolveResult.Fail();
            }
        }

        /// <summary>
        /// Plugin point — processor projects (or external code) can register a function name
        /// once at startup. Registration is global (static dictionary) so make sure names don't
        /// collide with built-ins or with each other. Pass null handler to unregister.
        /// </summary>
        public static void Register(string name, CustomFunctionHandler handler)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (CustomFunctions == null)
            {
                CustomFunctions = new Dictionary<string, CustomFunctionHandler>(StringComparer.OrdinalIgnoreCase);
            }
            if (handler == null) CustomFunctions.Remove(name);
            else CustomFunctions[name] = handler;
        }

        public delegate ResolveResult CustomFunctionHandler(List<string> rawArgs, FunctionContext ctx);
        private static Dictionary<string, CustomFunctionHandler> CustomFunctions;

        // -------------------------------------------------------------------------
        // cell("Sheet1!A1") / cell("A1") — read a cell from the rendered workbook.
        // -------------------------------------------------------------------------
        private static ResolveResult InvokeCell(List<string> args, FunctionContext ctx)
        {
            if (args == null || args.Count < 1) return ResolveResult.Fail();
            if (ctx.Workbook == null) return ResolveResult.Fail();
            string address = ctx.GetRawName(args[0]);
            var v = CellRefResolver.ReadCell(ctx.Workbook, address);
            if (v == null) return ResolveResult.Fail();
            string s = v as string;
            if (s != null && s.Length == 0) return ResolveResult.Fail();
            return ResolveResult.Ok(v);
        }

        // -------------------------------------------------------------------------
        // named("range_name") — read a named range's top-left cell from the workbook.
        // -------------------------------------------------------------------------
        private static ResolveResult InvokeNamed(List<string> args, FunctionContext ctx)
        {
            if (args == null || args.Count < 1) return ResolveResult.Fail();
            if (ctx.Workbook == null) return ResolveResult.Fail();
            string name = ctx.GetRawName(args[0]);
            var v = CellRefResolver.ReadNamedRange(ctx.Workbook, name);
            if (v == null) return ResolveResult.Fail();
            string s = v as string;
            if (s != null && s.Length == 0) return ResolveResult.Fail();
            return ResolveResult.Ok(v);
        }

        // -------------------------------------------------------------------------
        // evaluate(EXPR): substitute inner placeholders, run DataTable.Compute.
        // -------------------------------------------------------------------------
        private static ResolveResult InvokeEvaluate(List<string> rawArgs, FunctionContext ctx)
        {
            if (rawArgs == null || rawArgs.Count == 0) return ResolveResult.Fail();
            string raw = string.Join(",", rawArgs.ToArray());

            var tokens = PlaceholderParser.Tokenize(raw);
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < tokens.Count; i++)
            {
                var t = tokens[i];
                if (!t.IsPlaceholder)
                {
                    sb.Append(t.Text);
                    continue;
                }
                var inner = ctx.Resolver(t.Text);
                if (inner.Failed) return ResolveResult.Fail();
                sb.Append(FormatNumeric(inner.Value));
            }

            string expr = sb.ToString();
            try
            {
                using (var table = new DataTable())
                {
                    object computed = table.Compute(expr, "");
                    if (computed == null || computed == DBNull.Value) return ResolveResult.Fail();
                    return ResolveResult.Ok(computed);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("JSON evaluate failed: expr=" + expr + " -- " + ex.Message);
                return ResolveResult.Fail();
            }
        }

        private static string FormatNumeric(object value)
        {
            if (value == null) return "0";
            if (value is bool) return ((bool)value) ? "true" : "false";
            decimal d;
            if (FunctionContext.TryToDecimal(value, out d))
            {
                return d.ToString(CultureInfo.InvariantCulture);
            }
            return "'" + value.ToString().Replace("'", "''") + "'";
        }

        // -------------------------------------------------------------------------
        // if(COND, THEN, ELSE): COND is truthy if non-zero number, non-empty string,
        // bool true. ELSE arg is optional — defaults to empty string.
        // THEN/ELSE are resolved lazily (only the chosen branch).
        // -------------------------------------------------------------------------
        private static ResolveResult InvokeIf(List<string> args, FunctionContext ctx)
        {
            if (args == null || args.Count < 2) return ResolveResult.Fail();
            var condResult = ctx.ResolveArgAsValue(args[0]);
            if (condResult.Failed) return ResolveResult.Fail();
            bool truthy = IsTruthy(condResult.Value);
            if (truthy)
            {
                return ctx.ResolveArgAsValue(args[1]);
            }
            if (args.Count >= 3)
            {
                return ctx.ResolveArgAsValue(args[2]);
            }
            return ResolveResult.Ok("");
        }

        private static bool IsTruthy(object v)
        {
            if (v == null) return false;
            if (v is bool) return (bool)v;
            decimal d;
            if (FunctionContext.TryToDecimal(v, out d)) return d != 0;
            string s = v.ToString();
            if (s.Length == 0) return false;
            if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase)) return false;
            if (s == "0") return false;
            return true;
        }

        // -------------------------------------------------------------------------
        // ifnull(VALUE, DEFAULT): return DEFAULT if VALUE resolves null/empty.
        // -------------------------------------------------------------------------
        private static ResolveResult InvokeIfNull(List<string> args, FunctionContext ctx)
        {
            if (args == null || args.Count < 1) return ResolveResult.Fail();
            var v = ctx.ResolveArgAsValue(args[0]);
            if (!v.Failed && v.Value != null)
            {
                string s = v.Value as string;
                if (s == null || s.Length > 0) return v;
            }
            if (args.Count >= 2) return ctx.ResolveArgAsValue(args[1]);
            return ResolveResult.Ok("");
        }

        // -------------------------------------------------------------------------
        // concat(A, B, C, ...): stringify each arg and concatenate.
        // Missing/failed args contribute empty string (does not abort).
        // -------------------------------------------------------------------------
        private static ResolveResult InvokeConcat(List<string> args, FunctionContext ctx)
        {
            if (args == null || args.Count == 0) return ResolveResult.Ok("");
            var sb = new System.Text.StringBuilder();
            foreach (var arg in args)
            {
                var r = ctx.ResolveArgAsValue(arg);
                if (r.Failed) continue;
                if (r.Value != null) sb.Append(FunctionContext.StringifyValue(r.Value));
            }
            return ResolveResult.Ok(sb.ToString());
        }

        // -------------------------------------------------------------------------
        // Aggregators: sum/avg/min/max(LIST_NAME, FIELD) over registered list ADO.
        // count(LIST_NAME) is separate (no field arg).
        // -------------------------------------------------------------------------
        private enum Aggregate { Sum, Avg, Min, Max }

        private static ResolveResult InvokeAggregate(List<string> args, FunctionContext ctx, Aggregate kind)
        {
            if (args == null || args.Count < 2) return ResolveResult.Fail();
            string listName = ctx.GetRawName(args[0]);
            string fieldName = ctx.GetRawName(args[1]);
            if (string.IsNullOrEmpty(listName) || string.IsNullOrEmpty(fieldName)) return ResolveResult.Fail();

            IEnumerable<object> list;
            if (ctx.ListData == null || !ctx.ListData.TryGetValue(listName, out list) || list == null)
            {
                return ResolveResult.Fail();
            }

            decimal acc = 0;
            decimal? minVal = null;
            decimal? maxVal = null;
            int count = 0;
            foreach (var item in list)
            {
                object propValue;
                if (!FunctionContext.TryGetPropertyValue(item, fieldName, out propValue) || propValue == null) continue;
                decimal d;
                if (!FunctionContext.TryToDecimal(propValue, out d)) continue;
                acc += d;
                count++;
                if (!minVal.HasValue || d < minVal.Value) minVal = d;
                if (!maxVal.HasValue || d > maxVal.Value) maxVal = d;
            }

            switch (kind)
            {
                case Aggregate.Sum: return ResolveResult.Ok(acc);
                case Aggregate.Avg: return count == 0 ? ResolveResult.Ok((decimal)0) : ResolveResult.Ok(acc / count);
                case Aggregate.Min: return minVal.HasValue ? ResolveResult.Ok(minVal.Value) : ResolveResult.Fail();
                case Aggregate.Max: return maxVal.HasValue ? ResolveResult.Ok(maxVal.Value) : ResolveResult.Fail();
            }
            return ResolveResult.Fail();
        }

        private static ResolveResult InvokeCount(List<string> args, FunctionContext ctx)
        {
            if (args == null || args.Count < 1) return ResolveResult.Fail();
            string listName = ctx.GetRawName(args[0]);
            IEnumerable<object> list;
            if (ctx.ListData == null || !ctx.ListData.TryGetValue(listName, out list) || list == null)
            {
                return ResolveResult.Ok(0);
            }
            int count = 0;
            foreach (var _ in list) count++;
            return ResolveResult.Ok((decimal)count);
        }

        // -------------------------------------------------------------------------
        // fmt(VALUE, FORMAT): format a value using .NET format string. Number formats
        // (N0, N2, C, P...) use Vietnamese culture for thousand/decimal separators
        // since the HIS UI is Vietnamese. Falls back to invariant if vi-VN unavailable.
        // -------------------------------------------------------------------------
        private static ResolveResult InvokeFmt(List<string> args, FunctionContext ctx)
        {
            if (args == null || args.Count < 2) return ResolveResult.Fail();
            var valueResult = ctx.ResolveArgAsValue(args[0]);
            if (valueResult.Failed || valueResult.Value == null) return ResolveResult.Fail();
            string format = ctx.GetRawName(args[1]);
            if (string.IsNullOrEmpty(format)) return ResolveResult.Ok(FunctionContext.StringifyValue(valueResult.Value));

            CultureInfo culture;
            try { culture = CultureInfo.GetCultureInfo("vi-VN"); }
            catch { culture = CultureInfo.InvariantCulture; }

            decimal d;
            if (FunctionContext.TryToDecimal(valueResult.Value, out d))
            {
                try { return ResolveResult.Ok(d.ToString(format, culture)); }
                catch { return ResolveResult.Ok(d.ToString(CultureInfo.InvariantCulture)); }
            }

            var formattable = valueResult.Value as IFormattable;
            if (formattable != null)
            {
                try { return ResolveResult.Ok(formattable.ToString(format, culture)); }
                catch { /* fall through */ }
            }
            return ResolveResult.Ok(valueResult.Value.ToString());
        }

        // -------------------------------------------------------------------------
        // date(VALUE, FORMAT): parse VALUE as long yyyyMMddHHmmss / yyyyMMdd, output FORMAT.
        // -------------------------------------------------------------------------
        private static readonly string[] DATE_INPUT_PATTERNS =
        {
            "yyyyMMddHHmmss",
            "yyyyMMddHHmm",
            "yyyyMMddHH",
            "yyyyMMdd"
        };

        private static ResolveResult InvokeDate(List<string> args, FunctionContext ctx)
        {
            if (args == null || args.Count < 2) return ResolveResult.Fail();
            var valueResult = ctx.ResolveArgAsValue(args[0]);
            if (valueResult.Failed || valueResult.Value == null) return ResolveResult.Fail();
            string format = ctx.GetRawName(args[1]);
            if (string.IsNullOrEmpty(format)) format = "dd/MM/yyyy";

            string s = valueResult.Value.ToString();
            // Strip decimal point if value came in as 20260516103015.0
            int dot = s.IndexOf('.');
            if (dot > 0) s = s.Substring(0, dot);

            DateTime dt;
            foreach (var pat in DATE_INPUT_PATTERNS)
            {
                if (DateTime.TryParseExact(s, pat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return ResolveResult.Ok(dt.ToString(format, CultureInfo.InvariantCulture));
                }
            }
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return ResolveResult.Ok(dt.ToString(format, CultureInfo.InvariantCulture));
            }
            return ResolveResult.Fail();
        }

        // -------------------------------------------------------------------------
        // substr(VALUE, START [, LEN]): substring with safe bounds.
        // -------------------------------------------------------------------------
        private static ResolveResult InvokeSubstr(List<string> args, FunctionContext ctx)
        {
            if (args == null || args.Count < 2) return ResolveResult.Fail();
            var vr = ctx.ResolveArgAsValue(args[0]);
            if (vr.Failed || vr.Value == null) return ResolveResult.Fail();
            string s = vr.Value.ToString();

            var startRes = ctx.ResolveArgAsValue(args[1]);
            if (startRes.Failed) return ResolveResult.Fail();
            decimal startDec;
            if (!FunctionContext.TryToDecimal(startRes.Value, out startDec)) return ResolveResult.Fail();
            int start = (int)startDec;
            if (start < 0) start = 0;
            if (start >= s.Length) return ResolveResult.Ok("");

            if (args.Count >= 3)
            {
                var lenRes = ctx.ResolveArgAsValue(args[2]);
                if (lenRes.Failed) return ResolveResult.Fail();
                decimal lenDec;
                if (!FunctionContext.TryToDecimal(lenRes.Value, out lenDec)) return ResolveResult.Fail();
                int len = (int)lenDec;
                if (len < 0) len = 0;
                if (start + len > s.Length) len = s.Length - start;
                return ResolveResult.Ok(s.Substring(start, len));
            }
            return ResolveResult.Ok(s.Substring(start));
        }

        // -------------------------------------------------------------------------
        // upper/lower/trim(VALUE)
        // -------------------------------------------------------------------------
        private static ResolveResult InvokeStringTransform(List<string> args, FunctionContext ctx, Func<string, string> transform)
        {
            if (args == null || args.Count < 1) return ResolveResult.Fail();
            var v = ctx.ResolveArgAsValue(args[0]);
            if (v.Failed || v.Value == null) return ResolveResult.Fail();
            return ResolveResult.Ok(transform(v.Value.ToString()));
        }
    }
}

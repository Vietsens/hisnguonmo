/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * JSON template renderer for MPS print processors.
 *
 * Pipeline:
 *   1. Parse template into JToken (template must be valid JSON; placeholders live inside string values
 *      or inside array element templates).
 *   2. Loop expansion: for each JArray with exactly one JObject element whose property name matches
 *      a registered list ADO, clone the element N times and bind property accessors to each item.
 *   3. Placeholder resolution: walk the tree, replace each JValue(String) containing placeholders.
 *      Pipe fallback: try options in order, first non-failed wins.
 *   4. Smart type coercion: numeric / boolean strings get downgraded to their typed JValue so
 *      output JSON has correct types ("thanhtien": 1000000 instead of "1000000").
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Inventec.Common.JsonExport
{
    public static class JsonTemplateRenderer
    {
        /// <summary>
        /// Render a JSON template string into final JSON, substituting placeholders from
        /// singleKeys and expanding list-bound arrays from listData.
        /// </summary>
        /// <param name="templateContent">Raw JSON template (must parse as valid JSON).</param>
        /// <param name="singleKeys">Dictionary of scalar keys (typically AbstractProcessor.singleValueDictionary).</param>
        /// <param name="listData">Dictionary of named list ADOs registered for JSON export.</param>
        /// <returns>Rendered JSON string. Returns null on unrecoverable error (logged).</returns>
        /// <summary>
        /// Regex to translate "[[address]]" shorthand into the unified "&lt;#cell(\"address\");&gt;"
        /// placeholder form. Runs once before tokenization so everything else stays in one syntax.
        /// </summary>
        private static readonly System.Text.RegularExpressions.Regex CellRefRegex =
            new System.Text.RegularExpressions.Regex(@"\[\[([^\[\]]+)\]\]",
                System.Text.RegularExpressions.RegexOptions.Compiled);

        public static string Render(
            string templateContent,
            IDictionary<string, object> singleKeys,
            IDictionary<string, IEnumerable<object>> listData)
        {
            return Render(templateContent, singleKeys, listData, null);
        }

        /// <summary>
        /// Full Render with an optional FlexCel workbook for cell/named-range resolution (Phase 3).
        /// The workbook must already be rendered (placeholders replaced, formulas evaluated) — the
        /// caller is responsible for opening saveMemoryStream into an XlsFile and optionally calling
        /// Recalc before invoking this method.
        /// </summary>
        public static string Render(
            string templateContent,
            IDictionary<string, object> singleKeys,
            IDictionary<string, IEnumerable<object>> listData,
            FlexCel.XlsAdapter.XlsFile workbook)
        {
            if (string.IsNullOrWhiteSpace(templateContent)) return null;
            try
            {
                // Pre-process [[Sheet!A1]] shorthand into the function form so all resolution
                // goes through a single code path.
                string preprocessed = CellRefRegex.Replace(templateContent,
                    m => "<#cell(\"" + m.Groups[1].Value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\");>");

                JToken root = JToken.Parse(preprocessed);
                var ctx = new RenderContext(singleKeys, listData, workbook);

                ExpandLoops(root, ctx, null);
                ResolvePlaceholders(root, ctx);
                ApplyConditionalOmit(root);

                return root.ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("JsonTemplateRenderer.Render failed", ex);
                return null;
            }
        }

        // -----------------------------------------------------------------------------------
        // Loop expansion: arrays with [single JObject template] whose JSON field name matches
        // a registered list get expanded to N elements, each pre-bound to item properties.
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Expand list-template arrays. itemContext is the parent loop's current item, used for
        /// resolving nested loops where the inner list is a PROPERTY of the parent item rather
        /// than a globally-registered list.
        /// Phase 6: nested loops via item-scoped list resolution.
        /// </summary>
        private static void ExpandLoops(JToken node, RenderContext ctx, object itemContext)
        {
            JObject obj = node as JObject;
            if (obj != null)
            {
                // Snapshot property list because we'll be replacing JArray values in place
                var props = new List<JProperty>(obj.Properties());
                foreach (var prop in props)
                {
                    JToken val = prop.Value;
                    JArray arr = val as JArray;
                    if (arr != null && IsLoopTemplate(arr))
                    {
                        // Strip "?" suffix from property name for list lookup (so omit-marker
                        // doesn't break the loop binding).
                        string lookupName = prop.Name.EndsWith("?")
                            ? prop.Name.Substring(0, prop.Name.Length - 1)
                            : prop.Name;
                        IEnumerable<object> list = ResolveListForLoop(lookupName, ctx, itemContext);
                        if (list != null)
                        {
                            var template = (JObject)arr[0];
                            var expanded = new JArray();
                            foreach (var item in list)
                            {
                                var clone = (JObject)template.DeepClone();
                                // Recursively expand nested loops inside the cloned element, with
                                // the current item as the new scope so nested list properties resolve.
                                ExpandLoops(clone, ctx, item);
                                // Resolve placeholders in this cloned object using item context
                                ResolvePlaceholders(clone, ctx, item);
                                expanded.Add(clone);
                            }
                            prop.Value = expanded;
                            continue;
                        }
                        // No matching list → recurse normally so inner placeholders still resolve later
                    }
                    ExpandLoops(val, ctx, itemContext);
                }
                return;
            }
            JArray array = node as JArray;
            if (array != null)
            {
                foreach (var child in array)
                {
                    ExpandLoops(child, ctx, itemContext);
                }
            }
        }

        /// <summary>
        /// Look up the list to bind a loop array against. Search order:
        ///   1) Current item's property (case-insensitive) — enables nested loops.
        ///   2) Globally-registered list ADO via ctx.ListData.
        /// Returns null when nothing matches (loop stays as-is template).
        /// </summary>
        private static IEnumerable<object> ResolveListForLoop(string name, RenderContext ctx, object itemContext)
        {
            if (itemContext != null)
            {
                object value;
                if (FunctionContext.TryGetPropertyValue(itemContext, name, out value) && value != null && !(value is string))
                {
                    var generic = value as IEnumerable<object>;
                    if (generic != null) return generic;
                    var legacy = value as System.Collections.IEnumerable;
                    if (legacy != null) return WrapLegacyEnumerable(legacy);
                }
            }
            IEnumerable<object> list;
            if (ctx.ListData != null && ctx.ListData.TryGetValue(name, out list))
            {
                return list;
            }
            return null;
        }

        private static IEnumerable<object> WrapLegacyEnumerable(System.Collections.IEnumerable source)
        {
            var result = new List<object>();
            foreach (var x in source) result.Add(x);
            return result;
        }

        /// <summary>
        /// A JArray qualifies as a loop template when it contains exactly one JObject and that
        /// object has at least one string property containing a placeholder. Empty arrays and
        /// arrays with multiple elements are left alone.
        /// </summary>
        private static bool IsLoopTemplate(JArray arr)
        {
            if (arr.Count != 1) return false;
            return arr[0] is JObject;
        }

        // -----------------------------------------------------------------------------------
        // Placeholder resolution: walk the tree, rewrite each JValue(String) that contains
        // placeholders. itemContext (when set) provides property-level resolution for loop items.
        // -----------------------------------------------------------------------------------
        private static void ResolvePlaceholders(JToken node, RenderContext ctx, object itemContext = null)
        {
            JObject obj = node as JObject;
            if (obj != null)
            {
                foreach (var prop in obj.Properties())
                {
                    ResolvePlaceholders(prop.Value, ctx, itemContext);
                }
                return;
            }
            JArray arr = node as JArray;
            if (arr != null)
            {
                foreach (var child in arr)
                {
                    ResolvePlaceholders(child, ctx, itemContext);
                }
                return;
            }
            JValue jv = node as JValue;
            if (jv != null && jv.Type == JTokenType.String)
            {
                string raw = jv.Value as string;
                if (string.IsNullOrEmpty(raw) || raw.IndexOf(PlaceholderParser.OPEN_TAG, StringComparison.Ordinal) < 0)
                {
                    return; // no placeholder — leave as-is
                }
                ResolvedValue resolved = ResolveValueTemplate(raw, ctx, itemContext);
                ApplyResolvedValue(jv, resolved);
            }
        }

        /// <summary>
        /// Resolve a string value that may contain placeholders and pipe fallback.
        /// Returns the chosen option's value (may be a typed primitive) or empty string on full failure.
        /// </summary>
        private static ResolvedValue ResolveValueTemplate(string raw, RenderContext ctx, object itemContext)
        {
            var options = PlaceholderParser.SplitPipeOptions(raw);
            foreach (var opt in options)
            {
                var r = ResolveOption(opt, ctx, itemContext);
                if (!r.Failed)
                {
                    return new ResolvedValue { Value = r.Value, IsPureSingle = IsPureSinglePlaceholder(opt) };
                }
            }
            // All options failed — return empty string fallback so output JSON stays valid
            return new ResolvedValue { Value = "", IsPureSingle = false };
        }

        /// <summary>
        /// Resolve one pipe option. The option may be:
        ///   - a single placeholder (<#KEY;> or <#FN(...);>)
        ///   - mixed text + placeholders (e.g. "Mã: <#code;>, BN: <#name;>")
        /// </summary>
        private static ResolveResult ResolveOption(string optionTemplate, RenderContext ctx, object itemContext)
        {
            var tokens = PlaceholderParser.Tokenize(optionTemplate);

            // Case 1: single placeholder token, no surrounding literal text
            if (tokens.Count == 1 && tokens[0].IsPlaceholder)
            {
                return ResolvePlaceholderBody(tokens[0].Text, ctx, itemContext);
            }

            // Case 2: literal-only, no placeholder
            if (tokens.Count == 1 && !tokens[0].IsPlaceholder)
            {
                string lit = tokens[0].Text;
                return string.IsNullOrEmpty(lit) ? ResolveResult.Fail() : ResolveResult.Ok(lit);
            }

            // Case 3: mixed — stringify all parts; any failed placeholder makes the whole option fail
            var sb = new System.Text.StringBuilder();
            foreach (var t in tokens)
            {
                if (!t.IsPlaceholder)
                {
                    sb.Append(t.Text);
                    continue;
                }
                var inner = ResolvePlaceholderBody(t.Text, ctx, itemContext);
                if (inner.Failed) return ResolveResult.Fail();
                sb.Append(StringifyForMixedContext(inner.Value));
            }
            return ResolveResult.Ok(sb.ToString());
        }

        /// <summary>
        /// Resolve a placeholder body (the text between &lt;# and ;&gt;). Could be a bare key
        /// or a function call.
        /// </summary>
        private static ResolveResult ResolvePlaceholderBody(string body, RenderContext ctx, object itemContext)
        {
            if (string.IsNullOrEmpty(body)) return ResolveResult.Fail();

            // Shorthand for named range: <#@name;> → equivalent to <#named("name");>
            if (body.Length > 1 && body[0] == '@')
            {
                if (ctx.Workbook == null) return ResolveResult.Fail();
                string rangeName = body.Substring(1).Trim();
                if (rangeName.Length == 0) return ResolveResult.Fail();
                var v = CellRefResolver.ReadNamedRange(ctx.Workbook, rangeName);
                if (v == null) return ResolveResult.Fail();
                string s = v as string;
                if (s != null && s.Length == 0) return ResolveResult.Fail();
                return ResolveResult.Ok(v);
            }

            // Function call?
            string fn, argsRaw;
            if (PlaceholderParser.TryParseFunction(body, out fn, out argsRaw))
            {
                if (FunctionRegistry.IsKnown(fn))
                {
                    var rawArgs = PlaceholderParser.SplitArguments(argsRaw);
                    var fnCtx = new FunctionContext
                    {
                        SingleKeys = ctx.SingleKeys,
                        ListData = ctx.ListData,
                        ItemContext = itemContext,
                        Resolver = (innerBody) => ResolvePlaceholderBody(innerBody, ctx, itemContext),
                        Workbook = ctx.Workbook,
                    };
                    return FunctionRegistry.Invoke(fn, rawArgs, fnCtx);
                }
                // Unknown function name — treat as missing, let pipe fallback try next option
                return ResolveResult.Fail();
            }

            // Bare key — lookup item context first (for loop binding), then singleKeys
            string keyName = body.Trim();
            if (!PlaceholderParser.IsValidIdentifier(keyName)) return ResolveResult.Fail();

            if (itemContext != null)
            {
                object propValue;
                if (FunctionContext.TryGetPropertyValue(itemContext, keyName, out propValue))
                {
                    if (IsEmpty(propValue)) return ResolveResult.Fail();
                    return ResolveResult.Ok(propValue);
                }
            }

            if (ctx.SingleKeys != null)
            {
                // Case-insensitive lookup so templates can use either casing
                foreach (var kv in ctx.SingleKeys)
                {
                    if (string.Equals(kv.Key, keyName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (IsEmpty(kv.Value)) return ResolveResult.Fail();
                        return ResolveResult.Ok(kv.Value);
                    }
                }
            }

            return ResolveResult.Fail();
        }

        // -----------------------------------------------------------------------------------
        // Phase 7: conditional omit. Property names ending in "?" are dropped when their
        // resolved value is empty (empty string, null, empty array, empty object). Otherwise
        // the "?" suffix is stripped from the final output name.
        // Example:
        //   "ghichu?": "<#note;>"
        //   → If <#note;> is missing → property removed.
        //   → If <#note;> = "abc" → output is "ghichu": "abc"
        // -----------------------------------------------------------------------------------
        private static void ApplyConditionalOmit(JToken node)
        {
            JObject obj = node as JObject;
            if (obj != null)
            {
                var props = new List<JProperty>(obj.Properties());
                foreach (var prop in props)
                {
                    ApplyConditionalOmit(prop.Value);

                    if (prop.Name.EndsWith("?") && prop.Name.Length > 1)
                    {
                        if (IsEmptyJson(prop.Value))
                        {
                            prop.Remove();
                        }
                        else
                        {
                            string newName = prop.Name.Substring(0, prop.Name.Length - 1);
                            // Replace with renamed property at same position
                            var valueClone = prop.Value;
                            prop.Replace(new JProperty(newName, valueClone));
                        }
                    }
                }
                return;
            }
            JArray arr = node as JArray;
            if (arr != null)
            {
                foreach (var child in arr)
                {
                    ApplyConditionalOmit(child);
                }
            }
        }

        private static bool IsEmptyJson(JToken token)
        {
            if (token == null) return true;
            switch (token.Type)
            {
                case JTokenType.Null:
                    return true;
                case JTokenType.String:
                    {
                        string s = (string)((JValue)token).Value;
                        return string.IsNullOrEmpty(s);
                    }
                case JTokenType.Array:
                    return ((JArray)token).Count == 0;
                case JTokenType.Object:
                    {
                        var o = (JObject)token;
                        var props = o.Properties();
                        bool any = false;
                        foreach (var _ in props) { any = true; break; }
                        return !any;
                    }
                default:
                    return false;
            }
        }

        private static bool IsEmpty(object v)
        {
            if (v == null) return true;
            string s = v as string;
            if (s != null) return s.Length == 0;
            return false;
        }

        /// <summary>
        /// Replace the contents of a JValue with the resolved value, applying smart type coercion:
        /// numeric/boolean strings get a typed JValue so output JSON serializes them without quotes.
        /// Pure-single-placeholder options preserve the source value's native type.
        /// </summary>
        private static void ApplyResolvedValue(JValue target, ResolvedValue resolved)
        {
            object v = resolved.Value;

            if (v == null)
            {
                target.Value = "";
                return;
            }

            // Numeric or bool source values from a single placeholder → keep native type
            if (resolved.IsPureSingle)
            {
                if (v is bool)
                {
                    target.Replace(new JValue((bool)v));
                    return;
                }
                if (IsNumeric(v))
                {
                    target.Replace(new JValue(Convert.ToDecimal(v, CultureInfo.InvariantCulture)));
                    return;
                }
                target.Value = v.ToString();
                return;
            }

            // Mixed/literal result — string. Try numeric/bool coercion for raw context-like output.
            string s = v.ToString();
            decimal dec;
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out dec))
            {
                target.Replace(new JValue(dec));
                return;
            }
            bool boolVal;
            if (bool.TryParse(s, out boolVal))
            {
                target.Replace(new JValue(boolVal));
                return;
            }
            target.Value = s;
        }

        private static bool IsNumeric(object v)
        {
            return v is byte || v is sbyte || v is short || v is ushort
                || v is int || v is uint || v is long || v is ulong
                || v is float || v is double || v is decimal;
        }

        private static bool IsPureSinglePlaceholder(string option)
        {
            if (string.IsNullOrEmpty(option)) return false;
            if (!option.StartsWith(PlaceholderParser.OPEN_TAG)) return false;
            if (!option.EndsWith(PlaceholderParser.CLOSE_TAG)) return false;
            // Must be a single placeholder — no nested closures and no internal pipe
            int bodyStart = PlaceholderParser.OPEN_TAG.Length;
            int closeIdx = PlaceholderParser.FindClosingTag(option, bodyStart);
            return closeIdx == option.Length - PlaceholderParser.CLOSE_TAG.Length;
        }

        private static string StringifyForMixedContext(object v)
        {
            if (v == null) return "";
            IFormattable f = v as IFormattable;
            if (f != null) return f.ToString(null, CultureInfo.InvariantCulture);
            return v.ToString();
        }

        // -----------------------------------------------------------------------------------
        private class RenderContext
        {
            public IDictionary<string, object> SingleKeys;
            public IDictionary<string, IEnumerable<object>> ListData;
            public FlexCel.XlsAdapter.XlsFile Workbook;

            public RenderContext(IDictionary<string, object> singleKeys,
                IDictionary<string, IEnumerable<object>> listData,
                FlexCel.XlsAdapter.XlsFile workbook)
            {
                SingleKeys = singleKeys;
                ListData = listData;
                Workbook = workbook;
            }
        }

        private struct ResolvedValue
        {
            public object Value;
            public bool IsPureSingle;
        }
    }
}

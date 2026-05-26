/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Depth-aware parser for placeholder syntax <#KEY;> with nesting support.
 * Phase 1 of JSON template renderer.
 */
using System;
using System.Collections.Generic;

namespace Inventec.Common.JsonExport
{
    /// <summary>
    /// Token returned by the parser: either literal text or a placeholder body.
    /// </summary>
    internal class Token
    {
        public bool IsPlaceholder;
        public string Text;
    }

    /// <summary>
    /// Parses placeholder expressions of the form <#...;> with proper depth handling
    /// for nested placeholders. Also handles pipe fallback splitting.
    ///
    /// Public API:
    ///   - Tokenize(input): split into a sequence of literal/placeholder tokens.
    ///   - SplitPipeOptions(body): split a single value template by '|' at depth 0.
    ///   - ParseFunction(body): if placeholder body looks like FN(args), return name + args.
    /// </summary>
    internal static class PlaceholderParser
    {
        public const string OPEN_TAG = "<#";
        public const string CLOSE_TAG = ";>";

        /// <summary>
        /// Walk through input, return list of tokens. Each token is either literal text or
        /// the BODY of a placeholder (without the surrounding &lt;# and ;&gt; markers).
        /// </summary>
        public static List<Token> Tokenize(string input)
        {
            var result = new List<Token>();
            if (string.IsNullOrEmpty(input)) return result;

            int i = 0;
            int literalStart = 0;
            while (i < input.Length)
            {
                if (StartsWithAt(input, i, OPEN_TAG))
                {
                    // Flush literal text before this placeholder
                    if (i > literalStart)
                    {
                        result.Add(new Token { IsPlaceholder = false, Text = input.Substring(literalStart, i - literalStart) });
                    }
                    int bodyStart = i + OPEN_TAG.Length;
                    int closeIdx = FindClosingTag(input, bodyStart);
                    if (closeIdx < 0)
                    {
                        // No closing tag — treat the rest as literal to be safe
                        result.Add(new Token { IsPlaceholder = false, Text = input.Substring(i) });
                        return result;
                    }
                    string body = input.Substring(bodyStart, closeIdx - bodyStart);
                    result.Add(new Token { IsPlaceholder = true, Text = body });
                    i = closeIdx + CLOSE_TAG.Length;
                    literalStart = i;
                }
                else
                {
                    i++;
                }
            }
            if (literalStart < input.Length)
            {
                result.Add(new Token { IsPlaceholder = false, Text = input.Substring(literalStart) });
            }
            return result;
        }

        /// <summary>
        /// Starting from <paramref name="bodyStart"/>, find the index of the closing ";>" that
        /// matches the surrounding "&lt;#". Skips over nested "&lt;#...;&gt;" pairs.
        /// Returns -1 if no closing tag found.
        /// </summary>
        public static int FindClosingTag(string input, int bodyStart)
        {
            int depth = 1;
            int i = bodyStart;
            while (i < input.Length)
            {
                if (StartsWithAt(input, i, OPEN_TAG))
                {
                    depth++;
                    i += OPEN_TAG.Length;
                }
                else if (StartsWithAt(input, i, CLOSE_TAG))
                {
                    depth--;
                    if (depth == 0) return i;
                    i += CLOSE_TAG.Length;
                }
                else
                {
                    i++;
                }
            }
            return -1;
        }

        /// <summary>
        /// Split a value template by '|' at top level (not inside &lt;#...;&gt;).
        /// Each option is a self-contained expression to try in order for pipe fallback.
        /// </summary>
        public static List<string> SplitPipeOptions(string input)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(input))
            {
                result.Add(input ?? "");
                return result;
            }

            int depth = 0;
            int start = 0;
            int i = 0;
            while (i < input.Length)
            {
                if (StartsWithAt(input, i, OPEN_TAG))
                {
                    depth++;
                    i += OPEN_TAG.Length;
                }
                else if (StartsWithAt(input, i, CLOSE_TAG))
                {
                    if (depth > 0) depth--;
                    i += CLOSE_TAG.Length;
                }
                else if (input[i] == '|' && depth == 0)
                {
                    result.Add(input.Substring(start, i - start));
                    start = i + 1;
                    i++;
                }
                else
                {
                    i++;
                }
            }
            result.Add(input.Substring(start));
            return result;
        }

        /// <summary>
        /// If a placeholder body looks like "FN(args)", split into function name and raw args string.
        /// Returns false if the body is not a function call (no opening paren or unbalanced).
        /// The args string preserves nested placeholders so the caller can recursively resolve them.
        /// </summary>
        public static bool TryParseFunction(string body, out string functionName, out string argsRaw)
        {
            functionName = null;
            argsRaw = null;
            if (string.IsNullOrEmpty(body)) return false;

            int openParen = body.IndexOf('(');
            if (openParen <= 0) return false;
            if (!body.EndsWith(")")) return false;

            string name = body.Substring(0, openParen).Trim();
            if (!IsValidIdentifier(name)) return false;

            string args = body.Substring(openParen + 1, body.Length - openParen - 2);
            functionName = name;
            argsRaw = args;
            return true;
        }

        /// <summary>
        /// Split function arguments by ',' at top level (not inside &lt;#...;&gt; or string literal).
        /// String literals delimited by " are honored to allow commas inside string args.
        /// </summary>
        public static List<string> SplitArguments(string argsRaw)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(argsRaw))
            {
                return result;
            }

            int depth = 0;
            bool inString = false;
            int start = 0;
            int i = 0;
            while (i < argsRaw.Length)
            {
                char c = argsRaw[i];
                if (inString)
                {
                    if (c == '\\' && i + 1 < argsRaw.Length)
                    {
                        i += 2;
                        continue;
                    }
                    if (c == '"') inString = false;
                    i++;
                    continue;
                }
                if (c == '"')
                {
                    inString = true;
                    i++;
                    continue;
                }
                if (StartsWithAt(argsRaw, i, OPEN_TAG))
                {
                    depth++;
                    i += OPEN_TAG.Length;
                    continue;
                }
                if (StartsWithAt(argsRaw, i, CLOSE_TAG))
                {
                    if (depth > 0) depth--;
                    i += CLOSE_TAG.Length;
                    continue;
                }
                if (c == ',' && depth == 0)
                {
                    result.Add(argsRaw.Substring(start, i - start).Trim());
                    start = i + 1;
                    i++;
                    continue;
                }
                i++;
            }
            result.Add(argsRaw.Substring(start).Trim());
            return result;
        }

        public static bool IsValidIdentifier(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            for (int k = 0; k < s.Length; k++)
            {
                char c = s[k];
                if (!(char.IsLetterOrDigit(c) || c == '_')) return false;
            }
            return !char.IsDigit(s[0]);
        }

        private static bool StartsWithAt(string input, int index, string token)
        {
            if (index + token.Length > input.Length) return false;
            for (int k = 0; k < token.Length; k++)
            {
                if (input[index + k] != token[k]) return false;
            }
            return true;
        }
    }
}

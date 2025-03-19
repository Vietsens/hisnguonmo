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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Common.HtmlString
{
    public class ProcessorString
    {
        public static string InsertFontStyle(string input, FontStyle fontStyle)
        {
            StringBuilder result = new StringBuilder();
            switch (fontStyle)
            {
                case FontStyle.Bold:
                    result.Append("<b>").Append(input).Append("</b>");
                    break;
                case FontStyle.Italic:
                    result.Append("<i>").Append(input).Append("</i>");
                    break;
                case FontStyle.Regular:
                    result.Append(input);
                    break;
                case FontStyle.Strikeout:
                    result.Append("<s>").Append(input).Append("</s>");
                    break;
                case FontStyle.Underline:
                    result.Append("<u>").Append(input).Append("</u>");
                    break;
                default:
                    break;
            }
            return result.ToString();
        }

        public static string InsertColor(string input, Color color)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<color=" + color.Name + ">");
            result.Append(input);
            result.Append("</color>");
            return result.ToString();
        }

        public static string InsertBackColor(string input, Color color)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<backcolor=" + color.Name + ">");
            result.Append(input);
            result.Append("</backcolor>");
            return result.ToString();
        }

        public static string InsertSize(string input, int size)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<size=" + size + ">");
            result.Append(input);
            result.Append("</size>");
            return result.ToString();
        }

        public static string InsertNormalTag(string input, NormalTag.Tag tag, string align)
        {
            StringBuilder result = new StringBuilder();
            switch (tag)
            {
                case NormalTag.Tag.hr:
                    result.Append("<hr>").Append(input).Append("</hr>");
                    break;
                case NormalTag.Tag.i:
                    result.Append("<i>").Append(input).Append("</i>");
                    break;
                case NormalTag.Tag.img:
                    result.Append("<img>").Append(input).Append("</img>");
                    break;
                case NormalTag.Tag.ins:
                    result.Append("<ins>").Append(input).Append("</ins>");
                    break;
                case NormalTag.Tag.li:
                    result.Append("<li>").Append(input).Append("</li>");
                    break;
                case NormalTag.Tag.link:
                    result.Append("<link>").Append(input).Append("</link>");
                    break;
                case NormalTag.Tag.ol:
                    result.Append("<ol>").Append(input).Append("</ol>");
                    break;
                case NormalTag.Tag.small:
                    result.Append("<small>").Append(input).Append("</small>");
                    break;
                case NormalTag.Tag.span:
                    result.Append("<span>").Append(input).Append("</span>");
                    break;
                case NormalTag.Tag.strong:
                    result.Append("<strong>").Append(input).Append("</strong>");
                    break;
                case NormalTag.Tag.style:
                    result.Append("<style>").Append(input).Append("</style>");
                    break;
                case NormalTag.Tag.p:
                    result.Append("<p" + (!String.IsNullOrEmpty(align) ? " align=\"" + align + "\"" : "") + ">").Append(input).Append("</p>");
                    break;
                default:
                    break;
            }
            return result.ToString();
        }

        public static string InsertSpacialTag(string input, SpacialTag.Tag tag)
        {
            StringBuilder result = new StringBuilder();
            result.Append(input);
            switch (tag)
            {
                case SpacialTag.Tag.Br:
                    result.Append("<br>");
                    break;
                case SpacialTag.Tag.Nbsp:
                    result.Append("&nbsp;");
                    break;
                default:
                    break;
            }

            return result.ToString();
        }
    }
}

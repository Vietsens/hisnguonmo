/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Xml831
{
    /// <summary>
    /// Serialize model &lt;DATA&gt; QĐ831 ra chuỗi/file XML: UTF-8, có khai báo &lt;?xml?&gt;, indent,
    /// KHÔNG kèm namespace xsi/xsd (đặt namespace rỗng) — giống định dạng file mẫu.
    /// </summary>
    internal static class Ksk831Serializer
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Data));

        /// <summary>Model -&gt; chuỗi XML.</summary>
        internal static string ToXml(Data data)
        {
            if (data == null) return null;
            Ksk831Normalizer.FillEmpty(data);   // mọi thẻ luôn hiển thị (rỗng nếu không có dữ liệu)
            var ns = new XmlSerializerNamespaces();
            ns.Add("", ""); // bỏ xmlns:xsi / xmlns:xsd
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                Encoding = new UTF8Encoding(false),   // UTF-8 không BOM
                OmitXmlDeclaration = false
            };
            using (var ms = new MemoryStream())
            {
                using (var xw = XmlWriter.Create(ms, settings))
                {
                    Serializer.Serialize(xw, data, ns);
                    xw.Flush();
                }
                string xml = new UTF8Encoding(false).GetString(ms.ToArray());
                return ExpandEmptyTags(xml);
            }
        }

        /// <summary>Đổi thẻ rỗng tự đóng &lt;TAG /&gt; thành &lt;TAG&gt;&lt;/TAG&gt; cho khớp định dạng file mẫu (thẻ không có thuộc tính).</summary>
        private static string ExpandEmptyTags(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return xml;
            return Regex.Replace(xml, @"<([A-Za-z0-9_:\.\-]+)\s*/>", "<$1></$1>");
        }

        /// <summary>Model -&gt; ghi ra file XML (UTF-8 không BOM).</summary>
        internal static void ToFile(Data data, string filePath)
        {
            string xml = ToXml(data);
            if (xml == null) return;
            File.WriteAllText(filePath, xml, new UTF8Encoding(false));
        }
    }
}

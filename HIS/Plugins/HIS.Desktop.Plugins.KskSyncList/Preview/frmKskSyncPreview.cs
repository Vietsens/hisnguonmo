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
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.KskSyncList.Preview
{
    public partial class frmKskSyncPreview : DevExpress.XtraEditors.XtraForm
    {
        private readonly V_HIS_KSK_SYNC data;
        private readonly string content;

        public frmKskSyncPreview(V_HIS_KSK_SYNC data, string content)
        {
            InitializeComponent();
            this.data = data;
            this.content = content;
            SetIcon();
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmKskSyncPreview_Load(object sender, EventArgs e)
        {
            try
            {
                lblKskTypeValue.Text = SafeGet("KSK_TYPE_NAME");
                lblPatientValue.Text = string.Format("{0} - {1}", SafeGet("TDL_PATIENT_NAME"), SafeGet("TDL_PATIENT_CODE"));
                lblConclusionTimeValue.Text = FormatTime("CONCLUSION_TIME");
                lblStatusValue.Text = GetStatusText();

                // Hien thi dep: XML/JSON thut dong + to mau the (chi cho hien thi, khong doi data day len).
                RenderColored(PrettyFormat(content));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetStatusText()
        {
            try
            {
                int t = 0;
                try { t = Convert.ToInt32(data.SYNC_RESULT_TYPE); }
                catch { t = 0; }
                if (t == 0) t = 1;
                switch (t)
                {
                    case 2: return "Đã đồng bộ";
                    case 3: return "Thất bại";
                    case 4: return "Có chỉnh sửa";
                    default: return "Chưa đồng bộ";
                }
            }
            catch { return "Chưa đồng bộ"; }
        }

        private string SafeGet(string prop)
        {
            try
            {
                if (data == null) return "";
                var p = data.GetType().GetProperty(prop);
                var v = p != null ? p.GetValue(data, null) : null;
                return v == null ? "" : v.ToString();
            }
            catch { return ""; }
        }

        private string FormatTime(string prop)
        {
            try { return Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(SafeGet(prop))); }
            catch { return ""; }
        }

        // Mau hien thi
        private static readonly Color CLR_BRACKET = Color.Gray;                         // < > / ?
        private static readonly Color CLR_TAGNAME = Color.FromArgb(0, 0, 205);          // ten the (xanh duong)
        private static readonly Color CLR_TEXT = Color.FromArgb(163, 21, 21);           // gia tri (do sam)

        /// <summary>Đổ nội dung XML đã format vào RichTextBox và tô màu thẻ/giá trị.</summary>
        private void RenderColored(string formatted)
        {
            try
            {
                rtxtContent.Clear();
                if (string.IsNullOrEmpty(formatted)) return;

                // Khong phai XML (vd JSON) -> hien thi thuong, khong to mau.
                if (!formatted.TrimStart().StartsWith("<"))
                {
                    rtxtContent.Text = formatted;
                    return;
                }

                string[] parts = Regex.Split(formatted, "(<[^>]*>)");
                foreach (string part in parts)
                {
                    if (string.IsNullOrEmpty(part)) continue;
                    if (part[0] == '<') AppendTag(part);
                    else AppendColored(part, CLR_TEXT);
                }
                rtxtContent.SelectionStart = 0;
                rtxtContent.SelectionLength = 0;
                rtxtContent.ScrollToCaret();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                rtxtContent.Text = formatted;
            }
        }

        // Tô 1 thẻ: dấu <, /, ? màu xám; tên thẻ màu xanh; phần còn lại (>, />) màu xám.
        private void AppendTag(string tag)
        {
            Match m = Regex.Match(tag, @"^<[/\?]?([A-Za-z0-9_:\.\-]+)");
            if (!m.Success)
            {
                AppendColored(tag, CLR_TAGNAME);
                return;
            }
            int nameStart = m.Groups[1].Index;
            int nameEnd = nameStart + m.Groups[1].Length;
            AppendColored(tag.Substring(0, nameStart), CLR_BRACKET);
            AppendColored(tag.Substring(nameStart, nameEnd - nameStart), CLR_TAGNAME);
            AppendColored(tag.Substring(nameEnd), CLR_BRACKET);
        }

        private void AppendColored(string text, Color color)
        {
            if (string.IsNullOrEmpty(text)) return;
            rtxtContent.SelectionStart = rtxtContent.TextLength;
            rtxtContent.SelectionLength = 0;
            rtxtContent.SelectionColor = color;
            rtxtContent.AppendText(text);
        }

        /// <summary>Format XML/JSON có thụt dòng để hiển thị đẹp (chỉ dùng cho xem trước).</summary>
        private static string PrettyFormat(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return content;
            string s = content.TrimStart();
            try
            {
                if (s.StartsWith("<"))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(content);
                    StringBuilder sb = new StringBuilder();
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "    ",
                        NewLineChars = "\r\n",
                        NewLineHandling = NewLineHandling.Replace,
                        OmitXmlDeclaration = false
                    };
                    using (XmlWriter w = XmlWriter.Create(sb, settings))
                    {
                        doc.Save(w);
                    }
                    return sb.ToString();
                }
                if (s.StartsWith("{") || s.StartsWith("["))
                {
                    var token = Newtonsoft.Json.Linq.JToken.Parse(content);
                    return token.ToString(Newtonsoft.Json.Formatting.Indented);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return content;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try { this.Close(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
    }
}

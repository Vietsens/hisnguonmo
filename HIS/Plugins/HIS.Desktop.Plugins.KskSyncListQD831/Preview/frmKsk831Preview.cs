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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Preview
{
    /// <summary>
    /// Xem trước XML QĐ831 của 1 hồ sơ (V_HIS_KSK_PROFILE) — tương tự frmKskSyncPreview của KskSyncList:
    /// header thông tin + RichTextBox thụt dòng, tô màu thẻ/giá trị.
    /// </summary>
    public partial class frmKsk831Preview : DevExpress.XtraEditors.XtraForm
    {
        private readonly V_HIS_KSK_PROFILE data;
        private readonly string content;

        public frmKsk831Preview(V_HIS_KSK_PROFILE data, string content)
        {
            InitializeComponent();
            this.data = data;
            this.content = content;
        }

        private void frmKsk831Preview_Load(object sender, EventArgs e)
        {
            try
            {
                lblPatientValue.Text = string.Format("{0} - {1}", SafeGet("TDL_PATIENT_NAME"), SafeGet("TDL_PATIENT_CODE"));
                lblConclusionTimeValue.Text = FormatTime("CONCLUSION_TIME");
                lblStatusValue.Text = GetStatusText();

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
        private static readonly Color CLR_BRACKET = Color.Gray;
        private static readonly Color CLR_TAGNAME = Color.FromArgb(0, 0, 205);
        private static readonly Color CLR_TEXT = Color.FromArgb(163, 21, 21);

        private void RenderColored(string formatted)
        {
            try
            {
                rtxtContent.Clear();
                if (string.IsNullOrEmpty(formatted)) return;
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

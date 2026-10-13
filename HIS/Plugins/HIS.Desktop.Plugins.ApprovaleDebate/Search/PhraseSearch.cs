using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovaleDebate.Search
{
    /// <summary>
    /// Chuan hoa chuoi tim kiem: chu thuong, bo dau tieng Viet, gom khoang trang.
    /// Go "tong phan tich" hay "Tổng phân tích" deu khop "Tổng phân tích tế bào máu".
    /// </summary>
    internal static class PhraseSearchText
    {
        internal const string FIND_NULL_PROMPT = "Nhập chuỗi tìm kiếm ...";

        internal static string Normalize(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                return "";
            string decomposed = text.Trim().ToLower().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(decomposed.Length);
            bool lastSpace = false;
            foreach (char c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                    continue;
                if (Char.IsWhiteSpace(c))
                {
                    if (!lastSpace)
                        sb.Append(' ');
                    lastSpace = true;
                    continue;
                }
                lastSpace = false;
                sb.Append(c);
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Chuoi nguoi dung go: tim ca cum tu, bo ngoac kep neu nguoi dung quen tay go theo cu phap cu.
        /// </summary>
        internal static string NormalizeKey(string findText)
        {
            return Normalize((findText ?? "").Trim().Trim('"'));
        }

        internal static LabelControl CreateSummaryLabel(Control host)
        {
            LabelControl lbl = new LabelControl();
            lbl.AllowHtmlString = true;
            lbl.AutoSizeMode = LabelAutoSizeMode.Vertical;
            lbl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            lbl.Appearance.ForeColor = Color.FromArgb(0, 70, 140);
            lbl.Appearance.Options.UseForeColor = true;
            lbl.Padding = new Padding(4, 4, 4, 4);
            lbl.Dock = DockStyle.Bottom;
            lbl.Visible = false;
            host.Controls.Add(lbl);
            //Dock Bottom phai duoc dock truoc control Fill => dua xuong cuoi z-order
            lbl.SendToBack();
            return lbl;
        }

        internal static void ShowSummary(LabelControl lbl, string key, string summary)
        {
            if (lbl == null)
                return;
            if (String.IsNullOrEmpty(key))
            {
                lbl.Visible = false;
                return;
            }
            lbl.Text = String.IsNullOrEmpty(summary) ? "<b>Không tìm thấy kết quả phù hợp</b>" : summary;
            lbl.Visible = true;
        }
    }

    /// <summary>
    /// O tim kiem luon hien cua TreeList: tim ca cum tu (khong tach tung tu), khong phan biet hoa thuong va dau,
    /// giu dong cha (moc thoi gian, loai, phieu) va dong con (chi so xet nghiem) cua dong khop, hien dong dem ket qua.
    /// </summary>
    internal class TreeListPhraseSearch
    {
        private readonly TreeList tree;
        private readonly Func<TreeListNode, string> getNodeText;
        private readonly Func<List<TreeListNode>, string> buildSummary;
        private readonly LabelControl lblSummary;
        private readonly Dictionary<TreeListNode, bool> matchCache = new Dictionary<TreeListNode, bool>();
        private string cacheKey;

        /// <param name="getNodeText">Noi dung duoc tim cua 1 dong (rong = dong khong tham gia tim)</param>
        /// <param name="summaryHost">Control chua dong dem ket qua (dock duoi cung); null = khong hien dong dem</param>
        /// <param name="buildSummary">Dung dong dem tu cac dong tu khop</param>
        internal TreeListPhraseSearch(TreeList tree, Func<TreeListNode, string> getNodeText, Control summaryHost, Func<List<TreeListNode>, string> buildSummary)
        {
            this.tree = tree;
            this.getNodeText = getNodeText;
            this.buildSummary = buildSummary;

            tree.OptionsBehavior.EnableFiltering = true;
            tree.OptionsFilter.FilterMode = FilterMode.Extended;
            tree.OptionsFind.AllowFindPanel = true;
            tree.OptionsFind.AlwaysVisible = true;
            tree.OptionsFind.HighlightFindResults = true;
            tree.OptionsFind.FindNullPrompt = PhraseSearchText.FIND_NULL_PROMPT;
            tree.OptionsFind.ShowFindButton = false;
            tree.OptionsFind.ShowClearButton = false;
            tree.FilterNode += this.tree_FilterNode;
            tree.ColumnFilterChanged += this.tree_ColumnFilterChanged;

            if (summaryHost != null && buildSummary != null)
                this.lblSummary = PhraseSearchText.CreateSummaryLabel(summaryHost);
        }

        /// <summary>
        /// Goi sau khi nap lai du lieu de cap nhat dong dem theo chuoi dang tim.
        /// </summary>
        internal void UpdateSummary()
        {
            try
            {
                this.matchCache.Clear();
                string key = PhraseSearchText.NormalizeKey(this.tree.FindFilterText);
                this.cacheKey = key;
                if (this.lblSummary == null)
                    return;
                string summary = null;
                if (!String.IsNullOrEmpty(key))
                {
                    List<TreeListNode> matched = new List<TreeListNode>();
                    foreach (TreeListNode node in this.tree.GetNodeList())
                    {
                        if (this.IsMatch(node, key))
                            matched.Add(node);
                    }
                    summary = matched.Count > 0 ? this.buildSummary(matched) : null;
                }
                PhraseSearchText.ShowSummary(this.lblSummary, key, summary);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tree_ColumnFilterChanged(object sender, EventArgs e)
        {
            this.UpdateSummary();
        }

        private void tree_FilterNode(object sender, FilterNodeEventArgs e)
        {
            try
            {
                string key = PhraseSearchText.NormalizeKey(this.tree.FindFilterText);
                if (String.IsNullOrEmpty(key) || e.Node == null)
                    return;
                if (key != this.cacheKey)
                {
                    this.matchCache.Clear();
                    this.cacheKey = key;
                }
                e.Node.Visible = this.IsMatch(e.Node, key) || this.HasMatchedAncestor(e.Node, key) || this.HasMatchedDescendant(e.Node, key);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsMatch(TreeListNode node, string key)
        {
            bool result;
            if (this.matchCache.TryGetValue(node, out result))
                return result;
            string text = this.getNodeText(node);
            result = !String.IsNullOrEmpty(text) && PhraseSearchText.Normalize(text).Contains(key);
            this.matchCache[node] = result;
            return result;
        }

        private bool HasMatchedAncestor(TreeListNode node, string key)
        {
            TreeListNode parent = node.ParentNode;
            while (parent != null)
            {
                if (this.IsMatch(parent, key))
                    return true;
                parent = parent.ParentNode;
            }
            return false;
        }

        private bool HasMatchedDescendant(TreeListNode node, string key)
        {
            foreach (TreeListNode child in node.Nodes)
            {
                if (this.IsMatch(child, key) || this.HasMatchedDescendant(child, key))
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Ban GridView cua TreeListPhraseSearch (luoi phang, khong co dong cha/con).
    /// </summary>
    internal class GridViewPhraseSearch
    {
        private readonly GridView view;
        private readonly Func<object, string> getRowText;
        private readonly Func<List<object>, string> buildSummary;
        private readonly LabelControl lblSummary;

        internal GridViewPhraseSearch(GridView view, Func<object, string> getRowText, Control summaryHost, Func<List<object>, string> buildSummary)
        {
            this.view = view;
            this.getRowText = getRowText;
            this.buildSummary = buildSummary;

            view.OptionsFind.AllowFindPanel = true;
            view.OptionsFind.AlwaysVisible = true;
            view.OptionsFind.HighlightFindResults = true;
            view.OptionsFind.FindNullPrompt = PhraseSearchText.FIND_NULL_PROMPT;
            view.OptionsFind.ShowFindButton = false;
            view.OptionsFind.ShowClearButton = false;
            view.OptionsFind.FindDelay = 100;
            view.CustomRowFilter += this.view_CustomRowFilter;
            view.ColumnFilterChanged += this.view_ColumnFilterChanged;

            if (summaryHost != null && buildSummary != null)
                this.lblSummary = PhraseSearchText.CreateSummaryLabel(summaryHost);
        }

        internal void UpdateSummary()
        {
            try
            {
                if (this.lblSummary == null)
                    return;
                string key = PhraseSearchText.NormalizeKey(this.view.FindFilterText);
                string summary = null;
                IList rows = this.view.DataSource as IList;
                if (!String.IsNullOrEmpty(key) && rows != null)
                {
                    List<object> matched = new List<object>();
                    foreach (object row in rows)
                    {
                        if (this.IsMatch(row, key))
                            matched.Add(row);
                    }
                    summary = matched.Count > 0 ? this.buildSummary(matched) : null;
                }
                PhraseSearchText.ShowSummary(this.lblSummary, key, summary);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void view_ColumnFilterChanged(object sender, EventArgs e)
        {
            this.UpdateSummary();
        }

        private void view_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            try
            {
                string key = PhraseSearchText.NormalizeKey(this.view.FindFilterText);
                if (String.IsNullOrEmpty(key))
                    return;
                IList rows = this.view.DataSource as IList;
                if (rows == null || e.ListSourceRow < 0 || e.ListSourceRow >= rows.Count)
                    return;
                e.Visible = this.IsMatch(rows[e.ListSourceRow], key);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsMatch(object row, string key)
        {
            string text = row != null ? this.getRowText(row) : null;
            return !String.IsNullOrEmpty(text) && PhraseSearchText.Normalize(text).Contains(key);
        }
    }
}

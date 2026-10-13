using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.ADO;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist
{
    /// <summary>
    /// Tab Ket qua CLS: cay mo san theo moc thoi gian chi dinh -> dich vu -> chi so xet nghiem.
    /// </summary>
    public class UCSubclinicalResult : UserControl
    {
        private TreeList treeResult;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit;
        private TreeListColumn tc_Name;
        private TreeListColumn tc_Result;
        private TreeListColumn tc_Unit;
        private TreeListColumn tc_NormalRange;
        private TreeListColumn tc_ResultTime;
        private TreeListPhraseSearch phraseSearch;

        public UCSubclinicalResult()
        {
            try
            {
                this.InitTree();
                this.phraseSearch = new TreeListPhraseSearch(this.treeResult, this.GetSearchText, this, this.BuildSearchSummary);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitTree()
        {
            this.repositoryItemMemoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.treeResult = new TreeList();
            ((ISupportInitialize)this.treeResult).BeginInit();
            this.SuspendLayout();

            this.tc_Name = this.CreateColumn("Mốc chỉ định / Dịch vụ / Chỉ số", "NAME", 0, 330);
            this.tc_Result = this.CreateColumn("Kết quả", "RESULT", 1, 300);
            this.tc_Unit = this.CreateColumn("Đơn vị", "UNIT", 2, 80);
            this.tc_NormalRange = this.CreateColumn("Bình thường", "NORMAL_RANGE", 3, 110);
            this.tc_ResultTime = this.CreateColumn("Thời gian có kết quả", "RESULT_TIME", 4, 120);

            this.treeResult.Columns.AddRange(new TreeListColumn[] { this.tc_Name, this.tc_Result, this.tc_Unit, this.tc_NormalRange, this.tc_ResultTime });
            this.treeResult.RepositoryItems.Add(this.repositoryItemMemoEdit);
            this.treeResult.Dock = DockStyle.Fill;
            this.treeResult.KeyFieldName = "KEY_ID";
            this.treeResult.ParentFieldName = "PARENT_KEY_ID";
            this.treeResult.Name = "treeResult";
            this.treeResult.OptionsBehavior.Editable = false;
            this.treeResult.OptionsBehavior.AutoPopulateColumns = false;
            this.treeResult.OptionsView.AutoWidth = true;
            this.treeResult.OptionsView.ShowIndicator = false;
            this.treeResult.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treeResult.OptionsFind.FindDelay = 100;
            this.treeResult.NodeCellStyle += this.treeResult_NodeCellStyle;

            this.Controls.Add(this.treeResult);
            this.Name = "UCSubclinicalResult";
            ((ISupportInitialize)this.treeResult).EndInit();
            this.ResumeLayout(false);
        }

        private TreeListColumn CreateColumn(string caption, string fieldName, int visibleIndex, int width)
        {
            TreeListColumn column = new TreeListColumn();
            column.Caption = caption;
            column.FieldName = fieldName;
            column.Name = "tc_" + fieldName;
            column.ColumnEdit = this.repositoryItemMemoEdit;
            column.AppearanceCell.Options.UseTextOptions = true;
            column.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            column.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            column.AppearanceHeader.Options.UseTextOptions = true;
            column.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            column.OptionsColumn.AllowEdit = false;
            column.Visible = true;
            column.VisibleIndex = visibleIndex;
            column.Width = width;
            return column;
        }

        public void ReLoad(List<SubclinicalResultADO> data)
        {
            try
            {
                this.treeResult.BeginUpdate();
                this.treeResult.DataSource = new BindingList<SubclinicalResultADO>(data ?? new List<SubclinicalResultADO>());
                this.treeResult.ExpandAll();
                this.treeResult.EndUpdate();
                this.phraseSearch.UpdateSummary();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private SubclinicalResultADO GetRecord(TreeListNode node)
        {
            return node != null ? this.treeResult.GetDataRecordByNode(node) as SubclinicalResultADO : null;
        }

        private string GetSearchText(TreeListNode node)
        {
            SubclinicalResultADO data = this.GetRecord(node);
            return data != null ? data.SEARCH_TEXT : null;
        }

        /// <summary>
        /// "Tim thay N lan chi dinh - ngay ...": dem dich vu khop (dich vu tu khop hoac co chi so khop).
        /// </summary>
        private string BuildSearchSummary(List<TreeListNode> matchedNodes)
        {
            List<SubclinicalResultADO> services = new List<SubclinicalResultADO>();
            foreach (TreeListNode node in matchedNodes)
            {
                SubclinicalResultADO data = this.GetRecord(node);
                if (data == null)
                    continue;
                if (data.NODE_LEVEL == SubclinicalResultADO.LEVEL__TEST_INDEX)
                    data = this.GetRecord(node.ParentNode);
                if (data != null && data.NODE_LEVEL == SubclinicalResultADO.LEVEL__SERVICE && !services.Contains(data))
                    services.Add(data);
            }
            if (services.Count == 0)
                return null;
            string dates = String.Join(", ", services.GroupBy(o => o.INTRUCTION_DATE)
                .Select(g => g.Count() > 1 ? String.Format("{0} ({1} lần)", g.Key, g.Count()) : g.Key));
            return String.Format("<b>Tìm thấy {0} lần chỉ định</b> - ngày {1}", services.Count, dates);
        }

        private void treeResult_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                SubclinicalResultADO data = this.GetRecord(e.Node);
                if (data == null)
                    return;
                if (data.NODE_LEVEL == SubclinicalResultADO.LEVEL__INTRUCTION_TIME)
                {
                    e.Appearance.FontStyleDelta = FontStyle.Bold;
                    e.Appearance.ForeColor = Color.FromArgb(0, 70, 140);
                }
                else if (data.NODE_LEVEL == SubclinicalResultADO.LEVEL__SERVICE)
                {
                    if (e.Column == this.tc_Result && data.IS_NO_RESULT)
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Italic;
                        e.Appearance.ForeColor = Color.Gray;
                    }
                    else if (e.Column == this.tc_Name)
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Bold;
                    }
                }
                else if (e.Column == this.tc_Result && !String.IsNullOrEmpty(data.RESULT_CODE))
                {
                    //Nhu man xem chi so xet nghiem (HIS.Desktop.Plugins.SereServTein): 1 = thap (xanh duong), 2 = cao (do)
                    if (data.RESULT_CODE == "1")
                    {
                        e.Appearance.ForeColor = Color.Blue;
                        e.Appearance.FontStyleDelta = FontStyle.Bold;
                    }
                    else if (data.RESULT_CODE == "2")
                    {
                        e.Appearance.ForeColor = Color.Red;
                        e.Appearance.FontStyleDelta = FontStyle.Bold;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

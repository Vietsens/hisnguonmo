using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.UC
{
    /// <summary>
    /// UserControl gen dong 1 nhom benh (HIS_DISEASE_TYPE).
    /// Layout: DISEASE_TYPE_NAME (bold) phia trai, cac item ngang ngay sau (trai → phai).
    /// Toi da 5 cot/dong, xuong dong tu dong. Fill du chieu rong parent.
    /// </summary>
    public class ucDiseaseDetailGroup : UserControl
    {
        #region Fields
        private const int MAX_COLUMNS = 5;
        private const int ROW_HEIGHT = 30;
        private const int TITLE_WIDTH = 260;

        private Dictionary<long, CheckEdit> checkMapping = new Dictionary<long, CheckEdit>();
        private Dictionary<long, Control> textMapping = new Dictionary<long, Control>();

        private string diseaseTypeName;
        private List<V_HIS_DISEASE_DETAIL> details;
        #endregion

        #region Properties
        public Dictionary<long, CheckEdit> CheckMapping { get { return checkMapping; } }
        public Dictionary<long, Control> TextMapping { get { return textMapping; } }
        #endregion

        #region Constructor
        public ucDiseaseDetailGroup(string diseaseTypeName, List<V_HIS_DISEASE_DETAIL> details)
        {
            this.diseaseTypeName = diseaseTypeName ?? "";
            this.details = details ?? new List<V_HIS_DISEASE_DETAIL>();
            BuildUI();
        }
        #endregion

        #region Build UI
        private void BuildUI()
        {
            try
            {
                this.SuspendLayout();
                this.Dock = DockStyle.Top;

                var sortedDetails = (details ?? new List<V_HIS_DISEASE_DETAIL>())
                    .OrderBy(d => d.NUM_ORDER_DETAIL).ToList();

                // === Tinh layout ===
                // Moi item chiem 1 slot. IS_OTHER text dung ColSpan de fill rong hon.
                // Neu tong items <= MAX_COLUMNS: tat ca tren 1 dong, IS_OTHER text ColSpan fill phan con lai.
                // Neu tong items > MAX_COLUMNS: chia dong, IS_OTHER text chiem 2 slots.
                int totalItems = sortedDetails.Count;

                // Danh dau IS_OTHER text (wide) — can nhieu khong gian hon
                // - IS_OTHER text non-numeric (vd "Khac (ghi ro)") luon wide
                // - IS_CHECKBOX + IS_OTHER co caption DAI (>20 ky tu, vd "Neu hut ghi so luong dieu .../ngay")
                //   → caption + textbox 60px chac chan vuot cell width 1 slot → can wide de tranh clip
                const int LONG_CAPTION_THRESHOLD = 20;
                var isWideFlags = new bool[totalItems];
                for (int i = 0; i < totalItems; i++)
                {
                    bool isCheckbox = (sortedDetails[i].IS_CHECKBOX ?? 0) == 1;
                    bool isOther = (sortedDetails[i].IS_OTHER ?? 0) == 1;
                    string name = (sortedDetails[i].NAME ?? "").Trim();
                    bool isNumeric = isOther && IsNumericField(name);
                    bool hasLongCaption = isCheckbox && isOther && name.Length > LONG_CAPTION_THRESHOLD;
                    isWideFlags[i] = (isOther && !isNumeric) || hasLongCaption;
                }

                // Tinh tong slots can thiet (wide=2, thuong=1)
                int totalSlotsNeeded = 0;
                for (int i = 0; i < totalItems; i++)
                    totalSlotsNeeded += isWideFlags[i] ? 2 : 1;

                // Neu vua du 1 dong (totalItems <= MAX_COLUMNS): tat ca chiem 1 slot
                // IS_OTHER text se dung ColSpan trong grid thay vi chiem 2 slots
                bool fitOneLine = (totalItems <= MAX_COLUMNS);

                var layoutItems = new List<LayoutItem>();
                int currentSlot = 0;
                for (int i = 0; i < totalItems; i++)
                {
                    bool isWideOther = isWideFlags[i];
                    int slotSize = (isWideOther && !fitOneLine) ? 2 : 1;
                    bool isLast = (i == totalItems - 1);

                    int colInRow = currentSlot % MAX_COLUMNS;
                    int rowIdx = currentSlot / MAX_COLUMNS;

                    // Khong du slot tren dong → xuong dong moi
                    if (slotSize > 1 && colInRow + slotSize > MAX_COLUMNS)
                    {
                        currentSlot = (rowIdx + 1) * MAX_COLUMNS;
                        colInRow = 0;
                        rowIdx = currentSlot / MAX_COLUMNS;
                    }

                    int remainSlots = MAX_COLUMNS - colInRow;
                    int span = slotSize;

                    if (isWideOther)
                    {
                        if (fitOneLine)
                        {
                            // 1 dong: IS_OTHER text ColSpan fill phan con lai
                            span = MAX_COLUMNS - colInRow;
                            // Tru bot cho cac item sau (neu co)
                            int itemsAfter = totalItems - i - 1;
                            if (itemsAfter > 0 && span > 1)
                                span = Math.Max(span - itemsAfter, 1);
                        }
                        else
                        {
                            // Nhieu dong: tinh nhu cu
                            int slotsAfterMe = remainSlots - slotSize;
                            bool nextFitsOnSameLine = false;
                            if (!isLast && slotsAfterMe > 0)
                            {
                                bool nextIsWide = isWideFlags[i + 1];
                                int nextSize = nextIsWide ? 2 : 1;
                                nextFitsOnSameLine = (nextSize <= slotsAfterMe);
                            }
                            if (isLast || !nextFitsOnSameLine)
                                span = remainSlots;
                        }
                    }

                    layoutItems.Add(new LayoutItem
                    {
                        Detail = sortedDetails[i],
                        Row = rowIdx,
                        Col = colInRow,
                        ColSpan = span,
                        IsWideOther = isWideOther
                    });
                    currentSlot += span;
                }

                int rowCount = layoutItems.Count > 0 ? layoutItems.Max(li => li.Row) + 1 : 1;

                // === 1 TableLayoutPanel chung: cot 0 = title (Absolute), cot 1..5 = items ===
                var table = new TableLayoutPanel();
                table.Dock = DockStyle.Fill;
                table.AutoSize = true;
                table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                table.Margin = new Padding(0);
                // Pad ben phai de tranh scrollbar doc cua XtraScrollableControl che item cot cuoi
                table.Padding = new Padding(0, 0, 14, 0);
                table.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

                table.ColumnCount = 1 + MAX_COLUMNS;
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, TITLE_WIDTH));
                for (int c = 0; c < MAX_COLUMNS; c++)
                    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / MAX_COLUMNS));

                table.RowCount = rowCount;
                for (int r = 0; r < rowCount; r++)
                    table.RowStyles.Add(new RowStyle(SizeType.Absolute, ROW_HEIGHT));

                // Title: cot 0, row 0, span all rows, co duong ke phai phan cach
                var lblTitle = new LabelControl();
                lblTitle.Text = this.diseaseTypeName;
                lblTitle.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                lblTitle.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                lblTitle.AutoSizeMode = LabelAutoSizeMode.None;
                lblTitle.Dock = DockStyle.Fill;
                lblTitle.Padding = new Padding(2, 0, 6, 0);
                lblTitle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                table.Controls.Add(lblTitle, 0, 0);
                if (rowCount > 1)
                    table.SetRowSpan(lblTitle, rowCount);

                // Items: cot 1..5, ColSpan cho wide IS_OTHER
                foreach (var li in layoutItems)
                {
                    var detail = li.Detail;
                    bool isCheckbox = (detail.IS_CHECKBOX ?? 0) == 1;
                    bool isOther = (detail.IS_OTHER ?? 0) == 1;

                    Control itemControl = null;
                    if (isCheckbox && isOther)
                        itemControl = CreateCheckWithTextItem(detail);
                    else if (isCheckbox)
                        itemControl = CreateCheckOnlyItem(detail);
                    else if (isOther)
                        itemControl = CreateTextOnlyItem(detail);

                    if (itemControl != null)
                    {
                        itemControl.Dock = DockStyle.Fill;
                        // Sat nhau theo chieu ngang (left/right = 0); giu 1px theo chieu doc de tach 2 dong
                        itemControl.Margin = new Padding(0, 1, 0, 1);
                        int tableCol = li.Col + 1;
                        table.Controls.Add(itemControl, tableCol, li.Row);
                        if (li.ColSpan > 1)
                            table.SetColumnSpan(itemControl, li.ColSpan);
                    }
                }

                this.Height = rowCount * ROW_HEIGHT + 2;
                this.Controls.Add(table);
                this.ResumeLayout(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Create Item Controls
        /// <summary>IS_CHECKBOX=1 only — WordWrap caption</summary>
        private Control CreateCheckOnlyItem(V_HIS_DISEASE_DETAIL detail)
        {
            var chk = new CheckEdit();
            chk.Name = "chk_Disease_" + detail.ID;
            chk.Properties.Caption = (detail.NAME ?? "").Trim();
            chk.Properties.AutoWidth = true;
            chk.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Near;
            chk.Tag = detail.ID;
            chk.Dock = DockStyle.Fill;
            checkMapping[detail.ID] = chk;
            return chk;
        }

        /// <summary>IS_CHECKBOX=1 + IS_OTHER=1: CheckEdit sat TextEdit, caption WordWrap.
        /// Name co tu khoa so/ngay/nam → TextEdit nho, chi nhap so, disable khi chua tick.</summary>
        private Control CreateCheckWithTextItem(V_HIS_DISEASE_DETAIL detail)
        {
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Margin = new Padding(0);

            string caption = (detail.NAME ?? "").Trim();
            bool isNumeric = IsNumericField(caption);

            var chk = new CheckEdit();
            chk.Name = "chk_Disease_" + detail.ID;
            chk.Properties.Caption = caption;
            chk.Properties.AutoWidth = true;
            chk.Tag = detail.ID;
            chk.Dock = DockStyle.Left;
            int captionWidth = TextRenderer.MeasureText(caption, chk.Font).Width + 22;
            chk.Width = captionWidth;
            checkMapping[detail.ID] = chk;

            var txt = new TextEdit();
            txt.Name = "txt_Disease_" + detail.ID;
            txt.Tag = detail.ID;
            txt.Properties.MaxLength = 500;
            textMapping[detail.ID] = txt;

            if (isNumeric)
            {
                // O nho, chi nhap so, disable mac dinh
                txt.Dock = DockStyle.Left;
                txt.Width = 60;
                txt.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                txt.Properties.Mask.EditMask = "n0";
                txt.Properties.Mask.UseMaskAsDisplayFormat = true;
                txt.Enabled = false;

                // Enable/disable theo checkbox
                chk.CheckedChanged += (s, ev) =>
                {
                    txt.Enabled = chk.Checked;
                    if (!chk.Checked) txt.Text = "";
                };
            }
            else
            {
                txt.Dock = DockStyle.Fill;
            }

            // Fill truoc, Left sau — WinForms dock order
            panel.Controls.Add(txt);
            panel.Controls.Add(chk);
            return panel;
        }

        /// <summary>IS_OTHER=1 only (no checkbox): Label WordWrap + TextEdit fill</summary>
        private Control CreateTextOnlyItem(V_HIS_DISEASE_DETAIL detail)
        {
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Margin = new Padding(0);

            string labelText = (detail.NAME ?? "").Trim();

            var txt = new TextEdit();
            txt.Name = "txt_Disease_" + detail.ID;
            txt.Tag = detail.ID;
            txt.Dock = DockStyle.Fill;
            txt.Properties.MaxLength = 500;
            textMapping[detail.ID] = txt;

            // Fill truoc, Left sau
            panel.Controls.Add(txt);

            if (!string.IsNullOrEmpty(labelText))
            {
                var lbl = new LabelControl();
                lbl.Text = labelText;
                lbl.Dock = DockStyle.Left;
                lbl.AutoSizeMode = LabelAutoSizeMode.None;
                lbl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                lbl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                int lblWidth = TextRenderer.MeasureText(labelText, lbl.Font).Width + 8;
                lbl.Width = lblWidth;
                panel.Controls.Add(lbl);
            }

            return panel;
        }
        /// <summary>
        /// Kiem tra NAME co phai truong so (so luong, so nam, ngay, phut...).
        /// VD: "Số năm đã hút", "Số lượng điếu", "Thời gian trung bình", "Mất răng (số lượng)"
        /// </summary>
        private bool IsNumericField(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string lower = name.ToLower();
            string[] keywords = new string[]
            {
                "số",
                "thời gian", "phút", "tiếng", "giờ",
                "ngày", "năm", "tháng",
                "mất răng"
            };
            foreach (var kw in keywords)
            {
                if (lower.Contains(kw)) return true;
            }
            return false;
        }
        #endregion

        #region Public API
        public void LoadResults(List<HIS_DISEASE_DETAIL_RESULT> results)
        {
            try
            {
                if (results == null) return;
                foreach (var result in results)
                {
                    if (result.DISEASE_DETAIL_ID == null) continue;
                    long detailId = result.DISEASE_DETAIL_ID.Value;
                    if (checkMapping.ContainsKey(detailId))
                        checkMapping[detailId].Checked = (result.IS_CHECK ?? 0) == 1;
                    if (textMapping.ContainsKey(detailId))
                        textMapping[detailId].Text = result.OTHER ?? "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public List<ADO.DiseaseDetailResultADO> CollectResults(long? kskGeneralId = null)
        {
            var rows = new List<ADO.DiseaseDetailResultADO>();
            try
            {
                foreach (var kv in checkMapping)
                {
                    var row = new ADO.DiseaseDetailResultADO
                    {
                        DISEASE_DETAIL_ID = kv.Key,
                        IS_CHECK = kv.Value.Checked ? 1 : 0,
                        KSK_GENERAL_ID = kskGeneralId
                    };
                    if (textMapping.ContainsKey(kv.Key))
                        row.OTHER = (textMapping[kv.Key].Text ?? "").Trim();
                    rows.Add(row);
                }
                foreach (var kv in textMapping)
                {
                    if (checkMapping.ContainsKey(kv.Key)) continue;
                    rows.Add(new ADO.DiseaseDetailResultADO
                    {
                        DISEASE_DETAIL_ID = kv.Key,
                        OTHER = (kv.Value.Text ?? "").Trim(),
                        KSK_GENERAL_ID = kskGeneralId
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rows;
        }

        public void ResetAll()
        {
            try
            {
                foreach (var kv in checkMapping)
                    kv.Value.Checked = false;
                foreach (var kv in textMapping)
                    kv.Value.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Inner class
        private class LayoutItem
        {
            public V_HIS_DISEASE_DETAIL Detail { get; set; }
            public int Row { get; set; }
            public int Col { get; set; }
            public int ColSpan { get; set; }
            /// <summary>IS_OTHER text (khong phai numeric) — chiem rong, Fill</summary>
            public bool IsWideOther { get; set; }
        }
        #endregion
    }
}

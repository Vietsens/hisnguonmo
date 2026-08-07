/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * TOÀN BỘ GIAO DIỆN (design) nằm ở đây — dựng trong InitializeComponent(), KHÔNG dựng ở runtime code-behind.
 * Tìm kiếm + grid (trạng thái đẩy + cột Xem/Đẩy) + thanh đồng bộ/tự động + phân trang + footer.
 * Logic/data/event nằm ở các partial khác (UC.cs, __Process.cs, __AutoPush.cs).
 */
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.MainForm
{
    partial class UCInfectiousDiseaseSyncList
    {
        private System.ComponentModel.IContainer components = null;

        #region Declare — controls (design)
        // Tìm kiếm
        private PanelControl pnlSearch;
        private TextEdit txtSearchTreatmentCode, txtSearchPatientCode, txtSearchPatientName;
        private DateEdit dteSearchFrom, dteSearchTo;
        private ComboBoxEdit cboSyncStatus;
        private SimpleButton btnSearch;
        // Grid + phân trang
        private GridControl grdList;
        private GridView gvList;
        private GridColumn colView, colPush, colSyncStatus;
        private Inventec.UC.Paging.UcPaging ucPaging;
        // Đồng bộ + footer
        private PanelControl pnlSyncBar, pnlFooter;
        private SimpleButton btnSyncList, btnEdit, btnReconcile;
        // Tự động đẩy (control UI; Timer tạo ở __AutoPush.cs)
        private CheckEdit chkAutoPush;
        private SpinEdit spnAutoInterval;
        private LabelControl lblAutoStatus;
        #endregion

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // Dừng & giải phóng Timer tự động đẩy khi UC bị hủy.
                    if (autoPushTimer != null)
                    {
                        autoPushTimer.Stop();
                        autoPushTimer.Dispose();
                        autoPushTimer = null;
                    }
                    if (components != null) components.Dispose();
                }
            }
            catch (System.Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // --- Tìm kiếm ---
            pnlSearch = new PanelControl() { Dock = DockStyle.Top, Height = 76 };
            pnlSearch.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            AddLabel("Mã điều trị:", 6, 10);
            txtSearchTreatmentCode = new TextEdit() { Location = new Point(84, 8), Size = new Size(150, 22) };
            AddLabel("Mã BN:", 244, 10);
            txtSearchPatientCode = new TextEdit() { Location = new Point(300, 8), Size = new Size(120, 22) };
            AddLabel("Tên bệnh nhân:", 432, 10);
            txtSearchPatientName = new TextEdit() { Location = new Point(524, 8), Size = new Size(180, 22) };

            AddLabel("Từ ngày:", 6, 42);
            dteSearchFrom = NewDate(); dteSearchFrom.Location = new Point(84, 40); dteSearchFrom.Size = new Size(110, 22); dteSearchFrom.DateTime = DateTime.Now;
            AddLabel("Đến ngày:", 200, 42);
            dteSearchTo = NewDate(); dteSearchTo.Location = new Point(268, 40); dteSearchTo.Size = new Size(110, 22); dteSearchTo.DateTime = DateTime.Now;
            AddLabel("Trạng thái:", 388, 42);
            cboSyncStatus = new ComboBoxEdit() { Location = new Point(456, 40), Size = new Size(130, 22) };
            cboSyncStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboSyncStatus.Properties.Items.AddRange(new object[] { "Tất cả", "Chưa đồng bộ", "Đã đồng bộ", "Thất bại" });
            cboSyncStatus.SelectedIndex = 0;

            btnSearch = new SimpleButton() { Text = "Tìm kiếm (Ctrl+F)", Location = new Point(596, 40), Size = new Size(120, 24) };
            btnSearch.Click += (s, e) => { try { SearchList(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); } };
            pnlSearch.Controls.AddRange(new Control[] {
                txtSearchTreatmentCode, txtSearchPatientCode, txtSearchPatientName,
                dteSearchFrom, dteSearchTo, cboSyncStatus, btnSearch });

            // --- Grid ---
            grdList = new GridControl() { Dock = DockStyle.Fill };
            gvList = new GridView(grdList);
            grdList.MainView = gvList;
            gvList.OptionsBehavior.Editable = false;
            gvList.OptionsView.ShowGroupPanel = false;
            gvList.OptionsView.ColumnAutoWidth = false;
            gvList.OptionsSelection.MultiSelect = true;
            gvList.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
            gvList.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;

            AddGridCol("STT", "STT", 40);
            AddGridCol("TREATMENT_CODE", "Mã điều trị", 110);
            AddGridCol("PATIENT_CODE", "Mã BN", 90);
            AddGridCol("PATIENT_NAME", "Bệnh nhân", 160);
            AddGridCol("ICD_CODE", "ICD", 60);
            AddGridCol("IN_TIME_STR", "Thời gian vào", 110);
            colSyncStatus = AddGridCol("PUSH_STATE_STR", "Trạng thái đẩy", 110);
            colView = AddGridCol("VIEW_ACTION", "", 50);
            colPush = AddGridCol("PUSH_ACTION", "", 50);

            gvList.SelectionChanged += (s, e) => UpdateSyncBadge();
            gvList.DoubleClick += (s, e) => { try { OpenDetailForFocusedRow(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); } };
            gvList.RowCellStyle += gvList_RowCellStyle;
            gvList.RowCellClick += gvList_RowCellClick;

            // --- Đồng bộ + tự động đẩy ---
            pnlSyncBar = new PanelControl() { Dock = DockStyle.Bottom, Height = 34 };
            pnlSyncBar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            btnSyncList = new SimpleButton() { Text = "Đồng bộ lên cổng (0)", Location = new Point(6, 4), Size = new Size(180, 26), Enabled = false };
            btnSyncList.Click += (s, e) => { try { SyncSelected(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); } };

            chkAutoPush = new CheckEdit() { Text = "Tự động đẩy mỗi", Location = new Point(196, 6), Size = new Size(112, 22) };
            spnAutoInterval = new SpinEdit() { Location = new Point(312, 4), Size = new Size(56, 22) };
            spnAutoInterval.Properties.IsFloatValue = false;
            spnAutoInterval.Properties.MinValue = 1;
            spnAutoInterval.Properties.MaxValue = 1440;
            spnAutoInterval.EditValue = 5;
            var lblPhut = new LabelControl() { Text = "phút", Location = new Point(372, 8), AutoSizeMode = LabelAutoSizeMode.None, Size = new Size(28, 16) };
            lblAutoStatus = new LabelControl() { Text = "", Location = new Point(408, 8), AutoSizeMode = LabelAutoSizeMode.None, Size = new Size(340, 16) };
            chkAutoPush.CheckedChanged += chkAutoPush_CheckedChanged;
            spnAutoInterval.EditValueChanged += spnAutoInterval_EditValueChanged;

            pnlSyncBar.Controls.AddRange(new Control[] { btnSyncList, chkAutoPush, spnAutoInterval, lblPhut, lblAutoStatus });

            ucPaging = new Inventec.UC.Paging.UcPaging() { Dock = DockStyle.Bottom };

            // --- Footer ---
            pnlFooter = new PanelControl() { Dock = DockStyle.Bottom, Height = 40 };
            pnlFooter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            btnEdit = new SimpleButton() { Text = "Xem/Sửa chi tiết", Location = new Point(6, 7), Size = new Size(140, 26) };
            btnEdit.Click += btnEdit_Click;
            btnReconcile = new SimpleButton() { Text = "Làm mới", Location = new Point(152, 7), Size = new Size(100, 26) };
            btnReconcile.Click += (s, e) => { try { SearchList(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); } };
            pnlFooter.Controls.AddRange(new Control[] { btnEdit, btnReconcile });

            // --- Dock ---
            this.Controls.Add(grdList);
            this.Controls.Add(pnlFooter);
            this.Controls.Add(ucPaging);
            this.Controls.Add(pnlSyncBar);
            this.Controls.Add(pnlSearch);

            //
            // UCInfectiousDiseaseSyncList
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UCInfectiousDiseaseSyncList";
            this.Size = new System.Drawing.Size(1000, 600);
            this.Load += new System.EventHandler(this.UCInfectiousDiseaseSyncList_Load);
            this.ResumeLayout(false);
        }

        private void AddLabel(string text, int x, int y)
        {
            pnlSearch.Controls.Add(new LabelControl() { Text = text, Location = new Point(x, y), AutoSizeMode = LabelAutoSizeMode.None, Size = new Size(70, 16) });
        }

        private GridColumn AddGridCol(string fieldName, string caption, int width)
        {
            GridColumn c = gvList.Columns.AddVisible(fieldName);
            c.Caption = caption;
            c.Width = width;
            c.OptionsColumn.AllowEdit = false;
            return c;
        }

        private DateEdit NewDate()
        {
            var d = new DateEdit();
            d.Properties.Mask.EditMask = "dd/MM/yyyy";
            d.Properties.Mask.UseMaskAsDisplayFormat = true;
            return d;
        }
    }
}

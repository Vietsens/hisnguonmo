using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm.Dialogs
{
    /// <summary>
    /// Hộp chọn giấy chứng sinh khi lượt điều trị có nhiều trẻ (sinh đôi, sinh ba...).
    /// Dùng cho nút "Lấy từ GCS" ở mục Sinh đẻ.
    /// </summary>
    internal class frmSelectBaby : XtraForm
    {
        public HIS_BABY SelectedBaby { get; private set; }

        private GridControl gridControl;
        private GridView gridView;
        private SimpleButton btnSelect;
        private SimpleButton btnClose;
        private List<BabyRowADO> rows;

        internal class BabyRowADO
        {
            public HIS_BABY Baby { get; set; }
            public long? BABY_ORDER { get; set; }
            public string BABY_NAME { get; set; }
            public string GENDER_NAME { get; set; }
            public string BORN_TIME_STR { get; set; }
            public string BORN_RESULT_NAME { get; set; }
        }

        public frmSelectBaby(List<HIS_BABY> babies)
        {
            InitializeComponent();
            LoadData(babies);
        }

        private void InitializeComponent()
        {
            this.Text = "Chọn giấy chứng sinh";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.ClientSize = new Size(640, 260);
            this.KeyPreview = true;

            gridControl = new GridControl();
            gridView = new GridView(gridControl);
            gridControl.MainView = gridView;
            gridControl.Location = new Point(8, 8);
            gridControl.Size = new Size(624, 210);
            gridControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            gridControl.ViewCollection.Add(gridView);

            gridView.OptionsBehavior.Editable = false;
            gridView.OptionsBehavior.ReadOnly = true;
            gridView.OptionsView.ShowGroupPanel = false;
            gridView.OptionsView.ShowIndicator = false;
            gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView.OptionsView.ColumnAutoWidth = true;

            AddColumn("BABY_ORDER", "Con thứ", 60);
            AddColumn("BABY_NAME", "Họ tên", 220);
            AddColumn("GENDER_NAME", "Giới tính", 70);
            AddColumn("BORN_TIME_STR", "Ngày giờ sinh", 130);
            AddColumn("BORN_RESULT_NAME", "Tình trạng", 80);

            gridView.DoubleClick += gridView_DoubleClick;

            btnSelect = new SimpleButton();
            btnSelect.Text = "Chọn";
            btnSelect.Size = new Size(90, 24);
            btnSelect.Location = new Point(444, 228);
            btnSelect.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSelect.Click += btnSelect_Click;

            btnClose = new SimpleButton();
            btnClose.Text = "Đóng";
            btnClose.Size = new Size(90, 24);
            btnClose.Location = new Point(542, 228);
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.Cancel;

            this.CancelButton = btnClose;
            this.Controls.Add(gridControl);
            this.Controls.Add(btnSelect);
            this.Controls.Add(btnClose);
        }

        private void AddColumn(string fieldName, string caption, int width)
        {
            GridColumn col = gridView.Columns.AddVisible(fieldName, caption);
            col.Width = width;
            col.OptionsColumn.AllowEdit = false;
        }

        private void LoadData(List<HIS_BABY> babies)
        {
            try
            {
                rows = new List<BabyRowADO>();
                if (babies == null) babies = new List<HIS_BABY>();

                List<HIS_GENDER> genders = null;
                List<HIS_BORN_RESULT> bornResults = null;
                try
                {
                    genders = BackendDataWorker.Get<HIS_GENDER>();
                    bornResults = BackendDataWorker.Get<HIS_BORN_RESULT>();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                foreach (var baby in babies)
                {
                    BabyRowADO row = new BabyRowADO();
                    row.Baby = baby;
                    row.BABY_ORDER = baby.BABY_ORDER;
                    row.BABY_NAME = baby.BABY_NAME;
                    if (baby.GENDER_ID.HasValue && genders != null)
                    {
                        var gender = genders.FirstOrDefault(o => o.ID == baby.GENDER_ID.Value);
                        row.GENDER_NAME = gender != null ? gender.GENDER_NAME : null;
                    }
                    if (baby.BORN_TIME.HasValue && baby.BORN_TIME.Value > 0)
                    {
                        row.BORN_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(baby.BORN_TIME.Value);
                    }
                    if (baby.BORN_RESULT_ID.HasValue && bornResults != null)
                    {
                        var result = bornResults.FirstOrDefault(o => o.ID == baby.BORN_RESULT_ID.Value);
                        row.BORN_RESULT_NAME = result != null ? result.BORN_RESULT_NAME : null;
                    }
                    rows.Add(row);
                }

                gridControl.DataSource = rows;
                if (rows.Count > 0)
                {
                    gridView.FocusedRowHandle = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                var hit = view.CalcHitInfo(view.GridControl.PointToClient(Control.MousePosition));
                if (hit.InRow || hit.InRowCell)
                {
                    ConfirmSelection();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            ConfirmSelection();
        }

        private void ConfirmSelection()
        {
            try
            {
                BabyRowADO row = gridView.GetFocusedRow() as BabyRowADO;
                if (row == null || row.Baby == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn một giấy chứng sinh", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SelectedBaby = row.Baby;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    /// <summary>
    /// Danh sach xet nghiem cua 1 tui mau (chi dinh che pham mau theo tui)
    /// </summary>
    public class BloodBagADO
    {
        public List<BloodBagTestADO> Tests { get; set; }

        public BloodBagADO Clone()
        {
            return new BloodBagADO() { Tests = this.Tests != null ? this.Tests.Select(o => o.Clone()).ToList() : new List<BloodBagTestADO>() };
        }
    }

    /// <summary>
    /// 1 xet nghiem co the chon cho tui mau
    /// </summary>
    public class BloodBagTestADO
    {
        public long ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public bool IsChecked { get; set; }
        public long? PatientTypeId { get; set; }
        public short? IsExpend { get; set; }
        public decimal Amount { get; set; }
        //Thuoc bo dich vu di kem cua che pham
        public bool IsDefault { get; set; }
        //Doi tuong thanh toan co gia cho xet nghiem nay
        public List<long> AllowPatientTypeIds { get; set; }

        public BloodBagTestADO Clone()
        {
            BloodBagTestADO result = (BloodBagTestADO)this.MemberwiseClone();
            result.AllowPatientTypeIds = this.AllowPatientTypeIds != null ? new List<long>(this.AllowPatientTypeIds) : new List<long>();
            return result;
        }
    }

    /// <summary>
    /// Bang "Xet nghiem tui mau": chon xet nghiem rieng cho tung tui cua 1 dong che pham mau
    /// </summary>
    public class frmBloodBagService : XtraForm
    {
        private ListBoxControl lstBags;
        private GridControl gridControlTests;
        private GridView gridViewTests;
        private List<BloodBagADO> bags;
        private Func<List<BloodBagTestADO>> makeDefaultTests;
        private List<HIS_PATIENT_TYPE> patientTypes;
        private int currentBagIndex = -1;

        public List<BloodBagADO> ResultBags { get; private set; }

        public frmBloodBagService(string bloodServiceName, List<BloodBagADO> bags, Func<List<BloodBagTestADO>> makeDefaultTests, List<HIS_PATIENT_TYPE> patientTypes)
        {
            this.bags = bags != null ? bags.Select(o => o.Clone()).ToList() : new List<BloodBagADO>();
            this.makeDefaultTests = makeDefaultTests;
            this.patientTypes = patientTypes ?? new List<HIS_PATIENT_TYPE>();
            this.InitControls(bloodServiceName);
        }

        private void InitControls(string bloodServiceName)
        {
            this.Text = "Xét nghiệm túi máu - " + bloodServiceName;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(960, 560);
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.KeyPreview = true;

            //Danh sach tui
            this.lstBags = new ListBoxControl();
            this.lstBags.Dock = DockStyle.Left;
            this.lstBags.Width = 130;
            for (int i = 0; i < this.bags.Count; i++)
            {
                this.lstBags.Items.Add("Túi " + (i + 1));
            }
            this.lstBags.SelectedIndexChanged += lstBags_SelectedIndexChanged;

            //Luoi xet nghiem
            this.gridControlTests = new GridControl();
            this.gridViewTests = new GridView(this.gridControlTests);
            this.gridControlTests.MainView = this.gridViewTests;
            this.gridControlTests.ViewCollection.Add(this.gridViewTests);
            this.gridControlTests.Dock = DockStyle.Fill;
            this.gridViewTests.OptionsView.ShowGroupPanel = false;
            this.gridViewTests.OptionsView.ColumnAutoWidth = true;
            this.gridViewTests.OptionsFind.AlwaysVisible = true;
            this.gridViewTests.OptionsFind.FindNullPrompt = "Tìm theo mã hoặc tên xét nghiệm";
            this.gridViewTests.OptionsCustomization.AllowGroup = false;

            RepositoryItemCheckEdit chkSelect = new RepositoryItemCheckEdit();
            RepositoryItemLookUpEdit cboPatientType = new RepositoryItemLookUpEdit();
            cboPatientType.DataSource = this.patientTypes;
            cboPatientType.DisplayMember = "PATIENT_TYPE_NAME";
            cboPatientType.ValueMember = "ID";
            cboPatientType.NullText = "";
            cboPatientType.ShowHeader = false;
            cboPatientType.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_NAME", "Đối tượng"));
            this.gridControlTests.RepositoryItems.AddRange(new RepositoryItem[] { chkSelect, cboPatientType });

            GridColumn colChecked = this.AddColumn("IsChecked", "Chọn", 50, true);
            colChecked.ColumnEdit = chkSelect;
            this.AddColumn("ServiceCode", "Mã xét nghiệm", 110, false);
            this.AddColumn("ServiceName", "Tên xét nghiệm", 380, false);
            GridColumn colPatientType = this.AddColumn("PatientTypeId", "Đối tượng thanh toán", 170, true);
            colPatientType.ColumnEdit = cboPatientType;
            GridColumn colDefault = this.AddColumn("IsDefault", "Đi kèm", 60, false);
            colDefault.ColumnEdit = chkSelect;

            //Nut
            FlowLayoutPanel pnlButtons = new FlowLayoutPanel();
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Height = 42;
            pnlButtons.FlowDirection = FlowDirection.RightToLeft;
            pnlButtons.Padding = new Padding(6);
            SimpleButton btnCancel = this.AddButton(pnlButtons, "Hủy (Esc)", 90);
            SimpleButton btnOk = this.AddButton(pnlButtons, "Đồng ý (Ctrl S)", 120);
            SimpleButton btnDefault = this.AddButton(pnlButtons, "Khôi phục mặc định", 140);
            SimpleButton btnApplyAll = this.AddButton(pnlButtons, "Áp dụng cho tất cả túi", 160);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnOk.Click += btnOk_Click;
            btnDefault.Click += btnDefault_Click;
            btnApplyAll.Click += btnApplyAll_Click;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) { this.DialogResult = DialogResult.Cancel; this.Close(); }
                else if (e.Control && e.KeyCode == Keys.S) { btnOk_Click(btnOk, EventArgs.Empty); }
            };

            this.Controls.Add(this.gridControlTests);
            this.Controls.Add(this.lstBags);
            this.Controls.Add(pnlButtons);

            if (this.bags.Count > 0)
            {
                this.lstBags.SelectedIndex = 0;
                this.BindBag(0);
            }
        }

        private GridColumn AddColumn(string fieldName, string caption, int width, bool allowEdit)
        {
            GridColumn col = new GridColumn();
            col.FieldName = fieldName;
            col.Caption = caption;
            col.Width = width;
            col.Visible = true;
            col.VisibleIndex = this.gridViewTests.Columns.Count;
            col.OptionsColumn.AllowEdit = allowEdit;
            col.OptionsColumn.AllowFocus = true;
            this.gridViewTests.Columns.Add(col);
            return col;
        }

        private SimpleButton AddButton(FlowLayoutPanel panel, string text, int width)
        {
            SimpleButton btn = new SimpleButton();
            btn.Text = text;
            btn.Width = width;
            btn.Height = 26;
            panel.Controls.Add(btn);
            return btn;
        }

        private void BindBag(int index)
        {
            if (index < 0 || index >= this.bags.Count)
                return;
            this.currentBagIndex = index;
            this.gridControlTests.DataSource = null;
            this.gridControlTests.DataSource = this.bags[index].Tests;
        }

        private void CommitGrid()
        {
            if (this.gridViewTests.IsEditing)
                this.gridViewTests.CloseEditor();
            if (this.gridViewTests.FocusedRowModified)
                this.gridViewTests.UpdateCurrentRow();
        }

        private void lstBags_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.CommitGrid();
                this.BindBag(this.lstBags.SelectedIndex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnApplyAll_Click(object sender, EventArgs e)
        {
            try
            {
                this.CommitGrid();
                if (this.currentBagIndex < 0)
                    return;
                BloodBagADO source = this.bags[this.currentBagIndex];
                for (int i = 0; i < this.bags.Count; i++)
                {
                    if (i != this.currentBagIndex)
                    {
                        this.bags[i] = source.Clone();
                    }
                }
                XtraMessageBox.Show(string.Format("Đã áp dụng xét nghiệm của túi {0} cho tất cả {1} túi.", this.currentBagIndex + 1, this.bags.Count), "Thông báo");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            try
            {
                this.CommitGrid();
                if (this.currentBagIndex < 0 || this.makeDefaultTests == null)
                    return;
                this.bags[this.currentBagIndex] = new BloodBagADO() { Tests = this.makeDefaultTests() };
                this.BindBag(this.currentBagIndex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                this.CommitGrid();
                List<string> errors = new List<string>();
                for (int i = 0; i < this.bags.Count; i++)
                {
                    foreach (BloodBagTestADO test in this.bags[i].Tests.Where(o => o.IsChecked))
                    {
                        if (!test.PatientTypeId.HasValue || test.AllowPatientTypeIds == null || !test.AllowPatientTypeIds.Contains(test.PatientTypeId.Value))
                        {
                            errors.Add(string.Format("Túi {0}: {1}", i + 1, test.ServiceName));
                        }
                    }
                }
                if (errors.Count > 0)
                {
                    XtraMessageBox.Show("Các xét nghiệm sau chưa chọn đối tượng thanh toán có giá:\r\n" + string.Join("\r\n", errors), "Thông báo");
                    return;
                }
                this.ResultBags = this.bags;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisExecuteRoom.ADO;
using HIS.Desktop.Utilities.Extensions;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisExecuteRoom.HisExecuteRoom
{
    /// <summary>
    /// Thiết lập cảnh báo chuyển khám theo ICD của một phòng khám chuyên đề (việc 62129).
    /// Kết quả trả về qua 4 thuộc tính tương ứng 4 cột ADD_EXAM_* của HIS_EXECUTE_ROOM; form cha tự lưu.
    /// </summary>
    internal class frmAddExamWarning : XtraForm
    {
        internal const short OPTION_MAIN = 1;
        internal const short OPTION_SUB = 2;
        internal const short OPTION_BOTH = 3;

        internal string IcdCodes { get; private set; }
        internal string WarningRoomIds { get; private set; }
        internal short? IcdOption { get; private set; }
        internal string Times { get; private set; }

        LayoutControl layoutControl;
        CustomGridLookUpEdit cboIcd;
        CustomGridLookUpEdit cboRoom;
        RadioGroup rdoOption;
        GridControl gridTime;
        GridView gridViewTime;
        SimpleButton btnAddTime;
        SimpleButton btnOk;
        SimpleButton btnClose;
        BindingList<AddExamTimeADO> times;

        internal frmAddExamWarning(string roomName, string icdCodes, string warningRoomIds, short? icdOption, string addExamTimes)
        {
            InitControls();
            this.Text = "Thiết lập cảnh báo chuyển khám" + (String.IsNullOrWhiteSpace(roomName) ? "" : " - " + roomName);
            LoadIcd(icdCodes);
            LoadRoom(warningRoomIds);
            rdoOption.EditValue = (icdOption == OPTION_MAIN || icdOption == OPTION_SUB) ? icdOption.Value : OPTION_BOTH;
            times = new BindingList<AddExamTimeADO>(AddExamTimeADO.Parse(addExamTimes));
            gridTime.DataSource = times;
        }

        private void InitControls()
        {
            this.SuspendLayout();
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(720, 560);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;

            layoutControl = new LayoutControl();
            layoutControl.Dock = DockStyle.Fill;
            this.Controls.Add(layoutControl);

            cboIcd = new CustomGridLookUpEdit();
            cboIcd.Properties.NullText = "";
            cboIcd.Properties.PopupFormSize = new Size(600, 350);
            cboIcd.CustomDisplayText += cboIcd_CustomDisplayText;

            cboRoom = new CustomGridLookUpEdit();
            cboRoom.Properties.NullText = "";
            cboRoom.Properties.PopupFormSize = new Size(600, 350);
            cboRoom.CustomDisplayText += cboRoom_CustomDisplayText;

            rdoOption = new RadioGroup();
            rdoOption.Properties.Items.AddRange(new RadioGroupItem[] {
                new RadioGroupItem(OPTION_MAIN, "Chẩn đoán chính"),
                new RadioGroupItem(OPTION_SUB, "Chẩn đoán kèm theo"),
                new RadioGroupItem(OPTION_BOTH, "Cả hai")});
            rdoOption.Properties.Columns = 3;

            btnAddTime = new SimpleButton();
            btnAddTime.Text = "Thêm khung giờ";
            btnAddTime.Click += btnAddTime_Click;

            InitGridTime();

            btnOk = new SimpleButton();
            btnOk.Text = "Đồng ý";
            btnOk.Click += btnOk_Click;
            btnClose = new SimpleButton();
            btnClose.Text = "Đóng";
            btnClose.DialogResult = DialogResult.Cancel;
            this.CancelButton = btnClose;

            layoutControl.BeginUpdate();
            LayoutControlItem lciIcd = NewItem("ICD chuyển khám:", cboIcd, Size.Empty);
            LayoutControlItem lciRoom = NewItem("Phòng được cảnh báo:", cboRoom, Size.Empty);
            LayoutControlItem lciOption = NewItem("Vị trí ICD:", rdoOption, Size.Empty);
            lciOption.SizeConstraintsType = SizeConstraintsType.Custom;
            lciOption.MinSize = new Size(300, 30);
            lciOption.MaxSize = new Size(0, 30);
            LayoutControlItem lciAddTime = NewItem("Lịch áp dụng:", btnAddTime, new Size(250, 26));
            LayoutControlItem lciGrid = NewItem(null, gridTime, Size.Empty);
            LayoutControlItem lciOk = NewItem(null, btnOk, new Size(110, 26));
            LayoutControlItem lciClose = NewItem(null, btnClose, new Size(110, 26));

            layoutControl.Root.AddItem(lciIcd);
            layoutControl.Root.AddItem(lciRoom, lciIcd, InsertType.Bottom);
            layoutControl.Root.AddItem(lciOption, lciRoom, InsertType.Bottom);
            layoutControl.Root.AddItem(lciAddTime, lciOption, InsertType.Bottom);
            layoutControl.Root.AddItem(new EmptySpaceItem(), lciAddTime, InsertType.Right);
            layoutControl.Root.AddItem(lciGrid, lciAddTime, InsertType.Bottom);
            layoutControl.Root.AddItem(lciOk, lciGrid, InsertType.Bottom);
            layoutControl.Root.AddItem(new EmptySpaceItem(), lciOk, InsertType.Left);
            layoutControl.Root.AddItem(lciClose, lciOk, InsertType.Right);
            layoutControl.EndUpdate();
            this.ResumeLayout(false);
        }

        private static LayoutControlItem NewItem(string text, Control control, Size fixedSize)
        {
            LayoutControlItem item = new LayoutControlItem();
            item.Control = control;
            if (String.IsNullOrEmpty(text))
            {
                item.TextVisible = false;
            }
            else
            {
                item.Text = text;
                item.TextAlignMode = TextAlignModeItem.CustomSize;
                item.TextSize = new Size(130, 13);
                item.TextToControlDistance = 5;
            }
            if (fixedSize != Size.Empty)
            {
                item.SizeConstraintsType = SizeConstraintsType.Custom;
                item.MinSize = fixedSize;
                item.MaxSize = fixedSize;
            }
            return item;
        }

        private void InitGridTime()
        {
            gridTime = new GridControl();
            gridViewTime = new GridView(gridTime);
            gridTime.MainView = gridViewTime;
            gridTime.ViewCollection.Add(gridViewTime);
            gridViewTime.OptionsView.ShowGroupPanel = false;
            gridViewTime.OptionsView.ShowIndicator = false;
            gridViewTime.OptionsCustomization.AllowSort = false;
            gridViewTime.OptionsCustomization.AllowFilter = false;
            gridViewTime.OptionsCustomization.AllowColumnMoving = false;

            RepositoryItemLookUpEdit repDay = new RepositoryItemLookUpEdit();
            repDay.DataSource = new List<KeyValuePair<long, string>>() {
                new KeyValuePair<long, string>(2, "Thứ 2"),
                new KeyValuePair<long, string>(3, "Thứ 3"),
                new KeyValuePair<long, string>(4, "Thứ 4"),
                new KeyValuePair<long, string>(5, "Thứ 5"),
                new KeyValuePair<long, string>(6, "Thứ 6"),
                new KeyValuePair<long, string>(7, "Thứ 7"),
                new KeyValuePair<long, string>(1, "Chủ nhật")};
            repDay.ValueMember = "Key";
            repDay.DisplayMember = "Value";
            repDay.Columns.Add(new LookUpColumnInfo("Value", ""));
            repDay.ShowHeader = false;
            repDay.ShowFooter = false;
            repDay.NullText = "";
            repDay.TextEditStyle = TextEditStyles.DisableTextEditor;

            RepositoryItemTimeEdit repTime = new RepositoryItemTimeEdit();
            repTime.Mask.EditMask = "HH:mm";
            repTime.Mask.UseMaskAsDisplayFormat = true;
            repTime.NullText = "";

            RepositoryItemButtonEdit repDelete = new RepositoryItemButtonEdit();
            repDelete.TextEditStyle = TextEditStyles.HideTextEditor;
            repDelete.Buttons.Clear();
            repDelete.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
            repDelete.ButtonClick += repDelete_ButtonClick;

            gridTime.RepositoryItems.AddRange(new RepositoryItem[] { repDay, repTime, repDelete });

            GridColumn colDay = gridViewTime.Columns.AddVisible("DAY", "Thứ");
            colDay.ColumnEdit = repDay;
            colDay.Width = 150;
            GridColumn colFrom = gridViewTime.Columns.AddVisible("FROM_TIME", "Từ giờ");
            colFrom.ColumnEdit = repTime;
            colFrom.Width = 150;
            GridColumn colTo = gridViewTime.Columns.AddVisible("TO_TIME", "Đến giờ");
            colTo.ColumnEdit = repTime;
            colTo.Width = 150;
            GridColumn colDelete = gridViewTime.Columns.AddVisible("DELETE", "Xóa");
            colDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            colDelete.ColumnEdit = repDelete;
            colDelete.Width = 50;
            colDelete.OptionsColumn.ShowCaption = false;
        }

        private void LoadIcd(string icdCodes)
        {
            List<HIS_ICD> icds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == 1).ToList();
            List<string> codes = SplitValues(icdCodes, ';');
            // Mã đã chọn đưa lên đầu; mã đã khóa/không còn trong danh mục vẫn giữ để không mất thiết lập khi lưu lại.
            List<HIS_ICD> selected = new List<HIS_ICD>();
            foreach (string code in codes)
            {
                HIS_ICD icd = icds.FirstOrDefault(o => o.ICD_CODE == code)
                    ?? BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE == code)
                    ?? new HIS_ICD() { ICD_CODE = code, ICD_NAME = code };
                if (!selected.Contains(icd)) selected.Add(icd);
            }
            List<HIS_ICD> dataSource = new List<HIS_ICD>(selected);
            dataSource.AddRange(icds.Where(o => !selected.Contains(o)));
            InitCheckCombo(cboIcd, dataSource, "ICD_CODE", "Mã ICD", "ICD_NAME", "Tên bệnh");
            (cboIcd.Properties.Tag as GridCheckMarksSelection).SelectAll(selected);
        }

        private void LoadRoom(string warningRoomIds)
        {
            List<V_HIS_EXECUTE_ROOM> rooms = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>()
                .Where(o => o.IS_EXAM == 1 && o.IS_ACTIVE == 1)
                .OrderBy(o => o.EXECUTE_ROOM_CODE).ToList();
            List<string> ids = SplitValues(warningRoomIds, ',');
            List<V_HIS_EXECUTE_ROOM> selected = rooms.Where(o => ids.Contains(o.ROOM_ID.ToString())).ToList();
            List<V_HIS_EXECUTE_ROOM> dataSource = new List<V_HIS_EXECUTE_ROOM>(selected);
            dataSource.AddRange(rooms.Where(o => !selected.Contains(o)));
            InitCheckCombo(cboRoom, dataSource, "EXECUTE_ROOM_CODE", "Mã phòng", "EXECUTE_ROOM_NAME", "Tên phòng");
            GridColumn colDepartment = cboRoom.Properties.View.Columns.AddVisible("DEPARTMENT_NAME", "Khoa");
            colDepartment.Width = 200;
            (cboRoom.Properties.Tag as GridCheckMarksSelection).SelectAll(selected);
        }

        private void InitCheckCombo(CustomGridLookUpEdit cbo, object dataSource, string codeField, string codeCaption, string nameField, string nameCaption)
        {
            GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
            gridCheck.SelectionChanged += (s, e) => cbo.Text = GetDisplayText(cbo);
            cbo.Properties.Tag = gridCheck;
            cbo.Properties.DataSource = dataSource;
            cbo.Properties.DisplayMember = nameField;
            cbo.Properties.ValueMember = codeField;
            GridColumn colCode = cbo.Properties.View.Columns.AddVisible(codeField, codeCaption);
            colCode.Width = 100;
            GridColumn colName = cbo.Properties.View.Columns.AddVisible(nameField, nameCaption);
            colName.Width = 300;
            cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
            cbo.Properties.View.OptionsSelection.MultiSelect = true;
        }

        private string GetDisplayText(CustomGridLookUpEdit cbo)
        {
            if (cbo == cboIcd)
            {
                List<HIS_ICD> icds = GetSelected<HIS_ICD>(cboIcd);
                cboIcd.ToolTip = String.Join("\r\n", icds.Select(o => o.ICD_CODE + " - " + o.ICD_NAME));
                return String.Join("; ", icds.Select(o => o.ICD_CODE));
            }
            string roomNames = String.Join("; ", GetSelected<V_HIS_EXECUTE_ROOM>(cboRoom).Select(o => o.EXECUTE_ROOM_NAME));
            cboRoom.ToolTip = roomNames;
            return roomNames;
        }

        private void cboIcd_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            e.DisplayText = GetDisplayText(cboIcd);
        }

        private void cboRoom_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            e.DisplayText = GetDisplayText(cboRoom);
        }

        private List<T> GetSelected<T>(CustomGridLookUpEdit cbo)
        {
            GridCheckMarksSelection gridCheck = cbo.Properties.Tag as GridCheckMarksSelection;
            return gridCheck != null ? gridCheck.Selection.OfType<T>().ToList() : new List<T>();
        }

        private static List<string> SplitValues(string value, char separator)
        {
            if (String.IsNullOrWhiteSpace(value)) return new List<string>();
            return value.Split(separator).Select(o => o.Trim()).Where(o => o.Length > 0).Distinct().ToList();
        }

        private void btnAddTime_Click(object sender, EventArgs e)
        {
            gridViewTime.CloseEditor();
            times.Add(new AddExamTimeADO());
            gridViewTime.FocusedRowHandle = gridViewTime.GetRowHandle(times.Count - 1);
            gridViewTime.FocusedColumn = gridViewTime.Columns["DAY"];
            gridViewTime.ShowEditor();
        }

        private void repDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            AddExamTimeADO row = gridViewTime.GetFocusedRow() as AddExamTimeADO;
            if (row != null) times.Remove(row);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            gridViewTime.CloseEditor();
            gridViewTime.UpdateCurrentRow();

            List<HIS_ICD> icds = GetSelected<HIS_ICD>(cboIcd);
            List<V_HIS_EXECUTE_ROOM> rooms = GetSelected<V_HIS_EXECUTE_ROOM>(cboRoom);
            List<AddExamTimeADO> validTimes = times.Where(o => o.DAY.HasValue || o.FROM_TIME.HasValue || o.TO_TIME.HasValue).ToList();

            string error = ValidTimes(validTimes);
            if (String.IsNullOrEmpty(error) && icds.Count > 0 && validTimes.Count == 0)
                error = "Đã chọn ICD chuyển khám thì phải khai báo ít nhất 1 khung giờ áp dụng.";
            if (!String.IsNullOrEmpty(error))
            {
                XtraMessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            IcdCodes = icds.Count > 0 ? String.Join(";", icds.Select(o => o.ICD_CODE)) : null;
            WarningRoomIds = rooms.Count > 0 ? String.Join(",", rooms.Select(o => o.ROOM_ID)) : null;
            IcdOption = (short)rdoOption.EditValue;
            Times = AddExamTimeADO.Join(validTimes);
            this.DialogResult = DialogResult.OK;
        }

        private static string ValidTimes(List<AddExamTimeADO> validTimes)
        {
            foreach (AddExamTimeADO time in validTimes)
            {
                if (!time.DAY.HasValue || !time.FROM_TIME.HasValue || !time.TO_TIME.HasValue)
                    return "Khung giờ áp dụng phải nhập đủ Thứ, Từ giờ, Đến giờ.";
                if (time.FROM_TIME.Value.TimeOfDay >= time.TO_TIME.Value.TimeOfDay)
                    return String.Format("Khung giờ {0:HH:mm} - {1:HH:mm}: từ giờ phải nhỏ hơn đến giờ.", time.FROM_TIME, time.TO_TIME);
            }
            foreach (var day in validTimes.GroupBy(o => o.DAY.Value))
            {
                List<AddExamTimeADO> ordered = day.OrderBy(o => o.FROM_TIME.Value.TimeOfDay).ToList();
                for (int i = 1; i < ordered.Count; i++)
                {
                    if (ordered[i].FROM_TIME.Value.TimeOfDay < ordered[i - 1].TO_TIME.Value.TimeOfDay)
                        return String.Format("{0}: khung giờ {1:HH:mm} - {2:HH:mm} chồng với khung giờ {3:HH:mm} - {4:HH:mm}.",
                            day.Key == 1 ? "Chủ nhật" : "Thứ " + day.Key,
                            ordered[i].FROM_TIME, ordered[i].TO_TIME, ordered[i - 1].FROM_TIME, ordered[i - 1].TO_TIME);
                }
            }
            return null;
        }
    }
}

namespace HIS.Desktop.Plugins.SupportRequestManagement
{
    partial class UCSupportRequestManagement
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.pnlFilter = new DevExpress.XtraEditors.PanelControl();
            this.lblFrom = new DevExpress.XtraEditors.LabelControl();
            this.dtFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblTo = new DevExpress.XtraEditors.LabelControl();
            this.dtTo = new DevExpress.XtraEditors.DateEdit();
            this.lblStt = new DevExpress.XtraEditors.LabelControl();
            this.cboStt = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.lblDept = new DevExpress.XtraEditors.LabelControl();
            this.cboDept = new DevExpress.XtraEditors.LookUpEdit();
            this.lblBranch = new DevExpress.XtraEditors.LabelControl();
            this.cboBranch = new DevExpress.XtraEditors.LookUpEdit();
            this.lblRequester = new DevExpress.XtraEditors.LabelControl();
            this.cboRequester = new DevExpress.XtraEditors.GridLookUpEdit();
            this.lblReceiver = new DevExpress.XtraEditors.LabelControl();
            this.cboReceiver = new DevExpress.XtraEditors.GridLookUpEdit();
            this.lblAssigneeFilter = new DevExpress.XtraEditors.LabelControl();
            this.cboAssigneeFilter = new DevExpress.XtraEditors.GridLookUpEdit();
            this.btnMyTask = new DevExpress.XtraEditors.SimpleButton();
            this.lblKeyword = new DevExpress.XtraEditors.LabelControl();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.pnlDetail = new DevExpress.XtraEditors.PanelControl();
            this.tabDetail = new DevExpress.XtraTab.XtraTabControl();
            this.tabContent = new DevExpress.XtraTab.XtraTabPage();
            this.lblTitleCap = new DevExpress.XtraEditors.LabelControl();
            this.txtTitle = new DevExpress.XtraEditors.TextEdit();
            this.lblContentCap = new DevExpress.XtraEditors.LabelControl();
            this.memContent = new DevExpress.XtraEditors.MemoEdit();
            this.lblContactCap = new DevExpress.XtraEditors.LabelControl();
            this.txtContact = new DevExpress.XtraEditors.TextEdit();
            this.lblAnydeskCap = new DevExpress.XtraEditors.LabelControl();
            this.lblAnydesk = new DevExpress.XtraEditors.LabelControl();
            this.lblModuleCap = new DevExpress.XtraEditors.LabelControl();
            this.lblModule = new DevExpress.XtraEditors.LabelControl();
            this.lblFileCap = new DevExpress.XtraEditors.LabelControl();
            this.gridFile = new DevExpress.XtraGrid.GridControl();
            this.gridViewFile = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colFileName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFileSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblAssigneeCap = new DevExpress.XtraEditors.LabelControl();
            this.lblAssignee = new DevExpress.XtraEditors.LabelControl();
            this.lblFinishNoteCap = new DevExpress.XtraEditors.LabelControl();
            this.memFinishNote = new DevExpress.XtraEditors.MemoEdit();
            this.tabComment = new DevExpress.XtraTab.XtraTabPage();
            this.gridCmt = new DevExpress.XtraGrid.GridControl();
            this.gridViewCmt = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colCmtUser = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCmtTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCmtContent = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCmtEdit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repCmtEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.colCmtDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repCmtDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.pnlAction = new DevExpress.XtraEditors.PanelControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddCmt = new DevExpress.XtraEditors.SimpleButton();
            this.btnReceive = new DevExpress.XtraEditors.SimpleButton();
            this.btnFinish = new DevExpress.XtraEditors.SimpleButton();
            this.btnReject = new DevExpress.XtraEditors.SimpleButton();
            this.btnTransfer = new DevExpress.XtraEditors.SimpleButton();
            this.btnForward = new DevExpress.XtraEditors.SimpleButton();
            this.pnlList = new DevExpress.XtraEditors.PanelControl();
            this.gridList = new DevExpress.XtraGrid.GridControl();
            this.gridViewList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTitle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSttName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRequester = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDept = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRequestTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAssignee = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colReceiver = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFinishTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCrmCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFileCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCmtCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            ((System.ComponentModel.ISupportInitialize)(this.pnlFilter)).BeginInit();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDept.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBranch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRequester.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReceiver.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAssigneeFilter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlDetail)).BeginInit();
            this.pnlDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabDetail)).BeginInit();
            this.tabDetail.SuspendLayout();
            this.tabContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memContent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContact.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memFinishNote.Properties)).BeginInit();
            this.tabComment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCmt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCmt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repCmtEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repCmtDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAction)).BeginInit();
            this.pnlAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlList)).BeginInit();
            this.pnlList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repDelete)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFilter
            //
            this.pnlFilter.Controls.Add(this.lblFrom);
            this.pnlFilter.Controls.Add(this.dtFrom);
            this.pnlFilter.Controls.Add(this.lblTo);
            this.pnlFilter.Controls.Add(this.dtTo);
            this.pnlFilter.Controls.Add(this.lblStt);
            this.pnlFilter.Controls.Add(this.cboStt);
            this.pnlFilter.Controls.Add(this.lblDept);
            this.pnlFilter.Controls.Add(this.cboDept);
            this.pnlFilter.Controls.Add(this.lblBranch);
            this.pnlFilter.Controls.Add(this.cboBranch);
            this.pnlFilter.Controls.Add(this.lblRequester);
            this.pnlFilter.Controls.Add(this.cboRequester);
            this.pnlFilter.Controls.Add(this.lblReceiver);
            this.pnlFilter.Controls.Add(this.cboReceiver);
            this.pnlFilter.Controls.Add(this.lblAssigneeFilter);
            this.pnlFilter.Controls.Add(this.cboAssigneeFilter);
            this.pnlFilter.Controls.Add(this.btnMyTask);
            this.pnlFilter.Controls.Add(this.lblKeyword);
            this.pnlFilter.Controls.Add(this.txtKeyword);
            this.pnlFilter.Controls.Add(this.btnSearch);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(1366, 66);
            this.pnlFilter.TabIndex = 0;
            //
            // lblFrom
            //
            this.lblFrom.Location = new System.Drawing.Point(8, 11);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(45, 13);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "Từ ngày:";
            //
            // dtFrom
            //
            this.dtFrom.EditValue = null;
            this.dtFrom.Location = new System.Drawing.Point(66, 8);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtFrom.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtFrom.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtFrom.Size = new System.Drawing.Size(122, 20);
            this.dtFrom.TabIndex = 1;
            //
            // lblTo
            //
            this.lblTo.Location = new System.Drawing.Point(196, 11);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(52, 13);
            this.lblTo.TabIndex = 2;
            this.lblTo.Text = "Đến ngày:";
            //
            // dtTo
            //
            this.dtTo.EditValue = null;
            this.dtTo.Location = new System.Drawing.Point(256, 8);
            this.dtTo.Name = "dtTo";
            this.dtTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTo.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTo.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtTo.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtTo.Size = new System.Drawing.Size(122, 20);
            this.dtTo.TabIndex = 3;
            //
            // lblStt
            //
            this.lblStt.Location = new System.Drawing.Point(386, 11);
            this.lblStt.Name = "lblStt";
            this.lblStt.Size = new System.Drawing.Size(55, 13);
            this.lblStt.TabIndex = 4;
            this.lblStt.Text = "Trạng thái:";
            //
            // cboStt
            //
            this.cboStt.Location = new System.Drawing.Point(449, 8);
            this.cboStt.Name = "cboStt";
            this.cboStt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStt.Size = new System.Drawing.Size(190, 20);
            this.cboStt.TabIndex = 5;
            //
            // lblDept
            //
            this.lblDept.Location = new System.Drawing.Point(649, 11);
            this.lblDept.Name = "lblDept";
            this.lblDept.Size = new System.Drawing.Size(28, 13);
            this.lblDept.TabIndex = 6;
            this.lblDept.Text = "Khoa:";
            //
            // cboDept
            //
            this.cboDept.Location = new System.Drawing.Point(687, 8);
            this.cboDept.Name = "cboDept";
            this.cboDept.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboDept.Properties.NullText = "";
            this.cboDept.Size = new System.Drawing.Size(170, 20);
            this.cboDept.TabIndex = 7;
            this.cboDept.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboFilter_ButtonClick);
            //
            // lblBranch
            //
            this.lblBranch.Location = new System.Drawing.Point(867, 11);
            this.lblBranch.Name = "lblBranch";
            this.lblBranch.Size = new System.Drawing.Size(31, 13);
            this.lblBranch.TabIndex = 8;
            this.lblBranch.Text = "Cơ sở:";
            //
            // cboBranch
            //
            this.cboBranch.Location = new System.Drawing.Point(908, 8);
            this.cboBranch.Name = "cboBranch";
            this.cboBranch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboBranch.Properties.NullText = "";
            this.cboBranch.Size = new System.Drawing.Size(150, 20);
            this.cboBranch.TabIndex = 9;
            this.cboBranch.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboFilter_ButtonClick);
            //
            // lblRequester
            //
            this.lblRequester.Location = new System.Drawing.Point(8, 39);
            this.lblRequester.Name = "lblRequester";
            this.lblRequester.Size = new System.Drawing.Size(51, 13);
            this.lblRequester.TabIndex = 10;
            this.lblRequester.Text = "Người tạo:";
            //
            // cboRequester
            //
            this.cboRequester.Location = new System.Drawing.Point(66, 36);
            this.cboRequester.Name = "cboRequester";
            this.cboRequester.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboRequester.Properties.NullText = "";
            this.cboRequester.Size = new System.Drawing.Size(150, 20);
            this.cboRequester.TabIndex = 11;
            this.cboRequester.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboFilter_ButtonClick);
            //
            // lblReceiver
            //
            this.lblReceiver.Location = new System.Drawing.Point(224, 39);
            this.lblReceiver.Name = "lblReceiver";
            this.lblReceiver.Size = new System.Drawing.Size(81, 13);
            this.lblReceiver.TabIndex = 12;
            this.lblReceiver.Text = "Người tiếp nhận:";
            //
            // cboReceiver
            //
            this.cboReceiver.Location = new System.Drawing.Point(312, 36);
            this.cboReceiver.Name = "cboReceiver";
            this.cboReceiver.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboReceiver.Properties.NullText = "";
            this.cboReceiver.Size = new System.Drawing.Size(150, 20);
            this.cboReceiver.TabIndex = 13;
            this.cboReceiver.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboFilter_ButtonClick);
            //
            // lblAssigneeFilter
            //
            this.lblAssigneeFilter.Location = new System.Drawing.Point(470, 39);
            this.lblAssigneeFilter.Name = "lblAssigneeFilter";
            this.lblAssigneeFilter.Size = new System.Drawing.Size(103, 13);
            this.lblAssigneeFilter.TabIndex = 14;
            this.lblAssigneeFilter.Text = "Người được chỉ định:";
            //
            // cboAssigneeFilter
            //
            this.cboAssigneeFilter.Location = new System.Drawing.Point(580, 36);
            this.cboAssigneeFilter.Name = "cboAssigneeFilter";
            this.cboAssigneeFilter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboAssigneeFilter.Properties.NullText = "";
            this.cboAssigneeFilter.Size = new System.Drawing.Size(150, 20);
            this.cboAssigneeFilter.TabIndex = 15;
            this.cboAssigneeFilter.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboFilter_ButtonClick);
            //
            // btnMyTask
            //
            this.btnMyTask.Location = new System.Drawing.Point(736, 35);
            this.btnMyTask.Name = "btnMyTask";
            this.btnMyTask.Size = new System.Drawing.Size(84, 22);
            this.btnMyTask.TabIndex = 16;
            this.btnMyTask.Text = "Việc của tôi";
            this.btnMyTask.Click += new System.EventHandler(this.btnMyTask_Click);
            //
            // lblKeyword
            //
            this.lblKeyword.Location = new System.Drawing.Point(830, 39);
            this.lblKeyword.Name = "lblKeyword";
            this.lblKeyword.Size = new System.Drawing.Size(44, 13);
            this.lblKeyword.TabIndex = 17;
            this.lblKeyword.Text = "Từ khoá:";
            //
            // txtKeyword
            //
            this.txtKeyword.Location = new System.Drawing.Point(882, 36);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Mã yêu cầu / tiêu đề / nội dung";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Size = new System.Drawing.Size(200, 20);
            this.txtKeyword.TabIndex = 18;
            //
            // btnSearch
            //
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(1230, 35);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 22);
            this.btnSearch.TabIndex = 19;
            this.btnSearch.Text = "Tìm kiếm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // pnlDetail
            //
            this.pnlDetail.Controls.Add(this.tabDetail);
            this.pnlDetail.Controls.Add(this.pnlAction);
            this.pnlDetail.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDetail.Location = new System.Drawing.Point(0, 438);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Size = new System.Drawing.Size(1366, 330);
            this.pnlDetail.TabIndex = 1;
            //
            // tabDetail
            //
            this.tabDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDetail.Location = new System.Drawing.Point(2, 2);
            this.tabDetail.Name = "tabDetail";
            this.tabDetail.SelectedTabPage = this.tabContent;
            this.tabDetail.Size = new System.Drawing.Size(1362, 288);
            this.tabDetail.TabIndex = 0;
            this.tabDetail.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabContent,
            this.tabComment});
            //
            // tabContent
            //
            this.tabContent.Controls.Add(this.lblTitleCap);
            this.tabContent.Controls.Add(this.txtTitle);
            this.tabContent.Controls.Add(this.lblContentCap);
            this.tabContent.Controls.Add(this.memContent);
            this.tabContent.Controls.Add(this.lblContactCap);
            this.tabContent.Controls.Add(this.txtContact);
            this.tabContent.Controls.Add(this.lblAnydeskCap);
            this.tabContent.Controls.Add(this.lblAnydesk);
            this.tabContent.Controls.Add(this.lblModuleCap);
            this.tabContent.Controls.Add(this.lblModule);
            this.tabContent.Controls.Add(this.lblFileCap);
            this.tabContent.Controls.Add(this.gridFile);
            this.tabContent.Controls.Add(this.lblAssigneeCap);
            this.tabContent.Controls.Add(this.lblAssignee);
            this.tabContent.Controls.Add(this.lblFinishNoteCap);
            this.tabContent.Controls.Add(this.memFinishNote);
            this.tabContent.Name = "tabContent";
            this.tabContent.Size = new System.Drawing.Size(1356, 260);
            this.tabContent.Text = "Nội dung";
            //
            // lblTitleCap
            //
            this.lblTitleCap.Location = new System.Drawing.Point(10, 13);
            this.lblTitleCap.Name = "lblTitleCap";
            this.lblTitleCap.Size = new System.Drawing.Size(38, 13);
            this.lblTitleCap.TabIndex = 0;
            this.lblTitleCap.Text = "Tiêu đề:";
            //
            // txtTitle
            //
            this.txtTitle.Location = new System.Drawing.Point(124, 10);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(640, 20);
            this.txtTitle.TabIndex = 1;
            //
            // lblContentCap
            //
            this.lblContentCap.Location = new System.Drawing.Point(10, 41);
            this.lblContentCap.Name = "lblContentCap";
            this.lblContentCap.Size = new System.Drawing.Size(48, 13);
            this.lblContentCap.TabIndex = 2;
            this.lblContentCap.Text = "Nội dung:";
            //
            // memContent
            //
            this.memContent.Location = new System.Drawing.Point(124, 38);
            this.memContent.Name = "memContent";
            this.memContent.Size = new System.Drawing.Size(640, 140);
            this.memContent.TabIndex = 3;
            //
            // lblContactCap
            //
            this.lblContactCap.Location = new System.Drawing.Point(10, 190);
            this.lblContactCap.Name = "lblContactCap";
            this.lblContactCap.Size = new System.Drawing.Size(89, 13);
            this.lblContactCap.TabIndex = 4;
            this.lblContactCap.Text = "Thông tin liên lạc:";
            //
            // txtContact
            //
            this.txtContact.Location = new System.Drawing.Point(124, 187);
            this.txtContact.Name = "txtContact";
            this.txtContact.Size = new System.Drawing.Size(640, 20);
            this.txtContact.TabIndex = 5;
            //
            // lblAnydeskCap
            //
            this.lblAnydeskCap.Location = new System.Drawing.Point(786, 13);
            this.lblAnydeskCap.Name = "lblAnydeskCap";
            this.lblAnydeskCap.Size = new System.Drawing.Size(102, 13);
            this.lblAnydeskCap.TabIndex = 6;
            this.lblAnydeskCap.Text = "Mã điều khiển từ xa:";
            //
            // lblAnydesk
            //
            this.lblAnydesk.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblAnydesk.Appearance.Options.UseFont = true;
            this.lblAnydesk.Location = new System.Drawing.Point(906, 13);
            this.lblAnydesk.Name = "lblAnydesk";
            this.lblAnydesk.Size = new System.Drawing.Size(0, 13);
            this.lblAnydesk.TabIndex = 7;
            //
            // lblModuleCap
            //
            this.lblModuleCap.Location = new System.Drawing.Point(786, 36);
            this.lblModuleCap.Name = "lblModuleCap";
            this.lblModuleCap.Size = new System.Drawing.Size(96, 13);
            this.lblModuleCap.TabIndex = 8;
            this.lblModuleCap.Text = "Màn hình phát sinh:";
            //
            // lblModule
            //
            this.lblModule.Location = new System.Drawing.Point(906, 36);
            this.lblModule.Name = "lblModule";
            this.lblModule.Size = new System.Drawing.Size(0, 13);
            this.lblModule.TabIndex = 9;
            //
            // lblFileCap
            //
            this.lblFileCap.Location = new System.Drawing.Point(786, 62);
            this.lblFileCap.Name = "lblFileCap";
            this.lblFileCap.Size = new System.Drawing.Size(68, 13);
            this.lblFileCap.TabIndex = 10;
            this.lblFileCap.Text = "Tệp đính kèm:";
            //
            // gridFile
            //
            this.gridFile.Location = new System.Drawing.Point(906, 59);
            this.gridFile.MainView = this.gridViewFile;
            this.gridFile.Name = "gridFile";
            this.gridFile.Size = new System.Drawing.Size(420, 86);
            this.gridFile.TabIndex = 11;
            this.gridFile.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewFile});
            //
            // gridViewFile
            //
            this.gridViewFile.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colFileName,
            this.colFileSize});
            this.gridViewFile.GridControl = this.gridFile;
            this.gridViewFile.Name = "gridViewFile";
            this.gridViewFile.OptionsBehavior.Editable = false;
            this.gridViewFile.OptionsView.ShowGroupPanel = false;
            this.gridViewFile.DoubleClick += new System.EventHandler(this.gridViewFile_DoubleClick);
            this.gridViewFile.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewFile_CustomColumnDisplayText);
            //
            // colFileName
            //
            this.colFileName.Caption = "Tên tệp";
            this.colFileName.FieldName = "FILE_NAME";
            this.colFileName.Name = "colFileName";
            this.colFileName.Visible = true;
            this.colFileName.VisibleIndex = 0;
            this.colFileName.Width = 300;
            //
            // colFileSize
            //
            this.colFileSize.Caption = "Dung lượng";
            this.colFileSize.FieldName = "FILE_SIZE";
            this.colFileSize.Name = "colFileSize";
            this.colFileSize.Visible = true;
            this.colFileSize.VisibleIndex = 1;
            this.colFileSize.Width = 90;
            //
            // lblAssigneeCap
            //
            this.lblAssigneeCap.Location = new System.Drawing.Point(786, 155);
            this.lblAssigneeCap.Name = "lblAssigneeCap";
            this.lblAssigneeCap.Size = new System.Drawing.Size(103, 13);
            this.lblAssigneeCap.TabIndex = 12;
            this.lblAssigneeCap.Text = "Người được chỉ định:";
            //
            // lblAssignee
            //
            this.lblAssignee.Location = new System.Drawing.Point(906, 155);
            this.lblAssignee.Name = "lblAssignee";
            this.lblAssignee.Size = new System.Drawing.Size(0, 13);
            this.lblAssignee.TabIndex = 13;
            //
            // lblFinishNoteCap
            //
            this.lblFinishNoteCap.Location = new System.Drawing.Point(786, 181);
            this.lblFinishNoteCap.Name = "lblFinishNoteCap";
            this.lblFinishNoteCap.Size = new System.Drawing.Size(70, 13);
            this.lblFinishNoteCap.TabIndex = 14;
            this.lblFinishNoteCap.Text = "Ghi chú xử lý:";
            //
            // memFinishNote
            //
            this.memFinishNote.Location = new System.Drawing.Point(906, 178);
            this.memFinishNote.Name = "memFinishNote";
            this.memFinishNote.Size = new System.Drawing.Size(420, 60);
            this.memFinishNote.TabIndex = 15;
            //
            // tabComment
            //
            this.tabComment.Controls.Add(this.gridCmt);
            this.tabComment.Name = "tabComment";
            this.tabComment.Size = new System.Drawing.Size(1356, 260);
            this.tabComment.Text = "Trao đổi";
            //
            // gridCmt
            //
            this.gridCmt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCmt.Location = new System.Drawing.Point(0, 0);
            this.gridCmt.MainView = this.gridViewCmt;
            this.gridCmt.Name = "gridCmt";
            this.gridCmt.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repCmtEdit,
            this.repCmtDelete});
            this.gridCmt.Size = new System.Drawing.Size(1356, 260);
            this.gridCmt.TabIndex = 0;
            this.gridCmt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCmt});
            //
            // gridViewCmt
            //
            this.gridViewCmt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCmtUser,
            this.colCmtTime,
            this.colCmtContent,
            this.colCmtEdit,
            this.colCmtDelete});
            this.gridViewCmt.GridControl = this.gridCmt;
            this.gridViewCmt.Name = "gridViewCmt";
            this.gridViewCmt.OptionsView.ShowGroupPanel = false;
            this.gridViewCmt.OptionsView.RowAutoHeight = true;
            this.gridViewCmt.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewCmt_RowCellStyle);
            this.gridViewCmt.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewCmt_CustomRowCellEdit);
            this.gridViewCmt.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewCmt_CustomColumnDisplayText);
            //
            // colCmtUser
            //
            this.colCmtUser.Caption = "Người viết";
            this.colCmtUser.FieldName = "CMT_USERNAME";
            this.colCmtUser.Name = "colCmtUser";
            this.colCmtUser.OptionsColumn.AllowEdit = false;
            this.colCmtUser.Visible = true;
            this.colCmtUser.VisibleIndex = 0;
            this.colCmtUser.Width = 170;
            //
            // colCmtTime
            //
            this.colCmtTime.Caption = "Thời gian";
            this.colCmtTime.FieldName = "CMT_TIME";
            this.colCmtTime.Name = "colCmtTime";
            this.colCmtTime.OptionsColumn.AllowEdit = false;
            this.colCmtTime.Visible = true;
            this.colCmtTime.VisibleIndex = 1;
            this.colCmtTime.Width = 120;
            //
            // colCmtContent
            //
            this.colCmtContent.Caption = "Nội dung";
            this.colCmtContent.FieldName = "CONTENT";
            this.colCmtContent.Name = "colCmtContent";
            this.colCmtContent.OptionsColumn.AllowEdit = false;
            this.colCmtContent.Visible = true;
            this.colCmtContent.VisibleIndex = 2;
            this.colCmtContent.Width = 900;
            //
            // colCmtEdit
            //
            this.colCmtEdit.Caption = "Sửa";
            this.colCmtEdit.ColumnEdit = this.repCmtEdit;
            this.colCmtEdit.Name = "colCmtEdit";
            this.colCmtEdit.Visible = true;
            this.colCmtEdit.VisibleIndex = 3;
            this.colCmtEdit.Width = 50;
            //
            // repCmtEdit
            //
            this.repCmtEdit.AutoHeight = false;
            this.repCmtEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Sửa", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null)});
            this.repCmtEdit.Name = "repCmtEdit";
            this.repCmtEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repCmtEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repCmtEdit_ButtonClick);
            //
            // colCmtDelete
            //
            this.colCmtDelete.Caption = "Xoá";
            this.colCmtDelete.ColumnEdit = this.repCmtDelete;
            this.colCmtDelete.Name = "colCmtDelete";
            this.colCmtDelete.Visible = true;
            this.colCmtDelete.VisibleIndex = 4;
            this.colCmtDelete.Width = 50;
            //
            // repCmtDelete
            //
            this.repCmtDelete.AutoHeight = false;
            this.repCmtDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Xoá", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null)});
            this.repCmtDelete.Name = "repCmtDelete";
            this.repCmtDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repCmtDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repCmtDelete_ButtonClick);
            //
            // pnlAction
            //
            this.pnlAction.Controls.Add(this.btnSave);
            this.pnlAction.Controls.Add(this.btnAddCmt);
            this.pnlAction.Controls.Add(this.btnReceive);
            this.pnlAction.Controls.Add(this.btnFinish);
            this.pnlAction.Controls.Add(this.btnReject);
            this.pnlAction.Controls.Add(this.btnTransfer);
            this.pnlAction.Controls.Add(this.btnForward);
            this.pnlAction.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAction.Location = new System.Drawing.Point(2, 290);
            this.pnlAction.Name = "pnlAction";
            this.pnlAction.Size = new System.Drawing.Size(1362, 38);
            this.pnlAction.TabIndex = 1;
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(644, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 22);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Lưu";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnAddCmt
            //
            this.btnAddCmt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddCmt.Location = new System.Drawing.Point(730, 8);
            this.btnAddCmt.Name = "btnAddCmt";
            this.btnAddCmt.Size = new System.Drawing.Size(100, 22);
            this.btnAddCmt.TabIndex = 1;
            this.btnAddCmt.Text = "Thêm trao đổi";
            this.btnAddCmt.Click += new System.EventHandler(this.btnAddCmt_Click);
            //
            // btnReceive
            //
            this.btnReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReceive.Location = new System.Drawing.Point(836, 8);
            this.btnReceive.Name = "btnReceive";
            this.btnReceive.Size = new System.Drawing.Size(90, 22);
            this.btnReceive.TabIndex = 2;
            this.btnReceive.Text = "Tiếp nhận";
            this.btnReceive.Click += new System.EventHandler(this.btnReceive_Click);
            //
            // btnFinish
            //
            this.btnFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFinish.Location = new System.Drawing.Point(932, 8);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(90, 22);
            this.btnFinish.TabIndex = 3;
            this.btnFinish.Text = "Hoàn thành";
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            //
            // btnReject
            //
            this.btnReject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReject.Location = new System.Drawing.Point(1028, 8);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(90, 22);
            this.btnReject.TabIndex = 4;
            this.btnReject.Text = "Từ chối";
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            //
            // btnTransfer
            //
            this.btnTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransfer.Location = new System.Drawing.Point(1124, 8);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(100, 22);
            this.btnTransfer.TabIndex = 5;
            this.btnTransfer.Text = "Chuyển việc";
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            //
            // btnForward
            //
            this.btnForward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnForward.Location = new System.Drawing.Point(1230, 8);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(120, 22);
            this.btnForward.TabIndex = 6;
            this.btnForward.Text = "Chuyển công ty";
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            //
            // pnlList
            //
            this.pnlList.Controls.Add(this.gridList);
            this.pnlList.Controls.Add(this.ucPaging);
            this.pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlList.Location = new System.Drawing.Point(0, 66);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(1366, 372);
            this.pnlList.TabIndex = 2;
            //
            // gridList
            //
            this.gridList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridList.Location = new System.Drawing.Point(2, 2);
            this.gridList.MainView = this.gridViewList;
            this.gridList.Name = "gridList";
            this.gridList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repDelete});
            this.gridList.Size = new System.Drawing.Size(1362, 346);
            this.gridList.TabIndex = 0;
            this.gridList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewList});
            //
            // gridViewList
            //
            this.gridViewList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCode,
            this.colTitle,
            this.colSttName,
            this.colRequester,
            this.colDept,
            this.colRequestTime,
            this.colAssignee,
            this.colReceiver,
            this.colFinishTime,
            this.colCrmCode,
            this.colFileCount,
            this.colCmtCount,
            this.colDelete});
            this.gridViewList.GridControl = this.gridList;
            this.gridViewList.Name = "gridViewList";
            this.gridViewList.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewList.OptionsView.ShowGroupPanel = false;
            this.gridViewList.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewList_FocusedRowChanged);
            this.gridViewList.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewList_CustomRowCellEdit);
            this.gridViewList.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewList_CustomColumnDisplayText);
            //
            // colCode
            //
            this.colCode.Caption = "Mã yêu cầu";
            this.colCode.FieldName = "SUPPORT_REQUEST_CODE";
            this.colCode.Name = "colCode";
            this.colCode.OptionsColumn.AllowEdit = false;
            this.colCode.Visible = true;
            this.colCode.VisibleIndex = 0;
            this.colCode.Width = 110;
            //
            // colTitle
            //
            this.colTitle.Caption = "Tiêu đề";
            this.colTitle.FieldName = "TITLE";
            this.colTitle.Name = "colTitle";
            this.colTitle.OptionsColumn.AllowEdit = false;
            this.colTitle.Visible = true;
            this.colTitle.VisibleIndex = 1;
            this.colTitle.Width = 250;
            //
            // colSttName
            //
            this.colSttName.Caption = "Trạng thái";
            this.colSttName.FieldName = "SUPPORT_REQUEST_STT_NAME";
            this.colSttName.Name = "colSttName";
            this.colSttName.OptionsColumn.AllowEdit = false;
            this.colSttName.Visible = true;
            this.colSttName.VisibleIndex = 2;
            this.colSttName.Width = 120;
            //
            // colRequester
            //
            this.colRequester.Caption = "Người tạo";
            this.colRequester.FieldName = "REQUEST_USERNAME";
            this.colRequester.Name = "colRequester";
            this.colRequester.OptionsColumn.AllowEdit = false;
            this.colRequester.Visible = true;
            this.colRequester.VisibleIndex = 3;
            this.colRequester.Width = 125;
            //
            // colDept
            //
            this.colDept.Caption = "Khoa";
            this.colDept.FieldName = "REQUEST_DEPARTMENT_NAME";
            this.colDept.Name = "colDept";
            this.colDept.OptionsColumn.AllowEdit = false;
            this.colDept.Visible = true;
            this.colDept.VisibleIndex = 4;
            this.colDept.Width = 135;
            //
            // colRequestTime
            //
            this.colRequestTime.Caption = "Thời gian tạo";
            this.colRequestTime.FieldName = "REQUEST_TIME";
            this.colRequestTime.Name = "colRequestTime";
            this.colRequestTime.OptionsColumn.AllowEdit = false;
            this.colRequestTime.Visible = true;
            this.colRequestTime.VisibleIndex = 5;
            this.colRequestTime.Width = 115;
            //
            // colAssignee
            //
            this.colAssignee.Caption = "Người được chỉ định";
            this.colAssignee.FieldName = "ASSIGNEE_USERNAME";
            this.colAssignee.Name = "colAssignee";
            this.colAssignee.OptionsColumn.AllowEdit = false;
            this.colAssignee.Visible = true;
            this.colAssignee.VisibleIndex = 6;
            this.colAssignee.Width = 130;
            //
            // colReceiver
            //
            this.colReceiver.Caption = "Người tiếp nhận";
            this.colReceiver.FieldName = "RECEIVE_USERNAME";
            this.colReceiver.Name = "colReceiver";
            this.colReceiver.OptionsColumn.AllowEdit = false;
            this.colReceiver.Visible = true;
            this.colReceiver.VisibleIndex = 7;
            this.colReceiver.Width = 130;
            //
            // colFinishTime
            //
            this.colFinishTime.Caption = "TG hoàn thành";
            this.colFinishTime.FieldName = "FINISH_TIME";
            this.colFinishTime.Name = "colFinishTime";
            this.colFinishTime.OptionsColumn.AllowEdit = false;
            this.colFinishTime.Visible = true;
            this.colFinishTime.VisibleIndex = 8;
            this.colFinishTime.Width = 110;
            //
            // colCrmCode
            //
            this.colCrmCode.Caption = "Mã bên công ty";
            this.colCrmCode.FieldName = "CRM_REQUEST_CODE";
            this.colCrmCode.Name = "colCrmCode";
            this.colCrmCode.OptionsColumn.AllowEdit = false;
            this.colCrmCode.Visible = true;
            this.colCrmCode.VisibleIndex = 9;
            this.colCrmCode.Width = 100;
            //
            // colFileCount
            //
            this.colFileCount.Caption = "Tệp";
            this.colFileCount.FieldName = "FILE_COUNT";
            this.colFileCount.Name = "colFileCount";
            this.colFileCount.OptionsColumn.AllowEdit = false;
            this.colFileCount.Visible = true;
            this.colFileCount.VisibleIndex = 10;
            this.colFileCount.Width = 46;
            //
            // colCmtCount
            //
            this.colCmtCount.Caption = "Trao đổi";
            this.colCmtCount.FieldName = "CMT_COUNT";
            this.colCmtCount.Name = "colCmtCount";
            this.colCmtCount.OptionsColumn.AllowEdit = false;
            this.colCmtCount.Visible = true;
            this.colCmtCount.VisibleIndex = 11;
            this.colCmtCount.Width = 56;
            //
            // colDelete
            //
            this.colDelete.Caption = "Xoá";
            this.colDelete.ColumnEdit = this.repDelete;
            this.colDelete.Name = "colDelete";
            this.colDelete.Visible = true;
            this.colDelete.VisibleIndex = 12;
            this.colDelete.Width = 44;
            //
            // repDelete
            //
            this.repDelete.AutoHeight = false;
            this.repDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.repDelete.Name = "repDelete";
            this.repDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repDelete_ButtonClick);
            //
            // ucPaging
            //
            this.ucPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucPaging.Location = new System.Drawing.Point(2, 348);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(1362, 22);
            this.ucPaging.TabIndex = 1;
            //
            // UCSupportRequestManagement
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlList);
            this.Controls.Add(this.pnlDetail);
            this.Controls.Add(this.pnlFilter);
            this.Name = "UCSupportRequestManagement";
            this.Size = new System.Drawing.Size(1366, 768);
            this.Load += new System.EventHandler(this.UCSupportRequestManagement_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pnlFilter)).EndInit();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDept.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBranch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRequester.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReceiver.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAssigneeFilter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlDetail)).EndInit();
            this.pnlDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabDetail)).EndInit();
            this.tabDetail.ResumeLayout(false);
            this.tabContent.ResumeLayout(false);
            this.tabContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memContent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContact.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memFinishNote.Properties)).EndInit();
            this.tabComment.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridCmt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCmt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repCmtEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repCmtDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAction)).EndInit();
            this.pnlAction.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlList)).EndInit();
            this.pnlList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repDelete)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlFilter;
        private DevExpress.XtraEditors.LabelControl lblFrom;
        private DevExpress.XtraEditors.DateEdit dtFrom;
        private DevExpress.XtraEditors.LabelControl lblTo;
        private DevExpress.XtraEditors.DateEdit dtTo;
        private DevExpress.XtraEditors.LabelControl lblStt;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cboStt;
        private DevExpress.XtraEditors.LabelControl lblDept;
        private DevExpress.XtraEditors.LookUpEdit cboDept;
        private DevExpress.XtraEditors.LabelControl lblBranch;
        private DevExpress.XtraEditors.LookUpEdit cboBranch;
        private DevExpress.XtraEditors.LabelControl lblRequester;
        private DevExpress.XtraEditors.GridLookUpEdit cboRequester;
        private DevExpress.XtraEditors.LabelControl lblReceiver;
        private DevExpress.XtraEditors.GridLookUpEdit cboReceiver;
        private DevExpress.XtraEditors.LabelControl lblAssigneeFilter;
        private DevExpress.XtraEditors.GridLookUpEdit cboAssigneeFilter;
        private DevExpress.XtraEditors.SimpleButton btnMyTask;
        private DevExpress.XtraEditors.LabelControl lblKeyword;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.PanelControl pnlDetail;
        private DevExpress.XtraTab.XtraTabControl tabDetail;
        private DevExpress.XtraTab.XtraTabPage tabContent;
        private DevExpress.XtraEditors.LabelControl lblTitleCap;
        private DevExpress.XtraEditors.TextEdit txtTitle;
        private DevExpress.XtraEditors.LabelControl lblContentCap;
        private DevExpress.XtraEditors.MemoEdit memContent;
        private DevExpress.XtraEditors.LabelControl lblContactCap;
        private DevExpress.XtraEditors.TextEdit txtContact;
        private DevExpress.XtraEditors.LabelControl lblAnydeskCap;
        private DevExpress.XtraEditors.LabelControl lblAnydesk;
        private DevExpress.XtraEditors.LabelControl lblModuleCap;
        private DevExpress.XtraEditors.LabelControl lblModule;
        private DevExpress.XtraEditors.LabelControl lblFileCap;
        private DevExpress.XtraGrid.GridControl gridFile;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewFile;
        private DevExpress.XtraGrid.Columns.GridColumn colFileName;
        private DevExpress.XtraGrid.Columns.GridColumn colFileSize;
        private DevExpress.XtraEditors.LabelControl lblAssigneeCap;
        private DevExpress.XtraEditors.LabelControl lblAssignee;
        private DevExpress.XtraEditors.LabelControl lblFinishNoteCap;
        private DevExpress.XtraEditors.MemoEdit memFinishNote;
        private DevExpress.XtraTab.XtraTabPage tabComment;
        private DevExpress.XtraGrid.GridControl gridCmt;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewCmt;
        private DevExpress.XtraGrid.Columns.GridColumn colCmtUser;
        private DevExpress.XtraGrid.Columns.GridColumn colCmtTime;
        private DevExpress.XtraGrid.Columns.GridColumn colCmtContent;
        private DevExpress.XtraGrid.Columns.GridColumn colCmtEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repCmtEdit;
        private DevExpress.XtraGrid.Columns.GridColumn colCmtDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repCmtDelete;
        private DevExpress.XtraEditors.PanelControl pnlAction;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnAddCmt;
        private DevExpress.XtraEditors.SimpleButton btnReceive;
        private DevExpress.XtraEditors.SimpleButton btnFinish;
        private DevExpress.XtraEditors.SimpleButton btnReject;
        private DevExpress.XtraEditors.SimpleButton btnTransfer;
        private DevExpress.XtraEditors.SimpleButton btnForward;
        private DevExpress.XtraEditors.PanelControl pnlList;
        private DevExpress.XtraGrid.GridControl gridList;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewList;
        private DevExpress.XtraGrid.Columns.GridColumn colCode;
        private DevExpress.XtraGrid.Columns.GridColumn colTitle;
        private DevExpress.XtraGrid.Columns.GridColumn colSttName;
        private DevExpress.XtraGrid.Columns.GridColumn colRequester;
        private DevExpress.XtraGrid.Columns.GridColumn colDept;
        private DevExpress.XtraGrid.Columns.GridColumn colRequestTime;
        private DevExpress.XtraGrid.Columns.GridColumn colAssignee;
        private DevExpress.XtraGrid.Columns.GridColumn colReceiver;
        private DevExpress.XtraGrid.Columns.GridColumn colFinishTime;
        private DevExpress.XtraGrid.Columns.GridColumn colCrmCode;
        private DevExpress.XtraGrid.Columns.GridColumn colFileCount;
        private DevExpress.XtraGrid.Columns.GridColumn colCmtCount;
        private DevExpress.XtraGrid.Columns.GridColumn colDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repDelete;
        private Inventec.UC.Paging.UcPaging ucPaging;
    }
}

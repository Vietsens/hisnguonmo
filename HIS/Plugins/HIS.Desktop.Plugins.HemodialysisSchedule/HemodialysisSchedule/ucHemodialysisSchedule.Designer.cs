/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule
{
    partial class ucHemodialysisSchedule
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucHemodialysisSchedule));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControlMain = new DevExpress.XtraLayout.LayoutControl();
            this.txtNote = new DevExpress.XtraEditors.TextEdit();
            this.cboTemplate = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewTemplate = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnNextShift = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrevShift = new DevExpress.XtraEditors.SimpleButton();
            this.btnNextDate = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrevDate = new DevExpress.XtraEditors.SimpleButton();
            this.txtDepartmentCode = new DevExpress.XtraEditors.TextEdit();
            this.txtRoomCode = new DevExpress.XtraEditors.TextEdit();
            this.cboRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewRoom = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dtDate = new DevExpress.XtraEditors.DateEdit();
            this.cboShift = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewShift = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtSearchTop = new DevExpress.XtraEditors.TextEdit();
            this.btnSearchTop = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCopy = new DevExpress.XtraEditors.SimpleButton();
            this.dtCopyFromDate = new DevExpress.XtraEditors.DateEdit();
            this.gridControlSchedule = new DevExpress.XtraGrid.GridControl();
            this.gridViewSchedule = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.colSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colScheduleDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colShift = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colInTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTemplate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoTemplate = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
            this.repoTemplateView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repoShift = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.colNote = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cboDepartment = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewDept = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkAllDepartment = new DevExpress.XtraEditors.CheckEdit();
            this.dtInTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.dtInTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.txtSearchBottom = new DevExpress.XtraEditors.TextEdit();
            this.btnSearchBottom = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddToSchedule = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlTreatment = new DevExpress.XtraGrid.GridControl();
            this.gridViewTreatment = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSelect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.colDeleteB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoDeleteB = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoEmptyB = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.colSTTb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientNameB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientCodeB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTreatmentCodeB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDobB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGenderB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colInTimeB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTreatmentTypeB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHeinCardB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIcdNameB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lcgRoot = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciShift = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSearchTop = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnSearchTop = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnCopy = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCopyFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptyTop = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciGridSchedule = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDept = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAllDept = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSearchBottom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnSearchBottom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnAdd = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGridTreatment = new DevExpress.XtraLayout.LayoutControlItem();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.lciPaging = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRoomCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDepartmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnPrevDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnNextDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnPrevShift = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnNextShift = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciBtnSave = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnPrint = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTemplate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNote = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).BeginInit();
            this.layoutControlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNote.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTemplate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRoomCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboShift.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchTop.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCopyFromDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCopyFromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTemplateView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchBottom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDeleteB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEmptyB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgRoot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearchTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSearchTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCopyFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptyTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAllDept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearchBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSearchBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridTreatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaging)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoomCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnPrevDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnNextDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnPrevShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnNextShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnPrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlMain
            // 
            this.layoutControlMain.Controls.Add(this.txtNote);
            this.layoutControlMain.Controls.Add(this.cboTemplate);
            this.layoutControlMain.Controls.Add(this.btnNextShift);
            this.layoutControlMain.Controls.Add(this.btnPrevShift);
            this.layoutControlMain.Controls.Add(this.btnNextDate);
            this.layoutControlMain.Controls.Add(this.btnPrevDate);
            this.layoutControlMain.Controls.Add(this.txtDepartmentCode);
            this.layoutControlMain.Controls.Add(this.txtRoomCode);
            this.layoutControlMain.Controls.Add(this.cboRoom);
            this.layoutControlMain.Controls.Add(this.dtDate);
            this.layoutControlMain.Controls.Add(this.cboShift);
            this.layoutControlMain.Controls.Add(this.txtSearchTop);
            this.layoutControlMain.Controls.Add(this.btnSearchTop);
            this.layoutControlMain.Controls.Add(this.btnPrint);
            this.layoutControlMain.Controls.Add(this.btnSave);
            this.layoutControlMain.Controls.Add(this.btnCopy);
            this.layoutControlMain.Controls.Add(this.dtCopyFromDate);
            this.layoutControlMain.Controls.Add(this.gridControlSchedule);
            this.layoutControlMain.Controls.Add(this.cboDepartment);
            this.layoutControlMain.Controls.Add(this.chkAllDepartment);
            this.layoutControlMain.Controls.Add(this.dtInTimeFrom);
            this.layoutControlMain.Controls.Add(this.dtInTimeTo);
            this.layoutControlMain.Controls.Add(this.txtSearchBottom);
            this.layoutControlMain.Controls.Add(this.btnSearchBottom);
            this.layoutControlMain.Controls.Add(this.btnAddToSchedule);
            this.layoutControlMain.Controls.Add(this.gridControlTreatment);
            this.layoutControlMain.Controls.Add(this.ucPaging);
            this.layoutControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlMain.Location = new System.Drawing.Point(0, 0);
            this.layoutControlMain.Name = "layoutControlMain";
            this.layoutControlMain.Root = this.lcgRoot;
            this.layoutControlMain.Size = new System.Drawing.Size(1250, 700);
            this.layoutControlMain.TabIndex = 0;
            this.layoutControlMain.Text = "layoutControlMain";
            // 
            // txtNote
            // 
            this.txtNote.Location = new System.Drawing.Point(698, 317);
            this.txtNote.Name = "txtNote";
            this.txtNote.Size = new System.Drawing.Size(348, 20);
            this.txtNote.StyleController = this.layoutControlMain;
            this.txtNote.TabIndex = 26;
            // 
            // cboTemplate
            // 
            this.cboTemplate.Location = new System.Drawing.Point(77, 317);
            this.cboTemplate.Name = "cboTemplate";
            this.cboTemplate.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTemplate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboTemplate.Properties.NullText = "";
            this.cboTemplate.Properties.View = this.gridViewTemplate;
            this.cboTemplate.Size = new System.Drawing.Size(532, 20);
            this.cboTemplate.StyleController = this.layoutControlMain;
            this.cboTemplate.TabIndex = 25;
            this.cboTemplate.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboClearButton_ButtonClick);
            // 
            // gridViewTemplate
            // 
            this.gridViewTemplate.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewTemplate.Name = "gridViewTemplate";
            this.gridViewTemplate.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewTemplate.OptionsView.ShowGroupPanel = false;
            // 
            // btnNextShift
            // 
            this.btnNextShift.Image = ((System.Drawing.Image)(resources.GetObject("btnNextShift.Image")));
            this.btnNextShift.Location = new System.Drawing.Point(387, 26);
            this.btnNextShift.Name = "btnNextShift";
            this.btnNextShift.Size = new System.Drawing.Size(24, 22);
            this.btnNextShift.StyleController = this.layoutControlMain;
            this.btnNextShift.TabIndex = 24;
            this.btnNextShift.Click += new System.EventHandler(this.btnNextShift_Click);
            // 
            // btnPrevShift
            // 
            this.btnPrevShift.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevShift.Image")));
            this.btnPrevShift.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPrevShift.Location = new System.Drawing.Point(359, 26);
            this.btnPrevShift.Name = "btnPrevShift";
            this.btnPrevShift.Size = new System.Drawing.Size(24, 22);
            this.btnPrevShift.StyleController = this.layoutControlMain;
            this.btnPrevShift.TabIndex = 23;
            this.btnPrevShift.Click += new System.EventHandler(this.btnPrevShift_Click);
            // 
            // btnNextDate
            // 
            this.btnNextDate.Image = ((System.Drawing.Image)(resources.GetObject("btnNextDate.Image")));
            this.btnNextDate.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnNextDate.Location = new System.Drawing.Point(232, 26);
            this.btnNextDate.Name = "btnNextDate";
            this.btnNextDate.Size = new System.Drawing.Size(24, 22);
            this.btnNextDate.StyleController = this.layoutControlMain;
            this.btnNextDate.TabIndex = 22;
            this.btnNextDate.Click += new System.EventHandler(this.btnNextDate_Click);
            // 
            // btnPrevDate
            // 
            this.btnPrevDate.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevDate.Image")));
            this.btnPrevDate.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPrevDate.Location = new System.Drawing.Point(204, 26);
            this.btnPrevDate.Name = "btnPrevDate";
            this.btnPrevDate.Size = new System.Drawing.Size(24, 22);
            this.btnPrevDate.StyleController = this.layoutControlMain;
            this.btnPrevDate.TabIndex = 21;
            this.btnPrevDate.Click += new System.EventHandler(this.btnPrevDate_Click);
            // 
            // txtDepartmentCode
            // 
            this.txtDepartmentCode.Location = new System.Drawing.Point(77, 291);
            this.txtDepartmentCode.Name = "txtDepartmentCode";
            this.txtDepartmentCode.Size = new System.Drawing.Size(77, 20);
            this.txtDepartmentCode.StyleController = this.layoutControlMain;
            this.txtDepartmentCode.TabIndex = 20;
            this.txtDepartmentCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtDepartmentCode_KeyUp);
            // 
            // txtRoomCode
            // 
            this.txtRoomCode.Location = new System.Drawing.Point(77, 2);
            this.txtRoomCode.Name = "txtRoomCode";
            this.txtRoomCode.Size = new System.Drawing.Size(77, 20);
            this.txtRoomCode.StyleController = this.layoutControlMain;
            this.txtRoomCode.TabIndex = 19;
            this.txtRoomCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtRoomCode_KeyUp);
            // 
            // cboRoom
            // 
            this.cboRoom.Location = new System.Drawing.Point(154, 2);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboRoom.Properties.NullText = "";
            this.cboRoom.Properties.View = this.gridViewRoom;
            this.cboRoom.Size = new System.Drawing.Size(345, 20);
            this.cboRoom.StyleController = this.layoutControlMain;
            this.cboRoom.TabIndex = 0;
            this.cboRoom.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboClearButton_ButtonClick);
            this.cboRoom.EditValueChanged += new System.EventHandler(this.cboRoom_EditValueChanged);
            // 
            // gridViewRoom
            // 
            this.gridViewRoom.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewRoom.Name = "gridViewRoom";
            this.gridViewRoom.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewRoom.OptionsView.ShowGroupPanel = false;
            // 
            // dtDate
            // 
            this.dtDate.EditValue = null;
            this.dtDate.Location = new System.Drawing.Point(77, 26);
            this.dtDate.Name = "dtDate";
            this.dtDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtDate.Size = new System.Drawing.Size(123, 20);
            this.dtDate.StyleController = this.layoutControlMain;
            this.dtDate.TabIndex = 1;
            // 
            // cboShift
            // 
            this.cboShift.Location = new System.Drawing.Point(305, 26);
            this.cboShift.Name = "cboShift";
            this.cboShift.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboShift.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboShift.Properties.NullText = "";
            this.cboShift.Properties.View = this.gridViewShift;
            this.cboShift.Size = new System.Drawing.Size(50, 20);
            this.cboShift.StyleController = this.layoutControlMain;
            this.cboShift.TabIndex = 2;
            this.cboShift.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboClearButton_ButtonClick);
            // 
            // gridViewShift
            // 
            this.gridViewShift.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewShift.Name = "gridViewShift";
            this.gridViewShift.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewShift.OptionsView.ShowGroupPanel = false;
            // 
            // txtSearchTop
            // 
            this.txtSearchTop.Location = new System.Drawing.Point(415, 26);
            this.txtSearchTop.Name = "txtSearchTop";
            this.txtSearchTop.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearchTop.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearchTop.Size = new System.Drawing.Size(309, 20);
            this.txtSearchTop.StyleController = this.layoutControlMain;
            this.txtSearchTop.TabIndex = 3;
            this.txtSearchTop.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearchTop_KeyUp);
            // 
            // btnSearchTop
            // 
            this.btnSearchTop.Location = new System.Drawing.Point(728, 26);
            this.btnSearchTop.Name = "btnSearchTop";
            this.btnSearchTop.Size = new System.Drawing.Size(98, 22);
            this.btnSearchTop.StyleController = this.layoutControlMain;
            this.btnSearchTop.TabIndex = 4;
            this.btnSearchTop.Text = "Tìm (Ctrl F)";
            this.btnSearchTop.Click += new System.EventHandler(this.btnSearchTop_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(830, 26);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(68, 22);
            this.btnPrint.StyleController = this.layoutControlMain;
            this.btnPrint.TabIndex = 5;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(1184, 26);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(64, 22);
            this.btnSave.StyleController = this.layoutControlMain;
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(902, 26);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(76, 22);
            this.btnCopy.StyleController = this.layoutControlMain;
            this.btnCopy.TabIndex = 7;
            this.btnCopy.Text = "Sao chép";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // dtCopyFromDate
            // 
            this.dtCopyFromDate.EditValue = null;
            this.dtCopyFromDate.Location = new System.Drawing.Point(1047, 26);
            this.dtCopyFromDate.Name = "dtCopyFromDate";
            this.dtCopyFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCopyFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCopyFromDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtCopyFromDate.Size = new System.Drawing.Size(113, 20);
            this.dtCopyFromDate.StyleController = this.layoutControlMain;
            this.dtCopyFromDate.TabIndex = 8;
            // 
            // gridControlSchedule
            // 
            this.gridControlSchedule.Location = new System.Drawing.Point(2, 52);
            this.gridControlSchedule.MainView = this.gridViewSchedule;
            this.gridControlSchedule.Name = "gridControlSchedule";
            this.gridControlSchedule.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoDelete,
            this.repoTemplate,
            this.repoShift});
            this.gridControlSchedule.Size = new System.Drawing.Size(1246, 235);
            this.gridControlSchedule.TabIndex = 9;
            this.gridControlSchedule.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSchedule,
            this.repoTemplateView});
            // 
            // gridViewSchedule
            // 
            this.gridViewSchedule.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colDelete,
            this.colSTT,
            this.colScheduleDate,
            this.colShift,
            this.colPatientName,
            this.colPatientCode,
            this.colTreatmentCode,
            this.colDob,
            this.colGender,
            this.colInTime,
            this.colPatientType,
            this.colTemplate,
            this.colNote,
            this.colCreateTime,
            this.colCreator,
            this.colModifyTime,
            this.colModifier});
            this.gridViewSchedule.GridControl = this.gridControlSchedule;
            this.gridViewSchedule.Name = "gridViewSchedule";
            this.gridViewSchedule.OptionsView.ColumnAutoWidth = false;
            this.gridViewSchedule.OptionsView.ShowGroupPanel = false;
            this.gridViewSchedule.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewSchedule_CellValueChanged);
            this.gridViewSchedule.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSchedule_CustomUnboundColumnData);
            this.gridViewSchedule.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewSchedule_CustomColumnDisplayText);
            // 
            // colDelete
            // 
            this.colDelete.Caption = " ";
            this.colDelete.ColumnEdit = this.repoDelete;
            this.colDelete.FieldName = "DELETE";
            this.colDelete.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colDelete.Name = "colDelete";
            this.colDelete.OptionsColumn.ShowCaption = false;
            this.colDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colDelete.Visible = true;
            this.colDelete.VisibleIndex = 0;
            this.colDelete.Width = 30;
            // 
            // repoDelete
            // 
            this.repoDelete.AutoHeight = false;
            this.repoDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repoDelete.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.repoDelete.Name = "repoDelete";
            this.repoDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoDelete_ButtonClick);
            // 
            // colSTT
            // 
            this.colSTT.Caption = "STT";
            this.colSTT.FieldName = "STT";
            this.colSTT.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colSTT.Name = "colSTT";
            this.colSTT.OptionsColumn.AllowEdit = false;
            this.colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colSTT.Visible = true;
            this.colSTT.VisibleIndex = 1;
            this.colSTT.Width = 40;
            // 
            // colScheduleDate
            // 
            this.colScheduleDate.Caption = "Ngày";
            this.colScheduleDate.FieldName = "SCHEDULE_DATE";
            this.colScheduleDate.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colScheduleDate.Name = "colScheduleDate";
            this.colScheduleDate.OptionsColumn.AllowEdit = false;
            this.colScheduleDate.Visible = true;
            this.colScheduleDate.VisibleIndex = 2;
            this.colScheduleDate.Width = 80;
            // 
            // colShift
            // 
            this.colShift.Caption = "Ca";
            this.colShift.ColumnEdit = this.repoShift;
            this.colShift.FieldName = "KIDNEY_SHIFT";
            this.colShift.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colShift.Name = "colShift";
            this.colShift.Visible = true;
            this.colShift.VisibleIndex = 3;
            this.colShift.Width = 50;
            // 
            // colPatientName
            // 
            this.colPatientName.Caption = "Tên bệnh nhân";
            this.colPatientName.FieldName = "TDL_PATIENT_NAME";
            this.colPatientName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colPatientName.Name = "colPatientName";
            this.colPatientName.OptionsColumn.AllowEdit = false;
            this.colPatientName.Visible = true;
            this.colPatientName.VisibleIndex = 4;
            this.colPatientName.Width = 200;
            // 
            // colPatientCode
            // 
            this.colPatientCode.Caption = "Mã bệnh nhân";
            this.colPatientCode.FieldName = "TDL_PATIENT_CODE";
            this.colPatientCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colPatientCode.Name = "colPatientCode";
            this.colPatientCode.OptionsColumn.AllowEdit = false;
            this.colPatientCode.Visible = true;
            this.colPatientCode.VisibleIndex = 5;
            this.colPatientCode.Width = 90;
            // 
            // colTreatmentCode
            // 
            this.colTreatmentCode.Caption = "Mã điều trị";
            this.colTreatmentCode.FieldName = "TREATMENT_CODE";
            this.colTreatmentCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colTreatmentCode.Name = "colTreatmentCode";
            this.colTreatmentCode.OptionsColumn.AllowEdit = false;
            this.colTreatmentCode.Visible = true;
            this.colTreatmentCode.VisibleIndex = 6;
            this.colTreatmentCode.Width = 100;
            // 
            // colDob
            // 
            this.colDob.Caption = "Ngày sinh";
            this.colDob.FieldName = "DOB_DISPLAY";
            this.colDob.Name = "colDob";
            this.colDob.OptionsColumn.AllowEdit = false;
            this.colDob.Visible = true;
            this.colDob.VisibleIndex = 7;
            this.colDob.Width = 95;
            // 
            // colGender
            // 
            this.colGender.Caption = "Giới tính";
            this.colGender.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.colGender.Name = "colGender";
            this.colGender.OptionsColumn.AllowEdit = false;
            this.colGender.Visible = true;
            this.colGender.VisibleIndex = 8;
            this.colGender.Width = 60;
            // 
            // colInTime
            // 
            this.colInTime.Caption = "Ngày vào";
            this.colInTime.FieldName = "IN_TIME";
            this.colInTime.Name = "colInTime";
            this.colInTime.OptionsColumn.AllowEdit = false;
            this.colInTime.Visible = true;
            this.colInTime.VisibleIndex = 9;
            this.colInTime.Width = 140;
            // 
            // colPatientType
            // 
            this.colPatientType.Caption = "Đối tượng";
            this.colPatientType.FieldName = "TDL_PATIENT_TYPE_NAME";
            this.colPatientType.Name = "colPatientType";
            this.colPatientType.OptionsColumn.AllowEdit = false;
            this.colPatientType.Visible = true;
            this.colPatientType.VisibleIndex = 10;
            this.colPatientType.Width = 120;
            // 
            // colTemplate
            // 
            this.colTemplate.Caption = "Gói vật tư";
            this.colTemplate.ColumnEdit = this.repoTemplate;
            this.colTemplate.FieldName = "EXP_MEST_TEMPLATE_ID";
            this.colTemplate.Name = "colTemplate";
            this.colTemplate.Visible = true;
            this.colTemplate.VisibleIndex = 11;
            this.colTemplate.Width = 220;
            // 
            // repoTemplate
            // 
            this.repoTemplate.AutoHeight = false;
            this.repoTemplate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoTemplate.Name = "repoTemplate";
            this.repoTemplate.NullText = "";
            this.repoTemplate.View = this.repoTemplateView;
            // 
            // repoTemplateView
            // 
            this.repoTemplateView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repoTemplateView.GridControl = this.gridControlSchedule;
            this.repoTemplateView.Name = "repoTemplateView";
            this.repoTemplateView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repoTemplateView.OptionsView.ShowGroupPanel = false;
            //
            // repoShift
            //
            this.repoShift.AutoHeight = false;
            this.repoShift.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoShift.Name = "repoShift";
            this.repoShift.NullText = "";
            this.repoShift.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //
            // colNote
            // 
            this.colNote.Caption = "Ghi chú";
            this.colNote.FieldName = "NOTE";
            this.colNote.Name = "colNote";
            this.colNote.Visible = true;
            this.colNote.VisibleIndex = 12;
            this.colNote.Width = 240;
            // 
            // colCreateTime
            // 
            this.colCreateTime.Caption = "Ngày tạo";
            this.colCreateTime.FieldName = "CREATE_TIME";
            this.colCreateTime.Name = "colCreateTime";
            this.colCreateTime.OptionsColumn.AllowEdit = false;
            this.colCreateTime.Visible = true;
            this.colCreateTime.VisibleIndex = 13;
            this.colCreateTime.Width = 120;
            // 
            // colCreator
            // 
            this.colCreator.Caption = "Người tạo";
            this.colCreator.FieldName = "CREATOR";
            this.colCreator.Name = "colCreator";
            this.colCreator.OptionsColumn.AllowEdit = false;
            this.colCreator.Visible = true;
            this.colCreator.VisibleIndex = 14;
            this.colCreator.Width = 120;
            // 
            // colModifyTime
            // 
            this.colModifyTime.Caption = "Ngày sửa";
            this.colModifyTime.FieldName = "MODIFY_TIME";
            this.colModifyTime.Name = "colModifyTime";
            this.colModifyTime.OptionsColumn.AllowEdit = false;
            this.colModifyTime.Visible = true;
            this.colModifyTime.VisibleIndex = 15;
            this.colModifyTime.Width = 120;
            // 
            // colModifier
            // 
            this.colModifier.Caption = "Người sửa";
            this.colModifier.FieldName = "MODIFIER";
            this.colModifier.Name = "colModifier";
            this.colModifier.OptionsColumn.AllowEdit = false;
            this.colModifier.Visible = true;
            this.colModifier.VisibleIndex = 16;
            this.colModifier.Width = 120;
            // 
            // cboDepartment
            // 
            this.cboDepartment.Location = new System.Drawing.Point(154, 291);
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Properties.View = this.gridViewDept;
            this.cboDepartment.Size = new System.Drawing.Size(205, 20);
            this.cboDepartment.StyleController = this.layoutControlMain;
            this.cboDepartment.TabIndex = 10;
            this.cboDepartment.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboClearButton_ButtonClick);
            this.cboDepartment.EditValueChanged += new System.EventHandler(this.cboDepartment_EditValueChanged);
            // 
            // gridViewDept
            // 
            this.gridViewDept.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewDept.Name = "gridViewDept";
            this.gridViewDept.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewDept.OptionsView.ShowGroupPanel = false;
            // 
            // chkAllDepartment
            // 
            this.chkAllDepartment.Location = new System.Drawing.Point(363, 291);
            this.chkAllDepartment.Name = "chkAllDepartment";
            this.chkAllDepartment.Properties.Caption = "Toàn khoa";
            this.chkAllDepartment.Size = new System.Drawing.Size(72, 19);
            this.chkAllDepartment.StyleController = this.layoutControlMain;
            this.chkAllDepartment.TabIndex = 11;
            this.chkAllDepartment.CheckedChanged += new System.EventHandler(this.chkAllDepartment_CheckedChanged);
            // 
            // dtInTimeFrom
            // 
            this.dtInTimeFrom.EditValue = null;
            this.dtInTimeFrom.Location = new System.Drawing.Point(516, 291);
            this.dtInTimeFrom.Name = "dtInTimeFrom";
            this.dtInTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInTimeFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtInTimeFrom.Size = new System.Drawing.Size(93, 20);
            this.dtInTimeFrom.StyleController = this.layoutControlMain;
            this.dtInTimeFrom.TabIndex = 12;
            // 
            // dtInTimeTo
            // 
            this.dtInTimeTo.EditValue = null;
            this.dtInTimeTo.Location = new System.Drawing.Point(653, 291);
            this.dtInTimeTo.Name = "dtInTimeTo";
            this.dtInTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInTimeTo.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtInTimeTo.Size = new System.Drawing.Size(104, 20);
            this.dtInTimeTo.StyleController = this.layoutControlMain;
            this.dtInTimeTo.TabIndex = 13;
            // 
            // txtSearchBottom
            // 
            this.txtSearchBottom.Location = new System.Drawing.Point(761, 291);
            this.txtSearchBottom.Name = "txtSearchBottom";
            this.txtSearchBottom.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearchBottom.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearchBottom.Size = new System.Drawing.Size(285, 20);
            this.txtSearchBottom.StyleController = this.layoutControlMain;
            this.txtSearchBottom.TabIndex = 14;
            this.txtSearchBottom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearchBottom_KeyUp);
            // 
            // btnSearchBottom
            // 
            this.btnSearchBottom.Location = new System.Drawing.Point(1050, 291);
            this.btnSearchBottom.Name = "btnSearchBottom";
            this.btnSearchBottom.Size = new System.Drawing.Size(87, 22);
            this.btnSearchBottom.StyleController = this.layoutControlMain;
            this.btnSearchBottom.TabIndex = 15;
            this.btnSearchBottom.Text = "Tìm (Ctrl Shift F)";
            this.btnSearchBottom.Click += new System.EventHandler(this.btnSearchBottom_Click);
            // 
            // btnAddToSchedule
            // 
            this.btnAddToSchedule.Location = new System.Drawing.Point(1141, 291);
            this.btnAddToSchedule.Name = "btnAddToSchedule";
            this.btnAddToSchedule.Size = new System.Drawing.Size(107, 22);
            this.btnAddToSchedule.StyleController = this.layoutControlMain;
            this.btnAddToSchedule.TabIndex = 17;
            this.btnAddToSchedule.Text = "Đưa vào lịch (Ctrl A)";
            this.btnAddToSchedule.Click += new System.EventHandler(this.btnAddToSchedule_Click);
            // 
            // gridControlTreatment
            // 
            this.gridControlTreatment.Location = new System.Drawing.Point(2, 341);
            this.gridControlTreatment.MainView = this.gridViewTreatment;
            this.gridControlTreatment.Name = "gridControlTreatment";
            this.gridControlTreatment.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoCheck,
            this.repoDeleteB,
            this.repoEmptyB});
            this.gridControlTreatment.Size = new System.Drawing.Size(1246, 357);
            this.gridControlTreatment.TabIndex = 18;
            this.gridControlTreatment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTreatment});
            // 
            // gridViewTreatment
            // 
            this.gridViewTreatment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSelect,
            this.colDeleteB,
            this.colSTTb,
            this.colPatientNameB,
            this.colPatientCodeB,
            this.colTreatmentCodeB,
            this.colDobB,
            this.colGenderB,
            this.colInTimeB,
            this.colTreatmentTypeB,
            this.colHeinCardB,
            this.colIcdNameB});
            this.gridViewTreatment.GridControl = this.gridControlTreatment;
            this.gridViewTreatment.Name = "gridViewTreatment";
            this.gridViewTreatment.OptionsView.ShowGroupPanel = false;
            this.gridViewTreatment.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewTreatment_CustomRowCellEdit);
            this.gridViewTreatment.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewTreatment_CustomDrawColumnHeader);
            this.gridViewTreatment.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewTreatment_CustomUnboundColumnData);
            this.gridViewTreatment.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewTreatment_CustomColumnDisplayText);
            this.gridViewTreatment.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewTreatment_MouseDown);
            // 
            // colSelect
            // 
            this.colSelect.Caption = " ";
            this.colSelect.ColumnEdit = this.repoCheck;
            this.colSelect.FieldName = "IsSelected";
            this.colSelect.Name = "colSelect";
            this.colSelect.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colSelect.OptionsColumn.ShowCaption = false;
            this.colSelect.Visible = true;
            this.colSelect.VisibleIndex = 0;
            this.colSelect.Width = 30;
            // 
            // repoCheck
            // 
            this.repoCheck.AutoHeight = false;
            this.repoCheck.Name = "repoCheck";
            //
            // colDeleteB
            //
            this.colDeleteB.Caption = " ";
            this.colDeleteB.ColumnEdit = this.repoDeleteB;
            this.colDeleteB.FieldName = "DELETE_SCHEDULE";
            this.colDeleteB.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colDeleteB.Name = "colDeleteB";
            this.colDeleteB.OptionsColumn.ShowCaption = false;
            this.colDeleteB.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colDeleteB.Visible = true;
            this.colDeleteB.VisibleIndex = 1;
            this.colDeleteB.Width = 30;
            //
            // repoDeleteB
            //
            this.repoDeleteB.AutoHeight = false;
            this.repoDeleteB.Buttons.Clear();
            DevExpress.XtraEditors.Controls.EditorButton btnDeleteB = new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph);
            btnDeleteB.Image = ((System.Drawing.Image)(resources.GetObject("repoDelete.Buttons")));
            btnDeleteB.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.repoDeleteB.Buttons.Add(btnDeleteB);
            this.repoDeleteB.Name = "repoDeleteB";
            this.repoDeleteB.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoDeleteB.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoDeleteB_ButtonClick);
            //
            // repoEmptyB
            //
            this.repoEmptyB.AutoHeight = false;
            this.repoEmptyB.Buttons.Clear();
            this.repoEmptyB.Name = "repoEmptyB";
            this.repoEmptyB.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // colSTTb
            // 
            this.colSTTb.Caption = "STT";
            this.colSTTb.FieldName = "STT";
            this.colSTTb.Name = "colSTTb";
            this.colSTTb.OptionsColumn.AllowEdit = false;
            this.colSTTb.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colSTTb.Visible = true;
            this.colSTTb.VisibleIndex = 2;
            this.colSTTb.Width = 40;
            // 
            // colPatientNameB
            // 
            this.colPatientNameB.Caption = "Tên bệnh nhân";
            this.colPatientNameB.FieldName = "TDL_PATIENT_NAME";
            this.colPatientNameB.Name = "colPatientNameB";
            this.colPatientNameB.OptionsColumn.AllowEdit = false;
            this.colPatientNameB.Visible = true;
            this.colPatientNameB.VisibleIndex = 3;
            this.colPatientNameB.Width = 200;
            // 
            // colPatientCodeB
            // 
            this.colPatientCodeB.Caption = "Mã bệnh nhân";
            this.colPatientCodeB.FieldName = "TDL_PATIENT_CODE";
            this.colPatientCodeB.Name = "colPatientCodeB";
            this.colPatientCodeB.OptionsColumn.AllowEdit = false;
            this.colPatientCodeB.Visible = true;
            this.colPatientCodeB.VisibleIndex = 4;
            this.colPatientCodeB.Width = 90;
            // 
            // colTreatmentCodeB
            // 
            this.colTreatmentCodeB.Caption = "Mã điều trị";
            this.colTreatmentCodeB.FieldName = "TREATMENT_CODE";
            this.colTreatmentCodeB.Name = "colTreatmentCodeB";
            this.colTreatmentCodeB.OptionsColumn.AllowEdit = false;
            this.colTreatmentCodeB.Visible = true;
            this.colTreatmentCodeB.VisibleIndex = 5;
            this.colTreatmentCodeB.Width = 100;
            // 
            // colDobB
            // 
            this.colDobB.Caption = "Ngày sinh";
            this.colDobB.FieldName = "DOB_DISPLAY";
            this.colDobB.Name = "colDobB";
            this.colDobB.OptionsColumn.AllowEdit = false;
            this.colDobB.Visible = true;
            this.colDobB.VisibleIndex = 6;
            this.colDobB.Width = 95;
            // 
            // colGenderB
            // 
            this.colGenderB.Caption = "Giới tính";
            this.colGenderB.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.colGenderB.Name = "colGenderB";
            this.colGenderB.OptionsColumn.AllowEdit = false;
            this.colGenderB.Visible = true;
            this.colGenderB.VisibleIndex = 7;
            this.colGenderB.Width = 60;
            // 
            // colInTimeB
            // 
            this.colInTimeB.Caption = "Ngày vào";
            this.colInTimeB.FieldName = "IN_TIME";
            this.colInTimeB.Name = "colInTimeB";
            this.colInTimeB.OptionsColumn.AllowEdit = false;
            this.colInTimeB.Visible = true;
            this.colInTimeB.VisibleIndex = 8;
            this.colInTimeB.Width = 140;
            // 
            // colTreatmentTypeB
            // 
            this.colTreatmentTypeB.Caption = "Diện điều trị";
            this.colTreatmentTypeB.FieldName = "TREATMENT_TYPE_NAME";
            this.colTreatmentTypeB.Name = "colTreatmentTypeB";
            this.colTreatmentTypeB.OptionsColumn.AllowEdit = false;
            this.colTreatmentTypeB.Visible = true;
            this.colTreatmentTypeB.VisibleIndex = 9;
            this.colTreatmentTypeB.Width = 110;
            // 
            // colHeinCardB
            // 
            this.colHeinCardB.Caption = "Số thẻ BHYT";
            this.colHeinCardB.FieldName = "TDL_HEIN_CARD_NUMBER";
            this.colHeinCardB.Name = "colHeinCardB";
            this.colHeinCardB.OptionsColumn.AllowEdit = false;
            this.colHeinCardB.Visible = true;
            this.colHeinCardB.VisibleIndex = 10;
            this.colHeinCardB.Width = 130;
            // 
            // colIcdNameB
            // 
            this.colIcdNameB.Caption = "Chẩn đoán chính";
            this.colIcdNameB.FieldName = "ICD_NAME";
            this.colIcdNameB.Name = "colIcdNameB";
            this.colIcdNameB.OptionsColumn.AllowEdit = false;
            this.colIcdNameB.Visible = true;
            this.colIcdNameB.VisibleIndex = 11;
            this.colIcdNameB.Width = 220;
            //
            // ucPaging
            //
            this.ucPaging.Location = new System.Drawing.Point(2, 652);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(1246, 46);
            this.ucPaging.TabIndex = 27;
            //
            // lcgRoot
            // 
            this.lcgRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.lcgRoot.GroupBordersVisible = false;
            this.lcgRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciDate,
            this.lciShift,
            this.lciSearchTop,
            this.lciBtnSearchTop,
            this.lciBtnCopy,
            this.lciCopyFrom,
            this.emptyTop,
            this.lciGridSchedule,
            this.lciDept,
            this.lciAllDept,
            this.lciFrom,
            this.lciTo,
            this.lciSearchBottom,
            this.lciBtnSearchBottom,
            this.lciBtnAdd,
            this.lciGridTreatment,
            this.lciPaging,
            this.lciRoom,
            this.lciRoomCode,
            this.lciDepartmentCode,
            this.lciBtnPrevDate,
            this.lciBtnNextDate,
            this.lciBtnPrevShift,
            this.lciBtnNextShift,
            this.emptySpaceItem1,
            this.lciBtnSave,
            this.lciBtnPrint,
            this.lciTemplate,
            this.lciNote,
            this.emptySpaceItem2});
            this.lcgRoot.Location = new System.Drawing.Point(0, 0);
            this.lcgRoot.Name = "lcgRoot";
            this.lcgRoot.Size = new System.Drawing.Size(1250, 700);
            this.lcgRoot.TextVisible = false;
            // 
            // lciDate
            // 
            this.lciDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDate.Control = this.dtDate;
            this.lciDate.Location = new System.Drawing.Point(0, 24);
            this.lciDate.Name = "lciDate";
            this.lciDate.Size = new System.Drawing.Size(202, 26);
            this.lciDate.Text = "Ngày:";
            this.lciDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDate.TextSize = new System.Drawing.Size(70, 20);
            this.lciDate.TextToControlDistance = 5;
            // 
            // lciShift
            // 
            this.lciShift.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciShift.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciShift.Control = this.cboShift;
            this.lciShift.Location = new System.Drawing.Point(258, 24);
            this.lciShift.Name = "lciShift";
            this.lciShift.Size = new System.Drawing.Size(99, 26);
            this.lciShift.Text = "Ca:";
            this.lciShift.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciShift.TextSize = new System.Drawing.Size(40, 20);
            this.lciShift.TextToControlDistance = 5;
            // 
            // lciSearchTop
            // 
            this.lciSearchTop.Control = this.txtSearchTop;
            this.lciSearchTop.Location = new System.Drawing.Point(413, 24);
            this.lciSearchTop.Name = "lciSearchTop";
            this.lciSearchTop.Size = new System.Drawing.Size(313, 26);
            this.lciSearchTop.TextSize = new System.Drawing.Size(0, 0);
            this.lciSearchTop.TextVisible = false;
            // 
            // lciBtnSearchTop
            // 
            this.lciBtnSearchTop.Control = this.btnSearchTop;
            this.lciBtnSearchTop.Location = new System.Drawing.Point(726, 24);
            this.lciBtnSearchTop.Name = "lciBtnSearchTop";
            this.lciBtnSearchTop.Size = new System.Drawing.Size(102, 26);
            this.lciBtnSearchTop.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSearchTop.TextVisible = false;
            // 
            // lciBtnCopy
            // 
            this.lciBtnCopy.Control = this.btnCopy;
            this.lciBtnCopy.Location = new System.Drawing.Point(900, 24);
            this.lciBtnCopy.Name = "lciBtnCopy";
            this.lciBtnCopy.Size = new System.Drawing.Size(80, 26);
            this.lciBtnCopy.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnCopy.TextVisible = false;
            // 
            // lciCopyFrom
            // 
            this.lciCopyFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCopyFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCopyFrom.Control = this.dtCopyFromDate;
            this.lciCopyFrom.Location = new System.Drawing.Point(980, 24);
            this.lciCopyFrom.Name = "lciCopyFrom";
            this.lciCopyFrom.Size = new System.Drawing.Size(182, 26);
            this.lciCopyFrom.Text = "Ngày:";
            this.lciCopyFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCopyFrom.TextSize = new System.Drawing.Size(60, 20);
            this.lciCopyFrom.TextToControlDistance = 5;
            // 
            // emptyTop
            // 
            this.emptyTop.AllowHotTrack = false;
            this.emptyTop.Location = new System.Drawing.Point(1162, 24);
            this.emptyTop.Name = "emptyTop";
            this.emptyTop.Size = new System.Drawing.Size(20, 26);
            this.emptyTop.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciGridSchedule
            // 
            this.lciGridSchedule.Control = this.gridControlSchedule;
            this.lciGridSchedule.Location = new System.Drawing.Point(0, 50);
            this.lciGridSchedule.Name = "lciGridSchedule";
            this.lciGridSchedule.Size = new System.Drawing.Size(1250, 239);
            this.lciGridSchedule.TextSize = new System.Drawing.Size(0, 0);
            this.lciGridSchedule.TextVisible = false;
            // 
            // lciDept
            // 
            this.lciDept.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDept.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDept.Control = this.cboDepartment;
            this.lciDept.Location = new System.Drawing.Point(154, 289);
            this.lciDept.Name = "lciDept";
            this.lciDept.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciDept.Size = new System.Drawing.Size(207, 26);
            this.lciDept.Text = "Khoa:";
            this.lciDept.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDept.TextSize = new System.Drawing.Size(0, 0);
            this.lciDept.TextToControlDistance = 0;
            this.lciDept.TextVisible = false;
            // 
            // lciAllDept
            // 
            this.lciAllDept.Control = this.chkAllDepartment;
            this.lciAllDept.Location = new System.Drawing.Point(361, 289);
            this.lciAllDept.Name = "lciAllDept";
            this.lciAllDept.Size = new System.Drawing.Size(76, 26);
            this.lciAllDept.TextSize = new System.Drawing.Size(0, 0);
            this.lciAllDept.TextVisible = false;
            // 
            // lciFrom
            // 
            this.lciFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFrom.Control = this.dtInTimeFrom;
            this.lciFrom.Location = new System.Drawing.Point(437, 289);
            this.lciFrom.Name = "lciFrom";
            this.lciFrom.Size = new System.Drawing.Size(174, 26);
            this.lciFrom.Text = "Ngày vào từ:";
            this.lciFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFrom.TextSize = new System.Drawing.Size(72, 20);
            this.lciFrom.TextToControlDistance = 5;
            // 
            // lciTo
            // 
            this.lciTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTo.Control = this.dtInTimeTo;
            this.lciTo.Location = new System.Drawing.Point(611, 289);
            this.lciTo.Name = "lciTo";
            this.lciTo.Size = new System.Drawing.Size(148, 26);
            this.lciTo.Text = "Đến:";
            this.lciTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTo.TextSize = new System.Drawing.Size(35, 20);
            this.lciTo.TextToControlDistance = 5;
            // 
            // lciSearchBottom
            // 
            this.lciSearchBottom.Control = this.txtSearchBottom;
            this.lciSearchBottom.Location = new System.Drawing.Point(759, 289);
            this.lciSearchBottom.Name = "lciSearchBottom";
            this.lciSearchBottom.Size = new System.Drawing.Size(289, 26);
            this.lciSearchBottom.TextSize = new System.Drawing.Size(0, 0);
            this.lciSearchBottom.TextVisible = false;
            // 
            // lciBtnSearchBottom
            // 
            this.lciBtnSearchBottom.Control = this.btnSearchBottom;
            this.lciBtnSearchBottom.Location = new System.Drawing.Point(1048, 289);
            this.lciBtnSearchBottom.Name = "lciBtnSearchBottom";
            this.lciBtnSearchBottom.Size = new System.Drawing.Size(91, 26);
            this.lciBtnSearchBottom.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSearchBottom.TextVisible = false;
            // 
            // lciBtnAdd
            // 
            this.lciBtnAdd.Control = this.btnAddToSchedule;
            this.lciBtnAdd.Location = new System.Drawing.Point(1139, 289);
            this.lciBtnAdd.Name = "lciBtnAdd";
            this.lciBtnAdd.Size = new System.Drawing.Size(111, 26);
            this.lciBtnAdd.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnAdd.TextVisible = false;
            // 
            // lciGridTreatment
            // 
            this.lciGridTreatment.Control = this.gridControlTreatment;
            this.lciGridTreatment.Location = new System.Drawing.Point(0, 339);
            this.lciGridTreatment.Name = "lciGridTreatment";
            this.lciGridTreatment.Size = new System.Drawing.Size(1250, 311);
            this.lciGridTreatment.TextSize = new System.Drawing.Size(0, 0);
            this.lciGridTreatment.TextVisible = false;
            //
            // lciPaging
            //
            this.lciPaging.Control = this.ucPaging;
            this.lciPaging.Location = new System.Drawing.Point(0, 650);
            this.lciPaging.MaxSize = new System.Drawing.Size(0, 50);
            this.lciPaging.MinSize = new System.Drawing.Size(1, 50);
            this.lciPaging.Name = "lciPaging";
            this.lciPaging.Size = new System.Drawing.Size(1250, 50);
            this.lciPaging.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPaging.TextSize = new System.Drawing.Size(0, 0);
            this.lciPaging.TextVisible = false;
            // 
            // lciRoom
            // 
            this.lciRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciRoom.Control = this.cboRoom;
            this.lciRoom.Location = new System.Drawing.Point(154, 0);
            this.lciRoom.Name = "lciRoom";
            this.lciRoom.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciRoom.Size = new System.Drawing.Size(347, 24);
            this.lciRoom.Text = "Phòng chạy:";
            this.lciRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciRoom.TextSize = new System.Drawing.Size(0, 0);
            this.lciRoom.TextToControlDistance = 0;
            this.lciRoom.TextVisible = false;
            // 
            // lciRoomCode
            // 
            this.lciRoomCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciRoomCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciRoomCode.Control = this.txtRoomCode;
            this.lciRoomCode.Location = new System.Drawing.Point(0, 0);
            this.lciRoomCode.Name = "lciRoomCode";
            this.lciRoomCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciRoomCode.Size = new System.Drawing.Size(154, 24);
            this.lciRoomCode.Text = "Phòng chạy:";
            this.lciRoomCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciRoomCode.TextSize = new System.Drawing.Size(70, 13);
            this.lciRoomCode.TextToControlDistance = 5;
            // 
            // lciDepartmentCode
            // 
            this.lciDepartmentCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDepartmentCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDepartmentCode.Control = this.txtDepartmentCode;
            this.lciDepartmentCode.Location = new System.Drawing.Point(0, 289);
            this.lciDepartmentCode.Name = "lciDepartmentCode";
            this.lciDepartmentCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciDepartmentCode.Size = new System.Drawing.Size(154, 26);
            this.lciDepartmentCode.Text = "Khoa:";
            this.lciDepartmentCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDepartmentCode.TextSize = new System.Drawing.Size(70, 13);
            this.lciDepartmentCode.TextToControlDistance = 5;
            // 
            // lciBtnPrevDate
            // 
            this.lciBtnPrevDate.Control = this.btnPrevDate;
            this.lciBtnPrevDate.Location = new System.Drawing.Point(202, 24);
            this.lciBtnPrevDate.MaxSize = new System.Drawing.Size(28, 26);
            this.lciBtnPrevDate.MinSize = new System.Drawing.Size(28, 26);
            this.lciBtnPrevDate.Name = "lciBtnPrevDate";
            this.lciBtnPrevDate.Size = new System.Drawing.Size(28, 26);
            this.lciBtnPrevDate.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnPrevDate.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnPrevDate.TextVisible = false;
            // 
            // lciBtnNextDate
            // 
            this.lciBtnNextDate.Control = this.btnNextDate;
            this.lciBtnNextDate.Location = new System.Drawing.Point(230, 24);
            this.lciBtnNextDate.MaxSize = new System.Drawing.Size(28, 26);
            this.lciBtnNextDate.MinSize = new System.Drawing.Size(28, 26);
            this.lciBtnNextDate.Name = "lciBtnNextDate";
            this.lciBtnNextDate.Size = new System.Drawing.Size(28, 26);
            this.lciBtnNextDate.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnNextDate.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnNextDate.TextVisible = false;
            // 
            // lciBtnPrevShift
            // 
            this.lciBtnPrevShift.Control = this.btnPrevShift;
            this.lciBtnPrevShift.Location = new System.Drawing.Point(357, 24);
            this.lciBtnPrevShift.MaxSize = new System.Drawing.Size(28, 26);
            this.lciBtnPrevShift.MinSize = new System.Drawing.Size(28, 26);
            this.lciBtnPrevShift.Name = "lciBtnPrevShift";
            this.lciBtnPrevShift.Size = new System.Drawing.Size(28, 26);
            this.lciBtnPrevShift.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnPrevShift.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnPrevShift.TextVisible = false;
            // 
            // lciBtnNextShift
            // 
            this.lciBtnNextShift.Control = this.btnNextShift;
            this.lciBtnNextShift.Location = new System.Drawing.Point(385, 24);
            this.lciBtnNextShift.MaxSize = new System.Drawing.Size(28, 26);
            this.lciBtnNextShift.MinSize = new System.Drawing.Size(28, 26);
            this.lciBtnNextShift.Name = "lciBtnNextShift";
            this.lciBtnNextShift.Size = new System.Drawing.Size(28, 26);
            this.lciBtnNextShift.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnNextShift.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnNextShift.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(501, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(749, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciBtnSave
            // 
            this.lciBtnSave.Control = this.btnSave;
            this.lciBtnSave.Location = new System.Drawing.Point(1182, 24);
            this.lciBtnSave.Name = "lciBtnSave";
            this.lciBtnSave.Size = new System.Drawing.Size(68, 26);
            this.lciBtnSave.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSave.TextVisible = false;
            this.lciBtnSave.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciBtnPrint
            // 
            this.lciBtnPrint.Control = this.btnPrint;
            this.lciBtnPrint.Location = new System.Drawing.Point(828, 24);
            this.lciBtnPrint.Name = "lciBtnPrint";
            this.lciBtnPrint.Size = new System.Drawing.Size(72, 26);
            this.lciBtnPrint.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnPrint.TextVisible = false;
            this.lciBtnPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciTemplate
            // 
            this.lciTemplate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTemplate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTemplate.Control = this.cboTemplate;
            this.lciTemplate.Location = new System.Drawing.Point(0, 315);
            this.lciTemplate.Name = "lciTemplate";
            this.lciTemplate.Size = new System.Drawing.Size(611, 24);
            this.lciTemplate.Text = "Gói vật tư:";
            this.lciTemplate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTemplate.TextSize = new System.Drawing.Size(70, 20);
            this.lciTemplate.TextToControlDistance = 5;
            // 
            // lciNote
            // 
            this.lciNote.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNote.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNote.Control = this.txtNote;
            this.lciNote.Location = new System.Drawing.Point(611, 315);
            this.lciNote.Name = "lciNote";
            this.lciNote.Size = new System.Drawing.Size(437, 24);
            this.lciNote.Text = "Ghi chú:";
            this.lciNote.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNote.TextSize = new System.Drawing.Size(80, 20);
            this.lciNote.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(1048, 315);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(202, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // ucHemodialysisSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControlMain);
            this.Name = "ucHemodialysisSchedule";
            this.Size = new System.Drawing.Size(1250, 700);
            this.Load += new System.EventHandler(this.ucHemodialysisSchedule_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).EndInit();
            this.layoutControlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtNote.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTemplate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRoomCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboShift.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchTop.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCopyFromDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCopyFromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTemplateView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchBottom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDeleteB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEmptyB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgRoot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearchTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSearchTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCopyFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptyTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAllDept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearchBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSearchBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridTreatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaging)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoomCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnPrevDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnNextDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnPrevShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnNextShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnPrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControlMain;
        private DevExpress.XtraLayout.LayoutControlGroup lcgRoot;

        private DevExpress.XtraEditors.GridLookUpEdit cboRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewRoom;
        private DevExpress.XtraEditors.DateEdit dtDate;
        private DevExpress.XtraEditors.GridLookUpEdit cboShift;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewShift;
        private DevExpress.XtraEditors.TextEdit txtSearchTop;
        private DevExpress.XtraEditors.SimpleButton btnSearchTop;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCopy;
        private DevExpress.XtraEditors.DateEdit dtCopyFromDate;

        private DevExpress.XtraGrid.GridControl gridControlSchedule;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSchedule;
        private DevExpress.XtraGrid.Columns.GridColumn colDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoDelete;
        private DevExpress.XtraGrid.Columns.GridColumn colSTT;
        private DevExpress.XtraGrid.Columns.GridColumn colScheduleDate;
        private DevExpress.XtraGrid.Columns.GridColumn colShift;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn colTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn colDob;
        private DevExpress.XtraGrid.Columns.GridColumn colGender;
        private DevExpress.XtraGrid.Columns.GridColumn colInTime;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientType;
        private DevExpress.XtraGrid.Columns.GridColumn colTemplate;
        private DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repoTemplate;
        private DevExpress.XtraGrid.Views.Grid.GridView repoTemplateView;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repoShift;
        private DevExpress.XtraGrid.Columns.GridColumn colNote;
        private DevExpress.XtraGrid.Columns.GridColumn colCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn colCreator;
        private DevExpress.XtraGrid.Columns.GridColumn colModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn colModifier;

        private DevExpress.XtraEditors.GridLookUpEdit cboDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDept;
        private DevExpress.XtraEditors.CheckEdit chkAllDepartment;
        private DevExpress.XtraEditors.DateEdit dtInTimeFrom;
        private DevExpress.XtraEditors.DateEdit dtInTimeTo;
        private DevExpress.XtraEditors.TextEdit txtSearchBottom;
        private DevExpress.XtraEditors.SimpleButton btnSearchBottom;
        private DevExpress.XtraEditors.SimpleButton btnAddToSchedule;

        private DevExpress.XtraGrid.GridControl gridControlTreatment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTreatment;
        private DevExpress.XtraGrid.Columns.GridColumn colSelect;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repoCheck;
        private DevExpress.XtraGrid.Columns.GridColumn colDeleteB;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoDeleteB;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoEmptyB;
        private DevExpress.XtraGrid.Columns.GridColumn colSTTb;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientNameB;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientCodeB;
        private DevExpress.XtraGrid.Columns.GridColumn colTreatmentCodeB;
        private DevExpress.XtraGrid.Columns.GridColumn colDobB;
        private DevExpress.XtraGrid.Columns.GridColumn colGenderB;
        private DevExpress.XtraGrid.Columns.GridColumn colInTimeB;
        private DevExpress.XtraGrid.Columns.GridColumn colTreatmentTypeB;
        private DevExpress.XtraGrid.Columns.GridColumn colHeinCardB;
        private DevExpress.XtraGrid.Columns.GridColumn colIcdNameB;

        private DevExpress.XtraLayout.LayoutControlItem lciRoom;
        private DevExpress.XtraLayout.LayoutControlItem lciDate;
        private DevExpress.XtraLayout.LayoutControlItem lciShift;
        private DevExpress.XtraLayout.LayoutControlItem lciSearchTop;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSearchTop;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnPrint;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSave;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnCopy;
        private DevExpress.XtraLayout.LayoutControlItem lciCopyFrom;
        private DevExpress.XtraLayout.EmptySpaceItem emptyTop;
        private DevExpress.XtraLayout.LayoutControlItem lciGridSchedule;
        private DevExpress.XtraLayout.LayoutControlItem lciDept;
        private DevExpress.XtraLayout.LayoutControlItem lciAllDept;
        private DevExpress.XtraLayout.LayoutControlItem lciFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciTo;
        private DevExpress.XtraLayout.LayoutControlItem lciSearchBottom;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSearchBottom;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnAdd;
        private DevExpress.XtraLayout.LayoutControlItem lciGridTreatment;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraLayout.LayoutControlItem lciPaging;
        private DevExpress.XtraEditors.SimpleButton btnNextDate;
        private DevExpress.XtraEditors.SimpleButton btnPrevDate;
        private DevExpress.XtraEditors.TextEdit txtDepartmentCode;
        private DevExpress.XtraEditors.TextEdit txtRoomCode;
        private DevExpress.XtraLayout.LayoutControlItem lciRoomCode;
        private DevExpress.XtraLayout.LayoutControlItem lciDepartmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnPrevDate;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnNextDate;
        private DevExpress.XtraEditors.SimpleButton btnNextShift;
        private DevExpress.XtraEditors.SimpleButton btnPrevShift;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnPrevShift;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnNextShift;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.TextEdit txtNote;
        private DevExpress.XtraEditors.GridLookUpEdit cboTemplate;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTemplate;
        private DevExpress.XtraLayout.LayoutControlItem lciTemplate;
        private DevExpress.XtraLayout.LayoutControlItem lciNote;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}

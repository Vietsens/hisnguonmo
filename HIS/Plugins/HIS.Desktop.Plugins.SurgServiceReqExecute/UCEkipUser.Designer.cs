/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    partial class UCEkipUser
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.grdControlInformationSurg = new DevExpress.XtraGrid.GridControl();
            this.grdViewInformationSurg = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cboPosition = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GridLookupEdit_UserName = new HIS.Desktop.Utilities.Extensions.RepositoryItemCustomGridLookUpEdit();
            this.repositoryItemCustomGridLookUpEdit2View = new HIS.Desktop.Utilities.Extensions.CustomGridView();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GridLookUpEdit_Department = new Inventec.Desktop.CustomControl.CustomGrid.RepositoryItemCustomGridLookUpEdit();
            this.repositoryItemCustomGridLookUpEdit1View = new Inventec.Desktop.CustomControl.CustomGrid.CustomGridView();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnAdd = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.txtLogin = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.btnDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemGridLookUpEditUsername = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
            this.repositoryItemGridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemSearchLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.repositoryItemSearchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciInformationSurg = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdControlInformationSurg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewInformationSurg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridLookupEdit_UserName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCustomGridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridLookUpEdit_Department)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCustomGridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemGridLookUpEditUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemGridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInformationSurg)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.grdControlInformationSurg);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(742, 264);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // grdControlInformationSurg
            // 
            this.grdControlInformationSurg.Location = new System.Drawing.Point(2, 2);
            this.grdControlInformationSurg.MainView = this.grdViewInformationSurg;
            this.grdControlInformationSurg.Name = "grdControlInformationSurg";
            this.grdControlInformationSurg.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnAdd,
            this.txtLogin,
            this.btnDelete,
            this.repositoryItemGridLookUpEditUsername,
            this.repositoryItemSearchLookUpEdit1,
            this.GridLookUpEdit_Department,
            this.GridLookupEdit_UserName,
            this.cboPosition});
            this.grdControlInformationSurg.Size = new System.Drawing.Size(738, 260);
            this.grdControlInformationSurg.TabIndex = 4;
            this.grdControlInformationSurg.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewInformationSurg});
            this.grdControlInformationSurg.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.grdControlInformationSurg_ProcessGridKey);
            // 
            // grdViewInformationSurg
            // 
            this.grdViewInformationSurg.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn6,
            this.gridColumn5});
            this.grdViewInformationSurg.GridControl = this.grdControlInformationSurg;
            this.grdViewInformationSurg.Name = "grdViewInformationSurg";
            this.grdViewInformationSurg.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.grdViewInformationSurg.OptionsView.ShowGroupPanel = false;
            this.grdViewInformationSurg.OptionsView.ShowIndicator = false;
            this.grdViewInformationSurg.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.grdViewInformationSurg_CustomRowCellEdit);
            this.grdViewInformationSurg.ShownEditor += new System.EventHandler(this.grdViewInformationSurg_ShownEditor);
            this.grdViewInformationSurg.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.grdViewInformationSurg_FocusedColumnChanged);
            this.grdViewInformationSurg.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.grdViewInformationSurg_CellValueChanged);
            this.grdViewInformationSurg.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.grdViewInformationSurg_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "Vai trò";
            this.gridColumn1.ColumnEdit = this.cboPosition;
            this.gridColumn1.FieldName = "EXECUTE_ROLE_ID";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 209;
            // 
            // cboPosition
            // 
            this.cboPosition.AutoHeight = false;
            this.cboPosition.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPosition.Name = "cboPosition";
            this.cboPosition.NullText = "";
            this.cboPosition.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboPosition.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPosition_Closed);
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "Họ tên";
            this.gridColumn2.ColumnEdit = this.GridLookupEdit_UserName;
            this.gridColumn2.FieldName = "LOGINNAME";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 209;
            // 
            // GridLookupEdit_UserName
            // 
            this.GridLookupEdit_UserName.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.GridLookupEdit_UserName.AutoComplete = false;
            this.GridLookupEdit_UserName.AutoHeight = false;
            this.GridLookupEdit_UserName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.GridLookupEdit_UserName.Name = "GridLookupEdit_UserName";
            this.GridLookupEdit_UserName.NullText = "";
            this.GridLookupEdit_UserName.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.GridLookupEdit_UserName.View = this.repositoryItemCustomGridLookUpEdit2View;
            this.GridLookupEdit_UserName.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.GridLookupEdit_UserName_Closed);
            // 
            // repositoryItemCustomGridLookUpEdit2View
            // 
            this.repositoryItemCustomGridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemCustomGridLookUpEdit2View.Name = "repositoryItemCustomGridLookUpEdit2View";
            this.repositoryItemCustomGridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemCustomGridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "Khoa";
            this.gridColumn3.ColumnEdit = this.GridLookUpEdit_Department;
            this.gridColumn3.FieldName = "DEPARTMENT_ID";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 182;
            // 
            // GridLookUpEdit_Department
            // 
            this.GridLookUpEdit_Department.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.GridLookUpEdit_Department.AutoComplete = false;
            this.GridLookUpEdit_Department.AutoHeight = false;
            this.GridLookUpEdit_Department.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.GridLookUpEdit_Department.Name = "GridLookUpEdit_Department";
            this.GridLookUpEdit_Department.NullText = "";
            this.GridLookUpEdit_Department.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.GridLookUpEdit_Department.View = this.repositoryItemCustomGridLookUpEdit1View;
            // 
            // repositoryItemCustomGridLookUpEdit1View
            // 
            this.repositoryItemCustomGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemCustomGridLookUpEdit1View.Name = "repositoryItemCustomGridLookUpEdit1View";
            this.repositoryItemCustomGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemCustomGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "gridColumn4";
            this.gridColumn4.FieldName = "BtnDelete";
            this.gridColumn4.MaxWidth = 25;
            this.gridColumn4.MinWidth = 25;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.ShowCaption = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 25;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "gridColumn6";
            this.gridColumn6.FieldName = "BtnAdd";
            this.gridColumn6.MaxWidth = 25;
            this.gridColumn6.MinWidth = 25;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.ShowCaption = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 4;
            this.gridColumn6.Width = 25;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "gridColumn5";
            this.gridColumn5.FieldName = "LOGINNAME";
            this.gridColumn5.Name = "gridColumn5";
            // 
            // btnAdd
            // 
            this.btnAdd.AutoHeight = false;
            this.btnAdd.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnAdd.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnAdd_ButtonClick);
            // 
            // txtLogin
            // 
            this.txtLogin.AutoHeight = false;
            this.txtLogin.Name = "txtLogin";
            // 
            // btnDelete
            // 
            this.btnDelete.AutoHeight = false;
            this.btnDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Minus)});
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnDelete_ButtonClick);
            // 
            // repositoryItemGridLookUpEditUsername
            // 
            this.repositoryItemGridLookUpEditUsername.AutoHeight = false;
            this.repositoryItemGridLookUpEditUsername.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemGridLookUpEditUsername.Name = "repositoryItemGridLookUpEditUsername";
            this.repositoryItemGridLookUpEditUsername.NullText = "";
            this.repositoryItemGridLookUpEditUsername.View = this.repositoryItemGridLookUpEdit1View;
            // 
            // repositoryItemGridLookUpEdit1View
            // 
            this.repositoryItemGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemGridLookUpEdit1View.Name = "repositoryItemGridLookUpEdit1View";
            this.repositoryItemGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // repositoryItemSearchLookUpEdit1
            // 
            this.repositoryItemSearchLookUpEdit1.AutoHeight = false;
            this.repositoryItemSearchLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSearchLookUpEdit1.Name = "repositoryItemSearchLookUpEdit1";
            this.repositoryItemSearchLookUpEdit1.NullText = "";
            this.repositoryItemSearchLookUpEdit1.View = this.repositoryItemSearchLookUpEdit1View;
            // 
            // repositoryItemSearchLookUpEdit1View
            // 
            this.repositoryItemSearchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemSearchLookUpEdit1View.Name = "repositoryItemSearchLookUpEdit1View";
            this.repositoryItemSearchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemSearchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciInformationSurg});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(742, 264);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciInformationSurg
            // 
            this.lciInformationSurg.AppearanceItemCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lciInformationSurg.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciInformationSurg.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciInformationSurg.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciInformationSurg.Control = this.grdControlInformationSurg;
            this.lciInformationSurg.Location = new System.Drawing.Point(0, 0);
            this.lciInformationSurg.Name = "lciInformationSurg";
            this.lciInformationSurg.Size = new System.Drawing.Size(742, 264);
            this.lciInformationSurg.Text = "Kíp thực hiện:";
            this.lciInformationSurg.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciInformationSurg.TextSize = new System.Drawing.Size(0, 0);
            this.lciInformationSurg.TextToControlDistance = 0;
            this.lciInformationSurg.TextVisible = false;
            // 
            // UCEkipUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCEkipUser";
            this.Size = new System.Drawing.Size(742, 264);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdControlInformationSurg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewInformationSurg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridLookupEdit_UserName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCustomGridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridLookUpEdit_Department)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCustomGridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemGridLookUpEditUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemGridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInformationSurg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl grdControlInformationSurg;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewInformationSurg;
        private DevExpress.XtraLayout.LayoutControlItem lciInformationSurg;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnAdd;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit txtLogin;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemGridLookUpEditUsername;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemGridLookUpEdit1View;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit repositoryItemSearchLookUpEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemSearchLookUpEdit1View;
        private Inventec.Desktop.CustomControl.CustomGrid.RepositoryItemCustomGridLookUpEdit GridLookUpEdit_Department;
        private Inventec.Desktop.CustomControl.CustomGrid.CustomGridView repositoryItemCustomGridLookUpEdit1View;
        private Utilities.Extensions.RepositoryItemCustomGridLookUpEdit GridLookupEdit_UserName;
        private Utilities.Extensions.CustomGridView repositoryItemCustomGridLookUpEdit2View;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cboPosition;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
    }
}

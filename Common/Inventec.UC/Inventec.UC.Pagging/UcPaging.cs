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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;

namespace Inventec.UC.Paging
{
    public delegate void LoadDataDelegate(object data);

    public class UcPaging : UserControl
    {
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.ComboBoxEdit txtPageSize;
        private DevExpress.XtraEditors.SimpleButton btnRefreshNavigation;
        private DevExpress.XtraEditors.SimpleButton btnLastPage;
        private DevExpress.XtraEditors.SimpleButton btnNextPage;
        private DevExpress.XtraEditors.LabelControl lblTotalPage;
        private DevExpress.XtraEditors.TextEdit txtCurrentPage;
        private DevExpress.XtraEditors.SimpleButton btnPreviousPage;
        private DevExpress.XtraEditors.SimpleButton btnFirstPage;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        internal DevExpress.XtraEditors.LabelControl lblDisplayPageNo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;

        public PagingGrid pagingGrid;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        LoadDataDelegate LoadData;
        GridControl gridControl;
        DevExpress.XtraTreeList.TreeList treeControl;
        public UcPaging()
        {
            InitializeComponent();
        }

        public void Init(LoadDataDelegate loadData, CommonParam param)
        {
            try
            {
                btnPrint.Enabled = false;
                txtPageSize.EditValue = 100;
                pagingGrid = new PagingGrid();
                LoadData = loadData;

                int rowCount = (param == null ? 0 : (param.Count ?? 0));
                int recordData = (param == null ? 0 : (param.Limit ?? 0));
                pagingGrid.Innitial(lblDisplayPageNo, txtPageSize, txtCurrentPage, lblTotalPage, btnLastPage, btnPreviousPage, btnFirstPage, btnNextPage, rowCount, recordData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Init(LoadDataDelegate loadData, CommonParam param, int pagesize)
        {
            try
            {
                btnPrint.Enabled = false;
                txtPageSize.EditValue = pagesize;
                pagingGrid = new PagingGrid();
                LoadData = loadData;

                int rowCount = (param == null ? 0 : (param.Count ?? 0));
                int recordData = (param == null ? 0 : (param.Limit ?? 0));
                pagingGrid.Innitial(lblDisplayPageNo, txtPageSize, txtCurrentPage, lblTotalPage, btnLastPage, btnPreviousPage, btnFirstPage, btnNextPage, rowCount, recordData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Init(LoadDataDelegate loadData, CommonParam param, int pagesize, GridControl grid)
        {
            try
            {
                this.gridControl = grid;
                btnPrint.Enabled = true;
                txtPageSize.EditValue = pagesize;
                pagingGrid = new PagingGrid();
                LoadData = loadData;

                int rowCount = (param == null ? 0 : (param.Count ?? 0));
                int recordData = (param == null ? 0 : (param.Limit ?? 0));
                pagingGrid.Innitial(lblDisplayPageNo, txtPageSize, txtCurrentPage, lblTotalPage, btnLastPage, btnPreviousPage, btnFirstPage, btnNextPage, rowCount, recordData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Init(LoadDataDelegate loadData, CommonParam param, int pagesize, DevExpress.XtraTreeList.TreeList tree)
        {
            try
            {
                this.treeControl = tree;
                btnPrint.Enabled = true;
                txtPageSize.EditValue = pagesize;
                pagingGrid = new PagingGrid();
                LoadData = loadData;

                int rowCount = (param == null ? 0 : (param.Count ?? 0));
                int recordData = (param == null ? 0 : (param.Limit ?? 0));
                pagingGrid.Innitial(lblDisplayPageNo, txtPageSize, txtCurrentPage, lblTotalPage, btnLastPage, btnPreviousPage, btnFirstPage, btnNextPage, rowCount, recordData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UcPaging));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.lblDisplayPageNo = new DevExpress.XtraEditors.LabelControl();
            this.txtPageSize = new DevExpress.XtraEditors.ComboBoxEdit();
            this.btnRefreshNavigation = new DevExpress.XtraEditors.SimpleButton();
            this.btnLastPage = new DevExpress.XtraEditors.SimpleButton();
            this.btnNextPage = new DevExpress.XtraEditors.SimpleButton();
            this.lblTotalPage = new DevExpress.XtraEditors.LabelControl();
            this.txtCurrentPage = new DevExpress.XtraEditors.TextEdit();
            this.btnPreviousPage = new DevExpress.XtraEditors.SimpleButton();
            this.btnFirstPage = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPageSize.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentPage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.lblDisplayPageNo);
            this.layoutControl1.Controls.Add(this.txtPageSize);
            this.layoutControl1.Controls.Add(this.btnRefreshNavigation);
            this.layoutControl1.Controls.Add(this.btnLastPage);
            this.layoutControl1.Controls.Add(this.btnNextPage);
            this.layoutControl1.Controls.Add(this.lblTotalPage);
            this.layoutControl1.Controls.Add(this.txtCurrentPage);
            this.layoutControl1.Controls.Add(this.btnPreviousPage);
            this.layoutControl1.Controls.Add(this.btnFirstPage);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(315, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnPrint
            // 
            this.btnPrint.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.Image")));
            this.btnPrint.Location = new System.Drawing.Point(253, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(26, 20);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 24;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // lblDisplayPageNo
            // 
            this.lblDisplayPageNo.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDisplayPageNo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDisplayPageNo.Location = new System.Drawing.Point(282, 1);
            this.lblDisplayPageNo.MinimumSize = new System.Drawing.Size(0, 22);
            this.lblDisplayPageNo.Name = "lblDisplayPageNo";
            this.lblDisplayPageNo.Size = new System.Drawing.Size(32, 22);
            this.lblDisplayPageNo.StyleController = this.layoutControl1;
            this.lblDisplayPageNo.TabIndex = 23;
            // 
            // txtPageSize
            // 
            this.txtPageSize.EditValue = "100";
            this.txtPageSize.Location = new System.Drawing.Point(203, 2);
            this.txtPageSize.Name = "txtPageSize";
            this.txtPageSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPageSize.Properties.Items.AddRange(new object[] {
            "10",
            "20",
            "50",
            "100",
            "500",
            "1000",
            "2000"});
            this.txtPageSize.Size = new System.Drawing.Size(46, 20);
            this.txtPageSize.StyleController = this.layoutControl1;
            this.txtPageSize.TabIndex = 22;
            this.txtPageSize.SelectedIndexChanged += new System.EventHandler(this.txtPageSize_SelectedIndexChanged);
            // 
            // btnRefreshNavigation
            // 
            this.btnRefreshNavigation.AutoWidthInLayoutControl = true;
            this.btnRefreshNavigation.Image = ((System.Drawing.Image)(resources.GetObject("btnRefreshNavigation.Image")));
            this.btnRefreshNavigation.Location = new System.Drawing.Point(176, 2);
            this.btnRefreshNavigation.Name = "btnRefreshNavigation";
            this.btnRefreshNavigation.Size = new System.Drawing.Size(23, 20);
            this.btnRefreshNavigation.StyleController = this.layoutControl1;
            this.btnRefreshNavigation.TabIndex = 21;
            this.btnRefreshNavigation.Click += new System.EventHandler(this.btnRefreshNavigation_Click);
            // 
            // btnLastPage
            // 
            this.btnLastPage.AutoWidthInLayoutControl = true;
            this.btnLastPage.Image = ((System.Drawing.Image)(resources.GetObject("btnLastPage.Image")));
            this.btnLastPage.Location = new System.Drawing.Point(150, 2);
            this.btnLastPage.Name = "btnLastPage";
            this.btnLastPage.Size = new System.Drawing.Size(22, 20);
            this.btnLastPage.StyleController = this.layoutControl1;
            this.btnLastPage.TabIndex = 20;
            this.btnLastPage.Click += new System.EventHandler(this.btnLastPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.AutoWidthInLayoutControl = true;
            this.btnNextPage.Image = ((System.Drawing.Image)(resources.GetObject("btnNextPage.Image")));
            this.btnNextPage.Location = new System.Drawing.Point(123, 2);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(23, 20);
            this.btnNextPage.StyleController = this.layoutControl1;
            this.btnNextPage.TabIndex = 19;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // lblTotalPage
            // 
            this.lblTotalPage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalPage.Location = new System.Drawing.Point(90, 1);
            this.lblTotalPage.MinimumSize = new System.Drawing.Size(0, 22);
            this.lblTotalPage.Name = "lblTotalPage";
            this.lblTotalPage.Size = new System.Drawing.Size(30, 22);
            this.lblTotalPage.StyleController = this.layoutControl1;
            this.lblTotalPage.TabIndex = 17;
            // 
            // txtCurrentPage
            // 
            this.txtCurrentPage.Location = new System.Drawing.Point(54, 2);
            this.txtCurrentPage.Name = "txtCurrentPage";
            this.txtCurrentPage.Size = new System.Drawing.Size(33, 20);
            this.txtCurrentPage.StyleController = this.layoutControl1;
            this.txtCurrentPage.TabIndex = 16;
            this.txtCurrentPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCurrentPage_KeyPress);
            this.txtCurrentPage.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCurrentPage_PreviewKeyDown);
            // 
            // btnPreviousPage
            // 
            this.btnPreviousPage.AutoWidthInLayoutControl = true;
            this.btnPreviousPage.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousPage.Image")));
            this.btnPreviousPage.Location = new System.Drawing.Point(28, 2);
            this.btnPreviousPage.Name = "btnPreviousPage";
            this.btnPreviousPage.Size = new System.Drawing.Size(22, 20);
            this.btnPreviousPage.StyleController = this.layoutControl1;
            this.btnPreviousPage.TabIndex = 15;
            this.btnPreviousPage.Click += new System.EventHandler(this.btnPreviousPage_Click);
            // 
            // btnFirstPage
            // 
            this.btnFirstPage.AutoWidthInLayoutControl = true;
            this.btnFirstPage.Image = ((System.Drawing.Image)(resources.GetObject("btnFirstPage.Image")));
            this.btnFirstPage.Location = new System.Drawing.Point(2, 2);
            this.btnFirstPage.Name = "btnFirstPage";
            this.btnFirstPage.Size = new System.Drawing.Size(22, 20);
            this.btnFirstPage.StyleController = this.layoutControl1;
            this.btnFirstPage.TabIndex = 14;
            this.btnFirstPage.Click += new System.EventHandler(this.btnFirstPage_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem9,
            this.layoutControlItem8,
            this.layoutControlItem7,
            this.layoutControlItem6,
            this.layoutControlItem4,
            this.layoutControlItem3,
            this.layoutControlItem2,
            this.layoutControlItem10,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(315, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txtPageSize;
            this.layoutControlItem1.Location = new System.Drawing.Point(201, 0);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(50, 20);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(50, 24);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnFirstPage;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem9.MaxSize = new System.Drawing.Size(28, 26);
            this.layoutControlItem9.MinSize = new System.Drawing.Size(25, 20);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(26, 24);
            this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.btnPreviousPage;
            this.layoutControlItem8.Location = new System.Drawing.Point(26, 0);
            this.layoutControlItem8.MaxSize = new System.Drawing.Size(28, 26);
            this.layoutControlItem8.MinSize = new System.Drawing.Size(25, 20);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(26, 24);
            this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.txtCurrentPage;
            this.layoutControlItem7.Location = new System.Drawing.Point(52, 0);
            this.layoutControlItem7.MaxSize = new System.Drawing.Size(70, 24);
            this.layoutControlItem7.MinSize = new System.Drawing.Size(35, 18);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(37, 24);
            this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.lblTotalPage;
            this.layoutControlItem6.Location = new System.Drawing.Point(89, 0);
            this.layoutControlItem6.MaxSize = new System.Drawing.Size(0, 26);
            this.layoutControlItem6.MinSize = new System.Drawing.Size(30, 20);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1);
            this.layoutControlItem6.Size = new System.Drawing.Size(32, 24);
            this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnNextPage;
            this.layoutControlItem4.Location = new System.Drawing.Point(121, 0);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(30, 26);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(25, 20);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(27, 24);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnLastPage;
            this.layoutControlItem3.Location = new System.Drawing.Point(148, 0);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(30, 26);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(25, 20);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(26, 24);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnRefreshNavigation;
            this.layoutControlItem2.Location = new System.Drawing.Point(174, 0);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(30, 26);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(25, 20);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(27, 24);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.lblDisplayPageNo;
            this.layoutControlItem10.Location = new System.Drawing.Point(281, 0);
            this.layoutControlItem10.MaxSize = new System.Drawing.Size(0, 26);
            this.layoutControlItem10.MinSize = new System.Drawing.Size(14, 20);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1);
            this.layoutControlItem10.Size = new System.Drawing.Size(34, 24);
            this.layoutControlItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextToControlDistance = 0;
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnPrint;
            this.layoutControlItem5.Location = new System.Drawing.Point(251, 0);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(30, 26);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(25, 20);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(30, 24);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // UcPaging
            // 
            this.Controls.Add(this.layoutControl1);
            this.Name = "UcPaging";
            this.Size = new System.Drawing.Size(315, 24);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPageSize.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentPage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.ResumeLayout(false);

        }

        #region Paging grid
        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            try
            {
                pagingGrid.FirstPage();
                LoadData(GetParamPaging());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            try
            {
                pagingGrid.PreviousPage();
                LoadData(GetParamPaging());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            try
            {
                pagingGrid.NextPage();
                LoadData(GetParamPaging());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            try
            {
                pagingGrid.LastPage();
                LoadData(GetParamPaging());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefreshNavigation_Click(object sender, EventArgs e)
        {
            try
            {
                LoadData(GetParamPaging());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (pagingGrid != null)
                {
                    CommonParam param = GetParamPaging();
                    LoadData(param);
                    int rowCount = (param == null ? 0 : (param.Count ?? 0));
                    int recordData = (param == null ? 0 : (param.Limit ?? 0));
                    pagingGrid.Innitial(lblDisplayPageNo, txtPageSize, txtCurrentPage, lblTotalPage, btnLastPage, btnPreviousPage, btnFirstPage, btnNextPage, rowCount, recordData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private CommonParam GetParamPaging()
        {
            CommonParam param = new CommonParam();
            try
            {
                if (pagingGrid != null)
                {
                    param.Count = pagingGrid.MaxRec;
                    param.Start = pagingGrid.RecNo;
                    param.Limit = Convert.ToInt32(txtPageSize.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
            return param;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.gridControl != null)
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "Excel file|*.xlsx|All file|*.*";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        this.gridControl.ExportToXlsx(saveFile.FileName);
                    }
                }
                if (this.treeControl != null)
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "Excel file|*.xlsx|All file|*.*";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        this.treeControl.ExportToXlsx(saveFile.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void txtCurrentPage_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    pagingGrid.NextPageByPageNumber(int.Parse(txtCurrentPage.Text));
                    LoadData(GetParamPaging());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
        #endregion

        private void txtCurrentPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

    }
}

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
using DevExpress.Utils;
using DevExpress.XtraEditors.DXErrorProvider;
using System;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    partial class UCSurgServiceReqExecute2
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCSurgServiceReqExecute2));
            this.dxValidationProviderEditorInfo = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.toolTipControllerGrid = new DevExpress.Utils.ToolTipController(this.components);
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.grdControlInformationSurg = new DevExpress.XtraGrid.GridControl();
            this.grdViewInformationSurg = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repExecute = new Inventec.Desktop.CustomControl.RepositoryItemCustomGridLookUpEdit();
            this.repositoryItemCustomGridLookUpEdit2View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repUser = new Inventec.Desktop.CustomControl.RepositoryItemCustomGridLookUpEdit();
            this.repositoryItemCustomGridLookUpEdit1View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repDepartment = new Inventec.Desktop.CustomControl.RepositoryItemCustomGridLookUpEdit();
            this.customGridViewWithFilterMultiColumn1 = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repMinus = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repPlus = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnSaveEkip = new DevExpress.XtraEditors.SimpleButton();
            this.cboEkipUser = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.customGridLookUpEditWithFilterMultiColumn7View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.cboPtttGroup = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.customGridLookUpEditWithFilterMultiColumn6View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.cboPtttMethodReal = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.customGridLookUpEditWithFilterMultiColumn5View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.txtPtttGroup = new DevExpress.XtraEditors.TextEdit();
            this.txtPtttMethodReal = new DevExpress.XtraEditors.TextEdit();
            this.txtEmotionLessMethod = new DevExpress.XtraEditors.TextEdit();
            this.cboEmotionLessMethod = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.customGridLookUpEditWithFilterMultiColumn4View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.cboPtttMethod = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.customGridLookUpEditWithFilterMultiColumn3View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.txtPtttMethod = new DevExpress.XtraEditors.TextEdit();
            this.dteFinish = new DevExpress.XtraEditors.DateEdit();
            this.dteStart = new DevExpress.XtraEditors.DateEdit();
            this.cboDepartment = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.customGridLookUpEditWithFilterMultiColumn2View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.cboPtttTemp_v45072 = new DevExpress.XtraEditors.LookUpEdit();
            this.btnSavePtttTemp_v45072 = new DevExpress.XtraEditors.SimpleButton();
            this.txtIcdCode_v45072 = new DevExpress.XtraEditors.TextEdit();
            this.cboIcdName_v45072 = new DevExpress.XtraEditors.LookUpEdit();
            this.chkSuaIcd_v45072 = new DevExpress.XtraEditors.CheckEdit();
            this.txtIcdSubCode_v45072 = new DevExpress.XtraEditors.TextEdit();
            this.cboIcdText_v45072 = new DevExpress.XtraEditors.LookUpEdit();
            this.txtIcdCmCode_v45072 = new DevExpress.XtraEditors.TextEdit();
            this.cboIcdCmName_v45072 = new DevExpress.XtraEditors.LookUpEdit();
            this.chkSuaIcdCm_v45072 = new DevExpress.XtraEditors.CheckEdit();
            this.txtIcdCmSubCode_v45072 = new DevExpress.XtraEditors.TextEdit();
            this.cboIcdCmText_v45072 = new DevExpress.XtraEditors.LookUpEdit();
            this.spnTimeProcess_v45072 = new DevExpress.XtraEditors.SpinEdit();
            this.lblPhut_v45072 = new DevExpress.XtraEditors.LabelControl();
            this.cboEmotionLess_v45072 = new DevExpress.XtraEditors.LookUpEdit();
            this.txtManner_v45072 = new DevExpress.XtraEditors.MemoEdit();
            this.cboMachine_v45072 = new DevExpress.XtraEditors.LookUpEdit();
            this.txtConclude_v45072 = new DevExpress.XtraEditors.MemoEdit();
            this.txtInstructionNote_v45072 = new DevExpress.XtraEditors.MemoEdit();
            this.tabDescription_v45072 = new DevExpress.XtraTab.XtraTabControl();
            this.tabPageMoTa_v45072 = new DevExpress.XtraTab.XtraTabPage();
            this.txtDescription_v45072 = new DevExpress.XtraEditors.MemoEdit();
            this.tabPageGhiChu_v45072 = new DevExpress.XtraTab.XtraTabPage();
            this.txtNote_v45072 = new DevExpress.XtraEditors.MemoEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciPtttTemp_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIcdCode_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIcdName_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChkSuaIcd_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIcdSubCode_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIcdText_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIcdCmCode_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIcdCmName_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChkSuaIcdCm_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIcdCmSubCode_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIcdCmText_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTimeProcess_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciLblPhut_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciEmotionLess_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            // Việc 45072 — Bổ sung txt code + lci cho cbo (giống pattern Phương pháp TT theo y/c TuanLN)
            this.txtEmotionLessCode_v45072 = new DevExpress.XtraEditors.TextEdit();
            this.lciCboEmotionLess_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciManner_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMachine_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConclude_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciInstructionNote_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTabDescription_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem26 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem27 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem29 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem30 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem31 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem32 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem33 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciBtnSavePtttTemp_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem34 = new DevExpress.XtraLayout.LayoutControlItem();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.lblNote = new DevExpress.XtraEditors.LabelControl();
            this.lblType = new DevExpress.XtraEditors.LabelControl();
            this.lblHeinCardFromTo = new DevExpress.XtraEditors.LabelControl();
            this.lblAddress = new DevExpress.XtraEditors.LabelControl();
            this.lblKCBBD = new DevExpress.XtraEditors.LabelControl();
            this.lblHeinCardNumber = new DevExpress.XtraEditors.LabelControl();
            this.lblGender = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientDob = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientCode = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repStt = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnPatientType_v45072 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnRequestDoctor_v45072 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnBeginTime_v45072 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnEndTime_v45072 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnPrice_v45072 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtFind = new DevExpress.XtraEditors.TextEdit();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.cboStt = new DevExpress.XtraEditors.ComboBoxEdit();
            this.dteTo = new DevExpress.XtraEditors.DateEdit();
            this.dteFrom = new DevExpress.XtraEditors.DateEdit();
            this.cboService = new Inventec.Desktop.CustomControl.CustomGrid.CustomGridLookUpEdit();
            this.customGridLookUpEdit1View = new Inventec.Desktop.CustomControl.CustomGrid.CustomGridView();
            this.lblTotalPatient_v45072 = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalService_v45072 = new DevExpress.XtraEditors.LabelControl();
            this.btnDanhSachYLenh_v45072 = new DevExpress.XtraEditors.SimpleButton();
            this.chkKT_v45072 = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem35 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciChkKT_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnDanhSach_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTotalService_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTotalPatient_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciMachineCode_v45072 = new DevExpress.XtraLayout.LayoutControlItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdControlInformationSurg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewInformationSurg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repExecute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCustomGridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCustomGridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridViewWithFilterMultiColumn1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repMinus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEkipUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn7View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPtttGroup.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn6View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPtttMethodReal.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn5View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPtttGroup.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPtttMethodReal.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmotionLessMethod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEmotionLessMethod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn4View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPtttMethod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn3View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPtttMethod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFinish.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFinish.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStart.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStart.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPtttTemp_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCode_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcdName_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSuaIcd_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdSubCode_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcdText_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCmCode_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcdCmName_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSuaIcdCm_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCmSubCode_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcdCmText_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnTimeProcess_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEmotionLess_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtManner_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMachine_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConclude_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInstructionNote_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabDescription_v45072)).BeginInit();
            this.tabDescription_v45072.SuspendLayout();
            this.tabPageMoTa_v45072.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription_v45072.Properties)).BeginInit();
            this.tabPageGhiChu_v45072.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNote_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPtttTemp_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdCode_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdName_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkSuaIcd_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdSubCode_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdText_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdCmCode_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdCmName_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkSuaIcdCm_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdCmSubCode_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdCmText_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTimeProcess_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLblPhut_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciEmotionLess_v45072)).BeginInit();
            // Việc 45072 — BeginInit txt code + lci cbo Vô cảm
            ((System.ComponentModel.ISupportInitialize)(this.txtEmotionLessCode_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboEmotionLess_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciManner_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMachine_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclude_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInstructionNote_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTabDescription_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSavePtttTemp_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repStt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFind.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboService.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkKT_v45072.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkKT_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDanhSach_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalService_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalPatient_v45072)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMachineCode_v45072)).BeginInit();
            this.SuspendLayout();
            // 
            // dxValidationProviderEditorInfo
            // 
            this.dxValidationProviderEditorInfo.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider_ValidationFailed);
            // 
            // dxErrorProvider
            // 
            this.dxErrorProvider.ContainerControl = this;
            // 
            // toolTipControllerGrid
            // 
            this.toolTipControllerGrid.AllowHtmlText = true;
            this.toolTipControllerGrid.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipControllerGrid_GetActiveObjectInfo);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.groupControl2);
            this.layoutControl1.Controls.Add(this.groupControl1);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.txtFind);
            this.layoutControl1.Controls.Add(this.txtPatientCode);
            this.layoutControl1.Controls.Add(this.cboStt);
            this.layoutControl1.Controls.Add(this.dteTo);
            this.layoutControl1.Controls.Add(this.dteFrom);
            this.layoutControl1.Controls.Add(this.cboService);
            this.layoutControl1.Controls.Add(this.lblTotalPatient_v45072);
            this.layoutControl1.Controls.Add(this.lblTotalService_v45072);
            this.layoutControl1.Controls.Add(this.btnDanhSachYLenh_v45072);
            this.layoutControl1.Controls.Add(this.chkKT_v45072);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1648, 629);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(1523, 599);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(122, 27);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.layoutControl3);
            this.groupControl2.Location = new System.Drawing.Point(783, 146);
            this.groupControl2.Margin = new System.Windows.Forms.Padding(4);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(862, 447);
            this.groupControl2.TabIndex = 13;
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.grdControlInformationSurg);
            this.layoutControl3.Controls.Add(this.btnSaveEkip);
            this.layoutControl3.Controls.Add(this.cboEkipUser);
            this.layoutControl3.Controls.Add(this.cboPtttGroup);
            this.layoutControl3.Controls.Add(this.cboPtttMethodReal);
            this.layoutControl3.Controls.Add(this.txtPtttGroup);
            this.layoutControl3.Controls.Add(this.txtPtttMethodReal);
            this.layoutControl3.Controls.Add(this.txtEmotionLessMethod);
            this.layoutControl3.Controls.Add(this.cboEmotionLessMethod);
            this.layoutControl3.Controls.Add(this.cboPtttMethod);
            this.layoutControl3.Controls.Add(this.txtPtttMethod);
            this.layoutControl3.Controls.Add(this.dteFinish);
            this.layoutControl3.Controls.Add(this.dteStart);
            this.layoutControl3.Controls.Add(this.cboDepartment);
            this.layoutControl3.Controls.Add(this.cboPtttTemp_v45072);
            this.layoutControl3.Controls.Add(this.btnSavePtttTemp_v45072);
            this.layoutControl3.Controls.Add(this.txtIcdCode_v45072);
            this.layoutControl3.Controls.Add(this.cboIcdName_v45072);
            this.layoutControl3.Controls.Add(this.chkSuaIcd_v45072);
            this.layoutControl3.Controls.Add(this.txtIcdSubCode_v45072);
            this.layoutControl3.Controls.Add(this.cboIcdText_v45072);
            this.layoutControl3.Controls.Add(this.txtIcdCmCode_v45072);
            this.layoutControl3.Controls.Add(this.cboIcdCmName_v45072);
            this.layoutControl3.Controls.Add(this.chkSuaIcdCm_v45072);
            this.layoutControl3.Controls.Add(this.txtIcdCmSubCode_v45072);
            this.layoutControl3.Controls.Add(this.cboIcdCmText_v45072);
            this.layoutControl3.Controls.Add(this.spnTimeProcess_v45072);
            this.layoutControl3.Controls.Add(this.lblPhut_v45072);
            this.layoutControl3.Controls.Add(this.cboEmotionLess_v45072);
            // Việc 45072 — Add txt code Vô cảm
            this.layoutControl3.Controls.Add(this.txtEmotionLessCode_v45072);
            this.layoutControl3.Controls.Add(this.txtManner_v45072);
            this.layoutControl3.Controls.Add(this.cboMachine_v45072);
            this.layoutControl3.Controls.Add(this.txtConclude_v45072);
            this.layoutControl3.Controls.Add(this.txtInstructionNote_v45072);
            this.layoutControl3.Controls.Add(this.tabDescription_v45072);
            this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl3.Location = new System.Drawing.Point(2, 25);
            this.layoutControl3.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup3;
            this.layoutControl3.Size = new System.Drawing.Size(858, 420);
            this.layoutControl3.TabIndex = 0;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // grdControlInformationSurg
            // 
            this.grdControlInformationSurg.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.grdControlInformationSurg.Location = new System.Drawing.Point(118, 335);
            this.grdControlInformationSurg.MainView = this.grdViewInformationSurg;
            this.grdControlInformationSurg.Margin = new System.Windows.Forms.Padding(4);
            this.grdControlInformationSurg.Name = "grdControlInformationSurg";
            this.grdControlInformationSurg.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repUser,
            this.repExecute,
            this.repDepartment,
            this.repMinus,
            this.repPlus});
            this.grdControlInformationSurg.Size = new System.Drawing.Size(727, 82);
            this.grdControlInformationSurg.TabIndex = 17;
            this.grdControlInformationSurg.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewInformationSurg});
            this.grdControlInformationSurg.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.grdControlInformationSurg_ProcessGridKey);
            // 
            // grdViewInformationSurg
            // 
            this.grdViewInformationSurg.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11});
            this.grdViewInformationSurg.GridControl = this.grdControlInformationSurg;
            this.grdViewInformationSurg.Name = "grdViewInformationSurg";
            this.grdViewInformationSurg.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.grdViewInformationSurg.OptionsView.ShowGroupPanel = false;
            this.grdViewInformationSurg.OptionsView.ShowIndicator = false;
            this.grdViewInformationSurg.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.grdViewInformationSurg_CustomRowCellEdit);
            this.grdViewInformationSurg.ShownEditor += new System.EventHandler(this.grdViewInformationSurg_ShownEditor);
            this.grdViewInformationSurg.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.grdViewInformationSurg_FocusedColumnChanged);
            this.grdViewInformationSurg.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView2_CellValueChanged);
            this.grdViewInformationSurg.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.grdViewInformationSurg_CustomUnboundColumnData);
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "Vai trò";
            this.gridColumn6.ColumnEdit = this.repExecute;
            this.gridColumn6.FieldName = "EXECUTE_ROLE_ID";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 0;
            this.gridColumn6.Width = 145;
            // 
            // repExecute
            // 
            this.repExecute.AutoComplete = false;
            this.repExecute.AutoHeight = false;
            this.repExecute.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repExecute.Name = "repExecute";
            this.repExecute.NullText = "";
            this.repExecute.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.repExecute.View = this.repositoryItemCustomGridLookUpEdit2View;
            // 
            // repositoryItemCustomGridLookUpEdit2View
            // 
            this.repositoryItemCustomGridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemCustomGridLookUpEdit2View.Name = "repositoryItemCustomGridLookUpEdit2View";
            this.repositoryItemCustomGridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemCustomGridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn7
            // 
            this.gridColumn7.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn7.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn7.Caption = "Họ tên";
            this.gridColumn7.ColumnEdit = this.repUser;
            this.gridColumn7.FieldName = "LOGINNAME";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 1;
            this.gridColumn7.Width = 145;
            // 
            // repUser
            // 
            this.repUser.AutoComplete = false;
            this.repUser.AutoHeight = false;
            this.repUser.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repUser.Name = "repUser";
            this.repUser.NullText = "";
            this.repUser.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.repUser.View = this.repositoryItemCustomGridLookUpEdit1View;
            this.repUser.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.repUser_Closed);
            // 
            // repositoryItemCustomGridLookUpEdit1View
            // 
            this.repositoryItemCustomGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemCustomGridLookUpEdit1View.Name = "repositoryItemCustomGridLookUpEdit1View";
            this.repositoryItemCustomGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemCustomGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn8
            // 
            this.gridColumn8.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn8.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn8.Caption = "Khoa";
            this.gridColumn8.ColumnEdit = this.repDepartment;
            this.gridColumn8.FieldName = "DEPARTMENT_ID";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 2;
            this.gridColumn8.Width = 187;
            // 
            // repDepartment
            // 
            this.repDepartment.AutoComplete = false;
            this.repDepartment.AutoHeight = false;
            this.repDepartment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repDepartment.Name = "repDepartment";
            this.repDepartment.NullText = "";
            this.repDepartment.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.repDepartment.View = this.customGridViewWithFilterMultiColumn1;
            // 
            // customGridViewWithFilterMultiColumn1
            // 
            this.customGridViewWithFilterMultiColumn1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridViewWithFilterMultiColumn1.Name = "customGridViewWithFilterMultiColumn1";
            this.customGridViewWithFilterMultiColumn1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridViewWithFilterMultiColumn1.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "gridColumn9";
            this.gridColumn9.ColumnEdit = this.repMinus;
            this.gridColumn9.FieldName = "BtnDelete";
            this.gridColumn9.MaxWidth = 25;
            this.gridColumn9.MinWidth = 25;
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.OptionsColumn.ShowCaption = false;
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 3;
            this.gridColumn9.Width = 25;
            // 
            // repMinus
            // 
            this.repMinus.AutoHeight = false;
            this.repMinus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Minus)});
            this.repMinus.Name = "repMinus";
            this.repMinus.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repMinus.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repMinus_ButtonClick);
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "gridColumn10";
            this.gridColumn10.FieldName = "BtnAdd";
            this.gridColumn10.MaxWidth = 25;
            this.gridColumn10.MinWidth = 25;
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.OptionsColumn.ShowCaption = false;
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 4;
            this.gridColumn10.Width = 25;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "gridColumn11";
            this.gridColumn11.FieldName = "LOGINNAME";
            this.gridColumn11.Name = "gridColumn11";
            // 
            // repPlus
            // 
            this.repPlus.AutoHeight = false;
            this.repPlus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.repPlus.Name = "repPlus";
            this.repPlus.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repPlus.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repPlus_ButtonClick);
            // 
            // btnSaveEkip
            // 
            this.btnSaveEkip.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveEkip.Image")));
            this.btnSaveEkip.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSaveEkip.Location = new System.Drawing.Point(403, 307);
            this.btnSaveEkip.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveEkip.Name = "btnSaveEkip";
            this.btnSaveEkip.Size = new System.Drawing.Size(22, 20);
            this.btnSaveEkip.StyleController = this.layoutControl3;
            this.btnSaveEkip.TabIndex = 16;
            this.btnSaveEkip.Click += new System.EventHandler(this.btnSaveEkip_Click);
            // 
            // cboEkipUser
            // 
            this.cboEkipUser.Location = new System.Drawing.Point(118, 307);
            this.cboEkipUser.Margin = new System.Windows.Forms.Padding(4);
            this.cboEkipUser.Name = "cboEkipUser";
            this.cboEkipUser.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboEkipUser.Properties.AutoComplete = false;
            this.cboEkipUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboEkipUser.Properties.NullText = "";
            this.cboEkipUser.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboEkipUser.Properties.View = this.customGridLookUpEditWithFilterMultiColumn7View;
            this.cboEkipUser.Size = new System.Drawing.Size(279, 22);
            this.cboEkipUser.StyleController = this.layoutControl3;
            this.cboEkipUser.TabIndex = 15;
            this.cboEkipUser.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboEkipUser_ButtonClick);
            this.cboEkipUser.EditValueChanged += new System.EventHandler(this.cboEkipUser_EditValueChanged);
            // 
            // customGridLookUpEditWithFilterMultiColumn7View
            // 
            this.customGridLookUpEditWithFilterMultiColumn7View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridLookUpEditWithFilterMultiColumn7View.Name = "customGridLookUpEditWithFilterMultiColumn7View";
            this.customGridLookUpEditWithFilterMultiColumn7View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridLookUpEditWithFilterMultiColumn7View.OptionsView.ShowGroupPanel = false;
            // 
            // cboPtttGroup
            // 
            this.cboPtttGroup.Location = new System.Drawing.Point(595, 168);
            this.cboPtttGroup.Margin = new System.Windows.Forms.Padding(4);
            this.cboPtttGroup.Name = "cboPtttGroup";
            this.cboPtttGroup.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboPtttGroup.Properties.AutoComplete = false;
            this.cboPtttGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboPtttGroup.Properties.NullText = "";
            this.cboPtttGroup.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboPtttGroup.Properties.View = this.customGridLookUpEditWithFilterMultiColumn6View;
            this.cboPtttGroup.Size = new System.Drawing.Size(261, 22);
            this.cboPtttGroup.StyleController = this.layoutControl3;
            this.cboPtttGroup.TabIndex = 14;
            this.cboPtttGroup.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPtttGroup_Closed);
            this.cboPtttGroup.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPtttGroup_ButtonClick);
            // 
            // customGridLookUpEditWithFilterMultiColumn6View
            // 
            this.customGridLookUpEditWithFilterMultiColumn6View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridLookUpEditWithFilterMultiColumn6View.Name = "customGridLookUpEditWithFilterMultiColumn6View";
            this.customGridLookUpEditWithFilterMultiColumn6View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridLookUpEditWithFilterMultiColumn6View.OptionsView.ShowGroupPanel = false;
            // 
            // cboPtttMethodReal
            // 
            this.cboPtttMethodReal.Location = new System.Drawing.Point(167, 168);
            this.cboPtttMethodReal.Margin = new System.Windows.Forms.Padding(4);
            this.cboPtttMethodReal.Name = "cboPtttMethodReal";
            this.cboPtttMethodReal.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboPtttMethodReal.Properties.AutoComplete = false;
            this.cboPtttMethodReal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboPtttMethodReal.Properties.NullText = "";
            this.cboPtttMethodReal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboPtttMethodReal.Properties.View = this.customGridLookUpEditWithFilterMultiColumn5View;
            this.cboPtttMethodReal.Size = new System.Drawing.Size(259, 22);
            this.cboPtttMethodReal.StyleController = this.layoutControl3;
            this.cboPtttMethodReal.TabIndex = 13;
            this.cboPtttMethodReal.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPtttMethodReal_Closed);
            this.cboPtttMethodReal.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPtttMethodReal_ButtonClick);
            // 
            // customGridLookUpEditWithFilterMultiColumn5View
            // 
            this.customGridLookUpEditWithFilterMultiColumn5View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridLookUpEditWithFilterMultiColumn5View.Name = "customGridLookUpEditWithFilterMultiColumn5View";
            this.customGridLookUpEditWithFilterMultiColumn5View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridLookUpEditWithFilterMultiColumn5View.OptionsView.ShowGroupPanel = false;
            // 
            // txtPtttGroup
            // 
            this.txtPtttGroup.Location = new System.Drawing.Point(545, 168);
            this.txtPtttGroup.Margin = new System.Windows.Forms.Padding(4);
            this.txtPtttGroup.Name = "txtPtttGroup";
            this.txtPtttGroup.Size = new System.Drawing.Size(50, 22);
            this.txtPtttGroup.StyleController = this.layoutControl3;
            this.txtPtttGroup.TabIndex = 12;
            this.txtPtttGroup.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPtttGroup_PreviewKeyDown);
            // 
            // txtPtttMethodReal
            // 
            this.txtPtttMethodReal.Location = new System.Drawing.Point(117, 168);
            this.txtPtttMethodReal.Margin = new System.Windows.Forms.Padding(4);
            this.txtPtttMethodReal.Name = "txtPtttMethodReal";
            this.txtPtttMethodReal.Size = new System.Drawing.Size(50, 22);
            this.txtPtttMethodReal.StyleController = this.layoutControl3;
            this.txtPtttMethodReal.TabIndex = 11;
            this.txtPtttMethodReal.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPtttMethodReal_PreviewKeyDown);
            // 
            // txtEmotionLessMethod
            // 
            this.txtEmotionLessMethod.Location = new System.Drawing.Point(545, 142);
            this.txtEmotionLessMethod.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmotionLessMethod.Name = "txtEmotionLessMethod";
            this.txtEmotionLessMethod.Size = new System.Drawing.Size(50, 22);
            this.txtEmotionLessMethod.StyleController = this.layoutControl3;
            this.txtEmotionLessMethod.TabIndex = 10;
            this.txtEmotionLessMethod.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtEmotionLessMethod_PreviewKeyDown);
            // 
            // cboEmotionLessMethod
            // 
            this.cboEmotionLessMethod.Location = new System.Drawing.Point(595, 142);
            this.cboEmotionLessMethod.Margin = new System.Windows.Forms.Padding(4);
            this.cboEmotionLessMethod.Name = "cboEmotionLessMethod";
            this.cboEmotionLessMethod.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboEmotionLessMethod.Properties.AutoComplete = false;
            this.cboEmotionLessMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboEmotionLessMethod.Properties.NullText = "";
            this.cboEmotionLessMethod.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboEmotionLessMethod.Properties.View = this.customGridLookUpEditWithFilterMultiColumn4View;
            this.cboEmotionLessMethod.Size = new System.Drawing.Size(261, 22);
            this.cboEmotionLessMethod.StyleController = this.layoutControl3;
            this.cboEmotionLessMethod.TabIndex = 9;
            this.cboEmotionLessMethod.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboEmotionLessMethod_Closed);
            this.cboEmotionLessMethod.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboEmotionLessMethod_ButtonClick);
            // 
            // customGridLookUpEditWithFilterMultiColumn4View
            // 
            this.customGridLookUpEditWithFilterMultiColumn4View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridLookUpEditWithFilterMultiColumn4View.Name = "customGridLookUpEditWithFilterMultiColumn4View";
            this.customGridLookUpEditWithFilterMultiColumn4View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridLookUpEditWithFilterMultiColumn4View.OptionsView.ShowGroupPanel = false;
            // 
            // cboPtttMethod
            // 
            this.cboPtttMethod.Location = new System.Drawing.Point(167, 142);
            this.cboPtttMethod.Margin = new System.Windows.Forms.Padding(4);
            this.cboPtttMethod.Name = "cboPtttMethod";
            this.cboPtttMethod.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboPtttMethod.Properties.AutoComplete = false;
            this.cboPtttMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboPtttMethod.Properties.NullText = "";
            this.cboPtttMethod.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboPtttMethod.Properties.View = this.customGridLookUpEditWithFilterMultiColumn3View;
            this.cboPtttMethod.Size = new System.Drawing.Size(259, 22);
            this.cboPtttMethod.StyleController = this.layoutControl3;
            this.cboPtttMethod.TabIndex = 8;
            this.cboPtttMethod.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPtttMethod_Closed);
            this.cboPtttMethod.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPtttMethod_ButtonClick);
            // 
            // customGridLookUpEditWithFilterMultiColumn3View
            // 
            this.customGridLookUpEditWithFilterMultiColumn3View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridLookUpEditWithFilterMultiColumn3View.Name = "customGridLookUpEditWithFilterMultiColumn3View";
            this.customGridLookUpEditWithFilterMultiColumn3View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridLookUpEditWithFilterMultiColumn3View.OptionsView.ShowGroupPanel = false;
            // 
            // txtPtttMethod
            // 
            this.txtPtttMethod.Location = new System.Drawing.Point(117, 142);
            this.txtPtttMethod.Margin = new System.Windows.Forms.Padding(4);
            this.txtPtttMethod.Name = "txtPtttMethod";
            this.txtPtttMethod.Size = new System.Drawing.Size(50, 22);
            this.txtPtttMethod.StyleController = this.layoutControl3;
            this.txtPtttMethod.TabIndex = 7;
            this.txtPtttMethod.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPtttMethod_PreviewKeyDown);
            // 
            // dteFinish
            // 
            this.dteFinish.EditValue = null;
            this.dteFinish.Location = new System.Drawing.Point(546, 115);
            this.dteFinish.Margin = new System.Windows.Forms.Padding(4);
            this.dteFinish.Name = "dteFinish";
            this.dteFinish.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteFinish.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteFinish.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dteFinish.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dteFinish.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dteFinish.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dteFinish.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dteFinish.Size = new System.Drawing.Size(309, 22);
            this.dteFinish.StyleController = this.layoutControl3;
            this.dteFinish.TabIndex = 6;
            // 
            // dteStart
            // 
            this.dteStart.EditValue = null;
            this.dteStart.Location = new System.Drawing.Point(118, 115);
            this.dteStart.Margin = new System.Windows.Forms.Padding(4);
            this.dteStart.Name = "dteStart";
            this.dteStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteStart.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteStart.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dteStart.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dteStart.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dteStart.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dteStart.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dteStart.Size = new System.Drawing.Size(307, 22);
            this.dteStart.StyleController = this.layoutControl3;
            this.dteStart.TabIndex = 5;
            // 
            // cboDepartment
            // 
            this.cboDepartment.Location = new System.Drawing.Point(118, 87);
            this.cboDepartment.Margin = new System.Windows.Forms.Padding(4);
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboDepartment.Properties.AutoComplete = false;
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboDepartment.Properties.View = this.customGridLookUpEditWithFilterMultiColumn2View;
            this.cboDepartment.Size = new System.Drawing.Size(307, 22);
            this.cboDepartment.StyleController = this.layoutControl3;
            this.cboDepartment.TabIndex = 4;
            this.cboDepartment.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDepartment_ButtonClick);
            this.cboDepartment.EditValueChanged += new System.EventHandler(this.cboDepartment_EditValueChanged);
            // 
            // customGridLookUpEditWithFilterMultiColumn2View
            // 
            this.customGridLookUpEditWithFilterMultiColumn2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridLookUpEditWithFilterMultiColumn2View.Name = "customGridLookUpEditWithFilterMultiColumn2View";
            this.customGridLookUpEditWithFilterMultiColumn2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridLookUpEditWithFilterMultiColumn2View.OptionsView.ShowGroupPanel = false;
            // 
            // cboPtttTemp_v45072
            // 
            this.cboPtttTemp_v45072.Location = new System.Drawing.Point(118, 3);
            this.cboPtttTemp_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.cboPtttTemp_v45072.Name = "cboPtttTemp_v45072";
            this.cboPtttTemp_v45072.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboPtttTemp_v45072.Properties.NullText = "";
            this.cboPtttTemp_v45072.Size = new System.Drawing.Size(251, 22);
            this.cboPtttTemp_v45072.StyleController = this.layoutControl3;
            this.cboPtttTemp_v45072.TabIndex = 18;
            // 
            // btnSavePtttTemp_v45072
            // 
            this.btnSavePtttTemp_v45072.Image = ((System.Drawing.Image)(resources.GetObject("btnSavePtttTemp_v45072.Image")));
            this.btnSavePtttTemp_v45072.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSavePtttTemp_v45072.Location = new System.Drawing.Point(375, 3);
            this.btnSavePtttTemp_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.btnSavePtttTemp_v45072.Name = "btnSavePtttTemp_v45072";
            this.btnSavePtttTemp_v45072.Size = new System.Drawing.Size(22, 20);
            this.btnSavePtttTemp_v45072.StyleController = this.layoutControl3;
            this.btnSavePtttTemp_v45072.TabIndex = 19;
            this.btnSavePtttTemp_v45072.ToolTip = "Lưu mẫu Phẫu thuật thủ thuật";
            // 
            // txtIcdCode_v45072
            // 
            this.txtIcdCode_v45072.Location = new System.Drawing.Point(118, 31);
            this.txtIcdCode_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.txtIcdCode_v45072.Name = "txtIcdCode_v45072";
            this.txtIcdCode_v45072.Size = new System.Drawing.Size(107, 22);
            this.txtIcdCode_v45072.StyleController = this.layoutControl3;
            this.txtIcdCode_v45072.TabIndex = 20;
            // 
            // cboIcdName_v45072
            // 
            this.cboIcdName_v45072.Location = new System.Drawing.Point(231, 31);
            this.cboIcdName_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.cboIcdName_v45072.Name = "cboIcdName_v45072";
            this.cboIcdName_v45072.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboIcdName_v45072.Properties.NullText = "";
            this.cboIcdName_v45072.Size = new System.Drawing.Size(94, 22);
            this.cboIcdName_v45072.StyleController = this.layoutControl3;
            this.cboIcdName_v45072.TabIndex = 21;
            // 
            // chkSuaIcd_v45072
            // 
            this.chkSuaIcd_v45072.Location = new System.Drawing.Point(331, 31);
            this.chkSuaIcd_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.chkSuaIcd_v45072.Name = "chkSuaIcd_v45072";
            this.chkSuaIcd_v45072.Properties.Caption = "Sửa";
            this.chkSuaIcd_v45072.Size = new System.Drawing.Size(94, 20);
            this.chkSuaIcd_v45072.StyleController = this.layoutControl3;
            this.chkSuaIcd_v45072.TabIndex = 22;
            // 
            // txtIcdSubCode_v45072
            // 
            this.txtIcdSubCode_v45072.Location = new System.Drawing.Point(546, 31);
            this.txtIcdSubCode_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.txtIcdSubCode_v45072.Name = "txtIcdSubCode_v45072";
            this.txtIcdSubCode_v45072.Properties.NullText = "Nhập mã bệnh phụ";
            this.txtIcdSubCode_v45072.Properties.NullValuePrompt = "Nhập mã bệnh phụ";
            this.txtIcdSubCode_v45072.Size = new System.Drawing.Size(134, 22);
            this.txtIcdSubCode_v45072.StyleController = this.layoutControl3;
            this.txtIcdSubCode_v45072.TabIndex = 23;
            // 
            // cboIcdText_v45072
            // 
            this.cboIcdText_v45072.Location = new System.Drawing.Point(686, 31);
            this.cboIcdText_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.cboIcdText_v45072.Name = "cboIcdText_v45072";
            this.cboIcdText_v45072.Properties.NullText = "";
            this.cboIcdText_v45072.Properties.NullValuePrompt = "Nhấn F1 để chọn bệnh phụ";
            this.cboIcdText_v45072.Properties.NullValuePromptShowForEmptyValue = true;
            this.cboIcdText_v45072.Size = new System.Drawing.Size(169, 22);
            this.cboIcdText_v45072.StyleController = this.layoutControl3;
            this.cboIcdText_v45072.TabIndex = 24;
            // 
            // txtIcdCmCode_v45072
            // 
            this.txtIcdCmCode_v45072.Location = new System.Drawing.Point(118, 59);
            this.txtIcdCmCode_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.txtIcdCmCode_v45072.Name = "txtIcdCmCode_v45072";
            this.txtIcdCmCode_v45072.Size = new System.Drawing.Size(107, 22);
            this.txtIcdCmCode_v45072.StyleController = this.layoutControl3;
            this.txtIcdCmCode_v45072.TabIndex = 25;
            // 
            // cboIcdCmName_v45072
            // 
            this.cboIcdCmName_v45072.Location = new System.Drawing.Point(231, 59);
            this.cboIcdCmName_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.cboIcdCmName_v45072.Name = "cboIcdCmName_v45072";
            this.cboIcdCmName_v45072.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboIcdCmName_v45072.Properties.NullText = "";
            this.cboIcdCmName_v45072.Size = new System.Drawing.Size(94, 22);
            this.cboIcdCmName_v45072.StyleController = this.layoutControl3;
            this.cboIcdCmName_v45072.TabIndex = 26;
            // 
            // chkSuaIcdCm_v45072
            // 
            this.chkSuaIcdCm_v45072.Location = new System.Drawing.Point(331, 59);
            this.chkSuaIcdCm_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.chkSuaIcdCm_v45072.Name = "chkSuaIcdCm_v45072";
            this.chkSuaIcdCm_v45072.Properties.Caption = "Sửa";
            this.chkSuaIcdCm_v45072.Size = new System.Drawing.Size(94, 20);
            this.chkSuaIcdCm_v45072.StyleController = this.layoutControl3;
            this.chkSuaIcdCm_v45072.TabIndex = 27;
            // 
            // txtIcdCmSubCode_v45072
            // 
            this.txtIcdCmSubCode_v45072.Location = new System.Drawing.Point(546, 59);
            this.txtIcdCmSubCode_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.txtIcdCmSubCode_v45072.Name = "txtIcdCmSubCode_v45072";
            this.txtIcdCmSubCode_v45072.Size = new System.Drawing.Size(134, 22);
            this.txtIcdCmSubCode_v45072.StyleController = this.layoutControl3;
            this.txtIcdCmSubCode_v45072.TabIndex = 28;
            // 
            // cboIcdCmText_v45072
            // 
            this.cboIcdCmText_v45072.Location = new System.Drawing.Point(686, 59);
            this.cboIcdCmText_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.cboIcdCmText_v45072.Name = "cboIcdCmText_v45072";
            this.cboIcdCmText_v45072.Properties.NullText = "";
            this.cboIcdCmText_v45072.Size = new System.Drawing.Size(169, 22);
            this.cboIcdCmText_v45072.StyleController = this.layoutControl3;
            this.cboIcdCmText_v45072.TabIndex = 29;
            // 
            // spnTimeProcess_v45072
            // 
            this.spnTimeProcess_v45072.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spnTimeProcess_v45072.Location = new System.Drawing.Point(546, 87);
            this.spnTimeProcess_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.spnTimeProcess_v45072.Name = "spnTimeProcess_v45072";
            this.spnTimeProcess_v45072.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spnTimeProcess_v45072.Properties.DisplayFormat.FormatString = "#,##0";
            this.spnTimeProcess_v45072.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.spnTimeProcess_v45072.Properties.EditFormat.FormatString = "#,##0";
            this.spnTimeProcess_v45072.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.spnTimeProcess_v45072.Properties.IsFloatValue = false;
            this.spnTimeProcess_v45072.Properties.MaxValue = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.spnTimeProcess_v45072.Size = new System.Drawing.Size(74, 22);
            this.spnTimeProcess_v45072.StyleController = this.layoutControl3;
            this.spnTimeProcess_v45072.TabIndex = 30;
            // 
            // lblPhut_v45072
            // 
            this.lblPhut_v45072.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblPhut_v45072.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPhut_v45072.Location = new System.Drawing.Point(626, 88);
            this.lblPhut_v45072.Name = "lblPhut_v45072";
            this.lblPhut_v45072.Size = new System.Drawing.Size(27, 16);
            this.lblPhut_v45072.StyleController = this.layoutControl3;
            this.lblPhut_v45072.TabIndex = 99;
            this.lblPhut_v45072.Text = "phút";
            // 
            // cboEmotionLess_v45072
            // 
            this.cboEmotionLess_v45072.Location = new System.Drawing.Point(118, 195);
            this.cboEmotionLess_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.cboEmotionLess_v45072.Name = "cboEmotionLess_v45072";
            this.cboEmotionLess_v45072.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboEmotionLess_v45072.Properties.NullText = "";
            this.cboEmotionLess_v45072.Size = new System.Drawing.Size(307, 22);
            this.cboEmotionLess_v45072.StyleController = this.layoutControl3;
            this.cboEmotionLess_v45072.TabIndex = 31;
            // 
            // txtManner_v45072
            // 
            this.txtManner_v45072.Location = new System.Drawing.Point(118, 223);
            this.txtManner_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.txtManner_v45072.Name = "txtManner_v45072";
            this.txtManner_v45072.Size = new System.Drawing.Size(307, 22);
            this.txtManner_v45072.StyleController = this.layoutControl3;
            this.txtManner_v45072.TabIndex = 32;
            // 
            // cboMachine_v45072
            // 
            this.cboMachine_v45072.Location = new System.Drawing.Point(546, 195);
            this.cboMachine_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.cboMachine_v45072.Name = "cboMachine_v45072";
            this.cboMachine_v45072.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboMachine_v45072.Properties.NullText = "";
            this.cboMachine_v45072.Size = new System.Drawing.Size(309, 22);
            this.cboMachine_v45072.StyleController = this.layoutControl3;
            this.cboMachine_v45072.TabIndex = 33;
            // 
            // txtConclude_v45072
            // 
            this.txtConclude_v45072.Location = new System.Drawing.Point(118, 251);
            this.txtConclude_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.txtConclude_v45072.Name = "txtConclude_v45072";
            this.txtConclude_v45072.Size = new System.Drawing.Size(307, 22);
            this.txtConclude_v45072.StyleController = this.layoutControl3;
            this.txtConclude_v45072.TabIndex = 35;
            // 
            // txtInstructionNote_v45072
            // 
            this.txtInstructionNote_v45072.Location = new System.Drawing.Point(118, 279);
            this.txtInstructionNote_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.txtInstructionNote_v45072.Name = "txtInstructionNote_v45072";
            this.txtInstructionNote_v45072.Size = new System.Drawing.Size(307, 22);
            this.txtInstructionNote_v45072.StyleController = this.layoutControl3;
            this.txtInstructionNote_v45072.TabIndex = 36;
            // 
            // tabDescription_v45072
            // 
            this.tabDescription_v45072.Location = new System.Drawing.Point(431, 223);
            this.tabDescription_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.tabDescription_v45072.Name = "tabDescription_v45072";
            this.tabDescription_v45072.SelectedTabPage = this.tabPageMoTa_v45072;
            this.tabDescription_v45072.Size = new System.Drawing.Size(424, 106);
            this.tabDescription_v45072.TabIndex = 37;
            this.tabDescription_v45072.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabPageMoTa_v45072,
            this.tabPageGhiChu_v45072});
            // 
            // tabPageMoTa_v45072
            // 
            this.tabPageMoTa_v45072.Controls.Add(this.txtDescription_v45072);
            this.tabPageMoTa_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageMoTa_v45072.Name = "tabPageMoTa_v45072";
            this.tabPageMoTa_v45072.Size = new System.Drawing.Size(417, 72);
            this.tabPageMoTa_v45072.Text = "Mô tả";
            // 
            // txtDescription_v45072
            // 
            this.txtDescription_v45072.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription_v45072.Location = new System.Drawing.Point(0, 0);
            this.txtDescription_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.txtDescription_v45072.Name = "txtDescription_v45072";
            this.txtDescription_v45072.Size = new System.Drawing.Size(417, 72);
            this.txtDescription_v45072.TabIndex = 0;
            // 
            // tabPageGhiChu_v45072
            // 
            this.tabPageGhiChu_v45072.Controls.Add(this.txtNote_v45072);
            this.tabPageGhiChu_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageGhiChu_v45072.Name = "tabPageGhiChu_v45072";
            this.tabPageGhiChu_v45072.Size = new System.Drawing.Size(417, 72);
            this.tabPageGhiChu_v45072.Text = "Ghi chú";
            // 
            // txtNote_v45072
            // 
            this.txtNote_v45072.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNote_v45072.Location = new System.Drawing.Point(0, 0);
            this.txtNote_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.txtNote_v45072.Name = "txtNote_v45072";
            this.txtNote_v45072.Size = new System.Drawing.Size(417, 72);
            this.txtNote_v45072.TabIndex = 0;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPtttTemp_v45072,
            this.lciIcdCode_v45072,
            this.lciIcdName_v45072,
            this.lciChkSuaIcd_v45072,
            this.lciIcdSubCode_v45072,
            this.lciIcdText_v45072,
            this.lciIcdCmCode_v45072,
            this.lciIcdCmName_v45072,
            this.lciChkSuaIcdCm_v45072,
            this.lciIcdCmSubCode_v45072,
            this.lciIcdCmText_v45072,
            this.lciTimeProcess_v45072,
            this.lciLblPhut_v45072,
            this.lciEmotionLess_v45072,
            // Việc 45072 — Lci cbo Vô cảm (bên phải txt code)
            this.lciCboEmotionLess_v45072,
            this.lciManner_v45072,
            this.lciMachine_v45072,
            this.lciConclude_v45072,
            this.lciInstructionNote_v45072,
            this.lciTabDescription_v45072,
            this.layoutControlItem21,
            this.layoutControlItem22,
            this.layoutControlItem23,
            this.layoutControlItem24,
            this.layoutControlItem25,
            this.layoutControlItem26,
            this.layoutControlItem27,
            this.layoutControlItem29,
            this.layoutControlItem28,
            this.layoutControlItem30,
            this.layoutControlItem31,
            this.layoutControlItem32,
            this.layoutControlItem33,
            this.emptySpaceItem4,
            this.lciBtnSavePtttTemp_v45072,
            this.emptySpaceItem5,
            this.emptySpaceItem6,
            this.layoutControlItem34});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Size = new System.Drawing.Size(858, 420);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // lciPtttTemp_v45072
            // 
            this.lciPtttTemp_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPtttTemp_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPtttTemp_v45072.Control = this.cboPtttTemp_v45072;
            this.lciPtttTemp_v45072.Location = new System.Drawing.Point(0, 0);
            this.lciPtttTemp_v45072.Name = "lciPtttTemp_v45072";
            this.lciPtttTemp_v45072.Size = new System.Drawing.Size(372, 28);
            this.lciPtttTemp_v45072.Text = "Mẫu PTTT:";
            this.lciPtttTemp_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPtttTemp_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciPtttTemp_v45072.TextToControlDistance = 5;
            // 
            // lciIcdCode_v45072
            // 
            this.lciIcdCode_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIcdCode_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIcdCode_v45072.Control = this.txtIcdCode_v45072;
            this.lciIcdCode_v45072.Location = new System.Drawing.Point(0, 28);
            this.lciIcdCode_v45072.Name = "lciIcdCode_v45072";
            this.lciIcdCode_v45072.Size = new System.Drawing.Size(228, 28);
            this.lciIcdCode_v45072.Text = "CĐ chính:";
            this.lciIcdCode_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIcdCode_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciIcdCode_v45072.TextToControlDistance = 5;
            // 
            // lciIcdName_v45072
            // 
            this.lciIcdName_v45072.Control = this.cboIcdName_v45072;
            this.lciIcdName_v45072.Location = new System.Drawing.Point(228, 28);
            this.lciIcdName_v45072.Name = "lciIcdName_v45072";
            this.lciIcdName_v45072.Size = new System.Drawing.Size(100, 28);
            this.lciIcdName_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciIcdName_v45072.TextVisible = false;
            // 
            // lciChkSuaIcd_v45072
            // 
            this.lciChkSuaIcd_v45072.Control = this.chkSuaIcd_v45072;
            this.lciChkSuaIcd_v45072.Location = new System.Drawing.Point(328, 28);
            this.lciChkSuaIcd_v45072.Name = "lciChkSuaIcd_v45072";
            this.lciChkSuaIcd_v45072.Size = new System.Drawing.Size(100, 28);
            this.lciChkSuaIcd_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciChkSuaIcd_v45072.TextVisible = false;
            // 
            // lciIcdSubCode_v45072
            // 
            this.lciIcdSubCode_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIcdSubCode_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIcdSubCode_v45072.Control = this.txtIcdSubCode_v45072;
            this.lciIcdSubCode_v45072.Location = new System.Drawing.Point(428, 28);
            this.lciIcdSubCode_v45072.Name = "lciIcdSubCode_v45072";
            this.lciIcdSubCode_v45072.Size = new System.Drawing.Size(255, 28);
            this.lciIcdSubCode_v45072.Text = "CĐ phụ:";
            this.lciIcdSubCode_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIcdSubCode_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciIcdSubCode_v45072.TextToControlDistance = 5;
            // 
            // lciIcdText_v45072
            // 
            this.lciIcdText_v45072.Control = this.cboIcdText_v45072;
            this.lciIcdText_v45072.Location = new System.Drawing.Point(683, 28);
            this.lciIcdText_v45072.Name = "lciIcdText_v45072";
            this.lciIcdText_v45072.Size = new System.Drawing.Size(175, 28);
            this.lciIcdText_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciIcdText_v45072.TextVisible = false;
            // 
            // lciIcdCmCode_v45072
            // 
            this.lciIcdCmCode_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIcdCmCode_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIcdCmCode_v45072.Control = this.txtIcdCmCode_v45072;
            this.lciIcdCmCode_v45072.Location = new System.Drawing.Point(0, 56);
            this.lciIcdCmCode_v45072.Name = "lciIcdCmCode_v45072";
            this.lciIcdCmCode_v45072.Size = new System.Drawing.Size(228, 28);
            this.lciIcdCmCode_v45072.Text = "ICD9-CM chính:";
            this.lciIcdCmCode_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIcdCmCode_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciIcdCmCode_v45072.TextToControlDistance = 5;
            // 
            // lciIcdCmName_v45072
            // 
            this.lciIcdCmName_v45072.Control = this.cboIcdCmName_v45072;
            this.lciIcdCmName_v45072.Location = new System.Drawing.Point(228, 56);
            this.lciIcdCmName_v45072.Name = "lciIcdCmName_v45072";
            this.lciIcdCmName_v45072.Size = new System.Drawing.Size(100, 28);
            this.lciIcdCmName_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciIcdCmName_v45072.TextVisible = false;
            // 
            // lciChkSuaIcdCm_v45072
            // 
            this.lciChkSuaIcdCm_v45072.Control = this.chkSuaIcdCm_v45072;
            this.lciChkSuaIcdCm_v45072.Location = new System.Drawing.Point(328, 56);
            this.lciChkSuaIcdCm_v45072.Name = "lciChkSuaIcdCm_v45072";
            this.lciChkSuaIcdCm_v45072.Size = new System.Drawing.Size(100, 28);
            this.lciChkSuaIcdCm_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciChkSuaIcdCm_v45072.TextVisible = false;
            // 
            // lciIcdCmSubCode_v45072
            // 
            this.lciIcdCmSubCode_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIcdCmSubCode_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIcdCmSubCode_v45072.Control = this.txtIcdCmSubCode_v45072;
            this.lciIcdCmSubCode_v45072.Location = new System.Drawing.Point(428, 56);
            this.lciIcdCmSubCode_v45072.Name = "lciIcdCmSubCode_v45072";
            this.lciIcdCmSubCode_v45072.Size = new System.Drawing.Size(255, 28);
            this.lciIcdCmSubCode_v45072.Text = "ICD9-CM phụ:";
            this.lciIcdCmSubCode_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIcdCmSubCode_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciIcdCmSubCode_v45072.TextToControlDistance = 5;
            // 
            // lciIcdCmText_v45072
            // 
            this.lciIcdCmText_v45072.Control = this.cboIcdCmText_v45072;
            this.lciIcdCmText_v45072.Location = new System.Drawing.Point(683, 56);
            this.lciIcdCmText_v45072.Name = "lciIcdCmText_v45072";
            this.lciIcdCmText_v45072.Size = new System.Drawing.Size(175, 28);
            this.lciIcdCmText_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciIcdCmText_v45072.TextVisible = false;
            // 
            // lciTimeProcess_v45072
            // 
            this.lciTimeProcess_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTimeProcess_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTimeProcess_v45072.Control = this.spnTimeProcess_v45072;
            this.lciTimeProcess_v45072.Location = new System.Drawing.Point(428, 84);
            this.lciTimeProcess_v45072.Name = "lciTimeProcess_v45072";
            this.lciTimeProcess_v45072.Size = new System.Drawing.Size(195, 28);
            this.lciTimeProcess_v45072.Text = "TG xử lý:";
            this.lciTimeProcess_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTimeProcess_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciTimeProcess_v45072.TextToControlDistance = 5;
            // 
            // lciLblPhut_v45072
            // 
            this.lciLblPhut_v45072.Control = this.lblPhut_v45072;
            this.lciLblPhut_v45072.Location = new System.Drawing.Point(623, 84);
            this.lciLblPhut_v45072.Name = "lciLblPhut_v45072";
            this.lciLblPhut_v45072.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 0, 4, 0);
            this.lciLblPhut_v45072.Size = new System.Drawing.Size(30, 28);
            this.lciLblPhut_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciLblPhut_v45072.TextVisible = false;
            // 
            // lciEmotionLess_v45072 — Việc 45072 (y/c TuanLN): tách thành 2 cell giống Phương pháp TT
            // Cell trái = caption + txt code (167w), cell phải = cbo name (261w)
            this.lciEmotionLess_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciEmotionLess_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciEmotionLess_v45072.Control = this.txtEmotionLessCode_v45072;
            this.lciEmotionLess_v45072.Location = new System.Drawing.Point(0, 192);
            this.lciEmotionLess_v45072.Name = "lciEmotionLess_v45072";
            this.lciEmotionLess_v45072.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciEmotionLess_v45072.Size = new System.Drawing.Size(167, 28);
            this.lciEmotionLess_v45072.Text = "Vô cảm:";
            this.lciEmotionLess_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciEmotionLess_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciEmotionLess_v45072.TextToControlDistance = 5;
            //
            // lciCboEmotionLess_v45072 — cell phải chứa combo Vô cảm
            this.lciCboEmotionLess_v45072.Control = this.cboEmotionLess_v45072;
            this.lciCboEmotionLess_v45072.Location = new System.Drawing.Point(167, 192);
            this.lciCboEmotionLess_v45072.Name = "lciCboEmotionLess_v45072";
            this.lciCboEmotionLess_v45072.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciCboEmotionLess_v45072.Size = new System.Drawing.Size(261, 28);
            this.lciCboEmotionLess_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciCboEmotionLess_v45072.TextVisible = false;
            //
            // txtEmotionLessCode_v45072 — textbox hiển thị mã Vô cảm
            this.txtEmotionLessCode_v45072.Name = "txtEmotionLessCode_v45072";
            this.txtEmotionLessCode_v45072.StyleController = this.layoutControl3;
            this.txtEmotionLessCode_v45072.TabIndex = 200;
            // 
            // lciManner_v45072
            // 
            this.lciManner_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciManner_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciManner_v45072.Control = this.txtManner_v45072;
            this.lciManner_v45072.Location = new System.Drawing.Point(0, 220);
            this.lciManner_v45072.Name = "lciManner_v45072";
            this.lciManner_v45072.Size = new System.Drawing.Size(428, 28);
            this.lciManner_v45072.Text = "Cách thức:";
            this.lciManner_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciManner_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciManner_v45072.TextToControlDistance = 5;
            // 
            // lciMachine_v45072
            // 
            this.lciMachine_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMachine_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMachine_v45072.Control = this.cboMachine_v45072;
            this.lciMachine_v45072.Location = new System.Drawing.Point(428, 192);
            this.lciMachine_v45072.Name = "lciMachine_v45072";
            this.lciMachine_v45072.Size = new System.Drawing.Size(430, 28);
            this.lciMachine_v45072.Text = "Máy thực hiện:";
            this.lciMachine_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMachine_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciMachine_v45072.TextToControlDistance = 5;
            // 
            // lciConclude_v45072
            // 
            this.lciConclude_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConclude_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConclude_v45072.Control = this.txtConclude_v45072;
            this.lciConclude_v45072.Location = new System.Drawing.Point(0, 248);
            this.lciConclude_v45072.Name = "lciConclude_v45072";
            this.lciConclude_v45072.Size = new System.Drawing.Size(428, 28);
            this.lciConclude_v45072.Text = "Kết luận:";
            this.lciConclude_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConclude_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciConclude_v45072.TextToControlDistance = 5;
            // 
            // lciInstructionNote_v45072
            // 
            this.lciInstructionNote_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciInstructionNote_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciInstructionNote_v45072.Control = this.txtInstructionNote_v45072;
            this.lciInstructionNote_v45072.Location = new System.Drawing.Point(0, 276);
            this.lciInstructionNote_v45072.Name = "lciInstructionNote_v45072";
            this.lciInstructionNote_v45072.Size = new System.Drawing.Size(428, 28);
            this.lciInstructionNote_v45072.Text = "Ghi chú BSCĐ:";
            this.lciInstructionNote_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciInstructionNote_v45072.TextSize = new System.Drawing.Size(110, 20);
            this.lciInstructionNote_v45072.TextToControlDistance = 5;
            // 
            // lciTabDescription_v45072
            // 
            this.lciTabDescription_v45072.Control = this.tabDescription_v45072;
            this.lciTabDescription_v45072.Location = new System.Drawing.Point(428, 220);
            this.lciTabDescription_v45072.Name = "lciTabDescription_v45072";
            this.lciTabDescription_v45072.Size = new System.Drawing.Size(430, 112);
            this.lciTabDescription_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciTabDescription_v45072.TextVisible = false;
            // 
            // layoutControlItem21
            // 
            this.layoutControlItem21.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem21.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem21.Control = this.cboDepartment;
            this.layoutControlItem21.Location = new System.Drawing.Point(0, 84);
            this.layoutControlItem21.Name = "layoutControlItem21";
            this.layoutControlItem21.Size = new System.Drawing.Size(428, 28);
            this.layoutControlItem21.Text = "Khoa:";
            this.layoutControlItem21.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem21.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem21.TextToControlDistance = 5;
            // 
            // layoutControlItem22
            // 
            this.layoutControlItem22.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem22.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem22.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem22.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem22.Control = this.dteStart;
            this.layoutControlItem22.Location = new System.Drawing.Point(0, 112);
            this.layoutControlItem22.Name = "layoutControlItem22";
            this.layoutControlItem22.Size = new System.Drawing.Size(428, 28);
            this.layoutControlItem22.Text = "Bắt đầu:";
            this.layoutControlItem22.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem22.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem22.TextToControlDistance = 5;
            // 
            // layoutControlItem23
            // 
            this.layoutControlItem23.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem23.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem23.Control = this.dteFinish;
            this.layoutControlItem23.Location = new System.Drawing.Point(428, 112);
            this.layoutControlItem23.Name = "layoutControlItem23";
            this.layoutControlItem23.Size = new System.Drawing.Size(430, 28);
            this.layoutControlItem23.Text = "Kết thúc:";
            this.layoutControlItem23.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem23.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem23.TextToControlDistance = 5;
            // 
            // layoutControlItem24
            // 
            this.layoutControlItem24.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem24.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem24.Control = this.txtPtttMethod;
            this.layoutControlItem24.Location = new System.Drawing.Point(0, 140);
            this.layoutControlItem24.Name = "layoutControlItem24";
            this.layoutControlItem24.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem24.Size = new System.Drawing.Size(167, 26);
            this.layoutControlItem24.Text = "Phương pháp:";
            this.layoutControlItem24.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem24.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem24.TextToControlDistance = 5;
            // 
            // layoutControlItem25
            // 
            this.layoutControlItem25.Control = this.cboPtttMethod;
            this.layoutControlItem25.Location = new System.Drawing.Point(167, 140);
            this.layoutControlItem25.Name = "layoutControlItem25";
            this.layoutControlItem25.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem25.Size = new System.Drawing.Size(261, 26);
            this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem25.TextVisible = false;
            // 
            // layoutControlItem26
            // 
            this.layoutControlItem26.Control = this.cboEmotionLessMethod;
            this.layoutControlItem26.Location = new System.Drawing.Point(595, 140);
            this.layoutControlItem26.Name = "layoutControlItem26";
            this.layoutControlItem26.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem26.Size = new System.Drawing.Size(263, 26);
            this.layoutControlItem26.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem26.TextVisible = false;
            // 
            // layoutControlItem27
            // 
            this.layoutControlItem27.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem27.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem27.Control = this.txtEmotionLessMethod;
            this.layoutControlItem27.Location = new System.Drawing.Point(428, 140);
            this.layoutControlItem27.Name = "layoutControlItem27";
            this.layoutControlItem27.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem27.Size = new System.Drawing.Size(167, 26);
            this.layoutControlItem27.Text = "Phương pháp 2:";
            this.layoutControlItem27.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem27.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem27.TextToControlDistance = 5;
            // 
            // layoutControlItem29
            // 
            this.layoutControlItem29.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem29.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem29.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem29.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem29.Control = this.txtPtttGroup;
            this.layoutControlItem29.Location = new System.Drawing.Point(428, 166);
            this.layoutControlItem29.Name = "layoutControlItem29";
            this.layoutControlItem29.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem29.Size = new System.Drawing.Size(167, 26);
            this.layoutControlItem29.Text = "Phân loại:";
            this.layoutControlItem29.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem29.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem29.TextToControlDistance = 5;
            // 
            // layoutControlItem28
            // 
            this.layoutControlItem28.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem28.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem28.Control = this.txtPtttMethodReal;
            this.layoutControlItem28.Location = new System.Drawing.Point(0, 166);
            this.layoutControlItem28.Name = "layoutControlItem28";
            this.layoutControlItem28.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem28.Size = new System.Drawing.Size(167, 26);
            this.layoutControlItem28.Text = "Phương pháp TT:";
            this.layoutControlItem28.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem28.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem28.TextToControlDistance = 5;
            // 
            // layoutControlItem30
            // 
            this.layoutControlItem30.Control = this.cboPtttMethodReal;
            this.layoutControlItem30.Location = new System.Drawing.Point(167, 166);
            this.layoutControlItem30.Name = "layoutControlItem30";
            this.layoutControlItem30.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem30.Size = new System.Drawing.Size(261, 26);
            this.layoutControlItem30.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem30.TextVisible = false;
            // 
            // layoutControlItem31
            // 
            this.layoutControlItem31.Control = this.cboPtttGroup;
            this.layoutControlItem31.Location = new System.Drawing.Point(595, 166);
            this.layoutControlItem31.Name = "layoutControlItem31";
            this.layoutControlItem31.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem31.Size = new System.Drawing.Size(263, 26);
            this.layoutControlItem31.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem31.TextVisible = false;
            // 
            // layoutControlItem32
            // 
            this.layoutControlItem32.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem32.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem32.Control = this.cboEkipUser;
            this.layoutControlItem32.Location = new System.Drawing.Point(0, 304);
            this.layoutControlItem32.Name = "layoutControlItem32";
            this.layoutControlItem32.Size = new System.Drawing.Size(400, 28);
            this.layoutControlItem32.Text = "Kíp mẫu:";
            this.layoutControlItem32.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem32.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem32.TextToControlDistance = 5;
            // 
            // layoutControlItem33
            // 
            this.layoutControlItem33.Control = this.btnSaveEkip;
            this.layoutControlItem33.Location = new System.Drawing.Point(400, 304);
            this.layoutControlItem33.MaxSize = new System.Drawing.Size(28, 26);
            this.layoutControlItem33.MinSize = new System.Drawing.Size(28, 26);
            this.layoutControlItem33.Name = "layoutControlItem33";
            this.layoutControlItem33.Size = new System.Drawing.Size(28, 28);
            this.layoutControlItem33.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem33.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem33.TextVisible = false;
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.Location = new System.Drawing.Point(400, 0);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(458, 28);
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciBtnSavePtttTemp_v45072
            // 
            this.lciBtnSavePtttTemp_v45072.Control = this.btnSavePtttTemp_v45072;
            this.lciBtnSavePtttTemp_v45072.Location = new System.Drawing.Point(372, 0);
            this.lciBtnSavePtttTemp_v45072.MaxSize = new System.Drawing.Size(28, 26);
            this.lciBtnSavePtttTemp_v45072.MinSize = new System.Drawing.Size(28, 26);
            this.lciBtnSavePtttTemp_v45072.Name = "lciBtnSavePtttTemp_v45072";
            this.lciBtnSavePtttTemp_v45072.Size = new System.Drawing.Size(28, 28);
            this.lciBtnSavePtttTemp_v45072.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnSavePtttTemp_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSavePtttTemp_v45072.TextVisible = false;
            // 
            // emptySpaceItem5
            // 
            this.emptySpaceItem5.AllowHotTrack = false;
            this.emptySpaceItem5.Location = new System.Drawing.Point(653, 84);
            this.emptySpaceItem5.Name = "emptySpaceItem5";
            this.emptySpaceItem5.Size = new System.Drawing.Size(205, 28);
            this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem6
            // 
            this.emptySpaceItem6.AllowHotTrack = false;
            this.emptySpaceItem6.Location = new System.Drawing.Point(848, 332);
            this.emptySpaceItem6.Name = "emptySpaceItem6";
            this.emptySpaceItem6.Size = new System.Drawing.Size(10, 88);
            this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem34
            // 
            this.layoutControlItem34.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem34.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem34.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem34.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem34.Control = this.grdControlInformationSurg;
            this.layoutControlItem34.Location = new System.Drawing.Point(0, 332);
            this.layoutControlItem34.Name = "layoutControlItem34";
            this.layoutControlItem34.Size = new System.Drawing.Size(848, 88);
            this.layoutControlItem34.Text = "Kíp thực hiện:";
            this.layoutControlItem34.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem34.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem34.TextToControlDistance = 5;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.layoutControl2);
            this.groupControl1.Location = new System.Drawing.Point(783, 3);
            this.groupControl1.Margin = new System.Windows.Forms.Padding(4);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(862, 137);
            this.groupControl1.TabIndex = 12;
            this.groupControl1.Text = "Thông tin bệnh nhân";
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.lblNote);
            this.layoutControl2.Controls.Add(this.lblType);
            this.layoutControl2.Controls.Add(this.lblHeinCardFromTo);
            this.layoutControl2.Controls.Add(this.lblAddress);
            this.layoutControl2.Controls.Add(this.lblKCBBD);
            this.layoutControl2.Controls.Add(this.lblHeinCardNumber);
            this.layoutControl2.Controls.Add(this.lblGender);
            this.layoutControl2.Controls.Add(this.lblPatientDob);
            this.layoutControl2.Controls.Add(this.lblPatientName);
            this.layoutControl2.Controls.Add(this.lblPatientCode);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(2, 25);
            this.layoutControl2.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(858, 110);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // lblNote
            // 
            this.lblNote.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblNote.Location = new System.Drawing.Point(117, 82);
            this.lblNote.Margin = new System.Windows.Forms.Padding(4);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(739, 20);
            this.lblNote.StyleController = this.layoutControl2;
            this.lblNote.TabIndex = 22;
            // 
            // lblType
            // 
            this.lblType.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblType.Location = new System.Drawing.Point(721, 55);
            this.lblType.Margin = new System.Windows.Forms.Padding(4);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(134, 20);
            this.lblType.StyleController = this.layoutControl2;
            this.lblType.TabIndex = 21;
            this.lblType.Text = " ";
            // 
            // lblHeinCardFromTo
            // 
            this.lblHeinCardFromTo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHeinCardFromTo.Location = new System.Drawing.Point(420, 55);
            this.lblHeinCardFromTo.Margin = new System.Windows.Forms.Padding(4);
            this.lblHeinCardFromTo.Name = "lblHeinCardFromTo";
            this.lblHeinCardFromTo.Size = new System.Drawing.Size(220, 20);
            this.lblHeinCardFromTo.StyleController = this.layoutControl2;
            this.lblHeinCardFromTo.TabIndex = 20;
            this.lblHeinCardFromTo.Text = " ";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAddress.Location = new System.Drawing.Point(118, 55);
            this.lblAddress.Margin = new System.Windows.Forms.Padding(4);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(201, 20);
            this.lblAddress.StyleController = this.layoutControl2;
            this.lblAddress.TabIndex = 19;
            this.lblAddress.Text = " ";
            // 
            // lblKCBBD
            // 
            this.lblKCBBD.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblKCBBD.Location = new System.Drawing.Point(721, 29);
            this.lblKCBBD.Margin = new System.Windows.Forms.Padding(4);
            this.lblKCBBD.Name = "lblKCBBD";
            this.lblKCBBD.Size = new System.Drawing.Size(134, 20);
            this.lblKCBBD.StyleController = this.layoutControl2;
            this.lblKCBBD.TabIndex = 18;
            this.lblKCBBD.Text = " ";
            // 
            // lblHeinCardNumber
            // 
            this.lblHeinCardNumber.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHeinCardNumber.Location = new System.Drawing.Point(420, 29);
            this.lblHeinCardNumber.Margin = new System.Windows.Forms.Padding(4);
            this.lblHeinCardNumber.Name = "lblHeinCardNumber";
            this.lblHeinCardNumber.Size = new System.Drawing.Size(220, 20);
            this.lblHeinCardNumber.StyleController = this.layoutControl2;
            this.lblHeinCardNumber.TabIndex = 17;
            this.lblHeinCardNumber.Text = " ";
            // 
            // lblGender
            // 
            this.lblGender.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGender.Location = new System.Drawing.Point(118, 29);
            this.lblGender.Margin = new System.Windows.Forms.Padding(4);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(201, 20);
            this.lblGender.StyleController = this.layoutControl2;
            this.lblGender.TabIndex = 16;
            this.lblGender.Text = " ";
            // 
            // lblPatientDob
            // 
            this.lblPatientDob.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientDob.Location = new System.Drawing.Point(721, 3);
            this.lblPatientDob.Margin = new System.Windows.Forms.Padding(4);
            this.lblPatientDob.Name = "lblPatientDob";
            this.lblPatientDob.Size = new System.Drawing.Size(134, 20);
            this.lblPatientDob.StyleController = this.layoutControl2;
            this.lblPatientDob.TabIndex = 15;
            this.lblPatientDob.Text = " ";
            // 
            // lblPatientName
            // 
            this.lblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientName.Location = new System.Drawing.Point(420, 3);
            this.lblPatientName.Margin = new System.Windows.Forms.Padding(4);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(220, 20);
            this.lblPatientName.StyleController = this.layoutControl2;
            this.lblPatientName.TabIndex = 14;
            this.lblPatientName.Text = " ";
            // 
            // lblPatientCode
            // 
            this.lblPatientCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientCode.Location = new System.Drawing.Point(118, 3);
            this.lblPatientCode.Margin = new System.Windows.Forms.Padding(4);
            this.lblPatientCode.Name = "lblPatientCode";
            this.lblPatientCode.Size = new System.Drawing.Size(201, 20);
            this.lblPatientCode.StyleController = this.layoutControl2;
            this.lblPatientCode.TabIndex = 13;
            this.lblPatientCode.Text = " ";
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem20,
            this.layoutControlItem11,
            this.layoutControlItem12,
            this.layoutControlItem13,
            this.layoutControlItem14,
            this.layoutControlItem15,
            this.layoutControlItem16,
            this.layoutControlItem17,
            this.layoutControlItem18,
            this.layoutControlItem19});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Size = new System.Drawing.Size(858, 110);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem20.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem20.Control = this.lblPatientCode;
            this.layoutControlItem20.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.Size = new System.Drawing.Size(322, 26);
            this.layoutControlItem20.Text = "Mã bệnh nhân:";
            this.layoutControlItem20.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem20.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem20.TextToControlDistance = 5;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem11.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem11.Control = this.lblPatientName;
            this.layoutControlItem11.Location = new System.Drawing.Point(322, 0);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(321, 26);
            this.layoutControlItem11.Text = "Tên bệnh nhân:";
            this.layoutControlItem11.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem11.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem11.TextToControlDistance = 5;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem12.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem12.Control = this.lblPatientDob;
            this.layoutControlItem12.Location = new System.Drawing.Point(643, 0);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(215, 26);
            this.layoutControlItem12.Text = "Ngày sinh:";
            this.layoutControlItem12.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem12.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem12.TextToControlDistance = 5;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem13.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem13.Control = this.lblGender;
            this.layoutControlItem13.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(322, 26);
            this.layoutControlItem13.Text = "Giới tính:";
            this.layoutControlItem13.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem13.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem13.TextToControlDistance = 5;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem14.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem14.Control = this.lblHeinCardNumber;
            this.layoutControlItem14.Location = new System.Drawing.Point(322, 26);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(321, 26);
            this.layoutControlItem14.Text = "Số thẻ:";
            this.layoutControlItem14.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem14.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem14.TextToControlDistance = 5;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem15.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem15.Control = this.lblKCBBD;
            this.layoutControlItem15.Location = new System.Drawing.Point(643, 26);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.OptionsToolTip.ToolTip = "Khoa khám chữa bệnh ban đầu";
            this.layoutControlItem15.Size = new System.Drawing.Size(215, 26);
            this.layoutControlItem15.Text = "KCBBD:";
            this.layoutControlItem15.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem15.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem15.TextToControlDistance = 5;
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem16.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem16.Control = this.lblAddress;
            this.layoutControlItem16.Location = new System.Drawing.Point(0, 52);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(322, 26);
            this.layoutControlItem16.Text = "Địa chỉ";
            this.layoutControlItem16.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem16.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem16.TextToControlDistance = 5;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem17.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem17.Control = this.lblHeinCardFromTo;
            this.layoutControlItem17.Location = new System.Drawing.Point(322, 52);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Size = new System.Drawing.Size(321, 26);
            this.layoutControlItem17.Text = "Hạn thẻ:";
            this.layoutControlItem17.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem17.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem17.TextToControlDistance = 5;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem18.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem18.Control = this.lblType;
            this.layoutControlItem18.Location = new System.Drawing.Point(643, 52);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.Size = new System.Drawing.Size(215, 26);
            this.layoutControlItem18.Text = "Loại:";
            this.layoutControlItem18.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem18.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem18.TextToControlDistance = 5;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem19.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem19.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.layoutControlItem19.Control = this.lblNote;
            this.layoutControlItem19.Location = new System.Drawing.Point(0, 78);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 4, 2);
            this.layoutControlItem19.Size = new System.Drawing.Size(858, 32);
            this.layoutControlItem19.Text = "Ghi chú:";
            this.layoutControlItem19.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem19.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem19.TextToControlDistance = 5;
            // 
            // gridControl1
            // 
            this.gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridControl1.Location = new System.Drawing.Point(3, 64);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Margin = new System.Windows.Forms.Padding(4);
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repStt});
            this.gridControl1.Size = new System.Drawing.Size(774, 529);
            this.gridControl1.TabIndex = 11;
            this.gridControl1.ToolTipController = this.toolTipControllerGrid;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumnPatientType_v45072,
            this.gridColumnRequestDoctor_v45072,
            this.gridColumnBeginTime_v45072,
            this.gridColumnEndTime_v45072,
            this.gridColumnPrice_v45072,
            this.gridColumn12});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.GroupCount = 1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.AutoExpandAllGroups = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            // Việc 45072 — TuanLN báo: cần thanh scroll ngang vì nhiều cột bổ sung (ĐTTT, BS, BĐ, KT, Đơn giá)
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowHorzLines = true;
            this.gridView1.OptionsView.ShowVertLines = true;
            this.gridView1.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn5, DevExpress.Data.ColumnSortOrder.Descending)});
            this.gridView1.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridView1_RowCellClick);
            this.gridView1.CustomDrawGroupRow += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.gridView1_CustomDrawGroupRow);
            this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "gridColumn1";
            this.gridColumn1.ColumnEdit = this.repStt;
            this.gridColumn1.FieldName = "TRANGTHAI_IMG";
            this.gridColumn1.MaxWidth = 25;
            this.gridColumn1.MinWidth = 25;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.OptionsColumn.ShowCaption = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 46;
            // 
            // repStt
            // 
            this.repStt.Name = "repStt";
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Mã y lệnh";
            this.gridColumn2.FieldName = "TDL_SERVICE_REQ_CODE";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 96;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Tên dịch vụ";
            this.gridColumn3.FieldName = "TDL_SERVICE_NAME";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 337;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Ngày chỉ định";
            this.gridColumn4.FieldName = "INTRUCTION_TIME_str";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 100;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Họ tên";
            this.gridColumn5.FieldName = "TDL_PATIENT_CODE";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // gridColumnPatientType_v45072
            // 
            this.gridColumnPatientType_v45072.Caption = "ĐTTT";
            this.gridColumnPatientType_v45072.FieldName = "PATIENT_TYPE_NAME";
            this.gridColumnPatientType_v45072.Name = "gridColumnPatientType_v45072";
            this.gridColumnPatientType_v45072.OptionsColumn.AllowEdit = false;
            this.gridColumnPatientType_v45072.Visible = true;
            this.gridColumnPatientType_v45072.VisibleIndex = 4;
            this.gridColumnPatientType_v45072.Width = 100;
            // 
            // gridColumnRequestDoctor_v45072
            // 
            this.gridColumnRequestDoctor_v45072.Caption = "Bác sĩ chỉ định";
            this.gridColumnRequestDoctor_v45072.FieldName = "REQUEST_DOCTOR_DISPLAY";
            this.gridColumnRequestDoctor_v45072.Name = "gridColumnRequestDoctor_v45072";
            this.gridColumnRequestDoctor_v45072.OptionsColumn.AllowEdit = false;
            this.gridColumnRequestDoctor_v45072.Visible = true;
            this.gridColumnRequestDoctor_v45072.VisibleIndex = 5;
            this.gridColumnRequestDoctor_v45072.Width = 150;
            // 
            // gridColumnBeginTime_v45072
            // 
            this.gridColumnBeginTime_v45072.Caption = "Thời gian bắt đầu";
            this.gridColumnBeginTime_v45072.FieldName = "BEGIN_TIME_STR";
            this.gridColumnBeginTime_v45072.Name = "gridColumnBeginTime_v45072";
            this.gridColumnBeginTime_v45072.OptionsColumn.AllowEdit = false;
            this.gridColumnBeginTime_v45072.Visible = true;
            this.gridColumnBeginTime_v45072.VisibleIndex = 6;
            this.gridColumnBeginTime_v45072.Width = 120;
            // 
            // gridColumnEndTime_v45072
            // 
            this.gridColumnEndTime_v45072.Caption = "Thời gian kết thúc";
            this.gridColumnEndTime_v45072.FieldName = "END_TIME_STR";
            this.gridColumnEndTime_v45072.Name = "gridColumnEndTime_v45072";
            this.gridColumnEndTime_v45072.OptionsColumn.AllowEdit = false;
            this.gridColumnEndTime_v45072.Visible = true;
            this.gridColumnEndTime_v45072.VisibleIndex = 7;
            this.gridColumnEndTime_v45072.Width = 120;
            // 
            // gridColumnPrice_v45072
            // 
            this.gridColumnPrice_v45072.Caption = "Đơn giá";
            this.gridColumnPrice_v45072.DisplayFormat.FormatString = "#,##0";
            this.gridColumnPrice_v45072.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnPrice_v45072.FieldName = "PRICE_V45072";
            this.gridColumnPrice_v45072.Name = "gridColumnPrice_v45072";
            this.gridColumnPrice_v45072.OptionsColumn.AllowEdit = false;
            this.gridColumnPrice_v45072.Visible = true;
            this.gridColumnPrice_v45072.VisibleIndex = 8;
            this.gridColumnPrice_v45072.Width = 100;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "gridColumn12";
            this.gridColumn12.FieldName = "GroupFieldName";
            this.gridColumn12.Name = "gridColumn12";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(615, 31);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(162, 27);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(248, 31);
            this.txtFind.Margin = new System.Windows.Forms.Padding(4);
            this.txtFind.Name = "txtFind";
            this.txtFind.Properties.NullValuePrompt = "Nhập từ khóa tìm kiếm";
            this.txtFind.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtFind.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtFind.Size = new System.Drawing.Size(361, 22);
            this.txtFind.StyleController = this.layoutControl1;
            this.txtFind.TabIndex = 9;
            // 
            // txtPatientCode
            // 
            this.txtPatientCode.Location = new System.Drawing.Point(3, 31);
            this.txtPatientCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân";
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtPatientCode.Size = new System.Drawing.Size(239, 22);
            this.txtPatientCode.StyleController = this.layoutControl1;
            this.txtPatientCode.TabIndex = 8;
            this.txtPatientCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientCode_PreviewKeyDown);
            // 
            // cboStt
            // 
            this.cboStt.Location = new System.Drawing.Point(615, 3);
            this.cboStt.Margin = new System.Windows.Forms.Padding(4);
            this.cboStt.Name = "cboStt";
            this.cboStt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStt.Properties.Items.AddRange(new object[] {
            "Tất cả",
            "Chưa xử lý",
            "Đang xử lý",
            "Hoàn thành"});
            this.cboStt.Size = new System.Drawing.Size(162, 22);
            this.cboStt.StyleController = this.layoutControl1;
            this.cboStt.TabIndex = 7;
            // 
            // dteTo
            // 
            this.dteTo.EditValue = null;
            this.dteTo.Location = new System.Drawing.Point(248, 3);
            this.dteTo.Margin = new System.Windows.Forms.Padding(4);
            this.dteTo.Name = "dteTo";
            this.dteTo.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dteTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dteTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dteTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dteTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dteTo.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dteTo.Size = new System.Drawing.Size(155, 22);
            this.dteTo.StyleController = this.layoutControl1;
            this.dteTo.TabIndex = 5;
            // 
            // dteFrom
            // 
            this.dteFrom.EditValue = null;
            this.dteFrom.Location = new System.Drawing.Point(78, 3);
            this.dteFrom.Margin = new System.Windows.Forms.Padding(4);
            this.dteFrom.Name = "dteFrom";
            this.dteFrom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dteFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dteFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dteFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dteFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dteFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dteFrom.Size = new System.Drawing.Size(164, 22);
            this.dteFrom.StyleController = this.layoutControl1;
            this.dteFrom.TabIndex = 4;
            // 
            // cboService
            // 
            this.cboService.Location = new System.Drawing.Point(409, 3);
            this.cboService.Margin = new System.Windows.Forms.Padding(4);
            this.cboService.Name = "cboService";
            this.cboService.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboService.Properties.AutoComplete = false;
            this.cboService.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboService.Properties.NullText = "";
            this.cboService.Properties.NullValuePrompt = "Dịch vụ";
            this.cboService.Properties.NullValuePromptShowForEmptyValue = true;
            this.cboService.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboService.Properties.View = this.customGridLookUpEdit1View;
            this.cboService.Size = new System.Drawing.Size(200, 22);
            this.cboService.StyleController = this.layoutControl1;
            this.cboService.TabIndex = 6;
            this.cboService.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboService_CustomDisplayText);
            // 
            // customGridLookUpEdit1View
            // 
            this.customGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridLookUpEdit1View.Name = "customGridLookUpEdit1View";
            this.customGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // lblTotalPatient_v45072
            // 
            this.lblTotalPatient_v45072.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblTotalPatient_v45072.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalPatient_v45072.Location = new System.Drawing.Point(3, 599);
            this.lblTotalPatient_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.lblTotalPatient_v45072.Name = "lblTotalPatient_v45072";
            this.lblTotalPatient_v45072.Size = new System.Drawing.Size(173, 27);
            this.lblTotalPatient_v45072.StyleController = this.layoutControl1;
            this.lblTotalPatient_v45072.TabIndex = 15;
            this.lblTotalPatient_v45072.Text = "Tổng số BN:";
            // 
            // lblTotalService_v45072
            // 
            this.lblTotalService_v45072.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblTotalService_v45072.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalService_v45072.Location = new System.Drawing.Point(182, 599);
            this.lblTotalService_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.lblTotalService_v45072.Name = "lblTotalService_v45072";
            this.lblTotalService_v45072.Size = new System.Drawing.Size(174, 27);
            this.lblTotalService_v45072.StyleController = this.layoutControl1;
            this.lblTotalService_v45072.TabIndex = 16;
            this.lblTotalService_v45072.Text = "Tổng số dịch vụ:";
            // 
            // btnDanhSachYLenh_v45072
            // 
            this.btnDanhSachYLenh_v45072.Location = new System.Drawing.Point(615, 599);
            this.btnDanhSachYLenh_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.btnDanhSachYLenh_v45072.Name = "btnDanhSachYLenh_v45072";
            this.btnDanhSachYLenh_v45072.Size = new System.Drawing.Size(162, 27);
            this.btnDanhSachYLenh_v45072.StyleController = this.layoutControl1;
            this.btnDanhSachYLenh_v45072.TabIndex = 17;
            this.btnDanhSachYLenh_v45072.Text = "Danh sách y lệnh";
            // 
            // chkKT_v45072
            // 
            this.chkKT_v45072.Location = new System.Drawing.Point(1478, 599);
            this.chkKT_v45072.Margin = new System.Windows.Forms.Padding(4);
            this.chkKT_v45072.Name = "chkKT_v45072";
            this.chkKT_v45072.Properties.Caption = "";
            this.chkKT_v45072.Size = new System.Drawing.Size(39, 19);
            this.chkKT_v45072.StyleController = this.layoutControl1;
            this.chkKT_v45072.TabIndex = 18;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.layoutControlItem35,
            this.emptySpaceItem3,
            this.lciChkKT_v45072,
            this.lciBtnDanhSach_v45072,
            this.lciTotalService_v45072,
            this.lciTotalPatient_v45072,
            this.emptySpaceItem7});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1648, 629);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.dteFrom;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.OptionsToolTip.ToolTip = "Thời gian y lệnh";
            this.layoutControlItem1.Size = new System.Drawing.Size(245, 28);
            this.layoutControlItem1.Text = "TG y lệnh:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.dteTo;
            this.layoutControlItem2.Location = new System.Drawing.Point(245, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(161, 28);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.cboService;
            this.layoutControlItem3.Location = new System.Drawing.Point(406, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(206, 28);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.cboStt;
            this.layoutControlItem4.Location = new System.Drawing.Point(612, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(168, 28);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.txtPatientCode;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 28);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(245, 33);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.txtFind;
            this.layoutControlItem6.Location = new System.Drawing.Point(245, 28);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(367, 33);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnSearch;
            this.layoutControlItem7.Location = new System.Drawing.Point(612, 28);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(168, 33);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.gridControl1;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 61);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(780, 535);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.groupControl1;
            this.layoutControlItem9.Location = new System.Drawing.Point(780, 0);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(868, 143);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.groupControl2;
            this.layoutControlItem10.Location = new System.Drawing.Point(780, 143);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(868, 453);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem35
            // 
            this.layoutControlItem35.Control = this.btnSave;
            this.layoutControlItem35.Location = new System.Drawing.Point(1520, 596);
            this.layoutControlItem35.Name = "layoutControlItem35";
            this.layoutControlItem35.Size = new System.Drawing.Size(128, 33);
            this.layoutControlItem35.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem35.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(780, 596);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(660, 33);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciChkKT_v45072
            // 
            this.lciChkKT_v45072.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciChkKT_v45072.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciChkKT_v45072.Control = this.chkKT_v45072;
            this.lciChkKT_v45072.Location = new System.Drawing.Point(1440, 596);
            this.lciChkKT_v45072.Name = "lciChkKT_v45072";
            this.lciChkKT_v45072.Size = new System.Drawing.Size(80, 33);
            this.lciChkKT_v45072.Text = "KT:";
            this.lciChkKT_v45072.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciChkKT_v45072.TextSize = new System.Drawing.Size(30, 20);
            this.lciChkKT_v45072.TextToControlDistance = 5;
            // 
            // lciBtnDanhSach_v45072
            // 
            this.lciBtnDanhSach_v45072.Control = this.btnDanhSachYLenh_v45072;
            this.lciBtnDanhSach_v45072.Location = new System.Drawing.Point(612, 596);
            this.lciBtnDanhSach_v45072.Name = "lciBtnDanhSach_v45072";
            this.lciBtnDanhSach_v45072.Size = new System.Drawing.Size(168, 33);
            this.lciBtnDanhSach_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnDanhSach_v45072.TextVisible = false;
            // 
            // lciTotalService_v45072
            // 
            this.lciTotalService_v45072.Control = this.lblTotalService_v45072;
            this.lciTotalService_v45072.Location = new System.Drawing.Point(179, 596);
            this.lciTotalService_v45072.MaxSize = new System.Drawing.Size(180, 33);
            this.lciTotalService_v45072.MinSize = new System.Drawing.Size(180, 33);
            this.lciTotalService_v45072.Name = "lciTotalService_v45072";
            this.lciTotalService_v45072.Size = new System.Drawing.Size(180, 33);
            this.lciTotalService_v45072.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTotalService_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciTotalService_v45072.TextVisible = false;
            // 
            // lciTotalPatient_v45072
            // 
            this.lciTotalPatient_v45072.Control = this.lblTotalPatient_v45072;
            this.lciTotalPatient_v45072.Location = new System.Drawing.Point(0, 596);
            this.lciTotalPatient_v45072.MaxSize = new System.Drawing.Size(179, 33);
            this.lciTotalPatient_v45072.MinSize = new System.Drawing.Size(179, 33);
            this.lciTotalPatient_v45072.Name = "lciTotalPatient_v45072";
            this.lciTotalPatient_v45072.Size = new System.Drawing.Size(179, 33);
            this.lciTotalPatient_v45072.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTotalPatient_v45072.TextSize = new System.Drawing.Size(0, 0);
            this.lciTotalPatient_v45072.TextVisible = false;
            // 
            // emptySpaceItem7
            // 
            this.emptySpaceItem7.AllowHotTrack = false;
            this.emptySpaceItem7.Location = new System.Drawing.Point(359, 596);
            this.emptySpaceItem7.Name = "emptySpaceItem7";
            this.emptySpaceItem7.Size = new System.Drawing.Size(253, 33);
            this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(274, 252);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(369, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(321, 84);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(322, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            this.emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciMachineCode_v45072
            // 
            this.lciMachineCode_v45072.Location = new System.Drawing.Point(0, 0);
            this.lciMachineCode_v45072.Name = "lciMachineCode_v45072";
            this.lciMachineCode_v45072.Size = new System.Drawing.Size(0, 0);
            this.lciMachineCode_v45072.TextSize = new System.Drawing.Size(50, 20);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "circle-red.png");
            this.imageList1.Images.SetKeyName(1, "circle-white.png");
            this.imageList1.Images.SetKeyName(2, "circle-yellow.png");
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // UCSurgServiceReqExecute2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UCSurgServiceReqExecute2";
            this.Size = new System.Drawing.Size(1648, 629);
            this.Load += new System.EventHandler(this.UCSurgServiceReqExecute2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdControlInformationSurg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewInformationSurg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repExecute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCustomGridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCustomGridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridViewWithFilterMultiColumn1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repMinus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPlus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEkipUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn7View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPtttGroup.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn6View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPtttMethodReal.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn5View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPtttGroup.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPtttMethodReal.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmotionLessMethod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEmotionLessMethod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn4View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPtttMethod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn3View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPtttMethod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFinish.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFinish.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStart.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStart.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPtttTemp_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCode_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcdName_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSuaIcd_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdSubCode_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcdText_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCmCode_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcdCmName_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSuaIcdCm_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCmSubCode_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcdCmText_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnTimeProcess_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEmotionLess_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtManner_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMachine_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConclude_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInstructionNote_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabDescription_v45072)).EndInit();
            this.tabDescription_v45072.ResumeLayout(false);
            this.tabPageMoTa_v45072.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription_v45072.Properties)).EndInit();
            this.tabPageGhiChu_v45072.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtNote_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPtttTemp_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdCode_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdName_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkSuaIcd_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdSubCode_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdText_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdCmCode_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdCmName_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkSuaIcdCm_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdCmSubCode_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdCmText_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTimeProcess_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLblPhut_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciEmotionLess_v45072)).EndInit();
            // Việc 45072 — EndInit txt code + lci cbo Vô cảm
            ((System.ComponentModel.ISupportInitialize)(this.txtEmotionLessCode_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboEmotionLess_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciManner_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMachine_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclude_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInstructionNote_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTabDescription_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSavePtttTemp_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repStt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFind.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboService.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkKT_v45072.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkKT_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDanhSach_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalService_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalPatient_v45072)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMachineCode_v45072)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditorInfo;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider;
        private DevExpress.Utils.ToolTipController toolTipControllerGrid;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtFind;
        private DevExpress.XtraEditors.TextEdit txtPatientCode;
        private DevExpress.XtraEditors.ComboBoxEdit cboStt;
        private DevExpress.XtraEditors.DateEdit dteTo;
        private DevExpress.XtraEditors.DateEdit dteFrom;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraEditors.LabelControl lblNote;
        private DevExpress.XtraEditors.LabelControl lblType;
        private DevExpress.XtraEditors.LabelControl lblHeinCardFromTo;
        private DevExpress.XtraEditors.LabelControl lblAddress;
        private DevExpress.XtraEditors.LabelControl lblKCBBD;
        private DevExpress.XtraEditors.LabelControl lblHeinCardNumber;
        private DevExpress.XtraEditors.LabelControl lblGender;
        private DevExpress.XtraEditors.LabelControl lblPatientDob;
        private DevExpress.XtraEditors.LabelControl lblPatientName;
        private DevExpress.XtraEditors.LabelControl lblPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboPtttGroup;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridLookUpEditWithFilterMultiColumn6View;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboPtttMethodReal;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridLookUpEditWithFilterMultiColumn5View;
        private DevExpress.XtraEditors.TextEdit txtPtttGroup;
        private DevExpress.XtraEditors.TextEdit txtPtttMethodReal;
        private DevExpress.XtraEditors.TextEdit txtEmotionLessMethod;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboEmotionLessMethod;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridLookUpEditWithFilterMultiColumn4View;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboPtttMethod;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridLookUpEditWithFilterMultiColumn3View;
        private DevExpress.XtraEditors.TextEdit txtPtttMethod;
        private DevExpress.XtraEditors.DateEdit dteFinish;
        private DevExpress.XtraEditors.DateEdit dteStart;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboDepartment;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridLookUpEditWithFilterMultiColumn2View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem21;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem22;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem23;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem24;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem25;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem26;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem27;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem29;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem28;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem30;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem31;
        private DevExpress.XtraGrid.GridControl grdControlInformationSurg;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewInformationSurg;
        private DevExpress.XtraEditors.SimpleButton btnSaveEkip;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboEkipUser;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridLookUpEditWithFilterMultiColumn7View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem32;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem33;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem34;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private Inventec.Desktop.CustomControl.RepositoryItemCustomGridLookUpEdit repUser;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn repositoryItemCustomGridLookUpEdit1View;
        private Inventec.Desktop.CustomControl.RepositoryItemCustomGridLookUpEdit repExecute;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn repositoryItemCustomGridLookUpEdit2View;
        private Inventec.Desktop.CustomControl.RepositoryItemCustomGridLookUpEdit repDepartment;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridViewWithFilterMultiColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repMinus;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repPlus;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem35;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repStt;
        private System.Windows.Forms.ImageList imageList1;
        private Inventec.Desktop.CustomControl.CustomGrid.CustomGridLookUpEdit cboService;
        private Inventec.Desktop.CustomControl.CustomGrid.CustomGridView customGridLookUpEdit1View;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        // Việc 45072 — 5 cột bổ sung Grid trái
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnPatientType_v45072;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnRequestDoctor_v45072;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnBeginTime_v45072;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnEndTime_v45072;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnPrice_v45072;
        // Việc 45072 — Footer Grid trái
        private DevExpress.XtraEditors.LabelControl lblTotalPatient_v45072;
        private DevExpress.XtraEditors.LabelControl lblTotalService_v45072;
        private DevExpress.XtraEditors.SimpleButton btnDanhSachYLenh_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciTotalPatient_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciTotalService_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnDanhSach_v45072;
        // Việc 45072 — KT checkbox
        private DevExpress.XtraEditors.CheckEdit chkKT_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciChkKT_v45072;
        // Việc 45072 — Mẫu PTTT + button Lưu mẫu
        private DevExpress.XtraEditors.LookUpEdit cboPtttTemp_v45072;
        private DevExpress.XtraEditors.SimpleButton btnSavePtttTemp_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciPtttTemp_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSavePtttTemp_v45072;
        // Việc 45072 — 4 ICD (CĐ chính/phụ + ICD9 chính/phụ)
        private DevExpress.XtraEditors.TextEdit txtIcdCode_v45072;
        private DevExpress.XtraEditors.LookUpEdit cboIcdName_v45072;
        private DevExpress.XtraEditors.CheckEdit chkSuaIcd_v45072;
        private DevExpress.XtraEditors.TextEdit txtIcdSubCode_v45072;
        private DevExpress.XtraEditors.LookUpEdit cboIcdText_v45072;
        private DevExpress.XtraEditors.TextEdit txtIcdCmCode_v45072;
        private DevExpress.XtraEditors.LookUpEdit cboIcdCmName_v45072;
        private DevExpress.XtraEditors.CheckEdit chkSuaIcdCm_v45072;
        private DevExpress.XtraEditors.TextEdit txtIcdCmSubCode_v45072;
        private DevExpress.XtraEditors.LookUpEdit cboIcdCmText_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciIcdCode_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciIcdName_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciChkSuaIcd_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciIcdSubCode_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciIcdText_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciIcdCmCode_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciIcdCmName_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciChkSuaIcdCm_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciIcdCmSubCode_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciIcdCmText_v45072;
        // Việc 45072 — TG xử lý + Vô cảm
        private DevExpress.XtraEditors.SpinEdit spnTimeProcess_v45072;
        private DevExpress.XtraEditors.LookUpEdit cboEmotionLess_v45072;
        // Việc 45072 — Txt code Vô cảm + Lci cbo (tách 2 cell giống Phương pháp TT)
        private DevExpress.XtraEditors.TextEdit txtEmotionLessCode_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciCboEmotionLess_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciTimeProcess_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciEmotionLess_v45072;
        // Việc 45072 — Label "phút" hiển thị ngoài spin TG xử lý
        private DevExpress.XtraEditors.LabelControl lblPhut_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciLblPhut_v45072;
        // Việc 45072 — Cách thức + Máy + Mã máy
        private DevExpress.XtraEditors.MemoEdit txtManner_v45072;
        private DevExpress.XtraEditors.LookUpEdit cboMachine_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciManner_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciMachine_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciMachineCode_v45072;
        // Việc 45072 — Kết luận + Ghi chú BSCĐ + Tab Mô tả/Ghi chú
        private DevExpress.XtraEditors.MemoEdit txtConclude_v45072;
        private DevExpress.XtraEditors.MemoEdit txtInstructionNote_v45072;
        private DevExpress.XtraTab.XtraTabControl tabDescription_v45072;
        private DevExpress.XtraTab.XtraTabPage tabPageMoTa_v45072;
        private DevExpress.XtraEditors.MemoEdit txtDescription_v45072;
        private DevExpress.XtraTab.XtraTabPage tabPageGhiChu_v45072;
        private DevExpress.XtraEditors.MemoEdit txtNote_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciConclude_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciInstructionNote_v45072;
        private DevExpress.XtraLayout.LayoutControlItem lciTabDescription_v45072;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem7;
    }
}

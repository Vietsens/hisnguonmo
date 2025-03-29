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
namespace HIS.UC.ExamTreatmentFinish.Run
{
    partial class UCExamTreatmentFinish
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
			if(userControl!=null)
				userControl.StopTimer(nameModuleLink, this.Name + "timer1");
			base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCExamTreatmentFinish));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnCheckIcd = new DevExpress.XtraEditors.SimpleButton();
            this.panelControlSubIcd = new DevExpress.XtraEditors.PanelControl();
            this.cboCareer = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkIsExpXml4210Collinear = new DevExpress.XtraEditors.CheckEdit();
            this.chkPrintHosTransfer = new DevExpress.XtraEditors.CheckEdit();
            this.chkPrintPrescription = new DevExpress.XtraEditors.CheckEdit();
            this.btnICDInformation = new DevExpress.XtraEditors.SimpleButton();
            this.chkKyPhieuTrichLuc = new DevExpress.XtraEditors.CheckEdit();
            this.chkInPhieuTrichLuc = new DevExpress.XtraEditors.CheckEdit();
            this.chkSignBHXH = new DevExpress.XtraEditors.CheckEdit();
            this.chkPrintBHXH = new DevExpress.XtraEditors.CheckEdit();
            this.chkSignBordereau = new DevExpress.XtraEditors.CheckEdit();
            this.chkSignAppoinment = new DevExpress.XtraEditors.CheckEdit();
            this.txtIcdText = new DevExpress.XtraEditors.TextEdit();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboTraditionalIcds = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.customGridViewWithFilterMultiColumn1 = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.txtTraditionalIcdMainText = new DevExpress.XtraEditors.TextEdit();
            this.txtIcdSubCode = new DevExpress.XtraEditors.TextEdit();
            this.chkTraditionalIcd = new DevExpress.XtraEditors.CheckEdit();
            this.txtTraditionalIcdCode = new DevExpress.XtraEditors.TextEdit();
            this.cboTreatmentResult = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkEditIcd = new DevExpress.XtraEditors.CheckEdit();
            this.panelControlIcds = new DevExpress.XtraEditors.PanelControl();
            this.cboIcds = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.customGridLookUpEditWithFilterMultiColumn1View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.txtIcdMainText = new DevExpress.XtraEditors.TextEdit();
            this.txtIcdCode = new DevExpress.XtraEditors.TextEdit();
            this.panelExamTreatmentFinish = new DevExpress.XtraEditors.XtraScrollableControl();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.chkSignExam = new DevExpress.XtraEditors.CheckEdit();
            this.chkPrintExam = new DevExpress.XtraEditors.CheckEdit();
            this.cboHospSubs = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.gridLookUpEdit2View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.cboEndDeptSubs = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.gridView4 = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.txtAdviseNew = new DevExpress.XtraEditors.MemoEdit();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.txtConclusionNew = new DevExpress.XtraEditors.MemoEdit();
            this.xtraTabPage3 = new DevExpress.XtraTab.XtraTabPage();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.memNote = new DevExpress.XtraEditors.MemoEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem36 = new DevExpress.XtraLayout.LayoutControlItem();
            this.cboProgram = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewProgram = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblSoLuuTruBA = new DevExpress.XtraEditors.LabelControl();
            this.chkCapSoLuuTruBA = new DevExpress.XtraEditors.CheckEdit();
            this.chkBANT = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroupHisPatientProgram = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciChkCapSoLuuTruBA = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientProgram = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBANT = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem29 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem30 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem31 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem34 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem35 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSoLuuTruBA = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.cboTreatmentEndTypeExt = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.btnChiDinhDichVuHenKham = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.chkPrintBordereau = new DevExpress.XtraEditors.CheckEdit();
            this.lblOutCode = new DevExpress.XtraEditors.LabelControl();
            this.lblEndCode = new DevExpress.XtraEditors.LabelControl();
            this.chkPrintAppoinment = new DevExpress.XtraEditors.CheckEdit();
            this.cboTreatmentEndType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dtEndTime = new DevExpress.XtraEditors.DateEdit();
            this.dtTimeIn = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChiDinhDichVuhenKham = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlIPanelUCExtend = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIcdText = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem27 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIsExpXml4210Collinear = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciCareer = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem32 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem26 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem33 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlSubIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCareer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsExpXml4210Collinear.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintHosTransfer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintPrescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkKyPhieuTrichLuc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkInPhieuTrichLuc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignBHXH.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintBHXH.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignBordereau.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignAppoinment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdText.Properties)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboTraditionalIcds.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridViewWithFilterMultiColumn1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTraditionalIcdMainText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdSubCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTraditionalIcd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTraditionalIcdCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTreatmentResult.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditIcd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlIcds)).BeginInit();
            this.panelControlIcds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcds.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdMainText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignExam.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintExam.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboHospSubs.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndDeptSubs.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAdviseNew.Properties)).BeginInit();
            this.xtraTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtConclusionNew.Properties)).BeginInit();
            this.xtraTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memNote.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem36)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProgram.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewProgram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCapSoLuuTruBA.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBANT.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupHisPatientProgram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkCapSoLuuTruBA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientProgram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBANT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSoLuuTruBA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTreatmentEndTypeExt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintBordereau.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintAppoinment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTreatmentEndType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeIn.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeIn.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChiDinhDichVuhenKham)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlIPanelUCExtend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsExpXml4210Collinear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCareer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnCheckIcd);
            this.layoutControl1.Controls.Add(this.panelControlSubIcd);
            this.layoutControl1.Controls.Add(this.cboCareer);
            this.layoutControl1.Controls.Add(this.chkIsExpXml4210Collinear);
            this.layoutControl1.Controls.Add(this.chkPrintHosTransfer);
            this.layoutControl1.Controls.Add(this.chkPrintPrescription);
            this.layoutControl1.Controls.Add(this.btnICDInformation);
            this.layoutControl1.Controls.Add(this.chkKyPhieuTrichLuc);
            this.layoutControl1.Controls.Add(this.chkInPhieuTrichLuc);
            this.layoutControl1.Controls.Add(this.chkSignBHXH);
            this.layoutControl1.Controls.Add(this.chkPrintBHXH);
            this.layoutControl1.Controls.Add(this.chkSignBordereau);
            this.layoutControl1.Controls.Add(this.chkSignAppoinment);
            this.layoutControl1.Controls.Add(this.txtIcdText);
            this.layoutControl1.Controls.Add(this.panel1);
            this.layoutControl1.Controls.Add(this.txtIcdSubCode);
            this.layoutControl1.Controls.Add(this.chkTraditionalIcd);
            this.layoutControl1.Controls.Add(this.txtTraditionalIcdCode);
            this.layoutControl1.Controls.Add(this.cboTreatmentResult);
            this.layoutControl1.Controls.Add(this.chkEditIcd);
            this.layoutControl1.Controls.Add(this.panelControlIcds);
            this.layoutControl1.Controls.Add(this.txtIcdCode);
            this.layoutControl1.Controls.Add(this.panelExamTreatmentFinish);
            this.layoutControl1.Controls.Add(this.layoutControl3);
            this.layoutControl1.Controls.Add(this.cboTreatmentEndTypeExt);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Controls.Add(this.chkPrintBordereau);
            this.layoutControl1.Controls.Add(this.lblOutCode);
            this.layoutControl1.Controls.Add(this.lblEndCode);
            this.layoutControl1.Controls.Add(this.chkPrintAppoinment);
            this.layoutControl1.Controls.Add(this.cboTreatmentEndType);
            this.layoutControl1.Controls.Add(this.dtEndTime);
            this.layoutControl1.Controls.Add(this.dtTimeIn);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(706, 131, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 532);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnCheckIcd
            // 
            this.btnCheckIcd.Image = ((System.Drawing.Image)(resources.GetObject("btnCheckIcd.Image")));
            this.btnCheckIcd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCheckIcd.Location = new System.Drawing.Point(630, 28);
            this.btnCheckIcd.MaximumSize = new System.Drawing.Size(35, 0);
            this.btnCheckIcd.Name = "btnCheckIcd";
            this.btnCheckIcd.Size = new System.Drawing.Size(28, 22);
            this.btnCheckIcd.StyleController = this.layoutControl1;
            this.btnCheckIcd.TabIndex = 42;
            this.btnCheckIcd.ToolTip = "Tổng hợp từ các chẩn đoán phụ, chẩn đoán chính của tất cả các y lệnh và không chứ" +
    "a chẩn đoán chính của hồ sơ điều trị vào chẩn đoán phụ";
            this.btnCheckIcd.Click += new System.EventHandler(this.btnCheckIcd_Click);
            // 
            // panelControlSubIcd
            // 
            this.panelControlSubIcd.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.panelControlSubIcd.Appearance.Options.UseBackColor = true;
            this.panelControlSubIcd.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlSubIcd.Location = new System.Drawing.Point(0, 26);
            this.panelControlSubIcd.Name = "panelControlSubIcd";
            this.panelControlSubIcd.Size = new System.Drawing.Size(628, 26);
            this.panelControlSubIcd.TabIndex = 41;
            // 
            // cboCareer
            // 
            this.cboCareer.Location = new System.Drawing.Point(107, 102);
            this.cboCareer.Name = "cboCareer";
            this.cboCareer.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboCareer.Properties.AutoComplete = false;
            this.cboCareer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboCareer.Properties.NullText = "";
            this.cboCareer.Properties.View = this.gridView3;
            this.cboCareer.Size = new System.Drawing.Size(170, 20);
            this.cboCareer.StyleController = this.layoutControl1;
            this.cboCareer.TabIndex = 40;
            // 
            // gridView3
            // 
            this.gridView3.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView3.Name = "gridView3";
            this.gridView3.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView3.OptionsView.ShowGroupPanel = false;
            // 
            // chkIsExpXml4210Collinear
            // 
            this.chkIsExpXml4210Collinear.Location = new System.Drawing.Point(426, 256);
            this.chkIsExpXml4210Collinear.Name = "chkIsExpXml4210Collinear";
            this.chkIsExpXml4210Collinear.Properties.Caption = "";
            this.chkIsExpXml4210Collinear.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkIsExpXml4210Collinear.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkIsExpXml4210Collinear.Size = new System.Drawing.Size(36, 19);
            this.chkIsExpXml4210Collinear.StyleController = this.layoutControl1;
            this.chkIsExpXml4210Collinear.TabIndex = 39;
            this.chkIsExpXml4210Collinear.CheckedChanged += new System.EventHandler(this.chkIsExpXml4210Collinear_CheckedChanged);
            // 
            // chkPrintHosTransfer
            // 
            this.chkPrintHosTransfer.Enabled = false;
            this.chkPrintHosTransfer.Location = new System.Drawing.Point(426, 236);
            this.chkPrintHosTransfer.Name = "chkPrintHosTransfer";
            this.chkPrintHosTransfer.Properties.Caption = ":In";
            this.chkPrintHosTransfer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkPrintHosTransfer.Size = new System.Drawing.Size(36, 19);
            this.chkPrintHosTransfer.StyleController = this.layoutControl1;
            this.chkPrintHosTransfer.TabIndex = 38;
            this.chkPrintHosTransfer.CheckedChanged += new System.EventHandler(this.chkPrintHosTransfer_CheckedChanged);
            // 
            // chkPrintPrescription
            // 
            this.chkPrintPrescription.Location = new System.Drawing.Point(105, 256);
            this.chkPrintPrescription.Name = "chkPrintPrescription";
            this.chkPrintPrescription.Properties.Caption = ":In";
            this.chkPrintPrescription.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkPrintPrescription.Size = new System.Drawing.Size(36, 19);
            this.chkPrintPrescription.StyleController = this.layoutControl1;
            this.chkPrintPrescription.TabIndex = 37;
            this.chkPrintPrescription.CheckedChanged += new System.EventHandler(this.chkPrintPrescription_CheckedChanged);
            // 
            // btnICDInformation
            // 
            this.btnICDInformation.Image = ((System.Drawing.Image)(resources.GetObject("btnICDInformation.Image")));
            this.btnICDInformation.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnICDInformation.Location = new System.Drawing.Point(630, 2);
            this.btnICDInformation.MaximumSize = new System.Drawing.Size(35, 0);
            this.btnICDInformation.Name = "btnICDInformation";
            this.btnICDInformation.Size = new System.Drawing.Size(28, 22);
            this.btnICDInformation.StyleController = this.layoutControl1;
            this.btnICDInformation.TabIndex = 36;
            this.btnICDInformation.Text = " ";
            this.btnICDInformation.ToolTip = "Bổ sung thông tin chẩn đoán hiển thị trên giấy ra viện";
            this.btnICDInformation.Click += new System.EventHandler(this.btnICDInformation_Click);
            // 
            // chkKyPhieuTrichLuc
            // 
            this.chkKyPhieuTrichLuc.Location = new System.Drawing.Point(141, 235);
            this.chkKyPhieuTrichLuc.Name = "chkKyPhieuTrichLuc";
            this.chkKyPhieuTrichLuc.Properties.Caption = ":Ký";
            this.chkKyPhieuTrichLuc.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkKyPhieuTrichLuc.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkKyPhieuTrichLuc.Size = new System.Drawing.Size(38, 19);
            this.chkKyPhieuTrichLuc.StyleController = this.layoutControl1;
            this.chkKyPhieuTrichLuc.TabIndex = 35;
            this.chkKyPhieuTrichLuc.CheckedChanged += new System.EventHandler(this.chkKyPhieuTrichLuc_CheckedChanged);
            // 
            // chkInPhieuTrichLuc
            // 
            this.chkInPhieuTrichLuc.Location = new System.Drawing.Point(105, 236);
            this.chkInPhieuTrichLuc.Name = "chkInPhieuTrichLuc";
            this.chkInPhieuTrichLuc.Properties.Caption = ":In";
            this.chkInPhieuTrichLuc.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkInPhieuTrichLuc.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkInPhieuTrichLuc.Size = new System.Drawing.Size(36, 19);
            this.chkInPhieuTrichLuc.StyleController = this.layoutControl1;
            this.chkInPhieuTrichLuc.TabIndex = 34;
            this.chkInPhieuTrichLuc.CheckedChanged += new System.EventHandler(this.chkInPhieuTrichLuc_CheckedChanged);
            // 
            // chkSignBHXH
            // 
            this.chkSignBHXH.Enabled = false;
            this.chkSignBHXH.Location = new System.Drawing.Point(468, 174);
            this.chkSignBHXH.Name = "chkSignBHXH";
            this.chkSignBHXH.Properties.Caption = ":Ký";
            this.chkSignBHXH.Properties.FullFocusRect = true;
            this.chkSignBHXH.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkSignBHXH.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkSignBHXH.Size = new System.Drawing.Size(38, 19);
            this.chkSignBHXH.StyleController = this.layoutControl1;
            this.chkSignBHXH.TabIndex = 33;
            this.chkSignBHXH.CheckedChanged += new System.EventHandler(this.chkSignBHXH_CheckedChanged);
            // 
            // chkPrintBHXH
            // 
            this.chkPrintBHXH.Enabled = false;
            this.chkPrintBHXH.Location = new System.Drawing.Point(428, 174);
            this.chkPrintBHXH.Name = "chkPrintBHXH";
            this.chkPrintBHXH.Properties.Caption = ":In";
            this.chkPrintBHXH.Properties.FullFocusRect = true;
            this.chkPrintBHXH.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkPrintBHXH.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkPrintBHXH.Size = new System.Drawing.Size(36, 19);
            this.chkPrintBHXH.StyleController = this.layoutControl1;
            this.chkPrintBHXH.TabIndex = 32;
            this.chkPrintBHXH.CheckedChanged += new System.EventHandler(this.chkPrintBHXH_CheckedChanged);
            // 
            // chkSignBordereau
            // 
            this.chkSignBordereau.Location = new System.Drawing.Point(464, 218);
            this.chkSignBordereau.Name = "chkSignBordereau";
            this.chkSignBordereau.Properties.Caption = ":Ký";
            this.chkSignBordereau.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkSignBordereau.Size = new System.Drawing.Size(38, 19);
            this.chkSignBordereau.StyleController = this.layoutControl1;
            this.chkSignBordereau.TabIndex = 31;
            this.chkSignBordereau.CheckedChanged += new System.EventHandler(this.chkSignBordereau_CheckedChanged);
            // 
            // chkSignAppoinment
            // 
            this.chkSignAppoinment.Location = new System.Drawing.Point(141, 216);
            this.chkSignAppoinment.Name = "chkSignAppoinment";
            this.chkSignAppoinment.Properties.Caption = ":Ký";
            this.chkSignAppoinment.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkSignAppoinment.Size = new System.Drawing.Size(38, 19);
            this.chkSignAppoinment.StyleController = this.layoutControl1;
            this.chkSignAppoinment.TabIndex = 30;
            this.chkSignAppoinment.CheckedChanged += new System.EventHandler(this.chkSignAppoinment_CheckedChanged);
            // 
            // txtIcdText
            // 
            this.txtIcdText.Location = new System.Drawing.Point(202, 78);
            this.txtIcdText.Name = "txtIcdText";
            this.txtIcdText.Properties.NullValuePrompt = "Nhấn F1 để chọn bệnh";
            this.txtIcdText.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtIcdText.Size = new System.Drawing.Size(456, 20);
            this.txtIcdText.StyleController = this.layoutControl1;
            this.txtIcdText.TabIndex = 8;
            this.txtIcdText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIcdText_KeyDown);
            this.txtIcdText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtIcdText_KeyUp);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cboTraditionalIcds);
            this.panel1.Controls.Add(this.txtTraditionalIcdMainText);
            this.panel1.Location = new System.Drawing.Point(202, 54);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(356, 20);
            this.panel1.TabIndex = 28;
            // 
            // cboTraditionalIcds
            // 
            this.cboTraditionalIcds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboTraditionalIcds.Location = new System.Drawing.Point(0, 0);
            this.cboTraditionalIcds.Name = "cboTraditionalIcds";
            this.cboTraditionalIcds.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTraditionalIcds.Properties.AutoComplete = false;
            this.cboTraditionalIcds.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboTraditionalIcds.Properties.NullText = "";
            this.cboTraditionalIcds.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboTraditionalIcds.Properties.View = this.customGridViewWithFilterMultiColumn1;
            this.cboTraditionalIcds.Size = new System.Drawing.Size(356, 20);
            this.cboTraditionalIcds.TabIndex = 25;
            this.cboTraditionalIcds.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTraditionalIcds_Closed);
            this.cboTraditionalIcds.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTraditionalIcds_ButtonClick);
            this.cboTraditionalIcds.EditValueChanged += new System.EventHandler(this.cboTraditionalIcds_EditValueChanged);
            this.cboTraditionalIcds.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboTraditionalIcds_KeyUp);
            // 
            // customGridViewWithFilterMultiColumn1
            // 
            this.customGridViewWithFilterMultiColumn1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridViewWithFilterMultiColumn1.Name = "customGridViewWithFilterMultiColumn1";
            this.customGridViewWithFilterMultiColumn1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridViewWithFilterMultiColumn1.OptionsView.ShowGroupPanel = false;
            // 
            // txtTraditionalIcdMainText
            // 
            this.txtTraditionalIcdMainText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTraditionalIcdMainText.Location = new System.Drawing.Point(0, 0);
            this.txtTraditionalIcdMainText.Name = "txtTraditionalIcdMainText";
            this.txtTraditionalIcdMainText.Size = new System.Drawing.Size(356, 20);
            this.txtTraditionalIcdMainText.TabIndex = 27;
            this.txtTraditionalIcdMainText.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTraditionalIcdMainText_PreviewKeyDown);
            // 
            // txtIcdSubCode
            // 
            this.txtIcdSubCode.Location = new System.Drawing.Point(107, 78);
            this.txtIcdSubCode.Name = "txtIcdSubCode";
            this.txtIcdSubCode.Size = new System.Drawing.Size(95, 20);
            this.txtIcdSubCode.StyleController = this.layoutControl1;
            this.txtIcdSubCode.TabIndex = 7;
            this.txtIcdSubCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIcdSubCode_KeyDown);
            this.txtIcdSubCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtIcdSubCode_KeyUp);
            // 
            // chkTraditionalIcd
            // 
            this.chkTraditionalIcd.Location = new System.Drawing.Point(562, 54);
            this.chkTraditionalIcd.Name = "chkTraditionalIcd";
            this.chkTraditionalIcd.Properties.Caption = "Sửa";
            this.chkTraditionalIcd.Properties.FullFocusRect = true;
            this.chkTraditionalIcd.Size = new System.Drawing.Size(96, 19);
            this.chkTraditionalIcd.StyleController = this.layoutControl1;
            this.chkTraditionalIcd.TabIndex = 26;
            this.chkTraditionalIcd.CheckedChanged += new System.EventHandler(this.chkTraditionalIcd_CheckedChanged);
            this.chkTraditionalIcd.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkTraditionalIcd_PreviewKeyDown);
            // 
            // txtTraditionalIcdCode
            // 
            this.txtTraditionalIcdCode.Location = new System.Drawing.Point(107, 54);
            this.txtTraditionalIcdCode.Name = "txtTraditionalIcdCode";
            this.txtTraditionalIcdCode.Size = new System.Drawing.Size(95, 20);
            this.txtTraditionalIcdCode.StyleController = this.layoutControl1;
            this.txtTraditionalIcdCode.TabIndex = 24;
            this.txtTraditionalIcdCode.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtTraditionalIcdCode_InvalidValue);
            this.txtTraditionalIcdCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTraditionalIcdCode_PreviewKeyDown);
            // 
            // cboTreatmentResult
            // 
            this.cboTreatmentResult.Location = new System.Drawing.Point(416, 150);
            this.cboTreatmentResult.Name = "cboTreatmentResult";
            this.cboTreatmentResult.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTreatmentResult.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboTreatmentResult.Properties.NullText = "";
            this.cboTreatmentResult.Properties.View = this.gridView2;
            this.cboTreatmentResult.Size = new System.Drawing.Size(242, 20);
            this.cboTreatmentResult.StyleController = this.layoutControl1;
            this.cboTreatmentResult.TabIndex = 23;
            // 
            // gridView2
            // 
            this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // chkEditIcd
            // 
            this.chkEditIcd.Location = new System.Drawing.Point(562, 2);
            this.chkEditIcd.Name = "chkEditIcd";
            this.chkEditIcd.Properties.Caption = "Sửa";
            this.chkEditIcd.Properties.FullFocusRect = true;
            this.chkEditIcd.Size = new System.Drawing.Size(64, 19);
            this.chkEditIcd.StyleController = this.layoutControl1;
            this.chkEditIcd.TabIndex = 22;
            this.chkEditIcd.CheckedChanged += new System.EventHandler(this.chkEditIcd_CheckedChanged);
            this.chkEditIcd.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEditIcd_PreviewKeyDown);
            // 
            // panelControlIcds
            // 
            this.panelControlIcds.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlIcds.Controls.Add(this.cboIcds);
            this.panelControlIcds.Controls.Add(this.txtIcdMainText);
            this.panelControlIcds.Location = new System.Drawing.Point(202, 2);
            this.panelControlIcds.Name = "panelControlIcds";
            this.panelControlIcds.Size = new System.Drawing.Size(356, 22);
            this.panelControlIcds.TabIndex = 21;
            // 
            // cboIcds
            // 
            this.cboIcds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboIcds.Location = new System.Drawing.Point(0, 0);
            this.cboIcds.Name = "cboIcds";
            this.cboIcds.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboIcds.Properties.AutoComplete = false;
            this.cboIcds.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboIcds.Properties.NullText = "";
            this.cboIcds.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboIcds.Properties.View = this.customGridLookUpEditWithFilterMultiColumn1View;
            this.cboIcds.Size = new System.Drawing.Size(356, 20);
            this.cboIcds.TabIndex = 1;
            this.cboIcds.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboIcds_Closed);
            this.cboIcds.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboIcds_ButtonClick);
            this.cboIcds.EditValueChanged += new System.EventHandler(this.cboIcds_EditValueChanged);
            this.cboIcds.TextChanged += new System.EventHandler(this.cboIcds_TextChanged);
            this.cboIcds.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboIcds_KeyUp);
            // 
            // customGridLookUpEditWithFilterMultiColumn1View
            // 
            this.customGridLookUpEditWithFilterMultiColumn1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridLookUpEditWithFilterMultiColumn1View.Name = "customGridLookUpEditWithFilterMultiColumn1View";
            this.customGridLookUpEditWithFilterMultiColumn1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridLookUpEditWithFilterMultiColumn1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtIcdMainText
            // 
            this.txtIcdMainText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIcdMainText.Location = new System.Drawing.Point(0, 0);
            this.txtIcdMainText.Name = "txtIcdMainText";
            this.txtIcdMainText.Size = new System.Drawing.Size(356, 20);
            this.txtIcdMainText.TabIndex = 0;
            this.txtIcdMainText.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdMainText_PreviewKeyDown);
            // 
            // txtIcdCode
            // 
            this.txtIcdCode.Location = new System.Drawing.Point(107, 2);
            this.txtIcdCode.Name = "txtIcdCode";
            this.txtIcdCode.Size = new System.Drawing.Size(95, 20);
            this.txtIcdCode.StyleController = this.layoutControl1;
            this.txtIcdCode.TabIndex = 20;
            this.txtIcdCode.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtIcdCode_InvalidValue);
            this.txtIcdCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdCode_PreviewKeyDown);
            // 
            // panelExamTreatmentFinish
            // 
            this.panelExamTreatmentFinish.Location = new System.Drawing.Point(0, 309);
            this.panelExamTreatmentFinish.Name = "panelExamTreatmentFinish";
            this.panelExamTreatmentFinish.Size = new System.Drawing.Size(660, 93);
            this.panelExamTreatmentFinish.TabIndex = 19;
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.chkSignExam);
            this.layoutControl3.Controls.Add(this.chkPrintExam);
            this.layoutControl3.Controls.Add(this.cboHospSubs);
            this.layoutControl3.Controls.Add(this.cboEndDeptSubs);
            this.layoutControl3.Controls.Add(this.xtraTabControl1);
            this.layoutControl3.Controls.Add(this.cboProgram);
            this.layoutControl3.Controls.Add(this.lblSoLuuTruBA);
            this.layoutControl3.Controls.Add(this.chkCapSoLuuTruBA);
            this.layoutControl3.Controls.Add(this.chkBANT);
            this.layoutControl3.Location = new System.Drawing.Point(0, 402);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(735, 157, 250, 350);
            this.layoutControl3.Root = this.layoutControlGroupHisPatientProgram;
            this.layoutControl3.Size = new System.Drawing.Size(660, 130);
            this.layoutControl3.TabIndex = 18;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // chkSignExam
            // 
            this.chkSignExam.Location = new System.Drawing.Point(465, 2);
            this.chkSignExam.Name = "chkSignExam";
            this.chkSignExam.Properties.Caption = ":Ký";
            this.chkSignExam.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkSignExam.Size = new System.Drawing.Size(38, 19);
            this.chkSignExam.StyleController = this.layoutControl3;
            this.chkSignExam.TabIndex = 22;
            this.chkSignExam.CheckedChanged += new System.EventHandler(this.chkSignExam_CheckedChanged);
            // 
            // chkPrintExam
            // 
            this.chkPrintExam.Location = new System.Drawing.Point(425, 2);
            this.chkPrintExam.Name = "chkPrintExam";
            this.chkPrintExam.Properties.Caption = ":In";
            this.chkPrintExam.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkPrintExam.Size = new System.Drawing.Size(36, 19);
            this.chkPrintExam.StyleController = this.layoutControl3;
            this.chkPrintExam.TabIndex = 21;
            this.chkPrintExam.CheckedChanged += new System.EventHandler(this.chkPrintExam_CheckedChanged);
            // 
            // cboHospSubs
            // 
            this.cboHospSubs.Location = new System.Drawing.Point(433, 50);
            this.cboHospSubs.Name = "cboHospSubs";
            this.cboHospSubs.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboHospSubs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboHospSubs.Properties.NullText = "";
            this.cboHospSubs.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboHospSubs.Properties.View = this.gridLookUpEdit2View;
            this.cboHospSubs.Size = new System.Drawing.Size(225, 20);
            this.cboHospSubs.StyleController = this.layoutControl3;
            this.cboHospSubs.TabIndex = 20;
            this.cboHospSubs.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboHospSubs_ButtonClick);
            this.cboHospSubs.EditValueChanged += new System.EventHandler(this.cboHospSubs_EditValueChanged);
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // cboEndDeptSubs
            // 
            this.cboEndDeptSubs.Location = new System.Drawing.Point(107, 50);
            this.cboEndDeptSubs.Name = "cboEndDeptSubs";
            this.cboEndDeptSubs.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboEndDeptSubs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboEndDeptSubs.Properties.NullText = "";
            this.cboEndDeptSubs.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboEndDeptSubs.Properties.View = this.gridView4;
            this.cboEndDeptSubs.Size = new System.Drawing.Size(217, 20);
            this.cboEndDeptSubs.StyleController = this.layoutControl3;
            this.cboEndDeptSubs.TabIndex = 19;
            this.cboEndDeptSubs.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboEndDeptSubs_ButtonClick);
            this.cboEndDeptSubs.EditValueChanged += new System.EventHandler(this.cboEndDeptSubs_EditValueChanged);
            // 
            // gridView4
            // 
            this.gridView4.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView4.Name = "gridView4";
            this.gridView4.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView4.OptionsView.ShowGroupPanel = false;
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Location = new System.Drawing.Point(2, 74);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(656, 54);
            this.xtraTabControl1.TabIndex = 18;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2,
            this.xtraTabPage3});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.txtAdviseNew);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(650, 26);
            this.xtraTabPage1.Text = "Lời dặn";
            // 
            // txtAdviseNew
            // 
            this.txtAdviseNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAdviseNew.Location = new System.Drawing.Point(0, 0);
            this.txtAdviseNew.Name = "txtAdviseNew";
            this.txtAdviseNew.Size = new System.Drawing.Size(650, 26);
            this.txtAdviseNew.TabIndex = 0;
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Controls.Add(this.txtConclusionNew);
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(650, 26);
            this.xtraTabPage2.Text = "Kết luận";
            // 
            // txtConclusionNew
            // 
            this.txtConclusionNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConclusionNew.Location = new System.Drawing.Point(0, 0);
            this.txtConclusionNew.Name = "txtConclusionNew";
            this.txtConclusionNew.Size = new System.Drawing.Size(650, 26);
            this.txtConclusionNew.TabIndex = 0;
            // 
            // xtraTabPage3
            // 
            this.xtraTabPage3.Controls.Add(this.layoutControl4);
            this.xtraTabPage3.Name = "xtraTabPage3";
            this.xtraTabPage3.Size = new System.Drawing.Size(650, 26);
            this.xtraTabPage3.Text = "Ghi chú";
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.memNote);
            this.layoutControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl4.Location = new System.Drawing.Point(0, 0);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.Root = this.layoutControlGroup2;
            this.layoutControl4.Size = new System.Drawing.Size(650, 26);
            this.layoutControl4.TabIndex = 0;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // memNote
            // 
            this.memNote.Location = new System.Drawing.Point(2, 2);
            this.memNote.Name = "memNote";
            this.memNote.Size = new System.Drawing.Size(646, 22);
            this.memNote.StyleController = this.layoutControl4;
            this.memNote.TabIndex = 4;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem36});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Size = new System.Drawing.Size(650, 26);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem36
            // 
            this.layoutControlItem36.Control = this.memNote;
            this.layoutControlItem36.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem36.Name = "layoutControlItem36";
            this.layoutControlItem36.Size = new System.Drawing.Size(650, 26);
            this.layoutControlItem36.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem36.TextVisible = false;
            // 
            // cboProgram
            // 
            this.cboProgram.Location = new System.Drawing.Point(107, 26);
            this.cboProgram.Name = "cboProgram";
            this.cboProgram.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboProgram.Properties.AutoComplete = false;
            this.cboProgram.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.cboProgram.Properties.NullText = "";
            this.cboProgram.Properties.View = this.gridViewProgram;
            this.cboProgram.Size = new System.Drawing.Size(217, 20);
            this.cboProgram.StyleController = this.layoutControl3;
            this.cboProgram.TabIndex = 6;
            this.cboProgram.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboProgram_ButtonClick);
            this.cboProgram.EditValueChanged += new System.EventHandler(this.cboProgram_EditValueChanged);
            // 
            // gridViewProgram
            // 
            this.gridViewProgram.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewProgram.Name = "gridViewProgram";
            this.gridViewProgram.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewProgram.OptionsView.ShowGroupPanel = false;
            this.gridViewProgram.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewProgram_RowCellStyle);
            // 
            // lblSoLuuTruBA
            // 
            this.lblSoLuuTruBA.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSoLuuTruBA.Location = new System.Drawing.Point(431, 24);
            this.lblSoLuuTruBA.Name = "lblSoLuuTruBA";
            this.lblSoLuuTruBA.Size = new System.Drawing.Size(229, 20);
            this.lblSoLuuTruBA.StyleController = this.layoutControl3;
            this.lblSoLuuTruBA.TabIndex = 5;
            // 
            // chkCapSoLuuTruBA
            // 
            this.chkCapSoLuuTruBA.Location = new System.Drawing.Point(102, 0);
            this.chkCapSoLuuTruBA.Name = "chkCapSoLuuTruBA";
            this.chkCapSoLuuTruBA.Properties.Caption = "";
            this.chkCapSoLuuTruBA.Properties.FullFocusRect = true;
            this.chkCapSoLuuTruBA.Size = new System.Drawing.Size(28, 19);
            this.chkCapSoLuuTruBA.StyleController = this.layoutControl3;
            this.chkCapSoLuuTruBA.TabIndex = 4;
            this.chkCapSoLuuTruBA.CheckedChanged += new System.EventHandler(this.chkCapSoLuuTruBA_CheckedChanged);
            // 
            // chkBANT
            // 
            this.chkBANT.Location = new System.Drawing.Point(235, 0);
            this.chkBANT.Name = "chkBANT";
            this.chkBANT.Properties.Caption = "";
            this.chkBANT.Properties.FullFocusRect = true;
            this.chkBANT.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkBANT.Size = new System.Drawing.Size(53, 19);
            this.chkBANT.StyleController = this.layoutControl3;
            this.chkBANT.TabIndex = 15;
            // 
            // layoutControlGroupHisPatientProgram
            // 
            this.layoutControlGroupHisPatientProgram.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupHisPatientProgram.GroupBordersVisible = false;
            this.layoutControlGroupHisPatientProgram.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciChkCapSoLuuTruBA,
            this.lciPatientProgram,
            this.lciBANT,
            this.layoutControlItem29,
            this.layoutControlItem30,
            this.layoutControlItem31,
            this.layoutControlItem34,
            this.layoutControlItem35,
            this.lciSoLuuTruBA,
            this.emptySpaceItem2});
            this.layoutControlGroupHisPatientProgram.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroupHisPatientProgram.Name = "Root";
            this.layoutControlGroupHisPatientProgram.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroupHisPatientProgram.Size = new System.Drawing.Size(660, 130);
            this.layoutControlGroupHisPatientProgram.TextVisible = false;
            // 
            // lciChkCapSoLuuTruBA
            // 
            this.lciChkCapSoLuuTruBA.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciChkCapSoLuuTruBA.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciChkCapSoLuuTruBA.Control = this.chkCapSoLuuTruBA;
            this.lciChkCapSoLuuTruBA.Location = new System.Drawing.Point(0, 0);
            this.lciChkCapSoLuuTruBA.Name = "lciChkCapSoLuuTruBA";
            this.lciChkCapSoLuuTruBA.OptionsToolTip.ToolTip = "Tạo hồ sơ bệnh án ngoại trú";
            this.lciChkCapSoLuuTruBA.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciChkCapSoLuuTruBA.Size = new System.Drawing.Size(130, 24);
            this.lciChkCapSoLuuTruBA.Text = "Tạo BA ngoại trú:";
            this.lciChkCapSoLuuTruBA.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciChkCapSoLuuTruBA.TextSize = new System.Drawing.Size(100, 20);
            this.lciChkCapSoLuuTruBA.TextToControlDistance = 2;
            // 
            // lciPatientProgram
            // 
            this.lciPatientProgram.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciPatientProgram.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciPatientProgram.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientProgram.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientProgram.Control = this.cboProgram;
            this.lciPatientProgram.Location = new System.Drawing.Point(0, 24);
            this.lciPatientProgram.Name = "lciPatientProgram";
            this.lciPatientProgram.Size = new System.Drawing.Size(326, 24);
            this.lciPatientProgram.Text = "Chương trình:";
            this.lciPatientProgram.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientProgram.TextSize = new System.Drawing.Size(100, 20);
            this.lciPatientProgram.TextToControlDistance = 5;
            // 
            // lciBANT
            // 
            this.lciBANT.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBANT.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBANT.Control = this.chkBANT;
            this.lciBANT.Location = new System.Drawing.Point(130, 0);
            this.lciBANT.Name = "lciBANT";
            this.lciBANT.OptionsToolTip.ToolTip = "In bệnh án ngoại trú";
            this.lciBANT.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciBANT.Size = new System.Drawing.Size(158, 24);
            this.lciBANT.Text = "In BA ngoại trú:";
            this.lciBANT.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBANT.TextSize = new System.Drawing.Size(100, 20);
            this.lciBANT.TextToControlDistance = 5;
            // 
            // layoutControlItem29
            // 
            this.layoutControlItem29.Control = this.xtraTabControl1;
            this.layoutControlItem29.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem29.Name = "layoutControlItem29";
            this.layoutControlItem29.Size = new System.Drawing.Size(660, 58);
            this.layoutControlItem29.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem29.TextVisible = false;
            // 
            // layoutControlItem30
            // 
            this.layoutControlItem30.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem30.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem30.Control = this.cboEndDeptSubs;
            this.layoutControlItem30.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem30.Name = "layoutControlItem30";
            this.layoutControlItem30.OptionsToolTip.ToolTip = "Chọn người ký thay trưởng khoa";
            this.layoutControlItem30.Size = new System.Drawing.Size(326, 24);
            this.layoutControlItem30.Text = "Người ký thay TK:";
            this.layoutControlItem30.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem30.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem30.TextToControlDistance = 5;
            // 
            // layoutControlItem31
            // 
            this.layoutControlItem31.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem31.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem31.Control = this.cboHospSubs;
            this.layoutControlItem31.Location = new System.Drawing.Point(326, 48);
            this.layoutControlItem31.Name = "layoutControlItem31";
            this.layoutControlItem31.OptionsToolTip.ToolTip = "Chọn người ký thay giám đốc viện";
            this.layoutControlItem31.Size = new System.Drawing.Size(334, 24);
            this.layoutControlItem31.Text = "Người ký thay GĐ:";
            this.layoutControlItem31.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem31.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem31.TextToControlDistance = 5;
            // 
            // layoutControlItem34
            // 
            this.layoutControlItem34.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem34.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem34.Control = this.chkPrintExam;
            this.layoutControlItem34.Location = new System.Drawing.Point(288, 0);
            this.layoutControlItem34.Name = "layoutControlItem34";
            this.layoutControlItem34.Size = new System.Drawing.Size(175, 24);
            this.layoutControlItem34.Text = "Phiếu khám bệnh:";
            this.layoutControlItem34.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem34.TextSize = new System.Drawing.Size(130, 20);
            this.layoutControlItem34.TextToControlDistance = 5;
            // 
            // layoutControlItem35
            // 
            this.layoutControlItem35.Control = this.chkSignExam;
            this.layoutControlItem35.Location = new System.Drawing.Point(463, 0);
            this.layoutControlItem35.Name = "layoutControlItem35";
            this.layoutControlItem35.Size = new System.Drawing.Size(42, 24);
            this.layoutControlItem35.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem35.TextVisible = false;
            // 
            // lciSoLuuTruBA
            // 
            this.lciSoLuuTruBA.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSoLuuTruBA.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSoLuuTruBA.Control = this.lblSoLuuTruBA;
            this.lciSoLuuTruBA.CustomizationFormText = "Số lưu trữ BA ngoại trú:";
            this.lciSoLuuTruBA.Location = new System.Drawing.Point(326, 24);
            this.lciSoLuuTruBA.Name = "lciSoLuuTruBA";
            this.lciSoLuuTruBA.OptionsToolTip.ToolTip = "Số lưu trữ bệnh án ngoại trú";
            this.lciSoLuuTruBA.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciSoLuuTruBA.Size = new System.Drawing.Size(334, 24);
            this.lciSoLuuTruBA.Text = "Số LTBA ngoại trú:";
            this.lciSoLuuTruBA.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSoLuuTruBA.TextSize = new System.Drawing.Size(100, 20);
            this.lciSoLuuTruBA.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(505, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(155, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // cboTreatmentEndTypeExt
            // 
            this.cboTreatmentEndTypeExt.Location = new System.Drawing.Point(107, 174);
            this.cboTreatmentEndTypeExt.Name = "cboTreatmentEndTypeExt";
            this.cboTreatmentEndTypeExt.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTreatmentEndTypeExt.Properties.AutoComplete = false;
            this.cboTreatmentEndTypeExt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, true)});
            this.cboTreatmentEndTypeExt.Properties.NullText = "";
            this.cboTreatmentEndTypeExt.Properties.View = this.gridView1;
            this.cboTreatmentEndTypeExt.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.gridLookUpEdit1_Properties_ButtonClick);
            this.cboTreatmentEndTypeExt.Size = new System.Drawing.Size(182, 20);
            this.cboTreatmentEndTypeExt.StyleController = this.layoutControl1;
            this.cboTreatmentEndTypeExt.TabIndex = 16;
            this.cboTreatmentEndTypeExt.EditValueChanged += new System.EventHandler(this.cboTreatmentEndTypeExt_EditValueChanged);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.btnChiDinhDichVuHenKham);
            this.layoutControl2.Controls.Add(this.labelControl1);
            this.layoutControl2.Location = new System.Drawing.Point(2, 278);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(225, 341, 250, 350);
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(656, 29);
            this.layoutControl2.TabIndex = 14;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // btnChiDinhDichVuHenKham
            // 
            this.btnChiDinhDichVuHenKham.Image = ((System.Drawing.Image)(resources.GetObject("btnChiDinhDichVuHenKham.Image")));
            this.btnChiDinhDichVuHenKham.Location = new System.Drawing.Point(155, 2);
            this.btnChiDinhDichVuHenKham.Name = "btnChiDinhDichVuHenKham";
            this.btnChiDinhDichVuHenKham.Size = new System.Drawing.Size(49, 22);
            this.btnChiDinhDichVuHenKham.StyleController = this.layoutControl2;
            this.btnChiDinhDichVuHenKham.TabIndex = 5;
            this.btnChiDinhDichVuHenKham.Click += new System.EventHandler(this.btnChiDinhChoLanKhamSau_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(27, 2);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(124, 20);
            this.labelControl1.StyleController = this.layoutControl2;
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "Chỉ định dịch vụ hẹn khám";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem12,
            this.layoutControlItem13,
            this.emptySpaceItem1});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(656, 29);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.labelControl1;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(153, 29);
            this.layoutControlItem12.Text = " ";
            this.layoutControlItem12.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem12.TextSize = new System.Drawing.Size(20, 20);
            this.layoutControlItem12.TextToControlDistance = 5;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.btnChiDinhDichVuHenKham;
            this.layoutControlItem13.Location = new System.Drawing.Point(153, 0);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(53, 29);
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(206, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(450, 29);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // chkPrintBordereau
            // 
            this.chkPrintBordereau.Location = new System.Drawing.Point(426, 216);
            this.chkPrintBordereau.Name = "chkPrintBordereau";
            this.chkPrintBordereau.Properties.Caption = ":In";
            this.chkPrintBordereau.Properties.FullFocusRect = true;
            this.chkPrintBordereau.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkPrintBordereau.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkPrintBordereau.Size = new System.Drawing.Size(36, 19);
            this.chkPrintBordereau.StyleController = this.layoutControl1;
            this.chkPrintBordereau.TabIndex = 10;
            this.chkPrintBordereau.CheckedChanged += new System.EventHandler(this.chkPrintBordereau_CheckedChanged);
            // 
            // lblOutCode
            // 
            this.lblOutCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOutCode.Location = new System.Drawing.Point(105, 196);
            this.lblOutCode.Name = "lblOutCode";
            this.lblOutCode.Size = new System.Drawing.Size(186, 20);
            this.lblOutCode.StyleController = this.layoutControl1;
            this.lblOutCode.TabIndex = 13;
            // 
            // lblEndCode
            // 
            this.lblEndCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblEndCode.Location = new System.Drawing.Point(426, 196);
            this.lblEndCode.Name = "lblEndCode";
            this.lblEndCode.Size = new System.Drawing.Size(234, 20);
            this.lblEndCode.StyleController = this.layoutControl1;
            this.lblEndCode.TabIndex = 12;
            // 
            // chkPrintAppoinment
            // 
            this.chkPrintAppoinment.Location = new System.Drawing.Point(102, 216);
            this.chkPrintAppoinment.Name = "chkPrintAppoinment";
            this.chkPrintAppoinment.Properties.Caption = ":In";
            this.chkPrintAppoinment.Properties.FullFocusRect = true;
            this.chkPrintAppoinment.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkPrintAppoinment.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkPrintAppoinment.Size = new System.Drawing.Size(39, 19);
            this.chkPrintAppoinment.StyleController = this.layoutControl1;
            this.chkPrintAppoinment.TabIndex = 9;
            this.chkPrintAppoinment.CheckedChanged += new System.EventHandler(this.chkPrintAppoinment_CheckedChanged);
            // 
            // cboTreatmentEndType
            // 
            this.cboTreatmentEndType.Location = new System.Drawing.Point(107, 150);
            this.cboTreatmentEndType.Name = "cboTreatmentEndType";
            this.cboTreatmentEndType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTreatmentEndType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboTreatmentEndType.Properties.NullText = "";
            this.cboTreatmentEndType.Properties.View = this.gridLookUpEdit1View;
            this.cboTreatmentEndType.Size = new System.Drawing.Size(170, 20);
            this.cboTreatmentEndType.StyleController = this.layoutControl1;
            this.cboTreatmentEndType.TabIndex = 7;
            this.cboTreatmentEndType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTreatmentEndType_Closed);
            this.cboTreatmentEndType.EditValueChanged += new System.EventHandler(this.cboTreatmentEndType_EditValueChanged);
            this.cboTreatmentEndType.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboTreatmentEndType_PreviewKeyDown);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // dtEndTime
            // 
            this.dtEndTime.EditValue = null;
            this.dtEndTime.Location = new System.Drawing.Point(416, 126);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtEndTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtEndTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dtEndTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtEndTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dtEndTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtEndTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm:ss";
            this.dtEndTime.Size = new System.Drawing.Size(242, 20);
            this.dtEndTime.StyleController = this.layoutControl1;
            this.dtEndTime.TabIndex = 5;
            this.dtEndTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtEndTime_Closed);
            this.dtEndTime.EditValueChanged += new System.EventHandler(this.dtEndTime_EditValueChanged);
            this.dtEndTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtEndTime_PreviewKeyDown);
            // 
            // dtTimeIn
            // 
            this.dtTimeIn.EditValue = null;
            this.dtTimeIn.Location = new System.Drawing.Point(107, 126);
            this.dtTimeIn.Name = "dtTimeIn";
            this.dtTimeIn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeIn.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeIn.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTimeIn.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTimeIn.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTimeIn.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTimeIn.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtTimeIn.Size = new System.Drawing.Size(170, 20);
            this.dtTimeIn.StyleController = this.layoutControl1;
            this.dtTimeIn.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.lciChiDinhDichVuhenKham,
            this.layoutControlItem10,
            this.layoutControlItem9,
            this.layoutControlItem6,
            this.layoutControlItem16,
            this.layoutControlIPanelUCExtend,
            this.lciIcdText,
            this.layoutControlItem17,
            this.layoutControlItem18,
            this.layoutControlItem3,
            this.layoutControlItem22,
            this.layoutControlItem24,
            this.layoutControlItem8,
            this.layoutControlItem19,
            this.emptySpaceItem3,
            this.emptySpaceItem4,
            this.layoutControlItem7,
            this.layoutControlItem11,
            this.layoutControlItem15,
            this.emptySpaceItem5,
            this.layoutControlItem20,
            this.layoutControlItem14,
            this.layoutControlItem5,
            this.layoutControlItem21,
            this.layoutControlItem23,
            this.layoutControlItem25,
            this.emptySpaceItem6,
            this.layoutControlItem27,
            this.emptySpaceItem7,
            this.layoutControlItem28,
            this.lciIsExpXml4210Collinear,
            this.emptySpaceItem8,
            this.lciCareer,
            this.layoutControlItem32,
            this.layoutControlItem26,
            this.layoutControlItem33});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 532);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.dtTimeIn;
            this.layoutControlItem1.Enabled = false;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 124);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(279, 24);
            this.layoutControlItem1.Text = "Thời gian vào:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.dtEndTime;
            this.layoutControlItem2.Location = new System.Drawing.Point(279, 124);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(381, 24);
            this.layoutControlItem2.Text = "Thời gian ra:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(130, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem4.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem4.Control = this.cboTreatmentEndType;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 148);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(279, 24);
            this.layoutControlItem4.Text = "Loại ra viện:";
            this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem4.TextToControlDistance = 5;
            // 
            // lciChiDinhDichVuhenKham
            // 
            this.lciChiDinhDichVuhenKham.Control = this.layoutControl2;
            this.lciChiDinhDichVuhenKham.Location = new System.Drawing.Point(0, 276);
            this.lciChiDinhDichVuhenKham.Name = "lciChiDinhDichVuhenKham";
            this.lciChiDinhDichVuhenKham.Size = new System.Drawing.Size(660, 33);
            this.lciChiDinhDichVuhenKham.TextSize = new System.Drawing.Size(0, 0);
            this.lciChiDinhDichVuhenKham.TextVisible = false;
            this.lciChiDinhDichVuhenKham.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem10.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem10.Control = this.lblOutCode;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 196);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.OptionsToolTip.ToolTip = "Số chuyển viện";
            this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem10.Size = new System.Drawing.Size(291, 20);
            this.layoutControlItem10.Text = "Số CV:";
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem10.TextToControlDistance = 5;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem9.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem9.Control = this.lblEndCode;
            this.layoutControlItem9.Location = new System.Drawing.Point(291, 196);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem9.Size = new System.Drawing.Size(369, 20);
            this.layoutControlItem9.Text = "Số ra viện:";
            this.layoutControlItem9.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem9.TextSize = new System.Drawing.Size(130, 20);
            this.layoutControlItem9.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem6.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem6.Control = this.chkPrintAppoinment;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 216);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem6.Size = new System.Drawing.Size(141, 20);
            this.layoutControlItem6.Text = "Phiếu hẹn khám:";
            this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem6.TextToControlDistance = 2;
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.Control = this.layoutControl3;
            this.layoutControlItem16.Location = new System.Drawing.Point(0, 402);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem16.Size = new System.Drawing.Size(660, 130);
            this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem16.TextVisible = false;
            // 
            // layoutControlIPanelUCExtend
            // 
            this.layoutControlIPanelUCExtend.Control = this.panelExamTreatmentFinish;
            this.layoutControlIPanelUCExtend.Location = new System.Drawing.Point(0, 309);
            this.layoutControlIPanelUCExtend.Name = "layoutControlIPanelUCExtend";
            this.layoutControlIPanelUCExtend.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlIPanelUCExtend.Size = new System.Drawing.Size(660, 93);
            this.layoutControlIPanelUCExtend.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlIPanelUCExtend.TextVisible = false;
            this.layoutControlIPanelUCExtend.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciIcdText
            // 
            this.lciIcdText.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIcdText.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIcdText.Control = this.txtIcdCode;
            this.lciIcdText.Location = new System.Drawing.Point(0, 0);
            this.lciIcdText.Name = "lciIcdText";
            this.lciIcdText.OptionsToolTip.ToolTip = "Chẩn đoán ra viện";
            this.lciIcdText.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciIcdText.Size = new System.Drawing.Size(202, 26);
            this.lciIcdText.Text = "CĐ ra viện:";
            this.lciIcdText.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIcdText.TextSize = new System.Drawing.Size(100, 20);
            this.lciIcdText.TextToControlDistance = 5;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.Control = this.panelControlIcds;
            this.layoutControlItem17.Location = new System.Drawing.Point(202, 0);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem17.Size = new System.Drawing.Size(358, 26);
            this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem17.TextVisible = false;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.Control = this.chkEditIcd;
            this.layoutControlItem18.Location = new System.Drawing.Point(560, 0);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.Size = new System.Drawing.Size(68, 26);
            this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem18.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem3.Control = this.txtTraditionalIcdCode;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 52);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.OptionsToolTip.ToolTip = "Chẩn đoán ra viện y học cổ truyền";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem3.Size = new System.Drawing.Size(202, 24);
            this.layoutControlItem3.Text = "CĐ ra viện YHCT:";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // layoutControlItem22
            // 
            this.layoutControlItem22.Control = this.chkTraditionalIcd;
            this.layoutControlItem22.Location = new System.Drawing.Point(560, 52);
            this.layoutControlItem22.Name = "layoutControlItem22";
            this.layoutControlItem22.Size = new System.Drawing.Size(100, 24);
            this.layoutControlItem22.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem22.TextVisible = false;
            // 
            // layoutControlItem24
            // 
            this.layoutControlItem24.Control = this.panel1;
            this.layoutControlItem24.Location = new System.Drawing.Point(202, 52);
            this.layoutControlItem24.Name = "layoutControlItem24";
            this.layoutControlItem24.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem24.Size = new System.Drawing.Size(358, 24);
            this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem24.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.txtIcdSubCode;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 76);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.OptionsToolTip.ToolTip = "Chẩn đoán nhập viện y học cổ truyền kèm theo";
            this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem8.Size = new System.Drawing.Size(202, 24);
            this.layoutControlItem8.Text = "CĐ nhập viện YHCT (kèm theo):";
            this.layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem8.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem8.TextToControlDistance = 5;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.Control = this.txtIcdText;
            this.layoutControlItem19.Location = new System.Drawing.Point(202, 76);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem19.Size = new System.Drawing.Size(458, 24);
            this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem19.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(508, 172);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(152, 24);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.Location = new System.Drawing.Point(179, 216);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(112, 19);
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem7.Control = this.chkPrintBordereau;
            this.layoutControlItem7.Location = new System.Drawing.Point(291, 216);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem7.Size = new System.Drawing.Size(171, 20);
            this.layoutControlItem7.Text = "Bảng kê:";
            this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(130, 20);
            this.layoutControlItem7.TextToControlDistance = 5;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.chkSignAppoinment;
            this.layoutControlItem11.Location = new System.Drawing.Point(141, 216);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem11.Size = new System.Drawing.Size(38, 19);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.chkSignBordereau;
            this.layoutControlItem15.Location = new System.Drawing.Point(462, 216);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(42, 60);
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // emptySpaceItem5
            // 
            this.emptySpaceItem5.AllowHotTrack = false;
            this.emptySpaceItem5.Location = new System.Drawing.Point(504, 216);
            this.emptySpaceItem5.Name = "emptySpaceItem5";
            this.emptySpaceItem5.Size = new System.Drawing.Size(156, 60);
            this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem20.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem20.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem20.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem20.Control = this.cboTreatmentResult;
            this.layoutControlItem20.Location = new System.Drawing.Point(279, 148);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.Size = new System.Drawing.Size(381, 24);
            this.layoutControlItem20.Text = "Kết quả:";
            this.layoutControlItem20.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem20.TextSize = new System.Drawing.Size(130, 20);
            this.layoutControlItem20.TextToControlDistance = 5;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem14.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem14.Control = this.cboTreatmentEndTypeExt;
            this.layoutControlItem14.Location = new System.Drawing.Point(0, 172);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.OptionsToolTip.ToolTip = "Thông tin bổ sung";
            this.layoutControlItem14.Size = new System.Drawing.Size(291, 24);
            this.layoutControlItem14.Text = "TT bổ sung:";
            this.layoutControlItem14.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem14.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem14.TextToControlDistance = 5;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem5.Control = this.chkPrintBHXH;
            this.layoutControlItem5.Location = new System.Drawing.Point(291, 172);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.OptionsToolTip.ToolTip = "In phiếu nghỉ hưởng bảo hiểm xã hội";
            this.layoutControlItem5.Size = new System.Drawing.Size(175, 24);
            this.layoutControlItem5.Text = "Phiếu NH BHXH:";
            this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(130, 20);
            this.layoutControlItem5.TextToControlDistance = 5;
            // 
            // layoutControlItem21
            // 
            this.layoutControlItem21.Control = this.chkSignBHXH;
            this.layoutControlItem21.Location = new System.Drawing.Point(466, 172);
            this.layoutControlItem21.Name = "layoutControlItem21";
            this.layoutControlItem21.Size = new System.Drawing.Size(42, 24);
            this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem21.TextVisible = false;
            // 
            // layoutControlItem23
            // 
            this.layoutControlItem23.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem23.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem23.Control = this.chkInPhieuTrichLuc;
            this.layoutControlItem23.Location = new System.Drawing.Point(0, 236);
            this.layoutControlItem23.Name = "layoutControlItem23";
            this.layoutControlItem23.OptionsToolTip.ToolTip = "Phiếu kết quả khám bệnh (trích lục)";
            this.layoutControlItem23.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem23.Size = new System.Drawing.Size(141, 20);
            this.layoutControlItem23.Text = "Phiếu trích lục:";
            this.layoutControlItem23.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem23.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem23.TextToControlDistance = 5;
            // 
            // layoutControlItem25
            // 
            this.layoutControlItem25.Control = this.chkKyPhieuTrichLuc;
            this.layoutControlItem25.Location = new System.Drawing.Point(141, 235);
            this.layoutControlItem25.Name = "layoutControlItem25";
            this.layoutControlItem25.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem25.Size = new System.Drawing.Size(38, 19);
            this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem25.TextVisible = false;
            // 
            // emptySpaceItem6
            // 
            this.emptySpaceItem6.AllowHotTrack = false;
            this.emptySpaceItem6.Location = new System.Drawing.Point(179, 235);
            this.emptySpaceItem6.Name = "emptySpaceItem6";
            this.emptySpaceItem6.Size = new System.Drawing.Size(112, 19);
            this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem27
            // 
            this.layoutControlItem27.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem27.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem27.Control = this.chkPrintPrescription;
            this.layoutControlItem27.Location = new System.Drawing.Point(0, 256);
            this.layoutControlItem27.Name = "layoutControlItem27";
            this.layoutControlItem27.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem27.Size = new System.Drawing.Size(141, 20);
            this.layoutControlItem27.Text = "Đơn thuốc:";
            this.layoutControlItem27.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem27.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem27.TextToControlDistance = 5;
            // 
            // emptySpaceItem7
            // 
            this.emptySpaceItem7.AllowHotTrack = false;
            this.emptySpaceItem7.Location = new System.Drawing.Point(141, 254);
            this.emptySpaceItem7.Name = "emptySpaceItem7";
            this.emptySpaceItem7.Size = new System.Drawing.Size(150, 22);
            this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem28
            // 
            this.layoutControlItem28.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem28.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem28.Control = this.chkPrintHosTransfer;
            this.layoutControlItem28.Location = new System.Drawing.Point(291, 236);
            this.layoutControlItem28.Name = "layoutControlItem28";
            this.layoutControlItem28.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem28.Size = new System.Drawing.Size(171, 20);
            this.layoutControlItem28.Text = "Phiếu chuyển viện:";
            this.layoutControlItem28.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem28.TextSize = new System.Drawing.Size(130, 20);
            this.layoutControlItem28.TextToControlDistance = 5;
            // 
            // lciIsExpXml4210Collinear
            // 
            this.lciIsExpXml4210Collinear.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIsExpXml4210Collinear.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIsExpXml4210Collinear.Control = this.chkIsExpXml4210Collinear;
            this.lciIsExpXml4210Collinear.Location = new System.Drawing.Point(291, 256);
            this.lciIsExpXml4210Collinear.Name = "lciIsExpXml4210Collinear";
            this.lciIsExpXml4210Collinear.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciIsExpXml4210Collinear.Size = new System.Drawing.Size(171, 20);
            this.lciIsExpXml4210Collinear.Text = "Xuất xml thông tuyến:";
            this.lciIsExpXml4210Collinear.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIsExpXml4210Collinear.TextSize = new System.Drawing.Size(130, 20);
            this.lciIsExpXml4210Collinear.TextToControlDistance = 5;
            // 
            // emptySpaceItem8
            // 
            this.emptySpaceItem8.AllowHotTrack = false;
            this.emptySpaceItem8.Location = new System.Drawing.Point(279, 100);
            this.emptySpaceItem8.Name = "emptySpaceItem8";
            this.emptySpaceItem8.Size = new System.Drawing.Size(381, 24);
            this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciCareer
            // 
            this.lciCareer.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciCareer.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciCareer.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCareer.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCareer.Control = this.cboCareer;
            this.lciCareer.Location = new System.Drawing.Point(0, 100);
            this.lciCareer.Name = "lciCareer";
            this.lciCareer.Size = new System.Drawing.Size(279, 24);
            this.lciCareer.Text = "Nghề nghiệp:";
            this.lciCareer.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCareer.TextSize = new System.Drawing.Size(100, 20);
            this.lciCareer.TextToControlDistance = 5;
            // 
            // layoutControlItem32
            // 
            this.layoutControlItem32.Control = this.panelControlSubIcd;
            this.layoutControlItem32.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem32.Name = "layoutControlItem32";
            this.layoutControlItem32.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem32.Size = new System.Drawing.Size(628, 26);
            this.layoutControlItem32.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem32.TextVisible = false;
            // 
            // layoutControlItem26
            // 
            this.layoutControlItem26.Control = this.btnICDInformation;
            this.layoutControlItem26.Location = new System.Drawing.Point(628, 0);
            this.layoutControlItem26.Name = "layoutControlItem26";
            this.layoutControlItem26.Size = new System.Drawing.Size(32, 26);
            this.layoutControlItem26.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem26.TextVisible = false;
            // 
            // layoutControlItem33
            // 
            this.layoutControlItem33.Control = this.btnCheckIcd;
            this.layoutControlItem33.Location = new System.Drawing.Point(628, 26);
            this.layoutControlItem33.Name = "layoutControlItem33";
            this.layoutControlItem33.Size = new System.Drawing.Size(32, 26);
            this.layoutControlItem33.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem33.TextVisible = false;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // UCExamTreatmentFinish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCExamTreatmentFinish";
            this.Size = new System.Drawing.Size(660, 532);
            this.Load += new System.EventHandler(this.UCExamTreatmentFinish_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlSubIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCareer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsExpXml4210Collinear.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintHosTransfer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintPrescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkKyPhieuTrichLuc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkInPhieuTrichLuc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignBHXH.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintBHXH.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignBordereau.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignAppoinment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdText.Properties)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboTraditionalIcds.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridViewWithFilterMultiColumn1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTraditionalIcdMainText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdSubCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTraditionalIcd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTraditionalIcdCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTreatmentResult.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditIcd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlIcds)).EndInit();
            this.panelControlIcds.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboIcds.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdMainText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkSignExam.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintExam.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboHospSubs.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndDeptSubs.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtAdviseNew.Properties)).EndInit();
            this.xtraTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtConclusionNew.Properties)).EndInit();
            this.xtraTabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memNote.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem36)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProgram.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewProgram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCapSoLuuTruBA.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBANT.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupHisPatientProgram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkCapSoLuuTruBA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientProgram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBANT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSoLuuTruBA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTreatmentEndTypeExt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintBordereau.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintAppoinment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTreatmentEndType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeIn.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeIn.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChiDinhDichVuhenKham)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlIPanelUCExtend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsExpXml4210Collinear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCareer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.CheckEdit chkPrintBordereau;
        private DevExpress.XtraEditors.CheckEdit chkPrintAppoinment;
        private DevExpress.XtraEditors.GridLookUpEdit cboTreatmentEndType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.DateEdit dtEndTime;
        private DevExpress.XtraEditors.DateEdit dtTimeIn;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.LabelControl lblOutCode;
        private DevExpress.XtraEditors.LabelControl lblEndCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraEditors.SimpleButton btnChiDinhDichVuHenKham;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControlItem lciChiDinhDichVuhenKham;
        private DevExpress.XtraEditors.CheckEdit chkBANT;
        private DevExpress.XtraEditors.GridLookUpEdit cboTreatmentEndTypeExt;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupHisPatientProgram;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraEditors.GridLookUpEdit cboProgram;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewProgram;
        private DevExpress.XtraEditors.LabelControl lblSoLuuTruBA;
        private DevExpress.XtraEditors.CheckEdit chkCapSoLuuTruBA;
        private DevExpress.XtraLayout.LayoutControlItem lciChkCapSoLuuTruBA;
        private DevExpress.XtraLayout.LayoutControlItem lciSoLuuTruBA;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientProgram;
        private DevExpress.XtraEditors.XtraScrollableControl panelExamTreatmentFinish;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlIPanelUCExtend;
        private DevExpress.XtraEditors.PanelControl panelControlIcds;
        private DevExpress.XtraEditors.TextEdit txtIcdCode;
        private DevExpress.XtraLayout.LayoutControlItem lciIcdText;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraEditors.TextEdit txtIcdMainText;
        private DevExpress.XtraEditors.CheckEdit chkEditIcd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboIcds;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridLookUpEditWithFilterMultiColumn1View;
        private DevExpress.XtraEditors.TextEdit txtIcdSubCode;
        private DevExpress.XtraEditors.TextEdit txtIcdText;
        private DevExpress.XtraEditors.GridLookUpEdit cboTreatmentResult;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
        private DevExpress.XtraEditors.CheckEdit chkTraditionalIcd;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboTraditionalIcds;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridViewWithFilterMultiColumn1;
        private DevExpress.XtraEditors.TextEdit txtTraditionalIcdCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem22;
        private DevExpress.XtraEditors.TextEdit txtTraditionalIcdMainText;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem24;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraLayout.LayoutControlItem lciBANT;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
        private DevExpress.XtraEditors.CheckEdit chkSignAppoinment;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraEditors.CheckEdit chkSignBordereau;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem5;
        private DevExpress.XtraEditors.CheckEdit chkSignBHXH;
        private DevExpress.XtraEditors.CheckEdit chkPrintBHXH;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem21;
        private DevExpress.XtraEditors.CheckEdit chkKyPhieuTrichLuc;
        private DevExpress.XtraEditors.CheckEdit chkInPhieuTrichLuc;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem23;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem25;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem6;
        private DevExpress.XtraEditors.SimpleButton btnICDInformation;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem26;
        private DevExpress.XtraEditors.CheckEdit chkPrintPrescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem27;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem7;
		private DevExpress.XtraEditors.CheckEdit chkPrintHosTransfer;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem28;
		private DevExpress.XtraEditors.CheckEdit chkIsExpXml4210Collinear;
		private DevExpress.XtraLayout.LayoutControlItem lciIsExpXml4210Collinear;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem8;
        private DevExpress.XtraEditors.GridLookUpEdit cboCareer;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
        private DevExpress.XtraLayout.LayoutControlItem lciCareer;
		private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
		private DevExpress.XtraEditors.MemoEdit txtAdviseNew;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
		private DevExpress.XtraEditors.MemoEdit txtConclusionNew;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem29;
		private System.Windows.Forms.Timer timer1;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboHospSubs;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn gridLookUpEdit2View;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboEndDeptSubs;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn gridView4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem30;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem31;
        private DevExpress.XtraEditors.PanelControl panelControlSubIcd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem32;
        private DevExpress.XtraEditors.SimpleButton btnCheckIcd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem33;
        private DevExpress.XtraEditors.CheckEdit chkSignExam;
        private DevExpress.XtraEditors.CheckEdit chkPrintExam;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem34;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem35;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage3;
        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraEditors.MemoEdit memNote;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem36;
    }
}

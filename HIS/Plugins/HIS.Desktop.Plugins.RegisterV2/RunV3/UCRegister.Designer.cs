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
using DevExpress.XtraEditors.Controls;
using System;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    partial class UCRegister
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
			StopTimer(currentModule.ModuleLink, "timerRefeshAutoCreateBill");
			base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition7 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition8 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition9 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition10 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition11 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition12 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition13 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition14 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition15 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition16 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition17 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition18 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition19 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.chkAutoPay = new DevExpress.XtraEditors.CheckEdit();
            this.chkAutoDeposit = new DevExpress.XtraEditors.CheckEdit();
            this.chkAssignDoctor = new DevExpress.XtraEditors.CheckEdit();
            this.chkPrintExam = new DevExpress.XtraEditors.CheckEdit();
            this.chkAutoCreateBill = new DevExpress.XtraEditors.CheckEdit();
            this.chkPrintPatientCard = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem27 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAutoDeposit = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem26 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.ucCheckTT1 = new HIS.UC.UCCheckTT.UCCheckTT();
            this.ucPlusInfo1 = new HIS.UC.PlusInfo.UCPlusInfo();
            this.ucServiceRoomInfo1 = new HIS.UC.UCServiceRoomInfo.UCServiceRoomInfo();
            this.ucImageInfo1 = new HIS.UC.UCImageInfo.UCImageInfo();
            this.ucRelativeInfo1 = new HIS.UC.UCRelativeInfo.UCRelativeInfo();
            this.ucOtherServiceReqInfo1 = new HIS.UC.UCOtherServiceReqInfo.UCOtherServiceReqInfo();
            this.ucHeinInfo1 = new HIS.UC.UCHeniInfo.UCHeinInfo();
            this.ucAddressCombo1 = new HIS.UC.AddressCombo.UCAddressCombo();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.pnlServiceRoomInfomation = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciUCHeinInfo = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciUCServiceRoomInfo = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnTTChuyenTuyen = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.txtTo = new DevExpress.XtraEditors.TextEdit();
            this.txtFrom = new DevExpress.XtraEditors.TextEdit();
            this.btnGiayTo = new DevExpress.XtraEditors.SimpleButton();
            this.btnDepositRequest = new DevExpress.XtraEditors.SimpleButton();
            this.lblRegisterNumOrder = new DevExpress.XtraEditors.LabelControl();
            this.dropDownButton__Other = new DevExpress.XtraEditors.DropDownButton();
            this.btnRecallPatient = new DevExpress.XtraEditors.SimpleButton();
            this.btnCallPatient = new DevExpress.XtraEditors.SimpleButton();
            this.txtGateNumber = new DevExpress.XtraEditors.ButtonEdit();
            this.txtStepNumber = new DevExpress.XtraEditors.TextEdit();
            this.cboCashierRoom = new DevExpress.XtraEditors.LookUpEdit();
            this.btnSaveAndPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnPatientNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveAndAssain = new DevExpress.XtraEditors.SimpleButton();
            this.btnDepositDetail = new DevExpress.XtraEditors.SimpleButton();
            this.btnTreatmentBedRoom = new DevExpress.XtraEditors.SimpleButton();
            this.btnNewContinue = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcibtnPatientNewInfo = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcibtnDepositDetail = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRegisterNumOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcibtnDepositRequest = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem29 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem30 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.timerInitForm = new System.Windows.Forms.Timer();
            this.timerRefeshAutoCreateBill = new System.Windows.Forms.Timer();
            this.chkSignExam = new DevExpress.XtraEditors.CheckEdit();
            this.ucPatientRaw1 = new HIS.UC.UCPatientRaw.UCPatientRaw();
            this.layoutControlItem31 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoPay.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoDeposit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAssignDoctor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintExam.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoCreateBill.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintPatientCard.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAutoDeposit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlServiceRoomInfomation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUCHeinInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUCServiceRoomInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGateNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStepNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCashierRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnPatientNewInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnDepositDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRegisterNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnDepositRequest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignExam.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.layoutControl4);
            this.layoutControl1.Controls.Add(this.ucCheckTT1);
            this.layoutControl1.Controls.Add(this.ucPlusInfo1);
            this.layoutControl1.Controls.Add(this.ucServiceRoomInfo1);
            this.layoutControl1.Controls.Add(this.ucImageInfo1);
            this.layoutControl1.Controls.Add(this.ucRelativeInfo1);
            this.layoutControl1.Controls.Add(this.ucOtherServiceReqInfo1);
            this.layoutControl1.Controls.Add(this.ucHeinInfo1);
            this.layoutControl1.Controls.Add(this.ucAddressCombo1);
            this.layoutControl1.Controls.Add(this.ucPatientRaw1);
            this.layoutControl1.Location = new System.Drawing.Point(2, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1363, 853);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.chkSignExam);
            this.layoutControl4.Controls.Add(this.chkAutoPay);
            this.layoutControl4.Controls.Add(this.chkAutoDeposit);
            this.layoutControl4.Controls.Add(this.chkAssignDoctor);
            this.layoutControl4.Controls.Add(this.chkPrintExam);
            this.layoutControl4.Controls.Add(this.chkAutoCreateBill);
            this.layoutControl4.Controls.Add(this.chkPrintPatientCard);
            this.layoutControl4.Location = new System.Drawing.Point(908, 763);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.Root = this.layoutControlGroup3;
            this.layoutControl4.Size = new System.Drawing.Size(455, 90);
            this.layoutControl4.TabIndex = 14;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // chkAutoPay
            // 
            this.chkAutoPay.Location = new System.Drawing.Point(306, 2);
            this.chkAutoPay.Name = "chkAutoPay";
            this.chkAutoPay.Properties.Caption = "Thu tiền qua thẻ";
            this.chkAutoPay.Size = new System.Drawing.Size(147, 19);
            this.chkAutoPay.StyleController = this.layoutControl4;
            this.chkAutoPay.TabIndex = 17;
            this.chkAutoPay.ToolTip = "Tự động thực hiện thu tiền từ tài khoản ngân hàng của bệnh nhân nếu bệnh nhân có " +
    "thông tin Thẻ khám chữa bệnh thông minh trong trường hợp có check chọn \"Xuất biê" +
    "n lai/hóa đơn\" hoặc \"Tạm thu\"";
            this.chkAutoPay.CheckedChanged += new System.EventHandler(this.chkAutoPaid_CheckedChanged);
            // 
            // chkAutoDeposit
            // 
            this.chkAutoDeposit.Location = new System.Drawing.Point(153, 2);
            this.chkAutoDeposit.Name = "chkAutoDeposit";
            this.chkAutoDeposit.Properties.Caption = "Tạm thu";
            this.chkAutoDeposit.Size = new System.Drawing.Size(149, 19);
            this.chkAutoDeposit.StyleController = this.layoutControl4;
            this.chkAutoDeposit.TabIndex = 16;
            this.chkAutoDeposit.ToolTip = "Chỉ tự động tạm thu với bệnh nhân không phải bhyt";
            this.chkAutoDeposit.CheckedChanged += new System.EventHandler(this.chkAutoDeposit_CheckedChanged);
            // 
            // chkAssignDoctor
            // 
            this.chkAssignDoctor.Location = new System.Drawing.Point(2, 25);
            this.chkAssignDoctor.Name = "chkAssignDoctor";
            this.chkAssignDoctor.Properties.Caption = "Chỉ định BS";
            this.chkAssignDoctor.Size = new System.Drawing.Size(147, 19);
            this.chkAssignDoctor.StyleController = this.layoutControl4;
            this.chkAssignDoctor.TabIndex = 15;
            this.chkAssignDoctor.ToolTip = "Chỉ định bác sĩ khám";
            this.chkAssignDoctor.CheckedChanged += new System.EventHandler(this.chkAssignDoctor_CheckedChanged);
            // 
            // chkPrintExam
            // 
            this.chkPrintExam.Location = new System.Drawing.Point(153, 25);
            this.chkPrintExam.Name = "chkPrintExam";
            this.chkPrintExam.Properties.Caption = "In phiếu khám";
            this.chkPrintExam.Size = new System.Drawing.Size(149, 19);
            this.chkPrintExam.StyleController = this.layoutControl4;
            this.chkPrintExam.TabIndex = 14;
            this.chkPrintExam.CheckedChanged += new System.EventHandler(this.chkPrintExam_CheckedChanged);
            // 
            // chkAutoCreateBill
            // 
            this.chkAutoCreateBill.Location = new System.Drawing.Point(2, 2);
            this.chkAutoCreateBill.Name = "chkAutoCreateBill";
            this.chkAutoCreateBill.Properties.Caption = "Xuất biên lai/hóa đơn";
            this.chkAutoCreateBill.Size = new System.Drawing.Size(147, 19);
            this.chkAutoCreateBill.StyleController = this.layoutControl4;
            this.chkAutoCreateBill.TabIndex = 12;
            this.chkAutoCreateBill.ToolTip = "Chỉ tự động xuất với bệnh nhân không phải bhyt";
            this.chkAutoCreateBill.CheckedChanged += new System.EventHandler(this.chkAutoCreateBill_CheckedChanged);
            // 
            // chkPrintPatientCard
            // 
            this.chkPrintPatientCard.Location = new System.Drawing.Point(306, 25);
            this.chkPrintPatientCard.Name = "chkPrintPatientCard";
            this.chkPrintPatientCard.Properties.Caption = "In thẻ BN";
            this.chkPrintPatientCard.Size = new System.Drawing.Size(147, 19);
            this.chkPrintPatientCard.StyleController = this.layoutControl4;
            this.chkPrintPatientCard.TabIndex = 13;
            this.chkPrintPatientCard.ToolTip = "Tự động in thẻ bệnh nhân";
            this.chkPrintPatientCard.CheckedChanged += new System.EventHandler(this.chkPrintPatientCard_CheckedChanged);
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem22,
            this.layoutControlItem27,
            this.lciAutoDeposit,
            this.layoutControlItem24,
            this.layoutControlItem26,
            this.layoutControlItem17,
            this.layoutControlItem31});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(455, 90);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem22
            // 
            this.layoutControlItem22.Control = this.chkAutoCreateBill;
            this.layoutControlItem22.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem22.Name = "layoutControlItem22";
            this.layoutControlItem22.Size = new System.Drawing.Size(151, 23);
            this.layoutControlItem22.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem22.TextVisible = false;
            // 
            // layoutControlItem27
            // 
            this.layoutControlItem27.Control = this.chkAssignDoctor;
            this.layoutControlItem27.Location = new System.Drawing.Point(0, 23);
            this.layoutControlItem27.Name = "layoutControlItem27";
            this.layoutControlItem27.Size = new System.Drawing.Size(151, 67);
            this.layoutControlItem27.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem27.TextVisible = false;
            // 
            // lciAutoDeposit
            // 
            this.lciAutoDeposit.Control = this.chkAutoDeposit;
            this.lciAutoDeposit.Location = new System.Drawing.Point(151, 0);
            this.lciAutoDeposit.Name = "lciAutoDeposit";
            this.lciAutoDeposit.Size = new System.Drawing.Size(153, 23);
            this.lciAutoDeposit.TextSize = new System.Drawing.Size(0, 0);
            this.lciAutoDeposit.TextVisible = false;
            // 
            // layoutControlItem24
            // 
            this.layoutControlItem24.Control = this.chkPrintPatientCard;
            this.layoutControlItem24.Location = new System.Drawing.Point(304, 23);
            this.layoutControlItem24.Name = "layoutControlItem24";
            this.layoutControlItem24.Size = new System.Drawing.Size(151, 67);
            this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem24.TextVisible = false;
            // 
            // layoutControlItem26
            // 
            this.layoutControlItem26.Control = this.chkPrintExam;
            this.layoutControlItem26.Location = new System.Drawing.Point(151, 23);
            this.layoutControlItem26.Name = "layoutControlItem26";
            this.layoutControlItem26.Size = new System.Drawing.Size(153, 23);
            this.layoutControlItem26.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem26.TextVisible = false;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.Control = this.chkAutoPay;
            this.layoutControlItem17.Location = new System.Drawing.Point(304, 0);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Size = new System.Drawing.Size(151, 23);
            this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem17.TextVisible = false;
            // 
            // ucCheckTT1
            // 
            this.ucCheckTT1.Location = new System.Drawing.Point(910, 360);
            this.ucCheckTT1.Margin = new System.Windows.Forms.Padding(4);
            this.ucCheckTT1.Name = "ucCheckTT1";
            this.ucCheckTT1.Size = new System.Drawing.Size(451, 401);
            this.ucCheckTT1.TabIndex = 11;
            // 
            // ucPlusInfo1
            // 
            this.ucPlusInfo1.Location = new System.Drawing.Point(454, 493);
            this.ucPlusInfo1.Margin = new System.Windows.Forms.Padding(4);
            this.ucPlusInfo1.Name = "ucPlusInfo1";
            this.ucPlusInfo1.Size = new System.Drawing.Size(454, 360);
            this.ucPlusInfo1.TabIndex = 10;
            // 
            // ucServiceRoomInfo1
            // 
            this.ucServiceRoomInfo1.Location = new System.Drawing.Point(0, 718);
            this.ucServiceRoomInfo1.Margin = new System.Windows.Forms.Padding(4);
            this.ucServiceRoomInfo1.Name = "ucServiceRoomInfo1";
            this.ucServiceRoomInfo1.Size = new System.Drawing.Size(454, 135);
            this.ucServiceRoomInfo1.TabIndex = 3;
            // 
            // ucImageInfo1
            // 
            this.ucImageInfo1.Location = new System.Drawing.Point(910, 2);
            this.ucImageInfo1.Margin = new System.Windows.Forms.Padding(4);
            this.ucImageInfo1.Name = "ucImageInfo1";
            this.ucImageInfo1.Size = new System.Drawing.Size(451, 354);
            this.ucImageInfo1.TabIndex = 7;
            // 
            // ucRelativeInfo1
            // 
            this.ucRelativeInfo1.Location = new System.Drawing.Point(454, 0);
            this.ucRelativeInfo1.Margin = new System.Windows.Forms.Padding(4);
            this.ucRelativeInfo1.Name = "ucRelativeInfo1";
            this.ucRelativeInfo1.Size = new System.Drawing.Size(454, 178);
            this.ucRelativeInfo1.TabIndex = 5;
            // 
            // ucOtherServiceReqInfo1
            // 
            this.ucOtherServiceReqInfo1.Location = new System.Drawing.Point(454, 178);
            this.ucOtherServiceReqInfo1.Margin = new System.Windows.Forms.Padding(4);
            this.ucOtherServiceReqInfo1.Name = "ucOtherServiceReqInfo1";
            this.ucOtherServiceReqInfo1.Size = new System.Drawing.Size(454, 315);
            this.ucOtherServiceReqInfo1.TabIndex = 4;
            // 
            // ucHeinInfo1
            // 
            this.ucHeinInfo1.Location = new System.Drawing.Point(0, 403);
            this.ucHeinInfo1.Margin = new System.Windows.Forms.Padding(4);
            this.ucHeinInfo1.Name = "ucHeinInfo1";
            this.ucHeinInfo1.Size = new System.Drawing.Size(454, 315);
            this.ucHeinInfo1.TabIndex = 2;
            // 
            // ucAddressCombo1
            // 
            this.ucAddressCombo1.Location = new System.Drawing.Point(0, 178);
            this.ucAddressCombo1.Margin = new System.Windows.Forms.Padding(4);
            this.ucAddressCombo1.Name = "ucAddressCombo1";
            this.ucAddressCombo1.Size = new System.Drawing.Size(454, 225);
            this.ucAddressCombo1.TabIndex = 1;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.pnlServiceRoomInfomation,
            this.layoutControlItem18,
            this.lciUCHeinInfo,
            this.layoutControlItem20,
            this.layoutControlItem21,
            this.layoutControlItem23,
            this.lciUCServiceRoomInfo,
            this.layoutControlItem10,
            this.layoutControlItem19,
            this.layoutControlItem25});
            this.layoutControlGroup1.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            columnDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition1.Width = 16.629942706865783D;
            columnDefinition2.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition2.Width = 16.629942706865783D;
            columnDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition3.Width = 16.629942706865783D;
            columnDefinition4.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition4.Width = 16.629942706865783D;
            columnDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition5.Width = 16.629942706865783D;
            columnDefinition6.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition6.Width = 16.629942706865783D;
            this.layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
            columnDefinition1,
            columnDefinition2,
            columnDefinition3,
            columnDefinition4,
            columnDefinition5,
            columnDefinition6});
            rowDefinition1.Height = 5.2631578947368416D;
            rowDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition2.Height = 5.2631578947368416D;
            rowDefinition2.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition3.Height = 5.2631578947368416D;
            rowDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition4.Height = 5.2631578947368416D;
            rowDefinition4.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition5.Height = 5.2631578947368416D;
            rowDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition6.Height = 5.2631578947368416D;
            rowDefinition6.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition7.Height = 5.2631578947368416D;
            rowDefinition7.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition8.Height = 5.2631578947368416D;
            rowDefinition8.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition9.Height = 5.2631578947368416D;
            rowDefinition9.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition10.Height = 5.2631578947368416D;
            rowDefinition10.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition11.Height = 5.2631578947368416D;
            rowDefinition11.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition12.Height = 5.2631578947368416D;
            rowDefinition12.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition13.Height = 5.2631578947368416D;
            rowDefinition13.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition14.Height = 5.2631578947368416D;
            rowDefinition14.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition15.Height = 5.2631578947368416D;
            rowDefinition15.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition16.Height = 5.2631578947368416D;
            rowDefinition16.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition17.Height = 5.2631578947368416D;
            rowDefinition17.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition18.Height = 5.2631578947368416D;
            rowDefinition18.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition19.Height = 5.2631578947368416D;
            rowDefinition19.SizeType = System.Windows.Forms.SizeType.Percent;
            this.layoutControlGroup1.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
            rowDefinition1,
            rowDefinition2,
            rowDefinition3,
            rowDefinition4,
            rowDefinition5,
            rowDefinition6,
            rowDefinition7,
            rowDefinition8,
            rowDefinition9,
            rowDefinition10,
            rowDefinition11,
            rowDefinition12,
            rowDefinition13,
            rowDefinition14,
            rowDefinition15,
            rowDefinition16,
            rowDefinition17,
            rowDefinition18,
            rowDefinition19});
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1363, 853);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // ucPatientRaw1
            // 
            this.ucPatientRaw1.isAlertTreatmentEndInDay = false;
            this.ucPatientRaw1.Location = new System.Drawing.Point(0, 0);
            this.ucPatientRaw1.Margin = new System.Windows.Forms.Padding(4);
            this.ucPatientRaw1.Name = "ucPatientRaw1";
            this.ucPatientRaw1.ResultDataADO = null;
            this.ucPatientRaw1.Size = new System.Drawing.Size(454, 178);
            this.ucPatientRaw1.TabIndex = 0;
            // 
            // pnlServiceRoomInfomation
            // 
            this.pnlServiceRoomInfomation.Control = this.ucPatientRaw1;
            this.pnlServiceRoomInfomation.Location = new System.Drawing.Point(0, 0);
            this.pnlServiceRoomInfomation.Name = "pnlServiceRoomInfomation";
            this.pnlServiceRoomInfomation.OptionsTableLayoutItem.ColumnSpan = 2;
            this.pnlServiceRoomInfomation.OptionsTableLayoutItem.RowSpan = 4;
            this.pnlServiceRoomInfomation.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.pnlServiceRoomInfomation.Size = new System.Drawing.Size(454, 178);
            this.pnlServiceRoomInfomation.TextSize = new System.Drawing.Size(0, 0);
            this.pnlServiceRoomInfomation.TextVisible = false;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.Control = this.ucAddressCombo1;
            this.layoutControlItem18.Location = new System.Drawing.Point(0, 178);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.OptionsTableLayoutItem.ColumnSpan = 2;
            this.layoutControlItem18.OptionsTableLayoutItem.RowIndex = 4;
            this.layoutControlItem18.OptionsTableLayoutItem.RowSpan = 5;
            this.layoutControlItem18.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem18.Size = new System.Drawing.Size(454, 225);
            this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem18.TextVisible = false;
            // 
            // lciUCHeinInfo
            // 
            this.lciUCHeinInfo.Control = this.ucHeinInfo1;
            this.lciUCHeinInfo.Location = new System.Drawing.Point(0, 403);
            this.lciUCHeinInfo.Name = "lciUCHeinInfo";
            this.lciUCHeinInfo.OptionsTableLayoutItem.ColumnSpan = 2;
            this.lciUCHeinInfo.OptionsTableLayoutItem.RowIndex = 9;
            this.lciUCHeinInfo.OptionsTableLayoutItem.RowSpan = 7;
            this.lciUCHeinInfo.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciUCHeinInfo.Size = new System.Drawing.Size(454, 315);
            this.lciUCHeinInfo.TextSize = new System.Drawing.Size(0, 0);
            this.lciUCHeinInfo.TextVisible = false;
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.Control = this.ucOtherServiceReqInfo1;
            this.layoutControlItem20.Location = new System.Drawing.Point(454, 178);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.OptionsTableLayoutItem.ColumnIndex = 2;
            this.layoutControlItem20.OptionsTableLayoutItem.ColumnSpan = 2;
            this.layoutControlItem20.OptionsTableLayoutItem.RowIndex = 4;
            this.layoutControlItem20.OptionsTableLayoutItem.RowSpan = 7;
            this.layoutControlItem20.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem20.Size = new System.Drawing.Size(454, 315);
            this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem20.TextVisible = false;
            // 
            // layoutControlItem21
            // 
            this.layoutControlItem21.Control = this.ucRelativeInfo1;
            this.layoutControlItem21.Location = new System.Drawing.Point(454, 0);
            this.layoutControlItem21.Name = "layoutControlItem21";
            this.layoutControlItem21.OptionsTableLayoutItem.ColumnIndex = 2;
            this.layoutControlItem21.OptionsTableLayoutItem.ColumnSpan = 2;
            this.layoutControlItem21.OptionsTableLayoutItem.RowSpan = 4;
            this.layoutControlItem21.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem21.Size = new System.Drawing.Size(454, 178);
            this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem21.TextVisible = false;
            // 
            // layoutControlItem23
            // 
            this.layoutControlItem23.Control = this.ucImageInfo1;
            this.layoutControlItem23.Location = new System.Drawing.Point(908, 0);
            this.layoutControlItem23.Name = "layoutControlItem23";
            this.layoutControlItem23.OptionsTableLayoutItem.ColumnIndex = 4;
            this.layoutControlItem23.OptionsTableLayoutItem.ColumnSpan = 2;
            this.layoutControlItem23.OptionsTableLayoutItem.RowSpan = 8;
            this.layoutControlItem23.Size = new System.Drawing.Size(455, 358);
            this.layoutControlItem23.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem23.TextVisible = false;
            // 
            // lciUCServiceRoomInfo
            // 
            this.lciUCServiceRoomInfo.Control = this.ucServiceRoomInfo1;
            this.lciUCServiceRoomInfo.Location = new System.Drawing.Point(0, 718);
            this.lciUCServiceRoomInfo.Name = "lciUCServiceRoomInfo";
            this.lciUCServiceRoomInfo.OptionsTableLayoutItem.ColumnSpan = 2;
            this.lciUCServiceRoomInfo.OptionsTableLayoutItem.RowIndex = 16;
            this.lciUCServiceRoomInfo.OptionsTableLayoutItem.RowSpan = 3;
            this.lciUCServiceRoomInfo.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciUCServiceRoomInfo.Size = new System.Drawing.Size(454, 135);
            this.lciUCServiceRoomInfo.TextSize = new System.Drawing.Size(0, 0);
            this.lciUCServiceRoomInfo.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.ucPlusInfo1;
            this.layoutControlItem10.Location = new System.Drawing.Point(454, 493);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.OptionsTableLayoutItem.ColumnIndex = 2;
            this.layoutControlItem10.OptionsTableLayoutItem.ColumnSpan = 2;
            this.layoutControlItem10.OptionsTableLayoutItem.RowIndex = 11;
            this.layoutControlItem10.OptionsTableLayoutItem.RowSpan = 8;
            this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem10.Size = new System.Drawing.Size(454, 360);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.Control = this.ucCheckTT1;
            this.layoutControlItem19.Location = new System.Drawing.Point(908, 358);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.OptionsTableLayoutItem.ColumnIndex = 4;
            this.layoutControlItem19.OptionsTableLayoutItem.ColumnSpan = 2;
            this.layoutControlItem19.OptionsTableLayoutItem.RowIndex = 8;
            this.layoutControlItem19.OptionsTableLayoutItem.RowSpan = 9;
            this.layoutControlItem19.Size = new System.Drawing.Size(455, 405);
            this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem19.TextVisible = false;
            // 
            // layoutControlItem25
            // 
            this.layoutControlItem25.Control = this.layoutControl4;
            this.layoutControlItem25.Location = new System.Drawing.Point(908, 763);
            this.layoutControlItem25.Name = "layoutControlItem25";
            this.layoutControlItem25.OptionsTableLayoutItem.ColumnIndex = 4;
            this.layoutControlItem25.OptionsTableLayoutItem.ColumnSpan = 2;
            this.layoutControlItem25.OptionsTableLayoutItem.RowIndex = 17;
            this.layoutControlItem25.OptionsTableLayoutItem.RowSpan = 2;
            this.layoutControlItem25.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem25.Size = new System.Drawing.Size(455, 90);
            this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem25.TextVisible = false;
            // 
            // btnTTChuyenTuyen
            // 
            this.btnTTChuyenTuyen.Location = new System.Drawing.Point(750, 2);
            this.btnTTChuyenTuyen.Name = "btnTTChuyenTuyen";
            this.btnTTChuyenTuyen.Size = new System.Drawing.Size(77, 31);
            this.btnTTChuyenTuyen.StyleController = this.layoutControl3;
            this.btnTTChuyenTuyen.TabIndex = 8;
            this.btnTTChuyenTuyen.Text = "Chuyển tuyến";
            this.btnTTChuyenTuyen.Click += new System.EventHandler(this.btnTTChuyenTuyen_Click);
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.txtTo);
            this.layoutControl3.Controls.Add(this.txtFrom);
            this.layoutControl3.Controls.Add(this.btnGiayTo);
            this.layoutControl3.Controls.Add(this.btnDepositRequest);
            this.layoutControl3.Controls.Add(this.lblRegisterNumOrder);
            this.layoutControl3.Controls.Add(this.dropDownButton__Other);
            this.layoutControl3.Controls.Add(this.btnRecallPatient);
            this.layoutControl3.Controls.Add(this.btnTTChuyenTuyen);
            this.layoutControl3.Controls.Add(this.btnCallPatient);
            this.layoutControl3.Controls.Add(this.txtGateNumber);
            this.layoutControl3.Controls.Add(this.txtStepNumber);
            this.layoutControl3.Controls.Add(this.cboCashierRoom);
            this.layoutControl3.Controls.Add(this.btnSaveAndPrint);
            this.layoutControl3.Controls.Add(this.btnPatientNew);
            this.layoutControl3.Controls.Add(this.btnSaveAndAssain);
            this.layoutControl3.Controls.Add(this.btnDepositDetail);
            this.layoutControl3.Controls.Add(this.btnTreatmentBedRoom);
            this.layoutControl3.Controls.Add(this.btnNewContinue);
            this.layoutControl3.Controls.Add(this.btnPrint);
            this.layoutControl3.Controls.Add(this.btnSave);
            this.layoutControl3.Location = new System.Drawing.Point(2, 859);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(44, 269, 250, 350);
            this.layoutControl3.Root = this.Root;
            this.layoutControl3.Size = new System.Drawing.Size(1363, 35);
            this.layoutControl3.TabIndex = 4;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // txtTo
            // 
            this.txtTo.EditValue = "0";
            this.txtTo.Location = new System.Drawing.Point(163, 7);
            this.txtTo.Name = "txtTo";
            this.txtTo.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtTo.Properties.NullText = "0";
            this.txtTo.Size = new System.Drawing.Size(50, 20);
            this.txtTo.StyleController = this.layoutControl3;
            this.txtTo.TabIndex = 82;
            this.txtTo.ToolTipTitle = "Đến";
            this.txtTo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTo_KeyPress);
            // 
            // txtFrom
            // 
            this.txtFrom.EditValue = "0";
            this.txtFrom.Location = new System.Drawing.Point(109, 7);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtFrom.Properties.NullText = "0";
            this.txtFrom.Size = new System.Drawing.Size(50, 20);
            this.txtFrom.StyleController = this.layoutControl3;
            this.txtFrom.TabIndex = 81;
            this.txtFrom.ToolTipTitle = "Từ";
            this.txtFrom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFrom_KeyPress);
            // 
            // btnGiayTo
            // 
            this.btnGiayTo.Enabled = false;
            this.btnGiayTo.Location = new System.Drawing.Point(831, 2);
            this.btnGiayTo.Margin = new System.Windows.Forms.Padding(2);
            this.btnGiayTo.Name = "btnGiayTo";
            this.btnGiayTo.Size = new System.Drawing.Size(56, 31);
            this.btnGiayTo.StyleController = this.layoutControl3;
            toolTipItem1.Text = "Hồ sơ giấy tờ đính kèm";
            superToolTip1.Items.Add(toolTipItem1);
            this.btnGiayTo.SuperTip = superToolTip1;
            this.btnGiayTo.TabIndex = 80;
            this.btnGiayTo.Text = "Giấy tờ";
            this.btnGiayTo.Click += new System.EventHandler(this.btnGiayTo_Click);
            // 
            // btnDepositRequest
            // 
            this.btnDepositRequest.Enabled = false;
            this.btnDepositRequest.Location = new System.Drawing.Point(598, 2);
            this.btnDepositRequest.Name = "btnDepositRequest";
            this.btnDepositRequest.Size = new System.Drawing.Size(58, 31);
            this.btnDepositRequest.StyleController = this.layoutControl3;
            this.btnDepositRequest.TabIndex = 79;
            this.btnDepositRequest.Text = "YC tạm ứng";
            this.btnDepositRequest.Click += new System.EventHandler(this.btnDepositRequest_Click);
            // 
            // lblRegisterNumOrder
            // 
            this.lblRegisterNumOrder.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblRegisterNumOrder.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblRegisterNumOrder.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblRegisterNumOrder.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRegisterNumOrder.Location = new System.Drawing.Point(314, 10);
            this.lblRegisterNumOrder.Name = "lblRegisterNumOrder";
            this.lblRegisterNumOrder.Size = new System.Drawing.Size(48, 16);
            this.lblRegisterNumOrder.StyleController = this.layoutControl3;
            this.lblRegisterNumOrder.TabIndex = 78;
            // 
            // dropDownButton__Other
            // 
            this.dropDownButton__Other.Enabled = false;
            this.dropDownButton__Other.Location = new System.Drawing.Point(526, 2);
            this.dropDownButton__Other.Name = "dropDownButton__Other";
            this.dropDownButton__Other.Size = new System.Drawing.Size(68, 31);
            this.dropDownButton__Other.StyleController = this.layoutControl3;
            this.dropDownButton__Other.TabIndex = 77;
            this.dropDownButton__Other.Text = "Khác";
            this.dropDownButton__Other.Click += new System.EventHandler(this.dropDownButton__Other_Click);
            // 
            // btnRecallPatient
            // 
            this.btnRecallPatient.Location = new System.Drawing.Point(261, 2);
            this.btnRecallPatient.Name = "btnRecallPatient";
            this.btnRecallPatient.Size = new System.Drawing.Size(49, 31);
            this.btnRecallPatient.StyleController = this.layoutControl3;
            this.btnRecallPatient.TabIndex = 76;
            this.btnRecallPatient.Text = "Gọi lại (F6)";
            this.btnRecallPatient.ToolTip = "Gọi lại (F6)";
            this.btnRecallPatient.Click += new System.EventHandler(this.btnRecallPatient_Click);
            // 
            // btnCallPatient
            // 
            this.btnCallPatient.Location = new System.Drawing.Point(217, 2);
            this.btnCallPatient.Name = "btnCallPatient";
            this.btnCallPatient.Size = new System.Drawing.Size(40, 31);
            this.btnCallPatient.StyleController = this.layoutControl3;
            this.btnCallPatient.TabIndex = 75;
            this.btnCallPatient.Text = "Gọi (F5)";
            this.btnCallPatient.Click += new System.EventHandler(this.btnCallPatient_Click);
            // 
            // txtGateNumber
            // 
            this.txtGateNumber.EditValue = "";
            this.txtGateNumber.Location = new System.Drawing.Point(2, 7);
            this.txtGateNumber.Name = "txtGateNumber";
            this.txtGateNumber.Properties.Appearance.Options.UseTextOptions = true;
            this.txtGateNumber.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtGateNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Thiết lập thông báo khi gọi/gọi lại", null, null, true)});
            this.txtGateNumber.Properties.NullValuePrompt = "Cổng";
            this.txtGateNumber.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtGateNumber.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtGateNumber.Size = new System.Drawing.Size(58, 20);
            this.txtGateNumber.StyleController = this.layoutControl3;
            this.txtGateNumber.TabIndex = 74;
            this.txtGateNumber.ToolTip = "Nhập \"Mã cổng\" trong trường hợp các cổng sử dụng riêng dãy số thứ tự hoặc nhập th" +
    "eo định dạng \"Mã cổng:Mã dãy\" trong trường hợp các cổng sử dụng chung dãy số thứ" +
    " tự.";
            this.txtGateNumber.ToolTipTitle = "Cổng";
            this.txtGateNumber.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtGateNumber_ButtonClick);
            this.txtGateNumber.Leave += new System.EventHandler(this.txtGateNumber_Leave);
            // 
            // txtStepNumber
            // 
            this.txtStepNumber.Location = new System.Drawing.Point(64, 7);
            this.txtStepNumber.Name = "txtStepNumber";
            this.txtStepNumber.Properties.Appearance.Options.UseTextOptions = true;
            this.txtStepNumber.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtStepNumber.Properties.MaxLength = 5;
            this.txtStepNumber.Properties.NullValuePrompt = "Bước nhảy";
            this.txtStepNumber.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtStepNumber.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtStepNumber.Size = new System.Drawing.Size(41, 20);
            this.txtStepNumber.StyleController = this.layoutControl3;
            this.txtStepNumber.TabIndex = 73;
            this.txtStepNumber.ToolTipTitle = "Bước nhảy";
            this.txtStepNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStepNumber_KeyPress);
            this.txtStepNumber.Leave += new System.EventHandler(this.txtStepNumber_Leave);
            // 
            // cboCashierRoom
            // 
            this.cboCashierRoom.Location = new System.Drawing.Point(474, 7);
            this.cboCashierRoom.Name = "cboCashierRoom";
            this.cboCashierRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboCashierRoom.Properties.NullText = "";
            this.cboCashierRoom.Size = new System.Drawing.Size(48, 20);
            this.cboCashierRoom.StyleController = this.layoutControl3;
            this.cboCashierRoom.TabIndex = 64;
            // 
            // btnSaveAndPrint
            // 
            this.btnSaveAndPrint.Location = new System.Drawing.Point(1133, 2);
            this.btnSaveAndPrint.Name = "btnSaveAndPrint";
            this.btnSaveAndPrint.Size = new System.Drawing.Size(77, 31);
            this.btnSaveAndPrint.StyleController = this.layoutControl3;
            this.btnSaveAndPrint.TabIndex = 70;
            this.btnSaveAndPrint.Text = "Lưu in (Ctrl I/F8)";
            this.btnSaveAndPrint.Click += new System.EventHandler(this.btnSaveAndPrint_Click);
            // 
            // btnPatientNew
            // 
            this.btnPatientNew.Appearance.ForeColor = System.Drawing.Color.Green;
            this.btnPatientNew.Appearance.Options.UseForeColor = true;
            this.btnPatientNew.Appearance.Options.UseTextOptions = true;
            this.btnPatientNew.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.btnPatientNew.Location = new System.Drawing.Point(366, 2);
            this.btnPatientNew.Name = "btnPatientNew";
            this.btnPatientNew.Size = new System.Drawing.Size(64, 31);
            this.btnPatientNew.StyleController = this.layoutControl3;
            this.btnPatientNew.TabIndex = 63;
            this.btnPatientNew.Text = "BN mới (Ctrl R)";
            this.btnPatientNew.ToolTip = "Bệnh nhân mới (Ctrl R)";
            this.btnPatientNew.Visible = false;
            this.btnPatientNew.Click += new System.EventHandler(this.btnPatientNew_Click);
            // 
            // btnSaveAndAssain
            // 
            this.btnSaveAndAssain.Appearance.Options.UseTextOptions = true;
            this.btnSaveAndAssain.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnSaveAndAssain.Enabled = false;
            this.btnSaveAndAssain.Location = new System.Drawing.Point(966, 2);
            this.btnSaveAndAssain.Name = "btnSaveAndAssain";
            this.btnSaveAndAssain.Size = new System.Drawing.Size(90, 31);
            this.btnSaveAndAssain.StyleController = this.layoutControl3;
            this.btnSaveAndAssain.TabIndex = 68;
            this.btnSaveAndAssain.Text = "Chỉ định (Ctrl D)";
            this.btnSaveAndAssain.Click += new System.EventHandler(this.btnSaveAndAssain_Click);
            // 
            // btnDepositDetail
            // 
            this.btnDepositDetail.Enabled = false;
            this.btnDepositDetail.Location = new System.Drawing.Point(660, 2);
            this.btnDepositDetail.Name = "btnDepositDetail";
            this.btnDepositDetail.Size = new System.Drawing.Size(86, 31);
            this.btnDepositDetail.StyleController = this.layoutControl3;
            this.btnDepositDetail.TabIndex = 65;
            this.btnDepositDetail.Text = "Tạm ứng (Ctrl T)";
            this.btnDepositDetail.Click += new System.EventHandler(this.btnDepositDetail_Click);
            // 
            // btnTreatmentBedRoom
            // 
            this.btnTreatmentBedRoom.Appearance.Options.UseTextOptions = true;
            this.btnTreatmentBedRoom.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.btnTreatmentBedRoom.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Character;
            this.btnTreatmentBedRoom.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.btnTreatmentBedRoom.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnTreatmentBedRoom.Enabled = false;
            this.btnTreatmentBedRoom.Location = new System.Drawing.Point(891, 2);
            this.btnTreatmentBedRoom.Name = "btnTreatmentBedRoom";
            this.btnTreatmentBedRoom.Size = new System.Drawing.Size(71, 31);
            this.btnTreatmentBedRoom.StyleController = this.layoutControl3;
            this.btnTreatmentBedRoom.TabIndex = 67;
            this.btnTreatmentBedRoom.Text = "Vào buồng (Ctrl G)";
            this.btnTreatmentBedRoom.Click += new System.EventHandler(this.btnTreatmentBedRoom_Click);
            // 
            // btnNewContinue
            // 
            this.btnNewContinue.Location = new System.Drawing.Point(1280, 2);
            this.btnNewContinue.Name = "btnNewContinue";
            this.btnNewContinue.Size = new System.Drawing.Size(81, 31);
            this.btnNewContinue.StyleController = this.layoutControl3;
            this.btnNewContinue.TabIndex = 72;
            this.btnNewContinue.Text = "Mới (Ctrl N/F1)";
            this.btnNewContinue.Click += new System.EventHandler(this.btnNewContinue_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Enabled = false;
            this.btnPrint.Location = new System.Drawing.Point(1214, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(62, 31);
            this.btnPrint.StyleController = this.layoutControl3;
            this.btnPrint.TabIndex = 71;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(1060, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(69, 31);
            this.btnSave.StyleController = this.layoutControl3;
            this.btnSave.TabIndex = 69;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem8,
            this.lcibtnPatientNewInfo,
            this.layoutControlItem11,
            this.lcibtnDepositDetail,
            this.layoutControlItem13,
            this.layoutControlItem14,
            this.layoutControlItem15,
            this.layoutControlItem9,
            this.layoutControlItem16,
            this.layoutControlItem7,
            this.emptySpaceItem1,
            this.layoutControlItem12,
            this.lciRegisterNumOrder,
            this.lcibtnDepositRequest,
            this.layoutControlItem28,
            this.layoutControlItem29,
            this.layoutControlItem30});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(1363, 35);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnRecallPatient;
            this.layoutControlItem3.Location = new System.Drawing.Point(259, 0);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(71, 35);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(1, 35);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(53, 35);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnCallPatient;
            this.layoutControlItem4.Location = new System.Drawing.Point(215, 0);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(1, 35);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(44, 35);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.txtGateNumber;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(1, 35);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.OptionsToolTip.ToolTip = "Nhập \"Mã cổng\" trong trường hợp các cổng sử dụng riêng dãy số thứ tự hoặc nhập th" +
    "eo định dạng \"Mã cổng:Mã dãy\" trong trường hợp các cổng sử dụng chung dãy số thứ" +
    " tự.";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem5.Size = new System.Drawing.Size(62, 35);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.txtStepNumber;
            this.layoutControlItem6.Location = new System.Drawing.Point(62, 0);
            this.layoutControlItem6.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem6.MinSize = new System.Drawing.Size(1, 35);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem6.Size = new System.Drawing.Size(45, 35);
            this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.cboCashierRoom;
            this.layoutControlItem8.Location = new System.Drawing.Point(446, 0);
            this.layoutControlItem8.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem8.MinSize = new System.Drawing.Size(1, 35);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.OptionsToolTip.ToolTip = "Phòng thu ngân";
            this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem8.Size = new System.Drawing.Size(78, 35);
            this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem8.Text = "PTN:";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(23, 13);
            // 
            // lcibtnPatientNewInfo
            // 
            this.lcibtnPatientNewInfo.Control = this.btnPatientNew;
            this.lcibtnPatientNewInfo.Location = new System.Drawing.Point(364, 0);
            this.lcibtnPatientNewInfo.MaxSize = new System.Drawing.Size(0, 35);
            this.lcibtnPatientNewInfo.MinSize = new System.Drawing.Size(1, 35);
            this.lcibtnPatientNewInfo.Name = "lcibtnPatientNewInfo";
            this.lcibtnPatientNewInfo.Size = new System.Drawing.Size(68, 35);
            this.lcibtnPatientNewInfo.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lcibtnPatientNewInfo.TextSize = new System.Drawing.Size(0, 0);
            this.lcibtnPatientNewInfo.TextVisible = false;
            this.lcibtnPatientNewInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.btnSaveAndAssain;
            this.layoutControlItem11.Location = new System.Drawing.Point(964, 0);
            this.layoutControlItem11.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem11.MinSize = new System.Drawing.Size(91, 35);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(94, 35);
            this.layoutControlItem11.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // lcibtnDepositDetail
            // 
            this.lcibtnDepositDetail.Control = this.btnDepositDetail;
            this.lcibtnDepositDetail.Location = new System.Drawing.Point(658, 0);
            this.lcibtnDepositDetail.MaxSize = new System.Drawing.Size(0, 35);
            this.lcibtnDepositDetail.MinSize = new System.Drawing.Size(90, 35);
            this.lcibtnDepositDetail.Name = "lcibtnDepositDetail";
            this.lcibtnDepositDetail.Size = new System.Drawing.Size(90, 35);
            this.lcibtnDepositDetail.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lcibtnDepositDetail.TextSize = new System.Drawing.Size(0, 0);
            this.lcibtnDepositDetail.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.btnTreatmentBedRoom;
            this.layoutControlItem13.Location = new System.Drawing.Point(889, 0);
            this.layoutControlItem13.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem13.MinSize = new System.Drawing.Size(1, 35);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(75, 35);
            this.layoutControlItem13.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextVisible = false;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.btnNewContinue;
            this.layoutControlItem14.Location = new System.Drawing.Point(1278, 0);
            this.layoutControlItem14.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem14.MinSize = new System.Drawing.Size(82, 35);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(85, 35);
            this.layoutControlItem14.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.btnPrint;
            this.layoutControlItem15.Location = new System.Drawing.Point(1212, 0);
            this.layoutControlItem15.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem15.MinSize = new System.Drawing.Size(64, 35);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(66, 35);
            this.layoutControlItem15.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnSaveAndPrint;
            this.layoutControlItem9.Location = new System.Drawing.Point(1131, 0);
            this.layoutControlItem9.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem9.MinSize = new System.Drawing.Size(79, 35);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(81, 35);
            this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.Control = this.btnSave;
            this.layoutControlItem16.Location = new System.Drawing.Point(1058, 0);
            this.layoutControlItem16.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem16.MinSize = new System.Drawing.Size(71, 35);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(73, 35);
            this.layoutControlItem16.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem16.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnTTChuyenTuyen;
            this.layoutControlItem7.Location = new System.Drawing.Point(748, 0);
            this.layoutControlItem7.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem7.MinSize = new System.Drawing.Size(81, 35);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(81, 35);
            this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(432, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(14, 35);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.dropDownButton__Other;
            this.layoutControlItem12.Location = new System.Drawing.Point(524, 0);
            this.layoutControlItem12.MaxSize = new System.Drawing.Size(0, 36);
            this.layoutControlItem12.MinSize = new System.Drawing.Size(70, 35);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(72, 35);
            this.layoutControlItem12.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // lciRegisterNumOrder
            // 
            this.lciRegisterNumOrder.Control = this.lblRegisterNumOrder;
            this.lciRegisterNumOrder.Location = new System.Drawing.Point(312, 0);
            this.lciRegisterNumOrder.Name = "lciRegisterNumOrder";
            this.lciRegisterNumOrder.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 10, 2);
            this.lciRegisterNumOrder.Size = new System.Drawing.Size(52, 35);
            this.lciRegisterNumOrder.TextSize = new System.Drawing.Size(0, 0);
            this.lciRegisterNumOrder.TextVisible = false;
            this.lciRegisterNumOrder.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lcibtnDepositRequest
            // 
            this.lcibtnDepositRequest.Control = this.btnDepositRequest;
            this.lcibtnDepositRequest.Location = new System.Drawing.Point(596, 0);
            this.lcibtnDepositRequest.MaxSize = new System.Drawing.Size(0, 35);
            this.lcibtnDepositRequest.MinSize = new System.Drawing.Size(60, 35);
            this.lcibtnDepositRequest.Name = "lcibtnDepositRequest";
            this.lcibtnDepositRequest.Size = new System.Drawing.Size(62, 35);
            this.lcibtnDepositRequest.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lcibtnDepositRequest.TextSize = new System.Drawing.Size(0, 0);
            this.lcibtnDepositRequest.TextVisible = false;
            // 
            // layoutControlItem28
            // 
            this.layoutControlItem28.Control = this.btnGiayTo;
            this.layoutControlItem28.Location = new System.Drawing.Point(829, 0);
            this.layoutControlItem28.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem28.MinSize = new System.Drawing.Size(60, 35);
            this.layoutControlItem28.Name = "layoutControlItem28";
            this.layoutControlItem28.Size = new System.Drawing.Size(60, 35);
            this.layoutControlItem28.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem28.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem28.TextVisible = false;
            // 
            // layoutControlItem29
            // 
            this.layoutControlItem29.Control = this.txtFrom;
            this.layoutControlItem29.Location = new System.Drawing.Point(107, 0);
            this.layoutControlItem29.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem29.MinSize = new System.Drawing.Size(54, 24);
            this.layoutControlItem29.Name = "layoutControlItem29";
            this.layoutControlItem29.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem29.Size = new System.Drawing.Size(54, 35);
            this.layoutControlItem29.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem29.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem29.TextVisible = false;
            // 
            // layoutControlItem30
            // 
            this.layoutControlItem30.Control = this.txtTo;
            this.layoutControlItem30.Location = new System.Drawing.Point(161, 0);
            this.layoutControlItem30.MaxSize = new System.Drawing.Size(0, 29);
            this.layoutControlItem30.MinSize = new System.Drawing.Size(54, 29);
            this.layoutControlItem30.Name = "layoutControlItem30";
            this.layoutControlItem30.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem30.Size = new System.Drawing.Size(54, 35);
            this.layoutControlItem30.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem30.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem30.TextVisible = false;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.layoutControl3);
            this.layoutControl2.Controls.Add(this.layoutControl1);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(1367, 896);
            this.layoutControl2.TabIndex = 1;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(1367, 896);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.layoutControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1367, 857);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.layoutControl3;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 857);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1367, 39);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // timerInitForm
            // 
            this.timerInitForm.Interval = 500;
            // 
            // timerRefeshAutoCreateBill
            // 
            this.timerRefeshAutoCreateBill.Interval = 2000;
            // 
            // chkSignExam
            // 
            this.chkSignExam.Location = new System.Drawing.Point(153, 48);
            this.chkSignExam.Name = "chkSignExam";
            this.chkSignExam.Properties.Caption = "Ký phiếu khám";
            this.chkSignExam.Size = new System.Drawing.Size(149, 19);
            this.chkSignExam.StyleController = this.layoutControl4;
            this.chkSignExam.TabIndex = 18;
            this.chkSignExam.CheckedChanged += new System.EventHandler(this.chkSignExam_CheckedChanged);
            // 
            // layoutControlItem31
            // 
            this.layoutControlItem31.Control = this.chkSignExam;
            this.layoutControlItem31.Location = new System.Drawing.Point(151, 46);
            this.layoutControlItem31.Name = "layoutControlItem31";
            this.layoutControlItem31.Size = new System.Drawing.Size(153, 44);
            this.layoutControlItem31.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem31.TextVisible = false;
            // 
            // UCRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl2);
            this.Name = "UCRegister";
            this.Size = new System.Drawing.Size(1367, 896);
            this.Load += new System.EventHandler(this.UCRegister_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoPay.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoDeposit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAssignDoctor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintExam.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoCreateBill.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintPatientCard.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAutoDeposit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlServiceRoomInfomation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUCHeinInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUCServiceRoomInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGateNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStepNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCashierRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnPatientNewInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnDepositDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRegisterNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnDepositRequest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignExam.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.SimpleButton btnRecallPatient;
        private DevExpress.XtraEditors.SimpleButton btnCallPatient;
        private DevExpress.XtraEditors.ButtonEdit txtGateNumber;
        private DevExpress.XtraEditors.TextEdit txtStepNumber;
        internal DevExpress.XtraEditors.LookUpEdit cboCashierRoom;
        internal DevExpress.XtraEditors.SimpleButton btnSaveAndPrint;
        internal DevExpress.XtraEditors.SimpleButton btnPatientNew;
        internal DevExpress.XtraEditors.SimpleButton btnSaveAndAssain;
        internal DevExpress.XtraEditors.SimpleButton btnDepositDetail;
        internal DevExpress.XtraEditors.SimpleButton btnTreatmentBedRoom;
        internal DevExpress.XtraEditors.SimpleButton btnNewContinue;
        internal DevExpress.XtraEditors.SimpleButton btnPrint;
        internal DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem lcibtnPatientNewInfo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem lcibtnDepositDetail;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraLayout.LayoutControlItem pnlServiceRoomInfomation;
        internal UC.UCHeniInfo.UCHeinInfo ucHeinInfo1;
        internal UC.AddressCombo.UCAddressCombo ucAddressCombo1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
        private DevExpress.XtraLayout.LayoutControlItem lciUCHeinInfo;
        internal UC.UCOtherServiceReqInfo.UCOtherServiceReqInfo ucOtherServiceReqInfo1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
        internal UC.UCRelativeInfo.UCRelativeInfo ucRelativeInfo1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem21;
        internal UC.UCImageInfo.UCImageInfo ucImageInfo1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem23;
        internal UC.UCServiceRoomInfo.UCServiceRoomInfo ucServiceRoomInfo1;
        private DevExpress.XtraLayout.LayoutControlItem lciUCServiceRoomInfo;
        internal DevExpress.XtraEditors.SimpleButton btnTTChuyenTuyen;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        internal UC.PlusInfo.UCPlusInfo ucPlusInfo1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.DropDownButton dropDownButton__Other;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraEditors.LabelControl lblRegisterNumOrder;
        private DevExpress.XtraLayout.LayoutControlItem lciRegisterNumOrder;
        private System.Windows.Forms.Timer timerInitForm;
        internal DevExpress.XtraEditors.SimpleButton btnDepositRequest;
        private DevExpress.XtraLayout.LayoutControlItem lcibtnDepositRequest;
        private UC.UCCheckTT.UCCheckTT ucCheckTT1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraEditors.CheckEdit chkPrintPatientCard;
        internal DevExpress.XtraEditors.CheckEdit chkAutoCreateBill;
        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraEditors.CheckEdit chkPrintExam;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem22;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem24;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem26;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem25;
        private DevExpress.XtraEditors.CheckEdit chkAssignDoctor;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem27;
        private System.Windows.Forms.Timer timerRefeshAutoCreateBill;
        private DevExpress.XtraLayout.LayoutControlItem lciAutoDeposit;
        internal DevExpress.XtraEditors.CheckEdit chkAutoDeposit;
        internal DevExpress.XtraEditors.CheckEdit chkAutoPay;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraEditors.SimpleButton btnGiayTo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem28;
        private DevExpress.XtraEditors.TextEdit txtTo;
        private DevExpress.XtraEditors.TextEdit txtFrom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem29;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem30;
        private DevExpress.XtraEditors.CheckEdit chkSignExam;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem31;
        internal HIS.UC.UCPatientRaw.UCPatientRaw ucPatientRaw1;
    }
}

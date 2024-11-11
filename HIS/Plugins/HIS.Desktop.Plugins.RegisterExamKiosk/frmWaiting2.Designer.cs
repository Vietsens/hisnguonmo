
namespace HIS.Desktop.Plugins.RegisterExamKiosk
{
    partial class frmWaiting2
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
            this.timerLabel = new System.Windows.Forms.Timer(this.components);
            this.timerWallPaper = new System.Windows.Forms.Timer(this.components);
            this.timerCheckFocus = new System.Windows.Forms.Timer(this.components);
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelControlInput = new DevExpress.XtraEditors.PanelControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pbCamera = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pnSimpleButton1 = new Inventec.CustomControls.PNSimpleButton();
            this.lblPer = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.pbCccdImage = new System.Windows.Forms.PictureBox();
            this.lblExpried = new System.Windows.Forms.Label();
            this.lblEthe = new System.Windows.Forms.Label();
            this.lblGenderName = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblNational = new System.Windows.Forms.Label();
            this.lblDob = new System.Windows.Forms.Label();
            this.lblCccdCode = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblCapPer = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConfirm = new DevExpress.XtraEditors.SimpleButton();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.label2 = new DevExpress.XtraEditors.LabelControl();
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.txtNumberInput = new Inventec.CustomControls.PNTextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlInput)).BeginInit();
            this.panelControlInput.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCamera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCccdImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // timerCheckFocus
            // 
            this.timerCheckFocus.Interval = 2000;
            // 
            // layoutControl1
            // 
            this.layoutControl1.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.layoutControl1.Controls.Add(this.panel2);
            this.layoutControl1.Controls.Add(this.panel1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1370, 701);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panelControlInput);
            this.panel2.Location = new System.Drawing.Point(2, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(865, 697);
            this.panel2.TabIndex = 5;
            this.panel2.Resize += new System.EventHandler(this.panelControl_Resize);
            // 
            // panelControlInput
            // 
            this.panelControlInput.Appearance.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.panelControlInput.Appearance.Options.UseBackColor = true;
            this.panelControlInput.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlInput.Controls.Add(this.txtNumberInput);
            this.panelControlInput.Controls.Add(this.groupBox1);
            this.panelControlInput.Controls.Add(this.btnConfirm);
            this.panelControlInput.Controls.Add(this.pbImage);
            this.panelControlInput.Controls.Add(this.label2);
            this.panelControlInput.Controls.Add(this.lblMessage);
            this.panelControlInput.Location = new System.Drawing.Point(0, 2);
            this.panelControlInput.Name = "panelControlInput";
            this.panelControlInput.Size = new System.Drawing.Size(946, 697);
            this.panelControlInput.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.groupBox1.Controls.Add(this.pbCamera);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.pnSimpleButton1);
            this.groupBox1.Controls.Add(this.lblPer);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.pbCccdImage);
            this.groupBox1.Controls.Add(this.lblExpried);
            this.groupBox1.Controls.Add(this.lblEthe);
            this.groupBox1.Controls.Add(this.lblGenderName);
            this.groupBox1.Controls.Add(this.lblName);
            this.groupBox1.Controls.Add(this.lblDate);
            this.groupBox1.Controls.Add(this.lblNational);
            this.groupBox1.Controls.Add(this.lblDob);
            this.groupBox1.Controls.Add(this.lblCccdCode);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lblCapPer);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(27, 544);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(820, 495);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Visible = false;
            // 
            // pbCamera
            // 
            this.pbCamera.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbCamera.Location = new System.Drawing.Point(414, 20);
            this.pbCamera.Name = "pbCamera";
            this.pbCamera.Size = new System.Drawing.Size(131, 173);
            this.pbCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCamera.TabIndex = 25;
            this.pbCamera.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.Maroon;
            this.label7.Location = new System.Drawing.Point(446, 196);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 17);
            this.label7.TabIndex = 24;
            this.label7.Text = "Camera";
            // 
            // pnSimpleButton1
            // 
            this.pnSimpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pnSimpleButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.pnSimpleButton1.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.pnSimpleButton1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.pnSimpleButton1.BorderRadius = 20;
            this.pnSimpleButton1.BorderSize = 2;
            this.pnSimpleButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pnSimpleButton1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.pnSimpleButton1.ForeColor = System.Drawing.Color.White;
            this.pnSimpleButton1.Location = new System.Drawing.Point(672, 439);
            this.pnSimpleButton1.Margin = new System.Windows.Forms.Padding(0);
            this.pnSimpleButton1.Name = "pnSimpleButton1";
            this.pnSimpleButton1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.pnSimpleButton1.Size = new System.Drawing.Size(129, 47);
            this.pnSimpleButton1.TabIndex = 22;
            this.pnSimpleButton1.Text = "TIẾP TỤC";
            this.pnSimpleButton1.TextColor = System.Drawing.Color.White;
            this.pnSimpleButton1.UseVisualStyleBackColor = false;
            this.pnSimpleButton1.Visible = false;
            this.pnSimpleButton1.Click += new System.EventHandler(this.pnSimpleButton1_Click);
            // 
            // lblPer
            // 
            this.lblPer.AutoSize = true;
            this.lblPer.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.lblPer.ForeColor = System.Drawing.Color.Black;
            this.lblPer.Location = new System.Drawing.Point(686, 173);
            this.lblPer.Name = "lblPer";
            this.lblPer.Size = new System.Drawing.Size(54, 22);
            this.lblPer.TabIndex = 21;
            this.lblPer.Text = "70%";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label21.ForeColor = System.Drawing.Color.Maroon;
            this.label21.Location = new System.Drawing.Point(292, 196);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(101, 17);
            this.label21.TabIndex = 20;
            this.label21.Text = "Ảnh căn cước";
            // 
            // pbCccdImage
            // 
            this.pbCccdImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbCccdImage.Location = new System.Drawing.Point(275, 20);
            this.pbCccdImage.Name = "pbCccdImage";
            this.pbCccdImage.Size = new System.Drawing.Size(131, 173);
            this.pbCccdImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCccdImage.TabIndex = 19;
            this.pbCccdImage.TabStop = false;
            // 
            // lblExpried
            // 
            this.lblExpried.AutoSize = true;
            this.lblExpried.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.lblExpried.ForeColor = System.Drawing.Color.Black;
            this.lblExpried.Location = new System.Drawing.Point(531, 393);
            this.lblExpried.Name = "lblExpried";
            this.lblExpried.Size = new System.Drawing.Size(118, 22);
            this.lblExpried.TabIndex = 18;
            this.lblExpried.Text = "31/05/2050";
            // 
            // lblEthe
            // 
            this.lblEthe.AutoSize = true;
            this.lblEthe.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.lblEthe.ForeColor = System.Drawing.Color.Black;
            this.lblEthe.Location = new System.Drawing.Point(531, 358);
            this.lblEthe.Name = "lblEthe";
            this.lblEthe.Size = new System.Drawing.Size(52, 22);
            this.lblEthe.TabIndex = 17;
            this.lblEthe.Text = "Kinh";
            // 
            // lblGenderName
            // 
            this.lblGenderName.AutoSize = true;
            this.lblGenderName.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.lblGenderName.ForeColor = System.Drawing.Color.Black;
            this.lblGenderName.Location = new System.Drawing.Point(533, 322);
            this.lblGenderName.Name = "lblGenderName";
            this.lblGenderName.Size = new System.Drawing.Size(52, 22);
            this.lblGenderName.TabIndex = 16;
            this.lblGenderName.Text = "Nam";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.Black;
            this.lblName.Location = new System.Drawing.Point(531, 288);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(149, 22);
            this.lblName.TabIndex = 15;
            this.lblName.Text = "Nguyễn Văn An";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.lblDate.ForeColor = System.Drawing.Color.Black;
            this.lblDate.Location = new System.Drawing.Point(169, 393);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(118, 22);
            this.lblDate.TabIndex = 14;
            this.lblDate.Text = "31/05/2000";
            // 
            // lblNational
            // 
            this.lblNational.AutoSize = true;
            this.lblNational.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.lblNational.ForeColor = System.Drawing.Color.Black;
            this.lblNational.Location = new System.Drawing.Point(169, 358);
            this.lblNational.Name = "lblNational";
            this.lblNational.Size = new System.Drawing.Size(92, 22);
            this.lblNational.TabIndex = 13;
            this.lblNational.Text = "Việt Nam";
            // 
            // lblDob
            // 
            this.lblDob.AutoSize = true;
            this.lblDob.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.lblDob.ForeColor = System.Drawing.Color.Black;
            this.lblDob.Location = new System.Drawing.Point(171, 322);
            this.lblDob.Name = "lblDob";
            this.lblDob.Size = new System.Drawing.Size(118, 22);
            this.lblDob.TabIndex = 12;
            this.lblDob.Text = "10/10/1986";
            // 
            // lblCccdCode
            // 
            this.lblCccdCode.AutoSize = true;
            this.lblCccdCode.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.lblCccdCode.ForeColor = System.Drawing.Color.Black;
            this.lblCccdCode.Location = new System.Drawing.Point(169, 288);
            this.lblCccdCode.Name = "lblCccdCode";
            this.lblCccdCode.Size = new System.Drawing.Size(142, 22);
            this.lblCccdCode.TabIndex = 11;
            this.lblCccdCode.Text = "186615770125";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label12.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label12.Location = new System.Drawing.Point(411, 395);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(111, 18);
            this.label12.TabIndex = 10;
            this.label12.Text = "Ngày hết hạn:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label11.Location = new System.Drawing.Point(453, 360);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 18);
            this.label11.TabIndex = 9;
            this.label11.Text = "Dân tộc:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label10.Location = new System.Drawing.Point(450, 324);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 18);
            this.label10.TabIndex = 8;
            this.label10.Text = "Giới tính:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label9.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label9.Location = new System.Drawing.Point(463, 290);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 18);
            this.label9.TabIndex = 7;
            this.label9.Text = "Họ tên:";
            // 
            // lblCapPer
            // 
            this.lblCapPer.AutoSize = true;
            this.lblCapPer.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblCapPer.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.lblCapPer.Location = new System.Drawing.Point(551, 175);
            this.lblCapPer.Name = "lblCapPer";
            this.lblCapPer.Size = new System.Drawing.Size(129, 18);
            this.lblCapPer.TabIndex = 6;
            this.lblCapPer.Text = "Tỉ lệ trùng khớp:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label6.Location = new System.Drawing.Point(78, 395);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 18);
            this.label6.TabIndex = 4;
            this.label6.Text = "Ngày cấp:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label5.Location = new System.Drawing.Point(74, 360);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 18);
            this.label5.TabIndex = 3;
            this.label5.Text = "Quốc tịch:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label4.Location = new System.Drawing.Point(72, 324);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 18);
            this.label4.TabIndex = 2;
            this.label4.Text = "Ngày sinh:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label3.Location = new System.Drawing.Point(83, 290);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 18);
            this.label3.TabIndex = 1;
            this.label3.Text = "Số CCCD:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label1.Location = new System.Drawing.Point(33, 243);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(246, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Thông tin thẻ CCCD";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Appearance.BackColor = System.Drawing.Color.White;
            this.btnConfirm.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnConfirm.Appearance.BorderColor = System.Drawing.Color.White;
            this.btnConfirm.Appearance.Font = new System.Drawing.Font("Arial", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.Appearance.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.btnConfirm.Appearance.Options.UseBackColor = true;
            this.btnConfirm.Appearance.Options.UseBorderColor = true;
            this.btnConfirm.Appearance.Options.UseFont = true;
            this.btnConfirm.Appearance.Options.UseForeColor = true;
            this.btnConfirm.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnConfirm.AppearanceHovered.Font = new System.Drawing.Font("Arial", 24.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.AppearanceHovered.ForeColor = System.Drawing.Color.DodgerBlue;
            this.btnConfirm.AppearanceHovered.Options.UseBackColor = true;
            this.btnConfirm.AppearanceHovered.Options.UseFont = true;
            this.btnConfirm.AppearanceHovered.Options.UseForeColor = true;
            this.btnConfirm.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.Location = new System.Drawing.Point(590, 332);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(240, 66);
            this.btnConfirm.TabIndex = 9;
            this.btnConfirm.Text = "Đăng ký khám";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // pbImage
            // 
            this.pbImage.Location = new System.Drawing.Point(530, 32);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(151, 62);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 8;
            this.pbImage.TabStop = false;
            this.pbImage.Visible = false;
            // 
            // label2
            // 
            this.label2.Appearance.Font = new System.Drawing.Font("Arial", 24.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Appearance.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(10, 278);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(837, 38);
            this.label2.TabIndex = 2;
            this.label2.Text = "Xin mời quẹt thẻ hoặc nhập CCCD, CMND, mã bệnh nhân";
            // 
            // lblMessage
            // 
            this.lblMessage.Appearance.Font = new System.Drawing.Font("Arial", 35.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Location = new System.Drawing.Point(4, 111);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(928, 55);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "Hệ thống đăng ký khám bệnh thông minh";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(869, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(501, 701);
            this.panel1.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1370, 701);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.panel1;
            this.layoutControlItem1.Location = new System.Drawing.Point(869, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Size = new System.Drawing.Size(501, 701);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.panel2;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(869, 701);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // txtNumberInput
            // 
            this.txtNumberInput.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtNumberInput.BackColor = System.Drawing.Color.White;
            this.txtNumberInput.BackgroundColor = System.Drawing.Color.White;
            this.txtNumberInput.BorderColor = System.Drawing.Color.White;
            this.txtNumberInput.BorderRadius = 20;
            this.txtNumberInput.BorderSize = 1;
            this.txtNumberInput.EditMaskPn = "";
            this.txtNumberInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.txtNumberInput.Location = new System.Drawing.Point(4, 332);
            this.txtNumberInput.MaskTypes = DevExpress.XtraEditors.Mask.MaskType.None;
            this.txtNumberInput.MaxLengthTexts = 200;
            this.txtNumberInput.Name = "txtNumberInput";
            this.txtNumberInput.Padding = new System.Windows.Forms.Padding(15);
            this.txtNumberInput.Size = new System.Drawing.Size(568, 66);
            this.txtNumberInput.TabIndex = 10;
            this.txtNumberInput.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.txtNumberInput.TextHintNull = "Nhập số";
            this.txtNumberInput.Texts = "";
            this.txtNumberInput.Visible = false;
            this.txtNumberInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumberInput_KeyPress);
            // 
            // frmWaiting2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 701);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmWaiting2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmWaiting2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmWaitingScreen_FormClosed);
            this.Load += new System.EventHandler(this.frmWaiting2_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmWaitingScreen_KeyUp);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlInput)).EndInit();
            this.panelControlInput.ResumeLayout(false);
            this.panelControlInput.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCamera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCccdImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timerLabel;
        private System.Windows.Forms.Timer timerWallPaper;
        private System.Windows.Forms.Timer timerCheckFocus;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraEditors.PanelControl panelControlInput;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pbCamera;
        private System.Windows.Forms.Label label7;
        private Inventec.CustomControls.PNSimpleButton pnSimpleButton1;
        private System.Windows.Forms.Label lblPer;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.PictureBox pbCccdImage;
        private System.Windows.Forms.Label lblExpried;
        private System.Windows.Forms.Label lblEthe;
        private System.Windows.Forms.Label lblGenderName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblNational;
        private System.Windows.Forms.Label lblDob;
        private System.Windows.Forms.Label lblCccdCode;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblCapPer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.SimpleButton btnConfirm;
        private System.Windows.Forms.PictureBox pbImage;
        private DevExpress.XtraEditors.LabelControl label2;
        private DevExpress.XtraEditors.LabelControl lblMessage;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private Inventec.CustomControls.PNTextEdit txtNumberInput;
    }
}
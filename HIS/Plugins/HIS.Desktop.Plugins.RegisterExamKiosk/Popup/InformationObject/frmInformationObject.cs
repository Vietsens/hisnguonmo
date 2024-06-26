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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.RegisterExamKiosk.ADO;
using HIS.Desktop.Plugins.RegisterExamKiosk.Config;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.InformationObject
{
    public partial class frmInformationObject : Form
    {
        HIS.Desktop.Common.DelegateSelectData selectDataPatientType;
        HIS.Desktop.Common.DelegateSelectData selectData;
        ResultHistoryLDO resultHistoryLDO = null;
        HisPatientForKioskSDO InformationObject_;
        bool IsOppositeLine;
        bool IsAppointment;
        bool IsOnline;
        bool IsEmergency;
        bool IsIntroduce;
        bool IsEdit;
        bool IsEnable;
        List<HIS_MEDI_ORG> dtMediOrg;
        List<HIS_ICD> dtIcd;
        List<CmktADO> Cmkts;
        List<HIS_TRAN_PATI_FORM> dtTranpatiForm;
        List<HIS_TRAN_PATI_REASON> dtTranpatiReason;
        public static HisExamRegisterKioskSDO sdoInformationObj;
        public frmInformationObject()
        {
            InitializeComponent();
        }

        public frmInformationObject(HisPatientForKioskSDO InformationObject, ResultHistoryLDO resultHistoryLDO, HIS.Desktop.Common.DelegateSelectData selectDataPatientType, HIS.Desktop.Common.DelegateSelectData selectData)
        {
            InitializeComponent();
            try
            {
                InformationObject_ = InformationObject;
                this.resultHistoryLDO = resultHistoryLDO;
                this.selectDataPatientType = selectDataPatientType;
                this.selectData = selectData;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmInformationObject_Load(object sender, EventArgs e)
        {
            try
            {
                if (HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY__NotDisplayedRouteTypeOver) == "1")
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY__DoNotAllowToEditDefaultRouteType) == "1")
                {
                    chkOppositeLine.Enabled = false;
                    chkAppointment.Enabled = false;
                    chkOnline.Enabled = false;
                    chkEmergency.Enabled = false;
                    chkIntroduce.Enabled = false;
                }
                EnableControl(false);
                InitComboMediOrg();
                InitComboIcd();
                InitComboCmkt();
                InitComboForm();
                InitComboReason();
                AutoCheckType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboReason()
        {
            try
            {
                dtTranpatiReason = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                cboTransferInReasonId.Properties.DataSource = dtTranpatiReason;
                cboTransferInReasonId.Properties.DisplayMember = "TRAN_PATI_REASON_NAME";
                cboTransferInReasonId.Properties.ValueMember = "ID";

                cboTransferInReasonId.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboTransferInReasonId.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboTransferInReasonId.Properties.ImmediatePopup = true;
                cboTransferInReasonId.ForceInitialize();
                cboTransferInReasonId.Properties.View.Columns.Clear();
                cboTransferInReasonId.Properties.PopupFormSize = new System.Drawing.Size(750, 500);
                gridView2.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
                gridView2.Appearance.HeaderPanel.Options.UseFont = true;
                gridView2.Appearance.Row.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
                gridView2.Appearance.Row.Options.UseFont = true;


                GridColumn aColumnCode = cboTransferInReasonId.Properties.View.Columns.AddField("TRAN_PATI_REASON_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 150;
                aColumnCode.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);
                aColumnCode.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);

                GridColumn aColumnName = cboTransferInReasonId.Properties.View.Columns.AddField("TRAN_PATI_REASON_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 600;
                aColumnName.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);
                aColumnName.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboForm()
        {
            try
            {
                dtTranpatiForm = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                cboTransferInFormId.Properties.DataSource = dtTranpatiForm;
                cboTransferInFormId.Properties.DisplayMember = "TRAN_PATI_FORM_NAME";
                cboTransferInFormId.Properties.ValueMember = "ID";

                cboTransferInFormId.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboTransferInFormId.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboTransferInFormId.Properties.ImmediatePopup = true;
                cboTransferInFormId.ForceInitialize();
                cboTransferInFormId.Properties.View.Columns.Clear();
                cboTransferInFormId.Properties.PopupFormSize = new System.Drawing.Size(750, 500);
                gridView1.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
                gridView1.Appearance.HeaderPanel.Options.UseFont = true;
                gridView1.Appearance.Row.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
                gridView1.Appearance.Row.Options.UseFont = true;


                GridColumn aColumnCode = cboTransferInFormId.Properties.View.Columns.AddField("TRAN_PATI_FORM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 150;
                aColumnCode.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);
                aColumnCode.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);

                GridColumn aColumnName = cboTransferInFormId.Properties.View.Columns.AddField("TRAN_PATI_FORM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 600;
                aColumnName.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);
                aColumnName.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCmkt()
        {
            try
            {
                Cmkts = new List<CmktADO>() {
                    new CmktADO(1, "Chuyển đúng tuyến", "Chuyển đúng tuyến CMKT gồm các trường hợp chuyển người bệnh theo đúng quy định tại các khoản 1, 2, 3, 4  Điều 5 Thông tư"),
                    new CmktADO(2, "Chuyển vượt tuyến", "Chuyển vượt tuyến CMKT gồm các trường hợp chuyển người bệnh không theo đúng quy định tại các khoản 1, 2, 3, 4  Điều 5 Thông tư") };

                cboTransferInCmkt.Properties.DataSource = Cmkts;
                cboTransferInCmkt.Properties.DisplayMember = "MA_CMKT";
                cboTransferInCmkt.Properties.ValueMember = "ID";

                cboTransferInCmkt.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboTransferInCmkt.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboTransferInCmkt.Properties.ImmediatePopup = true;
                cboTransferInCmkt.ForceInitialize();
                cboTransferInCmkt.Properties.View.Columns.Clear();
                cboTransferInCmkt.Properties.PopupFormSize = new System.Drawing.Size(750, 500);
                gridLookUpEdit1View.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
                gridLookUpEdit1View.Appearance.HeaderPanel.Options.UseFont = true;
                gridLookUpEdit1View.Appearance.Row.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
                gridLookUpEdit1View.Appearance.Row.Options.UseFont = true;


                GridColumn aColumnCode = cboTransferInCmkt.Properties.View.Columns.AddField("MA_CMKT");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 150;
                aColumnCode.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);
                aColumnCode.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);

                GridColumn aColumnName = cboTransferInCmkt.Properties.View.Columns.AddField("TEN_CMKT");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 600;
                aColumnName.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);
                aColumnName.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkOppositeLine_Click(object sender, EventArgs e)
        {
            try
            {
                this.ResetText();
                resetForm();
                if (!IsOppositeLine)
                {
                    chkOppositeLine.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button__2_;
                    IsOppositeLine = true;
                    chkAppointment.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsAppointment = false;
                    chkOnline.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsOnline = false;
                    chkEmergency.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsEmergency = false;
                    chkIntroduce.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsIntroduce = false;
                    EnableControl(false);
                }
                else
                {
                    chkOppositeLine.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsOppositeLine = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAppointment_Click(object sender, EventArgs e)
        {
            try
            {
                this.ResetText();
                resetForm();
                if (Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.Register.IsAutoShowTransferFormInCaseOfAppointment")) == 1)
                {

                    chkAppointment.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button__2_;
                    IsAppointment = true;
                    chkOppositeLine.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsOppositeLine = false;
                    chkOnline.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsOnline = false;
                    chkEmergency.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsEmergency = false;
                    chkIntroduce.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsIntroduce = false;
                    EnableControl(true);


                    if (InformationObject_ != null )
                    {
                        txtNumber.Text = InformationObject_.TransferInCode;
                        if (!string.IsNullOrEmpty(InformationObject_.TransferInMediOrgCode))
                        {
                            var dt = dtMediOrg.First(o => o.MEDI_ORG_CODE == InformationObject_.TransferInMediOrgCode.Trim());
                            cbotxtWorkPlaceName.EditValue = dt.ID;
                            txtWorkPlaceCode.Text = InformationObject_.TransferInMediOrgCode.Trim();
                            var dt_ = dtIcd.First(o => o.ICD_CODE == InformationObject_.TransferInIcdCode.Trim());
                            cboIcdName.EditValue = dt_.ID;
                            txtIcdCode.Text = InformationObject_.TransferInIcdCode;
                        }
                    }
                }
                else
                {
                    this.ResetText();
                    resetForm();
                    if (!IsAppointment)
                    {
                        chkAppointment.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button__2_;
                        IsAppointment = true;
                        chkOppositeLine.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsOppositeLine = false;
                        chkOnline.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsOnline = false;
                        chkEmergency.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsEmergency = false;
                        chkIntroduce.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsIntroduce = false;
                        EnableControl(false);
                    }
                    else
                    {
                        chkAppointment.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsAppointment = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkOnline_Click(object sender, EventArgs e)
        {
            try
            {
                this.ResetText();
                resetForm();
                if (!IsOnline)
                {
                    chkOnline.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button__2_;
                    IsOnline = true;

                    chkAppointment.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsAppointment = false;
                    chkOppositeLine.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsOppositeLine = false;
                    chkEmergency.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsEmergency = false;
                    chkIntroduce.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsIntroduce = false;
                    EnableControl(false);
                }
                else
                {
                    chkOnline.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsOnline = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEmergency_Click(object sender, EventArgs e)
        {
            try
            {
                this.ResetText();
                resetForm();
                if (!IsEmergency)
                {
                    chkEmergency.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button__2_;
                    IsEmergency = true;

                    chkAppointment.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsAppointment = false;
                    chkOppositeLine.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsOppositeLine = false;
                    chkOnline.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsOnline = false;
                    chkIntroduce.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsIntroduce = false;
                    EnableControl(false);
                }
                else
                {
                    chkEmergency.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsEmergency = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIntroduce_Click(object sender, EventArgs e)
        {
            try
            {
                this.ResetText();
                resetForm();
                if (!IsIntroduce)
                {
                    chkIntroduce.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button__2_;
                    IsIntroduce = true;


                    chkAppointment.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsAppointment = false;
                    chkOppositeLine.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsOppositeLine = false;
                    chkOnline.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsOnline = false;
                    chkEmergency.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsEmergency = false;
                    EnableControl(true);
                }
                else
                {
                    chkIntroduce.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                    IsIntroduce = false;
                    EnableControl(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    if (e.Button.Index == 0)
                    {
                        chkEdit.Properties.Buttons[0].Visible = false;
                        chkEdit.Properties.Buttons[1].Visible = true;
                        IsEdit = true;
                    }
                    else if (e.Button.Index == 1)
                    {
                        chkEdit.Properties.Buttons[0].Visible = true;
                        chkEdit.Properties.Buttons[1].Visible = false;
                        IsEdit = false;
                    }

                    if (IsEdit)
                    {
                        txtIcdName.Visible = true;
                        cboIcdName.Visible = false;
                    }
                    else
                    {
                        txtIcdName.Visible = false;
                        cboIcdName.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMediOrg()
        {
            try
            {
                dtMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                //this.InitComboCommon(this.cbotxtWorkPlaceName, dtMediOrg, "ID", "MEDI_ORG_NAME", "MEDI_ORG_CODE");

                cbotxtWorkPlaceName.Properties.DataSource = dtMediOrg;
                cbotxtWorkPlaceName.Properties.DisplayMember = "MEDI_ORG_NAME";
                cbotxtWorkPlaceName.Properties.ValueMember = "ID";

                cbotxtWorkPlaceName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbotxtWorkPlaceName.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbotxtWorkPlaceName.Properties.ImmediatePopup = true;
                cbotxtWorkPlaceName.ForceInitialize();
                cbotxtWorkPlaceName.Properties.View.Columns.Clear();
                cbotxtWorkPlaceName.Properties.PopupFormSize = new System.Drawing.Size(750, 500);
                customGridView1.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
                customGridView1.Appearance.HeaderPanel.Options.UseFont = true;
                customGridView1.Appearance.Row.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
                customGridView1.Appearance.Row.Options.UseFont = true;


                GridColumn aColumnCode = cbotxtWorkPlaceName.Properties.View.Columns.AddField("MEDI_ORG_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 150;
                aColumnCode.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);
                aColumnCode.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);

                GridColumn aColumnName = cbotxtWorkPlaceName.Properties.View.Columns.AddField("MEDI_ORG_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 600;
                aColumnName.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);
                aColumnName.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboIcd()
        {
            try
            {
                dtIcd = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                //this.InitComboCommon(this.cboIcdName, dtIcd, "ID", "ICD_NAME", "ICD_CODE");

                cboIcdName.Properties.DataSource = dtIcd;
                cboIcdName.Properties.DisplayMember = "ICD_NAME";
                cboIcdName.Properties.ValueMember = "ID";

                cboIcdName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboIcdName.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboIcdName.Properties.ImmediatePopup = true;
                cboIcdName.ForceInitialize();
                cboIcdName.Properties.View.Columns.Clear();
                cboIcdName.Properties.PopupFormSize = new System.Drawing.Size(750, 500);
                customGridLookUpEdit1View.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
                customGridLookUpEdit1View.Appearance.HeaderPanel.Options.UseFont = true;
                customGridLookUpEdit1View.Appearance.Row.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
                customGridLookUpEdit1View.Appearance.Row.Options.UseFont = true;

                GridColumn aColumnCode = cboIcdName.Properties.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 150;
                aColumnCode.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 17, System.Drawing.FontStyle.Regular);
                aColumnCode.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17, System.Drawing.FontStyle.Regular);

                GridColumn aColumnName = cboIcdName.Properties.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 600;
                aColumnName.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 17, System.Drawing.FontStyle.Regular);
                aColumnName.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 17, System.Drawing.FontStyle.Regular);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "Mã", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 2));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "Tên", (displayMemberWidth > 0 ? displayMemberWidth : 250), 1));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 350);
                }

                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, true, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControl(bool enable)
        {
            try
            {
                txtNumber.Enabled = enable;
                txtWorkPlaceCode.Enabled = enable;
                txtIcdCode.Enabled = enable;
                cbotxtWorkPlaceName.Enabled = enable;
                cboIcdName.Enabled = enable;
                txtIcdName.Enabled = enable;
                chkEdit.Enabled = enable;
                label1.Enabled = enable;
                IsEnable = enable;
                dtTransferInTimeFrom.Enabled = enable;
                dtTransferInTimeTo.Enabled = enable;
                cboTransferInCmkt.Enabled = enable;
                cboTransferInFormId.Enabled = enable;
                cboTransferInReasonId.Enabled = enable;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void cbotxtWorkPlaceName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbotxtWorkPlaceName.EditValue != null)
                {
                    var dt = dtMediOrg.First(o => o.ID == Int64.Parse(cbotxtWorkPlaceName.EditValue.ToString()));
                    txtWorkPlaceCode.Text = dt.MEDI_ORG_CODE;
                }
                else
                {
                    txtWorkPlaceCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtWorkPlaceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtWorkPlaceCode.Text))
                    {
                        var dt = dtMediOrg.First(o => o.MEDI_ORG_CODE == txtWorkPlaceCode.Text.Trim());
                        cbotxtWorkPlaceName.EditValue = dt.ID;
                        txtIcdCode.Focus();
                        txtIcdCode.SelectAll();
                    }
                    else
                    {
                        cbotxtWorkPlaceName.Focus();
                        cbotxtWorkPlaceName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbotxtWorkPlaceName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cbotxtWorkPlaceName.EditValue != null)
                {
                    txtIcdCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtIcdCode.Text))
                    {
                        var dt = dtIcd.First(o => o.ICD_CODE == txtIcdCode.Text.Trim());
                        cboIcdName.EditValue = dt.ID;
                        cboTransferInCmkt.Focus();
                        cboTransferInCmkt.ShowPopup();
                    }
                    else
                    {
                        if (!IsEdit)
                        {
                            cboIcdName.Focus();
                            cboIcdName.ShowPopup();
                        }
                        else
                        {
                            txtIcdName.Focus();
                            txtIcdName.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboIcdName.EditValue != null)
                {
                    //btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboIcdName.EditValue != null && !string.IsNullOrEmpty(cboIcdName.EditValue.ToString()) && !string.IsNullOrWhiteSpace(cboIcdName.EditValue.ToString()))
                {
                    var dt = dtIcd.First(o => o.ID == Int64.Parse(cboIcdName.EditValue.ToString()));
                    txtIcdCode.Text = dt.ICD_CODE;
                }
                else
                {
                    txtIcdCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsOppositeLine && !IsAppointment && !IsOnline && !IsEmergency && !IsIntroduce)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn vui lòng chọn loại thông tin chuyển tuyến.", "Thông báo", MessageBoxButtons.OK);
                    return;
                }

                sdoInformationObj = new HisExamRegisterKioskSDO();
                if (IsOppositeLine)
                {
                    sdoInformationObj.RightRouteCode = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE;
                }
                else if (IsAppointment)
                {
                    sdoInformationObj.RightRouteCode = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                    sdoInformationObj.RightRouteTypeCode = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT;
                }
                else if (IsOnline)
                {
                    sdoInformationObj.RightRouteCode = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                    sdoInformationObj.RightRouteTypeCode = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER;
                }
                else if (IsEmergency)
                {
                    sdoInformationObj.RightRouteCode = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                    sdoInformationObj.RightRouteTypeCode = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                }
                else if (IsIntroduce)
                {
                    sdoInformationObj.RightRouteCode = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                    sdoInformationObj.RightRouteTypeCode = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
                }
                if (IsEnable)
                {
                    string err = "";

                    if (string.IsNullOrEmpty(txtNumber.Text)) err = "Thiếu thông tin số chuyển tuyến";
                    else if (string.IsNullOrEmpty(txtWorkPlaceCode.Text) || cbotxtWorkPlaceName.EditValue == null) err = "Thiếu thông tin nơi chuyển đến";
                    else if (string.IsNullOrEmpty(txtIcdCode.Text.Trim()) || (string.IsNullOrEmpty(txtIcdName.Text) && cboIcdName.EditValue == null)) err = "Thiếu thông tin chẩn đoán chính";

                    if (!string.IsNullOrEmpty(err))
                    {
                        XtraMessageBox.Show(err, "Thông báo");
                        return;
                    }

                    sdoInformationObj.TransferInCode = txtNumber.Text;
                    sdoInformationObj.TransferInMediOrgCode = txtWorkPlaceCode.Text;
                    sdoInformationObj.TransferInMediOrgName = cbotxtWorkPlaceName.EditValue != null ? dtMediOrg.First(o => o.ID == Int64.Parse(cbotxtWorkPlaceName.EditValue.ToString())).MEDI_ORG_NAME : "";
                    sdoInformationObj.TransferInIcdCode = txtIcdCode.Text.Trim();
                    if (!IsEdit)
                        sdoInformationObj.TransferInIcdName = cboIcdName.EditValue != null ? dtIcd.First(o => o.ID == Int64.Parse(cboIcdName.EditValue.ToString())).ICD_NAME : "";
                    else
                        sdoInformationObj.TransferInIcdName = txtIcdName.Text;

                    sdoInformationObj.TransferInTimeFrom = dtTransferInTimeFrom.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTransferInTimeFrom.DateTime) : null;
                    sdoInformationObj.TransferInTimeTo = dtTransferInTimeTo.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTransferInTimeTo.DateTime) : null;
                    sdoInformationObj.TransferInCmkt = cboTransferInCmkt.EditValue != null ? (long?)Int64.Parse(cboTransferInCmkt.EditValue.ToString()) : null;
                    sdoInformationObj.TransferInFormId = cboTransferInFormId.EditValue != null ? (long?)dtTranpatiForm.First(o => o.ID == Int64.Parse(cboTransferInFormId.EditValue.ToString())).ID : null;
                    sdoInformationObj.TransferInReasonId = cboTransferInReasonId.EditValue != null ? (long?)dtTranpatiReason.First(o => o.ID == Int64.Parse(cboTransferInReasonId.EditValue.ToString())).ID : null;

                }



                if (selectData != null)
                {
                    selectData(sdoInformationObj);
                }
                if (selectDataPatientType != null)
                {
                    if (IsOppositeLine && MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT != HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
                        && MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE != HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT)
                    {
                        selectDataPatientType(GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE")));
                    }
                    else
                    {
                        selectDataPatientType(GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")));
                    }
                }

                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                if ((data != null && data.ID > 0))
                {
                    result = data.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = 0;
            }
            return result;
        }

        private void txtNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtWorkPlaceCode.Focus();
                    txtWorkPlaceCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void resetForm()
        {

            try
            {

                txtIcdCode.Text = "";
                cboIcdName.Text = "";
                txtNumber.Text = "";
                cbotxtWorkPlaceName.EditValue = null;
                txtWorkPlaceCode.Text = "";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AutoCheckType()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("1_______");
                var orgHein = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == resultHistoryLDO.maDKBD);
                var branch = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch;
                Inventec.Common.Logging.LogSystem.Error("2_______");

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => branch), branch));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => orgHein), orgHein));
                if (orgHein != null && !String.IsNullOrEmpty(orgHein.MEDI_ORG_CODE))
                {
                    Inventec.Common.Logging.LogSystem.Error("3_______");
                    var ProvinceCode = orgHein.MEDI_ORG_CODE.Substring(0, 2);
                    if ((MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
                        || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT)
                        && branch != null && ProvinceCode == branch.HEIN_MEDI_ORG_CODE.Substring(0, 2)
                        && (orgHein.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT || orgHein.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE)
                        && (string.IsNullOrEmpty(branch.HEIN_MEDI_ORG_CODE) || !string.IsNullOrEmpty(branch.HEIN_MEDI_ORG_CODE) && !orgHein.MEDI_ORG_CODE.Contains(branch.HEIN_MEDI_ORG_CODE))
                        && (string.IsNullOrEmpty(branch.ACCEPT_HEIN_MEDI_ORG_CODE) || !string.IsNullOrEmpty(branch.ACCEPT_HEIN_MEDI_ORG_CODE) && !orgHein.MEDI_ORG_CODE.Contains(branch.ACCEPT_HEIN_MEDI_ORG_CODE))
                        && (string.IsNullOrEmpty(branch.SYS_MEDI_ORG_CODE) || !string.IsNullOrEmpty(branch.SYS_MEDI_ORG_CODE) && !orgHein.MEDI_ORG_CODE.Contains(branch.SYS_MEDI_ORG_CODE))
                        )
                    {
                        Inventec.Common.Logging.LogSystem.Error("4_______");
                        chkOnline.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button__2_;
                        IsOnline = true;
                        chkAppointment.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsAppointment = false;
                        chkOppositeLine.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsOppositeLine = false;
                        chkEmergency.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsEmergency = false;
                        chkIntroduce.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsIntroduce = false;
                        EnableControl(false);
                    }
                    else if (branch != null
                        && (string.IsNullOrEmpty(branch.HEIN_MEDI_ORG_CODE) || !string.IsNullOrEmpty(branch.HEIN_MEDI_ORG_CODE) && !orgHein.MEDI_ORG_CODE.Contains(branch.HEIN_MEDI_ORG_CODE))
                        && (string.IsNullOrEmpty(branch.ACCEPT_HEIN_MEDI_ORG_CODE) || !string.IsNullOrEmpty(branch.ACCEPT_HEIN_MEDI_ORG_CODE) && !orgHein.MEDI_ORG_CODE.Contains(branch.ACCEPT_HEIN_MEDI_ORG_CODE))
                        && (string.IsNullOrEmpty(branch.SYS_MEDI_ORG_CODE) || !string.IsNullOrEmpty(branch.SYS_MEDI_ORG_CODE) && !orgHein.MEDI_ORG_CODE.Contains(branch.SYS_MEDI_ORG_CODE))
                        && (ProvinceCode != branch.HEIN_MEDI_ORG_CODE.Substring(0, 2) || (orgHein.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL || orgHein.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE) || (MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
                        || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT)))
                    {
                        Inventec.Common.Logging.LogSystem.Error("5_______");
                        chkOppositeLine.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button__2_;
                        IsOppositeLine = true;
                        chkAppointment.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsAppointment = false;
                        chkOnline.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsOnline = false;
                        chkEmergency.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsEmergency = false;
                        chkIntroduce.Image = global::HIS.Desktop.Plugins.RegisterExamKiosk.Properties.Resources.radio_button;
                        IsIntroduce = false;
                        EnableControl(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTransferInCmkt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTransferInFormId.Focus();
                    cboTransferInFormId.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTransferInFormId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTransferInReasonId.Focus();
                    cboTransferInReasonId.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTransferInReasonId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

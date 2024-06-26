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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.UC.UCTransPati.Config;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.DelegateRegister;
using HIS.UC.Icd;
using HIS.UC.SecondaryIcd;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using System.Resources;
using HIS.UC.UCTransPati.Base;

namespace HIS.UC.UCTransPati
{
    public partial class UCTransPati : UserControl
    {
        #region Declare

        bool IsObligatoryTranferMediOrg { get; set; }
        string _TextIcdName = "";
        string autoCheckIcd { get; set; }
        int positionHandleControl = -1;
        Action<object> dlgFocusNextUserControl;
        internal IcdProcessor icdProcessor;
        internal UserControl ucIcd;


        internal SecondaryIcdProcessor SubIcdProcessor;
        internal UserControl ucSubIcd;
        #endregion

        #region Construct - Load

        public UCTransPati()
        {
            InitializeComponent();
            try
            {
                DataStore.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCTransPati_Load(object sender, EventArgs e)
        {
            try
            {
                this.IsObligatoryTranferMediOrg = DataStore.IsObligatoryTranferMediOrg || DataStore.ObligatoryTranferMediOrg == 3;
                InitDataToControl();
                this.txtMaNoiChuyenDen.Focus();
                this.txtMaNoiChuyenDen.SelectAll();
                this.DisableSomeControlByKeyConfig(DataStore.IsVisibleSomeControl);
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCTransPati
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCTransPati.Resources.Lang", typeof(UCTransPati).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTransPati.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLyDoChuyen.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTransPati.cboLyDoChuyen.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHinhThucChuyen.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTransPati.cboHinhThucChuyen.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNoiChuyenDen.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTransPati.cboNoiChuyenDen.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChuyenTuyen.Properties.NullText = Inventec.Common.Resource.Get.Value("UCTransPati.cboChuyenTuyen.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBenhChinh.Text = Inventec.Common.Resource.Get.Value("UCTransPati.lciBenhChinh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSoChuyenVien.Text = Inventec.Common.Resource.Get.Value("UCTransPati.lciSoChuyenVien.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChuyenTuyen.Text = Inventec.Common.Resource.Get.Value("UCTransPati.lciChuyenTuyen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNoiChuyenDen.Text = Inventec.Common.Resource.Get.Value("UCTransPati.lciNoiChuyenDen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHinhThucChuyen.Text = Inventec.Common.Resource.Get.Value("UCTransPati.lciHinhThucChuyen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLyDoChuyen.Text = Inventec.Common.Resource.Get.Value("UCTransPati.lciLyDoChuyen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFordtFromTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTransPati.lciFordtFromTime.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFordtFromTime.Text = Inventec.Common.Resource.Get.Value("UCTransPati.lciFordtFromTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFordtToTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTransPati.lciFordtToTime.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFordtToTime.Text = Inventec.Common.Resource.Get.Value("UCTransPati.lciFordtToTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        private void InitUcIcd()
        {
            try
            {
                this.icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.IsAcceptWordNotInData = true;
                ado.Width = 660;
                ado.Height = 24;
                ado.IsColor = IsObligatoryTranferMediOrg;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = autoCheckIcd == GlobalVariables.CommonStringTrue;
                ado.IsObligatoryTranferMediOrg = IsObligatoryTranferMediOrg;
                this.ucIcd = (UserControl)this.icdProcessor.Run(ado);

                if (this.ucIcd != null)
                {
                    this.pnlIcd.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSubIcd()
        {
            try
            {
                this.SubIcdProcessor = new SecondaryIcdProcessor();
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusIcd;
                ado.TextLblIcd = "Chẩn đoán phụ:";
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.Width = 660;
                ado.Height = 24;
                ado.HisIcds = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                this.ucSubIcd = (UserControl)this.SubIcdProcessor.Run(ado);

                if (this.ucIcd != null)
                {
                    this.panel1.Controls.Add(this.ucSubIcd);
                    this.ucSubIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.icdProcessor != null && this.ucIcd != null)
                {
                    var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }
        private void NextForcusSubIcd()
        {
            try
            {
                SubIcdProcessor.FocusControl(ucSubIcd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void NextForcusIcd()
        {
            try
            {
                txtInCode.Focus();
                txtInCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Event txtMaNoiChuyenDen

        private void txtMaNoiChuyenDen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadNoiChuyenDenCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event cboNoiChuyenDen

        private void cboNoiChuyenDen_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboNoiChuyenDen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MEDI_ORG data = DataStore.MediOrgs.SingleOrDefault(o => o.MEDI_ORG_CODE.Equals(this.cboNoiChuyenDen.EditValue ?? ""));
                        if (data != null)
                        {
                            this.txtMaNoiChuyenDen.Text = data.MEDI_ORG_CODE;
                            this.cboNoiChuyenDen.Properties.Buttons[1].Visible = true;
                            this.ProcessLevelOfMediOrg();
                        }
                    }
                    this.icdProcessor.FocusControl(ucIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiChuyenDen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboNoiChuyenDen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MEDI_ORG data = DataStore.MediOrgs.SingleOrDefault(o => o.MEDI_ORG_CODE.Equals(this.cboNoiChuyenDen.EditValue ?? ""));
                        if (data != null)
                        {
                            this.txtMaNoiChuyenDen.Text = data.MEDI_ORG_CODE;
                            this.cboNoiChuyenDen.Properties.Buttons[1].Visible = true;
                            this.ProcessLevelOfMediOrg();
                            this.icdProcessor.FocusControl(ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private void cboNoiChuyenDen_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboNoiChuyenDen.EditValue = null;
                    this.cboNoiChuyenDen.Properties.Buttons[1].Visible = false;
                    this.txtMaNoiChuyenDen.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        //#region Event txtMaChanDoan

        //private void txtMaChanDoanTD_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        //{
        //    try
        //    {
        //        if (!String.IsNullOrEmpty(txtMaChuanDoanTD.Text))
        //        {
        //            var searchResult = ((DataStore.Icds != null && DataStore.Icds.Count > 0) ? DataStore.Icds.Where(o => o.ICD_CODE.ToUpper() == txtMaChuanDoanTD.Text.ToUpper()).ToList() : null);
        //            if (searchResult == null || searchResult.Count == 0)
        //            {
        //                e.ErrorText = ResourceMessage.MaBenhChinhKhongHopLe;
        //            }
        //        }
        //        else
        //        {
        //            e.ErrorText = "";
        //        }

        //        AutoValidate = AutoValidate.EnableAllowFocusChange;
        //        e.ExceptionMode = ExceptionMode.DisplayError;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtMaChanDoanTD_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            this.LoadChuanDoanTDCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtMaChanDoanTD_Validating(object sender, CancelEventArgs e)
        //{
        //    try
        //    {
        //        if (!String.IsNullOrEmpty(txtMaChuanDoanTD.Text))
        //        {
        //            var searchResult = ((DataStore.Icds != null && DataStore.Icds.Count > 0) ? DataStore.Icds.Where(o => o.ICD_CODE.ToUpper() == txtMaChuanDoanTD.Text.ToUpper()).ToList() : null);
        //            if (searchResult == null || searchResult.Count == 0)
        //            {
        //                e.Cancel = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //#endregion

        //#region Event cboChanDoan

        //private void cboChanDoanDT_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
        //        {
        //            this.cboChuanDoanTD.EditValue = null;
        //            this.cboChuanDoanTD.Properties.Buttons[1].Visible = false;
        //            this.txtMaChuanDoanTD.Text = "";
        //            this.txtMaChuanDoanTD.ErrorText = "";
        //            this.chkHasDialogText.Checked = false;
        //            this.chkHasDialogText.Enabled = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void cboChanDoanDT_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
        //        {
        //            if (this.cboChuanDoanTD.EditValue != null)
        //                this.ChangecboChanDoanTD();
        //            else if (this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdName))
        //                this.ChangecboChanDoanTD_V2_GanICDNAME(this._TextIcdName);
        //            else
        //                SendKeys.Send("{TAB}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void cboChanDoanDT_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            if (this.cboChuanDoanTD.EditValue != null)
        //                this.ChangecboChanDoanTD();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void cboChanDoanDT_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(cboChuanDoanTD.Text))
        //        {
        //            cboChuanDoanTD.EditValue = null;
        //            txtMaChuanDoanTD.Text = "";
        //            chkHasDialogText.Checked = false;
        //        }
        //        else
        //        {
        //            this._TextIcdName = cboChuanDoanTD.Text;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //#endregion

        //#region chkHasDialogText

        //private void chkHasDialogText_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (chkHasDialogText.Checked == true)
        //        {
        //            cboChuanDoanTD.Visible = false;
        //            txtDialogText.Visible = true;
        //            if (this.IsObligatoryTranferMediOrg)
        //                txtDialogText.Text = this._TextIcdName;
        //            else
        //                txtDialogText.Text = cboChuanDoanTD.Text;

        //            txtDialogText.Focus();
        //            txtDialogText.SelectAll();
        //        }
        //        else
        //        {
        //            txtDialogText.Visible = false;
        //            cboChuanDoanTD.Visible = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void chkHasDialogText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            SendKeys.Send("{TAB}");
        //            SendKeys.Send("^a");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //#endregion

        #region Event txtIncode

        private void txtInCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboChuyenTuyen.Focus();
                    this.cboChuyenTuyen.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event txtMaHinhThucChuyen

        private void txtMaHinhThucChuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadTranPatiFormCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event cboHinhThucChuyen

        private void cboHinhThucChuyen_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboHinhThucChuyen.EditValue = null;
                    this.cboHinhThucChuyen.Properties.Buttons[1].Visible = false;
                    this.txtMaHinhThucChuyen.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHinhThucChuyen_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboHinhThucChuyen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = DataStore.TranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboHinhThucChuyen.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.txtMaHinhThucChuyen.Text = data.TRAN_PATI_FORM_CODE;
                            this.cboHinhThucChuyen.Properties.Buttons[1].Visible = true;
                        }
                    }

                    this.txtMaLyDoChuyen.Focus();
                    this.txtMaLyDoChuyen.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHinhThucChuyen_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboHinhThucChuyen.Properties.Buttons[1].Visible = (!String.IsNullOrEmpty((this.cboHinhThucChuyen.EditValue ?? "").ToString()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHinhThucChuyen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboHinhThucChuyen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = DataStore.TranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboHinhThucChuyen.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.txtMaHinhThucChuyen.Text = data.TRAN_PATI_FORM_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event txtMaLyDoCHuyen

        private void txtMaLyDoChuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadTranPatiReasonCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event cboLyDoChuyen

        private void cboLyDoChuyen_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboLyDoChuyen.EditValue = null;
                    this.cboLyDoChuyen.Properties.Buttons[1].Visible = false;
                    this.txtMaLyDoChuyen.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLyDoChuyen_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboLyDoChuyen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = DataStore.TranPatiReasons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboLyDoChuyen.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.cboLyDoChuyen.Properties.Buttons[1].Visible = true;
                            this.txtMaLyDoChuyen.Text = data.TRAN_PATI_REASON_CODE;
                        }
                    }
                    if (this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl(null);// tạo hàm set next focus
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLyDoChuyen_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboLyDoChuyen.Properties.Buttons[1].Visible = (!String.IsNullOrEmpty((this.cboLyDoChuyen.EditValue ?? "").ToString()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLyDoChuyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboLyDoChuyen.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = DataStore.TranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboLyDoChuyen.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.cboLyDoChuyen.Properties.Buttons[1].Visible = true;
                            if (this.dlgFocusNextUserControl != null)
                                this.dlgFocusNextUserControl(null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region cboChuyenTuyen

        private void cboChuyenTuyen_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboChuyenTuyen.EditValue = null;
                    this.cboChuyenTuyen.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChuyenTuyen_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                this.txtMaHinhThucChuyen.Focus();
                this.txtMaHinhThucChuyen.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void dtFromTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                dtToTime.Focus();
                dtToTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtToTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                txtMaNoiChuyenDen.Focus();
                txtMaNoiChuyenDen.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtFromTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtToTime.Focus();
                    dtToTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtToTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaNoiChuyenDen.Focus();
                    txtMaNoiChuyenDen.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboReviews_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboReviews.SelectedIndex = -1;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

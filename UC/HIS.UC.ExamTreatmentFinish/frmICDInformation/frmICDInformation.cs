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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.ExamTreatmentFinish.Config;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.ExamTreatmentFinish
{
    public partial class frmICDInformation : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        HIS_TREATMENT treatment = new HIS_TREATMENT();
        Action<HIS_TREATMENT> actSaveClick;
        Inventec.Desktop.Common.Modules.Module moduleData;
        long RoomId = 0;
        bool isAutoCheckIcd;
        long autoCheckIcd;
        List<HIS_ICD> currentIcds;
        List<HIS_ICD> currentTraditionalIcds;
        string _TextIcdName = "";
        bool IsAcceptWordNotInData = false;
        bool IsObligatoryTranferMediOrg = false;
        bool isAllowNoIcd = true;
        string[] icdSeparators = new string[] { ",", ";" };
        #endregion

        #region Construct
        public frmICDInformation(Inventec.Desktop.Common.Modules.Module moduleData,HIS_TREATMENT _treatment, Action<HIS_TREATMENT> _actSaveClick)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.treatment = _treatment;
                this.actSaveClick = _actSaveClick;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmICDInformation_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                HisConfig.GetConfig();
                this.autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                this.isAutoCheckIcd = (this.autoCheckIcd == 1);
                //if (this.RoomId > 0)
                //{
                //    this.isAllowNoIcd = (BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.RoomId).IS_ALLOW_NO_ICD == 1);
                //}
                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                this.currentTraditionalIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && p.IS_TRADITIONAL == 1).OrderBy(o => o.ICD_CODE).ToList();

                ValidateForm();
                //Fill data into datasource combo
                FillDataToControlsForm();

                //Gan gia tri mac dinh
                SetDefaultValue();
                SetDefaultFocus();
                SetCaptionByLanguageKey();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmICDInformation
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ExamTreatmentFinish.Resources.Lang", typeof(frmICDInformation).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmICDInformation.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcds.Properties.NullText = Inventec.Common.Resource.Get.Value("frmICDInformation.cboIcds.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmICDInformation.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmICDInformation.txtIcdText.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkEditIcd.Properties.Caption = Inventec.Common.Resource.Get.Value("frmICDInformation.chkEditIcd.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciICDCode.Text = Inventec.Common.Resource.Get.Value("frmICDInformation.lciICDCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciICDSubCode.Text = Inventec.Common.Resource.Get.Value("frmICDInformation.lciICDSubCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmICDInformation.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmICDInformation.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmICDInformation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                if (this.treatment == null)
                    return;

                if (String.IsNullOrWhiteSpace(this.treatment.SHOW_ICD_CODE) 
                    && String.IsNullOrWhiteSpace(this.treatment.SHOW_ICD_NAME) 
                    && String.IsNullOrWhiteSpace(this.treatment.SHOW_ICD_SUB_CODE)
                    && String.IsNullOrWhiteSpace(this.treatment.SHOW_ICD_TEXT))
                {
                    LoadIcdToControl(this.treatment.ICD_CODE, this.treatment.ICD_NAME);
                    stringIcds(this.treatment.ICD_SUB_CODE, this.treatment.ICD_TEXT);
                }
                else
                {
                    LoadIcdToControl(this.treatment.SHOW_ICD_CODE, this.treatment.SHOW_ICD_NAME);
                    stringIcds(this.treatment.SHOW_ICD_SUB_CODE, this.treatment.SHOW_ICD_TEXT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtIcdCode.Focus();
                txtIcdCode.SelectAll();
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
                    LoadIcdCombo(txtIcdCode.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "ICD không đúng";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcds_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcds.Properties.Buttons[1].Visible)
                        return;
                    this._TextIcdName = "";
                    cboIcds.EditValue = null;
                    txtIcdCode.Text = "";
                    txtIcdMainText.Text = "";
                    cboIcds.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                    else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdName))
                        this.ChangecboChanDoanTD_V2_GanICDNAME(this._TextIcdName);
                    else
                        SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.1");
                HIS_ICD icd = null;
                if (this.cboIcds.EditValue != null)
                    icd = this.currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboIcds.EditValue.ToString()));
                //if (this.isExecuteValueChanged && refeshIcd != null)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.2");
                //    this.refeshIcd(icd);
                //}

                if (icd != null)
                {
                    if (icd != null && icd.IS_REQUIRE_CAUSE == 1 && !this.isAllowNoIcd)
                    {
                        this.LoadRequiredCause(true);
                    }
                    else
                        this.LoadRequiredCause(false);
                }
                else
                {
                    this.LoadRequiredCause(false);
                }

                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcds_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcds.ClosePopup();
                    cboIcds.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcds.ClosePopup();
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                }
                else
                    cboIcds.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcds.Text))
                {
                    cboIcds.EditValue = null;
                    txtIcdMainText.Text = "";
                    chkEditIcd.Checked = false;
                }
                else
                {
                    this._TextIcdName = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditIcd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditIcd.Checked == true)
                {
                    cboIcds.Visible = false;
                    txtIcdMainText.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtIcdMainText.Text = this._TextIcdName;
                    else
                        txtIcdMainText.Text = cboIcds.Text;
                    txtIcdMainText.Focus();
                    txtIcdMainText.SelectAll();
                }
                else if (chkEditIcd.Checked == false)
                {
                    txtIcdMainText.Visible = false;
                    cboIcds.Visible = true;
                    txtIcdMainText.Text = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtIcdMainText.Text != null)
                    {
                        //this.data.DelegateRefeshIcdMainText(txtIcdMainText.Text);
                    }
                    if (cboIcds.EditValue != null)
                    {
                        //var hisIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == (long)cboIcds.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshIcd(hisIcd);
                    }
                    NextForcusSubIcd();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!ProccessorByIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
                    {
                        e.Handled = true;
                        return;
                    }
                    txtIcdText.Focus();
                    txtIcdText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProccessorByIcdCode(string currentValue)
        {
            bool valid = true;
            try
            {
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                if (!CheckIcdWrongCode(ref strIcdNames, ref strWrongIcdCodes))
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Debug("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
                }
                this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckIcdWrongCode(ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(this.txtIcdSubCode.Text))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = this.txtIcdSubCode.Text.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = this.currentIcds.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode);
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format("Không tìm thấy icd tương ứng với các mã sau: {0}", strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = this.txtIcdSubCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = this.txtIcdSubCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            this.txtIcdSubCode.Focus();
                            this.txtIcdSubCode.Select(startPositionWarm, lenghtPositionWarm);
                            valid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = ProcessIcdNameChanged(icdName__Olds, icdNames, this.currentIcds);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdSubCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string ProcessIcdNameChanged(string oldIcdNames, string newIcdNames, List<HIS_ICD> icdList)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = icdList.Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void txtIcdSubCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcdFull FormSecondaryIcd = new frmSecondaryIcdFull(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void stringIcds(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtIcdSubCode.Text = icdCode;
                }
                if (!string.IsNullOrEmpty(icdName))
                {
                    txtIcdText.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcdFull FormSecondaryIcd = new frmSecondaryIcdFull(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
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

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
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
                ProcessSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSave()
        {
            try
            {
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }
                UpdateTreatmentFromDataForm(ref this.treatment);
                actSaveClick(this.treatment);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void UpdateTreatmentFromDataForm(ref MOS.EFMODEL.DataModels.HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment == null)
                {
                    return;
                }
                if (chkEditIcd.Checked)
                {
                    treatment.SHOW_ICD_NAME = !String.IsNullOrEmpty(txtIcdMainText.Text) ? txtIcdMainText.Text : null;
                }
                else
                {
                    treatment.SHOW_ICD_NAME = !String.IsNullOrEmpty(cboIcds.Text) ? cboIcds.Text : null;
                }

                treatment.SHOW_ICD_CODE = !String.IsNullOrEmpty(txtIcdCode.Text) ? txtIcdCode.Text : null;

                treatment.SHOW_ICD_SUB_CODE = !String.IsNullOrEmpty(txtIcdSubCode.Text) ? txtIcdSubCode.Text : null;
                
                treatment.SHOW_ICD_TEXT = !String.IsNullOrEmpty(txtIcdText.Text) ? txtIcdText.Text : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

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
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Drawing;
using System;
using His.UC.LibraryMessage;
using ACS.EFMODEL.DataModels;

namespace HIS.UC.FormType.UserNameCombo
{
    public partial class UCUserNameCombo : DevExpress.XtraEditors.XtraUserControl
    {
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCUserNameCombo(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 35;
                this.config = config;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void Init()
        {
            try
            {
                HIS.UC.FormType.Loader.UserNameLoader.LoadDataToCombo(comboBoxEdit1);

                if (this.isValidData)
                {
                    Validation();
                    layoutControlItem2.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                SetTitle();
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    layoutControlItem2.Text = this.config.DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRoomCode_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    var search = textEdit1.Text.Trim().ToLower();
                    if (!String.IsNullOrEmpty(search))
                    {
                        var mediStocks = Config.HisFormTypeConfig.VHisRooms.Where(f => f.ROOM_CODE.ToLower().Contains(search)).ToList();
                        if (mediStocks != null)
                        {
                            if (mediStocks.Count == 1)
                            {
                                textEdit1.Text = mediStocks[0].ROOM_CODE;
                                comboBoxEdit1.EditValue = mediStocks[0].ID;
                                System.Windows.Forms.SendKeys.Send("{TAB}");
                            }
                            else
                            {
                                comboBoxEdit1.ShowPopup();
                                comboBoxEdit1.Focus();
                            }
                        }
                    }
                    else
                    {
                        comboBoxEdit1.ShowPopup();
                        comboBoxEdit1.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void comboBoxEdit1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (comboBoxEdit1.EditValue != null)
                    {
                        var department = Config.AcsFormTypeConfig.HisAcsUser.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit1.EditValue.ToString()));
                        if (department != null)
                        {
                            textEdit1.Text = department.LOGINNAME;
                        }
                    }
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (comboBoxEdit1.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.VHisRooms.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit1.EditValue.ToString()));
                        if (department != null)
                        {
                            textEdit1.Text = department.ROOM_CODE;
                        }
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetValue()
        {
            string value = "";
            try
            {
                string departmentId = (string)comboBoxEdit1.EditValue;
                departmentId = "\"" + departmentId + "\"";
                value = String.Format(this.config.JSON_OUTPUT, departmentId);
            }
            catch (Exception ex)
            {
                value = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }
        public void SetValue()
        {
            try
            {
                if (this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,this.config.JSON_OUTPUT, this.report.JSON_FILTER);
                    if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                    {
                        comboBoxEdit1.Properties.DataSource = Config.AcsFormTypeConfig.HisAcsUser;
                        textEdit1.Text = (Config.AcsFormTypeConfig.HisAcsUser.FirstOrDefault(f => f.ID == Inventec.Common.TypeConvert.Parse.ToInt64(value)) ?? new ACS_USER()).LOGINNAME;
                        comboBoxEdit1.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (this.isValidData != null && this.isValidData)
                {
                    this.positionHandleControl = -1;
                    result = dxValidationProvider1.Validate();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #region Validation
        private void ValidateRoom()
        {
            try
            {
                HIS.UC.FormType.UserNameCombo.Validation.UserNameValidationRule validRule = new HIS.UC.FormType.UserNameCombo.Validation.UserNameValidationRule();
                validRule.textEdit1 = textEdit1;
                validRule.comboBoxEdit1 = comboBoxEdit1;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit1, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Validation()
        {
            try
            {
                ValidateRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void comboBoxEdit1_Closed_1(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (comboBoxEdit1.EditValue != null)
                    {
                        var department = Config.AcsFormTypeConfig.HisAcsUser.FirstOrDefault(f => f.LOGINNAME == comboBoxEdit1.EditValue.ToString());
                        if (department != null)
                        {
                            textEdit1.Text = department.LOGINNAME;
                        }
                    }
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCUserNameCombo_Load(object sender, EventArgs e)
        {
            try
            {
                layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_USER_NAME_COMBO_LAYOUT_CONTROL_ITEM2", Resources.ResourceLanguageManager.languageUCUserNameCombo, Base.LanguageManager.GetCulture());
                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}


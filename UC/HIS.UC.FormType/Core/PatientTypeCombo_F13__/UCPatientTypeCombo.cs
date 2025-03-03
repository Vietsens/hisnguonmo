﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.UC.FormType.Loader;
using His.UC.LibraryMessage;
//using System.Windows.Forms;

namespace HIS.UC.FormType.PatientTypeCombo
{
    public partial class UCPatientTypeCombo : DevExpress.XtraEditors.XtraUserControl
    {
        PatientTypeComboFDO generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCPatientTypeCombo(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 25;

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
                PatientTypeLoader.LoadDataToCombo(cboPatientType);
                if (this.isValidData)
                {
                    Validation();
                    lciTitleName.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                SetTitle();//Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //Đặt tên cho combobox là đối tượng:
        void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    lciTitleName.Text = this.config.DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //Lấy giá trị cho cho đối tượng bệnh nhân.
        public string GetValue()
        {
            string value = "";
            try
            {
                long? departmentId = (long?)cboPatientType.EditValue;
                value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(departmentId));
            }
            catch (Exception ex)
            {
                value = "";
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
                        cboPatientType.Properties.DataSource = Config.HisFormTypeConfig.HisPatientTypes;
                        txtPatientTypeCode.Text = (Config.HisFormTypeConfig.HisPatientTypes.FirstOrDefault(f => f.ID == Inventec.Common.TypeConvert.Parse.ToInt64(value)) ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE()).PATIENT_TYPE_CODE;
                        cboPatientType.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Chung: lấy mã đối tượng bệnh nhân:
        /*public long GetID()
        {
            long ID = 0;
            try
            {
                long departmentId = (long)cboPatientType.EditValue;
                ID = String.Format(this.config.JSON_OUTPUT, departmentId);
            }
            catch (Exception ex)
            {
                value = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }*/
        //KiỂm tra tính bắt buộc
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

        private void txtPatientTypeCode_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                var search = txtPatientTypeCode.Text.Trim().ToLower();
                if (!String.IsNullOrEmpty(search))
                {
                    var mediStocks = Config.HisFormTypeConfig.HisPatientTypes.Where(f => f.PATIENT_TYPE_CODE.ToLower().Contains(search)).ToList();
                    if (mediStocks != null)
                    {
                        if (mediStocks.Count == 1)
                        {
                            txtPatientTypeCode.Text = mediStocks[0].PATIENT_TYPE_CODE;
                            cboPatientType.EditValue = mediStocks[0].ID;
                            System.Windows.Forms.SendKeys.Send("{TAB}");
                        }
                        else
                        {
                            cboPatientType.ShowPopup();
                            cboPatientType.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboPatientType.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.HisPatientTypes.FirstOrDefault(f => f.ID == long.Parse(cboPatientType.EditValue.ToString()));
                        if (department != null)
                        {
                            txtPatientTypeCode.Text = department.PATIENT_TYPE_CODE;
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

        private void cboPatientType_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (cboPatientType.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.HisPatientTypes.FirstOrDefault(f => f.ID == long.Parse(cboPatientType.EditValue.ToString()));
                        if (department != null)
                        {
                            txtPatientTypeCode.Text = department.PATIENT_TYPE_CODE;
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

        #region Validation
        private void ValidatePatientType()
        {
            try
            {
                HIS.UC.FormType.PatientTypeCombo.Validation.PatientTypeValidationRule validRule = new HIS.UC.FormType.PatientTypeCombo.Validation.PatientTypeValidationRule();
                validRule.txtPatientTypeCode = txtPatientTypeCode;
                validRule.cboPatientType = cboPatientType;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtPatientTypeCode, validRule);
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
                ValidatePatientType();
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

        private void UCPatientTypeCombo_Load(object sender, EventArgs e)
        {
            try
            {
                lciTitleName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_PATIENT_TYPE_COMBO_LCI_TITLE_NAME", Resources.ResourceLanguageManager.LanguageUCPatientTypeCombo, Base.LanguageManager.GetCulture());
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

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
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.TreatmentTypeCombo
{
    public partial class UCTreatmentTypeCombo : DevExpress.XtraEditors.XtraUserControl
    {
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCTreatmentTypeCombo(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
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
                HIS.UC.FormType.Loader.TreatmentTypeLoader.LoadDataToCombo(cboTreatmentType);
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

        private void txtTreatmentTypeCode_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    var search = txtTreatmentTypeCode.Text.Trim().ToLower();
                    if (!String.IsNullOrEmpty(search))
                    {
                        var mediStocks = Config.HisFormTypeConfig.HisTreatmentTypes.Where(f => f.TREATMENT_TYPE_CODE.ToLower().Contains(search)).ToList();
                        if (mediStocks != null)
                        {
                            if (mediStocks.Count == 1)
                            {
                                txtTreatmentTypeCode.Text = mediStocks[0].TREATMENT_TYPE_CODE;
                                cboTreatmentType.EditValue = mediStocks[0].ID;
                                System.Windows.Forms.SendKeys.Send("{TAB}");
                            }
                            else
                            {
                                cboTreatmentType.ShowPopup();
                                cboTreatmentType.Focus();
                            }
                        }
                    }
                    else
                    {
                        cboTreatmentType.ShowPopup();
                        cboTreatmentType.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboTreatmentType.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.HisTreatmentTypes.FirstOrDefault(f => f.ID == long.Parse(cboTreatmentType.EditValue.ToString()));
                        if (department != null)
                        {
                            txtTreatmentTypeCode.Text = department.TREATMENT_TYPE_CODE;
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

        private void cboTreatmentType_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (cboTreatmentType.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.HisTreatmentTypes.FirstOrDefault(f => f.ID == long.Parse(cboTreatmentType.EditValue.ToString()));
                        if (department != null)
                        {
                            txtTreatmentTypeCode.Text = department.TREATMENT_TYPE_CODE;
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
                long? departmentId = (long?)cboTreatmentType.EditValue;
                value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(departmentId));
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
                        cboTreatmentType.Properties.DataSource = Config.HisFormTypeConfig.HisTreatmentTypes;
                        txtTreatmentTypeCode.Text = (Config.HisFormTypeConfig.HisTreatmentTypes.FirstOrDefault(f => f.ID == Inventec.Common.TypeConvert.Parse.ToInt64(value)) ?? new MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE()).TREATMENT_TYPE_CODE;
                        cboTreatmentType.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
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
        private void ValidateTreatmentType()
        {
            try
            {
                HIS.UC.FormType.TreatmentTypeCombo.Validation.TreatmentTypeValidationRule validRule = new HIS.UC.FormType.TreatmentTypeCombo.Validation.TreatmentTypeValidationRule();
                validRule.txtTreatmentTypeCode = txtTreatmentTypeCode;
                validRule.cboTreatmentType = cboTreatmentType;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTreatmentTypeCode, validRule);
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
                ValidateTreatmentType();
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

        private void UCTreatmentTypeCombo_Load(object sender, EventArgs e)
        {
            try
            {
                lciTitleName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_TREATMENT_TYPE_COMBO_LCI_TITLE_NAME", Resources.ResourceLanguageManager.LanguageUCTreatmentTypeCombo, Base.LanguageManager.GetCulture());
                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

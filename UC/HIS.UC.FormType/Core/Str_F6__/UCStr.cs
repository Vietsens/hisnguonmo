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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;
using DevExpress.XtraEditors;
using His.UC.LibraryMessage;
using Inventec.Common.Logging;

namespace HIS.UC.FormType.Str
{
    public partial class UCStr : DevExpress.XtraEditors.XtraUserControl
    {
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        const string StrOutput0 = "_OUTPUT0:";
        string Output0 = "";
        string JsonOutput = "";

        public UCStr(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
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
                SetTitle();
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));

                if (this.isValidData)
                {
                    
                    
                    Validation();
                    layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RemoveStrOutput0(ref string JsonOutput)
        {
            try
            {
                //string JSON_OUTPUT = "sdfsdf_OUTPUT0:2x";
                int lastIndex = JsonOutput.LastIndexOf(StrOutput0);
                if (lastIndex >= 0)
                {
                    JsonOutput = JsonOutput.Substring(0, lastIndex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.Str.Text = Output0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        void GetValueOutput0(string JSON_OUTPUT, ref string Output0)
        {
            try
            {
                //string JSON_OUTPUT = "sdfsdf_OUTPUT0:2x";
                int lastIndex = JSON_OUTPUT.LastIndexOf(StrOutput0);
                if (lastIndex >= 0)
                {
                    Output0 = JSON_OUTPUT.Substring(lastIndex + StrOutput0.Length);
                }
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
                    layoutControlItem1.Text = this.config.DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string GetValue()
        {
            string value = "";
            try
            {
                string st = "";
                st = Str.Text;
                value = String.Format(this.JsonOutput, "\"" + ConvertUtils.ConvertToObjectFilter(st) + "\"");
           
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
                if (this.JsonOutput != null && this.report.JSON_FILTER != null)
                {
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, this.JsonOutput, this.report.JSON_FILTER);
                    if (value != null && value != "null")
                    {
                        Str.Text = value.Replace("\"", "");
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
        private void ValidateStr()
        {
            try
            {
                HIS.UC.FormType.Str.Validation.StrValidationRule validRule = new HIS.UC.FormType.Str.Validation.StrValidationRule();
                //validRule.txtTreatmentTypeCode = txtTreatmentTypeCode;
                validRule.Str = Str;
               validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(Str, validRule);
              
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
                ValidateStr();
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

        private void UCStr_Load(object sender, EventArgs e)
        {
            try
            {
                layoutControlItem1.Text = this.config.DESCRIPTION; //Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDICIN_LAYOUT_CONTROL_ITEM1", Resources.ResourceLanguageManager.LanguageUCStr, Base.LanguageManager.GetCulture());

                GetValueOutput0(this.config.JSON_OUTPUT, ref Output0);
                SetDefaultValue();
                JsonOutput = this.config.JSON_OUTPUT;
                RemoveStrOutput0(ref JsonOutput);
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

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
//using System.Windows.Forms;
using HIS.UC.FormType.Core.Numericft;
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.Numericft
{
    public partial class UCNumeric : DevExpress.XtraEditors.XtraUserControl
    {
    
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        // public static bool exitclick = false;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
       
        public UCNumeric(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
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
                SetTitle();//Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                if (this.report != null)
                {
                    SetValue();
                }
                if (this.isValidData)
                {
                    Validation();
                    layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutControlItem2.AppearanceItemCaption.ForeColor = Color.Maroon;
                    numericUpDown1.EditValue = 0;
                    numericUpDown2.EditValue = 0;
                }
                else
                {
                    numericUpDown1.EditValue = null;
                    numericUpDown2.EditValue = null;
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
                long? fromnum=null;
                if (numericUpDown1.EditValue != null) fromnum = Inventec.Common.TypeConvert.Parse.ToInt64(numericUpDown1.Value.ToString());
                long? tonum=null;
                if (numericUpDown2.EditValue != null) tonum = Inventec.Common.TypeConvert.Parse.ToInt64(numericUpDown2.Value.ToString());
                value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(fromnum), ConvertUtils.ConvertToObjectFilter(tonum));
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
                    var jsOutputSub = this.config.JSON_OUTPUT.Split(new string[] { "," }, StringSplitOptions.None);

                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,jsOutputSub[0], this.report.JSON_FILTER);
                    if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                        numericUpDown1.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);

                    if (jsOutputSub.Count() > 1)
                    {
                        value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,jsOutputSub[1], this.report.JSON_FILTER);
                        if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                            numericUpDown2.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
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
        private void ValidateNumeric()
        {
            try
            {
                HIS.UC.FormType.Numeric.NumericValidationRule validRule = new HIS.UC.FormType.Numeric.NumericValidationRule();
                //validRule.txtTreatmentTypeCode = txtTreatmentTypeCode;
                validRule.numericUpDown1 = numericUpDown1;
               validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(numericUpDown1, validRule);
              
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
                ValidateNumeric();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {

        }
        #endregion

        private void UCMedicin_Load(object sender, EventArgs e)
        {
            try
            {
                layoutControlItem1.Text = this.config.DESCRIPTION; //Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDICIN_LAYOUT_CONTROL_ITEM1", Resources.ResourceLanguageManager.LanguageUCMedicin, Base.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

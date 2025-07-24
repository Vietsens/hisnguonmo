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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraSpreadsheet.Utils;
using His.UC.LibraryMessage;
using HIS.UC.FormType.Core.RadioCheckBox.Validation;
using HIS.UC.FormType.Core.TrueOrFalse.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.FormType.Core.RadioCheckBox
{
    public partial class UCRadioCheckBox : System.Windows.Forms.UserControl
    {
        RadioCheckBoxFDO generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        const string StrOutput0 = "_OUTPUT0:";
        string Output0 = "";
        string JsonOutput = "";
        DynamicFilterRDO DynamicFilter;


        public UCRadioCheckBox(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //
                //FormTypeConfig.ReportHight += 30;
                this.config = config;

                if (paramRDO is GenerateRDO generateRDO)
                {
                    this.report = generateRDO.Report;
                    this.DynamicFilter = generateRDO.DynamicFilter;
                    if (this.DynamicFilter != null)
                    {
                        this.config = this.DynamicFilter.Fofi;
                    }
                }
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private Dictionary<int, CheckEdit> radioDict = new Dictionary<int, CheckEdit>();
        CheckEdit addRadioButton(string text, int index)
        {
            CheckEdit radio = new CheckEdit();
            try
            {
                if (this.DynamicFilter != null && this.DynamicFilter.Propeties != null && this.DynamicFilter.Propeties.Type == 1)
                {
                    radio.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
                    radio.CheckedChanged += Radio_CheckedChanged;
                    //radio.Properties.RadioGroupIndex = (int)this.DynamicFilter.ID;

                }
                else
                {
                    radio.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
                }
                radio.Dock = System.Windows.Forms.DockStyle.Top;
                radio.Location = new System.Drawing.Point(96, 2);
                radio.Name = "radio" + index;
                radio.Properties.AutoWidth = true;
                radio.Properties.Caption = text;
                radio.Size = new System.Drawing.Size(52, 19);
                radio.TabIndex = index;
                radio.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return radio;
        }
        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit radio = sender as CheckEdit;
            if (radio != null && radio.Properties.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Radio)
            {
                if (radio.Checked)
                {
                    foreach (var item in radioDict)
                    {
                        if (item.Value == radio)
                        {
                            continue;
                        }
                        else
                        {
                            item.Value.Checked = false;
                        }
                    }
                }
            }
        }
        void Init()
        {
            try
            {
                //
                if (this.DynamicFilter != null && this.DynamicFilter.Propeties != null && this.DynamicFilter.Propeties.DefaultSource != null)
                {
                    radioDict = new Dictionary<int, CheckEdit>();
                    var items = this.DynamicFilter.Propeties.DefaultSource.ToString().Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < items.Length; i++)
                    {
                        CheckEdit radio = addRadioButton(items[i], i);
                        radioDict[i] = radio;
                        int col = i % 3;
                        if (col == 0)
                        {
                            panel1.Controls.Add(radio);
                            panel1.Controls.SetChildIndex(radio, 0);
                        }
                        else if (col == 1)
                        {
                            panel2.Controls.Add(radio);
                            panel2.Controls.SetChildIndex(radio, 0);
                        }
                        else // col == 2
                        {
                            panel3.Controls.Add(radio);
                            panel3.Controls.SetChildIndex(radio, 0);
                        }
                    }
                    int chieuCao = 0;
                    foreach (Control item in panel1.Controls)
                    {
                        if (item is CheckEdit check)
                        {
                            chieuCao += check.Height;
                        }
                    }
                    this.ClientSize = new Size(ClientSize.Width, chieuCao + 10);
                }
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    SetValidation();
                }
                SetTitle();
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                GetValueOutput0(this.config.JSON_OUTPUT, ref Output0);
                //radio.EditValue = true;
                if (!string.IsNullOrWhiteSpace(Output0))
                {
                    foreach (var item in radioDict)
                    {
                        if (Output0.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Contains(item.Key.ToString()))
                        {
                            item.Value.Checked = true;
                        }
                    }
                }
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
                if (string.IsNullOrWhiteSpace(Output0))
                {
                    if (this.DynamicFilter != null && this.DynamicFilter.Propeties != null && this.DynamicFilter.Propeties.DefaultValue != null)
                    {
                        Output0 = this.DynamicFilter.Propeties.DefaultValue.ToString();
                    }
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    layoutControlItem4.Text = this.config.DESCRIPTION;
                    //radTrue.Text = this.config.DESCRIPTION.Split(',').ToList().First();
                    //radFalse.Text = this.config.DESCRIPTION.Split(',').ToList().Last();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public string GetValue()
        {
            try
            {
                var selectedIds = radioDict.Where(x => x.Value.Checked)
                                           .Select(x => x.Key);
                var TrueOrFalseId = Newtonsoft.Json.JsonConvert.SerializeObject(selectedIds);
                return String.Format(this.JsonOutput, ConvertUtils.ConvertToObjectFilter(TrueOrFalseId));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        public void SetValue()
        {
            try
            {
                if (this.JsonOutput != null && this.report.JSON_FILTER != null)
                {
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, this.JsonOutput, this.report.JSON_FILTER);
                    //if (value == "true") radTrue.Checked = true;
                    //else if (value == "false") radFalse.Checked = true;
                    foreach (var item in radioDict)
                    {
                        if (Output0.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Contains(item.Key.ToString()))
                        {
                            item.Value.Checked = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Validation
        private void SetValidateRad()
        {
            try
            {
                RadioCheckBoxValidationRule validRule = new RadioCheckBoxValidationRule();
                //validRule.radTrue = radTrue;
                //validRule.radFalse = radFalse;
                //validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                //validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //dxValidationProvider1.SetValidationRule(radTrue, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValidation()
        {
            try
            {
                SetValidateRad();
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

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
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

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
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
    }
}

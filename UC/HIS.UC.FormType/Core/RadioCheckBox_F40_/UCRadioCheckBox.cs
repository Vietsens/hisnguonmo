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
using HIS.UC.FormType.Core.RadioCheckBox.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
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
        void Init()
        {
            try
            {
                if (this.DynamicFilter != null && this.DynamicFilter.Propeties != null && this.DynamicFilter.Propeties.DefaultSource != null)
                {
                    if (this.DynamicFilter.Propeties.Type == 1)
                    {
                        this.checkedListBoxControl1.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
                        this.checkedListBoxControl1.ItemCheck += checkedListBoxControl1_ItemCheck;
                    }
                    else
                    {
                        this.checkedListBoxControl1.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
                    }
                    var items = this.DynamicFilter.Propeties.DefaultSource.ToString().Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < items.Length; i++)
                    {
                        this.checkedListBoxControl1.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[]
                        {
                            new DevExpress.XtraEditors.Controls.CheckedListBoxItem(i, items[i]) });
                    }
                }
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    SetValidation();
                }
                SetTitle();
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                GetValueOutput0(this.config.JSON_OUTPUT, ref Output0);
                if (!string.IsNullOrWhiteSpace(Output0))
                {
                    for (int i = 0; i < this.checkedListBoxControl1.Items.Count; i++)
                    {
                        if (Output0.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Contains(i.ToString()))
                        {
                            this.checkedListBoxControl1.Items[i].CheckState = CheckState.Checked;
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
        private void checkedListBoxControl1_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            if (checkedListBoxControl1.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Radio && e.State == CheckState.Checked)
            {
                for (int i = 0; i < checkedListBoxControl1.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkedListBoxControl1.Items[i].CheckState = CheckState.Unchecked;
                    }
                }
            }
        }

        void GetValueOutput0(string JSON_OUTPUT, ref string Output0)
        {
            try
            {
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
                    layoutControlItem1.Text = this.config.DESCRIPTION;
                    if (this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;
                    }
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
                var selectedIds = this.checkedListBoxControl1.Items.Where(x => x.CheckState == CheckState.Checked)
                                           .Select(x => x.Value);
                var val = Newtonsoft.Json.JsonConvert.SerializeObject(selectedIds);
                return String.Format(this.JsonOutput, val);
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
                    List<int> value = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, this.JsonOutput, this.report.JSON_FILTER));
                    if (value != null && value.Count > 0)
                    {
                        for (int i = 0; i < this.checkedListBoxControl1.Items.Count; i++)
                        {
                            this.checkedListBoxControl1.Items[i].CheckState = value.Contains(i) ? CheckState.Checked : CheckState.Unchecked;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetValidateRad()
        {
            try
            {
                RadioCheckBoxValidationRule validRule = new RadioCheckBoxValidationRule();
                validRule.radTrue = this.checkedListBoxControl1;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.checkedListBoxControl1, validRule);
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
        private void BestFitHeight(CheckedListBoxControl listBox)
        {
            CheckedListBoxViewInfo viewInfo = listBox.GetViewInfo() as CheckedListBoxViewInfo;
            // Tính toán số cột và hàng mới khi thu hẹp cửa sổ
            var tongChieuDai = viewInfo.ColumnWidth * viewInfo.ColumnCount;
            while (tongChieuDai > listBox.Width)
            {
                this.ClientSize = new Size(ClientSize.Width, ClientSize.Height + viewInfo.ItemHeight);
                listBox.PerformLayout();
                viewInfo = listBox.GetViewInfo() as CheckedListBoxViewInfo;
                tongChieuDai = viewInfo.ColumnWidth * viewInfo.ColumnCount;
            }
            // Tính toán số cột và hàng mới khi kéo rộng cửa sổ (Hình như không cần dùng ý trên vẫn được)
            viewInfo = listBox.GetViewInfo() as CheckedListBoxViewInfo;
            int columnCount_new = (int)Math.Floor((double)listBox.Width / viewInfo.ColumnWidth);
            int columnCount_current = viewInfo.ColumnCount;
            int rowCount_new = (int)Math.Ceiling((double)listBox.Items.Count / columnCount_new);
            int rowCount_current = (int)Math.Ceiling((double)listBox.Items.Count / viewInfo.ColumnCount);
            if (columnCount_new > columnCount_current && rowCount_new < rowCount_current || (rowCount_new == rowCount_current && rowCount_new == 1))
            {
                int newHeight = rowCount_new * viewInfo.ItemHeight;
                this.ClientSize = new Size(ClientSize.Width, newHeight + 10);
            }
        }

        private void UCRadioCheckBox_Resize(object sender, EventArgs e)
        {
            this.BestFitHeight(this.checkedListBoxControl1);
        }

        private void UCRadioCheckBox_Load(object sender, EventArgs e)
        {
            this.BestFitHeight(this.checkedListBoxControl1);
        }
    }
}

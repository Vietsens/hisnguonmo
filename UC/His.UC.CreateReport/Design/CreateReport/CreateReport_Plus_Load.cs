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
using DevExpress.XtraEditors.ViewInfo;
using His.UC.CreateReport.Design.CreateReport.Validation;
using HIS.UC.CreateReport.Loader;
using Inventec.Common.Logging;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.CreateReport.Design.CreateReport
{
    internal partial class CreateReport
    {
        List<V_SAR_RETY_FOFI> currentFormFields;
        private void CreateReport_Load()
        {
            try
            {
                language();
                ReportTemplateLoader.LoadDataToCombo(cboReportTemplate, null);
                LoadDataToForm();
                CreateReportControlByReportType();
                Validation();
                txtReportTemplateCode.Focus();
                txtReportTemplateCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckTimeCreate()
        {
            try
            {
                bool enable = true;
                string mess = "Báo cáo bị giới hạn thời gian. ";

                long hourNow = long.Parse(DateTime.Now.ToString("HHmm"));
                if (!String.IsNullOrWhiteSpace(this.reportType.HOUR_FROM))
                {
                    if (!String.IsNullOrWhiteSpace(this.reportType.HOUR_TO))
                    {
                        if (long.Parse(this.reportType.HOUR_FROM) < hourNow && long.Parse(this.reportType.HOUR_TO) > hourNow)
                        {
                            mess += string.Format("Vui lòng tạo báo cáo ngoài khoảng thời gian từ {0} đến {1}", ProcessHour(reportType.HOUR_FROM), ProcessHour(reportType.HOUR_TO));
                            enable = false;
                        }
                    }
                    else
                    {
                        if (long.Parse(this.reportType.HOUR_FROM) < hourNow)
                        {
                            mess += string.Format("Vui lòng tạo báo cáo trước {0}", ProcessHour(reportType.HOUR_FROM));
                            enable = false;
                        }
                    }
                }
                else if (!String.IsNullOrWhiteSpace(this.reportType.HOUR_TO) && long.Parse(this.reportType.HOUR_TO) > hourNow)
                {
                    mess += string.Format("Vui lòng tạo báo cáo sau {0}", ProcessHour(reportType.HOUR_TO));
                    enable = false;
                }

                if (!enable)
                {
                    XtraMessageBox.Show(mess);
                    btnSave.Enabled = enable;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string ProcessHour(string p)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(p))
                {
                    result = string.Format("{0}:{1}", p.Substring(0, 2), p.Substring(2, 2));
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void language()
        {
            try
            {
                //layoutReportTypeCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_CREATE_REPORT_LAYOUT_REPORT_TYPE_CODE", Resources.ResourceLanguageManager.LanguageCreateReport, Base.LanguageManager.GetCulture());
                //layoutReportTypeName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_CREATE_REPORT_LAYOUT_REPORT_TYPE_NAME", Resources.ResourceLanguageManager.LanguageCreateReport, Base.LanguageManager.GetCulture());
                //layoutReportTemplateCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_CREATE_REPORT_LAYOUT_REPORT_TEMPLATE_CODE", Resources.ResourceLanguageManager.LanguageCreateReport, Base.LanguageManager.GetCulture());
                //layoutReportName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_CREATE_REPORT_LAYOUT_REPORT_NAME", Resources.ResourceLanguageManager.LanguageCreateReport, Base.LanguageManager.GetCulture());
                //layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_CREATE_REPORT_LAYOUT_DESCRIPTION", Resources.ResourceLanguageManager.LanguageCreateReport, Base.LanguageManager.GetCulture());
                //btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_CREATE_REPORT_BTN_SAVE", Resources.ResourceLanguageManager.LanguageCreateReport, Base.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToForm()
        {
            try
            {
                lblReportTypeCode.Text = this.reportType.REPORT_TYPE_CODE;
                lblReportTypeName.Text = this.reportType.REPORT_TYPE_NAME;

                var listReportTemplate = CreateReportConfig.ReportTemplates.Where(o => o.REPORT_TYPE_ID == reportType.ID).OrderBy(p => p.REPORT_TEMPLATE_CODE).ToList();
                if (listReportTemplate != null && listReportTemplate.Count > 0)
                {
                    cboReportTemplate.Properties.DataSource = listReportTemplate;
                    if (this.generateRDO != null && this.generateRDO.Report != null)
                    {
                        this.txtReportTemplateCode.Text = this.generateRDO.Report.REPORT_TEMPLATE_CODE;
                        this.cboReportTemplate.EditValue = this.generateRDO.Report.REPORT_TEMPLATE_ID;
                        this.txtReportName.Text = this.generateRDO.Report.REPORT_NAME;
                        this.txtDescription.Text = this.generateRDO.Report.DESCRIPTION;
                    }
                    else
                    {
                        txtReportTemplateCode.Text = listReportTemplate.First().REPORT_TEMPLATE_CODE;
                        cboReportTemplate.EditValue = listReportTemplate.First().ID;
                        this.txtReportName.Text = listReportTemplate.First().REPORT_TEMPLATE_NAME;
                    }
                    txtReportName.Focus();
                    txtReportName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateReportControlByReportType()
        {
            try
            {
                this.currentFormFields = CreateReportConfig.RetyFofis.Where(o => o.REPORT_TYPE_ID == reportType.ID).OrderBy(o => o.NUM_ORDER).ToList();
                //nếu là tự khai báo sẽ tạo ra retyfofi để gen control.
                if (reportType.REPORT_TYPE_CODE.ToUpper().StartsWith("TKB") && reportType.SQL != null && this.currentFormFields.Count == 0)
                {
                    var querry = System.Text.Encoding.UTF8.GetString(reportType.SQL);
                    querry = querry.Replace(":", " :").Replace("&", " :");
                    var lstFilter = statementsSeperate(querry);
                    if (lstFilter != null)
                    {
                        List<string> lstOutPut = new List<string>();
                        int ic = -1;
                        while (true)
                        {
                            //lấy giá trị đầu tiên chứa dấu : (từ sau vị trí hiện tại)
                            string value = lstFilter.Skip(ic + 1).FirstOrDefault(o => o.StartsWith(":"));
                            //lấy  vị trị có giá trị bằng giá trị đó (từ sau vị trí hiện tại)
                            ic = Array.IndexOf<string>(lstFilter, value, ic + 1);
                            //nếu không có vị trí đó (ic=-1) thì thoát khỏi vòng lặp
                            if (ic < 0)
                            {
                                break;
                            }
                            //bỏ dấu : ra khỏi giá trị
                            value = value.Replace(":", "");
                            //nếu sau khi bỏ dấu : được giá trị rỗng thì lấy giá trị tiếp theo (áp dụng đối với trường hợp dấu : đặt xa so với giá trị)
                            if (string.IsNullOrWhiteSpace(value) && ic < lstFilter.Length - 1)
                            {
                                value = lstFilter[ic + 1].Replace(":", "");
                            }
                            //cuổi cùng kiểm tra nếu giá trị không rỗng thì Add vào danh sách
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                lstOutPut.Add(value);
                            }
                        }
                        
                        lstOutPut = lstOutPut.Distinct().ToList();

                        V_SAR_RETY_FOFI fofi = new V_SAR_RETY_FOFI();
                        fofi.REPORT_TYPE_ID = reportType.ID;
                        fofi.REPORT_TYPE_CODE = reportType.REPORT_TYPE_CODE;
                        fofi.REPORT_TYPE_NAME = reportType.REPORT_TYPE_NAME;
                        fofi.NUM_ORDER = 10;
                        fofi.IS_REQUIRE = 1;
                        fofi.FORM_FIELD_CODE = "FTHIS000034";
                        fofi.JSON_OUTPUT = string.Join(";", lstOutPut);

                        if (currentFormFields == null) currentFormFields = new List<V_SAR_RETY_FOFI>();

                        currentFormFields.Add(fofi);
                    }
                }
                else
                {

                }

                if (this.currentFormFields != null && this.currentFormFields.Count > 0)
                {
                    foreach (var item in this.currentFormFields)
                    {
                        System.Windows.Forms.UserControl control = GenerateControl(item);
                        control.Name = (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                        //control.Tag = item;
                        control.Dock = System.Windows.Forms.DockStyle.Top;
                        control.AutoSize = false;
                        xtraScrollTabContainerReportControl.Controls.Add(control);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string[] statementsSeperate(string sql)
        {
            string[] result = null;
            try
            {
                var fixedInput = System.Text.RegularExpressions.Regex.Replace(sql, "[^a-zA-Z0-9%: ._]", " ");
                result = fixedInput.Split(' ');
                result = result.Where(o => !String.IsNullOrWhiteSpace(o)).ToArray();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        System.Windows.Forms.UserControl GenerateControl(V_SAR_RETY_FOFI data)
        {
            System.Windows.Forms.UserControl result = null;
            try
            {
                result = HIS.UC.FormType.FormTypeMain.Run(data, this.generateRDO) as System.Windows.Forms.UserControl;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ValidateReportTemplate()
        {
            try
            {
                ReportTemplateValidationRule reportTemplateRule = new ReportTemplateValidationRule();
                reportTemplateRule.cboReportTemplate = cboReportTemplate;
                reportTemplateRule.txtReportTemplateCode = txtReportTemplateCode;
                reportTemplateRule.ErrorText = Base.MessageUtil.GetMessage(MessageLang.Message.Enum.TruongDuLieuBatBuoc);
                reportTemplateRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboReportTemplate, reportTemplateRule);
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
                ValidateReportTemplate();
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
    }
}

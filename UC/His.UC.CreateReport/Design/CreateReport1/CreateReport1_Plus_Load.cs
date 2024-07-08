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
using DevExpress.XtraLayout;
using His.UC.CreateReport.Data;
using His.UC.CreateReport.Design.CreateReport.Validation;
using HIS.UC.CreateReport.Loader;
using Inventec.Common.Logging;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.CreateReport.Design.CreateReport1
{
    internal partial class CreateReport1
    {
        List<V_SAR_RETY_FOFI> currentFormFields;
        private void CreateReport_Load()
        {
            try
            {
                language();

                //ReportTemplateLoader.LoadDataToCombo(cboReportTemplate, null);
                LoadReportTemplate();
                LoadDataToForm();
                CreateReportControlByReportType();
                Validation();
                grdReportTemplate.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadReportTemplate()
        {
            try
            {
                var listReportTemplate = CreateReportConfig.ReportTemplates.Where(o => o.REPORT_TYPE_ID == reportType.ID).OrderBy(p => p.REPORT_TEMPLATE_CODE).ToList();
                this.listReportTemplateADO = (from m in listReportTemplate
                                              select new ReportTemplateADO()
                                              {
                                                  ID = m.ID,
                                                  REPORT_TYPE_ID = m.REPORT_TYPE_ID,
                                                  IS_ACTIVE = m.IS_ACTIVE,
                                                  IS_DELETE = m.IS_DELETE,
                                                  EXTENSION_RECEIVE = m.EXTENSION_RECEIVE,
                                                  REPORT_TEMPLATE_CODE = m.REPORT_TEMPLATE_CODE,
                                                  REPORT_TEMPLATE_NAME = m.REPORT_TEMPLATE_NAME,
                                                  REPORT_TEMPLATE_URL = m.REPORT_TEMPLATE_URL,
                                                  TUTORIAL = m.TUTORIAL,
                                              }).ToList();
                if (listReportTemplateADO != null && listReportTemplateADO.Count > 0)
                {
                    listReportTemplateADO[0].IsChecked = true;
                }
                grdReportTemplate.BeginUpdate();
                grdReportTemplate.DataSource = listReportTemplateADO;
                grdReportTemplate.EndUpdate();
            }
            catch (Exception ex)
            {
                grdReportTemplate.EndUpdate();
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
                    if (this.generateRDO != null && this.generateRDO.Report != null)
                    {
                        foreach (var rt in listReportTemplateADO)
                        {
                            if (this.generateRDO.Report.REPORT_TEMPLATE_CODE == rt.REPORT_TEMPLATE_CODE)
                            {
                                rt.IsChecked = true;
                            }
                            else
                                rt.IsChecked = false;
                        }

                        this.txtReportName.Text = this.generateRDO.Report.REPORT_NAME;
                        this.txtDescription.Text = this.generateRDO.Report.DESCRIPTION;
                    }
                    else
                    {
                        listReportTemplateADO.ForEach(o => o.IsChecked = false);
                        listReportTemplateADO[0].IsChecked = true;
                        this.txtReportName.Text = listReportTemplateADO[0].REPORT_TEMPLATE_NAME;
                    }

                    grdReportTemplate.BeginUpdate();
                    grdReportTemplate.DataSource = listReportTemplateADO;
                    grdReportTemplate.EndUpdate();

                    //txtReportName.Focus();
                    //txtReportName.SelectAll();
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
                //this.currentFormFields = CreateReportConfig.RetyFofis.Where(o => o.REPORT_TYPE_ID == reportType.ID).OrderBy(o => o.NUM_ORDER).ToList();
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
                        fofi.WIDTH_RATIO = 2;
                        fofi.HEIGHT = 30;

                        if (currentFormFields == null) currentFormFields = new List<V_SAR_RETY_FOFI>();

                        currentFormFields.Add(fofi);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lcGenerateReportField.Width___runtime size:", ("[" + lcGenerateReportField.Width
                    + ", " + lcGenerateReportField.Height + "],ClientSize=" + lcGenerateReportField.ClientSize
                    + ", MaximumSize=" + lcGenerateReportField.MaximumSize
                    + ",Screen.PrimaryScreen.Bounds.Size:" + System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size))
                    + ", design size: [1356, 653]"
                    );
                lcGenerateReportField.BeginUpdate();
                lcGenerateReportField.Controls.Clear();
                layoutControlGroupGenerateReportField.Clear();

                if (this.currentFormFields != null && this.currentFormFields.Count > 0)
                {
                    //xtraTabControlFilter.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;

                    var dataRH = currentFormFields.Where(o => o.ROW_COUNT > 0 && o.COLUMN_COUNT > 0).FirstOrDefault();
                    long rowCount = dataRH != null ? dataRH.ROW_COUNT.Value : 0;
                    long columnCount = dataRH != null ? dataRH.COLUMN_COUNT.Value : 0;
                    if (rowCount > 0 && columnCount > 0)
                    {
                        //lcGenerateReportField.BeginUpdate();

                        int w__Row = (int)(lcGenerateReportField.Width / columnCount);

                        for (int i = 1; i <= rowCount; i++)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowCount), rowCount)
                                + Inventec.Common.Logging.LogUtil.TraceData("rowindex:i=", i));
                            List<BaseLayoutItem> layoutControlItemAdd_Rows = new List<BaseLayoutItem>();
                            for (int j = 0; j < columnCount; j++)
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => columnCount), columnCount)
                                                                + Inventec.Common.Logging.LogUtil.TraceData("columnindex:j=", j));
                                LayoutControl lcGenerateReportField__Row = new LayoutControl();
                                lcGenerateReportField__Row.Root.GroupBordersVisible = false;
                                lcGenerateReportField__Row.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);

                                LayoutControlGroup layoutControlGroupGenerateReportField__Row = lcGenerateReportField__Row.Root;
                                layoutControlGroupGenerateReportField__Row.GroupBordersVisible = false;
                                layoutControlGroupGenerateReportField__Row.ExpandButtonVisible = false;
                                layoutControlGroupGenerateReportField__Row.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);

                                int layout__row_height = 0;

                                long rowIndexFilter = ((j + 1) + (i - 1) * columnCount);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowIndexFilter), rowIndexFilter));
                                var formFieldInRows = currentFormFields.Where(o => o.ROW_INDEX == rowIndexFilter).OrderBy(k => k.NUM_ORDER).ToList();


                                if (formFieldInRows != null && formFieldInRows.Count > 0)
                                {
                                    int dem = 1;
                                    int totalwidthRatio = 0;

                                    List<BaseLayoutItem> layoutControlItemAdds = new List<BaseLayoutItem>();
                                    foreach (var item in formFieldInRows)
                                    {
                                        System.Windows.Forms.UserControl control = GenerateControl(item);

                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.FORM_FIELD_CODE), item.FORM_FIELD_CODE)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.REPORT_TYPE_CODE), item.REPORT_TYPE_CODE)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.WIDTH_RATIO), item.WIDTH_RATIO)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.HEIGHT), item.HEIGHT)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.NUM_ORDER), item.NUM_ORDER)
                                            + Inventec.Common.Logging.LogUtil.TraceData("control.Name", control.Name));

                                        control.Name = Guid.NewGuid().ToString();// (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss"));

                                        LayoutControlItem item1 = layoutControlGroupGenerateReportField__Row.AddItem();
                                        layoutControlItemAdds.Add(item1);
                                        // Bind a control to the layout item.

                                        int h = (int)(item.HEIGHT ?? 0);
                                        int wr = (int)(item.WIDTH_RATIO ?? 0);
                                        int w = wr > 0 ? (w__Row * wr / 3) : 0;
                                        int realH = (h > 0 ? h : (control.Height + 5));

                                        item1.Control = control;
                                        item1.TextVisible = false;
                                        item1.Name = Guid.NewGuid().ToString();// String.Format("lciForItemControlTemp{0}", (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss")));
                                        item1.SizeConstraintsType = SizeConstraintsType.Custom;

                                        item1.Height = realH;
                                        item1.MaxSize = new System.Drawing.Size(0, realH);
                                        item1.MinSize = new System.Drawing.Size(w, realH);
                                        if (w > 0 && realH > 0)
                                            item1.Size = new System.Drawing.Size(w, realH);

                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("item1.Size", item1.Size)
                                            + Inventec.Common.Logging.LogUtil.TraceData("control.Height", control.Height)
                                            + Inventec.Common.Logging.LogUtil.TraceData("realH", realH)
                                            + Inventec.Common.Logging.LogUtil.TraceData("h", h));

                                        if (wr > 0 && totalwidthRatio > 0 && totalwidthRatio + wr <= 3)
                                        {
                                            item1.Move(layoutControlItemAdds[dem - 2], DevExpress.XtraLayout.Utils.InsertType.Right);
                                            Inventec.Common.Logging.LogSystem.Debug("item1.Control.Name=" + item1.Control.Name + ",layoutControlItemAdds[dem - 2].Control.Name=" + ((LayoutControlItem)(layoutControlItemAdds[dem - 2])).Control.Name + ",totalwidthRatio=" + totalwidthRatio);

                                        }
                                        else
                                        {
                                            totalwidthRatio = 0;
                                            layout__row_height += realH;
                                        }
                                        dem++;
                                        totalwidthRatio += wr;


                                    }
                                    layoutControlItemAll.AddRange(layoutControlItemAdds);
                                }

                                lcGenerateReportField__Row.Height = layout__row_height + 5;

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("layoutControlGroupGenerateReportField__Row.Width", layoutControlGroupGenerateReportField__Row.Width)
                                  + Inventec.Common.Logging.LogUtil.TraceData("layoutControlGroupGenerateReportField__Row.Height", layoutControlGroupGenerateReportField__Row.Height)
                                  + Inventec.Common.Logging.LogUtil.TraceData("lcGenerateReportField__Row.Height", lcGenerateReportField__Row.Height));

                                LayoutControlItem item111 = layoutControlGroupGenerateReportField.AddItem();
                                layoutControlItemAdd_Rows.Add(item111);
                                // Bind a control to the layout item.
                                //int realH_Row = lcGenerateReportField__Row.Height;
                                item111.Control = lcGenerateReportField__Row;
                                item111.TextVisible = false;
                                item111.Name = Guid.NewGuid().ToString();// String.Format("lciForItemControlTemp{0}", (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss")));
                                item111.SizeConstraintsType = SizeConstraintsType.Custom;
                                int realH_Row = lcGenerateReportField__Row.Height;
                                item111.Height = realH_Row;
                                item111.MaxSize = new System.Drawing.Size(0, realH_Row);
                                item111.MinSize = new System.Drawing.Size(w__Row, realH_Row);
                                item111.Size = new System.Drawing.Size(w__Row, realH_Row);

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("item111.Size", item111.Size));
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => j), j));
                                if (j > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("j - 1=" + (j - 1) + ",layoutControlItemAdd_Rows.count =" + layoutControlItemAdd_Rows.Count);
                                    item111.Move(layoutControlItemAdd_Rows[j - 1], DevExpress.XtraLayout.Utils.InsertType.Right);
                                }

                                //Inventec.Common.Logging.LogSystem.Debug("item111.Control.Name=" + item111.Control.Name + ",layoutControlItemAdd_Rows[j - 1].Control.Name=" + ((LayoutControlItem)(layoutControlItemAdd_Rows[j - 1])).Control.Name);

                            }

                        }
                        //lcGenerateReportField.EndUpdate();
                    }
                    else
                    {
                        int dem = 1;
                        int totalwidthRatio = 0;
                        // lcGenerateReportField.BeginUpdate();
                        List<BaseLayoutItem> layoutControlItemAdds = new List<BaseLayoutItem>();
                        foreach (var item in this.currentFormFields)
                        {
                            System.Windows.Forms.UserControl control = GenerateControl(item);

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.FORM_FIELD_CODE), item.FORM_FIELD_CODE)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.REPORT_TYPE_CODE), item.REPORT_TYPE_CODE)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.WIDTH_RATIO), item.WIDTH_RATIO)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.HEIGHT), item.HEIGHT)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.NUM_ORDER), item.NUM_ORDER)
                                + Inventec.Common.Logging.LogUtil.TraceData("control.Name", control.Name));

                            control.Name = Guid.NewGuid().ToString();// (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss"));

                            LayoutControlItem item1 = layoutControlGroupGenerateReportField.AddItem();
                            layoutControlItemAdds.Add(item1);
                            // Bind a control to the layout item.



                            item1.Control = control;
                            item1.TextVisible = false;
                            item1.Name = Guid.NewGuid().ToString();// String.Format("lciForItemControlTemp{0}", (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss")));
                            item1.SizeConstraintsType = SizeConstraintsType.Custom;
                            int h = (int)(item.HEIGHT ?? 0);
                            int wr = (int)(item.WIDTH_RATIO ?? 0);
                            int w = wr > 0 ? (lcGenerateReportField.Width * wr / 3) : 0;
                            int realH = (h > 0 ? h : (control.Height + 5));
                            item1.Height = realH;
                            item1.MaxSize = new System.Drawing.Size(0, realH);
                            item1.MinSize = new System.Drawing.Size(w, realH);
                            if (w > 0 && realH > 0)
                                item1.Size = new System.Drawing.Size(w, realH);

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("item1.Size", item1.Size));

                            if (wr > 0 && totalwidthRatio > 0 && totalwidthRatio + wr <= 3)
                            {
                                item1.Move(layoutControlItemAdds[dem - 2], DevExpress.XtraLayout.Utils.InsertType.Right);
                                Inventec.Common.Logging.LogSystem.Debug("item1.Control.Name=" + item1.Control.Name + ",layoutControlItemAdds[dem - 2].Control.Name=" + ((LayoutControlItem)(layoutControlItemAdds[dem - 2])).Control.Name + ",totalwidthRatio=" + totalwidthRatio);
                            }
                            else
                            {
                                totalwidthRatio = 0;
                            }
                            dem++;
                            totalwidthRatio += wr;
                        }

                        layoutControlItemAll.AddRange(layoutControlItemAdds);
                        //lcGenerateReportField.EndUpdate();
                    }
                }
                lcGenerateReportField.EndUpdate();
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
                //ReportTemplateValidationRule reportTemplateRule = new ReportTemplateValidationRule();
                //reportTemplateRule.cboReportTemplate = cboReportTemplate;
                //reportTemplateRule.txtReportTemplateCode = txtReportTemplateCode;
                //reportTemplateRule.ErrorText = Base.MessageUtil.GetMessage(MessageLang.Message.Enum.TruongDuLieuBatBuoc);
                //reportTemplateRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //dxValidationProvider1.SetValidationRule(cboReportTemplate, reportTemplateRule);
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

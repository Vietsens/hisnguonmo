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
using DCV.APP.Report.JsonOutput;
using DevExpress.XtraLayout;
using HIS.UC.CreateReport.Base;
using Inventec.Core;
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
        string strFilter = "";
        string seperator = ",";

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Focus();
                if (!btnSave.Enabled) return;

                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                CommonParam param = new CommonParam();
                bool valid = true;

                Inventec.Desktop.Common.Message.WaitingManager.Show();
                MRS.SDO.CreateReportSDO data = new MRS.SDO.CreateReportSDO();
                data.Loginname = CreateReportConfig.LoginName;
                data.Username = CreateReportConfig.UserName;
                data.BranchId = CreateReportConfig.BranchId;
                data.ReportTypeCode = (reportType != null) ? reportType.REPORT_TYPE_CODE : null;
                var rtem = this.listReportTemplateADO.Where(o => o.IsChecked).FirstOrDefault();
                data.ReportTemplateCode = rtem != null ? rtem.REPORT_TEMPLATE_CODE : "";
                data.ReportName = txtReportName.Text.Trim();
                data.Description = txtDescription.Text.Trim();

                int count = 0;

                strFilter = "{";

                foreach (LayoutControlItem item in layoutControlItemAll)
                {
                    if (item != null && (item.Control is System.Windows.Forms.UserControl || item.Control is DevExpress.XtraEditors.XtraUserControl))
                    {
                        if (count == 0)
                        {
                            count += 1;
                        }
                        else
                        {
                            strFilter += seperator;
                        }

                        strFilter += CreateFilter(item.Control);

                        if (!CheckValid(item.Control))
                        {
                            valid = false;
                        }
                    }
                    else if (item != null && (item.Control is LayoutControl))
                    { 
                    
                    }
                }

                strFilter += "}";
                strFilter = strFilter.Replace(",,", ",");
                Inventec.Common.Logging.LogSystem.Info(strFilter);
                data.Filter = Newtonsoft.Json.JsonConvert.DeserializeObject(strFilter);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                if (valid)
                {
                    //if (CreateReportDelegate.ProcessCreateReport != null)
                    //    CreateReportDelegate.ProcessCreateReport(data);

                    if (CreateReportDelegate.ProcessCreateReportViewAway != null)
                        sarReportCreate = CreateReportDelegate.ProcessCreateReportViewAway(data);

                    //Disable nút yêu cầu
                    this.btnSave.Enabled = false;
                    //Nếu kết quả null thì Enable lại nút yêu cầu
                    if (sarReportCreate == null || sarReportCreate.ID == 0)
                    {
                        this.btnSave.Enabled = true;
                    }
                    else
                    {
                        //ngược lại nếu checkbox là true thì tạo 1 time có 15 lần check, mỗi lần 2s                        
                        tmViewAway.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool CheckValid(object dataItem)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                IDelegacy delegacy = new DCV.APP.Report.CheckValid.CheckValid(param, dataItem);
                result = (bool)delegacy.Execute();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        string CreateFilter(object dataItem)
        {
            string result = "";
            try
            {
                CommonParam param = new CommonParam();
                IDelegacy delegacy = new DCV.APP.Report.JsonOutput.JsonOutput(param, dataItem);
                result = (string)delegacy.Execute();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void bbtRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

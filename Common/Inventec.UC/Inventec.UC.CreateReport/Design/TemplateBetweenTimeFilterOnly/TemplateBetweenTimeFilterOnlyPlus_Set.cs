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
using Inventec.UC.CreateReport.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly
{
    internal partial class TemplateBetweenTimeFilterOnly
    {
        private void setDataToFilterTotal()
        {
            try
            {
                sarReport = new MRS.SDO.CreateReportSDO();
                sarReport.ReportTypeCode = GlobalStore.reportType.REPORT_TYPE_CODE;
                sarReport.ReportTemplateCode = GlobalStore.reportTemplates.Where(o => o.ID == (long)(cboReportTemplate.EditValue)).FirstOrDefault().REPORT_TEMPLATE_CODE;
                sarReport.ReportName = txtReportName.Text;
                sarReport.Description = txtDescription.Text;
                sarReport.Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                sarReport.Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                ConvertToFilter();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ConvertToFilter()
        {
            try
            {
                if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00024")
                {
                    MRS.Filter.Mrs00024Filter mrsFilter = new MRS.Filter.Mrs00024Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00025")
                {
                    MRS.Filter.Mrs00025Filter mrsFilter = new MRS.Filter.Mrs00025Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00026")
                {
                    MRS.Filter.Mrs00026Filter mrsFilter = new MRS.Filter.Mrs00026Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00027")
                {
                    MRS.Filter.Mrs00027Filter mrsFilter = new MRS.Filter.Mrs00027Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00028")
                {
                    MRS.Filter.Mrs00028Filter mrsFilter = new MRS.Filter.Mrs00028Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00029")
                {

                    MRS.Filter.Mrs00029Filter mrsFilter = new MRS.Filter.Mrs00029Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00030")
                {

                    MRS.Filter.Mrs00030Filter mrsFilter = new MRS.Filter.Mrs00030Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00017")
                {

                    MRS.Filter.Mrs00017Filter mrsFilter = new MRS.Filter.Mrs00017Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00018")
                {

                    MRS.Filter.Mrs00018Filter mrsFilter = new MRS.Filter.Mrs00018Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00019")
                {

                    MRS.Filter.Mrs00019Filter mrsFilter = new MRS.Filter.Mrs00019Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00020")
                {

                    MRS.Filter.Mrs00020Filter mrsFilter = new MRS.Filter.Mrs00020Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00021")
                {

                    MRS.Filter.Mrs00021Filter mrsFilter = new MRS.Filter.Mrs00021Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00031")
                {

                    MRS.Filter.Mrs00031Filter mrsFilter = new MRS.Filter.Mrs00031Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00032")
                {

                    MRS.Filter.Mrs00032Filter mrsFilter = new MRS.Filter.Mrs00032Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00033")
                {
                    MRS.Filter.Mrs00033Filter mrsFilter = new MRS.Filter.Mrs00033Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00034")
                {

                    MRS.Filter.Mrs00034Filter mrsFilter = new MRS.Filter.Mrs00034Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00035")
                {

                    MRS.Filter.Mrs00035Filter mrsFilter = new MRS.Filter.Mrs00035Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00036")
                {

                    MRS.Filter.Mrs00036Filter mrsFilter = new MRS.Filter.Mrs00036Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00037")
                {

                    MRS.Filter.Mrs00037Filter mrsFilter = new MRS.Filter.Mrs00037Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00038")
                {

                    MRS.Filter.Mrs00038Filter mrsFilter = new MRS.Filter.Mrs00038Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00039")
                {

                    MRS.Filter.Mrs00039Filter mrsFilter = new MRS.Filter.Mrs00039Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00040")
                {

                    MRS.Filter.Mrs00040Filter mrsFilter = new MRS.Filter.Mrs00040Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00041")
                {

                    MRS.Filter.Mrs00041Filter mrsFilter = new MRS.Filter.Mrs00041Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00042")
                {

                    MRS.Filter.Mrs00042Filter mrsFilter = new MRS.Filter.Mrs00042Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00043")
                {

                    MRS.Filter.Mrs00043Filter mrsFilter = new MRS.Filter.Mrs00043Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00044")
                {

                    MRS.Filter.Mrs00044Filter mrsFilter = new MRS.Filter.Mrs00044Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00045")
                {

                    MRS.Filter.Mrs00045Filter mrsFilter = new MRS.Filter.Mrs00045Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00046")
                {

                    MRS.Filter.Mrs00046Filter mrsFilter = new MRS.Filter.Mrs00046Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00047")
                {

                    MRS.Filter.Mrs00047Filter mrsFilter = new MRS.Filter.Mrs00047Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00048")
                {

                    MRS.Filter.Mrs00048Filter mrsFilter = new MRS.Filter.Mrs00048Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00050")
                {

                    MRS.Filter.Mrs00050Filter mrsFilter = new MRS.Filter.Mrs00050Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00052")
                {

                    MRS.Filter.Mrs00052Filter mrsFilter = new MRS.Filter.Mrs00052Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00055")
                {

                    MRS.Filter.Mrs00055Filter mrsFilter = new MRS.Filter.Mrs00055Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00056")
                {

                    MRS.Filter.Mrs00056Filter mrsFilter = new MRS.Filter.Mrs00056Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00057")
                {

                    MRS.Filter.Mrs00057Filter mrsFilter = new MRS.Filter.Mrs00057Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00058")
                {

                    MRS.Filter.Mrs00058Filter mrsFilter = new MRS.Filter.Mrs00058Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00059")
                {

                    MRS.Filter.Mrs00059Filter mrsFilter = new MRS.Filter.Mrs00059Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00060")
                {

                    MRS.Filter.Mrs00060Filter mrsFilter = new MRS.Filter.Mrs00060Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00061")
                {

                    MRS.Filter.Mrs00061Filter mrsFilter = new MRS.Filter.Mrs00061Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00068")
                {

                    MRS.Filter.Mrs00068Filter mrsFilter = new MRS.Filter.Mrs00068Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00077")
                {

                    MRS.Filter.Mrs00077Filter mrsFilter = new MRS.Filter.Mrs00077Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00078")
                {

                    MRS.Filter.Mrs00078Filter mrsFilter = new MRS.Filter.Mrs00078Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00079")
                {

                    MRS.Filter.Mrs00079Filter mrsFilter = new MRS.Filter.Mrs00079Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00080")
                {

                    MRS.Filter.Mrs00080Filter mrsFilter = new MRS.Filter.Mrs00080Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00087")
                {

                    MRS.Filter.Mrs00087Filter mrsFilter = new MRS.Filter.Mrs00087Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
                else if (GlobalStore.reportType.REPORT_TYPE_CODE == "MRS00088")
                {

                    MRS.Filter.Mrs00088Filter mrsFilter = new MRS.Filter.Mrs00088Filter();
                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        mrsFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    sarReport.Filter = mrsFilter;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool SetDelegateHasException(ProcessHasException hasException)
        {

            bool result = false;
            try
            {
                this._HasException = hasException;
                if (this._HasException != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hasException), hasException));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateCloseContainerForm(CloseContainerForm closeContainerForm)
        {

            bool result = false;
            try
            {
                this._CloseContainerForm = closeContainerForm;
                if (this._CloseContainerForm != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => closeContainerForm), closeContainerForm));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal void ShortcutButtonSearchClick()
        {
            try
            {
                //btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ShortcutButtonRefreshClick()
        {
            try
            {
                //btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

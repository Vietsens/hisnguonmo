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
using System.Windows.Forms;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using MOS.SDO;
using DevExpress.XtraCharts;
using System.Globalization;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Runtime.InteropServices;
using HIS.Desktop.Plugins.Dashboard.Base;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.Dashboard.ADO;

namespace HIS.Desktop.Plugins.Dashboard
{
    public partial class UCDashBoard : UserControl
    {
        List<HipoFinanceReportSDO> hipoFinanceReportSDOs { get; set; }
        List<HipoFinanceReportADO> hipoFinanceReportADOs { get; set; }
        DataTable dataTable { get; set; }

        public static bool isRunning = false;

        enum CHART_TYPE
        {
            TODAY
        }

        public UCDashBoard()
        {
            InitializeComponent();
        }

        private void UCDashBoard_Load(object sender, EventArgs e)
        {
            try
            {
                cboFilter.SelectedIndex = 2;
                btnThongKe_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadHipoFinanceReport()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                hipoFinanceReportADOs = this.FilterDataDateTime();
                if (hipoFinanceReportADOs != null && hipoFinanceReportADOs.Count > 0)
                {
                    List<List<string>> filter = hipoFinanceReportADOs.Select(o => o.listTimeStr).ToList();
                    hipoFinanceReportSDOs = new BackendAdapter(param).Get<List<HipoFinanceReportSDO>>("api/statistic/FinanceReport", ApiConsumers.MosConsumer, filter, param);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateChartControl()
        {
            try
            {


                LoadHipoFinanceReport();

                dataTable = new DataTable("Table");
                dataTable.Columns.Add("ky", typeof(string));
                dataTable.Columns.Add("tien_bh", typeof(decimal));
                dataTable.Columns.Add("tien_vienphi", typeof(decimal));
                dataTable.Columns.Add("tien_dv", typeof(decimal));
                dataTable.Columns.Add("tien_tamung", typeof(decimal));
                if (hipoFinanceReportADOs == null || hipoFinanceReportSDOs == null || hipoFinanceReportADOs.Count != hipoFinanceReportSDOs.Count)
                {
                    return;
                }
                for (int i = 0; i < hipoFinanceReportADOs.Count; i++)
                {
                    DataRow row = dataTable.NewRow();

                    row["ky"] = hipoFinanceReportADOs[i].Name;
                    row["tien_bh"] = hipoFinanceReportSDOs[i].dt_bh ?? 0;
                    row["tien_vienphi"] = hipoFinanceReportSDOs[i].dt_vp ?? 0;
                    row["tien_dv"] = hipoFinanceReportSDOs[i].dt_dv ?? 0;
                    row["tien_tamung"] = hipoFinanceReportSDOs[i].dt_tu ?? 0;
                    dataTable.Rows.Add(row);
                }
                chartControlDashBoard.Series[0].DataSource = dataTable;
                chartControlDashBoard.Series[1].DataSource = dataTable;
                chartControlDashBoard.Series[2].DataSource = dataTable;
                chartControlDashBoard.Series[3].DataSource = dataTable;
            }
            catch (Exception ex)
            {
                cboFilter.Enabled = true;
                btnThongKe.Enabled = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<string> GetListFirstLastTimeString(DateTime dtFrom, DateTime dtTo)
        {
            List<string> result = new List<string>();
            try
            {
                result.Add((dtFrom.ToString("yyyyMMdd") + "000000"));
                result.Add((dtTo.ToString("yyyyMMdd") + "235959"));
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;

        }

        private void PlusTime(ref DateTime dtPos, DateTime dtEnd, [Optional] long distance)
        {
            try
            {
                if (cboFilter.SelectedIndex == FilterCFG.TUAN)
                {
                    //dtPos = dtPos.AddHours(distance);
                }
                else if (cboFilter.SelectedIndex == FilterCFG.TUAN)
                {
                    //DateTime dt = dtPos.AddDays(distance);
                    //if (dt.Month != dtPos.Month)
                    //{
                    //    dtPos = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(dtPos.AddDays(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1).Day - dtPos.Day).ToString("yyyyMMdd") + "235959")).Value;
                    //}
                    //else
                    //{
                    //    dtPos = dt;
                    //}
                }
                else if (cboFilter.SelectedIndex == FilterCFG.TUAN)
                {
                    //dtPos = dtPos.AddDays(distance);
                }
                else if (cboFilter.SelectedIndex == FilterCFG.NGAY)
                {
                    distance = Inventec.Common.TypeConvert.Parse.ToInt64(ucFilterByDay.cboBuocNhay.EditValue.ToString());
                    DateTime dt = dtPos.AddDays(distance);
                    dtPos = DateTime.Compare(dt.Date, dtEnd.Date) >= 0 ? dtPos = dtEnd : dtPos = dt;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //InitEnableControl();
                if (cboFilter.SelectedIndex == FilterCFG.NGAY)
                {
                    this.InitUcFilterByDay();
                }
                else if (cboFilter.SelectedIndex == FilterCFG.TUAN)
                {
                    this.InitUcFilterByWeek();
                }
                else if (cboFilter.SelectedIndex == FilterCFG.THANG)
                {
                    this.InitUcFilterByMonth();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            try
            {
                if (Check())
                {
                    CreateChartControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool Check()
        {
            bool result = true;
            try
            {
                if (cboFilter.SelectedIndex == FilterCFG.NGAY && ucFilterByDay != null)
                {
                    result = ucFilterByDay.ValidateUC();
                }
                else if (cboFilter.SelectedIndex == FilterCFG.TUAN && ucFilterByWeek != null)
                {
                    result = result && ucFilterByWeek.ValidateUC();
                }
                else if (cboFilter.SelectedIndex == FilterCFG.THANG && ucFilterByMonth != null)
                {
                    result = result && ucFilterByMonth.ValidateUC();
                }

            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void chartControlDashBoard_AxisScaleChanged(object sender, AxisScaleChangedEventArgs e)
        {
            
        }
    }
}

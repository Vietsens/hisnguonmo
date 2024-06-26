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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LIS.Desktop.Plugins.SampleWarning
{
    public partial class frmSampleWarning : FormBase
    {
        private int start = 0;
        private int limit = 0;
        private int rowCount = 0;
        private int totalData = 0;

        public frmSampleWarning(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void frmSampleWarning_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetDefaultControl();
                this.FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                txtKeyword.Text = "";
                dtIntructionTimeFrom.DateTime = DateTime.Now;
                dtIntructionTimeTo.DateTime = DateTime.Now;
                cboTypeFilter.SelectedIndex = 0;
                spinHourNumber.EditValue = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillDataToGrid()
        {
            int numPageSize;
            if (ucPaging1.pagingGrid != null)
            {
                numPageSize = ucPaging1.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
            }

            LoadPaging(new CommonParam(0, numPageSize));

            CommonParam param = new CommonParam();
            param.Limit = this.rowCount;
            param.Count = this.totalData;
            ucPaging1.Init(LoadPaging, param, numPageSize);
        }

        private void LoadPaging(object param)
        {
            try
            {
                this.start = ((CommonParam)param).Start ?? 0;
                this.limit = ((CommonParam)param).Limit ?? 0;
                List<V_LIS_SAMPLE> listData = new List<V_LIS_SAMPLE>();
                CommonParam paramCommon = new CommonParam(this.start, this.limit);
                LisSampleViewFilter filter = new LisSampleViewFilter();
                filter.KEY_WORD = txtKeyword.Text;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "INTRUCTION_TIME";

                if (dtIntructionTimeFrom.EditValue != null && dtIntructionTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.INTRUCTION_TIME_FROM = Convert.ToInt64(dtIntructionTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtIntructionTimeTo.EditValue != null && dtIntructionTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.INTRUCTION_TIME_TO = Convert.ToInt64(dtIntructionTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }
                if (cboTypeFilter.SelectedIndex == 0)
                {
                    filter.SAMPLE_STT_IDs = new List<long>()
                    {
                        IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM,
                        IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI
                    };
                    filter.INTRUCTION_TIME_HOUR__GREATER = (long)spinHourNumber.Value;
                }
                else if (cboTypeFilter.SelectedIndex == 1)
                {
                    filter.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM;
                    filter.SAMPLE_TIME_HOUR__GREATER = (long)spinHourNumber.Value;
                }
                else
                {
                    filter.SAMPLE_STT_IDs = new List<long>()
                    {
                        IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN,
                        IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ,
                        IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                    };
                    filter.APPROVAL_TIME_HOUR__GREATER = (long)spinHourNumber.Value;
                }

                var rs = new BackendAdapter(paramCommon).GetRO<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, filter, paramCommon);
                if (rs != null)
                {
                    if (rs.Data != null)
                    {
                        listData = rs.Data;
                    }
                    this.rowCount = (listData == null ? 0 : listData.Count);
                    this.totalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }

                gridControlSample.BeginUpdate();
                gridControlSample.DataSource = listData;
                gridControlSample.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid();
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultControl();
                FillDataToGrid();
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_LIS_SAMPLE data = (V_LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + this.start;
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "PATIENT_NAME")
                        {
                            e.Value = data.LAST_NAME + " " + data.FIRST_NAME;
                        }
                        else if (e.Column.FieldName == "PATIENT_DOB_STR")
                        {
                            //e.Value = (data.DOB ?? 0).ToString().Substring(0, 4);
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                        }
                        else if (e.Column.FieldName == "GENDER_NAME")
                        {
                            e.Value = data.GENDER_CODE == "01" ? "Nữ" : "Nam";
                        }
                        else if (e.Column.FieldName == "SAMPLE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.SAMPLE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "REQUEST_USER")
                        {
                            e.Value = (data.REQUEST_LOGINNAME ?? "") + " - " + (data.REQUEST_USERNAME ?? "");
                        }
                        else if (e.Column.FieldName == "SAMPLE_USER")
                        {
                            e.Value = (data.SAMPLE_LOGINNAME ?? "") + " - " + (data.SAMPLE_USERNAME ?? "");
                        }
                        else if (e.Column.FieldName == "APPROVE_USER")
                        {
                            e.Value = (data.APPROVAL_LOGINNAME ?? "") + " - " + (data.APPROVAL_USERNAME ?? "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

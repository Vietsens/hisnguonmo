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
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.WebApiClient;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using SDA.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Notify
{
    public partial class frmNotify : Form
    {
        List<NotifyADO> listNotifyADO;
        private int pageNum = 20;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int start = 0;
        private int limit = 0;
        private bool isInit = true;
        private ApiConsumer sdaConsumer = null;
        private string loginname = "";
        private string departmentCode = "";
        public frmNotify(List<NotifyADO> _listNotifyADO, ApiConsumer consumer, CommonParam param, string login_name,string departmentCode)
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.listNotifyADO = _listNotifyADO;
                this.sdaConsumer = consumer;
                this.rowCount = _listNotifyADO != null ? _listNotifyADO.Count : 0;
                if (param != null)
                {
                    this.dataTotal = param.Count ?? 0;
                }
                this.loginname = login_name;
                this.departmentCode = departmentCode;
                try
                {
                    gridControlNotify.BeginUpdate();
                    gridControlNotify.DataSource = _listNotifyADO;
                    gridControlNotify.EndUpdate();
                    //gridViewNotify.Focus();
                    //gridViewNotify_RowCellClick(null, null);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewNotify_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewNotify_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var data = (NotifyADO)gridViewNotify.GetRow(e.RowHandle);
                if (data != null && !data.Status)
                {
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                {
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Regular);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewNotify_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
            {
                NotifyADO pData = (NotifyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                try
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start;
                    }
                    else if (e.Column.FieldName == "Status")
                    {
                        if (pData.Status)
                        {
                            e.Value = "Đã đọc";
                        }
                        else
                        {
                            e.Value = "Chưa đọc";
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                }
                catch (Exception ex)
                {

                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }

        private void gridViewNotify_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var row = (NotifyADO)gridViewNotify.GetFocusedRow();
                if (row != null)
                {
                    FillDataToMemo(row);
                    this.InitThreadCallApiRead(row);
                    row.Status = true;
                    gridControlNotify.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitThreadCallApiRead(NotifyADO row)
        {
            try
            {
                if (row != null)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(this.ThreadMaksRead));
                    thread.Start(row.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadMaksRead(object data)
        {
            try
            {
                long id = (long)data;
                CommonParam p = new CommonParam();
                if (!this.ProcessMaskRead(new List<long>() { id }, ref p))
                {
                    LogSystem.Warn("Danh dau da doc that bai. NotifyId: " + id + "\n" + LogUtil.TraceData("Param", p));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessMaskRead(List<long> ids, ref CommonParam commonParam)
        {
            bool result = false;
            try
            {
                if (ids != null && ids.Count > 0 && !String.IsNullOrWhiteSpace(loginname))
                {
                    SdaNotifySeenSDO sdo = new SdaNotifySeenSDO();
                    sdo.Ids = ids;
                    result = new BackendAdapter(commonParam).Post<bool>("api/SdaNotify/NotifySeen", sdaConsumer, sdo, commonParam);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void FillDataToMemo(NotifyADO data)
        {
            try
            {
                memoEditNotify.Text = data.CONTENT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSearch.Enabled) return;
                this.FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMaskReadAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnMaskReadAll.Enabled) return;
                if (listNotifyADO == null || listNotifyADO.Count <= 0 || !listNotifyADO.Any(a => !a.Status))
                    return;
                WaitingManager.Show();
                bool success = false;
                CommonParam p = new CommonParam();
                success = this.ProcessMaskRead(listNotifyADO.Where(o => !o.Status).Select(s => s.ID).ToList(), ref p);
                if (success)
                {
                    listNotifyADO.ForEach(f => f.Status = true);
                    gridControlNotify.RefreshDataSource();
                }
                WaitingManager.Hide();
                MessageManager.Show(this, p, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmNotify_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetDefaultControl();
                this.FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                txtKeword.Text = "";
                cboStatus.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                int pageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)pageNum;
                }

                FillDataToGridNotify(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridNotify, param, pageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGridNotify(object param)
        {
            try
            {
                if (isInit)
                {
                    isInit = false;
                    return;
                }
                this.start = ((CommonParam)param).Start ?? 0;
                this.limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                this.listNotifyADO = new List<NotifyADO>();

                SdaNotifyFilter filter = new SdaNotifyFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                //filter.HAS_RECEIVER_LOGINNAME_OR_NULL = true;
                //filter.RECEIVER_LOGINNAME_EXACT_OR_NULL = this.loginname;
                filter.KEY_WORD = txtKeword.Text;
                if (cboStatus.SelectedIndex == 0)
                {
                    filter.WATCHED = false;
                }
                else if (cboStatus.SelectedIndex == 1)
                {
                    filter.WATCHED = true;
                }
                filter.HAS_RECEIVER_LOGINNAME_OR_NULL = true;
                filter.NOW_TIME = Inventec.Common.DateTime.Get.Now().Value;
                filter.RECEIVER_DEPARTMENT_CODES_OR_NULL = departmentCode;
                filter.RECEIVER_LOGINNAMES_EXACT_OR_NULL = loginname;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                var rs = new BackendAdapter(paramCommon).GetRO<List<SDA_NOTIFY>>("/api/SdaNotify/Get", sdaConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs.Data), rs.Data));
                if (rs != null)
                {
                    if (rs.Data != null)
                    {
                        this.listNotifyADO = (from r in rs.Data select new NotifyADO(r)).ToList();
                        this.listNotifyADO.ForEach(e => e.SetIsRead(this.loginname));
                    }
                    this.rowCount = (this.listNotifyADO == null ? 0 : this.listNotifyADO.Count);
                    this.dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }

                gridControlNotify.BeginUpdate();
                gridControlNotify.DataSource = this.listNotifyADO;
                gridControlNotify.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewNotify_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                if (gridControlNotify.DataSource != null)
                {
                    gridViewNotify.SelectRow(0);
                    gridViewNotify.FocusedRowHandle = 0;
                    var row = (NotifyADO)gridViewNotify.GetFocusedRow();
                    FillDataToMemo(row);
                    this.InitThreadCallApiRead(row);
                    row.Status = true;
                    gridControlNotify.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

      
    }
}

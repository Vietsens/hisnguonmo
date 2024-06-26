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
using MOS.SDO;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.SchedulerJob
{
    public partial class UCSchedulerJob : HIS.Desktop.Utility.UserControlBase
    {
        List<UserSchedulerJobResultSDO> UserSchedulerJobResultSDOs { get; set; }

        public UCSchedulerJob(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
        }

        private void UCSchedulerJob_Load(object sender, EventArgs e)
        {
            try
            {
                LoadSchedulerJob();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSchedulerJob_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    UserSchedulerJobResultSDO dataRow = (UserSchedulerJobResultSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "NEXT_RUN_DATE_DISPLAY" && dataRow.NEXT_RUN_DATE.HasValue)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.NEXT_RUN_DATE ?? 0);
                    }
                    else if (e.Column.FieldName == "START_DATE_DISPLAY" && dataRow.START_DATE.HasValue)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.START_DATE ?? 0);
                    }
                    else if (e.Column.FieldName == "LAST_START_DATE_DISPLAY" && dataRow.LAST_START_DATE.HasValue)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.LAST_START_DATE ?? 0);
                    }
                    else if (e.Column.FieldName == "ENABLED_DISPLAY")
                    {
                        e.Value = dataRow.ENABLED ? "Bật" : "Tắt";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSchedulerJob_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                //UserSchedulerJobResultSDO userSchedulerJobResultSDO = (UserSchedulerJobResultSDO)gridViewSchedulerJob.GetRow(e.RowHandle);
                //if (userSchedulerJobResultSDO != null)
                //{
                //    if (userSchedulerJobResultSDO.ENABLED)
                //        e.Appearance.ForeColor = System.Drawing.Color.Blue;
                //    else
                //        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewSchedulerJob_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                UserSchedulerJobResultSDO data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewSchedulerJob.GetDataSourceRowIndex(e.RowHandle);
                    data = (UserSchedulerJobResultSDO)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        if (e.Column.FieldName == "TRANG_THAI")
                        {
                            if (data.ENABLED)
                            {
                                e.RepositoryItem = repositoryItemButtonEditEnabled;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditDisabled;
                            }
                        }
                        else if (e.Column.FieldName == "LOCK")
                        {
                            e.RepositoryItem = (data.ENABLED ? btnGUnLock : btnGLock);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {

                UserSchedulerJobResultSDO data = (UserSchedulerJobResultSDO)gridViewSchedulerJob.GetFocusedRow();
                WaitingManager.Show();
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/UserSchedulerJob/ChangeLock", ApiConsumers.MosConsumer, data.JOB_NAME, param);
                WaitingManager.Hide();
                if (result)
                {
                    success = true;
                    LoadSchedulerJob();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {

                UserSchedulerJobResultSDO data = (UserSchedulerJobResultSDO)gridViewSchedulerJob.GetFocusedRow();
                WaitingManager.Show();
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/UserSchedulerJob/ChangeLock", ApiConsumers.MosConsumer, data.JOB_NAME, param);
                WaitingManager.Hide();
                if (result)
                {
                    success = true;
                    LoadSchedulerJob();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGRun_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                UserSchedulerJobResultSDO data = (UserSchedulerJobResultSDO)gridViewSchedulerJob.GetFocusedRow();
                WaitingManager.Show();
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/UserSchedulerJob/Run", ApiConsumers.MosConsumer, data.JOB_NAME, param);
                WaitingManager.Hide();
                if (result)
                {
                    success = true;
                    LoadSchedulerJob();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadSchedulerJob();
        }
    }
}


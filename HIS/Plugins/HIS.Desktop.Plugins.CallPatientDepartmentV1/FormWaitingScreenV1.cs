using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientDepartmentV1
{
    public partial class FormWaitingScreenV1 : FormBase
    {
        private List<ADO.RoomADO> LstRoom;
        Inventec.Desktop.Common.Modules.Module _Module;
        int index = 0;
        int countOldList = 0;
        int reloadTimeInSeconds = 10;

        public FormWaitingScreenV1()
        {
            InitializeComponent();
        }

        public FormWaitingScreenV1(List<ADO.RoomADO> lstRoom, Inventec.Desktop.Common.Modules.Module module, int reloadTime)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.LstRoom = lstRoom;
                this._Module = module;
                this.reloadTimeInSeconds = reloadTime > 0 ? reloadTime : 10;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormWaitingScreenV1_Load(object sender, EventArgs e)
        {
            try
            {
                SetFromConfigToControl();

                // ✅ Load data async, form hiện ra ngay không bị trắng
                Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(300); // chờ form show xong
                    FillDataToGridControl();
                });

                timerReload.Interval = reloadTimeInSeconds * 1000;
                timerReload.Enabled = true;
                timerReload.Start();

                timerScroll.Interval = 5000;
                timerScroll.Enabled = true;
                timerScroll.Start();

                this.WindowState = FormWindowState.Maximized;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ✅ Thêm event Shown — chạy SAU khi form hiển thị hoàn toàn
        private void FormWaitingScreenV1_Shown(object sender, EventArgs e)
        {
            try
            {
                this.BringToFront();
                this.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFromConfigToControl()
        {
            try
            {
                List<int> gridBackColorCodes = WaitingScreenCFG.PARENT_BACK_COLOR_CODES;
                if (gridBackColorCodes != null && gridBackColorCodes.Count == 3)
                {
                    gridView1.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(gridBackColorCodes[0], gridBackColorCodes[1], gridBackColorCodes[2]);
                }

                List<int> gridpatientHeaderBackColorCodes = WaitingScreenCFG.GRID_NUM_ORDERS_HEADER_BACK_COLOR_CODES;
                if (gridpatientHeaderBackColorCodes != null && gridpatientHeaderBackColorCodes.Count == 3)
                {
                    foreach (GridColumn col in gridView1.Columns)
                    {
                        col.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    }
                }

                List<int> gridpatientHeaderForceColorCodes = WaitingScreenCFG.GRID_NUM_ORDERS_HEADER_FORCE_COLOR_CODES;
                if (gridpatientHeaderForceColorCodes != null && gridpatientHeaderForceColorCodes.Count == 3)
                {
                    foreach (GridColumn col in gridView1.Columns)
                    {
                        col.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                if (LstRoom == null || LstRoom.Count == 0) return;

                CommonParam param = new CommonParam();
                MOS.Filter.HisRoomCounterLView3Filter filter = new MOS.Filter.HisRoomCounterLView3Filter ();
                filter.ROOM_IDs = LstRoom.Select(o => o.ROOM_ID).ToList();

                var result = new BackendAdapter(param).Get<List<L_HIS_ROOM_COUNTER_3>>("api/HisRoom/GetCounterLView3", ApiConsumers.MosConsumer, filter, param);

                if (result != null && result.Count > 0)
                {
                    countOldList = result.Count;
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            gridControl1.BeginUpdate();
                            gridControl1.DataSource = result;
                            gridControl1.EndUpdate();
                            if (countOldList == index)
                            {
                                index = 0;
                            }
                            gridView1.FocusedRowHandle = index;
                        }));
                    }
                    else
                    {
                        gridControl1.BeginUpdate();
                        gridControl1.DataSource = result;
                        gridControl1.EndUpdate();
                    }
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerReload_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGrid()
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadLoadDataGrid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ExecuteThreadLoadDataGrid()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { FillDataToGridControl(); }));
                }
                else
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerScroll_Tick(object sender, EventArgs e)
        {
            try
            {
                CreateNextRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateNextRow()
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadCreateNextRow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ExecuteThreadCreateNextRow()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { NextRowGridView(); }));
                }
                else
                {
                    NextRowGridView();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextRowGridView()
        {
            try
            {
                if (gridView1.RowCount <= 0)
                    return;
                if (index == countOldList)
                    index = 0;
                gridView1.FocusedRowHandle = index;
                if (index == gridView1.RowCount)
                    index = 0;
                else
                    index++;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "NEXT_CALL_NUMBER" && (e.Value == null || e.Value == DBNull.Value))
                {
                    e.DisplayText = "Không có";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormWaitingScreenV1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timerReload.Stop();
                timerScroll.Stop();
            }
            catch (Exception ex)
            {
                timerReload.Stop();
                timerScroll.Stop();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

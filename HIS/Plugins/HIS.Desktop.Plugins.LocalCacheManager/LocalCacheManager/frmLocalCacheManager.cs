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
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Common.Repository;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using System.Configuration;

using System.IO;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.LocalCacheManager
{
    public partial class frmLocalCacheManager : HIS.Desktop.Utility.FormBase
    {
        List<CacheData> cacheDatas = new List<CacheData>();
        CacheWorker CacheWorker { get { return (CacheWorker)Worker.Get<CacheWorker>(); } }
        CacheMonitorGet CacheMonitorGet { get { return (CacheMonitorGet)Worker.Get<CacheMonitorGet>(); } }

        public frmLocalCacheManager(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmLocalCacheManager_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.LoadData();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void btnRefreshAll_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var listOlds = BackendDataWorker.GetAll().Select(o => o.Key.ToString()).ToList();
                if (listOlds != null)
                {
                    ProcessData.ProcessSyncAllData();
                    ProcessData.ReloadAllCacheBackendData(listOlds);
                    this.LoadData();
                    bool success = true;
                    CommonParam param = new CommonParam();
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = GetDataSourceWithFilter();
                gridView1.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        CacheData cacheData = (CacheData)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (cacheData != null)
                        {
                            if (e.Column.FieldName == "OBJECT_NAME_DISPLAY")
                            {
                                e.Value = (!String.IsNullOrEmpty(cacheData.Description) ? cacheData.Description : cacheData.ObjectName);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "ISTL")
                    {
                        string iSTL = ((view.GetRowCellValue(e.RowHandle, view.Columns["ISTL"]) ?? "").ToString());
                        if (iSTL == "1")
                        {
                            e.RepositoryItem = this.repositoryItembtnTDTLTrue;
                        }
                        else
                        {
                            e.RepositoryItem = this.repositoryItembtnTDTLFalse;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DeleteOneRow();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void repositoryItembtnTDTLTrue_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //Khi người dùng nhấn nút này cho phép thiết lập từ "TĐLM" => chuyển thành "Bỏ TĐLM" (sửa lại dữ liệu trong file CacheMonitorConfig.xml)
            try
            {
                CommonParam param = new CommonParam();
                CacheData rowData = gridView1.GetFocusedRow() as CacheData;
                if (HIS.Desktop.XmlCacheMonitor.CacheMonitorKeyStore.UpdateByCode(rowData.ObjectName, "0"))
                {
                    rowData.ISTL = "0";
                    if (this.gridView1.IsEditing)
                        this.gridView1.CloseEditor();

                    if (this.gridView1.FocusedRowModified)
                        this.gridView1.UpdateCurrentRow();
                    MessageManager.Show(param, true);
                }
                else
                {
                    param.Messages.Add("Cập nhật file CacheMonitorConfig.xml thất bại, ObjectName = " + rowData.ObjectName);
                    MessageManager.Show(param, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItembtnTDTLFalse_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //Khi người dùng nhấn nút này cho phép thiết lập từ "Bỏ TĐLM" => chuyển thành "TĐLM" (sửa lại dữ liệu trong file CacheMonitorConfig.xml)
            try
            {
                CommonParam param = new CommonParam();
                CacheData rowData = gridView1.GetFocusedRow() as CacheData;
                if (HIS.Desktop.XmlCacheMonitor.CacheMonitorKeyStore.UpdateByCode(rowData.ObjectName, "1"))
                {
                    rowData.ISTL = "1";
                    if (this.gridView1.IsEditing)
                        this.gridView1.CloseEditor();

                    if (this.gridView1.FocusedRowModified)
                        this.gridView1.UpdateCurrentRow();
                    MessageManager.Show(param, true);
                }
                else
                {
                    param.Messages.Add("Cập nhật file CacheMonitorConfig.xml thất bại, ObjectName = " + rowData.ObjectName);
                    MessageManager.Show(param, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DeleteOneRow()
        {
            try
            {
                var cacheData = (CacheData)gridView1.GetFocusedRow();
                if (cacheData != null)
                {
                    WaitingManager.Show();
                    bool success = BackendDataWorker.Reset(cacheData.ObjectType);
                    Inventec.Common.Logging.LogSystem.Info("Reset data success =" + success + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cacheData), cacheData));
                    ProcessData.ResetDataExt(cacheData.ObjectName);
                    ProcessData.ReloadCacheBackendData(cacheData.ObjectType);
                    success = true;
                    this.LoadData();
                    CommonParam param = new CommonParam();
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void btnDeleteFileCache_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có thực sự muốn xóa file cache không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();      
                    CacheWorker cacheWorker = new CacheWorker();
                    if (cacheWorker.TruncateAll())
                    {
                        Inventec.Common.Logging.LogSystem.Info("Xoa toan bo du lieu cache may tram thanh cong");
                        if (MessageBox.Show("Bạn cần thực hiện đăng nhập lại để tải lại dữ liệu cache", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            if (!Inventec.Desktop.Common.Token.TokenManager.Logout())
                                Inventec.Common.Logging.LogSystem.Warn(TabWorker.formClosing);

                            //this.LogoutAndResetToDefault();

                            // Thời gian kết thúc
                            watch.Stop();
                            Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "DeleteCacheAndLogoutApp", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));


                            System.Diagnostics.Process.Start(Application.ExecutablePath);
                            GlobalVariables.isLogouter = true;
                            GlobalVariables.IsLostToken = true;
                            //close this one
                            Inventec.Common.Logging.LogSystem.Debug("bbtnClose_ItemClick.2");
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                        }
                        else
                        {
                            watch.Stop();
                            Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "DeleteCache", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Xóa file cache thất bại");
                        Inventec.Common.Logging.LogSystem.Info("Xoa toan bo du lieu cache may tram that bai");
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void LogoutAndResetToDefault()
        {
            try
            {
                if (!Inventec.Desktop.Common.Token.TokenManager.Logout())
                    Inventec.Common.Logging.LogSystem.Debug(TabWorker.formClosing);
                GlobalVariables.currentModules = null;
                GlobalVariables.currentModuleRaws = null;
                GlobalVariables.CurrentRoomTypeCodes = null;
                TabControlBaseProcess.dicSaveData = new Dictionary<string, SaveDataBeforeClose>();
                HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.ResetForLogout();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

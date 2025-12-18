using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.MchExamServiceList.Properties;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.WebApiClient;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MCH.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.MchExamServiceList
{
    public partial class Uc_MchExamServiceList : UserControl
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        public Uc_MchExamServiceList(Inventec.Desktop.Common.Modules.Module module) : base()
        {
            InitializeComponent();
            this.currentModule = module;
        }

        private void Uc_MchExamServiceList_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                FillDataToGrid();
                // disable sync button initially
                try { btnSynchronizatioon.Enabled = false; } catch { }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MCH.EFMODEL.DataModels.V_MCH_EXAM_SERVICE>> apiResult = null;
                MCH.Filter.MchExamServiceViewFilter filter = new MCH.Filter.MchExamServiceViewFilter();
                SetFilter(ref filter);
                gridViewExamService.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).
                    GetRO<List<MCH.EFMODEL.DataModels.V_MCH_EXAM_SERVICE>>("api/MchExamService/GetView", ApiConsumer.ApiConsumers.MchConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlExamService.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlExamService.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewExamService.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewExamService.EndUpdate();
            }
        }

        private void SetFilter(ref MCH.Filter.MchExamServiceViewFilter filter)
        {
            try
            {
                if (txtTreatmentCode.Text != "")
                {
                    filter.TREATMENT_CODE = txtTreatmentCode.Text.Trim();
                }
                else if (txtPatientCode.Text != "")
                {
                    filter.PATIENT_CODE = txtPatientCode.Text.Trim();
                }
                else
                {
                    filter.ORDER_FIELD = "IMP_MEST_MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.KEY_WORD = txtKeyWord.Text.Trim();

                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                    if (cboStatus.EditValue != null)
                    {
                        switch (cboStatus.SelectedIndex)
                        {
                            case 0:
                                filter.HAS_SYNC_STATUS = null;
                                break;
                            case 1:
                                filter.HAS_SYNC_STATUS = true;
                                break;
                            case 2:
                                filter.HAS_SYNC_STATUS = false;
                                break;
                            case 3:
                                filter.SYNC_STATUS = 2;
                                break;
                            default:
                                filter.HAS_SYNC_STATUS = null;
                                break;
                        }
                    }
                }

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
                dtCreateTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartMonth() ?? 0));
                dtCreateTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                txtTreatmentCode.Text = "";
                txtPatientCode.Text = "";
                txtKeyWord.Text = "";
                cboStatus.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FillDataToGrid();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExamService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var data = (V_MCH_EXAM_SERVICE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "SYNC_STATUS_STR")
                        {
                            if (data.SYNC_STATUS == 1)
                            {
                                e.Value = "Thành công";
                            }
                            else if (data.SYNC_STATUS == 2)
                            {
                                e.Value = "Thất bại";
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            if (data.DOB != null && data.DOB != 0)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.DOB);
                            }
                        }    
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExamService_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_MCH_EXAM_SERVICE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DELETE")
                        {
                            string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            if (loginName == data.CREATOR)
                            {
                                e.RepositoryItem = repositoryItemButtonEditDeleteEnable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditDeleteDisable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_MCH_EXAM_SERVICE)gridViewExamService.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    CommonParam param = new CommonParam();

                    bool rs = new BackendAdapter(param).Post<bool>("api/MchExamService/Delete", ApiConsumers.MchConsumer, row.ID, param);

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, rs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditUpdate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_MCH_EXAM_SERVICE)gridViewExamService.GetFocusedRow();
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MchTreatmentExamService").FirstOrDefault();
                    if (moduleData == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ApproveAggrImpMest");
                        return;
                    }

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSynchronizatioon_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedRows = gridViewExamService.GetSelectedRows();
                List<long> selectedIds = new List<long>();

                foreach (int rowHandle in selectedRows)
                {
                    var data = (V_MCH_EXAM_SERVICE)gridViewExamService.GetRow(rowHandle);
                    if (data != null)
                    {
                        selectedIds.Add(data.ID);
                    }
                }

                if (selectedIds.Count > 0)
                {
                    bool success = false;

                    Inventec.Common.Logging.LogSystem.Info("Selected IDs: " + string.Join(",", selectedIds));

                    var data = new BackendAdapter(new CommonParam()).Post<V_MCH_EXAM_SERVICE>("api/MchExamService/SyncData", ApiConsumers.MchConsumer, selectedIds, new CommonParam());

                    if (data != null)
                    {
                        success = true;
                    }

                    SessionManager.ProcessTokenLost(new CommonParam());
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, new CommonParam(), success);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn hồ sơ", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExamService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int[] rows = gridViewExamService.GetSelectedRows();
                bool enable = rows != null && rows.Length > 0;
                try { btnSynchronizatioon.Enabled = enable; } catch { }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
                gridViewExamService.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
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
                    var data = apiResult.Data.OrderByDescending(o => o.EXECUTE_TIME).ThenBy(o => o.CREATE_TIME).ToList();
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
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }

                    filter.TREATMENT_CODE = code;
                }
                else if (txtPatientCode.Text != "")
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE = code;
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

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
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
                            if (data.IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.DOB.ToString().Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                            }
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                        {
                            if (data.EXECUTE_TIME != null && data.EXECUTE_TIME != 0)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
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
                    Inventec.Common.Logging.LogSystem.Info("Chưa chọn hồ sơ");
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

        private void repositoryItemButtonEditDeleteEnable_Click(object sender, EventArgs e)
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

        private void repositoryItemButtonEditUpdate_Click(object sender, EventArgs e)
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

        private void AdjustGridColumns()
        {
            try
            {
                // Keep first 3 visible columns' widths unchanged,   
                // distribute remaining width among the other visible columns.
                const int fixedCount = 3;
                var view = gridViewExamService;
                if (view == null || gridControlExamService == null) return;

                // Get visible columns in display order
                var visibleCols = view.Columns
                                      .Cast<DevExpress.XtraGrid.Columns.GridColumn>()
                                      .Where(c => c.Visible)
                                      .OrderBy(c => c.VisibleIndex)
                                      .ToList();

                if (visibleCols.Count <= fixedCount) return;

                // Sum widths of first N columns
                int fixedWidth = visibleCols.Take(fixedCount).Sum(c => c.Width);

                // Available client width inside the grid control
                int totalClientWidth = gridControlExamService.ClientSize.Width;
                // small guard
                if (totalClientWidth <= fixedWidth + 50) return;

                int remaining = totalClientWidth - fixedWidth;

                var otherCols = visibleCols.Skip(fixedCount).ToList();
                if (!otherCols.Any()) return;

                // Distribute remaining width proportionally to current widths (fallback to equal)
                int sumCurrent = otherCols.Sum(c => Math.Max(1, c.Width));
                if (sumCurrent <= 0) sumCurrent = otherCols.Count;

                // Assign width, ensure minimum width
                int minWidth = 50;
                var newWidths = new List<int>();
                foreach (var c in otherCols)
                {
                    int w = (int)Math.Round((double)Math.Max(1, c.Width) / sumCurrent * remaining);
                    if (w < minWidth) w = minWidth;
                    newWidths.Add(w);
                }

                // Fix rounding difference
                int assigned = newWidths.Sum();
                int diff = remaining - assigned;
                if (diff != 0)
                {
                    // apply difference to last column
                    newWidths[newWidths.Count - 1] += diff;
                    if (newWidths[newWidths.Count - 1] < minWidth)
                        newWidths[newWidths.Count - 1] = minWidth;
                }

                // Apply new widths
                for (int i = 0; i < otherCols.Count; i++)
                {
                    otherCols[i].Width = newWidths[i];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExamService_ColumnWidthChanged(object sender, ColumnEventArgs e)
        {
            try
            {
                // keep layout consistent when user manually resizes columns
                AdjustGridColumns();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlExamService_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                AdjustGridColumns();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtPatientCode.Text != "")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtTreatmentCode.Text != "")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtKeyWord.Text != "")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                // ensure we focus the row under mouse so GetFocusedRow() returns the correct item
                var pt = gridControlExamService.PointToClient(Control.MousePosition);
                var hit = gridViewExamService.CalcHitInfo(pt);
                if (hit != null && hit.RowHandle >= 0)
                    gridViewExamService.FocusedRowHandle = hit.RowHandle;

                // reuse existing logic
                repositoryItemButtonEditDeleteEnable_Click(sender, EventArgs.Empty);
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
                // ensure we focus the row under mouse so GetFocusedRow() returns the correct item
                var pt = gridControlExamService.PointToClient(Control.MousePosition);
                var hit = gridViewExamService.CalcHitInfo(pt);
                if (hit != null && hit.RowHandle >= 0)
                    gridViewExamService.FocusedRowHandle = hit.RowHandle;

                // reuse existing logic
                repositoryItemButtonEditUpdate_Click(sender, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Integrate.EditorLoader;
using LIS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.LisWellPlate.Validate;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using HIS.Desktop.Plugins.LisWellPlate.ADO;
using System.Dynamic;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.LisWellPlate.Popup;
using LIS.SDO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraGrid.Views.Grid;
namespace HIS.Desktop.Plugins.LisWellPlate.Run
{
    public partial class UCLisWellPlate : UserControl
    {
        #region Derlare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize = 0;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        int lastRowHandle = -1;
        int positionHandle = -1;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        V_LIS_WELL_PLATE currentLisWellPlate = null;
        List<LIS_WELL_PLATE_TYPE> lstDataWellPlate = null;
        List<V_LIS_WELL_PLATE_DETAIL> lstWellPlateDetails = null;
        List<HeaderADO> lstHeaders = new List<HeaderADO>();
        List<string> lstHeaderString = new List<string>();
        List<ExpandoObject> lstDataSource = new List<ExpandoObject>();
        Dictionary<string, HeaderADO> dicData = new Dictionary<string, HeaderADO>();
        long rows = 0;
        long columns = 0;
        List<LIS_SAMPLE> lstSampleResult = new List<LIS.EFMODEL.DataModels.LIS_SAMPLE>();
        List<HeaderADO> ListResult = new List<HeaderADO>();
        List<HeaderADO> ListNull = new List<HeaderADO>();
        int visibleIndex = 0;
        int newVisibleIndex = 0;
        int newRow = 0;
        int? currentX = 0;
        int? currentY = 0;
        int oldX = 0;
        int oldY = 0;
        List<V_LIS_WELL_PLATE> lstWellPlateLeft = new List<LIS.EFMODEL.DataModels.V_LIS_WELL_PLATE>();
        #endregion
        public UCLisWellPlate(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCLisWellPlate_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboOptionWellPlate();
                SetLanguage();
                SetValidateNote();
                SetValidateCbo();
                SetDefaultValueLeft();
                SetDefaultValueRight();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetLanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.LisWellPlate.Resources.Lang", typeof(HIS.Desktop.Plugins.LisWellPlate.Run.UCLisWellPlate).Assembly);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueLeft()
        {
            try
            {
                dtCreateFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartDay() ?? 0));
                dtCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                txtSearch.Text = "";
                txtSearchWellPlateCode.Text = "";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueRight()
        {
            try
            {
                ClearGrid();
                txtWellPlateCode.Text = "";
                txtNote.Text = "";
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                btnSave.Enabled = false;
                lcibtnSave.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                currentLisWellPlate = null;
                cboOptionWellPlate.EditValue = null;
                cboOptionWellPlate.Enabled = true;
                if (lstDataWellPlate != null && lstDataWellPlate.Count == 1)
                {
                    cboOptionWellPlate.EditValue = lstDataWellPlate.First().ID;
                    SetDataToInit(Int64.Parse(cboOptionWellPlate.EditValue.ToString()));
                    btnSave.Enabled = true;
                    lcibtnSave.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                if (currentLisWellPlate == null)
                {
                    btnFinishPutting.Enabled = false;
                    btnUnFinishPutting.Enabled = false;
                    btnFinishResulting.Enabled = false;
                    btnUnFinishResulting.Enabled = false;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboOptionWellPlate()
        {
            try
            {
                lstDataWellPlate = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<LIS_WELL_PLATE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WELL_PLATE_TYPE_CODE", "Mã", 70, 1));
                columnInfos.Add(new ColumnInfo("WELL_PLATE_TYPE_NAME", "Tên", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WELL_PLATE_TYPE_NAME", "ID", columnInfos, true, 220);
                ControlEditorLoader.Load(cboOptionWellPlate, lstDataWellPlate, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();

                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadGridDataLeft(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadGridDataLeft, param, pageSize, gridControl1);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
        }

        private void LoadGridDataLeft(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_LIS_WELL_PLATE>> apiResult = null;
                LIS.Filter.LisWellPlateViewFilter filter = new LIS.Filter.LisWellPlateViewFilter();
                SetFilter(ref filter);
                gridView1.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_LIS_WELL_PLATE>>("api/LisWellPlate/GetView", ApiConsumers.LisConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    lstWellPlateLeft = apiResult.Data;
                    if (lstWellPlateLeft != null && lstWellPlateLeft.Count > 0)
                    {
                        gridControl1.DataSource = lstWellPlateLeft;
                    }
                    else
                    {
                        gridControl1.DataSource = null;

                    }
                    rowCount = (lstWellPlateLeft == null ? 0 : lstWellPlateLeft.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref LIS.Filter.LisWellPlateViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                filter.WELL_PLATE_CODE__EXACT = txtSearchWellPlateCode.Text.Trim();
                filter.KEY_WORD = txtSearch.Text.Trim();
                if (dtCreateFrom.EditValue != null && dtCreateFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                if (dtCreateTo.EditValue != null && dtCreateTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTo.EditValue).ToString("yyyyMMdd") + "235959");

                if (cboStatus.EditValue != null)
                {
                    if (cboStatus.SelectedIndex == 0)
                    {
                        filter.WELL_PLATE_STATUS = 1;
                    }
                    else if (cboStatus.SelectedIndex == 1)
                    {
                        filter.WELL_PLATE_STATUS = 2;
                    }
                    else if (cboStatus.SelectedIndex == 2)
                    {
                        filter.WELL_PLATE_STATUS = 3;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_LIS_WELL_PLATE)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "STATUS")
                    {
                        if (data.WELL_PLATE_STATUS == 1)
                        {
                            e.Value = imageList1.Images[0];
                        }
                        else if (data.WELL_PLATE_STATUS == 2)
                        {
                            e.Value = imageList1.Images[1];
                        }
                        else if (data.WELL_PLATE_STATUS == 3)
                        {
                            e.Value = imageList1.Images[2];
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_LIS_WELL_PLATE data = (V_LIS_WELL_PLATE)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.RowHandle];

                    if (e.Column.FieldName == "DELETE")
                    {
                        if (data.WELL_PLATE_STATUS == 1)
                            e.RepositoryItem = btnEditDeleteEnable;
                        else
                            e.RepositoryItem = btnEditDeleteDisable;


                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEditDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_LIS_WELL_PLATE)gridView1.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
                if (row != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có chắc muốn xóa thông tin giếng không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        WaitingManager.Show();
                        if (row != null)
                        {
                            var rs = new BackendAdapter(param).Post<bool>("api/LisWellPlate/Delete", ApiConsumers.LisConsumer, row.ID, param);
                            if (rs)
                            {
                                FillDataToGrid();
                                WaitingManager.Hide();
                            }
                            WaitingManager.Hide();
                            MessageManager.Show(this.ParentForm, param, rs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValidateNote()
        {
            try
            {
                ValidMaxLength valid = new ValidMaxLength();
                valid.txtEdit = txtNote;
                valid.maxlength = 1000;
                dxValidationProvider1.SetValidationRule(txtNote, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValidateCbo()
        {
            try
            {
                ValidNull valid = new ValidNull();
                valid.cbo = cboOptionWellPlate;
                dxValidationProvider1.SetValidationRule(cboOptionWellPlate, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                currentLisWellPlate = (V_LIS_WELL_PLATE)gridView1.GetFocusedRow();
                if (currentLisWellPlate != null)
                {
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                    SetDataPlateRight();
                    SetDataToInit(currentLisWellPlate.WELL_PLATE_TYPE_ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        //-----------------------------------------------------------------------------------------------
        private void SetDataPlateRight()
        {
            try
            {
                if (currentLisWellPlate != null)
                {
                    txtWellPlateCode.Text = currentLisWellPlate.WELL_PLATE_CODE;
                    txtNote.Text = currentLisWellPlate.NOTE;
                    cboOptionWellPlate.EditValue = currentLisWellPlate.WELL_PLATE_TYPE_ID;
                    cboOptionWellPlate.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableControl()
        {
            try
            {
                bool enable = false;
                if (currentLisWellPlate != null && (currentLisWellPlate.WELL_PLATE_STATUS == 1 || currentLisWellPlate.WELL_PLATE_STATUS == null))
                {
                    enable = true;
                    lcibtnSave.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    btnSave.Enabled = true;
                }
                else if (currentLisWellPlate != null && (currentLisWellPlate.WELL_PLATE_STATUS == 2 || currentLisWellPlate.WELL_PLATE_STATUS == 3))
                {
                    enable = false;
                    btnSave.Enabled = false;
                    lcibtnSave.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (gridView2.Columns.Count > 0)
                {

                    foreach (DevExpress.XtraGrid.Columns.GridColumn item in gridView2.Columns)
                    {
                        item.OptionsColumn.AllowEdit = enable;
                    }
                }

                if (currentLisWellPlate != null && (currentLisWellPlate.WELL_PLATE_STATUS == 2 || currentLisWellPlate.WELL_PLATE_STATUS == 3))
                {
                    lcibtnSave.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (currentLisWellPlate != null && currentLisWellPlate.WELL_PLATE_STATUS == 1)
                {
                    btnFinishPutting.Enabled = true;
                }
                else
                {
                    btnFinishPutting.Enabled = false;
                }
                if (currentLisWellPlate != null && currentLisWellPlate.WELL_PLATE_STATUS == 2)
                {
                    btnUnFinishPutting.Enabled = true;
                    btnFinishResulting.Enabled = true;
                }
                else
                {
                    btnUnFinishPutting.Enabled = false;
                    btnFinishResulting.Enabled = false;
                }
                if (currentLisWellPlate != null && currentLisWellPlate.WELL_PLATE_STATUS == 3)
                {
                    btnUnFinishResulting.Enabled = true;
                }
                else
                {
                    btnUnFinishResulting.Enabled = false;
                }               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToInit(long id)
        {
            try
            {
                dicData = new Dictionary<string, HeaderADO>();
                lstDataSource = new List<ExpandoObject>();
                lstWellPlateDetails = new List<LIS.EFMODEL.DataModels.V_LIS_WELL_PLATE_DETAIL>();
                var dta = lstDataWellPlate.Where(o => o.ID == id).First();
                rows = dta.NUMBER_OF_ROW + 1;
                columns = dta.NUMBER_OF_COLUMN + 1;
                CommonParam param = new CommonParam();
                if (currentLisWellPlate != null)
                {
                    LisWellPlateDetailViewFilter filter = new LisWellPlateDetailViewFilter();
                    filter.WELL_PLATE_ID = currentLisWellPlate.ID;
                    lstWellPlateDetails = new BackendAdapter(param).Get<List<V_LIS_WELL_PLATE_DETAIL>>("api/LisWellPlateDetail/GetView", ApiConsumers.LisConsumer, filter, param);
                }
                List<LIS_SAMPLE> lstSample = new List<LIS.EFMODEL.DataModels.LIS_SAMPLE>();
                if (lstWellPlateDetails != null && lstWellPlateDetails.Count > 0)
                {
                    LisSampleFilter sampleFilter = new LisSampleFilter();
                    sampleFilter.IDs = lstWellPlateDetails.Select(o => o.SAMPLE_ID).ToList();
                    lstSample = new BackendAdapter(param).Get<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumers.LisConsumer, sampleFilter, param);
                }
                lstHeaders = new List<HeaderADO>();
                lstHeaderString = new List<string>();
                List<string> alphabet = new List<string>();
                for (char letter = 'A'; letter <= 'Z'; letter++)
                {
                    alphabet.Add(letter.ToString());
                }
                for (int i = 1; i < rows; i++)
                {
                    ExpandoObject obj = new ExpandoObject();
                    AddProperty(obj, "Alphabet", alphabet[i - 1]);
                    for (int j = 1; j < columns; j++)
                    {
                        int X = j;
                        int Y = i;
                        string barCode = "";
                        HeaderADO ado = new HeaderADO();
                        ado.X = X;
                        ado.Y = Y;
                        if (lstWellPlateDetails != null && lstWellPlateDetails.Count > 0)
                        {
                            var checkWellPlateDetail = lstWellPlateDetails.Where(o => o.X == X && o.Y == Y).ToList();
                            if (checkWellPlateDetail != null && checkWellPlateDetail.Count > 0)
                            {
                                var itemDetail = checkWellPlateDetail.First();
                                ado.SERVICE_RESULT_ID = itemDetail.SERVICE_RESULT_ID;
                                ado.FIRST_NAME = itemDetail.FIRST_NAME;
                                ado.LAST_NAME = itemDetail.LAST_NAME;
                                ado.GENDER_NAME = itemDetail.GENDER_NAME;
                                ado.SAMPLE_ID = itemDetail.SAMPLE_ID;
                                ado.DETAIL_ID = itemDetail.ID;
                                if (itemDetail.IS_HAS_NOT_DAY_DOB == 1)
                                {
                                    ado.DOB = itemDetail.DOB.ToString().Substring(0, 4);
                                }
                                else
                                {
                                    ado.DOB = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemDetail.DOB ?? 0);
                                }
                                if (lstSample != null && lstSample.Count > 0)
                                {
                                    var checkSample = lstSample.Where(o => o.ID == itemDetail.SAMPLE_ID).First();
                                    if (checkSample != null)
                                    {
                                        barCode = checkSample.BARCODE;
                                        ado.BARCODE = barCode;
                                    }

                                }
                            }
                        }
                        dicData.Add(i.ToString() + (j).ToString(), ado);
                        AddProperty(obj, j.ToString(), barCode);
                    }

                    lstDataSource.Add(obj);
                }
                InitGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitGrid()
        {
            try
            {
                ClearGrid();
                gridControl2.DataSource = null;
                gridControl2.DataSource = lstDataSource;
                gridControl2.RefreshDataSource();
                if (gridView2.Columns.Count > 0)
                {
                    gridView2.Columns[0].Caption = " ";
                    gridView2.Columns[0].Width = 5;
                    gridView2.Columns[0].AppearanceCell.Font = new Font(gridView2.Columns[0].AppearanceCell.Font, FontStyle.Bold);
                    gridView2.Columns[0].OptionsColumn.AllowEdit = false;
                    gridView2.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridView2.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridView2.Appearance.HeaderPanel.Font = new Font(gridView2.Appearance.HeaderPanel.Font, FontStyle.Bold);
                    gridView2.Appearance.Row.Font = new Font(gridView2.Appearance.HeaderPanel.Font, FontStyle.Bold);
                    gridView2.RowHeight = 50;
                    foreach (DevExpress.XtraGrid.Columns.GridColumn item in gridView2.Columns)
                    {
                        item.OptionsColumn.AllowEdit = false;
                    }
                }
                EnableControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClearGrid()
        {
            try
            {
                gridView2.Columns.Clear();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName != "Alphabet")
                    {
                        e.RepositoryItem = repositoryTxt_Ena;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryTxt_Ena_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                HeaderADO data = dicData[(gridView2.FocusedRowHandle + 1) + (gridView2.FocusedColumn.VisibleIndex).ToString()] as HeaderADO;
                if (data != null)
                {

                    TextEdit txt = sender as TextEdit;

                    if (txt.Text != null && txt.Text != "")
                    {
                        data.BARCODE = txt.Text;
                        oldX = gridView2.FocusedColumn.VisibleIndex;
                        oldY = gridView2.FocusedRowHandle;
                    }
                    else
                    {
                        data.BARCODE = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControl2_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                visibleIndex = gridView2.FocusedColumn.VisibleIndex;
                newRow = gridView2.FocusedRowHandle;
                
                if (e.KeyCode == Keys.Enter)
                {
                    ActionRowCell(newRow, visibleIndex, null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ActionRowCell(int newRow, int visibleIndex, int? numberKey)
        {
            try
            {
                var dta = dicData[(newRow + 1) + (visibleIndex).ToString()] as HeaderADO;
                if (dta != null)
                {
                    if (currentLisWellPlate != null && currentLisWellPlate.WELL_PLATE_STATUS == 1)
                    {
                        if (string.IsNullOrEmpty(dta.BARCODE))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa nhập mã vạch mẫu bệnh phẩm", "Thông báo");
                            gridView1.FocusedColumn = gridView1.VisibleColumns[visibleIndex];

                        }
                        else
                        {
                            CommonParam param = new CommonParam();
                            bool success = false;
                            LisWellPlateDetailSetSDO sdo = new LisWellPlateDetailSetSDO();
                            sdo.WellPlateId = currentLisWellPlate.ID;
                            sdo.Barcode = dta.BARCODE;
                            sdo.X = dta.X ?? 0;
                            sdo.Y = dta.Y ?? 0;
                            var resultData = new BackendAdapter(param).Post<LIS_WELL_PLATE_DETAIL>("api/LisWellPlateDetail/Set", ApiConsumers.LisConsumer, sdo, param);
                            if (resultData != null)
                            {
                                success = true;
                                LisWellPlateDetailViewFilter filter = new LisWellPlateDetailViewFilter();
                                filter.ID = resultData.ID;
                                var vResult = new BackendAdapter(param).Get<List<V_LIS_WELL_PLATE_DETAIL>>("api/LisWellPlateDetail/GetView", ApiConsumers.LisConsumer, filter, param);
                                if (vResult != null && vResult.Count > 0)
                                {
                                    var item = vResult.First();
                                    dicData[(newRow + 1) + (visibleIndex).ToString()].SERVICE_RESULT_ID = item.SERVICE_RESULT_ID;
                                    dicData[(newRow + 1) + (visibleIndex).ToString()].FIRST_NAME = item.FIRST_NAME;
                                    dicData[(newRow + 1) + (visibleIndex).ToString()].LAST_NAME = item.LAST_NAME;
                                    dicData[(newRow + 1) + (visibleIndex).ToString()].GENDER_NAME = item.GENDER_NAME;
                                    dicData[(newRow + 1) + (visibleIndex).ToString()].SAMPLE_ID = item.SAMPLE_ID;
                                    dicData[(newRow + 1) + (visibleIndex).ToString()].DETAIL_ID = item.ID;
                                    if (item.IS_HAS_NOT_DAY_DOB == 1)
                                    {
                                        dicData[(newRow + 1) + (visibleIndex).ToString()].DOB = item.DOB.ToString().Substring(0, 4);
                                    }
                                    else
                                    {
                                        dicData[(newRow + 1) + (visibleIndex).ToString()].DOB = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.DOB ?? 0);
                                    }
                                }
                                switch (numberKey)
                                {
                                    case 1:
                                    case 2:
                                        visibleIndex = visibleIndex - 1;
                                        break;
                                    case 3:
                                    case 4:
                                        newRow = newRow - 1;
                                        break;
                                }
                                NextFocusGrid(newRow, visibleIndex + 1);
                            }
                            else
                            {
                                dicData[(newRow + 1) + (visibleIndex).ToString()].BARCODE = "";
                                SetDataToInit(currentLisWellPlate.WELL_PLATE_TYPE_ID);
                                switch (numberKey)
                                {
                                    case 1:
                                        visibleIndex = visibleIndex + 1;
                                        break;
                                    case 2:
                                        visibleIndex = visibleIndex - 1;
                                        break;
                                    case 3:
                                        newRow = newRow - 1;
                                        break;
                                    case 4:
                                        newRow = newRow + 1;
                                        break;
                                }
                                gridView2.FocusedRowHandle = newRow;
                                gridView2.FocusedColumn = gridView2.VisibleColumns[visibleIndex];
                            }
                            WaitingManager.Hide();
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        }
                    }
                    else if (currentLisWellPlate != null && (currentLisWellPlate.WELL_PLATE_STATUS == 2 || currentLisWellPlate.WELL_PLATE_STATUS == 3) && !string.IsNullOrEmpty(dta.BARCODE))
                    {
                        currentX = dta.X;
                        currentY = dta.Y;
                        frmServiceResult frm = new frmServiceResult(currentModule, dta.SERVICE_RESULT_ID, currentLisWellPlate, dta.SAMPLE_ID, GetBoolean);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextFocusGrid(int newRow, int visibleIndex)
        {
            try
            {
                newVisibleIndex = visibleIndex;
                if (newVisibleIndex == gridView2.VisibleColumns.Count)
                {
                    newVisibleIndex = 1;
                    newRow = gridView2.FocusedRowHandle + 1;
                    if (newRow == gridView2.DataRowCount)
                    {
                        newVisibleIndex = 1;
                        newRow = 0;
                    }
                }

                gridView2.FocusedColumn = gridView2.VisibleColumns[newVisibleIndex];
                gridView2.FocusedRowHandle = newRow;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetBoolean(long? serviceResultId)
        {
            try
            {
                if (serviceResultId != null && serviceResultId > 0)
                {
                    dicData[currentY.ToString() + currentX.ToString()].SERVICE_RESULT_ID = serviceResultId;
                    //gridControl2.RefreshDataSource();
                    NextFocusGrid((currentY ?? 0 ) - 1, (currentX ?? 0) + 1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            try
            {
                var expandoDict = expando as IDictionary<string, object>;
                expandoDict[propertyName] = propertyValue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    ExpandoObject data = (ExpandoObject)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName != "Alphabet")
                        {
                            if (dicData.ContainsKey((e.RowHandle + 1) + (Int64.Parse(e.Column.FieldName)).ToString()) && dicData[(e.RowHandle + 1) + (Int64.Parse(e.Column.FieldName)).ToString()] is HeaderADO)
                            {
                                HeaderADO h = dicData[(e.RowHandle + 1) + (Int64.Parse(e.Column.FieldName)).ToString()] as HeaderADO;
                                if (h != null)
                                {
                                    switch (h.SERVICE_RESULT_ID)
                                    {
                                        case IMSys.DbConfig.LIS_RS.LIS_SERVICE_RESULT.ID__DUONG_TINH:
                                            e.Appearance.ForeColor = Color.Red;
                                            break;
                                        case IMSys.DbConfig.LIS_RS.LIS_SERVICE_RESULT.ID__AM_TINH:
                                            e.Appearance.ForeColor = Color.Green;
                                            break;
                                        case null:
                                            e.Appearance.ForeColor = Color.Black;
                                            break;
                                        default:
                                            e.Appearance.ForeColor = Color.Black;
                                            break;
                                    }
                                }
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

        private void gridView2_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                //HeaderADO header = dicData[e.Column.FieldName + e.RowHandle] as HeaderADO;
                //header.BARCODE = e.Value.ToString();
                //dicData[e.Column.FieldName + e.RowHandle] = header;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

        }

        private void gridView2_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DXMouseEventArgs ea = e as DXMouseEventArgs;
                GridView view = sender as GridView;
                GridHitInfo info = view.CalcHitInfo(ea.Location);
                if (info.InRow || info.InRowCell)
                {
                    visibleIndex = gridView2.FocusedColumn.VisibleIndex;
                    newVisibleIndex = visibleIndex + 1;
                    newRow = gridView2.FocusedRowHandle;
                    ActionRowCell(newRow, visibleIndex, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueRight();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOptionWellPlate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboOptionWellPlate.EditValue != null)
                {
                    SetDataToInit(Int64.Parse(cboOptionWellPlate.EditValue.ToString()));
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFinishPutting_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFinishPutting.Enabled)
                    return;



                bool success = false;
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param).Post<LIS_WELL_PLATE>("api/LisWellPlate/FinishPutting", ApiConsumers.LisConsumer, currentLisWellPlate.ID, param);
                if (result != null)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<LIS_WELL_PLATE, V_LIS_WELL_PLATE>();
                    currentLisWellPlate = AutoMapper.Mapper.Map<V_LIS_WELL_PLATE>(result);
                    foreach (var item in lstWellPlateLeft)
                    {
                        if (item.ID == currentLisWellPlate.ID)
                            item.WELL_PLATE_STATUS = currentLisWellPlate.WELL_PLATE_STATUS;
                    }
                    gridControl1.RefreshDataSource();
                    EnableControl();
                    WaitingManager.Hide();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnFinishPutting_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnUnFinishPutting.Enabled)
                    return;
                bool success = false;
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param).Post<LIS_WELL_PLATE>("api/LisWellPlate/UnfinishPutting", ApiConsumers.LisConsumer, currentLisWellPlate.ID, param);
                if (result != null)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<LIS_WELL_PLATE, V_LIS_WELL_PLATE>();
                    currentLisWellPlate = AutoMapper.Mapper.Map<V_LIS_WELL_PLATE>(result);
                    foreach (var item in lstWellPlateLeft)
                    {
                        if (item.ID == currentLisWellPlate.ID)
                            item.WELL_PLATE_STATUS = currentLisWellPlate.WELL_PLATE_STATUS;
                    }
                    gridControl1.RefreshDataSource();
                    EnableControl();
                    WaitingManager.Hide();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFinishResulting_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFinishResulting.Enabled)
                    return;
                ListResult = dicData.Values.Where(o => o.DETAIL_ID > 0 && o.SERVICE_RESULT_ID != null).ToList();
                ListNull = dicData.Values.Where(o => o.DETAIL_ID > 0 && o.SERVICE_RESULT_ID == null).ToList();
                if (ListNull != null && ListNull.Count > 0)
                {
                    frmMessage frm = new frmMessage(GetAction);
                    frm.ShowDialog();
                }
                else
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    var result = new BackendAdapter(param).Post<LIS_WELL_PLATE>("api/LisWellPlate/FinishResulting", ApiConsumers.LisConsumer, currentLisWellPlate.ID, param);
                    if (result != null)
                    {
                        success = true;
                        AutoMapper.Mapper.CreateMap<LIS_WELL_PLATE, V_LIS_WELL_PLATE>();
                        currentLisWellPlate = AutoMapper.Mapper.Map<V_LIS_WELL_PLATE>(result);
                        foreach (var item in lstWellPlateLeft)
                        {
                            if (item.ID == currentLisWellPlate.ID)
                                item.WELL_PLATE_STATUS = currentLisWellPlate.WELL_PLATE_STATUS;
                        }
                        gridControl1.RefreshDataSource();
                        SetDataToInit(currentLisWellPlate.WELL_PLATE_TYPE_ID);
                        WaitingManager.Hide();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetAction(EnumOption option)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                switch (option)
                {
                    case EnumOption.AUTO:
                        LisSampleResultListSDO lstSdo = new LisSampleResultListSDO();
                        lstSdo.ResultTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                        lstSdo.SampleIds = ListNull.Select(o => o.SAMPLE_ID ?? 0).ToList();
                        lstSdo.ServiceResultId = IMSys.DbConfig.LIS_RS.LIS_SERVICE_RESULT.ID__AM_TINH;
                        lstSampleResult = new BackendAdapter(param).Post<List<LIS_SAMPLE>>("api/LisSample/UpdateResultList", ApiConsumers.LisConsumer, lstSdo, param);
                        if (lstSampleResult != null && lstSampleResult.Count > 0)
                        {
                            //SetDataToInit(currentLisWellPlate.WELL_PLATE_TYPE_ID);

                            //foreach (var item in ListNull)
                            //{
                            //    dicData[(item.Y - 1).ToString() + item.X.ToString()].SERVICE_RESULT_ID = IMSys.DbConfig.LIS_RS.LIS_SERVICE_RESULT.ID__AM_TINH;
                            //}
                            //gridControl2.RefreshDataSource();
                            //success = true;
                            CallApiFinishResulting(ref param, ref success);
                            WaitingManager.Hide();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                        break;
                    case EnumOption.CONDITION:
                        CallApiFinishResulting(ref param, ref success);
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                        break;
                    case EnumOption.NONE:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallApiFinishResulting(ref CommonParam param, ref bool success)
        {
            try
            {
                var result = new BackendAdapter(param).Post<LIS_WELL_PLATE>("api/LisWellPlate/FinishResulting", ApiConsumers.LisConsumer, currentLisWellPlate.ID, param);
                if (result != null)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<LIS_WELL_PLATE, V_LIS_WELL_PLATE>();
                    currentLisWellPlate = AutoMapper.Mapper.Map<V_LIS_WELL_PLATE>(result);
                    foreach (var item in lstWellPlateLeft)
                    {
                        if (item.ID == currentLisWellPlate.ID)
                            item.WELL_PLATE_STATUS = currentLisWellPlate.WELL_PLATE_STATUS;
                    }
                    gridControl1.RefreshDataSource();
                    SetDataToInit(currentLisWellPlate.WELL_PLATE_TYPE_ID);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnFinishResulting_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnUnFinishResulting.Enabled)
                    return;
                bool success = false;
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param).Post<LIS_WELL_PLATE>("api/LisWellPlate/UnfinishResulting", ApiConsumers.LisConsumer, currentLisWellPlate.ID, param);
                if (result != null)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<LIS_WELL_PLATE, V_LIS_WELL_PLATE>();
                    currentLisWellPlate = AutoMapper.Mapper.Map<V_LIS_WELL_PLATE>(result);
                    foreach (var item in lstWellPlateLeft)
                    {
                        if (item.ID == currentLisWellPlate.ID)
                            item.WELL_PLATE_STATUS = currentLisWellPlate.WELL_PLATE_STATUS;
                    }
                    gridControl1.RefreshDataSource();
                    SetDataToInit(currentLisWellPlate.WELL_PLATE_TYPE_ID);
                    WaitingManager.Hide();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnSave.Enabled || !lcibtnSave.Visible)
                    return;
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                if (currentLisWellPlate == null)
                {
                    LisWellPlateCreateSDO sdoCr = new LisWellPlateCreateSDO();
                    sdoCr.Note = txtNote.Text;
                    sdoCr.WellPlateTypeId = Int64.Parse(cboOptionWellPlate.EditValue.ToString());
                    Inventec.Common.Logging.LogSystem.Debug("INPUT___CRE__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdoCr), sdoCr));
                    var resultData = new BackendAdapter(param).Post<LIS_WELL_PLATE>("api/LisWellPlate/CreateSdo", ApiConsumers.LisConsumer, sdoCr, param);
                    if (resultData != null)
                    {
                        success = true;
                        AutoMapper.Mapper.CreateMap<LIS_WELL_PLATE, V_LIS_WELL_PLATE>();
                        currentLisWellPlate = AutoMapper.Mapper.Map<V_LIS_WELL_PLATE>(resultData);
                    }
                }
                else
                {
                    LisWellPlateUpdateSDO sdoUp = new LisWellPlateUpdateSDO();
                    sdoUp.Note = txtNote.Text;
                    sdoUp.WellPlateId = currentLisWellPlate.ID;
                    sdoUp.WellPlateTypeId = Int64.Parse(cboOptionWellPlate.EditValue.ToString());
                    Inventec.Common.Logging.LogSystem.Debug("INPUT___CRE__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdoUp), sdoUp));
                    var resultData = new BackendAdapter(param).Post<LIS_WELL_PLATE>("api/LisWellPlate/UpdateSdo", ApiConsumers.LisConsumer, sdoUp, param);
                    if (resultData != null)
                    {
                        success = true;
                        AutoMapper.Mapper.CreateMap<LIS_WELL_PLATE, V_LIS_WELL_PLATE>();
                        currentLisWellPlate = AutoMapper.Mapper.Map<V_LIS_WELL_PLATE>(resultData);
                    }
                }
                if (success)
                {
                    SetDataPlateRight();
                    EnableControl();
                    FillDataToGrid();
                }


                WaitingManager.Hide();
                #region Hien thi message thong bao
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region TOOLTIP
        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {

                if (e.Info == null && e.SelectedControl == gridControl1)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl1.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "STATUS")
                            {
                                long? isConfirm = null;
                                if (!String.IsNullOrWhiteSpace((view.GetRowCellValue(lastRowHandle, "WELL_PLATE_STATUS") ?? "").ToString()))
                                {
                                    isConfirm = Convert.ToInt64((view.GetRowCellValue(lastRowHandle, "WELL_PLATE_STATUS") ?? "").ToString());
                                }
                                if (isConfirm == 1)
                                {
                                    text = "Tạo mới";
                                }
                                else if (isConfirm == 2)
                                {
                                    text = "Hoàn thành đặt mẫu";
                                }
                                else if (isConfirm == 3)
                                {
                                    text = "Kết thúc đọc kết quả";
                                }
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController2_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl2)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl2.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName != "Alphabet")
                            {
                                var dt = dicData[(lastRowHandle + 1) + (Int64.Parse(info.Column.FieldName)).ToString()] as HeaderADO;
                                if (dt != null)
                                {
                                    if (currentLisWellPlate != null && currentLisWellPlate.WELL_PLATE_STATUS == 1)
                                    {
                                        if (!string.IsNullOrEmpty(dt.BARCODE) && dt.SAMPLE_ID != null && dt.SAMPLE_ID > 0)
                                            text = string.Format("{0} {1} - {2} - {3}", dt.FIRST_NAME, dt.LAST_NAME, dt.GENDER_NAME, dt.DOB);
                                    }
                                    else if (currentLisWellPlate != null && (currentLisWellPlate.WELL_PLATE_STATUS == 2 || currentLisWellPlate.WELL_PLATE_STATUS == 3))
                                    {
                                        if (dt.DETAIL_ID != 0)
                                        {
                                            if (dt.SERVICE_RESULT_ID == IMSys.DbConfig.LIS_RS.LIS_SERVICE_RESULT.ID__DUONG_TINH)
                                            {
                                                text = "Dương tính";
                                            }
                                            else if (dt.SERVICE_RESULT_ID == IMSys.DbConfig.LIS_RS.LIS_SERVICE_RESULT.ID__AM_TINH)
                                            {
                                                text = "Âm tính";
                                            }
                                            else if (dt.SERVICE_RESULT_ID == IMSys.DbConfig.LIS_RS.LIS_SERVICE_RESULT.ID__NGHI_NGO_DUONG_TINH)
                                            {
                                                text = "Nghi ngờ dương tính";
                                            }
                                            else
                                            {
                                                text = "Chưa trả kết quả";
                                            }
                                        }
                                    }
                                }
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region ShortCut
        public void bbtnFindx()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void bbtnSavex()
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void bbtnNewx()
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void bbtnKTDMx()
        {
            try
            {
                btnFinishPutting_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void bbtnKTGMx()
        {
            try
            {
                btnFinishResulting_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void repositoryTxt_Ena_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(dicData[(oldY + 1).ToString() + oldX].BARCODE)
                    && dicData[(oldY + 1).ToString() + oldX].DETAIL_ID <= 0
                    && currentLisWellPlate != null
                    && currentLisWellPlate.WELL_PLATE_STATUS == 1)
                {
                    ActionRowCell(oldY, oldX, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}


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
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.VitaminA.ADO;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.VitaminA.Execute;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.IsAdmin;

namespace HIS.Desktop.Plugins.VitaminA
{
    public partial class UCVitaminA : UserControl
    {
        long roomId;
        long roomTypeId;
        int rowCount = 0;
        int dataTotal = 0;
        int numPageSize;
        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        List<HisMedicineTypeADO> medicineTypeADOs { get; set; }

        public UCVitaminA(long roomId, long roomTypeId)
        {
            InitializeComponent();
            try
            {
                this.roomId = roomId;
                this.roomTypeId = roomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCVitaminA_Load(object sender, EventArgs e)
        {
            try
            {
                InitDataControl();
                FillDataToGridVitaminA();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FindEnabled(false);
                FillDataToGridVitaminA();
                FindEnabled(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FindEnabled(bool enabled)
        {
            btnFind.Enabled = enabled;
        }

        private void gridViewVitaminA_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Selection" || e.Column.FieldName == "EXECUTE" || e.Column.FieldName == "ADD")
                    return;

                V_HIS_VITAMIN_A vitaminAChoose = gridViewVitaminA.GetFocusedRow() as V_HIS_VITAMIN_A;
                if (vitaminAChoose != null)
                {
                    LoadInfoVitaminAPatient(vitaminAChoose);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVitaminA_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_VITAMIN_A dataRow = (V_HIS_VITAMIN_A)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "TRANGTHAI_IMG")
                    {
                        //Chua uong: mau trang
                        //Da uong: mau cam

                        if (!dataRow.EXECUTE_TIME.HasValue)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                        else
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                    }

                    else if (e.Column.FieldName == "REQUEST_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.REQUEST_TIME);
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY")
                    {
                        if (dataRow.EXECUTE_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXECUTE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        if (dataRow.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            e.Value = dataRow.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        else
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.TDL_PATIENT_DOB.ToString());
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVitaminA_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_VITAMIN_A data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewVitaminA.GetDataSourceRowIndex(e.RowHandle);
                    data = (V_HIS_VITAMIN_A)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (e.Column.FieldName == "CANCEL")
                        {
                            if (!data.EXP_MEST_ID.HasValue && data.EXECUTE_TIME.HasValue && (data.EXECUTE_USERNAME == loginName 
                                || !CheckLoginAdmin.IsAdmin(loginName)))
                            {
                                e.RepositoryItem = repositoryItemButtonEditCancel;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditCancel__Disable;
                            }
                        }
                        else if (e.Column.FieldName == "EXECUTE")
                        {
                            if (!data.EXP_MEST_ID.HasValue
                                || String.IsNullOrEmpty(data.EXECUTE_USERNAME)
                                || (!String.IsNullOrEmpty(data.EXECUTE_USERNAME) && (data.EXECUTE_USERNAME == loginName || !CheckLoginAdmin.IsAdmin(loginName))))
                            {
                                e.RepositoryItem = repositoryItemButtonEditExecute;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditExecute_Disabled;
                            }
                        }
                        else if (e.Column.FieldName == "ADD")
                        {
                            if (data.EXP_MEST_ID.HasValue || !data.MEDICINE_TYPE_ID.HasValue)
                            {
                                e.RepositoryItem = repositoryItemButtonEditAdd_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditAdd;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlVitaminA)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlVitaminA.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "TRANGTHAI_IMG")
                            {
                                long executeTime = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "EXECUTE_TIME") ?? "").ToString());
                                if (executeTime > 0)
                                {
                                    text = "Đã uống";
                                }
                                else
                                {
                                    text = "Chưa uống";
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVitaminAHistory_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_VITAMIN_A dataRow = (V_HIS_VITAMIN_A)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "REQUEST_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.REQUEST_TIME);
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY")
                    {
                        if (dataRow.EXECUTE_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXECUTE_TIME ?? 0);
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditExecute_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_VITAMIN_A vitaminA = gridViewVitaminA.GetFocusedRow() as V_HIS_VITAMIN_A;
                if (vitaminA != null)
                {
                    frmExecute frm = new frmExecute(vitaminA, ReloadDataExecute);
                    frm.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadDataExecute(V_HIS_VITAMIN_A data)
        {
            try
            {
                if (data != null)
                {
                    List<V_HIS_VITAMIN_A> listVitaminA = gridControlVitaminA.DataSource as List<V_HIS_VITAMIN_A>;
                    foreach (var item in listVitaminA)
                    {
                        if (item.ID == data.ID)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_VITAMIN_A>(item, data);
                            gridControlVitaminA.RefreshDataSource();
                            AddVitaminToExport(item);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_VITAMIN_A vitaminA = gridViewVitaminA.GetFocusedRow() as V_HIS_VITAMIN_A;
                AddVitaminToExport(vitaminA);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddVitaminToExport(V_HIS_VITAMIN_A vitaminA)
        {
            try
            {
                if (vitaminA != null)
                {
                    if (CheckExitGridVitaminExport(vitaminA))
                    {
                        MessageBox.Show(String.Format("Tồn tại yêu cầu này trong danh sách xuất"), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    List<V_HIS_VITAMIN_A> listVitaminA = gridViewVitaminAExport.DataSource as List<V_HIS_VITAMIN_A>;
                    if (listVitaminA == null)
                    {
                        listVitaminA = new List<V_HIS_VITAMIN_A>();
                        listVitaminA.Add(vitaminA);
                        gridControlVitaminAExport.DataSource = listVitaminA;
                    }
                    else
                    {
                        listVitaminA.Add(vitaminA);
                        gridControlVitaminAExport.RefreshDataSource();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        private void repositoryItemButtonEditMinus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_VITAMIN_A vitaminA = gridViewVitaminAExport.GetFocusedRow() as V_HIS_VITAMIN_A;
                List<V_HIS_VITAMIN_A> listVitaminA = gridControlVitaminAExport.DataSource as List<V_HIS_VITAMIN_A>;
                if (vitaminA != null && listVitaminA != null && listVitaminA.Count > 0)
                {
                    listVitaminA.Remove(vitaminA);
                    gridControlVitaminAExport.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                gridControlVitaminAExport.DataSource = null;
                LoadInfoVitaminAPatient(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_VITAMIN_A> listVitaminA = gridControlVitaminAExport.DataSource as List<V_HIS_VITAMIN_A>;
                if (listVitaminA == null || listVitaminA.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy Vitamin A trong danh sách xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpMestVitaminA").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ExpMestVitaminA'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ExpMestVitaminA' is not plugins");
                long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                List<object> listArgs = new List<object>();
                listArgs.Add(listVitaminA);
                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)ReloadExport);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                WaitingManager.Hide();
                ((Form)extenceInstance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadExport(object data)
        {
            try
            {
                if (data!=null)
                {
                    FillDataToGridVitaminA();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditCancel_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_VITAMIN_A vitaminA = gridViewVitaminA.GetFocusedRow() as V_HIS_VITAMIN_A;
                if (vitaminA != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HIS_VITAMIN_A vitaminAInput = new HIS_VITAMIN_A();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_VITAMIN_A>(vitaminAInput, vitaminA);
                    vitaminAInput.EXECUTE_TIME = null;
                    vitaminAInput.MEDICINE_TYPE_ID = null;
                    vitaminAInput.AMOUNT = null;
                    HIS_VITAMIN_A vitaminAResult = new BackendAdapter(param)
                    .Post<HIS_VITAMIN_A>("api/HisVitaminA/Update", ApiConsumers.MosConsumer, vitaminAInput, param);
                    if (vitaminAResult != null)
                    {
                        success = true;
                        vitaminA.EXECUTE_TIME = null;
                        vitaminA.AMOUNT = null;
                        vitaminA.MEDICINE_TYPE_ID = null;
                        vitaminA.MEDICINE_TYPE_CODE = null;
                        vitaminA.MEDICINE_TYPE_NAME = null;
                        gridControlVitaminA.RefreshDataSource();
                    }
                    gridViewVitaminA.FocusedColumn = gridViewVitaminA.Columns[0];
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVitaminAExport_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_VITAMIN_A dataRow = (V_HIS_VITAMIN_A)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }

                    else if (e.Column.FieldName == "REQUEST_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.REQUEST_TIME);
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY" && dataRow.EXECUTE_TIME.HasValue)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXECUTE_TIME.Value);
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        if (dataRow.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            e.Value = dataRow.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        else
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.TDL_PATIENT_DOB.ToString());
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVitaminAHistory_CustomUnboundColumnData_1(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_VITAMIN_A dataRow = (V_HIS_VITAMIN_A)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }

                    else if (e.Column.FieldName == "REQUEST_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.REQUEST_TIME);
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY")
                    {
                        if (dataRow.EXECUTE_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXECUTE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        if (dataRow.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            e.Value = dataRow.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        else
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.TDL_PATIENT_DOB.ToString());
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVitaminAHistory_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_VITAMIN_A data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewVitaminA.GetDataSourceRowIndex(e.RowHandle);
                    data = (V_HIS_VITAMIN_A)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IS_SICK_DISPLAY")
                        {
                            e.RepositoryItem = data.IS_SICK == 1 ? repositoryItemButtonEditTick : null;
                        }
                        else if (e.Column.FieldName == "IS_ONE_MONTH_BORN_DISPLAY")
                        {
                            e.RepositoryItem = data.IS_ONE_MONTH_BORN == 1 ? repositoryItemButtonEditTick : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

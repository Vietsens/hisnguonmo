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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.UpdateResponsibilityOfRoom
{
    public partial class frmUpdateResponsibilityOfRoom : FormBase
    {
        #region Declare
        //int rowCount = 0;
        //int dataTotal = 0;
        //int startPage = 0;
        int start = 0;
        //PagingGrid pagingGrid;
        //int ActionType = -1;
        //int positionHandle = -1;
        //Inventec.Desktop.Common.Modules.Module currentModule;
        internal List<V_HIS_ROOM> lstResponsibilityOfRoomTemp { get; set; }
        internal List<HIS_USER_ROOM> _UserRoom { get; set; }
        internal List<HIS_DEPARTMENT> departments { get; set; }
        internal List<V_HIS_ROOM> rooms { get; set; }
        internal V_HIS_EXECUTE_ROOM executeRoom { get; set; }
        bool isNotHandlerWhileChangeToggetSwith;
        #endregion
        public frmUpdateResponsibilityOfRoom(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                //pagingGrid = new PagingGrid();
                //this.moduleData = moduleData;
                //gridControl2.ToolTipController = toolTipControllerGrid;
                //SetIcon();
                //try
                //{
                //    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                //    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                //}
                //catch (Exception ex)
                //{
                //    LogSystem.Warn(ex);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmUpdateResponsibilityOfRoom_Load(object sender, EventArgs e)
        {
            MeShow();
        }

        private void MeShow()
        {
            try
            {
                InitComboDoctor();
                //Load Combo Khoa
                LoadDepartment();
                //Load du lieu
                LoaddataToGridView();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboDoctor(long roomId)
        {
            try
            {
                this._UserRoom = new List<HIS_USER_ROOM>();
                MOS.Filter.HisUserRoomFilter filter = new HisUserRoomFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ROOM_ID = roomId;
                this._UserRoom = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_ROOM>>("api/HisUserRoom/Get", ApiConsumers.MosConsumer, filter, null);
                var loginname = _UserRoom.Select(o => o.LOGINNAME).ToList();
               

                //BackendDataWorker.Get<HIS_USER_ROOM>().Where(o => o.ROOM_ID == roomId).Select(o => o.LOGINNAME).ToList();

                var users = BackendDataWorker.Get<ACS_USER>().Where(o => loginname.Contains(o.LOGINNAME) && this._UserRoom.Exists(p => p.LOGINNAME == o.LOGINNAME)).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 75, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 225, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 300);
                ControlEditorLoader.Load(grdResponseDoctor, users, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDoctor()
        {
            try
            {
                //var loginname = //_UserRoom.Select(o => o.LOGINNAME).ToList();
                //    //BackendDataWorker.Get<HIS_USER_ROOM>().Where(o => o.ROOM_ID == roomId).Select(o => o.LOGINNAME).ToList();
                //BackendDataWorker.Get<HIS_USER_ROOM>().Select(o => o.LOGINNAME).ToList();

                var users = BackendDataWorker.Get<ACS_USER>();
                //var users = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonNumberTrue).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 75, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 225, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 300);
                ControlEditorLoader.Load(grdResponseDoctor, users, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hiển thị dữ liệu của các khoa lâm sàng, khám bệnh hoặc cấp cứu: HIS_DEPARTMENT có
        /// IS_ACTIVE = 1 và (IS_CLINICAL = 1 hoặc IS_EMERGENCY = 1 hoặc IS_EXAM = 1)
        /// </summary>
        private void LoadDepartment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = 1;
                departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
                departments = departments.Where(o => o.IS_ACTIVE == 1 && (o.IS_CLINICAL == 1 || o.IS_EMERGENCY == 1 || o.IS_EXAM == 1)).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(comboDepartment, departments, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoaddataToGridView()
        {
            try
            {
                HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Reset<V_HIS_ROOM>();
                List<V_HIS_ROOM> lstResponsibilityOfRoomResult = new List<V_HIS_ROOM>();
                //CommonParam param = new CommonParam();
                //HisRoomViewFilter filter = new HisRoomViewFilter();
                //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //this.lstResponsibilityOfRoomTemp = new BackendAdapter(param).Get<List<V_HIS_ROOM>>("api/HisRoomView/Get", ApiConsumers.MosConsumer, filter, null);
                var lstResponsibilityOfRoomTemp = BackendDataWorker.Get<V_HIS_ROOM>();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstResponsibilityOfRoomTemp), lstResponsibilityOfRoomTemp));

                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_ROOM, V_HIS_ROOM>();
                var lstResponsibilityOfRoom = AutoMapper.Mapper.Map<List<V_HIS_ROOM>, List<V_HIS_ROOM>>(lstResponsibilityOfRoomTemp);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstResponsibilityOfRoom), lstResponsibilityOfRoom));

                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());
                var departmerts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().ToList();
                if (departmerts == null || departmerts.Count == 0)
                    throw new ArgumentNullException("departmerts is null");

                var departmentIds = departmerts.Select(o => o.ID).ToArray();
                //var query = lstResponsibilityOfRoom.AsQueryable();


                if (!String.IsNullOrEmpty(txtKeyword.Text))
                {
                    lstResponsibilityOfRoomResult =
                        lstResponsibilityOfRoom
                        .Where(o =>
                            (o.ROOM_CODE.ToLower().Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese(o.ROOM_NAME.ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese(o.DEPARTMENT_NAME.ToLower()).Contains(keyword)
                            || (o.RESPONSIBLE_LOGINNAME != null && o.RESPONSIBLE_LOGINNAME.ToLower().Contains(keyword))
                            || (o.RESPONSIBLE_USERNAME != null && Inventec.Common.String.Convert.UnSignVNese(o.RESPONSIBLE_USERNAME.ToLower()).Contains(keyword)))
                            && (String.IsNullOrEmpty((comboDepartment.EditValue ?? "").ToString()) ||
                            (!String.IsNullOrEmpty((comboDepartment.EditValue ?? "").ToString()) && o.DEPARTMENT_ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.comboDepartment.EditValue).ToString())))
                        ).ToList();
                    lstResponsibilityOfRoomResult = lstResponsibilityOfRoomResult.Where(o => o.IS_EXAM == 1).ToList();
                   
                }
                else
                {
                    lstResponsibilityOfRoomResult =
                        lstResponsibilityOfRoom
                        .Where(o =>
                            String.IsNullOrEmpty((comboDepartment.EditValue ?? "").ToString()) ||
                            (!String.IsNullOrEmpty((comboDepartment.EditValue ?? "").ToString()) && o.DEPARTMENT_ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.comboDepartment.EditValue).ToString()))
                        ).ToList();
                    lstResponsibilityOfRoomResult = lstResponsibilityOfRoomResult.Where(o => o.IS_EXAM == 1).ToList();
                }

                gridControl1.DataSource = lstResponsibilityOfRoomResult;

                LogSystem.Debug(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstResponsibilityOfRoomResult), lstResponsibilityOfRoomResult)
                    + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => comboDepartment.EditValue), comboDepartment.EditValue));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewResponseDoctor_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var data = (V_HIS_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + start;
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void comboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    comboDepartment.Properties.Buttons[1].Visible = false;
                    comboDepartment.EditValue = null;
                    LoaddataToGridView();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void comboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (comboDepartment.EditValue != null && comboDepartment.EditValue != comboDepartment.OldEditValue)
                    {
                        comboDepartment.Properties.Buttons[1].Visible = true;
                    }
                    LoaddataToGridView();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void txtKeyword_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        LoaddataToGridView();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void gridViewResponseDoctor_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            //try
            //{
            //    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //    if (e.RowHandle >= 0)
            //    {
            //        V_HIS_ROOM data = (V_HIS_ROOM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
            //        if (e.Column.FieldName == gridColumnResponseDoctor.FieldName)
            //        {
            //            e.RepositoryItem = grdResponseDoctor;
            //            //LoadComboDoctor(data.ID);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSession.Warn(ex);
            //}
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;

                WaitingManager.Show();

                List<UpdateResponsibleUserSDO> updateDTO = new List<UpdateResponsibleUserSDO>();
                List<V_HIS_ROOM> list = gridControl1.DataSource as List<V_HIS_ROOM>;
                foreach (var item in list)
                {
                    UpdateResponsibleUserSDO model = new UpdateResponsibleUserSDO();
                    model.ResponsibleLoginName = item.RESPONSIBLE_LOGINNAME;
                    if (item.RESPONSIBLE_LOGINNAME != null)
                    {
                        model.ResponsibleUserName = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == item.RESPONSIBLE_LOGINNAME).FirstOrDefault().USERNAME;
                    }
                    model.RoomId = item.ID;
                    updateDTO.Add(model);
                }

                var resultData = new BackendAdapter(param).Post<List<MOS.EFMODEL.DataModels.HIS_ROOM>>("api/HisRoom/UpdateResponsibleUser", ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;

                }
                if (success)
                {
                    //BackendDataWorker.Reset<THE_DEPARTMENT>();
                    //SetFocusEditor();
                }
                HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Reset<V_HIS_ROOM>();

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion


                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                //SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewResponseDoctor_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {

                        view.FocusedRowHandle = hi.RowHandle;
                        view.FocusedColumn = hi.Column;

                        int rowHandle = gridViewResponseDoctor.GetVisibleRowHandle(hi.RowHandle);
                        Inventec.Common.Logging.LogSystem.Debug("dataRow.ID " + rowHandle);
                        var dataRow = (V_HIS_ROOM)gridViewResponseDoctor.GetRow(rowHandle);
                        Inventec.Common.Logging.LogSystem.Debug("dataRow " + dataRow.ID);
                        if (dataRow != null)
                        {
                            //LoadComboDoctor(dataRow.ID);
                            //Inventec.Common.Logging.LogSystem.Debug("dataRow.ID " + dataRow.ID);
                        }
                        //view.ShowEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            
        }

        private void gridViewResponseDoctor_ShownEditor(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            V_HIS_ROOM data = view.GetFocusedRow() as V_HIS_ROOM;

            if (view.FocusedColumn.FieldName == "RESPONSIBLE_LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
            {
                GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                if (data != null)
                {
                    this.FillDataIntoDoctorCombo(data, editor);
                    var dataSource = editor.Properties.DataSource;
                    editor.EditValue = data.RESPONSIBLE_LOGINNAME;
                }
            }
        }

        private void FillDataIntoDoctorCombo(V_HIS_ROOM data, GridLookUpEdit editor)
        {
            try
            {
                if (editor != null)
                {
                    List<ACS_USER> dataCombo = null;
                    this._UserRoom = new List<HIS_USER_ROOM>();
                    MOS.Filter.HisUserRoomFilter filter = new HisUserRoomFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.ROOM_ID = data.ID;
                    this._UserRoom = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_ROOM>>("api/HisUserRoom/Get", ApiConsumers.MosConsumer, filter, null);
                    var loginname = _UserRoom.Select(o => o.LOGINNAME).ToList();
                    var lstDoctor = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.IS_DOCTOR == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(o => o.LOGINNAME);

                    dataCombo = BackendDataWorker.Get<ACS_USER>().Where(o => loginname.Contains(o.LOGINNAME) && this._UserRoom.Exists(p => p.LOGINNAME == o.LOGINNAME)).ToList();
                    //Inventec.Common.Logging.LogSystem.Debug("dataCombo1: " + dataCombo.Count);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataCombo), dataCombo));
                    dataCombo = dataCombo.Where(o => lstDoctor.Contains(o.LOGINNAME)).ToList();
                    //Inventec.Common.Logging.LogSystem.Debug("dataCombo2: " + dataCombo.Count);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataCombo), dataCombo));
                    this.InitComboDoctor(editor, dataCombo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDoctor(GridLookUpEdit editor, List<ACS_USER> dataCombo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 75, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 225, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 300);
                ControlEditorLoader.Load(editor, (dataCombo != null ? dataCombo.ToList() : null), controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdResponseDoctor_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CommonParam param = new CommonParam();
                    var ekipUsers = gridControl1.DataSource as List<V_HIS_ROOM>;
                    var ekipUser = (V_HIS_ROOM)gridViewResponseDoctor.GetFocusedRow();
                    if (ekipUser != null)
                    {
                        ekipUser.RESPONSIBLE_LOGINNAME = null;
                        //grdResponseDoctor.NullText = "";
                        ekipUser.RESPONSIBLE_USERNAME = null;
                        grdResponseDoctor.AllowNullInput = DefaultBoolean.True;
                        gridViewResponseDoctor.EditingValue = null;
                        //grdResponseDoctor.AllowFocused = true;
                        this.gridControl1.RefreshDataSource();
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.isNotHandlerWhileChangeToggetSwith = true;
                this.LoaddataToGridView();
                this.isNotHandlerWhileChangeToggetSwith = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

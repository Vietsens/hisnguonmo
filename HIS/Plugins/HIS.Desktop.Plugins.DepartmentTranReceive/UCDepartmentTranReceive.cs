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
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.UC.Paging;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.DepartmentTranReceive
{
    public partial class UCDepartmentTranReceive : HIS.Desktop.Utility.UserControlBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        int rowCount = 0;
        int dataTotal = 0;
        public PagingGrid pagingGrid;

        List<HIS_DEPARTMENT_TRAN> _Departs { get; set; }

        public UCDepartmentTranReceive()
        {
            InitializeComponent();
        }

        public UCDepartmentTranReceive(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
        }

        private void UCDepartmentTranReceive_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetDefaultValueControl();
                LoadDataToGridDepartmentTran();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DepartmentTranReceive.Resources.Lang", typeof(HIS.Desktop.Plugins.DepartmentTranReceive.UCDepartmentTranReceive).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.XuLyTiepNhan.ToolTip = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.XuLyTiepNhan.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //toolTipItem4.Text = Inventec.Common.Resource.Get.Value("toolTipItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColVirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColVirPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDOB.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColDOB.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLogTime.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColLogTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColDepartmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNextDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColNextDepartmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNextDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColNextDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColInOut.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColInOut.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsReceive.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColIsReceive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBedRoomName.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColBedRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreate.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColCreate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit2.NullText = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.repositoryItemPictureEdit2.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //toolTipItem5.Text = Inventec.Common.Resource.Get.Value("toolTipItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //toolTipItem6.Text = Inventec.Common.Resource.Get.Value("toolTipItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReload.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.btnReload.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControl1.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.navBarControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.navBarGroupTimeCreate.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.navBarGroupTimeCreate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotReceive.Properties.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.chkNotReceive.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkReceived.Properties.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.chkReceived.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup1.Caption = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.navBarGroup1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCDepartmentTranReceive.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                //dtFromTime.EditValue = DateTime.Now;
                //dtToTime.EditValue = DateTime.Now;
                txtKeyWord.Text = "";
                chkReceived.Checked = false;
                chkNotReceive.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadDataToGridDepartmentTran()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                DepartmentTranPaging(new CommonParam(0, (int)pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(DepartmentTranPaging, param, pageSize, gridControlDepartmentTran);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        int startPage = 0;
        private void DepartmentTranPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                //var aaa = pagingGrid.PageCount;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                Inventec.Core.ApiResultObject<List<V_HIS_DEPARTMENT_TRAN>> apiResult = new ApiResultObject<List<V_HIS_DEPARTMENT_TRAN>>();
                HisDepartmentTranViewFilter departmentTranViewFilter = new HisDepartmentTranViewFilter();
                departmentTranViewFilter.KEY_WORD = txtKeyWord.Text;

                departmentTranViewFilter.DEPARTMENT_ID = WorkPlace.WorkPlaceSDO.Where(p => p.RoomId == this.currentModule.RoomId).FirstOrDefault().DepartmentId;
                departmentTranViewFilter.ORDER_DIRECTION = "DESC";
                departmentTranViewFilter.ORDER_FIELD = "MODIFY_TIME";

                if (chkReceived.Checked && chkNotReceive.Checked == false)
                {
                    departmentTranViewFilter.IS_RECEIVE = true;
                }
                if (chkReceived.Checked == false && chkNotReceive.Checked == true)
                {
                    departmentTranViewFilter.IS_RECEIVE = false;
                }

                gridControlDepartmentTran.DataSource = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW, ApiConsumers.MosConsumer, departmentTranViewFilter, paramCommon);

                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>)apiResult.Data;
                    if (data != null)
                    {
                        List<long> _Ids = data.Where(p => p.PREVIOUS_ID != null).Select(p => p.PREVIOUS_ID ?? 0).ToList();
                        if (_Ids != null && _Ids.Count > 0)
                        {
                            _Departs = new List<HIS_DEPARTMENT_TRAN>();
                            HisDepartmentTranViewFilter filter = new HisDepartmentTranViewFilter();
                            filter.IDs = _Ids;
                            _Departs = new BackendAdapter(paramCommon).Get<List<HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                        }


                        gridControlDepartmentTran.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDepartmentTran_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN data = (MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    long treatmentID = data.TREATMENT_ID;
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_DISPLAY")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.Value = "Hoạt động";
                            }
                            else
                            {
                                e.Value = "Tạm khóa";
                            }
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            var pdata = BackendDataWorker.Get<HIS_DEPARTMENT_TRAN>().Where(o => o.TREATMENT_ID == treatmentID);
                            if (HisConfigs.Get<string>("MOS.HIS_DEPARTMENT_TRAN.SHOW_REQUEST_TIME_WHEN_RECEIVING") == "1")
                            {
                                foreach (var item in pdata)
                                {

                                    if (item.REQUEST_TIME != null)
                                    {
                                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.REQUEST_TIME ?? 0);
                                    }
                                    else
                                        e.Value = DateTime.Now;
                                    }
                            }
                                }
                                
                                

                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IS_RECEIVE_DISPLAY")
                        {
                            if (data.DEPARTMENT_IN_TIME != null)
                            {
                                e.Value = "Đã tiếp nhận";

                            }
                            else
                            {
                                e.Value = "Chưa tiếp nhận";
                            }
                        }
                        else if (e.Column.FieldName == "LOG_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.DEPARTMENT_IN_TIME ?? 0);
                        }
                        //else if (e.Column.FieldName == "DEPARTMENT_NAME_DISPLAY" && data.PREVIOUS_ID != null)
                        //{
                        //    var _depart = (_Departs != null && _Departs.Count > 0) ? _Departs.FirstOrDefault(p => p.ID == data.PREVIOUS_ID) : null;
                        //    if (_depart != null)
                        //    {
                        //        var departName = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == _depart.DEPARTMENT_ID);
                        //        e.Value = departName != null ? departName.DEPARTMENT_NAME : null;
                        //    }
                        //}
                    }
                }
            
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDepartmentTran_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;

                if (e.RowHandle >= 0)
                {
                    V_HIS_DEPARTMENT_TRAN data = (V_HIS_DEPARTMENT_TRAN)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "XuLyTiepNhan")
                    {
                        if (data != null)
                        {
                            if (data.DEPARTMENT_IN_TIME != null)
                            {
                                e.RepositoryItem = ButtonEditXlChuyenKboa_disabe;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonEditXLChuyenKhoa_enable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGridDepartmentTran();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                LoadDataToGridDepartmentTran();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditXLChuyenKhoa_enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //mở module 
                var row = (MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN)gridViewDepartmentTran.GetFocusedRow();

                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BedRoomWithIn").FirstOrDefault();
                    moduleData.RoomId = this.currentModule.RoomId;
                    moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.BedRoomWithIn");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                        LoadDataToGridDepartmentTran();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSearch()
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

        public void bbtnReload()
        {
            try
            {
                btnReload_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtKeyWord.Text != null)
                    {
                        LoadDataToGridDepartmentTran();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

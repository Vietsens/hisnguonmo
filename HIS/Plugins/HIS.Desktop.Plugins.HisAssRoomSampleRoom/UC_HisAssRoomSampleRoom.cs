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
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using DevExpress.XtraGrid.Columns;
using HIS.UC.Room;
using HIS.UC.Room.ADO;
using HIS.UC.SampleRoom;
using HIS.UC.SampleRoom.ADO;
using HIS.Desktop.Plugins.HisAssRoomSampleRoom.entity;
using Inventec.Common.Controls.EditorLoader;
using AutoMapper;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Plugins.HisAssRoomSampleRoom.Properties;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraBars;

namespace HIS.Desktop.Plugins.HisAssRoomSampleRoom
{
    public partial class UC_HisAssRoomSampleRoom : HIS.Desktop.Utility.UserControlBase
    {

        #region Declare
        List<RoomAccountADO> listRoomDeleteADO { get; set; }
        List<RoomAccountADO> listRoomInsertADO { get; set; }
        List<V_HIS_ROOM> Room { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        HIS.UC.Room.UCRoomProcessor RoomProcessor;
        SampleRoomProcessor SampleRoomProcessor;
        UserControl ucGridControlER;
        UserControl ucGridControlSampleRoom;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        bool isCheckAll;
        internal List<HIS.UC.SampleRoom.SampleRoomADO> lstSampleRoomADOs { get; set; }
        internal List<HIS.UC.Room.RoomAccountADO> lstRoomADOs { get; set; }
        List<HIS_SAMPLE_ROOM> listSaro;
        List<V_HIS_ROOM> listRoom;
        long ERIdCheckByER = 0;
        long isChoseER;
        long isChoseSampleRoom;
        long sampleRoomIdCheckBySampleRoom = 0;
        List<HIS_ROOM_SARO> _HisRoomSaros { get; set; }
        List<HIS_ROOM_SARO> HisAssRoomSampleRoom1 { get; set; }
        bool checkRa = false;

        #endregion


        #region Constructor

        public UC_HisAssRoomSampleRoom(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
        }

        private void UC_HisAssRoomSampleRoom_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();
                txtSearchER.Focus();
                txtSearchER.SelectAll();
                LoadDataToCombo();
                InitUcgrid1();
                InitUcgrid2();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisAssRoomSampleRoom.Resources.Lang", typeof(HIS.Desktop.Plugins.HisAssRoomSampleRoom.UC_HisAssRoomSampleRoom).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_HisAssRoomSampleRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChooseBy.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisAssRoomSampleRoom.cboChooseBy.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_HisAssRoomSampleRoom.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UC_HisAssRoomSampleRoom.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchA.Text = Inventec.Common.Resource.Get.Value("UC_HisAssRoomSampleRoom.btnSearchA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchA.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_HisAssRoomSampleRoom.txtSearchA.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_HisAssRoomSampleRoom.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSeachER.Text = Inventec.Common.Resource.Get.Value("UC_HisAssRoomSampleRoom.btnSeachER.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchER.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_HisAssRoomSampleRoom.txtSearchER.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_HisAssRoomSampleRoom.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion


        #region Load column

        private void InitUcgrid1()
        {
            try
            {
                RoomProcessor = new UCRoomProcessor();
                RoomInitADO ado = new RoomInitADO();
                ado.ListRoomColumn = new List<RoomColumn>();
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                // ado.gridViewRoom_MouseDownRoom = RoomGrid_CellValueChanged;
                ado.gridViewRoom_MouseDownRoom = Room_MouseDown;
                object image = Properties.Resources.ResourceManager.GetObject("check1");

                RoomColumn colRadio1 = new RoomColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colRadio1);

                RoomColumn colCheck1 = new RoomColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = imageCollection.Images[0];
                colCheck1.ToolTip = "Chọn tất cả";
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colCheck1);

                RoomColumn colMaPhongXL = new RoomColumn("Mã phòng", "ROOM_CODE", 120, false);
                colMaPhongXL.VisibleIndex = 2;
                ado.ListRoomColumn.Add(colMaPhongXL);

                RoomColumn colTenPhongXL = new RoomColumn("Tên phòng", "ROOM_NAME", 120, false);
                colTenPhongXL.VisibleIndex = 3;
                ado.ListRoomColumn.Add(colTenPhongXL);

                this.ucGridControlER = (UserControl)RoomProcessor.Run(ado);
                if (ucGridControlER != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlER);
                    this.ucGridControlER.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcgrid2()
        {
            try
            {
                SampleRoomProcessor = new SampleRoomProcessor();
                SampleRoomInitADO ado = new SampleRoomInitADO();
                ado.ListSampleRoomColumn = new List<UC.SampleRoom.SampleRoomColumn>();
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.SampleRoomGrid_MouseDown = SampleRoom_MouseDown;
                object image = Properties.Resources.ResourceManager.GetObject("check1");

                SampleRoomColumn colRadio2 = new SampleRoomColumn("   ", "radio2", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListSampleRoomColumn.Add(colRadio2);

                SampleRoomColumn colCheck2 = new SampleRoomColumn("   ", "check2", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollection.Images[0];
                //colCheck2.image = StringAlignment.Center;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListSampleRoomColumn.Add(colCheck2);

                SampleRoomColumn colMaPhongLM = new SampleRoomColumn("Mã phòng lấy mẫu", "SAMPLE_ROOM_CODE", 120, false);
                colMaPhongLM.VisibleIndex = 2;
                ado.ListSampleRoomColumn.Add(colMaPhongLM);

                SampleRoomColumn colTenPhongLM = new SampleRoomColumn("Tên phòng lấy mẫu", "SAMPLE_ROOM_NAME", 120, false);
                colTenPhongLM.VisibleIndex = 3;
                ado.ListSampleRoomColumn.Add(colTenPhongLM);

                this.ucGridControlSampleRoom = (UserControl)SampleRoomProcessor.Run(ado);
                if (ucGridControlSampleRoom != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlSampleRoom);
                    this.ucGridControlSampleRoom.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        #region Load Combo

        private void LoadDataToCombo()
        {
            try
            {
                LoadComboStatus();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Phòng chỉ định"));
                status.Add(new Status(2, "Phòng lấy mẫu"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChooseBy, status, controlEditorADO);
                cboChooseBy.EditValue = status[0].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        #region Method


        private void FillDataToGrid2(UC_HisAssRoomSampleRoom uCHisAssRoomSampleRoom)
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                FillDataToGridSampleRoom(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridSampleRoom, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridSampleRoom(object data)
        {
            try
            {
                WaitingManager.Show();
                listSaro = new List<HIS_SAMPLE_ROOM>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                HisSampleRoomFilter SampleRoomFilter = new HisSampleRoomFilter();
                SampleRoomFilter.ORDER_FIELD = "MODIFY_TIME";
                SampleRoomFilter.ORDER_DIRECTION = "DESC";
                SampleRoomFilter.KEY_WORD = txtSearchA.Text;

                if ((long)cboChooseBy.EditValue == 2)
                {
                    isChoseSampleRoom = (long)cboChooseBy.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<HIS_SAMPLE_ROOM>>(
                    HisRequestUriStore.HIS_SAMPLE_ROOM_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    SampleRoomFilter,
                    param);

                lstSampleRoomADOs = new List<HIS.UC.SampleRoom.SampleRoomADO>();

                if (sar != null && sar.Data.Count > 0)
                {
                    //List<SAMPLE_ROOM> dataBySampleRoom = new List<SAMPLE_ROOM>();
                    listSaro = sar.Data;
                    foreach (var item in listSaro)
                    {
                        HIS.UC.SampleRoom.SampleRoomADO sampleRoomADO = new HIS.UC.SampleRoom.SampleRoomADO(item);
                        if (isChoseSampleRoom == 2)
                        {
                            sampleRoomADO.isKeyChoose1 = true;
                        }

                        lstSampleRoomADOs.Add(sampleRoomADO);
                    }
                }
                if (_HisRoomSaros != null && _HisRoomSaros.Count > 0)
                {
                    foreach (var item in _HisRoomSaros)
                    {
                        var check = lstSampleRoomADOs.FirstOrDefault(o => o.ID == item.SAMPLE_ROOM_ID);
                        if (check != null)
                        {
                            check.check2 = true;
                        }

                    }
                }

                lstSampleRoomADOs = lstSampleRoomADOs.OrderByDescending(p => p.check2).ToList();
                if (ucGridControlSampleRoom != null)
                {
                    SampleRoomProcessor.Reload(ucGridControlSampleRoom, lstSampleRoomADOs);
                }
                rowCount1 = (data == null ? 0 : lstSampleRoomADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1(UC_HisAssRoomSampleRoom uCHisAssRoomSampleRoom)
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                FillDataToGridRoom(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridRoom, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridRoom(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoom = new List<V_HIS_ROOM>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisRoomFilter ERFillter = new HisRoomFilter();
                ERFillter.ORDER_FIELD = "MODIFY_TIME";
                ERFillter.ORDER_DIRECTION = "DESC";
                ERFillter.KEY_WORD = txtSearchER.Text;

                if ((long)cboChooseBy.EditValue == 1)
                {
                    isChoseER = (long)cboChooseBy.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>(
                     "api/HisRoom/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ERFillter,
                     param);

                lstRoomADOs = new List<HIS.UC.Room.RoomAccountADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_HIS_ROOM> dataByRoom = new List<V_HIS_ROOM>();
                    listRoom = rs.Data;
                    foreach (var item in listRoom)
                    {
                        HIS.UC.Room.RoomAccountADO RoomADO = new HIS.UC.Room.RoomAccountADO(item);
                        if (isChoseER == 1)
                        {
                            RoomADO.isKeyChoose = true;
                        }
                        lstRoomADOs.Add(RoomADO);
                    }
                }

                if (HisAssRoomSampleRoom1 != null && HisAssRoomSampleRoom1.Count > 0)
                {
                    foreach (var item in HisAssRoomSampleRoom1)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == item.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlER != null)
                {
                    RoomProcessor.Reload(ucGridControlER, lstRoomADOs);
                }
                rowCount = (data == null ? 0 : lstRoomADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut1()
        {
            try
            {
                btnFind1_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut2()
        {
            try
            {
                btnFind2_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveShortcut()
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion


        #region Event

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid1(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid2(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                checkRa = false;
                isChoseSampleRoom = 0;
                isChoseER = 0;
                _HisRoomSaros = null;
                HisAssRoomSampleRoom1 = null;
                FillDataToGrid1(this);
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool resultSuccess = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                if (ucGridControlSampleRoom != null && ucGridControlER != null)
                {
                    object sampleRoom = SampleRoomProcessor.GetDataGridView(ucGridControlSampleRoom);
                    object ExecuteR = RoomProcessor.GetDataGridView(ucGridControlER);
                    if (isChoseER == 1)
                    {
                        if (sampleRoom is List<HIS.UC.SampleRoom.SampleRoomADO>)
                        {
                            var data = (List<HIS.UC.SampleRoom.SampleRoomADO>)sampleRoom;
                            if (data != null && data.Count > 0)
                            {
                                if (checkRa == true)
                                {
                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisRoomSaroFilter filter = new HisRoomSaroFilter();
                                    filter.ROOM_ID = ERIdCheckByER;

                                    var HisAssRoomSampleRoom = new BackendAdapter(param).Get<List<HIS_ROOM_SARO>>(
                                       "api/HisRoomSaro/Get",
                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                       filter,
                                       param);

                                    List<long> listSaroid = HisAssRoomSampleRoom.Select(p => p.SAMPLE_ROOM_ID).ToList();

                                    var dataCheckeds = data.Where(p => p.check2 == true).ToList();

                                    //List xoa

                                    var dataDeletes = data.Where(o => HisAssRoomSampleRoom.Select(p => p.SAMPLE_ROOM_ID)
                                        .Contains(o.ID) && o.check2 == false).ToList();

                                    //list them
                                    var dataCreates = dataCheckeds.Where(o => !HisAssRoomSampleRoom.Select(p => p.SAMPLE_ROOM_ID)
                                        .Contains(o.ID)).ToList();

                                    if (dataDeletes != null && dataDeletes.Count > 0)
                                    {
                                        List<long> deleteIds = HisAssRoomSampleRoom.Where(o => dataDeletes.Select(p => p.ID)
                                            .Contains(o.SAMPLE_ROOM_ID)).Select(o => o.ID).ToList();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "/api/HisRoomSaro/DeleteList",
                                                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                  deleteIds,
                                                  param);
                                        if (deleteResult)
                                            resultSuccess = true;

                                    }

                                    if (dataCreates != null && dataCreates.Count > 0)
                                    {
                                        List<HIS_ROOM_SARO> HisAssRoomSampleRoomCreates = new List<HIS_ROOM_SARO>();
                                        foreach (var item in dataCreates)
                                        {
                                            HIS_ROOM_SARO HisAssRoomSampleRoomCreate = new HIS_ROOM_SARO();
                                            HisAssRoomSampleRoomCreate.SAMPLE_ROOM_ID = item.ID;
                                            HisAssRoomSampleRoomCreate.ROOM_ID = ERIdCheckByER;
                                            HisAssRoomSampleRoomCreates.Add(HisAssRoomSampleRoomCreate);
                                        }

                                        var createResult = new BackendAdapter(param).Post<List<HIS_ROOM_SARO>>(
                                                   "/api/HisRoomSaro/CreateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   HisAssRoomSampleRoomCreates,
                                                   param);
                                        if (createResult != null && createResult.Count > 0)
                                            resultSuccess = true;
                                    }
                                    WaitingManager.Hide();

                                    #region Show message
                                    MessageManager.ShowAlert(this.ParentForm, param, resultSuccess);
                                    #endregion

                                    #region Process has exception
                                    SessionManager.ProcessTokenLost(param);
                                    #endregion

                                    data = data.OrderByDescending(p => p.check2).ToList();
                                    if (ucGridControlSampleRoom != null)
                                    {
                                        SampleRoomProcessor.Reload(ucGridControlSampleRoom, data);
                                    }
                                    WaitingManager.Hide();
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng chỉ định");
                                    WaitingManager.Hide();
                                }
                            }
                        }
                    }
                    if (isChoseSampleRoom == 2)
                    {
                        if (ExecuteR is List<HIS.UC.Room.RoomAccountADO>)
                        {
                            var data = (List<HIS.UC.Room.RoomAccountADO>)ExecuteR;

                            if (data != null && data.Count > 0)
                            {
                                if (checkRa == true)
                                {
                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisRoomSaroFilter filter = new HisRoomSaroFilter();
                                    filter.SAMPLE_ROOM_ID = sampleRoomIdCheckBySampleRoom;
                                    var erUser = new BackendAdapter(param).Get<List<HIS_ROOM_SARO>>(
                                       "api/HisRoomSaro/Get",
                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                       filter,
                                       param);

                                    var listERID = erUser.Select(p => p.ROOM_ID).ToList();

                                    var dataChecked = data.Where(p => p.check1 == true).ToList();

                                    //List xoa

                                    var dataDelete = data.Where(o => erUser.Select(p => p.ROOM_ID)
                                        .Contains(o.ID) && o.check1 == false).ToList();

                                    //list them
                                    var dataCreate = dataChecked.Where(o => !erUser.Select(p => p.ROOM_ID)
                                        .Contains(o.ID)).ToList();

                                    if (dataDelete != null && dataDelete.Count > 0)
                                    {
                                        List<long> deleteId = erUser.Where(o => dataDelete.Select(p => p.ID)

                                            .Contains(o.ROOM_ID)).Select(o => o.ID).ToList();
                                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                  "/api/HisRoomSaro/DeleteList",
                                                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                  deleteId,
                                                  param);
                                        if (deleteResult)
                                            resultSuccess = true;

                                    }

                                    if (dataCreate != null && dataCreate.Count > 0)
                                    {
                                        List<HIS_ROOM_SARO> erUserCreate = new List<HIS_ROOM_SARO>();
                                        foreach (var item in dataCreate)
                                        {
                                            HIS_ROOM_SARO erUserID = new HIS_ROOM_SARO();
                                            erUserID.ROOM_ID = item.ID;
                                            erUserID.SAMPLE_ROOM_ID = sampleRoomIdCheckBySampleRoom;
                                            erUserCreate.Add(erUserID);
                                        }

                                        var createResult = new BackendAdapter(param).Post<List<HIS_ROOM_SARO>>(
                                                   "/api/HisRoomSaro/CreateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   erUserCreate,
                                                   param);
                                        if (createResult != null && createResult.Count > 0)
                                            resultSuccess = true;
                                    }
                                    WaitingManager.Hide();
                                    #region Show message
                                    MessageManager.ShowAlert(this.ParentForm, param, resultSuccess);
                                    #endregion

                                    #region Process has exception
                                    SessionManager.ProcessTokenLost(param);
                                    #endregion

                                    data = data.OrderByDescending(p => p.check1).ToList();
                                    if (ucGridControlER != null)
                                    {
                                        RoomProcessor.Reload(ucGridControlER, data);
                                    }
                                    WaitingManager.Hide();
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng lấy mẫu");
                                    WaitingManager.Hide();
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RoomGrid_CellValueChanged(RoomAccountADO data, CellValueChangedEventArgs e)
        {
            try
            {
                //if (data != null)
                //{
                //    if (data.check1 == true)
                //    {

                //        listRoomInsertADO.Add(data);
                //    }
                //    else
                //    {

                //        listRoomDeleteADO.Add(data);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Room_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseER == 1)
                {
                    return;
                }

                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check1")
                        {
                            var lstCheckAll = lstRoomADOs;
                            List<HIS.UC.Room.RoomAccountADO> lstChecks = new List<HIS.UC.Room.RoomAccountADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollection.Images[1];
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollection.Images[0];
                                }

                                RoomProcessor.Reload(ucGridControlER, lstChecks);
                                //??

                            }
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SampleRoom_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseSampleRoom == 2)
                {
                    return;
                }
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check2")
                        {
                            var lstCheckAll = lstSampleRoomADOs;
                            List<HIS.UC.SampleRoom.SampleRoomADO> lstChecks = new List<HIS.UC.SampleRoom.SampleRoomADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check2 = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollection.Images[1];
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check2 = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollection.Images[0];
                                }

                                //ReloadData
                                SampleRoomProcessor.Reload(ucGridControlSampleRoom, lstChecks);
                                //??

                            }
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click(V_HIS_ROOM data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisRoomSaroFilter filter = new HisRoomSaroFilter();
                filter.ROOM_ID = data.ID;
                ERIdCheckByER = data.ID;
                _HisRoomSaros = new BackendAdapter(param).Get<List<HIS_ROOM_SARO>>(
                                "api/HisRoomSaro/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.SampleRoom.SampleRoomADO> dataNew = new List<HIS.UC.SampleRoom.SampleRoomADO>();
                dataNew = (from r in listSaro select new HIS.UC.SampleRoom.SampleRoomADO(r)).ToList();
                if (_HisRoomSaros != null && _HisRoomSaros.Count > 0)
                {
                    foreach (var item in _HisRoomSaros)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == item.SAMPLE_ROOM_ID);
                        if (check != null)
                        {
                            check.check2 = true;

                        }

                    }
                }

                dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
                if (ucGridControlSampleRoom != null)
                {
                    SampleRoomProcessor.Reload(ucGridControlSampleRoom, dataNew);
                }
                WaitingManager.Hide();
                checkRa = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click1(HIS_SAMPLE_ROOM data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisRoomSaroFilter filter = new HisRoomSaroFilter();
                filter.SAMPLE_ROOM_ID = data.ID;
                sampleRoomIdCheckBySampleRoom = data.ID;
                HisAssRoomSampleRoom1 = new BackendAdapter(param).Get<List<HIS_ROOM_SARO>>(
                                "api/HisRoomSaro/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Room.RoomAccountADO> dataNew = new List<HIS.UC.Room.RoomAccountADO>();
                dataNew = (from r in listRoom select new HIS.UC.Room.RoomAccountADO(r)).ToList();
                if (HisAssRoomSampleRoom1 != null && HisAssRoomSampleRoom1.Count > 0)
                {

                    foreach (var itemUsername in HisAssRoomSampleRoom1)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    if (ucGridControlER != null)
                    {
                        RoomProcessor.Reload(ucGridControlER, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid1(this);
                }
                WaitingManager.Hide();
                checkRa = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchER_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid1(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchA_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid2(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion


    }

}

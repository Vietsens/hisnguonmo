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
using HIS.UC.ExecuteRoom;
using HIS.UC.ExecuteRoom.ADO;
using HIS.UC.SampleRoom;
using HIS.UC.SampleRoom.ADO;
using HIS.Desktop.Plugins.SaroExro.entity;
using Inventec.Common.Controls.EditorLoader;
using AutoMapper;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Plugins.SaroExro.Properties;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.SaroExro
{
    public partial class UC_SaroExro : HIS.Desktop.Utility.UserControlBase
    {

        #region Declare
        List<ExecuteRoomADO> listExecuteRoomDeleteADO { get; set; }
        List<ExecuteRoomADO> listExecuteRoomInsertADO { get; set; }
        List<HIS_EXECUTE_ROOM> executeRoom { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        ExecuteRoomProcessor ERProcessor;
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
        internal List<HIS.UC.ExecuteRoom.ExecuteRoomADO> lstExecuteRoomADOs { get; set; }
        List<HIS_SAMPLE_ROOM> listSaro;
        List<HIS_EXECUTE_ROOM> listExecuteRoom;
        long ERIdCheckByER = 0;
        long isChoseER;
        long isChoseSampleRoom;
        long sampleRoomIdCheckBySampleRoom = 0;
        List<HIS_SARO_EXRO> saroExro { get; set; }
        List<HIS_SARO_EXRO> saroExro1 { get; set; }
        bool checkRa = false;

        #endregion


        #region Constructor

        public UC_SaroExro(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
        }

        private void UC_SaroExro_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SaroExro.Resources.Lang", typeof(HIS.Desktop.Plugins.SaroExro.UC_SaroExro).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_SaroExro.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChooseBy.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_SaroExro.cboChooseBy.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_SaroExro.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UC_SaroExro.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchA.Text = Inventec.Common.Resource.Get.Value("UC_SaroExro.btnSearchA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchA.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_SaroExro.txtSearchA.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_SaroExro.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSeachER.Text = Inventec.Common.Resource.Get.Value("UC_SaroExro.btnSeachER.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchER.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_SaroExro.txtSearchER.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_SaroExro.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                ERProcessor = new ExecuteRoomProcessor();
                ExecuteRoomInitADO ado = new ExecuteRoomInitADO();
                ado.ListExecuteRoomColumn = new List<ExecuteRoomColumn>();
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.ExecuteRoomGrid_CellValueChanged = ExecuteRoomGrid_CellValueChanged;
                ado.ExecuteRoomGrid_MouseDown = ExecuteRoom_MouseDown;
                object image = Properties.Resources.ResourceManager.GetObject("check1");

                ExecuteRoomColumn colRadio1 = new ExecuteRoomColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoomColumn.Add(colRadio1);

                ExecuteRoomColumn colCheck1 = new ExecuteRoomColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = imageCollection.Images[0];
                colCheck1.ToolTip = "Chọn tất cả";
                colCheck1.imageAlignment = StringAlignment.Center;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoomColumn.Add(colCheck1);

                ExecuteRoomColumn colMaPhongXL = new ExecuteRoomColumn("Mã phòng xử lý", "EXECUTE_ROOM_CODE", 120, false);
                colMaPhongXL.VisibleIndex = 2;
                ado.ListExecuteRoomColumn.Add(colMaPhongXL);

                ExecuteRoomColumn colTenPhongXL = new ExecuteRoomColumn("Tên phòng xử lý", "EXECUTE_ROOM_NAME", 120, false);
                colTenPhongXL.VisibleIndex = 3;
                ado.ListExecuteRoomColumn.Add(colTenPhongXL);

                this.ucGridControlER = (UserControl)ERProcessor.Run(ado);
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
                status.Add(new Status(1, "Phòng xử lý"));
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


        private void FillDataToGrid2(UC_SaroExro uCSaroExro)
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
                if (saroExro != null && saroExro.Count > 0)
                {
                    foreach (var item in saroExro)
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

        private void FillDataToGrid1(UC_SaroExro uCSaroExro)
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
                FillDataToGridExecuteRoom(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridExecuteRoom, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridExecuteRoom(object data)
        {
            try
            {
                WaitingManager.Show();
                listExecuteRoom = new List<HIS_EXECUTE_ROOM>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisExecuteRoomFilter ERFillter = new HisExecuteRoomFilter();
                ERFillter.ORDER_FIELD = "MODIFY_TIME";
                ERFillter.ORDER_DIRECTION = "DESC";
                ERFillter.KEY_WORD = txtSearchER.Text;

                if ((long)cboChooseBy.EditValue == 1)
                {
                    isChoseER = (long)cboChooseBy.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM>>(
                     HisRequestUriStore.HIS_EXECUTE_ROOM_GET,
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ERFillter,
                     param);

                lstExecuteRoomADOs = new List<HIS.UC.ExecuteRoom.ExecuteRoomADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_HIS_ROOM> dataByRoom = new List<V_HIS_ROOM>();
                    listExecuteRoom = rs.Data;
                    foreach (var item in listExecuteRoom)
                    {
                        HIS.UC.ExecuteRoom.ExecuteRoomADO ExecuteRoomADO = new HIS.UC.ExecuteRoom.ExecuteRoomADO(item);
                        if (isChoseER == 1)
                        {
                            ExecuteRoomADO.isKeyChoose = true;
                        }
                        lstExecuteRoomADOs.Add(ExecuteRoomADO);
                    }
                }

                if (saroExro1 != null && saroExro1.Count > 0)
                {
                    foreach (var item in saroExro1)
                    {
                        var check = lstExecuteRoomADOs.FirstOrDefault(o => o.ID == item.EXECUTE_ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlER != null)
                {
                    ERProcessor.Reload(ucGridControlER, lstExecuteRoomADOs);
                }
                rowCount = (data == null ? 0 : lstExecuteRoomADOs.Count);
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
                saroExro = null;
                saroExro1 = null;
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
                    object ExecuteR = ERProcessor.GetDataGridView(ucGridControlER);
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
                                    MOS.Filter.HisSaroExroFilter filter = new HisSaroExroFilter();
                                    filter.EXECUTE_ROOM_ID = ERIdCheckByER;

                                    var saroExro = new BackendAdapter(param).Get<List<HIS_SARO_EXRO>>(
                                       HisRequestUriStore.HIS_SARO_EXRO_GET,
                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                       filter,
                                       param);

                                    List<long> listSaroid = saroExro.Select(p => p.SAMPLE_ROOM_ID).ToList();

                                    var dataCheckeds = data.Where(p => p.check2 == true).ToList();

                                    //List xoa

                                    var dataDeletes = data.Where(o => saroExro.Select(p => p.SAMPLE_ROOM_ID)
                                        .Contains(o.ID) && o.check2 == false).ToList();

                                    //list them
                                    var dataCreates = dataCheckeds.Where(o => !saroExro.Select(p => p.SAMPLE_ROOM_ID)
                                        .Contains(o.ID)).ToList();
                                    if (dataCheckeds.Count != saroExro.Select(p => p.SAMPLE_ROOM_ID).Count())
                                    {

                                        if (dataDeletes != null && dataDeletes.Count > 0)
                                        {
                                            List<long> deleteIds = saroExro.Where(o => dataDeletes.Select(p => p.ID)
                                                .Contains(o.SAMPLE_ROOM_ID)).Select(o => o.ID).ToList();
                                            bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                      "/api/HisSaroExro/DeleteList",
                                                      HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                      deleteIds,
                                                      param);
                                            if (deleteResult)
                                                resultSuccess = true;

                                        }

                                        if (dataCreates != null && dataCreates.Count > 0)
                                        {
                                            List<HIS_SARO_EXRO> saroExroCreates = new List<HIS_SARO_EXRO>();
                                            foreach (var item in dataCreates)
                                            {
                                                HIS_SARO_EXRO saroExroCreate = new HIS_SARO_EXRO();
                                                saroExroCreate.SAMPLE_ROOM_ID = item.ID;
                                                saroExroCreate.EXECUTE_ROOM_ID = ERIdCheckByER;
                                                saroExroCreates.Add(saroExroCreate);
                                            }

                                            var createResult = new BackendAdapter(param).Post<List<HIS_SARO_EXRO>>(
                                                       "/api/HisSaroExro/CreateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       saroExroCreates,
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
                                    }
                                    data = data.OrderByDescending(p => p.check2).ToList();
                                    if (ucGridControlSampleRoom != null)
                                    {
                                        SampleRoomProcessor.Reload(ucGridControlSampleRoom, data);
                                    }
                                    WaitingManager.Hide();
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng xử lý");
                                    WaitingManager.Hide();
                                }
                            }
                        }
                    }
                    if (isChoseSampleRoom == 2)
                    {
                        if (ExecuteR is List<HIS.UC.ExecuteRoom.ExecuteRoomADO>)
                        {
                            var data = (List<HIS.UC.ExecuteRoom.ExecuteRoomADO>)ExecuteR;

                            if (data != null && data.Count > 0)
                            {
                                if (checkRa == true)
                                {
                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisSaroExroFilter filter = new HisSaroExroFilter();
                                    filter.SAMPLE_ROOM_ID = sampleRoomIdCheckBySampleRoom;
                                    var erUser = new BackendAdapter(param).Get<List<HIS_SARO_EXRO>>(
                                       HisRequestUriStore.HIS_SARO_EXRO_GET,
                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                       filter,
                                       param);

                                    var listERID = erUser.Select(p => p.EXECUTE_ROOM_ID).ToList();

                                    var dataChecked = data.Where(p => p.check1 == true).ToList();

                                    //List xoa

                                    var dataDelete = data.Where(o => erUser.Select(p => p.EXECUTE_ROOM_ID)
                                        .Contains(o.ID) && o.check1 == false).ToList();

                                    //list them
                                    var dataCreate = dataChecked.Where(o => !erUser.Select(p => p.EXECUTE_ROOM_ID)
                                        .Contains(o.ID)).ToList();
                                    if (dataChecked.Count != erUser.Select(p => p.EXECUTE_ROOM_ID).Count())
                                    {
                                        if (dataDelete != null && dataDelete.Count > 0)
                                        {
                                            List<long> deleteId = erUser.Where(o => dataDelete.Select(p => p.ID)

                                                .Contains(o.EXECUTE_ROOM_ID)).Select(o => o.ID).ToList();
                                            bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                      "/api/HisSaroExro/DeleteList",
                                                      HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                      deleteId,
                                                      param);
                                            if (deleteResult)
                                                resultSuccess = true;

                                        }

                                        if (dataCreate != null && dataCreate.Count > 0)
                                        {
                                            List<HIS_SARO_EXRO> erUserCreate = new List<HIS_SARO_EXRO>();
                                            foreach (var item in dataCreate)
                                            {
                                                HIS_SARO_EXRO erUserID = new HIS_SARO_EXRO();
                                                erUserID.EXECUTE_ROOM_ID = item.ID;
                                                erUserID.SAMPLE_ROOM_ID = sampleRoomIdCheckBySampleRoom;
                                                erUserCreate.Add(erUserID);
                                            }

                                            var createResult = new BackendAdapter(param).Post<List<HIS_SARO_EXRO>>(
                                                       "/api/HisSaroExro/CreateList",
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
                                    }
                                    data = data.OrderByDescending(p => p.check1).ToList();
                                    if (ucGridControlER != null)
                                    {
                                        ERProcessor.Reload(ucGridControlER, data);
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

        private void ExecuteRoomGrid_CellValueChanged(ExecuteRoomADO data, CellValueChangedEventArgs e)
        {
            try
            {
                //if (data != null)
                //{
                //    if (data.check1 == true)
                //    {

                //        listExecuteRoomInsertADO.Add(data);
                //    }
                //    else
                //    {

                //        listExecuteRoomDeleteADO.Add(data);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExecuteRoom_MouseDown(object sender, MouseEventArgs e)
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
                            var lstCheckAll = lstExecuteRoomADOs;
                            List<HIS.UC.ExecuteRoom.ExecuteRoomADO> lstChecks = new List<HIS.UC.ExecuteRoom.ExecuteRoomADO>();

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

                                ERProcessor.Reload(ucGridControlER, lstChecks);
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

        private void btn_Radio_Enable_Click(HIS_EXECUTE_ROOM data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisSaroExroFilter filter = new HisSaroExroFilter();
                filter.EXECUTE_ROOM_ID = data.ID;
                ERIdCheckByER = data.ID;
                saroExro = new BackendAdapter(param).Get<List<HIS_SARO_EXRO>>(
                                HisRequestUriStore.HIS_SARO_EXRO_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.SampleRoom.SampleRoomADO> dataNew = new List<HIS.UC.SampleRoom.SampleRoomADO>();
                dataNew = (from r in listSaro select new HIS.UC.SampleRoom.SampleRoomADO(r)).ToList();
                if (saroExro != null && saroExro.Count > 0)
                {
                    foreach (var item in saroExro)
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
                MOS.Filter.HisSaroExroFilter filter = new HisSaroExroFilter();
                filter.SAMPLE_ROOM_ID = data.ID;
                sampleRoomIdCheckBySampleRoom = data.ID;
                saroExro1 = new BackendAdapter(param).Get<List<HIS_SARO_EXRO>>(
                                HisRequestUriStore.HIS_SARO_EXRO_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.ExecuteRoom.ExecuteRoomADO> dataNew = new List<HIS.UC.ExecuteRoom.ExecuteRoomADO>();
                dataNew = (from r in listExecuteRoom select new HIS.UC.ExecuteRoom.ExecuteRoomADO(r)).ToList();
                if (saroExro1 != null && saroExro1.Count > 0)
                {

                    foreach (var itemUsername in saroExro1)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.EXECUTE_ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    if (ucGridControlER != null)
                    {
                        ERProcessor.Reload(ucGridControlER, dataNew);
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

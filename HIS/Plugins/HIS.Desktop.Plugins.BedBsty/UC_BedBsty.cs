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
using HIS.UC.Bed;
using HIS.UC.Bed.ADO;
using HIS.UC.BedServiceType;
using HIS.UC.BedServiceType.ADO;
using HIS.Desktop.Plugins.BedBsty.entity;
using Inventec.Common.Controls.EditorLoader;
using AutoMapper;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Plugins.BedBsty.Properties;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Controls;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraBars;
using MOS.SDO;
using DevExpress.XtraGrid;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.BedBsty
{
    public partial class UC_BedBsty : HIS.Desktop.Utility.UserControlBase
    {

        #region Declare
        List<HIS_BED_TYPE> bedType { get; set; }
        List<BedADO> listBedDeleteADO { get; set; }
        List<BedADO> listBedInsertADO { get; set; }
        //List<HIS_EXECUTE_ROLE> executeRole { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        BedProcessor BedProcessor;
        BedServiceTypeProcessor BedServiceTypeProcessor;
        UserControl ucGridControlBed;
        UserControl ucGridControlBedServiceType;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        bool isCheckAll;
        internal List<HIS.UC.BedServiceType.BedServiceTypeADO> lstBedServiceTypeADOs { get; set; }
        internal List<HIS.UC.Bed.BedADO> lstBedADOs { get; set; }
        List<HIS_SERVICE> listBedServiceType;
        List<HIS_BED> listBed;
        long BedIdCheckByBed = 0;
        long isChoseBed;
        long isChoseBedServiceType;
        long BstyIdCheckByBsty;
        List<HIS_BED_BSTY> bedBSTY { get; set; }
        List<HIS_BED_BSTY> bedBSTY1 { get; set; }
        HIS.UC.Bed.BedADO currentCopyBedAdo { get; set; }
        HIS.UC.BedServiceType.BedServiceTypeADO currentCopyBedServiceTypeAdo { get; set; }
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_SERVICE currentService;
        HIS_BED Bed;

        #endregion


        #region Constructor

        public UC_BedBsty(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
        }

        public UC_BedBsty(Inventec.Desktop.Common.Modules.Module currentModule, long ServiceType)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                if (this.currentModule != null)
                {
                    this.Text = currentModule.text;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public UC_BedBsty(HIS_SERVICE serviceData, Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentService = serviceData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public UC_BedBsty(HIS_BED executeBed, Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.Bed = executeBed;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UC_BedBsty_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();
                txtSearchBed.Focus();
                txtSearchBed.SelectAll();
                LoadDataToCombo();
                InitUcgrid1();
                InitUcgrid2();
                if (this.currentService == null && this.Bed == null)
                {
                    FillDataToGrid1(this);
                    FillDataToGrid2(this);
                }
                else if (this.Bed == null)
                {
                    cboChooseBy.EditValue = (long)1;
                    cboChooseBy.Enabled = false;
                    FillDataToGridBed(this);
                    FillDataToGrid2(this);
                    btn_Radio_Enable_Click1(this.currentService);
                }
                else if (this.currentService == null)
                {
                    cboChooseBy.EditValue = (long)2;
                    cboChooseBy.Enabled = false;
                    FillDataToGrid1(this);
                    FillDataToGridBsty(this);
                    btn_Radio_Enable_Click(this.Bed);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        #region Load column

        private void InitUcgrid1()
        {
            try
            {
                BedProcessor = new BedProcessor();
                BedInitADO ado = new BedInitADO();
                ado.ListBedColumn = new List<BedColumn>();
                ado.btn_Radio_Enable_Click_Bed = btn_Radio_Enable_Click;
                ado.BedGrid_CellValueChanged = BedGrid_CellValueChanged;
                ado.BedGrid_MouseDown = Bed_MouseDown;
                ado.gridView_MouseRightClick = BedGridView_MouseRightClick;

                BedColumn colRadio1 = new BedColumn("   ", "radio1", 50, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListBedColumn.Add(colRadio1);

                BedColumn colCheck1 = new BedColumn("   ", "check1", 50, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = imageCollection1.Images[0];
                colCheck1.ToolTip = "Chọn tất cả";
                colCheck1.imageAlignment = StringAlignment.Center;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListBedColumn.Add(colCheck1);

                BedColumn colMaGiuong = new BedColumn("Mã giường", "BED_CODE", 100, false);
                colMaGiuong.VisibleIndex = 2;
                ado.ListBedColumn.Add(colMaGiuong);

                BedColumn colTenGiuong = new BedColumn("Tên giường", "BED_NAME", 250, false);
                colTenGiuong.VisibleIndex = 3;
                ado.ListBedColumn.Add(colTenGiuong);

                BedColumn colTenBuong = new BedColumn("Tên buồng", "BED_ROOM_NAME", 250, false);
                colTenBuong.VisibleIndex = 4;
                ado.ListBedColumn.Add(colTenBuong);

                BedColumn colMaKhoa = new BedColumn("Mã khoa", "DEPARTMENT_CODE", 100, false);
                colMaKhoa.VisibleIndex = 5;
                ado.ListBedColumn.Add(colMaKhoa);

                BedColumn colTenKhoa = new BedColumn("Tên khoa", "DEPARTMENT_NAME", 250, false);
                colTenKhoa.VisibleIndex = 6;
                ado.ListBedColumn.Add(colTenKhoa);

                //BedColumn colTenVaiTro = new BedColumn("Tên vai trò", "EXECUTE_ROLE_NAME", 120, false);
                //colTenVaiTro.VisibleIndex = 3;
                //ado.ListBedColumn.Add(colTenVaiTro);

                this.ucGridControlBed = (UserControl)BedProcessor.Run(ado);
                if (ucGridControlBed != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlBed);
                    this.ucGridControlBed.Dock = DockStyle.Fill;
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
                BedServiceTypeProcessor = new BedServiceTypeProcessor();
                BedServiceTypeInitADO ado = new BedServiceTypeInitADO();
                ado.ListBedServiceTypeColumn = new List<BedServiceTypeColumn>();
                ado.btn_Radio_Enable_Click_bsty = btn_Radio_Enable_Click1;
                ado.BedServiceTypeGrid_MouseDown = BedServiceType_MouseDown;
                ado.gridView_MouseRightClick = BedServiceTypeGridView_MouseRightClick;

                BedServiceTypeColumn colRadio2 = new BedServiceTypeColumn("   ", "radio2", 20, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListBedServiceTypeColumn.Add(colRadio2);

                BedServiceTypeColumn colCheck2 = new BedServiceTypeColumn("   ", "check2", 20, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollection1.Images[0];
                colCheck2.ToolTip = "Chọn tất cả";
                //colCheck2.image = StringAlignment.Center;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListBedServiceTypeColumn.Add(colCheck2);

                BedServiceTypeColumn colTenDVGiuong = new BedServiceTypeColumn("Tên dịch vụ", "SERVICE_NAME", 120, false);
                colTenDVGiuong.VisibleIndex = 3;
                ado.ListBedServiceTypeColumn.Add(colTenDVGiuong);

                BedServiceTypeColumn colMaDVGiuong = new BedServiceTypeColumn("Mã dịch vụ", "SERVICE_CODE", 60, false);
                colMaDVGiuong.VisibleIndex = 2;
                ado.ListBedServiceTypeColumn.Add(colMaDVGiuong);

                this.ucGridControlBedServiceType = (UserControl)BedServiceTypeProcessor.Run(ado);
                if (ucGridControlBedServiceType != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlBedServiceType);
                    this.ucGridControlBedServiceType.Dock = DockStyle.Fill;
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
                CommonParam param = new CommonParam();
                MOS.Filter.HisBedTypeFilter BedTypeFilter = new HisBedTypeFilter();
                bedType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_BED_TYPE>>(
                    HisRequestUriStore.HIS_BED_TYPE_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    BedTypeFilter,
                    param);
                LoadDataToComboBedType(cboBedType, bedType);
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
                status.Add(new Status(1, "Giường"));
                status.Add(new Status(2, "Dịch vụ giường"));

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

        private void LoadDataToComboBedType(DevExpress.XtraEditors.GridLookUpEdit cboBedType, List<HIS_BED_TYPE> BedType)
        {
            try
            {
                cboBedType.Properties.DataSource = BedType;
                cboBedType.Properties.DisplayMember = "BED_TYPE_NAME";
                cboBedType.Properties.ValueMember = "ID";

                cboBedType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboBedType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboBedType.Properties.ImmediatePopup = true;
                cboBedType.ForceInitialize();
                cboBedType.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboBedType.Properties.View.Columns.AddField("BED_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboBedType.Properties.View.Columns.AddField("BED_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion


        #region Method


        private void FillDataToGrid2(UC_BedBsty uCBedBsty)
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
                FillDataToGridBsty(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridBsty, param, numPageSize, (GridControl)this.BedServiceTypeProcessor.GetGridControl(this.ucGridControlBedServiceType));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridBsty(object data)
        {
            try
            {
                WaitingManager.Show();
                listBedServiceType = new List<HIS_SERVICE>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                HisServiceFilter BedServiceTypeFilter = new HisServiceFilter();
                BedServiceTypeFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G;
                BedServiceTypeFilter.ORDER_FIELD = "SERVICE_CODE";
                BedServiceTypeFilter.ORDER_DIRECTION = "ASC";
                BedServiceTypeFilter.KEY_WORD = txtSearchBsty.Text;

                if ((long)cboChooseBy.EditValue == 2)
                {
                    isChoseBedServiceType = (long)cboChooseBy.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<HIS_SERVICE>>(
                    HisRequestUriStore.HIS_BSTY_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    BedServiceTypeFilter,
                    param);

                lstBedServiceTypeADOs = new List<HIS.UC.BedServiceType.BedServiceTypeADO>();

                if (sar != null && sar.Data.Count > 0)
                {
                    //List<ACS_USER> dataByAccount = new List<ACS_USER>();
                    listBedServiceType = sar.Data;
                    foreach (var item in listBedServiceType)
                    {
                        HIS.UC.BedServiceType.BedServiceTypeADO BedServiceTypeADO = new HIS.UC.BedServiceType.BedServiceTypeADO(item);
                        if (isChoseBedServiceType == 2)
                        {
                            BedServiceTypeADO.isKeyChoose1 = true;
                        }

                        lstBedServiceTypeADOs.Add(BedServiceTypeADO);
                    }
                }

                if (bedBSTY != null && bedBSTY.Count > 0)
                {
                    foreach (var item in bedBSTY)
                    {
                        var check = lstBedServiceTypeADOs.FirstOrDefault(o => o.ID == item.BED_SERVICE_TYPE_ID);
                        if (check != null)
                        {
                            check.check2 = true;
                        }
                    }
                }
                lstBedServiceTypeADOs = lstBedServiceTypeADOs.OrderByDescending(p => p.check2).ToList();
                if (ucGridControlBedServiceType != null)
                {
                    BedServiceTypeProcessor.Reload(ucGridControlBedServiceType, lstBedServiceTypeADOs);
                }
                rowCount1 = (data == null ? 0 : lstBedServiceTypeADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BedBsty.Resources.Lang", typeof(HIS.Desktop.Plugins.BedBsty.UC_BedBsty).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_BedBsty.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChooseBy.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_BedBsty.cboChooseBy.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_BedBsty.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UC_BedBsty.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchBsty.Text = Inventec.Common.Resource.Get.Value("UC_BedBsty.btnSearchBsty.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchBsty.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_BedBsty.txtSearchBsty.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_BedBsty.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBedType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_BedBsty.cboBedType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSeachBed.Text = Inventec.Common.Resource.Get.Value("UC_BedBsty.btnSeachBed.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchBed.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_BedBsty.txtSearchBed.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_BedBsty.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("UC_BedBsty.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid1(UC_BedBsty uCBedBsty)
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
                FillDataToGridBed(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridBed, param, numPageSize, (GridControl)this.BedProcessor.GetGridControl(this.ucGridControlBed));
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridBed(object data)
        {
            try
            {
                WaitingManager.Show();
                listBed = new List<HIS_BED>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisBedFilter BedFilter = new HisBedFilter();
                BedFilter.ORDER_FIELD = "BED_CODE";
                BedFilter.ORDER_DIRECTION = "ASC";
                BedFilter.KEY_WORD = txtSearchBed.Text;
                if (cboBedType.EditValue != null)
                    BedFilter.BED_TYPE_ID = (long)cboBedType.EditValue;
                if ((long)cboChooseBy.EditValue == 1)
                {
                    isChoseBed = (long)cboChooseBy.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_BED>>(
                     HisRequestUriStore.HIS_BED_GET,
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     BedFilter,
                     param);

                lstBedADOs = new List<HIS.UC.Bed.BedADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_HIS_ROOM> dataByRoom = new List<V_HIS_ROOM>();
                    listBed = rs.Data;
                    foreach (var item in listBed)
                    {
                        HIS.UC.Bed.BedADO BedADO = new HIS.UC.Bed.BedADO(item);
                        if (isChoseBed == 1)
                        {
                            BedADO.isKeyChoose = true;
                        }
                        var bedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().FirstOrDefault(o => o.ID == item.BED_ROOM_ID);
                        if (bedRoom != null)
                        {
                            BedADO.DEPARTMENT_CODE = bedRoom.DEPARTMENT_CODE;
                            BedADO.DEPARTMENT_NAME = bedRoom.DEPARTMENT_NAME;
                            BedADO.DEPARTMENT_ID = bedRoom.DEPARTMENT_ID;
                            BedADO.BED_ROOM_NAME = bedRoom.BED_ROOM_NAME;
                            BedADO.BED_ROOM_CODE = bedRoom.BED_ROOM_CODE;
                        }
                        lstBedADOs.Add(BedADO);
                    }
                }

                if (bedBSTY1 != null && bedBSTY1.Count > 0)
                {
                    foreach (var item in bedBSTY1)
                    {
                        var check = lstBedADOs.FirstOrDefault(o => o.ID == item.BED_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                lstBedADOs = lstBedADOs.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlBed != null)
                {
                    BedProcessor.Reload(ucGridControlBed, lstBedADOs);
                }
                rowCount = (data == null ? 0 : lstBedADOs.Count);
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

        public void Import()
        {
            try
            {
                btnImport.Focus();
                btnImport_Click(null, null);
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
                isChoseBedServiceType = 0;
                isChoseBed = 0;
                bedBSTY = null;
                bedBSTY1 = null;
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
                bool resultSuccess1 = false;
                bool resultSuccess2 = false;
                bool resultSuccess = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                if (ucGridControlBedServiceType != null && ucGridControlBed != null)
                {
                    object bsty = BedServiceTypeProcessor.GetDataGridView(ucGridControlBedServiceType);
                    object bed = BedProcessor.GetDataGridView(ucGridControlBed);
                    if (isChoseBed == 1)
                    {
                        if (bsty is List<HIS.UC.BedServiceType.BedServiceTypeADO>)
                        {
                            var data = (List<HIS.UC.BedServiceType.BedServiceTypeADO>)bsty;
                            if (data != null && data.Count > 0)
                            {
                                //Danh sach cac user duoc check
                                MOS.Filter.HisBedBstyFilter filter = new HisBedBstyFilter();
                                filter.BED_ID = BedIdCheckByBed;

                                var bedBsty = new BackendAdapter(param).Get<List<HIS_BED_BSTY>>(
                                   HisRequestUriStore.HIS_BED_BSTY_GET,
                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                   filter,
                                   param);

                                List<long> listBstyID = bedBsty.Select(p => p.BED_SERVICE_TYPE_ID).ToList();

                                var dataCheckeds = data.Where(p => p.check2 == true).ToList();

                                //List xoa

                                var dataDeletes = data.Where(o => bedBsty.Select(p => p.BED_SERVICE_TYPE_ID)
                                    .Contains(o.ID) && o.check2 == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !bedBsty.Select(p => p.BED_SERVICE_TYPE_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    List<long> deleteIds = bedBsty.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.BED_SERVICE_TYPE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisBedBsty/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteIds,
                                              param);
                                    if (deleteResult)
                                    {
                                        resultSuccess1 = true;
                                        bedBSTY = bedBSTY.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                    }
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_BED_BSTY> bedBstyCreates = new List<HIS_BED_BSTY>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_BED_BSTY bedBstyCreate = new HIS_BED_BSTY();
                                        bedBstyCreate.BED_SERVICE_TYPE_ID = item.ID;
                                        bedBstyCreate.BED_ID = BedIdCheckByBed;
                                        bedBstyCreates.Add(bedBstyCreate);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_BED_BSTY>>(
                                               "api/HisBedBsty/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               bedBstyCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        resultSuccess1 = true;
                                        bedBSTY.AddRange(createResult);
                                    }
                                }

                                data = data.OrderByDescending(p => p.check2).ToList();
                                if (ucGridControlBedServiceType != null)
                                {
                                    BedServiceTypeProcessor.Reload(ucGridControlBedServiceType, data);
                                }
                            }
                        }
                    }
                    if (isChoseBedServiceType == 2)
                    {
                        if (bed is List<HIS.UC.Bed.BedADO>)
                        {
                            var data = (List<HIS.UC.Bed.BedADO>)bed;

                            if (data != null && data.Count > 0)
                            {
                                //Danh sach cac user duoc check
                                MOS.Filter.HisBedBstyFilter filter = new HisBedBstyFilter();
                                filter.BED_SERVICE_TYPE_ID = BstyIdCheckByBsty;
                                var bedBsty = new BackendAdapter(param).Get<List<HIS_BED_BSTY>>(
                                   HisRequestUriStore.HIS_BED_BSTY_GET,
                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                   filter,
                                   param);

                                var listBstyID = bedBsty.Select(p => p.BED_ID).ToList();

                                var dataChecked = data.Where(p => p.check1 == true).ToList();

                                //List xoa

                                var dataDelete = data.Where(o => bedBsty.Select(p => p.BED_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !bedBsty.Select(p => p.BED_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataDelete != null && dataDelete.Count > 0)
                                {
                                    List<long> deleteId = bedBsty.Where(o => dataDelete.Select(p => p.ID)

                                        .Contains(o.BED_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisBedBsty/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                    {
                                        resultSuccess2 = true;
                                        bedBSTY1 = bedBSTY1.Where(o => !deleteId.Contains(o.ID)).ToList();
                                    }

                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_BED_BSTY> bedBstyCreate = new List<HIS_BED_BSTY>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_BED_BSTY BhtyID = new HIS_BED_BSTY();
                                        BhtyID.BED_ID = item.ID;
                                        BhtyID.BED_SERVICE_TYPE_ID = BstyIdCheckByBsty;
                                        bedBstyCreate.Add(BhtyID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_BED_BSTY>>(
                                               "api/HisBedBsty/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               bedBstyCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        resultSuccess2 = true;
                                        bedBSTY1.AddRange(createResult);
                                    }
                                }

                                data = data.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlBed != null)
                                {
                                    BedProcessor.Reload(ucGridControlBed, data);
                                }

                            }
                        }
                    }
                }
                if (resultSuccess1 == true || resultSuccess2 == true)
                    resultSuccess = true;
                WaitingManager.Hide();
                #region Show message
                MessageManager.ShowAlert(this.ParentForm, param, resultSuccess);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BedGrid_CellValueChanged(BedADO data, CellValueChangedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Bed_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseBed == 1)
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
                            var lstCheckAll = lstBedADOs;
                            List<HIS.UC.Bed.BedADO> lstChecks = new List<HIS.UC.Bed.BedADO>();

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
                                    hi.Column.Image = imageCollection1.Images[1];
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
                                    hi.Column.Image = imageCollection1.Images[0];
                                }

                                BedProcessor.Reload(ucGridControlBed, lstChecks);
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

        private void BedServiceType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseBedServiceType == 2)
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
                            var lstCheckAll = lstBedServiceTypeADOs;
                            List<HIS.UC.BedServiceType.BedServiceTypeADO> lstChecks = new List<HIS.UC.BedServiceType.BedServiceTypeADO>();

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
                                    hi.Column.Image = imageCollection1.Images[1];
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
                                    hi.Column.Image = imageCollection1.Images[0];
                                }

                                //ReloadData
                                BedServiceTypeProcessor.Reload(ucGridControlBedServiceType, lstChecks);
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

        private void btn_Radio_Enable_Click(HIS_BED data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisBedBstyFilter filter = new HisBedBstyFilter();
                filter.BED_ID = data.ID;
                BedIdCheckByBed = data.ID;
                bedBSTY = new BackendAdapter(param).Get<List<HIS_BED_BSTY>>(
                                HisRequestUriStore.HIS_BED_BSTY_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.BedServiceType.BedServiceTypeADO> dataNew = new List<HIS.UC.BedServiceType.BedServiceTypeADO>();
                dataNew = (from r in listBedServiceType select new HIS.UC.BedServiceType.BedServiceTypeADO(r)).ToList();
                if (bedBSTY != null && bedBSTY.Count > 0)
                {
                    foreach (var itemUsername in bedBSTY)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.BED_SERVICE_TYPE_ID);
                        if (check != null)
                        {
                            check.check2 = true;
                        }

                    }
                }

                dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
                if (ucGridControlBedServiceType != null)
                {
                    BedServiceTypeProcessor.Reload(ucGridControlBedServiceType, dataNew);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click1(HIS_SERVICE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisBedBstyFilter filter = new HisBedBstyFilter();
                filter.BED_SERVICE_TYPE_ID = data.ID;
                BstyIdCheckByBsty = data.ID;
                bedBSTY1 = new BackendAdapter(param).Get<List<HIS_BED_BSTY>>(
                                HisRequestUriStore.HIS_BED_BSTY_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Bed.BedADO> dataNew = new List<HIS.UC.Bed.BedADO>();
                dataNew = (from r in listBed select new HIS.UC.Bed.BedADO(r)).ToList();
                foreach (var item in dataNew)
                {
                    var bedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().FirstOrDefault(o => o.ID == item.BED_ROOM_ID);
                    if (bedRoom != null)
                    {
                        item.DEPARTMENT_CODE = bedRoom.DEPARTMENT_CODE;
                        item.DEPARTMENT_NAME = bedRoom.DEPARTMENT_NAME;
                        item.DEPARTMENT_ID = bedRoom.DEPARTMENT_ID;
                        item.BED_ROOM_NAME = bedRoom.BED_ROOM_NAME;
                        item.BED_ROOM_CODE = bedRoom.BED_ROOM_CODE;
                    }
                }
                if (bedBSTY1 != null && bedBSTY1.Count > 0)
                {

                    foreach (var itemUsername in bedBSTY1)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.BED_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    if (ucGridControlBed != null)
                    {
                        BedProcessor.Reload(ucGridControlBed, dataNew);
                    }
                }
                else
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

        private void txtSearchBed_KeyDown(object sender, KeyEventArgs e)
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

        private void txtSearchBsty_KeyDown(object sender, KeyEventArgs e)
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

        private void cboBedType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                cboBedType.Properties.Buttons[1].Visible = true;
                cboBedType.EditValue = null;
            }
        }

        private void BedGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.Bed.BedADO)
                {
                    var type = (HIS.UC.Bed.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.Bed.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseBed != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn giường!");
                                    break;
                                }
                                this.currentCopyBedAdo = (HIS.UC.Bed.BedADO)sender;
                                break;
                            }
                        case HIS.UC.Bed.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.Bed.BedADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyBedAdo == null && isChoseBed != 1)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyBedAdo != null && currentPaste != null && isChoseBed == 1)
                                {
                                    if (this.currentCopyBedAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisBedBstyCopyByBedSDO hisMestMatyCopyByMatySDO = new HisBedBstyCopyByBedSDO();
                                    hisMestMatyCopyByMatySDO.CopyBedId = this.currentCopyBedAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteBedId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_BED_BSTY>>("api/HisBedBsty/CopyByBed", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        bedBSTY = result;
                                        List<HIS.UC.BedServiceType.BedServiceTypeADO> dataNew = new List<HIS.UC.BedServiceType.BedServiceTypeADO>();
                                        dataNew = (from r in listBedServiceType select new HIS.UC.BedServiceType.BedServiceTypeADO(r)).ToList();
                                        if (bedBSTY != null && bedBSTY.Count > 0)
                                        {
                                            foreach (var itemUsername in bedBSTY)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.BED_SERVICE_TYPE_ID);
                                                if (check != null)
                                                {
                                                    check.check2 = true;
                                                }

                                            }
                                        }

                                        dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
                                        if (ucGridControlBedServiceType != null)
                                        {
                                            BedServiceTypeProcessor.Reload(ucGridControlBedServiceType, dataNew);
                                        }
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BedServiceTypeGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.BedServiceType.BedServiceTypeADO)
                {
                    var type = (HIS.UC.BedServiceType.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.BedServiceType.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseBedServiceType != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn loại dịch vụ giường!");
                                    break;
                                }
                                this.currentCopyBedServiceTypeAdo = (HIS.UC.BedServiceType.BedServiceTypeADO)sender;
                                break;
                            }
                        case HIS.UC.BedServiceType.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.BedServiceType.BedServiceTypeADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyBedServiceTypeAdo == null && isChoseBedServiceType != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyBedServiceTypeAdo != null && currentPaste != null && isChoseBedServiceType == 2)
                                {
                                    if (this.currentCopyBedServiceTypeAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisBedBstyCopyByBstySDO hisMestMatyCopyByMatySDO = new HisBedBstyCopyByBstySDO();
                                    hisMestMatyCopyByMatySDO.CopyBedServiceTypeId = this.currentCopyBedServiceTypeAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteBedServiceTypeId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_BED_BSTY>>("api/HisBedBsty/CopyByBsty", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        bedBSTY1 = result;
                                        List<HIS.UC.Bed.BedADO> dataNew = new List<HIS.UC.Bed.BedADO>();
                                        dataNew = (from r in listBed select new HIS.UC.Bed.BedADO(r)).ToList();
                                        if (bedBSTY1 != null && bedBSTY1.Count > 0)
                                        {

                                            foreach (var itemUsername in bedBSTY1)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.BED_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                                            if (ucGridControlBed != null)
                                            {
                                                BedProcessor.Reload(ucGridControlBed, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid1(this);
                                        }
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)RefreshData);
                if (this.currentModule != null)
                {
                    CallModule callModule = new CallModule(CallModule.HisImportHisBedBsty, currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisImportHisBedBsty, 0, 0, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public void RefreshData()
        {
            try
            {
                if (this.currentService == null && this.Bed == null)
                {
                    FillDataToGrid1(this);
                    FillDataToGrid2(this);
                }
                else if (this.Bed == null)
                {
                    cboChooseBy.EditValue = (long)1;
                    cboChooseBy.Enabled = false;
                    FillDataToGridBed(this);
                    FillDataToGrid2(this);
                    btn_Radio_Enable_Click1(this.currentService);
                }
                else if (this.currentService == null)
                {
                    cboChooseBy.EditValue = (long)2;
                    cboChooseBy.Enabled = false;
                    FillDataToGrid1(this);
                    FillDataToGridBsty(this);
                    btn_Radio_Enable_Click(this.Bed);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }


    }

}

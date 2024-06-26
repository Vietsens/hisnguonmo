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
using HIS.UC.MedicineTypeAcinGrid;
using HIS.UC.MedicineTypeAcinGrid.ADO;
using HIS.UC.MedicineTypeAcinGrid.Reload;
using HIS.UC.MedicineTypeAcinGrid.Run;
using HIS.UC.MedicineTypeGrid;
using HIS.UC.MedicineTypeGrid.ADO;
using HIS.UC.MedicineTypeGrid.Reload;
using HIS.UC.MedicineTypeGrid.Run;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraGrid.Columns;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.SetupMedicineTypeWithAcin.ADO;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;




namespace HIS.Desktop.Plugins.SetupMedicineTypeWithAcin
{
    public partial class UC_SetupMedicineTypeWithAcin : UserControl
    {
        #region Declare

        List<HIS_MEDICINE_TYPE> _listMedicineType { get; set; }

        MedicineTypeAcinGridProcessor medicineTypeAcinProcessor;
        MedicineTypeAcinGridInitADO medicineTypeAcinADO;
        V_HIS_MEDICINE_TYPE_ACIN medicineTypeAcin;
        List<V_HIS_MEDICINE_TYPE_ACIN> listMedicinTypeAcin;
        UserControl ucMedicineTypeAcin;
        internal List<HIS.Desktop.ADO.MedicineTypeAcinADO> lstMedicineTypeAcinADOs { get; set; }       


        MedicineTypeGridInitADO medicineTypeADO;
        MedicineTypeGridProcessor medicineTypeProcessor;
        V_HIS_MEDICINE_TYPE medicineType;
        List<V_HIS_MEDICINE_TYPE> listMedicinType;
        UserControl ucMedicineType = null;
        internal List<HIS.Desktop.ADO.MedicineTypeADO> lstMedicineTypeADOs { get; set; }


        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;

        #endregion

        #region Constructor
        public UC_SetupMedicineTypeWithAcin()
        {
            InitializeComponent();
        }

        private void UC_SetupMedicineTypeWithAcin_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                LoadDataToCombo();
                InitMedicineTypeGrid();
                InitMedicineTypeAcinGrid();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SetupMedicineTypeWithAcin.Resources.Lang", typeof(HIS.Desktop.Plugins.SetupMedicineTypeWithAcin.UC_SetupMedicineTypeWithAcin).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChooseBy.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.cboChooseBy.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchMTA.Text = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.btnSearchMTA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeywordMTA.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.txtKeywordMTA.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMedicineType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.cboMedicineType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchMT.Text = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.btnSearchMT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeywordMT.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.txtKeywordMT.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UC_SetupMedicineTypeWithAcin.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region LoadColumn

        private void InitMedicineTypeAcinGrid()
        {
            try
            {

                this.medicineTypeAcinProcessor = new MedicineTypeAcinGridProcessor();
                medicineTypeAcinADO = new MedicineTypeAcinGridInitADO();

                medicineTypeAcinADO.ListMedicineTypeAcinColumn = new List<MedicineTypeAcinGridColumn>();

                MedicineTypeAcinGridColumn colRadio1 = new MedicineTypeAcinGridColumn("", "radio2", 30, true, false);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                medicineTypeAcinADO.ListMedicineTypeAcinColumn.Add(colRadio1);

                MedicineTypeAcinGridColumn colCheck1 = new MedicineTypeAcinGridColumn("", "check2", 30, true, false);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                medicineTypeAcinADO.ListMedicineTypeAcinColumn.Add(colCheck1);

                MedicineTypeAcinGridColumn colMaHoatChat = new MedicineTypeAcinGridColumn("Mã hoạt chất", "ACTIVE_INGREDIENT_CODE", 123, false, true);
                colMaHoatChat.VisibleIndex = 2;
                medicineTypeAcinADO.ListMedicineTypeAcinColumn.Add(colMaHoatChat);

                MedicineTypeAcinGridColumn colTenHoatChat = new MedicineTypeAcinGridColumn("Tên hoạt chất", "ACTIVE_INGREDIENT_NAME", 500, false, true);
                colTenHoatChat.VisibleIndex = 3;
                medicineTypeAcinADO.ListMedicineTypeAcinColumn.Add(colTenHoatChat);



                this.ucMedicineTypeAcin = (UserControl)this.medicineTypeAcinProcessor.Run(medicineTypeAcinADO);
                if (this.ucMedicineTypeAcin != null)
                {
                    this.pnlMedicineTypeAcin.Controls.Add(this.ucMedicineTypeAcin);
                    this.ucMedicineTypeAcin.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMedicineTypeGrid()
        {
            try
            {

                this.medicineTypeProcessor = new MedicineTypeGridProcessor();
                medicineTypeADO = new MedicineTypeGridInitADO();

                medicineTypeADO.ListMedicineTypeColumn = new List<MedicineTypeGridColumn>();

                MedicineTypeGridColumn colRadio1 = new MedicineTypeGridColumn("", "radio1", 30, true, false);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                medicineTypeADO.ListMedicineTypeColumn.Add(colRadio1);

                MedicineTypeGridColumn colCheck1 = new MedicineTypeGridColumn("", "check1", 30, true, false);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                medicineTypeADO.ListMedicineTypeColumn.Add(colCheck1);

                MedicineTypeGridColumn colMaLoaiThuoc = new MedicineTypeGridColumn("Mã loại thuốc","MEDICINE_TYPE_CODE", 123, false, true);
                colMaLoaiThuoc.VisibleIndex = 2;
                medicineTypeADO.ListMedicineTypeColumn.Add(colMaLoaiThuoc);

                MedicineTypeGridColumn colTenLoaiThuoc = new MedicineTypeGridColumn("Tên loại thuốc","MEDICINE_TYPE_NAME", 500, false, true);
                colTenLoaiThuoc.VisibleIndex = 3;
                medicineTypeADO.ListMedicineTypeColumn.Add(colTenLoaiThuoc);



                this.ucMedicineType = (UserControl)this.medicineTypeProcessor.Run(medicineTypeADO);
                if (this.ucMedicineType != null)
                {
                    this.pnlMedicineType.Controls.Add(this.ucMedicineType);
                    this.ucMedicineType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion      

        #region Event

        private void btnSearchMT_Click(object sender, EventArgs e)
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

        private void btnSearchMTA_Click(object sender, EventArgs e)
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

        private void cboChooseBy_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                FillDataToGrid1(this);
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void btn_Radio_Enable_Click(V_HIS_ROOM data)
        //{
        //    try
        //    {
        //        WaitingManager.Show();
        //        CommonParam param = new CommonParam();
        //        MOS.Filter.HisUserRoomFilter filter = new HisUserRoomFilter();
        //        filter.ROOM_ID = data.ID;
        //        roomIdCheckByRoom = data.ID;
        //        var output = new BackendAdapter(param).Get<List<HIS_USER_ROOM>>(
        //                        HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_USER_ROOM_GET,
        //                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
        //                        filter,
        //                        param);
        //        List<HIS.Desktop.ADO.MedicineTypeADO> dataNew = new List<HIS.Desktop.ADO.MedicineTypeADO>();
        //        dataNew = (from r in listUser select new AccountADO(r)).ToList();
        //        if (output != null && output.Count > 0)
        //        {
        //            foreach (var itemUsername in output)
        //            {
        //                var check = dataNew.FirstOrDefault(o => o.LOGINNAME == itemUsername.LOGINNAME);
        //                if (check != null)
        //                {
        //                    check.check2 = true;
        //                }
        //            }
        //        }

        //        dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
        //        if (ucMedicineTypeAcin != null)
        //        {
        //            AccountProcessor.Reload(ucMedicineTypeAcin, dataNew);
        //        }
        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void btn_Radio_Enable_Click1(V_HIS_MEDICINE_TYPE_ACIN data)
        //{
        //    try
        //    {
        //        WaitingManager.Show();
        //        CommonParam param = new CommonParam();
        //        MOS.Filter.HisMedicineTypeFilter filter = new HisMedicineTypeFilter();
        //        filter.LOGINNAME = data.LOGINNAME;
        //        var output = new BackendAdapter(param).Get<List<V_HIS_USER_ROOM>>(
        //                        HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_USER_ROOM_GETVIEW,
        //                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
        //                        filter,
        //                        param);
        //        List<HIS.Desktop.ADO.RoomAccountADO> dataNew = new List<RoomAccountADO>();
        //        dataNew = (from r in listRoom select new RoomAccountADO(r)).ToList();
        //        if (output != null && output.Count > 0)
        //        {

        //            foreach (var itemUsername in output)
        //            {
        //                var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
        //                if (check != null)
        //                {
        //                    check.check1 = true;
        //                }
                        
        //            }
        //            //Reload lại from UC User
        //            //List<HIS.Desktop.ADO.AccountADO> dataNew = new List<AccountADO>();
        //            dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
        //            if (ucMedicineType != null)
        //            {
        //                medicineTypeProcessor.Reload(ucMedicineType, dataNew);
        //            }
        //        }
        //        else
        //        {
        //            FillDataToGrid1(this);
        //        }
        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Load Data To Combo

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineTypeFilter MedicineTypeFilter = new HisMedicineTypeFilter();
                _listMedicineType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MEDICINE_TYPE>>(
                    HisRequestUriStore.HIS_MEDICINE_TYPE_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    MedicineTypeFilter,
                    param);
                LoadDataToComboMedicineType(cboMedicineType, _listMedicineType);
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
                List<StatusADO> status = new List<StatusADO>();
                status.Add(new StatusADO(1, "Loại thuốc"));
                status.Add(new StatusADO(2, "Hoạt chất"));

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

        private void LoadDataToComboMedicineType(DevExpress.XtraEditors.GridLookUpEdit cboMedicineType, List<HIS_MEDICINE_TYPE> _listMedicineType)
        {
            try
            {
                cboMedicineType.Properties.DataSource = _listMedicineType;
                cboMedicineType.Properties.DisplayMember = "MEDICINE_TYPE_NAME";
                cboMedicineType.Properties.ValueMember = "ID";

                cboMedicineType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboMedicineType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboMedicineType.Properties.ImmediatePopup = true;
                cboMedicineType.ForceInitialize();
                cboMedicineType.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboMedicineType.Properties.View.Columns.AddField("MEDICINE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboMedicineType.Properties.View.Columns.AddField("MEDICINE_TYPE_NAME");
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

        private void FillDataToGridMedicineType(object data)
        {
            try
            {
                WaitingManager.Show();
                listMedicinType = new List<V_HIS_MEDICINE_TYPE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisMedicineTypeFilter MedicineTypeFilter = new HisMedicineTypeFilter();
                //MedicineTypeFilter.ORDER_FIELD = "DEPARTMENT_NAME";
                MedicineTypeFilter.ORDER_DIRECTION = "MOS";
                MedicineTypeFilter.KEY_WORD = txtKeywordMT.Text;
                if (cboMedicineType.EditValue != null)
                    MedicineTypeFilter.ID = (long)cboMedicineType.EditValue;
                long isChoose = 0;

                if ((long)cboChooseBy.EditValue == 1)
                {
                    isChoose = (long)cboChooseBy.EditValue;
                }
                if ((long)cboChooseBy.EditValue == 2)
                {
                    isChoose = (long)cboChooseBy.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>>(
                     HisRequestUriStore.HIS_MEDICINE_TYPE_GETVIEW,
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     MedicineTypeFilter,
                     param);

                lstMedicineTypeADOs = new List<HIS.Desktop.ADO.MedicineTypeADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_HIS_ROOM> dataByRoom = new List<V_HIS_ROOM>();
                    listMedicinType = rs.Data;
                    foreach (var item in listMedicinType)
                    {
                        HIS.Desktop.ADO.MedicineTypeADO medicineTypeADO = new HIS.Desktop.ADO.MedicineTypeADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS.Desktop.ADO.MedicineTypeADO>(medicineTypeADO, item);
                        if (isChoose == 1)
                        {
                            medicineTypeADO.isKeyChoose = true;
                        }
                        lstMedicineTypeADOs.Add(medicineTypeADO);
                    }
                }
                if (ucMedicineType != null)
                {
                    medicineTypeProcessor.Reload(ucMedicineType, lstMedicineTypeADOs);
                }
                rowCount = (data == null ? 0 : lstMedicineTypeADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillDataToGridMedicineTypeAcin(object data)
        {
            try
            {
                WaitingManager.Show();
                listMedicinTypeAcin = new List<V_HIS_MEDICINE_TYPE_ACIN>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                HisMedicineTypeAcinFilter medicineTypeAcinFilter = new HisMedicineTypeAcinFilter();
                //medicineTypeAcinFilter.ORDER_FIELD = "DEPARTMENT_NAME";
                medicineTypeAcinFilter.ORDER_DIRECTION = "MOS";
                medicineTypeAcinFilter.KEY_WORD = txtKeywordMTA.Text;

                long isChoose = 0;

                if ((long)cboChooseBy.EditValue == 2)
                {
                    isChoose = (long)cboChooseBy.EditValue;
                }
                if ((long)cboChooseBy.EditValue == 1)
                {
                    isChoose = (long)cboChooseBy.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<V_HIS_MEDICINE_TYPE_ACIN>>(
                    HisRequestUriStore.HIS_MEDICINE_TYPE_ACIN_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    medicineTypeAcinFilter,
                    param);

                lstMedicineTypeAcinADOs = new List<HIS.Desktop.ADO.MedicineTypeAcinADO>();

                if (sar != null && sar.Data.Count > 0)
                {
                    //List<ACS_USER> dataByAccount = new List<ACS_USER>();
                    listMedicinTypeAcin = sar.Data;
                    foreach (var item in listMedicinTypeAcin)
                    {
                        HIS.Desktop.ADO.MedicineTypeAcinADO MedicineTypeAcinADO = new HIS.Desktop.ADO.MedicineTypeAcinADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS.Desktop.ADO.MedicineTypeAcinADO>(MedicineTypeAcinADO, item);
                        

                        if (isChoose == 2)
                        {
                            MedicineTypeAcinADO.isKeyChoose1 = true;
                        }

                        lstMedicineTypeAcinADOs.Add(MedicineTypeAcinADO);
                    }
                }
                if (ucMedicineTypeAcin != null)
                {
                    medicineTypeAcinProcessor.Reload(ucMedicineTypeAcin, lstMedicineTypeAcinADOs);
                }
                rowCount1 = (data == null ? 0 : lstMedicineTypeAcinADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillDataToGrid1(UC_SetupMedicineTypeWithAcin uCMedicinTypeWA)
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
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridMedicineType(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridMedicineType, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2(UC_SetupMedicineTypeWithAcin uCMedicinTypeWA)
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
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridMedicineTypeAcin(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging3.Init(FillDataToGridMedicineTypeAcin, param);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

    }
}

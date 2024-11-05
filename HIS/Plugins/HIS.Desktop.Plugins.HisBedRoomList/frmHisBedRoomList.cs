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
using AutoMapper;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisBedRoomList.ADO;
using HIS.Desktop.Plugins.HisBedRoomList.Validation;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisBedRoomList
{
    public partial class frmHisBedRoomList : Form//HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module currentModule;
        string loginName = null;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        private const short IS_SURGERY_TRUE = 1;
        private const short IS_SURGERY_FALSE = 0;
        private const short IS_PAUSE_TRUE = 1;
        private const short IS_PAUSE_FALSE = 0;
        int ActionType = GlobalVariables.ActionAdd;
        int positionHandleControl = -1;
        V_HIS_BED_ROOM currentBedRoom = new V_HIS_BED_ROOM();
        List<HIS_TREATMENT_TYPE> listTreatmentTypeIds = new List<HIS_TREATMENT_TYPE>();
        List<HIS_TREATMENT_TYPE> treatmentTypeIdSelecteds = new List<HIS_TREATMENT_TYPE>();
        List<HIS_PATIENT_TYPE> listPatientTypeIds = new List<HIS_PATIENT_TYPE>();
        #endregion
        #region Construct
        public frmHisBedRoomList()
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmHisBedRoomList(Inventec.Desktop.Common.Modules.Module module)
        //: base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = module;
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        #region Private method
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void frmHisBedRoomList_Load(object sender, EventArgs e)
        {

            try
            {
                WaitingManager.Show();
                this.ValidControl();
                LoadDefault();//xoa het combo
                FillDataToGrid();
                var data = BackendDataWorker.Get<HIS_DEPARTMENT>();
                var datas = BackendDataWorker.Get<HIS_AREA>();
                List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> data1 = new List<HIS_DEPARTMENT>();
                List<MOS.EFMODEL.DataModels.HIS_AREA> data2 = new List<HIS_AREA>();
                foreach (var item in data)
                {
                    if (item.IS_ACTIVE == 1)
                    {
                        data1.Add(item);

                    }
                }
                foreach (var items in datas)
                {

                    if (items.IS_ACTIVE == 1)
                    {

                        data2.Add(items);
                    }
                }
                LoadDatatoDepartment(this.cboDepartment, data1);
                LoadDatatoKhuVuc(this.cboKhuVuc, data2);
                InitComboSpeciality();
                InitComboCashierRoom();
                InitComboDefaultsCLS();
                InitComboTreatmentTypeIds();
                InitComboAccountBook();
                //InitComboControlConfig();
                SetCaptionByLanguageKey();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void InitComboCashierRoom()
        {
            try
            {
                HIS_DEPARTMENT currentDepartment = new HIS_DEPARTMENT();
                if (cboDepartment.EditValue != null)
                {
                    currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "0").ToString())).FirstOrDefault();
                }
                var datass = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.BRANCH_ID == currentDepartment.BRANCH_ID).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CASHIER_ROOM_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboCashierRoom, datass, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboAccountBook()
        {
            try
            {

                var datass = BackendDataWorker.Get<HIS_ACCOUNT_BOOK>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE 
                    && o.IS_FOR_DEPOSIT == 1 
                    && o.IS_NOT_GEN_TRANSACTION_ORDER != 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboAccountBook, datass, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboDefaultsCLS()
        {
            try
            {
                HIS_PATIENT_TYPE currentDefaultsCLS = new HIS_PATIENT_TYPE();

                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listPatientTypeIds = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                var filteredList = listPatientTypeIds.Select(item => new
                {
                    ID = item.ID,
                    PATIENT_TYPE_CODE = item.PATIENT_TYPE_CODE,
                    PATIENT_TYPE_NAME = item.PATIENT_TYPE_NAME
                }).ToList();
                //var filteredList = listPatientTypeIds.Select(item => item.PATIENT_TYPE_NAME).ToList();

                if (filteredList != null)
                {

                    cboDefaultsCLS.Properties.DataSource = filteredList;
                    cboDefaultsCLS.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                    cboDefaultsCLS.Properties.ValueMember = "ID";

                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboDefaultsCLS.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                    col2.VisibleIndex = 1;
                    col2.Width = 100;
                    col2.Caption = "";
                    DevExpress.XtraGrid.Columns.GridColumn col3 = cboDefaultsCLS.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                    col3.VisibleIndex = 2;
                    col3.Width = 200;
                    col3.Caption = "";
                }
                cboDefaultsCLS.Properties.View.OptionsView.ShowColumnHeaders = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }


        private void InitComboTreatmentTypeIds()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboTreatmentTypeIds.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboTreatmentTypeIds);
                cboTreatmentTypeIds.Properties.Tag = gridCheck;
                cboTreatmentTypeIds.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                HisTreatmentTypeFilter filter = new HisTreatmentTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listTreatmentTypeIds = new BackendAdapter(param).Get<List<HIS_TREATMENT_TYPE>>("api/HisTreatmentType/Get", ApiConsumers.MosConsumer, filter, null).ToList();

                if (listTreatmentTypeIds != null)
                {
                    cboTreatmentTypeIds.Properties.DataSource = listTreatmentTypeIds;
                    cboTreatmentTypeIds.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
                    cboTreatmentTypeIds.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboTreatmentTypeIds.Properties.View.Columns.AddField("TREATMENT_TYPE_CODE");
                    col2.VisibleIndex = 1;
                    col2.Width = 100;
                    col2.Caption = "";
                    DevExpress.XtraGrid.Columns.GridColumn col3 = cboTreatmentTypeIds.Properties.View.Columns.AddField("TREATMENT_TYPE_NAME");
                    col3.VisibleIndex = 2;
                    col3.Width = 200;
                    col3.Caption = "";

                    cboTreatmentTypeIds.Properties.PopupFormWidth = 200;
                    cboTreatmentTypeIds.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboTreatmentTypeIds.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboTreatmentTypeIds.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboTreatmentTypeIds.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboTreatmentTypeIds(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                treatmentTypeIdSelecteds = new List<HIS_TREATMENT_TYPE>();
                if (gridCheckMark != null)
                {
                    List<HIS_TREATMENT_TYPE> erSelectedNews = new List<HIS_TREATMENT_TYPE>();
                    foreach (HIS_TREATMENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.TREATMENT_TYPE_NAME + ",";
                    }
                    cboTreatmentTypeIds.Text = typeName;
                    cboTreatmentTypeIds.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDatatoKhuVuc(DevExpress.XtraEditors.GridLookUpEdit comboBoxEdit, object _data)
        {

            try
            {
                var departmentsS = _data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("AREA_CODE", "", 90, 1));
                columnInfos.Add(new ColumnInfo("AREA_NAME", "", 350, 2));
                ControlEditorADO controlEditorADOs = new ControlEditorADO("AREA_NAME", "ID", columnInfos, false, 440);

                ControlEditorLoader.Load(cboKhuVuc, departmentsS, controlEditorADOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("LoadDatatoKhuVuc" + ex);
            }

        }

        private void InitComboSpeciality()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSpecialityFilter filter = new HisSpecialityFilter();
                filter.IS_ACTIVE = 1;
                List<HIS_SPECIALITY> data = new BackendAdapter(param).Get<List<HIS_SPECIALITY>>("api/HisSpeciality/Get", ApiConsumers.MosConsumer, filter, param);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SPECIALITY_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SPECIALITY_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SPECIALITY_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChuyenKhoa, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBedRoomList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBedRoomList.frmHisBedRoomList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IsLock.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.IsLock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Delete.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BedRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.BedRoomCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BedRoomName.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.BedRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.RoomTypeName.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.RoomTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.DepartmentName.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.DepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IsSurgery.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.IsSurgery.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.IsPauce.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.IsPauce.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Status.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.Status.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Create_time.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.Create_time.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Creator.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.Creator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ModifiTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.ModifiTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Modifier.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.Modifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChuyenKhoa.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.cboChuyenKhoa.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem_Import.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.barButtonItem_Import.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.barButtonItem4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bar3.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.bar3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsPause.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.chkIsPause.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsSurgery.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.chkIsSurgery.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboKhuVuc.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBedRoomList.cboKhuVuc.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region validate
        private void ValidControl()
        {
            try
            {
                ValidBedRoomCode();
                ValidBedRoomName();
                ValidDepartment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidBedRoomCode()
        {
            try
            {
                BedRoomCodeValidationRule BedRoomCode = new BedRoomCodeValidationRule();
                BedRoomCode.txtBedRoomCode = this.txtBedRoomCode;
                BedRoomCode.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtBedRoomCode, BedRoomCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidBedRoomName()
        {
            try
            {
                BedRoomNameValidationRule BedRoomName = new BedRoomNameValidationRule();
                BedRoomName.txtBedRoomName = this.txtBedRoomName;
                dxValidationProvider1.SetValidationRule(this.txtBedRoomName, BedRoomName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidDepartment()
        {
            try
            {
                DepartmentValidationRule Department = new DepartmentValidationRule();
                Department.cboDepartment = this.cboDepartment;
                dxValidationProvider1.SetValidationRule(this.cboDepartment, Department);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        private void LoadDefault()
        {
            try
            {
                this.txtBedRoomCode.ReadOnly = false;
                this.cboDepartment.ReadOnly = false;
                this.txtBedRoomCode.Text = "";
                this.txtBedRoomName.Text = "";
                this.chkIsPause.Checked = this.chkIsSurgery.Checked = false;
                this.chkIsRestrictReqService.Checked = false;
                this.cboDepartment.EditValue = null;
                this.cboTreatmentTypeIds.EditValue = null;
                this.cboCashierRoom.EditValue = null;
                this.cboDefaultsCLS.EditValue = null;
                this.cboAccountBook.EditValue = null;
                this.checkEdit1.Checked = false;
                this.cboChuyenKhoa.EditValue = null;
                this.cboKhuVuc.EditValue = null;
                txtConfig.Text = "";
                txtConfig.ReadOnly = true;
                cboAccountBook.EditValue = null;
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                this.txtBedRoomCode.Focus();
                GridCheckMarksSelection gridCheckMarkTreatmentTypeIds = cboTreatmentTypeIds.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkTreatmentTypeIds.ClearSelection(cboTreatmentTypeIds.Properties.View);
                cboTreatmentTypeIds.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
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
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridBedRoom(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridBedRoom, param, numPageSize, this.gridControlBedRoom);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void FillDataToGridBedRoom(object param)
        {
            try
            {
                List<V_HIS_BED_ROOM> listData = new List<V_HIS_BED_ROOM>();
                gridControlBedRoom.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 10;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisBedRoomViewFilter filter = new HisBedRoomViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                if (filter.KEY_WORD == "") paramCommon = new CommonParam(start, limit);
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_BED_ROOM>>(RequestUriStore.HIS_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    listData = (List<V_HIS_BED_ROOM>)result.Data;
                    rowCount = (listData == null ? 0 : listData.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);

                }

                var listDataSDO = (from r in listData select new VHisBedRoomADO(r)).ToList();
                gridControlBedRoom.BeginUpdate();
                gridControlBedRoom.DataSource = listDataSDO;
                gridControlBedRoom.EndUpdate();


                //gridControlBedRoom .DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDatatoDepartment(DevExpress.XtraEditors.GridLookUpEdit cboDepartment, object data)
        {
            try
            {
                var departments = data;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 90, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 350, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 440);
                ControlEditorLoader.Load(cboDepartment, departments, controlEditorADO);
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
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnDelete_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                V_HIS_BED_ROOM dataBedRoom = (V_HIS_BED_ROOM)gridViewListBedRoom.GetFocusedRow();
                if (MessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    HIS_BED_ROOM BedRoom = new HIS_BED_ROOM();
                    BedRoom.ID = dataBedRoom.ID;
                    BedRoom.ROOM_ID = dataBedRoom.ROOM_ID;
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_BED_ROOM_DELETE, ApiConsumers.MosConsumer, BedRoom, param);

                    if (success)
                    {
                        BackendDataWorker.Reset<HIS_BED_ROOM>();
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();

                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_BED_ROOM success = new HIS_BED_ROOM();
            bool notHandler = false;
            try
            {

                V_HIS_BED_ROOM dataBedRoom = (V_HIS_BED_ROOM)gridViewListBedRoom.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_BED_ROOM data1 = new HIS_BED_ROOM();
                    data1.ID = dataBedRoom.ID;
                    WaitingManager.Show();

                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BED_ROOM>(RequestUriStore.HIS_BED_ROOM_CHANGELOCK, ApiConsumers.MosConsumer, data1, param);

                    WaitingManager.Hide();
                    if (success != null) FillDataToGrid();
                    BackendDataWorker.Reset<HIS_BED_ROOM>();

                }
                else
                {
                    notHandler = true;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewListBedRoom_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_BED_ROOM data = (V_HIS_BED_ROOM)((IList)((BaseView)sender).DataSource)[e.RowHandle];

                    if (e.Column.FieldName == "IsLock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? repositoryItembtnUnLock : repositoryItembtnLock);
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? repositoryItembtnDeleteDisable : repositoryItembtnDelete);
                    }
                    else if (e.Column.FieldName == "IsSurgery" && data.IS_SURGERY == IS_SURGERY_TRUE)
                    {
                        e.RepositoryItem = repositoryItembtnIsSurgery;
                    }
                    else if (e.Column.FieldName == "RoomService")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IS_ACTIVE_TRUE && data.IS_RESTRICT_REQ_SERVICE == 1)
                                e.RepositoryItem = repositoryItembtnIsRestrictReqServiceEnable;
                            else
                                e.RepositoryItem = repositoryItembtnRestrictReqService;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "ThietLap")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IS_ACTIVE_TRUE && data.IS_RESTRICT_EXECUTE_ROOM == 1)
                            {
                                e.RepositoryItem = btnThietlapEnable;

                            }
                            else e.RepositoryItem = btnThietlapDisable;
                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    //else if (e.Column.FieldName == "IS_PAUSE_STR")
                    //{
                    //    if (data.IS_PAUSE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //    {
                    //        e.RepositoryItem = repositoryItembtnIsRestrictReqServiceEnable;
                    //    }
                    //    else
                    //    {
                    //        e.RepositoryItem = repositoryItembtnRestrictReqService;
                    //    }
                    //}


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnUnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_BED_ROOM success = new HIS_BED_ROOM();
            bool notHandler = false;
            try
            {

                V_HIS_BED_ROOM dataBedRoom = (V_HIS_BED_ROOM)gridViewListBedRoom.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    HIS_BED_ROOM data1 = new HIS_BED_ROOM();

                    data1.ID = dataBedRoom.ID;

                    WaitingManager.Show();

                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BED_ROOM>(RequestUriStore.HIS_BED_ROOM_CHANGELOCK, ApiConsumers.MosConsumer, data1, param);

                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_BED_ROOM>();
                        FillDataToGrid();
                    }



                }
                else
                {
                    notHandler = true;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewListBedRoom_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_BED_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_ACTIVE ?? -1).ToString());
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                        }

                        else if (e.Column.FieldName == "CREATE_TIME_ST")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.CREATE_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_ST")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.MODIFY_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_ST")
                        {
                            try
                            {
                                if (status == IS_ACTIVE_TRUE)
                                    e.Value = "Hoạt động";
                                else e.Value = "Tạm khóa";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS_PAUSE_STR")
                        {
                            try
                            {
                                if (data.IS_PAUSE == 1)
                                {
                                    e.Value = true;
                                }
                                else
                                {
                                    e.Value = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void gridControlBedRoom_Click(object sender, EventArgs e)
        {
            try
            {
                gridControlBedRoom.Refresh();
                var row = (V_HIS_BED_ROOM)gridViewListBedRoom.GetFocusedRow();
                //var row1 = (HIS_ROOM)gridViewListBedRoom.GetFocusedRow();
                if (row != null)
                {
                    GridCheckMarksSelection gridCheckMarkPart = cboTreatmentTypeIds.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkPart.ClearSelection(cboTreatmentTypeIds.Properties.View);

                    GridCheckMarksSelection gridCheckMark = cboTreatmentTypeIds.Properties.Tag as GridCheckMarksSelection;
                    if (!String.IsNullOrWhiteSpace(row.TREATMENT_TYPE_IDS) && cboTreatmentTypeIds.Properties.Tag != null)
                    {
                        ProcessSelectBusiness(row.TREATMENT_TYPE_IDS, gridCheckMark);
                    }
                    txtBedRoomCode.Text = row.BED_ROOM_CODE;
                    txtBedRoomName.Text = row.BED_ROOM_NAME;
                    cboDepartment.EditValue = row.DEPARTMENT_ID;
                    chkIsSurgery.Checked = row.IS_SURGERY == IS_SURGERY_TRUE ? true : false;
                    chkIsPause.Checked = row.IS_PAUSE == IS_PAUSE_TRUE ? true : false;
                    chkIsRestrictReqService.Checked = row.IS_RESTRICT_REQ_SERVICE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? true : false;
                    checkEdit1.Checked = row.IS_RESTRICT_EXECUTE_ROOM == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? true : false;
                    cboChuyenKhoa.EditValue = row.SPECIALITY_ID;
                    cboCashierRoom.EditValue = row.DEFAULT_CASHIER_ROOM_ID;
                    cboAccountBook.EditValue = row.QR_ACCOUNT_BOOK_ID;
                    //txtConfig.Text = row.QR_CONFIG_JSON;
                    if (row.IS_ACTIVE == IS_ACTIVE_TRUE)
                    {
                        CommonParam param = new CommonParam();
                        HisSpecialityFilter filter = new HisSpecialityFilter();
                        filter.IS_ACTIVE = 1;
                        List<HIS_ROOM> data = new BackendAdapter(param).Get<List<HIS_ROOM>>("api/HisRoom/Get", ApiConsumers.MosConsumer, filter, param);

                        cboDefaultsCLS.EditValue = data.Where(p => p.ID == row.ROOM_ID).FirstOrDefault().DEFAULT_INSTR_PATIENT_TYPE_ID;
                        cboKhuVuc.EditValue = data.Where(p => p.ID == row.ROOM_ID).FirstOrDefault().AREA_ID;
                        Inventec.Common.Logging.LogSystem.Warn("api/HisRoom/Get        " + data.FirstOrDefault().AREA_ID);
                        cboKhuVuc.Refresh();
                        txtConfig.Text = data.Where(p => p.ID == row.ROOM_ID).FirstOrDefault() != null ? data.Where(p => p.ID == row.ROOM_ID).FirstOrDefault().QR_CONFIG_JSON : "";
                    }
                    ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(ActionType);
                    this.txtBedRoomCode.ReadOnly = true;
                    this.cboDepartment.ReadOnly = true;
                    currentBedRoom = row;
                    btnEdit.Enabled = true;
                    if (row.IS_ACTIVE == IS_ACTIVE_FALSE) btnEdit.Enabled = false;
                }
                //var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == row.ROOM_ID);
                //if (room != null)
                //{
                //    checkEdit1.Checked = room.IS_RESTRICT_EXECUTE_ROOM == 1 ? true : false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSelectBusiness(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_TREATMENT_TYPE> ds = cboTreatmentTypeIds.Properties.DataSource as List<HIS_TREATMENT_TYPE>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_TREATMENT_TYPE> selects = new List<HIS_TREATMENT_TYPE>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID.ToString() == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.ActionType = GlobalVariables.ActionEdit;
            this.ProcessSave();
        }

        private void ProcessSave()
        {
            bool success = false;
            CommonParam param = new CommonParam();
            try
            {
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    currentBedRoom = new V_HIS_BED_ROOM();
                }
                currentBedRoom.BED_ROOM_CODE = txtBedRoomCode.Text;
                currentBedRoom.BED_ROOM_NAME = txtBedRoomName.Text;
                currentBedRoom.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? 0).ToString());
                currentBedRoom.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG;
                currentBedRoom.IS_SURGERY = chkIsSurgery.Checked ? IS_SURGERY_TRUE : IS_SURGERY_FALSE;

                GridCheckMarksSelection gridCheckMarkBusiness = cboTreatmentTypeIds.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMarkBusiness != null && gridCheckMarkBusiness.SelectedCount > 0)
                {
                    List<string> codes = new List<string>();
                    foreach (HIS_TREATMENT_TYPE rv in gridCheckMarkBusiness.Selection)
                    {
                        if (rv != null && !codes.Contains(rv.ID.ToString()))
                            codes.Add(rv.ID.ToString());
                    }

                    currentBedRoom.TREATMENT_TYPE_IDS = String.Join(",", codes);
                }

                else
                {
                    currentBedRoom.TREATMENT_TYPE_IDS = null;
                }


                HisBedRoomSDO HisBedRoomSDO = new HisBedRoomSDO();
                Mapper.CreateMap<V_HIS_BED_ROOM, HIS_BED_ROOM>();
                HisBedRoomSDO.HisBedRoom = Mapper.Map<V_HIS_BED_ROOM, HIS_BED_ROOM>(currentBedRoom);
                Mapper.CreateMap<V_HIS_BED_ROOM, HIS_ROOM>();

                HIS_ROOM hisRoom = new HIS_ROOM();
                hisRoom.IS_RESTRICT_EXECUTE_ROOM = checkEdit1.Checked ? (short)1 : (short)0;
                hisRoom.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? 0).ToString());
                hisRoom.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG;
                hisRoom.ID = currentBedRoom.ROOM_ID;
                hisRoom.IS_PAUSE = chkIsPause.Checked ? (short)1 : (short)0;
                hisRoom.IS_RESTRICT_REQ_SERVICE = chkIsRestrictReqService.Checked ? (short)1 : (short)0;
                if (cboAccountBook.EditValue != null) hisRoom.QR_ACCOUNT_BOOK_ID = Convert.ToInt64(Convert.ToInt64(cboAccountBook.EditValue));
                hisRoom.QR_CONFIG_JSON = txtConfig.Text;
                if (cboChuyenKhoa.EditValue != null)
                {
                    hisRoom.SPECIALITY_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboChuyenKhoa.EditValue.ToString());
                }
                else
                {
                    hisRoom.SPECIALITY_ID = null;
                }

                if (cboKhuVuc.EditValue != null)
                {
                    hisRoom.AREA_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboKhuVuc.EditValue.ToString());
                }
                else
                {
                    hisRoom.AREA_ID = null;

                }

                if (cboCashierRoom.EditValue != null)
                {
                    hisRoom.DEFAULT_CASHIER_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboCashierRoom.EditValue ?? "0").ToString());
                }

                if (cboDefaultsCLS.EditValue != null)
                {
                    hisRoom.DEFAULT_INSTR_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDefaultsCLS.EditValue ?? "0").ToString());
                }

                HisBedRoomSDO.HisRoom = hisRoom;

                WaitingManager.Show();
                HisBedRoomSDO result = null;
                Inventec.Common.Logging.LogSystem.Debug("Du lieu gui len api: " + RequestUriStore.HIS_BED_ROOM_CREATE + " : " + LogUtil.TraceData("HisBedRoomSDO", HisBedRoomSDO));

                if (this.ActionType == GlobalVariables.ActionAdd)
                {

                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisBedRoomSDO>(RequestUriStore.HIS_BED_ROOM_CREATE, ApiConsumer.ApiConsumers.MosConsumer, HisBedRoomSDO, param);
                }
                if (this.ActionType == GlobalVariables.ActionEdit)
                {

                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisBedRoomSDO>(RequestUriStore.HIS_BED_ROOM_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, HisBedRoomSDO, param);
                    if (result != null) this.ActionType = GlobalVariables.ActionAdd;
                }
                if (result != null)
                {
                    success = true;
                    btnRefesh_Click(null, null);

                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
                BackendDataWorker.Reset<HIS_ROOM_TYPE>();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                ActionType = GlobalVariables.ActionAdd;
                LoadDefault();
                FillDataToGrid();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider1, dxErrorProvider1);
                cboCashierRoom.Properties.DataSource = null;
                //cboDefaultsCLS.Properties.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtBedRoomCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBedRoomName.Focus();
                    txtBedRoomName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBedRoomName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepartment.Focus();
                    cboDepartment.SelectAll();

                    //e .Handled = true;
                    cboDepartment.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnEdit.Enabled)
                btnEdit_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnRefresh.Enabled)
                btnRefesh_Click(null, null);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnFind.Enabled)
                btnFind_Click(null, null);
        }

        private void cboDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDepartment.EditValue != null)
                    {

                        cboKhuVuc.Focus();
                        cboKhuVuc.ShowPopup();
                    }
                    else
                    {
                        cboKhuVuc.Focus();
                        cboKhuVuc.ShowPopup();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkIsSurgery_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsRestrictReqService.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkIsRestrictReqService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsPause.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkIsPause_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else if (this.ActionType == GlobalVariables.ActionEdit)
                    {
                        btnEdit.Focus();
                    }
                    else
                    {
                        btnRefresh.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboDepartment_Click(object sender, EventArgs e)
        {
            try
            {
                cboDepartment.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisImportBedRoom").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisImportBedRoom");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((HIS.Desktop.Common.RefeshReference)bbtnSearch);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSearch()
        {
            try
            {
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Import_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChuyenKhoa_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    if (cboChuyenKhoa.EditValue != null)
                    {

                        chkIsSurgery.Focus();
                    }
                    else
                    {
                        cboChuyenKhoa.Focus();
                        cboChuyenKhoa.ShowPopup();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboChuyenKhoa.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewListBedRoom_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_BED_ROOM data = (V_HIS_BED_ROOM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_ACTIVE_ST")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.ActionType = GlobalVariables.ActionAdd;
            this.ProcessSave();
        }

        private void barButtonItemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
                btnAdd_Click(null, null);
        }

        private void repositoryItembtnIsRestrictReqServiceEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_BED_ROOM)gridViewListBedRoom.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();

                    V_HIS_ROOM room = new V_HIS_ROOM();
                    room.ID = row.ROOM_ID;
                    room.ROOM_CODE = row.BED_ROOM_CODE;
                    room.ROOM_NAME = row.BED_ROOM_NAME;
                    room.ROOM_TYPE_ID = row.ROOM_TYPE_ID;
                    room.ROOM_TYPE_CODE = row.ROOM_TYPE_CODE;
                    listArgs.Add(room);
                    CallModule callModule = new CallModule(CallModule.ServiceRoom, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                    WaitingManager.Hide();
                    this.Hide();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnThietlapEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_BED_ROOM)gridViewListBedRoom.GetFocusedRow();

                if (row != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    V_HIS_ROOM room = new V_HIS_ROOM();
                    room.ID = row.ROOM_ID;
                    room.ROOM_CODE = row.BED_ROOM_CODE;
                    room.ROOM_NAME = row.BED_ROOM_NAME;
                    room.ROOM_TYPE_ID = row.ROOM_TYPE_ID;
                    room.ROOM_TYPE_CODE = row.ROOM_TYPE_CODE;
                    listArgs.Add(room);
                    CallModule callModule = new CallModule(CallModule.ExroRoom, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);


                    WaitingManager.Hide();
                    this.Hide();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }


        }

        private void cboKhuVuc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    if (cboKhuVuc.EditValue != null)
                    {

                        cboChuyenKhoa.Focus();
                        cboChuyenKhoa.ShowPopup();
                    }
                    else
                    {
                        cboChuyenKhoa.Focus();
                        cboChuyenKhoa.ShowPopup();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentTypeIds_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentTypeIds_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTreatmentTypeIds.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkTreatmentTypeIds = cboTreatmentTypeIds.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkTreatmentTypeIds.ClearSelection(cboTreatmentTypeIds.Properties.View);
                    treatmentTypeIdSelecteds = new List<HIS_TREATMENT_TYPE>();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentTypeIds_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cboTreatmentTypeIds.EditValue != null)
                {

                    cboCashierRoom.Focus();
                    cboCashierRoom.ShowPopup();
                }
                else
                {
                    cboCashierRoom.Focus();
                    cboCashierRoom.ShowPopup();
                }

            }
        }

        private void cboCashierRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSurgery.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboDepartment.Text.Trim()))
            {
                InitComboCashierRoom();
                if (cboCashierRoom == null)
                {
                    cboCashierRoom.Properties.Buttons[1].Visible = false;
                }
            }
        }

        private void cboCashierRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCashierRoom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDefaultsCLS_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDefaultsCLS.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDefaultsCLS_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDefaultsCLS.EditValue != null)
                {
                    string selectedValue = cboDefaultsCLS.EditValue.ToString().Trim('{', '}');
                    string[] parts = selectedValue.Split(',');

                    //cboDefaultsCLS.EditValue = dataSource.PATIENT_TYPE_NAME;
                    cboDefaultsCLS.EditValue = parts[2].Trim('"');
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        // thinhdt2
        //xu ly them cua hinh QR
        #region them cau hinh QR
        public class CONFIGADO
        {
            public string BANK { get; set; }
            public string VALUE { get; set; }
        }
        private void btnChangeConfig_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Rectangle buttonPosition = new Rectangle(btnChangeConfig.Bounds.X, btnChangeConfig.Bounds.Y, btnChangeConfig.Bounds.Width, btnChangeConfig.Bounds.Height);
                popupControlContainer1.ShowPopup(new Point(buttonPosition.X + 480, buttonPosition.Bottom + btnChangeConfig.Bounds.Height - 10));
                InitComboControlConfig();
                DataTable table = new DataTable();
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("Value", typeof(object));

                // Thêm dữ liệu cho 2 dòng "Ngân hàng" và "Cấu hình"


                table.Rows.Add("Ngân hàng", null);
                table.Rows.Add("Cấu hình", "");

                gridControl2.DataSource = table;


                if (!string.IsNullOrEmpty(txtConfig.Text))
                {
                    CONFIGADO config = Newtonsoft.Json.JsonConvert.DeserializeObject<CONFIGADO>(txtConfig.Text);
                    gridView12.SetRowCellValue(0, "Value", config.BANK);
                    gridView12.SetRowCellValue(1, "Value", config.VALUE);
                    
                }
                gridView12.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None;
                gridView12.OptionsBehavior.Editable = true;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        List<HIS_CONFIG> listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
        List<HIS_CONFIG> listConfigEncode = new List<HIS_CONFIG>();
        private void InitComboControlConfig()
        {
            try
            {


                int count = 0;
                listConfigEncode.Clear();
                foreach (var item in listConfig)
                {
                    HIS_CONFIG cf = new HIS_CONFIG();
                    cf.ID = count++;


                    string value = item.KEY;
                    int index = value.IndexOf("Info");
                    if (index > 0)
                    {
                        var shotkey = value.Substring(0, index);
                        string[] parts = shotkey.Split('.');
                        if (parts.Length > 0)
                        {
                            cf.KEY = parts[parts.Length - 1]; // Lấy phần cuối cùng sau khi tách
                        }
                    }
                    cf.VALUE = item.VALUE;
                    listConfigEncode.Add(cf);
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KEY", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("KEY", "KEY", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboBank, listConfigEncode, controlEditorADO);
            }

            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView12_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle == 0 && e.Column.FieldName == "Value")
                {
                    e.RepositoryItem = cboBank;
                }
                if (e.RowHandle == 1 && e.Column.FieldName == "Value")
                {
                    e.RepositoryItem = txtConfigValue;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public string KEY { get; set; }
        public string VALUE { get; set; }
        private void cboBank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                GridLookUpEdit grid = gridView12.ActiveEditor as GridLookUpEdit;

                if (grid != null)
                {
                    var selectedBank = grid.EditValue.ToString();
                    KEY = selectedBank;
                    
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBank_EditValueChanged(object sender, EventArgs e)
        {

            try
            {

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            try
            {

                var result = new
                {
                    BANK = this.KEY == null ? " ": this.KEY,
                    VALUE = this.VALUE == null ? " " : this.VALUE
                };

                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                txtConfig.Text = jsonString.ToString();



                PopupContainerBarControl control = popupControlContainer1.Parent as PopupContainerBarControl;
                control.ClosePopup();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBook_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {

                    cboAccountBook.DataBindings.Clear();
                    cboAccountBook.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtConfigValue_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var textEdit = sender as DevExpress.XtraEditors.TextEdit;
                if (textEdit != null)
                {

                    this.VALUE = textEdit.EditValue.ToString();


                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //
        #endregion

        private void buttonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtConfig.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboAccountBook.EditValue == null) return;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

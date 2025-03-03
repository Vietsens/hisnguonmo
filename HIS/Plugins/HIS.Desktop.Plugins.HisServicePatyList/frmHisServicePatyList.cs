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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraExport;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisServicePatyList.entity;
using HIS.Desktop.Plugins.HisServicePatyList.Resources;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisServicePatyList
{
    public partial class frmHisServicePatyList : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        PagingGrid pagingGrid;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY currentData;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE currentDataService;
        internal List<V_HIS_SERVICE_PATY> ListServicePatyprocess;
        long servicePatyId;
        bool isCheckAll;
        List<V_HIS_SERVICE> services;
        List<HIS_SERVICE_TYPE> serviceTypes;
        List<HIS_PATIENT_TYPE> patientTypes;
        List<HIS_PACKAGE> packageService;
        List<HIS_BRANCH> branchs;
        List<HIS_RATION_TIME> rationTimes;
        internal List<DepartmentADO> lstDepartMentADOs { get; set; }
        internal List<RoomRequiredADO> lstRoomRequiredADOs { get; set; }
        internal List<RoomPracticeADO> lstRoomPraticeADOs { get; set; }
        List<HIS_DEPARTMENT> departMent { get; set; }
        List<V_HIS_ROOM> roomRequired { get; set; }
        List<V_HIS_ROOM> roomPratice { get; set; }
        List<HIS_ROOM_TYPE> roomType { get; set; }
        List<HIS_ROOM_TYPE> roomTypes = new List<HIS_ROOM_TYPE>();

        List<V_HIS_SERVICE> listVServiceExport = new List<V_HIS_SERVICE>();
        List<V_HIS_SERVICE_PATY> listVServicePatyExport = new List<V_HIS_SERVICE_PATY>();
        List<HIS_SERVICE_TYPE> serviceTypeSelecteds;
        List<HIS_PATIENT_TYPE> patientTypeSelecteds;
        List<HIS_PATIENT_TYPE> patientTypeNameSelecteds;
        List<HIS_BRANCH> branchNameSelecteds;
        List<HIS_RATION_TIME> rationTimeNameSelecteds;

        private int MAX_REQUEST_LENGTH_PARAM = 1000;
        public frmHisServicePatyList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            SetCaptionByLanguageKey();
            try
            {
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                spinPayoutRatio.Properties.MinValue = 0;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this.ActionType = GlobalVariables.ActionAdd;
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //public frmHisServicePatyList(Inventec.Desktop.Common.Modules.Module moduleData)
        //    : this()
        //{

        //}

        public frmHisServicePatyList(Inventec.Desktop.Common.Modules.Module moduleData, MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY servicePaty)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                SetCaptionByLanguageKey();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this.ActionType = GlobalVariables.ActionAdd;
                    if (servicePaty != null)
                    {
                        currentData = servicePaty;
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmHisServicePatyList(Inventec.Desktop.Common.Modules.Module moduleData, MOS.EFMODEL.DataModels.V_HIS_SERVICE servicePaty)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                SetCaptionByLanguageKey();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this.ActionType = GlobalVariables.ActionAdd;
                    if (servicePaty != null)
                    {
                        currentDataService = servicePaty;

                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisServicePatyList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisServicePatyList.frmHisServicePatyList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemFind.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.barButtonItemFind.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.barButtonItemEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.barButtonItemAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemCancel.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.barButtonItemCancel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHourTo.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.cboHourTo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDayTo.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.cboDayTo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtFind3.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.txtFind3.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtFind2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.txtFind2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtFind1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.txtFind1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHourForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.cboHourForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDayFrom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.cboDayFrom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Department_Name.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Department_Name.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_RoomName.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_RoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBranchName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.cboBranchName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientTypeName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.cboPatientTypeName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.cboServiceName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceTypeName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.cboServiceTypeName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.lciServiceTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceName.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.lciServiceName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciPatientTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.lciPatientTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciBranchName.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.lciBranchName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRationTime.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.lciRationTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem36.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem36.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Stt.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Delete.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Service_type_name.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Service_type_name.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Service_Code.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Service_Code.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Service_Name.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Service_Name.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_DTTT.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_DTTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Amout.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Amout.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_VAT.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_VAT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Overtime_Price.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Overtime_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Priority.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Priority.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Intruction_Number_From.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Intruction_Number_From.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Intruction_Number_To.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Intruction_Number_To.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_From_Time.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_From_Time.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_To_Time.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_To_Time.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Treatment_From_Time.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Treatment_From_Time.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Treatment_To_Time.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Treatment_To_Time.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Branch_Name.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Branch_Name.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Status.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Status.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Branch_Code.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Branch_Code.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Create_Time.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Create_Time.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Creator.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Creator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Modify_Time.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Modify_Time.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Modifier.Caption = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.gridColumn_Modifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciPatientTypeFilter.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.LciPatientTypeFilter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisServicePatyList.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmHisServicePatyList_Load(object sender, EventArgs e)
        {
            try
            {
                gcExecuteRooomCheck.Image = imageCollection.Images[0];
                gcExecuteRooomCheck.ImageAlignment = StringAlignment.Center;
                gridViewRoomRequired.Columns.Where(o => o.FieldName == "Check").FirstOrDefault().Image = imageCollection.Images[0];
                gridViewRoomRequired.Columns.Where(o => o.FieldName == "Check").FirstOrDefault().ImageAlignment = StringAlignment.Center;
                gridViewDepartMent.Columns.Where(o => o.FieldName == "Check").FirstOrDefault().Image = imageCollection.Images[0];
                gridViewDepartMent.Columns.Where(o => o.FieldName == "Check").FirstOrDefault().ImageAlignment = StringAlignment.Center;
                FillDataToGridControl();
                FillDataTogridViewRoomPractice(new List<long>());
                FillDataTogridViewDepartMent(new List<long>());
                FillDataTogridViewRoomRequired(new List<long>());
                //LoadDataTocboServiceTypeName();
                loadService();
                LoadDataTocboServiceName(null);
                LoadDataTocboServiceCondition(null);
                LoadDataTocboService(null);
                LoadDataTocboServiceType();
                LoadDataTocboPatientTypeFilter();
                InitComboPackageService();
                //LoadDatacboPatientTypeName();
                //LoadDatacboBranchName();
                LoadDataTocboPatientClassify();

                InitCheck(cboPatientTypeName, SelectionGrid__PatientTypeName);
                InitCombo(cboPatientTypeName, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), "PATIENT_TYPE_NAME", "ID");
                InitCheck(cboBranchName, SelectionGrid__BranchName);
                InitCombo(cboBranchName, BackendDataWorker.Get<HIS_BRANCH>(), "BRANCH_NAME", "ID");
                InitCheck(cboRationTime, SelectionGrid__RationTime);
                InitComboRationTime(cboRationTime, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_RATION_TIME>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                LoadComboStatus();
                LoadComboStatusHour();
                LoadDataRoomType();
                ValidateForm();
                ResetFormData();
                if (currentData != null)
                {
                    FillDataToEditorControl(currentData);
                }
                else if (currentDataService != null)
                {
                    FillDataToEditorByServiceControl(currentDataService);
                }
                cboServiceCondition.Enabled = false;
                SetDefaultControlsProperties();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControlsProperties()
        {
            try
            {
                spinActualPrice.Properties.MinValue = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Sự kiện phím tắt
        private void barButtonItemFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnEdit.Enabled == false)
                {
                    return;
                }
                btnEdit.Focus();
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnAdd.Enabled == false)
                {
                    return;
                }
                btnAdd.Focus();
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void loadService()
        {
            try
            {
                services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_LEAF == 1 && o.IS_ACTIVE == 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataTogridViewRoomRequired(List<long> roomRequiredIds)
        {
            try
            {
                WaitingManager.Show();
                roomRequired = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>();
                lstRoomRequiredADOs = new List<RoomRequiredADO>();
                if (roomRequired != null && roomRequired.Count > 0)
                {
                    lstRoomRequiredADOs = (from m in roomRequired select new RoomRequiredADO(m, roomRequiredIds)).ToList();
                }
                lstRoomRequiredADOs = lstRoomRequiredADOs.OrderByDescending(o => o.Check).ThenBy(p => p.ROOM_NAME).ToList();
                gridControlRoomRequired.BeginUpdate();
                gridControlRoomRequired.DataSource = lstRoomRequiredADOs;
                gridControlRoomRequired.EndUpdate();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataTogridViewDepartMent(List<long> departmentIds)
        {
            try
            {
                WaitingManager.Show();
                departMent = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>();
                lstDepartMentADOs = new List<DepartmentADO>();
                if (departMent != null && departMent.Count > 0)
                {
                    lstDepartMentADOs = (from m in departMent select new DepartmentADO(m, departmentIds)).ToList();
                }
                lstDepartMentADOs = lstDepartMentADOs.OrderByDescending(o => o.Check).ThenBy(p => p.DEPARTMENT_NAME).ToList();
                gridControlDepartMent.BeginUpdate();
                gridControlDepartMent.DataSource = lstDepartMentADOs;
                gridControlDepartMent.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataTogridViewRoomPractice(List<long> roomPraticeIds)
        {
            try
            {
                WaitingManager.Show();
                roomPratice = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>();
                lstRoomPraticeADOs = new List<RoomPracticeADO>();
                if (roomPratice != null && roomPratice.Count > 0)
                {
                    lstRoomPraticeADOs = (from m in roomPratice select new RoomPracticeADO(m, roomPraticeIds)).ToList();

                }
                lstRoomPraticeADOs = lstRoomPraticeADOs.OrderByDescending(o => o.Check).ThenBy(p => p.ROOM_NAME).ToList();
                gridControlRoomPractice.BeginUpdate();
                gridControlRoomPractice.DataSource = lstRoomPraticeADOs;
                gridControlRoomPractice.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Load data to Combo
        private void LoadDataRoomType()
        {
            try
            {
                roomTypes = BackendDataWorker.Get<HIS_ROOM_TYPE>();
                LoadDataToComboRoomType(cboRoomType, roomTypes);
                LoadDataToComboRoomType(cboRoomType1, roomTypes);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboRoomType(object cbo, List<HIS_ROOM_TYPE> roomType)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_TYPE_NAME", "", 400, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_TYPE_NAME", "ID", columnInfos, false, 500);
                Inventec.Common.Controls.EditorLoader.ControlEditorLoader.Load(cbo, roomType, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /*private void LoadDatacboBranchName()
        {
            try
            {
                this.branchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBranchName, this.branchs, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDatacboPatientTypeName()
        {
            try
            {
                this.patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientTypeName, this.patientTypes, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }*/

        //Load combo dịch vụ theo loại dịch vụ
        private void LoadDataTocboServiceName(long? serviceTypeId)
        {
            try
            {
                List<V_HIS_SERVICE> serviceTemps = null;
                if (serviceTypeId.HasValue)
                {
                    serviceTemps = this.services.Where(o => o.SERVICE_TYPE_ID == serviceTypeId && o.IS_LEAF == 1).ToList();
                }
                else if (cboServiceTypeName.EditValue != null)
                {
                    serviceTemps = this.services.Where(o => o.SERVICE_TYPE_ID == (long)cboServiceTypeName.EditValue && o.IS_LEAF == 1).ToList();
                }
                else
                {
                    serviceTemps = this.services;
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboServiceName, serviceTemps, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboService(long? serviceType1Id)
        {
            try
            {
                List<V_HIS_SERVICE> serviceTemps = null;

                if (serviceType1Id.HasValue)
                {
                    serviceTemps = this.services.Where(o => o.SERVICE_TYPE_ID == serviceType1Id).ToList();
                }
                else if (cboServiceType.EditValue != null)
                {
                    serviceTemps = this.services.Where(o => o.SERVICE_TYPE_ID == (long)cboServiceType.EditValue).ToList();
                }
                else
                {
                    serviceTemps = this.services;
                }
                //foreach (var item in this.services)
                //{
                //    if (item.SERVICE_CODE == "TT1133")
                //    {
                //        Inventec.Common.Logging.LogSystem.Warn(item.SERVICE_CODE);
                //    }
                //}

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboService, serviceTemps, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboServiceCondition(long? serviceId)
        {
            try
            {
                List<HIS_SERVICE_CONDITION> serviceTemps = null;
                if (serviceId != null)
                {

                    serviceTemps = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().Where(o => o.SERVICE_ID == serviceId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_CONDITION_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboServiceCondition, serviceTemps, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboPatientClassify()
        {
            try
            {
                var listPatientClassify = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_CLASSIFY_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_CLASSIFY_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_CLASSIFY_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPatientClassify, listPatientClassify, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Load comboServiceType
        private void LoadDataTocboServiceTypeName(object cbo, List<HIS_SERVICE_TYPE> serviceTypes)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, serviceTypes, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboServiceType()
        {
            try
            {
                InitServiceReqTypeCheck();
                this.serviceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                //LoadDataTocboServiceTypeName(cboServiceType, this.serviceTypes);
                LoadDataTocboServiceTypeName(cboServiceTypeName, this.serviceTypes);
                InitComboServiceReqType(this.serviceTypes);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboServiceReqType(List<HIS_SERVICE_TYPE> serviceTypes)
        {
            try
            {
                var datas = serviceTypes;
                if (datas != null)
                {
                    cboServiceType.Properties.DataSource = datas;
                    cboServiceType.Properties.DisplayMember = "SERVICE_TYPE_NAME";
                    cboServiceType.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboServiceType.Properties.View.Columns.AddField("SERVICE_TYPE_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    cboServiceType.Properties.PopupFormWidth = 200;
                    cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboServiceType.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboServiceType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboServiceType.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitServiceReqTypeCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboServiceType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ServiceType);
                cboServiceType.Properties.Tag = gridCheck;
                cboServiceType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboServiceType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboServiceType.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__ServiceType(object sender, EventArgs e)
        {
            try
            {
                serviceTypeSelecteds = new List<HIS_SERVICE_TYPE>();
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        serviceTypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataTocboPatientTypeFilter()
        {
            try
            {
                InitPatientTypeFilterCheck();
                this.patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                InitComboPatientTypeFilter(this.patientTypes);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboPatientTypeFilter(List<HIS_PATIENT_TYPE> patientType)
        {
            try
            {
                var datas = patientType;
                if (datas != null)
                {
                    CboPatientTypeFilter.Properties.DataSource = datas;
                    CboPatientTypeFilter.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                    CboPatientTypeFilter.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = CboPatientTypeFilter.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    CboPatientTypeFilter.Properties.PopupFormWidth = 200;
                    CboPatientTypeFilter.Properties.View.OptionsView.ShowColumnHeaders = false;
                    CboPatientTypeFilter.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = CboPatientTypeFilter.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(CboPatientTypeFilter.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboPackageService()
        {

            try
            {
                this.packageService = null;

                packageService = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PACKAGE>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PACKAGE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PACKAGE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PACKAGE_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(cboPackageService, packageService, controlEditorADO);
                ControlEditorLoader.Load(cboPackageService1, packageService, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitPatientTypeFilterCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(CboPatientTypeFilter.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__PatientTypeFilter);
                CboPatientTypeFilter.Properties.Tag = gridCheck;
                CboPatientTypeFilter.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = CboPatientTypeFilter.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(CboPatientTypeFilter.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__PatientTypeFilter(object sender, EventArgs e)
        {
            try
            {
                patientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        patientTypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Chủ nhật"));
                status.Add(new Status(2, "Thứ 2"));
                status.Add(new Status(3, "Thứ 3"));
                status.Add(new Status(4, "Thứ 4"));
                status.Add(new Status(5, "Thứ 5"));
                status.Add(new Status(6, "Thứ 6"));
                status.Add(new Status(7, "Thứ 7"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDayFrom, status, controlEditorADO);
                ControlEditorLoader.Load(cboDayTo, status, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboStatusHour()
        {
            try
            {
                //Load dộng thời gian từ hàm cs hỗ trợ
                //dùng cú phspd truy vấn linq để khởi  tạo dữ liệu
                List<StatusHour> status = new List<StatusHour>();
                status.Add(new StatusHour(0000, "0000", "12:00 AM"));
                status.Add(new StatusHour(0015, "0015", "12:15 AM")); status.Add(new StatusHour(1215, "1215", "12:15 PM"));
                status.Add(new StatusHour(0030, "0030", "12:30 AM")); status.Add(new StatusHour(1230, "1230", "12:30 PM"));
                status.Add(new StatusHour(0045, "0045", "12:45 AM")); status.Add(new StatusHour(1245, "1245", "12:45 PM"));
                status.Add(new StatusHour(0100, "0100", "1:00 AM")); status.Add(new StatusHour(1300, "1300", "1:00 PM"));
                status.Add(new StatusHour(0115, "0115", "1:15 AM")); status.Add(new StatusHour(1315, "1315", "1:15 PM"));
                status.Add(new StatusHour(0130, "0130", "1:30 AM")); status.Add(new StatusHour(1330, "1330", "1:30 PM"));
                status.Add(new StatusHour(0145, "0145", "1:45 AM")); status.Add(new StatusHour(1345, "1345", "1:45 PM"));
                status.Add(new StatusHour(0200, "0200", "2:00 AM")); status.Add(new StatusHour(1400, "1400", "2:00 PM"));
                status.Add(new StatusHour(0215, "0215", "2:15 AM")); status.Add(new StatusHour(1415, "1415", "2:15 PM"));
                status.Add(new StatusHour(0230, "0230", "2:30 AM")); status.Add(new StatusHour(1430, "1430", "2:30 PM"));
                status.Add(new StatusHour(0245, "0245", "2:45 AM")); status.Add(new StatusHour(1445, "1445", "2:45 PM"));
                status.Add(new StatusHour(0300, "0300", "3:00 AM")); status.Add(new StatusHour(1500, "1500", "3:00 PM"));
                status.Add(new StatusHour(0315, "0315", "3:15 AM")); status.Add(new StatusHour(1515, "1515", "3:15 PM"));
                status.Add(new StatusHour(0330, "0330", "3:30 AM")); status.Add(new StatusHour(1530, "1530", "3:30 PM"));
                status.Add(new StatusHour(0345, "0345", "3:45 AM")); status.Add(new StatusHour(1545, "1545", "3:45 PM"));
                status.Add(new StatusHour(0400, "0400", "4:00 AM")); status.Add(new StatusHour(1600, "1600", "4:00 PM"));
                status.Add(new StatusHour(0415, "0415", "4:15 AM")); status.Add(new StatusHour(1615, "1615", "4:15 PM"));
                status.Add(new StatusHour(0430, "0430", "4:30 AM")); status.Add(new StatusHour(1630, "1630", "4:30 PM"));
                status.Add(new StatusHour(0445, "0445", "4:45 AM")); status.Add(new StatusHour(1645, "1645", "4:45 PM"));
                status.Add(new StatusHour(0500, "0500", "5:00 AM")); status.Add(new StatusHour(1700, "1700", "5:00 PM"));
                status.Add(new StatusHour(0515, "0515", "5:15 AM")); status.Add(new StatusHour(1715, "1715", "5:15 PM"));
                status.Add(new StatusHour(0530, "0530", "5:30 AM")); status.Add(new StatusHour(1730, "1730", "5:30 PM"));
                status.Add(new StatusHour(0545, "0545", "5:45 AM")); status.Add(new StatusHour(1745, "1745", "5:45 PM"));
                status.Add(new StatusHour(0600, "0600", "6:00 AM")); status.Add(new StatusHour(1800, "1800", "6:00 PM"));
                status.Add(new StatusHour(0615, "0615", "6:15 AM")); status.Add(new StatusHour(1815, "1815", "6:15 PM"));
                status.Add(new StatusHour(0630, "0630", "6:30 AM")); status.Add(new StatusHour(1830, "1830", "6:30 PM"));
                status.Add(new StatusHour(0645, "0645", "6:45 AM")); status.Add(new StatusHour(1845, "1845", "6:45 PM"));
                status.Add(new StatusHour(0700, "0700", "7:00 AM")); status.Add(new StatusHour(1900, "1900", "7:00 PM"));
                status.Add(new StatusHour(0715, "0715", "7:15 AM")); status.Add(new StatusHour(1915, "1915", "7:15 PM"));
                status.Add(new StatusHour(0730, "0730", "7:30 AM")); status.Add(new StatusHour(1930, "1930", "7:30 PM"));
                status.Add(new StatusHour(0745, "0745", "7:45 AM")); status.Add(new StatusHour(1945, "1945", "7:45 PM"));
                status.Add(new StatusHour(0800, "0800", "8:00 AM")); status.Add(new StatusHour(2000, "2000", "8:00 PM"));
                status.Add(new StatusHour(0815, "0815", "8:15 AM")); status.Add(new StatusHour(2015, "2015", "8:15 PM"));
                status.Add(new StatusHour(0830, "0830", "8:30 AM")); status.Add(new StatusHour(2030, "2030", "8:30 PM"));
                status.Add(new StatusHour(0845, "0845", "8:45 AM")); status.Add(new StatusHour(2045, "2045", "8:45 PM"));
                status.Add(new StatusHour(0900, "0900", "9:00 AM")); status.Add(new StatusHour(2100, "2100", "9:00 PM"));
                status.Add(new StatusHour(0915, "0915", "9:15 AM")); status.Add(new StatusHour(2115, "2115", "9:15 PM"));
                status.Add(new StatusHour(0930, "0930", "9:30 AM")); status.Add(new StatusHour(2130, "2130", "9:30 PM"));
                status.Add(new StatusHour(0945, "0945", "9:45 AM")); status.Add(new StatusHour(2145, "2145", "9:45 PM"));
                status.Add(new StatusHour(1000, "1000", "10:00 AM")); status.Add(new StatusHour(2200, "2200", "10:00 PM"));
                status.Add(new StatusHour(1015, "1015", "10:15 AM")); status.Add(new StatusHour(2215, "2215", "10:15 PM"));
                status.Add(new StatusHour(1030, "1030", "10:30 AM")); status.Add(new StatusHour(2230, "2230", "10:30 PM"));
                status.Add(new StatusHour(1045, "1045", "10:45 AM")); status.Add(new StatusHour(2245, "2245", "10:45 PM"));
                status.Add(new StatusHour(1100, "1100", "11:00 AM")); status.Add(new StatusHour(2300, "2300", "11:00 PM"));
                status.Add(new StatusHour(1115, "1115", "11:15 AM")); status.Add(new StatusHour(2315, "2315", "11:15 PM"));
                status.Add(new StatusHour(1130, "1130", "11:30 AM")); status.Add(new StatusHour(2330, "2330", "11:30 PM"));
                status.Add(new StatusHour(1145, "1145", "11:45 AM")); status.Add(new StatusHour(2345, "2345", "11:45 PM"));
                status.Add(new StatusHour(1200, "1200", "12:00 PM"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "statusCode", columnInfos, false, 350);
                ControlEditorLoader.Load(cboHourForm, status, controlEditorADO);
                ControlEditorLoader.Load(cboHourTo, status, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Load data to Grid
        public void RefreshData()
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillDataToGridControl()
        {
            try
            {
                int numPageSize;
                numPageSize = (this.ucPaging1.pagingGrid != null) ? this.ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                this.FillDataToGridServicePaty(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                this.ucPaging1.Init(this.FillDataToGridServicePaty, param, numPageSize, this.gridControlServicePaty);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridServicePaty(object data)
        {
            try
            {
                List<long> glstServiceID = new List<long>();
                WaitingManager.Show();
                start = ((CommonParam)data).Start ?? 0;
                limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>> apiResult = null;
                List<V_HIS_SERVICE_PATY> glstServicePay = new List<V_HIS_SERVICE_PATY>();
                HisServicePatyViewFilter filter = new HisServicePatyViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtFind.Text;

                if (currentDataService != null)
                    filter.SERVICE_ID = currentDataService.ID;

                SetFilter(ref filter);
                apiResult = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>>(
                    "api/HisServicePaty/GetView", ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null)
                {
                    var dataService = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>)apiResult.Data;
                    if (dataService != null)
                    {
                        gridControlServicePaty.DataSource = dataService;
                        rowCount = (dataService == null ? 0 : dataService.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                //}

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void SetFilter(ref MOS.Filter.HisServicePatyViewFilter filter)
        {
            try
            {
                if (serviceTypeSelecteds != null && serviceTypeSelecteds.Count > 0)
                {
                    filter.SERVICE_TYPE_IDs = serviceTypeSelecteds.Select(o => o.ID).ToList();
                }

                if (patientTypeSelecteds != null && patientTypeSelecteds.Count > 0)
                {
                    filter.PATIENT_TYPE_IDs = patientTypeSelecteds.Select(o => o.ID).ToList();
                }

                if (cboService.EditValue != null)
                {
                    filter.SERVICE_ID = (long)cboService.EditValue;
                }
                if (cboPackageService.EditValue != null && cboPackageService.EditValue.ToString() != "")
                {
                    filter.PACKAGE_ID = (long)cboPackageService.EditValue;
                }
                if (ChkInActive.Checked)
                {
                    filter.IN_ACTIVE_TIME = true;
                    filter.SERVICE_IS_ACTIVE = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServicePaty_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERVICE_PATY data = (V_HIS_SERVICE_PATY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + start;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_SV")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_SV")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TREATMENT_TO_TIME_SV")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TREATMENT_TO_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TREATMENT_FROM_TIME_SV")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TREATMENT_FROM_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "FROM_TIME_SV")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FROM_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TO_TIME_SV")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TO_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = data != null && data.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa";
                        }
                        else if (e.Column.FieldName == "VAT_RATIO_RS")
                        {
                            e.Value = (data.VAT_RATIO) * 100;   
                        }
                        else if (e.Column.FieldName == "PACKAGE_NAME")
                        {
                            e.Value = BackendDataWorker.Get<HIS_PACKAGE>().Where(o => o.ID == data.PACKAGE_ID).FirstOrDefault().PACKAGE_NAME;
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServicePaty_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                gridViewDepartMent.ClearSelection();
                gridViewRoomPractice.ClearSelection();
                gridViewRoomRequired.ClearSelection();
                this.currentData = (V_HIS_SERVICE_PATY)gridViewServicePaty.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentData), this.currentData));
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                    //set focus vào control editor đầu tiên
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangedDataRow(V_HIS_SERVICE_PATY data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);   
                    this.ActionType = GlobalVariables.ActionEdit;
                    cboRationTime.Properties.Buttons[1].Visible = true;
                    EnableControlChanged(this.ActionType);
                    SetValuePatientType(this.cboPatientTypeName, this.patientTypeNameSelecteds, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());
                    SetValueBranch(this.cboBranchName, this.branchNameSelecteds, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>());
                    SetValueRationTime(this.cboRationTime, this.rationTimeNameSelecteds, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_RATION_TIME>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    if (data.SERVICE_ID != null)
                    {
                        if (data.SERVICE_CONDITION_ID != null)
                        {
                            cboServiceCondition.Enabled = true;
                            LoadDataTocboServiceCondition(data.SERVICE_ID);
                            cboServiceCondition.EditValue = data.SERVICE_CONDITION_ID;
                        }
                        else
                        {
                            cboServiceCondition.Enabled = true;
                            LoadDataTocboServiceCondition(data.SERVICE_ID);
                            cboServiceCondition.EditValue = null;
                        }
                    }
                    else
                    {
                        cboServiceCondition.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValuePatientType(GridLookUpEdit gridLookUpEdit, List<HIS_PATIENT_TYPE> listSelect, List<HIS_PATIENT_TYPE> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    gridLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.ID == p.ID)).ToList();
                    GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();
                    gridCheckMark.Selection.AddRange(selectFilter);
                }
                gridLookUpEdit.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValueBranch(GridLookUpEdit gridLookUpEdit, List<HIS_BRANCH> listSelect, List<HIS_BRANCH> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    gridLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.ID == p.ID)).ToList();
                    GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();
                    gridCheckMark.Selection.AddRange(selectFilter);
                }
                gridLookUpEdit.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetValueRationTime(GridLookUpEdit gridLookUpEdit, List<HIS_RATION_TIME> listSelect, List<HIS_RATION_TIME> listAll)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                gridLookUpEdit.Properties.DataSource = listAll;
                List<HIS_RATION_TIME> selectFilter = null;
                if (listSelect != null)
                {
                    selectFilter = listAll.Where(o => listSelect.Exists(p => o.ID == p.ID)).ToList();
                }
                gridCheckMark.Selection.Clear();
                if (selectFilter != null)
                    gridCheckMark.Selection.AddRange(selectFilter);
                gridLookUpEdit.Text = null;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Fill data to From
        private void FillDataToEditorControl(V_HIS_SERVICE_PATY data)
        {
            try
            {
                if (data != null)
                {
                    servicePatyId = data.ID;
                    cboServiceTypeName.EditValue = data.SERVICE_TYPE_ID;
                    cboServiceType.EditValue = data.SERVICE_TYPE_ID;
                    cboService.EditValue = data.SERVICE_ID;
                    txtService.Text = data.SERVICE_CODE;
                    cboServiceTypeName.ReadOnly = true;
                    LoadDataTocboServiceName(null);
                    cboServiceName.EditValue = data.SERVICE_ID;
                    cboServiceName.ReadOnly = true;
                    patientTypeNameSelecteds = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.ID == data.PATIENT_TYPE_ID).ToList();
                    branchNameSelecteds = BackendDataWorker.Get<HIS_BRANCH>().Where(o => o.ID == data.BRANCH_ID).ToList();
                    cboPatientTypeName.Text = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == data.PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                    cboPatientClassify.EditValue = data.PATIENT_CLASSIFY_ID;
                    cboBranchName.Text = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == data.BRANCH_ID).BRANCH_NAME;
                    if (data.RATION_TIME_ID != null)
                    {
                        rationTimeNameSelecteds = BackendDataWorker.Get<HIS_RATION_TIME>().Where(o => o.ID == data.RATION_TIME_ID).ToList();
                        cboRationTime.Text = BackendDataWorker.Get<HIS_RATION_TIME>().FirstOrDefault(o => o.ID == data.RATION_TIME_ID).RATION_TIME_NAME;
                    }
                    else
                    {
                        rationTimeNameSelecteds = null;
                        cboRationTime.Text = null;
                    }
                    spinPrice.EditValue = data.PRICE;
                    spinVatRatio.EditValue = (data.VAT_RATIO) * 100;
                    spinIntructionFrom.EditValue = data.INTRUCTION_NUMBER_FROM;
                    spinIntructionTo.EditValue = data.INTRUCTION_NUMBER_TO;
                    spInstrNumByTypeFrom.EditValue = data.INSTR_NUM_BY_TYPE_FROM;
                    spInstrNumByTypeTo.EditValue = data.INSTR_NUM_BY_TYPE_TO;
                    spinOvertimePrice.EditValue = data.OVERTIME_PRICE;
                    spinActualPrice.EditValue = data.ACTUAL_PRICE;
                    if (data.FROM_TIME != null)
                    {
                        dtFromTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.FROM_TIME ?? 0);
                    }
                    else
                    {
                        dtFromTime.EditValue = null;
                    }
                    if (data.TO_TIME != null)
                    {
                        dtToTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TO_TIME ?? 0);
                    }
                    else
                    {
                        dtToTime.EditValue = null;
                    }

                    if (data.TREATMENT_FROM_TIME != null)
                    {
                        dtTreatmentFromTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TREATMENT_FROM_TIME ?? 0);
                    }
                    else
                    {
                        dtTreatmentFromTime.EditValue = null;
                    }
                    if (data.TREATMENT_TO_TIME != null)
                    {
                        dtTreatmentToTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TREATMENT_TO_TIME ?? 0);
                    }
                    else
                    {
                        dtTreatmentToTime.EditValue = null;
                    }

                    if (data.SERVICE_RATIO == null)
                    {
                        spinPayoutRatio.EditValue = 0;
                    }
                    spinPayoutRatio.EditValue = data.SERVICE_RATIO * 100;
                    spinPriority.EditValue = data.PRIORITY;
                    cboDayFrom.EditValue = data.DAY_FROM;
                    cboDayTo.EditValue = data.DAY_TO;
                    cboHourForm.EditValue = data.HOUR_FROM;
                    cboHourTo.EditValue = data.HOUR_TO;
                    txtServiceTypeName.EditValue = data.SERVICE_TYPE_CODE;
                    txtServiceTypeName.ReadOnly = true;
                    txtServiceName.EditValue = data.SERVICE_CODE;
                    txtServiceName.ReadOnly = true;
                    if (data.PACKAGE_ID != null)
                    {
                        cboPackageService1.EditValue = data.PACKAGE_ID;
                    }
                    else
                    {
                        cboPackageService1.EditValue = null;
                    }

                    //txtPatientTypeName.EditValue = data.PATIENT_TYPE_CODE;
                    //txtBranchName.EditValue = data.BRANCH_CODE;

                    FillDataTogridViewDepartMent(GetListId(data.REQUEST_DEPARMENT_IDS));  
                    FillDataTogridViewRoomPractice(GetListId(data.EXECUTE_ROOM_IDS));
                    FillDataTogridViewRoomRequired(GetListId(data.REQUEST_ROOM_IDS));

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToEditorByServiceControl(V_HIS_SERVICE data)
        {
            try
            {
                if (data != null)
                {
                    cboServiceTypeName.EditValue = data.SERVICE_TYPE_ID;
                    cboServiceType.EditValue = data.SERVICE_TYPE_ID;
                    cboService.EditValue = data.ID;
                    txtService.Text = data.SERVICE_CODE;
                    cboServiceTypeName.ReadOnly = true;
                    LoadDataTocboServiceName(null);
                    cboServiceName.EditValue = data.ID;

                    txtServiceTypeName.EditValue = data.SERVICE_TYPE_CODE;
                    txtServiceTypeName.ReadOnly = true;
                    txtServiceName.EditValue = data.SERVICE_CODE;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<long> GetListId(string str)
        {
            List<long> ids = new List<long>();
            try
            {
                if (!String.IsNullOrEmpty(str))
                {
                    string[] listids = str.Split(',');
                    foreach (var item in listids)
                    {
                        if (!String.IsNullOrEmpty(item))
                            ids.Add(Inventec.Common.TypeConvert.Parse.ToInt64(item));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<long>();
            }
            return ids;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (ListServicePatyprocess != null)
                {
                    CreateImportProcess();
                }
                else
                {
                    SaveProcess();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xử lý import dữ liệu 1 danh sách từ ...
        private void CreateImportProcess()
        {
            try
            {
                try
                {
                    bool success = false;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    var resultData = new BackendAdapter(param).Post<List<HIS_SERVICE_PATY>>(
                   "api/HisServicePaty/CreateList", ApiConsumers.MosConsumer, ListServicePatyprocess, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ListServicePatyprocess = null;
                    }
                    MessageManager.Show(this, param, success);
                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// </summary>

        private void SaveProcess()
        {
            try
            {

                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                {
                    return;
                }
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    try
                    {
                        CommonParam param = new CommonParam();

                        List<HIS_SERVICE_PATY> listServicePatyCreate = new List<HIS_SERVICE_PATY>();
                        if (this.branchNameSelecteds != null && this.branchNameSelecteds.Count > 0 && this.patientTypeNameSelecteds != null && this.patientTypeNameSelecteds.Count > 0)
                        {
                            HIS_SERVICE_PATY DTO = new HIS_SERVICE_PATY();

                            if (cboServiceName.EditValue != null) DTO.SERVICE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceName.EditValue ?? "0").ToString());

                            UpdateDTOFromDataForm(ref DTO);
                            GetDataGrid(ref DTO);
                            if (this.rationTimeNameSelecteds != null && this.rationTimeNameSelecteds.Count > 0)
                            {
                                foreach (var branchId in branchNameSelecteds)
                                {
                                    foreach (var patientTypeId in patientTypeNameSelecteds)
                                    {
                                        foreach (var rationTimeId in rationTimeNameSelecteds)
                                        {
                                            HIS_SERVICE_PATY updateDTO = new HIS_SERVICE_PATY();
                                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_PATY>(updateDTO, DTO);
                                            updateDTO.BRANCH_ID = branchId.ID;
                                            updateDTO.PATIENT_TYPE_ID = patientTypeId.ID;
                                            updateDTO.RATION_TIME_ID = rationTimeId.ID;
                                            var dataService = this.services.FirstOrDefault(o => o.ID == updateDTO.SERVICE_ID);

                                            if (dataService != null)
                                            {
                                                //11896
                                                if (!CheckHeinPrice(updateDTO, dataService))
                                                {
                                                    return;
                                                }
                                            }
                                            listServicePatyCreate.Add(updateDTO);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                foreach (var branchId in branchNameSelecteds)
                                {
                                    foreach (var patientTypeId in patientTypeNameSelecteds)
                                    {
                                        HIS_SERVICE_PATY updateDTO = new HIS_SERVICE_PATY();
                                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_PATY>(updateDTO, DTO);
                                        updateDTO.BRANCH_ID = branchId.ID;
                                        updateDTO.PATIENT_TYPE_ID = patientTypeId.ID;
                                        updateDTO.RATION_TIME_ID = null;
                                        var dataService = this.services.FirstOrDefault(o => o.ID == updateDTO.SERVICE_ID);

                                        if (dataService != null)
                                        {
                                            //11896
                                            if (!CheckHeinPrice(updateDTO, dataService))
                                            {
                                                return;
                                            }
                                        }
                                        listServicePatyCreate.Add(updateDTO);
                                    }
                                }
                            }

                        }
                        else
                        {
                            List<string> message = new List<string>();
                            if (this.branchNameSelecteds == null || this.branchNameSelecteds.Count == 0)
                            {
                                message.Add("Chưa chọn chi nhánh");
                            }
                            if (this.patientTypeNameSelecteds == null || this.patientTypeNameSelecteds.Count == 0)
                            {
                                message.Add("Chưa chọn đối tượng");
                            }
                            if (message.Count > 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(string.Join(", ", message), "Thông báo");
                                return;
                            }
                        }
                        SaveListProcess(ref listServicePatyCreate);

                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                else
                {
                    List<string> message = new List<string>();
                    if (this.branchNameSelecteds == null || this.branchNameSelecteds.Count == 0)
                    {
                        message.Add("Chưa chọn chi nhánh");
                    }
                    if (this.patientTypeNameSelecteds == null || this.patientTypeNameSelecteds.Count == 0)
                    {
                        message.Add("Chưa chọn đối tượng");
                    }

                    if (patientTypeNameSelecteds.Count > 1)
                    {
                        message.Add("Khi sửa chính sách giá không được chọn nhiều đối tượng");
                    }
                    if (branchNameSelecteds.Count > 1)
                    {
                        message.Add("Khi sửa chính sách giá không được chọn nhiều chi nhánh");
                    }
                    if (rationTimeNameSelecteds != null && rationTimeNameSelecteds.Count > 1)
                    {
                        message.Add("Khi sửa chính sách giá không được chọn nhiều bữa ăn");
                    }
                    if (message.Count > 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Join(", ", message), "Thông báo");
                        return;
                    }
                    CommonParam param = new CommonParam();
                    HIS_SERVICE_PATY updateDTO = new HIS_SERVICE_PATY();
                    HisServicePatyFilter filter = new HisServicePatyFilter();
                    filter.ID = servicePatyId;
                    var rs = new BackendAdapter(param).Get<List<HIS_SERVICE_PATY>>("api/HisServicePaty/Get", ApiConsumers.MosConsumer, filter, param);
                    if (rs != null && rs.Count > 0)
                    {
                        updateDTO = rs.First();
                    }
                    UpdateDTOFromDataForm(ref updateDTO);

                    updateDTO.BRANCH_ID = branchNameSelecteds.First().ID;
                    updateDTO.PATIENT_TYPE_ID = patientTypeNameSelecteds.First().ID;
                    if (rationTimeNameSelecteds != null && rationTimeNameSelecteds.Count > 0)
                    {
                        updateDTO.RATION_TIME_ID = rationTimeNameSelecteds.First().ID;
                    }
                    else
                        updateDTO.RATION_TIME_ID = null;
                    var dataService = this.services.FirstOrDefault(o => o.ID == updateDTO.SERVICE_ID);

                    if (dataService != null)
                    {
                        //11896
                        if (!CheckHeinPrice(updateDTO, dataService))
                        {
                            return;
                        }
                    }

                    GetDataGrid(ref updateDTO);
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERVICE_PATY>(
                    "api/HisServicePaty/Update", ApiConsumers.MosConsumer, updateDTO, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        //UpdateRowDataAfterEdit(resultData);
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal bool CheckHeinPrice(HIS_SERVICE_PATY data, V_HIS_SERVICE hisService)
        {
            bool valid = true;
            try
            {
                long patientTypeId_Bhyt = Inventec.Common.TypeConvert.Parse.ToInt32(HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"));
                if (data.OVERTIME_PRICE.HasValue && data.OVERTIME_PRICE > data.PRICE)
                {

                    DevExpress.XtraEditors.XtraMessageBox.Show("'Giá chệnh lệch' bạn nhập lớn hơn 'Giá'", "Thông báo");
                    return false;
                }
                if (data != null && data.PATIENT_TYPE_ID == patientTypeId_Bhyt && hisService != null)
                {
                    decimal? heinLimitPrice = null;
                    this.GetHeinLimitPrice(hisService, data, ref heinLimitPrice);
                    if (heinLimitPrice.HasValue && heinLimitPrice.Value >= data.PRICE)
                    {

                        DevExpress.XtraEditors.XtraMessageBox.Show("Giá bạn nhập nhỏ hơn hoặc bằng 'Giá trần BHYT'", "Thông báo");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        //private bool CheckTime(HIS_SERVICE_PATY data, V_HIS_SERVICE hisService)
        //{
        //    bool rs = false;
        //    try
        //    {
        //        if (data.FROM_TIME == null && data.TO_TIME != null)
        //        {
        //            if (hisService.HEIN_LIMIT_PRICE_IN_TIME != null && hisService.HEIN_LIMIT_PRICE_IN_TIME <= data.TO_TIME)
        //                rs = true;
        //        }
        //        else if (data.FROM_TIME != null && data.TO_TIME == null)
        //        {
        //            if (hisService.HEIN_LIMIT_PRICE_IN_TIME != null && hisService.HEIN_LIMIT_PRICE_IN_TIME >= data.FROM_TIME)
        //                rs = true;
        //        }
        //        else if (data.FROM_TIME != null && data.TO_TIME != null)
        //        {
        //            if (hisService.HEIN_LIMIT_PRICE_IN_TIME != null && hisService.HEIN_LIMIT_PRICE_IN_TIME >= data.FROM_TIME && hisService.HEIN_LIMIT_PRICE_IN_TIME <= data.TO_TIME)
        //                rs = true;
        //        }
        //        else if (data.FROM_TIME == null && data.TO_TIME == null)
        //        {
        //            return rs;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //        return false;
        //    }
        //    return rs;
        //}

        private void GetHeinLimitPrice(V_HIS_SERVICE hisService, HIS_SERVICE_PATY data, ref decimal? heinLimitPrice)
        {
            if (hisService.HEIN_LIMIT_PRICE.HasValue || hisService.HEIN_LIMIT_PRICE_OLD.HasValue)
            {
                //neu gia ap dung theo ngay vao vien, thi cac benh nhan vao vien truoc ngay ap dung se lay gia cu
                if (hisService.HEIN_LIMIT_PRICE_IN_TIME.HasValue)
                {
                    if ((!data.FROM_TIME.HasValue || data.FROM_TIME.Value <= hisService.HEIN_LIMIT_PRICE_IN_TIME) && (!data.TO_TIME.HasValue || data.TO_TIME.Value >= hisService.HEIN_LIMIT_PRICE_IN_TIME))
                    {
                        heinLimitPrice = hisService.HEIN_LIMIT_PRICE ?? hisService.HEIN_LIMIT_PRICE_OLD;
                    }
                    else if ((!data.TREATMENT_FROM_TIME.HasValue || data.TREATMENT_FROM_TIME.Value <= hisService.HEIN_LIMIT_PRICE_IN_TIME) && (!data.TREATMENT_TO_TIME.HasValue || data.TREATMENT_TO_TIME.Value >= hisService.HEIN_LIMIT_PRICE_IN_TIME))
                    {
                        heinLimitPrice = hisService.HEIN_LIMIT_PRICE ?? hisService.HEIN_LIMIT_PRICE_OLD;
                    }
                }
                //neu ap dung theo ngay chi dinh, thi cac chi dinh truoc ngay ap dung se tinh gia cu
                else if (hisService.HEIN_LIMIT_PRICE_INTR_TIME.HasValue)
                {
                    if ((!data.FROM_TIME.HasValue || data.FROM_TIME.Value <= hisService.HEIN_LIMIT_PRICE_INTR_TIME) && (!data.TO_TIME.HasValue || data.TO_TIME.Value >= hisService.HEIN_LIMIT_PRICE_INTR_TIME))
                    {
                        heinLimitPrice = hisService.HEIN_LIMIT_PRICE ?? hisService.HEIN_LIMIT_PRICE_OLD;
                    }
                    else if ((!data.TREATMENT_FROM_TIME.HasValue || data.TREATMENT_FROM_TIME.Value <= hisService.HEIN_LIMIT_PRICE_INTR_TIME) && (!data.TREATMENT_TO_TIME.HasValue || data.TREATMENT_TO_TIME.Value >= hisService.HEIN_LIMIT_PRICE_INTR_TIME))
                    {
                        heinLimitPrice = hisService.HEIN_LIMIT_PRICE ?? hisService.HEIN_LIMIT_PRICE_OLD;
                    }
                }
                //neu ca 2 truong ko co gia tri thi luon lay theo gia moi
                else
                {
                    heinLimitPrice = hisService.HEIN_LIMIT_PRICE ?? hisService.HEIN_LIMIT_PRICE_OLD;
                }
            }
        }

        private void GetDataGrid(ref HIS_SERVICE_PATY servicePaty)
        {
            try
            {
                var listDepartment = GetListId(servicePaty.REQUEST_DEPARMENT_IDS);
                if (listDepartment != null && listDepartment.Count > 0)
                {
                    List<long> listSave = new List<long>();
                    var dataDepartments = (List<DepartmentADO>)gridControlDepartMent.DataSource;
                    var check = dataDepartments.Where(p => p.Check && !listDepartment.Contains(p.ID)).ToList();
                    var uncheck = dataDepartments.Where(p => !p.Check && listDepartment.Contains(p.ID)).ToList();
                    var listCheck = listDepartment.Where(o => !uncheck.Select(p => p.ID).Contains(o)).ToList();

                    if (check != null && check.Count > 0)
                    {
                        listSave.AddRange(check.Select(o => o.ID).ToList());
                    }
                    if (listCheck != null && listCheck.Count > 0)
                    {
                        listSave.AddRange(listCheck);
                    }

                    if (listSave != null && listSave.Count > 0)
                    {
                        servicePaty.REQUEST_DEPARMENT_IDS = String.Join(",", listSave);
                    }
                    else
                    {
                        servicePaty.REQUEST_DEPARMENT_IDS = "";
                    }
                }
                else
                {
                    var dataDepartments = (List<DepartmentADO>)gridControlDepartMent.DataSource;
                    dataDepartments = dataDepartments.Where(p => p.Check == true).ToList();
                    servicePaty.REQUEST_DEPARMENT_IDS = "";
                    if (dataDepartments != null && dataDepartments.Count > 0)
                    {
                        List<long> sereServIds = dataDepartments.Select(o => o.ID).ToList();
                        servicePaty.REQUEST_DEPARMENT_IDS = String.Join(",", sereServIds);
                    }
                }

                var listExecuteRoom = GetListId(servicePaty.EXECUTE_ROOM_IDS);
                if (listExecuteRoom != null && listExecuteRoom.Count > 0)
                {
                    List<long> listSave = new List<long>();
                    var dataExecuteRooms = (List<RoomPracticeADO>)gridControlRoomPractice.DataSource;
                    var check = dataExecuteRooms.Where(p => p.Check && !listExecuteRoom.Contains(p.ID)).ToList();
                    var uncheck = dataExecuteRooms.Where(p => !p.Check && listExecuteRoom.Contains(p.ID)).ToList();
                    var listCheck = listExecuteRoom.Where(o => !uncheck.Select(p => p.ID).Contains(o)).ToList();

                    if (check != null && check.Count > 0)
                    {
                        listSave.AddRange(check.Select(o => o.ID).ToList());
                    }
                    if (listCheck != null && listCheck.Count > 0)
                    {
                        listSave.AddRange(listCheck);
                    }

                    if (listSave != null && listSave.Count > 0)
                    {
                        servicePaty.EXECUTE_ROOM_IDS = String.Join(",", listSave);
                    }
                    else
                    {
                        servicePaty.EXECUTE_ROOM_IDS = "";
                    }
                }
                else
                {
                    var RoomPratices = (List<RoomPracticeADO>)gridControlRoomPractice.DataSource;
                    RoomPratices = RoomPratices.Where(p => p.Check == true).ToList();
                    servicePaty.EXECUTE_ROOM_IDS = "";
                    if (RoomPratices != null && RoomPratices.Count > 0)
                    {
                        List<long> sereServIds = RoomPratices.Select(o => o.ID).ToList();
                        servicePaty.EXECUTE_ROOM_IDS = String.Join(",", sereServIds);
                    }
                }

                var listRequestRoom = GetListId(servicePaty.REQUEST_ROOM_IDS);
                if (listRequestRoom != null && listRequestRoom.Count > 0)
                {
                    List<long> listSave = new List<long>();
                    var dataRequestRooms = (List<RoomRequiredADO>)gridControlRoomRequired.DataSource;
                    var check = dataRequestRooms.Where(p => p.Check && !listRequestRoom.Contains(p.ID)).ToList();
                    var uncheck = dataRequestRooms.Where(p => !p.Check && listRequestRoom.Contains(p.ID)).ToList();
                    var listCheck = listRequestRoom.Where(o => !uncheck.Select(p => p.ID).Contains(o)).ToList();

                    if (check != null && check.Count > 0)
                    {
                        listSave.AddRange(check.Select(o => o.ID).ToList());
                    }
                    if (listCheck != null && listCheck.Count > 0)
                    {
                        listSave.AddRange(listCheck);
                    }

                    if (listSave != null && listSave.Count > 0)
                    {
                        servicePaty.REQUEST_ROOM_IDS = String.Join(",", listSave);
                    }
                    else
                    {
                        servicePaty.REQUEST_ROOM_IDS = "";
                    }
                }
                else
                {
                    var RoomRequireds = (List<RoomRequiredADO>)gridControlRoomRequired.DataSource;
                    RoomRequireds = RoomRequireds.Where(p => p.Check == true).ToList();
                    servicePaty.REQUEST_ROOM_IDS = "";
                    if (RoomRequireds != null && RoomRequireds.Count > 0)
                    {
                        List<long> sereServIds = RoomRequireds.Select(o => o.ID).ToList();
                        servicePaty.REQUEST_ROOM_IDS = String.Join(",", sereServIds);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                ChkInActive.Checked = false;
                txtServiceTypeName.ReadOnly = false;
                cboServiceTypeName.ReadOnly = false;
                txtServiceName.ReadOnly = false;
                cboServiceName.ReadOnly = false;
                btnEdit.Enabled = false;
                txtFind1.EditValue = null;
                txtFind2.EditValue = null;
                txtFind3.EditValue = null;
                //txtBranchName.EditValue = null;
                txtServiceName.EditValue = null;
                txtServiceTypeName.EditValue = null;
                //txtPatientTypeName.EditValue = null;
                branchNameSelecteds = new List<HIS_BRANCH>();
                patientTypeNameSelecteds = new List<HIS_PATIENT_TYPE>();
                rationTimeNameSelecteds = new List<HIS_RATION_TIME>();
                //patientTypeNameSelecteds.Add(new HIS_PATIENT_TYPE());
                cboPatientTypeName.Text = "";
                cboBranchName.Text = "";
                cboRationTime.Text = "";
                SetValuePatientType(this.cboPatientTypeName, this.patientTypeNameSelecteds, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());
                SetValueBranch(this.cboBranchName, this.branchNameSelecteds, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>());
                SetValueRationTime(this.cboRationTime, this.rationTimeNameSelecteds, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_RATION_TIME>().Where(o => o.IS_ACTIVE == 1).ToList());
                cboServiceName.EditValue = null;
                cboServiceTypeName.EditValue = null;
                spinPayoutRatio.EditValue = null;
                spinPrice.EditValue = null;
                spinVatRatio.EditValue = 0;
                spinIntructionFrom.EditValue = null;
                spinIntructionTo.EditValue = null;
                spInstrNumByTypeFrom.EditValue = null;
                spInstrNumByTypeTo.EditValue = null;
                dtFromTime.EditValue = null;
                dtToTime.EditValue = null;
                dtTreatmentFromTime.EditValue = null;
                dtTreatmentToTime.EditValue = null;
                spinPriority.EditValue = null;
                cboDayFrom.EditValue = null;
                cboDayTo.EditValue = null;
                cboHourForm.EditValue = null;
                cboHourTo.EditValue = null;
                spinOvertimePrice.EditValue = null;
                spinActualPrice.EditValue = null;
                cboServiceType.EditValue = null;
                cboService.EditValue = null;
                txtService.EditValue = null;
                cboRoomType.EditValue = null;
                cboRoomType1.EditValue = null;
                cboPackageService1.EditValue = null;
                LoadDataTocboServiceName(null);
                LoadDataTocboService(null);
                cboServiceCondition.Enabled = false;
                cboServiceCondition.EditValue = null;
                cboPatientClassify.EditValue = null;
                cboRationTime.Properties.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void UpdateRowDataAfterEdit(HIS_SERVICE_PATY data)
        //{
        //    try
        //    {
        //        if (data == null)
        //            throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_SERVICE_PATY) is null");
        //        var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY)gridViewServicePaty.GetFocusedRow();
        //        if (rowData != null)
        //        {
        //            CommonParam param = new CommonParam();
        //            HisServicePatyViewFilter filter = new HisServicePatyViewFilter()
        //            {
        //                ID = data.ID
        //            };
        //            rowData = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>(
        //           "api/HisServicePaty/GetView", ApiConsumers.MosConsumer, filter, param);
        //            gridViewServicePaty.RefreshRow(gridViewServicePaty.FocusedRowHandle);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //Update dữ liệu để lưu
        private void UpdateDTOFromDataForm(ref HIS_SERVICE_PATY updateDTO)
        {
            try
            {
                //PRIORITY
                //updateDTO.SERVICE_RATIO = 
                decimal spinInput = (decimal)spinPayoutRatio.Value / 100;
                updateDTO.SERVICE_RATIO = spinInput;
                if (updateDTO.SERVICE_RATIO == 0)
                {
                    updateDTO.SERVICE_RATIO = null;
                }
                updateDTO.PRICE = (decimal)spinPrice.Value;
                updateDTO.VAT_RATIO = Convert.ToDecimal((long)spinVatRatio.Value * 0.01);
                updateDTO.INTRUCTION_NUMBER_FROM = (long)spinIntructionFrom.Value;
                if (updateDTO.INTRUCTION_NUMBER_FROM == 0)
                {
                    updateDTO.INTRUCTION_NUMBER_FROM = null;
                }
                updateDTO.INTRUCTION_NUMBER_TO = (long)spinIntructionTo.Value;
                if (updateDTO.INTRUCTION_NUMBER_TO == 0)
                {
                    updateDTO.INTRUCTION_NUMBER_TO = null;
                }
                if (spInstrNumByTypeFrom.EditValue != null)
                    updateDTO.INSTR_NUM_BY_TYPE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(spInstrNumByTypeFrom.EditValue.ToString());
                else
                    updateDTO.INSTR_NUM_BY_TYPE_FROM = null;

                if (spInstrNumByTypeTo.EditValue != null)
                    updateDTO.INSTR_NUM_BY_TYPE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(spInstrNumByTypeTo.EditValue.ToString());
                else
                    updateDTO.INSTR_NUM_BY_TYPE_TO = null;

                updateDTO.OVERTIME_PRICE = (long)spinOvertimePrice.Value;
                if (updateDTO.OVERTIME_PRICE == 0)
                {
                    updateDTO.OVERTIME_PRICE = null;
                }
                if (spinActualPrice.EditValue != null)
                    updateDTO.ACTUAL_PRICE = spinActualPrice.Value;
                else
                    updateDTO.ACTUAL_PRICE = null;

                if (dtFromTime.DateTime != null && dtFromTime.DateTime != DateTime.MinValue)
                {
                    updateDTO.FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtFromTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    updateDTO.FROM_TIME = null;
                }
                if (dtToTime.DateTime != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    updateDTO.TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtToTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    updateDTO.TO_TIME = null;
                }

                if (dtTreatmentFromTime.DateTime != null && dtTreatmentFromTime.DateTime != DateTime.MinValue)
                {
                    updateDTO.TREATMENT_FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTreatmentFromTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    updateDTO.TREATMENT_FROM_TIME = null;
                }
                if (dtTreatmentToTime.DateTime != null && dtTreatmentToTime.DateTime != DateTime.MinValue)
                {
                    updateDTO.TREATMENT_TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTreatmentToTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    updateDTO.TREATMENT_TO_TIME = null;
                }
                updateDTO.PRIORITY = (long)spinPriority.Value;
                if (updateDTO.PRIORITY == 0)
                {
                    updateDTO.PRIORITY = null;
                }
                if (cboDayFrom.EditValue != null)
                {
                    updateDTO.DAY_FROM = Convert.ToInt16(cboDayFrom.EditValue);
                }
                else { updateDTO.DAY_FROM = null; }
                if (cboDayTo.EditValue != null)
                {
                    updateDTO.DAY_TO = Convert.ToInt16(cboDayTo.EditValue);
                }
                else { updateDTO.DAY_TO = null; }

                if (cboHourForm.EditValue != null) { updateDTO.HOUR_FROM = (string)cboHourForm.EditValue; }
                else { updateDTO.HOUR_FROM = null; }

                if (cboHourTo.EditValue != null) { updateDTO.HOUR_TO = (string)cboHourTo.EditValue; }
                else
                {
                    updateDTO.HOUR_TO = null;
                }

                if (cboPackageService1.EditValue != null && cboPackageService1.EditValue.ToString() != "")
                { updateDTO.PACKAGE_ID = (long)cboPackageService1.EditValue; }
                else
                {
                    updateDTO.PACKAGE_ID = null;
                }
                if (cboServiceCondition.EditValue != null && cboServiceCondition.EditValue.ToString() != "")
                { updateDTO.SERVICE_CONDITION_ID = (long)cboServiceCondition.EditValue; }
                else
                {
                    updateDTO.SERVICE_CONDITION_ID = null;
                }
                if (cboPatientClassify.EditValue != null)
                    updateDTO.PATIENT_CLASSIFY_ID = (long)cboPatientClassify.EditValue;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //validation dữ liệu bắt buộc
        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(spinPrice);
                ValidationSingleControl(spinVatRatio);
                //ValidationSingleControl(txtPatientTypeName);
                ValidationSingleControl(txtServiceTypeName);
                ValidationSingleControl(txtServiceName);
                //ValidationSingleControl(txtBranchName);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format(ResourceMessage.ChuaNhapTruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ListServicePatyprocess = null;
                ResetFormData();
                FillDataToGridControl();
                FillDataTogridViewRoomPractice(new List<long>());
                FillDataTogridViewRoomRequired(new List<long>());
                FillDataTogridViewDepartMent(new List<long>());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceTypeName_KeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboServiceTypeName.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE gt = this.serviceTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceTypeName.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtServiceName.Focus();
                            cboServiceTypeName.ShowPopup();
                            LoadDataTocboServiceName(gt.ID);
                        }
                    }
                    else
                    {
                        cboServiceTypeName.ShowPopup();
                    }
                }
                else
                {
                    cboServiceTypeName.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceTypeName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboServiceTypeName.EditValue != null && cboServiceTypeName.EditValue != cboServiceTypeName.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE gt = this.serviceTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceTypeName.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtServiceTypeName.Text = gt.SERVICE_TYPE_CODE;
                            txtServiceName.Focus();
                            LoadDataTocboServiceName(gt.ID);
                        }
                    }
                    else
                    {
                        txtServiceName.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboServiceType.EditValue != null && cboServiceType.EditValue != cboServiceType.OldEditValue)
                    {
                        txtService.Text = "";
                        cboService.EditValue = null;
                        MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE gt = this.serviceTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtService.Focus();
                            LoadDataTocboService(gt.ID);
                        }
                    }
                    else
                    {
                        txtService.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboServiceName.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE gt = this.services.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceName.EditValue.ToString()));
                        if (gt != null)
                        {
                            cboPatientTypeName.Focus();
                            cboServiceName.ShowPopup();
                        }
                    }
                    else
                    {
                        cboServiceName.ShowPopup();
                    }
                }
                else
                {
                    cboServiceName.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboServiceName.EditValue != null && cboServiceName.EditValue != cboServiceName.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE gt = this.services.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceName.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtServiceName.Text = gt.SERVICE_CODE;
                            cboPatientTypeName.Focus();
                        }
                        cboServiceCondition.Enabled = true;
                        LoadDataTocboServiceCondition(gt.ID);
                    }
                    else
                    {
                        cboPatientTypeName.Focus();
                        cboServiceCondition.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboService_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboService.EditValue != null && cboService.EditValue != cboService.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE gt = this.services.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboService.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtService.Text = gt.SERVICE_CODE;
                            CboPatientTypeFilter.Focus();
                        }
                    }
                    else
                    {
                        CboPatientTypeFilter.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void cboPatientTypeName_KeyUp(object sender, KeyEventArgs e)
        //{

        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            if (cboPatientTypeName.EditValue != null)
        //            {
        //                MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE gt = this.patientTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientTypeName.EditValue.ToString()));
        //                if (gt != null)
        //                {
        //                    cboBranchName.Focus();
        //                }
        //            }
        //            else
        //            {
        //                cboPatientTypeName.ShowPopup();
        //            }
        //        }
        //        else
        //        {
        //            cboPatientTypeName.ShowPopup();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void cboPatientTypeName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPatientTypeName.EditValue != null && cboPatientTypeName.EditValue != cboPatientTypeName.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE gt = this.patientTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientTypeName.EditValue.ToString()));
                        if (gt != null)
                        {
                            cboPatientClassify.Focus();
                        }
                    }
                    else
                    {
                        cboPatientClassify.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBranchName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBranchName.EditValue != null && cboBranchName.EditValue != cboBranchName.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_BRANCH gt = this.branchs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBranchName.EditValue.ToString()));
                        if (gt != null)
                        {
                            //txtBranchName.Text = gt.BRANCH_CODE;
                            cboPatientTypeName.Focus();
                        }
                    }
                    else
                    {
                        cboPatientTypeName.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void cboBranchName_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            if (cboBranchName.EditValue != null)
        //            {
        //                spinPrice.Focus();
        //            }
        //            else
        //            {
        //                cboBranchName.ShowPopup();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void spinPrice_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinVatRatio.Focus();
                    spinVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinVatRatio_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinOvertimePrice.Focus();
                    spinOvertimePrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinIntructionFrom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinIntructionTo.Focus();
                    spinIntructionTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinIntructionTo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spInstrNumByTypeFrom.Enabled)
                    {
                        spInstrNumByTypeFrom.Focus();
                        spInstrNumByTypeFrom.SelectAll();
                    }
                    else
                    {
                        dtFromTime.Focus();
                        dtFromTime.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtFromTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtFromTime.EditValue != null)
                    {
                        dtToTime.Focus();
                    }
                    else
                    {
                        dtFromTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtToTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtToTime.EditValue != null)
                    {
                        dtTreatmentFromTime.Focus();
                    }
                    else
                    {
                        dtToTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTreatmentFromTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtTreatmentFromTime.EditValue != null)
                    {
                        dtTreatmentToTime.Focus();
                    }
                    else
                    {
                        dtTreatmentFromTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTreatmentToTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtTreatmentToTime.EditValue != null)
                    {
                        cboDayFrom.Focus();
                    }
                    else
                    {
                        dtTreatmentToTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinPriority_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboRationTime.Enabled)
                    {
                        cboRationTime.Focus();
                        cboRationTime.SelectAll();
                    }
                    else
                    {
                        cboPackageService1.Focus();
                        cboPackageService1.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDayFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDayFrom.EditValue != null)
                    {
                        cboDayTo.Focus();
                    }
                    else
                    {
                        cboDayFrom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDayFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                cboDayTo.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDayTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDayTo.EditValue != null)
                    {
                        cboHourForm.Focus();
                    }
                    else
                    {
                        cboDayTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDayTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                cboHourForm.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHourForm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHourForm.EditValue != null)
                    {
                        cboHourTo.Focus();
                    }
                    else
                    {
                        cboHourForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHourForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                cboHourTo.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHourTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHourTo.EditValue != null)
                    {
                        gridControlRoomPractice.Focus();
                    }
                    else
                    {
                        cboHourTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHourTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                gridControlRoomPractice.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (V_HIS_SERVICE_PATY)gridViewServicePaty.GetFocusedRow();
                V_HIS_SERVICE_PATY rowData = data as V_HIS_SERVICE_PATY;

                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisServicePaty/Delete", ApiConsumers.MosConsumer, rowData, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
                        }
                        MessageManager.Show(this, param, success);
                    }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void txtFind1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            FillDataTogridViewRoomPractice(new List<long>());
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void txtFind3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            FillDataTogridViewRoomRequired(new List<long>());
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void dtToTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtToTime.DateTime = dtToTime.DateTime.Date.AddHours(23).AddMinutes(59);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtServiceTypeName.Text))
                    {
                        string key = Inventec.Common.String.Convert.UnSignVNese(this.txtServiceTypeName.Text.ToLower().Trim());
                        var data = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => Inventec.Common.String.Convert.UnSignVNese(o.SERVICE_TYPE_CODE.ToLower()).Contains(key)).ToList();
                        List<HIS_SERVICE_TYPE> result = (data != null ? (data.Count == 1 ? data : data.Where(o => o.SERVICE_TYPE_CODE == key).ToList()) : null);

                        if (result != null && result.Count == 1)
                        {
                            LoadDataTocboServiceName(result[0].ID);
                            txtServiceTypeName.Text = result[0].SERVICE_TYPE_CODE;
                            cboServiceTypeName.EditValue = result[0].ID;
                            FocusMoveText(txtServiceName);
                        }
                        else
                        {
                            cboServiceTypeName.EditValue = null;
                            FocusShowPopup(cboServiceTypeName);
                        }
                    }
                    else
                    {
                        cboServiceTypeName.EditValue = null;
                        FocusShowPopup(cboServiceTypeName);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtServiceName.Text))
                    {
                        string key = this.txtServiceName.Text.ToLower();
                        var data = this.services.Where(o => o.SERVICE_CODE.ToLower() == key).ToList();

                        //List<V_HIS_SERVICE> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.SERVICE_CODE.ToLower() == txtServiceName.Text.ToLower()).ToList()) : null);
                        if (data != null && data.Count == 1)
                        {
                            cboServiceName.EditValue = data[0].ID;
                            txtServiceName.Text = data[0].SERVICE_CODE;
                            FocusMoveText(cboPatientTypeName);
                            cboServiceCondition.Enabled = true;
                            LoadDataTocboServiceCondition(data[0].ID);
                        }
                        else
                        {
                            cboServiceName.EditValue = null;
                            FocusShowPopup(cboServiceName);
                        }
                    }
                    else
                    {
                        cboServiceName.EditValue = null;
                        FocusShowPopup(cboServiceName);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusShowPopup(GridLookUpEdit cbo)
        {
            try
            {
                cbo.Focus();
                cbo.ShowPopup();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusMoveText(TextEdit textEdit)
        {
            try
            {
                textEdit.Focus();
                textEdit.SelectAll();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtService_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataTocboService(null);
                    if (!String.IsNullOrEmpty(txtService.Text))
                    {
                        string key = this.txtService.Text.ToLower();
                        var data = this.services.Where(o => o.SERVICE_CODE.ToLower() == key).ToList();
                        //List<V_HIS_SERVICE> results = data != null ? (data.Count == 1 ? data : data.Where(o => o.SERVICE_CODE.ToLower() == key).ToList()) : null;
                        if (data != null && data.Count == 1)
                        {
                            txtService.Text = data[0].SERVICE_CODE;
                            cboService.EditValue = data[0].ID;
                            btnFind.Focus();
                        }
                        else
                        {
                            cboService.EditValue = null;
                            FocusShowPopup(cboService);
                        }
                    }
                    else
                    {
                        cboService.EditValue = null;
                        FocusShowPopup(cboService);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void txtPatientTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            bool valid = false;
        //            if (!String.IsNullOrEmpty(this.txtPatientTypeName.Text))
        //            {
        //                string key = Inventec.Common.String.Convert.UnSignVNese(txtPatientTypeName.Text.ToLower().Trim());
        //                var data = this.patientTypes.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.PATIENT_TYPE_CODE.ToLower()).Contains(key)).ToList();
        //                List<HIS_PATIENT_TYPE> results = data != null ? (data.Count == 1 ? data : data.Where(o => o.PATIENT_TYPE_CODE == (key)).ToList()) : null;
        //                if (results != null && results.Count == 1)
        //                {
        //                    txtPatientTypeName.Text = results[0].PATIENT_TYPE_CODE;
        //                    cboPatientTypeName.EditValue = results[0].ID;
        //                    FocusMoveText(txtBranchName);
        //                }
        //                else
        //                {
        //                    cboPatientTypeName.EditValue = null;
        //                    FocusShowPopup(cboPatientTypeName);
        //                }
        //            }
        //            else
        //            {
        //                cboPatientTypeName.EditValue = null;
        //                FocusShowPopup(cboPatientTypeName);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void txtBranchName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            if (!String.IsNullOrEmpty(txtBranchName.Text))
        //            {
        //                string key = Inventec.Common.String.Convert.UnSignVNese(txtBranchName.Text.ToLower().Trim());
        //                var data = this.branchs.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.BRANCH_CODE.ToLower().Trim()).Contains(key)).ToList();
        //                List<HIS_BRANCH> results = data != null ? (data.Count == 1 ? data : data.Where(o => o.BRANCH_CODE == key).ToList()) : null;
        //                if (results != null && results.Count == 1)
        //                {
        //                    txtBranchName.Text = results[0].BRANCH_CODE;
        //                    cboBranchName.EditValue = results[0].ID;
        //                    spinPrice.Focus();
        //                    spinPrice.SelectAll();
        //                }
        //                else
        //                {
        //                    cboBranchName.EditValue = null;
        //                    FocusShowPopup(cboBranchName);
        //                }
        //            }
        //            else
        //            {
        //                cboBranchName.EditValue = null;
        //                FocusShowPopup(cboBranchName);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void txtFind2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtFind2.Text.ToLower().Trim());
                var departMentTemp = lstDepartMentADOs.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.DEPARTMENT_NAME.ToLower()).Contains(keyword)).OrderByDescending(p => p.Check).ThenBy(q => q.DEPARTMENT_NAME).ToList();
                gridControlDepartMent.DataSource = null;
                gridControlDepartMent.DataSource = departMentTemp;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFind1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtFind1.Text.ToLower().Trim());
                var roomPraticeTemp = lstRoomPraticeADOs.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.ROOM_NAME.ToLower()).Contains(keyword)).OrderByDescending(p => p.Check).ThenBy(q => q.ROOM_NAME).ToList();
                gridControlRoomPractice.DataSource = null;
                gridControlRoomPractice.DataSource = roomPraticeTemp;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFind3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtFind3.Text.ToLower().Trim());
                var roomRequiredTemp = lstRoomRequiredADOs.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.ROOM_NAME.ToLower()).Contains(keyword)).OrderByDescending(p => p.Check).ThenBy(q => q.ROOM_NAME).ToList();
                gridControlRoomRequired.DataSource = null;
                gridControlRoomRequired.DataSource = roomRequiredTemp;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTreatmentToTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtTreatmentToTime.DateTime = dtTreatmentToTime.DateTime.Date.AddHours(23).AddMinutes(59);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinOvertimePrice_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinActualPrice.Focus();
                    spinActualPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoomType1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal && cboRoomType1.EditValue != null)
                {
                    var roomRequiredTemp = lstRoomRequiredADOs.Where(o => o.ROOM_TYPE_ID == (long)cboRoomType1.EditValue).ToList();
                    gridControlRoomRequired.DataSource = null;
                    gridControlRoomRequired.DataSource = roomRequiredTemp;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoomType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal && cboRoomType.EditValue != null)
                {
                    var roomPraticeTemp = lstRoomPraticeADOs.Where(o => o.ROOM_TYPE_ID == (long)cboRoomType.EditValue).ToList();
                    gridControlRoomPractice.DataSource = null;
                    gridControlRoomPractice.DataSource = roomPraticeTemp;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Xử lý check All cho grid
        private void gridViewRoomPractice_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "Check")
                        {
                            var lstCheckAll = lstRoomPraticeADOs;
                            List<RoomPracticeADO> lstChecks = new List<RoomPracticeADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.Check = true;
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
                                            item.Check = false;
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
                                gridControlRoomPractice.BeginUpdate();
                                gridControlRoomPractice.DataSource = lstChecks;
                                gridControlRoomPractice.EndUpdate();
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

        private void gridViewRoomRequired_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "Check")
                        {
                            var lstCheckAll = lstRoomRequiredADOs;
                            List<RoomRequiredADO> lstChecks = new List<RoomRequiredADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.Check = true;
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
                                            item.Check = false;
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
                                gridControlRoomRequired.BeginUpdate();
                                gridControlRoomRequired.DataSource = lstChecks;
                                gridControlRoomRequired.EndUpdate();
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

        private void gridViewDepartMent_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "Check")
                        {
                            var lstCheckAll = lstDepartMentADOs;
                            List<DepartmentADO> lstChecks = new List<DepartmentADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.Check = true;
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
                                            item.Check = false;
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
                                gridControlDepartMent.BeginUpdate();
                                gridControlDepartMent.DataSource = lstChecks;
                                gridControlDepartMent.EndUpdate();
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

        private void cboRoomType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboRoomType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ROOM_TYPE gt = this.roomTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomType.EditValue.ToString()));
                        if (gt != null)
                        {
                            cboRoomType.ShowPopup();
                        }
                    }
                    else
                    {
                        cboRoomType.ShowPopup();
                    }
                }
                else
                {
                    cboRoomType.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoomType1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboRoomType1.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ROOM_TYPE gt = this.roomTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomType1.EditValue.ToString()));
                        if (gt != null)
                        {
                            cboRoomType1.ShowPopup();
                        }
                    }
                    else
                    {
                        cboRoomType1.ShowPopup();
                    }
                }
                else
                {
                    cboRoomType1.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboService_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboService.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE gt = this.services.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboService.EditValue.ToString()));
                        if (gt != null)
                        {
                            btnFind.Focus();
                            cboService.ShowPopup();
                        }
                    }
                    else
                    {
                        cboService.ShowPopup();
                    }
                }
                else
                {
                    cboService.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboServiceType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE gt = this.serviceTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtService.Focus();
                            cboServiceType.ShowPopup();
                            LoadDataTocboService(gt.ID);
                        }
                    }
                    else
                    {
                        cboServiceType.ShowPopup();
                    }
                }
                else
                {
                    cboServiceType.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                    if (gridCheckMark == null) return;
                    gridCheckMark.ClearSelection(cboServiceType.Properties.View);
                    txtService.Focus();
                    //cboServiceType.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDayFrom_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDayFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboHourForm_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHourForm.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDayTo_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDayTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboHourTo_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHourTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore
                  (HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                  Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                  HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate(MPS.Processor.Mps000258.PDO.Mps000258PDO.printTypeCode, DelegateRunPrinterExportExcel);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinterExportExcel(string printCode, string fileName)
        {
            bool result = true;
            try
            {
                string savePath = null;
                var SaveFileExportExcel = new SaveFileDialog();
                if (SaveFileExportExcel.ShowDialog() == DialogResult.OK)
                {
                    savePath = SaveFileExportExcel.FileName;
                }
                else
                {
                    return false;
                }

                WaitingManager.Show();
                if (!string.IsNullOrWhiteSpace(savePath))
                {
                    ThreadGetDataService();

                    var listPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                    MPS.Processor.Mps000258.PDO.Mps000258PDO mps000258PDO = new MPS.Processor.Mps000258.PDO.Mps000258PDO(listVServicePatyExport, listVServiceExport, listPatientType);

                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000258.PDO.Mps000258PDO.printTypeCode, fileName, mps000258PDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, savePath + ".xlsx");

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printCode, moduleData != null ? moduleData.RoomId : 0);

                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                    WaitingManager.Hide();
                    if (result)
                        MessageManager.Show(Resources.ResourceMessage.HisServicePaty__ExportExcel__ThanhCong);
                    else
                        MessageManager.Show(Resources.ResourceMessage.HisServicePaty__ExportExcel__ThatBai);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void ThreadGetDataService()
        {
            Thread service = new Thread(GetServiceDVKT);
            Thread servicePaty = new Thread(GetServicePatyDVKT);
            try
            {
                service.Start();
                servicePaty.Start();

                service.Join();
                servicePaty.Join();
            }
            catch (Exception ex)
            {
                service.Abort();
                servicePaty.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceDVKT()
        {
            try
            {
                var param = new CommonParam();
                HisServiceViewFilter filter = new HisServiceViewFilter();

                if (serviceTypeSelecteds != null && serviceTypeSelecteds.Count > 0)
                {
                    filter.SERVICE_TYPE_IDs = serviceTypeSelecteds.Select(o => o.ID).ToList();
                }
                if (cboService.EditValue != null)
                {
                    filter.ID = (long)cboService.EditValue;
                }
                filter.IS_ACTIVE = 1;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var apiResult = new BackendAdapter(param).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    listVServiceExport = apiResult.Where(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServicePatyDVKT()
        {
            try
            {
                List<long> glstServiceID = new List<long>();
                var param = new CommonParam();
                HisServicePatyViewFilter filter = new HisServicePatyViewFilter();

                filter.IS_ACTIVE = 1;

                SetFilter(ref filter);
                listVServicePatyExport = new BackendAdapter(param).Get<List<V_HIS_SERVICE_PATY>>("api/HisServicePaty/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.SERVICE_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {

                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboService.EditValue = null;
                    txtService.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spInstrNumByTypeFrom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spInstrNumByTypeTo.Focus();
                    spInstrNumByTypeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spInstrNumByTypeTo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtFromTime.Focus();
                    dtFromTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceTypeName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceTypeName.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceTypeName.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                {
                    spInstrNumByTypeFrom.Enabled = true;
                    spInstrNumByTypeTo.Enabled = true;
                }
                else
                {
                    spInstrNumByTypeFrom.Enabled = false;
                    spInstrNumByTypeTo.Enabled = false;
                }
                if (cboServiceTypeName.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceTypeName.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN)
                {
                    cboRationTime.Enabled = true;
                }
                else
                {
                    cboRationTime.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboPatientTypeFilter_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboPatientTypeFilter_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                    if (gridCheckMark == null) return;
                    gridCheckMark.ClearSelection(CboPatientTypeFilter.Properties.View);
                    cboServiceType.Focus();
                    //cboServiceType.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientTypeFilter_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    cboPackageService.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboRationTime(GridLookUpEdit cbo, object data)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "RATION_TIME_NAME";
                cbo.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField("RATION_TIME_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = " ";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cbo.Properties.View.Columns.AddField("RATION_TIME_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__PatientTypeName(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> sgSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.patientTypeNameSelecteds = new List<HIS_PATIENT_TYPE>();
                    this.patientTypeNameSelecteds.AddRange(sgSelectedNews);
                }

                this.cboPatientTypeName.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientTypeName_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBranchName_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_BRANCH rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    if (true)
                    {

                    }
                    sb.Append(rv.BRANCH_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__BranchName(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_BRANCH> sgSelectedNews = new List<HIS_BRANCH>();
                    foreach (MOS.EFMODEL.DataModels.HIS_BRANCH rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.BRANCH_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.branchNameSelecteds = new List<HIS_BRANCH>();
                    this.branchNameSelecteds.AddRange(sgSelectedNews);
                }

                this.cboBranchName.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__RationTime(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_RATION_TIME> sgSelectedNews = new List<HIS_RATION_TIME>();
                    foreach (MOS.EFMODEL.DataModels.HIS_RATION_TIME rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.RATION_TIME_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.rationTimeNameSelecteds = new List<HIS_RATION_TIME>();
                    this.rationTimeNameSelecteds.AddRange(sgSelectedNews);
                }

                this.cboRationTime.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tao list chinh sach gia doi tuong
        private void SaveListProcess(ref List<HIS_SERVICE_PATY> listServicePaty)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var resultData = new BackendAdapter(param).Post<List<HIS_SERVICE_PATY>>(
               "api/HisServicePaty/CreateList", ApiConsumers.MosConsumer, listServicePaty, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                    ResetFormData();
                    listServicePaty = null;
                }
                MessageManager.Show(this, param, success);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinVatRatio_TextChanged(object sender, EventArgs e)
        {
            if (spinVatRatio.Value > 100)
                spinVatRatio.Value = 100;
        }

        private void spinVatRatio_EditValueChanged(object sender, EventArgs e)
        {
            if (spinVatRatio.Value > 100)
                spinVatRatio.Value = 100;
        }

        private void ChkInActive_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFind.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboPatientTypeFilter_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPackageService.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPackageService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPackageService.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                    if (gridCheckMark == null) return;
                    gridCheckMark.ClearSelection(cboPackageService.Properties.View);
                    cboPackageService.Focus();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPackageService_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPackageService.EditValue != null && cboPackageService.EditValue != cboPackageService.OldEditValue && cboPackageService.EditValue.ToString() != "")
                    {
                        MOS.EFMODEL.DataModels.HIS_PACKAGE gt = this.packageService.FirstOrDefault(o => o.ID == (long)cboPackageService.EditValue);
                        if (gt != null)
                        {
                            cboPackageService.EditValue = gt.ID;
                            ChkInActive.Focus();
                        }
                    }
                    else
                    {
                        ChkInActive.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPackageService_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ChkInActive.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPackageService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboPackageService.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPackageService1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void cboPackageService1_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPackageService1.EditValue != null && cboPackageService1.EditValue.ToString() != "" && cboPackageService1.Text != "")
                    {
                        MOS.EFMODEL.DataModels.HIS_PACKAGE gt = this.packageService.FirstOrDefault(o => o.ID == (long)cboPackageService1.EditValue);
                        if (gt != null)
                        {
                            cboPackageService1.EditValue = gt.ID;
                            if (cboServiceCondition.Enabled)
                            {
                                cboServiceCondition.Focus();
                                cboServiceCondition.SelectAll();
                            }
                            else
                            {
                                spinIntructionFrom.Focus();
                            }
                        }
                    }
                    else
                    {
                        if (cboServiceCondition.Enabled)
                        {
                            cboServiceCondition.Focus();
                            cboServiceCondition.SelectAll();
                        }
                        else
                        {
                            spinIntructionFrom.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPackageService1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (cboPackageService1.EditValue != null && cboPackageService1.EditValue.ToString() != "" && cboPackageService1.Text != "")
                //{
                //    cboPackageService1.ShowPopup();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPackageService1_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPackageService1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceCondition_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {

                    spinIntructionFrom.Focus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceCondition_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinIntructionFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceCondition_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceCondition.EditValue = null;
                    //cboServiceCondition.Focus();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPackageService1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboServiceCondition.Enabled)
                    {
                        cboServiceCondition.Focus();
                        cboServiceCondition.SelectAll();
                    }
                    else
                    {
                        spinIntructionFrom.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinActualPrice_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinPriority.Focus();
                    spinPriority.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinActualPrice_Leave(object sender, EventArgs e)
        {
            try
            {
                if (spinActualPrice.Value <= 0)
                {
                    spinActualPrice.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinActualPrice_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinActualPrice.Value == 0)
                {
                    spinActualPrice.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientClassify_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPatientClassify.Text.Trim() == "")
                {
                    cboPatientClassify.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboRationTime_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                if (this.rationTimeNameSelecteds != null)
                {
                    foreach (MOS.EFMODEL.DataModels.HIS_RATION_TIME rv in gridCheckMark.Selection)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.RATION_TIME_NAME.ToString());
                    }
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboRationTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRationTime.EditValue != null && cboRationTime.EditValue != cboRationTime.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_RATION_TIME gt = this.rationTimes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRationTime.EditValue.ToString()));
                        if (gt != null)
                        {
                            //txtBranchName.Text = gt.BRANCH_CODE;
                            cboServiceCondition.Focus();
                        }
                    }
                    else
                    {
                        cboRationTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRationTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPackageService1.Focus();
                    cboPackageService1.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRationTime_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //if (cboRationTime.Text.Trim() == "")
                //{
                //    cboRationTime.EditValue = null;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboRationTime_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();
                    cboRationTime.EditValue = null;
                    rationTimeNameSelecteds = null;
                    cboRationTime.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinPayoutRatio_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(spinPayoutRatio.Text) && spinPayoutRatio.Value < 0)
            {
                e.Cancel = true;
                MessageBox.Show("Tỉ lệ thanh toán(%) không hợp lệ, tỉ lệ thanh toán phải là số dương!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void spinPayoutRatio_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(spinPayoutRatio.Text))
            {
                e.ErrorText = "Tỉ lệ thanh toán(%) không hợp lệ, tỉ lệ thanh toán phải là số dương!";
            }
            else
            {
                e.ErrorText = string.Empty;
            }
        }
    }
}

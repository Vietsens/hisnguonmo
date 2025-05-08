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
using DevExpress.XtraEditors;
using HTC.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HTC.Filter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Controls.Session;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using DevExpress.Utils;
using Inventec.Common.TypeConvert;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Plugins.TreatmentList.ADO;
using HIS.Desktop.Plugins.TreatmentList.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using MOS.SDO;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LocalStorage.Location;
using System.IO;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using DevExpress.Utils.Menu;
using His.Bhyt.ExportXml.Base;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.TreatmentList.Base;
using ACS.Filter;
using DevExpress.XtraGrid.Views.Grid;
using HIS.UC.UCCauseOfDeath.ADO;


namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class UCTreatmentList : UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        PopupMenuProcessor popupMenuProcessor = null;
        MediRecordMenuPopupProcessor emrMenuPopupProcessor = null;
        List<V_HIS_TREATMENT_4> ListHisTreatment = new List<V_HIS_TREATMENT_4>();
        List<HIS_ICD> ListICD = new List<HIS_ICD>();
        V_HIS_TREATMENT_4 currentTreatment = null;
        internal Inventec.Desktop.Common.Modules.Module module;
        V_HIS_TREATMENT_4 currentTreatmentPrint = null;
        string loginName = null;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        ACS_CONTROL controlDelete;
        List<ACS.SDO.AcsRoleUserSDO> RoleUse;
        List<ACS_CONTROL_ROLE> ControlRule;

        List<HIS_TREATMENT_TYPE> _DienDieuTriSelecteds;
        List<V_HIS_ROOM> _EndRoomSelecteds;
        List<HIS_DEPARTMENT> _EndDepartmentSelecteds;
        List<HIS_DEPARTMENT> DepartmentSelecteds;
        List<KieuBenhNhanADO> _KieuBenhNhanSelecteds;
        List<TrangThaiADO> _TrangThaiSelecteds;

        List<HIS_TREATMENT_TYPE> listTreatmentType;
        List<V_HIS_ROOM> listRoom;
        List<HIS_DEPARTMENT> listDepartment;
        List<KieuBenhNhanADO> listKieuBenhNhan;
        List<TrangThaiADO> listTrangThai;
        List<V_HIS_KSK_CONTRACT> listKskContract;
        List<HIS_PATIENT_TYPE> patientTypeSelecteds;
        List<HIS_PATIENT_TYPE> listPatientType;
        CauseOfDeathADO causeResult { get; set; }

        bool isDelete = false;
        bool isLoadForm = false;

        internal string typeCodeFind__KeyWork_InDate = Resources.ResourceMessage.typeCodeFind__KeyWork_InDate;//Set lại giá trị trong resource
        internal string typeCodeFind_InDate = Resources.ResourceMessage.typeCodeFind_InDate;//Set lại giá trị trong resource
        internal string typeCodeFind__InMonth = Resources.ResourceMessage.typeCodeFind__InMonth;//Set lại giá trị trong resource
        internal string typeCodeFind__InYear = Resources.ResourceMessage.typeCodeFind__InYear;//Set lại giá trị trong resource
        internal string typeCodeFind__InTime = Resources.ResourceMessage.typeCodeFind__InTime;//Set lại giá trị trong resource

        internal string typeCodeFind__KeyWork_OutDate = Resources.ResourceMessage.typeCodeFind__KeyWork_OutDate;//Set lại giá trị trong resource
        internal string typeCodeFind_OutDate = Resources.ResourceMessage.typeCodeFind_OutDate;//Set lại giá trị trong resource
        internal string typeCodeFind__OutMonth = Resources.ResourceMessage.typeCodeFind__OutMonth;//Set lại giá trị trong resource
        internal string typeCodeFind__OutYear = Resources.ResourceMessage.typeCodeFind__OutYear;//Set lại giá trị trong resource
        internal string typeCodeFind__OutTime = Resources.ResourceMessage.typeCodeFind__OutTime;//Set lại giá trị trong resource



        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.TreatmentList";
        DevExpress.XtraBars.BarManager barManager = new DevExpress.XtraBars.BarManager();
        PopupMenu menu;
        private enum PrintType
        {
            InPhieuKSKDinhKy,
            InSoKSKDinhKy,
            XuatExcelKQKSK,
            InPhieuKQCLSKSK
        }

        string fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", "HIS.Desktop.Plugins.TreatmentList.gridViewtreatmentList.xml"));

        List<V_HIS_TREATMENT_3> hisTreatments = new List<V_HIS_TREATMENT_3>();
        List<ADO.TempExcelDataADO> ListTemp;
        List<ADO.TempExcelDataADO> ListTempXN;
        List<string> lstHeaderColumns;
        List<string> lstHeaderColumnsXN;
        HIS_SERVICE_REQ serviceReqExamEndType;
        public UCTreatmentList(Inventec.Desktop.Common.Modules.Module module, string treatmentCode)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.currentModule = module;
                txtTreatment.Text = treatmentCode;
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCTreatmentList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            currentModule = module;
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCTreatmentList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                isLoadForm = true;
                SetCaptionByLanguageKey();
                InitTypeFind();
                InitComboOutHopital();
                GetDataCombo();
                //InitComboEndDepartment();
                InitComboContract();
                InitCheck(cboDienDieuTri, SelectionGrid__DienDieuTri);
                InitCombo(cboDienDieuTri, listTreatmentType, "TREATMENT_TYPE_NAME", "ID");
                InitCheck(cboEndRoom, SelectionGrid__PhongKetThuc);
                InitCombo(cboEndRoom, listRoom, "ROOM_NAME", "ID");
                InitCheck(cboEndDepartment, SelectionGrid__EndDepartment);
                InitCombo(cboEndDepartment, listDepartment, "DEPARTMENT_NAME", "ID");
                InitCheck(cboKieuBenhNhan, SelectionGrid__KieuBenhNhan);
                InitCombo(cboKieuBenhNhan, listKieuBenhNhan, "KieuBenhNhan", "ID");
                InitCheck(cboTrangThai, SelectionGrid__TrangThai);
                InitCombo(cboTrangThai, listTrangThai, "TrangThai", "ID");
                InitComboPatientTypeCheck();
                InitComboPatientType();
                LoadAcsControls();
                SetDefaultControl();
                FillDataToGrid();
                InitCboKhoaVaoVien();
                InitCboTreatmentEndTypeExt();
                InitControlState();
                isLoadForm = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServiceReq(long id)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.IS_ACTIVE = 1;
                filter.TREATMENT_ID = id;
                var dt = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                if (dt != null && dt.Count > 0)
                    serviceReqExamEndType = dt.FirstOrDefault(o => o.EXAM_END_TYPE == 3);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void LoadCombo()
        //{
        //    try
        //    {
        //        var data = new List<object>();
        //        data.Add(new { ID = 1, Value = Resources.ResourceMessage.InDay });
        //        data.Add(new { ID = 2, Value = Resources.ResourceMessage.InMonth });
        //        //data.Add(new { ID = 1, Value = "Trong ngày" });
        //        //data.Add(new { ID = 2, Value = "Trong tháng" });
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("Value", "", 150, 1));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("Value", "ID", columnInfos, false, 150);
        //        ControlEditorLoader.Load(cboInHospital, data, controlEditorADO);
        //        ControlEditorLoader.Load(cboOutHospital, data, controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {


                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDataCombo()
        {
            try
            {
                listTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                listRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                listTrangThai = new List<TrangThaiADO>();
                listKieuBenhNhan = new List<KieuBenhNhanADO>();

                listKieuBenhNhan.Add(new KieuBenhNhanADO(1, "Bệnh nhân mãn tính"));
                listKieuBenhNhan.Add(new KieuBenhNhanADO(2, "Bệnh nhân thường"));

                listTrangThai.Add(new TrangThaiADO(1, "Đang điều trị"));
                listTrangThai.Add(new TrangThaiADO(2, "Đã kết thúc điều trị"));
                listTrangThai.Add(new TrangThaiADO(3, "Đã duyệt khóa tài chính"));
                listTrangThai.Add(new TrangThaiADO(4, "Đã duyệt khóa bảo hiểm"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SelectionGrid__DienDieuTri(object sender, EventArgs e)
        {
            try
            {
                _DienDieuTriSelecteds = new List<HIS_TREATMENT_TYPE>();
                foreach (HIS_TREATMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _DienDieuTriSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__PhongKetThuc(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<V_HIS_ROOM> sgSelectedNews = new List<V_HIS_ROOM>();
                    foreach (V_HIS_ROOM rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.ROOM_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this._EndRoomSelecteds = new List<V_HIS_ROOM>();
                    this._EndRoomSelecteds.AddRange(sgSelectedNews);
                }
                this.cboEndRoom.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__EndDepartment(object sender, EventArgs e)
        {
            try
            {
                _EndDepartmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _EndDepartmentSelecteds.Add(rv);
                }
                if (!isLoadForm)
                {
                    if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                    {
                        cboEndRoom.Properties.DataSource = listRoom.Where(o => _EndDepartmentSelecteds.Select(p => p.DEPARTMENT_CODE).ToList().Contains(o.DEPARTMENT_CODE)).ToList();

                        GridCheckMarksSelection gridCheckMark = cboEndRoom.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.ClearSelection(cboEndRoom.Properties.View);
                        }
                        _EndRoomSelecteds = new List<V_HIS_ROOM>();
                    }
                    else
                    {
                        cboEndRoom.Properties.DataSource = listRoom;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__TrangThai(object sender, EventArgs e)
        {
            try
            {
                _TrangThaiSelecteds = new List<TrangThaiADO>();
                foreach (TrangThaiADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _TrangThaiSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__KieuBenhNhan(object sender, EventArgs e)
        {
            try
            {
                _KieuBenhNhanSelecteds = new List<KieuBenhNhanADO>();
                foreach (KieuBenhNhanADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _KieuBenhNhanSelecteds.Add(rv);
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
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboContract()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisKskContractViewFilter filter = new HisKskContractViewFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "KSK_CONTRACT_CODE";
                listKskContract = new BackendAdapter(param).Get<List<V_HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", "Mã hợp đồng", 100, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "Tên công ty", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboContract, listKskContract, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCboKhoaVaoVien()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = 1;

                DepartmentSelecteds = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "Mã khoa", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên khoa", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboKhoaVaoVien, DepartmentSelecteds, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadAcsControls()
        {
            try
            {
                this.controlAcs = new List<ACS.EFMODEL.DataModels.ACS_CONTROL>();

                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    this.controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }

                var apiResult = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == this.loginName);
                if (apiResult != null)
                {
                    CommonParam param = new CommonParam();
                    AcsRoleUserFilter roleUserFilter = new AcsRoleUserFilter();
                    roleUserFilter.USER_ID = apiResult.ID;
                    this.RoleUse = new BackendAdapter(param).Get<List<ACS.SDO.AcsRoleUserSDO>>
                      ("api/AcsRoleUser/Get", ApiConsumers.AcsConsumer, roleUserFilter, param);
                }

                CommonParam paramControlRole = new CommonParam();
                AcsControlRoleFilter filter = new AcsControlRoleFilter();
                var controlRule = new BackendAdapter(paramControlRole).Get<List<ACS_CONTROL_ROLE>>
                       ("api/AcsControlRole/Get", ApiConsumers.AcsConsumer, filter, paramControlRole);
                CommonParam parmaControl = new CommonParam();
                this.controlDelete = BackendDataWorker.Get<ACS_CONTROL>().FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnDelete);
                if (this.controlDelete != null && controlRule != null && controlRule.Count > 0)
                {
                    this.ControlRule = controlRule.Where(o => o.CONTROL_ID == this.controlDelete.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitCboTreatmentEndTypeExt()
        {
            try
            {
                var treatmentEndTypeExts = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE_EXT>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_END_TYPE_EXT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TREATMENT_END_TYPE_EXT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_END_TYPE_EXT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboTreatmentEndTypeExt, treatmentEndTypeExts, controlEditorADO);
                cboTreatmentEndTypeExt.Properties.PopupFormSize = new Size(0, 200);
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

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(/*cbo.Properties.DataSource*/null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultControl()
        {
            try
            {
                ResetCombo(cboDienDieuTri);
                ResetCombo(cboEndDepartment);
                ResetCombo(cboKieuBenhNhan);
                ResetCombo(cboTrangThai);
                ResetCombo(cboEndRoom);
                ResetComboPatientType(cboPatientType);
                cboDienDieuTri.Enabled = false;
                cboDienDieuTri.Enabled = true;
                cboEndDepartment.Enabled = false;
                cboEndDepartment.Enabled = true;
                cboKieuBenhNhan.Enabled = false;
                cboKieuBenhNhan.Enabled = true;
                cboTrangThai.Enabled = false;
                cboTrangThai.Enabled = true;
                cboContract.Enabled = false;
                cboContract.Enabled = true;
                cboPatientType.Enabled = false;
                cboPatientType.Enabled = true;

                InDepartment.Checked = true;
                chkIsRejectStore.Checked = false;

                btnCodeFind.Text = typeCodeFind_InDate;
                cboOutOfHospital.Text = typeCodeFind_OutDate;
                this.typeCodeFind_InDate = "Trong ngày";
                this.typeCodeFind__KeyWork_InDate = this.typeCodeFind_InDate;
                FormatDtIntructionDate();
                FormatDtIntructionOutDate();
                //cboInHospital.EditValue = 1;
                //cboOutHospital.EditValue = null;
                dtInHospital.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                dtOutHospital.Text = null;
                btnPreDayOutHospital.Enabled = false;
                btnNextDayOutHospital.Enabled = false;
                txtKeyword.Text = "";
                cboContract.EditValue = null;
                cboEndDepartment.EditValue = null;
                btnPrintServiceReq.Enabled = false;
                btnXuatXML.Enabled = false;
                btnPrintfKSK.Enabled = false;
                btnRecordChecking.Enabled = false;
                btnDelete.Enabled = false;
                txtInCode.Text = "";
                txtOutCode.Text = "";
                txtStoreCode.Text = "";
                txtPatientName.Text = "";
                txtPatient.Text = "";
                txtSocialInsuranceNumber.Text = "";
                layoutInDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutOutDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (gridViewtreatmentList.GetSelectedRows().Count() > 0 && !string.IsNullOrEmpty(Config.HisConfigCFG.SYNC_XML_FPT_OPTION))
                {
                    btnGuiHS.Enabled = true;
                }
                else
                {
                    btnGuiHS.Enabled = false;
                }

                if (HisConfigCFG.SearchPatientsAcrossHospital)
                {
                    if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.RadEntireHospital) != null)
                    {
                        layoutRadEntireHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        radioButton1.Checked = true;
                    }
                    else
                    {
                        layoutRadEntireHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                }
                else
                {
                    layoutRadEntireHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    radioButton1.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FomatControlByTimeOut()
        {
            try
            {
                ResetCombo(cboDienDieuTri);
                ResetCombo(cboEndDepartment);
                ResetCombo(cboKieuBenhNhan);
                ResetCombo(cboTrangThai);
                ResetComboPatientType(cboPatientType);
                cboDienDieuTri.Enabled = false;
                cboDienDieuTri.Enabled = true;
                cboEndDepartment.Enabled = false;
                cboEndDepartment.Enabled = true;
                cboKieuBenhNhan.Enabled = false;
                cboKieuBenhNhan.Enabled = true;
                cboTrangThai.Enabled = false;
                cboTrangThai.Enabled = true;
                cboContract.Enabled = false;
                cboContract.Enabled = true;
                cboPatientType.Enabled = false;
                cboPatientType.Enabled = true;

                InDepartment.Checked = true;
                chkIsRejectStore.Checked = false;
                dtInHospital.Text = null;
                dtOutHospital.Text = null;
                btnPreDayOutHospital.Enabled = false;
                btnNextDayOutHospital.Enabled = false;
                txtKeyword.Text = "";
                cboContract.EditValue = null;
                cboEndDepartment.EditValue = null;
                btnPrintServiceReq.Enabled = false;
                btnXuatXML.Enabled = false;
                btnPrintfKSK.Enabled = false;
                btnRecordChecking.Enabled = false;
                txtInCode.Text = "";
                txtStoreCode.Text = "";
                txtPatientName.Text = "";
                txtPatient.Text = "";
                txtSocialInsuranceNumber.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FomatControlByTimeIn()
        {
            try
            {
                ResetCombo(cboDienDieuTri);
                ResetCombo(cboEndDepartment);
                ResetCombo(cboKieuBenhNhan);
                ResetCombo(cboTrangThai);
                ResetComboPatientType(cboPatientType);
                cboDienDieuTri.Enabled = false;
                cboDienDieuTri.Enabled = true;
                cboEndDepartment.Enabled = false;
                cboEndDepartment.Enabled = true;
                cboKieuBenhNhan.Enabled = false;
                cboKieuBenhNhan.Enabled = true;
                cboTrangThai.Enabled = false;
                cboTrangThai.Enabled = true;
                cboContract.Enabled = false;
                cboContract.Enabled = true;
                cboPatientType.Enabled = false;
                cboPatientType.Enabled = true;

                InDepartment.Checked = true;
                chkIsRejectStore.Checked = false;
                dtInHospital.Text = null;
                dtOutHospital.Text = null;
                btnPreDayOutHospital.Enabled = false;
                btnNextDayOutHospital.Enabled = false;
                txtKeyword.Text = "";
                cboContract.EditValue = null;
                cboEndDepartment.EditValue = null;
                btnPrintServiceReq.Enabled = false;
                btnXuatXML.Enabled = false;
                btnPrintfKSK.Enabled = false;
                btnRecordChecking.Enabled = false;
                txtStoreCode.Text = "";
                txtPatientName.Text = "";
                txtPatient.Text = "";
                txtSocialInsuranceNumber.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FomatControlByTimeStoreCode()
        {
            try
            {
                ResetCombo(cboDienDieuTri);
                ResetCombo(cboEndDepartment);
                ResetCombo(cboKieuBenhNhan);
                ResetCombo(cboTrangThai);
                ResetComboPatientType(cboPatientType);
                cboDienDieuTri.Enabled = false;
                cboDienDieuTri.Enabled = true;
                cboEndDepartment.Enabled = false;
                cboEndDepartment.Enabled = true;
                cboKieuBenhNhan.Enabled = false;
                cboKieuBenhNhan.Enabled = true;
                cboTrangThai.Enabled = false;
                cboTrangThai.Enabled = true;
                cboContract.Enabled = false;
                cboContract.Enabled = true;
                cboPatientType.Enabled = false;
                cboPatientType.Enabled = true;

                InDepartment.Checked = true;
                chkIsRejectStore.Checked = false;
                dtInHospital.Text = null;
                dtOutHospital.Text = null;
                btnPreDayOutHospital.Enabled = false;
                btnNextDayOutHospital.Enabled = false;
                txtKeyword.Text = "";
                cboContract.EditValue = null;
                cboEndDepartment.EditValue = null;
                btnPrintServiceReq.Enabled = false;
                btnXuatXML.Enabled = false;
                btnPrintfKSK.Enabled = false;
                btnRecordChecking.Enabled = false;
                txtPatientName.Text = "";
                txtPatient.Text = "";
                txtSocialInsuranceNumber.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FomatControlByTimePatientCode()
        {
            try
            {
                ResetCombo(cboDienDieuTri);
                ResetCombo(cboEndDepartment);
                ResetCombo(cboKieuBenhNhan);
                ResetCombo(cboTrangThai);
                ResetComboPatientType(cboPatientType);
                cboDienDieuTri.Enabled = false;
                cboDienDieuTri.Enabled = true;
                cboEndDepartment.Enabled = false;
                cboEndDepartment.Enabled = true;
                cboKieuBenhNhan.Enabled = false;
                cboKieuBenhNhan.Enabled = true;
                cboTrangThai.Enabled = false;
                cboTrangThai.Enabled = true;
                cboContract.Enabled = false;
                cboContract.Enabled = true;
                cboPatientType.Enabled = false;
                cboPatientType.Enabled = true;

                InDepartment.Checked = true;
                chkIsRejectStore.Checked = false;
                dtInHospital.Text = null;
                dtOutHospital.Text = null;
                btnPreDayOutHospital.Enabled = false;
                btnNextDayOutHospital.Enabled = false;
                txtKeyword.Text = "";
                cboContract.EditValue = null;
                cboEndDepartment.EditValue = null;
                btnPrintServiceReq.Enabled = false;
                btnXuatXML.Enabled = false;
                btnPrintfKSK.Enabled = false;
                btnRecordChecking.Enabled = false;
                txtPatientName.Text = "";
                txtSocialInsuranceNumber.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FomatControlBySocialInsuranceNumber()
        {
            try
            {
                ResetCombo(cboDienDieuTri);
                ResetCombo(cboEndDepartment);
                ResetCombo(cboKieuBenhNhan);
                ResetCombo(cboTrangThai);
                ResetComboPatientType(cboPatientType);
                cboDienDieuTri.Enabled = false;
                cboDienDieuTri.Enabled = true;
                cboEndDepartment.Enabled = false;
                cboEndDepartment.Enabled = true;
                cboKieuBenhNhan.Enabled = false;
                cboKieuBenhNhan.Enabled = true;
                cboTrangThai.Enabled = false;
                cboTrangThai.Enabled = true;
                cboContract.Enabled = false;
                cboContract.Enabled = true;
                cboPatientType.Enabled = false;
                cboPatientType.Enabled = true;

                InDepartment.Checked = true;
                chkIsRejectStore.Checked = false;
                dtInHospital.Text = null;
                dtOutHospital.Text = null;
                btnPreDayOutHospital.Enabled = false;
                btnNextDayOutHospital.Enabled = false;
                txtKeyword.Text = "";
                cboContract.EditValue = null;
                cboEndDepartment.EditValue = null;
                btnPrintServiceReq.Enabled = false;
                btnXuatXML.Enabled = false;
                btnPrintfKSK.Enabled = false;
                btnRecordChecking.Enabled = false;
                txtPatientName.Text = "";
                txtPatient.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                int pageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                //gridControlTreatmentList.DataSource = null;
                FillDataToGridTreatment(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatment, param, pageSize, gridControlTreatmentList);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGridTreatment(object param)
        {
            try
            {
                start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisTreatmentView4Filter filter = new HisTreatmentView4Filter();


                if (!string.IsNullOrEmpty(txtTreatment.Text))
                {
                    string code = txtTreatment.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatment.Text = code;
                    }
                    filter = new HisTreatmentView4Filter();
                    filter.TREATMENT_CODE__EXACT = code;
                    SetDefaultControl();
                    dtInHospital.Text = null;
                }

                else if (!string.IsNullOrEmpty(txtOutCode.Text))
                {
                    filter.END_CODE__EXACT = txtOutCode.Text.Trim();
                    FomatControlByTimeOut();
                }
                else if (!string.IsNullOrEmpty(txtInCode.Text))
                {
                    filter.IN_CODE__EXACT = txtInCode.Text.Trim();
                    FomatControlByTimeIn();
                }
                else if (!string.IsNullOrEmpty(txtStoreCode.Text))
                {
                    filter.STORE_CODE__EXACT = txtStoreCode.Text.Trim();
                    FomatControlByTimeStoreCode();
                }
                else if (!string.IsNullOrEmpty(txtPatient.Text))
                {
                    string code = txtPatient.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatient.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                    FomatControlByTimePatientCode();
                }
                else if (!string.IsNullOrEmpty(txtSocialInsuranceNumber.Text))
                {
                    filter.TDL_SOCIAL_INSURANCE_NUMBER__EXACT = txtSocialInsuranceNumber.Text.Trim();
                    FomatControlBySocialInsuranceNumber();
                }
                else
                {
                    if (this.InDepartment.Checked)
                    {

                        filter.WAS_BEEN_DEPARTMENT_ID = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.currentModule.RoomId).First().DEPARTMENT_ID;
                    }

                    if (this.chkIsRejectStore.Checked)
                    {
                        filter.APPROVAL_STORE_STT_ID = 2;
                    }
                    else
                    {
                        filter.APPROVAL_STORE_STT_ID = null;
                    }

                    if (this.radioButtonAtDepartment.Checked)
                    {
                        filter.LAST_DEPARTMENT_ID = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.currentModule.RoomId).First().DEPARTMENT_ID;
                        filter.IS_PAUSE = false;
                    }
                    if (this._DienDieuTriSelecteds != null && this._DienDieuTriSelecteds.Count > 0)
                    {
                        filter.TDL_TREATMENT_TYPE_IDs = this._DienDieuTriSelecteds.Select(o => o.ID).ToList();
                    }
                    if (this._EndRoomSelecteds != null && this._EndRoomSelecteds.Count > 0)
                    {
                        filter.END_ROOM_IDs = this._EndRoomSelecteds.Select(o => o.ID).ToList();
                        filter.IS_PAUSE = true;
                    }
                    if (this._KieuBenhNhanSelecteds != null && this._KieuBenhNhanSelecteds.Count > 0)
                    {
                        if (this._KieuBenhNhanSelecteds.Exists(o => o.ID == 1))
                        {
                            filter.IS_CHRONIC = true;
                        }
                        if (this._KieuBenhNhanSelecteds.Exists(o => o.ID == 2))
                        {
                            filter.IS_CHRONIC = false;
                        }
                        if (this._KieuBenhNhanSelecteds.Exists(o => o.ID == 1) && this._KieuBenhNhanSelecteds.Exists(o => o.ID == 2))
                        {
                            filter.IS_CHRONIC = null;
                        }
                    }

                    if (this._TrangThaiSelecteds != null && this._TrangThaiSelecteds.Count > 0)
                    {
                        if (this._TrangThaiSelecteds.Exists(o => o.ID == 3))
                        {
                            filter.FEE_LOCK_TIME_FROM = 1;
                            filter.FEE_LOCK_TIME_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "235959");
                        }
                        if (this._TrangThaiSelecteds.Exists(o => o.ID == 4))
                        {
                            filter.IS_LOCK_HEIN = true;
                        }

                        if (this._TrangThaiSelecteds.Exists(o => o.ID == 1) && !this._TrangThaiSelecteds.Exists(o => o.ID == 2)) filter.IS_PAUSE = false;
                        if (!this._TrangThaiSelecteds.Exists(o => o.ID == 1) && this._TrangThaiSelecteds.Exists(o => o.ID == 2)) filter.IS_PAUSE = true;
                    }
                    if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                    {
                        var vHisRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                        if (vHisRooms != null)
                        {

                            filter.END_ROOM_IDs = vHisRooms.Where(o => _EndDepartmentSelecteds.Exists(p => p.ID == o.DEPARTMENT_ID)).Select(p => p.ID).ToList();
                        }
                        //else
                        //{
                        //    Inventec.Common.Logging.LogSystem.Info("danh sach phong");
                        //}
                    }

                    if (cboContract.EditValue != null)
                    {
                        filter.TDL_KSK_CONTRACT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString());
                    }
                    if (cboKhoaVaoVien.EditValue != null)
                    {
                        filter.HOSPITALIZE_DEPARTMENT_ID = (long)cboKhoaVaoVien.EditValue;
                    }
                    if (cboTreatmentEndTypeExt.EditValue != null)
                    {
                        filter.TREATMENT_END_TYPE_EXT_ID = (long)cboTreatmentEndTypeExt.EditValue;
                    }
                    filter.PATIENT_NAME = txtPatientName.Text.Trim();

                    filter.KEY_WORD = txtKeyword.Text != "" ? txtKeyword.Text : null;


                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";

                    if ((dtInHospital.EditValue == null || dtInHospital.EditValue.ToString() == "") && (dtOutHospital.EditValue == null || dtOutHospital.EditValue.ToString() == ""))
                    {
                        WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.TranhCaoTai, Resources.ResourceMessage.Thongbao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                            return;
                    }
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate
                        && dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue)
                    {
                        filter.IN_DATE_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtInHospital.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InMonth
                        && dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue)
                    {
                        filter.IN_MONTH_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtInHospital.EditValue).ToString("yyyyMM") + "00000000");
                    }
                    else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InYear
                        && dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue)
                    {
                        filter.IN_YEAR_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtInHospital.EditValue).ToString("yyyy") + "0000000000");
                    }
                    else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InTime
                        && dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue
                        && dtInDateCome.EditValue != null && dtInDateCome.DateTime != DateTime.MinValue)
                    {
                        filter.IN_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtInHospital.EditValue).ToString("yyyyMMddHHmm") + "00");
                        filter.IN_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtInDateCome.EditValue).ToString("yyyyMMddHHmm") + "59");
                    }


                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate
                        && dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_DATE_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtOutHospital.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    else if (this.typeCodeFind__KeyWork_OutDate == typeCodeFind__OutMonth
                        && dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_MONTH_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtOutHospital.EditValue).ToString("yyyyMM") + "00000000");
                    }
                    else if (this.typeCodeFind__KeyWork_OutDate == typeCodeFind__OutYear
                        && dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_YEAR_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtOutHospital.EditValue).ToString("yyyy") + "0000000000");
                    }
                    else if (this.typeCodeFind__KeyWork_OutDate == typeCodeFind__OutTime
                        && dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue
                        && dtOutDateCome.EditValue != null && dtOutDateCome.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtOutHospital.EditValue).ToString("yyyyMMddHHmm") + "00");
                        filter.OUT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtOutDateCome.EditValue).ToString("yyyyMMddHHmm") + "59");
                    }

                    // Loc theo doi tuong benh nhan
                    if (this.patientTypeSelecteds != null && this.patientTypeSelecteds.Count > 0)
                    {
                        filter.TDL_PATIENT_TYPE_IDs = patientTypeSelecteds.Select(o => o.ID).ToList();
                    }
                }
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_4>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW4, ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null)
                {
                    ListHisTreatment = (List<V_HIS_TREATMENT_4>)result.Data;
                    gridControlTreatmentList.BeginUpdate();
                    gridControlTreatmentList.DataSource = null;
                    gridControlTreatmentList.DataSource = ListHisTreatment;
                    rowCount = (ListHisTreatment == null ? 0 : ListHisTreatment.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    gridControlTreatmentList.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_HIS_TREATMENT_4)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + start;
                        }
                        else if (e.Column.FieldName == "ST_DEPLAY")
                        {
                            DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                            short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_PAUSE ?? -1).ToString());
                            decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((data.IS_ACTIVE ?? -1).ToString());
                            short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_LOCK_HEIN ?? -1).ToString());
                            //Status
                            //1- dang dieu tri
                            //2- da ket thuc
                            //3- khóa hồ sơ
                            //4- duyệt bhyt
                            if (status_islockhein != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    if (status_ispause != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                    {
                                        e.Value = imageList1.Images[0];
                                    }
                                    else
                                    {
                                        e.Value = imageList1.Images[1];
                                    }
                                }
                                else
                                {
                                    e.Value = imageList1.Images[2];
                                }
                            }
                            else
                            {
                                e.Value = imageList1.Images[3];
                            }
                        }
                        else if (e.Column.FieldName == "DOB_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                        }
                        else if (e.Column.FieldName == "IS_CHRONIC_STR")
                        {
                            e.Value = "";
                            if (data.IS_CHRONIC == 1)
                            {
                                e.Value = "Mãn tính";
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "CLINICAL_IN_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CLINICAL_IN_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "ICD_MAIN_TEXT_ST")
                        {
                            e.Value = data.ICD_NAME;
                        }
                        else if (e.Column.FieldName == "REJECT_STORE_REASON_OBJECT")
                        {
                            if (data.APPROVAL_STORE_STT_ID == (short)2)
                            {
                                e.Value = data.REJECT_STORE_REASON;
                            }
                        }
                        else if (e.Column.FieldName == "AUTO_DISCOUNT_RATIO_STR")
                        {
                            e.Value = data.AUTO_DISCOUNT_RATIO.HasValue ? Inventec.Common.Number.Convert.NumberToString(((data.AUTO_DISCOUNT_RATIO ?? 0) * 100), ConfigApplications.NumberSeperator) : "";
                        }
                        else if (e.Column.FieldName == "SENT_INTEGRATE_HIS")
                        {
                            if (data.IS_INTEGRATE_HIS_SENT == (short)1)
                            {
                                e.Value = Inventec.Common.Resource.Get.Value("UCTreatmentList.DaGui", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                            }
                            else
                            {
                                e.Value = Inventec.Common.Resource.Get.Value("UCTreatmentList.ChuaGui", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                            }
                        }

                        else if (e.Column.FieldName == "IN_DEPARTMENT_NAME")
                        {

                            //if (data.HOSPITALIZE_DEPARTMENT_ID.Value != null)
                            //{
                            //    var inDepartmentID = data.HOSPITALIZE_DEPARTMENT_ID.Value;
                            //    var departName2 = listDepartment.Where(o => o.ID == inDepartmentID).FirstOrDefault().DEPARTMENT_NAME;
                            //    e.Value = departName2;
                            //}
                            //else
                            // {
                            if (data.HOPITALIZE_DEPARTMENT_NAME != null)
                            {


                                e.Value = data.HOPITALIZE_DEPARTMENT_NAME;
                            }

                            //  }
                            // var inDepartmentID = data.HOSPITALIZE_DEPARTMENT_ID.Value;


                            //Cột "Khoa nhập viện" dựa vào trường HOSPITALIZE_DEPARTMENT_ID trong HIS_TREATMENT join vào bảng HIS_DEPARTMENT để lấy ra tên khoa(DEPARTMENT_NAME)

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewtreatmentList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short isPause = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewtreatmentList.GetRowCellValue(e.RowHandle, "IS_PAUSE") ?? "").ToString());
                    long treatmentEndTypeId = Int64.Parse((gridViewtreatmentList.GetRowCellValue(e.RowHandle, "TREATMENT_END_TYPE_ID") ?? "0").ToString());
                    short isAutoDiscount = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewtreatmentList.GetRowCellValue(e.RowHandle, "IS_AUTO_DISCOUNT") ?? "").ToString());
                    long TreatmentTypeId = Int64.Parse((gridViewtreatmentList.GetRowCellValue(e.RowHandle, "TDL_TREATMENT_TYPE_ID") ?? "0").ToString());
                    string TreatmentCode = (gridViewtreatmentList.GetRowCellValue(e.RowHandle, "TREATMENT_CODE") ?? "0").ToString();
                    long endRoomId = Int64.Parse((gridViewtreatmentList.GetRowCellValue(e.RowHandle, "END_ROOM_ID") ?? "0").ToString());
                    long endDepartmentId = Int64.Parse((gridViewtreatmentList.GetRowCellValue(e.RowHandle, "END_DEPARTMENT_ID") ?? "0").ToString());
                    string departmentIds = (gridViewtreatmentList.GetRowCellValue(e.RowHandle, "DEPARTMENT_IDS") ?? "").ToString();
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    bool isfinishButton = (isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds));
                    if (e.Column.FieldName == "ServiceReq")
                    {
                        e.RepositoryItem = (isfinishButton == true ? repositoryItembtnServiceReq : repositoryItembtnServiceReqU);
                    }
                    else if (e.Column.FieldName == "BedRoomIn")
                    {
                        e.RepositoryItem = (isfinishButton == true ? repositoryItembtnBedRoomIn : repositoryItembtnBedRoomInU);
                    }
                    else if (e.Column.FieldName == "Finish")
                    {
                        if (HisConfigCFG.IsUnlockConditionOption)
                        {
                            bool IsEnableFinish = (isPause != 1) && (TreatmentTypeId != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && IsStayingDepartment(departmentIds) || CheckLoginAdmin.IsAdmin(loginName));
                            bool IsEnableUnFinish = (isPause == 1) && ((TreatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && endRoomId == currentModule.RoomId) || (TreatmentTypeId != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && IsStayingDepartment(endDepartmentId.ToString())) || CheckLoginAdmin.IsAdmin(loginName));
                            e.RepositoryItem = IsEnableFinish ? repositoryItembtnFinish : (IsEnableUnFinish ? repositoryItembtnUnFinish : (isPause == 1 ? repositoryItembtnUnFinish_Disable : repositoryItembtnFinish_Disable));
                        }
                        else
                        {
                            e.RepositoryItem = (isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds)) ? repositoryItembtnFinish : (isPause == 1 && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(endDepartmentId.ToString())) ? repositoryItembtnUnFinish : (isPause == 1 ? repositoryItembtnUnFinish_Disable : repositoryItembtnFinish_Disable));
                        }
                    }
                    else if (e.Column.FieldName == "PRINT_DISPLAY")
                    {

                        e.RepositoryItem = repositoryItembtnEdit_Print;
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        if (this.controlDelete != null)
                        {
                            if (this.controlDelete.IS_ANONYMOUS == 1)
                            {
                                e.RepositoryItem = BtnDelete_Enable;
                                isDelete = true;
                            }
                            else
                            {
                                if (RoleUse != null && ControlRule != null && RoleUse.Count > 0 && ControlRule.Count > 0)
                                {
                                    var CheckRole = this.RoleUse.Where(o => this.ControlRule.Select(p => p.ROLE_ID).Contains(o.ROLE_ID)).ToList();
                                    if ((CheckRole != null && CheckRole.Count > 0))
                                    {
                                        e.RepositoryItem = BtnDelete_Enable;
                                        isDelete = true;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = BtnDelete_Disable;
                                        isDelete = false;
                                    }
                                }
                                else
                                {
                                    e.RepositoryItem = BtnDelete_Disable;
                                    isDelete = false;
                                }

                            }
                        }
                    }
                    else if (e.Column.FieldName == "IS_AUTO_DISCOUNT_DISPLAY")
                    {
                        if (isAutoDiscount == 1)
                        {
                            e.RepositoryItem = ButtonEditIsAutoDiscount;
                        }
                    }
                    else if (e.Column.FieldName == "DeleteEndInfo")
                    {
                        if (isPause != 1 && treatmentEndTypeId != 0)
                        {
                            e.RepositoryItem = ButtonDeleteEndInfo;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEndInfoDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnFind(object sender, EventArgs e)
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

        public void BtnRefreshs()
        {
            try
            {
                if (btnRefresh.Enabled)
                {
                    BtnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultControl();
                txtTreatment.Text = "";
                txtPatient.Text = "";
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnSearch()
        {
            try
            {
                if (btnFind.Enabled)
                {
                    btnFind_Click(null, null);
                }
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
                WaitingManager.Show();
                FillDataToGrid();
                btnPrintServiceReq.Enabled = false;
                btnPrintfKSK.Enabled = false;
                btnRecordChecking.Enabled = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetDefaultControl();
                dtInHospital.Text = null;
                btnFind_Click(null, null);
            }
        }

        private void txtPatient_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void txtSocialInsuranceNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlTreatmentList)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlTreatmentList.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "TDL_PATIENT_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "TDL_PATIENT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "ST_DEPLAY")
                            {
                                short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_PAUSE") ?? "-1").ToString());
                                decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(lastRowHandle, "IS_ACTIVE") ?? "-1").ToString());
                                short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_LOCK_HEIN") ?? "-1").ToString());
                                //Status
                                //1- dang dieu tri
                                //2- da ket thuc
                                //3- khóa hồ sơ
                                //4- duyệt bhyt
                                if (status_islockhein != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                    {
                                        if (status_ispause != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                        {
                                            text = "Đang điều trị";
                                        }
                                        else
                                        {
                                            text = "Kết thúc điều trị";
                                        }
                                    }
                                    else
                                    {
                                        text = "Khóa hồ sơ";
                                    }
                                }
                                else
                                {
                                    text = "Duyệt bảo hiểm y tế";
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

        private void UCTreatmentList_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Control)
            //{
            // if (e.KeyCode == Keys.F) btnFind_Click(null, null);

            //  if (e.KeyCode == Keys.R)  btnRefresh_Click(null, null);

            //}

        }

        bool IsStayingDepartment(string departmentIds)
        {
            bool result = false;
            try
            {
                result = (this.currentModule != null && NowDepartmentOfTreatment(departmentIds) == HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).DepartmentId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        long NowDepartmentOfTreatment(string departmentIds)
        {
            long result = 0;
            try
            {
                string strResult = "";
                List<string> DepartmentIds = departmentIds.Split(',').ToList();
                strResult = DepartmentIds[DepartmentIds.Count - 1];
                DepartmentIds = strResult.Split('_').ToList();
                strResult = DepartmentIds[0];
                result = Convert.ToInt64(strResult);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void cboDienDieuTri_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dienDieuTri = "";
                if (_DienDieuTriSelecteds != null && _DienDieuTriSelecteds.Count > 0)
                {
                    foreach (var item in _DienDieuTriSelecteds)
                    {
                        dienDieuTri += item.TREATMENT_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = dienDieuTri;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboEndDepartment_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string endDepartment = "";
                if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                {
                    foreach (var item in _EndDepartmentSelecteds)
                    {
                        endDepartment += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = endDepartment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboTrangThai_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string trangThai = "";
                if (_TrangThaiSelecteds != null && _TrangThaiSelecteds.Count > 0)
                {
                    foreach (var item in _TrangThaiSelecteds)
                    {
                        trangThai += item.TrangThai + ", ";
                    }
                }

                e.DisplayText = trangThai;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboKieuBenhNhan_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string kieuBenhNhan = "";
                if (_KieuBenhNhanSelecteds != null && _KieuBenhNhanSelecteds.Count > 0)
                {
                    foreach (var item in _KieuBenhNhanSelecteds)
                    {
                        kieuBenhNhan += item.KieuBenhNhan + ", ";
                    }
                }
                e.DisplayText = kieuBenhNhan;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboContract_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string text = "";
                if (cboContract.EditValue != null && listKskContract != null && listKskContract.Count > 0)
                {
                    var ksk = listKskContract.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString()));
                    if (ksk != null)
                    {
                        text = ksk.KSK_CONTRACT_CODE + " - " + ksk.WORK_PLACE_NAME;
                    }
                }
                e.DisplayText = text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboContract_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboContract.Enabled = false;
                cboContract.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboEndDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboEndDepartment.Enabled = false;
                cboEndDepartment.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewtreatmentList_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (gridViewtreatmentList.GetSelectedRows().Count() > 0)
                {
                    btnPrintServiceReq.Enabled = true;
                    btnXuatXML.Enabled = true;
                    btnPrintfKSK.Enabled = true;
                    btnRecordChecking.Enabled = true;
                    if (!string.IsNullOrEmpty(Config.HisConfigCFG.SYNC_XML_FPT_OPTION))
                    {
                        btnGuiHS.Enabled = true;
                    }
                    else
                    {
                        btnGuiHS.Enabled = false;
                    }

                    // btnDelete
                    bool enableBtnDelete_HIS000027 = false;
                    bool enableBtnDelete_Data = true;
                    var selectedRows = gridViewtreatmentList.GetSelectedRows();
                    foreach (var i in selectedRows)
                    {
                        var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                        string departmentIds = (gridViewtreatmentList.GetRowCellValue(i, "DEPARTMENT_IDS") ?? "").ToString();
                        var status_ispause = row.IS_PAUSE;

                        if (status_ispause == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            || row.IS_LOCK_FEE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            || !IsStayingDepartment(departmentIds))
                        {
                            enableBtnDelete_Data = false;
                            break;
                        }
                    }

                    if (this.controlDelete != null)
                    {
                        if (this.controlDelete.IS_ANONYMOUS == 1)
                        {
                            enableBtnDelete_HIS000027 = true;
                        }
                        else
                        {
                            if (RoleUse != null && ControlRule != null && RoleUse.Count > 0 && ControlRule.Count > 0)
                            {
                                var CheckRole = this.RoleUse.Where(o => this.ControlRule.Select(p => p.ROLE_ID).Contains(o.ROLE_ID)).ToList();
                                if ((CheckRole != null && CheckRole.Count > 0))
                                {
                                    enableBtnDelete_HIS000027 = true;
                                }
                            }
                        }
                    }
                    btnDelete.Enabled = enableBtnDelete_Data && enableBtnDelete_HIS000027;
                }
                else
                {
                    btnPrintServiceReq.Enabled = false;
                    btnXuatXML.Enabled = false;
                    btnPrintfKSK.Enabled = false;
                    btnRecordChecking.Enabled = false;
                    btnGuiHS.Enabled = false;
                    btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnImportKsk_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listObj = new List<object>();
                listObj.Add((HIS.Desktop.Common.DelegateSelectData)DelegateKSK);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisImportKsk", this.currentModule.RoomId, this.currentModule.RoomTypeId, listObj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnPrintServiceReq_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_TREATMENT_4> listTreatment = new List<V_HIS_TREATMENT_4>();
                var rowHandles = gridViewtreatmentList.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                        if (row != null)
                        {
                            listTreatment.Add(row);
                        }
                    }
                }

                if (listTreatment != null && listTreatment.Count > 0)
                {
                    frmServiceType frm = new frmServiceType(currentModule, listTreatment.Select(o => o.ID).ToList(), listTreatment);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewtreatmentList_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var treatmentData = (V_HIS_TREATMENT_4)view.GetRow(hi.RowHandle);
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        short isPause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(hi.RowHandle, "IS_PAUSE") ?? "").ToString());
                        long treatmentEndTypeId = Int64.Parse((gridViewtreatmentList.GetRowCellValue(hi.RowHandle, "TREATMENT_END_TYPE_ID") ?? "0").ToString());
                        string departmentIds = (view.GetRowCellValue(hi.RowHandle, "DEPARTMENT_IDS") ?? "").ToString();
                        bool AssignService = false;
                        bool isfinishButton = false;
                        long endDepartmentId = Int64.Parse((view.GetRowCellValue(hi.RowHandle, "END_DEPARTMENT_ID") ?? "0").ToString());

                        long TreatmentTypeId = Int64.Parse((view.GetRowCellValue(hi.RowHandle, "TDL_TREATMENT_TYPE_ID") ?? "0").ToString());
                        string TreatmentCode = (view.GetRowCellValue(hi.RowHandle, "TREATMENT_CODE") ?? "0").ToString();
                        long endRoomId = Int64.Parse((view.GetRowCellValue(hi.RowHandle, "END_ROOM_ID") ?? "0").ToString());
                        if (treatmentData != null)
                        {
                            if (hi.Column.FieldName == "Finish")
                            {
                                #region ----- Kết thúc điều trị -----
                                if (treatmentData != null)
                                {
                                    if (!HisConfigCFG.IsUnlockConditionOption)
                                    {
                                        isfinishButton = (isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds));
                                        if ((isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds)))
                                        {
                                            repositoryItembtnFinish_Click(treatmentData);
                                        }
                                        else if (isPause == 1 && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(endDepartmentId.ToString())))
                                        {
                                            repositoryItembtnUnifinish_Click(treatmentData);
                                        }
                                    }
                                    else
                                    {
                                        bool IsEnableFinish = (isPause != 1) && (TreatmentTypeId != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && IsStayingDepartment(departmentIds) || CheckLoginAdmin.IsAdmin(loginName));
                                        bool IsEnableUnFinish = (isPause == 1) && ((TreatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && endRoomId == currentModule.RoomId) || (TreatmentTypeId != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && IsStayingDepartment(endDepartmentId.ToString())) || CheckLoginAdmin.IsAdmin(loginName));
                                        if (IsEnableFinish)
                                            repositoryItembtnFinish_Click(treatmentData);
                                        else if (IsEnableUnFinish)
                                            repositoryItembtnUnifinish_Click(treatmentData);
                                    }
                                }
                                #endregion
                            }
                            if (hi.Column.FieldName == "Delete")
                            {
                                #region ----- Xóa -----
                                //if (treatmentData != null && CheckLoginAdmin.IsAdmin(loginName))
                                if (isDelete)
                                {
                                    BtnDelete_Enable_ButtonClick(treatmentData);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "PRINT_DISPLAY")
                            {
                                #region ----- In -----
                                if (treatmentData != null)
                                {
                                    repositoryItemButtonEdit_Print_ButtonClick(treatmentData);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "ServiceReqList")
                            {
                                #region ----- Danh sách yêu cầu -----
                                if (treatmentData != null)
                                {
                                    repositoryItembtnServiceReqList_Click(treatmentData);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "Edit")
                            {
                                #region ----- Sửa hsdt -----
                                if (treatmentData != null)
                                {
                                    repositoryItembtnEditTreatment_btnClick(treatmentData);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "PaySereServ")
                            {
                                #region ----- Bảng kê thanh toán -----
                                if (treatmentData != null)
                                {
                                    repositoryItembtnPaySereServ_Click(treatmentData);
                                }
                                #endregion
                            }
                            if (hi.Column.FieldName == "DeleteEndInfo")
                            {
                                #region ----- Xóa thông tin ra viện -----
                                if (isPause != 1 && treatmentEndTypeId != 0)
                                {
                                    ButtonDeleteEndInfo_ButtonClick(treatmentData);
                                }
                                #endregion
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

        private List<HIS_SERE_SERV_EXT> GetSereServExt(long treatmentId)
        {
            List<HIS_SERE_SERV_EXT> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServExtFilter filter = new HisSereServExtFilter();
                filter.TDL_TREATMENT_ID = treatmentId;

                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private List<V_HIS_BED_LOG> GetBedLog(long treatmentId)
        {
            List<V_HIS_BED_LOG> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_ID = treatmentId;

                rs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private void cboContract_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboContract.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDepartment_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEndDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DelegateKSK(object data)
        {
            try
            {
                if (data != null)
                {
                    cboContract.EditValue = (long)data;
                    dtInHospital.EditValue = null;
                    dtOutHospital.EditValue = null;
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtStoreCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtInCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtOutCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void BtnDelete_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

        }

        private void BtnDelete_Enable_ButtonClick(V_HIS_TREATMENT_4 data)
        {
            try
            {
                if (data == null) return;
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa hồ sơ điều trị?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, data);

                    WaitingManager.Show();

                    var rs = new BackendAdapter(param).Post<bool>("api/HisTreatment/DeleteTestData", ApiConsumers.MosConsumer, treatment, param);

                    if (rs)
                    {
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();

                    MessageManager.Show(this.ParentForm, param, rs);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnPrintHuongDan_Click(object sender, EventArgs e)
        {
            try
            {
                List<HIS_TREATMENT> listTreatment = new List<HIS_TREATMENT>();
                var rowHandles = gridViewtreatmentList.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                        if (row != null)
                        {
                            HIS_TREATMENT treat = new HIS_TREATMENT();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treat, row);
                            listTreatment.Add(treat);
                        }
                    }
                }

                if (listTreatment != null && listTreatment.Count > 0)
                {
                    var PrintServiceReqProcessor = new HIS.Desktop.Plugins.Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor(listTreatment, this.currentModule != null ? this.currentModule.RoomId : 0);
                    PrintServiceReqProcessor.Print("Mps000276", false);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn hồ sơ");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintfKSK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrintfKSK.Enabled) return;
                InitMenuPrint();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewtreatmentList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                try
                {
                    if (e.RowHandle < 0)
                        return;
                    int rowHandleSelected = gridViewtreatmentList.GetVisibleRowHandle(e.RowHandle);
                    var data = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(rowHandleSelected);
                    if (data != null && data.APPROVAL_STORE_STT_ID == 2)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlTreatmentList_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button.IsRight())
            //{                
            //    ContextMenuStrip mnu = new ContextMenuStrip();
            //    ToolStripMenuItem mnuCopy = new ToolStripMenuItem("Copy");
            //    ToolStripMenuItem mnuCut = new ToolStripMenuItem("Cut");
            //    ToolStripMenuItem mnuPaste = new ToolStripMenuItem("Paste");
            //    //Assign event handlers
            //    mnuCopy.Click += new EventHandler(mnuCopy_Click);
            //    mnuCut.Click += new EventHandler(mnuCut_Click);
            //    mnuPaste.Click += new EventHandler(mnuPaste_Click);
            //    //Add to main context menu
            //    mnu.Items.AddRange(new ToolStripItem[] { mnuCopy, mnuCut, mnuPaste });
            //    //Assign to datagridview
            //    mnu.Show(e.Location);
            //}
        }

        private void mnuCut_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void mnuPaste_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        //hàm khi chọn trong ngày thì format ô dateedit bên cạnh sẽ thể hiện là dd/MM/yyyy
        //khi chọn trong tháng thì format ô dateedit bên cạnh sẽ thể hiện là MM/yyyy
        //lưu ý khi set gtri dtInHospital.EditValue = DateTime.Now phải để ở cuối 
        //nếu để trên đầu sẽ ảnh hưởng tới hàm dtInHospital_EditValueChanged 
        //private void cboInHospital_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cboInHospital.EditValue != null)
        //        {

        //            if (cboInHospital.EditValue.ToString() == "1")
        //            {                        
        //                dtInHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.MonthView;
        //                dtInHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy";
        //                dtInHospital.Properties.EditMask = "dd/MM/yyyy";
        //                dtInHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
        //                dtInHospital.EditValue = DateTime.Now;
        //            }
        //            else
        //            {                        
        //                dtInHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
        //                dtInHospital.Properties.EditFormat.FormatString = "MM/yyyy";
        //                dtInHospital.Properties.EditMask = "MM/yyyy";
        //                dtInHospital.Properties.DisplayFormat.FormatString = "MM/yyyy";
        //                dtInHospital.EditValue = DateTime.Now;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void cboOutHospital_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cboOutHospital.EditValue != null)
        //        {

        //            if (cboOutHospital.EditValue.ToString() == "1")
        //            {         
        //                dtOutHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.MonthView;
        //                dtOutHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy";
        //                dtOutHospital.Properties.EditMask = "dd/MM/yyyy";
        //                dtOutHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
        //                dtOutHospital.EditValue = DateTime.Now;
        //            }
        //            else
        //            {
        //                dtOutHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
        //                dtOutHospital.Properties.EditFormat.FormatString = "MM/yyyy";
        //                dtOutHospital.Properties.EditMask = "MM/yyyy";
        //                dtOutHospital.Properties.DisplayFormat.FormatString = "MM/yyyy";
        //                dtOutHospital.EditValue = DateTime.Now;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void btnPreDayInHospital_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(btnCodeFind.Text))
                {
                    var currentdate = dtInHospital.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtInHospital.EditValue = currentdate.AddDays(-1);
                    else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                        dtInHospital.EditValue = currentdate.AddMonths(-1);
                    else
                        dtInHospital.EditValue = currentdate.AddYears(-1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNextDayInHospital_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(btnCodeFind.Text))
                {
                    var currentdate = dtInHospital.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtInHospital.EditValue = currentdate.AddDays(1);
                    else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                        dtInHospital.EditValue = currentdate.AddMonths(1);
                    else
                        dtInHospital.EditValue = currentdate.AddYears(1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPreDayOutHospital_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(cboOutOfHospital.Text))
                {
                    var currentdate = dtOutHospital.DateTime;
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                        dtOutHospital.EditValue = currentdate.AddDays(-1);
                    else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                        dtOutHospital.EditValue = currentdate.AddMonths(-1);
                    else
                        dtOutHospital.EditValue = currentdate.AddYears(-1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNextDayOutHospital_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(cboOutOfHospital.Text))
                {
                    var currentdate = dtOutHospital.DateTime;
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                        dtOutHospital.EditValue = currentdate.AddDays(1);
                    else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                        dtOutHospital.EditValue = currentdate.AddMonths(1);
                    else
                        dtOutHospital.EditValue = currentdate.AddYears(1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //hàm khi thay đổi ngày thì comboInHospital sẽ thay đổi theo format đang đc thể hiện 
        // ví dụ: format 07/2020 thì sẽ hiện là trong tháng, 02/07/2020 thì sẽ hiện trong ngày
        //private void dtInHospital_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue)
        //        {
        //            btnPreDayInHospital.Enabled = true;
        //            btnNextDayInHospital.Enabled = true;

        //            String[] array = dtInHospital.Text.Split('/');
        //            if (array.Length == 2)
        //            {
        //                cboInHospital.EditValue = 2;
        //            }
        //            else
        //            {
        //                cboInHospital.EditValue = 1;
        //            }
        //        }
        //        else
        //        {
        //            btnPreDayInHospital.Enabled = false;
        //            btnNextDayInHospital.Enabled = false;
        //            cboInHospital.EditValue = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void dtOutHospital_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue)
                {
                    btnPreDayOutHospital.Enabled = true;
                    btnNextDayOutHospital.Enabled = true;
                }
                else
                {
                    btnPreDayOutHospital.Enabled = false;
                    btnNextDayOutHospital.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemInDateCode = new DXMenuItem(typeCodeFind__KeyWork_InDate, new EventHandler(btnCodeFind_Click));
                itemInDateCode.Tag = "InDate";
                menu.Items.Add(itemInDateCode);

                DXMenuItem itemInMonth = new DXMenuItem(typeCodeFind__InMonth, new EventHandler(btnCodeFind_Click));
                itemInMonth.Tag = "InMonth";
                menu.Items.Add(itemInMonth);

                DXMenuItem itemInYear = new DXMenuItem(typeCodeFind__InYear, new EventHandler(btnCodeFind_Click));
                itemInYear.Tag = "InMonth";
                menu.Items.Add(itemInYear);

                DXMenuItem itemInTime = new DXMenuItem(typeCodeFind__InTime, new EventHandler(btnCodeFind_Click));
                itemInTime.Tag = "InTime";
                menu.Items.Add(itemInTime);

                btnCodeFind.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboOutHopital()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemOutDateCode = new DXMenuItem(typeCodeFind__KeyWork_OutDate, new EventHandler(cboOutOfHospital_Click));
                itemOutDateCode.Tag = "OutDate";
                menu.Items.Add(itemOutDateCode);

                DXMenuItem itemOutMonth = new DXMenuItem(typeCodeFind__OutMonth, new EventHandler(cboOutOfHospital_Click));
                itemOutMonth.Tag = "OutMonth";
                menu.Items.Add(itemOutMonth);
                DXMenuItem itemOutYear = new DXMenuItem(typeCodeFind__OutYear, new EventHandler(cboOutOfHospital_Click));
                itemOutYear.Tag = "OutYear";
                menu.Items.Add(itemOutYear);

                DXMenuItem itemOutTime = new DXMenuItem(typeCodeFind__OutTime, new EventHandler(cboOutOfHospital_Click));
                itemOutTime.Tag = "OutTime";
                menu.Items.Add(itemOutTime);


                cboOutOfHospital.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormatDtIntructionDate()
        {
            try
            {
                layoutInDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem28.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                simpleLabelItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                {
                    dtInHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtInHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtInHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtInHospital.Properties.EditMask = "dd/MM/yyyy";
                    dtInHospital.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                {
                    dtInHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dtInHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dtInHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.EditFormat.FormatString = "MM/yyyy";
                    dtInHospital.Properties.EditMask = "MM/yyyy";
                    dtInHospital.Properties.Mask.EditMask = "MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InYear)
                {
                    dtInHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearsGroupView;
                    dtInHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.DisplayFormat.FormatString = "yyyy";
                    dtInHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.EditFormat.FormatString = "yyyy";
                    dtInHospital.Properties.EditMask = "yyyy";
                    dtInHospital.Properties.Mask.EditMask = "yyyy";
                }
                else
                {
                    simpleLabelItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutInDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem26.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem28.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dtInHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtInHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtInHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtInHospital.Properties.EditMask = "dd/MM/yyyy HH:mm";
                    dtInHospital.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dtInDateCome.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtInDateCome.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInDateCome.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtInDateCome.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInDateCome.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtInDateCome.Properties.EditMask = "dd/MM/yyyy HH:mm";
                    dtInDateCome.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dtInHospital.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                    dtInDateCome.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormatDtIntructionOutDate()
        {
            try
            {
                layoutOutDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem29.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                simpleLabelItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                {
                    dtOutHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtOutHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtOutHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtOutHospital.Properties.EditMask = "dd/MM/yyyy";
                    dtOutHospital.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                {
                    dtOutHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dtOutHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dtOutHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.EditFormat.FormatString = "MM/yyyy";
                    dtOutHospital.Properties.EditMask = "MM/yyyy";
                    dtOutHospital.Properties.Mask.EditMask = "MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__InYear)
                {
                    dtOutHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearsGroupView;
                    dtOutHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.DisplayFormat.FormatString = "yyyy";
                    dtOutHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.EditFormat.FormatString = "yyyy";
                    dtOutHospital.Properties.EditMask = "yyyy";
                    dtOutHospital.Properties.Mask.EditMask = "yyyy";
                }
                else
                {
                    simpleLabelItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutOutDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem29.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dtOutHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtOutHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtOutHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtOutHospital.Properties.EditMask = "dd/MM/yyyy HH:mm";
                    dtOutHospital.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dtOutDateCome.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtOutDateCome.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutDateCome.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtOutDateCome.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutDateCome.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtOutDateCome.Properties.EditMask = "dd/MM/yyyy HH:mm";
                    dtOutDateCome.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dtOutHospital.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                    dtOutDateCome.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCodeFind_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                btnCodeFind.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_InDate = btnMenuCodeFind.Caption;

                FormatDtIntructionDate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboOutOfHospital_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                cboOutOfHospital.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_OutDate = btnMenuCodeFind.Caption;

                FormatDtIntructionOutDate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDefaultData()
        {
            try
            {
                btnCodeFind.Text = typeCodeFind__KeyWork_InDate;
                FormatDtIntructionDate();
                FormatDtIntructionOutDate();
                //dtCreatefrom.Properties.VistaDisplayMode = DefaultBoolean.True;
                //dtCreatefrom.Properties.VistaEditTime = DefaultBoolean.True;
                //dtCreateTo.Properties.VistaDisplayMode = DefaultBoolean.True;
                //dtCreateTo.Properties.VistaEditTime = DefaultBoolean.True;
                //dtCreatefrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                //dtCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                dtInHospital.DateTime = DateTime.Now;
                //dtOutHospital.DateTime = null;
                //cboFind.SelectedIndex = 0;
                //cboTreatmentType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInHospital_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue)
                {
                    btnPreDayInHospital.Enabled = true;
                    btnNextDayInHospital.Enabled = true;
                }
                else
                {
                    btnPreDayInHospital.Enabled = false;
                    btnNextDayInHospital.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnXuatXML_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_TREATMENT_4> listSelection = new List<V_HIS_TREATMENT_4>();
                var rowHandles = gridViewtreatmentList.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                        if (row != null)
                        {
                            listSelection.Add(row);
                        }
                    }
                }
                if (!btnXuatXML.Enabled || listSelection == null || listSelection.Count == 0) return;
                CommonParam param = new CommonParam();
                bool success = false;
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    success = this.GenerateXml(ref param, listSelection, fbd.SelectedPath);
                    WaitingManager.Hide();

                    MessageManager.Show(this.ParentForm, param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool GenerateXml(ref CommonParam paramExport, List<V_HIS_TREATMENT_4> listSelection, string pathSave)
        {
            bool result = false;
            try
            {
                if (listSelection.Count > 0)
                {
                    listSelection = listSelection.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();
                    if (String.IsNullOrEmpty(pathSave))
                    {
                        var dicInfo = System.IO.Directory.CreateDirectory(pathSave);
                        if (dicInfo == null)
                        {
                            paramExport.Messages.Add(Resources.ResourceMessage.KhongCoDuongDan);
                            return result;
                        }
                    }


                    GlobalConfigStore.PathSaveXml = pathSave;

                    GlobalConfigStore.Branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
                    hisTreatments = new List<V_HIS_TREATMENT_3>();
                    int skip = 0;
                    while (listSelection.Count - skip > 0)
                    {
                        var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        CommonParam param = new CommonParam();
                        HisTreatmentView3Filter treatmentFilter = new HisTreatmentView3Filter();
                        treatmentFilter.IDs = limit.Select(o => o.ID).ToList();
                        var resultTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_3>>(HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW_3, ApiConsumers.MosConsumer, treatmentFilter, param);
                        if (resultTreatment != null && resultTreatment.Count > 0)
                        {
                            hisTreatments.AddRange(resultTreatment);
                        }

                        string message = "";
                        //CreateThreadGetData(limit);
                        message = ProcessExportXmlDetail(ref result, hisTreatments);
                        if (!String.IsNullOrEmpty(message))
                        {
                            paramExport.Messages.Add(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        //#region Thread
        //private void CreateThreadGetData(List<V_HIS_TREATMENT_4> listSelection)
        //{
        //    System.Threading.Thread Treatment3 = new System.Threading.Thread(ThreadGetTreatment3);
        //    try
        //    {
        //        Treatment3.Start(listSelection);

        //        Treatment3.Join();
        //    }
        //    catch (Exception ex)
        //    {
        //        Treatment3.Abort();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        //private void ThreadGetTreatment3(object obj)
        //{
        //    try
        //    {
        //        if (obj == null) return;
        //        List<V_HIS_TREATMENT_4> listSelection = (List<V_HIS_TREATMENT_4>)obj;
        //        hisTreatments = new List<V_HIS_TREATMENT_3>();

        //        var skip = 0;
        //        while (listSelection.Count - skip > 0)
        //        {
        //            var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
        //            skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

        //            CommonParam param = new CommonParam();
        //            HisTreatmentView3Filter treatmentFilter = new HisTreatmentView3Filter();
        //            treatmentFilter.IDs = limit.Select(o => o.ID).ToList();
        //            var resultTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_3>>(HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW_3, ApiConsumers.MosConsumer, treatmentFilter, param);
        //            if (resultTreatment != null && resultTreatment.Count > 0)
        //            {
        //                hisTreatments.AddRange(resultTreatment);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        //#endregion

        //InputADO chứa thông tin V_HIS_TREATMENT_3 Treatment(thông tin hồ sơ điều trị), HIS_BRANCH Branch(chi nhánh tiếp đón), V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter(Diện điều trị cuối cùng), HIS_SERVICE_REQ ExamServiceReq(y lệnh khám có thời gian y lệnh nhỏ nhất)

        string ProcessExportXmlDetail(ref bool isSuccess, List<V_HIS_TREATMENT_3> hisTreatments)
        {
            string result = "";
            Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
            try
            {
                if (hisTreatments != null && hisTreatments.Count > 0)
                {
                    List<V_HIS_PATIENT_TYPE_ALTER> LstPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
                    List<HIS_SERVICE_REQ> LstExamServiceReq = new List<HIS_SERVICE_REQ>();
                    List<V_HIS_SERE_SERV_2> lstSereServ2 = new List<V_HIS_SERE_SERV_2>();
                    CommonParam param1 = new CommonParam();

                    HisPatientTypeAlterViewFilter PatientTypefilter = new HisPatientTypeAlterViewFilter();
                    PatientTypefilter.TREATMENT_IDs = hisTreatments.Select(o => o.ID).ToList();

                    LstPatientTypeAlter = new BackendAdapter(param1).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, PatientTypefilter, param1);

                    HisServiceReqFilter ServiceReqFilter = new HisServiceReqFilter();
                    ServiceReqFilter.TREATMENT_IDs = hisTreatments.Select(o => o.ID).ToList();

                    LstExamServiceReq = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("/api/HisServiceReq/Get", ApiConsumers.MosConsumer, ServiceReqFilter, new CommonParam());


                    HisSereServView2Filter ssFilter = new HisSereServView2Filter();
                    ssFilter.TREATMENT_IDs = hisTreatments.Select(o => o.ID).ToList();

                    lstSereServ2 = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_2>>("/api/HisSereServ/GetView", ApiConsumers.MosConsumer, ssFilter, new CommonParam());

                    foreach (var treatment in hisTreatments)
                    {
                        InputADO ado = new InputADO();
                        ado.Treatment = treatment;
                        ado.Branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == treatment.BRANCH_ID);

                        //ado.PatientTypeAlter = BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALTER>().Where(o => o.TREATMENT_ID == treatment.ID).OrderByDescending(p => p.LOG_TIME).FirstOrDefault();
                        //ado.ExamServiceReq = BackendDataWorker.Get<HIS_SERVICE_REQ>().Where(o => o.TREATMENT_ID == treatment.ID).OrderBy(p => p.INTRUCTION_TIME).FirstOrDefault();

                        ado.PatientTypeAlter = LstPatientTypeAlter.Count > 0 ? LstPatientTypeAlter.Where(o => o.TREATMENT_ID == treatment.ID).OrderByDescending(p => p.LOG_TIME).FirstOrDefault() : null;
                        ado.ExamServiceReq = LstExamServiceReq.Count > 0 ? LstExamServiceReq.Where(o => o.TREATMENT_ID == treatment.ID).OrderBy(p => p.INTRUCTION_TIME).FirstOrDefault() : null;
                        ado.TotalIcdData = BackendDataWorker.Get<HIS_ICD>();
                        ado.ListHeinMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>();
                        var ss2 = lstSereServ2.Where(o => o.TDL_TREATMENT_ID == treatment.ID).OrderBy(o => o.INTRUCTION_TIME).ToList();
                        if (ss2 != null && ss2.Count() > 0)
                        {
                            ado.ListSereServ = ss2;
                        }
                        His.Bhyt.ExportXml.CreateXmlMain xmlMain = new His.Bhyt.ExportXml.CreateXmlMain(ado);

                        string errorMess = "";
                        string codeError = "";
                        string fullFileName = "";

                        fullFileName = xmlMain.RunCheckInPath(ref errorMess, ref codeError);

                        if (!string.IsNullOrEmpty(errorMess) && !string.IsNullOrEmpty(codeError))
                        {
                            result = errorMess + "," + codeError;
                        }
                        else
                        {
                            result = errorMess + codeError;
                        }
                        if (!String.IsNullOrWhiteSpace(fullFileName))
                        {
                            isSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }


        private void ResetComboPatientType(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                    cbo.EditValue = null;
                    cbo.Focus();
                    this.patientTypeSelecteds.AddRange(listPatientType);
                    gridCheckMark.SelectAll(this.patientTypeSelecteds);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboPatientTypeCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboPatientType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboPatientType.Properties.Tag = gridCheck;
                cboPatientType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPatientType.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitComboPatientType()
        {
            try
            {
                CommonParam commonParam = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = 1;
                listPatientType = new List<HIS_PATIENT_TYPE>();
                listPatientType = new BackendAdapter(commonParam).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, commonParam);


                cboPatientType.Properties.DataSource = listPatientType;
                cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn column = cboPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                column.VisibleIndex = 1;
                column.Width = 200;
                column.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.cboPaytientType.Column.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                cboPatientType.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboPatientType.Properties.View.OptionsSelection.MultiSelect = true;
                cboPatientType.Properties.View.BestFitColumns();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void Event_Check(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                patientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> erSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.PATIENT_TYPE_NAME);
                            erSelectedNews.Add(er);
                        }

                    }
                    patientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                    patientTypeSelecteds.AddRange(erSelectedNews);
                }
                cboPatientType.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string paytientType = "";
                if (this.patientTypeSelecteds != null && this.patientTypeSelecteds.Count > 0 && this.patientTypeSelecteds.Count < this.listPatientType.Count)
                {
                    foreach (var item in this.patientTypeSelecteds)
                    {

                        paytientType += item.PATIENT_TYPE_NAME;

                        if (!(item == patientTypeSelecteds.Last()))
                        {
                            paytientType += ", ";
                        }

                    }
                }
                else if (this.patientTypeSelecteds.Count == this.listPatientType.Count)
                {
                    paytientType = Inventec.Common.Resource.Get.Value("UCTreatmentList.cboPaytientType.Column.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                }
                e.DisplayText = paytientType;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void cboPatientType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboPatientType.Properties.View);
                    }
                    cboPatientType.EditValue = null;
                    cboPatientType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKhoaVaoVien_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboKhoaVaoVien.EditValue = null;
                    cboKhoaVaoVien.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKhoaVaoVien_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboKhoaVaoVien.EditValue = null;
                    cboKhoaVaoVien.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMenuPrint()
        {
            try
            {
                this.barManager.Form = this;
                if (this.menu == null)
                    this.menu = new PopupMenu(this.barManager);
                this.menu.ItemLinks.Clear();

                BarButtonItem InPhieuKSKDinhKy = new BarButtonItem(barManager, "Phiếu kiểm tra sức khỏe định kỳ", 1);
                InPhieuKSKDinhKy.Tag = PrintType.InPhieuKSKDinhKy;
                InPhieuKSKDinhKy.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(InPhieuKSKDinhKy);

                BarButtonItem InSoKSKDinhKy = new BarButtonItem(barManager, "Sổ khám sức khỏe định kỳ", 2);
                InSoKSKDinhKy.Tag = PrintType.InSoKSKDinhKy;
                InSoKSKDinhKy.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(InSoKSKDinhKy);

                BarButtonItem XuatExcelKQKSK = new BarButtonItem(barManager, "Xuất excel kết quả khám sức khỏe", 3);
                XuatExcelKQKSK.Tag = PrintType.XuatExcelKQKSK;
                XuatExcelKQKSK.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(XuatExcelKQKSK);

                BarButtonItem InPhieuKQCLSKSK = new BarButtonItem(barManager, "In phiếu kết quả CLS khám sức khỏe", 4);
                InPhieuKQCLSKSK.Tag = PrintType.InPhieuKQCLSKSK;
                InPhieuKQCLSKSK.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(InPhieuKQCLSKSK);

                this.menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void onClick__Pluss(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintType type = (PrintType)(e.Item.Tag);

                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    List<V_HIS_TREATMENT_4> listTreatment = new List<V_HIS_TREATMENT_4>();
                    var rowHandles = gridViewtreatmentList.GetSelectedRows();
                    if (rowHandles != null && rowHandles.Count() > 0)
                    {
                        foreach (var i in rowHandles)
                        {
                            var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                            if (row != null)
                            {
                                listTreatment.Add(row);
                            }
                        }
                    }

                    switch (type)
                    {
                        case PrintType.InPhieuKSKDinhKy:
                            if (listTreatment != null && listTreatment.Count > 0)
                            {
                                ProcessPrintf(listTreatment);
                            }
                            break;
                        case PrintType.InSoKSKDinhKy:
                            if (listTreatment != null && listTreatment.Count > 0)
                            {
                                foreach (var item in listTreatment)
                                {
                                    ProcessPrintfSoKSK(item);
                                }
                            }
                            break;
                        case PrintType.XuatExcelKQKSK:
                            if (listTreatment != null && listTreatment.Count > 0)
                            {
                                ProcessExcell(listTreatment);
                            }
                            break;
                        case PrintType.InPhieuKQCLSKSK:
                            ProcessPrintKQCLSKSK(listTreatment);
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView5_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ExcellDataADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName.Contains("TDL_SERVICE_NAME_"))
                        {
                            if (lstHeaderColumns != null && lstHeaderColumns.Count > 0)
                            {
                                for (int i = 0; i < lstHeaderColumns.Count; i++)
                                {
                                    var dt = ListTemp.Where(o => o.TDL_SERVICE_NAME == lstHeaderColumns[i] && o.ID_TREATMENT == data.ID).ToList();
                                    if (dt != null && dt.Count > 0)
                                    {
                                        if (e.Column.FieldName == "TDL_SERVICE_NAME_" + i)
                                        {
                                            if (!string.IsNullOrEmpty(dt.FirstOrDefault().CONCLUDE))
                                            {
                                                e.Value = dt.FirstOrDefault().CONCLUDE;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (e.Column.FieldName.Contains("TDL_SERVICE_NAME___SERE_SERV_TEIN"))
                        {
                            if (lstHeaderColumnsXN != null && lstHeaderColumnsXN.Count > 0)
                            {
                                for (int i = 0; i < lstHeaderColumnsXN.Count; i++)
                                {
                                    var dt = ListTempXN.Where(o => o.TDL_SERVICE_NAME == lstHeaderColumnsXN[i] && o.ID_TREATMENT == data.ID).ToList();
                                    if (dt != null && dt.Count > 0)
                                    {
                                        if (e.Column.FieldName == "TDL_SERVICE_NAME___SERE_SERV_TEIN" + i)
                                        {
                                            if (!string.IsNullOrEmpty(dt.FirstOrDefault().VALUE))
                                            {
                                                e.Value = dt.FirstOrDefault().VALUE;
                                            }
                                        }
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

        private void btnGuiHS_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<V_HIS_TREATMENT_4> listSelection = new List<V_HIS_TREATMENT_4>();
                var rowHandles = gridViewtreatmentList.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                        if (row != null)
                        {
                            listSelection.Add(row);
                        }
                    }
                }
                CommonParam param = new CommonParam();

                var rs = new BackendAdapter(param).Post<bool>("api/HisTreatment/TransferXml", ApiConsumers.MosConsumer, listSelection.Select(o => o.ID), param);
                //Inventec.Common.Logging.LogSystem.Debug("Intput___:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listSelection.Select(o => o.ID)), listSelection.Select(o => o.ID)));
                //Inventec.Common.Logging.LogSystem.Info("Output___:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, rs);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa hồ sơ điều trị?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ProcessDeleteListTreatment();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDeleteListTreatment()
        {
            try
            {
                List<V_HIS_TREATMENT_4> listSelection = new List<V_HIS_TREATMENT_4>();
                var rowHandles = gridViewtreatmentList.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                        if (row != null)
                        {
                            listSelection.Add(row);
                        }
                    }
                }
                bool success = false;
                string messageSummary = "";
                Dictionary<string, List<string>> dicFailedTreatmentMessage = new Dictionary<string, List<string>>();
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                foreach (var item in listSelection)
                {
                    if (item == null)
                        continue;
                    var rs = ProcessDeleteTreatment(item, ref param);
                    if (rs == false)
                    {
                        string message = param != null ? MessageUtil.GetMessageAlert(param) : " ";
                        if (!dicFailedTreatmentMessage.ContainsKey(message))
                        {
                            dicFailedTreatmentMessage.Add(message, new List<string>());
                        }
                        dicFailedTreatmentMessage[message].Add(item.TREATMENT_CODE);
                    }
                    else
                    {
                        success = true;
                    }
                }

                if (dicFailedTreatmentMessage.Count() > 0)
                {
                    messageSummary = " Các hồ sơ sau thực hiện xóa thất bại: ";
                    foreach (var mess in dicFailedTreatmentMessage)
                    {
                        string strCodes = "";
                        foreach (var treatmentCode in mess.Value)
                        {
                            strCodes += treatmentCode + ", ";
                        }
                        if (strCodes.Length > 0)
                            strCodes = strCodes.Remove(strCodes.Length - 2, 2);

                        messageSummary += strCodes + ": " + mess.Key + "; ";
                    }
                    messageSummary = messageSummary.Remove(messageSummary.Length - 2, 2);
                }


                DevExpress.XtraEditors.XtraMessageBox.Show("Hoàn thành xử lý." + messageSummary, "Thông báo", MessageBoxButtons.OK);
                if (success)
                {
                    FillDataToGrid();
                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessDeleteTreatment(V_HIS_TREATMENT_4 data, ref CommonParam param)
        {
            bool result = false;
            try
            {
                if (data == null) return false;
                param = new CommonParam();
                HIS_TREATMENT treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, data);
                result = new BackendAdapter(param).Post<bool>("api/HisTreatment/DeleteTestData", ApiConsumers.MosConsumer, treatment, param);

                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnRecordChecking_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRecordChecking.Enabled) return;

                List<long> listTreatmentIds = new List<long>();
                var rowHandles = gridViewtreatmentList.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                        if (row != null)
                        {
                            listTreatmentIds.Add(row.ID);
                        }
                    }
                }
                if (listTreatmentIds.Count() > 0)
                {
                    List<object> listObj = new List<object>();
                    listObj.Add(listTreatmentIds);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTreatmentRecordChecking", this.currentModule.RoomId, this.currentModule.RoomTypeId, listObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlTreatmentList_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    if (gridViewtreatmentList.GetRowCellValue(gridViewtreatmentList.FocusedRowHandle, gridViewtreatmentList.FocusedColumn) != null && gridViewtreatmentList.GetRowCellValue(gridViewtreatmentList.FocusedRowHandle, gridViewtreatmentList.FocusedColumn).ToString() != String.Empty)
                        Clipboard.SetText(gridViewtreatmentList.GetRowCellValue(gridViewtreatmentList.FocusedRowHandle, gridViewtreatmentList.FocusedColumn).ToString());
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewtreatmentList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ChangeAllEditColumn(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewtreatmentList_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeAllEditColumn(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeAllEditColumn(bool IsAllowEdit)
        {
            try
            {
                if (gridViewtreatmentList.FocusedRowHandle > -1)
                {
                    var columnFocus = gridViewtreatmentList.FocusedColumn;
                    if (columnFocus.FieldName == "TREATMENT_CODE" || columnFocus.FieldName == "TDL_PATIENT_CODE" || columnFocus.FieldName == "TDL_PATIENT_NAME" || columnFocus.FieldName == "TDL_HEIN_CARD_NUMBER" || columnFocus.FieldName == "TDL_PATIENT_ADDRESS" || columnFocus.FieldName == "ICD_MAIN_TEXT_ST")
                    {
                        foreach (var item in gridViewtreatmentList.Columns.ToList())
                        {
                            if (item.FieldName == columnFocus.FieldName)
                            {
                                item.OptionsColumn.AllowEdit = IsAllowEdit;
                                item.OptionsColumn.ReadOnly = true;
                                break;
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

        private void ButtonDeleteEndInfo_ButtonClick(V_HIS_TREATMENT_4 data)
        {
            try
            {
                if (data == null) return;
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa thông tin ra viện không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    bool success = false;
                    var rs = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/DeleteEndInfo", ApiConsumers.MosConsumer, data.ID, param);

                    if (rs != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();

                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndRoom_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
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
                foreach (V_HIS_ROOM rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.ROOM_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentEndTypeExt_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTreatmentEndTypeExt.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

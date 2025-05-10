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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.BedRoomPartial.Base;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.ADO;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using AutoMapper;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Plugins.BedRoomPartial.Key;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory;
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory.Base;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using HIS.Desktop.Plugins.BedRoomPartial.Resources;
using EMR.EFMODEL.DataModels;
using DevExpress.XtraTreeList;
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;
using System.Reflection;
using EMR.SDO;
using System.IO;
using DevExpress.XtraEditors;
using DevExpress.XtraExport;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    public partial class UCBedRoomPartial : UserControlBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        internal long treatmentId;
        internal string treatmentCode;
        bool isUseAddedTime = false;
        internal L_HIS_TREATMENT_BED_ROOM treatmentBedRoomRow { get; set; }
        internal L_HIS_TREATMENT_BED_ROOM RowCellClickBedRoom { get; set; }

        internal ServiceReqGroupByDateADO rowClickByDate { get; set; }

        BedRoomPopupMenuProcessor bedRoomPopupMenuProcessor;
        MediRecordMenuPopupProcessor emrMenuPopupProcessor = null;

        DHisSereServ2 TreeClickData;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        int pageSize = 0;
        int pageIndex = 0;

        int lastRowHandle = -1;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string ModuleLinkName = "HIS.Desktop.Plugins.BedRoomPartial";
        List<TreatmentBedRoomADO> _TreatmentBedRoomADOs { get; set; }
        public bool IsFormClosingOption { get; private set; }
        public List<string> lstModuleLinkApply;
        List<HIS_TREATMENT_ROOM> executeRoomSelecteds;
        List<V_HIS_ROOM> bedRoomFilterSelecteds;
        List<HIS_PATIENT_CLASSIFY> patientClassifyFilterSelecteds = new List<HIS_PATIENT_CLASSIFY>();
        List<V_HIS_ROOM> bedRoomAlls;
        List<HIS_TREATMENT> histreatment;
        /// <summary>
        /// khoa mà người dùng đang làm việc
        /// </summary>
        long DepartmentID;
        bool IsExpand;
        DHisSereServ2 _SereServADORightMouseClick = new DHisSereServ2();
        long wkRoomId { get; set; }

        long wkRoomTypeId = 0;
        V_HIS_BED_ROOM currentBedRoom;
        UCTreeListService ucAll, ucCLS, ucMediMate, ucOrther;
        bool IsExpandList = true;
        public UCBedRoomPartial()
            : base(null)
        {
            InitializeComponent();
        }

        public UCBedRoomPartial(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.wkRoomId = this.currentModule != null ? this.currentModule.RoomId : 0;
                this.wkRoomTypeId = this.currentModule != null ? this.currentModule.RoomTypeId : 0;
                LciGroupEmrDocument1.Expanded = false;//mặc định lần đâu là ẩn đi
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCBedRoomPartial_Load(object sender, EventArgs e)
        {
            try
            {
                AddUc();
                rowClickByDate = new ServiceReqGroupByDateADO();
                gridViewTreatmentBedRoom.FocusedRowHandle = -1;
                ResourceLangManager.InitResourceLanguageManager();
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BedRoomPartial.Resources.Lang", typeof(HIS.Desktop.Plugins.BedRoomPartial.UCBedRoomPartial).Assembly);
                this.SetCaptionByLanguageKey();
                this.currentBedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
                this.DepartmentID = currentBedRoom != null ? currentBedRoom.DEPARTMENT_ID : 0;
                this.ucViewEmrDocumentReq.ShowBar = true;
                this.ucViewEmrDocumentResult.ShowBar = true;
                InItHasCoTreatmentCombo();
                InitComboBedRoomCheck();
                InitComboBedRoom();
                InitComboPatientFilter();
                InitComboPATIENT_CLASSIFY();
                InitComboTreatmentType();
                InitComboEmployee();
                InitComboTreatmentStatus();
                InitComboFilterByDepartment();
                SetTreeListDateTimeProperties();

                this.InitControlState();

                cboPatientFilter.EditValue = (long)0;
                cboFilterByDepartment.EditValue = (long)0;
                this.txtKeyWord.Text = "";
                this.FillDataToGridTreatmentBedRoom();
                this.LoadInfoPatientTotalInfo();
                gridControlTreatmentBedRoom.ToolTipController = this.toolTipController1;
                LoadKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKey()
        {
            try
            {
                IsFormClosingOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.HIS_CONFIG_KEY__FormClosingOption) == GlobalVariables.CommonStringTrue;
                if (!string.IsNullOrEmpty(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.HIS_CONFIG_KEY__FormClosingOption)))
                {
                    lstModuleLinkApply = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.HIS_CONFIG_KEY__ModuleLinkApply).Split(',').ToList();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTreeListDateTimeProperties()
        {
            try
            {
                treeListDateTime.KeyFieldName = "TREELIST_ID";
                treeListDateTime.ParentFieldName = "PARENT_ID";
                treeListDateTime.Appearance.FocusedCell.BackColor = Color.LightBlue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddUc()
        {
            try
            {
                ucAll = new UCTreeListService(imageCollection1, currentModule);
                ucMediMate = new UCTreeListService(imageCollection1, currentModule);
                ucCLS = new UCTreeListService(imageCollection1, currentModule);
                ucOrther = new UCTreeListService(imageCollection1, currentModule);
                panelControl1.Controls.Add(ucAll);
                ucAll.Dock = DockStyle.Fill;
                panelControl2.Controls.Add(ucCLS);
                ucCLS.Dock = DockStyle.Fill;
                panelControl3.Controls.Add(ucMediMate);
                ucMediMate.Dock = DockStyle.Fill;
                panelControl4.Controls.Add(ucOrther);
                ucOrther.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboPatientFilter()
        {
            try
            {
                List<ADO.PatientFilterADO> listFilterPatient = new List<ADO.PatientFilterADO>();

                listFilterPatient.Add(new ADO.PatientFilterADO(0, Resources.ResourceMessage.BNDangOTrongBuong));
                listFilterPatient.Add(new ADO.PatientFilterADO(1, Resources.ResourceMessage.BNVaoTrongKhoang));
                listFilterPatient.Add(new ADO.PatientFilterADO(2, Resources.ResourceMessage.BNChuaKeDonTrongKhoang));
                cboPatientFilter.Properties.DataSource = listFilterPatient;
                cboPatientFilter.Properties.DisplayMember = "PatientFilter";
                cboPatientFilter.Properties.ValueMember = "ID";
                cboPatientFilter.Properties.ForceInitialize();
                cboPatientFilter.Properties.Columns.Clear();
                cboPatientFilter.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PatientFilter", "", 200));
                cboPatientFilter.Properties.ShowHeader = false;
                cboPatientFilter.Properties.ImmediatePopup = true;
                cboPatientFilter.Properties.DropDownRows = 3;
                cboPatientFilter.Properties.PopupWidth = 220;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InItHasCoTreatmentCombo()
        {
            try
            {
                List<HasCoTreatmentADO> HasCoTreatmentList = new List<HasCoTreatmentADO>();

                HasCoTreatmentADO allAdo = new HasCoTreatmentADO();
                allAdo.HAS_CO_TREATMENT_CODE = "ALL";
                allAdo.HAS_CO_TREATMENT_NAME = Resources.ResourceMessage.TatCa;
                HasCoTreatmentList.Add(allAdo);

                HasCoTreatmentADO NoCo = new HasCoTreatmentADO();
                NoCo.HAS_CO_TREATMENT_CODE = "NOCO";
                NoCo.HAS_CO_TREATMENT_NAME = Resources.ResourceMessage.DieuTriThuong;
                HasCoTreatmentList.Add(NoCo);

                HasCoTreatmentADO CO = new HasCoTreatmentADO();
                CO.HAS_CO_TREATMENT_CODE = "CO";
                CO.HAS_CO_TREATMENT_NAME = Resources.ResourceMessage.DieuTriKetHop;
                HasCoTreatmentList.Add(CO);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HAS_CO_TREATMENT_NAME", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HAS_CO_TREATMENT_NAME", "HAS_CO_TREATMENT_CODE", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboHasCoTreatment, HasCoTreatmentList, controlEditorADO);
                cboHasCoTreatment.EditValue = "ALL";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_PATIENT_CLASSIFY firstRowComboPATIENT_CLASSIFY = new HIS_PATIENT_CLASSIFY();

        private void InitComboPATIENT_CLASSIFY()
        {
            try
            {
                List<HIS_PATIENT_CLASSIFY> datas = new List<HIS_PATIENT_CLASSIFY>();

                firstRowComboPATIENT_CLASSIFY.PATIENT_CLASSIFY_CODE = "ALL";
                firstRowComboPATIENT_CLASSIFY.PATIENT_CLASSIFY_NAME = Resources.ResourceMessage.TatCa;
                firstRowComboPATIENT_CLASSIFY.ID = 0;
                datas.Add(firstRowComboPATIENT_CLASSIFY);
                datas.AddRange(BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_CLASSIFY_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_CLASSIFY_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_CLASSIFY_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboPATIENT_CLASSIFY, datas, controlEditorADO);

                cboPATIENT_CLASSIFY.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboPATIENT_CLASSIFY.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(this.Event_ComboPATIENT_CLASSIFY_Check);
                cboPATIENT_CLASSIFY.Properties.Tag = gridCheck;
                cboPATIENT_CLASSIFY.Properties.View.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cboPATIENT_CLASSIFY_View_MouseUp);

                GridCheckMarksSelection gridCheckMark = cboPATIENT_CLASSIFY.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPATIENT_CLASSIFY.Properties.View);
                    List<HIS_PATIENT_CLASSIFY> data = new List<HIS_PATIENT_CLASSIFY>();
                    data.Add(firstRowComboPATIENT_CLASSIFY);
                    gridCheckMark.SelectAll(data);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPATIENT_CLASSIFY_View_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = cboPATIENT_CLASSIFY.Properties.View as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.FieldName != "PATIENT_CLASSIFY_CODE" && hi.Column.FieldName != "PATIENT_CLASSIFY_NAME")
                        {
                            GridCheckMarksSelection gridCheckMark = cboPATIENT_CLASSIFY.Properties.Tag as GridCheckMarksSelection;
                            List<HIS_PATIENT_CLASSIFY> data = new List<HIS_PATIENT_CLASSIFY>();
                            var selectedRows = view.GetSelectedRows();
                            if (hi.RowHandle == 0) //Tất cả
                            {
                                if (this.patientClassifyFilterSelecteds.Contains(this.firstRowComboPATIENT_CLASSIFY))
                                    data.Add(this.firstRowComboPATIENT_CLASSIFY);

                                if (gridCheckMark != null)
                                {
                                    if (data.Count == 0)
                                        gridCheckMark.ClearSelection(cboPATIENT_CLASSIFY.Properties.View);
                                    else
                                        gridCheckMark.SelectAll(data);
                                    cboPATIENT_CLASSIFY.Properties.View.RefreshData();
                                }
                            }
                            else if (hi.RowHandle > 0)
                            {
                                if (this.patientClassifyFilterSelecteds.Contains(this.firstRowComboPATIENT_CLASSIFY))
                                {
                                    data = this.patientClassifyFilterSelecteds.Where(o => o.ID != 0).ToList();

                                    if (gridCheckMark != null)
                                    {
                                        gridCheckMark.SelectAll(data);
                                        cboPATIENT_CLASSIFY.Properties.View.RefreshData();
                                    }
                                }
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

        private void Event_ComboPATIENT_CLASSIFY_Check(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_CLASSIFY> erSelectedNews = new List<HIS_PATIENT_CLASSIFY>();
                    foreach (HIS_PATIENT_CLASSIFY er in gridCheckMark.Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.PATIENT_CLASSIFY_NAME);
                            erSelectedNews.Add(er);
                        }
                    }

                    this.patientClassifyFilterSelecteds = new List<HIS_PATIENT_CLASSIFY>();
                    this.patientClassifyFilterSelecteds.AddRange(erSelectedNews);
                }
                this.cboPATIENT_CLASSIFY.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTreatmentType()
        {
            try
            {
                List<HasCoTreatmentADO> HasCoTreatmentList = new List<HasCoTreatmentADO>();
                List<HIS_TREATMENT_TYPE> datas = new List<HIS_TREATMENT_TYPE>();
                HIS_TREATMENT_TYPE firstRow = new HIS_TREATMENT_TYPE();
                firstRow.TREATMENT_TYPE_CODE = "ALL";
                firstRow.TREATMENT_TYPE_NAME = Resources.ResourceMessage.TatCa;
                firstRow.ID = 0;
                datas.Add(firstRow);
                datas.AddRange(BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboTreatmentType, datas, controlEditorADO);
                cboTreatmentType.EditValue = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboTreatmentStatus()
        {
            try
            {
                var lstTreatmentStt = new List<TreatmentStatusADO>(){
                new TreatmentStatusADO(){Value = 0, Name = Resources.ResourceMessage.TatCa},
                new TreatmentStatusADO(){Value = 1, Name = Resources.ResourceMessage.DangDieuTriTrongBuong},
                new TreatmentStatusADO(){Value = 2, Name = Resources.ResourceMessage.DangDieuTriNgoaiBuong},
                new TreatmentStatusADO(){Value = 3, Name = Resources.ResourceMessage.DaKetThucDieuTri}};
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Value", columnInfos, false, 200);
                ControlEditorLoader.Load(cboTreatmentStatus, lstTreatmentStt, controlEditorADO);
                cboTreatmentStatus.EditValue = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboEmployee()
        {
            try
            {
                var data = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DOCTOR == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME","Tên đăng nhập", 150, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ và tên", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(cboEmployee, data, controlEditorADO);
                cboEmployee.Properties.ImmediatePopup = true;
                cboEmployee.Properties.PopupFormMinSize = new Size(400, cboEmployee.Properties.PopupFormMinSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboFilterByDepartment()
        {
            try
            {
                List<ADO.DepartmentFilterADO> listFilterDepartment = new List<ADO.DepartmentFilterADO>();

                listFilterDepartment.Add(new ADO.DepartmentFilterADO(0, Resources.ResourceMessage.TrongKhoa));
                listFilterDepartment.Add(new ADO.DepartmentFilterADO(1, Resources.ResourceMessage.TatCa));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DepartmentFilter", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DepartmentFilter", "ID", columnInfos, false, 151);
                ControlEditorLoader.Load(cboFilterByDepartment, listFilterDepartment, controlEditorADO);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BedRoomPartial.Resources.Lang", typeof(UCBedRoomPartial).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnBedHistory.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnBedHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKeDonYHCT.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnKeDonYHCT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnInToDieuTri.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnInToDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnHoiChan.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnHoiChan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTuTruc.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnTuTruc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDanhSachYeuCau.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnDanhSachYeuCau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChiDinhDichVu.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnChiDinhDichVu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnBangKe.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnBangKe.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKeDonThuoc.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnKeDonThuoc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChiDinhMau.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnChiDinhMau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKetThucDieuTri.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnKetThucDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChuyenKhoa.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnChuyenKhoa.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlTreeSereServ.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlTreeSereServ.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage4.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.xtraTabPage4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnThuGon.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnThuGon.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabDocumentReq.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.xtraTabDocumentReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabDocumentResult.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.xtraTabDocumentResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciGroupEmrDocument1.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.LciGroupEmrDocument1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFilterByDepartment.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciFilterByDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColDateTime.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.treeColDateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxPatientInfo.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.groupBoxPatientInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciPatientName.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDOB.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciDOB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGenner.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciGenner.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem7.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciTreatmentType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciApprovalNote.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciApprovalNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciApprovalNote.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciApprovalNote.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAddress.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem38.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem38.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem38.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem38.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLbTreatDoctor.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lciLbTreatDoctor.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem23.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem45.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem45.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.cboTreatmentStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkOBSERVED.Properties.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.chkOBSERVED.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPATIENT_CLASSIFY.Properties.NullText = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.cboPATIENT_CLASSIFY.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBedRoomSelect.Properties.NullText = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.cboBedRoomSelect.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKhongHienThiKTT.Properties.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.chkKhongHienThiKTT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKhongHienThiKTT.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.chkKhongHienThiKTT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHasCoTreatment.Properties.NullText = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.cboHasCoTreatment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblTotalPatietInfo.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.lblTotalPatietInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewTreatmentBedRoom.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridViewTreatmentBedRoom.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSTT.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.grdColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientName.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.grdColPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.grdColPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.grdColTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAddTime.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.grdColAddTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBedName.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.grdColBedName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRoomName.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.grdColRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ClassifyName.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gc_ClassifyName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ClassifyName.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gc_ClassifyName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_MedisoftH.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gc_MedisoftH.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientFilter.Properties.NullText = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.cboPatientFilter.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColPhone.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColPhone.Tooltip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColCardExpiry.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemButton_IsUse.Buttons[0].ToolTip = Resources.ResourceMessage.ThuocVtBNDaDung;
                this.repositoryItemButtonIS_EMERGENCY_STR.Buttons[0].ToolTip = Resources.ResourceMessage.BenhNhanCapCuu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khoi Tao Du Lieu Bed Room
        /// </summary>
        private void FillDataToGridTreatmentBedRoom()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridTreatment(new CommonParam(0, (int)pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatment, param, pageSize, gridControlTreatmentBedRoom);
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
                WaitingManager.Show();
                List<L_HIS_TREATMENT_BED_ROOM> lstTreatmentBedRoom = new List<L_HIS_TREATMENT_BED_ROOM>();
                _TreatmentBedRoomADOs = new List<TreatmentBedRoomADO>();
                this.pageIndex = 0;

                gridControlTreatmentBedRoom.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                if (chkKhongHienThiKTT.Checked == true)
                {
                    treatFilter.HAS_OUT_TIME = false;
                }
                var resultRO = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<L_HIS_TREATMENT_BED_ROOM>>(UriApi.HIS_TREATMENT_BED_ROOM_GETLVIEW, ApiConsumers.MosConsumer, treatFilter, paramCommon);
                if (resultRO != null)
                {
                    lstTreatmentBedRoom = (List<L_HIS_TREATMENT_BED_ROOM>)resultRO.Data.OrderBy(p => p.TDL_PATIENT_FIRST_NAME).ToList();
                    rowCount = (lstTreatmentBedRoom == null ? 0 : lstTreatmentBedRoom.Count);
                    dataTotal = (resultRO.Param == null ? 0 : resultRO.Param.Count ?? 0);
                }
                if (lstTreatmentBedRoom != null && lstTreatmentBedRoom.Count > 0)
                {
                    _TreatmentBedRoomADOs.AddRange((from r in lstTreatmentBedRoom select new HIS.Desktop.Plugins.BedRoomPartial.Base.TreatmentBedRoomADO(r)).OrderBy(p => p.BED_ROOM_NAME).ToList());
                }


                if ((this.bedRoomFilterSelecteds != null && this.bedRoomFilterSelecteds.Count > 1))
                {
                    grdColSTT.Width = 120;

                }
                else
                {
                    grdColSTT.Width = 75;

                }

                CommonParam par = new CommonParam();
                MOS.Filter.HisTreatmentFilter _TreatmentFilter = new HisTreatmentFilter();
                _TreatmentFilter.IDs = _TreatmentBedRoomADOs.Select(o => o.TREATMENT_ID).ToList();
                histreatment = new BackendAdapter(par).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, _TreatmentFilter, par);
                gridControlTreatmentBedRoom.BeginUpdate();
                gridControlTreatmentBedRoom.DataSource = _TreatmentBedRoomADOs;
                gridControlTreatmentBedRoom.EndUpdate();
                gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedRow = false;
                gridViewTreatmentBedRoom.BestFitColumns();
                if ((this.bedRoomFilterSelecteds != null && this.bedRoomFilterSelecteds.Count > 1))
                {
                    grdColRoomName.GroupIndex = 0;
                    gridViewTreatmentBedRoom.ExpandAllGroups();
                }
                else
                {
                    grdColSTT.Width = 50;
                    grdColRoomName.GroupIndex = -1;
                }
                if (RowCellClickBedRoom != null && RowCellClickBedRoom.ID > 0 && _TreatmentBedRoomADOs != null && _TreatmentBedRoomADOs.Count > 0)
                {
                    var rowCell = _TreatmentBedRoomADOs.Exists(o => o.ID == RowCellClickBedRoom.ID);
                    RowCellClickBedRoom = rowCell ? _TreatmentBedRoomADOs.FirstOrDefault(o => o.ID == RowCellClickBedRoom.ID) : null;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTreatmentBedRoomFilter(ref MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter, bool _isUseAddedTime)
        {
            try
            {
                treatFilter.ORDER_DIRECTION = "ASC";
                treatFilter.ORDER_FIELD = "TDL_PATIENT_FIRST_NAME";

                if (this.bedRoomFilterSelecteds != null && this.bedRoomFilterSelecteds.Count > 0)
                {
                    var currentBedRooms = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => this.bedRoomFilterSelecteds.Select(k => k.ID).Contains(o.ROOM_ID)).ToList();
                    if (currentBedRooms != null && currentBedRooms.Count > 0)
                    {
                        treatFilter.BED_ROOM_IDs = currentBedRooms.Select(k => k.ID).ToList();
                    }
                }
                else
                {
                    if (this.currentBedRoom != null)
                    {
                        treatFilter.BED_ROOM_ID = this.currentBedRoom.ID;
                    }
                }

                if (!String.IsNullOrEmpty(txtKeyWord.Text))
                {
                    treatFilter.KEYWORD__PATIENT_NAME__TREATMENT_CODE__BED_NAME__PATIENT_CODE = txtKeyWord.Text;
                }
                if (chkOBSERVED.Checked)
                {
                    treatFilter.OBSERVED_TIME_BETWEEN = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                }
                if (_isUseAddedTime)
                {
                    if (cboPatientFilter.EditValue != null && (long)cboPatientFilter.EditValue == 2)
                    {
                        
                        if (dtFrom.EditValue != null && dtTo.EditValue != null && dtFrom.DateTime != DateTime.MinValue && dtTo.DateTime != DateTime.MinValue)
                        {
                            
                            if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
                            {
                                treatFilter.NO_PRESCRIPTION_FROM = Convert.ToInt64(dtFrom.DateTime.ToString("yyyyMMdd") + "000000");
                            }
                            if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
                            {
                                treatFilter.NO_PRESCRIPTION_TO = Convert.ToInt64(dtTo.DateTime.ToString("yyyyMMdd") + "235959");
                            }
                        }
                    }
                    else
                    {
                        if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
                        {
                            treatFilter.ADD_TIME_FROM = Convert.ToInt64(dtFrom.DateTime.ToString("yyyyMMdd") + "000000");
                        }
                        if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
                        {
                            treatFilter.ADD_TIME_TO = Convert.ToInt64(dtTo.DateTime.ToString("yyyyMMdd") + "235959");
                        }
                    }

                    switch (Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentStatus.EditValue ?? "0").ToString()))
                    {
                        case 1:
                            treatFilter.IS_IN_ROOM = true;
                            treatFilter.IS_PAUSE = false;
                            break;
                        case 2:
                            treatFilter.IS_IN_ROOM = false;
                            treatFilter.IS_PAUSE = false;
                            break;
                        case 3:
                            treatFilter.IS_PAUSE = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    treatFilter.IS_IN_ROOM = true;

                    if (cboHasCoTreatment.EditValue.ToString() == "ALL")
                    {

                    }
                    else if (cboHasCoTreatment.EditValue.ToString() == "NOCO")
                    {
                        treatFilter.HAS_CO_TREATMENT_ID = false;
                    }
                    else if (cboHasCoTreatment.EditValue.ToString() == "CO")
                    {
                        treatFilter.HAS_CO_TREATMENT_ID = true;
                    }
                }
                if ((long)cboPatientFilter.EditValue == 3)
                {
                    if (this.executeRoomSelecteds != null && this.executeRoomSelecteds.Count() > 0)
                    {
                        treatFilter.TREATMENT_ROOM_IDs = this.executeRoomSelecteds.Select(o => o.ID).Distinct().ToList();
                    }
                }

                if (this.patientClassifyFilterSelecteds != null && this.patientClassifyFilterSelecteds.Count > 0)
                {
                    if (!this.patientClassifyFilterSelecteds.Exists(o => o.ID == 0)) //không chọn Tất cả
                    {
                        treatFilter.PATIENT_CLASSIFY_IDs = this.patientClassifyFilterSelecteds.Select(k => k.ID).ToList();
                    }
                }

                if (cboTreatmentType.EditValue != null)
                {
                    long treatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentType.EditValue.ToString());
                    if (treatmentTypeId > 0)
                    {
                        treatFilter.TREATMENT_TYPE_ID = treatmentTypeId;
                    }
                }
                if (chkDuDkRavien.Checked)
                {
                    treatFilter.IS_APPROVE_FINISH = true;
                }
                if(cboEmployee.EditValue != null)
                {
                    treatFilter.DOCTOR_LOGINNAME = cboEmployee.EditValue.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadInfoPatientTotalInfo()
        {
            try
            {
                CommonParam paramCommon = new CommonParam(0, Int32.MaxValue);
                MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                if (chkKhongHienThiKTT.Checked == true)
                {
                    treatFilter.HAS_OUT_TIME = false;
                }
                var resultRO = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsyncRO<List<L_HIS_TREATMENT_BED_ROOM>>(UriApi.HIS_TREATMENT_BED_ROOM_GETLVIEW, ApiConsumers.MosConsumer, paramCommon, treatFilter, 0, null);
                if (resultRO != null && resultRO.Data != null)
                {
                    int bhytCount = resultRO.Data.Where(o => !String.IsNullOrEmpty(o.TDL_HEIN_CARD_NUMBER)).Count();
                    lblTotalPatietInfo.Text = String.Format(Resources.ResourceMessage.BHYT, bhytCount, resultRO.Param.Count);
                    lblTotalPatietInfo.ToolTip = String.Format(Resources.ResourceMessage.CoBenhNhanhBHYTTrongTongSoBenhNhan, bhytCount, resultRO.Param.Count);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectPatient(L_HIS_TREATMENT_BED_ROOM rowclickBedRoom, long treatmentId)
        {
            try
            {
                FillDataToLableControl(rowclickBedRoom);
                //ServiceReq3 OrderByDateTime Hiển thị các ngày có chỉ định dịch vụ
                LoadDataDateByTreatmentToTreeList(treatmentId);
                SetEnableButton(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                ///
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TreatmentBedRoomADO data = (TreatmentBedRoomADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        pageIndex = gridViewTreatmentBedRoom.GetRowHandle(e.ListSourceRowIndex);
                        e.Value = (pageIndex + 1 + start);
                    }
                    else if (e.Column.FieldName == "DayTreat")
                    {
                        if (data.CLINICAL_IN_TIME == null)
                            e.Value = "";
                        else
                        {
                            TimeSpan? durationTime = new TimeSpan(0, 0, 0, 0);
                            durationTime = DateTime.Now - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.CLINICAL_IN_TIME ?? 0);
                            e.Value = durationTime.Value.Days + 1;
                        }
                    }
                    else if (e.Column.FieldName == "DayHospitalize")
                    {
                        if (data.CLINICAL_IN_TIME == null)
                            e.Value = "";
                        else
                        {
                            string dayHospitalize = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CLINICAL_IN_TIME ?? 0);
                            e.Value = (System.String.Format("{0:dd/MM/yyyy hh:mm}", dayHospitalize)).Substring(0, (System.String.Format("{0:dd/MM/yyyy hh:mm}", dayHospitalize)).Length - 3);
                        }
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_DOB_Str")
                    {
                        e.Value = MPS.AgeUtil.CalculateFullAge(data.TDL_PATIENT_DOB);
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_GENDER_NAME")
                    {
                        e.Value = data.TDL_PATIENT_GENDER_NAME;
                    }
                    else if (e.Column.FieldName == "TREATMENT_ROOM_NAME_str")
                    {
                        if ((this.bedRoomFilterSelecteds != null && this.bedRoomFilterSelecteds.Count > 1))
                        {
                            e.Value = data.BED_ROOM_NAME + " (" + _TreatmentBedRoomADOs.Where(o => o.BED_ROOM_ID == data.BED_ROOM_ID).ToList().Count + ")";
                        }
                        else
                        {
                            e.Value = data.TREATMENT_ROOM_NAME + " (" + _TreatmentBedRoomADOs.Where(o => o.TREATMENT_ROOM_NAME == data.TREATMENT_ROOM_NAME).ToList().Count + ")";
                        }
                    }
                    else if (e.Column.FieldName == "IN_CODE_STR")
                    {
                        if (histreatment.FirstOrDefault(o => o.ID == data.TREATMENT_ID).IN_CODE != null)
                        {
                            e.Value = histreatment.FirstOrDefault(o => o.ID == data.TREATMENT_ID).IN_CODE;
                        }

                    }
                    if (e.Column.FieldName == "TDL_PATIENT_NAME_STR")
                    {
                        e.Value = data.TDL_PATIENT_NAME;
                    }
                    if (e.Column.FieldName == "TDL_HEIN_CARD_TIME_str")
                    {
                        e.Value = data.TDL_HEIN_CARD_FROM_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_HEIN_CARD_FROM_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_HEIN_CARD_TO_TIME ?? 0) : "";
                    }
                    if (e.Column.FieldName == "ICD_CODE_ICD_NAME")
                    {
                        e.Value = data.ICD_CODE + " - " + data.ICD_NAME;
                    }
                    if (e.Column.FieldName == "ICD_SUB_CODE_ICD_TEXT")
                    {
                        e.Value = string.Join("; ", data.ICD_SUB_CODE) + " - " + string.Join("; ", data.ICD_TEXT);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
            {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = 0;
                    if (this.bedRoomFilterSelecteds != null && this.bedRoomFilterSelecteds.Count > 1)
                    {
                        rowHandle = hi.RowHandle;
                        this.treatmentBedRoomRow = (L_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetRow(rowHandle);
                        this.RowCellClickBedRoom = (L_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetRow(rowHandle);
                    }
                    else
                    {
                        rowHandle = gridViewTreatmentBedRoom.GetVisibleRowHandle(hi.RowHandle);
                        this.treatmentBedRoomRow = (L_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetRow(rowHandle);
                        this.RowCellClickBedRoom = (L_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetRow(rowHandle);
                    }

                    this.ProcessUpdateWorkplaceByRoomWithTreatment();
                    gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }
                    if (this.emrMenuPopupProcessor == null)
                        this.emrMenuPopupProcessor = new Library.FormMedicalRecord.MediRecordMenuPopupProcessor();
                    this.bedRoomPopupMenuProcessor = new BedRoomPopupMenuProcessor(this.treatmentBedRoomRow, this.BedRoomMouseRight_Click, barManager1);
                    this.bedRoomPopupMenuProcessor.InitMenu(this.emrMenuPopupProcessor, this.wkRoomId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsLoadTreeListDateTime = false;
        private void LoadDataDateByTreatmentToTreeList(long treatmentId)
        {
            try
            {
                List<HIS_TRACKING> ListTracking = null;
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Get<List<HisServiceReqGroupByDateSDO>>(HisRequestUriStore.HIS_SERVICE_REQ_GET_GROUP_BY_DATE, ApiConsumers.MosConsumer, treatmentId, param).OrderByDescending(p => p.InstructionDate).Distinct().ToList();
                treeListDateTime.DataSource = null;
                this.rowClickByDate = new ServiceReqGroupByDateADO();
                this.IsLoadTreeListDateTime = false;
                if (rs != null && rs.Count > 0)
                {
                    HisTrackingFilter filter = new HisTrackingFilter();
                    filter.TREATMENT_IDs = rs.Select(o => o.TreatmentId).ToList();
                    ListTracking = new BackendAdapter(param).Get<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);
                    if (ListTracking != null && ListTracking.Count > 0)
                    {
                        ListTracking = ListTracking.OrderByDescending(o => o.TRACKING_TIME).Distinct().ToList();
                    }
                    List<long> listTreeListIDs = new List<long>();

                    List<ServiceReqGroupByDateADO> Result = new List<ADO.ServiceReqGroupByDateADO>();

                    foreach (var item in rs)
                    {
                        ServiceReqGroupByDateADO adoParent = new ADO.ServiceReqGroupByDateADO(item);
                        listTreeListIDs.Add(adoParent.TREELIST_ID);
                        if (adoParent.TREELIST_ID > 0)
                            Result.Add(adoParent);

                        if (ListTracking != null)
                        {
                            foreach (var tracking in ListTracking)
                            {
                                if (adoParent.InstructionDate.ToString().Substring(0, 8) == tracking.TRACKING_TIME.ToString().Substring(0, 8))
                                {
                                    ServiceReqGroupByDateADO adoChild = new ADO.ServiceReqGroupByDateADO(item, true, tracking.TRACKING_TIME, listTreeListIDs);
                                    listTreeListIDs.Add(adoChild.TREELIST_ID);
                                    adoChild.isParent = false;
                                    adoChild.InstructionDate = tracking.TRACKING_TIME;
                                    adoChild.TRACKING_ID = tracking.ID;
                                    adoChild.TRACKING_TIME = tracking.TRACKING_TIME;
                                    if (adoChild.TREELIST_ID > 0)
                                        Result.Add(adoChild);
                                }
                            }
                        }

                    }
                    IsLoadTreeListDateTime = true;
                    treeListDateTime.DataSource = Result.OrderByDescending(o => o.InstructionDate).ToList();
                    treeListDateTime.BestFitColumns();

                    this.rowClickByDate = Result[0];
                }
                IsLoadTreeListDateTime = false;
                LoadDataSereServByTreatmentId(this.rowClickByDate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public static string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                Inventec.Common.Logging.LogSystem.Error(String.Format("treatmentTypeCode {0}", new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0));
                result = ((int)((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100)) + "%";
                Inventec.Common.Logging.LogSystem.Error(String.Format("treatmentTypeCode {0} , heinCardNumber {1}, levelCode {2}, rightRouteCode {3} ", treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void FillDataToLableControl(L_HIS_TREATMENT_BED_ROOM data)
        {      

            try
            {
                bool hasAllergyCard = false;
                CommonParam param = new CommonParam();
                if (data != null)
                {
                    lblPatientCode.Text = data.TDL_PATIENT_CODE;
                    var allErgyCardFilter = new HisAllergyCardFilter();
                    allErgyCardFilter.TREATMENT_ID = data.TREATMENT_ID;
                    var allergyCards = new BackendAdapter(new CommonParam()).Get<List<HIS_ALLERGY_CARD>>("/api/HisAllergyCard/Get", ApiConsumers.MosConsumer, allErgyCardFilter, new CommonParam());
                    hasAllergyCard = (allergyCards != null && allergyCards.Any());
                    if (hasAllergyCard)
                    {
                        lblPatientCode.Appearance.Image = global::HIS.Desktop.Plugins.BedRoomPartial.Properties.Resources.thuoc;
                        lblPatientCode.ToolTip = "Bệnh nhân có thẻ dị ứng";
                        lblPatientCode.Appearance.ImageAlign = ContentAlignment.MiddleRight;
                        lblPatientCode.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.RightCenter;
                    }
                    else
                    {      
                        lblPatientCode.Appearance.Image = null;
                        lblPatientCode.ToolTip = "";        
                    }
                    lblPatientName.Text = data.TDL_PATIENT_NAME;
                    lblGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    lblDOB.Text = String.Format("{0}({1})", Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB), MPS.AgeUtil.CalculateFullAge(data.TDL_PATIENT_DOB)); ;
                    lblAdress.Text = data.TDL_PATIENT_ADDRESS;
                    lblHeinMediOrgCode.Text = data.TDL_HEIN_MEDI_ORG_CODE;

                    var _Treatment = histreatment != null && histreatment.Count > 0 ? histreatment.FirstOrDefault(o => o.ID == data.TREATMENT_ID) : null;
                    if (_Treatment != null)
                    {
                        lblIcdName.Text = _Treatment.ICD_CODE + " - " + _Treatment.ICD_NAME;
                        lblIcdText.Text = _Treatment.ICD_SUB_CODE + " - " + _Treatment.ICD_TEXT;
                        if (!String.IsNullOrEmpty(_Treatment.DOCTOR_LOGINNAME) && !String.IsNullOrEmpty(_Treatment.DOCTOR_USERNAME))
                            lbTreatDoctor.Text = _Treatment.DOCTOR_LOGINNAME + " - " + _Treatment.DOCTOR_USERNAME;
                        else
                            lbTreatDoctor.Text = _Treatment.DOCTOR_LOGINNAME + _Treatment.DOCTOR_USERNAME;
                    }

                    MOS.Filter.HisPatientTypeAlterViewFilter _alterFilter = new HisPatientTypeAlterViewFilter();
                    _alterFilter.TREATMENT_ID = data.TREATMENT_ID;
                    _alterFilter.LOG_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    var rsAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, _alterFilter, param).OrderByDescending(p => p.LOG_TIME).FirstOrDefault();
                    if (rsAlter != null && rsAlter.ID > 0)
                    {
                        lblHeinCardNumber.Text = data.TDL_HEIN_CARD_NUMBER + (!string.IsNullOrEmpty(data.TDL_HEIN_CARD_NUMBER) ? "(" + GetDefaultHeinRatioForView(data.TDL_HEIN_CARD_NUMBER, BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == data.TDL_TREATMENT_TYPE_ID).HEIN_TREATMENT_TYPE_CODE, rsAlter.LEVEL_CODE, rsAlter.RIGHT_ROUTE_CODE) + ")" : "");
                        lblTreatmentType.Text = rsAlter.PATIENT_TYPE_NAME;

                        var patientBhyt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT));
                        if (rsAlter.PATIENT_TYPE_ID == patientBhyt.ID)
                        {
                            lblTreatmentType.Text += "(" + MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteStore.GetByCode(rsAlter.RIGHT_ROUTE_CODE).HeinRightRouteName + ")";
                        }
                        if (data.TDL_PATIENT_CLASSIFY_ID != null && data.TDL_PATIENT_CLASSIFY_ID > 0)
                        {
                            List<HIS_PATIENT_CLASSIFY> dataCLASSIFY = new List<HIS_PATIENT_CLASSIFY>();
                            dataCLASSIFY.AddRange(BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE));
                            if (dataCLASSIFY.FirstOrDefault(o => o.ID == data.TDL_PATIENT_CLASSIFY_ID) != null)
                            {
                                lblTreatmentType.Text += " - " + dataCLASSIFY.FirstOrDefault(o => o.ID == data.TDL_PATIENT_CLASSIFY_ID).PATIENT_CLASSIFY_NAME;
                            }

                        }


                        if (rsAlter.PATIENT_TYPE_ID !=
                            (patientBhyt != null ? patientBhyt.ID : 0)
                           )
                        {
                            lblHeinCardNumber.Text = "";
                            lblHanTu.Text = "";
                        }
                        else
                        {
                            lblHanTu.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(rsAlter.HEIN_CARD_FROM_TIME ?? 0) + "-" + Inventec.Common.DateTime.Convert.TimeNumberToDateString(rsAlter.HEIN_CARD_TO_TIME ?? 0);
                        }
                    }
                    else
                    {
                        lblTreatmentType.Text = "";
                    }
                    if (!String.IsNullOrEmpty(data.APPROVE_FINISH_NOTE) && !String.IsNullOrEmpty(data.NOTE))
                        lblApprovalNote.Text = data.APPROVE_FINISH_NOTE + " - " + data.NOTE;
                    else if (!String.IsNullOrEmpty(data.APPROVE_FINISH_NOTE))
                        lblApprovalNote.Text = data.APPROVE_FINISH_NOTE;
                    else if (!String.IsNullOrEmpty(data.NOTE))
                        lblApprovalNote.Text = data.NOTE;
                    else
                        lblApprovalNote.Text = "";

                    lblPatientPhone.Text = data.TDL_PATIENT_PHONE ?? data.TDL_PATIENT_MOBILE;
                    lblCccd.Text = data.TDL_PATIENT_CCCD_NUMBER ?? data.TDL_PATIENT_CMND_NUMBER ?? data.TDL_PATIENT_PASSPORT_NUMBER;
                    lblTreatmentMethod.Text = data.TREATMENT_METHOD;
                    var treatmentEndType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == data.TREATMENT_END_TYPE_ID);
                    lblTreatmentEndType.Text = treatmentEndType != null ? treatmentEndType.TREATMENT_END_TYPE_NAME : "";

                    if (!String.IsNullOrEmpty(data.TDL_PATIENT_AVATAR_URL))
                    {
                        System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(data.TDL_PATIENT_AVATAR_URL);
                        pictureEditAvatar.Image = Image.FromStream(stream);
                        pictureEditAvatar.Image.Tag = data.TDL_PATIENT_AVATAR_URL;
                    }
                    else
                    {
                        string pathLocal = GetPathDefault();
                        pictureEditAvatar.Image = Image.FromFile(pathLocal);
                    }
                }
                else
                {
                    lblPatientCode.Text = null;
                    lblPatientName.Text = null;
                    lblGender.Text = null;
                    lblDOB.Text = null;
                    lblAdress.Text = null;
                    lblTreatmentType.Text = null;
                    lblHeinCardNumber.Text = null;
                    lblIcdName.Text = null;
                    lblIcdText.Text = null;
                    lblHeinMediOrgCode.Text = null;
                    lblHanTu.Text = "";
                    lblApprovalNote.Text = "";
                    lblPatientPhone.Text = null;
                    lblCccd.Text = null;
                    lblTreatmentEndType.Text = "";
                    lblTreatmentMethod.Text = "";
                    string pathLocal = GetPathDefault();
                    pictureEditAvatar.Image = Image.FromFile(pathLocal);
                }
                ProcessUpdateWorkplaceByRoomWithTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessUpdateWorkplaceByRoomWithTreatment()
        {
            try
            {
                if (RowCellClickBedRoom == null) return;
                this.currentBedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().FirstOrDefault(o => o.ID == RowCellClickBedRoom.BED_ROOM_ID);
                if (this.currentBedRoom != null && WorkPlaceWorker.UpdateWorkingRoomByTreatmentRoom(this.currentBedRoom.ROOM_ID))
                {
                    this.wkRoomId = this.currentBedRoom.ROOM_ID;
                    this.wkRoomTypeId = this.currentBedRoom.ROOM_TYPE_ID;
                    this.currentModule.RoomId = wkRoomId;
                    this.currentModule.RoomTypeId = wkRoomTypeId;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("V_HIS_BED_ROOM khong hop le hoac goi ham WorkPlaceWorker.UpdateWorkingRoomByTreatmentRoom xu ly that bai____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.wkRoomId), this.wkRoomId)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.wkRoomTypeId), this.wkRoomTypeId)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentBedRoom), this.currentBedRoom));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServByTreatmentId(ServiceReqGroupByDateADO currentHisServiceReq)
        {
            try
            {
                List<SereServADO> SereServADOs = new List<SereServADO>();
                List<DHisSereServ2> dataNew = new List<DHisSereServ2>();
                List<HIS_SERVICE_REQ> dataServiceReq = new List<HIS_SERVICE_REQ>();
                WaitingManager.Show();
                if (currentHisServiceReq != null && currentHisServiceReq.TreatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    DHisSereServ2Filter _sereServ2Filter = new DHisSereServ2Filter();
                    _sereServ2Filter.TREATMENT_ID = currentHisServiceReq.TreatmentId;
                    _sereServ2Filter.INTRUCTION_DATE = Int64.Parse(currentHisServiceReq.InstructionDate.ToString().Substring(0, 8) + "000000");
                    dataNew = new BackendAdapter(param).Get<List<DHisSereServ2>>("api/HisSereServ/GetDHisSereServ2", ApiConsumers.MosConsumer, _sereServ2Filter, param);
                    if (dataNew != null && dataNew.Count > 0)
                    {
                        if ((long)cboFilterByDepartment.EditValue == (long)0) //Theo khoa
                        {
                            dataNew = dataNew.Where(o => o.REQUEST_DEPARTMENT_ID == this.DepartmentID).ToList();
                        }

                        if (!currentHisServiceReq.isParent)
                        {
                            dataNew = dataNew.Where(o => o.TRACKING_ID == currentHisServiceReq.TRACKING_ID).ToList();
                        }
                        HisServiceReqFilter filter = new HisServiceReqFilter();
                        filter.IDs = dataNew.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        dataServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                        var listRootByType = dataNew.OrderByDescending(o => o.TRACKING_TIME).GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                        var department = currentModule != null ? BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == currentModule.RoomId) : null;
                        var departmentId = department != null ? department.DEPARTMENT_ID : 0;
                        foreach (var types in listRootByType)
                        {
                            SereServADO ssRootType = new SereServADO();
                            #region Parent
                            ssRootType.CONCRETE_ID__IN_SETY = types.First().TDL_SERVICE_TYPE_ID + "";
                            var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == types.First().TDL_SERVICE_TYPE_ID);
                            long idSerReqType = 0;
                            long idDepartment = 0;
                            long idExecuteDepartment = 0;
                            short? IsTemporaryPres = 0;
                            if (dataServiceReq != null && dataServiceReq.Count > 0)
                            {
                                if (dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID) != null && dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).ToList().Count > 0)
                                {
                                    idSerReqType = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().SERVICE_REQ_TYPE_ID;
                                    idDepartment = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().REQUEST_DEPARTMENT_ID;
                                    idExecuteDepartment = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().EXECUTE_DEPARTMENT_ID;
                                    IsTemporaryPres = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().IS_TEMPORARY_PRES;
                                }
                            }
                            ssRootType.TRACKING_TIME = types.First().TRACKING_TIME;
                            ssRootType.TDL_SERVICE_TYPE_ID = types.First().TDL_SERVICE_TYPE_ID;
                            ssRootType.SERVICE_CODE = serviceType != null ? serviceType.SERVICE_TYPE_NAME : null;
                            #endregion
                            SereServADOs.Add(ssRootType);
                            var listRootSety = types.GroupBy(g => g.SERVICE_REQ_ID).ToList();
                            foreach (var rootSety in listRootSety)
                            {
                                #region Child
                                SereServADO ssRootSety = new SereServADO();
                                ssRootSety.CONCRETE_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY + "_" + rootSety.First().SERVICE_REQ_ID;
                                //qtcode
                                if (rootSety.First().USE_TIME.HasValue)  
                                {
                                    ssRootSety.REQUEST_DEPARTMENT_NAME = string.Format("Dự trù: {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().USE_TIME.Value));
                                }
                                //qtcode
                                ssRootSety.PARENT_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY;
                                ssRootSety.REQUEST_DEPARTMENT_ID = idDepartment;
                                ssRootSety.EXECUTE_DEPARTMENT_ID = idExecuteDepartment;
                                ssRootSety.SERVICE_REQ_TYPE_ID = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(p => p.ID == idSerReqType) != null ?
                                BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(p => p.ID == idSerReqType).ID : 0;
                                ssRootSety.TRACKING_TIME = rootSety.First().TRACKING_TIME;
                                ssRootSety.SERVICE_REQ_ID = rootSety.First().SERVICE_REQ_ID;
                                ssRootSety.SERVICE_REQ_STT_ID = rootSety.First().SERVICE_REQ_STT_ID;
                                ssRootSety.TDL_SERVICE_TYPE_ID = rootSety.First().TDL_SERVICE_TYPE_ID;
                                ssRootSety.SERVICE_CODE = rootSety.First().SERVICE_REQ_CODE;
                                ssRootSety.SERVICE_REQ_CODE = rootSety.First().SERVICE_REQ_CODE;
                                ssRootSety.IS_TEMPORARY_PRES = IsTemporaryPres;
                                if (dataServiceReq != null && dataServiceReq.Count > 0)
                                {
                                    var serviceReq = dataServiceReq.FirstOrDefault(o => o.ID == rootSety.First().SERVICE_REQ_ID) ?? new HIS_SERVICE_REQ();
                                    ssRootSety.SAMPLE_TIME = serviceReq.SAMPLE_TIME;
                                    ssRootSety.RECEIVE_SAMPLE_TIME = serviceReq.RECEIVE_SAMPLE_TIME;
                                }
                                ssRootSety.TDL_TREATMENT_ID = rootSety.First().TDL_TREATMENT_ID;
                                ssRootSety.PRESCRIPTION_TYPE_ID = rootSety.First().PRESCRIPTION_TYPE_ID;
                                ssRootSety.REQUEST_LOGINNAME = rootSety.First().REQUEST_LOGINNAME;
                                ssRootSety.REQUEST_DEPARTMENT_ID = rootSety.First().REQUEST_DEPARTMENT_ID ?? 0;
                                ssRootSety.SERVICE_NAME = String.Format("- {0} - {1}", rootSety.First().REQUEST_ROOM_NAME, rootSety.First().REQUEST_DEPARTMENT_NAME);
                                var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rootSety.First().TDL_INTRUCTION_TIME ?? 0);
                                ssRootSety.NOTE_ADO = time.Substring(0, time.Count() - 3);
                                if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                                    && (rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL || HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1" || (HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "2"
                                    && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                    && rootSety.First().IS_NO_EXECUTE != 1)
                                {
                                    ssRootSety.IsEnableEdit = true;
                                }
                                if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                                  || (rootSety.First().REQUEST_DEPARTMENT_ID == departmentId && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                  && rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                {
                                    ssRootSety.IsEnableDelete = true;
                                }


                                SereServADOs.Add(ssRootSety);
                                #endregion
                                int d = 0;
                                foreach (var item in rootSety)
                                {
                                    d++;
                                    #region Child (+n)
                                    SereServADO ado = new SereServADO(item);
                                    ado.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
                                    ado.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                    if (!String.IsNullOrWhiteSpace(item.TUTORIAL))
                                    {
                                        ado.NOTE_ADO = string.Format("{0}. {1}", item.TUTORIAL, item.INSTRUCTION_NOTE);

                                    }
                                    else
                                    {
                                        ado.NOTE_ADO = string.Format("{0}", item.INSTRUCTION_NOTE);
                                    }
                                    ado.AMOUNT_SER = string.Format("{0} - {1}", item.AMOUNT, item.SERVICE_UNIT_NAME);
                                    ado.IS_TEMPORARY_PRES = IsTemporaryPres;
                                    SereServADOs.Add(ado);
                                    #endregion
                                }
                            }
                        }
                    }
                }

                WaitingManager.Hide();
                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenBy(p => p.SERVICE_CODE).ThenBy(o => o.SERVICE_NAME).ToList();

                    #region ALL

                    ucAll.ReLoad(treeView_Click, GroupDataByTracking(dataNew, dataServiceReq), this.RowCellClickBedRoom, Edit_Click, Delete_Click);
                    #endregion

                    #region CLS

                    List<SereServADO> listCLS = new List<SereServADO>();
                    listCLS.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                        ));

                    ucCLS.ReLoad(treeView_Click, listCLS, this.RowCellClickBedRoom, Edit_Click, Delete_Click);

                    #endregion

                    #region MediMate

                    List<SereServADO> listMediMate = new List<SereServADO>();
                    listMediMate.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                        ));

                    ucMediMate.ReLoad(treeView_Click, listMediMate, this.RowCellClickBedRoom, Edit_Click, Delete_Click);

                    #endregion

                    #region Orther

                    List<SereServADO> listOther = new List<SereServADO>();
                    listOther.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                        ));

                    ucOrther.ReLoad(treeView_Click, listOther, this.RowCellClickBedRoom, Edit_Click, Delete_Click);

                    #endregion

                    #region reloadTabControl
                    IsExpandList = true;
                    btnThuGon.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__BTN__THU_GON", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); //"Thu gọn";                                       

                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[3];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[2];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[1];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[0];
                    #endregion

                }
                else
                {
                    ucAll.ReLoad(treeView_Click, null, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
                    ucCLS.ReLoad(treeView_Click, null, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
                    ucMediMate.ReLoad(treeView_Click, null, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
                    ucOrther.ReLoad(treeView_Click, null, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<SereServADO> GroupDataByTracking(List<DHisSereServ2> dataNew, List<HIS_SERVICE_REQ> dataServiceReq)
        {
            List<SereServADO> SereServADOs = new List<ADO.SereServADO>();
            try
            {
                var departmentId = BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == currentModule.RoomId).DEPARTMENT_ID;
                var listRootByTracking = dataNew.OrderByDescending(o => o.TRACKING_TIME).GroupBy(o => o.TRACKING_TIME).ToList();
                foreach (var tracking in listRootByTracking)
                {
                    #region GrandFather
                    SereServADO ssRootTrackingTime = new ADO.SereServADO();
                    ssRootTrackingTime.CONCRETE_ID__IN_SETY = tracking.First().TRACKING_TIME + "_";
                    string dayHospitalize = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(tracking.First().TRACKING_TIME ?? 0);
                    ssRootTrackingTime.SERVICE_CODE = !string.IsNullOrEmpty(dayHospitalize) ? (System.String.Format("{0:dd/MM/yyyy HH:mm}", dayHospitalize)).Substring(0, (System.String.Format("{0:dd/MM/yyyy HH:mm}", dayHospitalize)).Length - 3) : "Chưa tạo tờ điều trị";
                    SereServADOs.Add(ssRootTrackingTime);
                    int count = 0;
                    #endregion
                    var listRootType = tracking.GroupBy(g => g.TDL_SERVICE_TYPE_ID).ToList();
                    foreach (var types in listRootType)
                    {
                        #region Parent
                        count++;
                        SereServADO ssRootType = new SereServADO();
                        ssRootType.CONCRETE_ID__IN_SETY = ssRootTrackingTime.CONCRETE_ID__IN_SETY + "_" + types.First().TRACKING_TIME + "_" + count;
                        ssRootType.PARENT_ID__IN_SETY = ssRootTrackingTime.CONCRETE_ID__IN_SETY;
                        var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == types.First().TDL_SERVICE_TYPE_ID);
                        long idSerReqType = 0;
                        long idDepartment = 0;
                        long idExecuteDepartment = 0;
                        short? IsTemporaryPres = 0;
                        if (dataServiceReq != null && dataServiceReq.Count > 0)
                        {
                            if (dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID) != null && dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).ToList().Count > 0)
                            {
                                idSerReqType = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().SERVICE_REQ_TYPE_ID;
                                idDepartment = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().REQUEST_DEPARTMENT_ID;
                                idExecuteDepartment = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().EXECUTE_DEPARTMENT_ID;
                                IsTemporaryPres = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().IS_TEMPORARY_PRES;
                            }
                        }
                        ssRootType.TRACKING_TIME = types.First().TRACKING_TIME;
                        ssRootType.TDL_SERVICE_TYPE_ID = types.First().TDL_SERVICE_TYPE_ID;
                        ssRootType.SERVICE_CODE = serviceType != null ? serviceType.SERVICE_TYPE_NAME : null;
                        #endregion
                        SereServADOs.Add(ssRootType);
                        var listRootSety = types.GroupBy(g => g.SERVICE_REQ_ID).ToList();
                        foreach (var rootSety in listRootSety)
                        {
                            #region Child
                            SereServADO ssRootSety = new SereServADO();
                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY + "_" + rootSety.First().SERVICE_REQ_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY;
                            ssRootSety.REQUEST_DEPARTMENT_ID = idDepartment;
                            ssRootSety.EXECUTE_DEPARTMENT_ID = idExecuteDepartment;
                            ssRootSety.SERVICE_REQ_TYPE_ID = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(p => p.ID == idSerReqType) != null ?
                            BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(p => p.ID == idSerReqType).ID : 0;
                            //qtcode
                            if (rootSety.First().USE_TIME.HasValue)
                            {
                                ssRootSety.REQUEST_DEPARTMENT_NAME = string.Format("Dự trù: {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().USE_TIME.Value));
                            }
                            //qtcode
                            ssRootSety.TRACKING_TIME = rootSety.First().TRACKING_TIME;
                            ssRootSety.SERVICE_REQ_ID = rootSety.First().SERVICE_REQ_ID;
                            ssRootSety.SERVICE_REQ_STT_ID = rootSety.First().SERVICE_REQ_STT_ID;
                            ssRootSety.TDL_SERVICE_TYPE_ID = rootSety.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.PRESCRIPTION_TYPE_ID = rootSety.First().PRESCRIPTION_TYPE_ID;
                            ssRootSety.TDL_TREATMENT_ID = rootSety.First().TDL_TREATMENT_ID;
                            ssRootSety.REQUEST_LOGINNAME = rootSety.First().REQUEST_LOGINNAME;
                            ssRootSety.REQUEST_DEPARTMENT_ID = rootSety.First().REQUEST_DEPARTMENT_ID ?? 0;
                            ssRootSety.SERVICE_CODE = rootSety.First().SERVICE_REQ_CODE;
                            ssRootSety.SERVICE_REQ_CODE = rootSety.First().SERVICE_REQ_CODE;
                            ssRootSety.IS_TEMPORARY_PRES = IsTemporaryPres;
                            if (dataServiceReq != null && dataServiceReq.Count > 0)
                            {
                                var serviceReq = dataServiceReq.FirstOrDefault(o => o.ID == rootSety.First().SERVICE_REQ_ID) ?? new HIS_SERVICE_REQ();
                                ssRootSety.SAMPLE_TIME = serviceReq.SAMPLE_TIME;
                                ssRootSety.RECEIVE_SAMPLE_TIME = serviceReq.RECEIVE_SAMPLE_TIME;
                            }
                            ssRootSety.SERVICE_NAME = String.Format("- {0} - {1}", rootSety.First().REQUEST_ROOM_NAME, rootSety.First().REQUEST_DEPARTMENT_NAME);
                            var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rootSety.First().TDL_INTRUCTION_TIME ?? 0);
                            ssRootSety.NOTE_ADO = time.Substring(0, time.Count() - 3);
                           
                            if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                                    && (rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL || HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1" || (HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "2"
                                    && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                    && rootSety.First().IS_NO_EXECUTE != 1)
                            {
                                ssRootSety.IsEnableEdit = true;
                            }
                            if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                              || (rootSety.First().REQUEST_DEPARTMENT_ID == departmentId && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                              && rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            {
                                ssRootSety.IsEnableDelete = true;
                            }

                            SereServADOs.Add(ssRootSety);
                            #endregion
                            int d = 0;
                            foreach (var item in rootSety)
                            {
                                d++;
                                #region Child (+n)
                                SereServADO ado = new SereServADO(item);
                                ado.IS_TEMPORARY_PRES = IsTemporaryPres;
                                ado.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
                                ado.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                ado.child = 4;

                                if (!String.IsNullOrWhiteSpace(item.TUTORIAL))
                                {
                                    ado.NOTE_ADO = string.Format("{0}. {1}", item.TUTORIAL, item.INSTRUCTION_NOTE);

                                }
                                else
                                {
                                    ado.NOTE_ADO = string.Format("{0}", item.INSTRUCTION_NOTE);
                                }

                                ado.AMOUNT_SER = string.Format("{0} - {1}", item.AMOUNT, item.SERVICE_UNIT_NAME);
                                SereServADOs.Add(ado);
                                #endregion
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                SereServADOs = new List<ADO.SereServADO>();
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return SereServADOs;
        }

        private void Delete_Click(SereServADO data)
        {
            try
            {
                if (data != null && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") == "1")
                {
                    CommonParam paramEmr = new CommonParam();
                    CommonParam param = new CommonParam();
                    bool success = false;

                    EMR.Filter.EmrDocumentFilter filter = new EMR.Filter.EmrDocumentFilter();
                    filter.TREATMENT_CODE__EXACT = this.treatmentCode;
                    filter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN;
                    var resultEmrDocument = new BackendAdapter(paramEmr).Get<List<EMR_DOCUMENT>>("api/EmrDocument/Get", ApiConsumers.EmrConsumer, filter, paramEmr);
                    if (resultEmrDocument != null && resultEmrDocument.Count() > 0)
                    {
                        resultEmrDocument = resultEmrDocument.Where(o => o.IS_DELETE != 1).ToList();
                        var checkServiceReqCode = "SERVICE_REQ_CODE:" + data.SERVICE_CODE;
                        var resultEmrDocumentLast = new List<EMR_DOCUMENT>();
                        foreach (var item in resultEmrDocument)
                        {
                            if (item.HIS_CODE != null && item.HIS_CODE.Contains(checkServiceReqCode))
                            {
                                resultEmrDocumentLast.Add(item);
                            }
                        }
                        if (resultEmrDocumentLast.Count() > 0 && resultEmrDocumentLast != null)
                        {
                            #region
                            if (MessageBox.Show(Resources.ResourceMessage.YLenhDaTonTaiVanBanKy, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                WaitingManager.Show();
                                MOS.SDO.HisServiceReqSDO sdoHisServiceReq = new MOS.SDO.HisServiceReqSDO();
                                sdoHisServiceReq.Id = data.SERVICE_REQ_ID;
                                sdoHisServiceReq.RequestRoomId = this.currentModule.RoomId;
                                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisServiceReq/Delete", ApiConsumers.MosConsumer, sdoHisServiceReq, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                                WaitingManager.Hide();
                                if (success == true)
                                {
                                    var result = false;
                                    foreach (var item in resultEmrDocumentLast)
                                    {
                                        result = new BackendAdapter(paramEmr).Post<bool>("api/EmrDocument/Delete", ApiConsumers.EmrConsumer, item.ID, paramEmr);
                                    }
                                    MessageManager.Show(this.ParentForm, paramEmr, result);
                                    LoadDataSereServByTreatmentId(this.rowClickByDate);
                                }
                                else
                                {
                                    MessageManager.Show(this.ParentForm, param, success);
                                }

                                #region Process has exception
                                SessionManager.ProcessTokenLost(param);
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            ProcessDeleteServiceReq(data);
                        }
                    }
                    else
                    {
                        ProcessDeleteServiceReq(data);
                    }
                }
                else
                {
                    ProcessDeleteServiceReq(data);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDeleteServiceReq(ADO.SereServADO data)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam paramCommon = new CommonParam();
                MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                sdo.Id = data.SERVICE_REQ_ID;
                sdo.RequestRoomId = this.currentModule.RoomId;
                success = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<bool>("api/HisServiceReq/Delete", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (success)
                {
                    LoadDataSereServByTreatmentId(this.rowClickByDate);
                    if(data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                    {
                        FillDataToGridTreatmentBedRoom();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, paramCommon, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Edit_Click(SereServADO currentSS)
        {
            try
            {
                if (currentSS != null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = currentSS.TDL_TREATMENT_ID;
                    var dtTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);

                    if (dtTreatment != null && dtTreatment.Count > 0)
                    {
                        if (dtTreatment[0].IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE || dtTreatment[0].IS_PAUSE == 1)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(
                           Resources.ResourceMessage.HoSoDieuTriDangTamKhoa,
                           Resources.ResourceMessage.ThongBao,
                           MessageBoxButtons.OK);
                            return;
                        }
                    }
                    HisServiceReqFilter sfilter = new HisServiceReqFilter();
                    sfilter.ID = currentSS.SERVICE_REQ_ID;
                    var dtServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, sfilter, param);

                    if (currentSS.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                    {
                        WaitingManager.Show();
                        List<object> sendObj = new List<object>() { currentSS.SERVICE_REQ_ID };
                        CallModule("HIS.Desktop.Plugins.UpdateExamServiceReq", sendObj);
                        WaitingManager.Hide();
                    }
                    else if ((currentSS.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || currentSS.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT) && IsCheckDepartmentTran())
                    {
                        AssignPrescriptionEditADO assignEditADO = null;
                        HisExpMestFilter expfilter = new HisExpMestFilter();
                        expfilter.SERVICE_REQ_ID = dtServiceReq != null && dtServiceReq.Count > 0 ? dtServiceReq[0].ID : 0;
                        var expMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (expMests != null && expMests.Count == 1)
                        {
                            var expMest = expMests.FirstOrDefault();
                            if (expMest.IS_NOT_TAKEN.HasValue && expMest.IS_NOT_TAKEN.Value == 1)
                            {
                                WaitingManager.Hide();
                                MessageBox.Show(Resources.ResourceMessage.DonKhongLayKhongChoPhepSua);
                                return;
                            }
                            assignEditADO = new AssignPrescriptionEditADO(dtServiceReq[0], expMest, FillDataApterSave);
                        }
                        else
                        {
                            assignEditADO = new AssignPrescriptionEditADO(dtServiceReq[0], null, FillDataApterSave);
                        }

                        var assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(currentSS.TDL_TREATMENT_ID ?? 0, 0, currentSS.SERVICE_REQ_ID ?? 0);
                        if (dtTreatment != null && dtTreatment.Count > 0)
                        {
                            assignServiceADO.GenderName = dtTreatment[0].TDL_PATIENT_GENDER_NAME;
                            assignServiceADO.PatientDob = dtTreatment[0].TDL_PATIENT_DOB;
                            assignServiceADO.PatientName = dtTreatment[0].TDL_PATIENT_NAME;
                        }

                        assignServiceADO.AssignPrescriptionEditADO = assignEditADO;

                        List<object> sendObj = new List<object>() { assignServiceADO };

                        if (currentSS.PRESCRIPTION_TYPE_ID == 1)
                        {
                            string moduleLink = "HIS.Desktop.Plugins.AssignPrescriptionPK";
                            CallModule(moduleLink, sendObj);

                        }
                        else if (currentSS.PRESCRIPTION_TYPE_ID == 2)
                        {
                            CallModule("HIS.Desktop.Plugins.AssignPrescriptionYHCT", sendObj);
                        }
                    }
                    else if (currentSS.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM && IsCheckDepartmentTran())
                    {

                        HIS.Desktop.ADO.AssignBloodADO assignBloodADO = new HIS.Desktop.ADO.AssignBloodADO(currentSS.TDL_TREATMENT_ID ?? 0, 0, 0);
                        if (dtTreatment != null && dtTreatment.Count > 0)
                        {
                            assignBloodADO.PatientDob = dtTreatment[0].TDL_PATIENT_DOB;
                            assignBloodADO.PatientName = dtTreatment[0].TDL_PATIENT_NAME;
                            assignBloodADO.GenderName = dtTreatment[0].TDL_PATIENT_GENDER_NAME;
                        }
                        List<object> sendObj = new List<object>() { assignBloodADO, dtServiceReq };
                        CallModule("HIS.Desktop.Plugins.HisAssignBlood", sendObj);
                    }
                    else if (IsCheckDepartmentTran())
                    {
                        AssignServiceEditADO assignServiceEditADO = new AssignServiceEditADO(currentSS.SERVICE_REQ_ID ?? 0, dtServiceReq[0].INTRUCTION_TIME, (HIS.Desktop.Common.RefeshReference)RefreshClick);
                        List<object> sendObj = new List<object>() { assignServiceEditADO };
                        CallModule("HIS.Desktop.Plugins.AssignServiceEdit", sendObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshClick()
        {
            try
            {
                WaitingManager.Show();
                LoadDataSereServByTreatmentId(this.rowClickByDate);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterSave(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    WaitingManager.Show();
                    LoadDataSereServByTreatmentId(this.rowClickByDate);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallModule(string moduleLink, List<object> data)
        {
            try
            {
                CallModule callModule = new CallModule(moduleLink, this.wkRoomId, this.wkRoomTypeId, data);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeView_Click(SereServADO data)
        {
            try
            {
                if (data != null)
                {
                    TreeClickData = data;
                    if (TreeClickData != null && !String.IsNullOrWhiteSpace(TreeClickData.SERVICE_REQ_CODE))
                    {
                        ProcessLoadDocumentBySereServ(TreeClickData);
                    }
                    else
                    {
                        this.ucViewEmrDocumentReq.ReloadDocument(new List<V_EMR_DOCUMENT>());
                        this.ucViewEmrDocumentResult.ReloadDocument(new List<V_EMR_DOCUMENT>());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListDateTime_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            try
            {
                this.rowClickByDate = new ServiceReqGroupByDateADO();
                this.rowClickByDate = (ServiceReqGroupByDateADO)treeListDateTime.GetDataRecordByNode(treeListDateTime.FocusedNode);
                //IsLoadTreeListDateTime: Khi load source vào tree đang tự động focus dẫn tới gọi API Dhissereserv2 2 lần
                if (this.rowClickByDate != null && !IsLoadTreeListDateTime)
                {
                    //Reload tree
                    LoadDataSereServByTreatmentId(this.rowClickByDate);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void LogThreadSessionSearch()
        {   
            try
            {
                if ((dtTo.DateTime.Date - dtFrom.DateTime.Date).TotalDays > 90)
                {
                    XtraMessageBox.Show("Khoảng thời gian tìm kiếm quá dài, vui lòng chọn tối đa 3 tháng!",
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dtTo.DateTime.Date > DateTime.Now)
                {
                    XtraMessageBox.Show("Khoảng thời gian tìm kiếm quá dài, vui lòng chọn tối đa 3 tháng!",
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtTo.EditValue = DateTime.Now;
                }
                if (dtFrom.DateTime.Date > DateTime.Now)
                {
                    dtFrom.EditValue = DateTime.Now;
                }
                treeListDateTime.DataSource = null;
                LoadDataSereServByTreatmentId(null);
                FillDataToLableControl(null);

                FillDataToGridTreatmentBedRoom();
                SetEnableButton(false);
                LoadInfoPatientTotalInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LogTheadInSessionInfo(LogThreadSessionSearch, "btnSearch_Click");
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
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButton(bool isKey)
        {
            try
            {
                btnChiDinhGPBL.Enabled = isKey;
                btnChiDinhDichVu.Enabled = isKey;
                btnKeDonThuoc.Enabled = isKey;
                btnChiDinhMau.Enabled = isKey;

                if (this.RowCellClickBedRoom != null && this.RowCellClickBedRoom.CO_TREATMENT_ID != null)
                {
                    if (isKey)
                    {
                        btnChuyenKhoa.Enabled = !isKey;
                        btnKetThucDieuTri.Enabled = !isKey;
                    }
                    else
                    {
                        btnChuyenKhoa.Enabled = isKey;
                        btnKetThucDieuTri.Enabled = isKey;
                    }
                }
                else
                {
                    btnChuyenKhoa.Enabled = isKey;
                    btnKetThucDieuTri.Enabled = isKey;
                }

                btnBangKe.Enabled = isKey;
                btnDanhSachYeuCau.Enabled = isKey;
                btnTuTruc.Enabled = isKey;
                btnInToDieuTri.Enabled = isKey;
                btnHoiChan.Enabled = isKey;
                btnKeDonYHCT.Enabled = isKey;
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

        private void RefeshData()
        {
            try
            {
                LoadDataSereServByTreatmentId(rowClickByDate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void keyF2Focused()
        {
            try
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnKeDonYHCT_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null && IsCheckDepartmentTran())
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionYHCT").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionYHCT");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(RowCellClickBedRoom.TREATMENT_ID, 0, 0);
                        assignServiceADO.PatientDob = RowCellClickBedRoom.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = RowCellClickBedRoom.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = RowCellClickBedRoom.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.TreatmentCode = RowCellClickBedRoom.TREATMENT_CODE;
                        assignServiceADO.TreatmentId = RowCellClickBedRoom.TREATMENT_ID;
                        listArgs.Add(assignServiceADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBedHistory_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BedHistory").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.BedHistory");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(RowCellClickBedRoom);
                        listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                        //listArgs.Add((RefeshReference)Refesh);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool OrderBy = true;

        private void gridViewTreatmentBedRoom_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridHitInfo hitInfo = gridViewTreatmentBedRoom.CalcHitInfo(e.Location);
                if (hitInfo.InColumnPanel && hitInfo.Column.FieldName == "ADD_TIME_STR")
                {
                    //do something
                    WaitingManager.Show();
                    if (_TreatmentBedRoomADOs != null && _TreatmentBedRoomADOs.Count > 0)
                    {
                        if (OrderBy)
                        {
                            _TreatmentBedRoomADOs = _TreatmentBedRoomADOs.OrderBy(p => p.ADD_TIME).ToList();
                            OrderBy = false;
                        }
                        else
                        {
                            _TreatmentBedRoomADOs = _TreatmentBedRoomADOs.OrderByDescending(p => p.ADD_TIME).ToList();
                            OrderBy = true;
                        }
                    }

                    gridControlTreatmentBedRoom.DataSource = null;
                    gridControlTreatmentBedRoom.BeginUpdate();
                    gridControlTreatmentBedRoom.DataSource = _TreatmentBedRoomADOs;
                    gridControlTreatmentBedRoom.EndUpdate();
                    gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedCell = false;
                    gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedRow = false;
                    gridViewTreatmentBedRoom.BestFitColumns();

                    this.RowCellClickBedRoom = null;
                    LoadDataSereServByTreatmentId(null);
                    lblPatientCode.Text = null;
                    lblPatientName.Text = null;
                    lblGender.Text = null;
                    lblDOB.Text = null;
                    lblAdress.Text = null;
                    lblTreatmentType.Text = null;
                    lblHeinCardNumber.Text = null;
                    lblIcdName.Text = null;
                    lblIcdText.Text = null;
                    lblHeinMediOrgCode.Text = null;
                    lblHanTu.Text = "";

                    SetEnableButton(false);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);
                string DISPLAY_COLOR = (vw.GetRowCellValue(e.RowHandle, "DISPLAY_COLOR") ?? "").ToString();
                if (!string.IsNullOrWhiteSpace(DISPLAY_COLOR))
                {
                    List<int> parentBackColorCodes = GetColorValues(DISPLAY_COLOR);
                    if (parentBackColorCodes != null && parentBackColorCodes.Count >= 3)
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                    }
                }
                else
                    e.Appearance.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<int> GetColorValues(string rgbCode)
        {
            List<int> result = new List<int>();
            try
            {
                if (!String.IsNullOrWhiteSpace(rgbCode))
                {
                    result = new List<int>();
                    string[] Codes = rgbCode.Split(',');
                    foreach (var item in Codes)
                    {
                        result.Add(Inventec.Common.TypeConvert.Parse.ToInt32(item));
                    }

                    if (result.Count < 3)
                    {
                        int rsCount = result.Count;
                        while (rsCount < 4)
                        {
                            rsCount++;
                            result.Add(0);
                        }
                    }
                }
                else
                {
                    result.Add(0);
                    result.Add(0);
                    result.Add(0);
                }
            }
            catch (Exception ex)
            {
                //màu đen
                result = new List<int>();
                result.Add(0);
                result.Add(0);
                result.Add(0);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void repositoryItemBtnMedisoft_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                TreatmentBedRoomADO row = (TreatmentBedRoomADO)gridViewTreatmentBedRoom.GetFocusedRow();
                if (row != null)
                {
                    InitDataADO ado = new InitDataADO();
                    ado.ProviderType = ProviderType.Medisoft;
                    ado.PatientId = row.PATIENT_ID;
                    OtherTreatmentHistoryProcessor history = new OtherTreatmentHistoryProcessor(ado);
                    if (history != null)
                    {
                        history.Run(Library.OtherTreatmentHistory.Enum.XemCanLamSan);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "ICON")
                    {
                        e.RepositoryItem = repositoryItemButton_White;
                        long? dtCo = (long?)view.GetRowCellValue(e.RowHandle, "CO_TREATMENT_ID");
                        if (dtCo != null)
                        {
                            e.RepositoryItem = repositoryItemButton_Come;
                        }
                        else
                        {
                            bool check = false;
                            string dtCoIds = (string)view.GetRowCellValue(e.RowHandle, "CO_TREAT_DEPARTMENT_IDS");
                            if (!string.IsNullOrEmpty(dtCoIds))
                            {
                                List<string> lstCoTreatDepartmentId = new List<string>();
                                lstCoTreatDepartmentId = dtCoIds.Split(',').ToList();
                                if (lstCoTreatDepartmentId != null && lstCoTreatDepartmentId.Count > 0)
                                {
                                    foreach (var item in lstCoTreatDepartmentId)
                                    {
                                        if (!String.IsNullOrEmpty(item) && long.Parse(item) != this.DepartmentID)
                                        {
                                            check = true;
                                        }
                                    }
                                }
                                e.RepositoryItem = (check == true ? repositoryItemButton_Leave : repositoryItemButton_White);
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButton_White;
                            }
                        }

                    }
                    if (e.Column.FieldName == "IS_EMERGENCY_STR")
                    {

                        if (histreatment != null && histreatment.Count > 0)
                        {
                            long dtTre = (long)view.GetRowCellValue(e.RowHandle, "TREATMENT_ID");
                            if (histreatment.FirstOrDefault(o => o.ID == dtTre).IS_EMERGENCY == 1)
                            {
                                e.RepositoryItem = repositoryItemButtonIS_EMERGENCY_STR;
                            }
                        }
                    }
                    if (e.Column.FieldName == "TDL_PATIENT_NAME")
                    {

                        var dtApp = view.GetRowCellValue(e.RowHandle, "IS_APPROVE_FINISH") ?? "";
                        if (dtApp.ToString() == "1")
                        {
                            e.RepositoryItem = repositoryItemTextEditTDL_PATIENT_NAME_STR;
                        }
                        else
                        {

                            e.RepositoryItem = repositoryItemTextEditTDL_PATIENT_NAME_STR_NO;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlTreatmentBedRoom)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlTreatmentBedRoom.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";

                            if (info.Column.FieldName == "ICON")
                            {
                                long? dtCo = (long?)view.GetRowCellValue(info.RowHandle, "CO_TREATMENT_ID");
                                if (dtCo != null)
                                {
                                    string dtLastDepartment = (string)view.GetRowCellValue(info.RowHandle, "LAST_DEPARTMENT_NAME");
                                    if (!string.IsNullOrEmpty(dtLastDepartment))
                                    {
                                        text = String.Format(ResourceMessage.ToolTipCome, dtLastDepartment);
                                    }
                                }
                                else
                                {
                                    string dtTreatIds = (string)view.GetRowCellValue(info.RowHandle, "CO_TREAT_DEPARTMENT_IDS");
                                    if (!string.IsNullOrEmpty(dtTreatIds))
                                    {
                                        string Leave = "";

                                        List<string> lstCoTreatDepartmentId = new List<string>();
                                        lstCoTreatDepartmentId = dtTreatIds.Split(',').ToList();

                                        if (lstCoTreatDepartmentId != null && lstCoTreatDepartmentId.Count > 0)
                                        {
                                            foreach (var item in lstCoTreatDepartmentId)
                                            {
                                                Leave += " " + BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == long.Parse(item)).DEPARTMENT_NAME + ",";
                                            }

                                            Leave = Leave.Substring(0, (Leave.Length - 1));

                                            text = ResourceMessage.ToolTipLeaves + Leave;
                                        }

                                    }
                                }
                            }
                            if (info.Column.FieldName == "IS_EMERGENCY_STR")
                            {
                                text = Resources.ResourceMessage.BenhNhanCapCuu;
                            }
                            if (info.Column.FieldName == "TDL_PATIENT_NAME")
                            {
                                var dtApp = view.GetRowCellValue(info.RowHandle, "IS_APPROVE_FINISH") ?? "";
                                if (dtApp.ToString() == "1")
                                {
                                    text = Resources.ResourceMessage.BenhNhanDaDuDieuKienRaVien;
                                }

                            }
                            lastInfo = new DevExpress.Utils.ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
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

        private void xtraTabDocument_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (TreeClickData != null && !String.IsNullOrWhiteSpace(TreeClickData.SERVICE_REQ_CODE))
                {
                    ProcessLoadDocumentBySereServ(TreeClickData);
                }
                else
                {
                    this.ucViewEmrDocumentReq.ReloadDocument(new List<V_EMR_DOCUMENT>());
                    this.ucViewEmrDocumentResult.ReloadDocument(new List<V_EMR_DOCUMENT>());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<EmrDocumentFileSDO> GetEmrDocumentFile(string hiscode, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark, ref CommonParam paramCommon)
        {
            EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
            var emrFilter = new EMR.Filter.EmrDocumentViewFilter(); 
            emrFilter.TREATMENT_CODE__EXACT = this.RowCellClickBedRoom.TREATMENT_CODE;
            emrFilter.IS_DELETE = false;
            if (xtraTabDocument.SelectedTabPage == xtraTabDocumentReq)
            {
                emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN;
            }
            else if (xtraTabDocument.SelectedTabPage == xtraTabDocumentResult)
            {
                emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
            }
            sdo.HisCode = hiscode;
            sdo.EmrDocumentViewFilter = emrFilter;
            sdo.IsMerge = IsMerge;
            sdo.IsShowPatientSign = IsShowPatientSign;
            sdo.IsShowWatermark = IsShowWatermark;
            var roomWorking = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
            sdo.RoomCode = roomWorking != null ? roomWorking.ROOM_CODE : null;
            sdo.DepartmentCode = roomWorking != null ? roomWorking.DEPARTMENT_CODE : null;
            return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
        }
        private void ProcessLoadDocumentBySereServ(DHisSereServ2 data)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") != "1")
                    return;
                WaitingManager.Show();
                List<EmrDocumentFileSDO> listData = new List<EmrDocumentFileSDO>();
                if (data != null)
                {
                    string hisCode = "SERVICE_REQ_CODE:" + data.SERVICE_REQ_CODE;
                    CommonParam paramCommon = new CommonParam();
                    listData = GetEmrDocumentFile(hisCode, true, null, null, ref paramCommon);
                    if (listData != null && listData.Count > 0)
                    {
                        listData = listData.Where(o => o.Extension.ToLower().Equals("pdf")).ToList();
                    }
                }

                if (xtraTabDocument.SelectedTabPage == xtraTabDocumentReq)
                {
                    this.ucViewEmrDocumentReq.ReloadDocument(listData);
                }
                else if (xtraTabDocument.SelectedTabPage == xtraTabDocumentResult)
                {
                    this.ucViewEmrDocumentResult.ReloadDocument(listData);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == LciGroupEmrDocument1.Name)
                        {
                            LciGroupEmrDocument1.Expanded = item.VALUE == "1";
                        }
                        if (item.KEY == "chkKhongHienThiKTT")
                        {
                            chkKhongHienThiKTT.Checked = item.VALUE == "1";
                        }
                    }
                }

                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void layoutControlTreeSereServ_GroupExpandChanged(object sender, DevExpress.XtraLayout.Utils.LayoutGroupEventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                string name = e.Group.Name;
                string value = "";

                if (e.Group.Name == LciGroupEmrDocument1.Name)
                {
                    value = LciGroupEmrDocument1.Expanded ? "1" : null;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = name;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                if (this.currentControlStateRDO != null)
                {
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientFilter_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                layoutControlItem34.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem35.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if ((long)cboPatientFilter.EditValue == (long)0)
                {
                    dtTo.Enabled = false;
                    dtFrom.Enabled = false;
                    cboTreatmentStatus.EditValue = null;
                    cboTreatmentStatus.Enabled = false;
                    dtTo.Text = "";
                    dtFrom.Text = "";
                    this.isUseAddedTime = false;
                }
                else if ((long)cboPatientFilter.EditValue == 3)
                {
                    layoutControlItem34.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem35.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dtTo.Enabled = false;
                    dtFrom.Enabled = false;
                    cboTreatmentStatus.EditValue = null;
                    cboTreatmentStatus.Enabled = false;
                    dtTo.Text = "";
                    dtFrom.Text = "";
                    this.isUseAddedTime = false;
                }
                else
                {
                    dtTo.Enabled = true;
                    dtFrom.Enabled = true;
                    cboTreatmentStatus.EditValue = 0;
                    cboTreatmentStatus.Enabled = true;
                    dtFrom.DateTime = DateTime.Now;
                    dtTo.DateTime = DateTime.Now;
                    this.isUseAddedTime = true;
                }
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboFilterByDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Immediate || e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    LoadDataSereServByTreatmentId(this.rowClickByDate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnThuGon_Click(object sender, EventArgs e)
        {
            try
            {
                if (ucAll != null && ucMediMate != null && ucCLS != null && ucOrther != null)
                {
                    ucAll.Expand(!IsExpandList);
                    ucMediMate.Expand(!IsExpandList);
                    ucCLS.Expand(!IsExpandList);
                    ucOrther.Expand(!IsExpandList);
                    if (ucAll.getExpand())
                    {
                        btnThuGon.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__BTN__THU_GON", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); //"Thu gọn";

                    }
                    else
                    {
                        btnThuGon.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__BTN__CHI_TIET", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());// "Chi tiết";

                    }
                    IsExpandList = !IsExpandList;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_ROOM> GetBedRoomFilter()
        {
            List<V_HIS_ROOM> list = new List<V_HIS_ROOM>();
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var userRoomByUserIds = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == loginName && (o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)).Select(o => o.ROOM_ID).ToList();

                var roomWorking = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                list = (userRoomByUserIds != null && userRoomByUserIds.Count > 0) ? BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG
                    && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && roomWorking != null
                    && userRoomByUserIds.Contains(o.ID)
                    && o.DEPARTMENT_ID == roomWorking.DEPARTMENT_ID).ToList() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
            return list;
        }

        private void InitComboBedRoom()
        {
            try
            {
                this.bedRoomAlls = GetBedRoomFilter();
                cboBedRoomSelect.Properties.DataSource = this.bedRoomAlls;
                cboBedRoomSelect.Properties.DisplayMember = "ROOM_NAME";
                cboBedRoomSelect.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn column = cboBedRoomSelect.Properties.View.Columns.AddField("ROOM_NAME");
                column.VisibleIndex = 1;
                column.Width = 200;
                column.Caption = Resources.ResourceMessage.TatCa;
                cboBedRoomSelect.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboBedRoomSelect.Properties.View.OptionsSelection.MultiSelect = true;
                if (bedRoomAlls != null && bedRoomAlls.Count > 0)
                {
                    string optionRoom = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ChooseRoom.GroupRoomOption");

                    List<V_HIS_ROOM> roomslt = new List<V_HIS_ROOM>();
                    if (optionRoom == "1")
                    {
                        roomslt = this.bedRoomAlls;
                    }
                    else
                    {
                        roomslt = this.bedRoomAlls.Where(o => o.ID == currentModule.RoomId).ToList();
                    }

                    GridCheckMarksSelection gridCheckMark = cboBedRoomSelect.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null && roomslt != null && roomslt.Count > 0)
                    {
                        gridCheckMark.SelectAll(roomslt);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBedRoomCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboBedRoomSelect.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_BedRoomCheck);
                cboBedRoomSelect.Properties.Tag = gridCheck;
                cboBedRoomSelect.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboBedRoomSelect.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBedRoomSelect.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Event_BedRoomCheck(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                this.bedRoomFilterSelecteds = new List<V_HIS_ROOM>();
                if (gridCheckMark != null)
                {
                    List<V_HIS_ROOM> erSelectedNews = new List<V_HIS_ROOM>();
                    foreach (V_HIS_ROOM er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.ROOM_NAME);
                            erSelectedNews.Add(er);
                        }

                    }
                    this.bedRoomFilterSelecteds = new List<V_HIS_ROOM>();
                    this.bedRoomFilterSelecteds.AddRange(erSelectedNews);
                }
                this.cboBedRoomSelect.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCBedRoomPartial_Leave(object sender, EventArgs e)
        {
            try
            {
                // KhongHienThiKTT
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateKhongHienThiKTT = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "chkKhongHienThiKTT" && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                if (csAddOrUpdateKhongHienThiKTT != null)
                {
                    csAddOrUpdateKhongHienThiKTT.VALUE = (chkKhongHienThiKTT.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdateKhongHienThiKTT = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateKhongHienThiKTT.KEY = "chkKhongHienThiKTT";
                    csAddOrUpdateKhongHienThiKTT.VALUE = (chkKhongHienThiKTT.Checked ? "1" : "");
                    csAddOrUpdateKhongHienThiKTT.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateKhongHienThiKTT);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void cboBedRoomSelect_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.bedRoomFilterSelecteds != null && this.bedRoomFilterSelecteds.Count > 0)
                {
                    foreach (var item in this.bedRoomFilterSelecteds)
                    {
                        roomName += item.ROOM_NAME + ", ";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void gridViewTreatmentBedRoom_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.RowCellClickBedRoom = (L_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetFocusedRow();
                gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedCell = true;
                gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedRow = true;
                if (this.RowCellClickBedRoom != null)
                {
                    this.treatmentId = this.RowCellClickBedRoom.TREATMENT_ID;
                    this.treatmentCode = this.RowCellClickBedRoom.TREATMENT_CODE;
                    LogTheadInSessionInfo(() => SelectPatient(this.RowCellClickBedRoom, this.RowCellClickBedRoom.TREATMENT_ID), "gridViewTreatmentBedRoomRowClick");
                }
                else
                {
                    SetEnableButton(false);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridViewTreatmentBedRoom_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.gridViewTreatmentBedRoom.GetGroupRowValue(e.RowHandle, this.grdColRoomName) ?? "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmployee_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (cboEmployee.EditValue != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                cboEmployee.EditValue = null;
        }

        private string GetPathDefault()
        {
            string imageDefaultPath = string.Empty;
            try
            {
                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                imageDefaultPath = localPath + "\\Img\\ImageStorage\\notImage.jpg";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return imageDefaultPath;
        }

        private void cboPATIENT_CLASSIFY_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string strDisplay = "";
                if (this.patientClassifyFilterSelecteds != null && this.patientClassifyFilterSelecteds.Count > 0)
                {
                    foreach (var item in this.patientClassifyFilterSelecteds)
                    {
                        strDisplay += item.PATIENT_CLASSIFY_NAME + ", ";
                    }
                    if (!String.IsNullOrWhiteSpace(strDisplay) && strDisplay.Length >= 2)
                        strDisplay = strDisplay.Remove(strDisplay.Length - 2, 2);
                }
                e.DisplayText = strDisplay;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsCheckDepartmentTran()
        {
            bool result = true;
            try
            {
                if (RowCellClickBedRoom != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisDepartmentTranFilter _departmentTranFilter = new HisDepartmentTranFilter();
                    _departmentTranFilter.TREATMENT_ID = RowCellClickBedRoom.TREATMENT_ID;
                    _departmentTranFilter.DEPARTMENT_ID = BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == currentModule.RoomId).DEPARTMENT_ID;
                    var _DepartmentTrans = new BackendAdapter(param).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, _departmentTranFilter, param);
                    if (_DepartmentTrans != null && _DepartmentTrans.Count > 0)
                    {
                        long from = 0;
                        long to = 0;
                        if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
                        {
                            from = Convert.ToInt64(dtFrom.DateTime.ToString("yyyyMMdd") + "000000");
                        }
                        if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
                        {
                            to = Convert.ToInt64(dtTo.DateTime.ToString("yyyyMMdd") + "235959");
                        }
                        var department =  from != 0 && to != 0 ? _DepartmentTrans.Where(o => from <= (o.DEPARTMENT_IN_TIME ?? 0) && (o.DEPARTMENT_IN_TIME ?? 0) <= to).ToList().FirstOrDefault() : _DepartmentTrans.OrderByDescending(o=>o.DEPARTMENT_IN_TIME ?? 0).ToList().FirstOrDefault();
                        if (department != null)
                        {
                            if (department.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE && DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("{0} đã khóa chi phí không cho phép thực hiện chỉ định, kê đơn", BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == department.DEPARTMENT_ID).DEPARTMENT_NAME),
                       HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm),
                       MessageBoxButtons.OK) == DialogResult.OK)
                                return false;
                        }
                    }
                    if (string.IsNullOrEmpty(RowCellClickBedRoom.BED_CODE))
                    {
                        var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == RowCellClickBedRoom.TDL_TREATMENT_TYPE_ID).IS_REQUIRED_SERVICE_BED == 1;
                        if (treatmentType)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân chưa chỉ định giường không cho phép kê đơn, chỉ định",
                       HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm));
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

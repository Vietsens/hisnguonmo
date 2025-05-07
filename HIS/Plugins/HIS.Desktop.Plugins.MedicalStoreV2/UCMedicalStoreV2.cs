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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Customization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.MedicalStoreV2.ADO;
using HIS.Desktop.Plugins.MedicalStoreV2.Borrow;
using HIS.Desktop.Plugins.MedicalStoreV2.Config;
using HIS.Desktop.Plugins.MedicalStoreV2.Popup;
using HIS.Desktop.Plugins.MedicalStoreV2.Resources;
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
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicalStoreV2
{
    public partial class UCMedicalStoreV2 : UserControlBase
    {
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        Inventec.Desktop.Common.Modules.Module currentModule = null;
        int startPageTreatment = 0, startPageMediRecord = 0;
        int rowCountTreatment = 0, rowCountMediRecord = 0;
        int dataTotalTreatment = 0, dataTotalMediRecord = 0;
        List<HIS_TREATMENT_TYPE> _TreatmentTypeSelecteds;
        List<HIS_DEPARTMENT> _EndDepartmentSelects;
        List<HIS_DEPARTMENT> _ExecuteDepartmentSelects;
        List<MOS.EFMODEL.DataModels.HIS_PROGRAM> _ProgramSelects;
        double? blueConfig;
        double? orangeConfig;
        string IsRequiredLatchApproveStore = "0";
        List<HIS_PATIENT_TYPE> patientTypeSelecteds;
        internal RepositoryItemCheckEdit checkEdit;
        internal HisTreatmentLView3Filter _FilterLoadTree { get; set; }
        PopupMenuProcessor popupMenuProcessor = null;
        HIS_TREATMENT currentTreatment;


        bool isInit = true;
        private List<HIS_LOCATION_STORE> lstLocationStore;

        public UCMedicalStoreV2()
        {
            InitializeComponent();
        }

        public UCMedicalStoreV2(Inventec.Desktop.Common.Modules.Module module) : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCMedicalStore_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                checkEdit = (RepositoryItemCheckEdit)treeListMedicalStore.RepositoryItems.Add("CheckEdit");
                dtOutTimeFrom.DateTime = DateTime.Now;
                dtOutTimeTo.DateTime = DateTime.Now;
                rdoStatus__All.Checked = true;
                SetCaptionByLanguageKey();
                LoadConfig();
                InitControlState();
                LoadComboStatus();
                LoadComboMediRecordType();
                //InitPatientTypeCheck();
                //InitComboPatientType();
                LoaddataToTreeList();
                LoadDataToGridControlTreatment();
                LoadDataToGridControlMediRecord();
                InitGridLookUpEdit_MultiSelect(cboTreatmentType, SelectionGrid__TreatmentType);
                InitGridLookUpEdit_MultiSelect(cboEndDepartment, SelectionGrid__EndDepartment);
                InitGridLookUpEdit_MultiSelect(cboExecuteDepartment, SelectionGrid__ExecuteDepartment);
                InitGridLookUpEdit_MultiSelect(cboPatientType, SelectionGrid__PatientType);
                InitGridLookUpEdit_MultiSelect(cboProgram, SelectionGrid__Program);
                LoadComboEndDepartment();
                LoadComboExecuteDepartment();
                LoadComboTreatmentType();
                InitCombo(cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), "PATIENT_TYPE_NAME", "ID");
                InitCombo(cboProgram, BackendDataWorker.Get<HIS_PROGRAM>(), "PROGRAM_NAME", "ID");
                LoadComboLocationStore();
                LoadComboLocationRepGrid();
                isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboLocationRepGrid()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOCATION_STORE_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("LOCATION_STORE_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("LOCATION_STORE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(repLocationStore, lstLocationStore, controlEditorADO);
                repLocationStore.ImmediatePopup = true;
                repLocationStore.PopupFormSize = new Size(350, repLocationStore.PopupFormSize.Height);

                ControlEditorLoader.Load(repLocationStoreDis, lstLocationStore, controlEditorADO);
                repLocationStoreDis.ImmediatePopup = true;
                repLocationStoreDis.PopupFormSize = new Size(350, repLocationStoreDis.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadConfig()
        {
            try
            {
                string blue = HisConfigs.Get<string>("HIS.Desktop.Plugins.MedicalStore.BLUE_WARNING_STORE_TIME");
                string orange = HisConfigs.Get<string>("HIS.Desktop.Plugins.MedicalStore.ORANGE_WARNING_STORE_TIME");
                // Kiểm tra cấu hình hệ thốngHIS.Desktop.Plugins.MedicalStoreV2.IsRequiredLatchApproveStore"
                // Bắt buộc duyệt hồ sơ bệnh án mới hiển thị ở màn hình tủ bệnh án hay không
                // 1:bắt buộc - 0: không bắt buộc
                IsRequiredLatchApproveStore = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.MedicalStoreV2.IsRequiredLatchApproveStore");
                if (!string.IsNullOrEmpty(blue))
                {
                    blueConfig = Inventec.Common.TypeConvert.Parse.ToDouble(blue);
                }
                if (!string.IsNullOrEmpty(orange))
                {
                    orangeConfig = Inventec.Common.TypeConvert.Parse.ToDouble(orange);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__PatientType(object sender, EventArgs e)
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

        private void InitGridLookUpEdit_MultiSelect(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
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

        private void SelectionGrid__EndDepartment(object sender, EventArgs e)
        {
            try
            {
                _EndDepartmentSelects = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _EndDepartmentSelects.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__ExecuteDepartment(object sender, EventArgs e)
        {
            try
            {
                _ExecuteDepartmentSelects = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _ExecuteDepartmentSelects.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__TreatmentType(object sender, EventArgs e)
        {
            try
            {
                _TreatmentTypeSelecteds = new List<HIS_TREATMENT_TYPE>();
                foreach (HIS_TREATMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _TreatmentTypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Program(object sender, EventArgs e)
        {
            try
            {
                _ProgramSelects = new List<HIS_PROGRAM>();
                foreach (HIS_PROGRAM rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _ProgramSelects.Add(rv);
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MedicalStoreV2.Resources.Lang", typeof(UCMedicalStoreV2).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExportExcel.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.btnExportExcel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExecuteDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.cboExecuteDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkJustShowTreatmentLatch.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.checkJustShowTreatmentLatch.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkJustShowTreatmentLatch.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.checkJustShowTreatmentLatch.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImportStore.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.btnImportStore.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.rdoStatus__Stored.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.rdoStatus__Stored.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.rdoStatus__UnStored.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.rdoStatus__UnStored.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.rdoStatus__All.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.rdoStatus__All.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCodeMediRecord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.txtPatientCodeMediRecord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediRecordType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.cboMediRecordType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblDanhSachHoSoChuaLuuTru.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lblDanhSachHoSoChuaLuuTru.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblDanhSachBenhAn.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lblDanhSachBenhAn.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.treeListColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn29.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn31.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn35.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn35.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn35.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn35.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn44.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn44.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn45.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn45.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn46.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn46.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn47.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn47.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn48.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn48.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn49.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn49.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn36.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn36.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn36.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn37.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn37.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn37.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn37.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn40.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn40.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn41.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn41.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn41.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn41.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn42.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn42.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn42.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn42.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn43.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn43.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkAutoShowStore.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.checkAutoShowStore.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchMediRecord.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.btnSearchMediRecord.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn33.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn27.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCodeMediRecord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.txtTreatmentCodeMediRecord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn57.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn57.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColView.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.grdColView.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn34.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn34.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn34.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn50.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn50.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn58.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn58.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn59.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn59.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn60.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn60.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn61.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn61.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn63.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn63.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn63.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn63.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn64.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn64.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn64.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn64.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn65.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn65.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn65.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn65.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn68.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn68.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn70.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn70.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn70.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn70.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn72.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn72.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn51.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn51.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn73.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn73.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn39.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn39.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn75.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn75.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn71.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn71.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn38.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn38.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn53.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn53.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn53.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn53.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboProgram.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.cboProgram.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveTreatment.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.btnSaveTreatment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveTreatment.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.btnSaveTreatment.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeywordMediRecord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.txtKeywordMediRecord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatusEnd.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.cboStatusEnd.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchTreatment.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.btnSearchTreatment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCodeSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.txtTreatmentCodeSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCodeSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.txtPatientCodeSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeywordTreatment.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.txtKeywordTreatment.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEndDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.cboEndDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStatusEnd.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lciStatusEnd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEndDepartment.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lciEndDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOutTimeFilter.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lciOutTimeFilter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInTimeFilter.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lciInTimeFilter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lciTreatmentType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientType.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lciPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStoreTimeFilter.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lciStoreTimeFilter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProgramFilter.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lciProgramFilter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFilterMediRecordType.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.lciFilterMediRecordType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridLookUpEdit1.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridLookUpEdit1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn55.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn55.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn55.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn55.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn54.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStoreV2.gridColumn54.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoaddataToTreeList()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDataStoreView1Filter filter = new HisDataStoreView1Filter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_DATA_STORE_1>>(
                    "api/HisDataStore/GetView1", ApiConsumers.MosConsumer, filter, param)
                    .Where(p => p.ROOM_ID == this.currentModule.RoomId ||
                                p.ROOM_TYPE_ID == this.currentModule.RoomTypeId)
                    .ToList();

                if (currentDataStore != null && currentDataStore.Count > 0)
                {
                    var listAdo = new List<DataStoreADO>();

                    Dictionary<long, bool> previousCheckStates = new Dictionary<long, bool>();
                    if (treeListMedicalStore.DataSource is BindingList<DataStoreADO> previousData)
                    {
                        foreach (var item in previousData)
                        {
                            previousCheckStates[item.ID] = item.CheckStore;
                        }
                    }

                    foreach (var item in currentDataStore)
                    {
                        var ado = new DataStoreADO(item);
                        if (previousCheckStates.ContainsKey(ado.ID))
                        {
                            ado.CheckStore = previousCheckStates[ado.ID];
                        }
                        else
                        {
                            ado.CheckStore = true; 
                        }

                        listAdo.Add(ado);
                    }
                    var binding = new BindingList<DataStoreADO>(listAdo);

                    treeListMedicalStore.KeyFieldName = "ID";
                    treeListMedicalStore.ParentFieldName = "PARENT_ID";
                    treeListMedicalStore.DataSource = binding;
                }
                checkEdit.ValueChecked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //try
            //{
            //    CommonParam param = new CommonParam();
            //    HisDataStoreView1Filter filter = new HisDataStoreView1Filter();
            //    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
            //    var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_DATA_STORE_1>>("api/HisDataStore/GetView1", ApiConsumers.MosConsumer, filter, param).Where(p => p.ROOM_ID == this.currentModule.RoomId || p.ROOM_TYPE_ID == this.currentModule.RoomTypeId).ToList();

            //    if (currentDataStore != null && currentDataStore.Count > 0)
            //    {
            //        var listAdo = new List<DataStoreADO>();
            //        foreach (var item in currentDataStore)
            //        {
            //            var ado = new DataStoreADO(item);
            //            if (treeListMedicalStore.DataSource != null)
            //            {
            //                BindingList<DataStoreADO> existingData = (BindingList<DataStoreADO>)treeListMedicalStore.DataSource;
            //                var existingItem = existingData.FirstOrDefault(o => o.ID == ado.ID);
            //                if (existingItem != null)
            //                {
            //                    ado.CheckStore = existingItem.CheckStore;
            //                }
            //                else
            //                {
            //                    ado.CheckStore = true; 
            //                }
            //            }
            //            else
            //            {
            //                ado.CheckStore = true;
            //            }
            //            listAdo.Add(ado);
            //        }

            //        var binding = new BindingList<DataStoreADO>(listAdo);

            //        treeListMedicalStore.KeyFieldName = "ID";
            //        treeListMedicalStore.ParentFieldName = "PARENT_ID";
            //        treeListMedicalStore.DataSource = binding;
            //    }
            //    else
            //    {
            //        treeListMedicalStore.DataSource = null;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void RefreshDataAfterSave()
        {
            try
            {
                LoaddataToTreeList();
                LoadDataToGridControlTreatment();
                LoadDataToGridControlMediRecord();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridControlTreatment()
        {
            try
            {
                int pageSize = 0;
                if (ucPagingTreatment.pagingGrid != null)
                {
                    pageSize = ucPagingTreatment.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                TreatmentPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCountTreatment;
                param.Count = dataTotalTreatment;
                ucPagingTreatment.Init(TreatmentPaging, param, pageSize, this.gridControlTreatment);
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
                status.Add(new Status(1, ResourceMessage.DaKetThuc));
                status.Add(new Status(2, ResourceMessage.ChuaKetThuc));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboStatusEnd, status, controlEditorADO);
                cboStatusEnd.EditValue = status[0].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboMediRecordType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_RECORD_TYPE_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("MEDI_RECORD_TYPE_NAME", "", 230, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_RECORD_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMediRecordType, BackendDataWorker.Get<HIS_MEDI_RECORD_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TreatmentPaging(object param)
        {
            try
            {
                InitRestoreLayoutGridViewFromXml(gridViewTreatment);
                WaitingManager.Show();
                startPageTreatment = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPageTreatment, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT_3>> apiResult = null;

                HisTreatmentLView3Filter filter = new HisTreatmentLView3Filter();

                SetFilterTreatment(ref filter);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisTreatment/GetLView3 filter ", filter));

                apiResult = new BackendAdapter(paramCommon).GetRO<List<L_HIS_TREATMENT_3>>("api/HisTreatment/GetLView3", ApiConsumers.MosConsumer, filter, paramCommon);
                gridControlTreatment.BeginUpdate();
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT_3>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        List<TreatmentADO> listTreatmentADO = new List<TreatmentADO>();
                        foreach (var item in data)
                        {
                            var ado = new TreatmentADO(item);
                            ado.CheckStore = false;
                            listTreatmentADO.Add(ado);
                        }

                        gridControlTreatment.DataSource = null;
                        gridControlTreatment.DataSource = listTreatmentADO;
                        gridControlTreatment.Focus();
                        gridViewTreatment.FocusedRowHandle = 0;
                        rowCountTreatment = (data == null ? 0 : data.Count);
                        dataTotalTreatment = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        rowCountTreatment = 0;
                        dataTotalTreatment = 0;
                        gridControlTreatment.DataSource = null;
                    }
                }
                else
                {
                    rowCountTreatment = 0;
                    dataTotalTreatment = 0;
                    gridControlTreatment.DataSource = null;
                }
                gridControlTreatment.EndUpdate();
                WaitingManager.Hide();

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

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_AUTO_SHOW_STORE)
                        {
                            checkAutoShowStore.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_JUST_SHOW_TREATMENT_LATCH)
                        {
                            checkJustShowTreatmentLatch.Checked = item.VALUE == "1";
                        }
                        if (IsRequiredLatchApproveStore == "1")
                        {
                            checkJustShowTreatmentLatch.CheckState = CheckState.Checked;
                            checkJustShowTreatmentLatch_CheckedChanged(null, null);
                            checkJustShowTreatmentLatch.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetFilterMediRecord(ref HisMediRecordView1Filter filter)
        {
            try
            {
                this._FilterLoadTree = new HisTreatmentLView3Filter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "STORE_CODE";

                if (!string.IsNullOrEmpty(txtTreatmentCodeMediRecord.Text))
                {
                    string code = txtTreatmentCodeMediRecord.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCodeMediRecord.Text = code;
                    }
                    filter.TREATMENT_CODE = code;
                }
                if (!string.IsNullOrEmpty(txtPatientCodeMediRecord.Text))
                {
                    string code = txtPatientCodeMediRecord.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCodeMediRecord.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }
                if (!string.IsNullOrEmpty(txtStoreCodeMediRecord.Text))
                {
                    filter.STORE_CODE__EXACT = txtStoreCodeMediRecord.Text.Trim();
                }
                if (!string.IsNullOrEmpty(txtKeywordMediRecord.Text))
                {
                    string code = txtKeywordMediRecord.Text.Trim();
                    filter.KEY_WORD = code;
                }

                if (dtStoreTime_From.EditValue != null)
                {
                    filter.STORE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtStoreTime_From.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtStoreTime_To.EditValue != null)
                {
                    filter.STORE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtStoreTime_To.DateTime.ToString("yyyyMMdd") + "235959");
                }
                if (_ProgramSelects != null && _ProgramSelects.Count > 0)
                {
                    filter.PROGRAM_IDs = _ProgramSelects.Select(o => o.ID).ToList();
                }
                if (rdoStatus__UnStored.Checked)
                {
                    filter.IS_NOT_STORED = true;
                }
                else if (rdoStatus__Stored.Checked)
                {
                    filter.IS_NOT_STORED = false;
                }
                else
                {
                    filter.IS_NOT_STORED = null;
                }
                if (cboMediRecordType.EditValue != null)
                {
                    filter.MEDI_RECORD_TYPE_ID = Convert.ToInt64(cboMediRecordType.EditValue);
                }
                List<DataStoreADO> dataStore = new List<DataStoreADO>();
                var binding = (BindingList<DataStoreADO>)treeListMedicalStore.DataSource;
                if (binding != null && binding.Count > 0)
                    dataStore = binding.ToList();

                var dataStoreIds = dataStore != null ? dataStore.Where(o => o.CheckStore).Select(o => o.ID).ToList() : null;

                if (dataStoreIds != null && dataStoreIds.Count > 0)
                {
                    filter.DATA_STORE_IDs = dataStoreIds;
                }
                else
                {
                    filter.DATA_STORE_ID = 0;
                }
                if (cboLocationStore.EditValue != null)
                    filter.LOCATION_STORE_ID = Int64.Parse(cboLocationStore.EditValue.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFilterTreatment(ref HisTreatmentLView3Filter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.IS_STORED = false;
                if (checkJustShowTreatmentLatch.Checked)
                {
                    filter.APPROVAL_STORE_STT_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT.APPROVAL_STORE_STT_ID__CHOT;
                }
                if (!string.IsNullOrEmpty(txtTreatmentCodeSearch.Text))
                {
                    string code = txtTreatmentCodeSearch.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCodeSearch.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }

                if (!string.IsNullOrEmpty(txtPatientCodeSearch.Text))
                {
                    string code = txtPatientCodeSearch.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCodeSearch.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }

                if (!string.IsNullOrEmpty(txtKeywordTreatment.Text))
                {
                    filter.KEY_WORD = txtKeywordTreatment.Text.Trim();
                }

                if (cboStatusEnd.EditValue != null)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt32((cboStatusEnd.EditValue ?? "").ToString()) == 1)
                    {
                        filter.IS_PAUSE = true;
                    }
                    if (Inventec.Common.TypeConvert.Parse.ToInt32((cboStatusEnd.EditValue ?? "").ToString()) == 2)
                    {
                        filter.IS_PAUSE = false;
                    }
                }

                if (this._EndDepartmentSelects != null && this._EndDepartmentSelects.Count > 0)
                {
                    filter.END_DEPARTMENT_IDs = this._EndDepartmentSelects.Select(o => o.ID).Distinct().ToList();
                }

                if (this._ExecuteDepartmentSelects != null && this._ExecuteDepartmentSelects.Count > 0)
                {
                    filter.LAST_DEPARTMENT_IDs = this._ExecuteDepartmentSelects.Select(o => o.ID).Distinct().ToList();
                }

                if (this._TreatmentTypeSelecteds != null && this._TreatmentTypeSelecteds.Count > 0)
                {
                    filter.TDL_TREATMENT_TYPE_IDs = this._TreatmentTypeSelecteds.Select(o => o.ID).Distinct().ToList();
                }
                if (patientTypeSelecteds != null && patientTypeSelecteds.Count > 0)
                {
                    filter.TDL_PATIENT_TYPE_IDs = patientTypeSelecteds.Select(o => o.ID).ToList();
                }

                if (dtOutTimeFrom.EditValue != null)
                {
                    filter.OUT_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtOutTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtOutTimeTo.EditValue != null)
                {
                    filter.OUT_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtOutTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                if (dtInTimeFrom.EditValue != null)
                {
                    filter.IN_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtInTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtInTimeTo.EditValue != null)
                {
                    filter.IN_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtInTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<TreatmentADO> listCheckTreatment = new List<TreatmentADO>();
                var listDataSourceTreatment = (List<TreatmentADO>)gridControlTreatment.DataSource;
                listCheckTreatment = listDataSourceTreatment.Where(o => o.CheckTreatment).ToList();

                if (listCheckTreatment != null && listCheckTreatment.Count > 0)
                {
                    HIS.Desktop.Plugins.MedicalStoreV2.ChooseStore.frmSaveStore frmChooseStore = new ChooseStore.frmSaveStore(this.currentModule, listCheckTreatment, RefreshDataAfterSave, currentControlStateRDO, controlStateWorker);
                    frmChooseStore.ShowDialog();

                    txtPatientCodeSearch.Focus();
                    txtPatientCodeSearch.SelectAll();
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.VuiLongChonHoSoCanLuuTru, ResourceMessage.ThongBao);
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataToGridControlTreatment();
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
                LoadDataToGridControlTreatment();
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
                col2.Width = 300;
                col2.Caption = ResourceMessage.TatCa;
                cbo.Properties.PopupFormWidth = 300;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //{
                //    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboTreatmentType()
        {
            try
            {
                InitCombo(cboTreatmentType, BackendDataWorker.Get<HIS_TREATMENT_TYPE>(), "TREATMENT_TYPE_NAME", "ID");
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboEndDepartment()
        {
            try
            {
                InitCombo(cboEndDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.IS_ACTIVE == 1).ToList(), "DEPARTMENT_NAME", "ID");
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboExecuteDepartment()
        {
            try
            {
                InitCombo(cboExecuteDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && p.IS_CLINICAL == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "DEPARTMENT_NAME", "ID");
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void bbtnSEARCH()
        {
            try
            {
                if (btnSearchTreatment.Enabled)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void bbtnSEARCHMediRecord()
        {
            try
            {
                if (btnSearchMediRecord.Enabled)
                    btnSearchMediRecord_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void bbtnImportStore()
        {
            try
            {
                if (btnImportStore.Enabled)
                    btnImportStore_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void bbtnSAVE()
        {
            try
            {
                if (btnSaveTreatment.Enabled)
                {
                    btnSaveTreatment.Focus();
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void keyF2Focused()
        {
            try
            {
                if (txtTreatmentCodeSearch.Enabled)
                {
                    txtTreatmentCodeSearch.Focus();
                    txtTreatmentCodeSearch.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void keyF3Focused()
        {
            try
            {
                if (txtPatientCodeSearch.Enabled)
                {
                    txtPatientCodeSearch.Focus();
                    txtPatientCodeSearch.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediRecord_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var dataStoreID = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewMediRecord.GetRowCellValue(e.RowHandle, "DATA_STORE_ID") ?? "").ToString());
                    var IsNotStored = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewMediRecord.GetRowCellValue(e.RowHandle, "IS_NOT_STORED") ?? "").ToString());
                    var row = (MediRecordADO)gridViewMediRecord.GetRow(e.RowHandle);
                    if (row != null)
                    {
                        if (e.Column.FieldName == "Delete")
                        {
                            if (row.GIVE_DATE != null)
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_Delete_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = (dataStoreID != null && dataStoreID > 0) ? repositoryItemButtonEdit_Delete_Enable : repositoryItemButtonEdit_Delete_Disable;
                            }
                        }

                        else if (e.Column.FieldName == "Print")
                        {
                            e.RepositoryItem = (dataStoreID != null && dataStoreID > 0) ? repositoryItemButtonEdit_Print_Enable : repositoryItemButtonEdit_Print_Disable;
                        }
                        else if (e.Column.FieldName == "TreatmentBorrow")
                        {
                            e.RepositoryItem = (!row.GIVE_DATE.HasValue || (row.GIVE_DATE.HasValue && row.RECEIVE_DATE.HasValue)) ? Btn_TreatmentBorrow_Enable : Btn_TreatmentBorrow_Disable;
                        }

                        else if (e.Column.FieldName == "ReceiveTreatment")
                        {
                            e.RepositoryItem = (row.GIVE_DATE.HasValue && !row.RECEIVE_DATE.HasValue) ? ButtonEditReceiveTreatment_Enabed : ButtonEditReceiveTreatment_Disabed;
                        }
                        else if (e.Column.FieldName == "IMPORT_OR_UNIMPORT")
                        {
                            if (row.IS_NOT_STORED == 1)
                            {
                                e.RepositoryItem = ButtonEditImport;
                            }
                            else if (!row.IS_NOT_STORED.HasValue)
                            {
                                e.RepositoryItem = ButtonEditUnImport;
                            }
                        }
                        else if (e.Column.FieldName == "CheckStore")
                        {
                            if (row.IS_NOT_STORED == 1)
                            {
                                e.RepositoryItem = repositoryItemCheckEdit_Store__Enable;
                            }
                            else if (!row.IS_NOT_STORED.HasValue)
                            {
                                e.RepositoryItem = repositoryItemCheckEdit_Store__Disable;
                            }
                        }
                        else if (e.Column.FieldName == "ViewEmr")
                        {
                            if (String.IsNullOrWhiteSpace(row.TDL_TREATMENT_CODES))
                            {
                                e.RepositoryItem = repositoryItemButtonView_Disable;
                            }
                            else if (!row.IS_NOT_STORED.HasValue)
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_ViewEmrTreatment;
                            }
                        }
                        else if (e.Column.FieldName == "LOCATION_STORE_ID")
                        {
                            if (IsNotStored == 1) 
                                e.RepositoryItem = repLocationStore; 
                            else e.RepositoryItem = repLocationStoreDis;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MediRecordADO data = (MediRecordADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPageTreatment;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                        }
                        else if (e.Column.FieldName == "STORE_CODE_CONFIG")
                        {
                            //e.Value = HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(data.TDL_PATIENT_DOB) + "  tuổi";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewTreatment_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var row = (MediRecordADO)gridViewMediRecord.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (row.GIVE_DATE != null && row.RECEIVE_DATE == null)
                    {
                        //long dayLong = Inventec.Common.TypeConvert.Parse.ToInt64(row.GIVE_DATE.ToString());
                        var day = Inventec.Common.DateTime.Calculation.DifferenceDate((long)row.GIVE_DATE.Value, Inventec.Common.DateTime.Get.Now() ?? 0);
                        if (day <= 10)
                            e.Appearance.ForeColor = Color.Green;
                        else
                            e.Appearance.ForeColor = Color.Red;
                    }

                    if (row.IS_NOT_STORED == 1 && row.STORE_TIME.HasValue)
                    {

                        TimeSpan timeSpan = DateTime.Now - (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.STORE_TIME.Value) ?? new DateTime());

                        if (HisConfigCFG.BlueWarningStoreTime.HasValue
                            && timeSpan.TotalHours > HisConfigCFG.BlueWarningStoreTime.Value)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        if (HisConfigCFG.OrangeWarningStoreTime.HasValue
                            && timeSpan.TotalHours > HisConfigCFG.OrangeWarningStoreTime.Value)
                        {
                            e.Appearance.ForeColor = Color.Orange;
                        }
                        if (HisConfigCFG.RedWarningStoreTime.HasValue
                            && timeSpan.TotalHours > HisConfigCFG.RedWarningStoreTime.Value)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void treeListMedicalStore_CustomDrawColumnHeader(object sender, DevExpress.XtraTreeList.CustomDrawColumnHeaderEventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                if (e.Column != null && e.Column.VisibleIndex == 0)
                {
                    Rectangle checkRect = new Rectangle(e.Bounds.Left + tree.Columns.FirstOrDefault(o => o.FieldName == "CheckStore").Width / 2 + 5, e.Bounds.Top + 2, 12, 12);
                    DevExpress.XtraTreeList.ViewInfo.ColumnInfo info = (DevExpress.XtraTreeList.ViewInfo.ColumnInfo)e.ObjectArgs;
                    if (info.CaptionRect.Left < 15)
                        info.CaptionRect = new Rectangle(new Point(info.CaptionRect.Left + 15, info.CaptionRect.Top), info.CaptionRect.Size);
                    e.Painter.DrawObject(info);

                    DrawCheckBox(e.Graphics, checkEdit, checkRect, IsAllSelected(sender as TreeList));
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeList1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                Point pt = new Point(e.X, e.Y);
                TreeListHitInfo hit = tree.CalcHitInfo(pt);
                if (hit.Column != null)
                {
                    DevExpress.XtraTreeList.ViewInfo.ColumnInfo info = tree.ViewInfo.ColumnsInfo[hit.Column];
                    Rectangle checkRect = new Rectangle(info.Bounds.Left + tree.Columns.FirstOrDefault(o => o.FieldName == "CheckStore").Width / 2 + 5, info.Bounds.Top + 2, 12, 12);
                    if (checkRect.Contains(pt))
                    {
                        List<DataStoreADO> dataStore = new List<DataStoreADO>();
                        var binding = (BindingList<DataStoreADO>)treeListMedicalStore.DataSource;
                        if (binding != null && binding.Count > 0)
                            dataStore = binding.ToList();

                        if (dataStore != null && dataStore.Count > 0)
                        {
                            if ((bool)checkEdit.ValueChecked)
                            {
                                foreach (var item in dataStore)
                                {
                                    item.CheckStore = true;
                                }
                                checkEdit.ValueChecked = false;
                            }
                            else
                            {
                                foreach (var item in dataStore)
                                {
                                    item.CheckStore = false;
                                }
                                checkEdit.ValueChecked = true;
                            }
                        }

                        if (dataStore != null)
                        {
                            var bindingTree = new BindingList<DataStoreADO>(dataStore);
                            treeListMedicalStore.KeyFieldName = "ID";
                            treeListMedicalStore.ParentFieldName = "PARENT_ID";
                            treeListMedicalStore.DataSource = bindingTree;
                        }

                        LoadDataToGridControlMediRecord();
                        //EmbeddedCheckBoxChecked(tree);
                        //throw new DevExpress.Utils.HideException();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckEdit_CheckStore_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                treeListMedicalStore.PostEditor();

                WaitingManager.Show();
                DataStoreADO ado = (DataStoreADO)treeListMedicalStore.GetDataRecordByNode(treeListMedicalStore.FocusedNode);
                List<DataStoreADO> dataStores = null;
                var binding = (BindingList<DataStoreADO>)treeListMedicalStore.DataSource;
                if (binding != null && binding.Count > 0)
                    dataStores = binding.ToList();

                if (ado != null && dataStores != null && dataStores.Count > 0)
                {
                    dataStores.ForEach(o =>
                        {
                            if (o.PARENT_ID == ado.ID) o.CheckStore = ado.CheckStore;
                        });
                    var bindingTree = new BindingList<DataStoreADO>(dataStores);
                    treeListMedicalStore.KeyFieldName = "ID";
                    treeListMedicalStore.ParentFieldName = "PARENT_ID";
                    treeListMedicalStore.DataSource = bindingTree;
                }
                gridViewMediRecord.Focus();
                LoadDataToGridControlMediRecord();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void repositoryItemButtonEdit_Delete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (TreatmentADO)gridViewMediRecord.GetFocusedRow();
                if (data != null)
                {
                    bool result = false;
                    CommonParam param = new CommonParam();
                    //data.DATA_STORE_ID = null;
                    var current = new BackendAdapter(param).Post<L_HIS_TREATMENT_3>("api/HisTreatment/UpdateDataStoreId", ApiConsumers.MosConsumer, data, param);
                    if (current != null)
                    {
                        result = true;
                        LoaddataToTreeList();
                        LoadDataToGridControlTreatment();
                    }
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, result);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void repositoryItemButtonEdit_Print_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (MediRecordADO)gridViewMediRecord.GetFocusedRow();
                if (data != null)
                {
                    PrintMediRecord(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_AddPatientProgram_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (TreatmentADO)gridViewMediRecord.GetFocusedRow();
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisPatientProgram").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisPatientProgram");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        //listArgs.Add(row.PATIENT_ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();

                        LoadDataToGridControlTreatment();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_TreatmentBorrow_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MediRecordADO)gridViewMediRecord.GetFocusedRow();
                if (row != null)
                {
                    //CommonParam param = new CommonParam();
                    //HisTreatmentBorrowFilter filter = new HisTreatmentBorrowFilter();
                    //filter.TREATMENT_ID = row.ID;
                    //filter.IS_RECEIVE = false;

                    //var rsApi = new BackendAdapter(param).Get<List<HIS_TREATMENT_BORROW>>("api/HisTreatmentBorrow/Get", ApiConsumers.MosConsumer, filter, param);
                    //if (rsApi != null && rsApi.Count > 0)
                    //{
                    //    DevExpress.XtraEditors.XtraMessageBox.Show("Hồ sơ bệnh án đã được mượn", "Thông báo");
                    //}
                    //else
                    //{
                    TreatmentBorrow frm = new TreatmentBorrow(row, RefreshDataAfterSaveMediRecord);
                    frm.ShowDialog();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void RefreshDataAfterSaveMediRecord()
        {
            try
            {
                LoaddataToTreeList();
                LoadDataToGridControlMediRecord();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatusEnd_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboStatusEnd.Properties.Buttons[1].Visible = true;
                    cboStatusEnd.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearchTreatment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataToGridControlTreatment();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProgramCode_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void cboEndDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEndDepartment.EditValue = null;
                    this._EndDepartmentSelects = null;
                    ResetCombo(cboEndDepartment);
                    cboStatusEnd.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientType.EditValue = null;
                    this.patientTypeSelecteds = null;
                    ResetCombo(cboPatientType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTreatmentType.EditValue = null;
                    this._TreatmentTypeSelecteds = null;
                    ResetCombo(cboTreatmentType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtStoreCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearchMediRecord_Click(null, null);
                }
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
                string department = "";
                if (_EndDepartmentSelects != null && _EndDepartmentSelects.Count > 0)
                {
                    foreach (var item in _EndDepartmentSelects)
                    {
                        department += item.DEPARTMENT_NAME + ", ";
                    }
                    //cboStatusEnd.Enabled = false;
                    cboStatusEnd.EditValue = 1;
                }
                else
                {
                    cboStatusEnd.Enabled = true;
                }

                e.DisplayText = department;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string treatmentTypeName = "";
                if (_TreatmentTypeSelecteds != null && _TreatmentTypeSelecteds.Count > 0)
                {
                    foreach (var item in _TreatmentTypeSelecteds)
                    {
                        treatmentTypeName += item.TREATMENT_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = treatmentTypeName;
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
                string patientTypeName = "";
                if (patientTypeSelecteds != null && patientTypeSelecteds.Count > 0)
                {
                    foreach (var item in patientTypeSelecteds)
                    {
                        patientTypeName += item.PATIENT_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = patientTypeName;
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
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                    cbo.Enabled = false;
                    cbo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool IsAllSelected(TreeList tree)
        {
            return tree.Selection.Count > 0 && tree.Selection.Count == tree.AllNodesCount;
        }

        protected void DrawCheckBox(Graphics g, RepositoryItemCheckEdit edit, Rectangle r, bool Checked)
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter;
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args;
            info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            painter = edit.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = Checked;
            info.Bounds = r;
            info.CalcViewInfo(g);
            args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, new DevExpress.Utils.Drawing.GraphicsCache(g), r);
            painter.Draw(args);
            args.Cache.Dispose();
        }

        private void EmbeddedCheckBoxChecked(TreeList tree)
        {
            if (IsAllSelected(tree))
                tree.Selection.Clear();
            else
                SelectAll(tree);
        }

        private void SelectAll(TreeList tree)
        {
            tree.BeginUpdate();
            tree.NodesIterator.DoOperation(new SelectNodeOperation());
            tree.EndUpdate();
        }

        class SelectNodeOperation : TreeListOperation
        {
            public override void Execute(TreeListNode node)
            {
                node.Selected = true;
            }
        }

        private void gridViewTreatment_CustomUnboundColumnData_1(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TreatmentADO data = (TreatmentADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPageTreatment;
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                        else if (e.Column.FieldName == "AGE")
                        {
                            e.Value = HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(data.TDL_PATIENT_DOB) + "  tuổi";
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_GENDER_NAME")
                        {
                            //e.Value = HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(data.TDL_PATIENT_DOB) + "  tuổi";
                        }
                        else if (e.Column.FieldName == "REJECT_STORE_REASON_STR")
                        {
                            
                            e.Value = data.REJECT_STORE_REASON;
                            
                        }
                        else if (e.Column.FieldName == "CLINICAL_IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CLINICAL_IN_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentOfMediRecord_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TreatmentADO data = (TreatmentADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPageTreatment;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            //e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            //e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                        else if (e.Column.FieldName == "AGE")
                        {
                            e.Value = HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(data.TDL_PATIENT_DOB) + "  tuổi";
                        }
                        else if (e.Column.FieldName == "STORE_CODE_CONFIG")
                        {
                            //e.Value = HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(data.TDL_PATIENT_DOB) + "  tuổi";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearchMediRecord_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGridControlMediRecord();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridControlMediRecord()
        {
            try
            {
                int pageSize = 0;
                if (ucPagingMediRecord.pagingGrid != null)
                {
                    pageSize = ucPagingMediRecord.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                MediRecordPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCountMediRecord;
                param.Count = dataTotalMediRecord;
                ucPagingMediRecord.Init(MediRecordPaging, param, pageSize, this.gridControlMediRecord);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MediRecordPaging(object param)
        {
            try
            {
                InitRestoreLayoutGridViewFromXml(gridViewMediRecord);
                WaitingManager.Show();
                startPageMediRecord = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPageMediRecord, limit);
                ApiResultObject<List<V_HIS_MEDI_RECORD_1>> apiResult = null;

                HisMediRecordView1Filter filter = new HisMediRecordView1Filter();

                SetFilterMediRecord(ref filter);

                LogSystem.Debug(LogUtil.TraceData("api/HisMediRecord/GetView1 filter", filter));

                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_MEDI_RECORD_1>>("api/HisMediRecord/GetView1", ApiConsumers.MosConsumer, filter, paramCommon);
                gridControlMediRecord.BeginUpdate();
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD_1>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        List<MediRecordADO> listMediRecordADO = new List<MediRecordADO>();
                        foreach (var item in data)
                        {
                            var ado = new MediRecordADO(item);
                            ado.CheckStore = false;
                            listMediRecordADO.Add(ado);
                        }

                        gridControlMediRecord.DataSource = null;
                        gridControlMediRecord.DataSource = listMediRecordADO;
                        rowCountMediRecord = (data == null ? 0 : data.Count);
                        dataTotalMediRecord = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        rowCountMediRecord = 0;
                        dataTotalMediRecord = 0;
                        gridControlMediRecord.DataSource = null;
                    }
                }
                else
                {
                    rowCountMediRecord = 0;
                    dataTotalMediRecord = 0;
                    gridControlMediRecord.DataSource = null;
                }
                gridControlMediRecord.EndUpdate();
                WaitingManager.Hide();

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

        private void cboProgram_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboProgram.EditValue = null;
                    this._ProgramSelects = null;
                    ResetCombo(cboProgram);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProgram_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string Program = "";
                if (_ProgramSelects != null && _ProgramSelects.Count > 0)
                {
                    foreach (var item in _ProgramSelects)
                    {
                        Program += item.PROGRAM_NAME + ", ";
                    }
                }
                else
                {
                    cboStatusEnd.Enabled = true;
                }

                e.DisplayText = Program;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediRecord_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;

                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            if (checkEdit == null)
                                return;

                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatment_RowCellStyle_1(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var row = (TreatmentADO)gridViewTreatment.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (row.TDL_HEIN_CARD_NUMBER != null)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }

                    //12664
                    if (row.FEE_LOCK_TIME != null)
                    {
                        try
                        {
                            TimeSpan timeSpan = DateTime.Now - (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.FEE_LOCK_TIME ?? 0) ?? new DateTime());
                            if (blueConfig != null)
                            {
                                if (orangeConfig != null)
                                {
                                    if (timeSpan.TotalHours > orangeConfig)
                                    {
                                        if (orangeConfig > blueConfig)
                                        {
                                            e.Appearance.ForeColor = Color.Orange;
                                        }
                                        else if (timeSpan.TotalHours > blueConfig)
                                        {
                                            e.Appearance.ForeColor = Color.Blue;
                                        }
                                    }
                                    else if (timeSpan.TotalHours > blueConfig)
                                    {
                                        e.Appearance.ForeColor = Color.Blue;
                                    }
                                }
                                else
                                {
                                    if (timeSpan.TotalHours > blueConfig)
                                    {
                                        e.Appearance.ForeColor = Color.Blue;
                                    }
                                }
                            }
                            else if (orangeConfig != null)
                            {
                                if (timeSpan.TotalHours > orangeConfig)
                                    e.Appearance.ForeColor = Color.Orange;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediRecord_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var dataSource = (MediRecordADO)gridViewMediRecord.GetFocusedRow();
                if (dataSource != null)
                {
                    InitRestoreLayoutGridViewFromXml(gridViewTreatmentOfMediRecord);
                    // load data to grid treatment by medirecord
                    HisTreatmentLView3Filter filter = new HisTreatmentLView3Filter();
                    filter.MEDI_RECORD_ID = dataSource.ID;

                    WaitingManager.Show();
                    var apiResult = new BackendAdapter(new CommonParam()).Get<List<L_HIS_TREATMENT_3>>("api/HisTreatment/GetLView3", ApiConsumers.MosConsumer, filter, null);
                    WaitingManager.Hide();

                    if (apiResult != null && apiResult.Count > 0)
                    {
                        gridControlTreatmentOfMediRecord.BeginUpdate();
                        List<TreatmentADO> listTreatmentADO = new List<TreatmentADO>();
                        //if (filter.DATA_STORE_IDs != null && filter.DATA_STORE_IDs.Count > 0)
                        //{
                        foreach (var item in apiResult)
                        {
                            var ado = new TreatmentADO(item);
                            ado.CheckStore = true;
                            listTreatmentADO.Add(ado);
                        }
                        //}
                        //else
                        //{
                        //foreach (var item in apiResult)
                        //{
                        //    var ado = new TreatmentADO(item);
                        //    ado.CheckStore = false;
                        //    listTreatmentADO.Add(ado);
                        //}
                        //}

                        gridControlTreatmentOfMediRecord.DataSource = null;
                        gridControlTreatmentOfMediRecord.DataSource = listTreatmentADO;
                        gridControlTreatmentOfMediRecord.EndUpdate();
                    }
                    else
                    {
                        gridControlTreatmentOfMediRecord.BeginUpdate();
                        gridControlTreatmentOfMediRecord.DataSource = null;
                        gridControlTreatmentOfMediRecord.EndUpdate();
                    }
                }
                else
                {
                    gridControlTreatmentOfMediRecord.BeginUpdate();
                    gridControlTreatmentOfMediRecord.DataSource = null;
                    gridControlTreatmentOfMediRecord.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditReceiveTreatment_Enabed_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var focus = (MediRecordADO)gridViewMediRecord.GetFocusedRow();
                if (focus != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    MOS.Filter.HisMediRecordBorrowFilter filter = new HisMediRecordBorrowFilter();
                    filter.MEDI_RECORD_ID = focus.ID;
                    filter.IS_RECEIVE = false;
                    var mediRecordBorrows = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_RECORD_BORROW>>("api/HisMediRecordBorrow/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                    if (mediRecordBorrows != null && mediRecordBorrows.Count > 0)
                    {
                        //HIS_MEDI_RECORD_BORROW treatmentBorrow = new HIS_MEDI_RECORD_BORROW();
                        //treatmentBorrow.MEDI_RECORD_ID = focus.ID;
                        //treatmentBorrow.ID = mediRecordBorrows.FirstOrDefault().ID;
                        var listBorrows = mediRecordBorrows.OrderByDescending(x => x.ID).FirstOrDefault();
                        if (listBorrows != null)
                        {
                            HIS_MEDI_RECORD_BORROW treatmentBorrow = new HIS_MEDI_RECORD_BORROW();
                            treatmentBorrow.MEDI_RECORD_ID = focus.ID;
                            treatmentBorrow.ID = listBorrows.ID;

                            WaitingManager.Show();
                            var rsApi = new BackendAdapter(param).Post<HIS_MEDI_RECORD_BORROW>("api/HisMediRecordBorrow/Receive", ApiConsumer.ApiConsumers.MosConsumer, treatmentBorrow, param);
                            WaitingManager.Hide();

                            if (rsApi != null)
                            {
                                success = true;
                                btnSearchMediRecord_Click(null, null);
                            }

                            #region Hien thi message thong bao
                            MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                            SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("mediRecordBorrows null");
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditDeleteEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var focus = (TreatmentADO)gridViewTreatmentOfMediRecord.GetFocusedRow();
                if (focus != null && DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanCoMuonXoaDuLieuKhong, ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    HIS_TREATMENT treatmentBorrow = new HIS_TREATMENT();
                    treatmentBorrow.ID = focus.ID;
                    treatmentBorrow.STORE_CODE = null;
                    WaitingManager.Show();
                    var rsApi = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/OutOfMediRecord", ApiConsumer.ApiConsumers.MosConsumer, treatmentBorrow, param);
                    WaitingManager.Hide();

                    if (rsApi != null)
                    {
                        success = true;
                        gridViewTreatmentOfMediRecord.DeleteRow(gridViewTreatmentOfMediRecord.FocusedRowHandle);
                        LoadDataToGridControlTreatment();
                        LoadDataToGridControlMediRecord();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentOfMediRecord_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var row = (TreatmentADO)gridViewTreatmentOfMediRecord.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (row.TDL_HEIN_CARD_NUMBER != null)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentOfMediRecord_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var mediRecord = (MediRecordADO)gridViewMediRecord.GetFocusedRow();
                    if (mediRecord != null)
                    {
                        if (e.Column.FieldName == "Delete")
                        {
                            e.RepositoryItem = (!mediRecord.GIVE_DATE.HasValue || (mediRecord.GIVE_DATE.HasValue && mediRecord.RECEIVE_DATE.HasValue)) ? ButtonEditDeleteEnable : ButtonEditDeleteDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCodeMediRecord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearchMediRecord_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnViewTreatment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (TreatmentADO)gridViewTreatment.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.TREATMENT_CODE);
                    if (this.currentModule != null)
                    {
                        CallModule callModule = new CallModule(CallModule.EmrDocument, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.EmrDocument, 0, 0, listArgs);
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditImport_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var focus = (MediRecordADO)gridViewMediRecord.GetFocusedRow();
                if (focus != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    WaitingManager.Show();
                    HisMediRecordStoreSDO sdo = new HisMediRecordStoreSDO();
                    sdo.id = focus.ID;
                    sdo.LocationStoreId = focus.LOCATION_STORE_ID;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    var rsApi = new BackendAdapter(param).Post<HIS_MEDI_RECORD>("api/HisMediRecord/Store", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                    WaitingManager.Hide();

                    if (rsApi != null)
                    {
                        success = true;
                        btnSearchMediRecord_Click(null, null);
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditUnImport_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var focus = (MediRecordADO)gridViewMediRecord.GetFocusedRow();
                if (focus != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    WaitingManager.Show();
                    var rsApi = new BackendAdapter(param).Post<HIS_MEDI_RECORD>("api/HisMediRecord/UnStore", ApiConsumer.ApiConsumers.MosConsumer, focus.ID, param);
                    WaitingManager.Hide();

                    if (rsApi != null)
                    {
                        success = true;
                        btnSearchMediRecord_Click(null, null);
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeywordMediRecord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearchMediRecord_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_ViewEmrTreatment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MediRecordADO)gridViewMediRecord.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    List<string> treatmentCodes = row.TDL_TREATMENT_CODES.Split(',').ToList();
                    listArgs.Add(treatmentCodes);
                    listArgs.Add(true);
                    if (this.currentModule != null)
                    {
                        CallModule callModule = new CallModule(CallModule.EmrDocument, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.EmrDocument, 0, 0, listArgs);
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkAutoShowStore_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_AUTO_SHOW_STORE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (checkAutoShowStore.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_AUTO_SHOW_STORE;
                    csAddOrUpdate.VALUE = (checkAutoShowStore.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCodeSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataToGridControlTreatment();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewTreatment.PostEditor();
                TreatmentADO data = (TreatmentADO)gridViewTreatment.GetFocusedRow();
                if (checkAutoShowStore.Checked && data != null && data.CheckTreatment)
                {
                    List<TreatmentADO> listCheckTreatment = new List<TreatmentADO>();
                    var listDataSourceTreatment = (List<TreatmentADO>)gridControlTreatment.DataSource;
                    listCheckTreatment = listDataSourceTreatment.Where(o => o.CheckTreatment).ToList();

                    if (listCheckTreatment != null && listCheckTreatment.Count > 0)
                    {
                        HIS.Desktop.Plugins.MedicalStoreV2.ChooseStore.frmSaveStore frmChooseStore = new ChooseStore.frmSaveStore(this.currentModule, listCheckTreatment, RefreshDataAfterSave, currentControlStateRDO, controlStateWorker);
                        frmChooseStore.ShowDialog();
                        txtPatientCodeSearch.Focus();
                        txtPatientCodeSearch.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediRecordType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMediRecordType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCodeMediRecord_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearchMediRecord_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoStatus__All_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoStatus__All.Checked)
                {
                    rdoStatus__UnStored.Checked = false;
                    rdoStatus__Stored.Checked = false;
                    btnSearchMediRecord_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoStatus__Stored_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoStatus__UnStored.Checked)
                {
                    rdoStatus__All.Checked = false;
                    rdoStatus__Stored.Checked = false;
                    btnSearchMediRecord_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoStatus__UnStored_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoStatus__Stored.Checked)
                {
                    rdoStatus__All.Checked = false;
                    rdoStatus__UnStored.Checked = false;
                    btnSearchMediRecord_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImportStore_Click(object sender, EventArgs e)
        {
            try
            {
                var mediRecordList = (List<MediRecordADO>)gridControlMediRecord.DataSource;

                if (mediRecordList != null && mediRecordList.Count > 0)
                {
                    var mediRecordCheck = mediRecordList.Where(o => o.CheckStore).ToList();

                    if (mediRecordCheck != null && mediRecordCheck.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("btnImportStore_Click mediRecordCheck ", mediRecordCheck));
                        CommonParam param = new CommonParam();

                        var mediRecordIdList = mediRecordCheck.Select(o => new { o.ID, o.LOCATION_STORE_ID }).Distinct().ToList();
                        bool result = false;
                        List<HIS_MEDI_RECORD> mediRecordResults = null;
                        List<HisMediRecordStoreSDO> lstSdo = new List<HisMediRecordStoreSDO>();
                        foreach (var item in mediRecordIdList)
                        {
                            HisMediRecordStoreSDO sdo = new HisMediRecordStoreSDO();
                            sdo.id = item.ID;
                            sdo.LocationStoreId = item.LOCATION_STORE_ID;
                            lstSdo.Add(sdo);
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSdo), lstSdo));
                        mediRecordResults = new BackendAdapter(param).Post<List<HIS_MEDI_RECORD>>("api/HisMediRecord/StoreList", ApiConsumers.MosConsumer, lstSdo, param);
                        if (mediRecordResults != null && mediRecordResults.Count > 0)
                        {
                            result = true;
                            btnSearchMediRecord_Click(null, null);
                        }

                        #region Show message
                        MessageManager.Show(this.ParentForm, param, result);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                    else
                        MessageManager.Show(ResourceMessage.VuiLongChonHoSoCanLuuTru);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_RejectStore_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (TreatmentADO)gridViewTreatment.GetFocusedRow();
                if (row != null)
                {
                    frmRejectStore frm = new frmRejectStore(this.currentModule, row, refreshDataTreatment);
                    frm.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void refreshDataTreatment(object data)
        {
            try
            {
                if (data != null)
                {
                    this.btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_UnRejectStore_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var row = (TreatmentADO)gridViewTreatment.GetFocusedRow();
                if (row != null)
                {
                    if (MessageBox.Show(ResourceMessage.BanCothucSuMuonHuyTuChoiDuyetBenhAn, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        HisTreatmentRejectStoreSDO treatmentStoreSDO = new HisTreatmentRejectStoreSDO();
                        treatmentStoreSDO.TreatmentId = row.ID;
                        bool result = false;
                        HIS_TREATMENT treatmentResult = null;
                        treatmentResult = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UnrejectStore", ApiConsumers.MosConsumer, treatmentStoreSDO, param);

                        if (treatmentResult != null)
                        {
                            result = true;
                            this.btnSearch_Click(null, null);
                        }

                        #region Show message

                        MessageManager.Show(this.ParentForm, param, result);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var row = (TreatmentADO)gridViewTreatment.GetRow(e.RowHandle);
                    if (row != null)
                    {
                        if (e.Column.FieldName == "REJECT_STORE_STT")
                        {
                            if (row.APPROVAL_STORE_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.APPROVAL_STORE_STT_ID__TU_CHOI)
                                e.RepositoryItem = repositoryItemButtonEdit_UnRejectStore;
                            else
                                e.RepositoryItem = repositoryItemButtonEdit_RejectStore;
                        }
                        else if (e.Column.FieldName == "CheckTreatment")
                        {
                            if (row.APPROVAL_STORE_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.APPROVAL_STORE_STT_ID__TU_CHOI)
                            {
                                e.RepositoryItem = repositoryItemCheckEdit_Store__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheckEdit3;
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

        private void checkJustShowTreatmentLatch_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_JUST_SHOW_TREATMENT_LATCH && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (checkJustShowTreatmentLatch.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_JUST_SHOW_TREATMENT_LATCH;
                    csAddOrUpdate.VALUE = (checkJustShowTreatmentLatch.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                LoadDataToGridControlTreatment();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatusEnd_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboStatusEnd.EditValue != null && Convert.ToInt16(cboStatusEnd.EditValue) == 2)
                {
                    dtOutTimeFrom.EditValue = null;
                    dtOutTimeTo.EditValue = null;
                    ResetCombo(cboEndDepartment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void btnExportExcel_Click(object sender, EventArgs e)
		{
			try
			{
                CommonParam paramCommon = new CommonParam();
                HisMediRecordView2Filter filter = new HisMediRecordView2Filter();
                SetFilterMediRecord2(ref filter);
                var apiResult = new BackendAdapter(paramCommon).Get<List<V_HIS_MEDI_RECORD_2>>("api/HisMediRecord/GetView2", ApiConsumers.MosConsumer, filter, paramCommon);
                Export(apiResult);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}
        private void SetFilterMediRecord2(ref HisMediRecordView2Filter filter)
		{
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                if (!string.IsNullOrEmpty(txtTreatmentCodeMediRecord.Text))
                {
                    string code = txtTreatmentCodeMediRecord.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCodeMediRecord.Text = code;
                    }
                    filter.TREATMENT_CODE = code;
                }
                if (!string.IsNullOrEmpty(txtPatientCodeMediRecord.Text))
                {
                    string code = txtPatientCodeMediRecord.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCodeMediRecord.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }
                if (!string.IsNullOrEmpty(txtStoreCodeMediRecord.Text))
                {
                    filter.STORE_CODE__EXACT = txtStoreCodeMediRecord.Text.Trim();
                }
                if (!string.IsNullOrEmpty(txtKeywordMediRecord.Text))
                {
                    string code = txtKeywordMediRecord.Text.Trim();
                    filter.KEY_WORD = code;
                }

                if (dtStoreTime_From.EditValue != null)
                {
                    filter.STORE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtStoreTime_From.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtStoreTime_To.EditValue != null)
                {
                    filter.STORE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtStoreTime_To.DateTime.ToString("yyyyMMdd") + "235959");
                }
                if (_ProgramSelects != null && _ProgramSelects.Count > 0)
                {
                    filter.PROGRAM_IDs = _ProgramSelects.Select(o => o.ID).ToList();
                }
                if (rdoStatus__UnStored.Checked)
                {
                    filter.IS_NOT_STORED = true;
                }
                else if (rdoStatus__Stored.Checked)
                {
                    filter.IS_NOT_STORED = false;
                }
                else
                {
                    filter.IS_NOT_STORED = null;
                }
                if (cboMediRecordType.EditValue != null)
                {
                    filter.MEDI_RECORD_TYPE_ID = Convert.ToInt64(cboMediRecordType.EditValue);
                }
                List<DataStoreADO> dataStore = new List<DataStoreADO>();
                var binding = (BindingList<DataStoreADO>)treeListMedicalStore.DataSource;
                if (binding != null && binding.Count > 0)
                    dataStore = binding.ToList();

                var dataStoreIds = dataStore != null ? dataStore.Where(o => o.CheckStore).Select(o => o.ID).ToList() : null;

                if (dataStoreIds != null && dataStoreIds.Count > 0)
                {
                    filter.DATA_STORE_IDs = dataStoreIds;
                }
                else
                {
                    filter.DATA_STORE_ID = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Export(List<V_HIS_MEDI_RECORD_2> data)
        {
            try
            {
                Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CreateExport));
                thread.Priority = ThreadPriority.Normal;
                thread.IsBackground = true;
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                try
                {
                    thread.Start(data);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void CreateExport(object obj)
		{
			try
			{
                List<V_HIS_MEDI_RECORD_2> export = obj as List<V_HIS_MEDI_RECORD_2>;
                List<string> expCode = new List<string>();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "EXPORT_TUBENHAN.xlsx");

                //chọn đường dẫn
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.KhongTimThayFile, templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BieuMauDangMoHoacKhongTonTaiFile, templateFile));
                        return;
                    }

                    ProcessData(export, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.XuatFileThanhCong);

                                if (MessageBox.Show(ResourceMessage.BanCoMuonMoFile,
                                    ResourceMessage.ThongBao, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.XuLyThatBai);
                    }
                }
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

        private void ProcessData(List<V_HIS_MEDI_RECORD_2> data, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.SetCommonFunctions();
                List<MediRecordADO2> lst = new List<MediRecordADO2>();
				foreach (var item in data)
				{
                    MediRecordADO2 ado = new MediRecordADO2(item);
                    lst.Add(ado);
                }
                objectTag.AddObjectData(store, "MedicalStore", lst);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }
        private void LoadComboLocationStore()
        {

            try
            {
                lstLocationStore = BackendDataWorker.Get<HIS_LOCATION_STORE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOCATION_STORE_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("LOCATION_STORE_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("LOCATION_STORE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboLocationStore, lstLocationStore, controlEditorADO);
                cboLocationStore.Properties.ImmediatePopup = true;
                cboLocationStore.Properties.PopupFormSize = new Size(350, cboLocationStore.Properties.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void cboLocationStore_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboLocationStore.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridViewMediRecord_ShownEditor(object sender, EventArgs e)
        {

            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MediRecordADO data = view.GetFocusedRow() as MediRecordADO;
                if (view.FocusedColumn.FieldName == "LOCATION_STORE_ID" && view.ActiveEditor is GridLookUpEdit && data.IS_NOT_STORED == 1)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    LoadComboLocationGrid(editor);
                    editor.EditValue = data.LOCATION_STORE_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadComboLocationGrid(GridLookUpEdit editor)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOCATION_STORE_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("LOCATION_STORE_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("LOCATION_STORE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(editor, lstLocationStore, controlEditorADO);
                editor.Properties.ImmediatePopup = true;
                editor.Properties.PopupFormSize = new Size(350, editor.Properties.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repLocationStore_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    var data = (MediRecordADO)gridViewMediRecord.GetFocusedRow();
                    if (data != null)
                    {
                        data.LOCATION_STORE_ID = null;
                    }
                    gridViewMediRecord.GridControl.RefreshDataSource();
                    gridViewMediRecord.FocusedRowHandle = gridViewMediRecord.FocusedRowHandle - 1;
                    gridViewMediRecord.FocusedRowHandle = gridViewMediRecord.FocusedRowHandle + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repLocationStore_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit gridLookUpEdit = (GridLookUpEdit)sender;
                var data = (MediRecordADO)gridViewMediRecord.GetFocusedRow();
                if (data != null)
                {
                    data.LOCATION_STORE_ID = Int64.Parse(gridLookUpEdit.EditValue.ToString());
                }
                gridViewMediRecord.GridControl.RefreshDataSource();
                gridViewMediRecord.FocusedRowHandle = gridViewMediRecord.FocusedRowHandle - 1;
                gridViewMediRecord.FocusedRowHandle = gridViewMediRecord.FocusedRowHandle + 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExecuteDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExecuteDepartment.EditValue = null;
                    this._ExecuteDepartmentSelects = null;
                    ResetCombo(cboExecuteDepartment);
                    cboStatusEnd.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteDepartment_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string department = "";
                if (_ExecuteDepartmentSelects != null && _ExecuteDepartmentSelects.Count > 0)
                {
                    foreach (var item in _ExecuteDepartmentSelects)
                    {
                        department += item.DEPARTMENT_NAME + ", ";
                    }
                    //cboStatusEnd.Enabled = false;
                    //cboStatusEnd.EditValue = 1;
                }
                else
                {
                    cboStatusEnd.Enabled = true;
                }

                e.DisplayText = department;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditInventory_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (TreatmentADO)gridViewTreatment.GetFocusedRow();
                MRSummaryDetailADO ado = new MRSummaryDetailADO();
                ado.TreatmentId = data.ID;
                ado.processType = MRSummaryDetailADO.OpenFrom.MedicalStoreV2;
                List<object> listArgs = new List<object>();
                listArgs.Add(ado);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.MRSummaryList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = gridViewTreatment.GetVisibleRowHandle(hi.RowHandle);

                    var currentRowTreatment = (TreatmentADO)gridViewTreatment.GetRow(rowHandle);

                    gridViewTreatment.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewTreatment.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }

                    var listDataSourceTreatment = (List<TreatmentADO>)gridControlTreatment.DataSource;

                    PopupMenuProcessor popupMenuProcessor = new PopupMenuProcessor(currentRowTreatment, barManager1, Treatment_MouseRightClick, (RefeshReference)BtnRefreshs);
                    popupMenuProcessor.InitMenuTreat();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

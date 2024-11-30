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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.Library.CacheClient.ControlState;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
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

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    public partial class frmSettingConfigSync : HIS.Desktop.Utility.FormBase
    {
        List<HIS_BRANCH> branchSelecteds;
        List<HIS_PATIENT_TYPE> patientTypeSelecteds;
        List<HIS_PATIENT_TYPE> patientTypeTTSelecteds;
        List<HIS_TREATMENT_TYPE> treatmentTypeSelecteds;
        ConfigSyncADO configSync;
        Action<ConfigSyncADO> actAfterSave;
        bool autoSyncIsRunning;
        public frmSettingConfigSync()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmSettingConfigSync(ConfigSyncADO configSync, bool autoSyncIsRunning, Action<ConfigSyncADO> actAfterSave)
        {
            try
            {
                InitializeComponent();

                this.configSync = configSync;
                this.autoSyncIsRunning = autoSyncIsRunning;
                this.actAfterSave = actAfterSave;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmSettingConfigSync_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                SetCaptionByLanguageKey();
                InitCombobox();
                SetDefaultValue();
                
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExportXmlQD130.Resources.Lang", typeof(frmSettingConfigSync).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkOutTime.Properties.Caption = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.chkOutTime.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkFeeLockTime.Properties.Caption = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.chkFeeLockTime.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboLoaiHS.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.cboLoaiHS.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBranch.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.cboBranch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBranch.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.lciBranch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.lciPatientType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientType.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.lciPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.lciTreatmentType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLoaiHS.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.lciLoaiHS.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPeriod.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.lciPeriod.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientTypeTT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.cboPatientTypeTT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSettingConfigSync.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCombobox()
        {
            InitCboBranch();
            InitCboPatientType();
            InitCboPatientTypeTT();
            InitCboTreatmentType();
            InitComboLoaiHS();
        }
        public void InitComboLoaiHS()
        {
            try
            {
                List<FilterTypeADO> ListStatusAll = new List<FilterTypeADO>();
                FilterTypeADO tatCa = new FilterTypeADO(0, Resources.ResourceMessageLang.TatCa);
                ListStatusAll.Add(tatCa);

                FilterTypeADO duyetBhyt = new FilterTypeADO(1, Resources.ResourceMessageLang.DaKhoaBHYT);
                ListStatusAll.Add(duyetBhyt);

                FilterTypeADO ketthuc = new FilterTypeADO(2, Resources.ResourceMessageLang.DaKTDieuTri);
                ListStatusAll.Add(ketthuc);

                FilterTypeADO dacosovaovien = new FilterTypeADO(3, Resources.ResourceMessageLang.DaCoSoVaoVien);
                ListStatusAll.Add(dacosovaovien);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "id", columnInfos, false, 250);
                ControlEditorLoader.Load(cboLoaiHS, ListStatusAll, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCboBranch()
        {
            InitCheck(cboBranch, SelectionGrid__cboBranch);
            InitCombo(cboBranch, BackendDataWorker.Get<HIS_BRANCH>(), "BRANCH_NAME", "ID");
        }
        private void InitCboPatientType()
        {
            InitCheck(cboPatientType, SelectionGrid__cboPatientType);
            InitCombo(cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "PATIENT_TYPE_NAME", "ID");
        }
        private void InitCboPatientTypeTT()
        {
            InitCheck(cboPatientTypeTT, SelectionGrid__cboPatientTypeTT);
            InitCombo(cboPatientTypeTT, BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "PATIENT_TYPE_NAME", "ID");
        }
        private void InitCboTreatmentType()
        {
            InitCheck(cboTreatmentType, SelectionGrid__cboTreatmentType);
            InitCombo2(cboTreatmentType, BackendDataWorker.Get<HIS_TREATMENT_TYPE>().ToList(), "TREATMENT_TYPE_CODE", "TREATMENT_TYPE_NAME", "ID");
        }
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col1.VisibleIndex = 1;
                col1.Width = 200;
                col1.Caption = Resources.ResourceMessageLang.TatCa;

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
        private void InitCombo2(GridLookUpEdit cbo, object data, string DisplayValue1, string DisplayValue2, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue2;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField(DisplayValue1);
                col1.VisibleIndex = 1;
                col1.Width = 80;
                col1.Caption = "ALL";

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue2);
                col2.VisibleIndex = 2;
                col2.Width = 200;
                col2.Caption = Resources.ResourceMessageLang.TatCa;

                cbo.Properties.PopupFormWidth = 280;
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

        private void SelectionGrid__cboBranch(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_BRANCH> sgSelectedNews = new List<HIS_BRANCH>();
                    foreach (HIS_BRANCH rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.BRANCH_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.branchSelecteds = new List<HIS_BRANCH>();
                    this.branchSelecteds.AddRange(sgSelectedNews);
                }

                this.cboBranch.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboTreatmentType(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_TREATMENT_TYPE> sgSelectedNews = new List<HIS_TREATMENT_TYPE>();
                    foreach (HIS_TREATMENT_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.treatmentTypeSelecteds = new List<HIS_TREATMENT_TYPE>();
                    this.treatmentTypeSelecteds.AddRange(sgSelectedNews);
                }

                this.cboTreatmentType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboPatientType(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> sgSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (HIS_PATIENT_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.patientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                    this.patientTypeSelecteds.AddRange(sgSelectedNews);
                }

                this.cboPatientType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboPatientTypeTT(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> sgSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (HIS_PATIENT_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.patientTypeTTSelecteds = new List<HIS_PATIENT_TYPE>();
                    this.patientTypeTTSelecteds.AddRange(sgSelectedNews);
                }

                this.cboPatientTypeTT.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessSelectBranch(List<long> branchIds)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboBranch.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBranch.Properties.View);
                }
                if (branchIds != null && branchIds.Count > 0 && cboBranch.Properties.Tag != null)
                {
                    List<HIS_BRANCH> ds = cboBranch.Properties.DataSource as List<HIS_BRANCH>;

                    List<HIS_BRANCH> selects = new List<HIS_BRANCH>();
                    foreach (var item in branchIds)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
                else
                {
                    cboBranch.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboBranch.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusinessCodes != null)
                    {
                        gridCheckMarkBusinessCodes.ClearSelection(cboBranch.Properties.View);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessSelectPatientType(List<long> patientTypeIds)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPatientType.Properties.View);
                }
                if (patientTypeIds != null && patientTypeIds.Count > 0 && cboPatientType.Properties.Tag != null)
                {
                    List<HIS_PATIENT_TYPE> ds = cboPatientType.Properties.DataSource as List<HIS_PATIENT_TYPE>;

                    List<HIS_PATIENT_TYPE> selects = new List<HIS_PATIENT_TYPE>();
                    foreach (var item in patientTypeIds)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
                else
                {
                    cboPatientType.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusinessCodes != null)
                    {
                        gridCheckMarkBusinessCodes.ClearSelection(cboPatientType.Properties.View);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessSelectPatientTypeTT(List<long> patientTypeIds)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboPatientTypeTT.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPatientTypeTT.Properties.View);
                }
                if (patientTypeIds != null && patientTypeIds.Count > 0 && cboPatientTypeTT.Properties.Tag != null)
                {
                    List<HIS_PATIENT_TYPE> ds = cboPatientTypeTT.Properties.DataSource as List<HIS_PATIENT_TYPE>;

                    List<HIS_PATIENT_TYPE> selects = new List<HIS_PATIENT_TYPE>();
                    foreach (var item in patientTypeIds)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
                else
                {
                    cboPatientTypeTT.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboPatientTypeTT.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusinessCodes != null)
                    {
                        gridCheckMarkBusinessCodes.ClearSelection(cboPatientTypeTT.Properties.View);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessSelectTreatmentType(List<long> treatmentTypeIds)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboTreatmentType.Properties.View);
                }
                if (treatmentTypeIds != null && treatmentTypeIds.Count > 0 && cboTreatmentType.Properties.Tag != null)
                {
                    List<HIS_TREATMENT_TYPE> ds = cboTreatmentType.Properties.DataSource as List<HIS_TREATMENT_TYPE>;

                    List<HIS_TREATMENT_TYPE> selects = new List<HIS_TREATMENT_TYPE>();
                    foreach (var item in treatmentTypeIds)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
                else
                {
                    cboTreatmentType.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusinessCodes != null)
                    {
                        gridCheckMarkBusinessCodes.ClearSelection(cboTreatmentType.Properties.View);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultValue()
        {
            try
            {
                chkFeeLockTime.Checked = true;
                if (configSync != null)
                {
                    ProcessSelectBranch(configSync.branchIds);
                    ProcessSelectPatientType(configSync.patientTypeIds);
                    ProcessSelectTreatmentType(configSync.treatmentTypeIds);
                    ProcessSelectPatientTypeTT(configSync.patientTypeTTIds);
                    cboLoaiHS.EditValue = configSync.statusId;
                    spnPeriod.Value = configSync.period;
                    spnPeriod.Enabled = !autoSyncIsRunning;
                    chkOutTime.Checked = configSync.isCheckOutTime;
                    chkCollinearXML.Checked = configSync.isCheckCollinearXml;
                    chkXML3176.Checked = configSync.isXML3176;
                    chkDontSend.Enabled = false;
                    chkDontSend.Checked = configSync.dontSend;
                    txtFolder.Text = configSync.folderPath;
                }
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
                if (spnPeriod.Value <= 0)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.ChuKyPhaiLonHon0);
                    return;
                }
                if (this.branchSelecteds != null && this.branchSelecteds.Count > 0)
                {
                    this.configSync.branchIds = branchSelecteds.Select(o => o.ID).ToList();
                }
                else
                {
                    this.configSync.branchIds = null;
                }
                if (this.patientTypeSelecteds != null && this.patientTypeSelecteds.Count > 0)
                {
                    this.configSync.patientTypeIds = patientTypeSelecteds.Select(o => o.ID).ToList();
                }
                else
                {
                    this.configSync.patientTypeIds = null;
                }
                if (this.patientTypeTTSelecteds != null && this.patientTypeTTSelecteds.Count > 0)
                {
                    this.configSync.patientTypeTTIds = patientTypeTTSelecteds.Select(o => o.ID).ToList();
                }
                else
                {
                    this.configSync.patientTypeTTIds = null;
                }
                if (this.treatmentTypeSelecteds != null && this.treatmentTypeSelecteds.Count > 0)
                {
                    this.configSync.treatmentTypeIds = treatmentTypeSelecteds.Select(o => o.ID).ToList();
                }
                else
                {
                    this.configSync.treatmentTypeIds = null;
                }
                if (cboLoaiHS.EditValue != null)
                    this.configSync.statusId = (int)cboLoaiHS.EditValue;
                else
                    this.configSync.statusId = null;
                this.configSync.period = spnPeriod.Value;
                this.configSync.isCheckOutTime = chkOutTime.Checked;
                this.configSync.isCheckCollinearXml = chkCollinearXML.Checked;
                this.configSync.isXML3176 = chkXML3176.Checked;
                #region bo sung folder luu
                this.configSync.folderPath = txtFolder.Text;
                this.configSync.dontSend = chkDontSend.Checked;
                #endregion
                if (this.actAfterSave != null)
                {
                    this.actAfterSave(this.configSync);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBranch_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
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
                foreach (HIS_BRANCH rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.BRANCH_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
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
                foreach (HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
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

        private void cboTreatmentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
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
                foreach (HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiHS_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboLoaiHS.Properties.Buttons[1].Visible = cboLoaiHS.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLoaiHS_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboLoaiHS.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientTypeTT_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
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
                foreach (HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
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

        private void chkCollinearXML_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCollinearXML.Checked)
                {
                    chkOutTime.Checked = true;
                    chkFeeLockTime.Checked = false;
                    chkFeeLockTime.Enabled = false;
                }
                else
                {
                    chkFeeLockTime.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region xu li folder luu
        public SavePathADO savePathADO = new SavePathADO();
        private void txtFolder_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.savePathADO.pathXml = fbd.SelectedPath;
                    txtFolder.Text = this.savePathADO.pathXml;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFolder_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtFolder.Text))
                {
                    chkDontSend.Enabled = false;
                }
                else chkDontSend.Enabled = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}

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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.MedicalStore.ADO;
using HIS.Desktop.Plugins.MedicalStore.Borrow;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicalStore
{
    public partial class UCMedicalStore : UserControl
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        int startPage = 0;
        int rowCount = 0;
        int dataTotal = 0;
        List<HIS_TREATMENT_TYPE> _TreatmentTypeSelecteds;
        List<HIS_DEPARTMENT> _EndDepartmentSelects;
        double? blueConfig;
        double? orangeConfig;
        List<HIS_PATIENT_TYPE> patientTypeSelecteds;
        internal RepositoryItemCheckEdit checkEdit;
        internal HisTreatmentView9Filter _FilterLoadTree { get; set; }

        public UCMedicalStore()
        {
            InitializeComponent();
        }

        public UCMedicalStore(Inventec.Desktop.Common.Modules.Module module)
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
                checkEdit = (RepositoryItemCheckEdit)treeListMedicalStore.RepositoryItems.Add("CheckEdit");
                dtOutTimeFrom.DateTime = DateTime.Now;
                dtOutTimeTo.DateTime = DateTime.Now;
                SetCaptionByLanguageKey();
                LoadConfig();
                LoadComboStatus();
                //InitPatientTypeCheck();
                //InitComboPatientType();
                LoaddataToTreeList();
                LoadDataToGridControl();
                InitGridLookUpEdit_MultiSelect(cboTreatmentType, SelectionGrid__TreatmentType);
                InitGridLookUpEdit_MultiSelect(cboEndDepartment, SelectionGrid__EndDepartment);
                InitGridLookUpEdit_MultiSelect(cboPatientType, SelectionGrid__PatientType);
                LoadComboEndDepartment();
                LoadComboTreatmentType();
                InitCombo(cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), "PATIENT_TYPE_NAME", "ID");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadConfig()
        {
            try
            {
                string blue = HisConfigs.Get<string>("HIS.Desktop.Plugins.MedicalStore.BLUE_WARNING_STORE_TIME");
                string orange = HisConfigs.Get<string>("HIS.Desktop.Plugins.MedicalStore.ORANGE_WARNING_STORE_TIME");

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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MedicalStore.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicalStore.UCMedicalStore).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalStore.txtSearch.Properties.NullValuePrompt",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchTreatment.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalStore.txtSearchTreatment.Properties.NullValuePrompt",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicalStore.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridLookUpEdit1.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridLookUpEdit1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatusEnd.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicalStore.cboStatusEnd.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEndDepartment.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.lciEndDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.lciTreatmentType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtStoreCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalStore.txtStoreCode.Properties.NullValuePrompt",
                   Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcStoreTime.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.lcStoreTime.Text",
                   Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                HisDataStoreViewFilter filter = new HisDataStoreViewFilter();
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_DATA_STORE>>(HisRequestUriStore.HIS_DATA_STORE_GETVIEW, ApiConsumers.MosConsumer, filter, param).Where(p => p.ROOM_ID == this.currentModule.RoomId || p.ROOM_TYPE_ID == this.currentModule.RoomTypeId).ToList();

                if (currentDataStore != null && currentDataStore.Count > 0)
                {
                    var listAdo = new List<DataStoreADO>();
                    foreach (var item in currentDataStore)
                    {
                        var ado = new DataStoreADO(item);
                        if (this._FilterLoadTree != null
                            && ((this._FilterLoadTree.DATA_STORE_ID_NULL__OR__INs != null
                                && this._FilterLoadTree.DATA_STORE_ID_NULL__OR__INs.Count > 0
                                && this._FilterLoadTree.DATA_STORE_ID_NULL__OR__INs.Contains(item.ID))
                                || (this._FilterLoadTree.DATA_STORE_IDs != null
                                && this._FilterLoadTree.DATA_STORE_IDs.Count > 0
                                && this._FilterLoadTree.DATA_STORE_IDs.Contains(item.ID))
                                ))
                            ado.CheckStore = true;
                        listAdo.Add(ado);
                    }

                    var chuaLuu = new DataStoreADO();
                    chuaLuu.DATA_STORE_NAME = "Chưa lưu";
                    chuaLuu.DataStoreNameWithCountTreatment = "Chưa lưu";
                    if (this._FilterLoadTree != null
                        && this._FilterLoadTree.DATA_STORE_ID_NULL__OR__INs != null
                        && this._FilterLoadTree.DATA_STORE_ID_NULL__OR__INs.Count > 0)
                        chuaLuu.CheckStore = true;
                    chuaLuu.IsChuaLuu = true;

                    listAdo.Add(chuaLuu);

                    var binding = new BindingList<DataStoreADO>(listAdo);

                    if (currentDataStore != null)
                    {
                        treeListMedicalStore.KeyFieldName = "ID";
                        treeListMedicalStore.ParentFieldName = "PARENT_ID";
                        treeListMedicalStore.DataSource = binding;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshDataAfterSave()
        {
            try
            {
                LoaddataToTreeList();
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridControl()
        {
            try
            {
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                DataStorePaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(DataStorePaging, param, pageSize, this.gridControlTreatment);
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
                status.Add(new Status(1, "Đã kết thúc"));
                status.Add(new Status(2, "Chưa kết thúc"));

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

        private void DataStorePaging(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_9>> apiResult = null;

                HisTreatmentView9Filter filter = new HisTreatmentView9Filter();

                SetFilter(ref filter);

                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_9>>("api/HisTreatment/GetView9", ApiConsumers.MosConsumer, filter, paramCommon);
                gridControlTreatment.BeginUpdate();
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_9>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        List<TreatmentADO> listTreatmentADO = new List<TreatmentADO>();
                        if (filter.DATA_STORE_IDs != null && filter.DATA_STORE_IDs.Count > 0)
                        {
                            foreach (var item in data)
                            {
                                var ado = new TreatmentADO(item);
                                ado.CheckStore = true;
                                listTreatmentADO.Add(ado);
                            }
                        }
                        else
                        {
                            foreach (var item in data)
                            {
                                var ado = new TreatmentADO(item);
                                ado.CheckStore = false;
                                listTreatmentADO.Add(ado);
                            }
                        }

                        gridControlTreatment.DataSource = null;
                        gridControlTreatment.DataSource = listTreatmentADO;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        rowCount = 0;
                        dataTotal = 0;
                        gridControlTreatment.DataSource = null;
                    }
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
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
        
        private void SetFilter(ref HisTreatmentView9Filter filter)
        {
            try
            {
                this._FilterLoadTree = new HisTreatmentView9Filter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                if (!string.IsNullOrEmpty(txtSearchTreatment.Text))
                {
                    string code = txtSearchTreatment.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtSearchTreatment.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }
                if (!string.IsNullOrEmpty(txtStoreCode.Text))
                {
                    string code = txtStoreCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtStoreCode.Text = code;
                    }
                    filter.STORE_CODE__EXACT = code;
                }
                if (!string.IsNullOrEmpty(txtProgramCode.Text))
                {
                    string code = txtProgramCode.Text.Trim();
                    filter.PATIENT_PROGRAM_CODE = code;
                }

                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    filter.KEY_WORD = txtSearch.Text.Trim();
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
                    filter.IS_PAUSE = true;
                }

                if (this._TreatmentTypeSelecteds != null && this._TreatmentTypeSelecteds.Count > 0)
                {
                    filter.TREATMENT_TYPE_IDs = this._TreatmentTypeSelecteds.Select(o => (decimal)o.ID).Distinct().ToList();
                }
                if (patientTypeSelecteds != null && patientTypeSelecteds.Count > 0)
                {
                    filter.TDL_PATIENT_TYPE_IDs = patientTypeSelecteds.Select(o => o.ID).ToList();
                }
                //if (chkSave.Checked == true)
                //{
                //    filter.HAS_DATA_STORE = true;
                //}
                //else if (chkSave.Checked == false)
                //{
                //    filter.HAS_DATA_STORE = false;
                //}

                if (dtOutTimeFrom.EditValue != null)
                {
                    filter.OUT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtOutTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtOutTimeTo.EditValue != null)
                {
                    filter.OUT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtOutTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }
                if (dtStoreTime_From.EditValue != null)
                {
                    filter.STORE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtStoreTime_From.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtStoreTime_To.EditValue != null)
                {
                    filter.STORE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtStoreTime_To.DateTime.ToString("yyyyMMdd") + "235959");
                }
                if (dtInTimeFrom.EditValue != null)
                {
                    filter.IN_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtInTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtInTimeTo.EditValue != null)
                {
                    filter.IN_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtInTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                List<DataStoreADO> dataStore = new List<DataStoreADO>();
                var binding = (BindingList<DataStoreADO>)treeListMedicalStore.DataSource;
                if (binding != null && binding.Count > 0)
                    dataStore = binding.ToList();

                if (dataStore != null && dataStore.Count > 0)
                {
                    var dataStoreIds = dataStore.Where(o => o.CheckStore).Select(o => o.ID).ToList();
                    if (dataStoreIds != null && dataStoreIds.Count > 0)
                    {
                        var dataChuaLuu = dataStore.FirstOrDefault(p => dataStoreIds.Contains(p.ID) && p.IsChuaLuu == true);
                        if (dataChuaLuu != null)
                        {
                            filter.DATA_STORE_ID_NULL__OR__INs = dataStoreIds;
                            this._FilterLoadTree.DATA_STORE_ID_NULL__OR__INs = dataStoreIds;
                        }
                        else
                        {
                            filter.DATA_STORE_IDs = dataStoreIds;
                            this._FilterLoadTree.DATA_STORE_IDs = dataStoreIds;
                        }
                    }

                    //else
                    //{
                    //    filter.HAS_DATA_STORE = false;
                    //}
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
                    if (HisConfigs.Get<string>("MOS.HIS_TREATMENT.STORE_CODE_OPTION") == "4" && HisConfigs.Get<string>("HIS_RS.DATA_STORE.AUTO_SELECT_OPTION") == "1")
                    {
                        HIS.Desktop.Plugins.MedicalStore.ChooseStore.ChooseStore_TIM frmChooseStore = new ChooseStore.ChooseStore_TIM(this.currentModule, listCheckTreatment, RefreshDataAfterSave);
                        frmChooseStore.ShowDialog();
                    }
                    else
                    {
                        HIS.Desktop.Plugins.MedicalStore.ChooseStore.frmChooseStore frmChooseStore = new ChooseStore.frmChooseStore(this.currentModule, listCheckTreatment, RefreshDataAfterSave);
                        frmChooseStore.ShowDialog();
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn hồ sơ cần lưu trữ", "Thông báo");
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
                    LoadDataToGridControl();
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
                LoadDataToGridControl();
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
                col2.Caption = "Tất cả";
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

        public void bbtnSEARCH()
        {
            try
            {
                if (btnSearch.Enabled)
                    btnSearch_Click(null, null);
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
                if (btnSave.Enabled)
                {
                    btnSave.Focus();
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
                if (txtSearchTreatment.Enabled)
                {
                    txtSearchTreatment.Focus();
                    txtSearchTreatment.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var dataStoreID = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewTreatment.GetRowCellValue(e.RowHandle, "DATA_STORE_ID") ?? "").ToString());
                    var row = (TreatmentADO)gridViewTreatment.GetRow(e.RowHandle);
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

                        else if (e.Column.FieldName == "CheckTreatment")
                        {
                            e.RepositoryItem = row.CheckStore ? CheckDisable : CheckEnable;
                        }
                        else if (e.Column.FieldName == "TreatmentBorrow")
                        {
                            e.RepositoryItem = row.DATA_STORE_ID != null ? Btn_TreatmentBorrow_Enable : Btn_TreatmentBorrow_Disable;
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
                    TreatmentADO data = (TreatmentADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
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

        private void gridViewTreatment_RowCellStyle(object sender, RowCellStyleEventArgs e)
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

                    if (row.GIVE_DATE != null)
                    {
                        long dayLong = Inventec.Common.TypeConvert.Parse.ToInt64(row.GIVE_DATE.ToString());
                        var day = Inventec.Common.DateTime.Calculation.DifferenceDate(dayLong, Inventec.Common.DateTime.Get.Now() ?? 0);

                        if (day <= 10)
                            e.Appearance.ForeColor = Color.Green;
                        else
                            e.Appearance.ForeColor = Color.Red;
                    }


                    //12664
                    if (row.DATA_STORE_ID == null && row.FEE_LOCK_TIME != null)
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

        private void treeListMedicalStore_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {

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
                        LoadDataToGridControl();
                    }

                    //EmbeddedCheckBoxChecked(tree);
                    //throw new DevExpress.Utils.HideException();
                }
            }
        }

        private void repositoryItemCheckEdit_CheckStore_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewTreatment.Focus();
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void repositoryItemButtonEdit_Delete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (TreatmentADO)gridViewTreatment.GetFocusedRow();
                if (data != null)
                {
                    bool result = false;
                    CommonParam param = new CommonParam();
                    data.DATA_STORE_ID = null;
                    var current = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateDataStoreId", ApiConsumers.MosConsumer, data, param);
                    if (current != null)
                    {
                        result = true;
                        LoaddataToTreeList();
                        LoadDataToGridControl();
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
                var data = (TreatmentADO)gridViewTreatment.GetFocusedRow();
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
                var row = (TreatmentADO)gridViewTreatment.GetFocusedRow();
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisPatientProgram").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisPatientProgram");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.PATIENT_ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();

                        LoadDataToGridControl();
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
                var row = (TreatmentADO)gridViewTreatment.GetFocusedRow();
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentBorrowFilter filter = new HisTreatmentBorrowFilter();
                    filter.TREATMENT_ID = row.ID;
                    filter.IS_RECEIVE = false;

                    var rsApi = new BackendAdapter(param).Get<List<HIS_TREATMENT_BORROW>>("api/HisTreatmentBorrow/Get", ApiConsumers.MosConsumer, filter, param);
                    if (rsApi != null && rsApi.Count > 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Hồ sơ bệnh án đã được mượn", "Thông báo");
                    }
                    else
                    {
                        TreatmentBorrow frm = new TreatmentBorrow(row, RefreshDataAfterSave);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    LoadDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProgramCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                    LoadDataToGridControl();
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
                    cboStatusEnd.Enabled = false;
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

    }
}

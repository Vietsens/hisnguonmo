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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisServiceReReInDiffDay.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisServiceReReInDiffDay.Validtion;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;

namespace HIS.Desktop.Plugins.HisServiceReReInDiffDay.HisServiceReReInDiffDay
{
    public partial class frmHisServiceReReInDiffDay : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_SERVICE_RERE_TIME currentData;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        List<HIS_SERVICE_TYPE> lstServiceType;

        List<HIS_SERVICE> lstService = new List<HIS_SERVICE>();
        List<DayADO> LstDay;


        #endregion


        #region Construct
        public frmHisServiceReReInDiffDay(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        #region Private method
        private void frmHisServiceReReInDiffDay_Load(object sender, EventArgs e)
        {

            try
            {
                MeShow();
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

                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyWord.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Public method
        private void MeShow()
        {

            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //tao du lieu cho lstday
                loadlstday();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();

                lstService = BackendDataWorker.Get<HIS_SERVICE>();


                //Fill data into datasource combo
                FillDataToControlsForm();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidateGridLookupWithTextEdit(cboServiceTypeName, txtServiceTypeCode, dxValidationProvider1);
                ValidateGridLookupWithTextEdit(cboServiceName, txtServiceCode, dxValidationProvider1);
                ValidationTime();
                ValidFromDateAndToDate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidFromDateAndToDate()
        {
            try
            {
                ValidateDateFromAndDateTo validateTimeFromAndTimeTo = new ValidateDateFromAndDateTo();
                validateTimeFromAndTimeTo.cboFrom = cboInstructionDateFrom;
                validateTimeFromAndTimeTo.cboTo = cboInstructionDateTo;
                dxValidationProvider1.SetValidationRule(cboInstructionDateFrom, validateTimeFromAndTimeTo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationTime()
        {
            try
            {
                ValidationTime validateTimeFromAndTimeTo = new ValidationTime();
                validateTimeFromAndTimeTo.tmIntructionTimeFrom = tmIntructionTimeFrom;
                validateTimeFromAndTimeTo.tmIntructionTimeTo = tmIntructionTimeTo;
                dxValidationProvider1.SetValidationRule(tmIntructionTimeFrom, validateTimeFromAndTimeTo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void InitTabIndex()
        {

            try
            {
                //TODO
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisServiceReReInDiffDay.Resources.Lang", typeof(HIS.Desktop.Plugins.HisServiceReReInDiffDay.HisServiceReReInDiffDay.frmHisServiceReReInDiffDay).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt

                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReReInDiffDay.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, numPageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_SERVICE_RERE_TIME>> apiResult = null;

                HisServiceRereTimeFilter filter = new HisServiceRereTimeFilter();
                SetFilterNavBar(ref filter);

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_SERVICE_RERE_TIME>>(HisRequestUriStore.HIS_SERVICE_RERE_TIME_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_SERVICE_RERE_TIME>)apiResult.Data;
                    if (data != null)
                    {
                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisServiceRereTimeFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyWord.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlsForm()
        {
            InitComboServiceType();

            InitComboDay();
        }

        private void loadlstday()
        {

            try
            {
                LstDay = new List<DayADO>()
                {
                    new DayADO(1,"Chủ nhật"),
                    new DayADO(2,"Thứ 2"),
                    new DayADO(3,"Thứ 3"),
                    new DayADO(4,"Thứ 4"),
                    new DayADO(5,"Thứ 5"),
                    new DayADO(6,"Thứ 6"),
                    new DayADO(7,"Thứ 7"),
                };
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InitComboDay()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboInstructionDateFrom, LstDay, controlEditorADO);
                ControlEditorLoader.Load(cboInstructionDateTo, LstDay, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboService()
        {
            try
            {
                var data = lstService.Where(o => o.IS_ACTIVE == 1 && o.SERVICE_TYPE_ID == (long)cboServiceTypeName.EditValue).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboServiceName, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboServiceType()
        {
            try
            {
                lstServiceType = new List<HIS_SERVICE_TYPE>();

                lstServiceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboServiceTypeName, lstServiceType, controlEditorADO);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!layoutControl4.IsInitialized) return;
                layoutControl4.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl4.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl4.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region even
        private void cboServiceTypeName_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboServiceTypeName.EditValue = null;
                    txtServiceTypeCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceName_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboServiceName.EditValue = null;
                    txtServiceCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInstructionDateFrom_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboInstructionDateFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInstructionDateTo_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboInstructionDateTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmIntructionTimeFrom_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    tmIntructionTimeFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmIntructionTimeTo_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    tmIntructionTimeTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        #endregion

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_SERVICE_RERE_TIME)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetFocusEditor()
        {

            try
            {
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ChangedDataRow(HIS_SERVICE_RERE_TIME data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(HIS_SERVICE_RERE_TIME data)
        {
            try
            {
                if (data != null)
                {
                    cboServiceName.EditValue = data.SERVICE_ID;

                    var typeID = lstService.Where(o => o.ID == data.SERVICE_ID).FirstOrDefault();
                    cboServiceTypeName.EditValue = typeID.SERVICE_TYPE_ID;

                    cboInstructionDateFrom.EditValue = data.INSTR_DAY_FROM ?? null;

                    cboInstructionDateTo.EditValue = data.INSTR_DAY_TO ?? null;

                    if (!String.IsNullOrWhiteSpace(data.INSTR_TIME_FROM) && data.INSTR_TIME_FROM.Length == 4)
                        tmIntructionTimeFrom.EditValue = String.Format("{0:00}:{1:00}", data.INSTR_TIME_FROM.Substring(0, 2), data.INSTR_TIME_FROM.Substring(2, 2));
                    else
                        tmIntructionTimeFrom.EditValue = null;

                    if (!String.IsNullOrWhiteSpace(data.INSTR_TIME_TO) && data.INSTR_TIME_TO.Length == 4)
                        tmIntructionTimeTo.EditValue = String.Format("{0:00}:{1:00}", data.INSTR_TIME_TO.Substring(0, 2), data.INSTR_TIME_TO.Substring(2, 2));
                    else
                        tmIntructionTimeTo.EditValue = null;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_SERVICE_RERE_TIME data = (HIS_SERVICE_RERE_TIME)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGUnLock);

                    }

                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGDelete : btnGEnable);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_SERVICE_RERE_TIME pData = (HIS_SERVICE_RERE_TIME)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "SERVICE_NAME_STR")
                    {

                        try
                        {
                            e.Value = lstService.Where(o => o.ID == pData.SERVICE_ID).FirstOrDefault().SERVICE_NAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot dich vu SERVICE_NAME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "SERVICE_CODE_STR")
                    {

                        try
                        {
                            e.Value = lstService.Where(o => o.ID == pData.SERVICE_ID).FirstOrDefault().SERVICE_CODE;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot dich vu SERVICE_NAME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "INSTR_DAY_FROM_STR")
                    {

                        try
                        {
                            e.Value = LstDay.Where(o => o.ID == pData.INSTR_DAY_FROM).FirstOrDefault().NAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot dich vu SERVICE_NAME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "INSTR_DAY_TO_STR")
                    {

                        try
                        {
                            e.Value = LstDay.Where(o => o.ID == pData.INSTR_DAY_TO).FirstOrDefault().NAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot dich vu SERVICE_NAME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "INSTR_TIME_FROM_DISPLAY")
                    {
                        e.Value = !String.IsNullOrWhiteSpace(pData.INSTR_TIME_FROM) && pData.INSTR_TIME_FROM.Length == 4 ? String.Format("{0}:{1}", pData.INSTR_TIME_FROM.Substring(0, 2), pData.INSTR_TIME_FROM.Substring(2, 2)) : "";
                    }

                    else if (e.Column.FieldName == "INSTR_TIME_TO_DISPLAY")
                    {
                        e.Value = !String.IsNullOrWhiteSpace(pData.INSTR_TIME_TO) && pData.INSTR_TIME_TO.Length == 4 ? String.Format("{0}:{1}", pData.INSTR_TIME_TO.Substring(0, 2), pData.INSTR_TIME_TO.Substring(2, 2)) : "";
                    }
                    gridControl1.RefreshDataSource();
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_SERVICE_RERE_TIME)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceTypeName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboServiceTypeName.EditValue != null)
                    {
                        var data = lstServiceType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceTypeName.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtServiceTypeCode.Text = data.SERVICE_TYPE_CODE;
                            txtServiceCode.Focus();
                            txtServiceCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceCode.Focus();
                    txtServiceCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(txtServiceTypeCode.Text))
                    {
                        cboServiceTypeName.EditValue = null;
                        cboServiceTypeName.Focus();
                        cboServiceTypeName.ShowPopup();
                    }
                    else
                    {
                        List<HIS_SERVICE_TYPE> searchs = null;
                        var listData1 = lstServiceType.Where(o => o.IS_ACTIVE == 1).Where(o => o.SERVICE_TYPE_CODE.ToUpper().Contains(txtServiceTypeCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.SERVICE_TYPE_CODE.ToUpper() == txtServiceTypeCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtServiceTypeCode.Text = searchs[0].SERVICE_TYPE_CODE;
                            cboServiceTypeName.EditValue = searchs[0].ID;
                            txtServiceCode.Focus();
                            txtServiceCode.SelectAll();
                        }
                        else
                        {
                            cboServiceTypeName.Focus();
                            cboServiceTypeName.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(txtServiceTypeCode.Text))
                    {
                        cboServiceName.EditValue = null;
                        cboServiceName.Focus();
                        cboServiceName.ShowPopup();
                    }
                    else
                    {
                        List<HIS_SERVICE> searchs = null;
                        var listData1 = lstService.Where(o => o.IS_ACTIVE == 1).Where(o => o.SERVICE_CODE.ToUpper().Contains(txtServiceCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.SERVICE_CODE.ToUpper() == txtServiceCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtServiceCode.Text = searchs[0].SERVICE_CODE;
                            cboServiceName.EditValue = searchs[0].ID;
                            cboInstructionDateFrom.Focus();
                        }
                        else
                        {
                            cboServiceName.Focus();
                            cboServiceName.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboServiceName.EditValue != null)
                    {
                        var data = lstService.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceName.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtServiceCode.Text = data.SERVICE_CODE;
                            cboInstructionDateFrom.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboInstructionDateFrom.Focus();
                    cboInstructionDateFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInstructionDateFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboInstructionDateTo.Focus();
                    cboInstructionDateTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInstructionDateFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboInstructionDateTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInstructionDateTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    tmIntructionTimeFrom.Focus();
                    tmIntructionTimeFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInstructionDateTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    tmIntructionTimeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmIntructionTimeFrom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    tmIntructionTimeTo.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmIntructionTimeTo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else
                        btnEdit.Focus();
                    e.Handled = true;
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
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_SERVICE_RERE_TIME updateDTO = new MOS.EFMODEL.DataModels.HIS_SERVICE_RERE_TIME();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<HIS_SERVICE_RERE_TIME>(HisRequestUriStore.HIS_SERVICE_RERE_TIME_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<HIS_SERVICE_RERE_TIME>(HisRequestUriStore.HIS_SERVICE_RERE_TIME_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_SERVICE_RERE_TIME currentDTO)
        {
            try
            {
                if (cboServiceName.EditValue != null)
                    currentDTO.SERVICE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceName.EditValue ?? "0").ToString());
                if (cboInstructionDateFrom.EditValue != null)
                {
                    currentDTO.INSTR_DAY_FROM = (long)cboInstructionDateFrom.EditValue;
                }
                else 
                {
                    currentDTO.INSTR_DAY_FROM = null;
                }

                if (cboInstructionDateTo.EditValue != null)
                {
                    currentDTO.INSTR_DAY_TO = (long)cboInstructionDateTo.EditValue;
                }
                else 
                {
                    currentDTO.INSTR_DAY_TO = null;
                }

                tmIntructionTimeFrom.DeselectAll();
                if (tmIntructionTimeFrom.EditValue != null)
                    currentDTO.INSTR_TIME_FROM = String.Format("{0:00}{1:00}", tmIntructionTimeFrom.TimeSpan.Hours, tmIntructionTimeFrom.TimeSpan.Minutes);
                else
                    currentDTO.INSTR_TIME_FROM = null;

                tmIntructionTimeTo.DeselectAll();
                if (tmIntructionTimeTo.EditValue != null)
                    currentDTO.INSTR_TIME_TO = String.Format("{0:00}{1:00}", tmIntructionTimeTo.TimeSpan.Hours, tmIntructionTimeTo.TimeSpan.Minutes);
                else
                    currentDTO.INSTR_TIME_TO = null;

                currentDTO.IS_DIFFERENT_DAY = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref HIS_SERVICE_RERE_TIME currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceRereTimeFilter filter = new HisServiceRereTimeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_RERE_TIME>>(HisRequestUriStore.HIS_SERVICE_RERE_TIME_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
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
                if (cboServiceTypeName.EditValue != null)
                {
                    var data = lstServiceType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceTypeName.EditValue ?? "").ToString()));
                    if (data != null)
                    {
                        txtServiceTypeCode.Text = data.SERVICE_TYPE_CODE;
                    }

                    InitComboService();
                }
                else
                {
                    txtServiceTypeCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_SERVICE_RERE_TIME success = new HIS_SERVICE_RERE_TIME();
            bool notHandler = false;
            try
            {

                HIS_SERVICE_RERE_TIME data = (HIS_SERVICE_RERE_TIME)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_SERVICE_RERE_TIME data1 = new HIS_SERVICE_RERE_TIME();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_RERE_TIME>(HisRequestUriStore.HIS_SERVICE_RERE_TIME_CHANGE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_SERVICE_RERE_TIME success = new HIS_SERVICE_RERE_TIME();
            bool notHandler = false;
            try
            {

                HIS_SERVICE_RERE_TIME data = (HIS_SERVICE_RERE_TIME)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_SERVICE_RERE_TIME data1 = new HIS_SERVICE_RERE_TIME();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_RERE_TIME>(HisRequestUriStore.HIS_SERVICE_RERE_TIME_CHANGE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit.Enabled = false;
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_SERVICE_RERE_TIME)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {


                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_SERVICE_RERE_TIME_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            currentData = ((List<HIS_SERVICE_RERE_TIME>)gridControl1.DataSource).FirstOrDefault();


                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceName_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (cboServiceName.EditValue != null)
                {
                    var data = lstService.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceName.EditValue ?? "").ToString()));
                    if (data != null)
                    {
                        txtServiceCode.Text = data.SERVICE_CODE;
                    }
                }
                else
                {
                    txtServiceCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboInstructionDateFrom_Properties_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboInstructionDateFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInstructionDateTo_Properties_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboInstructionDateTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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




    }
}

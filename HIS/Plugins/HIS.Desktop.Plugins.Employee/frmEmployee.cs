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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.Filter;
using HIS.Desktop.Plugins.Employee;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.Controls;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraGrid.Views.Grid;
using System.Reflection;
using DevExpress.Data.Filtering;
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.Employee
{
    public partial class frmEmployee : HIS.Desktop.Utility.FormBase
    {
        public frmEmployee(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            SetIcon();
            SetCaptionByLanguageKey();
        }

        #region global
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.HIS_EMPLOYEE currentData;
        MOS.EFMODEL.DataModels.HIS_EMPLOYEE resultData;
        List<HIS_EMPLOYEE> listEmployee;
        int positionHandle = -1;
        int ActionType = -1;
        int rowCount;
        int dataTotal;
        DelegateSelectData delegateSelect = null;
        internal long id;
        int startPage;
        int limit;
        List<HIS_MEDI_STOCK> SelectMediStock = new List<HIS_MEDI_STOCK>();
        List<HIS_MEDI_STOCK> MediStockDefaut;
        #endregion

        #region private first
        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                //ucPaging.Init(loadPaging, param);
                ucPaging.Init(LoadPaging, param, numPageSize, gridControl1);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SaveProcess()
        {
            try
            {
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider.Validate())
                    return;

                CommonParam param = new CommonParam();
                bool success = false;
                MOS.EFMODEL.DataModels.HIS_EMPLOYEE updateDTO = new MOS.EFMODEL.DataModels.HIS_EMPLOYEE();

                WaitingManager.Show();

                //bool str = cbbLoginName.Text.IsNormalized(NormalizationForm.FormD);
                //if (!str)
                //{
                //	MessageBox.Show("Mã sai định dạng" + "\n" + "Không được nhập có dấu", "Thông báo");
                //	cbbLoginName.Focus();
                //	cbbLoginName.SelectAll();
                //	return;
                //}      
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    updateDTO = currentData;
                }
                var loginname = cbbLoginName.EditValue.ToString();
                var data = listEmployee.Where(o => o.LOGINNAME == loginname).FirstOrDefault();

                updateDTO.LOGINNAME = loginname;
                updateDTO.DIPLOMA = txtDipLoma.Text.Trim();

                if (cboRank.EditValue != null)
                    updateDTO.MEDICINE_TYPE_RANK = Inventec.Common.TypeConvert.Parse.ToInt64(cboRank.EditValue.ToString());
                else
                    updateDTO.MEDICINE_TYPE_RANK = null;

                if (this.spinMaxBhytServiceReqPerDay.EditValue != null)
                    updateDTO.MAX_BHYT_SERVICE_REQ_PER_DAY = Inventec.Common.TypeConvert.Parse.ToInt64(spinMaxBhytServiceReqPerDay.EditValue.ToString());
                else
                    updateDTO.MAX_BHYT_SERVICE_REQ_PER_DAY = null;

                if (checkAdmin.Checked == true)
                {
                    updateDTO.IS_ADMIN = 1;
                }
                else
                {
                    updateDTO.IS_ADMIN = null;
                }
                if (checkDoctor.Checked == true)
                {
                    updateDTO.IS_DOCTOR = 1;
                }
                else
                {
                    updateDTO.IS_DOCTOR = null;
                }
                if (cboDepartment.EditValue != null)
                {
                    updateDTO.DEPARTMENT_ID = Convert.ToInt64(cboDepartment.EditValue);
                }
                else
                {
                    updateDTO.DEPARTMENT_ID = null;
                }
                updateDTO.ACCOUNT_NUMBER = txtAccountNumber.Text;
                updateDTO.BANK = txtBank.Text;
                if (checkAllowUpdate.Checked)
                {
                    updateDTO.ALLOW_UPDATE_OTHER_SCLINICAL = 1;
                }
                else
                {
                    updateDTO.ALLOW_UPDATE_OTHER_SCLINICAL = null;
                }
                if (this.SelectMediStock != null && this.SelectMediStock.Count > 0)
                {
                    string MediStockIDs = "";
                    foreach (var item in this.SelectMediStock)
                    {
                        if (MediStockIDs.Length > 0)
                        {
                            MediStockIDs += ", " + item.ID;
                        }
                        else
                            MediStockIDs = item.ID.ToString();
                    }
                    updateDTO.DEFAULT_MEDI_STOCK_IDS = MediStockIDs;
                }
                else
                    updateDTO.DEFAULT_MEDI_STOCK_IDS = "";

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    if (data != null)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Đã tồn tại dữ liệu nhân viên trên hệ thống");
                        return;
                    }

                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>
                      (HIS.Desktop.Plugins.Employee.HisRequestUriStore.HIS_EMPLOYEE_CREATE, ApiConsumers.MosConsumer,
                      updateDTO, param);
                }
                else if (updateDTO != null)
                {
                    resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>
                      (HIS.Desktop.Plugins.Employee.HisRequestUriStore.HIS_EMPLOYEE_UPDATE, ApiConsumers.MosConsumer,
                      updateDTO, param);
                    //if (resultData != null)
                    //{
                    //  success = true;
                    //  FillDataToGridControl();
                    //}
                }
                if (resultData != null)
                {
                    BackendDataWorker.Reset<ACS_USER>();
                    success = true;
                    FillDataToGridControl();
                    btnRefresh_Click(null, null);
                    LoadEmployee();
                }
                else
                {
                    if (data != null)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Đã tồn tại dữ liệu nhân viên trên hệ thống");
                        FillDataToGridControl();
                        return;
                    }
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                cbbLoginName.Focus();
                cbbLoginName.SelectAll();
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ChangedDataRow()
        {
            try
            {
                currentData = new MOS.EFMODEL.DataModels.HIS_EMPLOYEE();
                currentData = (MOS.EFMODEL.DataModels.HIS_EMPLOYEE)gridviewFormList.GetFocusedRow();
                checkAdmin.Checked = false;
                checkDoctor.Checked = false;
                this.SelectMediStock = new List<HIS_MEDI_STOCK>();
                if (currentData != null)
                {
                    if (currentData.DEFAULT_MEDI_STOCK_IDS != null && currentData.DEFAULT_MEDI_STOCK_IDS.Length > 0)
                    {
                        string[] array = currentData.DEFAULT_MEDI_STOCK_IDS.Split(',');
                        if (array != null && array.Count() > 0)
                        {
                            for (int i = 0; i < array.Count(); i++)
                            {
                                HIS_MEDI_STOCK data = new HIS_MEDI_STOCK();
                                data = this.MediStockDefaut.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(array[i]));
                                this.SelectMediStock.Add(data);
                            }
                        }

                    }
                    GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.SelectAll(this.SelectMediStock);
                    }
                    cboMediStock.Enabled = false;
                    cboMediStock.Enabled = true;
                    cbbLoginName.EditValue = currentData.LOGINNAME;
                    cboRank.EditValue = currentData.MEDICINE_TYPE_RANK;
                    this.spinMaxBhytServiceReqPerDay.EditValue = currentData.MAX_BHYT_SERVICE_REQ_PER_DAY;
                    txtDipLoma.Text = currentData.DIPLOMA;
                    if (currentData.IS_DOCTOR != null && currentData.IS_DOCTOR == 1)
                        checkDoctor.Checked = true;
                    if (currentData.IS_ADMIN != null && currentData.IS_ADMIN == 1)
                        checkAdmin.Checked = true;
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    cboDepartment.EditValue = currentData.DEPARTMENT_ID;
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    txtAccountNumber.Text = currentData.ACCOUNT_NUMBER ?? "";
                    txtBank.Text = currentData.BANK ?? "";
                    if (currentData.ALLOW_UPDATE_OTHER_SCLINICAL != null && currentData.ALLOW_UPDATE_OTHER_SCLINICAL == 1)
                        checkAllowUpdate.Checked = true;
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                      (dxValidationProvider, dxErrorProvider);
                }
                cbbLoginName.Focus();
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
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultValue()
        {
            try
            {
                cbbLoginName.Focus();
                cbbLoginName.SelectAll();
                txtSearch.Text = "";
                LoadEmployee();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadEmployee()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisEmployeeFilter filter = new HisEmployeeFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtSearch.Text;

                this.listEmployee = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EMPLOYEE>>
                  (HIS.Desktop.Plugins.Employee.HisRequestUriStore.HIS_EMPLOYEE_GET, ApiConsumers.MosConsumer, filter, param);
                gridControl1.BeginUpdate();
                gridControl1.DataSource = this.listEmployee;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditInfo.IsInitialized) return;
                lcEditInfo.BeginUpdate();

                foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditInfo.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                    {
                        DevExpress.XtraEditors.BaseEdit formatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        formatFrm.ResetText();
                        formatFrm.EditValue = null;
                        cbbLoginName.Focus();
                        checkAdmin.Checked = false;
                        checkDoctor.Checked = false;
                        cboDepartment.EditValue = null;
                        txtAccountNumber.Text = "";
                        txtBank.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                lcEditInfo.EndUpdate();
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(cbbLoginName);
                ValidationGreatThanZeroControl(this.spinMaxBhytServiceReqPerDay);
                ValidationMaxlengthControl(txtAccountNumber, 50);
                ValidationMaxlengthControl(txtBank, 200);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboLoginName()
        {
            try
            {
                List<ColumnInfo> columninfos = new List<ColumnInfo>();
                columninfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columninfos.Add(new ColumnInfo("USERNAME", "", 300, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO("LOGINNAME", "LOGINNAME", columninfos, false, 450);
                ControlEditorLoader.Load(cbbLoginName, new List<ACS_USER>(), controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboRank()
        {
            try
            {
                List<RankADO> listData = new List<RankADO>();
                listData.Add(new RankADO(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__1));
                listData.Add(new RankADO(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__2));
                listData.Add(new RankADO(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__3));
                listData.Add(new RankADO(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__4));
                listData.Add(new RankADO(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.MEDICINE_TYPE_RANK__5));
                List<ColumnInfo> columninfos = new List<ColumnInfo>();

                //columninfos.Add(new ColumnInfo("LOGINNAME", "", 84, 1));
                columninfos.Add(new ColumnInfo("RANK", "", 184, 1));

                ControlEditorADO controlEditorADO = new ControlEditorADO("RANK", "ID", columninfos, false, 184);
                ControlEditorLoader.Load(cboRank, listData, controlEditorADO);
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

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.Employee.Resources.Lang", typeof(HIS.Desktop.Plugins.Employee.frmEmployee).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmEmployee.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditInfo.Text = Inventec.Common.Resource.Get.Value("frmEmployee.lcEditInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRank.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEmployee.cboRank.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmEmployee.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barSearch.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.barSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barEdit.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.barEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barAdd.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.barAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barRefresh.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.barRefresh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbbLoginName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEmployee.cbbLoginName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmEmployee.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmEmployee.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmEmployee.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkDoctor.Properties.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.checkDoctor.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkAdmin.Properties.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.checkAdmin.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmEmployee.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAllowUpdate.Text = Inventec.Common.Resource.Get.Value("frmEmployee.lciAllowUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkAllowUpdate.ToolTip = Inventec.Common.Resource.Get.Value("frmEmployee.checkAllowUpdate.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmEmployee.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmEmployee.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmEmployee.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmEmployee.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmEmployee.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmEmployee.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmEmployee.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridDefaultMediStock.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.gridDefaultMediStock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridDefaultMediStock.ToolTip = Inventec.Common.Resource.Get.Value("frmEmployee.gridDefaultMediStock.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmEmployee.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barImport.Caption = Inventec.Common.Resource.Get.Value("frmEmployee.barImport.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmEmployee.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon
                  (System.IO.Path.Combine
                  (LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitComboDepartment()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region private second
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPage, limit);

                MOS.Filter.HisEmployeeFilter filterSearch = new HisEmployeeFilter();
                filterSearch.KEY_WORD = txtSearch.Text.Trim();
                filterSearch.ORDER_FIELD = "MODIFY_TIME";
                filterSearch.ORDER_DIRECTION = "DESC";

                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>> apiResult = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                  (HIS.Desktop.Plugins.Employee.HisRequestUriStore.HIS_EMPLOYEE_GET, ApiConsumers.MosConsumer, filterSearch, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>)apiResult.Data;
                    gridviewFormList.GridControl.DataSource = data;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidationMaxlengthControl(BaseEdit control, int length)
        {
            try
            {
                ControlMaxLengthValidationRule rule = new ControlMaxLengthValidationRule();
                rule.editor = control;
                rule.maxLength = length;
                rule.ErrorText = String.Format("Độ dài không được vượt quá {0}", length);
                rule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(control, rule);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidationGreatThanZeroControl(SpinEdit control)
        {
            try
            {
                ControlGreatThanZeroValidationRule validRule = new ControlGreatThanZeroValidationRule();
                validRule.spin = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion

        #region event
        private void frmEmployee_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
                InitComboRank();
                InitComboLoginName();
                InitComboDepartment();
                LoadDataMediStock();
                InitCheck(cboMediStock, SelectionMediStock);
                InitCombo(cboMediStock, this.MediStockDefaut);
                backgroundWorker1.RunWorkerAsync();
                SetDefaultValue();
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                ValidateForm();
                txtDipLoma.Properties.Mask.EditMask = ".{20}";
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridviewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                if (backgroundWorker1.IsBusy)
                {
                    WaitingManager.Show();
                }
                else
                {
                    ChangedDataRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    MOS.EFMODEL.DataModels.HIS_EMPLOYEE data = (MOS.EFMODEL.DataModels.HIS_EMPLOYEE)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnUnlock : btnLock;
                    if (e.Column.FieldName == "DELETE")
                        e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDelete : btnUndelete;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_EMPLOYEE data = (MOS.EFMODEL.DataModels.HIS_EMPLOYEE)gridviewFormList.GetRow(e.ListSourceRowIndex);
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                if (e.Column.FieldName == "STATUS")
                    e.Value = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? "Hoạt động" : "Tạm khóa";
                if (e.Column.FieldName == "JOB")
                {
                    if (data.IS_ADMIN == 1)
                        e.Value = "Quản trị";
                    if (data.IS_DOCTOR == 1)
                        e.Value = "Bác sỹ";
                    if (data.IS_ADMIN == 1 && data.IS_DOCTOR == 1)
                        e.Value = "Quản trị, Bác sĩ";
                }
                if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(createTime));
                }
                if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    string mobdifyTime = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(mobdifyTime));
                }
                if (e.Column.FieldName == "IS_ALLOW_UPDATE_OTHER_SCLINICAL")
                {
                    e.Value = data.ALLOW_UPDATE_OTHER_SCLINICAL == 1;
                }
                if (e.Column.FieldName == "default_medi_stock_ids_STR")
                {
                    string MediStockCode = "";
                    if (data.DEFAULT_MEDI_STOCK_IDS.Length > 0)
                    {
                        string[] arrListStr = data.DEFAULT_MEDI_STOCK_IDS.Split(',');
                        for (int i = 0; i < arrListStr.Count(); i++)
                        {       
                            var dataMedi = this.MediStockDefaut.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(arrListStr[i]));
                            if (MediStockCode.Length > 0)
                            {
                                MediStockCode += "," + dataMedi.MEDI_STOCK_CODE;
                            }
                            else
                            {
                                MediStockCode = dataMedi.MEDI_STOCK_CODE;
                            }
                        }
                    }
                    e.Value = MediStockCode;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_EMPLOYEE data = (MOS.EFMODEL.DataModels.HIS_EMPLOYEE)gridviewFormList.GetRow(e.RowHandle);
                if (e.Column.FieldName == "STATUS")
                    e.Appearance.ForeColor = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.HIS_EMPLOYEE resultLock = new MOS.EFMODEL.DataModels.HIS_EMPLOYEE();
                bool notHandler = false;

                MOS.EFMODEL.DataModels.HIS_EMPLOYEE currentLock = (MOS.EFMODEL.DataModels.HIS_EMPLOYEE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //resultLock.ID = currentLock.ID;
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>
                      (HIS.Desktop.Plugins.Employee.HisRequestUriStore.HIS_EMPLOYEE_CHANGELOCK, ApiConsumers.MosConsumer, currentLock.ID, param);

                    if (resultLock != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.HIS_EMPLOYEE resultUnlock = new MOS.EFMODEL.DataModels.HIS_EMPLOYEE();
                bool notHandler = false;

                MOS.EFMODEL.DataModels.HIS_EMPLOYEE currentUnlock = (MOS.EFMODEL.DataModels.HIS_EMPLOYEE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //resultUnlock.ID = currentUnlock.ID;
                    WaitingManager.Show();
                    resultUnlock = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>(HIS.Desktop.Plugins.Employee.HisRequestUriStore.HIS_EMPLOYEE_CHANGELOCK, ApiConsumers.MosConsumer,
                      currentUnlock.ID, param);

                    if (resultUnlock != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_EMPLOYEE currentDelete = (MOS.EFMODEL.DataModels.HIS_EMPLOYEE)gridviewFormList.GetFocusedRow();
                if (currentDelete != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage
                      (LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        success = new BackendAdapter(param).Post<bool>
                        (HIS.Desktop.Plugins.Employee.HisRequestUriStore.HIS_EMPLOYEE_DELETE, ApiConsumers.MosConsumer, currentDelete.ID, param);
                        if (success)
                            FillDataToGridControl();
                        MessageManager.Show(this, param, success);
                    }
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.SelectMediStock = new List<HIS_MEDI_STOCK>();
                this.ActionType = GlobalVariables.ActionAdd;
                this.currentData = new HIS_EMPLOYEE();
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider, dxErrorProvider);
                ResetFormData();
                LoadEmployee();
                RestCombo(cboMediStock);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDipLoma.Focus();
                    txtDipLoma.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDipLoma_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRank.Focus();
                    cboRank.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkAdmin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkDoctor.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkDoctor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    if (this.ActionType == GlobalVariables.ActionEdit)
                        btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbLoginName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtDipLoma.Focus();
                txtDipLoma.SelectAll();
            }
            else
            {
                cbbLoginName.ShowPopup();
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> obj = new List<object>();
                CallModule callModule = new CallModule(CallModule.HisImportEmployee, 0, 0, obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRank_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinMaxBhytServiceReqPerDay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinMaxBhytServiceReqPerDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepartment.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRank.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRank_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboRank.Properties.Buttons[1].Visible = (cboRank.EditValue != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //CommonParam param = new CommonParam();
                //ACS.Filter.AcsUserFilter filter = new ACS.Filter.AcsUserFilter();
                //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                //var data = new BackendAdapter(param).Get<List<ACS.EFMODEL.DataModels.ACS_USER>>
                //  (HIS.Desktop.Plugins.Employee.HisRequestUriStore.ACS_USER_GET, ApiConsumers.AcsConsumer, filter, param);
                var data = BackendDataWorker.Get<ACS_USER>();
                this.Invoke(new Action(() =>
                {
                    this.cbbLoginName.Properties.DataSource = data;
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WaitingManager.Hide();
        }

        private void cbbLoginName_Properties_BeforePopup(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                WaitingManager.Show();
            }
        }

        private void cboDepartment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtAccountNumber.Focus();
                    txtAccountNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDepartment.EditValue != null)
                {
                    cboDepartment.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboDepartment.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                }
                else
                {
                    cboDepartment.ShowPopup();
                    SelectFirstRowPopup(cboDepartment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SelectFirstRowPopup(GridLookUpEdit cbo)
        {
            try
            {
                if (cbo != null && cbo.IsPopupOpen)
                {
                    DevExpress.Utils.Win.IPopupControl popupEdit = cbo as DevExpress.Utils.Win.IPopupControl;
                    DevExpress.XtraEditors.Popup.PopupLookUpEditForm popupWindow = popupEdit.PopupWindow as DevExpress.XtraEditors.Popup.PopupLookUpEditForm;
                    if (popupWindow != null)
                    {
                        popupWindow.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccountNumber_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBank.Focus();
                    txtBank.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBank_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMediStock.Focus();
                    cboMediStock.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #region ---Combo Medi Stock---
        private void SelectionMediStock(object sender, EventArgs e)
        {
            try
            {
                this.SelectMediStock = new List<HIS_MEDI_STOCK>();
                foreach (HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        SelectMediStock.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCheck(DevExpress.XtraEditors.GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
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
        private void InitCombo(DevExpress.XtraEditors.GridLookUpEdit cbo, List<HIS_MEDI_STOCK> data)
        {
            try
            {
                if (data != null)
                {
                    cbo.Properties.DataSource = data;
                    cbo.Properties.DisplayMember = "MEDI_STOCK_NAME";
                    cbo.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "Tất cả";
                    cbo.Properties.PopupFormWidth = 200;
                    cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                    cbo.Properties.View.OptionsSelection.MultiSelect = true;


                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDataMediStock()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMediStockFilter filter = new HisMediStockFilter();
                this.MediStockDefaut = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK>>("api/HisMediStock/Get", ApiConsumers.MosConsumer, filter, param);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboMediStock_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string display = "";
                foreach (var item in this.SelectMediStock)
                {
                    if (display.Trim().Length > 0)
                    {
                        display += ", " + item.MEDI_STOCK_NAME;
                    }
                    else
                        display = item.MEDI_STOCK_NAME;
                }
                e.DisplayText = display;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    checkAdmin.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void RestCombo(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection grid = cbo.Properties.Tag as GridCheckMarksSelection;
                if (grid != null)
                {
                    grid.SelectAll(null);
                }
                cboMediStock.Enabled = false;
                cboMediStock.Enabled = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}

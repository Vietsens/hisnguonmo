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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;

using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System.Resources;
using MOS.Filter;
using Inventec.Common.Logging;
using Inventec.UC.Paging;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.ADO;
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using HIS.Desktop.Plugins.ImpUserTemp.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Mapper;
using Inventec.Common.TypeConvert;
using HIS.Desktop.Plugins.ImpUserTemp.Validation;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.ImpUserTemp.ImpUserTemp
{
    public partial class frmInforImpUserTemp : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        PagingGrid pagingGrid;

        int positionHandle = -1;
        HIS_IMP_USER_TEMP currentData;

        private int dataTotal;

        List<HIS_EXECUTE_ROLE> executeRoleUsers;
        List<ACS_USER> acsUsers;
        internal List<HisImpTempUserADO> impUserAdoTemp;
        Inventec.Desktop.Common.Modules.Module moduleData;
        long IDrow;
        string name;
        private int rowCount;
        private DelegateRefreshData refeshData;
        private int startPage;
        private int actionType;
        private List<string> list = new List<string>();

        #endregion
        #region Construct
        public frmInforImpUserTemp(DelegateRefreshData _refeshData, Inventec.Desktop.Common.Modules.Module moduleData, List<String> list)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.impUserAdoTemp = new List<HisImpTempUserADO>();
                this.currentData = new HIS_IMP_USER_TEMP();
                this.moduleData = moduleData;
                this.rowCount = 0;
                this.dataTotal = 0;
                this.startPage = 0;
                this.positionHandle = -1;
                this.list = list;
                this.refeshData = _refeshData;

                try
                {
                    this.actionType = 1;
                    this.Icon = Icon.ExtractAssociatedIcon
                        (System.IO.Path.Combine
                        (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                        System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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



        public frmInforImpUserTemp(DelegateRefreshData _refeshData, Inventec.Desktop.Common.Modules.Module moduleData, long Id, string name, List<String> list)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.impUserAdoTemp = new List<HisImpTempUserADO>();
                this.currentData = new HIS_IMP_USER_TEMP();
                this.moduleData = moduleData;
                this.rowCount = 0;
                this.dataTotal = 0;
                this.startPage = 0;
                this.positionHandle = -1;
                this.list = list;
                this.IDrow = Id;
                this.name = name;
                this.refeshData = _refeshData;
                try
                {
                    this.actionType = 2;
                    this.Icon = Icon.ExtractAssociatedIcon
                        (System.IO.Path.Combine
                        (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                        System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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



        #endregion
        #region Private Method
        private void frmInforImpUserTemp_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void Show()
        {

            SetCaptionByLanguagekey();

            this.LoadDataToCombo();
            if (IDrow > 0)
            {
                FillDataToGridConTrolByID(this.IDrow, name);
            }
            else
            {
                FillDataToGridControl(this.currentData);
            }
            if (this.gridControl1.DataSource == null)
            {
                this.InitGrid();
            }
            this.ValidationSingleControl(this.txtUserName);
            ValidateMaxLength(txtImpMestNumber, 30);
            ValidateMaxLength(mmConclude, 2000);

        }
        private void ValidateMaxLength(BaseControl control, int maxlength)
        {
            try
            {
                ValidationMaxLength valid = new ValidationMaxLength();
                valid.textEdit = control;
                valid.maxLength = maxlength;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDataToCombo()
        {
            try
            {
                this.ComboAcsUser(repositoryItemSearchLookUpEdit1);
                this.ComboExecuteRole(cboExecuteRoleName);
            }
            catch (Exception exception)
            {
                Inventec.Common.Logging.LogSystem.Warn(exception);
            }
        }


        private void SetCaptionByLanguagekey()
        {
            try
            {

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ImpUserTemp.Resources.Lang", typeof(HIS.Desktop.Plugins.ImpUserTemp.ImpUserTemp.frmImpUserTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmInforImpUserTemp.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmInforImpUserTemp.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmInforImpUserTemp.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColumn1.Caption = Inventec.Common.Resource.Get.Value("frmInforImpUserTemp.grdColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColumn2.Caption = Inventec.Common.Resource.Get.Value("frmInforImpUserTemp.grdColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitGrid()
        {
            try
            {
                List<ADO.HisImpTempUserADO> list = new List<HisImpTempUserADO>();
                HisImpTempUserADO item = new HisImpTempUserADO
                {
                    Action = 1
                };
                list.Add(item);
                this.gridControl1.DataSource = list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit cbo)
        {
            try
            {
                acsUsers = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();

                cbo.DataSource = acsUsers;
                cbo.DisplayMember = "USERNAME";
                cbo.ValueMember = "LOGINNAME";
                cbo.TextEditStyle = TextEditStyles.Standard;
                cbo.PopupFilterMode = PopupFilterMode.Contains;
                cbo.ImmediatePopup = true;
                cbo.View.Columns.Clear();
                GridColumn column = cbo.View.Columns.AddField("LOGINNAME");
                column.Caption = "Mã";
                column.Visible = true;
                column.VisibleIndex = 1;
                column.Width = 100;
                GridColumn column2 = cbo.View.Columns.AddField("USERNAME");
                column2.Caption = "Tên";
                column2.Visible = true;
                column2.VisibleIndex = 2;
                column2.Width = 200;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void ComboExecuteRole(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cbo)
        {
            try
            {
                executeRoleUsers = new List<HIS_EXECUTE_ROLE>();
                executeRoleUsers = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                executeRoleUsers = executeRoleUsers.Where(dt => dt.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, executeRoleUsers, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToGridConTrolByID(long IDdata, string name)
        {
            try
            {
                List<HisImpTempUserADO> list = new List<HisImpTempUserADO>();
                CommonParam commonParam = new CommonParam();
                HisImpUserTempFilter tempFilter = new MOS.Filter.HisImpUserTempFilter();
                tempFilter.ID = IDdata;
                var rsImpUserTemp = new BackendAdapter(commonParam).Get<List<HIS_IMP_USER_TEMP>>("api/HisImpUserTemp/Get", ApiConsumers.MosConsumer, tempFilter, commonParam).FirstOrDefault();
                if (rsImpUserTemp != null)
                {
                    txtImpMestNumber.Text = rsImpUserTemp.IMP_MEST_NUMBER;
                    mmConclude.Text = rsImpUserTemp.CONCLUDE;
                }
                HisImpUserTempDtFilter filter = new MOS.Filter.HisImpUserTempDtFilter();
                filter.IMP_USER_TEMP_ID = IDdata;
                txtUserName.Text = name;
                List<V_HIS_IMP_USER_TEMP_DT> list2 = new BackendAdapter(commonParam).Get<List<V_HIS_IMP_USER_TEMP_DT>>(ImpRequestUriStore.IMP_USER_TEMP_DT_GETVIEW, ApiConsumers.MosConsumer, filter, commonParam);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => list2), list2));
                if (list2.Count > 0)
                {
                    List<ADO.HisImpTempUserADO> hisImpUserAdoTemp = new List<HisImpTempUserADO>();
                    List<string> loginNames = list2.Select(o => o.LOGINNAME).ToList();
                    AcsUserFilter acsFilter = new AcsUserFilter();
                    acsFilter.LOGINNAMEs = loginNames;
                    List<ACS_USER> isActive = new BackendAdapter(commonParam).Get<List<ACS.EFMODEL.DataModels.ACS_USER>>(AcsUserRequestUriStore.ACS_RS_GET, ApiConsumers.AcsConsumer, acsFilter, commonParam);
                    List<string> isActiveLoginName = isActive.Where(o => o.IS_ACTIVE == 1).Select(i => i.LOGINNAME).ToList();
                    foreach (var hisImpUserTemp in list2)
                    {
                        var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == hisImpUserTemp.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        HisImpTempUserADO AdoTemp = new HisImpTempUserADO();
                        AdoTemp.EXECUTE_ROLE_ID = hisImpUserTemp.EXECUTE_ROLE_ID;
                        AdoTemp.LOGINNAME = hisImpUserTemp.LOGINNAME;
                        AdoTemp.USERNAME = hisImpUserTemp.USERNAME;
                        if (list.Count == 0)
                        {
                            AdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                        else
                        {
                            AdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }
                        if (isActiveLoginName.Contains(hisImpUserTemp.LOGINNAME))
                        {
                            list.Add(AdoTemp);
                        }
                    }
                    gridControl1.DataSource = list;
                }

            }
            catch (Exception exception)
            {
                Inventec.Common.Logging.LogSystem.Warn(exception);
                WaitingManager.Hide();
            }
        }
        private void FillDataToGridControl(HIS_IMP_USER_TEMP data)
        {
            try
            {
                txtImpMestNumber.Text = data.IMP_MEST_NUMBER;
                mmConclude.Text = data.CONCLUDE;
                List<HisImpTempUserADO> list = new List<HisImpTempUserADO>();
                CommonParam commonParam = new CommonParam();
                HisImpUserTempDtFilter filter = new MOS.Filter.HisImpUserTempDtFilter();
                filter.IMP_USER_TEMP_ID = data.ID;

                List<V_HIS_IMP_USER_TEMP_DT> list2 = new BackendAdapter(commonParam).Get<List<V_HIS_IMP_USER_TEMP_DT>>(ImpRequestUriStore.IMP_USER_TEMP_DT_GETVIEW, ApiConsumers.MosConsumer, filter, commonParam);
                if (list2.Count > 0)
                {
                    foreach (V_HIS_IMP_USER_TEMP_DT v_his_imp_user_temp_dt in list2)
                    {
                        HisImpTempUserADO objDestination = new HisImpTempUserADO();
                        DataObjectMapper.Map<HisImpTempUserADO>(objDestination, v_his_imp_user_temp_dt);
                        if (v_his_imp_user_temp_dt != list2[0])
                        {
                            objDestination.Action = 2;
                        }
                        else
                        {
                            objDestination.Action = 1;
                        }
                        list.Add(objDestination);

                    }
                    this.gridControl1.DataSource = null;
                    this.gridControl1.DataSource = list;

                }
            }
            catch (Exception exception)
            {
                Inventec.Common.Logging.LogSystem.Warn(exception);
                WaitingManager.Hide();
            }

        }
        #endregion
        #region Validate

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

     

        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool flag = true;
            bool flag2 = false;
            try
            {

                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (flag)
                {
                    List<HisImpTempUserADO> dataSource;
                    List<HIS_IMP_USER_TEMP_DT> list2;
                    HIS_IMP_USER_TEMP_DT his_imp_user_temp_dt;
                    string mess = "";
                    bool isCheck = false;
                    WaitingManager.Show();
                    CommonParam commonParam = new CommonParam();
                    this.currentData.IMP_USER_TEMP_NAME = this.txtUserName.Text.Trim();
                    this.currentData.IMP_MEST_NUMBER = this.txtImpMestNumber.Text.Trim();
                    this.currentData.CONCLUDE = this.mmConclude.Text.Trim();
                    
                    HIS_IMP_USER_TEMP his_imp_temp = new HIS_IMP_USER_TEMP();
                    if (IDrow > 0)
                    {
                        this.currentData.ID = IDrow;
                    }


                    if (this.actionType == 2)
                    {
                        dataSource = this.gridControl1.DataSource as List<HisImpTempUserADO>;

                        list2 = new List<HIS_IMP_USER_TEMP_DT>();
                        if ((dataSource != null) && (dataSource.Count > 0))
                        {
                            foreach (HisImpTempUserADO rado in dataSource)
                            {

                                his_imp_user_temp_dt = new HIS_IMP_USER_TEMP_DT();

                                his_imp_user_temp_dt.LOGINNAME = rado.LOGINNAME;//=> acs_user => USERNAME
                                his_imp_user_temp_dt.EXECUTE_ROLE_ID = rado.EXECUTE_ROLE_ID;
                                var lstAcsUser = acsUsers.Where(o => o.LOGINNAME == rado.LOGINNAME).ToList();
                                if (lstAcsUser != null && lstAcsUser.Count > 0)
                                {
                                    his_imp_user_temp_dt.USERNAME = acsUsers.Where(o => o.LOGINNAME == rado.LOGINNAME).FirstOrDefault().USERNAME;
                                }

                                list2.Add(his_imp_user_temp_dt);

                            }
                            this.currentData.HIS_IMP_USER_TEMP_DT = list2;
                        }
                        
                        if (!name.Equals(txtUserName.Text.Trim()))
                        {
                            if (list.Contains(txtUserName.Text.Trim()))
                            {
                                DialogResult d = MessageBox.Show("Mẫu trùng tên. Bạn có muốn lưu?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (d == DialogResult.Yes)
                                {
                                    if (this.currentData.HIS_IMP_USER_TEMP_DT != null && this.currentData.HIS_IMP_USER_TEMP_DT.Count > 0)
                                    {
                                        var lstLoginName = this.currentData.HIS_IMP_USER_TEMP_DT.Select(o => o.LOGINNAME).Distinct().ToList();
                                        foreach (var Login in lstLoginName)
                                        {

                                            List<string> lstExecuteRoleName = new List<string>();
                                            var check = this.currentData.HIS_IMP_USER_TEMP_DT.Where(o => o.LOGINNAME == Login).ToList();
                                           
                                            if (check.FirstOrDefault().EXECUTE_ROLE_ID == 0 && string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME))
                                            {
                                                mess += "Vai trò và thành viên hội đồng mẫu không hợp lệ\n";
                                            }
                                            else
                                            {
                                                if (check.FirstOrDefault().EXECUTE_ROLE_ID == 0)
                                                {
                                                    mess += String.Format("Thành viên {0} chưa được thiết lập vai trò thực hiện.\n", check.FirstOrDefault().USERNAME);
                                                }
                                                else
                                                {
                                                    foreach (var item in check.Select(o => o.EXECUTE_ROLE_ID).ToList())
                                                    {
                                                        lstExecuteRoleName.Add(executeRoleUsers.FirstOrDefault(o => o.ID == item).EXECUTE_ROLE_NAME);
                                                    }
                                                }
                                                string messLogin = "";
                                                if (lstExecuteRoleName.Count > 0)
                                                {
                                                    messLogin += String.Join(", ", lstExecuteRoleName);
                                                }
                                                if (string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME))
                                                {
                                                    mess += String.Format("Vai trò {0} chưa được thiết lập người thực hiện.\n", messLogin);
                                                }
                                                //else
                                                //{
                                                //    if (check != null && check.Count > 1)
                                                //    {
                                                //        mess += String.Format("Thành viên {0} được thiết lập với các vai trò {1}.\n", check.FirstOrDefault().USERNAME, messLogin);
                                                //    }

                                                //}
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(mess))
                                    {
                                        WaitingManager.Hide();
                                        MessageBox.Show(mess, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }

                                    his_imp_temp = new BackendAdapter(commonParam).Post<HIS_IMP_USER_TEMP>(ImpRequestUriStore.IMP_USER_TEMP_UPDATE, ApiConsumers.MosConsumer, this.currentData, commonParam);
                                }
                                else
                                {
                                    isCheck = true;
                                    txtUserName.Focus();
                                }
                            }
                            else
                            {
                                if (this.currentData.HIS_IMP_USER_TEMP_DT != null && this.currentData.HIS_IMP_USER_TEMP_DT.Count > 0)
                                {
                                    var lstLoginName = this.currentData.HIS_IMP_USER_TEMP_DT.Select(o => o.LOGINNAME).Distinct().ToList();
                                    foreach (var Login in lstLoginName)
                                    {

                                        List<string> lstExecuteRoleName = new List<string>();
                                        var check = this.currentData.HIS_IMP_USER_TEMP_DT.Where(o => o.LOGINNAME == Login).ToList();

                                        if (check.FirstOrDefault().EXECUTE_ROLE_ID == 0 && string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME))
                                        {
                                            mess += ("Vai trò và thành viên hội đồng mẫu không hợp lệ.\n");
                                        }
                                        else
                                        {
                                            if (check.FirstOrDefault().EXECUTE_ROLE_ID == 0)
                                            {
                                                mess += String.Format("Thành viên {0} chưa được thiết lập vai trò thực hiện.\n", check.FirstOrDefault().USERNAME);
                                            }
                                            else
                                            {
                                                foreach (var item in check.Select(o => o.EXECUTE_ROLE_ID).ToList())
                                                {
                                                    lstExecuteRoleName.Add(executeRoleUsers.FirstOrDefault(o => o.ID == item).EXECUTE_ROLE_NAME);
                                                }
                                            }
                                            string messLogin = "";
                                            if (lstExecuteRoleName.Count > 0)
                                            {
                                                messLogin += String.Join(", ", lstExecuteRoleName);
                                            }
                                            if (string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME))
                                            {
                                                mess += String.Format("Vai trò {0} chưa được thiết lập người thực hiện.\n", messLogin);
                                            }
                                        }

                                    }
                                }
                                if (!string.IsNullOrEmpty(mess))
                                {
                                    WaitingManager.Hide();
                                    MessageBox.Show(mess, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                his_imp_temp = new BackendAdapter(commonParam).Post<HIS_IMP_USER_TEMP>(ImpRequestUriStore.IMP_USER_TEMP_UPDATE, ApiConsumers.MosConsumer, this.currentData, commonParam);

                            }                                                
                        }
                        else if (list.Contains(txtUserName.Text.Trim()) && name.Equals(txtUserName.Text.Trim()))
                        {
                            if (this.currentData.HIS_IMP_USER_TEMP_DT != null && this.currentData.HIS_IMP_USER_TEMP_DT.Count > 0)
                            {
                                var lstLoginName = this.currentData.HIS_IMP_USER_TEMP_DT.Select(o => o.LOGINNAME).Distinct().ToList();
                                foreach (var Login in lstLoginName)
                                {

                                    List<string> lstExecuteRoleName = new List<string>();
                                    var check = this.currentData.HIS_IMP_USER_TEMP_DT.Where(o => o.LOGINNAME == Login).ToList();

                                    if (check.FirstOrDefault().EXECUTE_ROLE_ID == 0 && string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME))
                                    {
                                        mess += ("Vai trò và thành viên hội đồng mẫu không hợp lệ.\n");
                                    }
                                    else
                                    {
                                        if (check.FirstOrDefault().EXECUTE_ROLE_ID == 0)
                                        {
                                            mess += String.Format("Thành viên {0} chưa được thiết lập vai trò thực hiện.\n", check.FirstOrDefault().USERNAME);
                                        }
                                        else
                                        {
                                            foreach (var item in check.Select(o => o.EXECUTE_ROLE_ID).ToList())
                                            {
                                                lstExecuteRoleName.Add(executeRoleUsers.FirstOrDefault(o => o.ID == item).EXECUTE_ROLE_NAME);
                                            }
                                        }
                                        string messLogin = "";
                                        if (lstExecuteRoleName.Count > 0)
                                        {
                                            messLogin += String.Join(", ", lstExecuteRoleName);
                                        }
                                        if (string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME))
                                        {
                                            mess += String.Format("Vai trò {0} chưa được thiết lập người thực hiện.\n", messLogin);
                                        }
                                        //else
                                        //{
                                        //    if (check != null && check.Count > 1)
                                        //    {
                                        //        mess += String.Format("Thành viên {0} được thiết lập với các vai trò {1}.\n", check.FirstOrDefault().USERNAME, messLogin);
                                        //    }

                                        //}
                                    }

                                }
                            }
                            if (!string.IsNullOrEmpty(mess))
                            {
                                WaitingManager.Hide();
                                MessageBox.Show(mess, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            his_imp_temp = new BackendAdapter(commonParam).Post<HIS_IMP_USER_TEMP>(ImpRequestUriStore.IMP_USER_TEMP_UPDATE, ApiConsumers.MosConsumer, this.currentData, commonParam);

                        }
                    }
                    else
                    {

                        dataSource = this.gridControl1.DataSource as List<HisImpTempUserADO>;
                        list2 = new List<HIS_IMP_USER_TEMP_DT>();
                        if ((dataSource != null) && (dataSource.Count > 0))
                        {
                            foreach (HisImpTempUserADO rado in dataSource)
                            {
                                his_imp_user_temp_dt = new HIS_IMP_USER_TEMP_DT();
                                his_imp_user_temp_dt.LOGINNAME = rado.LOGINNAME;
                                his_imp_user_temp_dt.EXECUTE_ROLE_ID = rado.EXECUTE_ROLE_ID;
                                var lstAcsUser = acsUsers.Where(o => o.LOGINNAME == rado.LOGINNAME).ToList();
                                if (lstAcsUser != null && lstAcsUser.Count > 0)
                                {
                                    his_imp_user_temp_dt.USERNAME = acsUsers.Where(o => o.LOGINNAME == rado.LOGINNAME).FirstOrDefault().USERNAME;
                                }

                                list2.Add(his_imp_user_temp_dt);

                            }
                            this.currentData.HIS_IMP_USER_TEMP_DT = list2;
                        }
                        if (list.Contains(txtUserName.Text.Trim()))
                        {
                            DialogResult d = MessageBox.Show("Mẫu trùng tên. Bạn có muốn lưu?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (d == DialogResult.Yes)
                            {
                                if (this.currentData.HIS_IMP_USER_TEMP_DT != null && this.currentData.HIS_IMP_USER_TEMP_DT.Count > 0)
                                {
                                    var lstLoginName = this.currentData.HIS_IMP_USER_TEMP_DT.Select(o => o.LOGINNAME).Distinct().ToList();
                                    foreach (var Login in lstLoginName)
                                    {

                                        List<string> lstExecuteRoleName = new List<string>();
                                        var check = this.currentData.HIS_IMP_USER_TEMP_DT.Where(o => o.LOGINNAME == Login).ToList();

                                        if (check.FirstOrDefault().EXECUTE_ROLE_ID == 0 && string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME))
                                        {
                                            mess += "Vai trò và thành viên hội đồng mẫu không hợp lệ.\n";
                                        }
                                        else
                                        {
                                            if (check.FirstOrDefault().EXECUTE_ROLE_ID == 0)
                                            {
                                                mess += String.Format("Thành viên {0} chưa được thiết lập vai trò thực hiện.\n", check.FirstOrDefault().USERNAME);
                                            }
                                            else
                                            {
                                                foreach (var item in check.Select(o => o.EXECUTE_ROLE_ID).ToList())
                                                {
                                                    lstExecuteRoleName.Add(executeRoleUsers.FirstOrDefault(o => o.ID == item).EXECUTE_ROLE_NAME);
                                                }
                                            }
                                            string messLogin = "";
                                            if (lstExecuteRoleName.Count > 0)
                                            {
                                                messLogin += String.Join(", ", lstExecuteRoleName);
                                            }
                                            if (string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME))
                                            {
                                                mess += String.Format("Vai trò {0} chưa được thiết lập người thực hiện.\n", messLogin);
                                            }
                                            //else
                                            //{
                                            //    if (check != null && check.Count > 1)
                                            //    {
                                            //        mess += String.Format("Thành viên {0} được thiết lập với các vai trò {1}.\n", check.FirstOrDefault().USERNAME, messLogin);
                                            //    }

                                            //}
                                        }
                                    }
                                }


                                his_imp_temp = new BackendAdapter(commonParam).Post<HIS_IMP_USER_TEMP>(ImpRequestUriStore.IMP_USER_TEMP_CREATE, ApiConsumers.MosConsumer, this.currentData, commonParam);


                            }
                            else
                            {
                                isCheck = true;
                                txtUserName.Focus();
                            }
                        }
                        else
                        {
                            if (this.currentData.HIS_IMP_USER_TEMP_DT != null && this.currentData.HIS_IMP_USER_TEMP_DT.Count > 0)
                            {   
                               
                                var lstLoginName = this.currentData.HIS_IMP_USER_TEMP_DT.Select(o => o.LOGINNAME).ToList();
                                foreach (var Login in lstLoginName)
                                {

                                    List<string> lstExecuteRoleName = new List<string>();
                                    var check = this.currentData.HIS_IMP_USER_TEMP_DT.Where(o => o.LOGINNAME == Login).ToList();
                                    LogSystem.Warn(check.FirstOrDefault().EXECUTE_ROLE_ID + " " + string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME));
                                    if (check.FirstOrDefault().EXECUTE_ROLE_ID == 0 && string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME))
                                    {
                                        mess += "Vai trò và thành viên hội đồng mẫu không hợp lệ.\n";
                                    }
                                    else
                                    {
                                        if (check.FirstOrDefault().EXECUTE_ROLE_ID == 0)
                                        {
                                            mess += String.Format("Thành viên {0} chưa được thiết lập vai trò thực hiện.\n", check.FirstOrDefault().USERNAME);
                                        }
                                        else
                                        {
                                            foreach (var item in check.Select(o => o.EXECUTE_ROLE_ID).ToList())
                                            {
                                                lstExecuteRoleName.Add(executeRoleUsers.FirstOrDefault(o => o.ID == item).EXECUTE_ROLE_NAME);
                                            }
                                        }
                                        string messLogin = "";
                                        if (lstExecuteRoleName.Count > 0)
                                        {
                                            messLogin += String.Join(",", lstExecuteRoleName);
                                        }
                                        if (string.IsNullOrEmpty(check.FirstOrDefault().LOGINNAME))
                                        {
                                            mess += String.Format("Vai trò {0} chưa được thiết lập người thực hiện.\n", messLogin);
                                        }
                                        //else
                                        //{
                                        //    if (check != null && check.Count > 1)
                                        //    {
                                        //        mess += String.Format("Thành viên {0} được thiết lập với các vai trò {1}.\n", check.FirstOrDefault().USERNAME, messLogin);
                                        //    }

                                        //}
                                    }

                                }
                            }
                            if (!string.IsNullOrEmpty(mess))
                            {
                                WaitingManager.Hide();
                                MessageBox.Show(mess, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            his_imp_temp = new BackendAdapter(commonParam).Post<HIS_IMP_USER_TEMP>(ImpRequestUriStore.IMP_USER_TEMP_CREATE, ApiConsumers.MosConsumer, this.currentData, commonParam);

                        }

                    }
                    if (!isCheck)
                    {
                        WaitingManager.Hide();
                        if (his_imp_temp != null)
                        {
                            if (this.refeshData != null)
                            {
                                this.refeshData();
                            }
                            flag2 = true;
                            base.Close();
                        }
                        MessageManager.Show(this, commonParam, new bool?(flag2));
                    }
                }
            }
            catch (Exception exception)
            {
                WaitingManager.Hide();
                LogSystem.Warn(exception);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                List<HisImpTempUserADO> dataSource = this.gridControl1.DataSource as List<HisImpTempUserADO>;
                if ((dataSource == null) || (dataSource.Count < 1))
                {
                    HisImpTempUserADO item = new HisImpTempUserADO
                    {
                        Action = 2
                    };
                    this.impUserAdoTemp.Add(item);
                    this.gridControl1.DataSource = null;
                    this.gridControl1.DataSource = this.impUserAdoTemp;
                }
                else
                {
                    HisImpTempUserADO rado2 = new HisImpTempUserADO
                    {
                        Action = 2
                    };
                    dataSource.Add(rado2);
                    this.gridControl1.DataSource = null;
                    this.gridControl1.DataSource = dataSource;
                }
            }
            catch (Exception exception)
            {
                Inventec.Common.Logging.LogSystem.Warn(exception);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<HisImpTempUserADO> dataSource = this.gridControl1.DataSource as List<HisImpTempUserADO>;
                HisImpTempUserADO focusedRow = (HisImpTempUserADO)this.gridView1.GetFocusedRow();
                if ((focusedRow != null) && (dataSource.Count > 0))
                {
                    dataSource.Remove(focusedRow);
                    if (dataSource == null || dataSource.Count == 0)
                    {
                        HisImpTempUserADO RoleUserAdo = new HisImpTempUserADO();
                        dataSource.Add(RoleUserAdo);
                    }
                    this.gridControl1.DataSource = null;
                    this.gridControl1.DataSource = dataSource;
                }
            }
            catch (Exception exception)
            {
                Inventec.Common.Logging.LogSystem.Warn(exception);
            }
        }
        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnDelete")
                {
                    int num = Convert.ToInt32(e.RowHandle);
                    switch (Parse.ToInt32((this.gridView1.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString()))
                    {
                        case 1:
                            e.RepositoryItem = this.btnAdd;
                            break;

                        case 2:
                            e.RepositoryItem = this.btnDelete;
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Inventec.Common.Logging.LogSystem.Warn(exception);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "USERNAME")
                    {
                        try
                        {
                            string status = (view.GetRowCellValue(e.ListSourceRowIndex, "USERNAME") ?? "").ToString();
                            ACS.EFMODEL.DataModels.ACS_USER data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == status);
                            e.Value = data.USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi hien thi gia tri cot USERNAME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "USERNAME")
                {
                    gridView1.ShowEditor();
                    ((GridLookUpEdit)gridView1.ActiveEditor).ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
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


    }
}

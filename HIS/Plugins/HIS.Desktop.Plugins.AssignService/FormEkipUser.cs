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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using Newtonsoft.Json;
using Inventec.Desktop.Common.Message;
using ACS.EFMODEL.DataModels;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AssignService.Resources;
using ACS.Filter;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.LocalStorage.BackendData.ADO;

namespace HIS.Desktop.Plugins.AssignService
{
    public partial class FormEkipUser : FormBase
    {
        Action<SereServADO> lstEkipUser;
        Action<long> ekipId;
        internal List<HIS_EKIP_TEMP> ekipTemps { get; set; }
        internal List<HIS_EXECUTE_ROLE_USER> executeRoleUsers { get; set; }
        internal List<AcsUserADO> AcsUserADOList { get; set; }

        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        private string MODULELINK = "FormEkipUser";
        Inventec.Desktop.Common.Modules.Module CurrentModuleData;
        long? idPttt;
        List<HIS_EKIP_USER> lstEkip = new List<HIS_EKIP_USER>();
        SereServADO currentADO;
        public FormEkipUser(Action<SereServADO> lst, SereServADO ado, Inventec.Desktop.Common.Modules.Module module)
        {
            this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            this.lstEkipUser = lst;
            this.currentADO = ado;
            this.CurrentModuleData = module;
            InitializeComponent();
        }

        private void FormEkipUser_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //this.SetCaptionByLanguageKey();
                this.ComboEkipTemp(cboEkipTemp);
                this.ComboExecuteRole();
                AcsUserADOList = ProcessAcsUser();
                ComboAcsUser();
                LoadExecuteRoleUser();
                LoadDefaultGrid();
                this.InitControlState();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {

            try
            {
                if (currentADO.SereServEkipADO != null && currentADO.SereServEkipADO.listEkipUser.Count > 0)
                {
                    currentADO.SereServEkipADO.listEkipUser.First().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = currentADO.SereServEkipADO.listEkipUser;
                }
                cboEkipTemp.EditValue = (currentADO.SereServEkipADO != null && currentADO.SereServEkipADO.EKIP_TEMP_ID != null && currentADO.SereServEkipADO.EKIP_TEMP_ID > 0) ? currentADO.SereServEkipADO.EKIP_TEMP_ID : 0;
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private List<AcsUserADO> ProcessAcsUser()
        {
            List<AcsUserADO> AcsUserADOList = null;
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                List<V_HIS_EMPLOYEE> employeeList = null;

                //if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                //{
                //    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                //}
                //else
                //{
                CommonParam paramCommon = new CommonParam();
                dynamic filter = new System.Dynamic.ExpandoObject();
                datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                datas = datas.Where(o => o.IS_ACTIVE == 1).ToList();
                if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                //}

                //if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>())
                //{
                //    employeeList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>();
                //}
                //else
                //{
                //    CommonParam paramCommon = new CommonParam();
                //    dynamic filter = new System.Dynamic.ExpandoObject();
                employeeList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>>("api/HisEmployee/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (employeeList != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE), employeeList, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                //}

                var departmentList = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1 && o.IS_CLINICAL == 1).ToList();
                AcsUserADOList = new List<AcsUserADO>();

                foreach (var item in datas)
                {
                    AcsUserADO user = new AcsUserADO();
                    user.ID = item.ID;
                    user.LOGINNAME = item.LOGINNAME;
                    user.USERNAME = item.USERNAME;
                    user.MOBILE = item.MOBILE;
                    user.PASSWORD = item.PASSWORD;
                    user.IS_ACTIVE = item.IS_ACTIVE;

                    var check = employeeList.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                    if (check != null)
                    {

                        user.DOB = Inventec.Common.DateTime.Convert.TimeNumberToDateString(check.DOB ?? 0);

                        user.DIPLOMA = check.DIPLOMA;
                        var checkDepartment = departmentList.FirstOrDefault(o => o.ID == check.DEPARTMENT_ID);

                        if (checkDepartment != null)
                        {
                            user.DEPARTMENT_NAME = checkDepartment.DEPARTMENT_NAME;

                        }
                    }
                    AcsUserADOList.Add(user);
                }

                AcsUserADOList = AcsUserADOList.OrderBy(o => o.USERNAME).ToList();
            }
            catch (Exception ex)
            {
                AcsUserADOList = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return AcsUserADOList;
        }

        public async Task ComboEkipTemp(GridLookUpEdit cbo)
        {
            try
            {
                var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.CurrentModuleData.RoomId).DepartmentId;

                string logginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam param = new CommonParam();
                HisEkipTempFilter filter = new HisEkipTempFilter();
                ekipTemps = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>>("/api/HisEkipTemp/Get", ApiConsumers.MosConsumer, filter, param);
                if (ekipTemps != null && ekipTemps.Count > 0)
                {
                    ekipTemps = ekipTemps.Where(o => (o.IS_PUBLIC == 1 || o.CREATOR == logginName || (o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == DepartmentID)) && o.IS_ACTIVE == 1).OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EKIP_TEMP_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EKIP_TEMP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, ekipTemps, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboExecuteRole()
        {
            try
            {
                List<HIS_EXECUTE_ROLE> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_EXECUTE_ROLE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EXECUTE_ROLE>>("api/HisExecuteRole/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_EXECUTE_ROLE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (datas != null && datas.Count > 0)
                {
                    datas = datas.Where(p => p.IS_ACTIVE == 1 && p.IS_DISABLE_IN_EKIP != 1).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboPosition, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboAcsUser()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("DOB", "", 100, 3));
                columnInfos.Add(new ColumnInfo("DIPLOMA", "", 100, 4));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 200, 5));

                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 800);
                ControlEditorLoader.Load(GridLU_User, this.AcsUserADOList, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadExecuteRoleUser()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("---S");
                HisExecuteRoleUserFilter filter = new HisExecuteRoleUserFilter();
                executeRoleUsers = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER>>("api/HisExecuteRoleUser/Get", ApiConsumers.MosConsumer, filter, new CommonParam());
                Inventec.Common.Logging.LogSystem.Error("---F");
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
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridView1.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = Bbtn_Add;
                    }
                    else// if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = Bbtn_Minus;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "LOGINNAME")
                {
                    gridView1.ShowEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisEkipUserADO data = view.GetFocusedRow() as HisEkipUserADO;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    List<string> loginNames = new List<string>();
                    if (data != null && data.EXECUTE_ROLE_ID > 0)
                    {
                        if (data.LOGINNAME != null)
                            editor.EditValue = data.LOGINNAME;
                        var executeRoleUserTemps = executeRoleUsers != null ? executeRoleUsers.Where(o => o.EXECUTE_ROLE_ID == data.EXECUTE_ROLE_ID).ToList() : null;
                        if (executeRoleUserTemps != null && executeRoleUserTemps.Count > 0)
                        {
                            loginNames = executeRoleUserTemps.Select(o => o.LOGINNAME).Distinct().ToList();
                        }
                    }

                    ComboAcsUser(editor, loginNames);
                    gridView1.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<AcsUserADO> acsUserAlows = new List<AcsUserADO>();
                //BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.ID == this.serviceReq.EXECUTE_DEPARTMENT_ID);
                if (loginNames != null && loginNames.Count > 0)
                {

                    acsUserAlows = this.AcsUserADOList.Where(o => loginNames.Contains(o.LOGINNAME) && o.IS_ACTIVE == 1).ToList();

                }
                else
                {
                    acsUserAlows = this.AcsUserADOList.Where(o => o.IS_ACTIVE == 1).ToList();
                }

                cbo.Properties.DataSource = acsUserAlows;
                cbo.Properties.DisplayMember = "USERNAME";
                cbo.Properties.ValueMember = "LOGINNAME";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Tài khoản";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Họ tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

                GridColumn aColumnDIPLOMA = cbo.Properties.View.Columns.AddField("DIPLOMA");
                aColumnDIPLOMA.Caption = "CCHN";
                aColumnDIPLOMA.Visible = true;
                aColumnDIPLOMA.VisibleIndex = 3;
                aColumnDIPLOMA.Width = 100;

                GridColumn aColumnDepartment = cbo.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnDepartment.Caption = "Tên khoa";
                aColumnDepartment.Visible = true;
                aColumnDepartment.VisibleIndex = 4;
                aColumnDepartment.Width = 200;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Bbtn_Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                var ekipUsers = gridControl1.DataSource as List<HisEkipUserADO>;
                if (ekipUsers == null || ekipUsers.Count < 1)
                {
                    HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                    //ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    ekipUserAdoTemps.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    ekipUserAdoTemps.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = ekipUserAdoTemps;
                }
                else
                {
                    HisEkipUserADO participant = new HisEkipUserADO();
                    //participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUsers.Add(participant);
                    ekipUsers.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    ekipUsers.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = ekipUsers;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Bbtn_Minus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var ekipUsers = gridControl1.DataSource as List<HisEkipUserADO>;
                var ekipUser = (HisEkipUserADO)gridView1.GetFocusedRow();
                if (ekipUser != null)
                {
                    if (ekipUsers.Count > 0)
                    {
                        ekipUsers.Remove(ekipUser);
                        ekipUsers.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                        ekipUsers.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        gridControl1.DataSource = null;
                        gridControl1.DataSource = ekipUsers;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultGrid()
        {
            try
            {
                List<HisEkipUserADO> lstAdo = new List<HisEkipUserADO>();
                HisEkipUserADO ado = new HisEkipUserADO();
                ado.Action = (int)HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                lstAdo.Add(ado);
                gridControl1.DataSource = null;
                gridControl1.DataSource = lstAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(saveClick, "btnSave_Click");
        }

        private void saveClick()
        {
            try
            {
                WaitingManager.Show();
                var Alls = gridControl1.DataSource as List<HisEkipUserADO>;
                if (Alls != null && Alls.Count > 0)
                {
                    var ekipUserCheck = ProcessEkipUser(Alls);
                    if (!ekipUserCheck)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu ekip trùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                        WaitingManager.Show();
                    }
                    if (CheckEkip())
                    {
                        WaitingManager.Show();
                        SereServEkipADO ado = new SereServEkipADO();
                        ado.EKIP_TEMP_ID = cboEkipTemp.EditValue != null ? (long?)Convert.ToInt64(cboEkipTemp.EditValue) : null;
                        ado.listEkipUser = new List<HisEkipUserADO>();
                        if (lstEkip != null && lstEkip.Count() > 0)
                        {
                            foreach (var item in lstEkip)
                            {
                                HisEkipUserADO ekipUserADO = new HisEkipUserADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HisEkipUserADO>(ekipUserADO, item);
                                ado.listEkipUser.Add(ekipUserADO);
                            }
                        }

                        currentADO.SereServEkipADO = ado;

                        this.lstEkipUser(currentADO);
                        this.Close();
                    }
                }
                else
                {
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ProcessEkipUser(List<HisEkipUserADO> sereServADOs)
        {
            bool result = true;
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();

                //= grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                if (sereServADOs != null && sereServADOs.Count > 0)
                {
                    foreach (var item in sereServADOs)
                    {

                        MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, item);

                        var acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == ekipUser.LOGINNAME);
                        if (acsUser != null)
                        {
                            ekipUser.USERNAME = acsUser.USERNAME;
                            item.USERNAME = acsUser.USERNAME;
                        }
                        ekipUsers.Add(ekipUser);
                    }
                }

                var groupEkipUser = ekipUsers.GroupBy(x => new { x.LOGINNAME, x.EXECUTE_ROLE_ID });
                foreach (var item in groupEkipUser)
                {
                    if (item.Count() >= 2)
                    {
                        return false;
                    }
                }
                lstEkip = ekipUsers;
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckEkip()
        {
            bool result = true;
            try
            {
                WaitingManager.Hide();
                List<HIS_EKIP_USER> hasInvalid = lstEkip
                        .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                            || o.EXECUTE_ROLE_ID <= 0).Distinct().ToList();
                if (hasInvalid != null && hasInvalid.Count > 0)
                {
                    List<HIS_EXECUTE_ROLE> datas = null;
                    if (BackendDataWorker.IsExistsKey<HIS_EXECUTE_ROLE>())
                    {
                        datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                    }
                    else
                    {
                        CommonParam paramCommon = new CommonParam();
                        dynamic filter = new System.Dynamic.ExpandoObject();
                        datas = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EXECUTE_ROLE>>("api/HisExecuteRole/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                        if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_EXECUTE_ROLE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    }
                    var executeRoleNull = datas.Where(o => o.IS_ACTIVE == 1 && o.IS_DISABLE_IN_EKIP != 1 && hasInvalid.Select(p => p.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();
                    if (executeRoleNull == null)
                        result = true;



                    string mess = String.Format("Bạn chưa nhập thông tin tương ứng với (các) vai trò: {0}. Bạn có muốn thực hiện không?", String.Join(",", executeRoleNull.Select(o => o.EXECUTE_ROLE_NAME).ToArray()));

                    if (MessageBox.Show(mess, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return false;
                    else
                        result = true;


                }
                List<string> messError = new List<string>();
                var grLoginname = lstEkip.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME)).GroupBy(o => o.LOGINNAME).ToList();
                foreach (var item in grLoginname)
                {
                    if (item.Count() > 1)
                    {
                        var lstExeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => item.Select(s => s.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();

                        messError.Add(string.Format("Tài khoản {0} được thiết lập với các vai trò {1}", item.Key, string.Join(",", lstExeRole.Select(s => s.EXECUTE_ROLE_NAME))));
                    }
                }

                if (messError.Count > 0)
                {
                    XtraMessageBox.Show(string.Join("\n", messError), "Thông báo");
                    return false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void cboEkipTemp_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEkipTemp.EditValue != null)
                    {
                        var data = this.ekipTemps.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboEkipTemp.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            LoadGridEkipUserFromTemp(data.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadGridEkipUserFromTemp(long ekipTempId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipTempUserFilter filter = new HisEkipTempUserFilter();
                filter.EKIP_TEMP_ID = ekipTempId;
                List<HIS_EKIP_TEMP_USER> ekipTempUsers = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/Get", ApiConsumers.MosConsumer, filter, param);

                if (ekipTempUsers != null && ekipTempUsers.Count > 0)
                {
                    List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                    List<string> loginNames = ekipTempUsers.Select(o => o.LOGINNAME).ToList();
                    AcsUserFilter acsFilter = new AcsUserFilter();
                    acsFilter.LOGINNAMEs = loginNames;
                    List<ACS.EFMODEL.DataModels.ACS_USER> isActive = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => loginNames.Exists(p => p == o.LOGINNAME)).ToList();
                    List<string> isActiveLoginName = isActive.Where(o => o.IS_ACTIVE == 1).Select(i => i.LOGINNAME).ToList();
                    foreach (var ekipTempUser in ekipTempUsers)
                    {
                        var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == ekipTempUser.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.EXECUTE_ROLE_ID = ekipTempUser.EXECUTE_ROLE_ID;
                        ekipUserAdoTemp.LOGINNAME = ekipTempUser.LOGINNAME;
                        ekipUserAdoTemp.USERNAME = ekipTempUser.USERNAME;
                        ekipUserAdoTemp.DEPARTMENT_ID = ekipTempUser.DEPARTMENT_ID;
                        if (ekipUserAdoTemps.Count == 0)
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                        else
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }

                        if (isActiveLoginName.Contains(ekipTempUser.LOGINNAME))
                        {
                            ekipUserAdoTemps.Add(ekipUserAdoTemp);
                        }

                    }
                    gridControl1.DataSource = ekipUserAdoTemps;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
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
                            string status = (view.GetRowCellValue(e.ListSourceRowIndex, "LOGINNAME") ?? "").ToString();
                            ACS.EFMODEL.DataModels.ACS_USER data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>().SingleOrDefault(o => o.LOGINNAME == status && o.IS_ACTIVE == 1);
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

        private void FormEkipUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormEkipUser
        /// </summary>
        //private void SetCaptionByLanguageKey()
        //{
        //    try
        //    {
        //        ////Khoi tao doi tuong resource
        //        Resources.ResourceLanguageManager.LanguageResource__FormEkipUser = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(FormEkipUser).Assembly);

        //        ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
        //        this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormEkipUser.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__FormEkipUser, LanguageManager.GetCulture());
        //        this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormEkipUser.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource__FormEkipUser, LanguageManager.GetCulture());
        //        this.cboEkipTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("FormEkipUser.cboEkipTemp.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__FormEkipUser, LanguageManager.GetCulture());
        //        this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("FormEkipUser.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__FormEkipUser, LanguageManager.GetCulture());
        //        this.GridLU_User.NullText = Inventec.Common.Resource.Get.Value("FormEkipUser.GridLU_User.NullText", Resources.ResourceLanguageManager.LanguageResource__FormEkipUser, LanguageManager.GetCulture());
        //        this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("FormEkipUser.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__FormEkipUser, LanguageManager.GetCulture());
        //        this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("FormEkipUser.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource__FormEkipUser, LanguageManager.GetCulture());
        //        this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("FormEkipUser.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource__FormEkipUser, LanguageManager.GetCulture());
        //        this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("FormEkipUser.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource__FormEkipUser, LanguageManager.GetCulture());
        //        this.Text = Inventec.Common.Resource.Get.Value("FormEkipUser.Text", Resources.ResourceLanguageManager.LanguageResource__FormEkipUser, LanguageManager.GetCulture());
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

    }
}

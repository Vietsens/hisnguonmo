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
using DevExpress.XtraEditors.Repository;
using Inventec.Common.Controls.EditorLoader;
using HIS.UC.FormType.Core.RoleUserGrid_F36__.ADO;
using DevExpress.XtraEditors;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;

namespace HIS.UC.FormType.Core.RoleUserGrid_F36__
{
    public partial class UCRoleUser : UserControl
    {
        private bool isValidData = false;
        private SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        private SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        List<HisMestInveUserAdo> listRoleUserAdo = new List<HisMestInveUserAdo>();
        List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER> HisExecuteRoleUser = new List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER>();

        public UCRoleUser()
        {
            InitializeComponent();
        }

        public UCRoleUser(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
            : this()
        {
            try
            {
                this.config = config;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }

                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                SetValue();
                LoadDataToCombo();
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
                InitComboLookUp(this.repositoryItemLookUp__ExecuteRoleName, Config.HisFormTypeConfig.HisExecuteRole);
                ComboAcsUser(this.repositoryItemLookUp__ExecuteRoleUser, Config.AcsFormTypeConfig.HisAcsUser);

                MOS.Filter.HisExecuteRoleUserFilter filter = new MOS.Filter.HisExecuteRoleUserFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                HisExecuteRoleUser = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER>("/api/HisExecuteRoleUser/Get", ApiConsumerStore.MosConsumer, filter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cbo, List<ACS.EFMODEL.DataModels.ACS_USER> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboLookUp(RepositoryItemLookUpEdit cbo, List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "EXECUTE_ROLE_CODE", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue()
        {
            try
            {
                gridControl1.DataSource = null;
                List<RoleUserADO> ListADO = new List<RoleUserADO>();
                if (this.report != null && this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {
                    var jsOutputSub = this.config.JSON_OUTPUT.Split(new string[] { "," }, StringSplitOptions.None);
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, jsOutputSub[0], this.report.JSON_FILTER);
                    if (value != null && value != "null")
                    {
                        ListADO = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RoleUserADO>>(value);
                    }
                }

                if (ListADO != null && ListADO.Count > 0)
                {
                    foreach (var item in ListADO)
                    {
                        HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisMestInveUserAdo>(RoleUserAdo, item);
                        this.listRoleUserAdo.Add(RoleUserAdo);
                    }
                }
                else if (this.listRoleUserAdo == null || this.listRoleUserAdo.Count == 0)
                {
                    HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo();
                    this.listRoleUserAdo.Add(RoleUserAdo);
                }

                listRoleUserAdo.First().Action = GlobalVariables.ActionAdd;
                gridControl1.DataSource = this.listRoleUserAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    if (listRoleUserAdo == null || listRoleUserAdo.Count == 0)
                    {
                        result = false;
                    }
                    else
                    {
                        result = listRoleUserAdo.Exists(o => !string.IsNullOrWhiteSpace(o.EXECUTE_ROLE_CODE) && !String.IsNullOrWhiteSpace(o.LOGINNAME));
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public string GetValue()
        {
            string value = "";
            try
            {
                if (listRoleUserAdo != null && listRoleUserAdo.Count > 0)
                {
                    List<RoleUserADO> ListADO = new List<RoleUserADO>();
                    foreach (var item in listRoleUserAdo)
                    {
                        if (!String.IsNullOrWhiteSpace(item.EXECUTE_ROLE_CODE) && !String.IsNullOrWhiteSpace(item.LOGINNAME))
                        {
                            RoleUserADO ado = new RoleUserADO();

                            var role = Config.HisFormTypeConfig.HisExecuteRole.FirstOrDefault(o => o.EXECUTE_ROLE_CODE == item.EXECUTE_ROLE_CODE);
                            if (role != null)
                            {
                                ado.EXECUTE_ROLE_CODE = role.EXECUTE_ROLE_CODE;
                                ado.EXECUTE_ROLE_NAME = role.EXECUTE_ROLE_NAME;
                            }

                            var user = Config.AcsFormTypeConfig.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                            if (user != null)
                            {
                                ado.LOGINNAME = user.LOGINNAME;
                                ado.USERNAME = user.USERNAME;
                            }

                            ListADO.Add(ado);
                        }
                    }

                    value = String.Format(this.config.JSON_OUTPUT, Newtonsoft.Json.JsonConvert.SerializeObject(ListADO));
                }
            }
            catch (Exception ex)
            {
                value = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }

        private void repositoryItemButton__Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo();
                RoleUserAdo.Action = GlobalVariables.ActionEdit;
                this.listRoleUserAdo.Add(RoleUserAdo);
                gridControl1.DataSource = null;
                gridControl1.DataSource = this.listRoleUserAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HisMestInveUserAdo)gridView1.GetFocusedRow();
                if (row != null)
                {
                    this.listRoleUserAdo.Remove(row);
                }

                if (this.listRoleUserAdo == null || this.listRoleUserAdo.Count == 0)
                {
                    HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo();
                    RoleUserAdo.Action = GlobalVariables.ActionAdd;
                    this.listRoleUserAdo.Add(RoleUserAdo);
                }

                gridControl1.DataSource = null;
                gridControl1.DataSource = this.listRoleUserAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisMestInveUserAdo data = null;
                if (e.RowHandle > 0)
                {
                    data = (HisMestInveUserAdo)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "ADD")
                    {
                        if (data.Action == GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = repositoryItemButton__Add;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Delete;
                        }
                    }
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
                gridView1.ClearColumnErrors();
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisMestInveUserAdo data = view.GetFocusedRow() as HisMestInveUserAdo;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is LookUpEdit)
                {
                    LookUpEdit editor = view.ActiveEditor as LookUpEdit;

                    List<string> loginNames = new List<string>();
                    if (data != null && !String.IsNullOrWhiteSpace(data.EXECUTE_ROLE_CODE))
                    {
                        if (data.LOGINNAME != null)
                            editor.EditValue = data.LOGINNAME;

                        var role = Config.HisFormTypeConfig.HisExecuteRole.FirstOrDefault(o => o.EXECUTE_ROLE_CODE == data.EXECUTE_ROLE_CODE);
                        var rs = HisExecuteRoleUser.Where(p => p.EXECUTE_ROLE_ID == role.ID).ToList();
                        if (rs != null && rs.Count > 0)
                        {
                            loginNames = rs.Select(o => o.LOGINNAME).Distinct().ToList();
                        }
                    }

                    ComboAcsUser(editor, loginNames);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ComboAcsUser(LookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = Config.AcsFormTypeConfig.HisAcsUser.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    GridView view = sender as GridView;
                    GridColumn onOrderCol = view.Columns["LOGINNAME"];
                    var data = (HisMestInveUserAdo)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null && !String.IsNullOrWhiteSpace(data.EXECUTE_ROLE_CODE))
                    {
                        if (String.IsNullOrEmpty(data.LOGINNAME.Trim()))
                        {
                            e.Valid = false;
                            view.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc");
                        }
                        else
                        {
                            var ktra = this.listRoleUserAdo.Where(p => p.LOGINNAME == data.LOGINNAME).ToList();
                            //if (ktra != null && ktra.Count > 1)
                            //{
                            //    e.Valid = false;
                            //    view.SetColumnError(onOrderCol, "'" + Config.AcsFormTypeConfig.HisAcsUser.FirstOrDefault(p => p.LOGINNAME == data.LOGINNAME).USERNAME + "'" + " đã được gán vai trò");
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                //Suppress displaying the error message box
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using Inventec.UC.ListReports.Base;
using Inventec.Core;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Desktop.Common.Message;

namespace Inventec.UC.ListReports.Form
{
    public partial class frmShare : DevExpress.XtraEditors.XtraForm
    {
        List<ACS.EFMODEL.DataModels.ACS_USER> ListUser = new List<ACS.EFMODEL.DataModels.ACS_USER>();
        List<ACS.EFMODEL.DataModels.ACS_USER> _ListUser = new List<ACS.EFMODEL.DataModels.ACS_USER>();
        List<ACS.EFMODEL.DataModels.ACS_USER> ListUsers = new List<ACS.EFMODEL.DataModels.ACS_USER>();
        List<ACS.EFMODEL.DataModels.ACS_USER> ListUserSelected = new List<ACS.EFMODEL.DataModels.ACS_USER>();
        //List<ACS.EFMODEL.DataModels.ACS_USER> _listUsers = new List<ACS.EFMODEL.DataModels.ACS_USER>();        

        private SAR.EFMODEL.DataModels.V_SAR_REPORT viewReport;
        private ProcessHasException _HasException;
        //Data.DataInit currentData;

        public frmShare(SAR.EFMODEL.DataModels.V_SAR_REPORT report, ProcessHasException hasException)
        {
            InitializeComponent();
            try
            {
                SetIconForForm();
                this._HasException = hasException;
                this.viewReport = report;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIconForForm()
        {
            try
            {
                string filePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                string pathFileIcon = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), Config.GlobalStore.pathFileIcon);
                this.Icon = new Icon(pathFileIcon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmShare_Load(object sender, EventArgs e)
        {
            try
            {
                SetValueControl();
                gridControlSelectedUser.DataSource = ListUserSelected;
                LoadDataToRam();
                //ProcessCheckedSelected();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueControl()
        {
            try
            {
                lblReportCode.Text = viewReport.REPORT_CODE;
                lblReportName.Text = viewReport.REPORT_NAME;
                lblReportStt.Text = viewReport.REPORT_STT_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToRam()
        {
            try
            {
                ACS.Filter.AcsUserFilter filter = new ACS.Filter.AcsUserFilter();
                ListUser = new Acs.AcsUserGet().Get(filter);
                if (ListUser != null && ListUser.Count > 0)
                {
                    gridControlUserList.DataSource = ListUser;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCheckedSelected()
        {
            try
            {
                List<string> jsonReaders = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(viewReport.JSON_READER);
                if (jsonReaders != null && jsonReaders.Count > 0)
                {
                    foreach (var reader in jsonReaders)
                    {
                        var user = ListUser.FirstOrDefault(o => o.LOGINNAME == reader);
                        if (user != null)
                        {
                            int index = ((List<ACS.EFMODEL.DataModels.ACS_USER>)gridControlUserList.DataSource).IndexOf(user);
                            gridViewUserList.SetRowCellValue(index, gridViewUserList.Columns["SELECTED"], true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkSelected_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var user = (ACS.EFMODEL.DataModels.ACS_USER)gridViewUserList.GetFocusedRow();
                CheckEdit checkEdit = sender as CheckEdit;
                if (checkEdit.Checked)
                {
                    if (user != null && !ListUserSelected.Contains(user))
                    {
                        ListUserSelected.Add(user);
                    }
                }
                else
                {
                    if (ListUserSelected.Contains(user))
                    {
                        ListUserSelected.Remove(user);
                    }
                }
                //gridControlSelectedUser.RefreshDataSource();
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
                bool success = false;
                CommonParam param = new CommonParam();
                if (ListUserSelected != null)
                {
                    WaitingManager.Show();
                    SAR.EFMODEL.DataModels.SAR_REPORT Report = new SAR.EFMODEL.DataModels.SAR_REPORT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<SAR.EFMODEL.DataModels.SAR_REPORT>(Report, viewReport);
                    var users = ListUserSelected.Select(o => o.LOGINNAME).ToList();
                    Report.JSON_READER = Newtonsoft.Json.JsonConvert.SerializeObject(users);
                    var result = new Sar.SarReport.Send.SarReportSendFactory(param).createFactory(Report).Send();
                    if (result != null)
                    {
                        success = true;
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, new CommonParam(), true);
                    #endregion ;

                    #region Has exception
                    if (_HasException != null) _HasException(param);
                    #endregion

                    if (success) this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
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

        private void gridViewUserList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var data = (ACS.EFMODEL.DataModels.ACS_USER)gridViewUserList.GetFocusedRow();
                if (data != null)
                {
                    if (e.Column.FieldName == "SELECTED")
                    {
                        e.RepositoryItem = checkSelected;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewUserList_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                var rows = gridViewUserList.GetSelectedRows();
                if (rows != null)
                {
                    gridViewSelectedUser.BeginUpdate();
                    ListUserSelected.Clear();
                    foreach (var index in rows)
                    {
                        var user = (ACS.EFMODEL.DataModels.ACS_USER)gridViewUserList.GetRow(index);
                        if (user != null)
                        {
                            ListUserSelected.Add(user);
                        }
                    }
                    gridViewSelectedUser.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSourcegridControlUserList()
        {
            gridControlUserList.BeginUpdate();
            gridControlUserList.DataSource = ListUser;
            gridControlUserList.EndUpdate();
        }

        private void SearchUser()
        {
            try
            {
                if (string.IsNullOrEmpty(txtSearch.Text.Trim()))
                {
                    txtSearch.Text = null;
                    gridControlUserList.DataSource = null;

                    //gridControlSelectedUser.DataSource = null;
                    //LoadDataToRam();     
                    LoadDataSourcegridControlUserList();
                }
                else
                {
                    var listUsers = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                    foreach (var listUser in ListUser)
                    {
                        if (!string.IsNullOrEmpty(listUser.LOGINNAME) && listUser.LOGINNAME.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUsers.Add(listUser);
                        else if (!string.IsNullOrEmpty(listUser.USERNAME) && listUser.USERNAME.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUsers.Add(listUser);
                        else if (!string.IsNullOrEmpty(listUser.MOBILE) && listUser.MOBILE.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUsers.Add(listUser);
                        else if (!string.IsNullOrEmpty(listUser.EMAIL) && listUser.EMAIL.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUsers.Add(listUser);
                    }

                    //gridControlSelectedUser.DataSource = null;
                    gridControlUserList.DataSource = null;
                    gridControlUserList.DataSource = listUsers;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridControlUserList.DataSource = null;
                    SearchUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

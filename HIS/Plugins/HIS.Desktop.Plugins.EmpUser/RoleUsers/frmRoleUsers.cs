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
using ACS.EFMODEL.DataModels;
using ACS.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.EmpUser.RoleUsers
{
    public partial class frmRoleUsers : DevExpress.XtraEditors.XtraForm
    {

        ACS_USER acsUser = null;
        List<ACS_ROLE_USER> listAcsRoleUser = null;
        List<AcsRoleUserSDO> listCheck = null;

        public frmRoleUsers()
        {
            InitializeComponent();
        }

        private void setCaptionByLanguage()
        {
            HIS.Desktop.Plugins.EmpUser.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.EmpUser.Resources.Lang", typeof(HIS.Desktop.Plugins.EmpUser.RoleUsers.frmRoleUsers).Assembly);
            //Inventec.Common.Resource.Get getValue;
            this.Text = Inventec.Common.Resource.Get.Value("frmRoleUsers.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnROLE_NAME.Caption = Inventec.Common.Resource.Get.Value("frmRoleUser.gridColumnROLE_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmRoleUser.btnSave.text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("frmRoleUser.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }

        public frmRoleUsers(ACS.EFMODEL.DataModels.ACS_USER acsUser)
        {
            InitializeComponent();
            try
            {
                this.acsUser = acsUser;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridControl()
        {
            try
            {
                //WaitingManager.Show();
                CommonParam param = new CommonParam();

                ACS.Filter.AcsRoleUserViewFilter roleUserFilter = new ACS.Filter.AcsRoleUserViewFilter();
                roleUserFilter.USER_ID = acsUser.ID;
                roleUserFilter.KEY_WORD = txtSearch.Text;
                roleUserFilter.ORDER_DIRECTION = "DESC";
                roleUserFilter.ORDER_FIELD = "MODIFY_TIME";

                var result = new BackendAdapter(param).Get<List<ACS.SDO.AcsRoleUserSDO>>
                  (HisRequestUriStore.ACS_ROLE_USER_GET_VIEW, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, roleUserFilter, param);

                if (result != null && result.Count > 0)
                {
                    if (string.IsNullOrEmpty(txtSearch.Text.Trim()))
                    {
                        result = result.OrderByDescending(o => o.USER_ID == acsUser.ID).ToList();
                        listCheck = result.Where(o => o.USER_ID == acsUser.ID).ToList();
                        gridViewRoleUser.BeginUpdate();
                        gridViewRoleUser.GridControl.DataSource = result;

                        if (listCheck != null && listCheck.Count > 0)
                        {
                            foreach (var item in listCheck)
                            {
                                int rowHandle = gridViewRoleUser.LocateByValue("ROLE_ID", item.ROLE_ID);
                                if (rowHandle != GridControl.InvalidRowHandle)
                                    gridViewRoleUser.SelectRow(rowHandle);
                            }
                        }
                        gridViewRoleUser.EndUpdate();
                    }
                    else
                    {
                        result = result.OrderByDescending(o => o.USER_ID == acsUser.ID).ToList();
                        var listCheckSearch = result.Where(o => o.USER_ID == acsUser.ID).ToList();
                        gridViewRoleUser.BeginUpdate();

                        gridViewRoleUser.GridControl.DataSource = result;

                        if (listCheckSearch != null && listCheckSearch.Count > 0)
                        {
                            foreach (var item in listCheckSearch)
                            {
                                int rowHandle = gridViewRoleUser.LocateByValue("ROLE_ID", item.ROLE_ID);
                                if (rowHandle != GridControl.InvalidRowHandle)
                                    gridViewRoleUser.SelectRow(rowHandle);
                            }
                        }
                        gridViewRoleUser.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                //WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmRoleUsers_Load(object sender, EventArgs e)
        {
            LoadDataToGridControl();
            setCaptionByLanguage();
        }

        private void txtSearch_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
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

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    LoadDataToGridControl();
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
                CommonParam param = new CommonParam();
                bool success = false;

                AcsRoleUserForUpdateSDO updateDTO = new AcsRoleUserForUpdateSDO();

                WaitingManager.Show();

                List<ACS_ROLE_USER> data = new List<ACS_ROLE_USER>();
                List<AcsRoleUserSDO> roleUserSdo = new List<AcsRoleUserSDO>();
                List<AcsRoleUserSDO> checkAdd = new List<AcsRoleUserSDO>();
                List<AcsRoleUserSDO> checkDelete = new List<AcsRoleUserSDO>();
                List<AcsRoleUserSDO> listUncheck = new List<AcsRoleUserSDO>();

                var listInt = gridViewRoleUser.GetSelectedRows();
                var listDataSource = (List<AcsRoleUserSDO>)gridViewRoleUser.DataSource;

                if (listInt != null && listInt.Count() > 0)
                {
                    foreach (var item in listInt)
                    {
                        var roleUser = new AcsRoleUserSDO();
                        roleUser = (AcsRoleUserSDO)gridViewRoleUser.GetRow(item);
                        roleUserSdo.Add(roleUser);
                    }
                }

                if (listCheck != null)
                {
                    if (listDataSource != null && listDataSource.Count > 0)
                    {
                        foreach (var item in roleUserSdo)
                        {
                            listDataSource.Remove(item);
                        }

                        checkDelete = listCheck.Where(o => listDataSource.Select(p => p.ROLE_ID).Contains(o.ROLE_ID)).ToList();
                    }
                    checkAdd = roleUserSdo.Where(o => !listCheck.Select(p => p.ROLE_ID).Contains(o.ROLE_ID)).ToList();
                    if (checkAdd != null && checkAdd.Count > 0)
                    {
                        listCheck.AddRange(checkAdd);
                    }

                    if (checkDelete != null && checkDelete.Count > 0)
                    {
                        foreach (var item in checkDelete)
                        {
                            listCheck.Remove(item);
                        }
                    }
                }


                AutoMapper.Mapper.CreateMap<AcsRoleUserSDO, ACS_ROLE_USER>();
                updateDTO.RoleUsers = AutoMapper.Mapper.Map<List<ACS_ROLE_USER>>(listCheck);
                updateDTO.User = this.acsUser;

                foreach (var item in updateDTO.RoleUsers)
                {
                    item.USER_ID = acsUser.ID;
                }

                if (updateDTO != null)
                {
                    var resultData = new BackendAdapter(param).Post<bool>
                      (HisRequestUriStore.ACS_ROLE_USER_UPDATE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        LoadDataToGridControl();
                    }
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                LoadDataToGridControl();
            }
        }
    }
}

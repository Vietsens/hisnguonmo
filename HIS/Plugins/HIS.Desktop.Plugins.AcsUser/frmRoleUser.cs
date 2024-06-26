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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.AcsUser.ADO;
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

namespace HIS.Desktop.Plugins.AcsUser
{
    public partial class frmRoleUser : Form
    {

        ACS.EFMODEL.DataModels.ACS_USER currentData;

        public frmRoleUser()
        {
            InitializeComponent();
        }
        public frmRoleUser(ACS.EFMODEL.DataModels.ACS_USER data)
        {
            InitializeComponent();
            try
            {
                this.currentData = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<ACS_ROLE> acsrole;
        List<AcsRoleADO> lstAcsRoleADO;

        private void LoadPaging()
        {
            try
            {
                lstAcsRoleADO = new List<AcsRoleADO>();
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                var roleuser = new List<ACS.SDO.AcsRoleUserSDO>();
                //ACS.Filter.AcsRoleFilter filter = new ACS.Filter.AcsRoleFilter();
                ACS.Filter.AcsRoleUserViewFilter roleuserfilter = new ACS.Filter.AcsRoleUserViewFilter();
                roleuserfilter.USER_ID = currentData.ID;
                roleuserfilter.KEY_WORD = txtSearch.Text;
                roleuserfilter.ORDER_DIRECTION = "DESC";
                roleuserfilter.ORDER_FIELD = "MODIFY_TIME";

                //var obj = new BackendAdapter(param).Get<List<ACS.EFMODEL.DataModels.ACS_ROLE>>
                //  (HisRequestUriStore.ACS_ROLE_GET, ApiConsumer.ApiConsumers.AcsConsumer, filter, param);

                var getData = new BackendAdapter(param).Get<List<ACS.SDO.AcsRoleUserSDO>>
                  (HisRequestUriStore.ACS_ROLE_USER_GET_VIEW, ApiConsumer.ApiConsumers.AcsConsumer, roleuserfilter, param);
                if (getData != null && getData.Count > 0)
                {
                    roleuser = getData;
                }

                if (roleuser != null && roleuser.Count > 0)
                {
                    foreach (var itemRoleUser in roleuser)
                    {
                        AcsRoleADO ado = new AcsRoleADO(itemRoleUser);
                        if (itemRoleUser.USER_ID == currentData.ID && itemRoleUser.ROLE_ID > 0)
                        {
                            ado.CHECKACSROLEUSER = true;
                        }
                        else
                        {
                            ado.CHECKACSROLEUSER = false;
                        }
                        lstAcsRoleADO.Add(ado);
                    }
                }


                lstAcsRoleADO = lstAcsRoleADO.OrderByDescending(o => o.CHECKACSROLEUSER == true).ToList();
                gridAcsRole.BeginUpdate();
                gridAcsRole.GridControl.DataSource = lstAcsRoleADO;
                gridAcsRole.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            bool success = false;
            AcsRoleUserForUpdateSDO updateDTO = new AcsRoleUserForUpdateSDO();
            ACS.Filter.AcsRoleUserFilter filter = new ACS.Filter.AcsRoleUserFilter();
            WaitingManager.Show();

            var vRoleUser = lstAcsRoleADO.Where(o => o.CHECKACSROLEUSER == true).ToList();
            List<ACS_ROLE_USER> data = new List<ACS_ROLE_USER>();

            AutoMapper.Mapper.CreateMap<AcsRoleADO, ACS_ROLE_USER>();
            updateDTO.RoleUsers = AutoMapper.Mapper.Map<List<ACS_ROLE_USER>>(vRoleUser);

            updateDTO.User = this.currentData;
            //var userid = updateDTO.RoleUsers.Select(o => o.USER_ID == currentData.ID);

            foreach (var item in updateDTO.RoleUsers)
            {
                item.USER_ID = currentData.ID;
            }

            if (updateDTO != null)
            {
                var resultData = new BackendAdapter(param).Post<bool>
                  (HisRequestUriStore.ACS_ROLE_USER_UPDATE, ApiConsumers.AcsConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    LoadPaging();
                }
            }

            WaitingManager.Hide();
            MessageManager.Show(this, param, success);
            SessionManager.ProcessTokenLost(param);
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

        private void frmRoleUser_Load(object sender, EventArgs e)
        {
            try
            {
                LoadPaging();
                SetIcon();
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
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstAcsRoleADO != null && lstAcsRoleADO.Count > 0)
                {
                    foreach (var item in lstAcsRoleADO)
                    {
                        item.CHECKACSROLEUSER = true;
                    }
                    gridAcsRole.BeginUpdate();
                    gridAcsRole.GridControl.DataSource = lstAcsRoleADO;
                    gridAcsRole.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstAcsRoleADO != null && lstAcsRoleADO.Count > 0)
                {
                    foreach (var item in lstAcsRoleADO)
                    {
                        item.CHECKACSROLEUSER = false;
                    }
                    gridAcsRole.BeginUpdate();
                    gridAcsRole.GridControl.DataSource = lstAcsRoleADO;
                    gridAcsRole.EndUpdate();
                }
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
                LoadPaging();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {

            try
            {
                this.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                LoadPaging();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp_1(object sender, KeyEventArgs e)
        {

        }
    }
}

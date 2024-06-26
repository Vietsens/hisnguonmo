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
using ACS.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using MOS.EFMODEL.DataModels;
using MOS.SDO;

namespace HIS.Desktop.Plugins.InvoiceBook.Popup.AssignAuthorized
{
    public partial class frmAssignAuthorized
    {
        #region Method_Form-----------------------------------------------------------------------------------------------
        private void SearchUser()
        {
            try
            {
                if (string.IsNullOrEmpty(txtSearch.Text.Trim()))
                {
                    txtSearch.Text = null;
                    LoadDataSourceGridViewUser();
                    LoadDataSourceGridViewUserInvoiceBook();
                }
                else
                {
                    var listUsers = new List<ACS_USER>();
                    foreach (var listUser in _listUsers)
                    {
                        if (!string.IsNullOrEmpty(listUser.LOGINNAME) && listUser.LOGINNAME.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUsers.Add(listUser);
                        else if (!string.IsNullOrEmpty(listUser.USERNAME) && listUser.USERNAME.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUsers.Add(listUser);
                        else if (!string.IsNullOrEmpty(listUser.MOBILE) && listUser.MOBILE.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUsers.Add(listUser);
                        else if (!string.IsNullOrEmpty(listUser.EMAIL) && listUser.EMAIL.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUsers.Add(listUser);
                       /* else if (!string.IsNullOrEmpty(listUser.CREATOR) && listUser.CREATOR.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUsers.Add(listUser);
                        else if (!string.IsNullOrEmpty(listUser.MODIFIER) && listUser.MODIFIER.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUsers.Add(listUser);*/
                    }

                   // var listUserTemporary = new List<ACS_USER>();
                    /*foreach (var user in _listUsersTemporary)
                    {
                        if (!string.IsNullOrEmpty(user.LOGINNAME) && user.LOGINNAME.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUserTemporary.Add(user);
                        else if (!string.IsNullOrEmpty(user.USERNAME) && user.USERNAME.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUserTemporary.Add(user);
                        else if (!string.IsNullOrEmpty(user.MOBILE) && user.MOBILE.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUserTemporary.Add(user);
                        else if (!string.IsNullOrEmpty(user.EMAIL) && user.EMAIL.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUserTemporary.Add(user);
                        else if (!string.IsNullOrEmpty(user.CREATOR) && user.CREATOR.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUserTemporary.Add(user);
                        else if (!string.IsNullOrEmpty(user.MODIFIER) && user.MODIFIER.ToLower().Contains(txtSearch.Text.ToLower()))
                            listUserTemporary.Add(user);
                    }
                    */
                    gctUser.DataSource = null;
                    gctUser.DataSource = listUsers;

                   // gctUserInvoiceBook.DataSource = null;
                   // gctUserInvoiceBook.DataSource = listUserTemporary;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveAuthorized()
        {
            try
            {
                if (HisInvoiceBookInUc == null) return;
                var userInvoiceBookSdo = new HisUserInvoiceBookSDO
                {
                    InvoiceBookId = HisInvoiceBookInUc.ID,
                    LoginNames = _listUsersTemporary.Select(s => s.LOGINNAME).ToList()
                };
                InsertUserInvoiceBook(userInvoiceBookSdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessKeyDown(KeyEventArgs e)
        {
            try
            {
                switch ((Keys)e.KeyValue)
                {
                    case Keys.Enter:
                        SearchUser();
                        break;
                    case Keys.Escape:
                        this.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnSearch()
        {
            try
            {
                //SearchUser();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnSave()
        {
            try
            {
               // SaveAuthorized();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ThisClose()
        {
            try
            {
                ProcessKeyDown(new KeyEventArgs(Keys.Escape));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Method_AcsUser--------------------------------------------------------------------------------------------
        private void SetDataToGridUser()
        {
            try
            {
                gctUser.DataSource = null;
                var result = AcsUserGetData();
                if (result == null) return;
                _listUsers = new List<ACS_USER>();
                _listUsers = result;

                var listUsers = _listUsers.Where(s => _listUsersInvoiceBook.Select(a => a.LOGINNAME).Contains(s.LOGINNAME)).ToList();
                _listUsersTemporary.AddRange(listUsers);
                _listUsers.RemoveAll(s => listUsers.Contains(s));

                LoadDataSourceGridViewUser();
                LoadDataSourceGridViewUserInvoiceBook();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSourceGridViewUser()
        {
            gctUser.BeginUpdate();
            gctUser.DataSource = _listUsers;
            gctUser.EndUpdate();
        }

        private void AssignUserPermission(ACS_USER acsUser)
        {
            try
            {
                //if (!string.IsNullOrEmpty(txtSearch.Text))
               // {
                    var selectAssignPermission = _listUsers.Where(s => s == acsUser).ToList();
                    if (!selectAssignPermission.Any()) return;
                    var selectRemoveFirst = selectAssignPermission.First();
                    _listUsersTemporary.Add(selectRemoveFirst);
                    _listUsers.RemoveAll(s => s == selectRemoveFirst);
                   // SearchUser();
                    LoadDataSourceGridViewUser();   
                    LoadDataSourceGridViewUserInvoiceBook();                    
                   // return;
                //}
               // else
                /* {
                     var selectAssignPermission = _listUsers.Where(s => s == acsUser).ToList();
                     if (!selectAssignPermission.Any()) return;
                     var selectRemoveFirst = selectAssignPermission.First();
                     _listUsersTemporary.Add(selectRemoveFirst);
                     _listUsers.RemoveAll(s => s == selectRemoveFirst);
                     LoadDataSourceGridViewUser();
                     LoadDataSourceGridViewUserInvoiceBook();
                 }*/
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion End_Method_AcsUser

        #region Method_UserInvoiceBook------------------------------------------------------------------------------------
        private void SetDataToGridUserInvoiceBook()
        {
            try
            {
                gctUserInvoiceBook.DataSource = null;
                var result = UserInvoiceBookGetData(HisInvoiceBookInUc);
                if (result == null) return;
                _listUsersInvoiceBook = new List<V_HIS_USER_INVOICE_BOOK>();
                _listUsersInvoiceBook = result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSourceGridViewUserInvoiceBook()
        {
            gctUserInvoiceBook.BeginUpdate();
            gctUserInvoiceBook.DataSource = _listUsersTemporary;
            gctUserInvoiceBook.EndUpdate();
        }

        private void DiscardRights(ACS_USER acsUser)
        {
            try
            {
                var selectDiscardRights = _listUsersTemporary.Where(s => s == acsUser).ToList();
                if (!selectDiscardRights.Any()) return;
                var selectRemoveFirst = selectDiscardRights.First();
                _listUsers.Add(selectRemoveFirst);
                _listUsersTemporary.RemoveAll(s => s == selectRemoveFirst);
                LoadDataSourceGridViewUser();                
                LoadDataSourceGridViewUserInvoiceBook();
                txtSearch.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion End_Method_AcsUser
    }
}

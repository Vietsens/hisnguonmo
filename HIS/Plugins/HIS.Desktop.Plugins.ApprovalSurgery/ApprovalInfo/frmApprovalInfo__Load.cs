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
using Inventec.Common.Controls.EditorLoader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovalSurgery.ApprovalInfo
{
    
    public partial class frmApprovalInfo : Form
    {
        private void LoadDataDefault()
        {
            try
            {
                dtTime.DateTime = DateTime.Now;
                LoadCboUser();
                switch (action)
                {
                    case 1:
                        this.Text = "Thông tin duyệt";
                        lciTime.Text = "Thời gian duyệt:";
                        lciLoginname.Text = "Người duyệt";
                        break;
                    case 2:
                        this.Text = "Thông tin hủy duyệt";
                        lciTime.Text = "T/gian hủy duyệt:";
                        lciLoginname.Text = "Người hủy duyệt";
                        break;
                    case 3:
                        this.Text = "Thông tin từ chối";
                        lciTime.Text = "T/gian từ chối:";
                        lciLoginname.Text = "Người từ chối";
                        break;
                    case 4:
                        this.Text = "Thông tin hủy từ chối";
                        lciTime.Text = "T/gian hủy t/chối:";
                        lciLoginname.Text = "Người hủy t/chối";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCboUser()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cboLoginname, data, controlEditorADO);

                string LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                cboLoginname.EditValue = LoginName;
                txtLoginname.Text = LoginName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadLoginName(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboLoginname.Focus();
                    cboLoginname.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                        .Where(o => o.LOGINNAME.ToLower().Contains(searchCode.ToLower())).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboLoginname.EditValue = data[0].LOGINNAME;
                            txtLoginname.Focus();
                            txtLoginname.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.LOGINNAME.ToLower() == searchCode.ToLower());
                            if (search != null)
                            {
                                cboLoginname.EditValue = search.LOGINNAME;
                                txtLoginname.Focus();
                                txtLoginname.SelectAll();
                            }
                            else
                            {
                                cboLoginname.EditValue = null;
                                cboLoginname.Focus();
                                cboLoginname.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboLoginname.EditValue = null;
                        cboLoginname.Focus();
                        cboLoginname.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

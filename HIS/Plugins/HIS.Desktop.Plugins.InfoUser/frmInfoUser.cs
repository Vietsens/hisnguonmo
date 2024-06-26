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
using HIS.Desktop.LocalStorage.BackendData;
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

namespace HIS.Desktop.Plugins.InfoUser
{
    public partial class frmInfoUser : HIS.Desktop.Utility.FormBase
    {
        private string loginName;
        Inventec.Desktop.Common.Modules.Module module;
        ACS_USER currentAcsUser;
        V_HIS_EMPLOYEE currentEmployee;


        public frmInfoUser(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmInfoUser(Inventec.Desktop.Common.Modules.Module module, string _loginName)
            : this(module)
        {
            try
            {
                this.loginName = _loginName;
                this.Text = module.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmInfoUser_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetValueControl();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueControl()
        {
            try
            {
                this.currentAcsUser = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == this.loginName).First();
                this.currentEmployee = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.LOGINNAME == this.loginName).First();

                if (this.currentAcsUser != null)
                {
                    lblHoTen.Text = this.currentAcsUser.USERNAME ?? "";
                    lblSoDienThoai.Text = this.currentAcsUser.MOBILE ?? "";
                }
                if (this.currentEmployee != null)
                {
                    lblNamSinh.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentEmployee.DOB ?? 0);
                    lblKhoaPhong.Text = this.currentEmployee.DEPARTMENT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

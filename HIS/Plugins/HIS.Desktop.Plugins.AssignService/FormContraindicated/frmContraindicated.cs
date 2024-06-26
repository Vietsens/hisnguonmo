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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AssignService.ADO;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.AssignService.FormContraindicated
{
    public partial class frmContraindicated : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_ICD_SERVICE> chanChongChiDinh;
        Action<bool> ActionWar;
        bool IsClickBtnY = false;
        public frmContraindicated()
        {
            InitializeComponent();
        }

        public frmContraindicated(Inventec.Desktop.Common.Modules.Module moduleData, List<HIS_ICD_SERVICE> chanChongChiDinh, Action<bool> ActionWar = null)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.moduleData = moduleData;
                this.chanChongChiDinh = chanChongChiDinh;
                this.ActionWar = ActionWar;
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmContraindicated_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControlProperties();
                LoadDataToGrid();
                if(ActionWar != null)
                {
                    btnN.Text = "Không";
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }    
                //
                btnN.Select();
                btnN.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGrid()
        {
            try
            {
                gridControl1.DataSource = null;
                List<ContraindicatedADO> data = new List<ContraindicatedADO>();
                if (this.chanChongChiDinh != null && this.chanChongChiDinh.Count() > 0)
                {
                    foreach (var item in this.chanChongChiDinh)
                    {
                        data.Add(new ContraindicatedADO(item));
                    }
                }
                gridControl1.BeginUpdate();
                gridControl1.DataSource = data;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControlProperties()
        {
            try
            {
                this.layoutControlRoot.MinimumSize = new System.Drawing.Size(this.layoutControlRoot.Width, 200);

                this.layoutControlRoot.AutoSize = true;
                this.layoutControlRoot.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                this.AutoSize = true;
                this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (ActionWar != null)
                    ActionWar(false);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnY_Click(object sender, EventArgs e)
        {
            try
            {
                IsClickBtnY = true;
                if (ActionWar != null)
                    ActionWar(true);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmContraindicated_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (ActionWar != null && !IsClickBtnY)
                    ActionWar(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

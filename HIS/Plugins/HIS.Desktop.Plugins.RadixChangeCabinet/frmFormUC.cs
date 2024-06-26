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

namespace HIS.Desktop.Plugins.RadixChangeCabinet
{
    public partial class frmFormUC : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_EXP_MEST_4 _expMest;
        V_HIS_MEDI_STOCK _MediStock;
        UCRadixChangeCabinet uc;

        public frmFormUC()
        {
            InitializeComponent();
        }

        public frmFormUC(Inventec.Desktop.Common.Modules.Module _Module, V_HIS_EXP_MEST_4 expMest, V_HIS_MEDI_STOCK _mediStock)
        {
            InitializeComponent();
            try
            {
                this.moduleData = _Module;
                this._expMest = expMest;
                this._MediStock = _mediStock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmFormUC_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();

                uc = new UCRadixChangeCabinet(this.moduleData, this._expMest, this._MediStock);
                uc.Dock = DockStyle.Fill;
                this.Controls.Add(uc);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon
                  (System.IO.Path.Combine
                  (LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                if (this.moduleData != null)
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__F2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null)
                {
                    uc.FocusF2();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null)
                {
                    uc.Luu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null)
                {
                    uc.Them();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Moi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null)
                {
                    uc.Moi();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__In_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null)
                {
                    uc.In();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null)
                {
                    uc.Sua();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

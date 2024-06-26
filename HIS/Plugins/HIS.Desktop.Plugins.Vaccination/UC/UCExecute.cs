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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using Inventec.Desktop.CustomControl;

namespace HIS.Desktop.Plugins.Vaccination.UC
{
    public partial class UCExecute : UserControl
    {
        internal List<HIS_VACC_REACT_PLACE> _vaccReactPlaceSelected;

        public UCExecute()
        {
            InitializeComponent();
        }

        private void UCExecute_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboReactPlace_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string ReactPlaceName = "";
                if (_vaccReactPlaceSelected != null && _vaccReactPlaceSelected.Count > 0)
                {
                    foreach (var item in _vaccReactPlaceSelected)
                    {
                        ReactPlaceName += item.VACC_REACT_PLACE_NAME + ", ";
                    }
                }
                e.DisplayText = ReactPlaceName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTinhTrangHienTai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTinhTrangHienTai.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(cboTinhTrangHienTai.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_VACC_HEALTH_STT.ID__TU_VONG)
                {
                    dtDeathTime.Enabled = true;
                }
                else
                {
                    dtDeathTime.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboReactPlace_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                cboReactPlace.Enabled = false;
                cboReactPlace.Enabled = true;
                txtReactResponser.Focus();
                txtReactResponser.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtReactResponser_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTinhTrangHienTai.Focus();
                    cboTinhTrangHienTai.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTinhTrangHienTai_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dtDeathTime.Enabled)
                {
                    dtDeathTime.Focus();
                    dtDeathTime.ShowPopup();
                }
                else
                {
                    txtReactReporter.Focus();
                    txtReactReporter.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDeathTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                txtReactReporter.Focus();
                txtReactReporter.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtReactReporter_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboFollow.Focus();
                    cboFollow.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFollow_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

        }

        private void cboReactPlace_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    cboReactPlace.ShowPopup();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

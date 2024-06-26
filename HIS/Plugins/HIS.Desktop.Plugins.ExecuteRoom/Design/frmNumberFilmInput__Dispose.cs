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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExecuteRoom.Design
{
    public partial class frmNumberFilmInput : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                _SSExt = null;
                _SereServ = null;
                _dlg = null;
                this.TxtNumberFailFilm.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.TxtNumberFailFilm_KeyDown);
                this.TxtNumberFailFilm.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.TxtNumberFailFilm_KeyPress);
                this.barButtonItem_Save.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_Save_ItemClick);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.cboSizeOfFilm.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboSizeOfFilm_Closed);
                this.cboSizeOfFilm.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboSizeOfFilm_KeyUp);
                this.txtNumberOfFilm.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtNumberOfFilm_KeyDown);
                this.txtNumberOfFilm.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtNumberOfFilm_KeyPress);
                this.Load -= new System.EventHandler(this.frmNumberFilmInput_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                LciNumberFailFilm = null;
                TxtNumberFailFilm = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem_Save = null;
                bar1 = null;
                barManager1 = null;
                dxValidationProvider1 = null;
                emptySpaceItem1 = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                lciMunberOfFilm = null;
                txtNumberOfFilm = null;
                gridLookUpEdit1View = null;
                cboSizeOfFilm = null;
                btnSave = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

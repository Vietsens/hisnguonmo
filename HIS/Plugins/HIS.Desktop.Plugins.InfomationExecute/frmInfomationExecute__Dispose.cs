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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InfomationExecute
{
    public partial class frmInfomationExecute : FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                isReturnObject = false;
                _Content = null;
                lstDataSer = null;
                lstAdo = null;
                _DelegateSelectData = null;
                treatmentId = 0;
                currentModule = null;
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.barButtonItem2.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
                this.btnSearch.Click -= new System.EventHandler(this.btnSearch_Click);
                this.txtSearch.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSearch_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.frmInfomationExecute_Load);
                gridView1.GridControl.DataSource = null;
                gridControl1.DataSource = null;
                gridColumn2 = null;
                gridColumn1 = null;
                barButtonItem2 = null;
                barButtonItem1 = null;
                emptySpaceItem2 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                emptySpaceItem1 = null;
                gridView1 = null;
                gridControl1 = null;
                btnSave = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                barDockControl4 = null;
                barDockControl3 = null;
                barDockControl2 = null;
                barDockControl1 = null;
                bar1 = null;
                barManager1 = null;
                txtSearch = null;
                btnSearch = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AnticipateCreateV2
{
    public partial class UCAnticipateCreateV2 : HIS.Desktop.Utility.UserControlBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                isCheckAll = false;
                _MediStocks = null;
                HisMediStockMetyByStocks = null;
                HisMediStockMety = null;
                HisMediStockMatyByStocks = null;
                HisMediStockMaty = null;
                fileName = null;
                mediFilter = null;
                RoomTypeId = 0;
                RoomId = 0;
                moduleData = null;
                isCheck = false;
                mediStockIds = null;
                currentMediStock = null;
                // Cây kết quả tự dựng (pivot)
                if (treeListPivot != null)
                {
                    treeListPivot.CustomUnboundColumnData -= pivotTree_CustomUnboundColumnData;
                    treeListPivot.DoubleClick -= pivotTree_DoubleClick;
                    treeListPivot.DataSource = null;
                    treeListPivot.Dispose();
                }
                if (treeListBlood != null)
                {
                    treeListBlood.CustomUnboundColumnData -= bloodType_CustomUnboundColumnData;
                    treeListBlood.DataSource = null;
                    treeListBlood.Dispose();
                }
                treeListPivot = null;
                treeListBlood = null;
                lstBlood = null;
                lstBloodInStocks = null;
                lstMateInStocks = null;
                lstMediInStocks = null;
                dicMediImpExp = null;
                dicMateImpExp = null;
                dicMediImpExpByType = null;
                dicMateImpExpByType = null;
                // vCong 52461 — chỉ gỡ sự kiện + null control CÒN tồn tại trên Designer
                // (các control lọc/loại/cbo/nút cũ đã được gỡ khỏi Designer).
                this.txtKeyWork.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtKeyWork_KeyUp);
                this.gridViewMediStock.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMediStock_CustomUnboundColumnData);
                this.gridViewMediStock.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewMediStock_MouseDown);
                this.Load -= new System.EventHandler(this.UCAnticipateCreateV2_Load);
                gridViewMediStock.GridControl.DataSource = null;
                gridControlMediStock.DataSource = null;
                imageListIcon = null;
                imageCollection1 = null;
                repositoryItemCheck_D = null;
                repositoryItemCheck_E = null;
                gridColumnCheck = null;
                layoutControlItem14 = null;
                txtKeyWork = null;
                saveFileDialog = null;
                panelControlMediMate = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                gridColumn4 = null;
                gridColumn3 = null;
                gridColumn2 = null;
                gridColumn1 = null;
                gridViewMediStock = null;
                gridControlMediStock = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                Root = null;
                layoutControl2 = null;
                layoutControlGroup2 = null;
                layoutControl3 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using MOS.Filter;
using Inventec.Core;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.UC.KskContract.Run
{
    public partial class UCKskContract__Template2 : UserControl
    {
        private void InitComboContract()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisKskContractViewFilter filter = new HisKskContractViewFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "KSK_CONTRACT_CODE";
                listKskContract = new BackendAdapter(param).Get<List<V_HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", "Mã hợp đồng", 100, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "Tên công ty", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboKskContract, listKskContract, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void DisposeControl()
        {
            try
            {
                kskContractInput = null;
                positionHandle = 0;
                listKskContract = null;
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.cboKskContract.EditValueChanged -= new System.EventHandler(this.cboKskContract_EditValueChanged);
                this.cboKskContract.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboKskContract_CustomDisplayText);
                this.cboKskContract.Leave -= new System.EventHandler(this.cboKskContract_Leave);
                this.cboKskContract.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboKskContract_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCKskContract__Template2_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                layoutControlItem1 = null;
                layoutControlItem6 = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                layoutControlGroup2 = null;
                gridLookUpEdit1View = null;
                cboKskContract = null;
                lblWorkPlaceName = null;
                lblPaymentRatio = null;
                lblEffectDate = null;
                lblExpiryDate = null;
                layoutControl2 = null;
                groupControl1 = null;
                dxValidationProvider1 = null;
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

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
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.RequestDeposit
{
    public partial class UC_RequestDeposit : UserControl
    {
        private void getDataDepositReq(long treatmentid)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDepositReqViewFilter filter = new HisDepositReqViewFilter();
                filter.TREATMENT_ID = treatmentid;
                listDepositReq = new BackendAdapter(param).Get<List<V_HIS_DEPOSIT_REQ>>(HisRequestUriStore.HIS_DEPOSIT_REQ_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillDataToGridDepositReq()
        {
            try
            {
                this.listDepositReqProcessor.Reload(this.ucRequestDeposit, listDepositReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /*
                private void getDataSelectRow()
                {
                    try
                    {
                        currentdepositReq = null;  
                        var result = (V_HIS_DEPOSIT_REQ)this.listDepositReqProcessor.GetSelectRow(ucRequestDeposit);
                        if (result != null)
                        {
                            currentdepositReq = result;
                            txtAmount.Text = currentdepositReq.AMOUNT.ToString();
                            txtGhiChu.Text = currentdepositReq.DESCRIPTION;
                        }
                        else
                        {
                            currentdepositReq = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
        */

        private void RemoveValidate()
        {
            dxValidationProvider1.RemoveControlError(txtAmount);
        }
        private void SetDefaultValue()
        {
            try
            {
                txtGhiChu.Text = null;
                txtAmount.Text = null;
                this.action = GlobalVariables.ActionAdd;
                EnableControlChanged(action);
                RemoveValidate();
                txtAmount.Focus();
                txtAmount.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }

        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

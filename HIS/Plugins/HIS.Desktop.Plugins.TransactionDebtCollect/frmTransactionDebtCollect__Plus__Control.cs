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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionDebtCollect
{
    public partial class frmTransactionDebtCollect : HIS.Desktop.Utility.FormBase
    {


        private void cboAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null)
                    {
                        var account = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                        if (account != null)
                        {
                            GlobalVariables.DefaultAccountBookDebtCollect = new List<V_HIS_ACCOUNT_BOOK>();
                            GlobalVariables.DefaultAccountBookDebtCollect.Add(account);
                            SetDataToDicNumOrderInAccountBook(account);
                        }
                    }
                    else
                    {
                        spinTongTuDen.Text = "";
                    }
                    dtTransactionTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransactionTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (layoutTongTuDen.Enabled)
                    {
                        spinTongTuDen.Focus();
                        spinTongTuDen.SelectAll();
                    }
                    //else if (lciTranferAmount.Enabled)
                    //{
                    //    spinTransferAmount.Focus();
                    //    spinTransferAmount.SelectAll();
                    //}
                    //else
                    //{
                    //    txtDiscount.Focus();
                    //    txtDiscount.SelectAll();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

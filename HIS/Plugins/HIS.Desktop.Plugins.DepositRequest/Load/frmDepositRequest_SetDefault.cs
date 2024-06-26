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
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ListDepositRequest;
using HIS.Desktop.LibraryMessage;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.DepositRequest.Resources;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.DepositRequest
{
    public partial class UCDepositRequest : UserControlBase
    {
        V_HIS_DEPOSIT_REQ adeposit;
        V_HIS_ACCOUNT_BOOK listAccountBook;
        V_HIS_TRANSACTION_5 vhissTransaction;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private void SetDefaultAccountBookForUser()
        {
            try
            {
                //if (ListAccountBook != null && ListAccountBook.Count > 0)
                //{
                //    ListAccountBook = ListAccountBook.OrderByDescending(o => o.CREATE_TIME).ToList();
                //    cboAccountBook.EditValue = ListAccountBook.FirstOrDefault().ID;
                //    txtAccountBookCode.Text = ListAccountBook.FirstOrDefault().ACCOUNT_BOOK_CODE;
                //    txtTotalFromNumberOder.Text = ListAccountBook.FirstOrDefault().TOTAL + "/" + ListAccountBook.FirstOrDefault().FROM_NUM_ORDER + "/" + (int)(ListAccountBook.FirstOrDefault().CURRENT_NUM_ORDER ?? 0);
                //}

                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }

                if (accountBook == null) accountBook = ListAccountBook.FirstOrDefault();

                txtAccountBookCode.Text = null;
                cboAccountBook.EditValue = null;
                txtTotalFromNumberOder.Text = null;

                if (accountBook != null)
                {
                    txtAccountBookCode.Text = accountBook.ACCOUNT_BOOK_CODE;
                    cboAccountBook.EditValue = accountBook.ID;
                    txtTotalFromNumberOder.Text = accountBook.TOTAL + "/" + accountBook.FROM_NUM_ORDER + "/" + (int)(accountBook.CURRENT_NUM_ORDER ?? 0);
                    setDataToDicNumOrderInAccountBook(accountBook);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultPayFormForUser()
        {
            try
            {
                if (ListPayForm != null && ListPayForm.Count > 0)
                {
                    var PayFormMinByCode = ListPayForm.OrderBy(o => o.PAY_FORM_CODE);
                    var payFormDefault = PayFormMinByCode.FirstOrDefault();
                    if (payFormDefault != null)
                    {
                        var data = ListPayForm.FirstOrDefault(o => o.PAY_FORM_CODE == payFormDefault.PAY_FORM_CODE);
                        if (data != null)
                        {
                            cboPayForm.EditValue = data.ID;
                            txtPayFormCode.Text = data.PAY_FORM_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }
    }
}

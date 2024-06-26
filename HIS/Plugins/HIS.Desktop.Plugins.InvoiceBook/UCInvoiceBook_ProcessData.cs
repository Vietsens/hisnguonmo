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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    public partial class UCInvoiceBook : HIS.Desktop.Utility.UserControlBase
    {
        #region Get_Data

        internal ApiResultObject<List<V_HIS_INVOICE_BOOK>> InvoiceBookGetDatas(object param)
        {
            var result = new ApiResultObject<List<V_HIS_INVOICE_BOOK>>();
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                HisInvoiceBookViewFilter filter = new HisInvoiceBookViewFilter();
                filter.ORDER_DIRECTION = "MODIFY_TIME";
                filter.ORDER_FIELD = "ASC";

                result = new BackendAdapter(paramCommon).GetRO<List<V_HIS_INVOICE_BOOK>>
                    (ApiConsumer.HisRequestUriStore.HIS_INVOICE_BOOK_GET__VIEW, ApiConsumers.MosConsumer, filter, paramCommon);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal ApiResultObject<List<V_HIS_INVOICE>> InvoiceGetDatas(V_HIS_INVOICE_BOOK invoiceBook, object param)
        {
            var result = new ApiResultObject<List<V_HIS_INVOICE>>();
            try
            {
                startPage2 = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;

                var paramCommon = new CommonParam(startPage2, limit);
                HisInvoiceFilter filter = new HisInvoiceFilter();
                filter.ORDER_DIRECTION = "MODIFY_TIME";
                filter.ORDER_FIELD = "ASC";

                filter.INVOICE_BOOK_ID = invoiceBook.ID;
                filter.KEY_WORD = txtSearchInvoice.Text;
                result = new BackendAdapter(paramCommon).GetRO<List<V_HIS_INVOICE>>
                    (ApiConsumer.HisRequestUriStore.HIS_INVOICE_GET__VIEW, ApiConsumers.MosConsumer, filter, paramCommon);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal ApiResultObject<List<HIS_INVOICE_DETAIL>> InvoiceDetailGetDatas(V_HIS_INVOICE invoiceDetail, object param)
        {
            var result = new ApiResultObject<List<HIS_INVOICE_DETAIL>>();
            try
            {
                startPage3 = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                var paramCommon = new CommonParam();
                //if (limit > 0)
                paramCommon = new CommonParam(startPage3, limit);

                HisInvoiceDetailFilter filter = new HisInvoiceDetailFilter();

                filter.INVOICE_ID = invoiceDetail.ID;
                filter.ORDER_DIRECTION = "MODIFY_TIME";
                filter.ORDER_FIELD = "ASC";

                result = new BackendAdapter(paramCommon).GetRO<List<HIS_INVOICE_DETAIL>>
                    (HisRequestUriStore.HIS_INVOICE_DETAIL_GET, ApiConsumers.MosConsumer, filter, paramCommon);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal List<V_HIS_USER_INVOICE_BOOK> UserInvoiceBookGetData()
        {
            var result = new List<V_HIS_USER_INVOICE_BOOK>();
            try
            {
                var paramCommon = new CommonParam();

                HisUserInvoiceBookViewFilter userInvoiceBookFilter = new HisUserInvoiceBookViewFilter();
                
                userInvoiceBookFilter.LOGINNAME = _logginname;
                userInvoiceBookFilter.ORDER_DIRECTION = "MODIFY_TIME";
                userInvoiceBookFilter.ORDER_FIELD = "ASC";

                result = new BackendAdapter(paramCommon).Get<List<V_HIS_USER_INVOICE_BOOK>>
                    (ApiConsumer.HisRequestUriStore.HIS_USER_INVOICE_BOOK_GET__VIEW, ApiConsumers.MosConsumer, userInvoiceBookFilter, paramCommon);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region Create_Data

        internal HIS_INVOICE_BOOK InvoiceBookCreateData(HIS_INVOICE_BOOK invoiceBook)
        {
            HIS_INVOICE_BOOK resultCreate;
            try
            {
                var common = new CommonParam();
                resultCreate = new BackendAdapter(common).Post<HIS_INVOICE_BOOK>
                  (HisRequestUriStore.HIS_INVOICE_BOOK_CREATE, ApiConsumers.MosConsumer, invoiceBook, common);
            }
            catch (Exception ex)
            {
                resultCreate = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return resultCreate;
        }

        #endregion

        #region Update_Data

        #endregion

        #region Delete_Data

        internal bool InvoiceBookDeleteData(HIS_INVOICE_BOOK invoiceBook)
        {
            bool result;
            try
            {
                var common = new CommonParam();
                result = new BackendAdapter(common).Post<bool>
                  (ApiConsumer.HisRequestUriStore.HIS_INVOICE_BOOK_DELETE, ApiConsumers.MosConsumer, invoiceBook, common);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion
    }
}

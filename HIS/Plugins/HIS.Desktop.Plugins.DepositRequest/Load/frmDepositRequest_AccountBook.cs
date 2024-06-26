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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DepositRequest
{
    public partial class UCDepositRequest : UserControlBase
    {
        internal List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK> ListAccountBook;
        internal List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> ListPayForm;
        private void loadPayForm()
        {
            try
            {
                MOS.Filter.HisPayFormFilter Filter = new HisPayFormFilter();
                Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                ListPayForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadAccountBook()
        {
            try
            {
                CommonParam param = new CommonParam();

                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                HisUserAccountBookFilter useAccountBookFilter = new HisUserAccountBookFilter();
                useAccountBookFilter.LOGINNAME__EXACT = loginName;
                var userAccountBooks = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_USER_ACCOUNT_BOOK>>("api/HisUserAccountBook/Get", ApiConsumers.MosConsumer, useAccountBookFilter, null);

                var cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId && o.ROOM_TYPE_ID == currentModule.RoomTypeId);
                HisCaroAccountBookFilter caroAccountBookFilter = new HisCaroAccountBookFilter();
                caroAccountBookFilter.CASHIER_ROOM_ID = cashierRoom != null ? cashierRoom.ID : 0;//0 để không tìm đc sổ nào
                var caroAccountBooks = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_CARO_ACCOUNT_BOOK>>("api/HisCaroAccountBook/Get", ApiConsumers.MosConsumer, caroAccountBookFilter, null);

                List<long> ids = new List<long>();
                ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
                // Kiểm tra sổ còn hay k
                if (userAccountBooks != null && userAccountBooks.Count > 0)
                {
                    ids.AddRange(userAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }

                if (caroAccountBooks != null && caroAccountBooks.Count > 0)
                {
                    ids.AddRange(caroAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }

                if (ids != null && ids.Count > 0)
                {
                    ids = ids.Distinct().ToList();
                    int dem = 0;
                    while (ids.Count >= dem)
                    {
                        var idsTmp = ids.Skip(dem).Take(100).ToList();
                        dem += 100;
                        MOS.Filter.HisAccountBookViewFilter Filter = new HisAccountBookViewFilter();
                        Filter.IDs = idsTmp;
                        Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        Filter.FOR_DEPOSIT = true;
                        Filter.ORDER_FIELD = "CREATE_TIME";
                        Filter.ORDER_DIRECTION = "DESC";
                        Filter.IS_OUT_OF_BILL = false;
                        var rsData = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, Filter, param);
                        //Kiem tra so thu chi con su dug duoc
                        if (rsData != null && rsData.Count > 0)
                        {
                            ListAccountBook.AddRange(rsData);
                            //foreach (var item in rsData)
                            //{
                            //    //if ((item.FROM_NUM_ORDER + item.TOTAL - 1) > item.CURRENT_NUM_ORDER)
                            //    //{
                            //        ListAccountBook.Add(item);
                            //    //}
                            //}
                            this.ListAccountBook = this.ListAccountBook.Where(o => o.WORKING_SHIFT_ID == null || o.WORKING_SHIFT_ID == (HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO.WorkingShiftId ?? 0)).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboPayForm()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboPayForm, this.ListPayForm, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboAccountBook()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboAccountBook, this.ListAccountBook, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadPayFormCombo(string _payFormCode)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> listResult = new List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>();
                listResult = ListPayForm.Where(o => (o.PAY_FORM_CODE != null && o.PAY_FORM_CODE.StartsWith(_payFormCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPayForm, listResult, controlEditorADO);

                if (listResult.Count == 1)
                {
                    cboPayForm.EditValue = listResult[0].ID;
                    txtPayFormCode.Text = listResult[0].PAY_FORM_CODE;
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
                else
                {
                    cboPayForm.EditValue = null;
                    cboPayForm.Focus();
                    cboPayForm.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadAccountBookCombo(string _accountBookCode)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK> listResult = new List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>();
                if (!String.IsNullOrWhiteSpace(_accountBookCode))
                {
                    listResult = ListAccountBook.Where(o => (o.ACCOUNT_BOOK_CODE != null && o.ACCOUNT_BOOK_CODE.StartsWith(_accountBookCode))).ToList();
                }

                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                //columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboAccountBook, listResult, controlEditorADO);
              
            
                if (listResult.Count == 1)
                {
                    cboAccountBook.EditValue = listResult[0].ID;
                    txtAccountBookCode.Text = listResult[0].ACCOUNT_BOOK_CODE;
                    txtTotalFromNumberOder.Text = listResult[0].TOTAL + "/" + listResult[0].FROM_NUM_ORDER + "/" + (int)(listResult[0].CURRENT_NUM_ORDER ?? 0);
                    txtPayFormCode.Focus();
                    txtPayFormCode.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboAccountBook.EditValue = null;
                    cboAccountBook.Focus();
                    //cboAccountBook.ShowPopup();
                }
                else
                {
                    //ControlEditorLoader.Load(cboAccountBook, ListAccountBook, controlEditorADO);
                    cboAccountBook.EditValue = null;
                    cboAccountBook.Focus();
                    //cboAccountBook.ShowPopup();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultPayForm()
        {
            try
            {
                HIS_PAY_FORM data = null;
                if (!String.IsNullOrEmpty(ConfigApplicationWorker.Get<string>(AppConfigKeys.HFS_KEY__PAY_FORM_CODE)))
                {
                    data = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.PAY_FORM_CODE == ConfigApplicationWorker.Get<string>(AppConfigKeys.HFS_KEY__PAY_FORM_CODE));
                }
                else
                {
                    data = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.PAY_FORM_CODE == GlobalVariables.HIS_PAY_FORM_CODE__CONSTANT);
                }
                if (data != null)
                {
                    txtPayFormCode.Text = data.PAY_FORM_CODE;
                    cboPayForm.EditValue = data.ID;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

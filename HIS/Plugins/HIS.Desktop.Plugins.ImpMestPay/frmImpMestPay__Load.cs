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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.ImpMestPay
{
    public partial class frmImpMestPay : FormBase
    {
        private void FillDataToControl()
        {
            try
            {
                //if (!String.IsNullOrEmpty(txtImpMestCode.Text))
                //{
                //    string code = txtImpMestCode.Text.Trim();
                //    if (code.Length < 12 && checkDigit(code))
                //    {
                //        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                //        txtImpMestCode.Text = code;
                //    }
                //}

                FillDataToGrid();


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

        private void FillDataToGrid()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__NUM_PAGESIZE);
                }

                FillDataToGridImpMestPay(new CommonParam(0, (int)numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridImpMestPay, param, numPageSize, gridControlImpMestPay);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToGridImpMestPay(object param)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSize = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_IMP_MEST_PAY>> apiResult = new ApiResultObject<List<V_HIS_IMP_MEST_PAY>>();
                HisImpMestPayViewFilter impMestPayViewFilter = new HisImpMestPayViewFilter();
                if (this._ImpMestProposeId > 0)
                {
                    impMestPayViewFilter.IMP_MEST_PROPOSE_ID = this._ImpMestProposeId;
                }

                impMestPayViewFilter.KEY_WORD = txtKeyword.Text;
                // impMestPayViewFilter.DOCUMENT_NUMBER__EXACT = txtDocumentNumber.Text;
                impMestPayViewFilter.ORDER_FIELD = "PAY_TIME";
                impMestPayViewFilter.ORDER_DIRECTION = "DESC";

                apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<V_HIS_IMP_MEST_PAY>>("api/HisImpMestPay/GetView", ApiConsumers.MosConsumer, impMestPayViewFilter, paramCommon);

                gridControlImpMestPay.DataSource = null;

                if (apiResult != null)
                {
                    var impMestPays = (List<V_HIS_IMP_MEST_PAY>)apiResult.Data;
                    rowCount = (impMestPays == null ? 0 : impMestPays.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    if (rowCount > 0)
                    {
                        gridControlImpMestPay.DataSource = impMestPays;
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void LoadToControl(V_HIS_IMP_MEST_PROPOSE _impMestPropose)
        {
            try
            {
                if (_impMestPropose != null)
                {
                    lblSupplier.Text = _impMestPropose.SUPPLIER_NAME;

                    decimal totalDocumentPrice = GetImpMestAmount(_impMestPropose.ID);
                    lblDocumentPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalDocumentPrice, ConfigApplications.NumberSeperator);
                    decimal totalPayAmount = GetImpMestPayAmount(_impMestPropose.ID, null);
                    decimal amount = spinAmount.EditValue != null ? spinAmount.Value : 0;
                    lblImpMestPayAmount.Text = Inventec.Common.Number.Convert.NumberToString(totalPayAmount, ConfigApplications.NumberSeperator);
                    lblRemainAmount.Text = Inventec.Common.Number.Convert.NumberToString(((totalPayAmount) - totalPayAmount - amount), ConfigApplications.NumberSeperator);
                    txtPayerLoginname.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    cboPayer.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    dtPayForm.DateTime = DateTime.Now;
                }
                else
                {
                    lblSupplier.Text = null;
                    lblDocumentPrice.Text = null;
                    lblImpMestPayAmount.Text = null;
                    lblRemainAmount.Text = null;
                    txtPayerLoginname.Text = null;
                    cboPayer.EditValue = null;
                    dtPayForm.EditValue = null;
                    txtImpMestCodeCreate.Text = null;
                    dtNextPayTime.EditValue = null;
                    cboPayForm.EditValue = null;
                    txtPayFormCode.Text = null;
                    actionType = PEnum.ACTION_TYPE.VIEW;
                    impMestProposeId = null;
                }

                spinAmount.EditValue = null;
                spinNextAmount.EditValue = null;
                InitEnabledControl();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal GetImpMestPayAmount(long impMestProposeId, long? notId)
        {
            decimal result = 0;
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestPayFilter filter = new HisImpMestPayFilter();
                filter.IMP_MEST_PROPOSE_ID = impMestProposeId;
                var impMests = new BackendAdapter(param)
                    .Get<List<HIS_IMP_MEST_PAY>>("api/HisImpMestPay/Get", ApiConsumers.MosConsumer, filter, param);
                if (impMests != null && impMests.Count > 0)
                {
                    if (notId.HasValue)
                        impMests = impMests.Where(o => o.ID != notId.Value).ToList();
                    result = impMests.Sum(o => o.AMOUNT);
                }


            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private decimal GetImpMestAmount(long impMestProposeId)
        {
            decimal result = 0;
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestFilter filter = new HisImpMestFilter();
                filter.IMP_MEST_PROPOSE_ID = impMestProposeId;
                var impMests = new BackendAdapter(param)
                    .Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, filter, param);
                if (impMests != null && impMests.Count > 0)
                {
                    result = impMests.Sum(o => o.DOCUMENT_PRICE ?? 0 - o.DISCOUNT ?? 0);
                }


            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitEnabledControl()
        {
            try
            {
                switch (this.actionType)
                {
                    case PEnum.ACTION_TYPE.CREATE:
                        btnCreate.Enabled = true;
                        btnUpdate.Enabled = false;
                        btnRefesh.Enabled = true;
                        txtImpMestCodeCreate.Enabled = true;
                        break;
                    case PEnum.ACTION_TYPE.UPDATE:
                        btnCreate.Enabled = false;
                        btnUpdate.Enabled = true;
                        txtImpMestCodeCreate.Enabled = false;
                        break;
                    case PEnum.ACTION_TYPE.VIEW:
                        btnCreate.Enabled = false;
                        btnUpdate.Enabled = false;
                        txtImpMestCodeCreate.Enabled = true;
                        break;
                    default:
                        btnCreate.Enabled = false;
                        btnUpdate.Enabled = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadImpMestPayEdit(V_HIS_IMP_MEST_PAY impMestPay)
        {
            try
            {
                if (impMestPay != null)
                {
                    decimal totalDocumentPrice = GetImpMestAmount(impMestPay.IMP_MEST_PROPOSE_ID);
                    lblDocumentPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalDocumentPrice, ConfigApplications.NumberSeperator);
                    lblSupplier.Text = impMestPay.SUPPLIER_NAME;
                    txtImpMestCodeCreate.Text = impMestPay.IMP_MEST_PROPOSE_CODE;
                    decimal impMestPayAmount = GetImpMestPayAmount(impMestPay.IMP_MEST_PROPOSE_ID, impMestPay.ID);
                    lblImpMestPayAmount.Text = Inventec.Common.Number.Convert.NumberToString(impMestPayAmount, ConfigApplications.NumberSeperator);
                    txtPayerLoginname.Text = impMestPay.PAYER_LOGINNAME;
                    cboPayer.EditValue = impMestPay.PAYER_LOGINNAME;
                    if (impMestPay.PAY_FORM_ID.HasValue)
                    {
                        HIS_PAY_FORM payForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>()
                            .FirstOrDefault(o => o.ID == impMestPay.PAY_FORM_ID);
                        if (payForm != null)
                        {
                            txtPayFormCode.Text = payForm.PAY_FORM_CODE;
                            cboPayForm.EditValue = payForm.ID;
                        }
                        else
                        {
                            txtPayFormCode.Text = "";
                            cboPayForm.EditValue = null;
                        }
                    }
                    else
                    {
                        txtPayFormCode.Text = "";
                        cboPayForm.EditValue = null;
                    }

                    spinAmount.Value = impMestPay.AMOUNT;
                    lblRemainAmount.Text = Inventec.Common.Number.Convert.NumberToString((totalDocumentPrice - impMestPayAmount - impMestPay.AMOUNT), ConfigApplications.NumberSeperator);

                    if (impMestPay.PAY_TIME > 0)
                    {
                        dtPayForm.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(impMestPay.PAY_TIME).Value;
                    }
                    else
                    {
                        dtPayForm.EditValue = null;
                    }

                    if (impMestPay.NEXT_PAY_TIME.HasValue)
                    {
                        dtNextPayTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(impMestPay.NEXT_PAY_TIME.Value).Value;
                    }
                    else
                    {
                        dtNextPayTime.EditValue = null;
                    }
                    if (impMestPay.NEXT_AMOUNT.HasValue)
                        spinNextAmount.Value = impMestPay.NEXT_AMOUNT.Value;
                    else
                    {
                        spinNextAmount.EditValue = null;
                    }

                    actionType = PEnum.ACTION_TYPE.UPDATE;
                    impMestPayId = impMestPay.ID;
                    InitEnabledControl();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadPayerLoginname(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPayer.Focus();
                    cboPayer.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME.ToUpper().Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPayer.EditValue = data[0].LOGINNAME;
                            cboPayer.Properties.Buttons[1].Visible = true;
                            txtPayFormCode.Focus();
                            txtPayFormCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.LOGINNAME == searchCode);
                            if (search != null)
                            {
                                cboPayer.EditValue = search.ID;
                                cboPayer.Properties.Buttons[1].Visible = true;
                                txtPayFormCode.Focus();
                                txtPayFormCode.SelectAll();
                            }
                            else
                            {
                                cboPayer.EditValue = null;
                                cboPayer.Focus();
                                cboPayer.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboPayer.EditValue = null;
                        cboPayer.Focus();
                        cboPayer.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadPayerForm(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPayForm.Focus();
                    cboPayForm.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.PAY_FORM_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPayForm.EditValue = data[0].ID;
                            cboPayForm.Properties.Buttons[1].Visible = true;
                            spinAmount.Focus();
                            spinAmount.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PAY_FORM_CODE == searchCode);
                            if (search != null)
                            {
                                cboPayForm.EditValue = search.ID;
                                cboPayForm.Properties.Buttons[1].Visible = true;
                                spinAmount.Focus();
                                spinAmount.SelectAll();
                            }
                            else
                            {
                                cboPayForm.EditValue = null;
                                cboPayForm.Focus();
                                cboPayForm.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboPayForm.EditValue = null;
                        cboPayForm.Focus();
                        cboPayForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ImpMestPay.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestPay.FromDesign
{
    public partial class frmImpMestPayPlus : FormBase
    {
        private int positionHandleControl = -1;

        private int proposeStart = 0;
        private int proposeLimit = 0;
        private int proposeCount = 0;
        private int proposeTotalData = 0;
        private int payStart = 0;
        private int payLimit = 0;
        private int payCount = 0;
        private int payTotalData = 0;

        private long proposeId;

        private V_HIS_IMP_MEST_PROPOSE currentPropose = null;
        private V_HIS_IMP_MEST_PAY currentPay = null;
        Inventec.Desktop.Common.Modules.Module module;
        private enum ACCTION_TYPE
        {
            CREATE = 1,
            UPDATE = 2,
            DEFAULT = 3
        }

        private string Loginname;

        public frmImpMestPayPlus(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            this.module = module;
            InitializeComponent();
        }

        public frmImpMestPayPlus(Inventec.Desktop.Common.Modules.Module module, long impMestProposeId)
            : base(module)
        {
            this.module = module;
            InitializeComponent();
            proposeId = impMestProposeId;
        }

        private void frmImpMestPayPlus_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LogSystem.Info("frmImpMestPayPlus_Load.1");
                Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                LogSystem.Info("frmImpMestPayPlus_Load.2");
                ValidControl();
                LogSystem.Info("frmImpMestPayPlus_Load.3");
                LoadDataToComboUser();
                LogSystem.Info("frmImpMestPayPlus_Load.4");
                LoadGrid();
                LogSystem.Info("frmImpMestPayPlus_Load.5");
                LoadCboPayForm();
                LogSystem.Info("frmImpMestPayPlus_Load.6");
                cboProposeStt.SelectedIndex = 0;
                LoadImpMestPropose();
                LogSystem.Info("frmImpMestPayPlus_Load.7");
                SetDataToControlPay();
                LogSystem.Info("frmImpMestPayPlus_Load.8");
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadImpMestPropose()
        {
            try
            {
                if (this.proposeId > 0)
                {
                    HisImpMestProposeViewFilter filter = new HisImpMestProposeViewFilter();
                    filter.ID = this.proposeId;
                    List<V_HIS_IMP_MEST_PROPOSE> proposes = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_PROPOSE>>("api/HisImpMestPropose/GetView", ApiConsumers.MosConsumer, filter, null);
                    if (proposes != null && proposes.Count == 1)
                    {
                        this.currentPropose = proposes.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadGrid()
        {
            try
            {
#if DEBUG
                {
                    LogSystem.Info("frmImpMestPayPlus_Load.4.1");
                    this.FillDataImpMesProposeToGrid();
                    this.FillDataImpMestPayToGrid();
                    LogSystem.Info("frmImpMestPayPlus_Load.4.2");
                }
#else
                {
                    LogSystem.Info("frmImpMestPayPlus_Load.4.3");
                    List<Action> methods = new List<Action>();
                    methods.Add(this.FillDataImpMesProposeToGrid);
                    methods.Add(this.FillDataImpMestPayToGrid);
                    ThreadCustomManager.MultipleThreadWithJoin(methods);
                
                    LogSystem.Info("frmImpMestPayPlus_Load.4.4");
                }
#endif
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataImpMesProposeToGrid()
        {
            try
            {
                int num = 50;
                if (ucPagingPropose.pagingGrid != null)
                {
                    num = ucPagingPropose.pagingGrid.PageSize;
                }
                else
                {
                    num = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__NUM_PAGESIZE);
                }

                PagingImpMestPropose(new CommonParam(0, (int)num));
                CommonParam param = new CommonParam();
                param.Limit = proposeCount;
                param.Count = proposeTotalData;
                ucPagingPropose.Init(PagingImpMestPropose, param, num, gridControlImpMestPropose);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PagingImpMestPropose(object param)
        {
            try
            {
                proposeStart = ((CommonParam)param).Start ?? 0;
                proposeLimit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(proposeStart, proposeLimit);
                List<V_HIS_IMP_MEST_PROPOSE> listData = new List<V_HIS_IMP_MEST_PROPOSE>();

                HisImpMestProposeViewFilter proposeFilter = new HisImpMestProposeViewFilter();
                if (this.proposeId > 0)
                {
                    proposeFilter.ID = this.proposeId;
                }
                else
                {
                    proposeFilter.KEY_WORD = txtKeywordPropose.Text;
                    proposeFilter.ORDER_FIELD = "CREATE_TIME";
                    proposeFilter.ORDER_DIRECTION = "DESC";
                    if (cboProposeStt.SelectedIndex == 1)
                    {
                        proposeFilter.PAY_STATUS = (short)1;
                    }
                    else if (cboProposeStt.SelectedIndex == 2)
                    {
                        proposeFilter.PAY_STATUS = (short)2;
                    }
                    else if (cboProposeStt.SelectedIndex == 3)
                    {
                        proposeFilter.PAY_STATUS = (short)3;
                    }

                }
                var rs = new BackendAdapter(paramCommon).GetRO<List<V_HIS_IMP_MEST_PROPOSE>>("api/HisImpMestPropose/GetView", ApiConsumers.MosConsumer, proposeFilter, paramCommon);

                if (rs != null)
                {
                    listData = (List<V_HIS_IMP_MEST_PROPOSE>)rs.Data;
                    proposeCount = (listData == null ? 0 : listData.Count);
                    proposeTotalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }

                gridControlImpMestPropose.BeginUpdate();
                gridControlImpMestPropose.DataSource = listData;
                gridControlImpMestPropose.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void FillDataImpMestPayToGrid()
        {
            try
            {
                int num = 50;
                if (ucPagingPay.pagingGrid != null)
                {
                    num = ucPagingPay.pagingGrid.PageSize;
                }
                else
                {
                    num = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__NUM_PAGESIZE);
                }

                PagingImpMestPay(new CommonParam(0, (int)num));
                CommonParam param = new CommonParam();
                param.Limit = payCount;
                param.Count = payTotalData;
                ucPagingPay.Init(PagingImpMestPay, param, num, gridControlImpMestPay);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PagingImpMestPay(object param)
        {
            try
            {
                payStart = ((CommonParam)param).Start ?? 0;
                payLimit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(payStart, payLimit);
                List<V_HIS_IMP_MEST_PAY> listData = new List<V_HIS_IMP_MEST_PAY>();

                HisImpMestPayViewFilter payFilter = new HisImpMestPayViewFilter();
                if (this.proposeId > 0)
                {
                    payFilter.IMP_MEST_PROPOSE_ID = this.proposeId;
                }

                payFilter.KEY_WORD = txtKeywordPay.Text;
                payFilter.ORDER_FIELD = "PAY_TIME";
                payFilter.ORDER_DIRECTION = "DESC";

                var rs = new BackendAdapter(paramCommon).GetRO<List<V_HIS_IMP_MEST_PAY>>("api/HisImpMestPay/GetView", ApiConsumers.MosConsumer, payFilter, paramCommon);

                if (rs != null)
                {
                    listData = (List<V_HIS_IMP_MEST_PAY>)rs.Data;
                    payCount = (listData == null ? 0 : listData.Count);
                    payTotalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }

                gridControlImpMestPay.BeginUpdate();
                gridControlImpMestPay.DataSource = listData;
                gridControlImpMestPay.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDataToControlPay()
        {
            try
            {
                txtImpMestProposeCode.Text = "";
                lblSupplierName.Text = "";
                lblTotalPay.Text = Inventec.Common.Number.Convert.NumberToString(0, 0);
                lblPayed.Text = Inventec.Common.Number.Convert.NumberToString(0, 0);
                dtPayTime.DateTime = DateTime.Now;
                cboPayer.EditValue = Loginname;
                spinAmount.Value = 0;
                lblRemainAmount.Text = Inventec.Common.Number.Convert.NumberToString(0, 0);
                txtPayslipCode.Text = "";
                txtStandingOrderCode.Text = "";
                dtNextPayTime.EditValue = null;
                spinNextPayAmount.EditValue = null;
                txtDescription.Text = "";
                btnAdd.Enabled = false;
                btnEdit.Enabled = false;
                txtImpMestProposeCode.Enabled = true;
                if (this.currentPropose != null)
                {
                    txtImpMestProposeCode.Text = this.currentPropose.IMP_MEST_PROPOSE_CODE ?? "";
                    lblSupplierName.Text = this.currentPropose.SUPPLIER_NAME ?? "";
                    lblTotalPay.Text = Inventec.Common.Number.Convert.NumberToString(this.currentPropose.TOTAL_PAY ?? 0, 0);
                    lblPayed.Text = Inventec.Common.Number.Convert.NumberToString(this.currentPropose.PAYED ?? 0, 0);
                    if (cboPayForm.EditValue == null)
                    {
                        cboPayForm.EditValue = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.PAY_FORM_CODE == "01").ID;
                    }
                    spinAmount.EditValue = Inventec.Common.Number.Convert.NumberToString((this.currentPropose.TOTAL_PAY ?? 0) - (this.currentPropose.PAYED ?? 0), 0);
                    decimal SoTien = 0;
                    if (spinAmount.EditValue != null)
                    {
                        SoTien = Inventec.Common.TypeConvert.Parse.ToInt64(spinAmount.EditValue.ToString() ?? "");
                    }
                    lblRemainAmount.Text = Inventec.Common.Number.Convert.NumberToString((this.currentPropose.TOTAL_PAY ?? 0) - (this.currentPropose.PAYED ?? 0) - SoTien, 0);
                    btnAdd.Enabled = true;
                    btnEdit.Enabled = false;
                    dtPayTime.Focus();
                }
                else if (this.currentPay != null)
                {
                    txtImpMestProposeCode.Text = this.currentPay.IMP_MEST_PROPOSE_CODE ?? "";
                    lblSupplierName.Text = this.currentPay.SUPPLIER_NAME ?? "";
                    lblTotalPay.Text = Inventec.Common.Number.Convert.NumberToString(this.currentPay.TOTAL_PAY ?? 0, 0);
                    lblPayed.Text = Inventec.Common.Number.Convert.NumberToString(this.currentPay.PAYED ?? 0, 0);
                    dtPayTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentPay.PAY_TIME).Value;
                    cboPayer.EditValue = this.currentPay.PAYER_LOGINNAME;
                    cboPayForm.EditValue = this.currentPay.PAY_FORM_ID;
                    spinAmount.Value = this.currentPay.AMOUNT;
                    decimal SoTien = 0;
                    if (this.currentPay.AMOUNT != null)
                    {
                        SoTien = Inventec.Common.TypeConvert.Parse.ToInt64(this.currentPay.AMOUNT.ToString() ?? "");
                    }
                    // lblRemainAmount.Text = Inventec.Common.Number.Convert.NumberToString((this.currentPay.TOTAL_PAY ?? 0) + (this.currentPay.AMOUNT) - (this.currentPay.PAYED ?? 0), 0);
                    lblRemainAmount.Text = Inventec.Common.Number.Convert.NumberToString((this.currentPay.TOTAL_PAY ?? 0) - (this.currentPay.AMOUNT) - (this.currentPay.PAYED ?? 0), 0);
                    txtPayslipCode.Text = this.currentPay.PAYSLIP_CODE ?? "";
                    txtStandingOrderCode.Text = this.currentPay.STANDING_ORDER_CODE ?? "";
                    if (this.currentPay.NEXT_PAY_TIME.HasValue)
                    {
                        dtNextPayTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentPay.NEXT_PAY_TIME.Value).Value;
                    }
                    spinNextPayAmount.EditValue = this.currentPay.NEXT_AMOUNT;
                    txtDescription.Text = this.currentPay.DESCRIPTION ?? "";
                    btnAdd.Enabled = false;
                    btnEdit.Enabled = true;
                    txtImpMestProposeCode.Enabled = false;
                    dtPayTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboUser()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.InitComboUser(); }));
                }
                else
                {
                    this.InitComboUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Load người chỉ định
        private async Task InitComboUser()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("InitComboUser. 1");
                List<ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboPayer, datas, controlEditorADO);

                var data = datas.Where(o => o.LOGINNAME.ToUpper().Equals(Loginname.ToUpper())).FirstOrDefault();
                if (data != null)
                {
                    this.cboPayer.EditValue = data.LOGINNAME;
                    this.txtPayerLoginname.Text = data.LOGINNAME;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboPayForm()
        {
            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 250, 2));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPayForm, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestPropose_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_IMP_MEST_PROPOSE dataRow = (V_HIS_IMP_MEST_PROPOSE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + proposeStart;
                    }
                    else if (e.Column.FieldName == "USER_STR")
                    {
                        if (!String.IsNullOrWhiteSpace(dataRow.PROPOSE_LOGINNAME))
                        {
                            e.Value = String.Format("{0} - {1}", dataRow.PROPOSE_LOGINNAME, dataRow.PROPOSE_USERNAME);
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.MODIFY_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.CREATE_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestPropose_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                V_HIS_IMP_MEST_PROPOSE dataRow = (V_HIS_IMP_MEST_PROPOSE)gridViewImpMestPropose.GetRow(e.RowHandle);
                if (dataRow != null)
                {
                    decimal totalPay = dataRow.TOTAL_PAY ?? 0;
                    decimal payed = dataRow.PAYED ?? 0;
                    if (payed != totalPay)
                    {
                        if (payed == 0)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (payed < totalPay)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestPropose_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                this.currentPay = null;
                this.currentPropose = (V_HIS_IMP_MEST_PROPOSE)gridViewImpMestPropose.GetFocusedRow();
                this.SetDataToControlPay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeywordPropose_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnProposeFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnProposeFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnProposeFind.Enabled) return;
                WaitingManager.Show();
                FillDataImpMesProposeToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestPay_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_IMP_MEST_PAY dataRow = (V_HIS_IMP_MEST_PAY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + payStart;
                    }
                    else if (e.Column.FieldName == "PAY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.PAY_TIME);
                    }
                    else if (e.Column.FieldName == "USER_PAY")
                    {
                        if (!String.IsNullOrWhiteSpace(dataRow.PAYER_LOGINNAME))
                        {
                            e.Value = String.Format("{0} - {1}", dataRow.PAYER_LOGINNAME, dataRow.PAYER_USERNAME);
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "NEXT_PAY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.NEXT_PAY_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.MODIFY_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.CREATE_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestPay_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                V_HIS_IMP_MEST_PAY dataRow = (V_HIS_IMP_MEST_PAY)gridViewImpMestPay.GetRow(e.RowHandle);
                if (dataRow != null)
                {
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (dataRow.CREATOR == Loginname || CheckLoginAdmin.IsAdmin(Loginname))
                        {
                            e.RepositoryItem = repositoryItemButton_Pay_Delete_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton_Pay_Delete_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestPay_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                this.currentPropose = null;
                this.currentPay = (V_HIS_IMP_MEST_PAY)gridViewImpMestPay.GetFocusedRow();
                this.SetDataToControlPay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeywordPay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnPayFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPayFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPayFind.Enabled) return;
                WaitingManager.Show();
                FillDataImpMestPayToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMestProposeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    this.currentPay = null;
                    this.currentPropose = null;
                    string code = txtImpMestProposeCode.Text.Trim();
                    if (code.Length < 8 && checkDigit(code))
                    {
                        code = string.Format("{0:00000000}", Convert.ToInt64(code));
                        txtImpMestProposeCode.Text = code;
                    }

                    CommonParam param = new CommonParam();

                    HisImpMestProposeViewFilter _propose = new HisImpMestProposeViewFilter();
                    _propose.IMP_MEST_PROPOSE_CODE__EXACT = code;

                    var impMestProposes = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_PROPOSE>>("api/HisImpMestPropose/GetView", ApiConsumers.MosConsumer, _propose, param);
                    if (impMestProposes != null && impMestProposes.Count == 1)
                    {
                        this.currentPropose = impMestProposes.FirstOrDefault();
                        this.SetDataToControlPay();
                    }
                    WaitingManager.Hide();
                    dtPayTime.Focus();
                    dtPayTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPayTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtPayerLoginname.Focus();
                    txtPayerLoginname.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPayTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPayerLoginname.Focus();
                    txtPayerLoginname.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPayerLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        cboPayer.Focus();
                        cboPayer.ShowPopup();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboPayer.EditValue = searchResult[0].USERNAME;
                            txtPayFormCode.Focus();
                            txtPayFormCode.SelectAll();
                        }
                        else
                        {
                            cboPayer.Focus();
                            cboPayer.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayer_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtPayFormCode.Focus();
                    txtPayFormCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayer_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPayerLoginname.Text = "";
                if (cboPayer.EditValue != null)
                {
                    var data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME.ToUpper().Contains(cboPayer.EditValue.ToString().ToUpper()));
                    if (data != null)
                    {
                        txtPayerLoginname.Text = data.LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayer_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboPayer.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string key = txtPayFormCode.Text.ToLower().Trim();
                    if (String.IsNullOrWhiteSpace(key))
                    {
                        cboPayForm.Focus();
                        cboPayForm.ShowPopup();
                    }
                    else
                    {
                        var listData = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.PAY_FORM_CODE.ToLower().Contains(key) || o.PAY_FORM_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            cboPayForm.EditValue = listData[0].ID;
                            spinAmount.Focus();
                            spinAmount.SelectAll();
                        }
                        else
                        {
                            cboPayForm.Focus();
                            cboPayForm.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPayFormCode.Text = "";
                if (cboPayForm.EditValue != null)
                {
                    var data = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                    if (data != null)
                    {
                        txtPayFormCode.Text = data.PAY_FORM_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboPayForm.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPayslipCode.Focus();
                    txtPayslipCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinAmount.ContainsFocus)
                {
                    decimal documentPrice = Inventec.Common.TypeConvert.Parse.ToDecimal(lblTotalPay.Text);
                    decimal impMestPayAmount = Inventec.Common.TypeConvert.Parse.ToDecimal(lblPayed.Text);
                    lblRemainAmount.Text = Inventec.Common.Number.Convert.NumberToString((documentPrice - impMestPayAmount - spinAmount.Value), 0);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPayslipCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtStandingOrderCode.Focus();
                    txtStandingOrderCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStandingOrderCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtNextPayTime.Focus();
                    dtNextPayTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtNextPayTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    spinNextPayAmount.Focus();
                    spinNextPayAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtNextPayTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinNextPayAmount.Focus();
                    spinNextPayAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinNextPayAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Propose_View_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_IMP_MEST_PROPOSE)gridViewImpMestPropose.GetFocusedRow();
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisSuggestedPayment").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisSuggestedPayment");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row);
                        listArgs.Add((int)3);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Pay_Delete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DialogResult myResult;
                myResult = MessageBox.Show("Bạn có muốn xóa thông tin thanh toán?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (myResult != DialogResult.OK)
                {
                    return;
                }
                this.currentPay = null;
                V_HIS_IMP_MEST_PAY impMestPay = gridViewImpMestPay.GetFocusedRow() as V_HIS_IMP_MEST_PAY;
                if (impMestPay != null)
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();

                    bool result = new BackendAdapter(param).Post<bool>("api/HisImpMestPay/Delete", ApiConsumers.MosConsumer, impMestPay.ID, param);
                    if (result)
                    {
                        PagingImpMestPropose(new CommonParam(proposeStart, proposeLimit));
                        PagingImpMestPay(new CommonParam(payStart, payLimit));
                        SetDataToControlPay();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, result);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnAdd.Enabled || this.currentPropose == null || !dxValidationProvider1.Validate()) return;
                decimal remainAmount = 0;
                if (lblRemainAmount != null)
                    remainAmount = Inventec.Common.TypeConvert.Parse.ToDecimal(lblRemainAmount.Text);
                if (remainAmount < 0)
                    MessageBox.Show("Số tiền còn lại nhỏ hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_IMP_MEST_PAY impMestPay = new HIS_IMP_MEST_PAY();
                this.CreateData(ref impMestPay);

                var result = new BackendAdapter(param).Post<HIS_IMP_MEST_PAY>("api/HisImpMestPay/Create", ApiConsumers.MosConsumer, impMestPay, param);
                if (result != null)
                {
                    this.currentPay = null;
                    this.currentPropose = null;
                    success = true;
                    PagingImpMestPropose(new CommonParam(proposeStart, proposeLimit));
                    PagingImpMestPay(new CommonParam(payStart, payLimit));
                    SetDataToControlPay();
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnEdit.Enabled || this.currentPay == null || !dxValidationProvider1.Validate()) return;
                decimal remainAmount = 0;
                if (lblRemainAmount != null)
                    remainAmount = Inventec.Common.TypeConvert.Parse.ToDecimal(lblRemainAmount.Text);
                if (remainAmount < 0)
                    MessageBox.Show("Số tiền còn lại nhỏ hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                Mapper.CreateMap<V_HIS_IMP_MEST_PAY, HIS_IMP_MEST_PAY>();

                HIS_IMP_MEST_PAY impMestPay = Mapper.Map<HIS_IMP_MEST_PAY>(this.currentPay);
                this.CreateData(ref impMestPay);

                var result = new BackendAdapter(param).Post<HIS_IMP_MEST_PAY>("api/HisImpMestPay/Update", ApiConsumers.MosConsumer, impMestPay, param);
                if (result != null)
                {
                    this.currentPay = null;
                    this.currentPropose = null;
                    success = true;
                    PagingImpMestPropose(new CommonParam(proposeStart, proposeLimit));
                    PagingImpMestPay(new CommonParam(payStart, payLimit));
                    SetDataToControlPay();
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentPay = null;
                this.currentPropose = null;
                SetDataToControlPay();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateData(ref HIS_IMP_MEST_PAY impMestPay)
        {
            try
            {
                if (impMestPay == null) impMestPay = new HIS_IMP_MEST_PAY();
                if (this.currentPropose != null) impMestPay.IMP_MEST_PROPOSE_ID = this.currentPropose.ID;

                impMestPay.AMOUNT = spinAmount.Value;
                if (spinNextPayAmount.EditValue != null)
                {
                    impMestPay.NEXT_AMOUNT = spinNextPayAmount.Value;
                }
                if (dtNextPayTime.EditValue != null && dtNextPayTime.DateTime != DateTime.MinValue)
                    impMestPay.NEXT_PAY_TIME = Convert.ToInt64(dtNextPayTime.DateTime.ToString("yyyyMMddHHmmss"));

                if (cboPayForm.EditValue != null)
                {
                    impMestPay.PAY_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString());
                }
                impMestPay.PAY_TIME = Convert.ToInt64(dtPayTime.DateTime.ToString("yyyyMMddHHmmss"));
                if (cboPayer.EditValue != null)
                {
                    ACS_USER user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboPayer.EditValue.ToString());
                    if (user != null)
                    {
                        impMestPay.PAYER_LOGINNAME = user.LOGINNAME;
                        impMestPay.PAYER_USERNAME = user.USERNAME;
                    }
                }
                impMestPay.PAYSLIP_CODE = txtPayslipCode.Text;
                impMestPay.STANDING_ORDER_CODE = txtStandingOrderCode.Text;
                impMestPay.DESCRIPTION = txtDescription.Text;
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

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlAmount();
                ValidateSingleControl(dtPayTime);
                ValidationControlMaxLength(txtPayslipCode, 20, dxValidationProvider1);
                ValidationControlMaxLength(txtStandingOrderCode, 200, dxValidationProvider1);
                ValidationControlMaxLength(txtDescription, 2000, dxValidationProvider1);
                ValidControlNextAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlAmount()
        {
            try
            {
                AmountValidationRule validRule = new AmountValidationRule();
                validRule.spinAmount = spinAmount;
                validRule.lblRemainAmount = lblRemainAmount;
                dxValidationProvider1.SetValidationRule(spinAmount, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlNextAmount()
        {
            try
            {
                NextAmountValidationRule validRule = new NextAmountValidationRule();
                validRule.spinAmount = spinAmount;
                validRule.lblRemainAmount = lblRemainAmount;
                validRule.spinNextAmount = spinNextPayAmount;
                dxValidationProvider1.SetValidationRule(spinNextPayAmount, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dx, [Optional] bool IsRequest)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.IsRequired = IsRequest;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            dx.SetValidationRule(control, validate);
        }

        private void ValidateSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButton_ProposeFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnProposeFind_Click(null, null);
        }

        private void barButton_PayFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnPayFind_Click(null, null);
        }

        private void barButton_Add_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
                btnAdd_Click(null, null);
        }

        private void barButton_Edit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnEdit.Enabled)
                btnEdit_Click(null, null);
        }

        private void barButton_New_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnNew_Click(null, null);
        }

        private void repPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate("Mps000327", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000327":
                        Mps000327(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void Mps000327(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.module.RoomId);

                var impMestPay = (V_HIS_IMP_MEST_PAY)gridViewImpMestPay.GetFocusedRow();

                HisImpMestProposeViewFilter filter = new HisImpMestProposeViewFilter();
                filter.ID = impMestPay.IMP_MEST_PROPOSE_ID;
                List<V_HIS_IMP_MEST_PROPOSE> proposes = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_PROPOSE>>("api/HisImpMestPropose/GetView", ApiConsumers.MosConsumer, filter, null);
                var prop = new V_HIS_IMP_MEST_PROPOSE();
                if (proposes != null && proposes.Count == 1)
                {
                    prop = proposes.FirstOrDefault();
                }

                if (prop.ID == 0)
                    return;
                MOS.Filter.HisImpMestViewFilter Impfilter = new HisImpMestViewFilter();
                Impfilter.IMP_MEST_PROPOSE_ID = prop.ID;
                List<V_HIS_IMP_MEST> impMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, Impfilter, null);
                MPS.Processor.Mps000327.PDO.Mps000327PDO mps000327RDO = new MPS.Processor.Mps000327.PDO.Mps000327PDO(
               prop,
                impMest,
                 new List<V_HIS_IMP_MEST_PAY> { impMestPay }
                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000327RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000327RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);


                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisMestPatyTrty
{
    public partial class frmHisMestPatyTrty : FormBase
    {
        private int rowCount = 0;
        private int dataTotal = 0;
        private int start = 0;
        private int limit = 0;
        private int positionHandle = -1;

        private V_HIS_MEST_PATY_TRTY currentData = null;

        public frmHisMestPatyTrty(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void frmHisMestPatyTrty_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.ValidControl();
                this.InitComboMediStock();
                this.InitComboPatientType();
                this.InitComboTreatmentType();
                this.FillDataToGridControl();
                this.SetDataToControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                this.ValidateGridLookupWithTextEdit(cboMediStock, txtMediStockCode, dxValidationProvider1);
                this.ValidationSingleControl(cboPatientType, dxValidationProvider1);
                this.ValidationSingleControl(cboTreatmentType, dxValidationProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboMediStock()
        {
            try
            {
                List<V_HIS_MEDI_STOCK> lstData = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                colum.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 200, 2));
                ControlEditorADO controlADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", colum, false, 300);
                ControlEditorLoader.Load(cboMediStock, lstData, controlADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboPatientType()
        {
            try
            {
                List<HIS_PATIENT_TYPE> lstData = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 80, 1));
                colum.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 220, 2));
                ControlEditorADO controlADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", colum, false, 300);
                ControlEditorLoader.Load(cboPatientType, lstData, controlADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboTreatmentType()
        {
            try
            {
                List<HIS_TREATMENT_TYPE> lstData = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("TREATMENT_TYPE_CODE", "", 100, 1));
                colum.Add(new ColumnInfo("TREATMENT_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "ID", colum, false, 300);
                ControlEditorLoader.Load(cboTreatmentType, lstData, controlADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FillDataToGridControl()
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, numPageSize, this.gridControlMestPatyTrty);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                List<V_HIS_MEST_PATY_TRTY> listData = new List<V_HIS_MEST_PATY_TRTY>();
                this.start = ((CommonParam)param).Start ?? 0;
                this.limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisMestPatyTrtyViewFilter filter = new HisMestPatyTrtyViewFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                if (!String.IsNullOrWhiteSpace(this.txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                }

                var rs = new BackendAdapter(paramCommon).GetRO<List<V_HIS_MEST_PATY_TRTY>>("api/HisMestPatyTrty/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (rs != null)
                {
                    listData = rs.Data;
                    rowCount = (listData == null ? 0 : listData.Count);
                    dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }
                gridControlMestPatyTrty.BeginUpdate();
                gridControlMestPatyTrty.DataSource = listData;
                gridControlMestPatyTrty.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToControl()
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                if (this.currentData != null)
                {
                    btnAdd.Enabled = false;
                    btnEdit.Enabled = true;
                    cboMediStock.EditValue = this.currentData.MEDI_STOCK_ID;
                    cboPatientType.EditValue = this.currentData.PATIENT_TYPE_ID;
                    cboTreatmentType.EditValue = this.currentData.TREATMENT_TYPE_ID;
                }
                else
                {
                    btnAdd.Enabled = true;
                    btnEdit.Enabled = false;
                    cboMediStock.EditValue = null;
                    cboPatientType.EditValue = null;
                    cboTreatmentType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                    {
                        btnFind_Click(null, null);
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled) return;
                WaitingManager.Show();
                this.currentData = null;
                this.SetDataToControl();
                this.FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMestPatyTrty_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    V_HIS_MEST_PATY_TRTY pData = (V_HIS_MEST_PATY_TRTY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMestPatyTrty_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {

                    V_HIS_MEST_PATY_TRTY data = (V_HIS_MEST_PATY_TRTY)gridViewMestPatyTrty.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "CHANGE_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? repositoryItemBtn_Unlock : repositoryItemBtn_Lock);

                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = repositoryItemBtn_Delete_Enable;
                            else
                                e.RepositoryItem = repositoryItemBtn_Delete_Disable;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediStockCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtMediStockCode.Text))
                    {
                        string key = txtMediStockCode.Text.ToLower().Trim();
                        List<V_HIS_MEDI_STOCK> lstData = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.MEDI_STOCK_CODE.ToLower().Contains(key)).ToList();
                        if (lstData != null && lstData.Count == 1)
                        {
                            cboMediStock.EditValue = lstData[0].ID;
                        }
                        else
                        {
                            cboMediStock.Focus();
                            cboMediStock.ShowPopup();
                        }
                    }
                    else
                    {
                        cboMediStock.Focus();
                        cboMediStock.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboPatientType.Focus();
                    cboPatientType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtMediStockCode.Text = "";
                if (cboMediStock.EditValue != null)
                {
                    V_HIS_MEDI_STOCK stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboMediStock.EditValue));
                    if (stock != null)
                    {
                        txtMediStockCode.Text = stock.MEDI_STOCK_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboTreatmentType.Focus();
                    cboTreatmentType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtn_Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_MEST_PATY_TRTY row = (V_HIS_MEST_PATY_TRTY)gridViewMestPatyTrty.GetFocusedRow();
                if (row == null)
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                HIS_MEST_PATY_TRTY rs = new BackendAdapter(param).Post<HIS_MEST_PATY_TRTY>("api/HisMestPatyTrty/ChangeLock", ApiConsumers.MosConsumer, row.ID, param);
                if (rs != null)
                {
                    success = true;
                    FillDataToGridControl();
                    btnRefresh_Click(null, null);
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

        private void repositoryItemBtn_Unlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_MEST_PATY_TRTY row = (V_HIS_MEST_PATY_TRTY)gridViewMestPatyTrty.GetFocusedRow();
                if (row == null)
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                HIS_MEST_PATY_TRTY rs = new BackendAdapter(param).Post<HIS_MEST_PATY_TRTY>("api/HisMestPatyTrty/ChangeLock", ApiConsumers.MosConsumer, row.ID, param);
                if (rs != null)
                {
                    success = true;
                    FillDataToGridControl();
                    btnRefresh_Click(null, null);
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

        private void repositoryItemBtn_Delete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_MEST_PATY_TRTY row = (V_HIS_MEST_PATY_TRTY)gridViewMestPatyTrty.GetFocusedRow();
                if (row == null)
                {
                    return;
                }
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>("/api/HisMestPatyTrty/Delete", ApiConsumers.MosConsumer, row.ID, param);
                    if (success)
                    {
                        FillDataToGridControl();
                        btnRefresh_Click(null, null);
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateControlToObject(HIS_MEST_PATY_TRTY data)
        {
            try
            {
                data.MEDI_STOCK_ID = Convert.ToInt64(cboMediStock.EditValue);
                data.PATIENT_TYPE_ID = Convert.ToInt64(cboPatientType.EditValue);
                data.TREATMENT_TYPE_ID = Convert.ToInt64(cboTreatmentType.EditValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnEdit.Enabled || !dxValidationProvider1.Validate() || this.currentData == null)
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                Mapper.CreateMap<V_HIS_MEST_PATY_TRTY, HIS_MEST_PATY_TRTY>();
                HIS_MEST_PATY_TRTY data = Mapper.Map<HIS_MEST_PATY_TRTY>(this.currentData);
                this.UpdateControlToObject(data);

                HIS_MEST_PATY_TRTY rs = new BackendAdapter(param).Post<HIS_MEST_PATY_TRTY>("api/HisMestPatyTrty/Update", ApiConsumers.MosConsumer, data, param);

                if (rs != null)
                {
                    success = true;
                    FillDataToGridControl();
                    btnRefresh_Click(null, null);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAdd.Enabled || !dxValidationProvider1.Validate())
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_MEST_PATY_TRTY data = new HIS_MEST_PATY_TRTY();
                this.UpdateControlToObject(data);

                HIS_MEST_PATY_TRTY rs = new BackendAdapter(param).Post<HIS_MEST_PATY_TRTY>("api/HisMestPatyTrty/Create", ApiConsumers.MosConsumer, data, param);

                if (rs != null)
                {
                    success = true;
                    FillDataToGridControl();
                    btnRefresh_Click(null, null);
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRefresh.Enabled) return;

                WaitingManager.Show();
                this.currentData = null;
                this.SetDataToControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMestPatyTrty_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (V_HIS_MEST_PATY_TRTY)gridViewMestPatyTrty.GetFocusedRow();
                this.SetDataToControl();
                if (this.currentData != null)
                {
                    txtMediStockCode.Focus();
                    txtMediStockCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMestPatyTrty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.currentData = (V_HIS_MEST_PATY_TRTY)gridViewMestPatyTrty.GetFocusedRow();
                    this.SetDataToControl();
                    if (this.currentData != null)
                    {
                        txtMediStockCode.Focus();
                        txtMediStockCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboMediStock.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboPatientType.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    cboTreatmentType.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

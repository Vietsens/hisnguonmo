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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.ListMediStock
{
    public partial class frmListMediStock : HIS.Desktop.Utility.FormBase
    {
        internal List<V_HIS_MEDI_STOCK> mediStocks { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK currentData;
        int positionHandle = -1;
        int ActionType = -1;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        internal long roomId;
        internal long mediStockId;

        public frmListMediStock()
        {
            InitializeComponent();
        }

        public frmListMediStock(Inventec.Desktop.Common.Modules.Module _moduleData)
		:base(_moduleData)
        {
            InitializeComponent();
            this.currentModule = _moduleData;
        }

        private void frmListMediStock_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
                FillDataToControlsForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboDepartment();
                InitComboParent();

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboParent()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlRoomTypeADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboParent, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>(), controlRoomTypeADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDepartment()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDepartMentName, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewListMediStock_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK pData = (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            string modifyTime = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(modifyTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ALLOW_IMP_SUPPLIER_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_ALLOW_IMP_SUPPLIER == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi cho phep nhap tu NCC IS_ALLOW_IMP_SUPPLIER_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_BUSINESS_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_BUSINESS == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri kho kinh doanh IS_BUSINESS_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_AUTO_APPROVE_EXP_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_AUTO_APPROVE_EXP == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_AUTO_APPROVE_EXP_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_AUTO_EXECUTE_EXP_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_AUTO_EXECUTE_EXP == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_AUTO_EXECUTE_EXP_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_CABINET_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_CABINET == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_CABINET_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_AUTO_CREATE_CHMS_IMP_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_AUTO_CREATE_CHMS_IMP == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_AUTO_CREATE_CHMS_IMP_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_AUTO_APPROVE_IMP_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_AUTO_APPROVE_IMP == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_AUTO_APPROVE_IMP_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_GOODS_RESTRICT_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_GOODS_RESTRICT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_GOODS_RESTRICT_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_ACTIVE_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_PAUSE_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_PAUSE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_PAUSE_STR", ex);
                        }
                    }
                    gridControlListMediStock.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewListMediStock_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_MEDI_STOCK data = (V_HIS_MEDI_STOCK)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnLock : btnLock);

                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
                                e.RepositoryItem = btnDelete;
                            else
                                e.RepositoryItem = btnDeleteDisable;
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

        private void ChangedDataRow(V_HIS_MEDI_STOCK data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void FillDataToEditorControl(V_HIS_MEDI_STOCK data)
        {
            try
            {
                if (data != null)
                {
                    roomId = data.ROOM_ID;
                    mediStockId = data.ID;
                    txtMediStockCode.Text = data.MEDI_STOCK_CODE;
                    txtMediStockName.Text = data.MEDI_STOCK_NAME;
                    cboDepartMentName.EditValue = data.DEPARTMENT_ID;
                    cboParent.EditValue = data.PARENT_ID;
                    chkIsApproveExp.Checked = (data.IS_AUTO_APPROVE_EXP == 1 ? true : false);
                    chkIsApproveImp.Checked = (data.IS_AUTO_APPROVE_IMP == 1 ? true : false);
                    chkIsBusiness.Checked = (data.IS_BUSINESS == 1 ? true : false);
                    chkIsCabinet.Checked = (data.IS_CABINET == 1 ? true : false);
                    chkIsCreatImp.Checked = (data.IS_AUTO_CREATE_CHMS_IMP == 1 ? true : false);
                    chkIsExeCuteExp.Checked = (data.IS_AUTO_EXECUTE_EXP == 1 ? true : false);
                    chkIsPause.Checked = (data.IS_PAUSE == 1 ? true : false);
                    chkIsResTrict.Checked = (data.IS_GOODS_RESTRICT == 1 ? true : false);
                    chkIsSuppLier.Checked = (data.IS_ALLOW_IMP_SUPPLIER == 1 ? true : false);

                }
                else
                {
                    roomId = 0;
                    mediStockId = 0;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void gridViewListMediStock_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK)gridViewListMediStock.GetFocusedRow();
                if (rowData != null)
                {
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            {
                CommonParam param = new CommonParam();
                V_HIS_MEDI_STOCK hisMediStock = new V_HIS_MEDI_STOCK();
                bool notHandler = false;
                try
                {
                    WaitingManager.Show();
                    V_HIS_MEDI_STOCK dataMediStock = (V_HIS_MEDI_STOCK)gridViewListMediStock.GetFocusedRow();
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        V_HIS_MEDI_STOCK data1 = new V_HIS_MEDI_STOCK();
                        data1.ID = dataMediStock.ID;
                        WaitingManager.Show();
                        hisMediStock = new BackendAdapter(param).Post<V_HIS_MEDI_STOCK>("api/HisMediStock/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                        WaitingManager.Hide();
                        if (hisMediStock != null)
                        {
                            FillDataToGridControl();
                            btnEdit.Enabled = false;
                            notHandler = true;
                        }
                    }

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_MEDI_STOCK hisMediStock = new V_HIS_MEDI_STOCK();
            bool notHandler = false;
            try
            {
                WaitingManager.Show();
                V_HIS_MEDI_STOCK dataMediStock = (V_HIS_MEDI_STOCK)gridViewListMediStock.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_MEDI_STOCK data1 = new V_HIS_MEDI_STOCK();
                    data1.ID = dataMediStock.ID;
                    WaitingManager.Show();
                    hisMediStock = new BackendAdapter(param).Post<V_HIS_MEDI_STOCK>("api/HisMediStock/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisMediStock != null)
                    {
                        FillDataToGridControl();
                        btnEdit.Enabled = true;
                        notHandler = true;
                    }
                }
                MessageManager.Show(this.ParentForm, param, notHandler);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewListMediStock_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK)gridViewListMediStock.GetFocusedRow();
            if (rowData == null)
                return;
            if (rowData.IS_ACTIVE == 1 && e.Column.FieldName == "IS_ACTIVE_STR")
            {
                e.Appearance.ForeColor = Color.Green;
            }
            else if (rowData.IS_ACTIVE == 0 && e.Column.FieldName == "IS_ACTIVE_STR")
            {
                e.Appearance.ForeColor = Color.Black;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionEdit;
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.SDO.HisMediStockSDO mediStockSDO = new MOS.SDO.HisMediStockSDO();
                MOS.SDO.HisMediStockSDO mediStockResultSDO = new MOS.SDO.HisMediStockSDO();

                mediStockSDO.HisRoom = SetDataRoom();
                mediStockSDO.HisMediStock = SetDataMediStock();

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    mediStockResultSDO = new BackendAdapter(param)
                    .Post<MOS.SDO.HisMediStockSDO>("api/HisMediStock/Create", ApiConsumers.MosConsumer, mediStockSDO, param);

                }
                else
                {
                    if (roomId > 0 && mediStockId > 0)
                    {
                        mediStockSDO.HisRoom.ID = roomId;
                        mediStockSDO.HisMediStock.ID = mediStockId;
                        mediStockResultSDO = new BackendAdapter(param)
                        .Post<HisMediStockSDO>("api/HisMediStock/Update", ApiConsumers.MosConsumer, mediStockSDO, param);
                    }
                }
                if (mediStockResultSDO != null)
                {
                    success = true;
                    FillDataToGridControl();
                    ResetFormData();
                }
                if (success)
                {
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_MEDI_STOCK SetDataMediStock()
        {
            HIS_MEDI_STOCK mediStock = new HIS_MEDI_STOCK();
            try
            {
                if (!String.IsNullOrEmpty(txtMediStockCode.Text))
                    mediStock.MEDI_STOCK_CODE = txtMediStockCode.Text;
                if (!String.IsNullOrEmpty(txtMediStockName.Text))
                    mediStock.MEDI_STOCK_NAME = txtMediStockName.Text;
                mediStock.IS_GOODS_RESTRICT = (short)(chkIsResTrict.Checked ? 1 : 0);
                mediStock.IS_ALLOW_IMP_SUPPLIER = (short)(chkIsSuppLier.Checked ? 1 : 0);
                mediStock.IS_AUTO_APPROVE_EXP = (short)(chkIsApproveExp.Checked ? 1 : 0);
                mediStock.IS_AUTO_APPROVE_IMP = (short)(chkIsApproveImp.Checked ? 1 : 0);
                mediStock.IS_AUTO_CREATE_CHMS_IMP = (short)(chkIsCreatImp.Checked ? 1 : 0);
                mediStock.IS_AUTO_EXECUTE_EXP = (short)(chkIsExeCuteExp.Checked ? 1 : 0);
                mediStock.IS_BUSINESS = (short)(chkIsBusiness.Checked ? 1 : 0);
                mediStock.IS_CABINET = (short)(chkIsCabinet.Checked ? 1 : 0);
                if (cboParent.EditValue != null)
                    mediStock.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboParent.EditValue.ToString());

            }
            catch (Exception ex)
            {
                mediStock = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return mediStock;
        }

        private HIS_ROOM SetDataRoom()
        {
            HIS_ROOM Parent = new HIS_ROOM();
            try
            {
                //if (cboParent.EditValue != null) Parent = Inventec.Common.TypeConvert.Parse.ToInt64((cboParent.EditValue ?? "0").ToString());
                if (cboDepartMentName.EditValue != null)
                    Parent.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartMentName.EditValue.ToString());
                if (chkIsPause.Checked)
                    Parent.IS_PAUSE = 1;
                
            }
            catch (Exception ex)
            {
                Parent = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return Parent;
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                //ResetFormData();

                txtMediStockName.Text = "";
                txtMediStockCode.Text = "";
                cboParent.EditValue = null;
                cboDepartMentName.EditValue = null;
                //spMaxRequestByDay.EditValue = null;
                chkIsApproveExp.CheckState = CheckState.Unchecked;
                chkIsApproveImp.CheckState = CheckState.Unchecked;
                chkIsBusiness.CheckState = CheckState.Unchecked;
                chkIsCabinet.CheckState = CheckState.Unchecked;
                chkIsCreatImp.CheckState = CheckState.Unchecked;
                chkIsExeCuteExp.CheckState = CheckState.Unchecked;
                chkIsPause.CheckState = CheckState.Unchecked;
                chkIsResTrict.CheckState = CheckState.Unchecked;
                chkIsSuppLier.CheckState = CheckState.Unchecked;
                chkIsApproveExp.Properties.FullFocusRect = false;
                chkIsApproveImp.Properties.FullFocusRect = false;
                chkIsPause.Properties.FullFocusRect = false;
                chkIsBusiness.Properties.FullFocusRect = false;
                chkIsCabinet.Properties.FullFocusRect = false;
                chkIsCreatImp.Properties.FullFocusRect = false;
                chkIsExeCuteExp.Properties.FullFocusRect = false;
                chkIsResTrict.Properties.FullFocusRect = false;
                chkIsSuppLier.Properties.FullFocusRect = false;

                //SetFocusEditor();
                txtMediStockCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ExpMestVitaminA.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestVitaminA
{
    public partial class frmExpMestVitaminA : HIS.Desktop.Utility.FormBase
    {
        List<V_HIS_VITAMIN_A> currentListVitaminA;
        Inventec.Desktop.Common.Modules.Module currentModule;
        int positionHandle = -1;
        HIS.Desktop.Common.DelegateSelectData delegateReturnSuccess;

        public frmExpMestVitaminA(Inventec.Desktop.Common.Modules.Module module, List<V_HIS_VITAMIN_A> vitaminA, HIS.Desktop.Common.DelegateSelectData dlg)
        {
            InitializeComponent();

            try
            {
                this.currentModule = module;
                this.currentListVitaminA = vitaminA;
                this.delegateReturnSuccess = dlg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmExpMestVitaminA_Load(object sender, EventArgs e)
        {

            try
            {
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                ValidationSingleControl(cboMediStock);
                SetIcon();
                LoadComboImpMediStock();
                FillDataToGrid(this.currentListVitaminA, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboImpMediStock()
        {
            try
            {
                List<V_HIS_MEDI_STOCK> data = new List<V_HIS_MEDI_STOCK>();

                if (this.currentModule != null && this.currentModule.RoomId > 0)
                {
                    var listMestRoom = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == this.currentModule.RoomId).ToList();
                    var a = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                    data = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listMestRoom.Select(p => p.MEDI_STOCK_ID).Contains(o.ID) && o.IS_ACTIVE == 1).ToList();

                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                    ControlEditorLoader.Load(cboMediStock, data, controlEditorADO);
                    if (data != null && data.Count == 1)
                    {
                        cboMediStock.EditValue = data[0].ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private List<HisMedicineInStockSDO> GetMedicineInStock(long mediStockId)
        {
            List<HisMedicineInStockSDO> rs = null;

            try
            {
                CommonParam param = new CommonParam();
                HisMedicineStockViewFilter medicineFilter = new HisMedicineStockViewFilter();
                medicineFilter.MEDI_STOCK_ID = mediStockId;
                medicineFilter.IS_LEAF = 1;
                medicineFilter.MEDICINE_TYPE_IS_ACTIVE = true;
                medicineFilter.ORDER_DIRECTION = "ASC";
                medicineFilter.ORDER_FIELD = "MEDICINE_TYPE_NAME";

                rs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisMedicineInStockSDO>>("/api/HisMedicine/GetInStockMedicine", ApiConsumers.MosConsumer, medicineFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private void FillDataToGrid(List<V_HIS_VITAMIN_A> vitaminA, List<HisMedicineInStockSDO> medicineInStock)
        {

            try
            {
                List<ExpMestVitaminAADO> listExpMestVitaminAADO = new List<ExpMestVitaminAADO>();
                AutoMapper.Mapper.CreateMap<V_HIS_VITAMIN_A, ExpMestVitaminAADO>();
                var vitaminAAdos = AutoMapper.Mapper.Map<List<ExpMestVitaminAADO>>(vitaminA);

                var group = vitaminAAdos.GroupBy(o => o.MEDICINE_TYPE_ID).ToList();
                foreach (var itemGr in group)
                {
                    var ado = new ExpMestVitaminAADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestVitaminAADO>(ado, itemGr.FirstOrDefault());

                    ado.AMOUNT = itemGr.Sum(o => o.AMOUNT);

                    if (medicineInStock != null)
                    {
                        var medi = medicineInStock.Where(o => o.MEDICINE_TYPE_ID == itemGr.FirstOrDefault().MEDICINE_TYPE_ID);
                        if (medi != null)
                        {
                            ado.AVAILABLE = medi.Sum(o => (o.AvailableAmount ?? 0));
                            ado.TOTAL_IN_MEDI_STOCK = medi.Sum(o => (o.TotalAmount ?? 0));
                        }
                    }
                    listExpMestVitaminAADO.Add(ado);
                }

                gridControlExpMestVitaminA.BeginUpdate();
                gridControlExpMestVitaminA.DataSource = (listExpMestVitaminAADO != null && listExpMestVitaminAADO.Count > 0) ? listExpMestVitaminAADO : vitaminAAdos;
                gridControlExpMestVitaminA.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboMediStock_EditValueChanged(object sender, EventArgs e)
        {

            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboMediStock_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboMediStock.EditValue != null)
                {
                    FillDataToGrid(this.currentListVitaminA, GetMedicineInStock(Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStock.EditValue.ToString())));
                }
                else
                {
                    FillDataToGrid(this.currentListVitaminA, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestVitaminA_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {

            try
            {
                var row = (ExpMestVitaminAADO)gridViewExpMestVitaminA.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "TOTAL_IN_MEDI_STOCK")
                    {
                        if (row.TOTAL_IN_MEDI_STOCK == 0)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                    else if (e.Column.FieldName == "AVAILABLE")
                    {
                        if (row.AVAILABLE == 0)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                    else if (e.Column.FieldName == "AMOUNT")
                    {
                        if (row.AMOUNT == 0)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                CommonParam param = new CommonParam();
                bool success = false;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (!CheckValidData())
                    return;

                HisExpMestVitaminASDO sdo = new HisExpMestVitaminASDO();
                sdo.ReqRoomId = this.currentModule.RoomId;
                sdo.MediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStock.EditValue.ToString());
                sdo.VitaminAIds = currentListVitaminA.Select(o => o.ID).ToList();

                WaitingManager.Show();
                var rs = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/VitaminACreate", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                }

                if (this.delegateReturnSuccess != null)
                    this.delegateReturnSuccess(rs);

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

        private bool CheckValidData()
        {
            bool rs = true;
            try
            {
                var listExpMestVitaminAADO = (List<ExpMestVitaminAADO>)gridControlExpMestVitaminA.DataSource;
                if (listExpMestVitaminAADO != null && listExpMestVitaminAADO.Count > 0)
                {
                    var available = listExpMestVitaminAADO.Where(o => o.AVAILABLE == 0).ToList();
                    if (available != null && available.Count > 0)
                    {
                        var message = string.Format("Thuốc: {0} trong kho có khả dụng bằng 0", string.Join(",", available.Select(o => o.MEDICINE_TYPE_NAME).ToList()));
                        DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo");
                        return false;
                    }

                    var amount = listExpMestVitaminAADO.Where(o => o.AMOUNT == 0).ToList();
                    if (amount != null && amount.Count > 0)
                    {
                        var message = string.Format("Thuốc: {0} có số lượng xuất bằng 0", string.Join(",", amount.Select(o => o.MEDICINE_TYPE_NAME).ToList()));
                        DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo");
                        return false;
                    }

                    var totalInStock = listExpMestVitaminAADO.Where(o => o.TOTAL_IN_MEDI_STOCK == 0).ToList();
                    if (totalInStock != null && totalInStock.Count > 0)
                    {
                        var message = string.Format("Thuốc: {0} trong kho có tồn kho bằng 0", string.Join(",", totalInStock.Select(o => o.MEDICINE_TYPE_NAME).ToList()));
                        DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo");
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return rs;
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestVitaminA_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExpMestVitaminAADO pData = (ExpMestVitaminAADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
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

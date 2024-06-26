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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.ImpMestCreate.Form
{
    public partial class frmImpSourceReturn : HIS.Desktop.Utility.FormBase
    {
        DelegateSelectData _dlgReturn;

        long inDay = 0;
        long otherDay = 0;

        public frmImpSourceReturn()
        {
            InitializeComponent();
        }

        public frmImpSourceReturn(DelegateSelectData _dlg)
        {
            InitializeComponent();
            try
            {
                _dlgReturn = _dlg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmImpSourceReturn_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadConfig()
        {
            try
            {
                string InDay = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_IMP_MEST.SALE_RETURN_RATIO.IN_DAY");
                string OtherDay = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_IMP_MEST.SALE_RETURN_RATIO.OTHER_DAY");

                this.inDay = Inventec.Common.TypeConvert.Parse.ToInt64(InDay);
                this.otherDay = Inventec.Common.TypeConvert.Parse.ToInt64(OtherDay);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Search(txtExpMestCode.Text.Trim(), false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<ExpMestMedicineADO> _Medicines { get; set; }
        List<ExpMestMaterialADO> _Materials { get; set; }

        private void Search(string code, bool IsTransaction)
        {
            try
            {
                this._Medicines = new List<ExpMestMedicineADO>();
                this._Materials = new List<ExpMestMaterialADO>();
                CommonParam param = new CommonParam();

                MOS.Filter.HisExpMestFilter filter = new MOS.Filter.HisExpMestFilter();
                if (IsTransaction)
                {
                    MOS.Filter.HisTransactionFilter _Tfilter = new HisTransactionFilter();
                    string codeConver = String.Format("{0:000000000000}", Convert.ToInt64(txtTransactionCode.Text.Trim()));
                    txtTransactionCode.Text = codeConver;
                    _Tfilter.TRANSACTION_CODE__EXACT = codeConver;
                    _Tfilter.IS_ACTIVE = 1;
                    var dataTransactions = new BackendAdapter(param)
                                 .Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, _Tfilter, param);
                    if (dataTransactions != null && dataTransactions.Count > 0)
                    {
                        filter.BILL_IDs = dataTransactions.Select(p => p.ID).Distinct().ToList();
                    }
                }
                else if (!string.IsNullOrEmpty(code))
                {
                    string codeConver = String.Format("{0:000000000000}", Convert.ToInt64(code));
                    txtExpMestCode.Text = codeConver;
                    filter.EXP_MEST_CODE__EXACT = codeConver;
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtExpMestCode.Text))
                    {
                        string codeConver = String.Format("{0:000000000000}", Convert.ToInt64(txtExpMestCode.Text.Trim()));
                        txtExpMestCode.Text = codeConver;
                        filter.EXP_MEST_CODE__EXACT = codeConver;
                    }
                    else
                    {
                        MOS.Filter.HisTransactionFilter _Tfilter = new HisTransactionFilter();
                        string codeConver = String.Format("{0:000000000000}", Convert.ToInt64(txtTransactionCode.Text.Trim()));
                        txtTransactionCode.Text = codeConver;
                        _Tfilter.TRANSACTION_CODE__EXACT = codeConver;
                        _Tfilter.IS_ACTIVE = 1;
                        var dataTransactions = new BackendAdapter(param)
                                     .Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, _Tfilter, param);
                        if (dataTransactions != null && dataTransactions.Count > 0)
                        {
                            filter.BILL_IDs = dataTransactions.Select(p => p.ID).Distinct().ToList();
                        }
                    }
                }
                filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                var dataExpMests = new BackendAdapter(param)
                             .Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, param);

                if (dataExpMests != null && dataExpMests.Count > 0)
                {
                    string IntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataExpMests[0].FINISH_DATE ?? 0);
                    string day = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(DateTime.Now);
                    if (IntructionTime.Trim() == day.Trim())
                        spinTyLeTraLai.EditValue = this.inDay;
                    else
                        spinTyLeTraLai.EditValue = this.otherDay;

                    List<long> _expMestIds = dataExpMests.Select(p => p.ID).ToList();

                    List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = null;
                    List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = null;
                    List<Action> methods = new List<Action>();
                    methods.Add(() => { expMestMedicines = GetExpMestMedicineByExpMests(_expMestIds); });
                    methods.Add(() => { expMestMaterials = GetExpMestMaterialByExpMests(_expMestIds); });
                    ThreadCustomManager.MultipleThreadWithJoin(methods);

                    if (expMestMaterials != null && expMestMaterials.Count > 0)
                    {
                        xtraTabControl.SelectedTabPageIndex = 1;

                        var dataGroups = expMestMaterials.GroupBy(p => p.ID).Select(p => p.ToList()).ToList();
                        foreach (var item in dataGroups)
                        {
                            ExpMestMaterialADO ado = new ExpMestMaterialADO(item[0]);
                            ado.AMOUNT = item.Sum(p => p.AMOUNT);
                            _Materials.Add(ado);
                        }
                    }

                    if (expMestMedicines != null && expMestMedicines.Count > 0)
                    {
                        xtraTabControl.SelectedTabPageIndex = 0;

                        var dataGroups = expMestMedicines.GroupBy(p => p.ID).Select(p => p.ToList()).ToList();
                        foreach (var item in dataGroups)
                        {
                            ExpMestMedicineADO ado = new ExpMestMedicineADO(item[0]);
                            ado.AMOUNT = item.Sum(p => p.AMOUNT);
                            _Medicines.Add(ado);
                        }

                    }

                }

                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = _Medicines;
                gridControlMedicine.EndUpdate();

                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = _Materials;
                gridControlMaterial.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_EXP_MEST_MEDICINE> GetExpMestMedicineByExpMests(List<long> _expMestIds)
        {
            List<V_HIS_EXP_MEST_MEDICINE> result = null;
            try
            {
                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_IDs = _expMestIds;
                result = new BackendAdapter(new CommonParam())
                  .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, null);

            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<V_HIS_EXP_MEST_MATERIAL> GetExpMestMaterialByExpMests(List<long> _expMestIds)
        {
            List<V_HIS_EXP_MEST_MATERIAL> result = null;
            try
            {
                HisExpMestMaterialViewFilter medicineFilter = new HisExpMestMaterialViewFilter();
                medicineFilter.EXP_MEST_IDs = _expMestIds;
                medicineFilter.IS_ACTIVE = 1;
                result = new BackendAdapter(new CommonParam())
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, medicineFilter, null);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewMedicine.PostEditor();
                gridViewMaterial.PostEditor();

                btnSave.Focus();
                List<object> listArgs = new List<object>();
                if (this._Materials != null && _Materials.Count > 0)
                {
                    var listSelect = _Materials.Where(o => o.IsMoba).ToList();
                    if (listSelect != null && listSelect.Count > 0)
                    {
                        var dataError = listSelect.FirstOrDefault(p => p.IsError);
                        if (dataError != null)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng trả không hợp lệ", "Thông báo");
                            return;
                        }
                        foreach (var item in listSelect)
                        {
                            item.PRICE = item.PRICE * spinTyLeTraLai.Value / 100;
                        }
                        listArgs.Add(listSelect);
                    }
                }
                if (this._Medicines != null && _Medicines.Count > 0)
                {
                    var listSelect = _Medicines.Where(o => o.IsMoba).ToList();
                    if (listSelect != null && listSelect.Count > 0)
                    {
                        var dataError = listSelect.FirstOrDefault(p => p.IsError);
                        if (dataError != null)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng trả không hợp lệ", "Thông báo");
                            return;
                        }
                        foreach (var item in listSelect)
                        {
                            item.PRICE = item.PRICE * spinTyLeTraLai.Value / 100;
                        }
                        listArgs.Add(listSelect);
                    }
                }
                if (this._dlgReturn != null)
                {
                    this._dlgReturn(listArgs);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (gridViewMedicine.FocusedRowHandle < 0 || gridViewMedicine.FocusedColumn.FieldName != "YCT_AMOUNT")
                    return;
                var data = (ExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[gridViewMedicine.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.YCT_AMOUNT <= 0)
                    {
                        valid = false;
                        message = "Số lượng trả phải lớn hơn 0";
                    }
                    else if (data.YCT_AMOUNT > (data.AMOUNT - (data.TH_AMOUNT ?? 0)))
                    {
                        valid = false;
                        message = String.Format("Số lượng trả lớn hơn số lượng xuất", data.YCT_AMOUNT, (data.AMOUNT - (data.TH_AMOUNT ?? 0)));
                    }
                    //else if (!data.IS_ALLOW_EXPORT_ODD)
                    //{
                    //    decimal x = Math.Abs(Math.Round(data.YCT_AMOUNT, 3) - Math.Floor(data.YCT_AMOUNT));
                    //    if (x > 0)
                    //    {
                    //        valid = false;
                    //        message = "Không trả lẻ";
                    //    }
                    //}
                    if (!valid)
                    {
                        data.IsError = true;
                        gridViewMedicine.SetColumnError(gridViewMedicine.FocusedColumn, message);
                    }
                    else
                    {
                        data.IsError = false;
                        gridViewMedicine.ClearColumnErrors();
                        CalculTotalPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (gridViewMaterial.FocusedRowHandle < 0 || gridViewMaterial.FocusedColumn.FieldName != "YCT_AMOUNT")
                    return;
                var data = (ExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[gridViewMaterial.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.YCT_AMOUNT <= 0)
                    {
                        valid = false;
                        message = "Số lượng trả phải lớn hơn 0";
                    }
                    else if (data.YCT_AMOUNT > (data.AMOUNT - (data.TH_AMOUNT ?? 0)))
                    {
                        valid = false;
                        message = String.Format("Số lượng trả lớn hơn số lượng xuất", data.YCT_AMOUNT, (data.AMOUNT - (data.TH_AMOUNT ?? 0)));
                    }
                    //else if (!data.IS_ALLOW_EXPORT_ODD)
                    //{
                    //    decimal x = Math.Abs(Math.Round(data.YCT_AMOUNT, 3) - Math.Floor(data.YCT_AMOUNT));
                    //    if (x > 0)
                    //    {
                    //        valid = false;
                    //        message = "Không trả lẻ";
                    //    }
                    //}
                    if (!valid)
                    {
                        data.IsError = true;
                        gridViewMaterial.SetColumnError(gridViewMaterial.FocusedColumn, message);
                    }
                    else
                    {
                        data.IsError = false;
                        gridViewMaterial.ClearColumnErrors();
                        CalculTotalPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //if (gridViewMedicine.RowCount > 0)
            //{
            //    for (int i = 0; i < gridViewMedicine.SelectedRowsCount; i++)
            //    {
            //        if (gridViewMedicine.GetSelectedRows()[i] >= 0)
            //        {
            //            _ExpMestChecks.Add((V_HIS_EXP_MEST)gridViewExpMestReq.GetRow(gridViewExpMestReq.GetSelectedRows()[i]));
            //        }
            //    }
            //}

            try
            {
                for (int i = 0; i < gridViewMedicine.DataRowCount; i++)
                {
                    var data = (ExpMestMedicineADO)gridViewMedicine.GetRow(i);
                    if (data != null)
                    {
                        if (gridViewMedicine.IsRowSelected(i))
                        {
                            data.IsMoba = true;
                            data.YCT_AMOUNT = data.AMOUNT;
                        }
                        else
                        {
                            data.IsMoba = false;
                            data.YCT_AMOUNT = 0;
                        }
                    }
                    CalculTotalPrice();
                }
                gridControlMedicine.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void CalculTotalPrice()
        {
            try
            {
                decimal totalPrice = 0;
                decimal totalFeePrice = 0;
                decimal totalVatPrice = 0;
                if (this._Materials != null && _Materials.Count > 0)
                {
                    var listSelect = _Materials.Where(o => o.IsMoba).ToList();
                    if (listSelect != null && listSelect.Count > 0)
                    {
                        totalFeePrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.YCT_AMOUNT));
                        totalVatPrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.YCT_AMOUNT * (s.IMP_VAT_RATIO)));
                    }
                }
                if (this._Medicines != null && _Medicines.Count > 0)
                {
                    var listSelect = _Medicines.Where(o => o.IsMoba).ToList();
                    if (listSelect != null && listSelect.Count > 0)
                    {
                        totalFeePrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.YCT_AMOUNT));
                        totalVatPrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.YCT_AMOUNT * (s.IMP_VAT_RATIO)));
                    }
                }
                totalVatPrice = Math.Round(totalVatPrice, 4);
                totalPrice = totalFeePrice + totalVatPrice;
                //lblTongTien.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalFeePrice, 4);
                lblTongTien.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 4);
                //lblTongTien.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalVatPrice, 4);

                lblTienTraLai.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice * spinTyLeTraLai.Value / 100, 4);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                for (int i = 0; i < gridViewMaterial.DataRowCount; i++)
                {
                    var data = (ExpMestMaterialADO)gridViewMaterial.GetRow(i);
                    if (data != null)
                    {
                        if (gridViewMaterial.IsRowSelected(i))
                        {
                            data.IsMoba = true;
                            data.YCT_AMOUNT = data.AMOUNT;
                        }
                        else
                        {
                            data.IsMoba = false;
                            data.YCT_AMOUNT = 0;
                        }
                    }
                    CalculTotalPrice();
                }
                gridControlMaterial.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                var data = (ExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data != null)
                {
                    if (e.Column.FieldName == "PRICE_STR")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(data.PRICE ?? 0, ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                var data = (ExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data != null)
                {
                    if (e.Column.FieldName == "PRICE_STR")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(data.IMP_PRICE, ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTyLeTraLai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalculTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                Search("", false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTransactionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Search("", true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtExpMestCode.Focus();
                txtExpMestCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtTransactionCode.Focus();
                txtTransactionCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ImpMestChmsCreate.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
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
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestChmsCreate
{
    public partial class frmImpMestChmsCreate : HIS.Desktop.Utility.FormBase
    {
        long checkPrint;
        long expMestId;
        long expMestTypeId;
        PrintType printType;
        V_HIS_EXP_MEST hisExpMest = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        List<VHisExpMestMedicineADO> listExpMestMedicineADO = new List<VHisExpMestMedicineADO>();
        List<VHisExpMestMaterialADO> listExpMestMaterialADO = new List<VHisExpMestMaterialADO>();
        List<V_HIS_EXP_MEST_BLOOD> listExpMestBlood = new List<V_HIS_EXP_MEST_BLOOD>();

        HisImpMestResultSDO resultMobaSdo = null;
        HIS.Desktop.Common.DelegateRefreshData refreshData;
        V_HIS_MEDI_STOCK medistock = null;

        public frmImpMestChmsCreate(Inventec.Desktop.Common.Modules.Module module, long data, long expMestTypeId, HIS.Desktop.Common.DelegateRefreshData refresh)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.SetIcon();
                this.expMestId = data;
                this.currentModule = module;
                this.expMestTypeId = expMestTypeId;
                this.refreshData = refresh;
                if (this.currentModule != null)
                {
                    if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                    {
                        this.Text = "Tạo phiếu nhập chuyển kho";
                    }
                    else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                    {
                        this.Text = "Tạo phiếu nhập bù cơ số";
                    }
                    else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL)
                    {
                        this.Text = "Tạo phiếu nhập bù lẻ";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void frmImpMestChmsCreate_Load(object sender, EventArgs e)
        {
            try
            {
                this.medistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                LoadExpMest();
                LoadExpMestMedicine();
                LoadExpMestMaterial();
                LoadExpMestBlood();
                btnSave.Enabled = true;
                btnPrint.Enabled = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMest()
        {
            try
            {
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this.expMestId;
                var hisExpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expMestFilter, null);
                if (hisExpMests != null && hisExpMests.Count == 1)
                {
                    hisExpMest = hisExpMests.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMedicine()
        {
            try
            {
                if (this.hisExpMest != null)
                {
                    CommonParam param = new CommonParam();
                    HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter()
                    {
                        EXP_MEST_ID = this.hisExpMest.ID,
                    };
                    var listExpMestMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, param);
                    if (listExpMestMedicine != null && listExpMestMedicine.Count > 0)
                    {
                        var group = listExpMestMedicine.GroupBy(o => new { o.MEDICINE_ID, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.IS_OUT_PARENT_FEE }).ToList();
                        foreach (var item in group)
                        {
                            VHisExpMestMedicineADO medi = new VHisExpMestMedicineADO(item.First());
                            medi.AMOUNT = item.Sum(s => s.AMOUNT);
                            medi.CAN_MOBA_AMOUNT = item.Sum(s => s.AMOUNT) - item.Sum(p => p.TH_AMOUNT ?? 0);
                            listExpMestMedicineADO.Add(medi);
                        }
                    }
                    else
                    {
                        listExpMestMedicineADO = new List<VHisExpMestMedicineADO>();
                    }
                    gridControlExpMestMedicine.BeginUpdate();
                    gridControlExpMestMedicine.DataSource = listExpMestMedicineADO;
                    gridControlExpMestMedicine.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMaterial()
        {
            try
            {
                if (this.hisExpMest != null)
                {
                    HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter()
                    {
                        EXP_MEST_ID = this.hisExpMest.ID,
                    };
                    var listExpMestMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter, null);
                    if (listExpMestMaterial != null && listExpMestMaterial.Count > 0)
                    {
                        var group = listExpMestMaterial.GroupBy(o => new { o.MATERIAL_ID, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.IS_OUT_PARENT_FEE }).ToList();
                        foreach (var item in group)
                        {
                            VHisExpMestMaterialADO mate = new VHisExpMestMaterialADO(item.First());
                            mate.AMOUNT = item.Sum(s => s.AMOUNT);
                            mate.CAN_MOBA_AMOUNT = item.Sum(s => s.AMOUNT) - item.Sum(p => p.TH_AMOUNT ?? 0);
                            listExpMestMaterialADO.Add(mate);
                        }
                    }
                    else
                    {
                        listExpMestMaterialADO = new List<VHisExpMestMaterialADO>();
                    }
                    gridControlExpMestMaterial.BeginUpdate();
                    gridControlExpMestMaterial.DataSource = listExpMestMaterialADO;
                    gridControlExpMestMaterial.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestBlood()
        {
            try
            {
                if (this.hisExpMest != null)
                {
                    HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter()
                    {
                        EXP_MEST_ID = this.hisExpMest.ID,
                    };
                    listExpMestBlood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, null);

                    gridControlExpMestBlood.BeginUpdate();
                    gridControlExpMestBlood.DataSource = listExpMestBlood;
                    gridControlExpMestBlood.EndUpdate();
                }
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
                if (listExpMestMaterialADO != null && listExpMestMaterialADO.Count > 0)
                {
                    var listSelect = listExpMestMaterialADO.Where(o => o.IsMoba).ToList();
                    if (listSelect != null && listSelect.Count > 0)
                    {
                        totalFeePrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT));
                        totalVatPrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT * (s.VAT_RATIO ?? 0)));
                    }
                }
                if (listExpMestMedicineADO != null && listExpMestMedicineADO.Count > 0)
                {
                    var listSelect = listExpMestMedicineADO.Where(o => o.IsMoba).ToList();
                    if (listSelect != null && listSelect.Count > 0)
                    {
                        totalFeePrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT));
                        totalVatPrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT * (s.VAT_RATIO ?? 0)));
                    }
                }
                totalVatPrice = Math.Round(totalVatPrice, 0);
                totalPrice = totalFeePrice + totalVatPrice;
                //lblTotalFeePrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalFeePrice, 0);
                //lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0);
                //lblTotalVatPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalVatPrice, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "VAT_RATIO_PLUS")
                        {
                            try
                            {
                                e.Value = (data.VAT_RATIO ?? 0) * 100;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                    var data = (VHisExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                        {
                            e.RepositoryItem = repositoryItemSpinMediMobaAmountDisable;
                        }
                        else if (data.IsMoba)
                        {
                            e.RepositoryItem = repositoryItemMediSpinMobaAmount;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemSpinMediMobaAmountDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
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

        private void gridViewExpMestMedicine_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
                //GridView view = sender as GridView;
                //if (e.RowHandle < 0)
                //    return;
                //var data = (VHisExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                //if (data == null)
                //    return;
                //if (data.IsMoba)
                //{
                //    if (data.MOBA_AMOUNT > data.CAN_MOBA_AMOUNT)
                //    {
                //        e.Valid = false;
                //        view.SetColumnError(view.Columns["MOBA_AMOUNT"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_CONTROL__INVALID_ROW__MOBA_AMOUNT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                //    }
                //    else if (data.MOBA_AMOUNT <= 0)
                //    {
                //        e.Valid = false;
                //        view.SetColumnError(view.Columns["MOBA_AMOUNT"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_CONTROL__INVALID_ROW__GREATE_THAN_ZERO", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                //    }

                //    if (!e.Valid)
                //    {
                //        gridViewExpMestMedicine.FocusedColumn = view.Columns["MOBA_AMOUNT"];
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                for (int i = 0; i < gridViewExpMestMedicine.DataRowCount; i++)
                {
                    var data = (VHisExpMestMedicineADO)gridViewExpMestMedicine.GetRow(i);
                    if (data != null)
                    {
                        if (gridViewExpMestMedicine.IsRowSelected(i))
                        {
                            data.IsMoba = true;
                            data.MOBA_AMOUNT = (data.MOBA_AMOUNT > 0) ? data.MOBA_AMOUNT : data.CAN_MOBA_AMOUNT;
                        }
                        else
                        {
                            data.IsMoba = false;
                            data.MOBA_AMOUNT = 0;
                        }
                    }
                    CalculTotalPrice();
                }
                gridControlExpMestMedicine.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                    CalculTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "VAT_RATIO_PLUS")
                        {
                            try
                            {
                                e.Value = (data.VAT_RATIO ?? 0) * 100;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                //if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                //{
                //    var data = (VHisExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                //    if (data != null)
                //    {
                //        if (data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                //        {
                //            e.RepositoryItem = repositoryItemSpinMateMobaAmountDisable;
                //        }
                //        else if (data.IsMoba)
                //        {
                //            e.RepositoryItem = repositoryItemSpinMateMobaAmount;
                //        }
                //        else
                //        {
                //            e.RepositoryItem = repositoryItemSpinMateMobaAmountDisable;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
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

        private void gridViewExpMestMaterial_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
                //GridView view = sender as GridView;
                //if (e.RowHandle < 0)
                //    return;
                //var data = (VHisExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                //if (data == null)
                //    return;
                //if (data.IsMoba)
                //{
                //    if (data.MOBA_AMOUNT > data.CAN_MOBA_AMOUNT)
                //    {
                //        e.Valid = false;
                //        view.SetColumnError(view.Columns["MOBA_AMOUNT"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_CONTROL__INVALID_ROW__MOBA_AMOUNT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                //    }
                //    else if (data.MOBA_AMOUNT <= 0)
                //    {
                //        e.Valid = false;
                //        view.SetColumnError(view.Columns["MOBA_AMOUNT"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_CONTROL__INVALID_ROW__GREATE_THAN_ZERO", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                //    }
                //    if (!e.Valid)
                //    {
                //        gridViewExpMestMaterial.FocusedColumn = view.Columns["MOBA_AMOUNT"];
                //    }
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                for (int i = 0; i < gridViewExpMestMaterial.DataRowCount; i++)
                {
                    var data = (VHisExpMestMaterialADO)gridViewExpMestMaterial.GetRow(i);
                    if (data != null)
                    {
                        if (gridViewExpMestMaterial.IsRowSelected(i))
                        {
                            data.IsMoba = true;
                            data.MOBA_AMOUNT = (data.MOBA_AMOUNT > 0) ? data.MOBA_AMOUNT : data.CAN_MOBA_AMOUNT;
                        }
                        else
                        {
                            data.IsMoba = false;
                            data.MOBA_AMOUNT = 0;
                        }
                    }
                    CalculTotalPrice();
                }
                gridControlExpMestMaterial.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                    CalculTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled || this.expMestId <= 0 || this.hisExpMest == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool sucsess = false;
                CreateMobaImpMest(param, ref sucsess);
                if (sucsess)
                {
                    listExpMestMaterialADO = new List<VHisExpMestMaterialADO>();
                    listExpMestMedicineADO = new List<VHisExpMestMedicineADO>();

                    this.LoadExpMestMaterial();
                    this.LoadExpMestMedicine();
                    this.LoadExpMestBlood();
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                    this.refreshData();
                }
                WaitingManager.Hide();

                MessageManager.Show(this, param, sucsess);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateMobaImpMest(CommonParam param, ref bool sucsess)
        {
            try
            {
                HIS_IMP_MEST data = new HIS_IMP_MEST();

                data.CHMS_EXP_MEST_ID = this.expMestId;
                if (this.expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                    data.IMP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK;
                else if (this.expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                    data.IMP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCS;
                else if (this.expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL)
                    data.IMP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BL;

                data.REQ_ROOM_ID = this.currentModule.RoomId;
                if (medistock != null)
                {
                    data.MEDI_STOCK_ID = medistock.ID;
                    //data.MEDI_STOCK_ID = hisExpMest.IMP_MEDI_STOCK_ID ?? 0;
                }
                data.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestResultSDO>("api/HisImpMest/ChmsCreate", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    sucsess = true;
                    resultMobaSdo = rs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                sucsess = false;
            }
        }

        private enum PrintType
        {
            ThuocVatTu,
            Mau
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultMobaSdo == null)
                    return;
                OnClickPrint();
                //PopupMenu menu = new PopupMenu(barManager1);
                //menu.ItemLinks.Clear();

                //BarButtonItem itemThuoc = new BarButtonItem();
                //itemThuoc.Caption = "In phiếu nhập chuyển kho thuốc vật tư";
                //itemThuoc.ItemClick += OnClickPrint;
                //itemThuoc.Tag = PrintType.ThuocVatTu;

                //BarButtonItem itemMau = new BarButtonItem();
                //itemMau.Caption = "In phiếu nhập chuyển kho máu";
                //itemMau.ItemClick += OnClickPrint;
                //itemMau.Tag = PrintType.Mau;

                //menu.AddItems(new BarItem[] { itemThuoc });
                //menu.AddItems(new BarItem[] { itemMau });

                //menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

        private void OnClickPrint()
        {
            try
            {
                RunPrint143();
                RunPrint226();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RunPrint143()
        {
            try
            {
                store.RunPrintTemplate("Mps000143", delegateRunPrint143);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RunPrint226()
        {
            try
            {
                store.RunPrintTemplate("Mps000226", delegateRunPrint224);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewExpMestMaterial.PostEditor();
                gridViewExpMestMedicine.PostEditor();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunPrint143(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultMobaSdo == null)
                    return result;

                if ((this.resultMobaSdo.ImpMaterials != null && this.resultMobaSdo.ImpMaterials.Count > 0) || (this.resultMobaSdo.ImpMedicines != null && this.resultMobaSdo.ImpMedicines.Count > 0))
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();

                    MOS.Filter.HisExpMestViewFilter hisExpMestViewFilter = new MOS.Filter.HisExpMestViewFilter();
                    hisExpMestViewFilter.ID = this.expMestId;
                    var expMestView = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, hisExpMestViewFilter, null);

                    MPS.Processor.Mps000143.PDO.Mps000143PDO.Mps000143Key mps000143Key = new MPS.Processor.Mps000143.PDO.Mps000143PDO.Mps000143Key();

                    if (expMestView != null && expMestView.Count > 0)
                    {
                        mps000143Key.EXP_MEDI_STOCK_CODE = expMestView.FirstOrDefault().MEDI_STOCK_CODE;
                        mps000143Key.EXP_MEDI_STOCK_NAME = expMestView.FirstOrDefault().MEDI_STOCK_NAME;
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);
                    WaitingManager.Hide();

                    long keyPrintType = ConfigApplicationWorker.Get<long>(Base.AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__CHE_DO_IN_GOP_PHIEU_TRA);

                    if (keyPrintType == 1)
                    {
                        mps000143Key.KEY_NAME_TITLES = "";
                        MPS.Processor.Mps000143.PDO.Mps000143PDO rdo = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.resultMobaSdo.ImpMest, this.resultMobaSdo.ImpMedicines, this.resultMobaSdo.ImpMaterials, mps000143Key, ConfigApplications.NumberSeperator);
                        PrintData(printTypeCode, fileName, rdo, false, inputADO, ref result);
                    }
                    else
                    {
                        var _ImpMestMedi_Ts = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var _ImpMestMedi_GNs = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var _ImpMestMedi_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var _ImpMestMedi_TDs = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var _ImpMestMedi_PXs = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var _ImpMestMedi_Others = new List<V_HIS_IMP_MEST_MEDICINE>();

                        if (this.resultMobaSdo.ImpMedicines != null && this.resultMobaSdo.ImpMedicines.Count > 0)
                        {
                            var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                            var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                            bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                            bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                            bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                            bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);

                            _ImpMestMedi_Ts = this.resultMobaSdo.ImpMedicines.Where(p => !mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0)).ToList();
                            _ImpMestMedi_GNs = this.resultMobaSdo.ImpMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn).ToList();
                            _ImpMestMedi_HTs = this.resultMobaSdo.ImpMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht).ToList();
                            _ImpMestMedi_TDs = this.resultMobaSdo.ImpMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc).ToList();
                            _ImpMestMedi_PXs = this.resultMobaSdo.ImpMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px).ToList();

                            _ImpMestMedi_Others = this.resultMobaSdo.ImpMedicines.Where(p => mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0) &&
                                p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                            && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                            && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                            && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX).ToList();
                        }

                        #region thuoc thuong
                        if (_ImpMestMedi_Ts != null && _ImpMestMedi_Ts.Count > 0)
                        {
                            mps000143Key.KEY_NAME_TITLES = "THUỐC THƯỜNG";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_Ts = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.resultMobaSdo.ImpMest, _ImpMestMedi_Ts, null, mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo_Ts, false, inputADO, ref result);
                        }
                        #endregion

                        #region Gay nghien, huong than
                        if ((_ImpMestMedi_GNs != null && _ImpMestMedi_GNs.Count > 0) || (_ImpMestMedi_HTs != null && _ImpMestMedi_HTs.Count > 0))
                        {
                            long keyPrintTypeHTGN = ConfigApplicationWorker.Get<long>(Base.AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                            if (keyPrintTypeHTGN == 1)
                            {
                                List<V_HIS_IMP_MEST_MEDICINE> DataGroups = new List<V_HIS_IMP_MEST_MEDICINE>();

                                if (_ImpMestMedi_GNs != null && _ImpMestMedi_GNs.Count > 0)
                                {
                                    DataGroups.AddRange(_ImpMestMedi_GNs);
                                }

                                if (_ImpMestMedi_HTs != null && _ImpMestMedi_HTs.Count > 0)
                                {
                                    DataGroups.AddRange(_ImpMestMedi_HTs);
                                }

                                mps000143Key.KEY_NAME_TITLES = "GÂY NGHIỆN, HƯỚNG THẦN";
                                MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_GNHTs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.resultMobaSdo.ImpMest, DataGroups, null, mps000143Key, ConfigApplications.NumberSeperator);
                                PrintData(printTypeCode, fileName, rdo_GNHTs, false, inputADO, ref result);
                            }
                            else
                            {
                                if (_ImpMestMedi_GNs != null && _ImpMestMedi_GNs.Count > 0)
                                {
                                    mps000143Key.KEY_NAME_TITLES = "GÂY NGHIỆN";
                                    MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_GNs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.resultMobaSdo.ImpMest, _ImpMestMedi_GNs, null, mps000143Key, ConfigApplications.NumberSeperator);
                                    PrintData(printTypeCode, fileName, rdo_GNs, false, inputADO, ref result);
                                }

                                if (_ImpMestMedi_HTs != null && _ImpMestMedi_HTs.Count > 0)
                                {
                                    mps000143Key.KEY_NAME_TITLES = "HƯỚNG THẦN";
                                    MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_HTs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.resultMobaSdo.ImpMest, _ImpMestMedi_HTs, null, mps000143Key, ConfigApplications.NumberSeperator);
                                    PrintData(printTypeCode, fileName, rdo_HTs, false, inputADO, ref result);
                                }
                            }
                        }
                        #endregion

                        #region thuoc doc
                        if (_ImpMestMedi_TDs != null && _ImpMestMedi_TDs.Count > 0)
                        {
                            mps000143Key.KEY_NAME_TITLES = "ĐỘC";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_TDs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.resultMobaSdo.ImpMest, _ImpMestMedi_TDs, null, mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo_TDs, false, inputADO, ref result);
                        }
                        #endregion

                        #region thuoc phong xa
                        if (_ImpMestMedi_PXs != null && _ImpMestMedi_PXs.Count > 0)
                        {
                            mps000143Key.KEY_NAME_TITLES = "PHÓNG XẠ";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_PXs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.resultMobaSdo.ImpMest, _ImpMestMedi_PXs, null, mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo_PXs, false, inputADO, ref result);
                        }
                        #endregion

                        #region thuoc khac
                        if (_ImpMestMedi_Others != null && _ImpMestMedi_Others.Count > 0)
                        {
                            mps000143Key.KEY_NAME_TITLES = "KHÁC";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_Ks = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.resultMobaSdo.ImpMest, _ImpMestMedi_Others, null, mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo_Ks, false, inputADO, ref result);
                        }
                        #endregion

                        #region vat tu
                        if (this.resultMobaSdo.ImpMaterials != null && this.resultMobaSdo.ImpMaterials.Count > 0)
                        {
                            mps000143Key.KEY_NAME_TITLES = "VẬT TƯ";
                            MPS.Processor.Mps000143.PDO.Mps000143PDO rdo_VTs = new MPS.Processor.Mps000143.PDO.Mps000143PDO(this.resultMobaSdo.ImpMest, null, this.resultMobaSdo.ImpMaterials, mps000143Key, ConfigApplications.NumberSeperator);
                            PrintData(printTypeCode, fileName, rdo_VTs, false, inputADO, ref result);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void PrintData(string printTypeCode, string fileName, object data, bool printNow, Inventec.Common.SignLibrary.ADO.InputADO inputADO, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (printNow)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunPrint224(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultMobaSdo == null)
                    return result;
                if (this.resultMobaSdo.ImpBloods != null && this.resultMobaSdo.ImpBloods.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();

                    MOS.Filter.HisExpMestViewFilter hisExpMestViewFilter = new MOS.Filter.HisExpMestViewFilter();
                    hisExpMestViewFilter.ID = this.expMestId;
                    var expMestView = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, hisExpMestViewFilter, null);

                    MPS.Processor.Mps000226.PDO.Mps000226PDO.Mps000226Key mps000226Key = new MPS.Processor.Mps000226.PDO.Mps000226PDO.Mps000226Key();

                    if (expMestView != null && expMestView.Count > 0)
                    {
                        mps000226Key.EXP_MEDI_STOCK_CODE = expMestView.FirstOrDefault().MEDI_STOCK_CODE;
                        mps000226Key.EXP_MEDI_STOCK_NAME = expMestView.FirstOrDefault().MEDI_STOCK_NAME;
                    }
                    //MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialViewFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                    //expMestMaterialViewFilter.EXP_MEST_ID = this.resultMobaSdo.MobaImpMest.EXP_MEST_ID;
                    //var expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialViewFilter, null);
                    //V_HIS_SALE_EXP_MEST saleExpMest = null;
                    //if (this.hisExpMest.EXP_MEST_TYPE_ID == HisExpMestTypeCFG.HisExpMestTypeId__Sale)
                    //{
                    //    HisSaleExpMestViewFilter saleExpFilter = new HisSaleExpMestViewFilter();
                    //    saleExpFilter.EXP_MEST_ID = this.hisExpMest.ID;
                    //    var listSale = new BackendAdapter(param).Get<List<V_HIS_SALE_EXP_MEST>>("api/HisSaleExpMest/GetView", ApiConsumers.MosConsumer, saleExpFilter, null);
                    //    if (listSale != null && listSale.Count == 1)
                    //    {
                    //        saleExpMest = listSale.First();
                    //    }
                    //    else
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Info("Khong lay duoc Sale_Exp_Mest theo expMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisExpMest), hisExpMest));
                    //    }
                    //}
                    WaitingManager.Hide();
                    MPS.Processor.Mps000226.PDO.Mps000226PDO rdo = null;

                    rdo = new MPS.Processor.Mps000226.PDO.Mps000226PDO(this.resultMobaSdo.ImpMest, this.resultMobaSdo.ImpBloods, mps000226Key, ConfigApplications.NumberSeperator);

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
                    if (result)
                    {
                        //this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ImpMestChmsCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.ImpMestChmsCreate.frmImpMestChmsCreate).Assembly);

                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.xtraTabPageMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_Stt.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_MedicineTypName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_MobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_Amount.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_CanMobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_CanMobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_ImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VatRatio.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_BidNumber.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_PackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_RegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMedicine_ExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.xtraTabPageMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_Stt.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_MaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_MaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_MobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_Amount.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_CanMobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_CanMobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_ImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_VatRatio.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_VirTotalImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_BidNumber.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_PackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_RegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn_ExpMestMaterial_ExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.bbtnRCSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCPrint.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.bbtnRCPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageBlood.Text = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.xtraTabPageBlood.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.ToolTip = Inventec.Common.Resource.Get.Value("frmImpMestChmsCreate.gridColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModuleBase != null)
                {
                    this.Text = this.currentModuleBase.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlood_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_EXP_MEST_BLOOD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
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

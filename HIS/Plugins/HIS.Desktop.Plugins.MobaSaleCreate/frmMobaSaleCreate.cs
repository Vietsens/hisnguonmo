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
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MobaSaleCreate.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.MobaSaleCreate
{
    public partial class frmMobaSaleCreate : HIS.Desktop.Utility.FormBase
    {
        long expMestId;
        V_HIS_EXP_MEST hisExpMest = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        bool Is_PrintHD = false;

        List<VHisExpMestMedicineADO> listExpMestMedicineADO = new List<VHisExpMestMedicineADO>();
        List<VHisExpMestMaterialADO> listExpMestMaterialADO = new List<VHisExpMestMaterialADO>();

        HisImpMestResultSDO resultMobaSdo = null;

        public frmMobaSaleCreate(Inventec.Desktop.Common.Modules.Module module, long data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.expMestId = data;
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
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

        private void frmMobaSaleCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                LoadExpMest();
                LoadExpMestMedicine();
                LoadExpMestMaterial();
                btnSave.Enabled = true;
                btnPrint.Enabled = false;
                BtnPrintExNTr.Enabled = false;
                if (listExpMestMedicineADO != null && listExpMestMedicineADO.Count > 0)
                {
                    xtraTabControlMain.SelectedTabPage = xtraTabPageMedicine;
                }
                else if (listExpMestMaterialADO != null && listExpMestMaterialADO.Count > 0)
                {
                    xtraTabControlMain.SelectedTabPage = xtraTabPageMaterial;
                }
                else
                {
                    xtraTabControlMain.SelectedTabPage = xtraTabPageMedicine;
                }
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
                    listExpMestMedicineADO = new List<VHisExpMestMedicineADO>();
                    var listExpMestMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, param);
                    if (listExpMestMedicine != null && listExpMestMedicine.Count > 0)
                    {
                        HisMedicineFilter mediFilter = new HisMedicineFilter();
                        mediFilter.IDs = listExpMestMedicine.Select(p => p.MEDICINE_ID ?? 0).ToList();
                        var medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, mediFilter, param);
                        var group = listExpMestMedicine.GroupBy(o => new { o.MEDICINE_ID }).ToList();
                        foreach (var item in group)
                        {
                            VHisExpMestMedicineADO medi = new VHisExpMestMedicineADO(item.First());
                            medi.AMOUNT = item.Sum(s => s.AMOUNT);
                            medi.CAN_MOBA_AMOUNT = item.Sum(s => s.AMOUNT) - item.Sum(p => p.TH_AMOUNT ?? 0);
                            medi.TDL_BID_PACKAGE_CODE = medicines.FirstOrDefault(o => o.ID == item.First().MEDICINE_ID).TDL_BID_PACKAGE_CODE;
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
                    listExpMestMaterialADO = new List<VHisExpMestMaterialADO>();
                    var listExpMestMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter, null);
                    if (listExpMestMaterial != null && listExpMestMaterial.Count > 0)
                    {
                        HisMaterialFilter mateFilter = new HisMaterialFilter();
                        mateFilter.IDs = listExpMestMaterial.Select(p => p.MATERIAL_ID ?? 0).ToList();
                        var materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, mateFilter, new CommonParam());
                        var group = listExpMestMaterial.GroupBy(o => new { o.MATERIAL_ID }).ToList();
                        foreach (var item in group)
                        {
                            VHisExpMestMaterialADO mate = new VHisExpMestMaterialADO(item.First());
                            mate.AMOUNT = item.Sum(s => s.AMOUNT);
                            mate.CAN_MOBA_AMOUNT = item.Sum(s => s.AMOUNT) - item.Sum(p => p.TH_AMOUNT ?? 0);
                            mate.TDL_BID_PACKAGE_CODE = materials.FirstOrDefault(o => o.ID == item.First().MATERIAL_ID).TDL_BID_PACKAGE_CODE;
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
                lblTotalFeePrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalFeePrice, 0);
                lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0);
                lblTotalVatPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalVatPrice, 0);
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
                GridView view = sender as GridView;
                if (e.RowHandle < 0)
                    return;
                var data = (VHisExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data == null)
                    return;
                if (data.IsMoba)
                {
                    if (data.MOBA_AMOUNT > data.CAN_MOBA_AMOUNT)
                    {
                        e.Valid = false;
                        view.SetColumnError(view.Columns["MOBA_AMOUNT"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_CONTROL__INVALID_ROW__MOBA_AMOUNT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                    }
                    else if (data.MOBA_AMOUNT <= 0)
                    {
                        e.Valid = false;
                        view.SetColumnError(view.Columns["MOBA_AMOUNT"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_CONTROL__INVALID_ROW__GREATE_THAN_ZERO", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                    }

                    if (!e.Valid)
                    {
                        gridViewExpMestMedicine.FocusedColumn = view.Columns["MOBA_AMOUNT"];
                    }
                }
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
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                    var data = (VHisExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                        {
                            e.RepositoryItem = repositoryItemSpinMateMobaAmountDisable;
                        }
                        else if (data.IsMoba)
                        {
                            e.RepositoryItem = repositoryItemSpinMateMobaAmount;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemSpinMateMobaAmountDisable;
                        }
                    }
                }
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
                //e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
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
                //    //if (!e.Valid)
                //    //{
                //    //    gridViewExpMestMaterial.FocusedColumn = view.Columns["MOBA_AMOUNT"];
                //    //}
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
                    this.LoadExpMestMaterial();
                    this.LoadExpMestMedicine();
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                    BtnPrintExNTr.Enabled = true;
                }
                WaitingManager.Hide();
                if (sucsess)
                {
                    MessageManager.Show(this, param, sucsess);
                }
                else
                {
                    MessageManager.Show(param, sucsess);
                }
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
                HisImpMestMobaSaleSDO data = new HisImpMestMobaSaleSDO();
                data.MobaMedicines = new List<HisMobaMedicineSDO>();
                data.MobaMaterials = new List<HisMobaMaterialSDO>();
                data.ExpMestId = this.expMestId;
                data.RequestRoomId = this.currentModule.RoomId;
                data.Description = txtDescription.Text.Trim();
                var listMedicine = listExpMestMedicineADO.Where(o => o.IsMoba).ToList();
                var listMaterial = listExpMestMaterialADO.Where(o => o.IsMoba).ToList();
                if ((listMaterial == null || listMaterial.Count == 0) && (listMedicine == null || listMedicine.Count == 0))
                {
                    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuaChonThuocVatTu);
                    return;
                }
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    foreach (var item in listMedicine)
                    {
                        if (item.MOBA_AMOUNT <= 0)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiPhaiLonHonKhong);
                            return;
                        }
                        if (item.MOBA_AMOUNT > item.CAN_MOBA_AMOUNT)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiKhongDuocLonHonSoLuongKhaDung);
                            return;
                        }
                        HisMobaMedicineSDO medi = new HisMobaMedicineSDO();
                        medi.MedicineId = item.MEDICINE_ID ?? 0;
                        medi.Amount = item.MOBA_AMOUNT;
                        data.MobaMedicines.Add(medi);
                    }
                }

                if (listMaterial != null && listMaterial.Count > 0)
                {
                    foreach (var item in listMaterial)
                    {
                        if (item.MOBA_AMOUNT <= 0)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiPhaiLonHonKhong);
                            return;
                        }
                        if (item.MOBA_AMOUNT > item.CAN_MOBA_AMOUNT)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiKhongDuocLonHonSoLuongKhaDung);
                            return;
                        }
                        HisMobaMaterialSDO medi = new HisMobaMaterialSDO();
                        medi.MaterialId = item.MATERIAL_ID ?? 0;
                        medi.Amount = item.MOBA_AMOUNT;
                        data.MobaMaterials.Add(medi);
                    }
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestResultSDO>(RequestUri.HIS_MOBA_SALE_CREATE, ApiConsumers.MosConsumer, data, param);
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultMobaSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000214, delegateRunPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private bool delegateRunPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultMobaSdo == null)
                    return result;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                if (this.resultMobaSdo.ImpMest.MOBA_EXP_MEST_ID != null)
                {
                    MOS.Filter.HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                    hisExpMestMedicineViewFilter.EXP_MEST_ID = this.resultMobaSdo.ImpMest.MOBA_EXP_MEST_ID;
                    expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, null);

                    MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialViewFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                    expMestMaterialViewFilter.EXP_MEST_ID = this.resultMobaSdo.ImpMest.MOBA_EXP_MEST_ID;
                    expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialViewFilter, null);

                    MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    expMestFilter.ID = this.resultMobaSdo.ImpMest.MOBA_EXP_MEST_ID;
                    expMest = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, null).FirstOrDefault();

                }
                WaitingManager.Hide();

                MPS.Processor.Mps000214.PDO.Mps000214PDO rdo = new MPS.Processor.Mps000214.PDO.Mps000214PDO(this.resultMobaSdo.ImpMest, this.resultMobaSdo.ImpMedicines, this.resultMobaSdo.ImpMaterials, expMestMedicines, expMestMaterials, expMest);
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest != null ? this.hisExpMest.TDL_TREATMENT_CODE : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO });
                if (Is_PrintHD)
                {
                    PrintHoaDon();
                }

                if (result)
                {
                    this.Close();
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
                //this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__CAPTION", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Button
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__BTN_SAVE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__BTN_PRINT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                this.layoutTotalFeePrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__LAYOUT_TOTAL_FEE_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTotalPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__LAYOUT_TOTAL_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTotalVatPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__LAYOUT_TOTAL_VAT_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //GridControl Material
                this.gridColumn_ExpMestMaterial_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_CONTROL__COLUMN_STT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_BidNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_BID_NUMBER", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_CanMobaAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_CAN_MOBA_AMOUNT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_EXPIRED_DATE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_IMP_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_MATERIAL_TYPE_CODE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_MATERIAL_TYPE_NAME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MobaAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_MOBA_AMOUNT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_REGISTER_NUMBER", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_VatRatio.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MATERIAL__COLUMN_VIR_TOTAL_IMP_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //grid Control Medicine
                this.gridColumn_ExpMestMedicine_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_BidNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_BID_NUMBER", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_CanMobaAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_CAN_MOBA_AMOUNT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_EXPIRED_DATE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_IMP_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_MEDICINE_TYPE_CODE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_MEDICINE_TYPE_NAME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MobaAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_MOBA_AMOUNT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_REGISTER_NUMBER", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_CONTROL__COLUMN_STT", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VatRatio.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MOBA_IMP_MEST_CREATE__GRID_MEDICINE__COLUMN_VIR_TOTAL_IMP_PRICE", Base.ResourceLangManager.LanguageFrmMobaImpMestCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnPrintExNTr_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnPrintExNTr.Enabled || this.resultMobaSdo == null)
                    return;

                Is_PrintHD = true;

                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000214, delegateRunPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintHoaDon()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000092", delegateRunPrintHoaDon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunPrintHoaDon(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultMobaSdo == null)
                    return result;

                WaitingManager.Show();
                CommonParam param = new CommonParam();

                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this.resultMobaSdo.ImpMest.MOBA_EXP_MEST_ID;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);
                if (expMests == null) return result;

                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.IDs = expMests.Select(s => s.BILL_ID ?? 0).ToList();
                List<V_HIS_TRANSACTION> transactionList = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);
                V_HIS_TRANSACTION transaction = new V_HIS_TRANSACTION();
                if (transactionList != null || transactionList.Count > 0)
                {
                    transaction = transactionList.FirstOrDefault();
                    //MessageBox.Show("Phiếu xuất chưa có hóa đơn thanh toán");
                    //return result;
                }

                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                HisImpMestFilter impMestFilter = new HisImpMestFilter();
                impMestFilter.MOBA_EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_IMP_MEST> impMests = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestFilter, param);


                MPS.Processor.Mps000092.PDO.Mps000092PDO rdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(expMests, expMestMedicines, expMestMaterials, transaction, impMests);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                MPS.ProcessorBase.Core.PrintData printdata = null;
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printdata);
            }
            catch (Exception ex)
            {
                result = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
